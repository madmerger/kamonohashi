using Nssol.Platypus.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nssol.Platypus.Models.TenantModels
{
    /// <summary>
    /// プロジェクトメンバー。ユーザーとプロジェクトの紐づけ、およびプロジェクト内での権限レベルを管理する。
    /// </summary>
    public class ProjectMember : TenantModelBase
    {
        /// <summary>
        /// プロジェクトID
        /// </summary>
        [Required]
        public long ProjectId { get; set; }

        /// <summary>
        /// ユーザーID
        /// </summary>
        [Required]
        public long UserId { get; set; }

        /// <summary>
        /// プロジェクト内のロール種別
        /// </summary>
        [Required]
        public ProjectRoleType RoleType { get; set; }

        /// <summary>
        /// プロジェクト
        /// </summary>
        [ForeignKey(nameof(ProjectId))]
        public virtual Project Project { get; set; }

        /// <summary>
        /// ユーザー
        /// </summary>
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }
    }
}
