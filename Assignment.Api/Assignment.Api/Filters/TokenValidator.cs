using Assignment.Infrastructures.EntityFrameworkCore;
using Assignment.Models.Common;
using Assignment.Utilities.Constants;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;

namespace Assignment.Api.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class TokenValidator : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            Action<HttpStatusCode> response = async (status) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.HttpContext.Response.ContentType = "application/json";
                await context.HttpContext.Response.WriteAsync(new ExceptionDetail { Code = $"{(int)status}", Message = status.ToString() }.ToString());
            };

            string? token = context.HttpContext.Request.Headers.FirstOrDefault(f => f.Key.ToLower().Equals(AppConstant.Authorization.ToLower())).Value.ToString().Split(" ").LastOrDefault()?.Trim();

            JwtSecurityToken securityToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            Dictionary<string, string> claims = securityToken.Claims.ToDictionary(x => x.Type, x => x.Value);

            claims.TryGetValue(ClaimsConstant.NameId, out string? tokenUserId);
            claims.TryGetValue(ClaimsConstant.TokenId, out string? tokenId);

            object? routeValue = context.HttpContext.Request.RouteValues.GetValueOrDefault(AppConstant.UserId);

            DataContext? dataContext = context.HttpContext.RequestServices.GetService<DataContext>();

            if (dataContext != null && !await dataContext.TokenManagers.AnyAsync(w => w.Id == new Guid(tokenId ?? string.Empty) && w.IsActive))
            {
                response(HttpStatusCode.Unauthorized);
                return;
            }

            if (routeValue is not null)
            {
                string userId = (string)routeValue;

                if (string.IsNullOrEmpty(tokenUserId) || !userId.ToLower().Equals(tokenUserId.ToLower()))
                {
                    response(HttpStatusCode.Forbidden);
                    return;
                }
            }

            _ = await next();
        }
    }
}
