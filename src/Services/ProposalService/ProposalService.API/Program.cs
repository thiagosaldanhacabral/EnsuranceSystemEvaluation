using ProposalService.API.Endpoints;
using ProposalService.API.Middleware;
using ProposalService.Application;
using ProposalService.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

try
{
    Log.Information("Starting ProposalService API");

    // Add services to the container
    builder.Services.AddApplicationServices();
    builder.Services.AddInfrastructureServices(builder.Configuration);

    // Add response compression
    builder.Services.AddResponseCompression(options =>
    {
        options.EnableForHttps = true;
    });

    // Add OpenAPI/Swagger
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new()
        {
            Title = "ProposalService API",
            Version = "v1",
            Description = "API for managing insurance proposals",
            Contact = new()
            {
                Name = "Development Team",
                Email = "dev@ensurance.com"
            }
        });
    });

    // Add health checks
    builder.Services.AddHealthChecks()
        .AddSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection") ?? "",
            name: "database",
            timeout: TimeSpan.FromSeconds(3));

    // Add CORS if needed
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });

    var app = builder.Build();

    // Apply migrations automatically on startup
    Log.Information("Checking and applying database migrations...");
    try
    {
        await app.Services.ApplyMigrationsAsync();
    }
    catch (Exception ex)
    {
        Log.Fatal(ex, "Failed to apply database migrations");
        throw;
    }

    // Configure the HTTP request pipeline
    app.UseMiddleware<ExceptionHandlingMiddleware>();

    if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Docker")
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProposalService API v1");
            c.RoutePrefix = "swagger";
        });
    }

    app.UseCors();
    app.UseResponseCompression();
    app.UseSerilogRequestLogging();

    // Map endpoints
    app.MapProposalEndpoints();

    // Map health checks
    app.MapHealthChecks("/health");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
