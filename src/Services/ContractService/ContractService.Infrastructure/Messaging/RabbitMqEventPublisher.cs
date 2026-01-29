using ContractService.Domain.Events;
using ContractService.Domain.Ports;
using MassTransit;

namespace ContractService.Infrastructure.Messaging;

/// <summary>
/// RabbitMQ implementation of event publisher using MassTransit
/// </summary>
public class RabbitMqEventPublisher : IContractEventPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;

    public RabbitMqEventPublisher(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class
    {
        await _publishEndpoint.Publish(@event, cancellationToken);
    }
}
