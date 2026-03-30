namespace ONGES.Campaign.API.Endpoints.Campaigns;

using Application.DTOs.Responses;
using Application.Interfaces;

public class GetAll : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/", HandleAsync)
            .WithName("Obter todas as Campanhas")
            .WithSummary("Obtém todas as campanhas.")
            .WithDescription("Obtém a lista de todas as campanhas. Requer perfil Gestor.")
            .Produces<List<CampaignResponse>>(200)
            .RequireAuthorization("SomenteGestor");

    private static async Task<IResult> HandleAsync(
        ICampaignService service,
        CancellationToken cancellationToken = default)
    {
        var result = await service.GetAllAsync(cancellationToken);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(result);
    }
}
