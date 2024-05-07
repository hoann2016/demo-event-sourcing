using System.Text.Json;
using System.Text.Json.Serialization;
using CQRS.Core.Events;
using Post.Common.Events;

namespace Post.Query.Infrastructure.Converters
{
    public class EventJsonConverter : JsonConverter<BaseEvent>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsAssignableFrom(typeof(BaseEvent));
        }

        public override BaseEvent? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (!JsonDocument.TryParseValue(ref reader, out var document))
                throw new JsonException($"Failed to parse JSON document");
            if (!document.RootElement.TryGetProperty("Type", out var type))
                throw new JsonException($"Failed to get Type property");
            var typeDiscriinator = type.GetString();
            var json = document.RootElement.GetRawText();
            return typeDiscriinator switch
            {
                nameof(PostCreatedEvent) => JsonSerializer.Deserialize<PostCreatedEvent>(json),
                nameof(MessageUpdatedEvent) => JsonSerializer.Deserialize<MessageUpdatedEvent>(json),
                nameof(PostLikedEvent) => JsonSerializer.Deserialize<PostLikedEvent>(json),
                nameof(CommentAddedEvent) => JsonSerializer.Deserialize<CommentAddedEvent>(json),
                nameof(CommentUpdatedEvent) => JsonSerializer.Deserialize<CommentUpdatedEvent>(json),
                nameof(CommentRemovedEvent) => JsonSerializer.Deserialize<CommentRemovedEvent>(json),
                nameof(PostRemovedEvent) => JsonSerializer.Deserialize<PostRemovedEvent>(json),
                _ => throw new JsonException($"Unknown event type: {typeDiscriinator}")
            };
        }

        public override void Write(Utf8JsonWriter writer, BaseEvent value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}