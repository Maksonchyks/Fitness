using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FitnessApp.Identity.Application.DTOs;
using FitnessApp.Identity.Domain.Interfaces.Repositories;
using MediatR;
using System.Linq;

namespace FitnessApp.Identity.Application.UseCases.Users.GetUsers
{
    public class GetUsersQuery : IRequest<List<UserResponse>>
    {
    }

    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, List<UserResponse>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetUsersQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<List<UserResponse>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetAllAsync(cancellationToken);
            return _mapper.Map<List<UserResponse>>(users);
        }
    }
}
