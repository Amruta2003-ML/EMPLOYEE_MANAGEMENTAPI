using System.Collections.Concurrent;
using EMPLOYEE_MANAGEMENTAPII.Models;

namespace EMPLOYEE_MANAGEMENTAPII.Services;

public interface IEmployeeService
{
    IEnumerable<Employee> GetAll(string? search = null, string? department = null);
    Employee? GetById(int id);
    Employee Create(EmployeeDto dto);
    bool Update(int id, EmployeeDto dto, out Employee? updated);
    bool Delete(int id);
}

public class EmployeeService : IEmployeeService
{
    private readonly ConcurrentDictionary<int, Employee> _employees = new();
    private int _nextId = 1;

    public EmployeeService()
    {
        // Seed initial employees
        var seedData = new[]
        {
            new EmployeeDto("Alice", "Johnson", "alice.johnson@example.com", "Engineering", "Senior Software Engineer", 95000, new DateOnly(2022, 3, 15)),
            new EmployeeDto("Bob", "Smith", "bob.smith@example.com", "Human Resources", "HR Specialist", 62000, new DateOnly(2021, 6, 1)),
            new EmployeeDto("Carlos", "Mendoza", "carlos.mendoza@example.com", "Finance", "Financial Analyst", 78000, new DateOnly(2023, 1, 10)),
            new EmployeeDto("Diana", "Prince", "diana.prince@example.com", "Marketing", "Marketing Director", 110000, new DateOnly(2020, 9, 20)),
            new EmployeeDto("Evan", "Davis", "evan.davis@example.com", "Engineering", "QA Engineer", 72000, new DateOnly(2023, 8, 5))
        };

        foreach (var dto in seedData)
        {
            Create(dto);
        }
    }

    public IEnumerable<Employee> GetAll(string? search = null, string? department = null)
    {
        var query = _employees.Values.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(department))
        {
            query = query.Where(e => e.Department.Equals(department, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            query = query.Where(e =>
                e.FirstName.Contains(s, StringComparison.OrdinalIgnoreCase) ||
                e.LastName.Contains(s, StringComparison.OrdinalIgnoreCase) ||
                e.Email.Contains(s, StringComparison.OrdinalIgnoreCase) ||
                e.Position.Contains(s, StringComparison.OrdinalIgnoreCase));
        }

        return query.OrderBy(e => e.Id);
    }

    public Employee? GetById(int id)
    {
        _employees.TryGetValue(id, out var emp);
        return emp;
    }

    public Employee Create(EmployeeDto dto)
    {
        var id = Interlocked.Increment(ref _nextId) - 1;
        var employee = new Employee
        {
            Id = id,
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            Email = dto.Email.Trim(),
            Department = dto.Department.Trim(),
            Position = dto.Position.Trim(),
            Salary = dto.Salary,
            HireDate = dto.HireDate
        };

        _employees[id] = employee;
        return employee;
    }

    public bool Update(int id, EmployeeDto dto, out Employee? updated)
    {
        if (!_employees.ContainsKey(id))
        {
            updated = null;
            return false;
        }

        var employee = new Employee
        {
            Id = id,
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            Email = dto.Email.Trim(),
            Department = dto.Department.Trim(),
            Position = dto.Position.Trim(),
            Salary = dto.Salary,
            HireDate = dto.HireDate
        };

        _employees[id] = employee;
        updated = employee;
        return true;
    }

    public bool Delete(int id)
    {
        return _employees.TryRemove(id, out _);
    }
}
