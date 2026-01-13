using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Identity.Application.Common.Exceptions;
using FitnessApp.Identity.Application.Interfaces;
using FitnessApp.Identity.Domain.Entities;
using FitnessApp.Identity.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Configuration;

namespace FitnessApp.Identity.Application.Services
{
    public class TokenService : ITokenService
    {
        private readonly IJwtService _jwtService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IDateTimeService _dateTimeService;
        private readonly IConfiguration _configuration;

        public TokenService(
            IJwtService jwtService,
            IRefreshTokenRepository refreshTokenRepository,
            IDateTimeService dateTimeService,
            IConfiguration configuration)
        {
            _jwtService = jwtService;
            _refreshTokenRepository = refreshTokenRepository;
            _dateTimeService = dateTimeService;
            _configuration = configuration;
        }

        public async Task<(string AccessToken, string RefreshToken, DateTime ExpiresAt)> GenerateTokensAsync(User user)
        {
            var accessToken = _jwtService.GenerateJwtToken(user);

            var refreshTokenValue = _jwtService.GenerateRefreshToken();
            var ipAddress = GetIpAddress();

            var refreshToken = RefreshToken.Create(
                user.Id,
                refreshTokenValue,
                GetRefreshTokenExpirationDays(),
                ipAddress);

            await _refreshTokenRepository.AddAsync(refreshToken);

            // Оновлення часу останнього входу
            user.UpdateLastLogin();

            return (accessToken, refreshTokenValue, _dateTimeService.UtcNow.AddMinutes(GetAccessTokenExpirationMinutes()));
        }

        public async Task<RefreshToken> GenerateRefreshTokenAsync(Guid userId, string ipAddress)
        {
            var refreshTokenValue = _jwtService.GenerateRefreshToken();

            var refreshToken = RefreshToken.Create(
                userId,
                refreshTokenValue,
                GetRefreshTokenExpirationDays(),
                ipAddress);

            await _refreshTokenRepository.AddAsync(refreshToken);

            return refreshToken;
        }

        public async Task<bool> ValidateRefreshTokenAsync(string token, Guid userId)
        {
            var refreshToken = await _refreshTokenRepository.GetByTokenAsync(token);

            if (refreshToken == null)
                return false;

            if (refreshToken.UserId != userId)
                return false;

            if (refreshToken.IsRevoked || refreshToken.IsExpired)
                return false;

            return true;
        }

        public async Task RevokeRefreshTokenAsync(string token, string ipAddress, string reason = "Revoked by user")
        {
            var refreshToken = await _refreshTokenRepository.GetByTokenAsync(token);

            if (refreshToken == null)
                throw new NotFoundException("RefreshToken", token);

            if (refreshToken.IsRevoked)
                throw new ConflictException("Token is already revoked");

            refreshToken.Revoke(ipAddress, reason: reason);
            _refreshTokenRepository.Update(refreshToken);
        }

        private int GetAccessTokenExpirationMinutes()
        {
            return _configuration.GetValue<int>("JwtSettings:AccessTokenExpirationMinutes", 60);
        }

        private int GetRefreshTokenExpirationDays()
        {
            return _configuration.GetValue<int>("JwtSettings:RefreshTokenExpirationDays", 7);
        }

        private string GetIpAddress()
        {
            // Це буде імплементовано в контролері
            return string.Empty;
        }
    }
}
