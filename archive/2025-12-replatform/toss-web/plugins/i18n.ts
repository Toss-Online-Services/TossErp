import { createI18n } from 'vue-i18n'
import en from '~/locales/en.json'
import zu from '~/locales/zu.json'
import xh from '~/locales/xh.json'
import st from '~/locales/st.json'
import tn from '~/locales/tn.json'

export default defineNuxtPlugin(({ vueApp }) => {
  const i18n = createI18n({
    legacy: false,
    globalInjection: true,
    locale: 'en',
    fallbackLocale: 'en',
    messages: {
      en,
      zu,
      xh,
      st,
      tn
    }
  })

  vueApp.use(i18n)
})

