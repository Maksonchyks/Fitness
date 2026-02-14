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

            RuleFor(x => x.PowerMetrics)
                .NotNull()
                .When(x => x.FitnessGoal == FitnessGoal.Powerlifting || x.FitnessGoal == FitnessGoal.Bodybuilding)
                .WithMessage("Power metrics are required for your selected fitness goal.");
        }
    }
}
