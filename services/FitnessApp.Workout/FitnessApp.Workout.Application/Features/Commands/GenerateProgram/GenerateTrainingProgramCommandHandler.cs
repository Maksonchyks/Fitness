using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FitnessApp.Workout.Application.DTOs;
using FitnessApp.Workout.Application.Interfaces;
using FitnessApp.Workout.Application.Interfaces.Generators;
using FitnessApp.Workout.Domain.Entities;
using FitnessApp.Workout.Domain.Interfaces.Persistence;
using FitnessApp.Workout.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FitnessApp.Workout.Application.Features.Commands.GenerateProgram
{
    public class GenerateTrainingProgramCommandHandler : IRequestHandler<GenerateTrainingProgramCommand, TrainingProgramResponse>
    {
        private readonly ITrainingProgramFactory _factory;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GenerateTrainingProgramCommandHandler(
            ITrainingProgramFactory factory,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _factory = factory;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<TrainingProgramResponse> Handle(GenerateTrainingProgramCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated)
                throw new UnauthorizedAccessException();

            var userId = _currentUserService.UserId!.Value;

            var profile = new ProgramProfile(
                userId,
                request.FitnessGoal,
                request.Intensity,
                request.PowerMetrics
                );

            var generator = _factory.GetGenerator(profile.Goal);
            var program = generator.Generate(profile);

            await _unitOfWork.Programs.AddAsync(program, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _mapper.Map<TrainingProgramResponse>(program);
        }
    }
}
