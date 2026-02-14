using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FitnessApp.Workout.Application.DTOs;
using FitnessApp.Workout.Domain.Interfaces.Persistence;
using MediatR;

namespace FitnessApp.Workout.Application.Features.Queries.GetActiveProgram
{
    public class GetActiveProgramHandler : IRequestHandler<GetActiveProgramQuery, TrainingProgramResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetActiveProgramHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<TrainingProgramResponse> Handle(GetActiveProgramQuery request, CancellationToken ct)
        {
            var programs = await _unitOfWork.Programs.GetByUserIdAsync(request.UserId, ct);
            var activeProgram = programs.OrderByDescending(p => p.CreatedOn).FirstOrDefault();
            return _mapper.Map<TrainingProgramResponse>(activeProgram);
        }
    }
}
