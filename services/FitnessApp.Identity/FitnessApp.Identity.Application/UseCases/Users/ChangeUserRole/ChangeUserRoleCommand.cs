using System;
using System.Threading;
using System.Threading.Tasks;
using FitnessApp.Identity.Application.Common.Exceptions;
using FitnessApp.Identity.Domain.Interfaces.Repositories;
using MediatR;

namespace FitnessApp.Identity.Application.UseCases.Users.ChangeUserRole
{
    public record ChangeUserRoleCommand(Guid UserId, string RoleName) : IRequest;

    public class ChangeUserRoleCommandHandler : IRequestHandler<ChangeUserRoleCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;

        public ChangeUserRoleCommandHandler(IUserRepository userRepository, IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        public async Task Handle(ChangeUserRoleCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                throw new NotFoundException("User", request.UserId);
            }

            var role = await _roleRepository.GetByNameAsync(request.RoleName, cancellationToken);
            if (role == null)
            {
                throw new NotFoundException("Role", request.RoleName);
            }

            // Skip if user already has exactly this role
            if (user.UserRoles.Count == 1 && user.UserRoles.Any(ur => ur.RoleId == role.Id))
                return;

            // Clear existing roles via repository (handles EF tracking)
            await _userRepository.ClearUserRolesAsync(user.Id, cancellationToken);

            // Re-fetch user to get clean state after role deletion
            user = (await _userRepository.GetByIdAsync(request.UserId, cancellationToken))!;

            // Add the new role
            user.AddRole(role);

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
