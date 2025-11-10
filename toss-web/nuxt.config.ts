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
  optimization: {
    splitChunks: {
      layouts: true,
      pages: true,
      commons: true
    }
  },
  modules: [
    '@nuxtjs/tailwindcss',
    '@nuxtjs/color-mode',
    '@pinia/nuxt',
    '@vueuse/nuxt',
    '@vite-pwa/nuxt',
    '@sentry/nuxt/module'
  ],
  // Sentry module configuration for automatic source map upload on production build
  // Values are read from environment variables. Provide SENTRY_AUTH_TOKEN only in CI (never commit).
  sentry: {
    org: process.env.SENTRY_ORG || 'your-org-slug',
    project: process.env.SENTRY_PROJECT || 'your-project-slug',
    authToken: process.env.SENTRY_AUTH_TOKEN,
    // Additional tuning could be added here (e.g. deploy, release) once backend release pipeline is in place.
  },
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
    server: {
      watch: {
        usePolling: false,
        useFsEvents: true
      }
    },
    build: {
      rollupOptions: {
        output: {
          manualChunks: {
            'chart': ['chart.js', 'chartjs-adapter-date-fns'],
            'export': ['xlsx', 'jspdf', 'jspdf-autotable', 'html2canvas'],
            'vendor': ['vue', 'vue-router', 'pinia']
          }
        }
      },
      chunkSizeWarningLimit: 1000
    },
    optimizeDeps: {
      include: ['chart.js', 'xlsx', 'jspdf'],
      force: false
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
  },
  pwa: {
    registerType: 'autoUpdate',
    manifest: {
      name: 'TOSS ERP III - Township One-Stop Solution',
      short_name: 'TOSS ERP',
      description: 'AI-powered collaborative business platform for South African SMMEs',
      theme_color: '#1d4ed8',
      background_color: '#0f172a',
      display: 'standalone',
      orientation: 'portrait',
      scope: '/',
      start_url: '/',
      icons: [
        {
          src: '/icons/icon-72x72.png',
          sizes: '72x72',
          type: 'image/png',
          purpose: 'any maskable'
        },
        {
          src: '/icons/icon-96x96.png',
          sizes: '96x96',
          type: 'image/png',
          purpose: 'any maskable'
        },
        {
          src: '/icons/icon-128x128.png',
          sizes: '128x128',
          type: 'image/png',
          purpose: 'any maskable'
        },
        {
          src: '/icons/icon-144x144.png',
          sizes: '144x144',
          type: 'image/png',
          purpose: 'any maskable'
        },
        {
          src: '/icons/icon-152x152.png',
          sizes: '152x152',
          type: 'image/png',
          purpose: 'any maskable'
        },
        {
          src: '/icons/icon-192x192.png',
          sizes: '192x192',
          type: 'image/png',
          purpose: 'any maskable'
        },
        {
          src: '/icons/icon-384x384.png',
          sizes: '384x384',
          type: 'image/png',
          purpose: 'any maskable'
        },
        {
          src: '/icons/icon-512x512.png',
          sizes: '512x512',
          type: 'image/png',
          purpose: 'any maskable'
        }
      ]
    },
    workbox: {
      navigateFallback: '/',
      globPatterns: ['**/*.{js,css,html,png,svg,ico}'],
      runtimeCaching: [
        {
          urlPattern: /^https:\/\/fonts\.googleapis\.com\/.*/i,
          handler: 'CacheFirst',
          options: {
            cacheName: 'google-fonts-cache',
            expiration: {
              maxEntries: 10,
              maxAgeSeconds: 60 * 60 * 24 * 365 // 1 year
            },
            cacheableResponse: {
              statuses: [0, 200]
            }
          }
        },
        {
          urlPattern: /^https:\/\/fonts\.gstatic\.com\/.*/i,
          handler: 'CacheFirst',
          options: {
            cacheName: 'gstatic-fonts-cache',
            expiration: {
              maxEntries: 10,
              maxAgeSeconds: 60 * 60 * 24 * 365 // 1 year
            },
            cacheableResponse: {
              statuses: [0, 200]
            }
          }
        },
        {
          urlPattern: /^https:\/\/.*\.(?:png|jpg|jpeg|svg|gif)$/,
          handler: 'CacheFirst',
          options: {
            cacheName: 'image-cache',
            expiration: {
              maxEntries: 50,
              maxAgeSeconds: 60 * 60 * 24 * 30 // 30 days
            }
          }
        },
        {
          urlPattern: /\/api\/.*/i,
          handler: 'NetworkFirst',
          options: {
            cacheName: 'api-cache',
            expiration: {
              maxEntries: 50,
              maxAgeSeconds: 60 * 5 // 5 minutes
            },
            networkTimeoutSeconds: 10
          }
        }
      ]
    },
    client: {
      installPrompt: true,
      periodicSyncForUpdates: 20
    },
    devOptions: {
      enabled: false,  // Disabled in dev to prevent crashes on Windows
      type: 'module'
    }
  }
})
