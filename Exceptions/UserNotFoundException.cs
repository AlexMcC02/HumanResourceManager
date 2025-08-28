namespace HumanResourceManager.Exceptions;

public class UserNotFoundException : Exception
{
    public UserNotFoundException(string username, string password)
    : base($"No account found with username {username} and/or password {password}.") {}
}