namespace ONGES.Campaign.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;

/// <summary>
/// Controller for the transparency panel.
/// Public access for active campaigns only.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class TransparencyController : ControllerBase
{
    private readonly ICampaignService _campaignService;

    public TransparencyController(ICampaignService campaignService)
    {
        _campaignService = campaignService;
    }

    /// <summary>
    /// Returns all active campaigns.
    /// Public endpoint for the transparency panel.
    /// Returns only Title, Financial Target and Total Amount Raised.
    /// </summary>
    [HttpGet("campaigns")]
    public IActionResult GetActiveCampaigns()
    {
        var result = _campaignService.GetAllActive();
        return Ok(result);
    }
}
