using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nssol.Platypus.Models.TenantModels
{
    /// <summary>
    /// パイプライン実行の各ステップ（ノード毎の実行状態）
    /// </summary>
    public class PipelineRunStep : TenantModelBase
    {
        /// <summary>
        /// 所属パイプラインRunID
        /// </summary>
        [Required]
        public long PipelineRunId { get; set; }

        /// <summary>
        /// 所属パイプラインRun
        /// </summary>
        [ForeignKey(nameof(PipelineRunId))]
        public virtual PipelineRun PipelineRun { get; set; }

        /// <summary>
        /// 対応するパイプラインノードID
        /// </summary>
        [Required]
        public long PipelineNodeId { get; set; }

        /// <summary>
        /// 対応するパイプラインノード
        /// </summary>
        [ForeignKey(nameof(PipelineNodeId))]
        public virtual PipelineNode PipelineNode { get; set; }

        /// <summary>
        /// ステータス: Pending / Running / Completed / Failed / Skipped
        /// </summary>
        [Required]
        public string Status { get; set; }

        /// <summary>
        /// 実行された学習履歴ID（Training/Inferenceの場合）
        /// </summary>
        public long? TrainingHistoryId { get; set; }

        /// <summary>
        /// 実行された学習履歴
        /// </summary>
        [ForeignKey(nameof(TrainingHistoryId))]
        public virtual TrainingHistory TrainingHistory { get; set; }

        /// <summary>
        /// 実行された推論履歴ID
        /// </summary>
        public long? InferenceHistoryId { get; set; }

        /// <summary>
        /// 実行された推論履歴
        /// </summary>
        [ForeignKey(nameof(InferenceHistoryId))]
        public virtual InferenceHistory InferenceHistory { get; set; }

        /// <summary>
        /// 実行された前処理履歴ID
        /// </summary>
        public long? PreprocessHistoryId { get; set; }

        /// <summary>
        /// 実行された前処理履歴
        /// </summary>
        [ForeignKey(nameof(PreprocessHistoryId))]
        public virtual PreprocessHistory PreprocessHistory { get; set; }

        /// <summary>
        /// ステップ実行開始日時
        /// </summary>
        public DateTime? StartedAt { get; set; }

        /// <summary>
        /// ステップ実行完了日時
        /// </summary>
        public DateTime? CompletedAt { get; set; }
    }
}
