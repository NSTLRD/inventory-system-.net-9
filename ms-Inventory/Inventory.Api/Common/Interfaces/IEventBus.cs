using System.Threading;
using System.Threading.Tasks;

namespace Inventory.Api.Common.Interfaces
{
    public interface IEventBus
    {
        Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
            where TEvent : class;
            
        void Subscribe<TEvent, THandler>()
            where TEvent : class
            where THandler : IEventHandler<TEvent>;
    }
    
    public interface IEventHandler<in TEvent> where TEvent : class
    {
        Task Handle(TEvent @event, CancellationToken cancellationToken = default);
    }
}