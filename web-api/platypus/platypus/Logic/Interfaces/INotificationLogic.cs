using Nssol.Platypus.Infrastructure;
using Nssol.Platypus.Models.TenantModels;
using System.Threading.Tasks;

namespace Nssol.Platypus.Logic.Interfaces
{
    /// <summary>
    /// 通知ロジックインターフェース（Slack + カスタムWebhook）
    /// </summary>
    public interface INotificationLogic
    {
        /// <summary>
        /// 学習結果を通知する（Slack + Webhook）
        /// </summary>
        /// <param name="history">学習履歴モデル</param>
        void InformJobResult(TrainingHistory history);

        /// <summary>
        /// 推論結果を通知する（Slack + Webhook）
        /// </summary>
        /// <param name="history">推論履歴モデル</param>
        void InformJobResult(InferenceHistory history);

        /// <summary>
        /// Webhook テスト通知する
        /// </summary>
        /// <param name="webhookUrl">Webhook URL</param>
        /// <returns>送信結果</returns>
        Task<Result<string, string>> InformWebhookTest(string webhookUrl);
    }
}
