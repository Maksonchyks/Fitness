using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Workout.Domain.Enums;
using FluentValidation;

namespace FitnessApp.Workout.Application.Features.Commands.GenerateProgram
{
    public class GenerateTrainingProgramValidator : AbstractValidator<GenerateTrainingProgramCommand>
    {
        public GenerateTrainingProgramValidator()
        {
            RuleFor(x => x.FitnessGoal).IsInEnum();
            RuleFor(x => x.Intensity).IsInEnum();
        }
    }
}
