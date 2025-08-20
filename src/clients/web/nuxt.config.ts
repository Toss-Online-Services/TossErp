// https://nuxt.com/docs/api/configuration/nuxt-config
import { defineNuxtConfig } from 'nuxt/config'

export default defineNuxtConfig({
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
    '~/assets/css/main.css'
  ],

  // Modules
  modules: [
    '@nuxtjs/tailwindcss',
    '@pinia/nuxt'
  ],

  // Auto-imports configuration
  imports: {
    autoImport: true
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
  }
})