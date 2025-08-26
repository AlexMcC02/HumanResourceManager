namespace HumanResourceManager.Exceptions;

public class InvalidBandValueException : Exception
{
    public InvalidBandValueException(string band)
    : base($"{band} is not a valid band. Note: this parameter is case sensitive!") {}
}