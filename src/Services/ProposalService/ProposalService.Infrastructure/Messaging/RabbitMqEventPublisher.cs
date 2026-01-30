using MassTransit;
using ProposalService.Domain.Ports;

namespace ProposalService.Infrastructure.Messaging;

/// <summary>
/// RabbitMQ event publisher implementation using MassTransit
/// </summary>
public class RabbitMqEventPublisher : IProposalEventPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;

    public RabbitMqEventPublisher(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task PublishAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default)
        where TEvent : class
    {
        await _publishEndpoint.Publish(domainEvent, cancellationToken);
    }
}
