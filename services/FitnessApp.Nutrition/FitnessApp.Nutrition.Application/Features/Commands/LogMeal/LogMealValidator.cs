using FluentValidation;

namespace FitnessApp.Nutrition.Application.Features.Commands.LogMeal
{
    public class LogMealValidator : AbstractValidator<LogMealCommand>
    {
        public LogMealValidator()
        {
            RuleFor(x => x.MealName).NotEmpty().MaximumLength(200);
            RuleFor(x => x.MealType).IsInEnum();
            RuleFor(x => x.Calories).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Proteins).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Fats).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Carbs).GreaterThanOrEqualTo(0);
        }
    }
}
