using FluentValidation;

namespace FitnessApp.Nutrition.Application.Features.Commands.SetDailyTarget
{
    public class SetDailyTargetValidator : AbstractValidator<SetDailyTargetCommand>
    {
        public SetDailyTargetValidator()
        {
            RuleFor(x => x.Goal).IsInEnum();
            RuleFor(x => x.Calories).GreaterThan(0).WithMessage("Daily calorie target must be greater than 0");
            RuleFor(x => x.Proteins).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Fats).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Carbs).GreaterThanOrEqualTo(0);
        }
    }
}
