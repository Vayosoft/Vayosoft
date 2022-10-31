using Vayosoft.Commons.Entities;
using Vayosoft.Commons.Events;

namespace Vayosoft.Commons.Aggregates
{
    public interface IAggregate: IAggregate<Guid>
    { }

    public interface IAggregate<out TKey> : IAggregateRoot, IEntity<TKey>
    {
        int Version { get; }

        IEvent[] DequeueUncommittedEvents();
    }
}
