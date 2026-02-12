using System.Text.Json.Serialization;
using ReSR.Domain.Aggregates.Messages;

namespace ReSR.Presentation.Api.Resources.Messages;
public abstract class MessageResource<T>(T from) where T : Message<T> {
    
    #region PROPERTIES
    
        [JsonIgnore]
        public Id Id { get; } = from.Id;

        public string Content { get; } = from.Content;
        public string SentAt  { get; } = from.SentAt.ToString();

    #endregion
}
