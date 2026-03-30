namespace ONGES.Campaign.API.Endpoints.Campaigns;

using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Interfaces;

public class Create : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("/", HandleAsync)
            .WithName("Criar Campanha")
            .WithSummary("Cria uma nova campanha.")
            .WithDescription("Cria uma nova campanha na aplicação. Requer perfil Gestor.")
            .Produces<CampaignResponse>(201)
            .Produces(400)
            .RequireAuthorization("SomenteGestor");

    private static async Task<IResult> HandleAsync(
        HttpContext httpContext,
        ICampaignService service,
        CreateCampaignRequest request,
        CancellationToken cancellationToken = default)
    {
        var creatorId = Guid.Parse(httpContext.User.FindFirst("UserId")?.Value ?? Guid.Empty.ToString());

        var result = await service.CreateAsync(request, creatorId, cancellationToken);

        return result.IsSuccess
            ? Results.Created($"/v1/campaigns/{result.Value.Id}", result.Value)
            : Results.BadRequest(result);
    }
}
