<script setup lang="ts">
definePageMeta({ layout: 'landing' })
useHead({ title: 'Create Account | TOSS' })

const apiBase = useRuntimeConfig().public.apiBase || 'http://localhost:5000/api'

const shopName = ref('Sample Shop')
const area = ref('Soweto')
const address = ref('123 Main Rd')
const firstName = ref('Admin')
const lastName = ref('User')
const phone = ref('+27110000001')
const email = ref('admin1@toss.local')
const password = ref('Admin123!')
const loading = ref(false)
const message = ref<string | null>(null)
const errorMessage = ref<string | null>(null)

const handleSubmit = async () => {
  loading.value = true
  message.value = null
  errorMessage.value = null
  try {
    await $fetch(`${apiBase}/v1/registration/store-owner`, {
      method: 'POST',
      body: {
        shopName: shopName.value,
        area: area.value,
        address: address.value,
        firstName: firstName.value,
        lastName: lastName.value,
        phone: phone.value,
        email: email.value,
        password: password.value,
        whatsappAlerts: true
      }
    })
    message.value = 'Account created. You can sign in now.'
  } catch (err: any) {
    console.error(err)
    errorMessage.value = err?.data?.message || 'Registration failed. Please try again.'
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="min-h-screen bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900 text-white">
    <div class="relative isolate px-6 py-12">
      <div class="absolute inset-0 bg-[radial-gradient(circle_at_10%_30%,rgba(16,185,129,0.15),transparent_35%),radial-gradient(circle_at_90%_10%,rgba(14,165,233,0.18),transparent_35%)]" />
      <div class="relative mx-auto max-w-5xl grid lg:grid-cols-2 gap-10 items-center">
        <div class="space-y-5">
          <p class="inline-flex items-center gap-2 rounded-full bg-white/10 px-3 py-1 text-sm text-cyan-200 ring-1 ring-white/10">
            Built for spazas, tshisanyamas, bakeries
          </p>
          <h1 class="text-4xl font-bold leading-tight">
            Create your shop in minutes.
            <span class="text-cyan-300">Run POS, stock, and money in one place.</span>
          </h1>
          <p class="text-slate-200">
            Simple onboarding: add your shop details and start selling. Works great on low-end Android devices and stays friendly offline.
          </p>
          <div class="flex flex-wrap gap-2 text-sm text-slate-300">
            <span class="rounded-full bg-white/5 px-3 py-1 ring-1 ring-white/10">Offline-friendly POS</span>
            <span class="rounded-full bg-white/5 px-3 py-1 ring-1 ring-white/10">Low-stock alerts</span>
            <span class="rounded-full bg-white/5 px-3 py-1 ring-1 ring-white/10">Cashbook & invoices</span>
          </div>
        </div>

        <div class="rounded-3xl bg-white/10 backdrop-blur border border-white/10 shadow-2xl shadow-cyan-500/20 p-8">
          <div class="flex items-center gap-3 mb-6">
            <span class="w-12 h-12 rounded-2xl bg-cyan-500/20 text-cyan-200 flex items-center justify-center">
              <i class="material-symbols-rounded text-2xl">how_to_reg</i>
            </span>
            <div>
              <p class="text-sm text-slate-200">Shop setup</p>
              <h2 class="text-xl font-semibold">Create account</h2>
            </div>
          </div>

          <form class="space-y-4" @submit.prevent="handleSubmit">
            <div class="grid grid-cols-2 gap-3">
              <div>
                <label class="text-sm text-slate-200">Shop name</label>
                <input
                  v-model="shopName"
                  required
                  class="mt-1 w-full rounded-xl bg-white/10 border border-white/10 px-3 py-2.5 text-white placeholder:text-slate-400 focus:outline-none focus:ring-2 focus:ring-cyan-400"
                />
              </div>
              <div>
                <label class="text-sm text-slate-200">Area</label>
                <input
                  v-model="area"
                  required
                  class="mt-1 w-full rounded-xl bg-white/10 border border-white/10 px-3 py-2.5 text-white placeholder:text-slate-400 focus:outline-none focus:ring-2 focus:ring-cyan-400"
                />
              </div>
            </div>

            <div>
              <label class="text-sm text-slate-200">Address</label>
              <input
                v-model="address"
                required
                class="mt-1 w-full rounded-xl bg-white/10 border border-white/10 px-3 py-2.5 text-white placeholder:text-slate-400 focus:outline-none focus:ring-2 focus:ring-cyan-400"
              />
            </div>

            <div class="grid grid-cols-2 gap-3">
              <div>
                <label class="text-sm text-slate-200">First name</label>
                <input
                  v-model="firstName"
                  required
                  class="mt-1 w-full rounded-xl bg-white/10 border border-white/10 px-3 py-2.5 text-white focus:outline-none focus:ring-2 focus:ring-cyan-400"
                />
              </div>
              <div>
                <label class="text-sm text-slate-200">Last name</label>
                <input
                  v-model="lastName"
                  required
                  class="mt-1 w-full rounded-xl bg-white/10 border border-white/10 px-3 py-2.5 text-white focus:outline-none focus:ring-2 focus:ring-cyan-400"
                />
              </div>
            </div>

            <div>
              <label class="text-sm text-slate-200">Phone</label>
              <input
                v-model="phone"
                required
                class="mt-1 w-full rounded-xl bg-white/10 border border-white/10 px-3 py-2.5 text-white focus:outline-none focus:ring-2 focus:ring-cyan-400"
              />
            </div>

            <div>
              <label class="text-sm text-slate-200">Email</label>
              <input
                v-model="email"
                type="email"
                required
                class="mt-1 w-full rounded-xl bg-white/10 border border-white/10 px-3 py-2.5 text-white placeholder:text-slate-400 focus:outline-none focus:ring-2 focus:ring-cyan-400"
                placeholder="you@example.com"
              />
            </div>

            <div>
              <label class="text-sm text-slate-200">Password</label>
              <input
                v-model="password"
                type="password"
                required
                class="mt-1 w-full rounded-xl bg-white/10 border border-white/10 px-3 py-2.5 text-white placeholder:text-slate-400 focus:outline-none focus:ring-2 focus:ring-cyan-400"
                placeholder="••••••••"
              />
            </div>

            <p v-if="message" class="text-sm text-emerald-200">{{ message }}</p>
            <p v-if="errorMessage" class="text-sm text-rose-200">{{ errorMessage }}</p>

            <button
              type="submit"
              :disabled="loading"
              class="w-full rounded-xl bg-cyan-500 hover:bg-cyan-400 text-slate-900 font-semibold py-3 shadow-lg shadow-cyan-500/30 disabled:opacity-70"
            >
              <span v-if="loading">Creating…</span>
              <span v-else>Create account</span>
            </button>
          </form>

          <p class="mt-4 text-sm text-slate-300">
            Already have an account?
            <NuxtLink to="/signin" class="text-cyan-200 hover:text-cyan-100">Sign in</NuxtLink>
          </p>
        </div>
      </div>
    </div>
  </div>
</template>


