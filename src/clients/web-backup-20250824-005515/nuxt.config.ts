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
        { name: 'description', content: 'TOSS ERP III - Enterprise Resource Planning System with Cooperative Economy Features' },
        { name: 'theme-color', content: '#2563eb' },
        { name: 'author', content: 'TOSS ERP Team' },
        { name: 'keywords', content: 'ERP, business management, cooperative economy, AI copilot, inventory, sales, finance' }
      ],
      link: [
        { rel: 'icon', type: 'image/x-icon', href: '/favicon.ico' },
        { rel: 'apple-touch-icon', href: '/apple-touch-icon.png' }
      ]
    }
  },

  // CSS and styling
  // css: [
  //   './assets/css/main.css'
  // ],

  // Essential modules for TOSS ERP III
  modules: [
    // UI and Design
    '@nuxt/ui',
    '@nuxt/icon',
    '@nuxt/fonts',
    '@nuxtjs/tailwindcss',
    '@nuxtjs/color-mode',
    
    // State Management
    '@pinia/nuxt',
    
    // Utilities and Composables
    '@vueuse/nuxt',
    '@vueuse/motion/nuxt',
    
    // Content Management
    '@nuxt/image',
    '@nuxt/content',
    
    // Internationalization
    '@nuxtjs/i18n',
    
    // Authentication & Security
    '@sidebase/nuxt-auth',
    'nuxt-security',
    
    // SEO and Analytics
    '@nuxtjs/seo',
    'nuxt-gtag'
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

  // Module Configurations
  
  // Nuxt UI Configuration
  ui: {
    colors: {
      primary: 'indigo',
      secondary: 'blue',
      success: 'green',
      info: 'sky',
      warning: 'amber',
      error: 'red',
      neutral: 'slate'
    }
  },

  // Color Mode Configuration
  colorMode: {
    preference: 'system',
    fallback: 'light',
    classSuffix: '',
    storageKey: 'toss-color-mode'
  },

  // Internationalization Configuration
  i18n: {
    locales: [
      { code: 'en', file: 'en.json', name: 'English' },
      { code: 'af', file: 'af.json', name: 'Afrikaans' },
      { code: 'zu', file: 'zu.json', name: 'isiZulu' },
      { code: 'xh', file: 'xh.json', name: 'isiXhosa' }
    ],
    defaultLocale: 'en',
    strategy: 'prefix_except_default',
    langDir: 'locales/',
    lazy: true,
    detectBrowserLanguage: {
      useCookie: true,
      cookieKey: 'toss_i18n_redirected',
      redirectOn: 'root'
    }
  },

  // Pinia Configuration with Persistence
  pinia: {
    storesDirs: ['./stores/**', './custom-folder/stores/**']
  },

  // Content Configuration
  content: {
    documentDriven: false,
    highlight: {
      theme: 'github-light'
    }
  },

  // Image Optimization
  image: {
    quality: 80,
    format: ['webp'],
    screens: {
      xs: 320,
      sm: 640,
      md: 768,
      lg: 1024,
      xl: 1280,
      xxl: 1536
    }
  },

  // Security Configuration
  security: {
    headers: {
      contentSecurityPolicy: {
        'img-src': ["'self'", 'data:', 'https:'],
        'script-src': ["'self'", "'unsafe-inline'", "'unsafe-eval'"]
      }
    }
  },

  // SEO Configuration
  site: {
    url: process.env.NUXT_PUBLIC_SITE_URL || 'http://localhost:3001',
    name: 'TOSS ERP III',
    description: 'Enterprise Resource Planning System with Cooperative Economy Features and AI Business Co-Pilot',
    defaultLocale: 'en'
  },

  // Analytics Configuration
  gtag: {
    id: process.env.NUXT_PUBLIC_GTAG_ID || 'G-XXXXXXXXXX',
    enabled: process.env.NODE_ENV === 'production'
  },

  // Authentication Configuration
  auth: {
    baseURL: process.env.AUTH_ORIGIN,
    provider: {
      type: 'authjs'
    }
  },

  // TypeScript configuration
  typescript: {
    strict: true,
    typeCheck: false  // Temporarily disabled for testing
  },

  // Runtime configuration
  runtimeConfig: {
    // Private keys (only available on server-side)
    authSecret: process.env.NUXT_AUTH_SECRET,
    githubClientSecret: process.env.NUXT_GITHUB_CLIENT_SECRET,
    
    // Public keys (exposed to client-side)
    public: {
      gatewayUrl: process.env.NUXT_PUBLIC_GATEWAY_URL || 'http://localhost:8080',
      authUrl: process.env.NUXT_PUBLIC_AUTH_URL || 'http://localhost:3001/api/auth',
      aiServiceUrl: process.env.NUXT_PUBLIC_AI_SERVICE_URL || 'http://localhost:5000',
      githubClientId: process.env.NUXT_PUBLIC_GITHUB_CLIENT_ID
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
      },
      // Proxy AI service requests
      '/ai-api': {
        target: process.env.NUXT_PUBLIC_AI_SERVICE_URL || 'http://localhost:5000',
        changeOrigin: true,
        secure: false
      }
    }
  },

  // Build optimization
  vite: {
    plugins: [
      // @ts-ignore
      require('vite-tsconfig-paths').default()
    ],
    resolve: {
      alias: {
        '~/assets': './assets',
        '@/assets': './assets'
      }
    },
    optimizeDeps: {
      include: ['chart.js', 'apexcharts', '@headlessui/vue', '@heroicons/vue']
    },
    define: {
      __VUE_PROD_HYDRATION_MISMATCH_DETAILS__: 'false'
    }
  },

  // Experimental features for enhanced performance
  experimental: {
    payloadExtraction: false,
    typedPages: true
  }
})