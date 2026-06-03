using Nssol.Platypus.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nssol.Platypus.Models.TenantModels
{
    /// <summary>
    /// リソース個別のアクセス制御。データセットやモデルに対する個別のアクセス許可を管理する。
    /// </summary>
    public class ResourcePermission : TenantModelBase
    {
        /// <summary>
        /// リソース種別
        /// </summary>
        [Required]
        public ResourceType2 ResourceType { get; set; }

        /// <summary>
        /// リソースID（DataSetまたはTrainingHistoryのID）
        /// </summary>
        [Required]
        public long ResourceId { get; set; }

        /// <summary>
        /// アクセスを許可するユーザーID
        /// </summary>
        [Required]
        public long UserId { get; set; }

        /// <summary>
        /// 許可するアクセスレベル
        /// </summary>
        [Required]
        public ProjectRoleType AccessLevel { get; set; }

        /// <summary>
        /// ユーザー
        /// </summary>
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }
    }
}
