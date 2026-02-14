using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Workout.Domain.Common;
using FitnessApp.Workout.Domain.Enums;
using FitnessApp.Workout.Domain.Exceptions;
using FitnessApp.Workout.Domain.ValueObjects;

namespace FitnessApp.Workout.Domain.Entities
{
    public class TrainingProgram : AggregateRoot
    {
        public Guid UserId { get; private set; }
        public ProgramProfile ProgramProfile { get; private set; }
        public DateTime CreatedOn { get; private set; }

        private readonly List<TrainingDay> _trainingDays = new();
        public IReadOnlyCollection<TrainingDay> TrainingDays => _trainingDays.AsReadOnly();

        private TrainingProgram(Guid userId, ProgramProfile programProfile) : base()
        {
            UserId = userId;
            ProgramProfile = programProfile;
            CreatedOn = DateTime.UtcNow;
        }
        public static TrainingProgram Create(Guid userId, ProgramProfile ProgramProfile, List<TrainingDay> days)
        {
            if (userId == Guid.Empty) throw new DomainException("UserId is required");
            if (ProgramProfile == null) throw new DomainException("ProgramProfile is required");

            var program = new TrainingProgram(userId, ProgramProfile);

            foreach (var day in days)
            {
                program.AddTrainingDay(day);
            }

            return program;
        }
        public static TrainingProgram CreateEmpty(Guid userId, ProgramProfile ProgramProfile)
        {
            if (userId == Guid.Empty) throw new DomainException("UserId is required");
            if (ProgramProfile == null) throw new DomainException("ProgramProfile is required");

            var program = new TrainingProgram(userId, ProgramProfile);

            return program;
        }

        public void AddTrainingDay(TrainingDay day)
        {
            if (day == null) throw new DomainException("Day cannot be null");

            if (day.TrainingProgramId != this.Id)
                throw new DomainException("Day belongs to another program");

            if (_trainingDays.Any(d => d.DayNumber == day.DayNumber))
                throw new DomainException($"Day with number {day.DayNumber} already exists");

            _trainingDays.Add(day);
        }
    }
}
