using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FitnessApp.Workout.Application.DTOs;
using FitnessApp.Workout.Domain.Interfaces.Persistence;
using MediatR;

namespace FitnessApp.Workout.Application.Features.Queries.GetTrainingDay
{
    public class GetTrainingDayHandler : IRequestHandler<GetTrainingDayQuery, TrainingDayDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetTrainingDayHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<TrainingDayDto> Handle(GetTrainingDayQuery request, CancellationToken ct)
        {
            var program = await _unitOfWork.Programs.GetByIdAsync(request.ProgramId, ct);
            var day = program?.TrainingDays.FirstOrDefault(d => d.DayNumber == request.DayNumber);
            return _mapper.Map<TrainingDayDto>(day);
        }
    }
}
