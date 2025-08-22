using Microsoft.AspNetCore.Mvc;

public class EmployeeController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public EmployeeController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("api/employees")]
    public IActionResult GetEmployees()
    {
        var employees = _context.Employees.ToList();
        return Ok(employees);
    }
} 