<template>
  <div class="min-h-screen flex items-center justify-center bg-gray-50 dark:bg-gray-900 py-12 px-4 sm:px-6 lg:px-8">
    <div class="max-w-md w-full space-y-8">
      <!-- Header -->
      <div>
        <div class="mx-auto h-12 w-12 flex items-center justify-center rounded-full bg-gradient-to-r from-green-400 to-green-600">
          <svg class="h-8 w-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 10V3L4 14h7v7l9-11h-7z"></path>
          </svg>
        </div>
        <h2 class="mt-6 text-center text-3xl font-extrabold text-gray-900 dark:text-white">
          Sign in to TOSS ERP
        </h2>
        <p class="mt-2 text-center text-sm text-gray-600 dark:text-gray-400">
          Your collaborative business platform
        </p>
      </div>

      <!-- Login Form -->
      <form class="mt-8 space-y-6" @submit.prevent="handleLogin">
        <div class="rounded-md shadow-sm -space-y-px">
          <div>
            <label for="email" class="sr-only">Email address</label>
            <input
              id="email"
              v-model="form.email"
              name="email"
              type="email"
              autocomplete="email"
              required
              class="appearance-none rounded-none relative block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 placeholder-gray-500 dark:placeholder-gray-400 text-gray-900 dark:text-white rounded-t-md focus:outline-none focus:ring-green-500 focus:border-green-500 focus:z-10 sm:text-sm bg-white dark:bg-gray-800"
              placeholder="Email address"
            />
          </div>
          <div>
            <label for="password" class="sr-only">Password</label>
            <input
              id="password"
              v-model="form.password"
              name="password"
              type="password"
              autocomplete="current-password"
              required
              class="appearance-none rounded-none relative block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 placeholder-gray-500 dark:placeholder-gray-400 text-gray-900 dark:text-white rounded-b-md focus:outline-none focus:ring-green-500 focus:border-green-500 focus:z-10 sm:text-sm bg-white dark:bg-gray-800"
              placeholder="Password"
            />
          </div>
        </div>

        <div class="flex items-center justify-between">
          <div class="flex items-center">
            <input
              id="remember-me"
              v-model="form.rememberMe"
              name="remember-me"
              type="checkbox"
              class="h-4 w-4 text-green-600 focus:ring-green-500 border-gray-300 dark:border-gray-600 rounded bg-white dark:bg-gray-800"
            />
            <label for="remember-me" class="ml-2 block text-sm text-gray-900 dark:text-white">
              Remember me
            </label>
          </div>

          <div class="text-sm">
            <NuxtLink to="/forgot-password" class="font-medium text-green-600 hover:text-green-500 dark:text-green-400 dark:hover:text-green-300">
              Forgot your password?
            </NuxtLink>
          </div>
        </div>

        <div>
          <button
            type="submit"
            :disabled="isLoading"
            class="group relative w-full flex justify-center py-2 px-4 border border-transparent text-sm font-medium rounded-md text-white bg-green-600 hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-500 disabled:opacity-50 disabled:cursor-not-allowed"
          >
            <span class="absolute left-0 inset-y-0 flex items-center pl-3">
              <svg v-if="!isLoading" class="h-5 w-5 text-green-500 group-hover:text-green-400" fill="currentColor" viewBox="0 0 20 20">
                <path fill-rule="evenodd" d="M5 9V7a5 5 0 0110 0v2a2 2 0 012 2v5a2 2 0 01-2 2H5a2 2 0 01-2-2v-5a2 2 0 012-2zm8-2v2H7V7a3 3 0 016 0z" clip-rule="evenodd" />
              </svg>
              <svg v-else class="animate-spin h-5 w-5 text-green-500" fill="none" viewBox="0 0 24 24">
                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
              </svg>
            </span>
            {{ isLoading ? 'Signing in...' : 'Sign in' }}
          </button>
        </div>

        <!-- Demo Credentials -->
        <div class="mt-6 p-4 bg-blue-50 dark:bg-blue-900/20 rounded-lg">
          <h3 class="text-sm font-medium text-blue-800 dark:text-blue-200 mb-2">Demo Credentials</h3>
          <div class="space-y-1 text-xs text-blue-600 dark:text-blue-300">
            <p><strong>Business Owner:</strong> owner@demo.toss.co.za / password123</p>
            <p><strong>Manager:</strong> manager@demo.toss.co.za / password123</p>
            <p><strong>Employee:</strong> employee@demo.toss.co.za / password123</p>
          </div>
          <button
            type="button"
            @click="fillDemoCredentials"
            class="mt-2 text-xs text-blue-600 dark:text-blue-400 hover:text-blue-500 dark:hover:text-blue-300 underline"
          >
            Use demo credentials
          </button>
        </div>

        <!-- Sign Up Link -->
        <div class="text-center">
          <p class="text-sm text-gray-600 dark:text-gray-400">
            Don't have an account?
            <NuxtLink to="/register" class="font-medium text-green-600 hover:text-green-500 dark:text-green-400 dark:hover:text-green-300">
              Sign up for free
            </NuxtLink>
          </p>
        </div>
      </form>

      <!-- Features Preview -->
      <div class="mt-8 grid grid-cols-2 gap-4 text-center">
        <div class="p-3 bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
          <div class="text-green-600 dark:text-green-400 mb-1">
            <svg class="w-6 h-6 mx-auto" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z"></path>
            </svg>
          </div>
          <p class="text-xs font-medium text-gray-900 dark:text-white">Group Buying</p>
          <p class="text-xs text-gray-500 dark:text-gray-400">Save together</p>
        </div>

        <div class="p-3 bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
          <div class="text-green-600 dark:text-green-400 mb-1">
            <svg class="w-6 h-6 mx-auto" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.663 17h4.673M12 3v1m6.364 1.636l-.707.707M21 12h-1M4 12H3m3.343-5.657l-.707-.707m2.828 9.9a5 5 0 117.072 0l-.548.547A3.374 3.374 0 0014 18.469V19a2 2 0 11-4 0v-.531c0-.895-.356-1.754-.988-2.386l-.548-.547z"></path>
            </svg>
          </div>
          <p class="text-xs font-medium text-gray-900 dark:text-white">AI Assistant</p>
          <p class="text-xs text-gray-500 dark:text-gray-400">Smart insights</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'

