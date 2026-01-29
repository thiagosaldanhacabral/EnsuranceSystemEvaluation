using ContractService.Application.Commands.ContractProposal;
using ContractService.Application.DTOs;
using ContractService.Application.Queries.GetContractById;
using ContractService.Application.Queries.ListContracts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ContractService.API.Endpoints;

/// <summary>
/// Contract API endpoints
/// </summary>
public static class ContractEndpoints
{
    public static void MapContractEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/contracts")
            .WithTags("Contracts");

        group.MapPost("/", CreateContract)
            .WithSummary("Contract an approved proposal")
            .WithDescription("Creates a new insurance contract from an approved proposal. The proposal must exist and be in Approved status.");

        group.MapGet("/", ListContracts)
            .WithSummary("List all contracts")
            .WithDescription("Retrieves a paginated list of insurance contracts ordered by contract date (newest first).");

        group.MapGet("/{id:guid}", GetContractById)
            .WithSummary("Get contract by ID")
            .WithDescription("Retrieves a specific insurance contract by its unique identifier.");
    }

    private static async Task<IResult> CreateContract(
        [FromBody] CreateContractRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new ContractProposalCommand
        {
            ProposalId = request.ProposalId,
            DurationMonths = request.DurationMonths
        };

        var result = await mediator.Send(command, cancellationToken);
        return Results.Created($"/api/contracts/{result.Id}", result);
    }

    private static async Task<IResult> ListContracts(
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new ListContractsQuery
        {
            PageNumber = pageNumber > 0 ? pageNumber : 1,
            PageSize = pageSize > 0 ? pageSize : 20
        };

        var result = await mediator.Send(query, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetContractById(
        Guid id,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetContractByIdQuery { ContractId = id };
        var result = await mediator.Send(query, cancellationToken);

        return result != null
            ? Results.Ok(result)
            : Results.NotFound(new { message = $"Contract {id} not found" });
    }
}

/// <summary>
/// Request model for creating a contract
/// </summary>
public record CreateContractRequest
{
    public Guid ProposalId { get; init; }
    public int DurationMonths { get; init; }
}
