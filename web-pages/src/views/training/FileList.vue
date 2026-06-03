<template>
  <div>
    <el-dialog
      class="dialog"
      :title="$t('messages.file_list')"
      :visible.sync="dialogVisible"
      :before-close="emitCancel"
      :close-on-click-modal="false"
    >
      <el-row>
        <el-col :span="6">
          <kqi-display-text-form
            :label="$t('labels.training_name')"
            :value="detail.name"
          />
        </el-col>
        <el-col :span="6">
          <kqi-display-text-form
            :label="$t('labels.started_at')"
            :value="detail.createdAt"
          />
        </el-col>
        <el-col :span="6">
          <kqi-display-text-form
            :label="$t('labels.completed_at')"
            :value="detail.completedAt"
          />
        </el-col>
        <el-col :span="6">
          <kqi-display-text-form
            :label="$t('labels.status')"
            :value="detail.statusType"
          />
        </el-col>
      </el-row>

      <kqi-file-viewer :file-list="fileList" @updatePath="updatePath" />
      <el-row :gutter="20" class="footer">
        <el-col class="right-button-group" :span="24">
          <el-button @click="emitCancel">{{ $t('common.cancel') }}</el-button>
          <el-button @click="emitReturn">{{ $t('common.back') }}</el-button>
        </el-col>
      </el-row>
    </el-dialog>
  </div>
</template>

<script>
import KqiDisplayTextForm from '@/components/KqiDisplayTextForm'
import KqiFileViewer from '@/components/KqiFileViewer'
import { createNamespacedHelpers } from 'vuex'
const { mapGetters, mapActions } = createNamespacedHelpers('training')

export default {
  components: {
    KqiDisplayTextForm,
    KqiFileViewer,
  },
  props: {
    id: {
      type: String,
      default: null,
    },
  },

  data() {
    return {
      dialogVisible: true,
      error: undefined,
      job: {},
      path: '/',
    }
  },
  computed: {
    ...mapGetters(['fileList', 'detail']),
  },
  async created() {
    await this.retrieveData()
  },
  methods: {
    ...mapActions(['fetchDetail', 'fetchFileList']),
    async retrieveData() {
      await this.fetchDetail(this.id)
      let params = {
        id: this.id,
        path: this.path,
        withUrl: true,
      }
      await this.fetchFileList(params)
    },
    async updatePath(path) {
      this.path = path
      await this.retrieveData()
    },

    emitCancel() {
      this.$emit('cancel')
    },
    emitReturn() {
      this.$emit('return')
    },
  },
}
</script>

<style lang="scss" scoped>
.dialog /deep/ .el-dialog {
  min-width: 800px;
}

.dialog /deep/ label {
  font-weight: bold !important;
}

.dialog /deep/ .el-dialog__title {
  font-size: 24px;
}

.right-button-group {
  text-align: right;
}

.footer {
  padding-top: 40px;
}
</style>
