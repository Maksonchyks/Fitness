using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FitnessApp.Workout.Application.Interfaces;
using FitnessApp.Workout.Domain.Entities;
using FitnessApp.Workout.Domain.Interfaces.Persistence;
using MediatR;

namespace FitnessApp.Workout.Application.Features.Commands.RecordSession
{
    public class RecordWorkoutSessionHandler : IRequestHandler<RecordWorkoutSessionCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public RecordWorkoutSessionHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<Guid> Handle(RecordWorkoutSessionCommand request, CancellationToken ct)
        {
            var userId = _currentUserService.UserId ?? throw new UnauthorizedAccessException();

            var results = _mapper.Map<List<ExerciseSet>>(request.Results);
            var session = WorkoutSession.Create(request.TrainingDayId, userId, results);

            await _unitOfWork.Sessions.AddAsync(session, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return session.Id;
        }
    }
}
