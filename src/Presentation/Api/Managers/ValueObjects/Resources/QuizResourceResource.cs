using ReSR.Domain.Aggregates.Resources;
using ReSR.Domain.Aggregates.Resources.ValueObjects;
using ReSR.Presentation.Api.Managers.Controllers;
using ReSR.Presentation.Api.Core.ValueObjects;

namespace ReSR.Presentation.Api.Managers.ValueObjects.Resources;
public class QuizResourceResource(QuizResource from) : ResourceResource(from), IResource<QuizResourceResource, QuizResource> {

    #region PROPERTIES

        public IEnumerable<QuizQuestion> Questions { get; } = from.Questions;

    #endregion
    #region METHODS

        public static implicit operator QuizResourceResource(QuizResource from) => new (from);

        public static QuizResourceResource From(QuizResource from) => from;
        public static IEnumerable<QuizResourceResource> From(IEnumerable<QuizResource> from) => from.Select(From);

        public static AnnotatedLink GetLink(QuizResource from) => new (Core.ValueObjects.HttpMethod.GET, from.Title, QuizResourceController.ROUTE, from.Id);
        public static IEnumerable<AnnotatedLink> GetLinks(IEnumerable<QuizResource> from) => from.Select(GetLink);
        
    #endregion

}