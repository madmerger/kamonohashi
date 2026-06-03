using Nssol.Platypus.Models.TenantModels;
using System;
using System.Linq;

namespace Nssol.Platypus.ApiModels.HpoApiModels
{
    /// <summary>
    /// HPOジョブ一覧出力モデル
    /// </summary>
    public class IndexOutputModel
    {
        public IndexOutputModel(HpoJob hpoJob)
        {
            Id = hpoJob.Id;
            DisplayId = hpoJob.DisplayId;
            Name = hpoJob.Name;
            Algorithm = hpoJob.Algorithm;
            Status = hpoJob.Status;
            MaxTrials = hpoJob.MaxTrials;
            CompletedTrials = hpoJob.Trials?.Count(t => t.Status == "Completed") ?? 0;
            TotalTrials = hpoJob.Trials?.Count ?? 0;
            ObjectiveMetric = hpoJob.ObjectiveMetric;
            OptimizationDirection = hpoJob.OptimizationDirection;
            BestMetricValue = GetBestMetricValue(hpoJob);
            DataSetName = hpoJob.DataSet?.Name;
            CreatedAt = hpoJob.CreatedAt.ToString("yyyy/MM/dd HH:mm:ss");
            StartedAt = hpoJob.StartedAt?.ToString("yyyy/MM/dd HH:mm:ss");
            CompletedAt = hpoJob.CompletedAt?.ToString("yyyy/MM/dd HH:mm:ss");
            Memo = hpoJob.Memo;
            CreatedBy = hpoJob.CreatedBy;
        }

        /// <summary>
        /// ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 表示用ID
        /// </summary>
        public long? DisplayId { get; set; }

        /// <summary>
        /// 名前
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 探索アルゴリズム
        /// </summary>
        public string Algorithm { get; set; }

        /// <summary>
        /// ステータス
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 最大トライアル数
        /// </summary>
        public int MaxTrials { get; set; }

        /// <summary>
        /// 完了トライアル数
        /// </summary>
        public int CompletedTrials { get; set; }

        /// <summary>
        /// 総トライアル数（生成済み）
        /// </summary>
        public int TotalTrials { get; set; }

        /// <summary>
        /// 目的メトリクス
        /// </summary>
        public string ObjectiveMetric { get; set; }

        /// <summary>
        /// 最適化方向
        /// </summary>
        public string OptimizationDirection { get; set; }

        /// <summary>
        /// ベストメトリクス値
        /// </summary>
        public double? BestMetricValue { get; set; }

        /// <summary>
        /// データセット名
        /// </summary>
        public string DataSetName { get; set; }

        /// <summary>
        /// 作成日時
        /// </summary>
        public string CreatedAt { get; set; }

        /// <summary>
        /// 開始日時
        /// </summary>
        public string StartedAt { get; set; }

        /// <summary>
        /// 完了日時
        /// </summary>
        public string CompletedAt { get; set; }

        /// <summary>
        /// メモ
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 作成者
        /// </summary>
        public string CreatedBy { get; set; }

        private double? GetBestMetricValue(HpoJob hpoJob)
        {
            var completedTrials = hpoJob.Trials?.Where(t => t.Status == "Completed" && t.MetricValue.HasValue);
            if (completedTrials == null || !completedTrials.Any())
            {
                return null;
            }

            return hpoJob.OptimizationDirection == "minimize"
                ? completedTrials.Min(t => t.MetricValue)
                : completedTrials.Max(t => t.MetricValue);
        }
    }
}
