namespace ONGES.Campaign.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Interfaces;
using System.Security.Claims;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class CampaignsController : ControllerBase
{
    private readonly ICampaignService _campaignService;

    public CampaignsController(ICampaignService campaignService)
    {
        _campaignService = campaignService;
    }

    /// <summary>
    /// Obtém uma campanha por ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CampaignResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _campaignService.GetByIdAsync(id, cancellationToken);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result.Value);
    }

    /// <summary>
    /// Obtém todas as campanhas
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CampaignResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await _campaignService.GetAllAsync(cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result.Value);
    }

    /// <summary>
    /// Cria uma nova campanha
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Gestor")]
    public async Task<ActionResult<CampaignResponse>> Create(
        CreateCampaignRequest request,
        CancellationToken cancellationToken)
    {
        var creatorId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());

        var result = await _campaignService.CreateAsync(request, creatorId, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
    }

    /// <summary>
    /// Atualiza uma campanha existente
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Gestor")]
    public async Task<ActionResult<CampaignResponse>> Update(
        Guid id,
        UpdateCampaignRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _campaignService.UpdateAsync(id, request, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result.Value);
    }

    /// <summary>
    /// Cancela uma campanha ativa
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Gestor")]
    public async Task<ActionResult<CampaignResponse>> Cancel(Guid id, CancellationToken cancellationToken)
    {
        var result = await _campaignService.CancelAsync(id, cancellationToken);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result.Value);
    }
}
