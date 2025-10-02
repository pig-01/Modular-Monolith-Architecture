namespace Base.Domain.SeedWorks;

public class BaseException : Exception
{
    public List<object> Param = [];
    public BaseException() : base() { }
    public BaseException(string message) : base(message) { }
    public BaseException(string message, Exception innerException) : base(message, innerException) { }
}
