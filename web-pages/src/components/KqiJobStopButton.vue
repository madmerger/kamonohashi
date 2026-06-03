<template>
  <span>
    <el-button type="danger" @click="dialogVisible = true">
      <span v-if="buttonLabel">{{ buttonLabel }}</span>
      <span v-else class="el-icon-delete" />
      <el-dialog
        id="dialog"
        :title="title"
        :visible.sync="dialogVisible"
        width="20%"
        center
        append-to-body
      >
        <br />
        <div style="text-align: center;">
          <el-button @click="dialogVisible = false">{{
            $t('common.cancel')
          }}</el-button>
          <el-button type="danger" @click="$emit('halt')">{{
            $t('common.abnormal_stop')
          }}</el-button>
          <el-button type="success" @click="$emit('userCancel')">
            {{ $t('common.normal_stop') }}
          </el-button>
        </div>
      </el-dialog>
    </el-button>
  </span>
</template>

<script>
export default {
  props: {
    title: { type: String, default: 'title' },
    buttonLabel: { type: String, default: '' },
  },
  data() {
    return {
      dialogVisible: false,
    }
  },
}
</script>

<style lang="scss" scoped>
#dialog {
  margin-top: 300px;
}
</style>
