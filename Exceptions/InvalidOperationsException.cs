namespace BankingApi.Exceptions;

public class InvalidOperationsException : Exception
{
    public InvalidOperationsException(string message) : base(message)
    {
    }
}
