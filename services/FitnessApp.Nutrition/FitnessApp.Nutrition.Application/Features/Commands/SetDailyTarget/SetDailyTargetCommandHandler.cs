using AutoMapper;
using FitnessApp.Nutrition.Application.DTOs;
using FitnessApp.Nutrition.Application.Interfaces;
using FitnessApp.Nutrition.Domain.Entities;
using FitnessApp.Nutrition.Domain.Interfaces.Persistence;
using FitnessApp.Nutrition.Domain.ValueObjects;
using MediatR;

namespace FitnessApp.Nutrition.Application.Features.Commands.SetDailyTarget
{
    public class SetDailyTargetCommandHandler : IRequestHandler<SetDailyTargetCommand, DailyTargetResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public SetDailyTargetCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<DailyTargetResponse> Handle(SetDailyTargetCommand request, CancellationToken ct)
        {
            if (!_currentUserService.IsAuthenticated)
                throw new UnauthorizedAccessException();

            var userId = _currentUserService.UserId!.Value;
            var nutrition = new NutritionValue(request.Calories, request.Proteins, request.Fats, request.Carbs);

            var existing = await _unitOfWork.DailyTargets.GetByUserIdAsync(userId, ct);

            if (existing != null)
            {
                existing.UpdateTarget(request.Goal, nutrition);
            }
            else
            {
                existing = DailyTarget.Create(userId, request.Goal, nutrition);
                await _unitOfWork.DailyTargets.AddAsync(existing, ct);
            }

            await _unitOfWork.SaveChangesAsync(ct);

            return _mapper.Map<DailyTargetResponse>(existing);
        }
    }
}
