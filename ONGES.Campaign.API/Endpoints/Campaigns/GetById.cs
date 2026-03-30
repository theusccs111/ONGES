namespace ONGES.Campaign.API.Endpoints.Campaigns;

using Application.DTOs.Responses;
using Application.Interfaces;

public class GetById : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/{id:guid}", HandleAsync)
            .WithName("Obter Campanha por ID")
            .WithSummary("Obtém uma campanha por ID.")
            .WithDescription("Obtém os dados de uma campanha pelo ID.")
            .Produces<CampaignResponse>(200)
            .Produces(404)
            .AllowAnonymous();

    private static async Task<IResult> HandleAsync(
        Guid id,
        ICampaignService service,
        CancellationToken cancellationToken = default)
    {
        var result = await service.GetByIdAsync(id, cancellationToken);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.NotFound(result);
    }
}
