namespace BO;


[Serializable]
public class BlDoesNotExistException : Exception
{
    public BlDoesNotExistException(string? message) : base(message) { }
    public BlDoesNotExistException(string message, Exception innerException)
        : base(message, innerException) { }
}

[Serializable]
public class BlDoesAlreadyExistException : Exception
{
    public BlDoesAlreadyExistException(string? message) : base(message) { }
    public BlDoesAlreadyExistException(string message, Exception innerException)
        : base(message, innerException) { }
}


[Serializable]
public class BlXMLFileLoadCreateException : Exception
{
    public BlXMLFileLoadCreateException(string? message) : base(message) { }
    public BlXMLFileLoadCreateException(string message, Exception innerException)
        : base(message, innerException) { }
}


#region Bl Logic Exception

[Serializable]
public class BlIntegrityOfValuesException : Exception
{
    public BlIntegrityOfValuesException(string? message) : base(message) { }
}

[Serializable]
public class BlNullPropertyException : Exception
{
    public BlNullPropertyException(string? message) : base(message) { }
}

[Serializable]
public class BlCannotBeDeletedException : Exception
{
    public BlCannotBeDeletedException(string? message) : base(message) { }
}

[Serializable]
public class BlCantUpdateException : Exception
{
    public BlCantUpdateException(string? message) : base(message) { }
}


[Serializable]
public class BlCantHandleItException : Exception
{
    public BlCantHandleItException(string? message) : base(message) { }
}

[Serializable]
public class BlPasswordException : Exception
{
    public BlPasswordException(string? message) : base(message) { }
}

#endregion
