// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  devtools: { enabled: true },
  
  // Modules
  modules: [
    '@nuxtjs/tailwindcss',
    '@pinia/nuxt',
    '@vueuse/nuxt',
    '@nuxtjs/color-mode'
  ],

  // App configuration
  app: {
    head: {
      title: 'TOSS ERP - Stock Management',
      meta: [
        { charset: 'utf-8' },
        { name: 'viewport', content: 'width=device-width, initial-scale=1' },
        { name: 'description', content: 'Enterprise Stock Management System' },
        { name: 'theme-color', content: '#2563eb' }
      ],
      link: [
        { rel: 'icon', type: 'image/x-icon', href: '/favicon.ico' }
      ]
    }
  },

  // CSS
  css: [
    '@/assets/css/main.css'
  ],

  // Tailwind CSS configuration
  tailwindcss: {
    cssPath: '@/assets/css/main.css',
    configPath: 'tailwind.config.ts',
    exposeConfig: false,
    injectPosition: 0,
    viewer: true
  },

  // Color mode configuration
  colorMode: {
    classSuffix: '',
    preference: 'system',
    fallback: 'light'
  },



  // Runtime config
  runtimeConfig: {
    public: {
      apiBase: process.env.API_BASE_URL || 'http://localhost:5000/api',
      appName: 'TOSS ERP Stock Management',
      appVersion: '1.0.0'
    }
  },

  // Build configuration
  build: {
    transpile: ['vue-chartjs', 'chart.js']
  },

  // Nitro configuration
  nitro: {
    preset: 'node-server'
  },

  // Vite configuration
  vite: {
    optimizeDeps: {
      include: ['chart.js', 'vue-chartjs']
    }
  }
})