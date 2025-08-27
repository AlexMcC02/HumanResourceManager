using FluentValidation;
using HumanResourceManager.Models;

namespace HumanResourceManager.Validators;

public class EmployeeValidator : AbstractValidator<Employee>
{
    public EmployeeValidator()
    {
        RuleFor(employee => employee.FirstName).NotNull().NotEmpty();
        RuleFor(employee => employee.SecondName).NotNull().NotEmpty();
        RuleFor(employee => employee.Band).IsInEnum();
        RuleFor(employee => employee.JobRole).NotNull().NotEmpty();
        RuleFor(employee => employee.Salary).InclusiveBetween(12500, 1000000);
    }
} 