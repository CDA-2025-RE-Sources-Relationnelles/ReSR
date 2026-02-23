using ReSR.Domain.Aggregates.Resources;
using ReSR.Domain.Aggregates.Resources.ValueObjects;
using ReSR.Presentation.Api.Users.Controllers;
using ReSR.Presentation.Api.Core.ValueObjects;
using ReSR.Presentation.Api.Managers.ValueObjects.Categories;
using ReSR.Presentation.Api.Users.ValueObjects.Accounts;

namespace ReSR.Presentation.Api.Users.ValueObjects.Resources;
public class QuizResourceResource(QuizResource from) : ResourceResource(from), IResource<QuizResourceResource, QuizResource> {

    #region PROPERTIES

        public string                    Content   { get; } = from.Content;
        public IEnumerable<QuizQuestion> Questions { get; } = from.Questions;

        public new ResourceLinks Links { get; } = new(
            Self     : GetLink(from),
            Category : CategoryResource.GetLink(from.Category),
            Owner    : from.Owner is not null ? UserPrivateResource.GetLink(from.Owner) : null,
            Comments : GetLink(from).WithSubRoute("comments"),
            Like     : GetLink(from).WithSubRoute("like").WithMethod(Core.ValueObjects.HttpMethod.POST),
            Bookmark : GetLink(from).WithSubRoute("bookmark").WithMethod(Core.ValueObjects.HttpMethod.POST),
            Exploit  : GetLink(from).WithSubRoute("exploit").WithMethod(Core.ValueObjects.HttpMethod.POST),
            StartSession  : GetLink(from).WithSubRoute("start-session").WithMethod(Core.ValueObjects.HttpMethod.POST)
        );

        public new readonly record struct ResourceLinks(
            AnnotatedLink  Self,
            AnnotatedLink  Category,
            AnnotatedLink? Owner,
            Link           Comments,
            Link           Like,
            Link           Bookmark,
            Link           Exploit,
            Link           StartSession
        );

    #endregion
    #region METHODS

        public static implicit operator QuizResourceResource(QuizResource from) => new (from);

        public static QuizResourceResource From(QuizResource from) => from;
        public static IEnumerable<QuizResourceResource> From(IEnumerable<QuizResource> from) => from.Select(From);

        public static AnnotatedLink GetLink(QuizResource from) => new (Core.ValueObjects.HttpMethod.GET, from.Title, QuizResourceController.ROUTE, from.Id);
        public static IEnumerable<AnnotatedLink> GetLinks(IEnumerable<QuizResource> from) => from.Select(GetLink);
        
    #endregion

}