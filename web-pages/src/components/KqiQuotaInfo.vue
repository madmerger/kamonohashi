<template>
  <div>
    <kqi-display-text-form
      label="CPU"
      :value="quota.cpu ? quota.cpu : $t('common.unlimited')"
    />
    <kqi-display-text-form
      :label="$t('labels.memory')"
      :value="quota.memory ? quota.memory + ' GB' : $t('common.unlimited')"
    />
    <kqi-display-text-form
      label="GPU"
      :value="quota.gpu ? quota.gpu : $t('common.unlimited')"
    />
  </div>
</template>

<script>
import KqiDisplayTextForm from '@/components/KqiDisplayTextForm'

export default {
  components: {
    KqiDisplayTextForm,
  },
  props: {
    quota: {
      type: Object,
      default: () => ({
        cpu: null,
        memory: null,
        gpu: null,
      }),
    },
  },
}
</script>

<style lang="scss" scoped></style>
