using FitnessApp.Nutrition.Application.Interfaces;
using FitnessApp.Nutrition.Domain.Interfaces.Persistence;
using MediatR;

namespace FitnessApp.Nutrition.Application.Features.Commands.DeleteMealLog
{
    public class DeleteMealLogCommandHandler : IRequestHandler<DeleteMealLogCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public DeleteMealLogCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(DeleteMealLogCommand request, CancellationToken ct)
        {
            if (!_currentUserService.IsAuthenticated)
                throw new UnauthorizedAccessException();

            var mealLog = await _unitOfWork.MealLogs.GetByIdAsync(request.MealLogId, ct);
            if (mealLog == null) return false;

            if (mealLog.UserId != _currentUserService.UserId!.Value)
                throw new UnauthorizedAccessException("Cannot delete another user's meal log");

            _unitOfWork.MealLogs.Remove(mealLog);
            await _unitOfWork.SaveChangesAsync(ct);
            return true;
        }
    }
}
