using FitnessApp.Workout.Application.Features.Commands.GenerateProgram;
using FitnessApp.Workout.Application.Features.Queries.GetActiveProgram;
using FitnessApp.Workout.Application.Features.Queries.GetTrainingDay;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessApp.Workout.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProgramsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly FitnessApp.Workout.Application.Interfaces.ICurrentUserService _currentUserService;

    public ProgramsController(IMediator mediator, FitnessApp.Workout.Application.Interfaces.ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    [HttpPost("generate")]
    public async Task<IActionResult> GenerateProgram([FromBody] GenerateTrainingProgramCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveProgram()
    {
        var userId = _currentUserService.UserId;
        if (userId == null) return Unauthorized();

        var query = new GetActiveProgramQuery(userId.Value);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{programId}/day/{dayNumber}")]
    public async Task<IActionResult> GetTrainingDay(Guid programId, int dayNumber)
    {
        var query = new GetTrainingDayQuery(programId, dayNumber);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetProgramHistory()
    {
        var result = await _mediator.Send(new FitnessApp.Workout.Application.Features.Queries.GetProgramHistory.GetProgramHistoryQuery());
        return Ok(result);
    }

    [HttpDelete("{programId}")]
    public async Task<IActionResult> DeleteProgram(Guid programId)
    {
        var result = await _mediator.Send(new FitnessApp.Workout.Application.Features.Commands.DeleteProgram.DeleteProgramCommand(programId));
        if (!result) return NotFound();
        return NoContent();
    }
}
