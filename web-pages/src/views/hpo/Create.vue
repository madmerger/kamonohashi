<template>
  <el-dialog
    class="dialog"
    title="HPOジョブ実行"
    :visible.sync="dialogVisible"
    :before-close="closeDialog"
    :close-on-click-modal="false"
    width="80%"
  >
    <kqi-display-error :error="error" />
    <el-form ref="runForm" :rules="rules" :model="form">
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="ジョブ名" prop="name">
            <el-input v-model="form.name" />
          </el-form-item>

          <el-form-item label="探索アルゴリズム" prop="algorithm">
            <el-select v-model="form.algorithm" placeholder="選択してください">
              <el-option label="グリッドサーチ" value="Grid" />
              <el-option label="ランダムサーチ" value="Random" />
              <el-option label="ベイズ最適化" value="Bayes" />
            </el-select>
          </el-form-item>

          <el-form-item label="最大トライアル数" prop="maxTrials">
            <el-input-number v-model="form.maxTrials" :min="1" :max="1000" />
          </el-form-item>

          <el-form-item label="目的メトリクス" prop="objectiveMetric">
            <el-input v-model="form.objectiveMetric" placeholder="例: loss, accuracy" />
          </el-form-item>

          <el-form-item label="最適化方向" prop="optimizationDirection">
            <el-radio-group v-model="form.optimizationDirection">
              <el-radio label="minimize">最小化</el-radio>
              <el-radio label="maximize">最大化</el-radio>
            </el-radio-group>
          </el-form-item>

          <el-form-item label="探索空間定義">
            <div v-for="(param, index) in form.searchSpace" :key="index" class="search-space-item">
              <el-row :gutter="10">
                <el-col :span="5">
                  <el-input v-model="param.name" placeholder="パラメータ名" size="small" />
                </el-col>
                <el-col :span="4">
                  <el-select v-model="param.type" placeholder="型" size="small" @change="onParamTypeChange(param)">
                    <el-option label="int" value="int" />
                    <el-option label="float" value="float" />
                    <el-option label="categorical" value="categorical" />
                  </el-select>
                </el-col>
                <el-col v-if="param.type !== 'categorical'" :span="4">
                  <el-input-number v-model="param.min" placeholder="最小値" size="small" :controls="false" />
                </el-col>
                <el-col v-if="param.type !== 'categorical'" :span="4">
                  <el-input-number v-model="param.max" placeholder="最大値" size="small" :controls="false" />
                </el-col>
                <el-col v-if="param.type !== 'categorical'" :span="4">
                  <el-input-number v-model="param.step" placeholder="ステップ" size="small" :controls="false" />
                </el-col>
                <el-col v-if="param.type === 'categorical'" :span="12">
                  <el-input v-model="param.valuesStr" placeholder="値（カンマ区切り）" size="small" />
                </el-col>
                <el-col :span="3">
                  <el-button type="danger" icon="el-icon-delete" size="small" @click="removeParam(index)" />
                </el-col>
              </el-row>
            </div>
            <el-button type="primary" size="small" icon="el-icon-plus" @click="addParam">
              パラメータ追加
            </el-button>
          </el-form-item>
        </el-col>

        <el-col :span="12">
          <kqi-data-set-selector
            v-model="form.dataSetId"
            :data-sets="dataSets"
          />

          <el-form-item label="実行コマンド" prop="entryPoint">
            <el-input
              v-model="form.entryPoint"
              type="textarea"
              :autosize="{ minRows: 5 }"
              placeholder="ハイパーパラメータは環境変数として注入されます"
            />
          </el-form-item>

          <kqi-container-selector
            v-model="form.containerImage"
            :registries="registries"
            :images="images"
            :tags="tags"
            @selectRegistry="selectRegistry"
            @selectImage="selectImage"
          />

          <kqi-git-selector
            v-model="form.gitModel"
            :gits="gits"
            :repositories="repositories"
            :branches="branches"
            :commits="commitsList"
            :loading-repositories="loadingRepositories"
            @selectGit="selectGit"
            @selectRepository="selectRepository"
            @selectBranch="selectBranch"
            @searchCommitId="searchCommitId"
            @getMoreCommits="getMoreCommits"
          />

          <kqi-resource-selector v-model="form.resource" :quota="quota" />

          <el-form-item label="メモ">
            <el-input v-model="form.memo" type="textarea" :autosize="{ minRows: 2 }" />
          </el-form-item>
        </el-col>
      </el-row>
    </el-form>

    <span slot="footer">
      <el-button @click="closeDialog">キャンセル</el-button>
      <el-button type="primary" @click="submit">実行</el-button>
    </span>
  </el-dialog>
