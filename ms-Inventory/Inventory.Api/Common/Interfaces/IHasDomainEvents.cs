using System.Collections.Generic;
using MediatR;

namespace Inventory.Api.Domain.Interfaces
{
    public interface IHasDomainEvents
    {
        public List<INotification> DomainEvents { get; }
        void AddDomainEvent(INotification ev);
        void ClearDomainEvents();
    }
}
