namespace ONGES.Campaign.API.Endpoints;

using Campaigns;
using Transparency;

public static class Endpoint
{
    public static void MapEndpoints(this WebApplication app)
    {
        var endpoints = app.MapGroup("");

        endpoints.MapGroup("v1/campaigns")
            .WithTags("Campanhas")
            .MapEndpoint<Create>()
            .MapEndpoint<Update>()
            .MapEndpoint<Cancel>()
            .MapEndpoint<GetById>()
            .MapEndpoint<GetAll>();

        endpoints.MapGroup("v1/transparency")
            .WithTags("Transparência")
            .MapEndpoint<GetActiveCampaigns>();
    }

    private static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app)
        where TEndpoint : IEndpoint
    {
        TEndpoint.Map(app);
        return app;
    }
}
