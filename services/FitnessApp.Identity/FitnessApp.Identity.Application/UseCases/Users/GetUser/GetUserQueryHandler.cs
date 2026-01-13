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

namespace FitnessApp.Identity.Application.UseCases.Users.GetUser
{
    public class GetUserQueryHandler : IRequestHandler<GetUserQuery, UserResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly ILogger<GetUserQueryHandler> _logger;

        public GetUserQueryHandler(
            IUserRepository userRepository,
            ICurrentUserService currentUserService,
            IMapper mapper,
            ILogger<GetUserQueryHandler> logger)
        {
            _userRepository = userRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<UserResponse> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting user profile: {UserId}", request.UserId);

            if (_currentUserService.UserId != request.UserId && !_currentUserService.IsInRole("Admin"))
            {
                _logger.LogWarning("Unauthorized access attempt to user {UserId} by user {CurrentUserId}",
                    request.UserId, _currentUserService.UserId);
                throw new ForbiddenException("You don't have permission to access this user");
            }

            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("User not found: {UserId}", request.UserId);
                throw new NotFoundException("User", request.UserId);
            }

            _logger.LogInformation("User profile retrieved: {UserId}", user.Id);

            return _mapper.Map<UserResponse>(user);
        }
    }
}