// Meta
definePageMeta({
  layout: false,
  auth: false
})

useHead({
  title: 'Sign In'
})

// Auth composable
const { login, isAuthenticated, isLoading, error: authError } = useAuth()

// State
const form = ref({
  email: '',
  password: '',
  rememberMe: false
})

// Methods
async function handleLogin() {
  if (isLoading.value) return

  try {
    const success = await login(form.value)
    
    if (success) {
      // Show success notification
      const notificationStore = useNotificationStore()
      notificationStore.add({
        type: 'success',
        title: 'Welcome back!',
        message: 'You have been successfully signed in.'
      })
      
      // Redirect to dashboard
      await navigateTo('/dashboard')
    } else {
      // Show error notification
      const notificationStore = useNotificationStore()
      notificationStore.add({
        type: 'error',
        title: 'Sign in failed',
        message: authError.value || 'Invalid email or password. Please try again.'
      })
    }
  } catch (error: any) {
    // Show error notification
    const notificationStore = useNotificationStore()
    notificationStore.add({
      type: 'error',
      title: 'Sign in failed',
      message: error.message || 'Invalid email or password. Please try again.'
    })
  }
}

function fillDemoCredentials() {
  form.value.email = 'owner@demo.toss.co.za'
  form.value.password = 'password123'
  form.value.rememberMe = true
}

// Redirect if already authenticated
onMounted(async () => {
  if (isAuthenticated.value) {
    await navigateTo('/dashboard')
  }
})
</script>

<style scoped>
/* Additional custom styles if needed */
</style>
