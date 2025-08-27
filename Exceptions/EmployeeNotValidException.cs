namespace HumanResourceManager.Exceptions;

public class EmployeeNotValidException : Exception
{
    public EmployeeNotValidException(string msg) : base(msg) {}
}