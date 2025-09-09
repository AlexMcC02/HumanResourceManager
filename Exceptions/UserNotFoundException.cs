namespace HumanResourceManager.Exceptions;

public class UserNotFoundException : Exception
{
    public UserNotFoundException()
    : base("An account with the provided username and password could not be found.") {}
}