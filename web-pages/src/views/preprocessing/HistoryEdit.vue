<template>
  <el-dialog
    class="dialog"
    :title="$t('titles.preprocessing_history_detail')"
    :visible="dialogVisible"
    :before-close="handleCancel"
    :close-on-click-modal="false"
  >
    <el-form ref="editForm">
      <kqi-display-error :error="error" />
      <kqi-display-text-form
        :label="$t('labels.preprocessing_history_id')"
        :value="preprocessingId"
      />
      <kqi-display-text-form
        :label="$t('labels.data_id')"
        :value="String(historyDetail.dataId)"
      />
      <kqi-display-text-form
        :label="$t('labels.data_name')"
        :value="historyDetail.dataName"
      />
      <kqi-display-text-form
        :label="$t('labels.preprocessing_name')"
        :value="historyDetail.preprocessName"
      />
      <kqi-display-text-form
        :label="$t('labels.execution_time_at')"
        :value="historyDetail.createdAt"
      />
      <el-form-item :label="$t('labels.preprocessing_log')">
        <br />
        <kqi-download-button
          :download-url="logFile.url"
          :file-name="logFile.fileName"
        />
        <el-button size="mini" @click="emitLog">{{
          $t('common.view')
        }}</el-button>
      </el-form-item>
      <kqi-display-text-form
        :label="$t('labels.status')"
        :value="historyDetail.status"
      />
      <div v-if="historyDetail.status === 'Running'">
        <el-form-item :label="$t('labels.operation')">
          <div class="el-input">
            <el-button @click="emitShell">{{
              $t('messages.shell_launch')
            }}</el-button>
          </div>
        </el-form-item>
      </div>
      <div v-if="historyEvents.length">
        <el-collapse accordion>
          <el-collapse-item :title="$t('titles.status_detail_log')">
            <div v-for="(event, index) in historyEvents" :key="index">
              <div v-if="event.isError">message:{{ event.message }}</div>
            </div>
          </el-collapse-item>
        </el-collapse>
      </div>
      <el-form-item v-if="outputDataIds" :label="$t('labels.output_data_id')">
        <div class="outputDataIds">
          <div v-if="outputDataIds.length >= 11">
            <el-button type="primary" @click="viewDataIds = !viewDataIds">
              {{ viewDataIds ? 'Hide DataIds' : 'View All DataIds' }}
            </el-button>
          </div>
          <div v-if="outputDataIds.length <= 10 || viewDataIds">
            <span
              v-for="(outputDataId, index) in outputDataIds"
              :key="index"
              class="outputDataId"
            >
              <el-link
                v-if="$store.getters['account/isAvailableData']"
                type="primary"
                @click="redirectDataEdit(outputDataId)"
              >
                {{ outputDataId }}
              </el-link>
              <span v-else type="primary">
                {{ outputDataId }}
              </span>
            </span>
          </div>
        </div>
      </el-form-item>
      <el-row>
        <el-col class="button-group">
          <el-button
            class="pull-right btn-cancel"
            icon="el-icon-close"
            @click="handleCancel"
          >
            {{ $t('common.close') }}
          </el-button>
          <kqi-delete-button
            class="pull-left btn-update"
            :message="$t('messages.delete_confirm_output')"
            @delete="handleRemove"
          />
        </el-col>
      </el-row>
    </el-form>
  </el-dialog>
</template>

<script>
import KqiDisplayError from '@/components/KqiDisplayError'
import KqiDisplayTextForm from '@/components/KqiDisplayTextForm'
import KqiDeleteButton from '@/components/KqiDeleteButton'
import KqiDownloadButton from '@/components/KqiDownloadButton'
import { createNamespacedHelpers } from 'vuex'
const { mapGetters, mapActions } = createNamespacedHelpers('preprocessing')

export default {
  components: {
    KqiDisplayError,
    KqiDisplayTextForm,
    KqiDeleteButton,
    KqiDownloadButton,
  },
  props: {
    id: {
      type: String,
      default: null,
    },
    dataId: {
      type: String,
      default: null,
    },
  },
  data() {
    return {
      dialogVisible: true,
      preprocessingId: null,
      viewDataIds: false,
      outputDataIds: null,
      error: null,
    }
  },
  computed: {
    ...mapGetters(['historyDetail', 'historyEvents', 'logFile']),
  },

  watch: {
    async id() {
      await this.changeValue()
    },
    async dataId() {
      await this.changeValue()
    },
  },

  async created() {
    await this.changeValue()
  },

  methods: {
    ...mapActions([
      'fetchHistoryDetail',
      'fetchHistoryEvents',
      'fetchLogFile',
      'deleteHistory',
    ]),

    async changeValue() {
      this.dataName = ''
      this.preprocessName = ''
      this.createdAt = ''
      this.status = ''

      if (this.id && this.dataId) {
        try {
          await this.fetchHistoryDetail({ id: this.id, dataId: this.dataId })
          this.preprocessingId = this.historyDetail.key.split('-')[1]
          this.error = null
          if (
            this.historyDetail.statusType === 'Running' ||
            this.historyDetail.statusType === 'Error'
          ) {
            await this.fetchHistoryEvents({ id: this.id, dataId: this.dataId })
          }
          await this.fetchLogFile({ id: this.id, dataId: this.dataId })
          if (this.historyDetail.outputDataIds.length !== 0) {
            this.outputDataIds = this.historyDetail.outputDataIds
          }
        } catch (e) {
          this.error = e
        }
      }
    },

    async handleRemove() {
      try {
        await this.deleteHistory({ id: this.id, dataId: this.dataId })
        this.$emit('done', 'delete')
        this.error = null
      } catch (e) {
        this.error = e
      }
    },

    async handleCancel() {
      this.emitCancel()
    },
    emitShell() {
      this.$emit('shell', { id: this.preprocessingId })
    },
    emitLog() {
      this.$emit('log', { id: this.id, dataId: this.dataId })
    },
    redirectDataEdit(dataId) {
      this.$router.push('/data/edit/' + dataId)
    },

    emitCancel() {
      this.$emit('cancel')
    },
  },
}
</script>

<style lang="scss" scoped>
.button-group {
  text-align: right;
  padding-top: 10px;
}

.btn-update {
  margin-left: 10px;
}

.dialog /deep/ label {
  font-weight: bold !important;
}

.pull-right {
  float: right !important;
}

.pull-left {
  float: left !important;
}
.outputDataId {
  margin: 10px;
}

.outputDataIds {
  display: inline-block;
  width: 100%;
}
</style>
