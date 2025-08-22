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
    public IActionResult GetEmployee(int id)
    {
        var employee = _context.Employees.Find(id);
        if (employee == null)
        {
            throw new EmployeeNotFoundException(id);
        }
        return Ok(employee);
    }
} 