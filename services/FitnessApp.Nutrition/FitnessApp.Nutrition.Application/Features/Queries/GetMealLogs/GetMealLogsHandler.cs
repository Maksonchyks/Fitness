using AutoMapper;
using FitnessApp.Nutrition.Application.DTOs;
using FitnessApp.Nutrition.Application.Interfaces;
using FitnessApp.Nutrition.Domain.Interfaces.Persistence;
using MediatR;

namespace FitnessApp.Nutrition.Application.Features.Queries.GetMealLogs
{
    public class GetMealLogsHandler : IRequestHandler<GetMealLogsQuery, List<MealLogResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public GetMealLogsHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<List<MealLogResponse>> Handle(GetMealLogsQuery request, CancellationToken ct)
        {
            if (!_currentUserService.IsAuthenticated)
                throw new UnauthorizedAccessException();

            var userId = _currentUserService.UserId!.Value;
            var date = request.Date ?? DateTime.UtcNow.Date;

            var meals = await _unitOfWork.MealLogs.GetByUserIdAndDateAsync(userId, date, ct);
            return meals.Select(m => _mapper.Map<MealLogResponse>(m)).ToList();
        }
    }
}
