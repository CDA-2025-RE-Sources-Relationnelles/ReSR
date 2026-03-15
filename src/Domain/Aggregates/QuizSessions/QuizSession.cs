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
        public ICollection<QuizParticipation> Participations { get; internal set; } = [];

    #endregion
    #region CONSTRUCTORS

        public static QuizSession Create(
            QuizResource      quizResource,
            IEnumerable<User> participants
        ) => new() {
            Resource       = quizResource,
            Participations = [.. participants.ToHashSet().Select(x => new QuizParticipation { User = x }) ],
        };

    #endregion
    #region METHODS
            
        /// <returns> A copy of the session with the given participation score if the user is a participant. </returns>
        public virtual IResponse<QuizSession> TryWithScore(User user, int value) {
            if (this.Participations.FirstOrDefault(x => x.User.Id == user.Id) is QuizParticipation participation) {
                if (participation.Score is not null) return Response.Failure<QuizSession>(new InvariantException("Une seule participation est possible par utilisateur !"));

                List<QuizParticipation> participations = [.. this.Participations];
                participations.Remove(participations.First(x => x.User.Id == user.Id));
                participations.Add(new () { User = user, Score = value });
                this.Participations = participations;
                return Response.Success(this);

            } else return Response.Failure<QuizSession>(new InvariantException("Seuls les participants invités à une session peuvent y participer !"));
        }

        public IEnumerable<IDomainEvent> DomainEvents { get; protected init; } = [];
        public QuizSession WithConsumedEvents(out IEnumerable<IDomainEvent> domainEvents) {
            domainEvents = this.DomainEvents;
            return this with { DomainEvents = [] };
        }

    #endregion
    
}