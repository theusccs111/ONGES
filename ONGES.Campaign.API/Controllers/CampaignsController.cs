namespace ONGES.Campaign.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Application.Commands;
using Application.DTOs;
using Application.Queries;
using System.Security.Claims;

/// <summary>
/// Controller responsável pela gestão de campanhas.
/// Acesso restrito a usuários com role GestorONG.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "GestorONG")]
public class CampaignsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CampaignsController> _logger;

    public CampaignsController(IMediator mediator, ILogger<CampaignsController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Cria uma nova campanha.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CampaignResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CampaignResponse>> CreateCampaign(
        [FromBody] CreateCampaignRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var creatorId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
            
            var command = new CreateCampaignCommand(
                request.Title,
                request.Description,
                request.FinancialTarget,
                request.StartDate,
                request.EndDate,
                creatorId);

            var result = await _mediator.Send(command, cancellationToken);
            
            _logger.LogInformation("Campaign created: {CampaignId}", result.Id);
            return CreatedAtAction(nameof(GetCampaignById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Error creating campaign");
            return BadRequest(new ApiResponse<object>(false, ex.Message, null, [ex.Message]));
        }
    }

    /// <summary>
    /// Obtém uma campanha pelo ID.
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CampaignResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CampaignResponse>> GetCampaignById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetCampaignByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    /// <summary>
    /// Atualiza uma campanha existente.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(CampaignResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CampaignResponse>> UpdateCampaign(
        [FromRoute] Guid id,
        [FromBody] UpdateCampaignRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new UpdateCampaignCommand(
                id,
                request.Title,
                request.Description,
                request.FinancialTarget,
                request.StartDate,
                request.EndDate);

            var result = await _mediator.Send(command, cancellationToken);
            
            _logger.LogInformation("Campaign updated: {CampaignId}", id);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Error updating campaign");
            return BadRequest(new ApiResponse<object>(false, ex.Message, null, [ex.Message]));
        }
    }

    /// <summary>
    /// Cancela uma campanha.
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelCampaign(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new CancelCampaignCommand(id);
            await _mediator.Send(command, cancellationToken);
            
            _logger.LogInformation("Campaign cancelled: {CampaignId}", id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Error cancelling campaign");
            return BadRequest(new ApiResponse<object>(false, ex.Message));
        }
    }

    /// <summary>
    /// Obtém todas as campanhas (apenas para GestorONG).
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<CampaignResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CampaignResponse>>> GetAllCampaigns(
        CancellationToken cancellationToken)
    {
        var query = new GetAllCampaignsQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}

/// <summary>
/// Controller para painel de transparência.
/// Acesso público apenas para campanhas ativas.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class TransparencyController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TransparencyController> _logger;

    public TransparencyController(IMediator mediator, ILogger<TransparencyController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Obtém todas as campanhas ativas.
    /// Endpoint público para painel de transparência.
    /// Retorna apenas Título, Meta Financeira e Valor Total Arrecadado.
    /// </summary>
    [HttpGet("campaigns")]
    [ProducesResponseType(typeof(List<TransparencyPanelResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<TransparencyPanelResponse>>> GetActiveCampaigns(
        CancellationToken cancellationToken)
    {
        try
        {
            var query = new GetActiveCampaignsQuery();
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving active campaigns");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
