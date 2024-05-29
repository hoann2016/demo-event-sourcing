using System.Text.Json;
using Confluent.Kafka;
using CQRS.Core.Events;
using CQRS.Core.Producers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Post.Cmd.Infrastructure.Producers
{
    public class EventProducer : IEventProducer
    {
        private readonly ProducerConfig _config;
        private readonly ILogger<EventProducer> _logger;

        public EventProducer(IOptions<ProducerConfig> config, ILogger<EventProducer> logger )
        {
            _config = config.Value;
            _logger = logger;
        }

        public async Task ProduceAsync<T>(string topic, T @event) where T : BaseEvent
        {
            using var producer = new ProducerBuilder<string, string>(_config)
                .SetKeySerializer(Serializers.Utf8)
                .SetValueSerializer(Serializers.Utf8)
                .Build();
            var eventMessage = new Message<string, string>
            {
                Key = Guid.NewGuid().ToString(),
                Value = JsonSerializer.Serialize(@event, @event.GetType())
            };
            _logger.LogInformation($"  topicName is :  {topic} ");
            var deliveryReport = await producer.ProduceAsync(topic, eventMessage);
            if (deliveryReport.Status == PersistenceStatus.NotPersisted)
            {
                throw new Exception(
                    $"Could not produce {@event.GetType().Name} to topic {topic} due to the following error: {deliveryReport.Message.Value}");
            }
        }
    }
}