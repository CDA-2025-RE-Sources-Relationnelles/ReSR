using FluentResponse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReSR.Application.Services.Users.Definitions;
using ReSR.Domain.Aggregates.Accounts.ValueObjects;
using ReSR.Domain.Aggregates.Messages;
using ReSR.Domain.Aggregates.QuizSessions;
using ReSR.Domain.Aggregates.Resources;
using ReSR.Domain.Aggregates.Resources.ValueObjects;
using ReSR.Domain.Ports;
using ReSR.Presentation.Api.Core.Extensions;
using ReSR.Presentation.Api.Users.ValueObjects.Resources;
using ReSR.Presentation.Api.Users.Authorization;
using ReSR.Presentation.Api.Users.Extensions;
using ReSR.Presentation.Api.Users.ValueObjects.Messages;
using ReSR.Presentation.Api.Users.ValueObjects.QuizSessions;
using ReSR.Application.ValueObjects.Resources;

namespace ReSR.Presentation.Api.Users.Controllers;
[ApiController]
[Route(ROUTE)]
public class QuizResourceController(
    IQuizResourceService resourceService,
    IRepository<QuizResource> repository
) : ControllerBase {

    public const string ROUTE = "/quiz-resources";
    #region DTOS

        public readonly record struct CreateUserQuizResourceDto(
            string                    Title,
            Id                        CategoryId,
            string                    Relationships,
            string                    Content,
            IEnumerable<QuizQuestion> Questions,
            bool                      IsPrivate
        );

        public readonly record struct UpdateUserQuizResourceDto(
            string? Title         = null,
            Id?     CategoryId    = null,
            string? Relationships = null,
            string? Content       = null
        );

        public readonly record struct PostQuizResourceCommentDto(
            string Content
        );

        public readonly record struct StartQuizSessionDto(
            IEnumerable<Id> ParticipantsId
        );

    #endregion
    #region ROUTES

        [HttpGet(ROUTE + "/public")]
        [EndpointDescription("Queries the public quiz resources.")]
        public Task<IResult> GetPublicTextResourcesAsync(
            Id?    categoryIdFilter    = null,
            string relationshipsFilter = nameof(Relationships.None),
            string orderBy             = nameof(OrderBy.Newest)
        ) => resourceService.GetAllPublicAsync(
            categoryIdFilter: categoryIdFilter,
            relationshipsFilter: Enum.TryParse<Relationships>(relationshipsFilter, true, out var relationshipsFilterParsed) ? relationshipsFilterParsed : Relationships.None,
            orderBy: Enum.TryParse<OrderBy>(orderBy, true, out var orderByParsed) ? orderByParsed : OrderBy.Newest
        ).ToResourceAsync<QuizResource, QuizResourceResource>(Results.Ok);


        [HttpGet(ROUTE + "/private")]
        [Authorize]
        [EndpointSummary("Only accessible for authenticated users.")]
        [EndpointDescription("Queries the user's private quiz resources.")]
        public Task<IResult> GetPrivateTextResourcesAsync(
            Id?    categoryIdFilter    = null,
            string relationshipsFilter = nameof(Relationships.None),
            string orderBy             = nameof(OrderBy.Newest)
        ) => resourceService.TryGetAllPrivateAsync(
            userId: User.GetUserId()!.Value,
            categoryIdFilter: categoryIdFilter,
            relationshipsFilter: Enum.TryParse<Relationships>(relationshipsFilter, true, out var relationshipsFilterParsed) ? relationshipsFilterParsed : Relationships.None,
            orderBy: Enum.TryParse<OrderBy>(orderBy, true, out var orderByParsed) ? orderByParsed : OrderBy.Newest
        ).ToResourceAsync<QuizResource, QuizResourceResource>(Results.Ok);

        [HttpGet("{resourceId}")]
        [Authorize(Policy = nameof(ResourceReadAuthorizationRequirement))]
        [EndpointDescription("Queries the quiz resource.")]
        public Task<IResult> GetQuizResourceAsync(Id resourceId) =>
            repository.TryGetAsync(resourceId).ToResourceAsync<QuizResource, QuizResourceResource>(Results.Ok);

        [HttpPost]
        [Authorize]
        [EndpointSummary("Only accessible for authenticated users.")]
        [EndpointDescription("Creates a new quiz resource.")]
        public Task<IResult> PostQuizResourceAsync(CreateUserQuizResourceDto dto) =>
            resourceService.TryCreateAsync(
                ownerId: User.GetUserId()!.Value,
                title: dto.Title,
                categoryId: dto.CategoryId,
                relationships: Enum.TryParse<Relationships>(dto.Relationships, true, out var relationships) ? relationships : Relationships.All,
                content: dto.Content,
                questions: dto.Questions,
                isPrivate: dto.IsPrivate
            ).ToResourceAsync<QuizResource, QuizResourceResource>(Results.Ok);

        [HttpPatch("{resourceId}")]
        [Authorize(Policy = nameof(ResourceWriteAuthorizationRequirement))]
        [EndpointSummary("Only accessible for the resource owner.")]
        [EndpointDescription("Updates the quiz resource.")]
        public Task<IResult> PatchQuizResourceAsync(Id resourceId, UpdateUserQuizResourceDto dto) =>
            resourceService.TryUpdateAsync(
                id: resourceId,
                title: dto.Title,
                categoryId: dto.CategoryId,
                relationships: Enum.TryParse<Relationships>(dto.Relationships, true, out var relationships) ? relationships : null,
                content: dto.Content
            ).ToResourceAsync<QuizResource, QuizResourceResource>(Results.Ok);

        [HttpPost("{resourceId}/questions")]
        [Authorize(Policy = nameof(ResourceWriteAuthorizationRequirement))]
        [EndpointSummary("Only accessible for the resource owner.")]
        [EndpointDescription("Adds a quiz resource question.")]
        public Task<IResult> PostQuizResourceQuestionAsync(Id resourceId, QuizQuestion dto) =>
            resourceService
                .TryAddQuestionAsync(resourceId, dto)
                .ToResourceAsync<QuizResource, QuizResourceResource>(Results.Ok);

        [HttpPut("{resourceId}/questions/{questionIndex}")]
        [Authorize(Policy = nameof(ResourceWriteAuthorizationRequirement))]
        [EndpointSummary("Only accessible for the resource owner.")]
        [EndpointDescription("Sets a quiz resource question.")]
        public Task<IResult> PutQuizResourceQuestionAsync(Id resourceId, int questionIndex, QuizQuestion dto) =>
            resourceService
                .TrySetQuestionAsync(resourceId, questionIndex, dto)
                .ToResourceAsync<QuizResource, QuizResourceResource>(Results.Ok);

        [HttpDelete("{resourceId}/questions/{questionIndex}")]
        [Authorize(Policy = nameof(ResourceWriteAuthorizationRequirement))]
        [EndpointSummary("Only accessible for the resource owner.")]
        [EndpointDescription("Removes a quiz resource question.")]
        public Task<IResult> DeleteQuizResourceQuestionAsync(Id resourceId, int questionIndex) =>
            resourceService
                .TryRemoveQuestionAsync(resourceId, questionIndex)
                .ToResourceAsync<QuizResource, QuizResourceResource>(Results.Ok);

        [HttpDelete("{resourceId}")]
        [Authorize(Policy = nameof(ResourceWriteAuthorizationRequirement))]
        [EndpointSummary("Only accessible for the resource owner.")]
        [EndpointDescription("Deletes the quiz resource.")]
        public Task<IResult> DeleteQuizResourceAsync(Id resourceId) =>
            repository.TryDeleteAsync(resourceId).ToResultAsync(Results.Ok);

        [HttpPost("{resourceId}/like")]
        [Authorize(Roles = nameof(User), Policy = nameof(ResourceReadAuthorizationRequirement))]
        [EndpointSummary("Only accessible for authenticated users with access to the resource.")]
        [EndpointDescription("Likes the quiz resource")]
        public Task<IResult> LikeQuizResourceAsync(Id resourceId, bool value = true) =>
            resourceService.TryLikeAsync(resourceId, User.GetUserId()!.Value, value).ToResourceAsync<QuizResource, QuizResourceResource>(Results.Ok);

        [HttpPost("{resourceId}/bookmark")]
        [Authorize(Roles = nameof(User), Policy = nameof(ResourceReadAuthorizationRequirement))]
        [EndpointSummary("Only accessible for authenticated users with access to the resource.")]
        [EndpointDescription("Likes the quiz resource")]
        public Task<IResult> BookmarkQuizResourceAsync(Id resourceId, bool value = true) =>
            resourceService.TryBookmarkAsync(resourceId, User.GetUserId()!.Value, value).ToResourceAsync<QuizResource, QuizResourceResource>(Results.Ok);

        [HttpPost("{resourceId}/exploit")]
        [Authorize(Roles = nameof(User), Policy = nameof(ResourceReadAuthorizationRequirement))]
        [EndpointSummary("Only accessible for authenticated users with access to the resource.")]
        [EndpointDescription("Likes the quiz resource")]
        public Task<IResult> ExploitQuizResourceAsync(Id resourceId, bool value = true) =>
            resourceService.TryExploitAsync(resourceId, User.GetUserId()!.Value, value).ToResourceAsync<QuizResource, QuizResourceResource>(Results.Ok);

        [HttpPost("{resourceId}/start-session")]
        [Authorize(Roles = nameof(User), Policy = nameof(ResourceReadAuthorizationRequirement))]
        [EndpointSummary("Only accessible for authenticated users with access to the resource.")]
        [EndpointDescription("Starts a session for this quiz resource")]
        public Task<IResult> StartQuizSessionAsync(Id resourceId, StartQuizSessionDto dto) =>
            resourceService.TryStartSessionAsync(
                resourceId,
                User.GetUserId()!.Value,
                dto.ParticipantsId
            ).ToResourceAsync<QuizSession, QuizSessionResource>(Results.Ok);


        [HttpPost("{resourceId}/comments")]
        [Authorize(Roles = nameof(User), Policy = nameof(ResourceReadAuthorizationRequirement))]
        [EndpointSummary("Only accessible for authenticated users with access to the resource.")]
        [EndpointDescription("Posts a comment to the quiz resource")]
        public Task<IResult> PostQuizResourceComment(Id resourceId, PostQuizResourceCommentDto dto) =>
            resourceService.TryPostCommentAsync(resourceId, User.GetUserId()!.Value, dto.Content).ToResourceAsync<Comment, CommentResource>(Results.Ok);

        [HttpGet("{resourceId}/comments")]
        [Authorize(Policy = nameof(ResourceReadAuthorizationRequirement))]
        [EndpointDescription("Queries commens from the text resource")]
        public Task<IResult> GetAllQuizResourceComments(Id resourceId) =>
            resourceService.TryGetCommentsAsync(resourceId).ToResourceAsync<Comment, CommentResource>(Results.Ok);


        [HttpPost("{resourceId}/confirm-verification")]
        [Authorize(Roles = nameof(UserPermissions.VerifyResources))]
        [EndpointSummary("Only accessible for authenticated users with resource verification permissions.")]
        [EndpointDescription("Confirms the quiz resource verification.")]
        public Task<IResult> ConfirmQuizResourceVerification(Id resourceId) =>
            resourceService.TryConfirmVerificationAsync(resourceId).ToResourceAsync<QuizResource, QuizResourceResource>(Results.Ok);

        [HttpPost("{resourceId}/reject-verification")]
        [Authorize(Roles = nameof(UserPermissions.VerifyResources))]
        [EndpointSummary("Only accessible for authenticated users with resource verification permissions.")]
        [EndpointDescription("Cancels the quiz resource verification.")]
        public Task<IResult> RejectQuizResourceVerification(Id resourceId) =>
            resourceService.TryRejectVerificationAsync(resourceId).ToResourceAsync<QuizResource, QuizResourceResource>(Results.Ok);

    #endregion
    
}
