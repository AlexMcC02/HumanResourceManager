using System.Threading.Tasks;
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
    public async Task<IActionResult> PostEmployee(CreateEmployeeDto dto)
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
} 