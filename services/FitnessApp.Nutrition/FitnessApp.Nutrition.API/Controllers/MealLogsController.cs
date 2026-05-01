using FitnessApp.Nutrition.Application.Features.Commands.LogMeal;
using FitnessApp.Nutrition.Application.Features.Commands.DeleteMealLog;
using FitnessApp.Nutrition.Application.Features.Queries.GetMealLogs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessApp.Nutrition.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MealLogsController : ControllerBase
{
    private readonly IMediator _mediator;

    public MealLogsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> LogMeal([FromBody] LogMealCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetMealLogs([FromQuery] DateTime? date)
    {
        var query = new GetMealLogsQuery(date);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMealLog(Guid id)
    {
        var result = await _mediator.Send(new DeleteMealLogCommand(id));
        if (!result) return NotFound();
        return NoContent();
    }
}
