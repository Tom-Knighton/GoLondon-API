using GoLondonAPI.Domain.Entities;
using GoLondonAPI.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace GoLondonAPI.Middleware
{
    [AttributeUsage(validOn: AttributeTargets.Class | AttributeTargets.Method)]
    public class APIKeyAuth: Attribute, IAsyncActionFilter
    {
        private const string headerName = "ApiKey";

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(headerName, out var extractedApiKey))
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 401,
                    Content = "Please provide a valid Api Key (under ApiKey header)"
                };
                return;
            }

            IUserService userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();

            User user = await userService.GetUserFromAPIKey(extractedApiKey);

            if (user == null)
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 401,
                    Content = "Invalid Api Key provided"
                };
                return;
            }

            await next();
        }
    }
}

