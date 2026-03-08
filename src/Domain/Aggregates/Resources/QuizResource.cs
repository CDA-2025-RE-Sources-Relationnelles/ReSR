using FluentResponse;
using FluentResponse.Interfaces;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Categories;
using ReSR.Domain.Aggregates.Resources.ValueObjects;
using ReSR.Domain.Core;

namespace ReSR.Domain.Aggregates.Resources;
public record QuizResource : Resource, IAggregateRoot<QuizResource> {

    #region PROPERTIES

        /// <summary> The resource's quiz questions. </summary>
        public ICollection<QuizQuestion> Questions { get; internal set; } = [];

    #endregion
    #region CONSTRUCTORS

        public static IResponse<QuizResource> TryCreate(
            string                    title,
            string                    description,
            Category                  category,
            Relationships             relationships,
            IEnumerable<QuizQuestion> questions,
            bool                      isPrivate = true,
            User?                     owner     = null
        ) => TryVerifyTitleInvariant(title)
                .OnSuccess(() => TryVerifyQuizQuestionsInvariant(questions))
                .OnSuccess(() => new QuizResource {
                    Title         = title,
                    Description   = description,
                    Category      = category,
                    Relationships = relationships,
                    Questions     = [.. questions],
                    Owner         = owner,
                    Visibility    = isPrivate
                        ? Visibility.Private 
                        : owner is not null ? Visibility.WaitingForVerification : Visibility.Public
                });

    #endregion
    #region METHODS
            
        public new QuizResource WithDescription(string value) =>
            (QuizResource)base.WithDescription(value);

        public new QuizResource WithCategory(Category value) =>
            (QuizResource)base.WithCategory(value);

        /// <returns> A copy of the resource with the given additional question if valid. </returns>
        public virtual IResponse<QuizResource> WithNewQuestion(QuizQuestion value) =>
            TryVerifyQuizQuestionInvariant(value)
                .OnSuccess(() => {
                    List<QuizQuestion> questions = [.. this.Questions, value];
                    return TryVerifyQuizQuestionsInvariant(questions).OnSuccess(() => {
                        this.Questions = questions;
                        return this;
                    });
                });

        /// <returns> A copy of the resource with the given additional question if valid. </returns>
        public virtual IResponse<QuizResource> WithQuestion(int index, QuizQuestion value) =>
            TryVerifyQuizQuestionInvariant(value)
                .OnSuccess(() => this.Questions.Count > index ? Response.Success() : Response.Failure($"Aucune question avec l'indice '{index}' !"))
                .OnSuccess(() => {
                    List<QuizQuestion> questions = [.. this.Questions];
                    questions[index] = value;
                    this.Questions = questions;
                    return this;
                });

        /// <returns> A copy of the resource without the question at the given index. </returns>
        public virtual IResponse<QuizResource> WithoutQuestion(int index) =>
            (this.Questions.Count > index
                ? Response.Success()
                : Response.Failure($"Aucune question avec l'indice '{index}' !"))
            .OnSuccess(() => {

                List<QuizQuestion> questions = [.. this.Questions];
                questions.RemoveAt(index);
                return TryVerifyQuizQuestionsInvariant(questions).OnSuccess(() => {
                    this.Questions = questions;
                    return this;
                });
            });


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



        #region OVERRIDES

            public new IResponse<QuizResource> TryWithTitle(string value) =>
                base.TryWithTitle(value).OnSuccess(x => (QuizResource)x);

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

            public new IResponse<QuizResource> TryWithConfirmedVerification() =>
                base.TryWithConfirmedVerification().OnSuccess(x => (QuizResource)x);

            public new IResponse<QuizResource> TryWithRejectedVerification() =>
                base.TryWithRejectedVerification().OnSuccess(x => (QuizResource)x);

            public new QuizResource WithConsumedEvents(out IEnumerable<IDomainEvent> domainEvents) =>
                (QuizResource)base.WithConsumedEvents(out domainEvents);

        #endregion
    #endregion

}
