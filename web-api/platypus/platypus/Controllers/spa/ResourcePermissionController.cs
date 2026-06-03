using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nssol.Platypus.ApiModels.ResourcePermissionApiModels;
using Nssol.Platypus.Controllers.Util;
using Nssol.Platypus.DataAccess.Core;
using Nssol.Platypus.DataAccess.Repositories.Interfaces.TenantRepositories;
using Nssol.Platypus.Filters;
using Nssol.Platypus.Infrastructure;
using Nssol.Platypus.Models.TenantModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Nssol.Platypus.Controllers.spa
{
    /// <summary>
    /// リソース権限管理を扱うためのAPI集
    /// </summary>
    [ApiController]
    [ApiVersion("1"), ApiVersion("2")]
    [Route("api/v{api-version:apiVersion}/tenant/resource-permissions")]
    public class ResourcePermissionController : PlatypusApiControllerBase
    {
        private readonly IProjectRepository projectRepository;
        private readonly IUnitOfWork unitOfWork;

        public ResourcePermissionController(
            IProjectRepository projectRepository,
            IUnitOfWork unitOfWork,
            IHttpContextAccessor accessor) : base(accessor)
        {
            this.projectRepository = projectRepository;
            this.unitOfWork = unitOfWork;
        }

        /// <summary>
        /// 指定リソースのアクセス権限一覧を取得
        /// </summary>
        [HttpGet]
        [PermissionFilter(MenuCode.ResourcePermission)]
        [ProducesResponseType(typeof(IEnumerable<IndexOutputModel>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAll([FromQuery] ResourceType2 resourceType, [FromQuery] long resourceId)
        {
            var permissions = await projectRepository.GetResourcePermissionsAsync(resourceType, resourceId);
            return JsonOK(permissions.Select(p => new IndexOutputModel(p)));
        }

        /// <summary>
        /// リソースへのアクセス権限を追加
        /// </summary>
        [HttpPost]
        [PermissionFilter(MenuCode.ResourcePermission)]
        [ProducesResponseType(typeof(IndexOutputModel), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> Create([FromBody] CreateInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return JsonBadRequest("Invalid inputs.");
            }
            if (!Enum.IsDefined(typeof(ResourceType2), model.ResourceType))
            {
                return JsonBadRequest("Invalid ResourceType value.");
            }
            if (!Enum.IsDefined(typeof(ProjectRoleType), model.AccessLevel))
            {
                return JsonBadRequest("Invalid AccessLevel value.");
            }

            var permission = new ResourcePermission
            {
                ResourceType = model.ResourceType,
                ResourceId = model.ResourceId,
                UserId = model.UserId,
                AccessLevel = model.AccessLevel,
            };

            projectRepository.AddResourcePermission(permission);
            unitOfWork.Commit();

            // User navigation propertyを読み込んでレスポンスに含める
            var saved = (await projectRepository.GetResourcePermissionsAsync(model.ResourceType, model.ResourceId))
                .FirstOrDefault(p => p.Id == permission.Id);
            return JsonCreated(new IndexOutputModel(saved ?? permission));
        }

        /// <summary>
        /// リソースへのアクセス権限を削除
        /// </summary>
        [HttpDelete("{id}")]
        [PermissionFilter(MenuCode.ResourcePermission)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> Delete(long id, [FromQuery] ResourceType2 resourceType, [FromQuery] long resourceId)
        {
            var permissions = await projectRepository.GetResourcePermissionsAsync(resourceType, resourceId);
            var permission = permissions.FirstOrDefault(p => p.Id == id);
            if (permission == null)
            {
                return JsonNotFound($"Permission Id {id} is not found.");
            }

            projectRepository.RemoveResourcePermission(permission);
            unitOfWork.Commit();
            return NoContent();
        }

        /// <summary>
        /// 現在のユーザーが特定リソースにアクセスできるか確認
        /// </summary>
        [HttpGet("check")]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CheckAccess(
            [FromQuery] ResourceType2 resourceType,
            [FromQuery] long resourceId,
            [FromQuery] ProjectRoleType requiredLevel)
        {
            if (!Enum.IsDefined(typeof(ProjectRoleType), requiredLevel))
            {
                return JsonBadRequest("Invalid requiredLevel value.");
            }
            if (!Enum.IsDefined(typeof(ResourceType2), resourceType))
            {
                return JsonBadRequest("Invalid resourceType value.");
            }
            var hasAccess = await projectRepository.HasResourceAccessAsync(
                CurrentUserInfo.Id, resourceType, resourceId, requiredLevel);
            return JsonOK(new { hasAccess });
        }
    }
}
