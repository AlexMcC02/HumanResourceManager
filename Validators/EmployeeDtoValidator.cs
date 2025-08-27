using FluentValidation;
using HumanResourceManager.DTO;

namespace HumanResourceManager.Validators;

public class EmployeeDtoValidator : AbstractValidator<EmployeeDto>
{
    public EmployeeDtoValidator()
    {
        RuleFor(employee => employee.FirstName).NotNull().NotEmpty();
        RuleFor(employee => employee.SecondName).NotNull().NotEmpty();
        RuleFor(employee => employee.Band).IsInEnum();
        RuleFor(employee => employee.JobRole).NotNull().NotEmpty();
        RuleFor(employee => employee.Salary).InclusiveBetween(12500, 1000000);
    }
}