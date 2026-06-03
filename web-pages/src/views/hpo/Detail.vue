<template>
  <el-dialog
    class="dialog"
    title="HPOジョブ詳細"
    :visible.sync="dialogVisible"
    :before-close="closeDialog"
    :close-on-click-modal="false"
    width="90%"
  >
    <div v-if="detail">
      <el-row :gutter="20">
        <el-col :span="8">
          <el-descriptions :column="1" border size="small">
            <el-descriptions-item label="ジョブ名">{{ detail.name }}</el-descriptions-item>
            <el-descriptions-item label="アルゴリズム">
              <el-tag :type="getAlgorithmTagType(detail.algorithm)" size="small">
                {{ detail.algorithm }}
              </el-tag>
            </el-descriptions-item>
            <el-descriptions-item label="ステータス">
              <el-tag :type="getStatusTagType(detail.status)" size="small">
                {{ detail.status }}
              </el-tag>
            </el-descriptions-item>
            <el-descriptions-item label="目的メトリクス">{{ detail.objectiveMetric }}</el-descriptions-item>
            <el-descriptions-item label="最適化方向">{{ detail.optimizationDirection === 'minimize' ? '最小化' : '最大化' }}</el-descriptions-item>
            <el-descriptions-item label="進捗">{{ detail.completedTrials }} / {{ detail.maxTrials }}</el-descriptions-item>
            <el-descriptions-item label="データセット">{{ detail.dataSetName }}</el-descriptions-item>
            <el-descriptions-item label="作成日時">{{ detail.createdAt }}</el-descriptions-item>
            <el-descriptions-item label="完了日時">{{ detail.completedAt || '-' }}</el-descriptions-item>
          </el-descriptions>
        </el-col>

        <el-col :span="16">
          <!-- 最適パラメータ推薦 -->
          <el-card v-if="detail.bestTrial" class="best-trial-card" shadow="always">
            <div slot="header">
              <span style="font-weight: bold; color: #409EFF;">
                <i class="el-icon-trophy" /> 最適パラメータ推薦
              </span>
            </div>
            <el-row :gutter="20">
              <el-col :span="12">
                <h4>パラメータ</h4>
                <el-table :data="bestTrialParams" size="small" border>
                  <el-table-column prop="name" label="パラメータ名" />
                  <el-table-column prop="value" label="値" />
                </el-table>
              </el-col>
              <el-col :span="12">
                <h4>結果</h4>
                <p>
                  <strong>メトリクス値:</strong>
                  {{ detail.bestTrial.metricValue != null ? detail.bestTrial.metricValue.toFixed(6) : '-' }}
                </p>
                <p>
                  <strong>トライアル #{{ detail.bestTrial.trialNo }}</strong>
                </p>
              </el-col>
            </el-row>
          </el-card>
        </el-col>
      </el-row>

      <!-- 探索結果の可視化グラフ -->
      <el-row :gutter="20" style="margin-top: 20px;">
        <el-col :span="24">
          <el-card>
            <div slot="header">
              <span style="font-weight: bold;">探索結果可視化</span>
              <el-select
                v-model="selectedParam"
                placeholder="パラメータ選択"
                size="small"
                style="float: right; width: 200px;"
              >
                <el-option
                  v-for="param in paramNames"
                  :key="param"
                  :label="param"
                  :value="param"
                />
              </el-select>
            </div>
            <div class="chart-container">
              <canvas ref="scatterChart" width="800" height="300" />
            </div>
          </el-card>
        </el-col>
      </el-row>

      <!-- トライアル一覧 -->
      <el-row style="margin-top: 20px;">
        <el-col :span="24">
          <h3>トライアル一覧</h3>
          <el-table :data="detail.trials" border size="small" max-height="400">
            <el-table-column prop="trialNo" label="#" width="60px" />
            <el-table-column label="パラメータ" min-width="300px">
              <template slot-scope="scope">
                <span v-for="(value, key) in scope.row.parameters" :key="key" class="param-tag">
                  <el-tag size="mini" type="info">{{ key }}={{ value }}</el-tag>
                </span>
              </template>
            </el-table-column>
            <el-table-column prop="metricValue" label="メトリクス値" width="140px">
              <template slot-scope="scope">
                {{ scope.row.metricValue != null ? scope.row.metricValue.toFixed(6) : '-' }}
              </template>
            </el-table-column>
            <el-table-column prop="status" label="ステータス" width="100px">
              <template slot-scope="scope">
                <el-tag :type="getStatusTagType(scope.row.status)" size="mini">
                  {{ scope.row.status }}
                </el-tag>
              </template>
            </el-table-column>
            <el-table-column prop="startedAt" label="開始" width="160px" />
            <el-table-column prop="completedAt" label="完了" width="160px" />
          </el-table>
        </el-col>
      </el-row>
    </div>
  </el-dialog>
