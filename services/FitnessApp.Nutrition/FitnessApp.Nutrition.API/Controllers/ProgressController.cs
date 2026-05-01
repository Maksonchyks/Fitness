using FitnessApp.Nutrition.Application.Features.Queries.GetDailyProgress;
using FitnessApp.Nutrition.Application.Features.Commands.SetDailyTarget;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessApp.Nutrition.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProgressController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProgressController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetDailyProgress([FromQuery] DateTime? date)
    {
        var query = new GetDailyProgressQuery(date);
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
