<template>
  <kqi-dialog
    :title="title"
    type="EDIT"
    @submit="onSubmit"
    @delete="deleteJob"
    @close="emitCancel"
  >
    <el-row type="flex" justify="end">
      <el-col :span="24" class="right-button-group">
        <el-button @click="emitCopyCreate">{{
          $t('common.copy_execution')
        }}</el-button>
      </el-col>
    </el-row>

    <el-form ref="updateForm" :model="form" :rules="rules">
      <kqi-display-error :error="error" />
      <el-row :gutter="20">
        <el-col :span="12">
          <kqi-display-text-form
            label="ID"
            :value="detail ? String(detail.id) : '0'"
          >
            <span slot="action">
              <div
                v-if="form.favorite"
                class="el-icon-star-on favorite"
                @click="form.favorite = false"
              />
              <div
                v-else
                class="el-icon-star-off favorite"
                @click="form.favorite = true"
              />
            </span>
          </kqi-display-text-form>
          <el-form-item :label="$t('labels.notebook_name')" prop="name">
            <el-input v-model="form.name" />
          </el-form-item>
          <div v-if="detail.parents && detail.parents.length > 0">
            <el-form-item :label="$t('labels.mounted_training')">
              <br />
              <div :class="{ scroll: detail.parents.length > 3 }">
                <div v-for="parent in detail.parents" :key="parent.id">
                  <el-popover
                    ref="parentDetail"
                    :title="$t('titles.mounted_training_detail')"
                    trigger="hover"
                    width="350"
                    placement="right"
                  >
                    <kqi-training-history-details :training="parent" />
                    <el-button
                      v-if="$store.getters['account/isAvailableTraining']"
                      slot="reference"
                      class="el-input"
                      @click="showParent(parent.id)"
                    >
                      {{ parent.fullName }}
                    </el-button>
                    <el-button v-else slot="reference" class="el-input">
                      {{ parent.fullName }}
                    </el-button>
                  </el-popover>
                </div>
              </div>
            </el-form-item>
          </div>
          <div v-if="detail.inferences && detail.inferences.length > 0">
            <el-form-item :label="$t('labels.mounted_inference')">
              <br />
              <div :class="{ scroll: detail.inferences.length > 3 }">
                <div v-for="inference in detail.inferences" :key="inference.id">
                  <el-popover
                    ref="parentDetail"
                    :title="$t('titles.mounted_inference_detail')"
                    trigger="hover"
                    width="350"
                    placement="right"
                  >
                    <kqi-inference-history-details :inference="inference" />
                    <el-button
                      v-if="$store.getters['account/isAvailableInference']"
                      slot="reference"
                      class="el-input"
                      @click="showInference(inference.id)"
                    >
                      {{ inference.fullName }}
                    </el-button>
                    <el-button v-else slot="reference" class="el-input">
                      {{ inference.fullName }}
                    </el-button>
                  </el-popover>
                </div>
              </div>
            </el-form-item>
          </div>
          <div v-if="detail.dataSet">
            <el-form-item :label="$t('labels.dataset')">
              <el-popover
                ref="dataSetDetail"
                :title="$t('titles.dataset_detail')"
                trigger="hover"
                width="350"
                placement="right"
              >
                <kqi-data-set-details :data-set="detail.dataSet" />
              </el-popover>
              <el-button
                v-if="$store.getters['account/isAvailableDataSet']"
                v-popover:dataSetDetail
                class="el-input button"
                @click="redirectEditDataSet"
              >
                {{ detail.dataSet.name }}
              </el-button>
              <el-button v-else v-popover:dataSetDetail class="el-input">
                {{ detail.dataSet.name }}
              </el-button>
            </el-form-item>
            <el-form-item :label="$t('labels.dataset_creation_method')">
              <div class="el-input">
                <span v-if="detail.localDataSet">{{
                  $t('common.local_copy')
                }}</span>
                <span v-else>{{ $t('common.symbolic_link') }}</span>
              </div>
            </el-form-item>
          </div>
          <el-form-item :label="$t('labels.model')">
            <div class="el-input">
              <span
                v-if="detail.gitModel && detail.gitModel.url !== null"
                style="padding-left: 3px;"
              >
                <a :href="detail.gitModel.url" target="_blank">
                  {{ detail.gitModel.owner }}/{{
                    detail.gitModel.repository
                  }}/{{ detail.gitModel.branch }}
                </a>
              </span>
              <span v-else>
                None
              </span>
            </div>
          </el-form-item>

          <kqi-display-text-form
            :label="$t('labels.container_image')"
            :value="detail.containerImage ? detail.containerImage.url : ''"
          />

          <el-form-item :label="$t('labels.startup_command')">
            <el-input
              v-model="detail.entryPoint"
              type="textarea"
              :autosize="{ minRows: 2 }"
              :readonly="true"
            />
          </el-form-item>

          <el-form-item :label="$t('labels.environment_variable')">
            <br />
            <div
              v-if="detail.options && detail.options.length > 0"
              class="el-input"
            >
              <el-row v-for="option in detail.options" :key="option.key">
                <el-col :span="8" :offset="1">{{ option.key }}</el-col>
                <el-col :span="12">{{ option.value }}</el-col>
              </el-row>
            </div>
            <div v-else>
              －
            </div>
          </el-form-item>

          <kqi-display-text-form
            :label="$t('labels.author')"
            :value="
              detail
                ? detail.displayNameCreatedBy
                  ? detail.createdBy + '【' + detail.displayNameCreatedBy + '】'
                  : detail.createdBy
                : ''
            "
          />
          <kqi-display-text-form
            :label="$t('labels.started_at')"
            :value="detail.startedAt"
          />
          <kqi-display-text-form
            :label="$t('labels.completed_at')"
            :value="detail.completedAt"
          />
          <kqi-display-text-form
            :label="$t('labels.waiting_time')"
            :value="detail.waitingTime"
          />
          <kqi-display-text-form
            :label="$t('labels.execution_time')"
            :value="detail.executionTime"
          />
        </el-col>
        <el-col :span="12">
          <kqi-display-text-form
            label="CPU"
            :value="detail ? String(detail.cpu) : '0'"
          />
          <kqi-display-text-form
            :label="$t('labels.memory_gb')"
            :value="detail ? String(detail.memory) : '0'"
          />
          <kqi-display-text-form
            label="GPU"
            :value="detail ? String(detail.gpu) : '0'"
          />
          <div v-if="detail">
            <kqi-display-text-form
              v-if="detail.expiresIn !== 0"
              :label="$t('labels.startup_period_h')"
              :value="String(detail.expiresIn / 60 / 60)"
            />
            <kqi-display-text-form
              v-else
              :label="$t('labels.startup_period')"
              :value="$t('common.no_expiration')"
            />
          </div>
          <kqi-display-text-form
            :label="$t('labels.partition')"
            :value="detail.partition"
          />
          <!-- status: スクリプトがこけたときなどに"failed"になる -->
          <!-- statusType: コンテナの生死等 -->
          <kqi-display-text-form
            :label="$t('labels.status')"
            :value="
              detail.status === detail.statusType
                ? detail.status
                : detail.statusType + ' (' + detail.status + ')'
            "
          />
          <div v-if="detail.conditionNote !== ``" class="k8s-event">
            {{ detail.conditionNote }}
          </div>
          <div v-if="events.length" class="k8s-event">
            <el-collapse accordion>
              <el-collapse-item :title="$t('titles.status_detail_log')">
                <div v-for="(event, index) in events" :key="index">
                  <div v-if="event.isError">message:{{ event.message }}</div>
                </div>
              </el-collapse-item>
            </el-collapse>
          </div>
          <div
            v-if="
              detail.statusType === 'Running' || detail.statusType === 'Error'
            "
          >
            <el-form-item :label="$t('labels.operation')">
              <div class="el-input">
                <kqi-delete-button
                  :button-label="$t('messages.job_stop')"
                  :message="$t('messages.stop_confirm')"
                  @delete="haltNotebook"
                />
              </div>
              <div v-if="detail.status === 'Running'">
                <div class="el-input" style="padding: 10px 0;">
                  <el-button @click="emitShell">{{
                    $t('messages.shell_launch')
                  }}</el-button>
                </div>
                <div>
                  <el-button
                    type="plain"
                    icon="el-icon-document"
                    @click="openNotebook"
                  >
                    {{ $t('messages.open_notebook') }}
                  </el-button>
                </div>
              </div>
            </el-form-item>
          </div>
          <el-form-item :label="$t('labels.container_output_file')">
            <br />
            <el-button @click="emitFiles">{{
              $t('messages.file_list')
            }}</el-button>
          </el-form-item>
          <el-form-item :label="$t('labels.log_file')">
            <br />
            <el-button size="mini" @click="emitLog">{{
              $t('messages.log_file_view')
            }}</el-button>
          </el-form-item>
          <el-form-item :label="$t('labels.memo')">
            <el-input
              v-model="form.memo"
              type="textarea"
              :autosize="{ minRows: 2, maxRows: 4 }"
            />
          </el-form-item>
        </el-col>
      </el-row>
    </el-form>
  </kqi-dialog>
