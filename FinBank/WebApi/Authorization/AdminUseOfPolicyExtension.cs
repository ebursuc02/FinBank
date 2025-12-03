using System.Text.Json;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApi.Authorization;

public static class AdminUseOfPolicyExtension
{
    public static AuthorizationBuilder AddAdminPolicy(
        this AuthorizationBuilder builder,
        string policyName) =>
        builder.AddPolicy(policyName, policy =>
            policy.RequireAssertion(async context =>
            {
                var user = context.User;

                var httpContext =
                    context.Resource as HttpContext ??
                    (context.Resource as AuthorizationFilterContext)?.HttpContext;

                if (httpContext == null)
                    return false;
                
                httpContext.Request.EnableBuffering();

                string body;
                using (var reader = new StreamReader(httpContext.Request.Body, leaveOpen: true))
                {
                    body = await reader.ReadToEndAsync();
                }
                
                httpContext.Request.Body.Position = 0;
                
                if (string.IsNullOrWhiteSpace(body))
                    return false;

                using var json = JsonDocument.Parse(body);
                if (!json.RootElement.TryGetProperty("role", out var roleProp))
                    return false;

                var requestedRole = roleProp.GetString();
                
                //allow if the requested role is a Customer or the user is an Admin
                return string.Equals(requestedRole, UserRole.Customer, StringComparison.OrdinalIgnoreCase) ||
                       user.IsInRole(UserRole.Admin);
            }));
}