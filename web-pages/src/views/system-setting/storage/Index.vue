<template>
  <div>
    <h2>{{ $t('titles.storage_management') }}</h2>
    <el-row>
      <el-col class="create-new">
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
        class="storage-table pl-index-table"
        :data="storages"
        border
        @row-click="openEditDialog"
      >
        <el-table-column prop="id" label="ID" width="100px" />
        <el-table-column
          prop="name"
          :label="$t('labels.storage_name')"
          width="auto"
        />
        <el-table-column
          prop="serverUrl"
          :label="$t('labels.host_port')"
          width="auto"
        />
        <el-table-column
          prop="nfsServer"
          :label="$t('labels.nfs_server')"
          width="auto"
        />
        <el-table-column
          prop="nfsRoot"
          :label="$t('labels.nfs_export_point')"
          width="auto"
        />
      </el-table>
    </el-row>
    <router-view @cancel="closeDialog()" @done="done()" />
  </div>
</template>

<script>
import { createNamespacedHelpers } from 'vuex'
const { mapGetters, mapActions } = createNamespacedHelpers('storage')

export default {
  title: this.$t('titles.storage_management'),
  computed: {
    ...mapGetters(['storages']),
  },
  async created() {
    await this.fetchStorages()
  },
  methods: {
    ...mapActions(['fetchStorages']),
    openCreateDialog() {
      this.$router.push('/storage/edit')
    },
    openEditDialog(selectedRow) {
      this.$router.push('/storage/edit/' + selectedRow.id)
    },
    closeDialog() {
      this.$router.push('/storage')
    },
    async done() {
      await this.fetchStorages()
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
