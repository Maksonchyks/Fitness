using FitnessApp.Nutrition.Application.Interfaces;
using FitnessApp.Nutrition.Domain.Interfaces.Persistence;
using MediatR;

namespace FitnessApp.Nutrition.Application.Features.Commands.ResetTemplateQueue
{
    public class ResetTemplateQueueCommandHandler : IRequestHandler<ResetTemplateQueueCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public ResetTemplateQueueCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(ResetTemplateQueueCommand request, CancellationToken ct)
        {
            if (!_currentUserService.IsAuthenticated)
                throw new UnauthorizedAccessException();

            var userId = _currentUserService.UserId!.Value;
            var queue = await _unitOfWork.TemplateQueues.GetByUserIdAsync(userId, ct);

            if (queue == null) return false;

            queue.Reset();
            await _unitOfWork.SaveChangesAsync(ct);
            return true;
        }
    }
}
