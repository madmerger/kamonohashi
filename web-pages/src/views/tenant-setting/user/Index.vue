<template>
  <div>
    <h2>{{ $t('titles.tenant_user_management') }}</h2>
    <el-row>
      <el-table
        class="data-table pl-index-table"
        :data="tenantUsers"
        border
        @row-click="openEditDialog"
      >
        <el-table-column
          prop="name"
          :label="$t('labels.user_name')"
          width="300px"
        >
          <template slot-scope="scope">
            <p>
              {{ scope.row.name
              }}<span v-if="scope.row.displayName"
                >【{{ scope.row.displayName }}】</span
              >
            </p>
          </template>
        </el-table-column>
        <el-table-column
          prop="serviceType"
          :label="$t('labels.auth_type')"
          width="150px"
        >
          <template slot-scope="scope">
            <span v-if="scope.row.serviceType === 1">{{
              $t('common.local')
            }}</span>
            <span v-else-if="scope.row.serviceType === 2">LDAP</span>
            <span v-else>{{ serviceType }}</span>
          </template>
        </el-table-column>
        <el-table-column prop="roles" :label="$t('labels.role')" width="auto">
          <template slot-scope="scope">
            <span v-for="role in scope.row.roles" :key="role.id">
              <el-tag class="role-tag" :type="role.isOrigin ? '' : 'success'">
                {{ role.displayName }}
              </el-tag>
            </span>
          </template>
        </el-table-column>
      </el-table>
    </el-row>

    <router-view @done="done()" @cancel="closeDialog()" />
  </div>
</template>

<script>
import { createNamespacedHelpers } from 'vuex'
const { mapGetters, mapActions } = createNamespacedHelpers('user')

export default {
  title() {
    return this.$t('titles.tenant_user_management')
  },
  data() {
    return {
      tenantEditDialogVisible: false,
    }
  },
  computed: {
    ...mapGetters(['tenantUsers']),
  },
  async created() {
    await this.fetchTenantUsers()
  },

  methods: {
    ...mapActions(['fetchTenantUsers']),

    async openEditDialog(row) {
      this.$router.push('/manage/user/' + row.id)
    },
    closeDialog() {
      this.$router.push('/manage/user')
    },
    async done() {
      this.closeDialog()
      await this.fetchTenantUsers()
      this.showSuccessMessage()
    },
  },
}
</script>

<style lang="scss" scoped>
.role-tag {
  margin-right: 8px;
}
</style>
