using EMPLOYEE_MANAGEMENTAPII.Models;
using EMPLOYEE_MANAGEMENTAPII.Services;

var builder = WebApplication.CreateBuilder(args);

// Register application services
builder.Services.AddSingleton<IEmployeeService, EmployeeService>();
builder.Services.AddOpenApi();

// Enable CORS for easy local development & testing
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

// Enable OpenAPI in development
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors();
app.UseHttpsRedirection();

// Serve the interactive web UI from wwwroot
app.UseDefaultFiles();
app.UseStaticFiles();

// Define Employee REST API endpoints
var employeesApi = app.MapGroup("/api/employees").WithTags("Employees");

// GET all employees (supports ?search= and ?department=)
employeesApi.MapGet("/", (IEmployeeService service, string? search, string? department) =>
{
    var employees = service.GetAll(search, department);
    return Results.Ok(employees);
})
.WithName("GetEmployees");

// GET single employee by ID
employeesApi.MapGet("/{id:int}", (int id, IEmployeeService service) =>
{
    var employee = service.GetById(id);
    return employee is not null ? Results.Ok(employee) : Results.NotFound(new { message = $"Employee with ID {id} not found." });
})
.WithName("GetEmployeeById");

// POST create employee
employeesApi.MapPost("/", (EmployeeDto dto, IEmployeeService service) =>
{
    if (string.IsNullOrWhiteSpace(dto.FirstName) || string.IsNullOrWhiteSpace(dto.LastName))
    {
        return Results.BadRequest(new { message = "First name and last name are required." });
    }

    if (string.IsNullOrWhiteSpace(dto.Email) || !dto.Email.Contains('@'))
    {
        return Results.BadRequest(new { message = "A valid email is required." });
    }

    var created = service.Create(dto);
    return Results.Created($"/api/employees/{created.Id}", created);
})
.WithName("CreateEmployee");

// PUT update employee
employeesApi.MapPut("/{id:int}", (int id, EmployeeDto dto, IEmployeeService service) =>
{
    if (string.IsNullOrWhiteSpace(dto.FirstName) || string.IsNullOrWhiteSpace(dto.LastName))
    {
        return Results.BadRequest(new { message = "First name and last name are required." });
    }

    if (string.IsNullOrWhiteSpace(dto.Email) || !dto.Email.Contains('@'))
    {
        return Results.BadRequest(new { message = "A valid email is required." });
    }

    var success = service.Update(id, dto, out var updated);
    return success ? Results.Ok(updated) : Results.NotFound(new { message = $"Employee with ID {id} not found." });
})
.WithName("UpdateEmployee");

// DELETE employee
employeesApi.MapDelete("/{id:int}", (int id, IEmployeeService service) =>
{
    var success = service.Delete(id);
    return success ? Results.Ok(new { message = $"Employee with ID {id} deleted successfully." }) : Results.NotFound(new { message = $"Employee with ID {id} not found." });
})
.WithName("DeleteEmployee");

app.Run();
