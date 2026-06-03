import enlang from 'element-ui/lib/locale/lang/en'
import jalang from 'element-ui/lib/locale/lang/ja'
import eslang from 'element-ui/lib/locale/lang/es'
import jaMessages from './ja'
import enMessages from './en'
import esMessages from './es'

let ja = {
  ...jalang,
  ...jaMessages,
}
ja.el.pagination.total = '合計 {total} 件'

let en = {
  ...enlang,
  ...enMessages,
}

let es = {
  ...eslang,
  ...esMessages,
}

let message = {
  ja,
  en,
  es,
}

export { message as default }
