<template>
  <el-dialog
    title="カスタムロール編集"
    :visible="true"
    width="600px"
    @close="$emit('cancel')"
  >
    <el-form :model="form" label-width="180px">
      <el-form-item label="ロール名" required>
        <el-input v-model="form.name" />
      </el-form-item>
      <el-form-item label="表示名" required>
        <el-input v-model="form.displayName" />
      </el-form-item>
      <el-form-item label="説明">
        <el-input v-model="form.description" type="textarea" />
      </el-form-item>
      <el-divider content-position="left">権限設定</el-divider>
      <el-form-item label="データ管理">
        <el-switch v-model="form.canManageData" />
      </el-form-item>
      <el-form-item label="データセット管理">
        <el-switch v-model="form.canManageDataSet" />
      </el-form-item>
      <el-form-item label="学習実行">
        <el-switch v-model="form.canRunTraining" />
      </el-form-item>
      <el-form-item label="推論実行">
        <el-switch v-model="form.canRunInference" />
      </el-form-item>
      <el-form-item label="ノートブック使用">
        <el-switch v-model="form.canUseNotebook" />
      </el-form-item>
      <el-form-item label="プロジェクト管理">
        <el-switch v-model="form.canManageProject" />
      </el-form-item>
      <el-form-item label="テナント設定変更">
        <el-switch v-model="form.canEditTenantSetting" />
      </el-form-item>
      <el-form-item label="リソース権限管理">
        <el-switch v-model="form.canManageResourcePermission" />
      </el-form-item>
    </el-form>
    <span slot="footer">
      <el-button type="danger" @click="deleteRole">削除</el-button>
      <el-button @click="$emit('cancel')">キャンセル</el-button>
      <el-button type="primary" @click="updateRole">更新</el-button>
    </span>
  </el-dialog>
</template>

<script>
import api from '@/api/api'

export default {
  props: {
    id: {
      type: String,
      required: true,
    },
  },
  data() {
    return {
      form: {},
    }
  },
  async created() {
    let response = await api.customRole.getById({ id: this.id })
    this.form = response.data
  },
  methods: {
    async updateRole() {
      await api.customRole.put({ id: this.id, ...this.form })
      this.$emit('done')
    },
    async deleteRole() {
      await this.$confirm('このカスタムロールを削除しますか？', '確認')
      await api.customRole.delete({ id: this.id })
      this.$emit('done')
    },
  },
}
</script>
