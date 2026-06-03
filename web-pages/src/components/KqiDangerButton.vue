<template>
  <span>
    <el-button type="danger" @click="showConfirm">
      <span v-if="buttonLabel">{{ buttonLabel }}</span>
      <span v-else class="el-icon-delete" />
    </el-button>
  </span>
</template>

<script>
export default {
  props: {
    buttonLabel: { type: String, default: '' },
    warningText: {
      type: String,
      default: '',
    },
    confirmText: {
      type: String,
      default: '',
    },
  },
  methods: {
    validateInput(input) {
      if (input === this.confirmText) {
        return true
      } else return this.$t('messages.input_mismatch')
    },
    showConfirm() {
      this.$prompt(this.warningText, 'Warning', {
        confirmButtonText: this.$t('common.confirm'),
        cancelButtonText: this.$t('common.cancel'),
        inputValidator: this.validateInput,
        inputErrorMessage: 'Invalid Name',
      })
        .then(() => {
          this.$emit('delete')
        })
        .catch(() => {
          this.$notify.info({
            type: 'info',
            message: this.$t('common.cancelled'),
          })
        })
    },
  },
}
</script>

<style lang="scss" scoped></style>
