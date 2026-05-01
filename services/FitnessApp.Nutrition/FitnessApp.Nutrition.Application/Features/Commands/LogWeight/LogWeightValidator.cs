using FluentValidation;

namespace FitnessApp.Nutrition.Application.Features.Commands.LogWeight
{
    public class LogWeightValidator : AbstractValidator<LogWeightCommand>
    {
        public LogWeightValidator()
        {
            RuleFor(x => x.Weight).GreaterThan(0).LessThan(500)
                .WithMessage("Weight must be between 0 and 500 kg");
        }
    }
}