</template>

<script>
import KqiDialog from '@/components/KqiDialog'
import KqiDisplayError from '@/components/KqiDisplayError'
import KqiDisplayTextForm from '@/components/KqiDisplayTextForm'
import KqiDeleteButton from '@/components/KqiDeleteButton'
import KqiDataSetDetails from '@/components/selector/KqiDataSetDetails'
import KqiTrainingHistoryDetails from '@/components/selector/KqiTrainingHistoryDetails'
import KqiInferenceHistoryDetails from '@/components/selector/KqiInferenceHistoryDetails'
import { createNamespacedHelpers } from 'vuex'
const { mapGetters, mapActions } = createNamespacedHelpers('notebook')
const kqiHost = process.env.VUE_APP_KAMONOHASHI_HOST || window.location.hostname

export default {
  components: {
    KqiDialog,
    KqiDisplayError,
    KqiDisplayTextForm,
    KqiDeleteButton,
    KqiDataSetDetails,
    KqiTrainingHistoryDetails,
    KqiInferenceHistoryDetails,
  },
  props: {
    id: {
      type: String,
      default: null,
    },
  },
  data() {
    return {
      rules: {
        name: [
          {
            required: true,
            trigger: 'blur',
            message: this.$t('common.required_field'),
          },
        ],
      },
      form: {
        name: null,
        favorite: false,
        memo: null,
      },
      title: '',
      error: null,
    }
  },
  computed: {
    ...mapGetters(['detail', 'events', 'endpoint']),
  },
  async created() {
    this.title = this.$t('titles.notebook_history')
    await this.retrieveData()
    this.form.name = this.detail.name
    this.form.favorite = this.detail.favorite
    this.form.memo = this.detail.memo
  },
  methods: {
    ...mapActions([
      'fetchDetail',
      'fetchEvents',
      'fetchEndpoint',
      'postHalt',
      'put',
      'delete',
    ]),
    async retrieveData() {
      await this.fetchDetail(this.id)
      if (
        this.detail.statusType === 'Running' ||
        this.detail.statusType === 'Error'
      ) {
        await this.fetchEvents(this.detail.id)
      }
    },
    async updateHistory() {
      let params = {
        id: this.detail.id,
        body: {
          name: this.form.name,
          memo: this.form.memo,
          favorite: this.form.favorite,
        },
      }
      await this.put(params)
    },
    async onSubmit() {
      let form = this.$refs.updateForm
      await form.validate(async valid => {
        if (valid) {
          try {
            await this.updateHistory()
            this.$emit('done')
            this.error = null
          } catch (e) {
            this.error = e
          }
        }
      })
    },
    async haltNotebook() {
      try {
        await this.postHalt(this.detail.id)
        await this.retrieveData()
        this.error = null
      } catch (e) {
        this.error = e
      }
    },
    async deleteJob() {
      try {
        await this.delete(this.detail.id)
        this.$emit('done', 'delete')
        this.error = null
      } catch (e) {
        this.error = e
      }
    },
    async openNotebook() {
      await this.fetchEndpoint(this.detail.id)
      window.open(
        `http://${kqiHost}:${this.endpoint.nodePort}${this.endpoint.token}`,
      )
    },

    // 親ジョブ履歴の表示
    showParent(parentId) {
      // 表示内容の変更は、beforeUpdated内で行う
      this.$router.push('/training/' + parentId)
    },
    showInference(inferenceId) {
      // 表示内容の変更は、beforeUpdated内で行う
      this.$router.push('/inference/' + inferenceId)
    },
    redirectEditDataSet() {
      this.$router.push('/dataset/edit/' + this.detail.dataSet.id)
    },
    emitFiles() {
      this.$emit('files', this.detail.id)
    },
    emitShell() {
      this.$emit('shell', this.detail.id)
    },
    emitLog() {
      this.$emit('log', this.detail.id)
    },
    emitCopyCreate() {
      this.$emit('copyCreate', this.detail.id)
    },
    emitCancel() {
      this.$emit('cancel')
    },
  },
}
</script>

<style lang="scss" scoped>
.right-button-group {
  text-align: right;
}

.dialog /deep/ label {
  font-weight: bold !important;
}

.dialog /deep/ .el-dialog__title {
  font-size: 24px;
}

.footer {
  padding-top: 40px;
}

.favorite {
  font-size: 20px;
  color: rgb(230, 162, 60);
}
.scroll {
  height: 125px;
  overflow-y: scroll;
}
</style>
