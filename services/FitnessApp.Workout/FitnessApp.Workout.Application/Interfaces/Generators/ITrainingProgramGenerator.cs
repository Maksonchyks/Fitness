using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Workout.Domain.Entities;
using FitnessApp.Workout.Domain.Enums;
using FitnessApp.Workout.Domain.ValueObjects;

namespace FitnessApp.Workout.Application.Interfaces.Generators
{
    public interface ITrainingProgramGenerator
    {
        FitnessGoal SupportedGoal { get; }
        TrainingProgram Generate(ProgramProfile profile);
    }
}
