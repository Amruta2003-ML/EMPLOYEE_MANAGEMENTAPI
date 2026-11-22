namespace EMPLOYEE_MANAGEMENTAPII.Models;

public class Employee
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public decimal Salary { get; set; }
    public DateOnly HireDate { get; set; }
}

public record EmployeeDto(
    string FirstName,
    string LastName,
    string Email,
    string Department,
    string Position,
    decimal Salary,
    DateOnly HireDate
);
