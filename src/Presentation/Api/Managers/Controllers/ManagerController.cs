using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReSR.Application.Services.Managers.Definitions;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Accounts.ValueObjects;
using ReSR.Domain.Ports;
using ReSR.Presentation.Api.Core.Extensions;
using ReSR.Presentation.Api.Managers.ValueObjects.Accounts;

namespace ReSR.Presentation.Api.Managers.Controllers;
[ApiController]
[Route(ROUTE)]
[Authorize(Policy = "BackOffice")]
public class ManagerController(
    IRepository<Manager> repository,
    IManagerCommandService commandService
) : ControllerBase {

    public const string ROUTE = "/manage/managers";

    #region DTOS

        public readonly record struct CreateManagerDto(
            string Email,
            string Password,
            string Permissions
        );

        public readonly record struct UpdateManagerDto(
            string? Email = null,
            string? Password = null,
            string? Permissions = null
        );

    #endregion
    #region ROUTES

        [HttpGet]
        [Authorize(Roles = nameof(ManagerPermissions.ReadManagers))]
        [EndpointSummary("Only accessible for managers with read permissions.")]
        [EndpointDescription("Queries the managers.")]
        public Task<IResult> GetManagersAsync(
            int pageIndex = 0,
            int pageSize  = 10
        ) => repository.GetAllAsync().ToPageResourceAsync<Manager, ManagerResource>(pageIndex, pageSize);

        [HttpGet("{managerId}")]
        [Authorize(Roles = nameof(ManagerPermissions.ReadManagers))]
        [EndpointSummary("Only accessible for managers with read permissions.")]
        [EndpointDescription("Queries the manager.")]
        public Task<IResult> GetManagerAsync(Id managerId) =>
            repository.TryGetAsync(managerId).ToResourceAsync<Manager, ManagerResource>(Results.Ok);

        [HttpPost]
        [Authorize(Roles = nameof(ManagerPermissions.WriteManagers))]
        [EndpointSummary("Only accessible for managers with write permissions.")]
        [EndpointDescription("Creates a new manager.")]
        public Task<IResult> PostManagerAsync(CreateManagerDto dto) =>
            commandService
                .TryCreateAsync(dto.Email, dto.Password, Enum.TryParse<ManagerPermissions>(dto.Permissions, true, out var permissions) ? permissions : ManagerPermissions.None)
                .ToResourceAsync<Manager, ManagerResource>(Results.Ok);

        [HttpPatch("{managerId}")]
        [Authorize(Roles = nameof(ManagerPermissions.WriteManagers))]
        [EndpointSummary("Only accessible for managers with read permissions.")]
        [EndpointDescription("Tries to update the manager.")]
        public Task<IResult> PatchManagerAsync(Id managerId, UpdateManagerDto dto) =>
            commandService
                .TryUpdateAsync(managerId, dto.Email, dto.Password, Enum.TryParse<ManagerPermissions>(dto.Permissions, true, out var permissions) ? permissions : null)
                .ToResourceAsync<Manager, ManagerResource>(Results.Ok);

        [HttpDelete("{managerId}")]
        [Authorize(Roles = nameof(ManagerPermissions.WriteManagers))]
        [EndpointSummary("Only accessible for managers with write permissions.")]
        [EndpointDescription("Tries to delete the manager.")]
        public Task<IResult> DeleteManagerAsync(Id managerId) =>
            commandService.TryDeleteAsync(managerId).ToResultAsync(Results.Ok);

    #endregion
}
