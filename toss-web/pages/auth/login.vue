<template>
  <div class="min-h-screen flex items-center justify-center bg-gradient-to-br from-slate-50 via-orange-50/30 to-slate-100 dark:from-slate-900 dark:via-slate-900 dark:to-slate-800 px-4 py-12">
    <!-- Decorative background elements -->
    <div class="absolute inset-0 overflow-hidden pointer-events-none">
      <div class="absolute top-20 left-10 w-72 h-72 bg-orange-200/20 dark:bg-orange-500/10 rounded-full blur-3xl"></div>
      <div class="absolute bottom-20 right-10 w-96 h-96 bg-orange-300/20 dark:bg-orange-400/10 rounded-full blur-3xl"></div>
    </div>

    <div class="max-w-md w-full space-y-8 relative z-10">
      <!-- Logo and Title -->
      <div class="text-center">
        <NuxtLink to="/" class="inline-flex justify-center mb-6 group">
          <div class="relative flex items-center justify-center w-16 h-16 bg-gradient-to-br from-orange-500 to-orange-600 dark:from-orange-600 dark:to-orange-700 rounded-2xl shadow-xl transition-transform group-hover:scale-110 group-hover:rotate-3">
            <span class="text-3xl font-black text-white">T</span>
            <div class="absolute inset-0 bg-gradient-to-br from-orange-400 to-orange-500 rounded-2xl opacity-0 group-hover:opacity-20 transition-opacity"></div>
          </div>
        </NuxtLink>
        <h2 class="text-3xl font-bold bg-gradient-to-r from-slate-900 to-slate-700 dark:from-white dark:to-slate-300 bg-clip-text text-transparent">
          Welcome to TOSS
        </h2>
        <p class="mt-2 text-sm text-slate-600 dark:text-slate-400">Sign in to your account to continue</p>
      </div>

      <!-- Login Form -->
      <MaterialCard variant="elevated" class="p-8">
        <form @submit.prevent="handleLogin" class="space-y-6">
          <MaterialInput
            v-model="form.email"
            label="Email or Phone"
            type="text"
            placeholder="Enter your email or phone"
            required
            variant="outlined"
          />

          <MaterialInput
            v-model="form.password"
            label="Password"
            type="password"
            placeholder="Enter your password"
            required
            variant="outlined"
          />

          <div class="flex items-center justify-between">
            <label class="flex items-center cursor-pointer group">
              <UiSwitch v-model="form.remember" />
              <span class="ml-3 text-sm text-slate-600 dark:text-slate-400 group-hover:text-slate-900 dark:group-hover:text-white transition-colors">
                Remember me
              </span>
            </label>
            <NuxtLink to="/auth/forgot-password" class="text-sm text-orange-600 hover:text-orange-700 dark:text-orange-400 dark:hover:text-orange-300 font-medium">
              Forgot password?
            </NuxtLink>
          </div>

          <MaterialButton
            type="submit"
            :disabled="loading"
            :loading="loading"
            color="primary"
            size="lg"
            full-width
          >
            {{ loading ? 'Signing in...' : 'Sign in' }}
          </MaterialButton>
        </form>

        <!-- Demo Mode -->
        <div class="mt-6 pt-6 border-t border-slate-200 dark:border-slate-700">
          <MaterialButton
            @click="handleDemoLogin"
            color="success"
            size="lg"
            full-width
          >
            🚀 Try Demo Mode
          </MaterialButton>
          <p class="mt-2 text-xs text-center text-slate-500 dark:text-slate-400">
            Skip login and explore the system
          </p>
        </div>
      </MaterialCard>

      <!-- Sign Up Link -->
      <p class="text-center text-sm text-slate-600 dark:text-slate-400">
        Don't have an account?
        <NuxtLink to="/auth/register" class="text-orange-600 hover:text-orange-700 dark:text-orange-400 dark:hover:text-orange-300 font-semibold">
          Sign up for free
        </NuxtLink>
      </p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'

definePageMeta({
  layout: false,
  middleware: []
})

useHead({
  title: 'Login - TOSS ERP',
  meta: [
    { name: 'description', content: 'Sign in to TOSS ERP' }
  ]
})

const form = ref({
  email: '',
  password: '',
  remember: false
})

const loading = ref(false)
const { login } = useAuth()

const handleLogin = async () => {
  loading.value = true
  try {
    const success = await login({
      email: form.value.email,
      password: form.value.password,
      rememberMe: form.value.remember
    })
    
    if (success) {
      // Check user role and redirect accordingly
      const { user } = useAuth()
      if (user.value?.roles && user.value.roles.length > 0) {
        const primaryRole = user.value.roles[0].toLowerCase()
        navigateTo(`/${primaryRole}/dashboard`)
      } else {
        navigateTo('/retailer/dashboard')
      }
    } else {
      alert('Login failed. Please check your credentials and try again.')
    }
  } catch (error) {
    console.error('Login failed:', error)
    alert('Login failed. Please try again or use Demo Mode.')
  } finally {
    loading.value = false
  }
}

const handleDemoLogin = async () => {
  loading.value = true
  try {
    // Auto-login with demo credentials
    const success = await login({
      email: 'demo@toss.co.za',
      password: 'demo123',
      rememberMe: false
    })
    
    if (success) {
      const { user } = useAuth()
      if (user.value?.roles && user.value.roles.length > 0) {
        const primaryRole = user.value.roles[0].toLowerCase()
        navigateTo(`/${primaryRole}/dashboard`)
      } else {
        navigateTo('/retailer/dashboard')
      }
    } else {
      // If login fails, just navigate anyway (for development)
      console.log('Demo mode - bypassing auth')
      navigateTo('/retailer/dashboard')
    }
  } catch (error) {
    // If login fails, just navigate anyway (for development)
    console.log('Demo mode - bypassing auth')
    navigateTo('/retailer/dashboard')
  } finally {
    loading.value = false
  }
}
</script>

