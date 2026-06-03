<template>
  <div class="dashboard-container">
    <!-- サマリーカード -->
    <el-row :gutter="20" class="summary-row">
      <el-col :span="6">
        <el-card class="summary-card" shadow="hover">
          <div class="summary-title">CPU 使用率</div>
          <div
            class="summary-value"
            :class="getAlertClass(summary.cpuUsageRate)"
          >
            {{ formatRate(summary.cpuUsageRate) }}%
          </div>
          <div class="summary-detail">
            {{ summary.usedCpu | fixed1 }} /
            {{ summary.totalCpu | fixed1 }} コア
          </div>
          <el-progress
            :percentage="clampPercentage(summary.cpuUsageRate)"
            :color="getProgressColor(summary.cpuUsageRate)"
            :show-text="false"
          />
        </el-card>
      </el-col>
      <el-col :span="6">
        <el-card class="summary-card" shadow="hover">
          <div class="summary-title">メモリ使用率</div>
          <div
            class="summary-value"
            :class="getAlertClass(summary.memoryUsageRate)"
          >
            {{ formatRate(summary.memoryUsageRate) }}%
          </div>
          <div class="summary-detail">
            {{ summary.usedMemory | fixed1 }} /
            {{ summary.totalMemory | fixed1 }} GB
          </div>
          <el-progress
            :percentage="clampPercentage(summary.memoryUsageRate)"
            :color="getProgressColor(summary.memoryUsageRate)"
            :show-text="false"
          />
        </el-card>
      </el-col>
      <el-col :span="6">
        <el-card class="summary-card" shadow="hover">
          <div class="summary-title">GPU 使用率</div>
          <div
            class="summary-value"
            :class="getAlertClass(summary.gpuUsageRate)"
          >
            {{ formatRate(summary.gpuUsageRate) }}%
          </div>
          <div class="summary-detail">
            {{ summary.usedGpu | fixed1 }} / {{ summary.totalGpu | fixed1 }}
          </div>
          <el-progress
            :percentage="clampPercentage(summary.gpuUsageRate)"
            :color="getProgressColor(summary.gpuUsageRate)"
            :show-text="false"
          />
        </el-card>
      </el-col>
      <el-col :span="6">
        <el-card class="summary-card" shadow="hover">
          <div class="summary-title">クラスタ状態</div>
          <div class="summary-value cluster-info">
            {{ summary.activeNodeCount }}/{{ summary.totalNodeCount }}
          </div>
          <div class="summary-detail">
            ノード稼働中 / コンテナ: {{ summary.runningContainerCount }}
          </div>
          <el-progress
            :percentage="nodePercentage"
            :color="getProgressColor(nodePercentage)"
            :show-text="false"
          />
        </el-card>
      </el-col>
    </el-row>

    <!-- アラート表示 -->
    <el-row v-if="alerts.length > 0" class="alert-row">
      <el-col :span="24">
        <el-alert
          v-for="(alert, index) in alerts"
          :key="index"
          :title="alert.message"
          :type="alert.type"
          show-icon
          class="alert-item"
        />
      </el-col>
    </el-row>

    <!-- 時系列チャート -->
    <el-row :gutter="20" class="chart-row">
      <el-col :span="24">
        <el-card shadow="hover">
          <div slot="header" class="chart-header">
            <span>リソース使用履歴</span>
            <el-radio-group
              v-model="historyPeriod"
              size="small"
              @change="fetchHistory"
            >
              <el-radio-button label="hour">1時間</el-radio-button>
              <el-radio-button label="day">1日</el-radio-button>
              <el-radio-button label="week">1週間</el-radio-button>
            </el-radio-group>
          </div>
          <div class="chart-container">
            <canvas ref="historyChart" />
          </div>
        </el-card>
      </el-col>
    </el-row>

    <!-- テナント別使用量 + ノード一覧 -->
    <el-row :gutter="20" class="chart-row">
      <el-col :span="12">
        <el-card shadow="hover">
          <div slot="header">テナント別リソース消費量</div>
          <div class="chart-container">
            <canvas ref="tenantChart" />
          </div>
        </el-card>
      </el-col>
      <el-col :span="12">
        <el-card shadow="hover">
          <div slot="header">ノード一覧とステータス</div>
          <el-table
            :data="nodeStatus"
            height="350"
            class="node-table"
            size="small"
          >
            <el-table-column prop="name" label="ノード名" width="auto" />
            <el-table-column label="ステータス" width="110" align="center">
              <template slot-scope="scope">
                <el-tag :type="getStatusTagType(scope.row.status)" size="small">
                  {{ scope.row.status }}
                </el-tag>
              </template>
            </el-table-column>
            <el-table-column label="CPU" width="120" align="right">
              <template slot-scope="scope">
                {{ scope.row.usedCpu | fixed1 }} /
                {{ scope.row.allocatableCpu | fixed1 }}
              </template>
            </el-table-column>
            <el-table-column label="メモリ(GB)" width="120" align="right">
              <template slot-scope="scope">
                {{ scope.row.usedMemory | fixed1 }} /
                {{ scope.row.allocatableMemory | fixed1 }}
              </template>
            </el-table-column>
            <el-table-column label="GPU" width="100" align="right">
              <template slot-scope="scope">
                {{ scope.row.usedGpu | fixed1 }} /
                {{ scope.row.allocatableGpu | fixed1 }}
              </template>
            </el-table-column>
            <el-table-column
              prop="containerCount"
              label="コンテナ"
              width="80"
              align="center"
            />
          </el-table>
        </el-card>
      </el-col>
    </el-row>

    <!-- アラート閾値設定 -->
    <el-row class="chart-row">
      <el-col :span="24">
        <el-card shadow="hover">
          <div slot="header">アラート閾値設定</div>
          <el-form :inline="true" size="small">
            <el-form-item label="CPU 閾値(%)">
              <el-input-number
                v-model="alertThresholds.cpu"
                :min="0"
                :max="100"
                @change="saveThresholds"
              />
            </el-form-item>
            <el-form-item label="メモリ 閾値(%)">
              <el-input-number
                v-model="alertThresholds.memory"
                :min="0"
                :max="100"
                @change="saveThresholds"
              />
            </el-form-item>
            <el-form-item label="GPU 閾値(%)">
              <el-input-number
                v-model="alertThresholds.gpu"
                :min="0"
                :max="100"
                @change="saveThresholds"
              />
            </el-form-item>
          </el-form>
        </el-card>
      </el-col>
    </el-row>
  </div>
