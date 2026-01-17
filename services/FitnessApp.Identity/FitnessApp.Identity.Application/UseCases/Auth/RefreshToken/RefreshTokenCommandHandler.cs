using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FitnessApp.Identity.Application.Common.Exceptions;
using FitnessApp.Identity.Application.DTOs;
using FitnessApp.Identity.Application.Interfaces;
using FitnessApp.Identity.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FitnessApp.Identity.Application.UseCases.Auth.RefreshToken
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, TokenResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly ILogger<RefreshTokenCommandHandler> _logger;

        public RefreshTokenCommandHandler(
            IUnitOfWork unitOfWork,
            IRefreshTokenRepository refreshTokenRepository,
            IUserRepository userRepository,
            ITokenService tokenService,
            IJwtService jwtService,
            IMapper mapper,
            ILogger<RefreshTokenCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _refreshTokenRepository = refreshTokenRepository;
            _userRepository = userRepository;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<TokenResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Refresh token attempt");

            // Пошук refresh token
            var refreshToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);

            if (refreshToken == null)
            {
                _logger.LogWarning("Refresh token not found: {Token}", request.RefreshToken);
                throw new UnauthorizedException("Invalid refresh token");
            }

            if (refreshToken.IsRevoked)
            {
                _logger.LogWarning("Refresh token revoked: {Token}", request.RefreshToken);

                if (!string.IsNullOrEmpty(refreshToken.ReplacedByToken))
                {
                    await _tokenService.RevokeRefreshTokenAsync(
                        refreshToken.ReplacedByToken,
                        request.IpAddress ?? string.Empty,
                        "Revoked due to token reuse");
                }

                throw new UnauthorizedException("Token has been revoked");
            }

            if (refreshToken.IsExpired)
            {
                _logger.LogWarning("Refresh token expired: {Token}", request.RefreshToken);
                throw new UnauthorizedException("Token has expired");
            }

            var user = await _userRepository.GetByIdAsync(refreshToken.UserId, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("User not found for refresh token: {UserId}", refreshToken.UserId);
                throw new NotFoundException("User", refreshToken.UserId);
            }

            refreshToken.Revoke(
                request.IpAddress ?? string.Empty,
                reason: "Replaced by new token");

            _refreshTokenRepository.Update(refreshToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var (accessToken, newRefreshToken, expiresAt) = await _tokenService.GenerateTokensAsync(user);

            _logger.LogInformation("Refresh token successful for user: {UserId}", user.Id);

            return new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = expiresAt
            };
        }
    }
}
