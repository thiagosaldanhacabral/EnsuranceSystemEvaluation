using FluentAssertions;
using NSubstitute;
using ContractService.Application.Commands.ContractProposal;
using ContractService.Domain.Entities;
using ContractService.Domain.Events;
using ContractService.Domain.Ports;
using ContractService.Domain.ValueObjects;
using SharedKernel.Domain;
using SharedKernel.Exceptions;

namespace ContractService.UnitTests.Application.Commands;

public class ContractProposalCommandHandlerTests
{
    private readonly IContractRepository _contractRepository;
    private readonly IProposalServiceClient _proposalClient;
    private readonly IContractEventPublisher _eventPublisher;
    private readonly ContractProposalCommandHandler _handler;

    public ContractProposalCommandHandlerTests()
    {
        _contractRepository = Substitute.For<IContractRepository>();
        _proposalClient = Substitute.For<IProposalServiceClient>();
        _eventPublisher = Substitute.For<IContractEventPublisher>();
        _handler = new ContractProposalCommandHandler(
            _contractRepository,
            _proposalClient,
            _eventPublisher);
    }

    [Fact]
    public async Task Handle_WithApprovedProposal_ShouldCreateContract()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var proposal = new ProposalDto
        {
            Id = proposalId,
            ProposalNumber = "PROP-20260129-00001",
            CustomerName = "John Doe",
            CustomerCPF = "11144477735",
            InsuranceValue = 50000m,
            Status = "Approved"
        };

        _proposalClient.GetProposalByIdAsync(proposalId, Arg.Any<CancellationToken>())
            .Returns(proposal);

        _contractRepository.GetByProposalIdAsync(proposalId, Arg.Any<CancellationToken>())
            .Returns((Contract?)null);

        var command = new ContractProposalCommand
        {
            ProposalId = proposalId,
            DurationMonths = 12
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.ProposalId.Should().Be(proposalId);
        result.CustomerName.Should().Be("John Doe");
        result.CustomerCPF.Should().Be("11144477735");
        result.Premium.Should().BeGreaterThan(0);
        result.ContractNumber.Should().StartWith("CONT-");

        await _contractRepository.Received(1).AddAsync(
            Arg.Any<Contract>(),
            Arg.Any<CancellationToken>());

        await _eventPublisher.Received(1).PublishAsync(
            Arg.Is<ContractCreatedEvent>(e => e.ContractId == result.Id),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ProposalNotFound_ShouldThrowDomainException()
    {
        // Arrange
        var proposalId = Guid.NewGuid();

        _proposalClient.GetProposalByIdAsync(proposalId, Arg.Any<CancellationToken>())
            .Returns((ProposalDto?)null);

        var command = new ContractProposalCommand
        {
            ProposalId = proposalId,
            DurationMonths = 12
        };

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage($"Proposal {proposalId} not found");
    }

    [Fact]
    public async Task Handle_ProposalNotApproved_ShouldThrowDomainException()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var proposal = new ProposalDto
        {
            Id = proposalId,
            ProposalNumber = "PROP-20260129-00001",
            CustomerName = "John Doe",
            CustomerCPF = "11144477735",
            InsuranceValue = 50000m,
            Status = "InAnalysis"
        };

        _proposalClient.GetProposalByIdAsync(proposalId, Arg.Any<CancellationToken>())
            .Returns(proposal);

        var command = new ContractProposalCommand
        {
            ProposalId = proposalId,
            DurationMonths = 12
        };

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage($"Proposal {proposalId} is not approved*");
    }

    [Fact]
    public async Task Handle_ProposalAlreadyContracted_ShouldThrowDomainException()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var proposal = new ProposalDto
        {
            Id = proposalId,
            ProposalNumber = "PROP-20260129-00001",
            CustomerName = "John Doe",
            CustomerCPF = "11144477735",
            InsuranceValue = 50000m,
            Status = "Approved"
        };

        // Create a real contract instead of mocking
        var customer = CustomerInfo.Create("John Doe", "11144477735");
        var premium = Money.Create(50000m);
        var existingContract = Contract.Create(
            proposalId,
            "CONT-20260129-12345",
            customer,
            premium,
            DateTime.UtcNow.Date.AddDays(1),
            DateTime.UtcNow.Date.AddMonths(12).AddDays(1));

        _proposalClient.GetProposalByIdAsync(proposalId, Arg.Any<CancellationToken>())
            .Returns(proposal);

        _contractRepository.GetByProposalIdAsync(proposalId, Arg.Any<CancellationToken>())
            .Returns(existingContract);

        var command = new ContractProposalCommand
        {
            ProposalId = proposalId,
            DurationMonths = 12
        };

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage($"Proposal {proposalId} is already contracted");
    }

    [Fact]
    public async Task Handle_ShouldCalculatePremiumBasedOnInsuranceValue()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var insuranceValue = 50000m;
        var proposal = new ProposalDto
        {
            Id = proposalId,
            ProposalNumber = "PROP-20260129-00001",
            CustomerName = "John Doe",
            CustomerCPF = "11144477735",
            InsuranceValue = insuranceValue,
            Status = "Approved"
        };

        _proposalClient.GetProposalByIdAsync(proposalId, Arg.Any<CancellationToken>())
            .Returns(proposal);

        _contractRepository.GetByProposalIdAsync(proposalId, Arg.Any<CancellationToken>())
            .Returns((Contract?)null);

        var command = new ContractProposalCommand
        {
            ProposalId = proposalId,
            DurationMonths = 12
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        // Premium is the full insurance value
        result.Premium.Should().Be(insuranceValue);
    }

    [Fact]
    public async Task Handle_ShouldSetCorrectStartAndEndDates()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var proposal = new ProposalDto
        {
            Id = proposalId,
            ProposalNumber = "PROP-20260129-00001",
            CustomerName = "John Doe",
            CustomerCPF = "11144477735",
            InsuranceValue = 50000m,
            Status = "Approved"
        };

        _proposalClient.GetProposalByIdAsync(proposalId, Arg.Any<CancellationToken>())
            .Returns(proposal);

        _contractRepository.GetByProposalIdAsync(proposalId, Arg.Any<CancellationToken>())
            .Returns((Contract?)null);

        var command = new ContractProposalCommand
        {
            ProposalId = proposalId,
            DurationMonths = 12
        };

        var beforeExecution = DateTime.UtcNow.Date;

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.StartDate.Should().BeOnOrAfter(beforeExecution.AddDays(1));
        result.EndDate.Should().BeCloseTo(result.StartDate.AddMonths(12), TimeSpan.FromDays(1));
    }
}
