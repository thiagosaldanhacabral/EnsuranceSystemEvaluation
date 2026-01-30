using MediatR;
using ProposalService.Application.DTOs;
using ProposalService.Domain.Entities;
using ProposalService.Domain.Events;
using ProposalService.Domain.Ports;
using ProposalService.Domain.ValueObjects;
using SharedKernel.Domain;

namespace ProposalService.Application.Commands.CreateProposal;

/// <summary>
/// Handler for creating a new proposal
/// </summary>
public sealed class CreateProposalCommandHandler : IRequestHandler<CreateProposalCommand, ProposalDto>
{
    private readonly IProposalRepository _repository;
    private readonly IProposalEventPublisher _eventPublisher;

    public CreateProposalCommandHandler(
        IProposalRepository repository,
        IProposalEventPublisher eventPublisher)
    {
        _repository = repository;
        _eventPublisher = eventPublisher;
    }

    public async Task<ProposalDto> Handle(CreateProposalCommand request, CancellationToken cancellationToken)
    {
        // Generate proposal number
        var proposalNumber = GenerateProposalNumber();

        // Create value objects
        var cpf = CPF.Create(request.CustomerCPF);
        var insuranceValue = Money.Create(request.InsuranceValue);

        // Create proposal entity
        var proposal = Proposal.Create(
            proposalNumber,
            request.CustomerName,
            cpf,
            insuranceValue
        );

        // Save to repository
        await _repository.AddAsync(proposal, cancellationToken);

        // Publish domain event
        var createdEvent = new ProposalCreatedEvent
        {
            ProposalId = proposal.Id,
            ProposalNumber = proposal.ProposalNumber,
            CustomerName = proposal.CustomerName,
            CustomerCPF = proposal.CustomerCPF.Number,
            InsuranceValue = proposal.InsuranceValue.Amount
        };

        await _eventPublisher.PublishAsync(createdEvent, cancellationToken);

        // Map to DTO
        return MapToDto(proposal);
    }

    private static string GenerateProposalNumber()
    {
        // Generate a proposal number in format: PROP-YYYYMMDD-XXXXX
        var date = DateTime.UtcNow.ToString("yyyyMMdd");
        var random = new Random().Next(10000, 99999);
        return $"PROP-{date}-{random}";
    }

    private static ProposalDto MapToDto(Proposal proposal)
    {
        return new ProposalDto
        {
            Id = proposal.Id,
            ProposalNumber = proposal.ProposalNumber,
            CustomerName = proposal.CustomerName,
            CustomerCPF = proposal.CustomerCPF.Formatted,
            InsuranceValue = proposal.InsuranceValue.Amount,
            Status = proposal.Status.ToString(),
            RejectionReason = proposal.RejectionReason,
            CreatedAt = proposal.CreatedAt,
            UpdatedAt = proposal.UpdatedAt
        };
    }
}
