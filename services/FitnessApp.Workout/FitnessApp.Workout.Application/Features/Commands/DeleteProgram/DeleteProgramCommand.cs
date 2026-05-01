using System;
using System.Threading;
using System.Threading.Tasks;
using FitnessApp.Workout.Application.Interfaces;
using FitnessApp.Workout.Domain.Interfaces.Persistence;
using MediatR;

namespace FitnessApp.Workout.Application.Features.Commands.DeleteProgram
{
    public record DeleteProgramCommand(Guid ProgramId) : IRequest<bool>;

    public class DeleteProgramCommandHandler : IRequestHandler<DeleteProgramCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public DeleteProgramCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(DeleteProgramCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated)
                throw new UnauthorizedAccessException();

            var program = await _unitOfWork.Programs.GetByIdAsync(request.ProgramId, cancellationToken);
            
            if (program == null)
                return false;

            if (program.UserId != _currentUserService.UserId!.Value)
                throw new UnauthorizedAccessException("You can only delete your own programs.");

            _unitOfWork.Programs.Remove(program);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
