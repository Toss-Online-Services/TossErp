import { fileURLToPath } from 'node:url'

// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  compatibilityDate: '2025-07-15',
  devtools: { enabled: true },
  modules: ['@nuxtjs/tailwindcss', 'shadcn-nuxt'],
  alias: {
    '@': fileURLToPath(new URL('.', import.meta.url)),
    '~': fileURLToPath(new URL('.', import.meta.url)),
  },
  vite: {
    resolve: {
      alias: {
        '@': fileURLToPath(new URL('.', import.meta.url)),
        '~': fileURLToPath(new URL('.', import.meta.url)),
      },
    },
  },
  tailwindcss: {
    cssPath: fileURLToPath(new URL('./assets/css/tailwind.css', import.meta.url)),
  },
  shadcn: {
    /**
     * Prefix for all the imported components
     */
    prefix: '',
    /**
     * Directory where the components reside.
     * @default "./components/ui"
     */
    componentDir: './components/ui'
  }
})