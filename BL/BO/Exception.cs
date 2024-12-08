namespace BO;


[Serializable]
public class BlDoesNotExisException :Exception
{
    public BlDoesNotExisException(string? message) : base(message) { }
    public BlDoesNotExisException(string message, Exception innerException) 
        : base(message, innerException) { }
}
internal class Exception
{
}
