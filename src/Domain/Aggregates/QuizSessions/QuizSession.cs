using System.Collections.Immutable;
using FluentResponse;
using FluentResponse.Interfaces;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.QuizSessions.ValueObjects;
using ReSR.Domain.Aggregates.Resources;
using ReSR.Domain.Core;

namespace ReSR.Domain.Aggregates.QuizSessions;

/// <summary>
/// A resource category.
/// </summary>
public record QuizSession(Id Id = default) : IAggregateRoot<QuizSession> {

    #region PROPERTIES

        /// <summary> The instant at which the session was opened. </summary>
        public DateTime OpenedAt { get; internal init; } = DateTime.UtcNow;

        /// <summary> The quiz resource used for this session. </summary>
        public virtual QuizResource Resource { get; internal init; } = null!;

        /// <summary> The invited users participation. </summary>
        public ImmutableList<QuizParticipation> Participations { get; internal init; } = [];

    #endregion
    #region CONSTRUCTORS

        public static QuizSession Create(
            QuizResource      quizResource,
            IEnumerable<User> participants
        ) => new() {
            Resource       = quizResource,
            Participations = [.. participants.ToHashSet().Select(x => new QuizParticipation(User: x)) ],
        };

    #endregion
    #region METHODS
            
        /// <returns> A copy of the session with the given participation score if the user is a participant. </returns>
        public virtual IResponse<QuizSession> TryWithScore(User user, int value) =>
            this.Participations.FindIndex(x => x.User.Id == user.Id) is int index
                ? Response.Success(this with { Participations = this.Participations.SetItem(index, new (user, value)) })
                : Response.Failure<QuizSession>(new InvariantException("Seules les participants d'une session peuvent y participer !"));


        public IEnumerable<IDomainEvent> DomainEvents { get; protected init; } = [];
        public QuizSession WithConsumedEvents(out IEnumerable<IDomainEvent> domainEvents) {
            domainEvents = this.DomainEvents;
            return this with { DomainEvents = [] };
        }

    #endregion
    
}