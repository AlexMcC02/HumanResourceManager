using HumanResourceManager.DTO;
using HumanResourceManager.Exceptions;
using HumanResourceManager.Models;
using HumanResourceManager.Query;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HumanResourceManager.Controllers;

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
    public async Task<IActionResult> GetEmployees([FromQuery] EmployeeQueryParameters queryParams)
    {
        var employees = _context.Employees.AsQueryable();

        if (!string.IsNullOrWhiteSpace(queryParams.JobRole))
        {
            employees = employees.Where(e => e.JobRole.Contains(queryParams.JobRole));
        }

        if (!string.IsNullOrWhiteSpace(queryParams.Band))
        {
            if (Enum.TryParse<Band>(queryParams.Band, true, out var bandEnum))
            {
                employees = employees.Where(e => e.Band == bandEnum);
            }
            else
            {
                throw new InvalidBandValueException(queryParams.Band);
            }
        }

        if (queryParams.MinSalary.HasValue)
        {
            employees = employees.Where(e => e.Salary >= queryParams.MinSalary);
        }

        if (queryParams.MaxSalary.HasValue)
        {
            employees = employees.Where(e => e.Salary <= queryParams.MaxSalary);
        }

        if (employees.Any())
        {

            var skip = (queryParams.Page - 1) * queryParams.PageSize;
            var paginatedEmployees = await employees
                .Skip(skip)
                .Take(queryParams.PageSize)
                .ToListAsync();

            return Ok(paginatedEmployees);
        }
        else
        {
            throw new EmployeesNotFoundException();
        }
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