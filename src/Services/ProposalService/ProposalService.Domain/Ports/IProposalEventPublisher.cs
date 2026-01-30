using ProposalService.Domain.Events;

namespace ProposalService.Domain.Ports;

/// <summary>
/// Interface for publishing proposal domain events
/// </summary>
public interface IProposalEventPublisher
{
    Task PublishAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default)
        where TEvent : class;
}
