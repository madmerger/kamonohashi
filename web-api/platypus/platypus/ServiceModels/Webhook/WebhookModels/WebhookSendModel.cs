using Nssol.Platypus.Models;

namespace Nssol.Platypus.ServiceModels.Webhook.WebhookModels
{
    /// <summary>
    /// カスタムWebhookのメッセージ送信用モデル
    /// </summary>
    public class WebhookSendModel
    {
        /// <summary>
        /// 送信先URL
        /// </summary>
        public string WebhookUrl { get; set; }

        /// <summary>
        /// イベント種別（JobCompleted, JobFailed, JobKilled, etc.）
        /// </summary>
        public string Event { get; set; }

        /// <summary>
        /// 対象ジョブの履歴ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 対象ジョブの名前
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 対象ジョブのテナント
        /// </summary>
        public Tenant Tenant { get; set; }

        /// <summary>
        /// 対象ジョブの作成者名
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// 対象ジョブのステータス
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 対象ジョブのURL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// ジョブ種別（Training / Inference）
        /// </summary>
        public string JobType { get; set; }

        /// <summary>
        /// カスタムメッセージ
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// カスタム通知テンプレート（RazorLight形式）。nullの場合はデフォルトテンプレートを使用。
        /// </summary>
        public string CustomTemplate { get; set; }
    }
}
