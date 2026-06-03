using Nssol.Platypus.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nssol.Platypus.Models.TenantModels
{
    /// <summary>
    /// プロジェクトとリソースの紐づけ。データセットや学習履歴をプロジェクトに所属させる。
    /// </summary>
    public class ProjectResourceMap : TenantModelBase
    {
        /// <summary>
        /// プロジェクトID
        /// </summary>
        [Required]
        public long ProjectId { get; set; }

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
        /// プロジェクト
        /// </summary>
        [ForeignKey(nameof(ProjectId))]
        public virtual Project Project { get; set; }
    }
}
