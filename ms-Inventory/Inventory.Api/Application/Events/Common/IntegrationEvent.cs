using System;
using MediatR;

namespace Inventory.Api.Application.Events.Common
{
    public abstract class IntegrationEvent : INotification
    {
        public Guid Id { get; }
        public DateTime CreationDate { get; }

        protected IntegrationEvent()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
        }
    }
}