namespace ONGES.Campaign.Application.Mappers;

using AutoMapper;
using Domain.Aggregates;
using DTOs;

/// <summary>
/// Mapeador de Campaign para DTOs usando AutoMapper.
/// </summary>
public class CampaignMappingProfile : Profile
{
    public CampaignMappingProfile()
    {
        CreateMap<CampaignAggregate, CampaignResponse>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.Value.ToString()));

        CreateMap<CampaignAggregate, TransparencyPanelResponse>();
    }
}
