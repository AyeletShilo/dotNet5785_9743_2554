namespace DO;

[Serializable]
public class DalDoesNotExistException : Exception
{
    public DalDoesNotExistException(string? message) : base(message) { }
}

[Serializable]
public class DalAlreadyExistException : Exception
{
    public DalAlreadyExistException(string? message) : base(message) { }
}

[Serializable]
public class DalXMLFileLoadCreateException: Exception
{    public DalXMLFileLoadCreateException(string? message) : base(message) { }
}
//[Serializable]
//public class DalDeletionImpossible : Exception
//{
//    public DalDeletionImpossible(string? message) : base(message) { }
//}



