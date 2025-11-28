using System.Security.Claims;
using System.Text;
using Infrastructure;
using Application;
using Application.Interfaces.Security;
using Application.Services.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using WebApi.Mappers;
using Microsoft.OpenApi.Models;
using WebApi.Authorization;


var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>(optional: true, reloadOnChange: true);
}

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddAutoMapper(_ => { }, typeof(WebApiMappingProfile).Assembly);
builder.Services.AddControllers();

builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? string.Empty);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = true;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateLifetime = true,
            RoleClaimType = ClaimTypes.Role
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddOwnerOfUserPolicy(AuthorizationPolicies.OwnerOfUserPolicy, "customerId");

builder.Services.AddAuthorization();
builder.Services.AddHttpClient();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapHealthChecks("/health");
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();