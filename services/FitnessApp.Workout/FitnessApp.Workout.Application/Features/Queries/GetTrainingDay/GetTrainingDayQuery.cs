using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Workout.Application.DTOs;
using MediatR;

namespace FitnessApp.Workout.Application.Features.Queries.GetTrainingDay
{
    public record GetTrainingDayQuery(Guid ProgramId, int DayNumber) : IRequest<TrainingDayDto>;
}
