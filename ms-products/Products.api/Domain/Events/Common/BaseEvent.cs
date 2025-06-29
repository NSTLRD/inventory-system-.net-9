using System;
using MediatR;

namespace Products.Api.Domain.Events.Common
{
    public abstract class BaseEvent : INotification
    {
        public Guid Id { get; }
        public DateTime CreationDate { get; }

        protected BaseEvent()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
        }
    }
}