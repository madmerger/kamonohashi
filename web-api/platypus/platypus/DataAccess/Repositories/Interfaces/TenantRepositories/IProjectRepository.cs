using Nssol.Platypus.DataAccess.Core;
using Nssol.Platypus.Infrastructure;
using Nssol.Platypus.Models.TenantModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nssol.Platypus.DataAccess.Repositories.Interfaces.TenantRepositories
{
    /// <summary>
    /// プロジェクトテーブルにアクセスするためのリポジトリインターフェイス
    /// </summary>
    public interface IProjectRepository : IRepositoryForTenant<Project>
    {
        /// <summary>
        /// プロジェクトメンバーを追加する
        /// </summary>
        void AddMember(ProjectMember member);

        /// <summary>
        /// プロジェクトメンバーを削除する
        /// </summary>
        void RemoveMember(ProjectMember member);

        /// <summary>
        /// プロジェクトのメンバー一覧を取得する
        /// </summary>
        Task<IEnumerable<ProjectMember>> GetMembersAsync(long projectId);

        /// <summary>
        /// 指定ユーザーのプロジェクト内での権限を取得する
        /// </summary>
        Task<ProjectMember> GetMemberAsync(long projectId, long userId);

        /// <summary>
        /// プロジェクトにリソースを追加する
        /// </summary>
        void AddResource(ProjectResourceMap resourceMap);

        /// <summary>
        /// プロジェクトからリソースを削除する
        /// </summary>
        void RemoveResource(ProjectResourceMap resourceMap);

        /// <summary>
        /// プロジェクトのリソース一覧を取得する
        /// </summary>
        Task<IEnumerable<ProjectResourceMap>> GetResourcesAsync(long projectId);

        /// <summary>
        /// ユーザーがアクセス可能なプロジェクトを取得する
        /// </summary>
        Task<IEnumerable<Project>> GetProjectsByUserAsync(long userId);

        /// <summary>
        /// リソースに対する個別アクセス権限を追加する
        /// </summary>
        void AddResourcePermission(ResourcePermission permission);

        /// <summary>
        /// リソースに対する個別アクセス権限を削除する
        /// </summary>
        void RemoveResourcePermission(ResourcePermission permission);

        /// <summary>
        /// 指定リソースのアクセス権限一覧を取得する
        /// </summary>
        Task<IEnumerable<ResourcePermission>> GetResourcePermissionsAsync(ResourceType2 resourceType, long resourceId);

        /// <summary>
        /// 指定ユーザーが特定リソースにアクセスできるか確認する
        /// </summary>
        Task<bool> HasResourceAccessAsync(long userId, ResourceType2 resourceType, long resourceId, ProjectRoleType requiredLevel);
    }
}
