import api from '@/api/api'

// initial state
const state = {
  pipelines: [],
  detail: {},
  runs: [],
  selectedRun: null,
}

// getters
const getters = {
  pipelines(state) {
    return state.pipelines
  },
  detail(state) {
    return state.detail
  },
  runs(state) {
    return state.runs
  },
  selectedRun(state) {
    return state.selectedRun
  },
}

// actions
const actions = {
  async fetchPipelines({ commit }) {
    let response = await api.pipeline.get()
    commit('setPipelines', response.data)
  },
  async fetchDetail({ commit }, id) {
    let response = await api.pipeline.getById({ id })
    commit('setDetail', response.data)
  },
  async createPipeline(context, params) {
    await api.pipeline.post({ body: params })
  },
  async updatePipeline(context, params) {
    let id = params.id
    let body = {
      name: params.name,
      memo: params.memo,
      nodes: params.nodes,
      edges: params.edges,
    }
    await api.pipeline.put({ id, body })
  },
  async deletePipeline(context, id) {
    await api.pipeline.delete({ id })
  },
  async executePipelineRun(context, id) {
    let response = await api.pipeline.postRun({ id })
    return response.data
  },
  async fetchRuns({ commit }, id) {
    let response = await api.pipeline.getRuns({ id })
    commit('setRuns', response.data)
  },
  async fetchRunDetail({ commit }, runId) {
    let response = await api.pipeline.getRunById({ runId })
    commit('setSelectedRun', response.data)
  },
}

// mutations
const mutations = {
  setPipelines(state, pipelines) {
    state.pipelines = pipelines
  },
  setDetail(state, detail) {
    state.detail = detail
  },
  setRuns(state, runs) {
    state.runs = runs
  },
  setSelectedRun(state, run) {
    state.selectedRun = run
  },
}

export default {
  namespaced: true,
  state,
  getters,
  actions,
  mutations,
}
