using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nssol.Platypus.ApiModels.ProjectApiModels;
using Nssol.Platypus.Controllers.Util;
using Nssol.Platypus.DataAccess.Core;
using Nssol.Platypus.DataAccess.Repositories.Interfaces.TenantRepositories;
using Nssol.Platypus.Filters;
using Nssol.Platypus.Infrastructure;
using Nssol.Platypus.Models.TenantModels;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Nssol.Platypus.Controllers.spa
{
    /// <summary>
    /// プロジェクト管理を扱うためのAPI集
    /// </summary>
    [ApiController]
    [ApiVersion("1"), ApiVersion("2")]
    [Route("api/v{api-version:apiVersion}/tenant/projects")]
    public class ProjectController : PlatypusApiControllerBase
    {
        private readonly IProjectRepository projectRepository;
        private readonly IUnitOfWork unitOfWork;

        public ProjectController(
            IProjectRepository projectRepository,
            IUnitOfWork unitOfWork,
            IHttpContextAccessor accessor) : base(accessor)
        {
            this.projectRepository = projectRepository;
            this.unitOfWork = unitOfWork;
        }

        /// <summary>
        /// プロジェクト一覧を取得
        /// </summary>
        [HttpGet]
        [PermissionFilter(MenuCode.ProjectManagement)]
        [ProducesResponseType(typeof(IEnumerable<IndexOutputModel>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAll()
        {
            var projects = projectRepository.GetAll();
            var result = new List<IndexOutputModel>();
            await foreach (var p in projects.AsAsyncEnumerable())
            {
                result.Add(new IndexOutputModel(p));
            }
            return JsonOK(result.OrderByDescending(p => p.Id));
        }

        /// <summary>
        /// 自分がアクセス可能なプロジェクト一覧を取得
        /// </summary>
        [HttpGet("mine")]
        [ProducesResponseType(typeof(IEnumerable<IndexOutputModel>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetMyProjects()
        {
            var projects = await projectRepository.GetProjectsByUserAsync(CurrentUserInfo.Id);
            return JsonOK(projects.Select(p => new IndexOutputModel(p)));
        }

        /// <summary>
        /// プロジェクト詳細を取得
        /// </summary>
        [HttpGet("{id}")]
        [PermissionFilter(MenuCode.ProjectManagement)]
        [ProducesResponseType(typeof(IndexOutputModel), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetById(long id)
        {
            var project = await projectRepository.GetByIdAsync(id);
            if (project == null)
            {
                return JsonNotFound($"Project Id {id} is not found.");
            }
            return JsonOK(new IndexOutputModel(project));
        }

        /// <summary>
        /// プロジェクトを新規作成する
        /// </summary>
        [HttpPost]
        [PermissionFilter(MenuCode.ProjectManagement)]
        [ProducesResponseType(typeof(IndexOutputModel), (int)HttpStatusCode.Created)]
        public IActionResult Create([FromBody] CreateInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return JsonBadRequest("Invalid inputs.");
            }

            var project = new Project
            {
                Name = model.Name,
                Description = model.Description,
            };

            projectRepository.Add(project);

            // 作成者をManagerとして追加（navigation propertyで単一コミット）
            var member = new ProjectMember
            {
                Project = project,
                UserId = CurrentUserInfo.Id,
                RoleType = ProjectRoleType.Manager,
            };
            projectRepository.AddMember(member);
            unitOfWork.Commit();

            return JsonCreated(new IndexOutputModel(project));
        }

        /// <summary>
        /// プロジェクトを編集する
        /// </summary>
        [HttpPut("{id}")]
        [PermissionFilter(MenuCode.ProjectManagement)]
        [ProducesResponseType(typeof(IndexOutputModel), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Edit(long id, [FromBody] EditInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return JsonBadRequest("Invalid inputs.");
            }

            var project = await projectRepository.GetByIdAsync(id);
            if (project == null)
            {
                return JsonNotFound($"Project Id {id} is not found.");
            }

            project.Name = model.Name;
            project.Description = model.Description;

            unitOfWork.Commit();
            return JsonOK(new IndexOutputModel(project));
        }

        /// <summary>
        /// プロジェクトを削除する
        /// </summary>
        [HttpDelete("{id}")]
        [PermissionFilter(MenuCode.ProjectManagement)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> Delete(long id)
        {
            var project = await projectRepository.GetByIdAsync(id);
            if (project == null)
            {
                return JsonNotFound($"Project Id {id} is not found.");
            }

            projectRepository.Delete(project);
            unitOfWork.Commit();
            return NoContent();
        }

        #region メンバー管理

        /// <summary>
        /// プロジェクトメンバー一覧を取得
        /// </summary>
        [HttpGet("{id}/members")]
        [PermissionFilter(MenuCode.ProjectManagement)]
        [ProducesResponseType(typeof(IEnumerable<MemberOutputModel>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetMembers(long id)
        {
            var project = await projectRepository.GetByIdAsync(id);
            if (project == null)
            {
                return JsonNotFound($"Project Id {id} is not found.");
            }

            var members = await projectRepository.GetMembersAsync(id);
            return JsonOK(members.Select(m => new MemberOutputModel(m)));
        }

        /// <summary>
        /// プロジェクトメンバーを追加
        /// </summary>
        [HttpPost("{id}/members")]
        [PermissionFilter(MenuCode.ProjectManagement)]
        [ProducesResponseType(typeof(MemberOutputModel), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> AddMember(long id, [FromBody] MemberInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return JsonBadRequest("Invalid inputs.");
            }

            var project = await projectRepository.GetByIdAsync(id);
            if (project == null)
            {
                return JsonNotFound($"Project Id {id} is not found.");
            }

            var existing = await projectRepository.GetMemberAsync(id, model.UserId);
            if (existing != null)
            {
                return JsonBadRequest($"User {model.UserId} is already a member of this project.");
            }

            var member = new ProjectMember
            {
                ProjectId = id,
                UserId = model.UserId,
                RoleType = model.RoleType,
            };
            projectRepository.AddMember(member);
            unitOfWork.Commit();

            // User navigation propertyを読み込んでレスポンスに含める
            var saved = await projectRepository.GetMemberAsync(id, model.UserId);
            return JsonCreated(new MemberOutputModel(saved ?? member));
        }

        /// <summary>
        /// プロジェクトメンバーの権限を変更
        /// </summary>
        [HttpPut("{id}/members/{userId}")]
        [PermissionFilter(MenuCode.ProjectManagement)]
        [ProducesResponseType(typeof(MemberOutputModel), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateMember(long id, long userId, [FromBody] MemberInputModel model)
        {
            var member = await projectRepository.GetMemberAsync(id, userId);
            if (member == null)
            {
                return JsonNotFound($"User {userId} is not a member of project {id}.");
            }

            member.RoleType = model.RoleType;
            unitOfWork.Commit();
            return JsonOK(new MemberOutputModel(member));
        }

        /// <summary>
        /// プロジェクトメンバーを削除
        /// </summary>
        [HttpDelete("{id}/members/{userId}")]
        [PermissionFilter(MenuCode.ProjectManagement)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> RemoveMember(long id, long userId)
        {
            var member = await projectRepository.GetMemberAsync(id, userId);
            if (member == null)
            {
                return JsonNotFound($"User {userId} is not a member of project {id}.");
            }

            projectRepository.RemoveMember(member);
            unitOfWork.Commit();
            return NoContent();
        }

        #endregion

        #region リソース管理

        /// <summary>
        /// プロジェクトのリソース一覧を取得
        /// </summary>
        [HttpGet("{id}/resources")]
        [PermissionFilter(MenuCode.ProjectManagement)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetResources(long id)
        {
            var project = await projectRepository.GetByIdAsync(id);
            if (project == null)
            {
                return JsonNotFound($"Project Id {id} is not found.");
            }

            var resources = await projectRepository.GetResourcesAsync(id);
            return JsonOK(resources.Select(r => new
            {
                r.Id,
                r.ResourceType,
                r.ResourceId,
            }));
        }

        /// <summary>
        /// プロジェクトにリソースを追加
        /// </summary>
        [HttpPost("{id}/resources")]
        [PermissionFilter(MenuCode.ProjectManagement)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> AddResource(long id, [FromBody] ResourceMapInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return JsonBadRequest("Invalid inputs.");
            }

            var project = await projectRepository.GetByIdAsync(id);
            if (project == null)
            {
                return JsonNotFound($"Project Id {id} is not found.");
            }

            var resourceMap = new ProjectResourceMap
            {
                ProjectId = id,
                ResourceType = model.ResourceType,
                ResourceId = model.ResourceId,
            };
            projectRepository.AddResource(resourceMap);
            unitOfWork.Commit();

            return JsonCreated(new { resourceMap.Id, resourceMap.ResourceType, resourceMap.ResourceId });
        }

        /// <summary>
        /// プロジェクトからリソースを削除
        /// </summary>
        [HttpDelete("{id}/resources/{resourceMapId}")]
        [PermissionFilter(MenuCode.ProjectManagement)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> RemoveResource(long id, long resourceMapId)
        {
            var resources = await projectRepository.GetResourcesAsync(id);
            var resource = resources.FirstOrDefault(r => r.Id == resourceMapId);
            if (resource == null)
            {
                return JsonNotFound($"Resource map {resourceMapId} is not found in project {id}.");
            }

            projectRepository.RemoveResource(resource);
            unitOfWork.Commit();
            return NoContent();
        }

        #endregion
    }
}
