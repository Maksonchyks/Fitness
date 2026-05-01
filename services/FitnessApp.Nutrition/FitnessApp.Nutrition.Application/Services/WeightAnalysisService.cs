using FitnessApp.Nutrition.Application.DTOs;
using FitnessApp.Nutrition.Domain.Entities;
using FitnessApp.Nutrition.Domain.Enums;
using FitnessApp.Nutrition.Domain.ValueObjects;

namespace FitnessApp.Nutrition.Application.Services
{
    public class WeightAnalysisService
    {
        /// <summary>
        /// Analyzes weight trend over 14 days and produces an adjustment recommendation
        /// if progress has stalled based on the user's nutrition goal.
        /// </summary>
        public NutritionAdjustmentDto? Analyze(IEnumerable<WeightLog> weightLogs, DailyTarget target)
        {
            var logs = weightLogs
                .OrderBy(l => l.LoggedAt)
                .ToList();

            // Need at least 5 entries over 14 days for meaningful analysis
            if (logs.Count < 5)
                return null;

            var now = DateTime.UtcNow;
            var midpoint = now.AddDays(-7);

            var week1Logs = logs.Where(l => l.LoggedAt < midpoint).ToList();
            var week2Logs = logs.Where(l => l.LoggedAt >= midpoint).ToList();

            // Need data in both weeks
            if (week1Logs.Count < 2 || week2Logs.Count < 2)
                return null;

            var avgWeek1 = week1Logs.Average(l => l.Weight);
            var avgWeek2 = week2Logs.Average(l => l.Weight);

            var weightChange = avgWeek2 - avgWeek1;

            return target.Goal switch
            {
                NutritionGoal.WeightLoss => AnalyzeWeightLoss(weightChange, target),
                NutritionGoal.MuscleGain => AnalyzeMuscleGain(weightChange, target),
                NutritionGoal.Maintenance => null, // No auto-adjustment for maintenance
                _ => null
            };
        }

        private NutritionAdjustmentDto? AnalyzeWeightLoss(float weightChange, DailyTarget target)
        {
            // If weight is not dropping (stagnant or increasing)
            if (weightChange >= 0)
            {
                var adjustmentPercent = weightChange > 0.5f ? -0.10f : -0.05f;
                var suggestedTarget = target.Target.ScaleBy(1 + adjustmentPercent);

                return new NutritionAdjustmentDto(
                    Message: "Ми помітили, що прогрес у схудненні уповільнився. Бажаєте адаптувати вашу норму КБЖВ?",
                    SuggestedTarget: new NutritionValueDto(
                        suggestedTarget.Calories,
                        suggestedTarget.Proteins,
                        suggestedTarget.Fats,
                        suggestedTarget.Carbs
                    ),
                    AdjustmentPercent: adjustmentPercent * 100
                );
            }
            return null;
        }

        private NutritionAdjustmentDto? AnalyzeMuscleGain(float weightChange, DailyTarget target)
        {
            // If weight is not increasing (stagnant or dropping)
            if (weightChange <= 0)
            {
                var adjustmentPercent = weightChange < -0.5f ? 0.10f : 0.05f;
                var suggestedTarget = target.Target.ScaleBy(1 + adjustmentPercent);

                return new NutritionAdjustmentDto(
                    Message: "Ми помітили, що набір маси уповільнився. Бажаєте збільшити вашу норму КБЖВ?",
                    SuggestedTarget: new NutritionValueDto(
                        suggestedTarget.Calories,
                        suggestedTarget.Proteins,
                        suggestedTarget.Fats,
                        suggestedTarget.Carbs
                    ),
                    AdjustmentPercent: adjustmentPercent * 100
                );
            }
            return null;
        }
    }
}
