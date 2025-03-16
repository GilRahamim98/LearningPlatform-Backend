using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Talent;

public class CatchAllFilter : IExceptionFilter
{
    // This method is called when an exception occurs in the application
    public void OnException(ExceptionContext context)
    {
        // Determine the error message based on the environment
        string message = AppConfig.IsProduction ? "Some error occur,please try again..." : GetInnerMessage(context.Exception);

        // Create an InternalServerError object with the error message
        InternalServerError error = new InternalServerError(message);

        // Create a JsonResult with the error object and set the status code to 500
        JsonResult result = new JsonResult(error);
        result.StatusCode = StatusCodes.Status500InternalServerError;

        // Set the result and mark the exception as handled
        context.Result = result;
        context.ExceptionHandled = true;
    }

    // Recursively retrieves the innermost exception message
    private string GetInnerMessage(Exception ex)
    {
        if (ex == null) return "";
        if (ex.InnerException == null) return ex.Message;
        return GetInnerMessage(ex.InnerException);
    }
}
