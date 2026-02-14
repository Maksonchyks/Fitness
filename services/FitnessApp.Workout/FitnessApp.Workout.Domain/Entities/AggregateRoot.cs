using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Workout.Domain.Interfaces;

namespace FitnessApp.Workout.Domain.Entities
{
    public class AggregateRoot : Entity, IAggregateRoot
    {
        public AggregateRoot() :base()
        { }
    }
}
