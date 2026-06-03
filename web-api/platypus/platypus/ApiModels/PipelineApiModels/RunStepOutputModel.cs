namespace Nssol.Platypus.ApiModels.PipelineApiModels
{
    /// <summary>
    /// パイプライン実行ステップ出力モデル
    /// </summary>
    public class RunStepOutputModel
    {
        /// <summary>
        /// ステップID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 対応ノードID
        /// </summary>
        public long PipelineNodeId { get; set; }

        /// <summary>
        /// ノード名
        /// </summary>
        public string NodeName { get; set; }

        /// <summary>
        /// ジョブ種別
        /// </summary>
        public string JobType { get; set; }

        /// <summary>
        /// ステータス
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 実行された学習履歴ID
        /// </summary>
        public long? TrainingHistoryId { get; set; }

        /// <summary>
        /// 実行された推論履歴ID
        /// </summary>
        public long? InferenceHistoryId { get; set; }

        /// <summary>
        /// 実行された前処理履歴ID
        /// </summary>
        public long? PreprocessHistoryId { get; set; }

        /// <summary>
        /// ステップ実行開始日時
        /// </summary>
        public string StartedAt { get; set; }

        /// <summary>
        /// ステップ実行完了日時
        /// </summary>
        public string CompletedAt { get; set; }
    }
}
