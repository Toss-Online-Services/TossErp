<script setup lang="ts">
definePageMeta({ layout: 'landing' })
useHead({ title: 'Error | TOSS' })

const route = useRoute()
const status = computed(() => Number(route.query.status || 500))
const message = computed(() => (route.query.message as string) || 'Something went wrong. Try again shortly.')

const tips = [
  'Check your connection and retry.',
  'If you just registered, confirm your email/OTP first.',
  'If the issue persists, try a different browser tab.'
]
</script>

<template>
  <div class="min-h-screen bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900 text-white">
    <div class="relative isolate px-6 py-12">
      <div class="absolute inset-0 bg-[radial-gradient(circle_at_20%_10%,rgba(239,68,68,0.16),transparent_35%),radial-gradient(circle_at_80%_20%,rgba(59,130,246,0.16),transparent_35%)]" />
      <div class="relative mx-auto max-w-3xl text-center space-y-6">
        <p class="inline-flex items-center gap-2 rounded-full bg-white/10 px-3 py-1 text-sm text-rose-200 ring-1 ring-white/10">
          Error {{ status }}
        </p>
        <h1 class="text-4xl font-bold">We hit a bump</h1>
        <p class="text-slate-200">{{ message }}</p>

        <div class="rounded-3xl bg-white/10 backdrop-blur border border-white/10 shadow-2xl shadow-rose-500/20 p-6 space-y-3 text-left">
          <p class="text-sm text-slate-200">Try these quick fixes:</p>
          <ul class="list-disc list-inside text-slate-300 space-y-2">
            <li v-for="tip in tips" :key="tip">{{ tip }}</li>
          </ul>
          <div class="flex flex-wrap gap-3 mt-4">
            <NuxtLink to="/signin" class="px-4 py-2 rounded-xl bg-white/80 text-slate-900 font-semibold hover:bg-white">
              Back to sign in
            </NuxtLink>
            <NuxtLink to="/" class="px-4 py-2 rounded-xl bg-emerald-500 text-slate-900 font-semibold hover:bg-emerald-400">
              Go home
            </NuxtLink>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>


