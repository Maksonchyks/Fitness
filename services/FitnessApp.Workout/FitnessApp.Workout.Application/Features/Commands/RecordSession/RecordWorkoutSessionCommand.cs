using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Workout.Application.DTOs;
using MediatR;

namespace FitnessApp.Workout.Application.Features.Commands.RecordSession
{
    public record RecordWorkoutSessionCommand(Guid TrainingDayId, List<ExerciseSetDto> Results) : IRequest<Guid>;
}
