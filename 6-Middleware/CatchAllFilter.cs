using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Talent;

public class CatchAllFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        string message = AppConfig.IsProduction ? "Some error occur,please try again..." : GetInnerMessage(context.Exception);
        InternalServerError error = new InternalServerError(message);
        JsonResult result = new JsonResult(error);
        result.StatusCode = StatusCodes.Status500InternalServerError;
        context.Result = result;
        context.ExceptionHandled = true;
    }

    private string GetInnerMessage(Exception ex)
    {
        if (ex == null) return "";
        if (ex.InnerException == null) return ex.Message;
        return GetInnerMessage(ex.InnerException);
    }
}
