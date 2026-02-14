using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Workout.Application.Interfaces.Generators;
using FitnessApp.Workout.Domain.Enums;
using FitnessApp.Workout.Domain.Exceptions;

namespace FitnessApp.Workout.Application.Services.Generators
{
    public class TrainingProgramFactory : ITrainingProgramFactory
    {
        private readonly IEnumerable<ITrainingProgramGenerator> _generators;

        public TrainingProgramFactory(IEnumerable<ITrainingProgramGenerator> generators)
        {
            _generators = generators;
        }

        public ITrainingProgramGenerator GetGenerator(FitnessGoal goal)
        {
            return _generators.FirstOrDefault(g => g.SupportedGoal == goal)
                   ?? throw new DomainException("Генератор для цієї цілі не знайдено");
        }
    }
}
