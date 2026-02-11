using Microsoft.AspNetCore.Mvc;
using ManagementSystem.DTOs;
using ManagementSystem.Services;

namespace ManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WebhookController : ControllerBase
{
    private readonly ReleaseService _releaseService;

    public WebhookController(ReleaseService releaseService)
    {
        _releaseService = releaseService;
    }

    [HttpPost]
    public async Task<IActionResult> ReceiveGitLabWebhook([FromBody] GitLabWebhookDto data)
    {
        await _releaseService.ProcessWebhook(data);

        return Ok(new { message = "Webhook processed successfully" });
    }
}