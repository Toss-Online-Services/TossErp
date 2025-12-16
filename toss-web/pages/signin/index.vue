<script setup lang="ts">
definePageMeta({ layout: 'landing' })
useHead({ title: 'Sign In | TOSS' })

const { login } = useAuthApi()

const email = ref('admin1@toss.local')
const password = ref('Admin123!')
const rememberMe = ref(true)
const loading = ref(false)
const errorMessage = ref<string | null>(null)

const handleSubmit = async () => {
  errorMessage.value = null
  loading.value = true
  try {
    const { error } = await login(email.value, password.value)
    if (error.value) {
      errorMessage.value = 'Sign in failed. Check your email and password.'
      return
    }
    await navigateTo('/')
  } catch (err) {
    console.error(err)
    errorMessage.value = 'Something went wrong. Please try again.'
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div
    class="min-h-screen flex items-center justify-center bg-cover bg-center bg-no-repeat text-white"
    style="background-image: url('https://images.unsplash.com/photo-1500530855697-b586d89ba3ee?auto=format&fit=crop&w=1920&q=80');"
  >
    <div class="bg-white/80 backdrop-blur-md rounded-2xl shadow-2xl max-w-md w-full mx-4">
      <div class="bg-gradient-to-b from-slate-950 via-slate-900 to-slate-800 rounded-t-2xl px-8 pt-8 pb-5 text-center shadow-lg">
        <h1 class="text-2xl font-semibold text-white mb-3">Sign in</h1>
        <div class="flex items-center justify-center gap-4 text-slate-200 mb-1">
          <span class="material-symbols-rounded text-xl">person</span>
          <span class="material-symbols-rounded text-xl">chat</span>
          <span class="material-symbols-rounded text-xl">verified_user</span>
        </div>
      </div>

      <form class="px-8 py-7 space-y-4" @submit.prevent="handleSubmit">
        <div>
          <label class="text-sm text-slate-700">Email</label>
          <input
            v-model="email"
            type="email"
            required
            class="mt-1 w-full rounded-lg border border-slate-300 bg-white px-3 py-2.75 text-slate-900 placeholder:text-slate-400 focus:outline-none focus:ring-2 focus:ring-slate-900"
            placeholder="Email"
          />
        </div>
        <div>
          <label class="text-sm text-slate-700">Password</label>
          <input
            v-model="password"
            type="password"
            required
            class="mt-1 w-full rounded-lg border border-slate-300 bg-white px-3 py-2.75 text-slate-900 placeholder:text-slate-400 focus:outline-none focus:ring-2 focus:ring-slate-900"
            placeholder="Password"
          />
        </div>
        <div class="flex items-center justify-between text-sm text-slate-700">
          <label class="inline-flex items-center gap-2">
            <input v-model="rememberMe" type="checkbox" class="h-4 w-4 rounded border-slate-300 text-slate-900 focus:ring-slate-900" />
            Remember me
          </label>
          <NuxtLink to="/reset" class="text-emerald-600 hover:text-emerald-700 font-medium">Forgot password?</NuxtLink>
        </div>

        <p v-if="errorMessage" class="text-sm text-rose-600">{{ errorMessage }}</p>

        <button
          type="submit"
          :disabled="loading"
          class="w-full rounded-lg bg-slate-900 hover:bg-slate-800 text-white font-semibold py-2.75 shadow-md disabled:opacity-70"
        >
          <span v-if="loading">Signing in…</span>
          <span v-else>Sign in</span>
        </button>
      </form>

      <div class="text-center text-sm text-slate-700 pb-6">
        Don’t have an account?
        <NuxtLink to="/signup" class="text-emerald-600 hover:text-emerald-700 font-semibold">Sign up</NuxtLink>
      </div>
    </div>
  </div>
</template>

