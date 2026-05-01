using AutoMapper;
using FitnessApp.Nutrition.Application.DTOs;
using FitnessApp.Nutrition.Application.Interfaces;
using FitnessApp.Nutrition.Domain.Entities;
using FitnessApp.Nutrition.Domain.Interfaces.Persistence;
using FitnessApp.Nutrition.Domain.ValueObjects;
using MediatR;

namespace FitnessApp.Nutrition.Application.Features.Commands.LogMeal
{
    public class LogMealCommandHandler : IRequestHandler<LogMealCommand, MealLogResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public LogMealCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<MealLogResponse> Handle(LogMealCommand request, CancellationToken ct)
        {
            if (!_currentUserService.IsAuthenticated)
                throw new UnauthorizedAccessException();

            var userId = _currentUserService.UserId!.Value;
            var nutrition = new NutritionValue(request.Calories, request.Proteins, request.Fats, request.Carbs);
            var mealLog = MealLog.Create(userId, request.MealName, request.MealType, nutrition);

            await _unitOfWork.MealLogs.AddAsync(mealLog, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return _mapper.Map<MealLogResponse>(mealLog);
        }
    }
}
