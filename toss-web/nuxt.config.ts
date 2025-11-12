// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  devtools: { enabled: true },
  compatibilityDate: '2025-08-24',
  // Enable hidden client source maps so Sentry can upload them without exposing references
  sourcemap: { client: 'hidden' },
  typescript: {
    strict: false,
    typeCheck: false
  },
  devServer: {
    port: 3001
  },
  experimental: {
    payloadExtraction: true,
    componentIslands: true
  },
  modules: [
    // Essential: shadcn-vue components
    'shadcn-nuxt',
    
    // Essential: Styling
    '@nuxtjs/tailwindcss', 
    '@nuxtjs/color-mode', 
    '@nuxt/icon',
    
    // Essential: State Management
    '@pinia/nuxt',
    '@nuxt/test-utils/module'
  ],
  
  // shadcn-nuxt configuration
  shadcn: {
    /**
     * Prefix for all the imported component
     */
    prefix: '',
    /**
     * Directory that the component lives in.
     * @default "./components/ui"
     */
    componentDir: './components/ui'
  },
  
  // Module Configurations
  
  tailwindcss: {
    cssPath: '~/assets/css/main.css'
  },
  colorMode: {
    preference: 'light',
    fallback: 'light',
    classSuffix: '',
    storageKey: 'nuxt-color-mode'
  },
  components: {
    global: true,
    dirs: [
      '~/components',
      '~/components/icons',
      '~/components/charts'
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
      apiBase: process.env.NUXT_PUBLIC_API_BASE || 'https://localhost:5001',
      apiTimeout: 30000,
      sentry: {
        dsn: process.env.NUXT_PUBLIC_SENTRY_DSN || ''
      }
    }
  },
  ssr: false,  // Disable SSR temporarily to fix router issues
  vite: {
    resolve: {
      alias: {
        // Ensure proper path resolution on Windows - use posix paths
        '#app': '/node_modules/nuxt/dist/app',
        '#head': '/node_modules/nuxt/dist/head/runtime'
      }
    },
    server: {
      watch: {
        usePolling: false,
        useFsEvents: true
      },
      fs: {
        strict: false,
        allow: ['..']
      }
    },
    build: {
      rollupOptions: {
        output: {
          // Sanitize chunk names to prevent Windows path issues
          sanitizeFileName(name: string): string {
            // Remove absolute paths and use only the filename
            const match = name.match(/([^/\\]+)$/);
            return (match && match[1]) || name;
          },
          entryFileNames: '_nuxt/entry.[hash].js',
          chunkFileNames: '_nuxt/[name].[hash].js',
          assetFileNames: '_nuxt/[name].[hash].[ext]',
          manualChunks: {
            'chart': ['chart.js', 'chartjs-adapter-date-fns'],
            'export': ['xlsx', 'jspdf', 'jspdf-autotable', 'html2canvas'],
            'vendor': ['vue', 'vue-router', 'pinia']
          }
        }
      },
      chunkSizeWarningLimit: 1000,
      // Prevent absolute paths in output
      modulePreload: false
    },
    optimizeDeps: {
      include: ['chart.js', 'xlsx', 'jspdf'],
      exclude: []
    }
  },
  nitro: {
    experimental: {
      wasm: true
    },
    devProxy: {
      '/api': {
        target: 'https://localhost:5001',
        changeOrigin: true,
        ws: true,
        secure: false  // Allow self-signed certificates in development
      }
    }
  }
})