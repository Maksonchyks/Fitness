using FitnessApp.Communication.Application.Interfaces;
using FitnessApp.Communication.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FitnessApp.Communication.Application.Features.Notifications.Queries.GetHistory
{
    public record GetHistoryQuery(Guid UserId) : IRequest<List<UserNotification>>;

    public class GetHistoryQueryHandler : IRequestHandler<GetHistoryQuery, List<UserNotification>>
    {
        private readonly INotificationRepository _notificationRepository;

        public GetHistoryQueryHandler(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<List<UserNotification>> Handle(GetHistoryQuery request, CancellationToken cancellationToken)
        {
            return await _notificationRepository.GetUserHistoryAsync(request.UserId, cancellationToken);
        }
    }
}
