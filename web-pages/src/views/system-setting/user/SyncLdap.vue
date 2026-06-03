<template>
  <el-dialog
    class="dialog"
    :title="$t('titles.ldap_sync')"
    :visible.sync="dialogVisible"
    :before-close="emitCancel"
    :close-on-click-modal="false"
  >
    <p>
      {{ $t('messages.ldap_sync_description') }}
    </p>
    <p>{{ $t('messages.ldap_sync_time_warning') }}</p>
    <el-row class="footer">
      <el-form ref="createForm" :model="form" :rules="rules">
        <kqi-display-error :error="error" />

        <el-form-item :label="$t('labels.ldap_user_id')" prop="name">
          <el-input v-model="form.name" />
        </el-form-item>
        <el-form-item :label="$t('labels.ldap_password')" prop="password">
          <el-input v-model="form.password" type="password" />
        </el-form-item>
      </el-form>
      <el-col :span="24" class="right-button-group">
        <el-button @click="emitCancel">{{ $t('common.cancel') }}</el-button>
        <el-button type="primary" @click="syncLdap">{{
          $t('common.sync_start')
        }}</el-button>
      </el-col>
    </el-row>
  </el-dialog>
</template>

<script>
import KqiDisplayError from '@/components/KqiDisplayError'
import { createNamespacedHelpers } from 'vuex'
const { mapActions } = createNamespacedHelpers('user')

const formRule = {
  required: true,
  trigger: 'blur',
  message: this.$t('common.required_field'),
}
export default {
  components: {
    KqiDisplayError,
  },
  data() {
    return {
      form: {
        name: '',
        password: '',
      },
      rules: {
        name: [formRule],
        password: [formRule],
      },
      dialogVisible: true,
      error: null,
    }
  },
  methods: {
    ...mapActions(['syncLdapUsers']),
    async syncLdap() {
      let form = this.$refs.createForm
      await form.validate(async valid => {
        if (valid) {
          if (await this.showConfirm()) {
            try {
              let params = {
                body: {
                  userName: this.form.name,
                  password: this.form.password,
                },
              }
              await this.syncLdapUsers(params)
              this.emitDone()
              this.error = null
            } catch (e) {
              this.error = e
            }
          }
        }
      })
    },
    async showConfirm() {
      let confirmMessage = this.$t('messages.ldap_sync_confirm')
      try {
        await this.$confirm(confirmMessage, 'Warning', {
          confirmButtonText: this.$t('common.yes'),
          cancelButtonText: this.$t('common.cancel'),
          type: 'warning',
        })
        return true
      } catch (e) {
        return false
      }
    },
    emitDone() {
      this.$emit('done')
      this.dialogVisible = false
    },
    emitCancel() {
      this.$emit('cancel')
    },
  },
}
</script>

<style lang="scss" scoped>
.right-button-group {
  text-align: right;
}
.footer {
  padding-top: 40px;
}
p {
  padding-top: 5px;
}
</style>
