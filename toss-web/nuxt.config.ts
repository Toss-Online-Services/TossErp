// https://nuxt.com/docs/api/configuration/nuxt-config

// Build modules array conditionally for better dev performance
const isDev = process.env.NODE_ENV === 'development'
const devApiTarget = process.env.TOSS_API_URL || 'http://localhost:5000'
const modules = [
  // Core & Styling
  '@nuxtjs/tailwindcss',
  '@nuxtjs/color-mode',
  '@nuxt/fonts',
  '@nuxt/icon',
  '@nuxt/image',
  
  // State Management
  '@pinia/nuxt',
  '@vueuse/nuxt',
  
  // PWA & Performance (skip heavy modules in dev)
  ...(isDev ? [] : ['@vite-pwa/nuxt']),
  '@nuxtjs/web-vitals',
  ...(isDev ? [] : ['@nuxtjs/partytown']),
  
  // Internationalization
  '@nuxtjs/i18n',
  
  // Forms & Validation
  '@formkit/nuxt',
  '@vee-validate/nuxt',
  
  // SEO & Analytics (skip in dev for faster startup)
  ...(isDev ? [] : ['@nuxtjs/sitemap', '@nuxtjs/robots', 'nuxt-schema-org', 'nuxt-gtag']),
  
  // Content & Utilities
  '@nuxt/content',
  'nuxt-lodash',
  
  // Device Detection
  '@nuxtjs/device',

  // UI Components
  'nuxt-swiper',
  'shadcn-nuxt',

  // Security & Monitoring (skip heavy Sentry in dev)
  ...(isDev ? [] : ['nuxt-security', '@sentry/nuxt/module'])
]

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
    port: 3001,
    host: '0.0.0.0'  // Listen on all interfaces to avoid connection issues
  },
  experimental: {
    payloadExtraction: true,
    componentIslands: true
  },
  modules,
  
  // Module Configurations
  
  // Internationalization (i18n)
  i18n: {
    locales: [
      { code: 'en', name: 'English', file: 'en.json' },
      { code: 'zu', name: 'isiZulu', file: 'zu.json' },
      { code: 'xh', name: 'isiXhosa', file: 'xh.json' },
      { code: 'af', name: 'Afrikaans', file: 'af.json' },
      { code: 'st', name: 'Sesotho', file: 'st.json' }
    ],
    defaultLocale: 'en',
    strategy: 'no_prefix',
    detectBrowserLanguage: {
      useCookie: true,
      cookieKey: 'i18n_redirected',
      redirectOn: 'root'
    },
    lazy: true,
    langDir: './locales'
  },
  
  // Icon Module
  icon: {
    size: '24px',
    class: 'icon',
    aliases: {
      'nuxt': 'logos:nuxt-icon',
      'cart': 'mdi:cart',
      'user': 'mdi:account',
      'dashboard': 'mdi:view-dashboard',
      'sales': 'mdi:cash-register',
      'inventory': 'mdi:package-variant',
      'analytics': 'mdi:chart-line',
      'settings': 'mdi:cog'
    }
  },
  
  // Image Optimization
  image: {
    quality: 80,
    format: ['webp', 'png', 'jpg'],
    screens: {
      xs: 320,
      sm: 640,
      md: 768,
      lg: 1024,
      xl: 1280,
      xxl: 1536
    },
    providers: {
      local: {
        provider: 'ipx',
        options: {
          modifiers: {
            fit: 'cover',
            format: 'webp'
          }
        }
      }
    }
  },
  
  // Fonts
  fonts: {
    families: [
      { name: 'Inter', provider: 'google', weights: [300, 400, 500, 600, 700] },
      { name: 'Roboto', provider: 'google', weights: [300, 400, 500, 700] }
    ],
    defaults: {
      weights: [400, 700],
      styles: ['normal', 'italic'],
      subsets: ['latin', 'latin-ext']
    }
  },
  
  // FormKit
  formkit: {
    autoImport: true,
    configFile: './formkit.config.ts'
  },
  
  // Device Detection
  device: {
    refreshOnResize: true
  },
  
  // SEO - Sitemap
  sitemap: {
    hostname: process.env.NUXT_PUBLIC_SITE_URL || 'https://toss-erp.com',
    gzip: true,
    routes: async () => {
      return [
        '/',
        '/dashboard',
        '/sales',
        '/inventory',
        '/customers',
        '/suppliers',
        '/reports'
      ]
    }
  },
  
  // Robots.txt
  robots: {
    UserAgent: '*',
    Disallow: ['/api/', '/admin/', '/private/'],
    Sitemap: process.env.NUXT_PUBLIC_SITE_URL ? `${process.env.NUXT_PUBLIC_SITE_URL}/sitemap.xml` : undefined
  },
  
  // Schema.org for SEO
  schemaOrg: {
    host: process.env.NUXT_PUBLIC_SITE_URL || 'https://toss-erp.com',
    identity: {
      type: 'Organization',
      name: 'TOSS ERP III',
      url: process.env.NUXT_PUBLIC_SITE_URL || 'https://toss-erp.com',
      logo: '/logo.png'
    }
  },
  
  // Google Analytics
  gtag: {
    id: process.env.NUXT_PUBLIC_GTAG_ID || '',
    enabled: !!process.env.NUXT_PUBLIC_GTAG_ID
  },
  
  // Web Vitals
  webVitals: {
    provider: 'log',
    debug: false,
    disabled: false
  },
  
  // Security Headers
  security: {
    headers: {
      crossOriginEmbedderPolicy: process.env.NODE_ENV === 'development' ? 'unsafe-none' : 'require-corp',
      contentSecurityPolicy: {
        'base-uri': ["'self'"],
        'font-src': ["'self'", 'https:', 'data:'],
        'form-action': ["'self'"],
        'frame-ancestors': ["'self'"],
        'img-src': ["'self'", 'data:', 'https:'],
        'object-src': ["'none'"],
        'script-src-attr': ["'none'"],
        'style-src': ["'self'", 'https:', "'unsafe-inline'"],
        'upgrade-insecure-requests': true
      }
    },
    rateLimiter: {
      tokensPerInterval: 150,
      interval: 60000,
      fireImmediately: false
    }
  },
  
  // Content Module
  content: {
    documentDriven: false,
    highlight: {
      theme: 'github-dark',
      preload: ['json', 'js', 'ts', 'html', 'css', 'vue']
    },
    markdown: {
      toc: {
        depth: 3,
        searchDepth: 3
      }
    }
  },
  
  // Lodash
  lodash: {
    prefix: '_',
    prefixSkip: false,
    upperAfterPrefix: false
  },
  
  // Swiper
  swiper: {
    modules: ['navigation', 'pagination', 'autoplay', 'effect-fade']
  },
  
  // Sentry module configuration for automatic source map upload on production build
  // Values are read from environment variables. Provide SENTRY_AUTH_TOKEN only in CI (never commit).
  sentry: {
    org: process.env.SENTRY_ORG || 'your-org-slug',
    project: process.env.SENTRY_PROJECT || 'your-project-slug',
    authToken: process.env.SENTRY_AUTH_TOKEN,
    // Disable Sentry in development for better performance
    disabled: process.env.NODE_ENV === 'development',
    // Additional tuning could be added here (e.g. deploy, release) once backend release pipeline is in place.
  },
  shadcn: {
    prefix: '',
    componentDir: './components/ui'
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
      '~/components/charts',
      '~/components/material'
    ]
  },
  imports: {
    autoImport: true
  },
  app: {
    baseURL: '/',
    buildAssetsDir: '/_nuxt/',
    // Performance optimizations
    keepalive: true, // Enable keepalive for better navigation performance
    head: {
      title: 'TOSS ERP III - Township One-Stop Solution',
      meta: [
        { charset: 'utf-8' },
        { name: 'viewport', content: 'width=device-width, initial-scale=1' },
        { name: 'description', content: 'TOSS ERP III - AI-powered collaborative business platform for South African SMMEs' }
      ],
      link: [
        { rel: 'icon', type: 'image/x-icon', href: '/favicon.ico' },
        { rel: 'preconnect', href: 'https://fonts.googleapis.com' },
        { rel: 'preconnect', href: 'https://fonts.gstatic.com', crossorigin: '' }
      ]
    }
  },
  runtimeConfig: {
    // Private keys (only available on server-side)
    apiSecret: '',
    // Public keys (exposed to client-side)
    public: {
      apiBase: process.env.NUXT_PUBLIC_API_BASE || (isDev ? devApiTarget : 'https://localhost:5001'),
      apiTimeout: 30000,
      sentry: {
        dsn: process.env.NUXT_PUBLIC_SENTRY_DSN || ''
      }
    }
  },
  ssr: true,  // Enable SSR for better initial load performance
  vite: {
    server: {
      watch: {
        usePolling: false,
        useFsEvents: true
      },
      fs: {
        strict: false,
        allow: ['..']
      },
      // Improve dev server performance
      hmr: {
        overlay: false // Disable error overlay for faster reloads
      }
    },
    build: {
      rollupOptions: {
        output: {
          manualChunks: (id) => {
            // Vendor chunks
            if (id.includes('node_modules')) {
              if (id.includes('chart.js') || id.includes('chartjs-adapter')) {
                return 'chart'
              }
              if (id.includes('xlsx') || id.includes('jspdf') || id.includes('html2canvas')) {
                return 'export'
              }
              if (id.includes('vue') || id.includes('vue-router') || id.includes('pinia')) {
                return 'vendor'
              }
              if (id.includes('@sentry')) {
                return 'sentry'
              }
              if (id.includes('i18n') || id.includes('vue-i18n')) {
                return 'i18n'
              }
              if (id.includes('formkit') || id.includes('vee-validate')) {
                return 'forms'
              }
              // Large libraries get their own chunk
              if (id.includes('lucide-vue') || id.includes('@heroicons')) {
                return 'icons'
              }
              // Everything else goes to vendor
              return 'vendor'
            }
          }
        }
      },
      chunkSizeWarningLimit: 500, // Reduced from 1000 to catch large chunks earlier
      // Optimize chunking for faster loads
      minify: process.env.NODE_ENV === 'production' ? 'terser' : false,
      terserOptions: {
        compress: {
          drop_console: process.env.NODE_ENV === 'production', // Remove console in production
          drop_debugger: true
        }
      }
    },
    optimizeDeps: {
      include: [
        'chart.js', 
        'chartjs-adapter-date-fns',
        'xlsx', 
        'jspdf', 
        'vue', 
        'vue-router', 
        'pinia',
        '@vueuse/core',
        'date-fns'
      ],
      exclude: ['@sentry/nuxt', '@sentry/vue', '@sentry/browser', '@sentry/core'], // Exclude Sentry from pre-bundling in dev
      force: false,
      // Pre-bundle common dependencies for faster startup
      esbuildOptions: {
        target: 'esnext',
        treeShaking: true
      }
    }
  },
  nitro: {
    experimental: {
      wasm: true
    },
    devProxy: {
      '/api': {
        target: devApiTarget,  // Use HTTP for better compatibility in dev
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
