using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nssol.Platypus.ApiModels.CustomRoleApiModels;
using Nssol.Platypus.Controllers.Util;
using Nssol.Platypus.DataAccess.Core;
using Nssol.Platypus.DataAccess.Repositories.Interfaces;
using Nssol.Platypus.Filters;
using Nssol.Platypus.Infrastructure;
using Nssol.Platypus.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Nssol.Platypus.Controllers.spa
{
    /// <summary>
    /// カスタムロール管理を扱うためのAPI集
    /// </summary>
    [ApiController]
    [ApiVersion("1"), ApiVersion("2")]
    [Route("api/v{api-version:apiVersion}/tenant/custom-roles")]
    public class CustomRoleController : PlatypusApiControllerBase
    {
        private readonly ICustomRoleRepository customRoleRepository;
        private readonly IUnitOfWork unitOfWork;

        public CustomRoleController(
            ICustomRoleRepository customRoleRepository,
            IUnitOfWork unitOfWork,
            IHttpContextAccessor accessor) : base(accessor)
        {
            this.customRoleRepository = customRoleRepository;
            this.unitOfWork = unitOfWork;
        }

        /// <summary>
        /// カスタムロール一覧を取得
        /// </summary>
        [HttpGet]
        [PermissionFilter(MenuCode.CustomRole)]
        [ProducesResponseType(typeof(IEnumerable<IndexOutputModel>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAll()
        {
            var tenantId = CurrentUserInfo.SelectedTenant.Id;
            var roles = await customRoleRepository.GetAllAsync(tenantId);
            return JsonOK(roles.Select(r => new IndexOutputModel(r)));
        }

        /// <summary>
        /// カスタムロール詳細を取得
        /// </summary>
        [HttpGet("{id}")]
        [PermissionFilter(MenuCode.CustomRole)]
        [ProducesResponseType(typeof(IndexOutputModel), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetById(long id)
        {
            var role = await customRoleRepository.GetByIdAsync(id);
            if (role == null)
            {
                return JsonNotFound($"Custom role Id {id} is not found.");
            }
            if (role.TenantId != CurrentUserInfo.SelectedTenant.Id)
            {
                return JsonNotFound($"Custom role Id {id} is not found.");
            }
            return JsonOK(new IndexOutputModel(role));
        }

        /// <summary>
        /// カスタムロールを新規作成する
        /// </summary>
        [HttpPost]
        [PermissionFilter(MenuCode.CustomRole)]
        [ProducesResponseType(typeof(IndexOutputModel), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> Create([FromBody] CreateInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return JsonBadRequest("Invalid inputs.");
            }

            var tenantId = CurrentUserInfo.SelectedTenant.Id;
            var existingRoles = await customRoleRepository.GetAllAsync(tenantId);
            if (existingRoles.Any(r => r.Name == model.Name))
            {
                return JsonBadRequest($"Custom role with name '{model.Name}' already exists.");
            }

            var role = new CustomRole
            {
                Name = model.Name,
                DisplayName = model.DisplayName,
                TenantId = CurrentUserInfo.SelectedTenant.Id,
                Description = model.Description,
                CanManageData = model.CanManageData,
                CanManageDataSet = model.CanManageDataSet,
                CanRunTraining = model.CanRunTraining,
                CanRunInference = model.CanRunInference,
                CanUseNotebook = model.CanUseNotebook,
                CanManageProject = model.CanManageProject,
                CanEditTenantSetting = model.CanEditTenantSetting,
                CanManageResourcePermission = model.CanManageResourcePermission,
            };

            customRoleRepository.Add(role, unitOfWork);
            return JsonCreated(new IndexOutputModel(role));
        }

        /// <summary>
        /// カスタムロールを編集する
        /// </summary>
        [HttpPut("{id}")]
        [PermissionFilter(MenuCode.CustomRole)]
        [ProducesResponseType(typeof(IndexOutputModel), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Edit(long id, [FromBody] EditInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return JsonBadRequest("Invalid inputs.");
            }

            var role = await customRoleRepository.GetByIdAsync(id);
            if (role == null)
            {
                return JsonNotFound($"Custom role Id {id} is not found.");
            }
            if (role.TenantId != CurrentUserInfo.SelectedTenant.Id)
            {
                return JsonNotFound($"Custom role Id {id} is not found.");
            }

            var tenantId = CurrentUserInfo.SelectedTenant.Id;
            var existingRoles = await customRoleRepository.GetAllAsync(tenantId);
            if (existingRoles.Any(r => r.Name == model.Name && r.Id != id))
            {
                return JsonBadRequest($"Custom role with name '{model.Name}' already exists.");
            }

            role.Name = model.Name;
            role.DisplayName = model.DisplayName;
            role.Description = model.Description;
            role.CanManageData = model.CanManageData;
            role.CanManageDataSet = model.CanManageDataSet;
            role.CanRunTraining = model.CanRunTraining;
            role.CanRunInference = model.CanRunInference;
            role.CanUseNotebook = model.CanUseNotebook;
            role.CanManageProject = model.CanManageProject;
            role.CanEditTenantSetting = model.CanEditTenantSetting;
            role.CanManageResourcePermission = model.CanManageResourcePermission;

            customRoleRepository.Update(role, unitOfWork);
            return JsonOK(new IndexOutputModel(role));
        }

        /// <summary>
        /// カスタムロールを削除する
        /// </summary>
        [HttpDelete("{id}")]
        [PermissionFilter(MenuCode.CustomRole)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> Delete(long id)
        {
            var role = await customRoleRepository.GetByIdAsync(id);
            if (role == null)
            {
                return JsonNotFound($"Custom role Id {id} is not found.");
            }
            if (role.TenantId != CurrentUserInfo.SelectedTenant.Id)
            {
                return JsonNotFound($"Custom role Id {id} is not found.");
            }

            await customRoleRepository.DeleteAsync(id, unitOfWork);
            return NoContent();
        }
    }
}
