using Nssol.Platypus.ServiceModels.Webhook.WebhookModels;
using System.Threading.Tasks;

namespace Nssol.Platypus.Services.Interfaces
{
    /// <summary>
    /// カスタムWebhook通知用サービスインターフェース
    /// </summary>
    public interface IWebhookService
    {
        /// <summary>
        /// カスタムWebhookにメッセージを送信する
        /// </summary>
        /// <param name="model">Webhook送信モデル</param>
        void SendWebhookAsync(WebhookSendModel model);

        /// <summary>
        /// カスタムWebhookにテスト通知を送信する
        /// </summary>
        /// <param name="model">Webhook送信モデル</param>
        /// <returns>送信成否</returns>
        Task<bool> SendTestWebhookAsync(WebhookSendModel model);
    }
}
