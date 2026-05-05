using System;

namespace FitnessApp.Identity.Application.Common.Events
{
    public record UserRoleChangedEvent(Guid UserId, string NewRole, string Email, string FullName);
}
