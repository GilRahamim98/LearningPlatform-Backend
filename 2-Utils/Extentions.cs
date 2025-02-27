using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Talent;

public static class Extentions
{
    public static string GetAllErrors(this ModelStateDictionary modelState)
    {
        string errors = "";
        foreach (KeyValuePair<string, ModelStateEntry> item in modelState)
        {
            foreach (ModelError err in item.Value.Errors)
            {
                Console.WriteLine(err.ErrorMessage);
                errors += err.ErrorMessage + " ";
            }
        }
        return errors.Trim();
    }
}
