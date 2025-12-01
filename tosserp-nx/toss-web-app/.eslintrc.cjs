module.exports = {
  root: true,
  extends: ['@nuxt/eslint-config'],
  rules: {
    // Allow console in development
    'no-console': process.env.NODE_ENV === 'production' ? 'warn' : 'off',
    // Vue specific
    'vue/multi-word-component-names': 'off',
    'vue/no-multiple-template-root': 'off',
  },
}

