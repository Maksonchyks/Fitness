using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FitnessApp.Identity.Application.Common.Exceptions;
using FitnessApp.Identity.Application.Common.Events;
using FitnessApp.Identity.Domain.Exceptions;
using FitnessApp.Identity.Domain.Interfaces.Repositories;
using MediatR;
using MassTransit;

namespace FitnessApp.Identity.Application.UseCases.Users.ChangeUserRole
{
    public record ChangeUserRoleCommand(Guid UserId, string RoleName, Guid PerformerUserId) : IRequest;

    public class ChangeUserRoleCommandHandler : IRequestHandler<ChangeUserRoleCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IPublishEndpoint _publishEndpoint;

        public ChangeUserRoleCommandHandler(
            IUserRepository userRepository, 
            IRoleRepository roleRepository,
            IPublishEndpoint publishEndpoint)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Handle(ChangeUserRoleCommand request, CancellationToken cancellationToken)
        {
            // 1. Заборона змінювати роль самому собі
            if (request.UserId == request.PerformerUserId)
            {
                throw new DomainException("Ви не можете змінювати роль самому собі для запобігання втрати доступу.");
            }

            // 2. Перевіряємо чи існує роль
            var role = await _roleRepository.GetByNameAsync(request.RoleName, cancellationToken);
            if (role == null)
            {
                throw new NotFoundException("Role", request.RoleName);
            }

            // 3. Отримуємо користувача
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                throw new NotFoundException("User", request.UserId);
            }

            // 4. Перевірка на останнього адміна
            if (request.RoleName != "Admin")
            {
                if (user.HasRole("Admin"))
                {
                    var adminCount = await _userRepository.CountUsersInRoleAsync("Admin", cancellationToken);
                    if (adminCount <= 1)
                    {
                        throw new DomainException("Неможливо видалити роль адміністратора, оскільки це останній адміністратор у системі.");
                    }
                }
            }

            // 5. Оновлення ролей
            await _userRepository.ClearUserRolesAsync(request.UserId, cancellationToken);
            await _userRepository.AddUserRoleAsync(request.UserId, role.Id, cancellationToken);

            // 6. Синхронізація через RabbitMQ
            await _publishEndpoint.Publish(new UserRoleChangedEvent(
                request.UserId, 
                request.RoleName, 
                user.Email.Value, 
                user.GetFullName()), cancellationToken);
        }
    }
}
