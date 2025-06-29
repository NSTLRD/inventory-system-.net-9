using System.Collections.Generic;
using MediatR;

namespace Products.Api.Domain.Interfaces
{
    /// <summary>
    /// Marker for aggregate roots that raise domain events.
    /// </summary>
    public interface IHasDomainEvents
    {
        IReadOnlyCollection<INotification> DomainEvents { get; }
        void ClearDomainEvents();
    }
}
