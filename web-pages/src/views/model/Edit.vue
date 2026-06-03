<template>
  <kqi-dialog
    :title="title"
    type="EDIT"
    @submit="onSubmit"
    @delete="deleteModel"
    @close="$emit('cancel')"
  >
    <el-form ref="editForm" :model="form" :rules="rules">
      <kqi-display-error :error="error" />
      <el-row :gutter="20">
        <el-col :span="12">
          <kqi-display-text-form label="ID" :value="String(id)" />
          <el-form-item label="モデル名" prop="name">
            <el-input v-model="form.name" />
          </el-form-item>
          <el-form-item label="説明">
            <el-input v-model="form.description" type="textarea" :rows="3" />
          </el-form-item>
        </el-col>
      </el-row>
    </el-form>

    <el-divider />

    <el-row type="flex" justify="space-between">
      <h3>バージョン履歴</h3>
      <el-button type="primary" size="small" @click="showVersionDialog = true">
        バージョン追加
      </el-button>
    </el-row>
    <el-table :data="versions" border>
      <el-table-column prop="version" label="Version" width="100px">
        <template slot-scope="scope">
          v{{ scope.row.version }}
        </template>
      </el-table-column>
      <el-table-column
        prop="trainingHistoryName"
        label="学習履歴"
        width="150px"
      >
        <template slot-scope="scope">
          <span v-if="scope.row.trainingHistoryId">
            {{ scope.row.trainingHistoryId }}:{{
              scope.row.trainingHistoryName
            }}
          </span>
          <span v-else>-</span>
        </template>
      </el-table-column>
      <el-table-column prop="accuracy" label="精度" width="100px">
        <template slot-scope="scope">
          {{ scope.row.accuracy != null ? scope.row.accuracy : '-' }}
        </template>
      </el-table-column>
      <el-table-column prop="status" label="ステータス" width="130px">
        <template slot-scope="scope">
          <el-tag
            :type="statusTagType(scope.row.status)"
            size="small"
          >
            {{ scope.row.status }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="description" label="メモ" width="auto" />
      <el-table-column prop="createdAt" label="作成日時" width="180px" />
      <el-table-column label="操作" width="200px">
        <template slot-scope="scope">
          <el-button
            size="mini"
            @click="promoteVersion(scope.row, 'staging')"
          >
            Staging
          </el-button>
          <el-button
            size="mini"
            type="success"
            @click="promoteVersion(scope.row, 'production')"
          >
            Production
          </el-button>
        </template>
      </el-table-column>
    </el-table>

    <el-dialog
      title="バージョン追加"
      :visible.sync="showVersionDialog"
      width="500px"
      append-to-body
    >
      <el-form ref="versionForm" :model="versionForm" :rules="versionRules">
        <el-form-item label="学習履歴ID">
          <el-input
            v-model.number="versionForm.trainingHistoryId"
            placeholder="学習履歴IDを入力（任意）"
          />
        </el-form-item>
        <el-form-item label="精度">
          <el-input
            v-model.number="versionForm.accuracy"
            placeholder="精度（任意）"
          />
        </el-form-item>
        <el-form-item label="ステータス">
          <el-select v-model="versionForm.status" placeholder="選択してください">
            <el-option label="none" value="none" />
            <el-option label="staging" value="staging" />
            <el-option label="production" value="production" />
          </el-select>
        </el-form-item>
        <el-form-item label="メモ">
          <el-input
            v-model="versionForm.description"
            type="textarea"
            :rows="2"
          />
        </el-form-item>
      </el-form>
      <span slot="footer">
        <el-button @click="showVersionDialog = false">キャンセル</el-button>
        <el-button type="primary" @click="addVersion">追加</el-button>
      </span>
    </el-dialog>
  </kqi-dialog>
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
      form: {
        name: '',
        description: '',
      },
      versions: [],
      error: null,
      rules: {
        name: [{ required: true, message: 'モデル名は必須です', trigger: 'blur' }],
      },
      showVersionDialog: false,
      versionForm: {
        trainingHistoryId: null,
        accuracy: null,
        status: 'none',
        description: '',
      },
      versionRules: {},
    }
  },
  computed: {
    title() {
      return 'モデル編集 ' + this.id
    },
  },
  async created() {
    await this.retrieveData()
  },
  methods: {
    async retrieveData() {
      try {
        let response = await api.model.getById({ id: this.id })
        let model = response.data
        this.form.name = model.name
        this.form.description = model.description
        this.versions = model.versions || []
      } catch (e) {
        this.error = e
      }
    },
    async onSubmit() {
      let valid = await this.$refs.editForm.validate().catch(() => false)
      if (!valid) return

      try {
        await api.model.putById({
          id: this.id,
          body: {
            name: this.form.name,
            description: this.form.description,
          },
        })
        this.$emit('done')
      } catch (e) {
        this.error = e
      }
    },
    async deleteModel() {
      try {
        await api.model.deleteById({ id: this.id })
        this.$emit('done')
      } catch (e) {
        this.error = e
      }
    },
    async addVersion() {
      try {
        let body = {
          status: this.versionForm.status || 'none',
          description: this.versionForm.description,
        }
        if (this.versionForm.trainingHistoryId) {
          body.trainingHistoryId = this.versionForm.trainingHistoryId
        }
        if (this.versionForm.accuracy != null && this.versionForm.accuracy !== '') {
          body.accuracy = this.versionForm.accuracy
        }
        await api.model.postVersion({ id: this.id, body })
        this.showVersionDialog = false
        this.versionForm = {
          trainingHistoryId: null,
          accuracy: null,
          status: 'none',
          description: '',
        }
        await this.retrieveData()
      } catch (e) {
        this.error = e
      }
    },
    async promoteVersion(version, status) {
      try {
        await api.model.putVersion({
          id: this.id,
          versionId: version.id,
          body: { status },
        })
        await this.retrieveData()
      } catch (e) {
        this.error = e
      }
    },
    statusTagType(status) {
      switch (status) {
        case 'production':
          return 'success'
        case 'staging':
          return 'warning'
        default:
          return 'info'
      }
    },
  },
}
</script>
