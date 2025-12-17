/** @type {import('tailwindcss').Config} */
export default {
  content: [
    './components/**/*.{js,vue,ts}',
    './layouts/**/*.vue',
    './pages/**/*.vue',
    './plugins/**/*.{js,ts}',
    './app.vue',
    './error.vue',
  ],
  darkMode: 'class',
  theme: {
    extend: {
      colors: {
        background: 'hsl(var(--background))',
        foreground: 'hsl(var(--foreground))',
        ct: {
          primary: 'hsl(var(--ct-primary))',
          secondary: 'hsl(var(--ct-secondary))',
          info: 'hsl(var(--ct-info))',
          success: 'hsl(var(--ct-success))',
          warning: 'hsl(var(--ct-warning))',
          danger: 'hsl(var(--ct-danger))',
          surface: 'hsl(var(--ct-surface))',
          'surface-variant': 'hsl(var(--ct-surface-variant))',
          'on-surface': 'hsl(var(--ct-on-surface))',
          border: 'hsl(var(--ct-border))',
          ring: 'hsl(var(--ct-ring))',
        },
        primary: 'hsl(var(--ct-primary))',
        secondary: 'hsl(var(--ct-secondary))',
        info: 'hsl(var(--ct-info))',
        success: 'hsl(var(--ct-success))',
        warning: 'hsl(var(--ct-warning))',
        danger: 'hsl(var(--ct-danger))',
        surface: 'hsl(var(--ct-surface))',
        muted: 'hsl(var(--ct-surface-variant))',
        border: 'hsl(var(--ct-border))',
        ring: 'hsl(var(--ct-ring))',
      },
      fontFamily: {
        sans: ['Inter', 'system-ui', '-apple-system', 'BlinkMacSystemFont', 'Segoe UI', 'Roboto', 'sans-serif'],
      },
      borderRadius: {
        DEFAULT: 'var(--ct-radius)',
        sm: 'var(--ct-radius-sm)',
        lg: 'var(--ct-radius-lg)',
        xl: 'var(--ct-radius-xl)',
        '2xl': 'var(--ct-radius-2xl)',
      },
      boxShadow: {
        'card': '0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06)',
        'card-hover': '0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -2px rgba(0, 0, 0, 0.05)',
        'elevation-1': '0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06)',
        'elevation-2': '0 0.3125rem 0.625rem 0 rgba(0, 0, 0, 0.12)',
        'elevation-3': '0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -2px rgba(0, 0, 0, 0.05)',
      },
    },
  },
  plugins: [],
}

