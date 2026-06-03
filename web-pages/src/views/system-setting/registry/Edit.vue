<template>
  <kqi-dialog
    :title="title"
    :type="id === null ? 'CREATE' : 'EDIT'"
    :disabled-params="disabledParams"
    @submit="submit"
    @delete="deleteRegistry"
    @close="emitCancel"
  >
    <el-form ref="createForm" :model="form" :rules="rules">
      <kqi-display-error :error="error" />
      <el-form-item :label="$t('labels.registry_name')" prop="name">
        <el-input v-model="form.name" :disabled="isNotEditable" />
      </el-form-item>
      <h3>{{ $t('titles.registry_info') }}</h3>
      <div style="padding-left: 30px; padding-right: 10px;">
        <el-form-item :label="$t('labels.type')" prop="serviceType">
          <el-select
            v-model="form.serviceType"
            style="width: 100%;"
            :disabled="isNotEditable"
            @change="changeService"
          >
            <el-option
              v-for="service in serviceTypes"
              :key="service.id"
              :label="service.name"
              :value="service.id"
            />
          </el-select>
        </el-form-item>
        <el-row>
          <el-col :span="16">
            <el-form-item :label="$t('labels.host_name')" prop="host">
              <el-input
                v-model="form.host"
                :disabled="isNotEditable"
                @change="handleChange"
              />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item :label="$t('labels.port')" prop="portNo">
              <el-input-number
                v-model="form.portNo"
                :min="1"
                :max="65535"
                controls-position="right"
                style="width: 100%;"
                :disabled="isNotEditable"
                @change="handleChange"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item label="API URL" prop="apiUrl">
          <el-input v-model="form.apiUrl" :disabled="isNotEditable" />
        </el-form-item>
        <div v-if="form.serviceType === 1">
          <el-form-item label="Registry URL" prop="registryUrl">
            <el-input v-model="form.registryUrl" :disabled="isNotEditable" />
          </el-form-item>
        </div>
        <div v-if="form.serviceType === 2">
          <el-form-item :label="$t('labels.project_name')" prop="projectName">
            <el-input v-model="form.projectName" :disabled="isNotEditable" />
          </el-form-item>
        </div>
      </div>
    </el-form>
  </kqi-dialog>
</template>

<script>
import KqiDialog from '@/components/KqiDialog'
import KqiDisplayError from '@/components/KqiDisplayError'
import { createNamespacedHelpers } from 'vuex'

const defaultProtocol = 'https://'
const { mapGetters, mapActions } = createNamespacedHelpers('registry')
export default {
  components: {
    KqiDialog,
    KqiDisplayError,
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
        name: null,
        host: null,
        portNo: null,
        projectName: null,
        serviceType: null,
        apiUrl: null,
        registryUrl: null,
      },
      rules: {
        name: [formRule],
        host: [formRule],
        apiUrl: [formRule],
        registryUrl: [formRule],
        portNo: [formRule],
        serviceType: [formRule],
        projectName: [formRule],
      },
      dialogVisible: true,
      error: null,
      isNotEditable: false,
    }
  },
  computed: {
    ...mapGetters(['detail', 'serviceTypes']),
    disabledParams() {
      return {
        deleteButton: this.isNotEditable,
        submitButton: this.isNotEditable,
      }
    },
  },
  async created() {
    if (this.id === null) {
      this.title = this.$t('titles.registry_registration')
    } else {
      this.title = this.$t('titles.registry_edit')
      try {
        await this.fetchDetail(this.id)
        this.form.name = this.detail.name
        this.form.host = this.detail.host
        this.form.portNo = this.detail.portNo
        this.form.projectName = this.detail.projectName
        this.form.serviceType = this.detail.serviceType
        this.form.apiUrl = this.detail.apiUrl
        this.form.registryUrl = this.detail.registryUrl

        this.isNotEditable = this.detail.isNotEditable
      } catch (e) {
        this.error = e
      }
    }
    await this.fetchServiceTypes()
  },
  methods: {
    ...mapActions([
      'fetchDetail',
      'fetchServiceTypes',
      'delete',
      'post',
      'put',
    ]),
    async submit() {
      let form = this.$refs.createForm
      await form.validate(async valid => {
        if (valid) {
          try {
            let params = {
              name: this.form.name,
              host: this.form.host,
              portNo: this.form.portNo,
              serviceType: this.form.serviceType,
              projectName: this.form.projectName,
              apiUrl: this.form.apiUrl,
              registryUrl:
                this.form.serviceType === 1
                  ? this.form.registryUrl
                  : this.form.apiUrl.endsWith('/')
                  ? this.form.apiUrl.slice(0, -1) + ':' + this.form.portNo
                  : this.form.apiUrl + ':' + this.form.portNo,
            }
            if (this.id === null) {
              await this.post(params)
            } else {
              await this.put({ id: this.id, params: params })
            }
            this.emitDone()
            this.error = null
          } catch (e) {
            this.error = e
          }
        }
      })
    },
    async deleteRegistry() {
      try {
        await this.delete(this.id)
        this.emitDone()
      } catch (e) {
        this.error = e
      }
    },
    handleChange() {
      if (this.form.host && !this.form.apiUrl) {
        this.form.apiUrl = defaultProtocol + this.form.host
      }
    },
    changeService() {
      this.form.projectName = null
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
