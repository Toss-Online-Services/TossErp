<script setup lang="ts">
definePageMeta({ layout: 'landing' })
useHead({ title: 'Account Locked | TOSS' })

const userName = ref('Admin User')
const email = ref('admin1@toss.local')
const password = ref('')
const loading = ref(false)
const errorMessage = ref<string | null>(null)

const { login } = useAuthApi()

const handleUnlock = async () => {
  loading.value = true
  errorMessage.value = null
  try {
    const { error } = await login(email.value, password.value)
    if (error.value) {
      errorMessage.value = 'Unlock failed. Check your password.'
      return
    }
    await navigateTo('/')
  } catch (err) {
    console.error(err)
    errorMessage.value = 'Unable to unlock right now.'
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="min-h-screen bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900 text-white">
    <div class="relative isolate px-6 py-12">
      <div class="absolute inset-0 bg-[radial-gradient(circle_at_20%_20%,rgba(255,255,255,0.12),transparent_35%),radial-gradient(circle_at_80%_20%,rgba(16,185,129,0.18),transparent_35%)]" />
      <div class="relative mx-auto max-w-xl">
        <div class="text-center space-y-3 mb-8">
          <p class="inline-flex items-center gap-2 rounded-full bg-white/10 px-3 py-1 text-sm text-emerald-200 ring-1 ring-white/10">
            Session locked for safety
          </p>
          <h1 class="text-3xl font-bold">Welcome back, {{ userName }}</h1>
          <p class="text-slate-300">Enter your password to unlock this device.</p>
        </div>

        <div class="rounded-3xl bg-white/10 backdrop-blur border border-white/10 shadow-2xl shadow-emerald-500/20 p-8">
          <div class="flex items-center gap-3 mb-6">
            <span class="w-14 h-14 rounded-2xl bg-emerald-500/20 text-emerald-200 flex items-center justify-center text-2xl font-semibold">
              {{ userName.charAt(0).toUpperCase() }}
            </span>
            <div>
              <p class="text-lg font-semibold text-white">{{ userName }}</p>
              <p class="text-sm text-slate-300">{{ email }}</p>
            </div>
          </div>

          <form class="space-y-4" @submit.prevent="handleUnlock">
            <div>
              <label class="text-sm text-slate-200">Password</label>
              <input
                v-model="password"
                type="password"
                required
                class="mt-1 w-full rounded-xl bg-white/10 border border-white/10 px-3 py-2.5 text-white placeholder:text-slate-400 focus:outline-none focus:ring-2 focus:ring-emerald-400"
                placeholder="••••••••"
              />
            </div>

            <p v-if="errorMessage" class="text-sm text-rose-200">{{ errorMessage }}</p>

            <button
              type="submit"
              :disabled="loading"
              class="w-full rounded-xl bg-emerald-500 hover:bg-emerald-400 text-slate-900 font-semibold py-3 shadow-lg shadow-emerald-500/30 disabled:opacity-70"
            >
              <span v-if="loading">Unlocking…</span>
              <span v-else>Unlock</span>
            </button>
          </form>

          <p class="mt-4 text-sm text-slate-300 text-center">
            Not you?
            <NuxtLink to="/signin" class="text-emerald-200 hover:text-emerald-100">Sign in with another account</NuxtLink>
          </p>
        </div>
      </div>
    </div>
  </div>
</template>


