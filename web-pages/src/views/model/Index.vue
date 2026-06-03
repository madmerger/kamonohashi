<template>
  <div>
    <h2>モデル管理</h2>
    <el-row type="flex" justify="space-between" :gutter="20">
      <el-col class="right-top-button" :span="8">
        <el-button
          icon="el-icon-edit-outline"
          type="primary"
          plain
          @click="openCreateDialog()"
        >
          新規登録
        </el-button>
      </el-col>
    </el-row>
    <el-row>
      <el-table
        class="data-table pl-index-table"
        :data="models"
        border
        @row-click="openEditDialog"
      >
        <el-table-column prop="id" label="ID" width="120px" />
        <el-table-column prop="name" label="モデル名" width="200px" />
        <el-table-column
          prop="description"
          label="説明"
          width="auto"
          class-name="description-column"
        />
        <el-table-column prop="createdBy" label="登録者" width="150px" />
        <el-table-column prop="createdAt" label="作成日時" width="200px" />
      </el-table>
    </el-row>
    <router-view @cancel="closeDialog" @done="closeDialog" />
  </div>
</template>

<script>
import api from '@/api/api'

export default {
  data() {
    return {
      models: [],
    }
  },
  async created() {
    await this.retrieveData()
  },
  methods: {
    async retrieveData() {
      try {
        let response = await api.model.get()
        this.models = response.data
      } catch (e) {
        this.$notify.error('モデル一覧の取得に失敗しました。')
      }
    },
    openCreateDialog() {
      this.$router.push('/model/create')
    },
    openEditDialog(row) {
      this.$router.push('/model/' + row.id)
    },
    closeDialog() {
      this.$router.push('/model')
      this.retrieveData()
    },
  },
}
</script>

<style lang="scss" scoped>
.right-top-button {
  text-align: right;
}
</style>
