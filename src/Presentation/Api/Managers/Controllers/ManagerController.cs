using FluentResponse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Accounts.ValueObjects;
using ReSR.Domain.Ports;
using ReSR.Presentation.Api.Core.Extensions;
using ReSR.Presentation.Api.Managers.ValueObjects.Accounts;

namespace ReSR.Presentation.Api.Managers.Controllers;
[ApiController]
[Route(ROUTE)]
public class ManagerController(
    IRepository<Manager> repository
) : ControllerBase {

    public const string ROUTE = "/manage/managers";

    #region DTOS

        public readonly record struct CreateManagerDto(
            string Password,
            string Email,
            string Permissions
        );

        public readonly record struct UpdateManagerDto(
            string? Password = null,
            string? Email = null,
            string? Permissions = null
        );

    #endregion
    #region ROUTES

        [HttpGet]
        [Authorize(Roles = nameof(ManagerPermissions.ReadManagers), AuthenticationSchemes = nameof(Manager))]
        [EndpointSummary("Only accessible for managers with read permissions.")]
        [EndpointDescription("Queries the managers.")]
        public Task<IResult> GetManagersAsync() =>
            repository.GetAllAsync().ToResourceAsync<Manager, ManagerResource>(Results.Ok);

        [HttpGet("{managerId}", Name = nameof(GetManagersAsync))]
        [Authorize(Roles = nameof(ManagerPermissions.ReadManagers), AuthenticationSchemes = nameof(Manager))]
        [EndpointSummary("Only accessible for managers with read permissions.")]
        [EndpointDescription("Queries the manager.")]
        public Task<IResult> GetManagerAsync(Id managerId) =>
            repository.TryGetAsync(managerId).ToResourceAsync<Manager, ManagerResource>(Results.Ok);

        [HttpPost]
        [Authorize(Roles = nameof(ManagerPermissions.WriteManagers), AuthenticationSchemes = nameof(Manager))]
        [EndpointSummary("Only accessible for managers with write permissions.")]
        [EndpointDescription("Creates a new manager.")]
        public Task<IResult> PostManagerAsync(CreateManagerDto dto) =>
            Manager
                .TryCreate(dto.Email, dto.Password, Enum.TryParse<ManagerPermissions>(dto.Permissions, true, out var permissions) ? permissions : ManagerPermissions.AdminRole)
                .OnSuccessAsync(repository.TryAddAsync)
                .ToResourceAsync<Manager, ManagerResource>(Results.Ok);

        [HttpPatch("{managerId}")]
        [Authorize(Roles = nameof(ManagerPermissions.WriteManagers), AuthenticationSchemes = nameof(Manager))]
        [EndpointSummary("Only accessible for managers with read permissions.")]
        [EndpointDescription("Queries the manager.")]
        public Task<IResult> PatchManagerAsync(Id managerId, UpdateManagerDto dto) =>
            repository.TryUpdateAsync(managerId, manager => {

                var response = FluentResponse.Response.Success(manager);

                if (dto.Email is not null) response = response.OnSuccess(x => x.TryWithMailAddress(dto.Email));
                if (dto.Password is not null) response = response.OnSuccess(x => x.TryWithPassword(dto.Password));
                if (Enum.TryParse<ManagerPermissions>(dto.Permissions, true, out var permissions)) response = response.OnSuccess(x => x.WithPermissions(permissions));

                return response;

            }).ToResourceAsync<Manager, ManagerResource>(Results.Ok);

        [HttpDelete("{managerId}")]
        [Authorize(Roles = nameof(ManagerPermissions.WriteManagers), AuthenticationSchemes = nameof(Manager))]
        [EndpointSummary("Only accessible for managers with write permissions.")]
        [EndpointDescription("Tries to delete the manager.")]
        public Task<IResult> DeleteManagerAsync(Id managerId) =>
            repository.TryDeleteAsync(managerId).ToResultAsync(Results.Ok);

    #endregion
}
