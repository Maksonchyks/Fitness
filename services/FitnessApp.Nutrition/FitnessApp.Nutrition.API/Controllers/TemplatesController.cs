using FitnessApp.Nutrition.Application.Features.Queries.GetNextTemplate;
using FitnessApp.Nutrition.Application.Features.Commands.ResetTemplateQueue;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessApp.Nutrition.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TemplatesController : ControllerBase
{
    private readonly IMediator _mediator;

    public TemplatesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("next")]
    public async Task<IActionResult> GetNextTemplate()
    {
        var result = await _mediator.Send(new GetNextTemplateQuery());
        if (result == null) return NotFound("No templates available");
        return Ok(result);
    }

    [HttpPost("reset")]
    public async Task<IActionResult> ResetQueue()
    {
        var result = await _mediator.Send(new ResetTemplateQueueCommand());
        return Ok(new { reset = result });
    }
}
