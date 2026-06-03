using Nssol.Platypus.Logic.Interfaces;
using Nssol.Platypus.ServiceModels.Webhook.WebhookModels;
using Nssol.Platypus.Services.Interfaces;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Nssol.Platypus.Services
{
    /// <summary>
    /// カスタムWebhook通知用サービス
    /// </summary>
    public class WebhookService : PlatypusServiceBase, IWebhookService
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public WebhookService(
            ICommonDiLogic commonDiLogic)
            : base(commonDiLogic, @"ServiceModels/Webhook/WebhookModels/Templates") { }

        /// <summary>
        /// カスタムWebhookにメッセージを送信する
        /// </summary>
        /// <param name="model">Webhook送信モデル</param>
        public async void SendWebhookAsync(WebhookSendModel model)
        {
            try
            {
                string body;
                if (!string.IsNullOrEmpty(model.CustomTemplate))
                {
                    body = ReplaceTemplatePlaceholders(model.CustomTemplate, model);
                }
                else
                {
                    body = await RenderEngine.CompileRenderAsync("webhook_notification.json", new
                    {
                        Event = model.Event,
                        Id = model.Id,
                        Name = model.Name,
                        TenantName = model.Tenant?.DisplayName,
                        CreatedBy = model.CreatedBy,
                        Status = model.Status,
                        Url = model.Url,
                        JobType = model.JobType,
                        Message = model.Message
                    });
                }

                var uri = new Uri(model.WebhookUrl);
                var response = await SendPostRequestAsync(new RequestParam()
                {
                    BaseUrl = uri.GetLeftPart(UriPartial.Authority),
                    ApiPath = uri.PathAndQuery,
                    Body = body
                });
                if (!response.IsSuccess)
                {
                    LogWarning("Webhookメッセージ送信に失敗: " + response.Error);
                }
            }
            catch (Exception ex)
            {
                LogWarning("Webhookメッセージ送信に失敗: " + ex.Message);
            }
        }

        /// <summary>
        /// カスタムWebhookにテスト通知を送信する
        /// </summary>
        /// <param name="model">Webhook送信モデル</param>
        /// <returns>送信成否</returns>
        public async Task<bool> SendTestWebhookAsync(WebhookSendModel model)
        {
            string body = await RenderEngine.CompileRenderAsync("webhook_test.json", new
            {
                CreatedBy = model.CreatedBy,
                TenantName = model.Tenant?.DisplayName,
                Url = model.Url
            });

            try
            {
                var uri = new Uri(model.WebhookUrl);
                var response = await SendPostRequestAsync(new RequestParam()
                {
                    BaseUrl = uri.GetLeftPart(UriPartial.Authority),
                    ApiPath = uri.PathAndQuery,
                    Body = body
                });
                if (!response.IsSuccess)
                {
                    LogWarning("Webhookテスト送信に失敗: " + response.Error);
                    return false;
                }
                return true;
            }
            catch (InvalidOperationException)
            {
                LogWarning("Webhookテスト送信に失敗: URLに誤りがあります");
                return false;
            }
            catch (HttpRequestException)
            {
                LogWarning("Webhookテスト送信に失敗: URLに誤りがあります");
                return false;
            }
            catch (UriFormatException)
            {
                LogWarning("Webhookテスト送信に失敗: URL形式が不正です");
                return false;
            }
        }

        private static string ReplaceTemplatePlaceholders(string template, WebhookSendModel model)
        {
            return template
                .Replace("{{Event}}", model.Event ?? "")
                .Replace("{{Id}}", model.Id.ToString())
                .Replace("{{Name}}", model.Name ?? "")
                .Replace("{{TenantName}}", model.Tenant?.DisplayName ?? "")
                .Replace("{{CreatedBy}}", model.CreatedBy ?? "")
                .Replace("{{Status}}", model.Status ?? "")
                .Replace("{{Url}}", model.Url ?? "")
                .Replace("{{JobType}}", model.JobType ?? "")
                .Replace("{{Message}}", model.Message ?? "");
        }
    }
}
