export default {
  mounted() {
    let { title } = this.$options
    if (title) {
      title = typeof title === 'function' ? title.call(this) : title
      if (this.$t) {
        title = this.$t(title) || title
      }
      document.title = `${title} - KAMONOHASHI`
    }
  },
  methods: {
    showSuccessMessage: function(msg) {
      if (msg) {
        this.$notify.success({ title: msg })
      } else {
        this.$notify.success({ title: this.$t('common.processedSuccessfully') })
      }
    },
  },
}
