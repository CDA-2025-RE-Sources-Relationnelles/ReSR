using System.Text.Json.Serialization;
using ReSR.Domain.Aggregates.QuizSessions;
using ReSR.Presentation.Api.Users.Controllers;
using ReSR.Presentation.Api.Core.ValueObjects;
using ReSR.Presentation.Api.Users.ValueObjects.Accounts;
using ReSR.Presentation.Api.Users.ValueObjects.Resources;

namespace ReSR.Presentation.Api.Users.ValueObjects.QuizSessions;
public class QuizSessionResource(QuizSession from) : IResource<QuizSessionResource, QuizSession> {

    #region PROPERTIES

        [JsonIgnore]
        public Id                             Id             { get; } = from.Id;
        public string                         OpenedAt       { get; } = from.OpenedAt.ToString();
        public IEnumerable<QuizParticipation> Participations { get; } = from.Participations
            .OrderByDescending(x => x.Score)
            .Select(x => new QuizParticipation(
                Score : x.Score,
                User  : UserPrivateResource.GetLink(x.User)
            ));

        public QuizSessionLinks Links { get; } = new(
            Self        : GetLink(from),
            Resource    : ResourceResource.GetLink(from.Resource),
            Participate : GetLink(from).WithSubRoute("participate").WithMethod(Core.ValueObjects.HttpMethod.POST)
        );

        public readonly record struct QuizSessionLinks(
            Link          Self,
            AnnotatedLink Resource,
            Link          Participate
        );

        public readonly record struct QuizParticipation(
            int?          Score,
            AnnotatedLink User
        );

    #endregion
    #region METHODS

        public static implicit operator QuizSessionResource(QuizSession from) => new (from);

        public static QuizSessionResource From(QuizSession from) => from;
        public static IEnumerable<QuizSessionResource> From(IEnumerable<QuizSession> from) => from.Select(From);

        public static Link GetLink(QuizSession from) => new (Core.ValueObjects.HttpMethod.GET, QuizSessionController.ROUTE, from.Id);
        public static IEnumerable<Link> GetLinks(IEnumerable<QuizSession> from) => from.Select(GetLink);
        
    #endregion

}