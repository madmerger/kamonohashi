<template>
  <el-dialog title="プロジェクト編集" :visible="true" @close="$emit('cancel')">
    <el-tabs v-model="activeTab">
      <el-tab-pane label="基本情報" name="info">
        <el-form :model="form" label-width="120px">
          <el-form-item label="プロジェクト名" required>
            <el-input v-model="form.name" />
          </el-form-item>
          <el-form-item label="説明">
            <el-input v-model="form.description" type="textarea" />
          </el-form-item>
        </el-form>
        <el-row type="flex" justify="end">
          <el-button type="danger" @click="deleteProject">削除</el-button>
          <el-button type="primary" @click="updateProject">更新</el-button>
        </el-row>
      </el-tab-pane>

      <el-tab-pane label="メンバー" name="members">
        <el-row type="flex" justify="end" class="margin">
          <el-button
            type="primary"
            icon="el-icon-plus"
            @click="addMemberDialogVisible = true"
          >
            メンバー追加
          </el-button>
        </el-row>
        <el-table :data="members" border>
          <el-table-column prop="userName" label="ユーザー名" width="200px" />
          <el-table-column prop="roleTypeName" label="権限" width="150px" />
          <el-table-column label="操作" width="120px">
            <template slot-scope="scope">
              <el-button
                type="danger"
                size="mini"
                @click="removeMember(scope.row)"
              >
                削除
              </el-button>
            </template>
          </el-table-column>
        </el-table>

        <!-- メンバー追加ダイアログ -->
        <el-dialog
          title="メンバー追加"
          :visible.sync="addMemberDialogVisible"
          append-to-body
        >
          <el-form :model="memberForm" label-width="120px">
            <el-form-item label="ユーザーID" required>
              <el-input-number
                v-model="memberForm.userId"
                :min="1"
              />
            </el-form-item>
            <el-form-item label="権限" required>
              <el-select v-model="memberForm.roleType">
                <el-option :value="1" label="閲覧のみ (Viewer)" />
                <el-option :value="2" label="実行 (Executor)" />
                <el-option :value="3" label="管理 (Manager)" />
              </el-select>
            </el-form-item>
          </el-form>
          <span slot="footer">
            <el-button @click="addMemberDialogVisible = false">
              キャンセル
            </el-button>
            <el-button type="primary" @click="addMember">追加</el-button>
          </span>
        </el-dialog>
      </el-tab-pane>

      <el-tab-pane label="リソース" name="resources">
        <el-row type="flex" justify="end" class="margin">
          <el-button
            type="primary"
            icon="el-icon-plus"
            @click="addResourceDialogVisible = true"
          >
            リソース追加
          </el-button>
        </el-row>
        <el-table :data="resources" border>
          <el-table-column prop="resourceType" label="種別" width="150px">
            <template slot-scope="scope">
              <span v-if="scope.row.resourceType === 1">データセット</span>
              <span v-else-if="scope.row.resourceType === 2">学習履歴</span>
            </template>
          </el-table-column>
          <el-table-column prop="resourceId" label="リソースID" />
          <el-table-column label="操作" width="120px">
            <template slot-scope="scope">
              <el-button
                type="danger"
                size="mini"
                @click="removeResource(scope.row)"
              >
                削除
              </el-button>
            </template>
          </el-table-column>
        </el-table>

        <!-- リソース追加ダイアログ -->
        <el-dialog
          title="リソース追加"
          :visible.sync="addResourceDialogVisible"
          append-to-body
        >
          <el-form :model="resourceForm" label-width="120px">
            <el-form-item label="種別" required>
              <el-select v-model="resourceForm.resourceType">
                <el-option :value="1" label="データセット" />
                <el-option :value="2" label="学習履歴（モデル）" />
              </el-select>
            </el-form-item>
            <el-form-item label="リソースID" required>
              <el-input-number
                v-model="resourceForm.resourceId"
                :min="1"
              />
            </el-form-item>
          </el-form>
          <span slot="footer">
            <el-button @click="addResourceDialogVisible = false">
              キャンセル
            </el-button>
            <el-button type="primary" @click="addResource">追加</el-button>
          </span>
        </el-dialog>
      </el-tab-pane>
    </el-tabs>
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
      activeTab: 'info',
      form: { name: '', description: '' },
      members: [],
      resources: [],
      addMemberDialogVisible: false,
      addResourceDialogVisible: false,
      memberForm: { userId: null, roleType: 1 },
      resourceForm: { resourceType: 1, resourceId: null },
    }
  },
  async created() {
    await this.fetchProject()
    await this.fetchMembers()
    await this.fetchResources()
  },
  methods: {
    async fetchProject() {
      let response = await api.project.getById({ id: this.id })
      this.form = response.data
    },
    async fetchMembers() {
      let response = await api.project.getMembers({ id: this.id })
      this.members = response.data
    },
    async fetchResources() {
      let response = await api.project.getResources({ id: this.id })
      this.resources = response.data
    },
    async updateProject() {
      await api.project.put({ id: this.id, ...this.form })
      this.$emit('done')
    },
    async deleteProject() {
      await this.$confirm('このプロジェクトを削除しますか？', '確認')
      await api.project.delete({ id: this.id })
      this.$emit('done')
    },
    async addMember() {
      await api.project.postMember({ id: this.id, ...this.memberForm })
      this.addMemberDialogVisible = false
      await this.fetchMembers()
    },
    async removeMember(member) {
      await api.project.deleteMember({ id: this.id, userId: member.userId })
      await this.fetchMembers()
    },
    async addResource() {
      await api.project.postResource({ id: this.id, ...this.resourceForm })
      this.addResourceDialogVisible = false
      await this.fetchResources()
    },
    async removeResource(resource) {
      await api.project.deleteResource({
        id: this.id,
        resourceMapId: resource.id,
      })
      await this.fetchResources()
    },
  },
}
</script>
