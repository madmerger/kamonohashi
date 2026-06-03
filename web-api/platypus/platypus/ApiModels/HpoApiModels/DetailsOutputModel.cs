using Nssol.Platypus.Models.TenantModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nssol.Platypus.ApiModels.HpoApiModels
{
    /// <summary>
    /// HPOジョブ詳細出力モデル
    /// </summary>
    public class DetailsOutputModel : IndexOutputModel
    {
        public DetailsOutputModel(HpoJob hpoJob) : base(hpoJob)
        {
            SearchSpace = hpoJob.SearchSpaceParsed?.Select(p => new SearchSpaceParameterModel
            {
                Name = p.Name,
                Type = p.Type,
                Min = p.Min,
                Max = p.Max,
                Step = p.Step,
                Values = p.Values
            }).ToList();

            ContainerImage = hpoJob.ContainerImage;
            ContainerTag = hpoJob.ContainerTag;
            EntryPoint = hpoJob.EntryPoint;
            Cpu = hpoJob.Cpu;
            Memory = hpoJob.Memory;
            Gpu = hpoJob.Gpu;
            Partition = hpoJob.Partition;
            DataSetId = hpoJob.DataSetId;

            Trials = hpoJob.Trials?.Select(t => new TrialOutputModel(t)).ToList()
                ?? new List<TrialOutputModel>();

            BestTrial = GetBestTrial(hpoJob);
        }

        /// <summary>
        /// 探索空間定義
        /// </summary>
        public List<SearchSpaceParameterModel> SearchSpace { get; set; }

        /// <summary>
        /// コンテナイメージ名
        /// </summary>
        public string ContainerImage { get; set; }

        /// <summary>
        /// コンテナタグ
        /// </summary>
        public string ContainerTag { get; set; }

        /// <summary>
        /// 実行コマンド
        /// </summary>
        public string EntryPoint { get; set; }

        /// <summary>
        /// CPUコア数
        /// </summary>
        public int Cpu { get; set; }

        /// <summary>
        /// メモリ（GB）
        /// </summary>
        public int Memory { get; set; }

        /// <summary>
        /// GPU数
        /// </summary>
        public int Gpu { get; set; }

        /// <summary>
        /// パーティション
        /// </summary>
        public string Partition { get; set; }

        /// <summary>
        /// データセットID
        /// </summary>
        public long DataSetId { get; set; }

        /// <summary>
        /// トライアル一覧
        /// </summary>
        public List<TrialOutputModel> Trials { get; set; }

        /// <summary>
        /// ベストトライアル（最適パラメータ推薦）
        /// </summary>
        public TrialOutputModel BestTrial { get; set; }

        private TrialOutputModel GetBestTrial(HpoJob hpoJob)
        {
            var completedTrials = hpoJob.Trials?.Where(t => t.Status == "Completed" && t.MetricValue.HasValue);
            if (completedTrials == null || !completedTrials.Any())
            {
                return null;
            }

            var best = hpoJob.OptimizationDirection == "minimize"
                ? completedTrials.OrderBy(t => t.MetricValue).First()
                : completedTrials.OrderByDescending(t => t.MetricValue).First();

            return new TrialOutputModel(best);
        }
    }

    /// <summary>
    /// トライアル出力モデル
    /// </summary>
    public class TrialOutputModel
    {
        public TrialOutputModel(HpoTrial trial)
        {
            Id = trial.Id;
            TrialNo = trial.TrialNo;
            Parameters = trial.ParametersDic;
            MetricValue = trial.MetricValue;
            Status = trial.Status;
            TrainingHistoryId = trial.TrainingHistoryId;
            StartedAt = trial.StartedAt?.ToString("yyyy/MM/dd HH:mm:ss");
            CompletedAt = trial.CompletedAt?.ToString("yyyy/MM/dd HH:mm:ss");
        }

        /// <summary>
        /// ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// トライアル番号
        /// </summary>
        public int TrialNo { get; set; }

        /// <summary>
        /// ハイパーパラメータ値
        /// </summary>
        public Dictionary<string, string> Parameters { get; set; }

        /// <summary>
        /// メトリクス値
        /// </summary>
        public double? MetricValue { get; set; }

        /// <summary>
        /// ステータス
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 対応する学習履歴ID
        /// </summary>
        public long? TrainingHistoryId { get; set; }

        /// <summary>
        /// 開始日時
        /// </summary>
        public string StartedAt { get; set; }

        /// <summary>
        /// 完了日時
        /// </summary>
        public string CompletedAt { get; set; }
    }
}
