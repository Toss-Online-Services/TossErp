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

  // CSS - Temporarily commented out to fix build issue
  // css: [
  //   '@/assets/css/main.css'
  // ],

  // Tailwind CSS configuration
  tailwindcss: {
    // cssPath: '@/assets/css/main.css', // Temporarily commented out
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
    // Private keys (server-side only)
    apiSecret: process.env.API_SECRET,
    
    // Public keys (client-side accessible)
    public: {
      apiBase: process.env.NUXT_PUBLIC_API_URL || 'http://localhost:8080/api',
      gatewayUrl: process.env.NUXT_PUBLIC_GATEWAY_URL || 'http://localhost:8080',
      appName: 'TOSS ERP Stock Management',
      appVersion: '1.0.0',
      environment: process.env.NODE_ENV || 'development',
      enableMockApi: process.env.NUXT_USE_MOCK_API === 'true',
      apiTimeout: parseInt(process.env.API_TIMEOUT || '30000'),
      enableRealTimeUpdates: process.env.ENABLE_REAL_TIME_UPDATES !== 'false',
      enableOfflineSupport: process.env.ENABLE_OFFLINE_SUPPORT !== 'false',
      enableAnalytics: process.env.ENABLE_ANALYTICS !== 'false'
    }
  },

  // Build configuration
  build: {
    transpile: ['vue-chartjs', 'chart.js']
  },

  // Nitro configuration - Use static preset for Docker build
  nitro: {
    preset: 'static',
    prerender: {
      crawlLinks: false,
      routes: ['/']
    },
    devProxy: {
      // Proxy API requests to gateway during development
      '/api': {
        target: process.env.NUXT_PUBLIC_GATEWAY_URL || 'http://localhost:8080',
        changeOrigin: true,
        secure: false
      }
    }
  },
  
  // Disable SSR for now to focus on SPA mode
  ssr: false,

  // Vite configuration
  vite: {
    optimizeDeps: {
      include: ['chart.js', 'vue-chartjs']
    },
    define: {
      // Make environment variables available to client
      __API_BASE_URL__: JSON.stringify(process.env.NUXT_PUBLIC_API_URL || 'http://localhost:8080/api'),
      __GATEWAY_URL__: JSON.stringify(process.env.NUXT_PUBLIC_GATEWAY_URL || 'http://localhost:8080')
    }
  }
})