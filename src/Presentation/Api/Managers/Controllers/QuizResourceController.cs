using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReSR.Application.Services.Managers.Definitions;
using ReSR.Application.ValueObjects.Resources;
using ReSR.Domain.Aggregates.Accounts.ValueObjects;
using ReSR.Domain.Aggregates.Resources;
using ReSR.Domain.Aggregates.Resources.ValueObjects;
using ReSR.Domain.Ports;
using ReSR.Presentation.Api.Core.Extensions;
using ReSR.Presentation.Api.Managers.ValueObjects.Resources;

namespace ReSR.Presentation.Api.Managers.Controllers;
[ApiController]
[Route(ROUTE)]
[Authorize(Policy = "BackOffice")]
public class QuizResourceController(
    IResourceQueryService<QuizResource> queryService,
    IQuizResourceCommandService commandService
) : ControllerBase {

    public const string ROUTE = "/manage/quiz-resources";
    #region DTOS

        public readonly record struct CreateQuizResourceDto(
            string                    Title,
            string                    Description,
            Id                        CategoryId,
            string                    Relationships,
            IEnumerable<QuizQuestion> Questions
        );

        public readonly record struct UpdateQuizResourceDto(
            string? Title         = null,
            string? Description   = null,
            Id?     CategoryId    = null,
            string? Relationships = null
        );

    #endregion
    #region ROUTES

        [HttpGet]
        [Authorize(Roles = nameof(ManagerPermissions.ReadContent))]
        [EndpointSummary("Only accessible for managers with read permissions.")]
        [EndpointDescription("Queries the quiz resources.")]
        public Task<IResult> GetQuizResourcesAsync(
            Id?    categoryIdFilter    = null,
            string relationshipsFilter = nameof(Relationships.None),
            string orderBy             = nameof(OrderBy.Newest)
        ) => queryService.GetAllAsync(
            categoryIdFilter: categoryIdFilter,
            relationshipsFilter: Enum.TryParse<Relationships>(relationshipsFilter, true, out var relationshipsFilterParsed) ? relationshipsFilterParsed : Relationships.None,
            orderBy: Enum.TryParse<OrderBy>(orderBy, true, out var orderByParsed) ? orderByParsed : OrderBy.Newest
        ).ToResourceAsync<QuizResource, QuizResourceResource>(Results.Ok);

        [HttpGet("{resourceId}")]
        [Authorize(Roles = nameof(ManagerPermissions.ReadContent))]
        [EndpointSummary("Only accessible for managers with read permissions.")]
        [EndpointDescription("Queries the quiz resource.")]
        public Task<IResult> GetQuizResourceAsync(Id resourceId) =>
            queryService.TryGetAsync(resourceId).ToResourceAsync<QuizResource, QuizResourceResource>(Results.Ok);

        [HttpPost]
        [Authorize(Roles = nameof(ManagerPermissions.WriteContent))]
        [EndpointSummary("Only accessible for managers with write permissions.")]
        [EndpointDescription("Creates a new quiz resource.")]
        public Task<IResult> PostQuizResourceAsync(CreateQuizResourceDto dto) =>
            commandService
                .TryCreateAsync(
                    title: dto.Title,
                    description: dto.Description,
                    categoryId: dto.CategoryId,
                    relationships: Enum.TryParse<Relationships>(dto.Relationships, true, out var permissions) ? permissions : Relationships.All,
                    questions: dto.Questions
                ).ToResourceAsync<QuizResource, QuizResourceResource>(Results.Ok);

        [HttpPatch("{resourceId}")]
        [Authorize(Roles = nameof(ManagerPermissions.WriteContent))]
        [EndpointSummary("Only accessible for managers with read permissions.")]
        [EndpointDescription("Updates the quiz resource.")]
        public Task<IResult> PatchQuizResourceAsync(Id resourceId, UpdateQuizResourceDto dto) =>
            commandService
                .TryUpdateAsync(
                    id: resourceId,
                    title: dto.Title,
                    description: dto.Description,
                    categoryId: dto.CategoryId,
                    relationships: Enum.TryParse<Relationships>(dto.Relationships, true, out var permissions) ? permissions : null
                ).ToResourceAsync<QuizResource, QuizResourceResource>(Results.Ok);


        [HttpPost("{resourceId}/questions")]
        [Authorize(Roles = nameof(ManagerPermissions.WriteContent))]
        [EndpointSummary("Only accessible for managers with read permissions.")]
        [EndpointDescription("Adds a quiz resource question.")]
        public Task<IResult> PostQuizResourceQuestionAsync(Id resourceId, QuizQuestion dto) =>
            commandService
                .TryAddQuestionAsync(resourceId, dto)
                .ToResourceAsync<QuizResource, QuizResourceResource>(Results.Ok);

        [HttpPut("{resourceId}/questions/{questionIndex}")]
        [Authorize(Roles = nameof(ManagerPermissions.WriteContent))]
        [EndpointSummary("Only accessible for managers with read permissions.")]
        [EndpointDescription("Sets a quiz resource question.")]
        public Task<IResult> PutQuizResourceQuestionAsync(Id resourceId, int questionIndex, QuizQuestion dto) =>
            commandService
                .TrySetQuestionAsync(resourceId, questionIndex, dto)
                .ToResourceAsync<QuizResource, QuizResourceResource>(Results.Ok);

        [HttpDelete("{resourceId}/questions/{questionIndex}")]
        [Authorize(Roles = nameof(ManagerPermissions.WriteContent))]
        [EndpointSummary("Only accessible for managers with read permissions.")]
        [EndpointDescription("Removes a quiz resource question.")]
        public Task<IResult> DeleteQuizResourceQuestionAsync(Id resourceId, int questionIndex) =>
            commandService
                .TryRemoveQuestionAsync(resourceId, questionIndex)
                .ToResourceAsync<QuizResource, QuizResourceResource>(Results.Ok);

        [HttpPost("{resourceId}/suspend")]
        [Authorize(Roles = nameof(ManagerPermissions.WriteContent))]
        [EndpointSummary("Only accessible for managers with write permissions.")]
        [EndpointDescription("Tries to suspend the quiz resource.")]
        public Task<IResult> SuspendUserAsync(Id resourceId, bool value = true) =>
            commandService.TrySuspendAsync(resourceId, value).ToResourceAsync<QuizResource, QuizResourceResource>(Results.Ok);

        [HttpDelete("{resourceId}")]
        [Authorize(Roles = nameof(ManagerPermissions.WriteContent))]
        [EndpointSummary("Only accessible for managers with write permissions.")]
        [EndpointDescription("Tries to delete the quiz resource.")]
        public Task<IResult> DeleteQuizResourceAsync(Id resourceId) =>
            commandService
                .TryDeleteAsync(resourceId)
                .ToResultAsync(Results.Ok);

    #endregion
    
}
