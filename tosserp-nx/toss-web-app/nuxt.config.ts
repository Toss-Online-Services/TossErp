import { nxViteTsPaths } from '@nx/vite/plugins/nx-tsconfig-paths.plugin';
import { defineNuxtConfig } from 'nuxt/config';

// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  workspaceDir: '../',
  srcDir: 'src',
  devtools: { enabled: true },
  devServer: {
    host: 'localhost',
    port: 4200,
  },
  typescript: {
    typeCheck: true,
    tsConfig: {
      extends: '../../tsconfig.base.json', // Nuxt copies this string as-is to the `./.nuxt/tsconfig.json`, therefore it needs to be relative to that directory
    },
  },
  imports: {
    autoImport: true,
  },
  css: ['~/assets/css/styles.css'],
  vite: {
    plugins: [nxViteTsPaths()],
  },
  modules: [
    '@vite-pwa/nuxt',
    '@nuxtjs/tailwindcss'
  ],
  runtimeConfig: {
    public: {
      apiBase: process.env.NUXT_PUBLIC_API_BASE || 'http://localhost:5000'
    }
  },
  pwa: {
    registerType: 'autoUpdate',
    manifest: {
      name: 'TOSS ERP-III',
      short_name: 'TOSS',
      description: 'Mobile-first ERP for township and rural SMMEs',
      theme_color: '#e91e63',
      background_color: '#ffffff',
      icons: [
        {
          src: '/favicon.ico',
          sizes: '64x64 32x32 24x24 16x16',
          type: 'image/x-icon'
        },
        {
          src: '/pwa-192x192.png',
          sizes: '192x192',
          type: 'image/png',
          purpose: 'any maskable'
        },
        {
          src: '/pwa-512x512.png',
          sizes: '512x512',
          type: 'image/png',
          purpose: 'any maskable'
        }
      ],
      display: 'standalone',
      orientation: 'portrait',
      start_url: '/',
      scope: '/',
      categories: ['business', 'productivity'],
      shortcuts: [
        {
          name: 'POS',
          short_name: 'POS',
          description: 'Quick access to Point of Sale',
          url: '/sales/pos',
          icons: [{ src: '/favicon.ico', sizes: '96x96' }]
        },
        {
          name: 'Stock',
          short_name: 'Stock',
          description: 'View inventory levels',
          url: '/stock',
          icons: [{ src: '/favicon.ico', sizes: '96x96' }]
        }
      ]
    },
    workbox: {
      navigateFallback: '/pos',
      globPatterns: ['**/*.{js,css,html,png,svg,ico}'],
      runtimeCaching: [
        {
          urlPattern: /^https:\/\/.*\/api\/.*/i,
          handler: 'NetworkFirst',
          options: {
            cacheName: 'api-cache',
            expiration: {
              maxEntries: 50,
              maxAgeSeconds: 5 * 60 // 5 minutes
            },
            networkTimeoutSeconds: 10
          }
        }
      ]
    },
    devOptions: {
      enabled: true,
      type: 'module'
    }
  }
});
