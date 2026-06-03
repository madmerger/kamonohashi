using System.Collections.Generic;

namespace Nssol.Platypus.ApiModels.TrainingApiModels
{
    /// <summary>
    /// 学習履歴比較の出力モデル
    /// </summary>
    public class CompareOutputModel
    {
        /// <summary>
        /// 比較対象の学習ジョブ一覧
        /// </summary>
        public List<CompareJobOutputModel> Jobs { get; set; }
    }

    /// <summary>
    /// 比較対象の個別ジョブ情報
    /// </summary>
    public class CompareJobOutputModel
    {
        /// <summary>
        /// 学習履歴ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 学習名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// ステータス
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 実行者
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// 開始日時
        /// </summary>
        public string StartedAt { get; set; }

        /// <summary>
        /// 完了日時
        /// </summary>
        public string CompletedAt { get; set; }

        /// <summary>
        /// エントリポイント
        /// </summary>
        public string EntryPoint { get; set; }

        /// <summary>
        /// ハイパーパラメータ（オプション）
        /// </summary>
        public List<KeyValuePair<string, string>> Options { get; set; }

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
        /// データセット名
        /// </summary>
        public string DataSetName { get; set; }

        /// <summary>
        /// コンテナイメージ
        /// </summary>
        public string ContainerImage { get; set; }

        /// <summary>
        /// ログ要約（メトリクス情報を含む）
        /// </summary>
        public string LogSummary { get; set; }

        /// <summary>
        /// メモ
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// タグ一覧
        /// </summary>
        public IEnumerable<string> Tags { get; set; }

        /// <summary>
        /// 実行時間
        /// </summary>
        public string ExecutionTime { get; set; }
    }
}
