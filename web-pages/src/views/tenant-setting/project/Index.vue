<template>
  <div>
    <h2>プロジェクト管理</h2>
    <el-row type="flex" justify="end" class="margin">
      <el-button type="primary" icon="el-icon-plus" @click="openCreateDialog">
        新規プロジェクト
      </el-button>
    </el-row>
    <el-table
      class="data-table pl-index-table"
      :data="projects"
      border
      @row-click="openEditDialog"
    >
      <el-table-column prop="name" label="プロジェクト名" width="250px" />
      <el-table-column prop="description" label="説明" />
      <el-table-column prop="createdAt" label="作成日" width="200px" />
    </el-table>

    <!-- 作成ダイアログ -->
    <el-dialog title="プロジェクト作成" :visible.sync="createDialogVisible">
      <el-form :model="createForm" label-width="120px">
        <el-form-item label="プロジェクト名" required>
          <el-input v-model="createForm.name" />
        </el-form-item>
        <el-form-item label="説明">
          <el-input v-model="createForm.description" type="textarea" />
        </el-form-item>
      </el-form>
      <span slot="footer">
        <el-button @click="createDialogVisible = false">キャンセル</el-button>
        <el-button type="primary" @click="createProject">作成</el-button>
      </span>
    </el-dialog>

    <router-view @done="fetchProjects" @cancel="closeDialog" />
  </div>
</template>

<script>
import api from '@/api/api'

export default {
  title: 'プロジェクト管理',
  data() {
    return {
      projects: [],
      createDialogVisible: false,
      createForm: {
        name: '',
        description: '',
      },
    }
  },
  async created() {
    await this.fetchProjects()
  },
  methods: {
    async fetchProjects() {
      let response = await api.project.get()
      this.projects = response.data
    },
    openCreateDialog() {
      this.createForm = { name: '', description: '' }
      this.createDialogVisible = true
    },
    async createProject() {
      await api.project.post(this.createForm)
      this.createDialogVisible = false
      await this.fetchProjects()
    },
    openEditDialog(row) {
      this.$router.push(`/manage/project/${row.id}`)
    },
    closeDialog() {
      this.$router.push('/manage/project')
    },
  },
}
</script>
