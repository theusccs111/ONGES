namespace ONGES.Campaign.API.Endpoints.Campaigns;

using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Interfaces;

public class Update : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPut("/{id:guid}", HandleAsync)
            .WithName("Atualizar Campanha")
            .WithSummary("Atualiza uma campanha existente.")
            .WithDescription("Atualiza os dados de uma campanha ativa. Requer perfil Gestor.")
            .Produces<CampaignResponse>(200)
            .Produces(400)
            .Produces(404)
            .RequireAuthorization("SomenteGestor");

    private static async Task<IResult> HandleAsync(
        Guid id,
        ICampaignService service,
        UpdateCampaignRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await service.UpdateAsync(id, request, cancellationToken);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(result);
    }
}
