using DTO;
using Errors;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("/human_resource_manager/api/")]
public class EmployeeController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public EmployeeController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("employees")]
    public IActionResult GetEmployees()
    {
        var employees = _context.Employees.ToList();
        return Ok(employees);
    }

    [HttpGet("employees/{id}")]
    public IActionResult GetEmployeeById(int id)
    {
        var employee = _context.Employees.Find(id);
        if (employee == null)
        {
            throw new EmployeeNotFoundException(id);
        }
        return Ok(employee);
    }

    [HttpPost("create_employee")]
    public async Task<IActionResult> PostEmployee(EmployeeDto dto)
    {
        var employee = new Employee
        {
            FirstName = dto.FirstName,
            SecondName = dto.SecondName,
            JobRole = dto.JobRole,
            Band = dto.Band,
            Salary = dto.Salary
        };

        await _context.Employees.AddAsync(employee);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetEmployeeById), new { id = employee.Id }, employee);
    }

    [HttpPut("modify_employee/{id}")]
    public async Task<IActionResult> PutEmployee(int id, EmployeeDto dto)
    {
        var employee = await _context.Employees.FindAsync(id);

        if (employee == null)
        {
            throw new EmployeeNotFoundException(id);
        }

        employee.FirstName = dto.FirstName;
        employee.SecondName = dto.SecondName;
        employee.JobRole = dto.JobRole;
        employee.Band = dto.Band;
        employee.Salary = dto.Salary;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("delete_employee/{id}")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        var employee = await _context.Employees.FindAsync(id);

        if (employee == null)
        {
            throw new EmployeeNotFoundException(id);
        }

        _context.Remove(employee);
        await _context.SaveChangesAsync();

        return NoContent();
    }
} 