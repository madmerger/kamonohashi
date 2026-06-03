using Microsoft.Extensions.Options;
using Nssol.Platypus.Infrastructure;
using Nssol.Platypus.Infrastructure.Options;
using Nssol.Platypus.Logic.Interfaces;
using Nssol.Platypus.Models.TenantModels;
using Nssol.Platypus.ServiceModels.Webhook.SlackModels;
using Nssol.Platypus.ServiceModels.Webhook.WebhookModels;
using Nssol.Platypus.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nssol.Platypus.Logic
{
    /// <summary>
    /// 通知ロジック（Slack + カスタムWebhook）
    /// </summary>
    public class NotificationLogic : PlatypusLogicBase, INotificationLogic
    {
        private readonly ISlackService slackService;
        private readonly IWebhookService webhookService;
        private readonly ContainerManageOptions containerOptions;

        private readonly Dictionary<ContainerStatus, string> ColorDictionary
            = new Dictionary<ContainerStatus, string>
        {
            { ContainerStatus.Completed,    "#67C23A" },
            { ContainerStatus.Killed,       "#D00000" },
            { ContainerStatus.UserCanceled, "#E6A23C" },
            { ContainerStatus.Failed,       "#D00000" }
        };

        private readonly Dictionary<ContainerStatus, string> EventDictionary
            = new Dictionary<ContainerStatus, string>
        {
            { ContainerStatus.Completed,    "job_completed" },
            { ContainerStatus.Killed,       "job_killed" },
            { ContainerStatus.UserCanceled, "job_canceled" },
            { ContainerStatus.Failed,       "job_failed" }
        };

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NotificationLogic(
            ICommonDiLogic commonDiLogic,
            ISlackService slackService,
            IWebhookService webhookService,
            IOptions<ContainerManageOptions> containerOptions) : base(commonDiLogic)
        {
            this.slackService = slackService;
            this.webhookService = webhookService;
            this.containerOptions = containerOptions.Value;
        }

        /// <summary>
        /// 学習結果を通知する（Slack + Webhook）
        /// </summary>
        public void InformJobResult(TrainingHistory history)
        {
            if (string.IsNullOrEmpty(containerOptions.WebEndPoint))
            {
                LogWarning("ホスト情報が未登録のため通知処理をスキップしました");
                return;
            }

            // Slack通知（ユーザ単位のSlackURL設定がある場合）
            if (!string.IsNullOrEmpty(CurrentUserInfo?.SlackUrl))
            {
                if (ColorDictionary.ContainsKey(history.GetStatus()))
                {
                    slackService.SendMessageAsync(new SendMessageInputModel()
                    {
                        Id = history.Id,
                        Name = history.Name,
                        CreatedBy = history.CreatedBy,
                        Mention = string.IsNullOrEmpty(CurrentUserInfo.Mention) ? null : CurrentUserInfo.Mention,
                        Status = history.GetStatus().Name,
                        Tenant = CurrentUserInfo.SelectedTenant,
                        Url = $"http://{containerOptions.WebEndPoint}/kamonohashi/#/training/{history.Id}?tenantId={CurrentUserInfo.SelectedTenant.Id}",
                        Title = "KAMONOHASHI 学習結果通知",
                        Color = ColorDictionary[history.GetStatus()],
                        WebhookUrl = CurrentUserInfo.SlackUrl
                    });
                }
                else
                {
                    slackService.SendMessageAsync(new SendMessageInputModel()
                    {
                        Id = history.Id,
                        Name = history.Name,
                        CreatedBy = history.CreatedBy,
                        Mention = string.IsNullOrEmpty(CurrentUserInfo.Mention) ? null : CurrentUserInfo.Mention,
                        Tenant = CurrentUserInfo.SelectedTenant,
                        Url = $"http://{containerOptions.WebEndPoint}/kamonohashi/#/training?tenantId={CurrentUserInfo.SelectedTenant.Id}",
                        Title = "KAMONOHASHI 学習履歴削除通知",
                        Color = ColorDictionary[ContainerStatus.Killed],
                        WebhookUrl = CurrentUserInfo.SlackUrl,
                        Message = "ジョブ実行中に履歴情報が削除されました"
                    });
                }
            }

            // カスタムWebhook通知（テナント単位のWebhookURL設定がある場合）
            SendWebhookNotification(history.Id, history.Name, history.CreatedBy, history.GetStatus(), "Training",
                $"http://{containerOptions.WebEndPoint}/kamonohashi/#/training/{history.Id}?tenantId={CurrentUserInfo?.SelectedTenant?.Id}");
        }

        /// <summary>
        /// 推論結果を通知する（Slack + Webhook）
        /// </summary>
        public void InformJobResult(InferenceHistory history)
        {
            if (string.IsNullOrEmpty(containerOptions.WebEndPoint))
            {
                LogWarning("ホスト情報が未登録のため通知処理をスキップしました");
                return;
            }

            // Slack通知（ユーザ単位のSlackURL設定がある場合）
            if (!string.IsNullOrEmpty(CurrentUserInfo?.SlackUrl))
            {
                if (ColorDictionary.ContainsKey(history.GetStatus()))
                {
                    slackService.SendMessageAsync(new SendMessageInputModel()
                    {
                        Id = history.Id,
                        Name = history.Name,
                        CreatedBy = history.CreatedBy,
                        Mention = string.IsNullOrEmpty(CurrentUserInfo.Mention) ? null : CurrentUserInfo.Mention,
                        Status = history.GetStatus().Name,
                        Tenant = CurrentUserInfo.SelectedTenant,
                        Url = $"http://{containerOptions.WebEndPoint}/kamonohashi/#/inference/{history.Id}?tenantId={CurrentUserInfo.SelectedTenant.Id}",
                        Title = "KAMONOHASHI 推論結果通知",
                        Color = ColorDictionary[history.GetStatus()],
                        WebhookUrl = CurrentUserInfo.SlackUrl
                    });
                }
                else
                {
                    slackService.SendMessageAsync(new SendMessageInputModel()
                    {
                        Id = history.Id,
                        Name = history.Name,
                        CreatedBy = history.CreatedBy,
                        Mention = string.IsNullOrEmpty(CurrentUserInfo.Mention) ? null : CurrentUserInfo.Mention,
                        Tenant = CurrentUserInfo.SelectedTenant,
                        Url = $"http://{containerOptions.WebEndPoint}/kamonohashi/#/inference?tenantId={CurrentUserInfo.SelectedTenant.Id}",
                        Title = "KAMONOHASHI 推論履歴削除通知",
                        Color = ColorDictionary[ContainerStatus.Killed],
                        WebhookUrl = CurrentUserInfo.SlackUrl,
                        Message = "ジョブ実行中に履歴情報が削除されました"
                    });
                }
            }

            // カスタムWebhook通知（テナント単位のWebhookURL設定がある場合）
            SendWebhookNotification(history.Id, history.Name, history.CreatedBy, history.GetStatus(), "Inference",
                $"http://{containerOptions.WebEndPoint}/kamonohashi/#/inference/{history.Id}?tenantId={CurrentUserInfo?.SelectedTenant?.Id}");
        }

        /// <summary>
        /// Webhookテスト通知する
        /// </summary>
        public async Task<Result<string, string>> InformWebhookTest(string webhookUrl)
        {
            if (string.IsNullOrEmpty(webhookUrl))
            {
                return Result<string, string>.CreateErrorResult("Webhook URLが設定されていません");
            }

            if (string.IsNullOrEmpty(containerOptions.WebEndPoint))
            {
                return Result<string, string>.CreateErrorResult("KAMONOHASHIのホスト情報が登録されていないため、通知できませんでした");
            }

            var result = await webhookService.SendTestWebhookAsync(new WebhookSendModel()
            {
                WebhookUrl = webhookUrl,
                CreatedBy = CurrentUserInfo?.Name,
                Tenant = CurrentUserInfo?.SelectedTenant,
                Url = $"http://{containerOptions.WebEndPoint}/kamonohashi/#/"
            });

            if (result)
            {
                return Result<string, string>.CreateResult("Webhookテスト通知の送信に成功しました");
            }
            else
            {
                return Result<string, string>.CreateErrorResult("Webhookテスト通知の送信に失敗しました。入力されたURLに誤りがあります");
            }
        }

        /// <summary>
        /// カスタムWebhook通知を送信する共通処理
        /// </summary>
        private void SendWebhookNotification(long id, string name, string createdBy, ContainerStatus status, string jobType, string url)
        {
            var tenant = CurrentUserInfo?.SelectedTenant;
            if (tenant == null || string.IsNullOrEmpty(tenant.WebhookUrl))
            {
                return;
            }

            string eventName = EventDictionary.ContainsKey(status) ? EventDictionary[status] : "job_status_changed";

            webhookService.SendWebhookAsync(new WebhookSendModel()
            {
                WebhookUrl = tenant.WebhookUrl,
                Event = eventName,
                Id = id,
                Name = name,
                CreatedBy = createdBy,
                Status = status.Name,
                Tenant = tenant,
                Url = url,
                JobType = jobType,
                CustomTemplate = tenant.WebhookNotificationTemplate
            });
        }
    }
}
