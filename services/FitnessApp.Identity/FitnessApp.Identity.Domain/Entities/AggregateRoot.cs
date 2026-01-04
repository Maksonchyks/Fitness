using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Identity.Domain.Interfaces;

namespace FitnessApp.Identity.Domain.Entities
{
    public abstract class AggregateRoot : Entity, IAggregateRoot
    {
        protected AggregateRoot() :base() { }
    }
}
