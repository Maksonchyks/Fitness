using AutoMapper;
using FitnessApp.Nutrition.Application.DTOs;
using FitnessApp.Nutrition.Application.Interfaces;
using FitnessApp.Nutrition.Domain.Interfaces.Persistence;
using MediatR;

namespace FitnessApp.Nutrition.Application.Features.Queries.GetWeightHistory
{
    public class GetWeightHistoryHandler : IRequestHandler<GetWeightHistoryQuery, List<WeightLogResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public GetWeightHistoryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<List<WeightLogResponse>> Handle(GetWeightHistoryQuery request, CancellationToken ct)
        {
            if (!_currentUserService.IsAuthenticated)
                throw new UnauthorizedAccessException();

            var userId = _currentUserService.UserId!.Value;
            var logs = await _unitOfWork.WeightLogs.GetByUserIdAsync(userId, request.Days, ct);
            return logs.Select(l => _mapper.Map<WeightLogResponse>(l)).ToList();
        }
    }
}
