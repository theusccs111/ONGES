namespace ONGES.Campaign.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs;
using Application.Interfaces;
using System.Security.Claims;

/// <summary>
/// Controller responsible for campaign management.
/// Access restricted to users with the GestorONG role.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "GestorONG")]
public class CampaignsController : ControllerBase
{
    private readonly ICampaignService _campaignService;

    public CampaignsController(ICampaignService campaignService)
    {
        _campaignService = campaignService;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var result = _campaignService.GetAll();
        return Ok(result);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public IActionResult GetById(Guid id)
    {
        var result = _campaignService.GetById(id);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateCampaignRequest request)
    {
        var creatorId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
        var result = _campaignService.Create(request, creatorId);
        await _campaignService.CompleteAsync();
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, [FromBody] UpdateCampaignRequest request)
    {
        var result = _campaignService.Update(id, request);
        await _campaignService.CompleteAsync();
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = _campaignService.Cancel(id);
        await _campaignService.CompleteAsync();
        return Ok(result);
    }
}
