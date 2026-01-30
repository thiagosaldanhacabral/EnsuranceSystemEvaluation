using ContractService.Domain.Ports;
using System.Net.Http.Json;

namespace ContractService.Infrastructure.ExternalServices;

/// <summary>
/// HTTP client for communicating with ProposalService
/// </summary>
public class ProposalServiceClient : IProposalServiceClient
{
    private readonly HttpClient _httpClient;

    public ProposalServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ProposalDto?> GetProposalByIdAsync(Guid proposalId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/proposals/{proposalId}", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;

                response.EnsureSuccessStatusCode();
            }

            return await response.Content.ReadFromJsonAsync<ProposalDto>(cancellationToken);
        }
        catch (HttpRequestException)
        {
            // Log the exception in production
            throw;
        }
    }
}
