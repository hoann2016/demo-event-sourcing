using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domain;
using CQRS.Core.Handler;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producers;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Infrastructure.Handlers
{
    public class EventSourcingHandler : IEventSourcingHandler<PostAggregate>
    {
        public EventSourcingHandler(IEventStore eventStore,IEventProducer eventProducer)
        {
            _eventStore = eventStore;
            _eventProducer=eventProducer;
        }

        private readonly IEventStore _eventStore;
        private readonly IEventProducer _eventProducer;

        public async Task<PostAggregate> GetByIdAsync(Guid aggregateId)
        {
            var aggregate = new PostAggregate();
            var events = await _eventStore.GetEventsAsync(aggregateId);
            if (events == null || !events.Any())
            {
                return aggregate;
            }

            aggregate.ReplayEvents(events);
            var lastEvent = events.Select(x => x.Version).Max();
            aggregate.Version = lastEvent;
            return aggregate;
        }

        public async Task SaveAsync(AggregateRoot aggregateRoot)
        {
            await _eventStore.SaveEventsAsync(aggregateRoot.Id, aggregateRoot.GetUncommittedChanges(),
                aggregateRoot.Version);
            aggregateRoot.MarkChangesAsCommitted();
        }

        public async Task RepublishEventsAsync()
        {
            var aggregateIds = await _eventStore.GetAggregateIdsAsync();
            if (aggregateIds == null || !aggregateIds.Any())
            {
                return;
            }

            foreach (var aggregateId in aggregateIds)
            {
                var aggregate = await GetByIdAsync(aggregateId);

                if (aggregate == null || !aggregate.Active)
                {
                    continue;
                }
                var events = await _eventStore.GetEventsAsync(aggregateId);
                foreach (var @event in events)
                {
                    var topic =Environment.GetEnvironmentVariable("KAFKA_TOPIC");
                    await _eventProducer.ProduceAsync(topic, @event);

                }
            }
        }
    }
}