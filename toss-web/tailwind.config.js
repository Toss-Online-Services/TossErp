/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./components/**/*.{js,vue,ts}",
    "./layouts/**/*.vue",
    "./pages/**/*.vue",
    "./plugins/**/*.{js,ts}",
    "./app.vue",
    "./error.vue"
  ],
  darkMode: 'class',
  theme: {
    extend: {
      colors: {
        // TOSS ERP Brand Colors - Consistent across all pages
        toss: {
          primary: '#1d4ed8',    // Blue 700 - Primary brand color
          secondary: '#f59e0b',  // Amber 500 - Secondary accent
          accent: '#10b981',     // Emerald 500 - Success/accent
          danger: '#ef4444',     // Red 500 - Error states
          warning: '#f59e0b',    // Amber 500 - Warning states
          success: '#10b981',    // Emerald 500 - Success states
          info: '#3b82f6',       // Blue 500 - Info states
        },
        brand: {
          50: '#eff6ff',
          100: '#dbeafe',
          200: '#bfdbfe',
          300: '#93c5fd',
          400: '#60a5fa',
          500: '#3b82f6',
          600: '#2563eb',
          700: '#1d4ed8',
          800: '#1e40af',
          900: '#1e3a8a',
        },
        // Consistent UI Colors - Used across all components
        ui: {
          // Background colors
          'bg-light': '#f8fafc',      // slate-50
          'bg-dark': '#0f172a',       // slate-900
          'card-light': '#ffffff',    // white
          'card-dark': '#1e293b',     // slate-800
          
          // Text colors
          'text-primary-light': '#0f172a',    // slate-900
          'text-primary-dark': '#ffffff',     // white
          'text-secondary-light': '#475569',  // slate-600
          'text-secondary-dark': '#94a3b8',   // slate-400
          
          // Border colors
          'border-light': '#e2e8f0',  // slate-200
          'border-dark': '#374151',   // slate-700
          
          // Interactive colors
          'hover-light': '#f1f5f9',   // slate-100
          'hover-dark': '#334155',    // slate-700
        }
      },
      fontFamily: {
        sans: ['Inter', 'system-ui', '-apple-system', 'BlinkMacSystemFont', 'Segoe UI', 'Roboto', 'sans-serif'],
      },
      spacing: {
        '18': '4.5rem',
        '88': '22rem',
      },
      animation: {
        'fade-in': 'fadeIn 0.3s ease-in-out',
        'slide-in-right': 'slideInRight 0.3s ease-out',
        'bounce-subtle': 'bounceSubtle 2s infinite',
      },
      keyframes: {
        fadeIn: {
          '0%': { opacity: '0', transform: 'translateY(10px)' },
          '100%': { opacity: '1', transform: 'translateY(0)' },
        },
        slideInRight: {
          '0%': { transform: 'translateX(100%)' },
          '100%': { transform: 'translateX(0)' },
        },
        bounceSubtle: {
          '0%, 100%': { transform: 'translateY(-5%)' },
          '50%': { transform: 'translateY(0)' },
        },
      },
      screens: {
        'xs': '475px',
      },
    },
  },
  plugins: [
    require('@tailwindcss/forms'),
    require('@tailwindcss/typography'),
    require('@tailwindcss/aspect-ratio'),
    function({ addUtilities }) {
      addUtilities({
        '.scrollbar-hide': {
          /* IE and Edge */
          '-ms-overflow-style': 'none',
          /* Firefox */
          'scrollbar-width': 'none',
          /* Safari and Chrome */
          '&::-webkit-scrollbar': {
            display: 'none'
          }
        }
      })
    }
  ],
}
