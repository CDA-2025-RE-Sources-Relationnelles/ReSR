using FluentResponse;
using FluentResponse.Interfaces;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Core;

namespace ReSR.Domain.Aggregates.Messages;

/// <summary>
/// A base message record.
/// </summary>
public abstract record Message<T>(Id Id = default) : IAggregateRoot<T> where T : Message<T> {

    #region PROPERTIES

        /// <summary> The message text content. </summary>
        public string Content { get; internal init; } = null!;

        /// <summary> The instant at which the message was sent. </summary>
        public DateTime SentAt { get; internal init; } = DateTime.UtcNow;

        /// <summary> The user sending the private message. </summary>
        public virtual User SentBy { get; internal init; } = null!;

    #endregion
    #region METHODS

        protected static IResponse TryVerifyContentInvariant(string value) =>
            !string.IsNullOrWhiteSpace(value)
            ? Response.Success()
            : Response.Failure(new InvariantException("Un message ne peut être vide !"));

        public IEnumerable<IDomainEvent> DomainEvents { get; protected init; } = [];
        public T WithConsumedEvents(out IEnumerable<IDomainEvent> domainEvents) {
            domainEvents = this.DomainEvents;
            return (T)this with { DomainEvents = [] };
        }

    #endregion
    
}