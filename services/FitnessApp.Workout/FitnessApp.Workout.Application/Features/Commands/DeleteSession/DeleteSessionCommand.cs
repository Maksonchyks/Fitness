using System;
using System.Threading;
using System.Threading.Tasks;
using FitnessApp.Workout.Application.Interfaces;
using FitnessApp.Workout.Domain.Interfaces.Persistence;
using MediatR;

namespace FitnessApp.Workout.Application.Features.Commands.DeleteSession
{
    public record DeleteSessionCommand(Guid SessionId) : IRequest<bool>;

    public class DeleteSessionCommandHandler : IRequestHandler<DeleteSessionCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public DeleteSessionCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(DeleteSessionCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated)
                throw new UnauthorizedAccessException();

            var session = await _unitOfWork.Sessions.GetByIdAsync(request.SessionId, cancellationToken);
            
            if (session == null)
                return false;

            if (session.UserId != _currentUserService.UserId!.Value)
                throw new UnauthorizedAccessException("You can only delete your own sessions.");

            _unitOfWork.Sessions.Remove(session);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
