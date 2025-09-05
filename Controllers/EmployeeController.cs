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
    private readonly ILogger<EmployeeController> _logger;

    public EmployeeController(ApplicationDbContext context, ILogger<EmployeeController> logger)
    {
        _context = context;
        _logger = logger;
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
                _logger.LogError("An invalid band was passed into the GetEmployees endpoint.");
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

            _logger.LogInformation("Fetched a paginated list of employees.");
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
        var employee = await _context.Employees.FindAsync(id);

        if (employee == null)
        {
            _logger.LogError($"The employee with ID of {id} could not be found.");
            throw new EmployeeNotFoundException(id);
        }

        _logger.LogInformation($"Employee {employee.FirstName} {employee.SecondName} was retrieved successfully.");
        return Ok(employee);
    }

    [Authorize]
    [HttpPost("create_employee")]
    [ServiceFilter(typeof(ValidJwtFilter))]
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

            _logger.LogError("The employee to be created did not pass validation.");
            throw new EmployeeNotValidException(listOfErrors);
        }
        else
        {

            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Employee {employee.FirstName} {employee.SecondName} created succesfully.");
            return CreatedAtAction(nameof(GetEmployeeById), new { id = employee.Id }, employee);
        }
    }

    [Authorize]
    [HttpPut("modify_employee/{id}")]
    [ServiceFilter(typeof(ValidJwtFilter))]
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

            _logger.LogError("The employee to be modified did not pass validation.");
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

            _logger.LogInformation($"Employee {employee.FirstName} {employee.SecondName} modified succesfully.");
            return NoContent();
        }
    }

    [Authorize]
    [HttpDelete("delete_employee/{id}")]
    [ServiceFilter(typeof(ValidJwtFilter))]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        var employee = await _context.Employees.FindAsync(id);

        if (employee == null)
        {
            _logger.LogError($"The employee with ID of {id} could not be deleted, as it does not exist.");
            throw new EmployeeNotFoundException(id);
        }

        _context.Remove(employee);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Employee with ID of {id} deleted successfully.");
        return NoContent();
    }
} 