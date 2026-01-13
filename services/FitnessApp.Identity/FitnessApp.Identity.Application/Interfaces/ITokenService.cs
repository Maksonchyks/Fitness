using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Identity.Domain.Entities;

namespace FitnessApp.Identity.Application.Interfaces
{
    public interface ITokenService
    {
        Task<(string AccessToken, string RefreshToken, DateTime ExpiresAt)> GenerateTokensAsync(User user);
        Task<RefreshToken> GenerateRefreshTokenAsync(Guid userId, string ipAddress);
        Task<bool> ValidateRefreshTokenAsync(string token, Guid userId);
        Task RevokeRefreshTokenAsync(string token, string ipAddress, string reason = "Revoked by user");
    }
}
