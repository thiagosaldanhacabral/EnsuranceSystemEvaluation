using MediatR;
using ProposalService.Application.Commands.CreateProposal;
using ProposalService.Application.Commands.UpdateProposalStatus;
using ProposalService.Application.Queries.GetProposalById;
using ProposalService.Application.Queries.ListProposals;

namespace ProposalService.API.Endpoints;

/// <summary>
/// Proposal endpoints using Minimal APIs
/// </summary>
public static class ProposalEndpoints
{
    public static void MapProposalEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/proposals")
            .WithTags("Proposals");

        group.MapPost("/", CreateProposal)
            .WithName("CreateProposal")
            .WithSummary("Create a new insurance proposal")
            .WithDescription("Creates a new proposal with customer information and insurance value")
            .Produces<Application.DTOs.ProposalDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapGet("/", ListProposals)
            .WithName("ListProposals")
            .WithSummary("List all proposals with pagination")
            .WithDescription("Retrieves a paginated list of proposals, optionally filtered by status")
            .Produces<PagedResult<Application.DTOs.ProposalDto>>(StatusCodes.Status200OK);

        group.MapGet("/{id:guid}", GetProposalById)
            .WithName("GetProposalById")
            .WithSummary("Get proposal by ID")
            .WithDescription("Retrieves a single proposal by its unique identifier")
            .Produces<Application.DTOs.ProposalDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPatch("/{id:guid}/status", UpdateProposalStatus)
            .WithName("UpdateProposalStatus")
            .WithSummary("Update proposal status")
            .WithDescription("Updates the status of a proposal (approve or reject)")
            .Produces<Application.DTOs.ProposalDto>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> CreateProposal(
        CreateProposalCommand command,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return Results.Created($"/api/proposals/{result.Id}", result);
    }

    private static async Task<IResult> ListProposals(
        IMediator mediator,
        int pageNumber = 1,
        int pageSize = 20,
        string? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = new ListProposalsQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            Status = status
        };

        var result = await mediator.Send(query, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetProposalById(
        Guid id,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetProposalByIdQuery { ProposalId = id };
        var result = await mediator.Send(query, cancellationToken);

        return result is not null
            ? Results.Ok(result)
            : Results.NotFound(new { message = $"Proposal with ID {id} not found" });
    }

    private static async Task<IResult> UpdateProposalStatus(
        Guid id,
        UpdateStatusRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new UpdateProposalStatusCommand
        {
            ProposalId = id,
            NewStatus = request.NewStatus,
            RejectionReason = request.RejectionReason
        };

        var result = await mediator.Send(command, cancellationToken);
        return Results.Ok(result);
    }
}

/// <summary>
/// Request model for updating proposal status
/// </summary>
public record UpdateStatusRequest
{
    public string NewStatus { get; init; } = string.Empty;
    public string? RejectionReason { get; init; }
}
