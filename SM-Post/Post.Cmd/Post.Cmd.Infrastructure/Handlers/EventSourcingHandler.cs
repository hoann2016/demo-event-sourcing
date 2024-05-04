using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domain;
using CQRS.Core.Handler;
using CQRS.Core.Infrastructure;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Infrastructure.Handlers
{
    public class EventSourcingHandler : IEventSourcingHandler<PostAggregate>
    {
        public EventSourcingHandler(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }
        
        private readonly IEventStore _eventStore;
        public async Task<PostAggregate> GetByIdAsync(Guid aggregateId)
        
        {
            var aggregate = new PostAggregate();
            var events=await _eventStore.GetEventsAsync(aggregateId);
            if(events==null || !events.Any())
            {
                return aggregate;
            }
            aggregate.ReplayEvents(events);
            var lastEvent = events.Select(x=>x.Version).Max();
            aggregate.Version = lastEvent;
            return aggregate;
        }

        public async Task SaveAsync(AggregateRoot aggregateRoot)
        {
            await _eventStore.SaveEventsAsync(aggregateRoot.Id, aggregateRoot.GetUncommittedChanges(), aggregateRoot.Version);
            aggregateRoot.MarkChangesAsCommitted();
        }
    }
}