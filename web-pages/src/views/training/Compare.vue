<template>
  <div>
    <h2>学習実験比較ダッシュボード</h2>

    <!-- ジョブ選択セクション -->
    <el-card class="compare-section">
      <div slot="header">
        <span>比較する学習ジョブを選択</span>
      </div>
      <el-row :gutter="20">
        <el-col :span="18">
          <el-select
            v-model="selectedIds"
            multiple
            filterable
            remote
            reserve-keyword
            placeholder="学習ジョブを選択（2つ以上）"
            :remote-method="searchJobs"
            :loading="loadingJobs"
            style="width: 100%"
          >
            <el-option
              v-for="item in jobOptions"
              :key="item.id"
              :label="`${item.id}: ${item.name}`"
              :value="item.id"
            />
          </el-select>
        </el-col>
        <el-col :span="6">
          <el-button
            type="primary"
            :disabled="selectedIds.length < 2"
            :loading="loadingCompare"
            @click="fetchCompareData"
          >
            比較実行
          </el-button>
          <el-button @click="clearSelection">クリア</el-button>
        </el-col>
      </el-row>
    </el-card>

    <!-- 比較結果 -->
    <div v-if="compareData">
      <!-- メトリクス比較テーブル -->
      <el-card class="compare-section">
        <div slot="header">
          <span>メトリクス比較テーブル</span>
        </div>
        <el-table :data="metricsTableData" border stripe>
          <el-table-column prop="metric" label="メトリクス" width="180" fixed />
          <el-table-column
            v-for="job in compareData.jobs"
            :key="job.id"
            :label="`${job.id}: ${job.name}`"
            min-width="150"
          >
            <template slot-scope="scope">
              {{ scope.row['job_' + job.id] || '-' }}
            </template>
          </el-table-column>
        </el-table>
      </el-card>

      <!-- ハイパーパラメータ並列座標プロット -->
      <el-card class="compare-section">
        <div slot="header">
          <span>ハイパーパラメータ 並列座標プロット</span>
        </div>
        <div ref="parallelCoordinates" class="chart-container">
          <canvas ref="parallelCanvas" />
        </div>
      </el-card>

      <!-- 損失曲線の重ね合わせ -->
      <el-card class="compare-section">
        <div slot="header">
          <span>損失曲線の重ね合わせ表示</span>
        </div>
        <div class="chart-container">
          <canvas ref="lossCanvas" />
        </div>
        <div v-if="!hasLossData" class="no-data-message">
          <el-alert
            title="損失データなし"
            description='選択されたジョブにはLogSummaryからパース可能な損失データがありません。学習スクリプトでLogSummaryにJSON形式（例: {"epoch": [1,2,3], "loss": [0.5, 0.3, 0.1]}）でメトリクスを記録してください。'
            type="info"
            show-icon
          />
        </div>
      </el-card>

      <!-- ジョブ詳細比較 -->
      <el-card class="compare-section">
        <div slot="header">
          <span>ジョブ詳細比較</span>
        </div>
        <el-table :data="detailTableData" border stripe>
          <el-table-column prop="field" label="項目" width="180" fixed />
          <el-table-column
            v-for="job in compareData.jobs"
            :key="'detail-' + job.id"
            :label="`${job.id}: ${job.name}`"
            min-width="200"
          >
            <template slot-scope="scope">
              {{ scope.row['job_' + job.id] || '-' }}
            </template>
          </el-table-column>
        </el-table>
      </el-card>
    </div>
  </div>
</template>

<script>
import api from '@/api/api'

