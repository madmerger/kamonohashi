using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nssol.Platypus.Models.TenantModels
{
    /// <summary>
    /// HPOトライアル（HPOジョブの子ジョブ）
    /// </summary>
    public class HpoTrial : TenantModelBase
    {
        /// <summary>
        /// トライアル番号（HPOジョブ内の連番）
        /// </summary>
        [Required]
        public int TrialNo { get; set; }

        /// <summary>
        /// 親HPOジョブID
        /// </summary>
        [Required]
        public long HpoJobId { get; set; }

        /// <summary>
        /// 対応する学習履歴ID（TrainingHistoryとの紐付け）
        /// </summary>
        public long? TrainingHistoryId { get; set; }

        /// <summary>
        /// ハイパーパラメータ値（JSON）
        /// </summary>
        [Required]
        public string Parameters { get; set; }

        /// <summary>
        /// <see cref="Parameters"/> のディクショナリ表現
        /// </summary>
        [NotMapped]
        public Dictionary<string, string> ParametersDic
        {
            get => string.IsNullOrEmpty(Parameters)
                ? new Dictionary<string, string>()
                : JsonConvert.DeserializeObject<Dictionary<string, string>>(Parameters);
            set => Parameters = JsonConvert.SerializeObject(value);
        }

        /// <summary>
        /// メトリクス値（目的関数の結果）
        /// </summary>
        public double? MetricValue { get; set; }

        /// <summary>
        /// ステータス（Pending, Running, Completed, Failed）
        /// </summary>
        [Required]
        public string Status { get; set; }

        /// <summary>
        /// 実行開始日時
        /// </summary>
        public DateTime? StartedAt { get; set; }

        /// <summary>
        /// 完了日時
        /// </summary>
        public DateTime? CompletedAt { get; set; }

        /// <summary>
        /// 親HPOジョブ
        /// </summary>
        [ForeignKey(nameof(HpoJobId))]
        public virtual HpoJob HpoJob { get; set; }

        /// <summary>
        /// 対応する学習履歴
        /// </summary>
        [ForeignKey(nameof(TrainingHistoryId))]
        public virtual TrainingHistory TrainingHistory { get; set; }
    }
}
