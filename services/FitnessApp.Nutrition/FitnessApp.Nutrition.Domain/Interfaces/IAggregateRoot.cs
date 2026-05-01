namespace FitnessApp.Nutrition.Domain.Interfaces
{
    public interface IAggregateRoot : IEntity
    {
        IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
    }
}
