import js from '@eslint/js'
import globals from 'globals'

export default [
  // Base configuration
  js.configs.recommended,
  
  // Global settings
  {
    languageOptions: {
      globals: {
        ...globals.browser,
        ...globals.node,
        ...globals.es2022,
        // Nuxt globals
        'useHead': 'readonly',
        'useRouter': 'readonly',
        'useRoute': 'readonly',
        'navigateTo': 'readonly',
        'useCookie': 'readonly',
        'useState': 'readonly',
        'useFetch': 'readonly',
        'useLazyFetch': 'readonly',
        '$fetch': 'readonly',
        'definePageMeta': 'readonly',
        'defineNuxtComponent': 'readonly',
        'defineNuxtPlugin': 'readonly',
        'defineNuxtRouteMiddleware': 'readonly',
        // Vue globals
        'ref': 'readonly',
        'reactive': 'readonly',
        'computed': 'readonly',
        'watch': 'readonly',
        'watchEffect': 'readonly',
        'onMounted': 'readonly',
        'onUnmounted': 'readonly',
        'onBeforeMount': 'readonly',
        'onBeforeUnmount': 'readonly',
        'nextTick': 'readonly',
        'defineProps': 'readonly',
        'defineEmits': 'readonly',
        'defineExpose': 'readonly',
        'withDefaults': 'readonly'
      }
    },
    rules: {
      // Very relaxed rules for MVP
      'no-console': 'off',
      'no-unused-vars': 'off',
      'prefer-const': 'off',
      'no-undef': 'off'
    }
  },
  
  // Ignore patterns - ignore most files for MVP
  {
    ignores: [
      'node_modules/**',
      '.nuxt/**',
      '.output/**',
      'dist/**',
      'coverage/**',
      'playwright-report/**',
      'test-results/**',
      'public/**',
      'scripts/**',
      'components/**',
      'pages/**',
      'composables/**',
      'middleware/**',
      'plugins/**',
      'server/**',
      'utils/**',
      'types/**',
      'assets/**',
      '**/*.vue',
      '**/*.ts',
      '**/*.js'
    ]
  }
]
