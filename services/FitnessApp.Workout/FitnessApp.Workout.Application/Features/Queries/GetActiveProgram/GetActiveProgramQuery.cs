using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Workout.Application.DTOs;
using MediatR;

namespace FitnessApp.Workout.Application.Features.Queries.GetActiveProgram
{
    public record GetActiveProgramQuery(Guid UserId) : IRequest<TrainingProgramResponse>;
}
