<template>
  <el-dialog
    class="preview-dialog"
    title="データセットプレビュー"
    :visible.sync="dialogVisible"
    :before-close="handleClose"
    :close-on-click-modal="false"
  >
    <el-tabs v-model="activeTab" @tab-click="handleTabClick">
      <!-- プレビュータブ -->
      <el-tab-pane label="プレビュー" name="preview">
        <div v-if="loadingPreview" class="loading-container">
          <i class="el-icon-loading" />
          <span>画像を読み込み中...</span>
        </div>
        <div v-else-if="imageFiles.length === 0" class="empty-message">
          プレビュー可能な画像ファイルがありません。
        </div>
        <div v-else class="thumbnail-grid">
          <div
            v-for="(file, index) in imageFiles"
            :key="index"
            class="thumbnail-item"
          >
            <el-image
              :src="file.url"
              fit="cover"
              class="thumbnail-image"
              :preview-src-list="imageUrls"
            />
            <div class="thumbnail-label">{{ file.fileName }}</div>
          </div>
        </div>
        <div v-if="imageFiles.length > 0" class="preview-footer">
          {{ imageFiles.length }} / {{ totalPreviewFiles }} 件の画像を表示中
        </div>
      </el-tab-pane>

      <!-- 統計タブ -->
      <el-tab-pane label="統計情報" name="statistics">
        <div v-if="loadingStats" class="loading-container">
          <i class="el-icon-loading" />
          <span>統計情報を読み込み中...</span>
        </div>
        <div v-else-if="statistics" class="statistics-container">
          <!-- 基本統計 -->
          <el-row :gutter="20" class="stats-summary">
            <el-col :span="8">
              <el-card shadow="hover" class="stats-card">
                <div class="stats-number">{{ statistics.totalDataCount }}</div>
                <div class="stats-label">データエントリ数</div>
              </el-card>
            </el-col>
            <el-col :span="8">
              <el-card shadow="hover" class="stats-card">
                <div class="stats-number">{{ statistics.totalFileCount }}</div>
                <div class="stats-label">ファイル総数</div>
              </el-card>
            </el-col>
            <el-col :span="8">
              <el-card shadow="hover" class="stats-card">
                <div class="stats-number">
                  {{ formatFileSize(statistics.totalFileSize) }}
                </div>
                <div class="stats-label">合計サイズ</div>
              </el-card>
            </el-col>
          </el-row>

          <!-- ファイルタイプ分布 -->
          <el-card
            v-if="fileTypeData.length > 0"
            shadow="hover"
            class="distribution-card"
          >
            <div slot="header">ファイルタイプ分布</div>
            <el-table :data="fileTypeData" border size="small">
              <el-table-column prop="type" label="ファイルタイプ" />
              <el-table-column prop="count" label="ファイル数" width="120px" />
              <el-table-column label="割合" width="200px">
                <template slot-scope="scope">
                  <el-progress
                    :percentage="scope.row.percentage"
                    :format="() => scope.row.percentage + '%'"
                  />
                </template>
              </el-table-column>
            </el-table>
          </el-card>
        </div>
      </el-tab-pane>

      <!-- クラス分布タブ -->
      <el-tab-pane label="クラス分布" name="distribution">
        <div v-if="loadingStats" class="loading-container">
          <i class="el-icon-loading" />
          <span>クラス分布を読み込み中...</span>
        </div>
        <div
          v-else-if="classDistributionData.length > 0"
          class="chart-container"
        >
          <canvas ref="classChart" />
          <el-table
            :data="classDistributionData"
            border
            size="small"
            class="distribution-table"
          >
            <el-table-column prop="name" label="クラス名" />
            <el-table-column prop="count" label="データ数" width="120px" />
            <el-table-column label="割合" width="200px">
              <template slot-scope="scope">
                <el-progress
                  :percentage="scope.row.percentage"
                  :color="scope.row.color"
                  :format="() => scope.row.percentage + '%'"
                />
              </template>
            </el-table-column>
          </el-table>
        </div>
        <div v-else class="empty-message">
          クラス分布情報がありません。（フラット配置のデータセットの場合、クラス分布は表示されません）
        </div>
      </el-tab-pane>
    </el-tabs>
  </el-dialog>
