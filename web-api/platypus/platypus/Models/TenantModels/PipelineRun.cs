using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nssol.Platypus.Models.TenantModels
{
    /// <summary>
    /// パイプラインの実行インスタンス
    /// </summary>
    public class PipelineRun : TenantModelBase
    {
        /// <summary>
        /// 所属パイプラインID
        /// </summary>
        [Required]
        public long PipelineId { get; set; }

        /// <summary>
        /// 所属パイプライン
        /// </summary>
        [ForeignKey(nameof(PipelineId))]
        public virtual Pipeline Pipeline { get; set; }

        /// <summary>
        /// ステータス: Pending / Running / Completed / Failed / Cancelled
        /// </summary>
        [Required]
        public string Status { get; set; }

        /// <summary>
        /// 実行開始日時
        /// </summary>
        public DateTime? StartedAt { get; set; }

        /// <summary>
        /// 実行完了日時
        /// </summary>
        public DateTime? CompletedAt { get; set; }

        /// <summary>
        /// メモ
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 各ステップの実行状態
        /// </summary>
        public virtual ICollection<PipelineRunStep> Steps { get; set; }
    }
}
