<script setup lang="ts">
definePageMeta({ layout: 'landing' })
useHead({ title: 'Reset Password | TOSS' })

const apiBase = useRuntimeConfig().public.apiBase || 'http://localhost:5000/api'
const email = ref('')
const loading = ref(false)
const message = ref<string | null>(null)
const errorMessage = ref<string | null>(null)

const handleSubmit = async () => {
  loading.value = true
  message.value = null
  errorMessage.value = null
  try {
    await $fetch(`${apiBase}/v1/auth/password-reset`, {
      method: 'POST',
      body: { email: email.value }
    })
    message.value = 'If that email exists, we sent reset instructions.'
  } catch (err: any) {
    console.error(err)
    errorMessage.value = err?.data?.message || 'Unable to request reset right now.'
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="min-h-screen bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900 text-white">
    <div class="relative isolate px-6 py-12">
      <div class="absolute inset-0 bg-[radial-gradient(circle_at_30%_10%,rgba(59,130,246,0.18),transparent_35%),radial-gradient(circle_at_80%_30%,rgba(16,185,129,0.18),transparent_35%)]" />
      <div class="relative mx-auto max-w-xl">
        <div class="text-center space-y-3 mb-8">
          <p class="inline-flex items-center gap-2 rounded-full bg-white/10 px-3 py-1 text-sm text-blue-200 ring-1 ring-white/10">
            Secure reset
          </p>
          <h1 class="text-3xl font-bold">Reset your password</h1>
          <p class="text-slate-300">We’ll send a link to get you back in. Keep an eye on your inbox or WhatsApp.</p>
        </div>

        <div class="rounded-3xl bg-white/10 backdrop-blur border border-white/10 shadow-2xl shadow-blue-500/20 p-8">
          <form class="space-y-4" @submit.prevent="handleSubmit">
            <div>
              <label class="text-sm text-slate-200">Email</label>
              <input
                v-model="email"
                type="email"
                required
                class="mt-1 w-full rounded-xl bg-white/10 border border-white/10 px-3 py-2.5 text-white placeholder:text-slate-400 focus:outline-none focus:ring-2 focus:ring-blue-400"
                placeholder="you@example.com"
              />
            </div>

            <p v-if="message" class="text-sm text-emerald-200">{{ message }}</p>
            <p v-if="errorMessage" class="text-sm text-rose-200">{{ errorMessage }}</p>

            <button
              type="submit"
              :disabled="loading"
              class="w-full rounded-xl bg-blue-500 hover:bg-blue-400 text-slate-900 font-semibold py-3 shadow-lg shadow-blue-500/30 disabled:opacity-70"
            >
              <span v-if="loading">Sending…</span>
              <span v-else>Send reset link</span>
            </button>
          </form>

          <p class="mt-4 text-sm text-slate-300 text-center">
            Back to
            <NuxtLink to="/signin" class="text-blue-200 hover:text-blue-100">sign in</NuxtLink>
          </p>
        </div>
      </div>
    </div>
  </div>
</template>


