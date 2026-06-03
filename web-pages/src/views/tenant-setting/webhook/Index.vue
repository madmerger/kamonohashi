<template>
  <div>
    <h2>Webhook設定</h2>
    <el-card>
      <kqi-display-error :error="error" />

      <el-form ref="webhookForm" :model="form" label-width="200px">
        <h3>カスタムWebhook</h3>
        <el-form-item label="Webhook URL">
          <el-input
            v-model="form.webhookUrl"
            type="text"
            placeholder="https://example.com/webhook"
          />
        </el-form-item>

        <el-form-item>
          <el-button
            type="primary"
            size="small"
            :loading="testLoading"
            @click="sendTestNotification"
          >
            テスト通知
          </el-button>
        </el-form-item>

        <h3>通知テンプレート</h3>
        <el-form-item label="Slack通知テンプレート">
          <el-input
            v-model="form.slackNotificationTemplate"
            type="textarea"
            :rows="6"
            placeholder="カスタムSlack通知テンプレート（空の場合はデフォルトを使用）"
          />
        </el-form-item>

        <el-form-item label="Webhook通知テンプレート">
          <el-input
            v-model="form.webhookNotificationTemplate"
            type="textarea"
            :rows="6"
            placeholder="カスタムWebhook通知テンプレート（空の場合はデフォルトを使用）"
          />
          <div class="template-help">
            <small>
              利用可能なプレースホルダー:
              <code>{{Event}}</code>,
              <code>{{Id}}</code>,
              <code>{{Name}}</code>,
              <code>{{Status}}</code>,
              <code>{{CreatedBy}}</code>,
              <code>{{TenantName}}</code>,
              <code>{{JobType}}</code>,
              <code>{{Url}}</code>
            </small>
          </div>
        </el-form-item>

        <el-row>
          <el-col class="right-button-group" :span="24">
            <el-button type="primary" @click="saveData">
              保存
            </el-button>
          </el-col>
        </el-row>
      </el-form>
    </el-card>
  </div>
</template>

<script>
import KqiDisplayError from '@/components/KqiDisplayError'
import api from '@/api/api'

export default {
  title: 'Webhook設定',
  components: {
    KqiDisplayError,
  },
  data() {
    return {
      error: null,
      testLoading: false,
      form: {
        webhookUrl: '',
        slackNotificationTemplate: '',
        webhookNotificationTemplate: '',
      },
    }
  },
  async created() {
    await this.retrieveData()
  },
  methods: {
    async retrieveData() {
      try {
        let result = (await api.tenant.getWebhook()).data
        this.form.webhookUrl = result.webhookUrl || ''
        this.form.slackNotificationTemplate =
          result.slackNotificationTemplate || ''
        this.form.webhookNotificationTemplate =
          result.webhookNotificationTemplate || ''
        this.error = null
      } catch (e) {
        this.error = e
      }
    },
    async saveData() {
      try {
        let params = {
          body: {
            webhookUrl: this.form.webhookUrl || null,
            slackNotificationTemplate:
              this.form.slackNotificationTemplate || null,
            webhookNotificationTemplate:
              this.form.webhookNotificationTemplate || null,
          },
        }
        await api.tenant.putWebhook(params)
        this.showSuccessMessage()
        this.error = null
      } catch (e) {
        this.error = e
      }
    },
    async sendTestNotification() {
      if (!this.form.webhookUrl) {
        this.$message.warning('Webhook URLを入力してください')
        return
      }
      this.testLoading = true
      try {
        let params = {
          body: {
            webhookUrl: this.form.webhookUrl,
          },
        }
        await api.tenant.postWebhookTest(params)
        this.$message.success('テスト通知を送信しました')
      } catch (e) {
        this.$message.error('テスト通知の送信に失敗しました')
      } finally {
        this.testLoading = false
      }
    },
  },
}
</script>

<style lang="scss" scoped>
.right-button-group {
  text-align: right;
  padding-top: 20px;
}

.template-help {
  margin-top: 5px;
  color: #909399;

  code {
    background-color: #f5f7fa;
    padding: 2px 4px;
    border-radius: 3px;
    font-size: 12px;
  }
}
</style>
