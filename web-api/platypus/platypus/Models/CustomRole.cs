using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nssol.Platypus.Models
{
    /// <summary>
    /// カスタムロール定義。テナント単位で作成可能なロールで、細かい権限フラグを持つ。
    /// </summary>
    public class CustomRole : ModelBase
    {
        /// <summary>
        /// ロール名
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// 表示名
        /// </summary>
        [Required]
        public string DisplayName { get; set; }

        /// <summary>
        /// テナントID
        /// </summary>
        [Required]
        public long TenantId { get; set; }

        /// <summary>
        /// 説明
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// データ管理の権限
        /// </summary>
        public bool CanManageData { get; set; }

        /// <summary>
        /// データセット管理の権限
        /// </summary>
        public bool CanManageDataSet { get; set; }

        /// <summary>
        /// 学習実行の権限
        /// </summary>
        public bool CanRunTraining { get; set; }

        /// <summary>
        /// 推論実行の権限
        /// </summary>
        public bool CanRunInference { get; set; }

        /// <summary>
        /// ノートブック使用の権限
        /// </summary>
        public bool CanUseNotebook { get; set; }

        /// <summary>
        /// プロジェクト管理の権限
        /// </summary>
        public bool CanManageProject { get; set; }

        /// <summary>
        /// テナント設定変更の権限
        /// </summary>
        public bool CanEditTenantSetting { get; set; }

        /// <summary>
        /// リソース権限管理の権限
        /// </summary>
        public bool CanManageResourcePermission { get; set; }

        /// <summary>
        /// テナント
        /// </summary>
        [ForeignKey(nameof(TenantId))]
        public virtual Tenant Tenant { get; set; }
    }
}
