namespace ONGES.Campaign.API.Endpoints.Transparency;

using Application.DTOs.Responses;
using Application.Interfaces;

public class GetActiveCampaigns : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/campaigns", HandleAsync)
            .WithName("Painel de Transparência")
            .WithSummary("Obtém as campanhas ativas para o painel de transparência.")
            .WithDescription("Retorna apenas Título, Meta Financeira e Valor Arrecadado das campanhas ativas.")
            .Produces<List<TransparencyPanelResponse>>(200)
            .AllowAnonymous();

    private static async Task<IResult> HandleAsync(
        ICampaignService service,
        CancellationToken cancellationToken = default)
    {
        var result = await service.GetAllActiveAsync(cancellationToken);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(result);
    }
}
