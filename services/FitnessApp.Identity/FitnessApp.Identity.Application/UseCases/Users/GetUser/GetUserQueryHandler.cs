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
        private readonly IRedisCacheService _cacheService;
        public GetUserQueryHandler(
            IUserRepository userRepository,
            ICurrentUserService currentUserService,
            IMapper mapper,
            ILogger<GetUserQueryHandler> logger,
            IRedisCacheService cacheService)
        {
            _userRepository = userRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _logger = logger;
            _cacheService = cacheService;
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

            string cacheKey = $"user_profile_{request.UserId}";

            var cachedUser = await _cacheService.GetAsync<UserResponse>(cacheKey, cancellationToken);

            if (cachedUser != null)
            {
                return cachedUser;
            }

            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("User not found: {UserId}", request.UserId);
                throw new NotFoundException("User", request.UserId);
            }

            _logger.LogInformation("User profile retrieved: {UserId}", user.Id);
            
            var response = _mapper.Map<UserResponse>(user);
            
            await _cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(10), cancellationToken);

            return response;
        }
    }
}
