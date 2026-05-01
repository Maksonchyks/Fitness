using AutoMapper;
using FitnessApp.Nutrition.Application.DTOs;
using FitnessApp.Nutrition.Application.Interfaces;
using FitnessApp.Nutrition.Application.Services;
using FitnessApp.Nutrition.Domain.Interfaces.Persistence;
using FitnessApp.Nutrition.Domain.ValueObjects;
using MediatR;

namespace FitnessApp.Nutrition.Application.Features.Queries.GetDailyProgress
{
    public class GetDailyProgressHandler : IRequestHandler<GetDailyProgressQuery, DailyProgressResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly WeightAnalysisService _weightAnalysis;

        public GetDailyProgressHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IMapper mapper,
            WeightAnalysisService weightAnalysis)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _weightAnalysis = weightAnalysis;
        }

        public async Task<DailyProgressResponse> Handle(GetDailyProgressQuery request, CancellationToken ct)
        {
            if (!_currentUserService.IsAuthenticated)
                throw new UnauthorizedAccessException();

            var userId = _currentUserService.UserId!.Value;
            var date = request.Date ?? DateTime.UtcNow.Date;

            // 1. Get today's meals
            var meals = await _unitOfWork.MealLogs.GetByUserIdAndDateAsync(userId, date, ct);
            var mealList = meals.ToList();

            // 2. Calculate consumed totals
            var consumed = mealList.Count > 0
                ? NutritionValue.Sum(mealList.Select(m => m.Nutrition))
                : NutritionValue.Zero;

            var consumedDto = _mapper.Map<NutritionValueDto>(consumed);

            // 3. Get user's daily target
            var dailyTarget = await _unitOfWork.DailyTargets.GetByUserIdAsync(userId, ct);
            NutritionValueDto? targetDto = dailyTarget != null
                ? _mapper.Map<NutritionValueDto>(dailyTarget.Target)
                : null;

            // 4. Calculate progress percentages
            NutritionProgressDto? progressDto = null;
            if (dailyTarget != null)
            {
                progressDto = new NutritionProgressDto(
                    CaloriesPercent: dailyTarget.Target.Calories > 0
                        ? (consumed.Calories / dailyTarget.Target.Calories) * 100f : 0f,
                    ProteinsPercent: dailyTarget.Target.Proteins > 0
                        ? (consumed.Proteins / dailyTarget.Target.Proteins) * 100f : 0f,
                    FatsPercent: dailyTarget.Target.Fats > 0
                        ? (consumed.Fats / dailyTarget.Target.Fats) * 100f : 0f,
                    CarbsPercent: dailyTarget.Target.Carbs > 0
                        ? (consumed.Carbs / dailyTarget.Target.Carbs) * 100f : 0f
                );
            }

            // 5. Lazy weight analysis — auto-adjustment recommendation
            NutritionAdjustmentDto? adjustment = null;
            if (dailyTarget != null)
            {
                var weightLogs = await _unitOfWork.WeightLogs.GetByUserIdAsync(userId, 14, ct);
                adjustment = _weightAnalysis.Analyze(weightLogs, dailyTarget);
            }

            // 6. Map meals
            var mealDtos = mealList.Select(m => _mapper.Map<MealLogResponse>(m)).ToList();

            return new DailyProgressResponse(
                Consumed: consumedDto,
                Target: targetDto,
                Progress: progressDto,
                Adjustment: adjustment,
                Meals: mealDtos
            );
        }
    }
}
