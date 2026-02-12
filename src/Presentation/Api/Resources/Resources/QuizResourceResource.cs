using ReSR.Domain.Aggregates.Resources;
using ReSR.Domain.Aggregates.Resources.ValueObjects;

namespace ReSR.Presentation.Api.Resources.Resources;
public class QuizResourceResource(QuizResource from) : ResourceResource(from), IResource<QuizResourceResource, QuizResource> {

    #region PROPERTIES

        public string                    Content   { get; } = from.Content;
        public IEnumerable<QuizQuestion> Questions { get; } = from.Questions;

    #endregion
    #region METHODS

        public static implicit operator QuizResourceResource(QuizResource from) => new (from);

        public static QuizResourceResource From(QuizResource from) => from;
        public static IEnumerable<QuizResourceResource> From(IEnumerable<QuizResource> from) => from.Select(From);

        public static ILink GetLink(QuizResource from) => new AnnotatedLink(HttpMethod.GET, from.Title, QuizResourceController.ROUTE, from.Id);
        public static IEnumerable<ILink> GetLinks(IEnumerable<QuizResource> from) => from.Select(GetLink);
        
    #endregion

}