using HumanResourceManager.DTO;
using HumanResourceManager.Exceptions;
using HumanResourceManager.Models;
using HumanResourceManager.Query;
using HumanResourceManager.Validators;
using Microsoft.AspNetCore.Authorization;
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

    [AllowAnonymous]
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

    [AllowAnonymous]
    [HttpGet("employees/{id}")]
    public async Task<IActionResult> GetEmployeeById(int id)
    {
        var employee = await _context.Employees.FindAsync(id) ?? throw new EmployeeNotFoundException(id);
        return Ok(employee);
    }

    [Authorize]
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

        var employeeValidator = new EmployeeValidator();
        var validationResult = employeeValidator.Validate(employee);

        if (!validationResult.IsValid)
        {
            var listOfErrors = "";

            foreach (var error in validationResult.Errors)
            {
                listOfErrors += $"Property {error.PropertyName} failed validation. Error was {error.ErrorMessage} ";
            }

            throw new EmployeeNotValidException(listOfErrors);
        }
        else
        {

            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEmployeeById), new { id = employee.Id }, employee);
        }
    }

    [Authorize]
    [HttpPut("modify_employee/{id}")]
    public async Task<IActionResult> PutEmployee(int id, EmployeeDto dto)
    {
        var employee = await _context.Employees.FindAsync(id) ?? throw new EmployeeNotFoundException(id);
        var employeeDtoValidator = new EmployeeDtoValidator();
        var validationResult = employeeDtoValidator.Validate(dto);

        if (!validationResult.IsValid)
        {
            var listOfErrors = "";

            foreach (var error in validationResult.Errors)
            {
                listOfErrors += $"Property {error.PropertyName} failed validation. Error was {error.ErrorMessage} ";
            }

            throw new EmployeeNotValidException(listOfErrors);
        }
        else
        {
            employee.FirstName = dto.FirstName;
            employee.SecondName = dto.SecondName;
            employee.JobRole = dto.JobRole;
            employee.Band = dto.Band;
            employee.Salary = dto.Salary;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    [Authorize]
    [HttpDelete("delete_employee/{id}")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        var employee = await _context.Employees.FindAsync(id) ?? throw new EmployeeNotFoundException(id);
        _context.Remove(employee);
        await _context.SaveChangesAsync();

        return NoContent();
    }
} 