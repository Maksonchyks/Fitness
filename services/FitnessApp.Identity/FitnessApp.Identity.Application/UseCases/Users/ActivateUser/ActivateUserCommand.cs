using System;
using System.Threading;
using System.Threading.Tasks;
using FitnessApp.Identity.Application.Common.Exceptions;
using FitnessApp.Identity.Domain.Interfaces.Repositories;
using MediatR;

namespace FitnessApp.Identity.Application.UseCases.Users.ActivateUser
{
    public record ActivateUserCommand(Guid UserId) : IRequest;

    public class ActivateUserCommandHandler : IRequestHandler<ActivateUserCommand>
    {
        private readonly IUserRepository _userRepository;

        public ActivateUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task Handle(ActivateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                throw new NotFoundException("User", request.UserId);
            }

            user.Activate();
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
