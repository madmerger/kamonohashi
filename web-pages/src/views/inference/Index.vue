<template>
  <div>
    <h2>{{ $t('titles.inference_management') }}</h2>
    <el-row type="flex" justify="space-between" :gutter="20">
      <kqi-pagination
        v-model="pageStatus"
        :total="total"
        @change="retrieveData"
      />
      <el-col class="right-top-button" :span="8">
        <el-button v-if="selections.length !== 0" @click="showDeleteConfirm">
          {{ $t('common.batch_delete') }}
        </el-button>
        <el-button
          icon="el-icon-edit-outline"
          type="primary"
          plain
          @click="openCreateDialog()"
        >
          {{ $t('common.new_execution') }}
        </el-button>
      </el-col>
    </el-row>
    <el-row :gutter="20">
      <el-col class="search">
        <kqi-smart-search-input
          v-model="searchCondition"
          :configs="searchConfigs"
          @search="search"
        />
      </el-col>
    </el-row>

    <el-row>
      <el-table
        class="data-table pl-index-table"
        :data="histories"
        border
        @row-click="openEditDialog"
        @selection-change="handleSelectionChange"
      >
        <el-table-column type="selection" width="55px" />
        <el-table-column width="25px">
          <div slot-scope="scope">
            <i v-if="scope.row.favorite" class="el-icon-star-on favorite" />
          </div>
        </el-table-column>
        <el-table-column prop="id" label="ID" width="120px" />
        <el-table-column
          prop="name"
          :label="$t('labels.inference_name')"
          width="150px"
        />
        <el-table-column
          prop="createdAt"
          :label="$t('labels.started_at')"
          width="100px"
        />
        <el-table-column :label="$t('labels.mounted_training')" width="200px">
          <template slot-scope="scope">
            <span
              v-for="(ParentName, index) in scope.row.parentFullNameList"
              :key="index"
            >
              <span class="parent">
                {{ ParentName }}
              </span>
            </span>
          </template>
        </el-table-column>
        <el-table-column :label="$t('labels.mounted_inference')" width="200px">
          <template slot-scope="scope">
            <span
              v-for="(ParentInferenceName, index) in scope.row
                .parentInferenceFullNameList"
              :key="index"
            >
              <span class="parent">
                {{ ParentInferenceName }}
              </span>
            </span>
          </template>
        </el-table-column>
        <el-table-column
          prop="dataSet.name"
          :label="$t('labels.dataset')"
          width="120px"
        />
        <el-table-column
          prop="entryPoint"
          :label="$t('labels.entry_point')"
          width="auto"
          class-name="entry-point-column"
        />
        <el-table-column
          prop="memo"
          :label="$t('labels.memo')"
          width="auto"
          class-name="memo-column"
        />
        <el-table-column
          prop="outputValue"
          :label="$t('labels.output_value')"
          width="200px"
          sortable
        />
        <el-table-column width="30px">
          <div slot-scope="scope">
            <div
              v-if="
                (scope.row.status === 'Running') |
                  (scope.row.status === 'Completed')
              "
            >
              <i class="el-icon-success" style="color: #67C23A;" />
            </div>
            <div v-else>
              <i class="el-icon-warning" style="color: #E6A23C;" />
            </div>
          </div>
        </el-table-column>
        <el-table-column
          prop="status"
          :label="$t('labels.status')"
          width="120px"
        />
      </el-table>
    </el-row>

    <el-row>
      <kqi-pagination
        v-model="pageStatus"
        :total="total"
        @change="retrieveData"
      />
    </el-row>

    <router-view
      @cancel="closeDialog"
      @cancelShell="closeDialog"
      @done="done"
      @return="back"
      @files="files"
      @shell="shell"
      @log="log"
      @copyCreate="copyCreate"
    />
  </div>
</template>

<script>
import KqiPagination from '@/components/KqiPagination'
import KqiSmartSearchInput from '@/components/KqiSmartSearchInput/Index'
import { createNamespacedHelpers } from 'vuex'
const { mapGetters, mapActions } = createNamespacedHelpers('inference')

