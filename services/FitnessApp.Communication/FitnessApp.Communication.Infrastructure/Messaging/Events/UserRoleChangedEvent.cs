using System;
using System.Text.Json.Serialization;

namespace FitnessApp.Identity.Application.Common.Events
{
    public record UserRoleChangedEvent(
        [property: JsonPropertyName("userId")] Guid UserId, 
        [property: JsonPropertyName("newRole")] string NewRole, 
        [property: JsonPropertyName("email")] string Email, 
        [property: JsonPropertyName("fullName")] string FullName);
}