</template>

<script>
import { createNamespacedHelpers } from 'vuex'
import Chart from 'chart.js'

const { mapGetters, mapActions } = createNamespacedHelpers('resource')

export default {
  filters: {
    fixed1(value) {
      if (value === undefined || value === null) return '0.0'
      return Number(value).toFixed(1)
    },
  },
  data() {
    return {
      historyPeriod: 'hour',
      historyChart: null,
      tenantChart: null,
      refreshTimer: null,
      alertThresholds: {
        cpu: 80,
        memory: 80,
        gpu: 80,
      },
    }
  },
  computed: {
    ...mapGetters([
      'dashboardSummary',
      'dashboardTenantUsage',
      'dashboardHistory',
      'dashboardNodeStatus',
    ]),
    summary() {
      return this.dashboardSummary || {}
    },
    nodeStatus() {
      return this.dashboardNodeStatus || []
    },
    nodePercentage() {
      if (!this.summary.totalNodeCount) return 0
      return Math.round(
        (this.summary.activeNodeCount / this.summary.totalNodeCount) * 100,
      )
    },
    alerts() {
      let alerts = []
      if (
        this.summary.cpuUsageRate !== undefined &&
        this.summary.cpuUsageRate > this.alertThresholds.cpu
      ) {
        alerts.push({
          type: 'warning',
          message: `CPU使用率が閾値(${
            this.alertThresholds.cpu
          }%)を超えています: ${this.formatRate(this.summary.cpuUsageRate)}%`,
        })
      }
      if (
        this.summary.memoryUsageRate !== undefined &&
        this.summary.memoryUsageRate > this.alertThresholds.memory
      ) {
        alerts.push({
          type: 'warning',
          message: `メモリ使用率が閾値(${
            this.alertThresholds.memory
          }%)を超えています: ${this.formatRate(this.summary.memoryUsageRate)}%`,
        })
      }
      if (
        this.summary.gpuUsageRate !== undefined &&
        this.summary.gpuUsageRate > this.alertThresholds.gpu
      ) {
        alerts.push({
          type: 'warning',
          message: `GPU使用率が閾値(${
            this.alertThresholds.gpu
          }%)を超えています: ${this.formatRate(this.summary.gpuUsageRate)}%`,
        })
      }
      return alerts
    },
  },
  watch: {
    dashboardHistory() {
      this.renderHistoryChart()
    },
    dashboardTenantUsage() {
      this.renderTenantChart()
    },
  },
  async created() {
    this.loadThresholds()
    await this.fetchAllData()
    // 30秒ごとに自動更新
    this.refreshTimer = setInterval(() => {
      this.fetchAllData()
    }, 30000)
  },
  beforeDestroy() {
    if (this.refreshTimer) {
      clearInterval(this.refreshTimer)
    }
    if (this.historyChart) {
      this.historyChart.destroy()
    }
    if (this.tenantChart) {
      this.tenantChart.destroy()
    }
  },
  methods: {
    ...mapActions([
      'fetchDashboardSummary',
      'fetchDashboardTenantUsage',
      'fetchDashboardHistory',
      'fetchDashboardNodeStatus',
    ]),
    async fetchAllData() {
      await Promise.all([
        this.fetchDashboardSummary(),
        this.fetchDashboardTenantUsage(),
        this.fetchDashboardHistory({ period: this.historyPeriod }),
        this.fetchDashboardNodeStatus(),
      ])
    },
    async fetchHistory() {
      await this.fetchDashboardHistory({ period: this.historyPeriod })
    },
    formatRate(value) {
      if (value === undefined || value === null) return '0.0'
      return Number(value).toFixed(1)
    },
    clampPercentage(value) {
      if (!value) return 0
      return Math.min(100, Math.max(0, Math.round(value)))
    },
    getProgressColor(percentage) {
      if (percentage >= 90) return '#F56C6C'
      if (percentage >= 70) return '#E6A23C'
      return '#67C23A'
    },
    getAlertClass(rate) {
      if (!rate) return ''
      if (rate >= 90) return 'rate-danger'
      if (rate >= 70) return 'rate-warning'
      return 'rate-normal'
    },
    getStatusTagType(status) {
      switch (status) {
        case 'Ready':
          return 'success'
        case 'NotReady':
          return 'warning'
        case 'Disconnected':
          return 'danger'
        default:
          return 'info'
      }
    },
    renderHistoryChart() {
      let history = this.dashboardHistory
      if (!history || !history.labels || history.labels.length === 0) return

      if (this.historyChart) {
        this.historyChart.destroy()
      }

      let ctx = this.$refs.historyChart
      if (!ctx) return

      this.historyChart = new Chart(ctx, {
        type: 'line',
        data: {
          labels: history.labels,
          datasets: [
            {
              label: 'CPU (%)',
              data: history.cpuUsage,
              borderColor: '#409EFF',
              backgroundColor: 'rgba(64, 158, 255, 0.1)',
              fill: true,
              tension: 0.3,
            },
            {
              label: 'メモリ (%)',
              data: history.memoryUsage,
              borderColor: '#67C23A',
              backgroundColor: 'rgba(103, 194, 58, 0.1)',
              fill: true,
              tension: 0.3,
            },
            {
              label: 'GPU (%)',
              data: history.gpuUsage,
              borderColor: '#E6A23C',
              backgroundColor: 'rgba(230, 162, 60, 0.1)',
              fill: true,
              tension: 0.3,
            },
          ],
        },
        options: {
          responsive: true,
          maintainAspectRatio: false,
          scales: {
            yAxes: [
              {
                ticks: {
                  beginAtZero: true,
                  max: 100,
                  callback: function(value) {
                    return value + '%'
                  },
                },
              },
            ],
          },
          tooltips: {
            callbacks: {
              label: function(tooltipItem, data) {
                let label = data.datasets[tooltipItem.datasetIndex].label || ''
                return (
                  label + ': ' + Number(tooltipItem.yLabel).toFixed(1) + '%'
                )
              },
            },
          },
        },
      })
    },
    renderTenantChart() {
      let tenantUsage = this.dashboardTenantUsage
      if (!tenantUsage || tenantUsage.length === 0) return

      if (this.tenantChart) {
        this.tenantChart.destroy()
      }

      let ctx = this.$refs.tenantChart
      if (!ctx) return

      let labels = tenantUsage.map(t => t.tenantDisplayName || t.tenantName)

      this.tenantChart = new Chart(ctx, {
        type: 'bar',
        data: {
          labels: labels,
          datasets: [
            {
              label: 'CPU',
              data: tenantUsage.map(t => t.cpuUsed),
              backgroundColor: 'rgba(64, 158, 255, 0.7)',
            },
            {
              label: 'メモリ (GB)',
              data: tenantUsage.map(t => t.memoryUsed),
              backgroundColor: 'rgba(103, 194, 58, 0.7)',
            },
            {
              label: 'GPU',
              data: tenantUsage.map(t => t.gpuUsed),
              backgroundColor: 'rgba(230, 162, 60, 0.7)',
            },
          ],
        },
        options: {
          responsive: true,
          maintainAspectRatio: false,
          scales: {
            yAxes: [
              {
                ticks: {
                  beginAtZero: true,
                },
              },
            ],
          },
        },
      })
    },
    loadThresholds() {
      let saved = localStorage.getItem('kamonohashi-alert-thresholds')
      if (saved) {
        try {
          this.alertThresholds = JSON.parse(saved)
        } catch (e) {
          // ignore parse error
        }
      }
    },
    saveThresholds() {
      localStorage.setItem(
        'kamonohashi-alert-thresholds',
        JSON.stringify(this.alertThresholds),
      )
    },
  },
}
</script>

<style lang="scss" scoped>
.dashboard-container {
  padding: 0 10px;
}

.summary-row {
  margin-bottom: 20px;
}

.summary-card {
  text-align: center;
  .summary-title {
    font-size: 14px;
    color: #909399;
    margin-bottom: 8px;
  }
  .summary-value {
    font-size: 32px;
    font-weight: bold;
    margin-bottom: 4px;
    &.rate-normal {
      color: #67c23a;
    }
    &.rate-warning {
      color: #e6a23c;
    }
    &.rate-danger {
      color: #f56c6c;
    }
    &.cluster-info {
      color: #409eff;
    }
  }
  .summary-detail {
    font-size: 12px;
    color: #909399;
    margin-bottom: 10px;
  }
}

.alert-row {
  margin-bottom: 20px;
}

.alert-item {
  margin-bottom: 8px;
}

.chart-row {
  margin-bottom: 20px;
}

.chart-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.chart-container {
  height: 350px;
  position: relative;
}

.node-table {
  width: 100%;
}
</style>
