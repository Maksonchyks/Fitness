using FitnessApp.Nutrition.Domain.Interfaces;

namespace FitnessApp.Nutrition.Domain.Entities
{
    public class AggregateRoot : Entity, IAggregateRoot
    {
        public AggregateRoot() : base()
        { }
    }
}
