<template>
  <div>
    <el-table
      class="data-table pl-index-table"
      :data="allocatableNodes"
      border
      size="mini"
      :empty-text="emptyMessage"
    >
      <el-table-column
        prop="name"
        :label="$t('labels.node_name')"
        width="150px"
      />
      <el-table-column
        prop="partition"
        :label="$t('labels.partition')"
        width="180px"
      />
      <el-table-column prop="allocatableCpu" label="CPU" width="80px" />
      <el-table-column
        prop="allocatableMemory"
        :label="$t('labels.memory_gb')"
        width="100px"
      />
      <el-table-column prop="allocatableGpu" label="GPU" width="80px" />
    </el-table>
  </div>
</template>

<script>
export default {
  props: {
    allocatableNodes: {
      type: Array,
      default: () => [],
    },
    emptyMessage: {
      type: String,
      default: this.$t('messages.no_available_nodes'),
    },
  },
}
</script>

<style lang="scss" scoped></style>
