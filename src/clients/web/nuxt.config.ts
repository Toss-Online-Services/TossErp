// https://nuxt.com/docs/api/configuration/nuxt-config
const defineNuxtConfig = (config: any) => config

export default defineNuxtConfig({
  // Nuxt 4 compatibility
  compatibilityDate: '2025-08-21',
  future: {
    compatibilityVersion: 4,
  },
  
  // Development server configuration
  devServer: {
    port: 3001,
    host: '0.0.0.0'
  },

  // App configuration
  app: {
    head: {
      title: 'TOSS ERP III',
      meta: [
        { charset: 'utf-8' },
        { name: 'viewport', content: 'width=device-width, initial-scale=1' },
        { name: 'description', content: 'TOSS ERP III - Enterprise Resource Planning System' }
      ],
      link: [
        { rel: 'icon', type: 'image/x-icon', href: '/favicon.ico' }
      ]
    }
  },

  // CSS and styling
  css: [
    // '~/assets/css/main.css'  // Temporarily commented out
  ],

  // Modules
  modules: [
    '@nuxtjs/tailwindcss',
    '@pinia/nuxt',
    '@vueuse/nuxt'
  ],

  // Auto-imports configuration (Nuxt 4 enhanced)
  imports: {
    autoImport: true,
    dirs: [
      'composables/**',
      'utils/**',
      'types/**'
    ]
  },

  // Components auto-import (Nuxt 4 enhanced)
  components: [
    {
      path: '~/components',
      pathPrefix: false,
    }
  ],

  // TypeScript configuration
  typescript: {
    strict: true,
    typeCheck: false  // Temporarily disabled for testing
  },

  // Runtime configuration
  runtimeConfig: {
    public: {
      gatewayUrl: process.env.NUXT_PUBLIC_GATEWAY_URL || 'http://localhost:8080'
    }
  },

  // Nitro configuration
  nitro: {
    devProxy: {
      // Proxy API requests to gateway during development
      '/api': {
        target: process.env.NUXT_PUBLIC_GATEWAY_URL || 'http://localhost:8080',
        changeOrigin: true,
        secure: false
      }
    }
  },

  // Build optimization
  vite: {
    optimizeDeps: {
      include: ['chart.js', 'apexcharts']
    }
  }
})