using Nssol.Platypus.ApiModels.Components;
using Nssol.Platypus.Models.TenantModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nssol.Platypus.ApiModels.HpoApiModels
{
    /// <summary>
    /// 新規HPOジョブ実行モデル
    /// </summary>
    public class CreateInputModel
    {
        /// <summary>
        /// 識別名
        /// </summary>
        [Required]
        [MinLength(1)]
        public string Name { get; set; }

        /// <summary>
        /// 探索アルゴリズム（Grid, Random, Bayes）
        /// </summary>
        [Required]
        public string Algorithm { get; set; }

        /// <summary>
        /// 探索空間定義
        /// </summary>
        [Required]
        public List<SearchSpaceParameterModel> SearchSpace { get; set; }

        /// <summary>
        /// 最大トライアル数
        /// </summary>
        [Required]
        [Range(1, 1000)]
        public int? MaxTrials { get; set; }

        /// <summary>
        /// 目的メトリクス名
        /// </summary>
        [Required]
        public string ObjectiveMetric { get; set; }

        /// <summary>
        /// 最適化方向（minimize / maximize）
        /// </summary>
        [Required]
        public string OptimizationDirection { get; set; }

        /// <summary>
        /// コンテナ情報
        /// </summary>
        [Required]
        public ContainerImageInputModel ContainerImage { get; set; }

        /// <summary>
        /// データセットID
        /// </summary>
        [Required]
        public long? DataSetId { get; set; }

        /// <summary>
        /// 学習モデルGit情報
        /// </summary>
        [Required]
        public GitCommitInputModel GitModel { get; set; }

        /// <summary>
        /// ジョブ実行コマンド
        /// </summary>
        [Required]
        public string EntryPoint { get; set; }

        /// <summary>
        /// CPUコア数
        /// </summary>
        [Required]
        public int? Cpu { get; set; }

        /// <summary>
        /// メモリ数(GB)
        /// </summary>
        [Required]
        public int? Memory { get; set; }

        /// <summary>
        /// GPU数
        /// </summary>
        [Required]
        public int? Gpu { get; set; }

        /// <summary>
        /// パーティション
        /// </summary>
        public string Partition { get; set; }

        /// <summary>
        /// メモ
        /// </summary>
        public string Memo { get; set; }
    }

    /// <summary>
    /// 探索空間パラメータモデル
    /// </summary>
    public class SearchSpaceParameterModel
    {
        /// <summary>
        /// パラメータ名
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// 型（int, float, categorical）
        /// </summary>
        [Required]
        public string Type { get; set; }

        /// <summary>
        /// 最小値（int/float用）
        /// </summary>
        public double? Min { get; set; }

        /// <summary>
        /// 最大値（int/float用）
        /// </summary>
        public double? Max { get; set; }

        /// <summary>
        /// ステップ（グリッドサーチ用）
        /// </summary>
        public double? Step { get; set; }

        /// <summary>
        /// カテゴリ値リスト（categorical用）
        /// </summary>
        public List<string> Values { get; set; }
    }
}
