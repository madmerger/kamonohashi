import Vue from 'vue'
import VueI18n from 'vue-i18n'
import message from './message/index'

Vue.use(VueI18n)
const savedLocale = localStorage.getItem('kamonohashi-locale') || 'ja'
const i18n = new VueI18n({ locale: savedLocale, messages: message })

export default i18n
