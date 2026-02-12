using System.Text.Json.Serialization;
using ReSR.Domain.Aggregates.QuizSessions;
using ReSR.Presentation.Api.Resources.Accounts;
using ReSR.Presentation.Api.Resources.Resources;

namespace ReSR.Presentation.Api.Resources.QuizSessions;
public class QuizSessionResource(QuizSession from) : IResource<QuizSessionResource, QuizSession> {

    #region PROPERTIES

        [JsonIgnore]
        public Id                             Id            { get; } = from.Id;
        public string                         OpenedAt      { get; } = from.OpenedAt.ToString();
        public IEnumerable<QuizParticipation> Participation { get; } = from.Participations.Select(x => new QuizParticipation(
            Score : x.Score,
            User  : UserResource.GetLink(x.User)
        ));

        public QuizSessionLinks Links { get; } = new(
            Self     : GetLink(from),
            Resource : ResourceResource.GetLink(from.Resource)
        );

        public readonly record struct QuizSessionLinks(
            ILink Self,
            ILink Resource
        );

        public readonly record struct QuizParticipation(
            int?  Score,
            ILink User
        );

    #endregion
    #region METHODS

        public static implicit operator QuizSessionResource(QuizSession from) => new (from);

        public static QuizSessionResource From(QuizSession from) => from;
        public static IEnumerable<QuizSessionResource> From(IEnumerable<QuizSession> from) => from.Select(From);

        public static ILink GetLink(QuizSession from) => new Link(HttpMethod.GET, QuizSessionController.ROUTE, from.Id);
        public static IEnumerable<ILink> GetLinks(IEnumerable<QuizSession> from) => from.Select(GetLink);
        
    #endregion

}