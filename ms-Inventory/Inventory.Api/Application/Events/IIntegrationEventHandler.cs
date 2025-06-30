
using System.Threading.Tasks;
using Inventory.Api.Application.Events.Common;

namespace Inventory.Api.Application.Events
{
    /// <summary>
    /// Handler para un IntegrationEvent.
    /// </summary>
    public interface IIntegrationEventHandler<in TEvent>
        where TEvent : IntegrationEvent
    {
        Task Handle(TEvent @event);
    }
}
