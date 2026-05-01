using FitnessApp.Nutrition.Domain.Common;

namespace FitnessApp.Nutrition.Domain.ValueObjects
{
    public sealed class NutritionValue : ValueObject
    {
        private NutritionValue() { }

        public float Calories { get; init; }
        public float Proteins { get; init; }
        public float Fats { get; init; }
        public float Carbs { get; init; }

        public NutritionValue(float calories, float proteins, float fats, float carbs)
        {
            Guard.AgainstNegativeValue(calories, nameof(Calories));
            Guard.AgainstNegativeValue(proteins, nameof(Proteins));
            Guard.AgainstNegativeValue(fats, nameof(Fats));
            Guard.AgainstNegativeValue(carbs, nameof(Carbs));

            Calories = calories;
            Proteins = proteins;
            Fats = fats;
            Carbs = carbs;
        }

        public static NutritionValue Zero => new(0, 0, 0, 0);

        public static NutritionValue operator +(NutritionValue a, NutritionValue b)
        {
            return new NutritionValue(
                a.Calories + b.Calories,
                a.Proteins + b.Proteins,
                a.Fats + b.Fats,
                a.Carbs + b.Carbs
            );
        }

        public static NutritionValue Sum(IEnumerable<NutritionValue> values)
        {
            var result = Zero;
            foreach (var value in values)
            {
                result = result + value;
            }
            return result;
        }

        public NutritionValue ScaleBy(float factor)
        {
            return new NutritionValue(
                Calories * factor,
                Proteins * factor,
                Fats * factor,
                Carbs * factor
            );
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Calories;
            yield return Proteins;
            yield return Fats;
            yield return Carbs;
        }
    }
}
