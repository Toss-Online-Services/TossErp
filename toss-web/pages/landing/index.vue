<script setup lang="ts">
// @ts-nocheck
import { ref } from 'vue'
import { useAuthApi } from '~/composables/useApi'

definePageMeta({
  layout: 'landing'
})

useHead({ title: 'Welcome | TOSS POS' })

const { login, token } = useAuthApi()

const loginEmail = ref('admin1@toss.local')
const loginPassword = ref('Admin123!')
const loginLoading = ref(false)
const loginError = ref<string | null>(null)

const regShop = ref('Sample Shop')
const regArea = ref('Soweto')
const regAddress = ref('123 Main Rd')
const regFirst = ref('Admin')
const regLast = ref('User')
const regPhone = ref('+27110000001')
const regEmail = ref('admin1@toss.local')
const regPassword = ref('Admin123!')
const regLoading = ref(false)
const regMessage = ref<string | null>(null)
const regError = ref<string | null>(null)

const apiBase = useRuntimeConfig().public.apiBase || 'http://localhost:5000/api'

async function handleLogin() {
  loginError.value = null
  loginLoading.value = true
  try {
    const { error } = await login(loginEmail.value, loginPassword.value)
    if (error.value) {
      loginError.value = 'Login failed. Please check credentials.'
      return
    }
    await navigateTo('/pos')
  } catch (err) {
    loginError.value = 'Login failed. Please try again.'
    console.error(err)
  } finally {
    loginLoading.value = false
  }
}

async function handleRegister() {
  regError.value = null
  regMessage.value = null
  regLoading.value = true
  try {
    await $fetch(`${apiBase}/v1/registration/store-owner`, {
      method: 'POST',
      body: {
        shopName: regShop.value,
        area: regArea.value,
        address: regAddress.value,
        firstName: regFirst.value,
        lastName: regLast.value,
        phone: regPhone.value,
        email: regEmail.value,
        password: regPassword.value,
        whatsappAlerts: true
      }
    })
    regMessage.value = 'Account created. You can log in now.'
  } catch (err: any) {
    regError.value = err?.data?.message || 'Registration failed. Please try again.'
    console.error(err)
  } finally {
    regLoading.value = false
  }
}
</script>

