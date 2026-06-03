namespace Nssol.Platypus.ApiModels.TenantApiModels
{
    /// <summary>
    /// テナントWebhook設定の出力モデル
    /// </summary>
    public class WebhookSettingOutputModel
    {
        /// <summary>
        /// カスタムWebhookの送信先URL
        /// </summary>
        public string WebhookUrl { get; set; }

        /// <summary>
        /// Slack通知テンプレート（カスタム）
        /// </summary>
        public string SlackNotificationTemplate { get; set; }

        /// <summary>
        /// Webhook通知テンプレート（カスタム）
        /// </summary>
        public string WebhookNotificationTemplate { get; set; }
    }
}
