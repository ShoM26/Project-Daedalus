using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ProjectDaedalus.API.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class InternalApiAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var expectedKey = configuration["InternalApi:BridgeApiKey"];
        
            if (string.IsNullOrEmpty(expectedKey))
            {
                context.Result = new StatusCodeResult(500); // Config error
                return;
            }

            if (!context.HttpContext.Request.Headers.TryGetValue("X-Internal-API-Key", out var providedKey))
            {
                context.Result = new UnauthorizedObjectResult(new { Error = "Internal API key required" });
                return;
            }

            if (providedKey != expectedKey)
            {
                context.Result = new UnauthorizedObjectResult(new { Error = "Invalid internal API key" });
            }
        }
    }
}
