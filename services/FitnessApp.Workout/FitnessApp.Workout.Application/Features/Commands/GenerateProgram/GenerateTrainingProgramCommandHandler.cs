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

            var powerMetrics = request.PowerMetrics;

            if (powerMetrics == null)
            {
                var sessions = await _unitOfWork.Sessions.GetByUserIdAsync(userId, cancellationToken);
                var recentSessions = sessions.Where(s => s.Date >= DateTime.UtcNow.AddDays(-14)).ToList();
                
                if (recentSessions.Any())
                {
                    float squatWeight = GetMaxWeight(recentSessions, FitnessApp.Workout.Domain.Enums.ExerciseType.BackSquat) ?? 0f;
                    float benchWeight = GetMaxWeight(recentSessions, FitnessApp.Workout.Domain.Enums.ExerciseType.BenchPress) ?? 0f;
                    float deadliftWeight = GetMaxWeight(recentSessions, FitnessApp.Workout.Domain.Enums.ExerciseType.DeadLift) ?? 0f;

                    if (request.FitnessGoal == FitnessApp.Workout.Domain.Enums.FitnessGoal.Powerlifting)
                    {
                        if (squatWeight > 0) squatWeight += 5.0f;
                        if (benchWeight > 0) benchWeight += 2.5f;
                        if (deadliftWeight > 0) deadliftWeight += 5.0f;
                    }
                    else 
                    {
                        if (squatWeight > 0) squatWeight += 2.5f;
                        if (benchWeight > 0) benchWeight += 2.5f;
                        if (deadliftWeight > 0) deadliftWeight += 2.5f;
                    }

                    if (squatWeight > 0 || benchWeight > 0 || deadliftWeight > 0)
                    {
                        powerMetrics = new PowerMetrics(
                            squatWeight > 0 ? squatWeight : 40f, 
                            benchWeight > 0 ? benchWeight : 40f, 
                            deadliftWeight > 0 ? deadliftWeight : 40f
                        );
                    }
                }
                
                // Default if still null
                if (powerMetrics == null)
                {
                    powerMetrics = new PowerMetrics(40f, 40f, 40f);
                }
            }

            var profile = new ProgramProfile(
                userId,
                request.FitnessGoal,
                request.Intensity,
                powerMetrics
            );

            var generator = _factory.GetGenerator(profile.Goal);
            var program = generator.Generate(profile);

            await _unitOfWork.Programs.AddAsync(program, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _mapper.Map<TrainingProgramResponse>(program);
        }

        private float? GetMaxWeight(IEnumerable<WorkoutSession> sessions, FitnessApp.Workout.Domain.Enums.ExerciseType exerciseType)
        {
            var exercises = sessions
                .SelectMany(s => s.PerformedExercises)
                .Where(e => e.ExerciseType == exerciseType)
                .ToList();

            if (!exercises.Any())
                return null;

            // Epley Formula: 1RM = Weight * (1 + 0.0333 * Reps)
            var max1Rm = exercises.Max(e => e.Weight * (1 + 0.0333f * e.Reps));

            // Round to nearest 2.5
            return (float)(Math.Round(max1Rm / 2.5) * 2.5);
        }
    }
}