export default {
  title() {
    return this.$t('titles.inference_management')
  },
  components: {
    KqiPagination,
    KqiSmartSearchInput,
  },
  data() {
    return {
      pageStatus: {
        currentPage: 1,
        currentPageSize: 10,
      },
      selections: [],
      searchCondition: {},
      searchConfigs: [
        { prop: 'id', name: 'ID', type: 'number' },
        { prop: 'name', name: this.$t('labels.inference_name'), type: 'text' },
        { prop: 'startedAt', name: this.$t('labels.started_at'), type: 'date' },
        { prop: 'startedBy', name: this.$t('labels.started_by'), type: 'text' },
        {
          prop: 'parentId',
          name: this.$t('labels.mounted_training_id'),
          type: 'number',
        },
        {
          prop: 'parentInferenceId',
          name: this.$t('labels.mounted_inference_id'),
          type: 'number',
        },
        {
          prop: 'parentName',
          name: this.$t('labels.mounted_training_name'),
          type: 'text',
        },
        {
          prop: 'parentInferenceName',
          name: this.$t('labels.mounted_inference_name'),
          type: 'text',
        },
        { prop: 'dataSet', name: this.$t('labels.dataset'), type: 'text' },
        {
          prop: 'entryPoint',
          name: this.$t('labels.entry_point'),
          type: 'text',
        },
        { prop: 'memo', name: this.$t('labels.memo'), type: 'text' },
        {
          prop: 'status',
          name: this.$t('labels.status'),
          type: 'select',
          option: {
            items: [
              'None',
              'Pending',
              'Succeeded',
              'Running',
              'Completed',
              'UserCanceled',
              'Failed',
              'Killed',
              'Invalid',
              'Forbidden',
              'Multiple',
              'Empty',
              'Error',
            ],
          },
        },
      ],
    }
  },
  computed: {
    ...mapGetters(['histories', 'total']),
  },

  async created() {
    await this.retrieveData()
  },
  methods: {
    ...mapActions(['fetchHistories', 'delete']),

    async retrieveData() {
      let params = this.searchCondition
      params.page = this.pageStatus.currentPage
      params.perPage = this.pageStatus.currentPageSize
      params.withTotal = true
      await this.fetchHistories(params)
    },
    async search() {
      this.pageStatus.currentPage = 1
      await this.retrieveData()
    },

    handleSelectionChange(val) {
      // checkboxの要素変更
      this.selections = val
    },

    async showDeleteConfirm() {
      let confirmMessage = this.$t('messages.inference_delete_confirm', {
        count: this.selections.length,
      })
      await this.$confirm(confirmMessage, 'Warning', {
        distinguishCancelAndClose: true,
        confirmButtonText: this.$t('common.yes'),
        cancelButtonText: this.$t('common.cancel'),
        type: 'warning',
      })
        .then(async () => {
          let successCount = 0
          for (let selection of this.selections) {
            try {
              await this.delete(selection.id)
              successCount++
              this.error = null
            } catch (e) {
              this.error = e
            }
          }
          await this.$notify.info({
            type: 'info',
            message: this.$t('messages.inference_deleted', {
              success: successCount,
              fail: this.selections.length - successCount,
            }),
          })
          this.pageStatus.currentPage = 1
          await this.retrieveData()
        })
        .catch(() => {
          // キャンセル時はなにもしないので例外を無視
        })
    },
    async done(type) {
      if (type === 'delete') {
        // 削除時、表示していたページにデータが無くなっている可能性がある。
        // 総数 % ページサイズ === 1の時、残り1の状態で削除したため、currentPageが1で無ければ1つ前のページに戻す
        if (this.total % this.pageStatus.currentPageSize === 1) {
          if (this.pageStatus.currentPage !== 1) {
            this.pageStatus.currentPage -= 1
          }
        }
      }
      this.closeDialog()
      await this.retrieveData()
      this.showSuccessMessage()
    },
    closeDialog() {
      this.$router.push('/inference')
    },
    back() {
      this.$router.go(-1)
    },
    openEditDialog(selectedRow) {
      this.$router.push('/inference/' + selectedRow.id)
    },
    openCreateDialog() {
      this.$router.push('/inference/create')
    },
    copyCreate(originId) {
      this.$router.push('/inference/create/' + originId)
    },
    files(id) {
      this.$router.push('/inference/' + id + '/files')
    },
    shell(id) {
      this.$router.push('/inference/' + id + '/shell')
    },
    log(id) {
      this.$router.push('/inference/' + id + '/log')
    },
  },
}
</script>

<style scoped>
.right-top-button {
  text-align: right;
}

.search {
  text-align: right;
  padding-top: 10px;
}

.el-table /deep/ .memo-column div.cell {
  white-space: pre-wrap;
}

.pagination /deep/ .el-input {
  text-align: left;
  width: 120px;
}

.favorite {
  color: rgb(230, 162, 60);
}

.parent {
  margin-right: 5px;
}
</style>

<style lang="scss">
.entry-point-column {
  display: table-cell;
}

.entry-point-column div.cell {
  /*! autoprefixer: off */
  overflow: hidden;
  -webkit-line-clamp: 3;
  -webkit-box-orient: vertical;
  display: -webkit-box;
}
</style>
