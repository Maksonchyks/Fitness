using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace FitnessApp.Workout.Application.Features.Commands.RecordSession
{
    public class RecordWorkoutSessionValidator : AbstractValidator<RecordWorkoutSessionCommand>
    {
        public RecordWorkoutSessionValidator()
        {
            RuleFor(x => x.TrainingDayId).NotEmpty();
            RuleFor(x => x.Results).NotEmpty().WithMessage("Workout cannot be empty");
            RuleForEach(x => x.Results).ChildRules(ex => {
                ex.RuleFor(s => s.Weight).GreaterThanOrEqualTo(0);
                ex.RuleFor(s => s.Reps).GreaterThan(0);
                ex.RuleFor(s => s.Sets).GreaterThan(0);
            });
        }
    }
}
