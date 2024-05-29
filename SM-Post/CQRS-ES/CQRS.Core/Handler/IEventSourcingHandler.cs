using CQRS.Core.Domain;

namespace CQRS.Core.Handler
{
    public interface IEventSourcingHandler<T>
    {
        Task SaveAsync(AggregateRoot aggregateRoot);
        Task<T> GetByIdAsync(Guid id);
        Task RepublishEventsAsync();
    }
}