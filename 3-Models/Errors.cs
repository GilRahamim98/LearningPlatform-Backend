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
    public ResourceNotFound(Guid id) : base($"Id '{id}' not found") { }
    public ResourceNotFound(string message) : base(message) { } 
}

public class ValidationError : BaseError
{
    public List<string> Errors { get; }

    public ValidationError(params string[] errors)
        : base( "One or more validation errors occurred.")
    {
        Errors = errors.ToList();
    }

    public ValidationError(IEnumerable<string> errors)
        : base("One or more validation errors occurred.")
    {
        Errors = errors.ToList();
    }
}

public class UnauthorizedError : BaseError 
{
    public UnauthorizedError(string message) : base(message) { }
}


public class InternalServerError : BaseError 
{
    public InternalServerError(string message) : base(message) { }
}

