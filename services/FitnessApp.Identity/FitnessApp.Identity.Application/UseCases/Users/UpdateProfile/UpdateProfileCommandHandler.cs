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

namespace FitnessApp.Identity.Application.UseCases.Users.UpdateProfile
{
    public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, UserResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateProfileCommandHandler> _logger;

        public UpdateProfileCommandHandler(
            IUserRepository userRepository,
            ICurrentUserService currentUserService,
            IMapper mapper,
            ILogger<UpdateProfileCommandHandler> logger)
        {
            _userRepository = userRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<UserResponse> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating profile for user: {UserId}", request.UserId);

            if (_currentUserService.UserId != request.UserId && !_currentUserService.IsInRole("Admin"))
            {
                _logger.LogWarning("Unauthorized update attempt for user {UserId} by user {CurrentUserId}",
                    request.UserId, _currentUserService.UserId);
                throw new ForbiddenException("You don't have permission to update this profile");
            }

            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("User not found: {UserId}", request.UserId);
                throw new NotFoundException("User", request.UserId);
            }

            var gender = ParseGender(request.Gender);
            var fitnessGoal = ParseFitnessGoal(request.FitnessGoal);

            user.UpdateProfile(
                request.FirstName,
                request.LastName,
                request.DateOfBirth,
                gender,
                fitnessGoal,
                request.ProfileImageUrl);

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Profile updated for user: {UserId}", user.Id);

            return _mapper.Map<UserResponse>(user);
        }

        private Gender? ParseGender(string? gender)
        {
            if (string.IsNullOrWhiteSpace(gender))
                return null;

            return Enum.TryParse<Gender>(gender, true, out var result) ? result : null;
        }

        private FitnessGoal? ParseFitnessGoal(string? fitnessGoal)
        {
            if (string.IsNullOrWhiteSpace(fitnessGoal))
                return null;

            return Enum.TryParse<FitnessGoal>(fitnessGoal, true, out var result) ? result : null;
        }
    }
}
