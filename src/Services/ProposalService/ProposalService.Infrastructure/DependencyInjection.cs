using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProposalService.Domain.Ports;
using ProposalService.Infrastructure.Messaging;
using ProposalService.Infrastructure.Persistence;
using ProposalService.Infrastructure.Persistence.Repositories;

namespace ProposalService.Infrastructure;

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
        services.AddDbContext<ProposalDbContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorNumbersToAdd: null);
                });
        });

        // Add Repositories
        services.AddScoped<IProposalRepository, ProposalRepository>();

        // Add RabbitMQ with MassTransit
        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                var rabbitMqHost = configuration["RabbitMQ:Host"] ?? "localhost";
                var rabbitMqUser = configuration["RabbitMQ:Username"] ?? "guest";
                var rabbitMqPass = configuration["RabbitMQ:Password"] ?? "guest";

                cfg.Host(rabbitMqHost, h =>
                {
                    h.Username(rabbitMqUser);
                    h.Password(rabbitMqPass);
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        // Add Event Publisher
        services.AddScoped<IProposalEventPublisher, RabbitMqEventPublisher>();

        return services;
    }

    /// <summary>
    /// Apply database migrations automatically with retry logic
    /// </summary>
    public static async Task ApplyMigrationsAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ProposalDbContext>();

        const int maxRetries = 10;
        var delay = TimeSpan.FromSeconds(3);

        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                // Apply migrations (this will create the database if it doesn't exist)
                Console.WriteLine("[ProposalService] Applying database migrations...");
                await context.Database.MigrateAsync();
                Console.WriteLine("[ProposalService] Migrations applied successfully");

                return;
            }
            catch (Exception ex)
            {
                if (i == maxRetries - 1)
                {
                    Console.WriteLine($"[ProposalService] Failed to apply migrations after {maxRetries} attempts: {ex.Message}");
                    throw;
                }

                Console.WriteLine($"[ProposalService] Migration attempt {i + 1}/{maxRetries} failed: {ex.Message}");
                Console.WriteLine($"[ProposalService] Retrying in {delay.TotalSeconds} seconds...");
                await Task.Delay(delay);
            }
        }
    }
}
