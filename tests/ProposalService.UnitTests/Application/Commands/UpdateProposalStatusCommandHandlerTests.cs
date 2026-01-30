using FluentAssertions;
using NSubstitute;
using ProposalService.Application.Commands.UpdateProposalStatus;
using ProposalService.Domain.Entities;
using ProposalService.Domain.Events;
using ProposalService.Domain.Ports;
using ProposalService.Domain.ValueObjects;
using SharedKernel.Domain;
using SharedKernel.Exceptions;

namespace ProposalService.UnitTests.Application.Commands;

public class UpdateProposalStatusCommandHandlerTests
{
    private readonly IProposalRepository _repository;
    private readonly IProposalEventPublisher _eventPublisher;
    private readonly UpdateProposalStatusCommandHandler _handler;

    public UpdateProposalStatusCommandHandlerTests()
    {
        _repository = Substitute.For<IProposalRepository>();
        _eventPublisher = Substitute.For<IProposalEventPublisher>();
        _handler = new UpdateProposalStatusCommandHandler(_repository, _eventPublisher);
    }

    [Fact]
    public async Task Handle_ApproveProposal_ShouldUpdateStatusAndPublishEvent()
    {
        // Arrange
        var proposal = CreateProposal();
        _repository.GetByIdAsync(proposal.Id, Arg.Any<CancellationToken>())
            .Returns(proposal);

        var command = new UpdateProposalStatusCommand
        {
            ProposalId = proposal.Id,
            NewStatus = "Approved"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be("Approved");
        result.UpdatedAt.Should().NotBeNull();

        await _repository.Received(1).UpdateAsync(
            Arg.Is<Proposal>(p => p.Status == ProposalStatus.Approved),
            Arg.Any<CancellationToken>());

        await _eventPublisher.Received(1).PublishAsync(
            Arg.Is<ProposalApprovedEvent>(e => e.ProposalId == proposal.Id),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_RejectProposal_ShouldUpdateStatusAndPublishEvent()
    {
        // Arrange
        var proposal = CreateProposal();
        _repository.GetByIdAsync(proposal.Id, Arg.Any<CancellationToken>())
            .Returns(proposal);

        var command = new UpdateProposalStatusCommand
        {
            ProposalId = proposal.Id,
            NewStatus = "Rejected",
            RejectionReason = "Insufficient documentation"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.Should().Be("Rejected");
        result.RejectionReason.Should().Be("Insufficient documentation");

        await _repository.Received(1).UpdateAsync(
            Arg.Is<Proposal>(p => p.Status == ProposalStatus.Rejected),
            Arg.Any<CancellationToken>());

        await _eventPublisher.Received(1).PublishAsync(
            Arg.Is<ProposalRejectedEvent>(e =>
                e.ProposalId == proposal.Id &&
                e.RejectionReason == "Insufficient documentation"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ProposalNotFound_ShouldThrowDomainException()
    {
        // Arrange
        _repository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Proposal?)null);

        var command = new UpdateProposalStatusCommand
        {
            ProposalId = Guid.NewGuid(),
            NewStatus = "Approved"
        };

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("Proposal * not found");
    }

    [Fact]
    public async Task Handle_InvalidStatus_ShouldThrowDomainException()
    {
        // Arrange
        var proposal = CreateProposal();
        _repository.GetByIdAsync(proposal.Id, Arg.Any<CancellationToken>())
            .Returns(proposal);

        var command = new UpdateProposalStatusCommand
        {
            ProposalId = proposal.Id,
            NewStatus = "InvalidStatus"
        };

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("Invalid status: InvalidStatus. Must be 'Approved' or 'Rejected'");
    }

    [Fact]
    public async Task Handle_RejectWithoutReason_ShouldUseDefaultReason()
    {
        // Arrange
        var proposal = CreateProposal();
        _repository.GetByIdAsync(proposal.Id, Arg.Any<CancellationToken>())
            .Returns(proposal);

        var command = new UpdateProposalStatusCommand
        {
            ProposalId = proposal.Id,
            NewStatus = "Rejected",
            RejectionReason = null
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.Should().Be("Rejected");
        result.RejectionReason.Should().Be("No reason provided");
    }

    private static Proposal CreateProposal()
    {
        var cpf = CPF.Create("11144477735");
        var insuranceValue = Money.Create(50000m);
        return Proposal.Create("PROP-20260129-00001", "John Doe", cpf, insuranceValue);
    }
}
