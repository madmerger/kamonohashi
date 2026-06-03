using Newtonsoft.Json;
using Nssol.Platypus.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nssol.Platypus.Models.TenantModels
{
    /// <summary>
    /// ハイパーパラメータ最適化（HPO）ジョブ
    /// </summary>
    public class HpoJob : TenantModelBase
    {
        /// <summary>
        /// 表示用ID
        /// </summary>
        public long? DisplayId { get; set; }

        /// <summary>
        /// 名前
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// 探索アルゴリズム（Grid, Random, Bayes）
        /// </summary>
        [Required]
        public string Algorithm { get; set; }

        /// <summary>
        /// 探索空間定義（JSON）
        /// パラメータ名、型（int/float/categorical）、範囲を定義
        /// </summary>
        [Required]
        public string SearchSpace { get; set; }

        /// <summary>
        /// <see cref="SearchSpace"/> のオブジェクト表現
        /// </summary>
        [NotMapped]
        public List<HpoSearchSpaceParameter> SearchSpaceParsed
        {
            get => string.IsNullOrEmpty(SearchSpace)
                ? new List<HpoSearchSpaceParameter>()
                : JsonConvert.DeserializeObject<List<HpoSearchSpaceParameter>>(SearchSpace);
            set => SearchSpace = JsonConvert.SerializeObject(value);
        }

        /// <summary>
        /// 最大トライアル数
        /// </summary>
        [Required]
        public int MaxTrials { get; set; }

        /// <summary>
        /// 最適化方向（minimize / maximize）
        /// </summary>
        [Required]
        public string ObjectiveMetric { get; set; }

        /// <summary>
        /// 最適化方向
        /// </summary>
        [Required]
        public string OptimizationDirection { get; set; }

        /// <summary>
        /// データセットID
        /// </summary>
        [Required]
        public long DataSetId { get; set; }

        /// <summary>
        /// 学習モデルGit
        /// </summary>
        [Required]
        public long ModelGitId { get; set; }

        /// <summary>
        /// 学習モデルリポジトリ
        /// </summary>
        [Required]
        public string ModelRepository { get; set; }

        /// <summary>
        /// 学習モデルリポジトリオーナー
        /// </summary>
        [Required]
        public string ModelRepositoryOwner { get; set; }

        /// <summary>
        /// 学習モデルブランチ
        /// </summary>
        public string ModelBranch { get; set; }

        /// <summary>
        /// 学習モデルコミットID
        /// </summary>
        [Required]
        public string ModelCommitId { get; set; }

        /// <summary>
        /// エントリポイント（実行コマンド）
        /// </summary>
        [Required]
        public string EntryPoint { get; set; }

        /// <summary>
        /// Dockerリポジトリ
        /// </summary>
        [Required]
        public long? ContainerRegistryId { get; set; }

        /// <summary>
        /// コンテナイメージ名
        /// </summary>
        [Required]
        public string ContainerImage { get; set; }

        /// <summary>
        /// コンテナタグ
        /// </summary>
        [Required]
        public string ContainerTag { get; set; }

        /// <summary>
        /// CPUコア数
        /// </summary>
        public int Cpu { get; set; }

        /// <summary>
        /// メモリ容量（GB）
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
        /// ステータス（Running, Completed, Failed, Cancelled）
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
        /// メモ
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 子トライアル一覧
        /// </summary>
        public virtual ICollection<HpoTrial> Trials { get; set; }

        /// <summary>
        /// データセット
        /// </summary>
        [ForeignKey(nameof(DataSetId))]
        public virtual DataSet DataSet { get; set; }
    }

    /// <summary>
    /// 探索空間パラメータ定義
    /// </summary>
    public class HpoSearchSpaceParameter
    {
        /// <summary>
        /// パラメータ名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 型（int, float, categorical）
        /// </summary>
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
        /// ステップ（int/float用、グリッドサーチ用）
        /// </summary>
        public double? Step { get; set; }

        /// <summary>
        /// カテゴリ値リスト（categorical用）
        /// </summary>
        public List<string> Values { get; set; }
    }
}
