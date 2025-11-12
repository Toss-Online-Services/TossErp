import { defineConfig } from 'vitest/config'
import { resolve } from 'path'
import vue from '@vitejs/plugin-vue'
import { defineNuxtConfig } from 'nuxt/config'

export default defineNuxtConfig({
  test: {
    environment: 'nuxt',
    globals: true,
    setupFiles: ['./tests/setup.ts'],
    exclude: [
      // Exclude Playwright E2E tests from unit runs
      '**/*.e2e.*',
      'tests/e2e/**',
      'tests/**/e2e/**',
      'node_modules/**'
    ],
    coverage: {
      provider: 'v8',
      reporter: ['text', 'json', 'html']
    }
  },
  vite: {
    plugins: [vue()],
    resolve: {
      alias: {
        '~': resolve(__dirname, './'),
        '@': resolve(__dirname, './'),
      }
    }
  }
})
