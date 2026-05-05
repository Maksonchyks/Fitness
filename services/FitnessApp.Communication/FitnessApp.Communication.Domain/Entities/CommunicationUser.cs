using System;
using FitnessApp.Communication.Domain.Common;

namespace FitnessApp.Communication.Domain.Entities
{
    public class CommunicationUser : Entity
    {
        public string Username { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string FullName { get; private set; } = string.Empty;
        public string? Role { get; set; } // Role can be updated dynamically
        public bool IsActive { get; private set; } = true;
        public bool IsOnline { get; private set; } = false;

        private CommunicationUser() { } // For EF

        public CommunicationUser(Guid id, string username, string email, string fullName, string? role = null) : base(id)
        {
            Guard.AgainstNullOrEmpty(username, nameof(username));
            Guard.AgainstNullOrEmpty(email, nameof(email));

            Username = username;
            Email = email;
            FullName = fullName;
            Role = role;
            IsActive = true;
        }

        public void UpdateInfo(string username, string email, string fullName)
        {
            Guard.AgainstNullOrEmpty(username, nameof(username));
            Guard.AgainstNullOrEmpty(email, nameof(email));
            
            Username = username;
            Email = email;
            FullName = fullName;
        }

        public void SetOnlineStatus(bool isOnline)
        {
            IsOnline = isOnline;
        }

        public void SetActiveStatus(bool isActive)
        {
            IsActive = isActive;
        }
    }
}
