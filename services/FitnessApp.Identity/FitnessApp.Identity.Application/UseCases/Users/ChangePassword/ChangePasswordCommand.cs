using System;
using System.Threading;
using System.Threading.Tasks;
using FitnessApp.Identity.Application.Common.Exceptions;
using FitnessApp.Identity.Domain.Interfaces.Repositories;
using FitnessApp.Identity.Application.Interfaces;
using MediatR;

namespace FitnessApp.Identity.Application.UseCases.Users.ChangePassword
{
    public record ChangePasswordCommand(Guid UserId, string CurrentPassword, string NewPassword) : IRequest;

    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;

        public ChangePasswordCommandHandler(IUserRepository userRepository, IPasswordService passwordService)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
        }

        public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                throw new NotFoundException("User", request.UserId);
            }

            if (!_passwordService.VerifyPassword(request.CurrentPassword, user.Password))
            {
                throw new ValidationException("Invalid current password");
            }

            var newPassword = _passwordService.CreatePassword(request.NewPassword);
            user.ChangePassword(newPassword);

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
