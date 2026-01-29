using FluentAssertions;
using NSubstitute;
using ProposalService.Application.Queries.GetProposalById;
using ProposalService.Domain.Entities;
using ProposalService.Domain.Ports;
using ProposalService.Domain.ValueObjects;
using SharedKernel.Domain;

namespace ProposalService.UnitTests.Application.Queries;

public class GetProposalByIdQueryHandlerTests
{
    private readonly IProposalRepository _repository;
    private readonly GetProposalByIdQueryHandler _handler;

    public GetProposalByIdQueryHandlerTests()
    {
        _repository = Substitute.For<IProposalRepository>();
        _handler = new GetProposalByIdQueryHandler(_repository);
    }

    [Fact]
    public async Task Handle_ProposalExists_ShouldReturnProposalDto()
    {
        // Arrange
        var proposal = CreateProposal();
        _repository.GetByIdAsync(proposal.Id, Arg.Any<CancellationToken>())
            .Returns(proposal);

        var query = new GetProposalByIdQuery { ProposalId = proposal.Id };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(proposal.Id);
        result.ProposalNumber.Should().Be(proposal.ProposalNumber);
        result.CustomerName.Should().Be(proposal.CustomerName);
        result.CustomerCPF.Should().Be(proposal.CustomerCPF.Number);
        result.InsuranceValue.Should().Be(proposal.InsuranceValue.Amount);
        result.Status.Should().Be(proposal.Status.ToString());
    }

    [Fact]
    public async Task Handle_ProposalNotFound_ShouldReturnNull()
    {
        // Arrange
        _repository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Proposal?)null);

        var query = new GetProposalByIdQuery { ProposalId = Guid.NewGuid() };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldCallRepositoryOnce()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var query = new GetProposalByIdQuery { ProposalId = proposalId };

        _repository.GetByIdAsync(proposalId, Arg.Any<CancellationToken>())
            .Returns((Proposal?)null);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        await _repository.Received(1).GetByIdAsync(proposalId, Arg.Any<CancellationToken>());
    }

    private static Proposal CreateProposal()
    {
        var cpf = CPF.Create("11144477735");
        var insuranceValue = Money.Create(50000m);
        return Proposal.Create("PROP-20260129-00001", "John Doe", cpf, insuranceValue);
    }
}
