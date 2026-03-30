namespace ONGES.Campaign.API.Endpoints.Campaigns;

using Application.DTOs.Responses;
using Application.Interfaces;

public class Cancel : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapDelete("/{id:guid}", HandleAsync)
            .WithName("Cancelar Campanha")
            .WithSummary("Cancela uma campanha ativa.")
            .WithDescription("Cancela uma campanha ativa. Requer perfil Gestor.")
            .Produces<CampaignResponse>(200)
            .Produces(404)
            .RequireAuthorization("SomenteGestor");

    private static async Task<IResult> HandleAsync(
        Guid id,
        ICampaignService service,
        CancellationToken cancellationToken = default)
    {
        var result = await service.CancelAsync(id, cancellationToken);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.NotFound(result);
    }
}
