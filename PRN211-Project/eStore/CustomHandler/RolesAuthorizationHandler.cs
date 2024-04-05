using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace eStore.CustomHandler
{
    public class RolesAuthorizationHandler : AuthorizationHandler<RolesAuthorizationRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RolesAuthorizationRequirement requirement)
        {
            if (context.User == null || !context.User.Identity.IsAuthenticated)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            bool validRole = false;
            if (requirement.AllowedRoles == null || !requirement.AllowedRoles.Any())
            {
                validRole = false;
            } else
            {
                var claims = context.User.Claims;
                var name = claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Name));

                if (name != null)
                {
                    var roles = requirement.AllowedRoles;
                    if (name.Value.Equals("Admin") && roles.Contains(name.Value))
                    {
                        validRole = true;
                    } else if (!name.Value.Equals("Admin") && roles.Contains("User"))
                    {
                        validRole = true;
                    } else 
                    {
                        validRole = false;
                    }
                }
            }

            if (validRole)
            {
                context.Succeed(requirement);
            } else
            {
                context.Fail();
            }
            return Task.CompletedTask;
        }
    }
}
