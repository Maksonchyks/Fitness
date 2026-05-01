using AutoMapper;
using FitnessApp.Nutrition.Application.DTOs;
using FitnessApp.Nutrition.Application.Interfaces;
using FitnessApp.Nutrition.Domain.Entities;
using FitnessApp.Nutrition.Domain.Interfaces.Persistence;
using MediatR;

namespace FitnessApp.Nutrition.Application.Features.Commands.LogWeight
{
    public class LogWeightCommandHandler : IRequestHandler<LogWeightCommand, WeightLogResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public LogWeightCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<WeightLogResponse> Handle(LogWeightCommand request, CancellationToken ct)
        {
            if (!_currentUserService.IsAuthenticated)
                throw new UnauthorizedAccessException();

            var userId = _currentUserService.UserId!.Value;
            var weightLog = WeightLog.Create(userId, request.Weight);

            await _unitOfWork.WeightLogs.AddAsync(weightLog, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return _mapper.Map<WeightLogResponse>(weightLog);
        }
    }
}
