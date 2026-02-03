using System.Collections.Immutable;
using FluentResponse;
using FluentResponse.Interfaces;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Categories;
using ReSR.Domain.Aggregates.Resources.ValueObjects;
using ReSR.Domain.Core;

namespace ReSR.Domain.Aggregates.Resources;
public record QuizResource : Resource, IAggregateRoot<QuizResource> {

    #region PROPERTIES

        /// <summary> The resource's text content. </summary>
        public string Content { get; internal init; } = null!;

        /// <summary> The resource's quiz questions. </summary>
        public ImmutableList<QuizQuestion> Questions { get; internal init; } = [];

    #endregion
    #region CONSTRUCTORS

        public static IResponse<QuizResource> TryCreate(
            string                    title,
            Category                  category,
            IEnumerable<string>       tags,
            string                    content,
            IEnumerable<QuizQuestion> questions,
            bool                      isPrivate = true,
            User?                     owner     = null
        ) => TryVerifyTitleInvariant(title)
                .OnSuccess(() => TryVerifyTagsInvariant(tags))
                .OnSuccess(() => TryVerifyQuizQuestionsInvariant(questions))
                .OnSuccess(() => new QuizResource {
                    Title      = title,
                    Category   = category,
                    RawTags    = string.Join(';', tags.ToHashSet()),
                    Content    = content,
                    Questions  = [.. ProcessQuestions(questions)],
                    Owner      = owner,
                    Visibility = isPrivate
                        ? Visibility.Private 
                        : Visibility.WaitingForVerification
                });

    #endregion
    #region METHODS
            
        /// <returns> A copy of the resource with the given content. </returns>
        public virtual QuizResource WithContent(string value) =>
            this with { Content = value };

        /// <returns> A copy of the resource with the given additional question if valid. </returns>
        public virtual IResponse<QuizResource> WithNewQuestion(QuizQuestion value) =>
            TryVerifyQuizQuestionInvariant(value)
                .OnSuccess(() => this with { Questions = this.Questions.Add(ProcessQuestion(value)) })
                .OnSuccess(x => TryVerifyQuizQuestionsInvariant(x.Questions).OnSuccess(() => x));

        /// <returns> A copy of the resource with the given additional question if valid. </returns>
        public virtual IResponse<QuizResource> WithQuestion(int index, QuizQuestion value) =>
            TryVerifyQuizQuestionInvariant(value).OnSuccess(() => this with { Questions = this.Questions.SetItem(index, ProcessQuestion(value)) });

        /// <returns> A copy of the resource without the question at the given index. </returns>
        public virtual IResponse<QuizResource> WithoutQuestion(int index) =>
            Response.Success(this with { Questions = this.Questions.RemoveAt(index) })
                .OnSuccess(x => TryVerifyQuizQuestionsInvariant(x.Questions).OnSuccess(() => x));



        protected static IResponse TryVerifyQuizQuestionsInvariant(IEnumerable<QuizQuestion> values) {
            
            if (values.Count() is >= 1 and <= 20) {
                foreach (var question in values) {
                    var response = TryVerifyQuizQuestionInvariant(question);
                    if (response is IFailure) return response;
                }

                return Response.Success();
            } else return Response.Failure(new InvariantException("Un quiz devrait avoir entre 1 et 20 questions !"));
        }

        protected static IResponse TryVerifyQuizQuestionInvariant(QuizQuestion value) =>
            value.Answers.Count is >= 2 and <= 4 && value.Answers.Any(x => x.IsCorrect)
            ? Response.Success()
            : Response.Failure(new InvariantException("Une question devrait avoir entre 2 et 4 réponses, et au moins 1 réponse correcte !"));

        protected static QuizQuestion ProcessQuestion(QuizQuestion value) =>
            value with { Answers = [.. value.Answers.Select((x, index) => x with { Index = index + 1 })] };

        protected static IEnumerable<QuizQuestion> ProcessQuestions(IEnumerable<QuizQuestion> values) =>
            values.Select((x, index) => x with {
                Index   = index + 1,
                Answers = [.. x.Answers.Select((x, index) => x with {
                    Index = index + 1
                })]
            });

        #region OVERRIDES

            public new IResponse<QuizResource> TryWithTitle(string value) =>
                (IResponse<QuizResource>)base.TryWithTitle(value);

            public new IResponse<QuizResource> TryWithTag(string value) =>
                (IResponse<QuizResource>)base.TryWithTag(value);

            public new QuizResource WithoutTag(string value) =>
                (QuizResource)base.WithoutTag(value);

            public new QuizResource WithRelationships(Relationships value) =>
                (QuizResource)base.WithRelationships(value);

            public new QuizResource WithLikeFrom(User from, bool value) =>
                (QuizResource)base.WithLikeFrom(from, value);

            public new QuizResource WithBookmarkFrom(User from, bool value) =>
                (QuizResource)base.WithBookmarkFrom(from, value);

            public new QuizResource WithExploitFrom(User from, bool value) =>
                (QuizResource)base.WithExploitFrom(from, value);

            public new QuizResource AsPrivate() =>
                (QuizResource)base.AsPrivate();

            public new QuizResource AsPublic() =>
                (QuizResource)base.AsPublic();

            public new QuizResource WithSuspension(bool value = true) =>
                (QuizResource)base.WithSuspension(value);

            public new IResponse<QuizResource> TryWithNewVerifyingUser(User value) =>
                (IResponse<QuizResource>)base.TryWithNewVerifyingUser(value);

            public new IResponse<QuizResource> TryWithConfirmedVerification() =>
                (IResponse<QuizResource>)base.TryWithConfirmedVerification();

            public new IResponse<QuizResource> TryWithRejectedVerification() =>
                (IResponse<QuizResource>)base.TryWithRejectedVerification();

            public new IResponse<QuizResource> TryWithCanceledVerification() =>
                (IResponse<QuizResource>)base.TryWithCanceledVerification();

            public new QuizResource WithConsumedEvents(out IEnumerable<IDomainEvent> domainEvents) =>
                (QuizResource)base.WithConsumedEvents(out domainEvents);

        #endregion
    #endregion

}
