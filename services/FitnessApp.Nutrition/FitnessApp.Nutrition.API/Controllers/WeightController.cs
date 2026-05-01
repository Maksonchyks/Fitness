using FitnessApp.Nutrition.Application.Features.Commands.LogWeight;
using FitnessApp.Nutrition.Application.Features.Queries.GetWeightHistory;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessApp.Nutrition.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WeightController : ControllerBase
{
    private readonly IMediator _mediator;

    public WeightController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> LogWeight([FromBody] LogWeightCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetWeightHistory([FromQuery] int days = 30)
    {
        var query = new GetWeightHistoryQuery(days);
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
