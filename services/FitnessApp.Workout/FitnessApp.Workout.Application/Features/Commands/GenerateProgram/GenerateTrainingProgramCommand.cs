using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Workout.Application.DTOs;
using FitnessApp.Workout.Domain.Enums;
using FitnessApp.Workout.Domain.ValueObjects;
using MediatR;

namespace FitnessApp.Workout.Application.Features.Commands.GenerateProgram
{
    public record GenerateTrainingProgramCommand(
        FitnessGoal FitnessGoal,
        Intensity Intensity,
        PowerMetrics? PowerMetrics
        ) : IRequest<TrainingProgramResponse>;
}
