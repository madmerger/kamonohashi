using Microsoft.EntityFrameworkCore;
using Nssol.Platypus.DataAccess.Core;
using Nssol.Platypus.DataAccess.Repositories.Interfaces.TenantRepositories;
using Nssol.Platypus.Infrastructure;
using Nssol.Platypus.Models.TenantModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nssol.Platypus.DataAccess.Repositories.TenantRepositories
{
    /// <summary>
    /// プロジェクトテーブルにアクセスするためのリポジトリクラス
    /// </summary>
    public class ProjectRepository : RepositoryForTenantBase<Project>, IProjectRepository
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ProjectRepository(CommonDbContext context, Microsoft.AspNetCore.Http.IHttpContextAccessor accessor)
            : base(context, accessor)
        {
        }

        /// <summary>
        /// プロジェクトメンバーを追加する
        /// </summary>
        public void AddMember(ProjectMember member)
        {
            member.TenantId = CurrentTenantId;
            GetDbSet<ProjectMember>().Add(member);
        }

        /// <summary>
        /// プロジェクトメンバーを削除する
        /// </summary>
        public void RemoveMember(ProjectMember member)
        {
            GetDbSet<ProjectMember>().Remove(member);
        }

        /// <summary>
        /// プロジェクトのメンバー一覧を取得する
        /// </summary>
        public async Task<IEnumerable<ProjectMember>> GetMembersAsync(long projectId)
        {
            return await GetDbSet<ProjectMember>()
                .Include(m => m.User)
                .Where(m => m.TenantId == CurrentTenantId && m.ProjectId == projectId)
                .ToListAsync();
        }

        /// <summary>
        /// 指定ユーザーのプロジェクト内での権限を取得する
        /// </summary>
        public async Task<ProjectMember> GetMemberAsync(long projectId, long userId)
        {
            return await GetDbSet<ProjectMember>()
                .Where(m => m.TenantId == CurrentTenantId && m.ProjectId == projectId && m.UserId == userId)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// プロジェクトにリソースを追加する
        /// </summary>
        public void AddResource(ProjectResourceMap resourceMap)
        {
            resourceMap.TenantId = CurrentTenantId;
            GetDbSet<ProjectResourceMap>().Add(resourceMap);
        }

        /// <summary>
        /// プロジェクトからリソースを削除する
        /// </summary>
        public void RemoveResource(ProjectResourceMap resourceMap)
        {
            GetDbSet<ProjectResourceMap>().Remove(resourceMap);
        }

        /// <summary>
        /// プロジェクトのリソース一覧を取得する
        /// </summary>
        public async Task<IEnumerable<ProjectResourceMap>> GetResourcesAsync(long projectId)
        {
            return await GetDbSet<ProjectResourceMap>()
                .Where(r => r.TenantId == CurrentTenantId && r.ProjectId == projectId)
                .ToListAsync();
        }

        /// <summary>
        /// ユーザーがアクセス可能なプロジェクトを取得する
        /// </summary>
        public async Task<IEnumerable<Project>> GetProjectsByUserAsync(long userId)
        {
            var projectIds = await GetDbSet<ProjectMember>()
                .Where(m => m.TenantId == CurrentTenantId && m.UserId == userId)
                .Select(m => m.ProjectId)
                .ToListAsync();

            return await GetAll()
                .Where(p => projectIds.Contains(p.Id))
                .OrderByDescending(p => p.Id)
                .ToListAsync();
        }

        /// <summary>
        /// リソースに対する個別アクセス権限を追加する
        /// </summary>
        public void AddResourcePermission(ResourcePermission permission)
        {
            permission.TenantId = CurrentTenantId;
            GetDbSet<ResourcePermission>().Add(permission);
        }

        /// <summary>
        /// リソースに対する個別アクセス権限を削除する
        /// </summary>
        public void RemoveResourcePermission(ResourcePermission permission)
        {
            GetDbSet<ResourcePermission>().Remove(permission);
        }

        /// <summary>
        /// 指定リソースのアクセス権限一覧を取得する
        /// </summary>
        public async Task<IEnumerable<ResourcePermission>> GetResourcePermissionsAsync(ResourceType2 resourceType, long resourceId)
        {
            return await GetDbSet<ResourcePermission>()
                .Include(p => p.User)
                .Where(p => p.TenantId == CurrentTenantId && p.ResourceType == resourceType && p.ResourceId == resourceId)
                .ToListAsync();
        }

        /// <summary>
        /// 指定ユーザーが特定リソースにアクセスできるか確認する
        /// </summary>
        public async Task<bool> HasResourceAccessAsync(long userId, ResourceType2 resourceType, long resourceId, ProjectRoleType requiredLevel)
        {
            // 個別リソース権限をチェック
            var directPermission = await GetDbSet<ResourcePermission>()
                .Where(p => p.TenantId == CurrentTenantId
                    && p.ResourceType == resourceType
                    && p.ResourceId == resourceId
                    && p.UserId == userId
                    && p.AccessLevel >= requiredLevel)
                .AnyAsync();

            if (directPermission)
            {
                return true;
            }

            // プロジェクト経由のアクセスをチェック
            var projectIds = await GetDbSet<ProjectResourceMap>()
                .Where(r => r.TenantId == CurrentTenantId
                    && r.ResourceType == resourceType
                    && r.ResourceId == resourceId)
                .Select(r => r.ProjectId)
                .ToListAsync();

            if (projectIds.Count == 0)
            {
                // どのプロジェクトにも所属していないリソースは全ユーザーがアクセス可能
                return true;
            }

            return await GetDbSet<ProjectMember>()
                .Where(m => m.TenantId == CurrentTenantId
                    && projectIds.Contains(m.ProjectId)
                    && m.UserId == userId
                    && m.RoleType >= requiredLevel)
                .AnyAsync();
        }
    }
}
