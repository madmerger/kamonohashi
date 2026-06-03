<template>
  <div>
    <h2>{{ $t('titles.user_group_management') }}</h2>
    <el-row :gutter="20">
      <el-col class="right-top-button">
        <el-button
          icon="el-icon-edit-outline"
          type="primary"
          plain
          @click="openCreateDialog"
        >
          {{ $t('common.new_registration') }}
        </el-button>
      </el-col>
    </el-row>
    <el-row>
      <el-table
        class="data-table pl-index-table"
        :data="userGroups"
        @row-click="openEditDialog"
      >
        <el-table-column prop="id" label="ID" width="100px" />
        <el-table-column
          prop="name"
          :label="$t('labels.user_group_name')"
          width="300px"
        />
        <el-table-column
          prop="isGroup"
          :label="$t('labels.type')"
          width="150px"
        >
          <template slot-scope="scope">
            <span v-if="scope.row.isGroup">{{ $t('common.group') }}</span>
            <span v-else>OU</span>
          </template>
        </el-table-column>
        <el-table-column prop="dn" label="DN" width="auto" />
        <el-table-column prop="memo" :label="$t('labels.memo')" width="auto" />
      </el-table>
    </el-row>
    <router-view @cancel="closeDialog()" @done="done()" />
  </div>
</template>
<script>
import { createNamespacedHelpers } from 'vuex'
const { mapGetters, mapActions } = createNamespacedHelpers('userGroup')

export default {
  title: this.$t('titles.user_group_management'),
  computed: {
    ...mapGetters(['userGroups']),
  },
  async created() {
    await this.fetchUserGroups()
  },
  methods: {
    ...mapActions(['fetchUserGroups']),
    openCreateDialog() {
      this.$router.push('/usergroup/edit')
    },
    openEditDialog(selectedRow) {
      this.$router.push('/usergroup/edit/' + selectedRow.id)
    },
    closeDialog() {
      this.$router.push('/usergroup')
    },
    async done() {
      await this.fetchUserGroups()
      this.closeDialog()
      this.showSuccessMessage()
    },
  },
}
</script>
<style lang="scss" scoped>
.right-top-button {
  text-align: right;
  padding-top: 10px;
}
</style>
