using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace Crolow.Cms.Server.Api.Attributes

{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class BearerAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] roles;
        public BearerAuthorizeAttribute(string[] roles)
        {
            this.roles = roles;
        }

        public BearerAuthorizeAttribute(string role)
        {
            roles = new[] { role };
        }

        public BearerAuthorizeAttribute()
        {
            roles = Array.Empty<string>();
        }



        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = (JwtSecurityToken)context.HttpContext.Items["User"];

            if (user == null)
            {
                // not logged in
                context.Result = new JsonResult(new { message = "Unauthorized Access !!!" }) { StatusCode = StatusCodes.Status401Unauthorized };
            }

            if (roles.Any())
            {
                if (!roles.Any(p => user.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == p)))
                {
                    // not logged in
                    context.Result = new JsonResult(new { message = "Unauthorized Access !!!" }) { StatusCode = StatusCodes.Status401Unauthorized };
                }
            }
        }
    }
}
