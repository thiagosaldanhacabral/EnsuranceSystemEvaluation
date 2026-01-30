using ContractService.API.Endpoints;
using ContractService.API.Middleware;
using ContractService.Application;
using ContractService.Infrastructure;
using Serilog;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .Build())
    .CreateLogger();

try
{
    Log.Information("Starting ContractService API");

    var builder = WebApplication.CreateBuilder(args);

    // Add environment-specific configuration
    builder.Configuration.AddJsonFile(
        $"appsettings.{builder.Environment.EnvironmentName}.json",
        optional: true,
        reloadOnChange: true);

    // Use Serilog
    builder.Host.UseSerilog((context, config) =>
        config.ReadFrom.Configuration(context.Configuration));

    // Add services to the container
    builder.Services.AddApplicationServices();
    builder.Services.AddInfrastructureServices(builder.Configuration);

    // Add API services
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new()
        {
            Title = "Contract Service API",
            Version = "v1",
            Description = "Insurance contract management service - handles contract creation and queries"
        });
    });

    // Add health checks
    builder.Services.AddHealthChecks()
        .AddSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string not found"),
            name: "database",
            tags: ["db", "sql"]);

    // Add CORS
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });

    // Add response compression
    builder.Services.AddResponseCompression(options =>
    {
        options.EnableForHttps = true;
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

    app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Docker")
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Contract Service API v1");
            options.RoutePrefix = "swagger";
        });
    }

    app.UseCors();
    app.UseResponseCompression();

    // Map health check endpoints
    app.MapHealthChecks("/health");
    app.MapHealthChecks("/health/ready");

    // Map API endpoints
    app.MapContractEndpoints();

    Log.Information("ContractService API started successfully");
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "ContractService API failed to start");
    throw;
}
finally
{
    await Log.CloseAndFlushAsync();
}
