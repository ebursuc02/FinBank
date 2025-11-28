using System.IdentityModel.Tokens.Jwt;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApi.Authorization;

public static class OwnerOfUserPolicyExtensions
{
    public static AuthorizationBuilder AddOwnerOfUserPolicy(
        this AuthorizationBuilder builder,
        string policyName,
        string routeParameterName)
        => builder.AddPolicy(policyName, policy =>
            policy.RequireAssertion(context =>
            {
                var user = context.User;

                if (user.IsInRole(UserRole.Admin) || user.IsInRole(UserRole.Banker))
                    return true;

                var httpContext = context.Resource as HttpContext ??
                                  (context.Resource as AuthorizationFilterContext)?.HttpContext;

                var routeCustomerId = httpContext?.Request.RouteValues[routeParameterName]?.ToString();
                var claimCustomerId = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                return !string.IsNullOrEmpty(routeCustomerId) && routeCustomerId == claimCustomerId;
            }));
}