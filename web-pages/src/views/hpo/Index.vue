<template>
  <div>
    <h2>HPO（ハイパーパラメータ最適化）管理</h2>
    <el-row type="flex" justify="space-between" :gutter="20">
      <kqi-pagination
        v-model="pageStatus"
        :total="total"
        @change="retrieveData"
      />
      <el-col class="right-top-button" :span="8">
        <el-button
          icon="el-icon-edit-outline"
          type="primary"
          plain
          @click="openCreateDialog()"
        >
          新規HPOジョブ
        </el-button>
      </el-col>
    </el-row>

    <el-table
      class="data-table pl-index-table"
      :data="tableData"
      border
      @row-click="openDetailDialog"
    >
      <el-table-column prop="id" label="ID" width="80px" />
      <el-table-column prop="name" label="ジョブ名" width="180px" />
      <el-table-column prop="algorithm" label="アルゴリズム" width="120px">
        <template slot-scope="scope">
          <el-tag :type="getAlgorithmTagType(scope.row.algorithm)" size="small">
            {{ scope.row.algorithm }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="status" label="ステータス" width="120px">
        <template slot-scope="scope">
          <el-tag :type="getStatusTagType(scope.row.status)" size="small">
            {{ scope.row.status }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column label="進捗" width="150px">
        <template slot-scope="scope">
          <el-progress
            :percentage="getProgressPercentage(scope.row)"
            :status="scope.row.status === 'Completed' ? 'success' : ''"
          />
        </template>
      </el-table-column>
      <el-table-column prop="objectiveMetric" label="目的メトリクス" width="140px" />
      <el-table-column prop="bestMetricValue" label="ベスト値" width="120px">
        <template slot-scope="scope">
          {{ scope.row.bestMetricValue != null ? scope.row.bestMetricValue.toFixed(6) : '-' }}
        </template>
      </el-table-column>
      <el-table-column prop="dataSetName" label="データセット" width="150px" />
      <el-table-column prop="createdAt" label="作成日時" width="170px" />
      <el-table-column label="操作" width="100px">
        <template slot-scope="scope">
          <el-button
            v-if="scope.row.status === 'Running'"
            type="danger"
            size="mini"
            @click.stop="handleHalt(scope.row)"
          >
            停止
          </el-button>
          <el-button
            v-else-if="scope.row.status !== 'Running'"
            type="danger"
            size="mini"
            icon="el-icon-delete"
            @click.stop="handleDelete(scope.row)"
          />
        </template>
      </el-table-column>
    </el-table>

    <router-view @done="retrieveData" />
  </div>
</template>

<script>
import KqiPagination from '@/components/KqiPagination'
import api from '@/api/api'

export default {
  title: 'HPO管理',
  components: {
    KqiPagination,
  },
  data() {
    return {
      pageStatus: { currentPage: 1, currentPageSize: 30 },
      total: 0,
      tableData: [],
    }
  },
  async created() {
    await this.retrieveData()
  },
  methods: {
    async retrieveData() {
      let params = {
        page: this.pageStatus.currentPage,
        perPage: this.pageStatus.currentPageSize,
        withTotal: true,
      }
      let response = await api.hpo.get(params)
      this.tableData = response.data
      this.total = parseInt(response.headers['x-total-count'])
    },
    openCreateDialog() {
      this.$router.push('/hpo/run')
    },
    openDetailDialog(row) {
      this.$router.push('/hpo/' + row.id)
    },
    async handleHalt(row) {
      try {
        await this.$confirm('HPOジョブを停止しますか？', '確認', {
          type: 'warning',
        })
        await api.hpo.postHaltById({ id: row.id })
        this.$notify.success('HPOジョブを停止しました')
        await this.retrieveData()
      } catch (e) {
        // キャンセル
      }
    },
    async handleDelete(row) {
      try {
        await this.$confirm('HPOジョブを削除しますか？', '確認', {
          type: 'warning',
        })
        await api.hpo.deleteById({ id: row.id })
        this.$notify.success('HPOジョブを削除しました')
        await this.retrieveData()
      } catch (e) {
        // キャンセル
      }
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
    getProgressPercentage(row) {
      if (row.maxTrials === 0) return 0
      return Math.round((row.completedTrials / row.maxTrials) * 100)
    },
  },
}
</script>

<style scoped>
.right-top-button {
  text-align: right;
}
</style>
