<template>
  <div>
    <h2>{{ $t('titles.role_management') }}</h2>
    <el-row>
      <el-col class="create-new">
        <el-button
          icon="el-icon-edit-outline"
          type="primary"
          plain
          @click="openCreateDialog"
        >
          {{ $t('common.new_creation') }}
        </el-button>
      </el-col>
    </el-row>
    <el-row>
      <el-table
        class="data-table pl-index-table"
        :data="roles"
        border
        @row-click="openEditDialog"
      >
        <el-table-column prop="id" label="ID" width="120px" />
        <el-table-column
          prop="name"
          :label="$t('labels.role_name')"
          width="auto"
        />
        <el-table-column
          prop="displayName"
          :label="$t('labels.display_name')"
          width="auto"
        />
        <el-table-column
          prop="isSystemRole"
          :label="$t('labels.type')"
          width="auto"
        >
          <template slot-scope="scope">
            <span v-if="scope.row.isSystemRole">{{ $t('common.system') }}</span>
            <span v-else-if="scope.row.tenantId">{{
              $t('messages.tenant_custom')
            }}</span>
            <span v-else>{{ $t('messages.tenant_common') }}</span>
          </template>
        </el-table-column>
        <el-table-column
          prop="sortOrder"
          :label="$t('labels.display_order')"
          width="auto"
        />
      </el-table>
    </el-row>

    <router-view @cancel="closeDialog()" @done="done()" />
  </div>
</template>

<script>
import { createNamespacedHelpers } from 'vuex'
const { mapGetters, mapActions } = createNamespacedHelpers('role')

export default {
  title: this.$t('titles.role_management'),
  computed: {
    ...mapGetters(['roles']),
  },
  async created() {
    await this.fetchRoles()
  },
  methods: {
    ...mapActions(['fetchRoles']),
    openCreateDialog() {
      this.$router.push('/role/edit')
    },
    openEditDialog(selectedRow) {
      this.$router.push('/role/edit/' + selectedRow.id)
    },
    closeDialog() {
      this.$router.push('/role')
    },
    async done() {
      await this.fetchRoles()
      this.closeDialog()
      this.showSuccessMessage()
    },
  },
}
</script>

<style lang="scss" scoped>
.create-new {
  text-align: right;
  padding-top: 10px;
}
</style>
