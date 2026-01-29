using ContractService.Application.DTOs;
using ContractService.Domain.Entities;
using ContractService.Domain.Events;
using ContractService.Domain.Exceptions;
using ContractService.Domain.Ports;
using ContractService.Domain.ValueObjects;
using MediatR;
using SharedKernel.Domain;

namespace ContractService.Application.Commands.ContractProposal;

/// <summary>
/// Handler for contracting an approved proposal
/// </summary>
public sealed class ContractProposalCommandHandler : IRequestHandler<ContractProposalCommand, ContractDto>
{
    private readonly IContractRepository _repository;
    private readonly IProposalServiceClient _proposalClient;
    private readonly IContractEventPublisher _eventPublisher;

    public ContractProposalCommandHandler(
        IContractRepository repository,
        IProposalServiceClient proposalClient,
        IContractEventPublisher eventPublisher)
    {
        _repository = repository;
        _proposalClient = proposalClient;
        _eventPublisher = eventPublisher;
    }

    public async Task<ContractDto> Handle(ContractProposalCommand request, CancellationToken cancellationToken)
    {
        // Check if proposal is already contracted
        var existingContract = await _repository.GetByProposalIdAsync(request.ProposalId, cancellationToken);
        if (existingContract != null)
            throw new ContractDomainException($"Proposal {request.ProposalId} is already contracted");

        // Get proposal from ProposalService
        var proposal = await _proposalClient.GetProposalByIdAsync(request.ProposalId, cancellationToken);
        if (proposal == null)
            throw new ContractDomainException($"Proposal {request.ProposalId} not found");

        // Validate proposal status
        if (!proposal.Status.Equals("Approved", StringComparison.OrdinalIgnoreCase))
            throw new ContractDomainException($"Proposal {request.ProposalId} is not approved. Current status: {proposal.Status}");

        // Generate contract number
        var contractNumber = GenerateContractNumber();

        // Create customer info
        var customer = CustomerInfo.Create(proposal.CustomerName, proposal.CustomerCPF);

        // Create premium (insurance value as premium)
        var premium = Money.Create(proposal.InsuranceValue);

        // Calculate dates
        var startDate = DateTime.UtcNow.Date.AddDays(1); // Start tomorrow
        var endDate = startDate.AddMonths(request.DurationMonths);

        // Create contract entity
        var contract = Contract.Create(
            request.ProposalId,
            contractNumber,
            customer,
            premium,
            startDate,
            endDate
        );

        // Save to repository
        await _repository.AddAsync(contract, cancellationToken);

        // Publish domain event
        var createdEvent = new ContractCreatedEvent
        {
            ContractId = contract.Id,
            ProposalId = contract.ProposalId,
            ContractNumber = contract.ContractNumber,
            CustomerName = contract.Customer.Name,
            Premium = contract.Premium.Amount,
            StartDate = contract.StartDate,
            EndDate = contract.EndDate
        };

        await _eventPublisher.PublishAsync(createdEvent, cancellationToken);

        // Map to DTO
        return MapToDto(contract);
    }

    private static string GenerateContractNumber()
    {
        // Generate a contract number in format: CONT-YYYYMMDD-XXXXX
        var date = DateTime.UtcNow.ToString("yyyyMMdd");
        var random = new Random().Next(10000, 99999);
        return $"CONT-{date}-{random}";
    }

    private static ContractDto MapToDto(Contract contract)
    {
        return new ContractDto
        {
            Id = contract.Id,
            ProposalId = contract.ProposalId,
            ContractNumber = contract.ContractNumber,
            CustomerName = contract.Customer.Name,
            CustomerCPF = contract.Customer.CPF,
            Premium = contract.Premium.Amount,
            ContractDate = contract.ContractDate,
            StartDate = contract.StartDate,
            EndDate = contract.EndDate
        };
    }
}
