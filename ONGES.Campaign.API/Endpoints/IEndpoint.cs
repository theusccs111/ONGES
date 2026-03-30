namespace ONGES.Campaign.API.Endpoints;

public interface IEndpoint
{
    static abstract void Map(IEndpointRouteBuilder app);
}
