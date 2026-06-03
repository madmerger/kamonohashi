<template>
  <el-dialog
    title="パイプライン編集"
    :visible="true"
    width="80%"
    @close="$router.push('/pipeline')"
  >
    <el-form ref="form" :model="form" :rules="rules" label-width="140px">
      <el-form-item label="パイプライン名" prop="name">
        <el-input v-model="form.name" />
      </el-form-item>
      <el-form-item label="メモ">
        <el-input v-model="form.memo" type="textarea" />
      </el-form-item>

      <el-divider>ノード定義</el-divider>

      <div v-for="(node, index) in form.nodes" :key="index" class="node-row">
        <el-row :gutter="10">
          <el-col :span="6">
            <el-form-item :label="'ノード' + (index + 1) + ' 名前'">
              <el-input v-model="node.name" placeholder="ノード名" />
            </el-form-item>
          </el-col>
          <el-col :span="5">
            <el-form-item label="ジョブ種別">
              <el-select v-model="node.jobType" placeholder="選択">
                <el-option label="前処理" value="Preprocessing" />
                <el-option label="学習" value="Training" />
                <el-option label="推論" value="Inference" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="11">
            <el-form-item label="ジョブパラメータ(JSON)">
              <el-input v-model="node.jobParams" type="textarea" :rows="2" />
            </el-form-item>
          </el-col>
          <el-col :span="2">
            <el-button
              type="danger"
              icon="el-icon-delete"
              circle
              size="mini"
              style="margin-top: 10px;"
              @click="removeNode(index)"
            />
          </el-col>
        </el-row>
      </div>
      <el-button type="text" icon="el-icon-plus" @click="addNode"
        >ノード追加</el-button
      >

      <el-divider>エッジ定義（依存関係）</el-divider>

      <div
        v-for="(edge, index) in form.edges"
        :key="'edge-' + index"
        class="edge-row"
      >
        <el-row :gutter="10">
          <el-col :span="9">
            <el-form-item label="依存元ノード">
              <el-select v-model="edge.sourceNodeIndex" placeholder="選択">
                <el-option
                  v-for="(node, ni) in form.nodes"
                  :key="ni"
                  :label="node.name || 'ノード' + (ni + 1)"
                  :value="ni"
                />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="2" style="text-align: center; line-height: 40px;">
            →
          </el-col>
          <el-col :span="9">
            <el-form-item label="依存先ノード">
              <el-select v-model="edge.targetNodeIndex" placeholder="選択">
                <el-option
                  v-for="(node, ni) in form.nodes"
                  :key="ni"
                  :label="node.name || 'ノード' + (ni + 1)"
                  :value="ni"
                />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="2">
            <el-button
              type="danger"
              icon="el-icon-delete"
              circle
              size="mini"
              style="margin-top: 10px;"
              @click="removeEdge(index)"
            />
          </el-col>
        </el-row>
      </div>
      <el-button type="text" icon="el-icon-plus" @click="addEdge"
        >エッジ追加</el-button
      >
    </el-form>

    <span slot="footer" class="dialog-footer">
      <el-button @click="$router.push('/pipeline')">キャンセル</el-button>
      <el-button type="primary" @click="handleSubmit">更新</el-button>
    </span>
  </el-dialog>
</template>

<script>
import { createNamespacedHelpers } from 'vuex'
const { mapActions, mapGetters } = createNamespacedHelpers('pipeline')

export default {
  props: {
    id: {
      type: [String, Number],
      default: null,
    },
  },
  data() {
    return {
      form: {
        name: '',
        memo: '',
        nodes: [],
        edges: [],
      },
      rules: {
        name: [
          {
            required: true,
            message: 'パイプライン名を入力してください',
            trigger: 'blur',
          },
        ],
      },
    }
  },
  computed: {
    ...mapGetters(['detail']),
  },
  async created() {
    await this.fetchDetail(this.id)
    if (this.detail) {
      this.form.name = this.detail.name
      this.form.memo = this.detail.memo
      this.form.nodes = (this.detail.nodes || []).map(n => ({
        name: n.name,
        jobType: n.jobType,
        jobParams: n.jobParams,
        positionX: n.positionX,
        positionY: n.positionY,
      }))
      // エッジのノードIDをインデックスに変換
      const nodeIds = (this.detail.nodes || []).map(n => n.id)
      this.form.edges = (this.detail.edges || [])
        .map(e => ({
          sourceNodeIndex: nodeIds.indexOf(e.sourceNodeId),
          targetNodeIndex: nodeIds.indexOf(e.targetNodeId),
        }))
        .filter(e => e.sourceNodeIndex >= 0 && e.targetNodeIndex >= 0)
    }
  },
  methods: {
    ...mapActions(['fetchDetail', 'updatePipeline']),
    addNode() {
      this.form.nodes.push({
        name: '',
        jobType: 'Training',
        jobParams: '',
        positionX: 0,
        positionY: 0,
      })
    },
    removeNode(index) {
      this.form.nodes.splice(index, 1)
      this.form.edges = this.form.edges
        .filter(e => e.sourceNodeIndex !== index && e.targetNodeIndex !== index)
        .map(e => ({
          sourceNodeIndex:
            e.sourceNodeIndex > index
              ? e.sourceNodeIndex - 1
              : e.sourceNodeIndex,
          targetNodeIndex:
            e.targetNodeIndex > index
              ? e.targetNodeIndex - 1
              : e.targetNodeIndex,
        }))
    },
    addEdge() {
      this.form.edges.push({ sourceNodeIndex: 0, targetNodeIndex: 0 })
    },
    removeEdge(index) {
      this.form.edges.splice(index, 1)
    },
    async handleSubmit() {
      this.$refs.form.validate(async valid => {
        if (!valid) return
        await this.updatePipeline({ id: this.id, ...this.form })
        this.$emit('done')
        this.$router.push('/pipeline')
      })
    },
  },
}
</script>

<style scoped>
.node-row,
.edge-row {
  background: #f5f7fa;
  border-radius: 4px;
  padding: 8px;
  margin-bottom: 8px;
}
</style>
