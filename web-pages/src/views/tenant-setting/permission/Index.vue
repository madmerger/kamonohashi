<template>
  <div>
    <h2>リソース権限管理</h2>
    <p class="description">
      データセットや学習モデルに対する個別のアクセス権限を管理します。
    </p>

    <el-row :gutter="20" class="margin">
      <el-col :span="8">
        <el-select v-model="filter.resourceType" placeholder="リソース種別">
          <el-option :value="1" label="データセット" />
          <el-option :value="2" label="学習履歴（モデル）" />
        </el-select>
      </el-col>
      <el-col :span="8">
        <el-input-number
          v-model="filter.resourceId"
          placeholder="リソースID"
          :min="1"
        />
      </el-col>
      <el-col :span="8">
        <el-button type="primary" @click="fetchPermissions">検索</el-button>
      </el-col>
    </el-row>

    <el-row type="flex" justify="end" class="margin">
      <el-button
        type="primary"
        icon="el-icon-plus"
        @click="addDialogVisible = true"
      >
        権限追加
      </el-button>
    </el-row>

    <el-table :data="permissions" border>
      <el-table-column label="リソース種別" width="150px">
        <template slot-scope="scope">
          <span v-if="scope.row.resourceType === 1">データセット</span>
          <span v-else-if="scope.row.resourceType === 2">学習履歴</span>
        </template>
      </el-table-column>
      <el-table-column prop="resourceId" label="リソースID" width="120px" />
      <el-table-column prop="userName" label="ユーザー名" width="200px" />
      <el-table-column label="アクセスレベル" width="150px">
        <template slot-scope="scope">
          <span v-if="scope.row.accessLevel === 1">閲覧のみ</span>
          <span v-else-if="scope.row.accessLevel === 2">実行</span>
          <span v-else-if="scope.row.accessLevel === 3">管理</span>
        </template>
      </el-table-column>
      <el-table-column label="操作" width="120px">
        <template slot-scope="scope">
          <el-button
            type="danger"
            size="mini"
            @click="removePermission(scope.row)"
          >
            削除
          </el-button>
        </template>
      </el-table-column>
    </el-table>

    <!-- 権限追加ダイアログ -->
    <el-dialog title="権限追加" :visible.sync="addDialogVisible" width="500px">
      <el-form :model="addForm" label-width="140px">
        <el-form-item label="リソース種別" required>
          <el-select v-model="addForm.resourceType">
            <el-option :value="1" label="データセット" />
            <el-option :value="2" label="学習履歴（モデル）" />
          </el-select>
        </el-form-item>
        <el-form-item label="リソースID" required>
          <el-input-number v-model="addForm.resourceId" :min="1" />
        </el-form-item>
        <el-form-item label="ユーザーID" required>
          <el-input-number v-model="addForm.userId" :min="1" />
        </el-form-item>
        <el-form-item label="アクセスレベル" required>
          <el-select v-model="addForm.accessLevel">
            <el-option :value="1" label="閲覧のみ (Viewer)" />
            <el-option :value="2" label="実行 (Executor)" />
            <el-option :value="3" label="管理 (Manager)" />
          </el-select>
        </el-form-item>
      </el-form>
      <span slot="footer">
        <el-button @click="addDialogVisible = false">キャンセル</el-button>
        <el-button type="primary" @click="addPermission">追加</el-button>
      </span>
    </el-dialog>
  </div>
</template>

<script>
import api from '@/api/api'

export default {
  title: 'リソース権限管理',
  data() {
    return {
      filter: {
        resourceType: 1,
        resourceId: null,
      },
      permissions: [],
      addDialogVisible: false,
      addForm: {
        resourceType: 1,
        resourceId: null,
        userId: null,
        accessLevel: 1,
      },
    }
  },
  methods: {
    async fetchPermissions() {
      if (!this.filter.resourceId) {
        this.$message.warning('リソースIDを入力してください')
        return
      }
      let response = await api.resourcePermission.get({
        resourceType: this.filter.resourceType,
        resourceId: this.filter.resourceId,
      })
      this.permissions = response.data
    },
    async addPermission() {
      await api.resourcePermission.post(this.addForm)
      this.addDialogVisible = false
      if (this.filter.resourceId) {
        await this.fetchPermissions()
      }
    },
    async removePermission(row) {
      await api.resourcePermission.delete({
        id: row.id,
        resourceType: row.resourceType,
        resourceId: row.resourceId,
      })
      await this.fetchPermissions()
    },
  },
}
</script>

<style scoped>
.description {
  color: #909399;
  margin-bottom: 20px;
}
.margin {
  margin-bottom: 16px;
}
</style>
