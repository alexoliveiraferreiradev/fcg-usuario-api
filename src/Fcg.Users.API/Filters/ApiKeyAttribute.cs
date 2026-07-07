using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Fcg.User.API.Filters
{
    public class ApiKeyAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var config = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var validApiKey = config["InternalSecrets:ServiceApiKey"];
            
            if (!context.HttpContext.Request.Headers.TryGetValue("x-internal-api-key", out var extractedApiKey))
            {
                context.Result = new UnauthorizedResult();
                return;
            }            
            if (!validApiKey.Equals(extractedApiKey))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
