using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FitnessApp.Identity.Application.Common.Exceptions;
using FitnessApp.Identity.Application.DTOs;
using FitnessApp.Identity.Application.Interfaces;
using FitnessApp.Identity.Domain.Entities;
using FitnessApp.Identity.Domain.Enums;
using FitnessApp.Identity.Domain.Interfaces.Repositories;
using FitnessApp.Identity.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FitnessApp.Identity.Application.UseCases.Auth.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IPasswordService _passwordService;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<RegisterCommandHandler> _logger;

        public RegisterCommandHandler(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IPasswordService passwordService,
            ITokenService tokenService,
            IEmailService emailService,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<RegisterCommandHandler> logger)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _passwordService = passwordService;
            _tokenService = tokenService;
            _emailService = emailService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Registration attempt for email: {Email}", request.Email);

            if (await _userRepository.ExistsByEmailAsync(request.Email, cancellationToken))
                throw new ConflictException($"Email '{request.Email}' is already registered");

            if (await _userRepository.ExistsByUsernameAsync(request.Username, cancellationToken))
                throw new ConflictException($"Username '{request.Username}' is already taken");

            var email = Email.Create(request.Email);
            var password = _passwordService.CreatePassword(request.Password);

            var user = User.Create(
                email,
                request.Username,
                password,
                request.FirstName,
                request.LastName);

            if (request.DateOfBirth.HasValue || request.Gender != null || request.FitnessGoal != null)
            {
                user.UpdateProfile(
                    request.FirstName,
                    request.LastName,
                    request.DateOfBirth,
                    ParseGender(request.Gender),
                    ParseFitnessGoal(request.FitnessGoal));
            }

            var defaultRole = await _roleRepository.GetByNameAsync("User", cancellationToken);
            if (defaultRole == null) throw new NotFoundException("Role", "User");
            user.AddRole(defaultRole);

            await _userRepository.AddAsync(user, cancellationToken);

            var (accessToken, refreshToken, expiresAt) = await _tokenService.GenerateTokensAsync(user);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User and tokens saved successfully: {UserId}", user.Id);

            _ = _emailService.SendEmailConfirmationAsync(request.Email, "confirmation-token-placeholder");

            var response = _mapper.Map<AuthResponse>(user);
            response.AccessToken = accessToken;
            response.RefreshToken = refreshToken;
            response.ExpiresAt = expiresAt;

            return response;
        }

        private Gender? ParseGender(string? gender) =>
            Enum.TryParse<Gender>(gender, true, out var result) ? result : null;

        private FitnessGoal? ParseFitnessGoal(string? fitnessGoal) =>
            Enum.TryParse<FitnessGoal>(fitnessGoal, true, out var result) ? result : null;
    }
}
