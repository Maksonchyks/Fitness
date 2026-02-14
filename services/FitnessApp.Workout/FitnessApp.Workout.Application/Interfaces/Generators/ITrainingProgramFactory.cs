using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Workout.Domain.Enums;

namespace FitnessApp.Workout.Application.Interfaces.Generators
{
    public interface ITrainingProgramFactory
    {
        ITrainingProgramGenerator GetGenerator(FitnessGoal goal);
    }
}
