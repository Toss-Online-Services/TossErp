/** @type {import('tailwindcss').Config} */
export default {
  content: [
    './components/**/*.{js,vue,ts}',
    './layouts/**/*.vue',
    './pages/**/*.vue',
    './app.vue',
    './app/**/*.{js,ts,vue}',
    './plugins/**/*.{js,ts}',
    './error.vue',
    './content/**/*.{md,mdx}',
    './node_modules/shadcn-vue/dist/**/*.{js,mjs}'
  ],
  darkMode: ['class', 'class'],
  theme: {
  	container: {
  		center: true,
  		padding: '2rem',
  		screens: {
  			'2xl': '1400px'
  		}
  	},
  	extend: {
  		colors: {
  			border: 'hsl(var(--border))',
  			input: 'hsl(var(--input))',
  			ring: 'hsl(var(--ring))',
  			background: 'hsl(var(--background))',
  			foreground: 'hsl(var(--foreground))',
  			primary: {
  				DEFAULT: 'hsl(var(--primary))',
  				foreground: 'hsl(var(--primary-foreground))'
  			},
  			secondary: {
  				DEFAULT: 'hsl(var(--secondary))',
  				foreground: 'hsl(var(--secondary-foreground))'
  			},
  			destructive: {
  				DEFAULT: 'hsl(var(--destructive))',
  				foreground: 'hsl(var(--destructive-foreground))'
  			},
  			muted: {
  				DEFAULT: 'hsl(var(--muted))',
  				foreground: 'hsl(var(--muted-foreground))'
  			},
  			accent: {
  				DEFAULT: 'hsl(var(--accent))',
  				foreground: 'hsl(var(--accent-foreground))'
  			},
  			popover: {
  				DEFAULT: 'hsl(var(--popover))',
  				foreground: 'hsl(var(--popover-foreground))'
  			},
  			card: {
  				DEFAULT: 'hsl(var(--card))',
  				foreground: 'hsl(var(--card-foreground))'
  			},
  			sidebar: {
  				DEFAULT: 'hsl(var(--sidebar-background))',
  				foreground: 'hsl(var(--sidebar-foreground))',
  				accent: 'hsl(var(--sidebar-accent))',
  				border: 'hsl(var(--sidebar-border))',
  				primary: 'hsl(var(--sidebar-primary))',
  				'primary-foreground': 'hsl(var(--sidebar-primary-foreground))',
  				'accent-foreground': 'hsl(var(--sidebar-accent-foreground))',
  				ring: 'hsl(var(--sidebar-ring))'
  			},
  			support: {
  				info: 'hsl(var(--support-info))',
  				warning: 'hsl(var(--support-warning))',
  				success: 'hsl(var(--support-success))'
  			},
  			toss: {
  				primary: 'var(--toss-primary)',
  				secondary: 'var(--toss-secondary)',
  				accent: 'var(--toss-accent)',
  				danger: 'var(--toss-danger)',
  				warning: 'var(--toss-warning)',
  				success: 'var(--toss-success)',
  				info: 'var(--toss-info)'
  			},
  			material: {
  				primary: '#1A73E8',
  				secondary: '#737373',
  				success: '#4CAF50',
  				info: '#1A73E8',
  				warning: '#fb8c00',
  				danger: '#F44335'
  			},
  			brand: {
  				'50': '#eff6ff',
  				'100': '#dbeafe',
  				'200': '#bfdbfe',
  				'300': '#93c5fd',
  				'400': '#60a5fa',
  				'500': '#3b82f6',
  				'600': '#2563eb',
  				'700': '#1d4ed8',
  				'800': '#1e40af',
  				'900': '#1e3a8a'
  			},
  			ui: {
  				'bg-light': '#f8fafc',
  				'bg-dark': '#0f172a',
  				'card-light': '#ffffff',
  				'card-dark': '#1e293b',
  				'text-primary-light': '#0f172a',
  				'text-primary-dark': '#ffffff',
  				'text-secondary-light': '#475569',
  				'text-secondary-dark': '#94a3b8',
  				'border-light': '#e2e8f0',
  				'border-dark': '#374151',
  				'hover-light': '#f1f5f9',
  				'hover-dark': '#334155'
  			},
  			chart: {
  				'1': 'hsl(var(--chart-1))',
  				'2': 'hsl(var(--chart-2))',
  				'3': 'hsl(var(--chart-3))',
  				'4': 'hsl(var(--chart-4))',
  				'5': 'hsl(var(--chart-5))'
  			}
  		},
  		fontFamily: {
  			sans: [
  				'Inter var',
  				'Inter',
  				'system-ui',
  				'-apple-system',
  				'BlinkMacSystemFont',
  				'Segoe UI',
  				'Roboto',
  				'sans-serif'
  			]
  		},
  		spacing: {
  			'18': '4.5rem',
  			'88': '22rem'
  		},
  		borderRadius: {
  			lg: 'var(--radius)',
  			md: 'calc(var(--radius) - 2px)',
  			sm: 'calc(var(--radius) - 4px)',
  			'material-sm': '0.375rem',
  			'material-lg': '0.5rem',
  			'material-xl': '0.75rem',
  			'material-xxl': '2rem'
  		},
  		boxShadow: {
  			soft: '0 20px 60px -25px hsl(var(--shadow-soft) / 0.55)',
  			strong: '0 40px 140px -60px hsl(var(--shadow-strong) / 0.65)',
  			'material': '0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06)',
  			'material-sm': '0 0.3125rem 0.625rem 0 rgba(0, 0, 0, 0.12)',
  			'material-lg': '0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -2px rgba(0, 0, 0, 0.05)',
  			'material-primary': '0 4px 20px 0 rgba(26, 115, 232, 0.14), 0 7px 10px -5px rgba(26, 115, 232, 0.4)',
  			'material-success': '0 4px 20px 0 rgba(76, 175, 80, 0.14), 0 7px 10px -5px rgba(76, 175, 80, 0.4)',
  			'material-info': '0 4px 20px 0 rgba(26, 115, 232, 0.14), 0 7px 10px -5px rgba(26, 115, 232, 0.4)',
  			'material-warning': '0 4px 20px 0 rgba(251, 140, 0, 0.14), 0 7px 10px -5px rgba(251, 140, 0, 0.4)',
  			'material-danger': '0 4px 20px 0 rgba(244, 67, 53, 0.14), 0 7px 10px -5px rgba(244, 67, 53, 0.4)',
  			'material-button': '0 2px 2px 0 rgba(26, 115, 232, 0.1), 0 3px 1px -2px rgba(26, 115, 232, 0.18), 0 1px 5px 0 rgba(26, 115, 232, 0.15)',
  			'material-button-hover': '0 8px 14px -8px rgba(26, 115, 232, 0.3), 0 3px 18px 0 rgba(26, 115, 232, 0.1), 0 7px 8px -4px rgba(26, 115, 232, 0.18)'
  		},
  		backgroundImage: {
  			'toss-grid': 'radial-gradient(circle, rgba(37,99,235,0.12) 1px, transparent 1px)',
  			'toss-radial': 'radial-gradient(circle at top, rgba(59,130,246,0.18), transparent 55%)',
  			'gradient-primary': 'linear-gradient(195deg, #49a3f1 0%, #1A73E8 100%)',
  			'gradient-secondary': 'linear-gradient(195deg, #747b8a 0%, #495361 100%)',
  			'gradient-success': 'linear-gradient(195deg, #66BB6A 0%, #43A047 100%)',
  			'gradient-info': 'linear-gradient(195deg, #49a3f1 0%, #1A73E8 100%)',
  			'gradient-warning': 'linear-gradient(195deg, #FFA726 0%, #FB8C00 100%)',
  			'gradient-danger': 'linear-gradient(195deg, #EF5350 0%, #E53935 100%)',
  			'gradient-dark': 'linear-gradient(195deg, #42424a 0%, #191919 100%)'
  		},
  		animation: {
  			'fade-in': 'fadeIn 0.3s ease-in-out',
  			'slide-in-right': 'slideInRight 0.3s ease-out',
  			'bounce-subtle': 'bounceSubtle 2s infinite',
  			'accordion-down': 'accordion-down 0.2s ease-out',
  			'accordion-up': 'accordion-up 0.2s ease-out'
  		},
  		keyframes: {
  			fadeIn: {
  				'0%': {
  					opacity: '0',
  					transform: 'translateY(10px)'
  				},
  				'100%': {
  					opacity: '1',
  					transform: 'translateY(0)'
  				}
  			},
  			slideInRight: {
  				'0%': {
  					transform: 'translateX(100%)'
  				},
  				'100%': {
  					transform: 'translateX(0)'
  				}
  			},
  			bounceSubtle: {
  				'0%, 100%': {
  					transform: 'translateY(-5%)'
  				},
  				'50%': {
  					transform: 'translateY(0)'
  				}
  			},
  			'accordion-down': {
  				from: {
  					height: '0'
  				},
  				to: {
  					height: 'var(--reka-accordion-content-height)'
  				}
  			},
  			'accordion-up': {
  				from: {
  					height: 'var(--reka-accordion-content-height)'
  				},
  				to: {
  					height: '0'
  				}
  			}
  		},
  		screens: {
  			xs: '475px'
  		}
  	}
  },
  plugins: [
    require('@tailwindcss/forms'),
    require('@tailwindcss/typography'),
    require('@tailwindcss/aspect-ratio'),
    require('tailwindcss-animate')
  ]
}
