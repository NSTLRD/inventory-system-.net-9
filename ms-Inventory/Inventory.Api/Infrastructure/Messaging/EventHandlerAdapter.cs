using System.Threading;
using System.Threading.Tasks;
using Inventory.Api.Application.Events;
using Inventory.Api.Application.Events.Common;
using Inventory.Api.Common.Interfaces;

namespace Inventory.Api.Infrastructure.Messaging
{
    public class IntegrationEventHandlerAdapter<TEvent> : IEventHandler<TEvent>
        where TEvent : IntegrationEvent
    {
        private readonly IIntegrationEventHandler<TEvent> _handler;

        public IntegrationEventHandlerAdapter(IIntegrationEventHandler<TEvent> handler)
        {
            _handler = handler;
        }

        public async Task Handle(TEvent @event, CancellationToken cancellationToken = default)
        {
            await _handler.Handle(@event);
        }
    }
}