</template>

<script>
import Chart from 'chart.js'
import { createNamespacedHelpers } from 'vuex'
const { mapGetters, mapMutations, mapActions } = createNamespacedHelpers(
  'dataSet',
)

const CHART_COLORS = [
  '#409EFF',
  '#67C23A',
  '#E6A23C',
  '#F56C6C',
  '#909399',
  '#00BCD4',
  '#9C27B0',
  '#FF9800',
  '#795548',
  '#607D8B',
]

const IMAGE_EXTENSIONS = [
  '.jpg',
  '.jpeg',
  '.png',
  '.gif',
  '.bmp',
  '.webp',
  '.svg',
  '.tiff',
  '.tif',
]

export default {
  components: {},
  props: {
    id: {
      type: String,
      default: null,
    },
  },
  data() {
    return {
      dialogVisible: true,
      activeTab: 'preview',
      loadingStats: false,
      loadingPreview: false,
      chartInstance: null,
      previewDataList: [],
      totalPreviewFiles: 0,
    }
  },
  computed: {
    ...mapGetters(['detail', 'statistics', 'previewFiles']),
    imageFiles() {
      let images = []
      for (let file of this.previewDataList) {
        if (file.url && this.isImageFile(file.fileName)) {
          images.push(file)
        }
      }
      return images
    },
    imageUrls() {
      return this.imageFiles.map(f => f.url)
    },
    fileTypeData() {
      if (!this.statistics || !this.statistics.fileTypeDistribution) return []
      let dist = this.statistics.fileTypeDistribution
      let total = Object.values(dist).reduce((a, b) => a + b, 0)
      return Object.entries(dist)
        .map(([type, count]) => ({
          type,
          count,
          percentage: total > 0 ? Math.round((count / total) * 100) : 0,
        }))
        .sort((a, b) => b.count - a.count)
    },
    classDistributionData() {
      if (!this.statistics || !this.statistics.classDistribution) return []
      let dist = this.statistics.classDistribution
      let total = Object.values(dist).reduce((a, b) => a + b, 0)
      return Object.entries(dist)
        .map(([name, count], index) => ({
          name,
          count,
          percentage: total > 0 ? Math.round((count / total) * 100) : 0,
          color: CHART_COLORS[index % CHART_COLORS.length],
        }))
        .sort((a, b) => b.count - a.count)
    },
  },
  async created() {
    await this.initialize()
  },
  beforeDestroy() {
    if (this.chartInstance) {
      this.chartInstance.destroy()
    }
  },
  methods: {
    ...mapActions(['fetchDetail', 'fetchStatistics', 'fetchPreviewFiles']),
    ...mapMutations(['clearPreviewFiles']),

    handleClose() {
      this.$emit('cancel')
    },

    async initialize() {
      if (!this.id) return

      // データセット詳細と統計を取得
      this.loadingStats = true
      this.loadingPreview = true
      try {
        await this.fetchDetail(this.id)
        await this.fetchStatistics(this.id)
      } catch (e) {
        // エラーはそのまま無視（画面にはデータなしで表示）
      }
      this.loadingStats = false

      // プレビュー用の画像URLを取得
      await this.loadPreviewImages()
      this.loadingPreview = false
    },

    async loadPreviewImages() {
      this.clearPreviewFiles()
      this.previewDataList = []

      if (!this.detail) return

      if (this.detail.isFlat) {
        // フラット配置の場合
        if (this.detail.flatEntries) {
          let entries = this.detail.flatEntries.slice(0, 20)
          await this.loadFilesForEntries(entries)
        }
        return
      }

      // 各データ種別からデータを集める（最大20件まで）
      let allEntries = []
      for (let key in this.detail.entries) {
        for (let entry of this.detail.entries[key]) {
          allEntries.push(entry)
          if (allEntries.length >= 20) break
        }
        if (allEntries.length >= 20) break
      }
      this.totalPreviewFiles = allEntries.length
      await this.loadFilesForEntries(allEntries)
    },

    async loadFilesForEntries(entries) {
      let allFiles = []
      for (let entry of entries) {
        try {
          await this.fetchPreviewFiles({ dataId: entry.id })
          for (let file of this.previewFiles) {
            allFiles.push(file)
          }
        } catch (e) {
          // 個々のデータのファイル取得に失敗してもスキップ
        }
      }
      this.previewDataList = allFiles
      this.totalPreviewFiles = allFiles.filter(f =>
        this.isImageFile(f.fileName),
      ).length
    },

    isImageFile(fileName) {
      if (!fileName) return false
      let ext =
        fileName.lastIndexOf('.') >= 0
          ? fileName.substring(fileName.lastIndexOf('.')).toLowerCase()
          : ''
      return IMAGE_EXTENSIONS.includes(ext)
    },

    formatFileSize(bytes) {
      if (bytes === 0) return '0 B'
      let units = ['B', 'KB', 'MB', 'GB', 'TB']
      let i = Math.floor(Math.log(bytes) / Math.log(1024))
      return (bytes / Math.pow(1024, i)).toFixed(1) + ' ' + units[i]
    },

    handleTabClick(tab) {
      if (tab.name === 'distribution' && !this.chartInstance) {
        this.$nextTick(() => {
          this.renderChart()
        })
      }
    },

    renderChart() {
      if (!this.$refs.classChart || this.classDistributionData.length === 0) {
        return
      }
      if (this.chartInstance) {
        this.chartInstance.destroy()
      }
      let ctx = this.$refs.classChart.getContext('2d')
      this.chartInstance = new Chart(ctx, {
        type: 'bar',
        data: {
          labels: this.classDistributionData.map(d => d.name),
          datasets: [
            {
              label: 'データ数',
              data: this.classDistributionData.map(d => d.count),
              backgroundColor: this.classDistributionData.map(d => d.color),
              borderWidth: 1,
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
                  stepSize: 1,
                },
                scaleLabel: {
                  display: true,
                  labelString: 'データ数',
                },
              },
            ],
            xAxes: [
              {
                scaleLabel: {
                  display: true,
                  labelString: 'クラス名',
                },
              },
            ],
          },
          legend: {
            display: false,
          },
          title: {
            display: true,
            text: 'クラス分布',
            fontSize: 16,
          },
        },
      })
    },
  },
}
</script>

