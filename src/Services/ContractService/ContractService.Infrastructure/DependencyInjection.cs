using ContractService.Domain.Ports;
using ContractService.Infrastructure.ExternalServices;
using ContractService.Infrastructure.Messaging;
using ContractService.Infrastructure.Persistence;
using ContractService.Infrastructure.Persistence.Repositories;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;

namespace ContractService.Infrastructure;

/// <summary>
/// Dependency injection configuration for Infrastructure layer
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Add DbContext
        services.AddDbContext<ContractDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorNumbersToAdd: null);
                }));

        // Add Repositories
        services.AddScoped<IContractRepository, ContractRepository>();

        // Add Event Publisher
        services.AddScoped<IContractEventPublisher, RabbitMqEventPublisher>();

        // Add ProposalService HTTP Client with Polly resilience policies
        services.AddHttpClient<IProposalServiceClient, ProposalServiceClient>(client =>
        {
            var baseUrl = configuration["ProposalService:BaseUrl"]
                ?? throw new InvalidOperationException("ProposalService:BaseUrl configuration is missing");
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(30);
        })
        .AddPolicyHandler(GetRetryPolicy())
        .AddPolicyHandler(GetCircuitBreakerPolicy());

        // Add MassTransit with RabbitMQ
        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                var rabbitMqConfig = configuration.GetSection("RabbitMQ");
                cfg.Host(rabbitMqConfig["Host"] ?? "localhost", "/", h =>
                {
                    h.Username(rabbitMqConfig["Username"] ?? "guest");
                    h.Password(rabbitMqConfig["Password"] ?? "guest");
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }

    /// <summary>
    /// Retry policy with exponential backoff
    /// </summary>
    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryAttempt, context) =>
                {
                    // Log retry attempt in production
                });
    }

    /// <summary>
    /// Circuit breaker policy to prevent cascading failures
    /// </summary>
    private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(30),
                onBreak: (outcome, duration) =>
                {
                    // Log circuit breaker opened in production
                },
                onReset: () =>
                {
                    // Log circuit breaker reset in production
                });
    }

    /// <summary>
    /// Extension method to apply database migrations
    /// </summary>
    public static async Task ApplyMigrationsAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ContractDbContext>();
        await context.Database.MigrateAsync();
    }
}
