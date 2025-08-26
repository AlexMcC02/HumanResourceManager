namespace HumanResourceManager.Exceptions;

public class EmployeesNotFoundException : Exception
{
    public EmployeesNotFoundException() : base("No employees found.") {}
}