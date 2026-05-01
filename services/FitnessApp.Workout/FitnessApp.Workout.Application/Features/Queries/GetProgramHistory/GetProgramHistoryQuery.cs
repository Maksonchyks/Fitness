using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FitnessApp.Workout.Application.DTOs;
using FitnessApp.Workout.Application.Interfaces;
using FitnessApp.Workout.Domain.Interfaces.Persistence;
using MediatR;

namespace FitnessApp.Workout.Application.Features.Queries.GetProgramHistory
{
    public record GetProgramHistoryQuery : IRequest<IEnumerable<TrainingProgramResponse>>;

    public class GetProgramHistoryQueryHandler : IRequestHandler<GetProgramHistoryQuery, IEnumerable<TrainingProgramResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public GetProgramHistoryQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TrainingProgramResponse>> Handle(GetProgramHistoryQuery request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated)
                throw new System.UnauthorizedAccessException();

            var programs = await _unitOfWork.Programs.GetByUserIdAsync(_currentUserService.UserId!.Value, cancellationToken);
            
            return _mapper.Map<IEnumerable<TrainingProgramResponse>>(programs.OrderByDescending(p => p.CreatedOn));
        }
    }
}
