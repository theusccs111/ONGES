namespace ONGES.Campaign.Application.Mappers;

using AutoMapper;
using Domain.Aggregates;
using DTOs;

/// <summary>
/// AutoMapper profile for Campaign entity to DTO mappings.
/// </summary>
public class CampaignMappingProfile : Profile
{
    public CampaignMappingProfile()
    {
        CreateMap<CampaignAggregate, CampaignResponse>()
            .ForMember(dest => dest.FinancialTarget, opt => opt.MapFrom(src => src.FinancialTarget.Amount))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.Value.ToString()));

        CreateMap<CampaignAggregate, TransparencyPanelResponse>()
            .ForMember(dest => dest.FinancialTarget, opt => opt.MapFrom(src => src.FinancialTarget.Amount));
    }
}
