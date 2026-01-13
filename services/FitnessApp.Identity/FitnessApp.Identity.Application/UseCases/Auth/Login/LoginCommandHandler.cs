using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FitnessApp.Identity.Application.Common.Exceptions;
using FitnessApp.Identity.Application.DTOs;
using FitnessApp.Identity.Application.Interfaces;
using FitnessApp.Identity.Domain.Enums;
using FitnessApp.Identity.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FitnessApp.Identity.Application.UseCases.Auth.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly ILogger<LoginCommandHandler> _logger;

        public LoginCommandHandler(
            IUnitOfWork unitOfWork,
            IUserRepository userRepository,
            IPasswordService passwordService,
            ITokenService tokenService,
            IMapper mapper,
            ILogger<LoginCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _passwordService = passwordService;
            _tokenService = tokenService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Login attempt for email: {Email}", request.Email);

            var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("Login failed: User not found for email {Email}", request.Email);
                throw new UnauthorizedException("Invalid email or password");
            }

            if (user.Status == AccountStatus.Inactive)
            {
                _logger.LogWarning("Login failed: Account is inactive for user {UserId}", user.Id);
                throw new UnauthorizedException("Account is inactive");
            }

            var isPasswordValid = _passwordService.VerifyPassword(request.Password, user.Password);

            if (!isPasswordValid)
            {
                _logger.LogWarning("Login failed: Invalid password for user {UserId}", user.Id);
                throw new UnauthorizedException("Invalid email or password");
            }

            var (accessToken, refreshToken, expiresAt) = await _tokenService.GenerateTokensAsync(user);

            user.UpdateLastLogin();
            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Login successful for user: {UserId}", user.Id);

            var response = _mapper.Map<AuthResponse>(user);
            response.AccessToken = accessToken;
            response.RefreshToken = refreshToken;
            response.ExpiresAt = expiresAt;

            return response;
        }
    }
}
