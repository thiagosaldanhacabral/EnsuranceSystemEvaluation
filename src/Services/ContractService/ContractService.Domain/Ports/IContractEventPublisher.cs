namespace ContractService.Domain.Ports;

/// <summary>
/// Interface for publishing contract domain events
/// </summary>
public interface IContractEventPublisher
{
    Task PublishAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default)
        where TEvent : class;
}