</template>

<script>
import api from '@/api/api'

export default {
  props: {
    id: {
      type: String,
      default: null,
    },
  },
  data() {
    return {
      dialogVisible: true,
      detail: null,
      selectedParam: null,
      chartInstance: null,
    }
  },
  computed: {
    paramNames() {
      if (!this.detail || !this.detail.trials || this.detail.trials.length === 0) {
        return []
      }
      let firstTrial = this.detail.trials.find(t => t.parameters)
      return firstTrial ? Object.keys(firstTrial.parameters) : []
    },
    bestTrialParams() {
      if (!this.detail || !this.detail.bestTrial) return []
      return Object.entries(this.detail.bestTrial.parameters).map(([name, value]) => ({
        name,
        value,
      }))
    },
  },
  watch: {
    selectedParam() {
      this.drawChart()
    },
  },
  async created() {
    await this.fetchDetail()
  },
  methods: {
    async fetchDetail() {
      let response = await api.hpo.getById({ id: this.id })
      this.detail = response.data
      if (this.paramNames.length > 0) {
        this.selectedParam = this.paramNames[0]
      }
      this.$nextTick(() => {
        this.drawChart()
      })
    },
    drawChart() {
      if (!this.detail || !this.selectedParam || !this.$refs.scatterChart) return

      let canvas = this.$refs.scatterChart
      let ctx = canvas.getContext('2d')
      ctx.clearRect(0, 0, canvas.width, canvas.height)

      let completedTrials = this.detail.trials.filter(
        t => t.status === 'Completed' && t.metricValue != null && t.parameters[this.selectedParam] != null
      )

      if (completedTrials.length === 0) {
        ctx.font = '14px Arial'
        ctx.fillStyle = '#909399'
        ctx.textAlign = 'center'
        ctx.fillText('完了済みトライアルがありません', canvas.width / 2, canvas.height / 2)
        return
      }

      let paramValues = completedTrials.map(t => parseFloat(t.parameters[this.selectedParam]) || 0)
      let metricValues = completedTrials.map(t => t.metricValue)

      let xMin = Math.min(...paramValues)
      let xMax = Math.max(...paramValues)
      let yMin = Math.min(...metricValues)
      let yMax = Math.max(...metricValues)

      // Add padding
      let xPad = (xMax - xMin) * 0.1 || 1
      let yPad = (yMax - yMin) * 0.1 || 0.1
      xMin -= xPad
      xMax += xPad
      yMin -= yPad
      yMax += yPad

      let chartLeft = 80
      let chartTop = 30
      let chartRight = canvas.width - 30
      let chartBottom = canvas.height - 50
      let chartWidth = chartRight - chartLeft
      let chartHeight = chartBottom - chartTop

      // Draw axes
      ctx.strokeStyle = '#DCDFE6'
      ctx.lineWidth = 1
      ctx.beginPath()
      ctx.moveTo(chartLeft, chartTop)
      ctx.lineTo(chartLeft, chartBottom)
      ctx.lineTo(chartRight, chartBottom)
      ctx.stroke()

      // Draw labels
      ctx.font = '12px Arial'
      ctx.fillStyle = '#606266'
      ctx.textAlign = 'center'
      ctx.fillText(this.selectedParam, (chartLeft + chartRight) / 2, canvas.height - 10)

      ctx.save()
      ctx.translate(15, (chartTop + chartBottom) / 2)
      ctx.rotate(-Math.PI / 2)
      ctx.fillText(this.detail.objectiveMetric, 0, 0)
      ctx.restore()

      // Draw grid and ticks
      ctx.font = '10px Arial'
      ctx.fillStyle = '#909399'
      for (let i = 0; i <= 5; i++) {
        let y = chartBottom - (i / 5) * chartHeight
        let val = yMin + (i / 5) * (yMax - yMin)
        ctx.textAlign = 'right'
        ctx.fillText(val.toFixed(4), chartLeft - 5, y + 3)

        ctx.strokeStyle = '#EBEEF5'
        ctx.beginPath()
        ctx.moveTo(chartLeft, y)
        ctx.lineTo(chartRight, y)
        ctx.stroke()
      }

      for (let i = 0; i <= 5; i++) {
        let x = chartLeft + (i / 5) * chartWidth
        let val = xMin + (i / 5) * (xMax - xMin)
        ctx.textAlign = 'center'
        ctx.fillText(val.toFixed(2), x, chartBottom + 15)
      }

      // Draw data points
      let bestIdx = -1
      if (this.detail.bestTrial) {
        bestIdx = completedTrials.findIndex(t => t.id === this.detail.bestTrial.id)
      }

      for (let i = 0; i < completedTrials.length; i++) {
        let x = chartLeft + ((paramValues[i] - xMin) / (xMax - xMin)) * chartWidth
        let y = chartBottom - ((metricValues[i] - yMin) / (yMax - yMin)) * chartHeight

        ctx.beginPath()
        if (i === bestIdx) {
          ctx.fillStyle = '#E6A23C'
          ctx.arc(x, y, 7, 0, Math.PI * 2)
          ctx.fill()
          ctx.strokeStyle = '#F56C6C'
          ctx.lineWidth = 2
          ctx.stroke()
        } else {
          ctx.fillStyle = '#409EFF'
          ctx.arc(x, y, 5, 0, Math.PI * 2)
          ctx.fill()
        }
      }

      // Legend
      ctx.font = '11px Arial'
      ctx.fillStyle = '#409EFF'
      ctx.beginPath()
      ctx.arc(chartRight - 100, chartTop + 10, 5, 0, Math.PI * 2)
      ctx.fill()
      ctx.fillStyle = '#606266'
      ctx.textAlign = 'left'
      ctx.fillText('トライアル', chartRight - 90, chartTop + 14)

      ctx.fillStyle = '#E6A23C'
      ctx.beginPath()
      ctx.arc(chartRight - 100, chartTop + 28, 5, 0, Math.PI * 2)
      ctx.fill()
      ctx.fillStyle = '#606266'
      ctx.fillText('ベスト', chartRight - 90, chartTop + 32)
    },
    getAlgorithmTagType(algorithm) {
      switch (algorithm?.toLowerCase()) {
        case 'grid': return ''
        case 'random': return 'success'
        case 'bayes': return 'warning'
        default: return 'info'
      }
    },
    getStatusTagType(status) {
      switch (status) {
        case 'Running': return 'primary'
        case 'Completed': return 'success'
        case 'Failed': return 'danger'
        case 'Cancelled': return 'warning'
        default: return 'info'
      }
    },
    closeDialog() {
      this.dialogVisible = false
      this.$router.push('/hpo')
    },
  },
}
</script>

<style scoped>
.best-trial-card {
  border-left: 4px solid #409EFF;
}
.param-tag {
  margin-right: 4px;
  margin-bottom: 2px;
  display: inline-block;
}
.chart-container {
  width: 100%;
  overflow-x: auto;
}
</style>
