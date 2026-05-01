using FitnessApp.Workout.Application.Features.Commands.RecordSession;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessApp.Workout.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SessionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SessionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> RecordSession([FromBody] RecordWorkoutSessionCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetSessionHistory()
    {
        var result = await _mediator.Send(new FitnessApp.Workout.Application.Features.Queries.GetSessionHistory.GetSessionHistoryQuery());
        return Ok(result);
    }

    [HttpDelete("{sessionId}")]
    public async Task<IActionResult> DeleteSession(Guid sessionId)
    {
        var result = await _mediator.Send(new FitnessApp.Workout.Application.Features.Commands.DeleteSession.DeleteSessionCommand(sessionId));
        if (!result) return NotFound();
        return NoContent();
    }
}
