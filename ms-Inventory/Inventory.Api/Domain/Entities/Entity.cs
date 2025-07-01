using System;
using System.Collections.Generic;
using MediatR;
using Inventory.Api.Common.Interfaces;

namespace Inventory.Api.Domain.Entities
{
    public abstract class Entity : IHasDomainEvents
    {
        private readonly List<INotification> _domainEvents = new();
        public List<INotification> DomainEvents => _domainEvents;

        public void AddDomainEvent(INotification ev) => _domainEvents.Add(ev);
        public void ClearDomainEvents() => _domainEvents.Clear();

        public Guid Id { get; protected set; }
        protected Entity()
        {
            Id = Guid.NewGuid();
        }
    }
}
