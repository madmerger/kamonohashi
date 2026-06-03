<template>
  <div>
    <h2>カスタムロール管理</h2>
    <el-row type="flex" justify="end" class="margin">
      <el-button type="primary" icon="el-icon-plus" @click="openCreateDialog">
        新規カスタムロール
      </el-button>
    </el-row>
    <el-table
      class="data-table pl-index-table"
      :data="roles"
      border
      @row-click="openEditDialog"
    >
      <el-table-column prop="name" label="ロール名" width="200px" />
      <el-table-column prop="displayName" label="表示名" width="200px" />
      <el-table-column prop="description" label="説明" />
      <el-table-column label="権限" width="auto">
        <template slot-scope="scope">
          <el-tag v-if="scope.row.canManageData" size="mini" class="perm-tag">
            データ管理
          </el-tag>
          <el-tag
            v-if="scope.row.canManageDataSet"
            size="mini"
            class="perm-tag"
          >
            データセット
          </el-tag>
          <el-tag
            v-if="scope.row.canRunTraining"
            size="mini"
            class="perm-tag"
          >
            学習実行
          </el-tag>
          <el-tag
            v-if="scope.row.canRunInference"
            size="mini"
            class="perm-tag"
          >
            推論実行
          </el-tag>
          <el-tag
            v-if="scope.row.canUseNotebook"
            size="mini"
            class="perm-tag"
          >
            ノートブック
          </el-tag>
          <el-tag
            v-if="scope.row.canManageProject"
            size="mini"
            class="perm-tag"
          >
            プロジェクト管理
          </el-tag>
        </template>
      </el-table-column>
    </el-table>

    <router-view @done="fetchRoles" @cancel="closeDialog" />

    <!-- 作成ダイアログ -->
    <el-dialog
      title="カスタムロール作成"
      :visible.sync="createDialogVisible"
      width="600px"
    >
      <el-form :model="createForm" label-width="180px">
        <el-form-item label="ロール名" required>
          <el-input v-model="createForm.name" />
        </el-form-item>
        <el-form-item label="表示名" required>
          <el-input v-model="createForm.displayName" />
        </el-form-item>
        <el-form-item label="説明">
          <el-input v-model="createForm.description" type="textarea" />
        </el-form-item>
        <el-divider content-position="left">権限設定</el-divider>
        <el-form-item label="データ管理">
          <el-switch v-model="createForm.canManageData" />
        </el-form-item>
        <el-form-item label="データセット管理">
          <el-switch v-model="createForm.canManageDataSet" />
        </el-form-item>
        <el-form-item label="学習実行">
          <el-switch v-model="createForm.canRunTraining" />
        </el-form-item>
        <el-form-item label="推論実行">
          <el-switch v-model="createForm.canRunInference" />
        </el-form-item>
        <el-form-item label="ノートブック使用">
          <el-switch v-model="createForm.canUseNotebook" />
        </el-form-item>
        <el-form-item label="プロジェクト管理">
          <el-switch v-model="createForm.canManageProject" />
        </el-form-item>
        <el-form-item label="テナント設定変更">
          <el-switch v-model="createForm.canEditTenantSetting" />
        </el-form-item>
        <el-form-item label="リソース権限管理">
          <el-switch v-model="createForm.canManageResourcePermission" />
        </el-form-item>
      </el-form>
      <span slot="footer">
        <el-button @click="createDialogVisible = false">キャンセル</el-button>
        <el-button type="primary" @click="createRole">作成</el-button>
      </span>
    </el-dialog>
  </div>
</template>

<script>
import api from '@/api/api'

export default {
  title: 'カスタムロール管理',
  data() {
    return {
      roles: [],
      createDialogVisible: false,
      createForm: this.getEmptyForm(),
    }
  },
  async created() {
    await this.fetchRoles()
  },
  methods: {
    getEmptyForm() {
      return {
        name: '',
        displayName: '',
        description: '',
        canManageData: false,
        canManageDataSet: false,
        canRunTraining: false,
        canRunInference: false,
        canUseNotebook: false,
        canManageProject: false,
        canEditTenantSetting: false,
        canManageResourcePermission: false,
      }
    },
    async fetchRoles() {
      let response = await api.customRole.get()
      this.roles = response.data
    },
    openCreateDialog() {
      this.createForm = this.getEmptyForm()
      this.createDialogVisible = true
    },
    async createRole() {
      await api.customRole.post(this.createForm)
      this.createDialogVisible = false
      await this.fetchRoles()
    },
    openEditDialog(row) {
      this.$router.push(`/manage/custom-role/${row.id}`)
    },
    closeDialog() {
      this.$router.push('/manage/custom-role')
    },
  },
}
</script>

<style scoped>
.perm-tag {
  margin-right: 4px;
  margin-bottom: 2px;
}
</style>
