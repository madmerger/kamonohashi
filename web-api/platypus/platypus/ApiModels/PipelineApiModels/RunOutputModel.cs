using System.Collections.Generic;

namespace Nssol.Platypus.ApiModels.PipelineApiModels
{
    /// <summary>
    /// パイプライン実行出力モデル
    /// </summary>
    public class RunOutputModel
    {
        /// <summary>
        /// Run ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// パイプラインID
        /// </summary>
        public long PipelineId { get; set; }

        /// <summary>
        /// パイプライン名
        /// </summary>
        public string PipelineName { get; set; }

        /// <summary>
        /// ステータス
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 実行開始日時
        /// </summary>
        public string StartedAt { get; set; }

        /// <summary>
        /// 実行完了日時
        /// </summary>
        public string CompletedAt { get; set; }

        /// <summary>
        /// メモ
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 各ステップの状態
        /// </summary>
        public IEnumerable<RunStepOutputModel> Steps { get; set; }

        /// <summary>
        /// 登録日時
        /// </summary>
        public string CreatedAt { get; set; }
    }
}
