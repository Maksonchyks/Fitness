using FitnessApp.Nutrition.Domain.Common;
using FitnessApp.Nutrition.Domain.Enums;
using FitnessApp.Nutrition.Domain.Exceptions;
using FitnessApp.Nutrition.Domain.ValueObjects;

namespace FitnessApp.Nutrition.Domain.Entities
{
    public class TemplateMeal : Entity
    {
        public Guid TemplateId { get; private set; }
        public MealPlanTemplate Template { get; private set; } = null!;
        public MealType MealType { get; private set; }
        public string DishName { get; private set; } = null!;
        public string Description { get; private set; } = null!;
        public NutritionValue Nutrition { get; private set; } = null!;

        protected TemplateMeal() { }

        public TemplateMeal(MealType mealType, string dishName, string description, NutritionValue nutrition) : base()
        {
            Guard.AgainstNullOrEmpty(dishName, nameof(DishName));
            Guard.AgainstNullOrEmpty(description, nameof(Description));
            if (nutrition == null) throw new DomainException("Nutrition values are required");

            MealType = mealType;
            DishName = dishName;
            Description = description;
            Nutrition = nutrition;
        }

        public TemplateMeal(Guid templateId, MealType mealType, string dishName, string description, NutritionValue nutrition) 
            : this(mealType, dishName, description, nutrition)
        {
            Guard.AgainstEmptyGuid(templateId, nameof(TemplateId));
            TemplateId = templateId;
        }
    }
}
