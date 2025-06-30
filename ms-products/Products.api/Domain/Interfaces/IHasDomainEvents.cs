using System.Collections.Generic;
using MediatR;

namespace Products.Api.Domain.Interfaces
{
    public interface IHasDomainEvents
    {
        IReadOnlyCollection<INotification> DomainEvents { get; }
        void ClearDomainEvents();
    }
}
