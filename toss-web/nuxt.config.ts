export default defineNuxtConfig({
  devtools: { enabled: true },
  compatibilityDate: '2025-08-24',
  modules: [
    '@nuxtjs/tailwindcss',
    '@nuxtjs/color-mode',
    '@pinia/nuxt',
    '@vueuse/nuxt'
  ],
  css: ['~/assets/css/main.scss'],
  colorMode: {
    classSuffix: ''
  },
  components: {
    global: true,
    dirs: [
      '~/components',
      '~/components/icons'
    ]
  },
  imports: {
    autoImport: true
  },
  app: {
    head: {
      title: 'TOSS ERP III - Township One-Stop Solution',
      meta: [
        { charset: 'utf-8' },
        { name: 'viewport', content: 'width=device-width, initial-scale=1' },
        { name: 'description', content: 'TOSS ERP III - AI-powered collaborative business platform for South African SMMEs' }
      ],
      link: [
        { rel: 'icon', type: 'image/x-icon', href: '/favicon.ico' }
      ]
    }
  },
  runtimeConfig: {
    // Private keys (only available on server-side)
    apiSecret: '',
    // Public keys (exposed to client-side)
    public: {
      apiBase: '/api'
    }
  },
  ssr: true,
  nitro: {
    experimental: {
      wasm: true
    },
    devProxy: {
      '/api/crm': {
        target: 'http://localhost:8081/api/crm',
        changeOrigin: true
      },
      '/api/analytics': {
        target: 'http://localhost:8081/api/analytics',
        changeOrigin: true
      },
      '/api/auth': {
        target: 'http://localhost:8081/api/auth',
        changeOrigin: true
      },
      '/api/hr': {
        target: 'http://localhost:8081/api/hr',
        changeOrigin: true
      },
      '/api/sales': {
        target: 'http://localhost:8081/api/sales',
        changeOrigin: true
      },
      '/api/stock': {
        target: 'http://localhost:8081/api/stock',
        changeOrigin: true
      },
      '/api/inventory': {
        target: 'http://localhost:8081/api/inventory',
        changeOrigin: true
      },
      '/api/financial': {
        target: 'http://localhost:8081/api/financial',
        changeOrigin: true
      },
      '/api/logistics': {
        target: 'http://localhost:8081/api/logistics',
        changeOrigin: true
      },
      '/api/ai': {
        target: 'http://localhost:8081/api/ai',
        changeOrigin: true
      },
      '/api/collaboration': {
        target: 'http://localhost:8081/api/collaboration',
        changeOrigin: true
      },
      '/api/projects': {
        target: 'http://localhost:8081/api/projects',
        changeOrigin: true
      },
      '/api/accounts': {
        target: 'http://localhost:8081/api/accounts',
        changeOrigin: true
      },
      '/api/assets': {
        target: 'http://localhost:8081/api/assets',
        changeOrigin: true
      },
      '/api/setup': {
        target: 'http://localhost:8081/api/setup',
        changeOrigin: true
      },
      '/api/notifications': {
        target: 'http://localhost:8081/api/notifications',
        changeOrigin: true
      },
      '/api/manufacturing': {
        target: 'http://localhost:8081/api/manufacturing',
        changeOrigin: true
      },
      '/api/group-buying': {
        target: 'http://localhost:8081/api/group-buying',
        changeOrigin: true
      },
      '/api/services': {
        target: 'http://localhost:8081/api/services',
        changeOrigin: true
      }
    }
  }
})
