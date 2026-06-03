<template>
  <kqi-dialog
    :title="$t('titles.tenant_user_edit')"
    :type="'EDIT'"
    :delete-button-params="deleteButtonParams"
    :disabled-params="disabledParams"
    @submit="submit"
    @delete="deleteUser"
    @close="emitCancel"
  >
    <el-form ref="form" :model="form" :rules="rules">
      <kqi-display-error :error="error" />
      <kqi-display-text-form
        :label="$t('labels.user_name')"
        :value="
          detail
            ? detail.displayName
              ? detail.name + '【' + detail.displayName + '】'
              : detail.name
            : ''
        "
      />
      <kqi-display-text-form
        :label="$t('labels.auth_type')"
        :value="displayServiceType"
      />
      <el-form-item :label="$t('labels.tenant_role')" prop="tenantRoleIds">
        <kqi-role-selector
          v-model="form.tenantRoleIds"
          :roles="roles"
          :show-system-role="false"
        />
        <div v-if="tenantNotOriginRoleIds.length > 0">
          <label>ユーザグループ経由でのテナントロール</label>
          <kqi-role-selector
            v-model="tenantNotOriginRoleIds"
            :roles="roles"
            :show-system-role="false"
            :is-disabled="true"
          />
        </div>
      </el-form-item>
    </el-form>
  </kqi-dialog>
</template>

<script>
import KqiDialog from '@/components/KqiDialog'
import KqiDisplayError from '@/components/KqiDisplayError'
import KqiDisplayTextForm from '@/components/KqiDisplayTextForm'
import KqiRoleSelector from '@/components/selector/KqiRoleSelector'
import { mapGetters, mapActions } from 'vuex'

export default {
  components: {
    KqiDialog,
    KqiDisplayError,
    KqiDisplayTextForm,
    KqiRoleSelector,
  },
  props: {
    id: {
      type: String,
      default: null,
    },
  },
  data() {
    const formRule = {
      required: true,
      trigger: 'blur',
      message: this.$t('common.required_field'),
    }
    return {
      form: {
        tenantRoleIds: [],
      },
      displayServiceType: '',
      disabledParams: {},
      deleteButtonParams: {},
      dialogVisible: true,
      error: null,
      rules: {
        tenantRoleIds: [formRule],
      },
      tenantNotOriginRoleIds: [],
    }
  },
  computed: {
    ...mapGetters({
      detail: ['user/tenantUserDetail'],
      roles: ['role/tenantRoles'],
    }),
  },

  async created() {
    try {
      await this['user/fetchTenantUserDetail'](this.id)
      await this['role/fetchTenantRoles']()

      // serviceTypeのIDを変換
      switch (this.detail.serviceType) {
        case 1:
          this.displayServiceType = this.$t('common.local')
          break
        case 2:
          this.displayServiceType = 'LDAP'
          break
        default:
          this.displayServiceType = this.detail.serviceType
          break
      }
      // ロール一覧からIDを抽出
      this.detail.roles.forEach(s => {
        if (s.isOrigin) {
          this.form.tenantRoleIds.push(s.id)
        }
        if (s.userGroupTanantMapIdLists.length > 0) {
          this.tenantNotOriginRoleIds.push(s.id)
        }
      })
      // ユーザグループ由来のロールがある時は必須チェックしない
      if (this.tenantNotOriginRoleIds.length > 0) {
        this.rules.tenantRoleIds = null
        // deleteButtonのパラメータを設定
        this.disabledParams = {
          deleteButton: true,
          submitButton: false,
        }
      } else {
        // dangerButtonのパラメータを設定
        this.deleteButtonParams = {
          isDanger: true,
          warningText: this.$t('messages.tenant_user_remove_confirm'),
          confirmText: this.detail.name,
        }
      }
      this.error = null
    } catch (e) {
      this.error = e
    }
  },

  methods: {
    ...mapActions([
      'user/fetchTenantUserDetail',
      'user/tenantRolesPut',
      'user/tenantUserDelete',
      'role/fetchTenantRoles',
    ]),
    async submit() {
      let form = this.$refs.form
      await form.validate(async valid => {
        if (valid) {
          try {
            let params = {
              id: this.id,
              body: this.form.tenantRoleIds,
            }
            await this['user/tenantRolesPut'](params)
            this.emitDone()
            this.error = null
          } catch (e) {
            this.error = e
          }
        }
      })
    },

    async deleteUser() {
      try {
        await this['user/tenantUserDelete'](this.id)
        this.emitDone()
        this.error = null
      } catch (e) {
        this.error = e
      }
    },

    emitDone() {
      this.$emit('done')
    },

    emitCancel() {
      this.$emit('cancel')
    },
  },
}
</script>

<style lang="scss" scoped></style>
