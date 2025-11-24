using Infrastructure;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>(optional: true, reloadOnChange: true);
}

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var cfg = builder.Configuration;
var connectionString = cfg.GetConnectionString("FinBank");
if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException("Connection string 'FinBank' is not configured.");


builder.Services.AddDbContext<FinBankDbContext>(options =>
{
    options.UseSqlServer(connectionString, sqlOpts =>
    {
        sqlOpts.CommandTimeout(cfg.GetValue<int?>("EF:CommandTimeoutSeconds") ?? 30);
        if (cfg.GetValue<bool>("EF:EnableRetryOnFailure"))
            sqlOpts.EnableRetryOnFailure(
                cfg.GetValue<int?>("EF:MaxRetryCount") ?? 5,
                TimeSpan.FromSeconds(cfg.GetValue<int?>("EF:MaxRetryDelaySeconds") ?? 30),
                null);
    });
    options.EnableDetailedErrors(cfg.GetValue<bool>("Ef:UseDetailedErrors"));
    options.EnableSensitiveDataLogging(cfg.GetValue<bool>("Ef:UseSensitiveDataLogging"));
});

// builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork>();
// builder.Services.AddScoped<IAccountRepository, AccountRepository>();
// builder.Services.AddScoped<ITransferRepository, TransferRepository>();

builder.Services.AddHealthChecks().AddSqlServer(connectionString, name: "db");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapHealthChecks("/health");
}


app.UseHttpsRedirection();
app.Run();