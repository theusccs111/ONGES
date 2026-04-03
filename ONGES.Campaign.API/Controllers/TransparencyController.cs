namespace ONGES.Campaign.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs.Responses;
using Application.Interfaces;

[ApiController]
[Route("api/v1/[controller]")]
[AllowAnonymous]
public class TransparencyController : ControllerBase
{
    private readonly ICampaignService _campaignService;

    public TransparencyController(ICampaignService campaignService)
    {
        _campaignService = campaignService;
    }

    /// <summary>
    /// Obtém as campanhas ativas para o painel de transparência
    /// </summary>
    /// <remarks>
    /// Retorna apenas Título, Meta Financeira e Valor Arrecadado das campanhas ativas.
    /// Esta é uma rota pública sem necessidade de autenticação.
    /// </remarks>
    [HttpGet("campaigns")]
    public async Task<ActionResult<IEnumerable<TransparencyPanelResponse>>> GetActiveCampaigns(CancellationToken cancellationToken)
    {
        var result = await _campaignService.GetAllActiveAsync(cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result.Value);
    }
}
