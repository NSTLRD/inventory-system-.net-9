using System;
using System.Collections.Generic;

namespace Products.Api.Domain.Entities
{
    public abstract class Entity
    {
        private List<object>? _domainEvents;

        public IReadOnlyCollection<object> DomainEvents
            => _domainEvents != null
               ? (IReadOnlyCollection<object>) _domainEvents.AsReadOnly()
               : Array.Empty<object>();

        protected void AddDomainEvent(object @event)
        {
            _domainEvents ??= new List<object>();
            _domainEvents.Add(@event);
        }

        public void ClearDomainEvents() => _domainEvents?.Clear();
    }
}
