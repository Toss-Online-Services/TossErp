<script setup lang="ts">
import { ref } from 'vue'

const form = ref({
  email: '',
  password: '',
  remember: false,
})

const loading = ref(false)
// @ts-ignore -- Nuxt auto-injects useAuth composable
const { login } = useAuth()

// @ts-ignore -- Nuxt auto-injects definePageMeta in setup
definePageMeta({
  layout: false,
  middleware: [],
})

// @ts-ignore -- Nuxt auto-injects useHead composable
useHead({
  title: 'Login - TOSS ERP',
  meta: [{ name: 'description', content: 'Sign in to TOSS ERP' }],
})

const handleLogin = async () => {
  loading.value = true
  try {
    await login({
      email: form.value.email,
      password: form.value.password,
      rememberMe: form.value.remember,
    })
  // @ts-ignore -- Nuxt auto-injects navigateTo helper
  await navigateTo('/dashboard')
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
  await login({ email: 'demo@toss.co.za', password: 'demo123', rememberMe: false })
    // @ts-ignore -- Nuxt auto-injects navigateTo helper
    await navigateTo('/dashboard')
  } catch (error) {
    console.log('Demo mode - bypassing auth')
    // @ts-ignore -- Nuxt auto-injects navigateTo helper
    await navigateTo('/dashboard')
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="flex min-h-screen items-center justify-center bg-slate-50 px-4 dark:bg-slate-900">
    <div class="w-full max-w-md space-y-8">
      <div class="text-center">
        <div class="mb-4 flex justify-center">
          <div class="flex h-16 w-16 items-center justify-center rounded-lg bg-gradient-to-r from-blue-500 to-purple-600">
            <span class="text-3xl font-bold text-white">T</span>
          </div>
        </div>
        <h2 class="text-3xl font-bold text-slate-900 dark:text-white">TOSS ERP</h2>
        <p class="mt-2 text-sm text-slate-600 dark:text-slate-400">Township Operations Support System</p>
      </div>

      <div class="rounded-lg border border-slate-200 bg-white p-8 shadow-lg dark:border-slate-700 dark:bg-slate-800">
        <form class="space-y-6" @submit.prevent="handleLogin">
          <div>
            <label class="mb-2 block text-sm font-medium text-slate-700 dark:text-slate-300" for="email">
              Email or Phone
            </label>
            <input
              id="email"
              v-model="form.email"
              type="text"
              required
              class="w-full rounded-lg border border-slate-300 bg-white px-4 py-3 text-slate-900 placeholder-slate-500 focus:border-transparent focus:ring-2 focus:ring-blue-500 dark:border-slate-600 dark:bg-slate-900 dark:text-white"
              placeholder="Enter your email or phone"
            />
          </div>

          <div>
            <label class="mb-2 block text-sm font-medium text-slate-700 dark:text-slate-300" for="password">
              Password
            </label>
            <input
              id="password"
              v-model="form.password"
              type="password"
              required
              class="w-full rounded-lg border border-slate-300 bg-white px-4 py-3 text-slate-900 placeholder-slate-500 focus:border-transparent focus:ring-2 focus:ring-blue-500 dark:border-slate-600 dark:bg-slate-900 dark:text-white"
              placeholder="Enter your password"
            />
          </div>

          <div class="flex items-center justify-between">
            <label class="flex items-center text-sm text-slate-600 dark:text-slate-400">
              <input
                v-model="form.remember"
                type="checkbox"
                class="h-4 w-4 rounded border-slate-300 text-blue-600 focus:ring-blue-500"
              />
              <span class="ml-2">Remember me</span>
            </label>
            <NuxtLink
              to="/auth/forgot-password"
              class="text-sm text-blue-600 hover:text-blue-700 dark:text-blue-400 dark:hover:text-blue-300"
            >
              Forgot password?
            </NuxtLink>
          </div>

          <Button type="submit" class="w-full py-6 text-base" :disabled="loading">
            {{ loading ? 'Signing in...' : 'Sign in' }}
          </Button>
        </form>

        <div class="mt-6 border-t border-slate-200 pt-6 dark:border-slate-700">
          <Button class="w-full py-6 text-base" variant="secondary" @click="handleDemoLogin">
            🚀 Try Demo Mode
          </Button>
          <p class="mt-2 text-center text-xs text-slate-500 dark:text-slate-400">
            Skip login and explore the system
          </p>
        </div>
      </div>

      <p class="text-center text-sm text-slate-600 dark:text-slate-400">
        Don't have an account?
        <NuxtLink
          to="/auth/register"
          class="font-medium text-blue-600 hover:text-blue-700 dark:text-blue-400 dark:hover:text-blue-300"
        >
          Sign up
        </NuxtLink>
      </p>
    </div>
  </div>
</template>
