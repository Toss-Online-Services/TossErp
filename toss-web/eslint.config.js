import nuxt from '@nuxt/eslint-config'

export default nuxt({
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
    'backup-old-code/**'
  ],
  rules: {
    '@typescript-eslint/no-explicit-any': 'off',
    '@typescript-eslint/no-unused-vars': ['warn', { argsIgnorePattern: '^_', varsIgnorePattern: '^_' }],
    '@typescript-eslint/no-extraneous-class': 'off',
    'vue/html-self-closing': 'off',
    'vue/attributes-order': 'off',
    'vue/first-attribute-linebreak': 'off',
    'vue/v-slot-style': 'off',
    'vue/no-unused-vars': ['warn', { ignorePattern: '^_' }],
    'nuxt/prefer-import-meta': 'off',
    'no-useless-catch': 'off'
  }
})
