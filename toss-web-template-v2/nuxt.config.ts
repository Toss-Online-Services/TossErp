// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  compatibilityDate: '2024-11-01',
  devtools: { enabled: true },
  
  modules: [
    '@nuxtjs/tailwindcss',
    '@pinia/nuxt',
    '@nuxt/icon',
    '@vueuse/nuxt'
  ],

  app: {
    head: {
      title: 'Material Dashboard 3 PRO - Nuxt',
      htmlAttrs: {
        lang: 'en'
      },
      meta: [
        { charset: 'utf-8' },
        { name: 'viewport', content: 'width=device-width, initial-scale=1' },
        { name: 'description', content: 'Material Dashboard 3 PRO React converted to Nuxt 4' }
      ],
      link: [
        { rel: 'icon', type: 'image/x-icon', href: '/favicon.ico' },
        { rel: 'stylesheet', href: 'https://fonts.googleapis.com/css?family=Roboto:300,400,500,700,900|Roboto+Slab:400,700' },
        { rel: 'stylesheet', href: 'https://fonts.googleapis.com/css2?family=Material+Icons' },
        { rel: 'stylesheet', href: 'https://use.fontawesome.com/releases/v5.15.4/css/all.css' }
      ]
    },
    pageTransition: { name: 'page', mode: 'out-in' }
  },

  vite: {
    optimizeDeps: {
      include: ['chart.js', 'vue-chartjs']
    }
  }
})
