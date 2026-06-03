<template>
  <div>
    <h2>パイプライン管理</h2>
    <el-row type="flex" justify="space-between" :gutter="20">
      <el-col class="right-top-button" :span="8">
        <el-button
          icon="el-icon-edit-outline"
          type="primary"
          plain
          @click="openCreateDialog()"
        >
          新規作成
        </el-button>
      </el-col>
    </el-row>
    <el-table
      class="data-table pl-index-table"
      :data="pipelines"
      border
      @row-click="openEditDialog"
    >
      <el-table-column prop="id" label="ID" width="80px" />
      <el-table-column prop="name" label="パイプライン名" width="auto" />
      <el-table-column prop="memo" label="メモ" width="auto" />
      <el-table-column prop="nodeCount" label="ノード数" width="100px" />
      <el-table-column prop="createdAt" label="登録日時" width="200px" />
      <el-table-column prop="modifiedAt" label="更新日時" width="200px" />
      <el-table-column label="操作" width="260px">
        <template slot-scope="scope">
          <el-button
            size="mini"
            type="primary"
            @click.stop="openRunDialog(scope.row)"
          >
            実行
          </el-button>
          <el-button size="mini" @click.stop="openRunsDialog(scope.row)">
            実行履歴
          </el-button>
          <el-button
            size="mini"
            type="danger"
            @click.stop="handleDelete(scope.row)"
          >
            削除
          </el-button>
        </template>
      </el-table-column>
    </el-table>

    <router-view @done="retrieveData" />

    <!-- 実行ダイアログ -->
    <el-dialog
      :visible.sync="runDialogVisible"
      title="パイプライン実行"
      width="400px"
    >
      <p>
        パイプライン「{{
          selectedPipeline && selectedPipeline.name
        }}」を実行しますか？
      </p>
      <span slot="footer" class="dialog-footer">
        <el-button @click="runDialogVisible = false">キャンセル</el-button>
        <el-button type="primary" @click="executePipeline">実行</el-button>
      </span>
    </el-dialog>

    <!-- 実行履歴ダイアログ -->
    <el-dialog
      :visible.sync="runsDialogVisible"
      :title="'実行履歴: ' + (selectedPipeline && selectedPipeline.name)"
      width="90%"
    >
      <el-table :data="runs" border>
        <el-table-column prop="id" label="Run ID" width="80px" />
        <el-table-column prop="status" label="ステータス" width="120px">
          <template slot-scope="scope">
            <el-tag :type="statusTagType(scope.row.status)">{{
              scope.row.status
            }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="startedAt" label="開始日時" width="180px" />
        <el-table-column prop="completedAt" label="完了日時" width="180px" />
        <el-table-column prop="createdAt" label="登録日時" width="180px" />
        <el-table-column label="ステップ" width="auto">
          <template slot-scope="scope">
            <div class="step-badges">
              <el-tag
                v-for="step in scope.row.steps"
                :key="step.id"
                :type="statusTagType(step.status)"
                size="mini"
                style="margin: 2px;"
              >
                {{ step.nodeName }}: {{ step.status }}
              </el-tag>
            </div>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="100px">
          <template slot-scope="scope">
            <el-button size="mini" @click="openRunDetail(scope.row)">
              詳細
            </el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-dialog>

    <!-- Run詳細ダイアログ -->
    <el-dialog
      :visible.sync="runDetailVisible"
      :title="'実行詳細: Run #' + (selectedRun && selectedRun.id)"
      width="80%"
    >
      <div v-if="selectedRun">
        <el-descriptions border :column="2">
          <el-descriptions-item label="ステータス">
            <el-tag :type="statusTagType(selectedRun.status)">{{
              selectedRun.status
            }}</el-tag>
          </el-descriptions-item>
          <el-descriptions-item label="開始日時">{{
            selectedRun.startedAt
          }}</el-descriptions-item>
          <el-descriptions-item label="完了日時">{{
            selectedRun.completedAt
          }}</el-descriptions-item>
          <el-descriptions-item label="登録日時">{{
            selectedRun.createdAt
          }}</el-descriptions-item>
        </el-descriptions>

        <h4 style="margin-top: 20px;">ステップ一覧</h4>
        <div class="pipeline-flow">
          <div
            v-for="step in selectedRun.steps"
            :key="step.id"
            class="pipeline-step"
            :class="'step-' + step.status.toLowerCase()"
          >
            <div class="step-header">
              <strong>{{ step.nodeName }}</strong>
              <el-tag :type="statusTagType(step.status)" size="mini">
                {{ step.status }}
              </el-tag>
            </div>
            <div class="step-body">
              <span class="step-type">{{ step.jobType }}</span>
              <span v-if="step.startedAt" class="step-time">{{
                step.startedAt
              }}</span>
              <span v-if="step.trainingHistoryId" class="step-link">
                学習ID: {{ step.trainingHistoryId }}
              </span>
              <span v-if="step.inferenceHistoryId" class="step-link">
                推論ID: {{ step.inferenceHistoryId }}
              </span>
              <span v-if="step.preprocessHistoryId" class="step-link">
                前処理ID: {{ step.preprocessHistoryId }}
              </span>
            </div>
          </div>
        </div>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import { createNamespacedHelpers } from 'vuex'
const { mapGetters, mapActions } = createNamespacedHelpers('pipeline')

export default {
  data() {
    return {
      runDialogVisible: false,
      runsDialogVisible: false,
      runDetailVisible: false,
      selectedPipeline: null,
    }
  },
  computed: {
    ...mapGetters(['pipelines', 'runs', 'selectedRun']),
  },
  async created() {
    await this.retrieveData()
  },
  methods: {
    ...mapActions([
      'fetchPipelines',
      'deletePipeline',
      'executePipelineRun',
      'fetchRuns',
      'fetchRunDetail',
    ]),
    async retrieveData() {
      await this.fetchPipelines()
    },
    openCreateDialog() {
      this.$router.push('/pipeline/create')
    },
    openEditDialog(row) {
      this.$router.push('/pipeline/' + row.id)
    },
    openRunDialog(row) {
      this.selectedPipeline = row
      this.runDialogVisible = true
    },
    async executePipeline() {
      await this.executePipelineRun(this.selectedPipeline.id)
      this.runDialogVisible = false
      this.$notify.success({
        title: '実行開始',
        message: 'パイプラインの実行を開始しました',
      })
    },
    async openRunsDialog(row) {
      this.selectedPipeline = row
      await this.fetchRuns(row.id)
      this.runsDialogVisible = true
    },
    async openRunDetail(run) {
      await this.fetchRunDetail(run.id)
      this.runDetailVisible = true
    },
    async handleDelete(row) {
      try {
        await this.$confirm(
          'パイプライン「' + row.name + '」を削除しますか？',
          '確認',
          {
            type: 'warning',
          },
        )
        await this.deletePipeline(row.id)
        await this.retrieveData()
        this.$notify.success({
          title: '削除完了',
          message: 'パイプラインを削除しました',
        })
      } catch (e) {
        // cancel
      }
    },
    statusTagType(status) {
      switch (status) {
        case 'Completed':
          return 'success'
        case 'Running':
          return ''
        case 'Failed':
          return 'danger'
        case 'Pending':
          return 'info'
        case 'Skipped':
          return 'warning'
        case 'Cancelled':
          return 'warning'
        default:
          return 'info'
      }
    },
  },
}
</script>

<style lang="scss" scoped>
.pipeline-flow {
  display: flex;
  flex-wrap: wrap;
  gap: 12px;
  margin-top: 10px;
}
.pipeline-step {
  border: 2px solid #dcdfe6;
  border-radius: 8px;
  padding: 12px;
  min-width: 180px;
  background: #fafafa;
}
.pipeline-step.step-running {
  border-color: #409eff;
  background: #ecf5ff;
}
.pipeline-step.step-completed {
  border-color: #67c23a;
  background: #f0f9eb;
}
.pipeline-step.step-failed {
  border-color: #f56c6c;
  background: #fef0f0;
}
.pipeline-step.step-pending {
  border-color: #909399;
  background: #f4f4f5;
}
.pipeline-step.step-skipped {
  border-color: #e6a23c;
  background: #fdf6ec;
}
.step-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 6px;
}
.step-body {
  display: flex;
  flex-direction: column;
  font-size: 12px;
  color: #606266;
}
.step-type {
  color: #909399;
}
.step-link {
  color: #409eff;
  margin-top: 2px;
}
.step-time {
  margin-top: 2px;
}
</style>
