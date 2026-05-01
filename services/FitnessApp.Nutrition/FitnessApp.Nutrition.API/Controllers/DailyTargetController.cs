using FitnessApp.Nutrition.Application.Features.Commands.SetDailyTarget;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessApp.Nutrition.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DailyTargetController : ControllerBase
{
    private readonly IMediator _mediator;

    public DailyTargetController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> SetDailyTarget([FromBody] SetDailyTargetCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
