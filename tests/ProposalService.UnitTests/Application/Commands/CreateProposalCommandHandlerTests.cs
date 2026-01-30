using FluentAssertions;
using NSubstitute;
using ProposalService.Application.Commands.CreateProposal;
using ProposalService.Domain.Entities;
using ProposalService.Domain.Events;
using ProposalService.Domain.Ports;
using ProposalService.Domain.ValueObjects;

namespace ProposalService.UnitTests.Application.Commands;

public class CreateProposalCommandHandlerTests
{
    private readonly IProposalRepository _repository;
    private readonly IProposalEventPublisher _eventPublisher;
    private readonly CreateProposalCommandHandler _handler;

    public CreateProposalCommandHandlerTests()
    {
        _repository = Substitute.For<IProposalRepository>();
        _eventPublisher = Substitute.For<IProposalEventPublisher>();
        _handler = new CreateProposalCommandHandler(_repository, _eventPublisher);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateProposal()
    {
        // Arrange
        var command = new CreateProposalCommand
        {
            CustomerCPF = "11144477735",
            CustomerName = "John Doe",
            InsuranceValue = 50000m
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.CustomerName.Should().Be(command.CustomerName);
        result.CustomerCPF.Should().Be("111.444.777-35");
        result.InsuranceValue.Should().Be(command.InsuranceValue);
        result.Status.Should().Be("InAnalysis");
        result.ProposalNumber.Should().NotBeNullOrEmpty();
        result.ProposalNumber.Should().StartWith("PROP-");

        await _repository.Received(1).AddAsync(
            Arg.Is<Proposal>(p => p.CustomerName == command.CustomerName),
            Arg.Any<CancellationToken>());

        await _eventPublisher.Received(1).PublishAsync(
            Arg.Is<ProposalCreatedEvent>(e => e.ProposalId == result.Id),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldGenerateUniqueProposalNumber()
    {
        // Arrange
        var command = new CreateProposalCommand
        {
            CustomerCPF = "11144477735",
            CustomerName = "John Doe",
            InsuranceValue = 50000m
        };

        // Act
        var result1 = await _handler.Handle(command, CancellationToken.None);
        var result2 = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result1.ProposalNumber.Should().NotBe(result2.ProposalNumber);
    }

    [Fact]
    public async Task Handle_ShouldSetCreatedDateToUtcNow()
    {
        // Arrange
        var command = new CreateProposalCommand
        {
            CustomerCPF = "11144477735",
            CustomerName = "John Doe",
            InsuranceValue = 50000m
        };

        var beforeCreation = DateTime.UtcNow;

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        var afterCreation = DateTime.UtcNow;

        // Assert
        result.CreatedAt.Should().BeOnOrAfter(beforeCreation);
        result.CreatedAt.Should().BeOnOrBefore(afterCreation);
    }

    [Fact]
    public async Task Handle_ShouldCallRepositoryWithCorrectData()
    {
        // Arrange
        var command = new CreateProposalCommand
        {
            CustomerCPF = "11144477735",
            CustomerName = "Jane Smith",
            InsuranceValue = 75000m
        };

        Proposal? capturedProposal = null;
        await _repository.AddAsync(Arg.Do<Proposal>(p => capturedProposal = p), Arg.Any<CancellationToken>());

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        capturedProposal.Should().NotBeNull();
        capturedProposal!.CustomerName.Should().Be("Jane Smith");
        capturedProposal.CustomerCPF.Number.Should().Be("11144477735");
        capturedProposal.InsuranceValue.Amount.Should().Be(75000m);
        capturedProposal.Status.Should().Be(ProposalStatus.InAnalysis);
    }
}
