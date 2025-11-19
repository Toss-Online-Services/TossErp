<template>
  <div class="min-h-screen flex items-center justify-center bg-slate-50 dark:bg-slate-900 px-4">
    <div class="max-w-md w-full space-y-8">
      <!-- Logo and Title -->
      <div class="text-center">
        <div class="flex justify-center mb-4">
          <div class="w-16 h-16 bg-gradient-to-r from-blue-500 to-purple-600 rounded-lg flex items-center justify-center">
            <span class="text-3xl font-bold text-white">T</span>
          </div>
        </div>
        <h2 class="text-3xl font-bold text-slate-900 dark:text-white">TOSS ERP</h2>
        <p class="mt-2 text-sm text-slate-600 dark:text-slate-400">Township Operations Support System</p>
      </div>

      <!-- Login Form -->
      <div class="bg-white dark:bg-slate-800 rounded-lg shadow-lg border border-slate-200 dark:border-slate-700 p-8">
        <form @submit.prevent="handleLogin" class="space-y-6">
          <div>
            <label for="email" class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
              Email or Phone
            </label>
            <input
              id="email"
              v-model="form.email"
              type="text"
              required
              class="w-full px-4 py-3 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-slate-900 text-slate-900 dark:text-white"
              placeholder="Enter your email or phone"
            />
          </div>

          <div>
            <label for="password" class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
              Password
            </label>
            <input
              id="password"
              v-model="form.password"
              type="password"
              required
              class="w-full px-4 py-3 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-slate-900 text-slate-900 dark:text-white"
              placeholder="Enter your password"
            />
          </div>

          <div class="flex items-center justify-between">
            <label class="flex items-center">
              <input
                v-model="form.remember"
                type="checkbox"
                class="w-4 h-4 text-blue-600 border-slate-300 rounded focus:ring-blue-500"
              />
              <span class="ml-2 text-sm text-slate-600 dark:text-slate-400">Remember me</span>
            </label>
            <a href="#" class="text-sm text-blue-600 hover:text-blue-700 dark:text-blue-400 dark:hover:text-blue-300">
              Forgot password?
            </a>
          </div>

          <button
            type="submit"
            :disabled="loading"
            class="w-full py-3 px-4 bg-blue-600 hover:bg-blue-700 text-white font-medium rounded-lg transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
          >
            {{ loading ? 'Signing in...' : 'Sign in' }}
          </button>
        </form>

        <!-- Demo Mode -->
        <div class="mt-6 pt-6 border-t border-slate-200 dark:border-slate-700">
          <button
            @click="handleDemoLogin"
            class="w-full py-3 px-4 bg-green-600 hover:bg-green-700 text-white font-medium rounded-lg transition-colors"
          >
            🚀 Try Demo Mode
          </button>
          <p class="mt-2 text-xs text-center text-slate-500 dark:text-slate-400">
            Skip login and explore the system
          </p>
        </div>
      </div>

      <!-- Sign Up Link -->
      <p class="text-center text-sm text-slate-600 dark:text-slate-400">
        Don't have an account?
        <NuxtLink to="/auth/register" class="text-blue-600 hover:text-blue-700 dark:text-blue-400 dark:hover:text-blue-300 font-medium">
          Sign up
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

