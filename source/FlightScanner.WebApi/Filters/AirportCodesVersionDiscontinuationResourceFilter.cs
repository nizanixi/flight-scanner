using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FlightScanner.WebApi.Filters;

public class AirportCodesVersionDiscontinuationResourceFilter : Attribute, IResourceFilter
{
    private const string OUTDATED_API_VERSION = "v1";

    public void OnResourceExecuted(ResourceExecutedContext context)
    {

    }

    public void OnResourceExecuting(ResourceExecutingContext context)
    {
        var uriPath = context.HttpContext.Request.Path;

        if (!string.IsNullOrEmpty(uriPath.Value) && uriPath.Value.Contains(OUTDATED_API_VERSION, StringComparison.CurrentCultureIgnoreCase))
        {
            var errorObject = new
            {
                Versioning = new[]
                {
                    $"API version {OUTDATED_API_VERSION} is expired. Please use the latest version."
                }
            };

            context.Result = new BadRequestObjectResult(errorObject);
        }
    }
}
