using CQRS.Core.Domain;
using CQRS.Core.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Post.Cmd.Infrastructure.Config;

namespace Post.Cmd.Infrastructure.Repositories
{
    public class EventStoreRepository : IEventStoreRepository
    {
        private readonly IMongoCollection<EventModel> _eventCollection;
        private readonly ILogger<EventStoreRepository> _logger;

        public EventStoreRepository(IOptions<MongoDbConfig> config, ILogger<EventStoreRepository> logger)
        {
            _logger = logger;
            var uri = $"mongodb://{config.Value.UserName}:{config.Value.Password}@{config.Value.Host}:27017/admin";
            _logger.LogWarning($"URI: {uri}");

            var client = new MongoClient(uri);
            var database = client.GetDatabase(config.Value.Database);
            _eventCollection = database.GetCollection<EventModel>(config.Value.Collection);
        }

        public async Task<List<EventModel>> FindAllAsync()
        {
            return await _eventCollection.Find(_ => true).ToListAsync().ConfigureAwait(false);
        }

        public async Task<List<EventModel>> FindByAggregateId(Guid aggregateId)
        {
            return await _eventCollection.Find(x => x.AggregateIdentifier == aggregateId).ToListAsync()
                .ConfigureAwait(false);
        }

        public async Task SaveAsync(EventModel @event)
        {
            await _eventCollection.InsertOneAsync(@event).ConfigureAwait(false);
        }
    }
}