<template>
  <div class="min-h-screen bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900 text-white">
    <div class="relative isolate overflow-hidden">
      <div class="absolute inset-0 bg-[radial-gradient(circle_at_top_left,rgba(147,197,253,0.25),transparent_45%),radial-gradient(circle_at_bottom_right,rgba(94,234,212,0.25),transparent_45%)]" />
      <header class="px-6 lg:px-12 py-8 flex items-center justify-between">
        <div class="flex items-center gap-3">
          <span class="w-10 h-10 rounded-xl bg-white/10 backdrop-blur border border-white/10 flex items-center justify-center text-lg font-bold">T</span>
          <div>
            <p class="text-sm text-slate-200">TOSS ERP-III</p>
            <p class="text-xs text-slate-400">POS • Stock • Money • People</p>
          </div>
        </div>
        <div class="hidden md:flex items-center gap-6 text-sm text-slate-200">
          <a href="#login" class="hover:text-white">Login</a>
          <a href="#register" class="hover:text-white">Register</a>
          <NuxtLink to="/pos" class="px-4 py-2 rounded-lg bg-emerald-500 hover:bg-emerald-400 text-slate-900 font-semibold shadow-lg shadow-emerald-500/30">
            Go to POS
          </NuxtLink>
        </div>
      </header>

      <main class="px-6 lg:px-12 pb-16">
        <section class="grid lg:grid-cols-2 gap-10 items-center py-6">
          <div class="space-y-6">
            <p class="inline-flex items-center gap-2 rounded-full bg-white/10 px-3 py-1 text-sm text-emerald-200 ring-1 ring-white/10">
              Inspired by Material Dashboard Pro
            </p>
            <h1 class="text-3xl lg:text-4xl font-bold leading-tight text-white">
              Sell faster with a mobile-first POS, built for township & rural shops.
            </h1>
            <p class="text-slate-200 text-base">
              Log in or create your shop to start ringing up sales, track low stock, and keep money in check. Works great on low-end Android devices.
            </p>
            <div class="flex flex-wrap gap-3 text-sm text-slate-200">
              <span class="inline-flex items-center gap-2 rounded-full bg-white/10 px-3 py-1 ring-1 ring-white/10">
                <i class="material-symbols-rounded text-lg">point_of_sale</i> POS + Offline-first
              </span>
              <span class="inline-flex items-center gap-2 rounded-full bg-white/10 px-3 py-1 ring-1 ring-white/10">
                <i class="material-symbols-rounded text-lg">inventory_2</i> Stock & Alerts
              </span>
              <span class="inline-flex items-center gap-2 rounded-full bg-white/10 px-3 py-1 ring-1 ring-white/10">
                <i class="material-symbols-rounded text-lg">account_balance_wallet</i> Money view
              </span>
            </div>
            <div class="flex gap-3">
              <NuxtLink to="/pos" class="px-5 py-3 rounded-xl bg-emerald-500 hover:bg-emerald-400 text-slate-900 font-semibold shadow-lg shadow-emerald-500/30">
                Launch POS
              </NuxtLink>
              <NuxtLink to="/copilot" class="px-5 py-3 rounded-xl bg-white/10 hover:bg-white/20 text-white font-semibold border border-white/10">
                Try AI Copilot
              </NuxtLink>
            </div>
          </div>

          <div class="grid md:grid-cols-2 gap-6" id="login">
            <div class="rounded-2xl bg-white/10 backdrop-blur border border-white/10 p-6 shadow-2xl shadow-emerald-500/20">
              <div class="flex items-center gap-2 mb-4">
                <i class="material-symbols-rounded text-emerald-300">login</i>
                <h2 class="text-xl font-semibold text-white">Login</h2>
              </div>
              <form class="space-y-4" @submit.prevent="handleLogin">
                <div>
                  <label class="text-sm text-slate-200">Email</label>
                  <input v-model="loginEmail" type="email" required class="mt-1 w-full rounded-lg bg-white/10 border border-white/10 px-3 py-2 text-white placeholder:text-slate-400 focus:outline-none focus:ring-2 focus:ring-emerald-400" />
                </div>
                <div>
                  <label class="text-sm text-slate-200">Password</label>
                  <input v-model="loginPassword" type="password" required class="mt-1 w-full rounded-lg bg-white/10 border border-white/10 px-3 py-2 text-white placeholder:text-slate-400 focus:outline-none focus:ring-2 focus:ring-emerald-400" />
                </div>
                <p v-if="loginError" class="text-sm text-rose-200">{{ loginError }}</p>
                <button type="submit" :disabled="loginLoading" class="w-full rounded-lg bg-emerald-500 hover:bg-emerald-400 text-slate-900 font-semibold py-2.5 shadow-lg shadow-emerald-500/30 disabled:opacity-70">
                  <span v-if="loginLoading">Signing in...</span>
                  <span v-else>Sign in</span>
                </button>
                <p class="text-xs text-slate-300">
                  Use the seeded demo account or the one you create below. Successful login redirects to POS.
                </p>
              </form>
            </div>

            <div class="rounded-2xl bg-white/5 backdrop-blur border border-white/10 p-6 shadow-xl" id="register">
              <div class="flex items-center gap-2 mb-4">
                <i class="material-symbols-rounded text-cyan-300">how_to_reg</i>
                <h2 class="text-xl font-semibold text-white">Register your shop</h2>
              </div>
              <form class="space-y-3" @submit.prevent="handleRegister">
                <div class="grid grid-cols-2 gap-3">
                  <div>
                    <label class="text-sm text-slate-200">Shop Name</label>
                    <input v-model="regShop" required class="mt-1 w-full rounded-lg bg-white/10 border border-white/10 px-3 py-2 text-white placeholder:text-slate-400 focus:outline-none focus:ring-2 focus:ring-cyan-400" />
                  </div>
                  <div>
                    <label class="text-sm text-slate-200">Area</label>
                    <input v-model="regArea" required class="mt-1 w-full rounded-lg bg-white/10 border border-white/10 px-3 py-2 text-white placeholder:text-slate-400 focus:outline-none focus:ring-2 focus:ring-cyan-400" />
                  </div>
                </div>
                <div>
                  <label class="text-sm text-slate-200">Address</label>
                  <input v-model="regAddress" required class="mt-1 w-full rounded-lg bg-white/10 border border-white/10 px-3 py-2 text-white placeholder:text-slate-400 focus:outline-none focus:ring-2 focus:ring-cyan-400" />
                </div>
                <div class="grid grid-cols-2 gap-3">
                  <div>
                    <label class="text-sm text-slate-200">First Name</label>
                    <input v-model="regFirst" required class="mt-1 w-full rounded-lg bg-white/10 border border-white/10 px-3 py-2 text-white focus:outline-none focus:ring-2 focus:ring-cyan-400" />
                  </div>
                  <div>
                    <label class="text-sm text-slate-200">Last Name</label>
                    <input v-model="regLast" required class="mt-1 w-full rounded-lg bg-white/10 border border-white/10 px-3 py-2 text-white focus:outline-none focus:ring-2 focus:ring-cyan-400" />
                  </div>
                </div>
                <div>
                  <label class="text-sm text-slate-200">Phone</label>
                  <input v-model="regPhone" required class="mt-1 w-full rounded-lg bg-white/10 border border-white/10 px-3 py-2 text-white focus:outline-none focus:ring-2 focus:ring-cyan-400" />
                </div>
                <div>
                  <label class="text-sm text-slate-200">Email</label>
                  <input v-model="regEmail" type="email" required class="mt-1 w-full rounded-lg bg-white/10 border border-white/10 px-3 py-2 text-white focus:outline-none focus:ring-2 focus:ring-cyan-400" />
                </div>
                <div>
                  <label class="text-sm text-slate-200">Password</label>
                  <input v-model="regPassword" type="password" required class="mt-1 w-full rounded-lg bg-white/10 border border-white/10 px-3 py-2 text-white focus:outline-none focus:ring-2 focus:ring-cyan-400" />
                </div>
                <p v-if="regMessage" class="text-sm text-emerald-200">{{ regMessage }}</p>
                <p v-if="regError" class="text-sm text-rose-200">{{ regError }}</p>
                <button type="submit" :disabled="regLoading" class="w-full rounded-lg bg-cyan-500 hover:bg-cyan-400 text-slate-900 font-semibold py-2.5 shadow-lg shadow-cyan-500/30 disabled:opacity-70">
                  <span v-if="regLoading">Creating...</span>
                  <span v-else>Create account</span>
                </button>
              </form>
            </div>
          </div>
        </section>
      </main>
    </div>
  </div>
</template>

