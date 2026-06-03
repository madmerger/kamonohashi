<!--name: 確認ダイヤログ付きの削除ボタン,-->
<!--description: ボタンを押すと確認が出て、そこで「はい」を押すとdeleteイベントを発火-->
<!--props: {-->
<!--buttonLable: ボタンに表示するラベル。指定しない場合はゴミ箱アイコンを表示,-->
<!--size: ボタンサイズ。 "mini" など-->
<!--disabled: 有効無効-->
<!--message: 削除確認メッセージ。「削除しますか」がデフォルト-->
<!--}-->
<!--events: {  delete: 削除確認に「はい」を選択時に発火}-->
<template>
  <span>
    <el-button
      type="danger"
      :size="size"
      :disabled="disabled"
      @click="showConfirm"
    >
      <span v-if="buttonLabel">{{ buttonLabel }}</span>
      <span v-else class="el-icon-delete" />
    </el-button>
  </span>
</template>

<script>
export default {
  name: 'DeleteButton',
  props: {
    buttonLabel: { type: String, default: '' },
    size: { type: String, default: '' },
    message: { type: String, default: this.$t('messages.delete_confirm') },
    disabled: { type: Boolean, default: false },
  },
  methods: {
    async showConfirm() {
      let confirmMessage = this.message
        ? this.message
        : this.$t('messages.delete_confirm')
      try {
        await this.$confirm(confirmMessage, 'Warning', {
          confirmButtonText: this.$t('common.yes'),
          cancelButtonText: this.$t('common.cancel'),
          type: 'warning',
        })
        this.$emit('delete')
      } catch (e) {
        // キャンセル時はなにもしないので例外を無視
      }
    },
  },
}
</script>

<style lang="scss" scoped>
.cancel-button {
  text-align: left;
}

.ok-button {
  text-align: right;
}
</style>
