<template>
  <div>
    <kqi-display-text-form
      :label="$t('common.preprocessId')"
      :value="preprocessing ? String(preprocessing.id) : '0'"
    />
    <kqi-display-text-form
      :label="$t('preprocessing.preprocessName')"
      :value="preprocessing ? preprocessing.name : ''"
    />
    <kqi-display-text-form
      :label="$t('common.memo')"
      :value="preprocessing ? preprocessing.memo : ''"
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
    preprocessing: {
      type: Object,
      default: () => ({
        id: 0,
        name: '',
        memo: '',
      }),
    },
  },
}
</script>

<style lang="scss" scoped></style>
