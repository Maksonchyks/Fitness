using System;
using System.Collections.Generic;

namespace FitnessApp.Communication.Domain.Common
{
    public abstract class Entity
    {
        public Guid Id { get; protected set; }

        protected Entity() => Id = Guid.NewGuid();
        protected Entity(Guid id) => Id = id;

        private readonly List<IDomainEvent> _domainEvents = new();
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        public void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
        public void ClearDomainEvents() => _domainEvents.Clear();
    }

    public interface IDomainEvent { }

    public abstract class AggregateRoot : Entity
    {
        protected AggregateRoot() : base() { }
        protected AggregateRoot(Guid id) : base(id) { }
    }
}
