using FitnessApp.Nutrition.Domain.Common;
using FitnessApp.Nutrition.Domain.Exceptions;
using FitnessApp.Nutrition.Domain.ValueObjects;

namespace FitnessApp.Nutrition.Domain.Entities
{
    public class MealPlanTemplate : AggregateRoot
    {
        public string Name { get; private set; } = null!;
        public string Description { get; private set; } = null!;
        public NutritionValue TotalNutrition { get; private set; } = null!;

        private readonly List<TemplateMeal> _meals = new();
        public IReadOnlyCollection<TemplateMeal> Meals => _meals.AsReadOnly();

        protected MealPlanTemplate() { }

        private MealPlanTemplate(string name, string description) : base()
        {
            Name = name;
            Description = description;
        }

        public static MealPlanTemplate Create(string name, string description, List<TemplateMeal> meals)
        {
            Guard.AgainstNullOrEmpty(name, nameof(Name));
            Guard.AgainstNullOrEmpty(description, nameof(Description));
            if (meals == null || meals.Count == 0)
                throw new DomainException("Template must have at least one meal");

            var template = new MealPlanTemplate(name, description);

            foreach (var meal in meals)
            {
                template._meals.Add(meal);
            }

            template.TotalNutrition = NutritionValue.Sum(meals.Select(m => m.Nutrition));

            return template;
        }
    }
}
