

namespace DO;

[Serializable]
public class DalDoesNotExistException : Exception
{
    public DalDoesNotExistException(string? message) : base(message) { }
}

public class DalAlreadyExistException : Exception
{
    public DalAlreadyExistException(string? message) : base(message) { }
}


public class DalDeletionImpossible : Exception
{
    public DalDeletionImpossible(string? message) : base(message) { }
}



