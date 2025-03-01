namespace Talent;

public abstract class BaseError
{
    public string Message { get; set; }
    protected BaseError(string message)
    {
        Message = message;
    }   
}

public class RouteNotFoundError : BaseError 
{
    public RouteNotFoundError(string method, string route) : base($"Route '{route}' on method '{method}' not exists.") { }
}

public class ResourceNotFound : BaseError 
{
    public ResourceNotFound(int id) : base($"Id '{id}' not found") { }
}

public class ValidationError : BaseError 
{
    public ValidationError(string message) : base(message) { }
}

public class UnauthorizedError : BaseError 
{
    public UnauthorizedError(string message) : base(message) { }
}


public class InternalServerError : BaseError 
{
    public InternalServerError(string message) : base(message) { }
}

