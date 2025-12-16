<script setup lang="ts">
definePageMeta({ layout: 'landing' })
useHead({ title: 'Verify Code | TOSS' })

const apiBase = useRuntimeConfig().public.apiBase || 'http://localhost:5000/api'
const email = ref('')
const code = ref('')
const loading = ref(false)
const message = ref<string | null>(null)
const errorMessage = ref<string | null>(null)

const handleSubmit = async () => {
  loading.value = true
  message.value = null
  errorMessage.value = null
  try {
    await $fetch(`${apiBase}/v1/auth/verify-otp`, {
      method: 'POST',
      body: { email: email.value, code: code.value }
    })
    message.value = 'Code verified. You can sign in now.'
  } catch (err: any) {
    console.error(err)
    errorMessage.value = err?.data?.message || 'Invalid or expired code.'
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="min-h-screen bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900 text-white">
    <div class="relative isolate px-6 py-12">
      <div class="absolute inset-0 bg-[radial-gradient(circle_at_15%_20%,rgba(244,114,182,0.2),transparent_35%),radial-gradient(circle_at_85%_15%,rgba(56,189,248,0.18),transparent_35%)]" />
      <div class="relative mx-auto max-w-xl">
        <div class="text-center space-y-3 mb-8">
          <p class="inline-flex items-center gap-2 rounded-full bg-white/10 px-3 py-1 text-sm text-pink-200 ring-1 ring-white/10">
            Quick verification
          </p>
          <h1 class="text-3xl font-bold">Enter the code we sent you</h1>
          <p class="text-slate-300">Check your email or SMS/WhatsApp for the 6-digit code to finish sign-up.</p>
        </div>

        <div class="rounded-3xl bg-white/10 backdrop-blur border border-white/10 shadow-2xl shadow-pink-500/20 p-8">
          <form class="space-y-4" @submit.prevent="handleSubmit">
            <div>
              <label class="text-sm text-slate-200">Email</label>
              <input
                v-model="email"
                type="email"
                required
                class="mt-1 w-full rounded-xl bg-white/10 border border-white/10 px-3 py-2.5 text-white placeholder:text-slate-400 focus:outline-none focus:ring-2 focus:ring-pink-400"
                placeholder="you@example.com"
              />
            </div>
            <div>
              <label class="text-sm text-slate-200">Verification code</label>
              <input
                v-model="code"
                required
                maxlength="6"
                class="mt-1 w-full rounded-xl bg-white/10 border border-white/10 px-3 py-2.5 text-white placeholder:text-slate-400 tracking-widest text-center focus:outline-none focus:ring-2 focus:ring-pink-400"
                placeholder="123456"
              />
            </div>

            <p v-if="message" class="text-sm text-emerald-200">{{ message }}</p>
            <p v-if="errorMessage" class="text-sm text-rose-200">{{ errorMessage }}</p>

            <button
              type="submit"
              :disabled="loading"
              class="w-full rounded-xl bg-pink-500 hover:bg-pink-400 text-slate-900 font-semibold py-3 shadow-lg shadow-pink-500/30 disabled:opacity-70"
            >
              <span v-if="loading">Verifying…</span>
              <span v-else>Verify code</span>
            </button>
          </form>

          <p class="mt-4 text-sm text-slate-300 text-center">
            Didn’t get it?
            <span class="text-pink-200">Resend via email or WhatsApp</span>
          </p>

          <p class="mt-4 text-sm text-slate-300 text-center">
            Back to
            <NuxtLink to="/signin" class="text-pink-200 hover:text-pink-100">sign in</NuxtLink>
          </p>
        </div>
      </div>
    </div>
  </div>
</template>