</template>

<script>
import KqiDisplayError from '@/components/KqiDisplayError'
import KqiDataSetSelector from '@/components/selector/KqiDataSetSelector'
import KqiContainerSelector from '@/components/selector/KqiContainerSelector'
import KqiGitSelector from '@/components/selector/KqiGitSelector'
import KqiResourceSelector from '@/components/selector/KqiResourceSelector'
import api from '@/api/api'
import registrySelectorUtil from '@/util/registrySelectorUtil'
import gitSelectorUtil from '@/util/gitSelectorUtil'

export default {
  components: {
    KqiDisplayError,
    KqiDataSetSelector,
    KqiContainerSelector,
    KqiGitSelector,
    KqiResourceSelector,
  },
  mixins: [registrySelectorUtil, gitSelectorUtil],
  data() {
    return {
      dialogVisible: true,
      error: null,
      form: {
        name: '',
        algorithm: 'Random',
        maxTrials: 20,
        objectiveMetric: 'loss',
        optimizationDirection: 'minimize',
        searchSpace: [
          { name: '', type: 'float', min: null, max: null, step: null, valuesStr: '' },
        ],
        dataSetId: null,
        entryPoint: '',
        containerImage: { registryId: null, image: '', tag: '' },
        gitModel: { gitId: null, repository: '', owner: '', branch: '', commitId: '' },
        resource: { cpu: 1, memory: 1, gpu: 0 },
        memo: '',
      },
      dataSets: [],
      quota: {},
      rules: {
        name: [{ required: true, message: 'ジョブ名を入力してください', trigger: 'blur' }],
        algorithm: [{ required: true, message: 'アルゴリズムを選択してください', trigger: 'change' }],
        maxTrials: [{ required: true, message: '最大トライアル数を入力してください', trigger: 'blur' }],
        objectiveMetric: [{ required: true, message: '目的メトリクスを入力してください', trigger: 'blur' }],
        optimizationDirection: [{ required: true, message: '最適化方向を選択してください', trigger: 'change' }],
        entryPoint: [{ required: true, message: '実行コマンドを入力してください', trigger: 'blur' }],
      },
    }
  },
  async created() {
    await this.fetchDataSets()
    await this.fetchQuota()
  },
  methods: {
    async fetchDataSets() {
      let response = await api.dataSet.get()
      this.dataSets = response.data
    },
    async fetchQuota() {
      let response = await api.cluster.getQuota()
      this.quota = response.data
    },
    addParam() {
      this.form.searchSpace.push({ name: '', type: 'float', min: null, max: null, step: null, valuesStr: '' })
    },
    removeParam(index) {
      this.form.searchSpace.splice(index, 1)
    },
    onParamTypeChange(param) {
      if (param.type === 'categorical') {
        param.min = null
        param.max = null
        param.step = null
      } else {
        param.valuesStr = ''
      }
    },
    async submit() {
      try {
        await this.$refs.runForm.validate()
      } catch (e) {
        return
      }

      let searchSpace = this.form.searchSpace.map(p => {
        let param = { name: p.name, type: p.type }
        if (p.type === 'categorical') {
          param.values = p.valuesStr.split(',').map(v => v.trim()).filter(v => v)
        } else {
          param.min = p.min
          param.max = p.max
          param.step = p.step
        }
        return param
      })

      let body = {
        name: this.form.name,
        algorithm: this.form.algorithm,
        maxTrials: this.form.maxTrials,
        objectiveMetric: this.form.objectiveMetric,
        optimizationDirection: this.form.optimizationDirection,
        searchSpace: searchSpace,
        dataSetId: this.form.dataSetId,
        entryPoint: this.form.entryPoint,
        containerImage: this.form.containerImage,
        gitModel: this.form.gitModel,
        cpu: this.form.resource.cpu,
        memory: this.form.resource.memory,
        gpu: this.form.resource.gpu,
        memo: this.form.memo,
      }

      try {
        await api.hpo.post(body)
        this.$notify.success('HPOジョブを開始しました')
        this.$emit('done')
        this.closeDialog()
      } catch (e) {
        this.error = e
      }
    },
    closeDialog() {
      this.dialogVisible = false
      this.$router.push('/hpo')
    },
  },
}
</script>

<style scoped>
.search-space-item {
  margin-bottom: 10px;
  padding: 8px;
  background: #f5f7fa;
  border-radius: 4px;
}
</style>
