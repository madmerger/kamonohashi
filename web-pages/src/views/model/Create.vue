<template>
  <kqi-dialog
    title="モデル登録"
    type="CREATE"
    @submit="onSubmit"
    @close="$emit('cancel')"
  >
    <el-form ref="createForm" :model="form" :rules="rules">
      <kqi-display-error :error="error" />
      <el-form-item label="モデル名" prop="name">
        <el-input v-model="form.name" />
      </el-form-item>
      <el-form-item label="説明">
        <el-input v-model="form.description" type="textarea" :rows="3" />
      </el-form-item>
    </el-form>
  </kqi-dialog>
</template>

<script>
import api from '@/api/api'

export default {
  data() {
    return {
      form: {
        name: '',
        description: '',
      },
      error: null,
      rules: {
        name: [{ required: true, message: 'モデル名は必須です', trigger: 'blur' }],
      },
    }
  },
  methods: {
    async onSubmit() {
      let valid = await this.$refs.createForm.validate().catch(() => false)
      if (!valid) return

      try {
        await api.model.post({
          body: {
            name: this.form.name,
            description: this.form.description,
          },
        })
        this.$emit('done')
      } catch (e) {
        this.error = e
      }
    },
  },
}
</script>