<style lang="scss" scoped>
.preview-dialog /deep/ .el-dialog {
  width: 80%;
  max-width: 1200px;
}

.preview-dialog /deep/ .el-dialog__title {
  font-size: 24px;
}

.loading-container {
  text-align: center;
  padding: 40px;
  color: #909399;
  i {
    font-size: 24px;
    margin-right: 8px;
  }
}

.empty-message {
  text-align: center;
  padding: 40px;
  color: #909399;
}

.thumbnail-grid {
  display: flex;
  flex-wrap: wrap;
  gap: 12px;
  padding: 10px;
}

.thumbnail-item {
  width: 150px;
  text-align: center;

  .thumbnail-image {
    width: 150px;
    height: 150px;
    border: 1px solid #ebeef5;
    border-radius: 4px;
    overflow: hidden;
  }

  .thumbnail-label {
    font-size: 12px;
    color: #606266;
    margin-top: 4px;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
  }
}

.preview-footer {
  text-align: center;
  padding: 10px;
  color: #909399;
  font-size: 14px;
}

.statistics-container {
  padding: 10px;
}

.stats-summary {
  margin-bottom: 20px;
}

.stats-card {
  text-align: center;
  .stats-number {
    font-size: 28px;
    font-weight: bold;
    color: #409eff;
  }
  .stats-label {
    font-size: 14px;
    color: #909399;
    margin-top: 4px;
  }
}

.distribution-card {
  margin-bottom: 20px;
}

.chart-container {
  padding: 10px;
  canvas {
    max-height: 300px;
    margin-bottom: 20px;
  }
}

.distribution-table {
  margin-top: 20px;
}
</style>
