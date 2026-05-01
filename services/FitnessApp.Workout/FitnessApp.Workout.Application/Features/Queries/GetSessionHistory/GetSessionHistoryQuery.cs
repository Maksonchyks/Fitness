using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FitnessApp.Workout.Application.DTOs;
using FitnessApp.Workout.Application.Interfaces;
using FitnessApp.Workout.Domain.Interfaces.Persistence;
using MediatR;

namespace FitnessApp.Workout.Application.Features.Queries.GetSessionHistory
{
    public record GetSessionHistoryQuery : IRequest<IEnumerable<WorkoutSessionDto>>;

    public class GetSessionHistoryQueryHandler : IRequestHandler<GetSessionHistoryQuery, IEnumerable<WorkoutSessionDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public GetSessionHistoryQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<WorkoutSessionDto>> Handle(GetSessionHistoryQuery request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated)
                throw new System.UnauthorizedAccessException();

            var sessions = await _unitOfWork.Sessions.GetByUserIdAsync(_currentUserService.UserId!.Value, cancellationToken);
            
            return _mapper.Map<IEnumerable<WorkoutSessionDto>>(sessions);
        }
    }
}