export default {
  title: '学習実験比較',
  data() {
    return {
      selectedIds: [],
      jobOptions: [],
      loadingJobs: false,
      loadingCompare: false,
      compareData: null,
      hasLossData: false,
      lossChart: null,
    }
  },
  computed: {
    metricsTableData() {
      if (!this.compareData) return []
      const rows = []
      const metrics = [
        { key: 'status', label: 'ステータス' },
        { key: 'executionTime', label: '実行時間' },
        { key: 'cpu', label: 'CPU' },
        { key: 'memory', label: 'メモリ (GB)' },
        { key: 'gpu', label: 'GPU' },
        { key: 'dataSetName', label: 'データセット' },
        { key: 'entryPoint', label: 'エントリポイント' },
      ]

      // Add LogSummary parsed metrics if available
      const parsedMetrics = this.getParsedMetrics()
      const allMetricKeys = new Set()
      Object.values(parsedMetrics).forEach(m => {
        Object.keys(m).forEach(k => {
          if (k !== 'epoch' && k !== 'step') allMetricKeys.add(k)
        })
      })

      metrics.forEach(m => {
        const row = { metric: m.label }
        this.compareData.jobs.forEach(job => {
          row['job_' + job.id] = this.getMetricValue(job, m.key)
        })
        rows.push(row)
      })

      // Add dynamic metrics from LogSummary
      allMetricKeys.forEach(key => {
        const row = { metric: key }
        this.compareData.jobs.forEach(job => {
          const pm = parsedMetrics[job.id]
          if (pm && pm[key]) {
            const values = pm[key]
            if (Array.isArray(values) && values.length > 0) {
              row['job_' + job.id] = values[values.length - 1].toFixed(4)
            } else {
              row['job_' + job.id] = String(values)
            }
          } else {
            row['job_' + job.id] = '-'
          }
        })
        rows.push(row)
      })

      return rows
    },
    detailTableData() {
      if (!this.compareData) return []
      const fields = [
        { key: 'id', label: 'ID' },
        { key: 'name', label: '学習名' },
        { key: 'status', label: 'ステータス' },
        { key: 'createdBy', label: '実行者' },
        { key: 'startedAt', label: '開始日時' },
        { key: 'completedAt', label: '完了日時' },
        { key: 'executionTime', label: '実行時間' },
        { key: 'entryPoint', label: 'エントリポイント' },
        { key: 'containerImage', label: 'コンテナイメージ' },
        { key: 'dataSetName', label: 'データセット' },
        { key: 'cpu', label: 'CPU' },
        { key: 'memory', label: 'メモリ (GB)' },
        { key: 'gpu', label: 'GPU' },
        { key: 'partition', label: 'パーティション' },
        { key: 'memo', label: 'メモ' },
        { key: 'tags', label: 'タグ' },
      ]

      // Add hyperparameters
      const allOptionKeys = new Set()
      this.compareData.jobs.forEach(job => {
        if (job.options) {
          job.options.forEach(opt => allOptionKeys.add(opt.key))
        }
      })

      const rows = []
      fields.forEach(f => {
        const row = { field: f.label }
        this.compareData.jobs.forEach(job => {
          let val = job[f.key]
          if (f.key === 'tags' && Array.isArray(val)) {
            val = val.join(', ')
          }
          row['job_' + job.id] = val != null ? String(val) : '-'
        })
        rows.push(row)
      })

      allOptionKeys.forEach(key => {
        const row = { field: `[パラメータ] ${key}` }
        this.compareData.jobs.forEach(job => {
          const opt = job.options ? job.options.find(o => o.key === key) : null
          row['job_' + job.id] = opt ? opt.value : '-'
        })
        rows.push(row)
      })

      return rows
    },
  },
  async created() {
    await this.loadAllJobs()
    // URL query params から初期選択
    if (this.$route.query.ids) {
      const ids = this.$route.query.ids
        .split(',')
        .map(id => parseInt(id))
        .filter(id => !isNaN(id))
      if (ids.length >= 2) {
        this.selectedIds = ids
        await this.fetchCompareData()
      }
    }
  },
  methods: {
    async loadAllJobs() {
      try {
        const res = await api.training.getSimple()
        this.jobOptions = res.data
      } catch (e) {
        this.jobOptions = []
      }
    },
    async searchJobs(query) {
      if (query !== '') {
        this.loadingJobs = true
        try {
          const res = await api.training.getSimple()
          this.jobOptions = res.data.filter(
            item =>
              String(item.id).includes(query) ||
              (item.name && item.name.includes(query)),
          )
        } catch (e) {
          this.jobOptions = []
        }
        this.loadingJobs = false
      } else {
        await this.loadAllJobs()
      }
    },
    async fetchCompareData() {
      if (this.selectedIds.length < 2) return
      this.loadingCompare = true
      try {
        const res = await api.training.getCompare({
          ids: this.selectedIds.join(','),
        })
        this.compareData = res.data
        this.$nextTick(() => {
          this.renderParallelCoordinates()
          this.renderLossChart()
        })
      } catch (e) {
        this.$message.error('比較データの取得に失敗しました。')
      }
      this.loadingCompare = false
    },
    clearSelection() {
      this.selectedIds = []
      this.compareData = null
      this.hasLossData = false
    },
    getMetricValue(job, key) {
      const val = job[key]
      if (val == null) return '-'
      return String(val)
    },
    getParsedMetrics() {
      const result = {}
      if (!this.compareData) return result
      this.compareData.jobs.forEach(job => {
        if (job.logSummary) {
          try {
            const parsed = JSON.parse(job.logSummary)
            result[job.id] = parsed
          } catch (e) {
            // LogSummary is not JSON, try key=value parsing
            const metrics = {}
            const lines = job.logSummary.split('\n')
            lines.forEach(line => {
              const match = line.match(/^(.+?)\s*[=:]\s*(.+)$/)
              if (match) {
                const key = match[1].trim()
                const val = parseFloat(match[2].trim())
                if (!isNaN(val)) {
                  metrics[key] = val
                }
              }
            })
            if (Object.keys(metrics).length > 0) {
              result[job.id] = metrics
            }
          }
        }
      })
      return result
    },
    renderParallelCoordinates() {
      if (!this.compareData || !this.$refs.parallelCanvas) return

      const canvas = this.$refs.parallelCanvas
      const ctx = canvas.getContext('2d')
      const container = this.$refs.parallelCoordinates

      canvas.width = container.clientWidth || 800
      canvas.height = 400

      ctx.clearRect(0, 0, canvas.width, canvas.height)

      // Collect all option keys
      const allKeys = new Set()
      this.compareData.jobs.forEach(job => {
        if (job.options) {
          job.options.forEach(opt => allKeys.add(opt.key))
        }
      })
      // Add resource params
      allKeys.add('cpu')
      allKeys.add('memory')
      allKeys.add('gpu')

      const axes = Array.from(allKeys)
      if (axes.length === 0) return

      const padding = { top: 50, bottom: 50, left: 80, right: 40 }
      const plotWidth = canvas.width - padding.left - padding.right
      const plotHeight = canvas.height - padding.top - padding.bottom

      // Calculate ranges for each axis
      const ranges = {}
      axes.forEach(key => {
        let min = Infinity
        let max = -Infinity
        this.compareData.jobs.forEach(job => {
          let val
          if (key === 'cpu') val = job.cpu
          else if (key === 'memory') val = job.memory
          else if (key === 'gpu') val = job.gpu
          else {
            const opt = job.options
              ? job.options.find(o => o.key === key)
              : null
            val = opt ? parseFloat(opt.value) : NaN
          }
          if (!isNaN(val)) {
            min = Math.min(min, val)
            max = Math.max(max, val)
          }
        })
        if (min === Infinity) {
          min = 0
          max = 1
        }
        if (min === max) {
          min -= 0.5
          max += 0.5
        }
        ranges[key] = { min, max }
      })

      const colors = [
        '#409EFF',
        '#67C23A',
        '#E6A23C',
        '#F56C6C',
        '#909399',
        '#1ABC9C',
        '#9B59B6',
        '#3498DB',
      ]

      // Draw axes
      const axisSpacing = plotWidth / (axes.length - 1 || 1)
      ctx.strokeStyle = '#DCDFE6'
      ctx.lineWidth = 1
      ctx.font = '11px sans-serif'
      ctx.fillStyle = '#606266'
      ctx.textAlign = 'center'

      axes.forEach((key, i) => {
        const x = padding.left + i * axisSpacing
        ctx.beginPath()
        ctx.moveTo(x, padding.top)
        ctx.lineTo(x, padding.top + plotHeight)
        ctx.stroke()

        // Axis label
        ctx.save()
        ctx.translate(x, padding.top - 10)
        ctx.fillText(key, 0, 0)
        ctx.restore()

        // Min/Max labels
        ctx.fillText(ranges[key].max.toFixed(2), x, padding.top - 2)
        ctx.fillText(
          ranges[key].min.toFixed(2),
          x,
          padding.top + plotHeight + 15,
        )
      })

      // Draw lines for each job
      this.compareData.jobs.forEach((job, jobIdx) => {
        ctx.strokeStyle = colors[jobIdx % colors.length]
        ctx.lineWidth = 2
        ctx.beginPath()

        let started = false
        axes.forEach((key, i) => {
          let val
          if (key === 'cpu') val = job.cpu
          else if (key === 'memory') val = job.memory
          else if (key === 'gpu') val = job.gpu
          else {
            const opt = job.options
              ? job.options.find(o => o.key === key)
              : null
            val = opt ? parseFloat(opt.value) : NaN
          }

          if (!isNaN(val)) {
            const x = padding.left + i * axisSpacing
            const range = ranges[key]
            const normalized = (val - range.min) / (range.max - range.min)
            const y = padding.top + plotHeight - normalized * plotHeight

            if (!started) {
              ctx.moveTo(x, y)
              started = true
            } else {
              ctx.lineTo(x, y)
            }
          }
        })
        ctx.stroke()
      })

      // Legend
      const legendY = canvas.height - 15
      let legendX = padding.left
      ctx.font = '12px sans-serif'
      this.compareData.jobs.forEach((job, idx) => {
        ctx.fillStyle = colors[idx % colors.length]
        ctx.fillRect(legendX, legendY - 8, 12, 12)
        ctx.fillStyle = '#303133'
        ctx.fillText(`${job.id}: ${job.name}`, legendX + 16, legendY + 2)
        legendX += ctx.measureText(`${job.id}: ${job.name}`).width + 30
      })
    },
    renderLossChart() {
      if (!this.compareData || !this.$refs.lossCanvas) return

      const canvas = this.$refs.lossCanvas
      const ctx = canvas.getContext('2d')

      canvas.width = canvas.parentElement.clientWidth || 800
      canvas.height = 350

      ctx.clearRect(0, 0, canvas.width, canvas.height)

      const parsedMetrics = this.getParsedMetrics()

      // Find loss-related data
      const lossData = {}
      let hasData = false
      Object.entries(parsedMetrics).forEach(([jobId, metrics]) => {
        const lossKey = Object.keys(metrics).find(
          k =>
            k.toLowerCase().includes('loss') ||
            k.toLowerCase().includes('error'),
        )
        if (lossKey && Array.isArray(metrics[lossKey])) {
          lossData[jobId] = metrics[lossKey]
          hasData = true
        }
      })

      this.hasLossData = hasData
      if (!hasData) return

      const padding = { top: 40, bottom: 50, left: 60, right: 20 }
      const plotWidth = canvas.width - padding.left - padding.right
      const plotHeight = canvas.height - padding.top - padding.bottom

      // Find ranges
      let maxEpoch = 0
      let minLoss = Infinity
      let maxLoss = -Infinity
      Object.values(lossData).forEach(values => {
        maxEpoch = Math.max(maxEpoch, values.length)
        values.forEach(v => {
          minLoss = Math.min(minLoss, v)
          maxLoss = Math.max(maxLoss, v)
        })
      })

      if (minLoss === maxLoss) {
        minLoss -= 0.1
        maxLoss += 0.1
      }

      const colors = [
        '#409EFF',
        '#67C23A',
        '#E6A23C',
        '#F56C6C',
        '#909399',
        '#1ABC9C',
        '#9B59B6',
        '#3498DB',
      ]

      // Draw grid
      ctx.strokeStyle = '#EBEEF5'
      ctx.lineWidth = 0.5
      for (let i = 0; i <= 5; i++) {
        const y = padding.top + (plotHeight * i) / 5
        ctx.beginPath()
        ctx.moveTo(padding.left, y)
        ctx.lineTo(padding.left + plotWidth, y)
        ctx.stroke()
      }

      // Draw axes
      ctx.strokeStyle = '#DCDFE6'
      ctx.lineWidth = 1
      ctx.beginPath()
      ctx.moveTo(padding.left, padding.top)
      ctx.lineTo(padding.left, padding.top + plotHeight)
      ctx.lineTo(padding.left + plotWidth, padding.top + plotHeight)
      ctx.stroke()

      // Y-axis labels
      ctx.font = '11px sans-serif'
      ctx.fillStyle = '#606266'
      ctx.textAlign = 'right'
      for (let i = 0; i <= 5; i++) {
        const val = maxLoss - ((maxLoss - minLoss) * i) / 5
        const y = padding.top + (plotHeight * i) / 5
        ctx.fillText(val.toFixed(4), padding.left - 5, y + 4)
      }

      // X-axis labels
      ctx.textAlign = 'center'
      for (let i = 0; i <= Math.min(maxEpoch - 1, 10); i++) {
        const epoch = Math.round(
          (i * (maxEpoch - 1)) / Math.min(maxEpoch - 1, 10),
        )
        const x = padding.left + (epoch / (maxEpoch - 1)) * plotWidth
        ctx.fillText(String(epoch + 1), x, padding.top + plotHeight + 15)
      }

      // Axis titles
      ctx.fillStyle = '#303133'
      ctx.font = '12px sans-serif'
      ctx.fillText('Epoch', padding.left + plotWidth / 2, canvas.height - 10)
      ctx.save()
      ctx.translate(15, padding.top + plotHeight / 2)
      ctx.rotate(-Math.PI / 2)
      ctx.fillText('Loss', 0, 0)
      ctx.restore()

      // Draw loss curves
      let colorIdx = 0
      const jobIdToIdx = {}
      this.compareData.jobs.forEach((job, idx) => {
        jobIdToIdx[job.id] = idx
      })

      Object.entries(lossData).forEach(([jobId, values]) => {
        const idx =
          jobIdToIdx[jobId] !== undefined ? jobIdToIdx[jobId] : colorIdx
        ctx.strokeStyle = colors[idx % colors.length]
        ctx.lineWidth = 2
        ctx.beginPath()

        values.forEach((val, i) => {
          const x = padding.left + (i / (maxEpoch - 1 || 1)) * plotWidth
          const y =
            padding.top +
            plotHeight -
            ((val - minLoss) / (maxLoss - minLoss)) * plotHeight

          if (i === 0) ctx.moveTo(x, y)
          else ctx.lineTo(x, y)
        })
        ctx.stroke()
        colorIdx++
      })

      // Legend
      const legendY = padding.top - 15
      let legendX = padding.left
      ctx.font = '12px sans-serif'
      this.compareData.jobs.forEach((job, idx) => {
        if (lossData[job.id]) {
          ctx.fillStyle = colors[idx % colors.length]
          ctx.fillRect(legendX, legendY - 8, 12, 12)
          ctx.fillStyle = '#303133'
          ctx.textAlign = 'left'
          ctx.fillText(`${job.id}: ${job.name}`, legendX + 16, legendY + 2)
          legendX += ctx.measureText(`${job.id}: ${job.name}`).width + 30
        }
      })
    },
  },
}
</script>

<style scoped>
.compare-section {
  margin-top: 20px;
}
.chart-container {
  width: 100%;
  min-height: 400px;
  position: relative;
}
.chart-container canvas {
  width: 100%;
}
.no-data-message {
  margin-top: 10px;
}
</style>
