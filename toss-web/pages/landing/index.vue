<script setup lang="ts">
import { ref } from 'vue'
import { useAuthApi } from '~/composables/useApi'

definePageMeta({
  layout: 'landing'
})

useHead({ title: 'Welcome | TOSS POS' })

const { login } = useAuthApi()

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
    await login(loginEmail.value, loginPassword.value)
    await navigateTo('/pos')
  } catch (err: any) {
    loginError.value = err?.message || 'Login failed. Please try again.'
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
  <div class="min-h-screen relative text-white flex flex-col">
    <div
      class="absolute inset-0 bg-cover bg-center"
      style="background-image: url('https://images.unsplash.com/photo-1578926375605-eaf7559b1458?auto=format&fit=crop&w=1920&q=80');"
    />
    <div class="absolute inset-0 bg-gradient-to-b from-slate-950/82 via-slate-950/78 to-slate-950/86" />

    <!-- Navbar -->
    <header class="relative z-20">
      <div class="mx-auto max-w-6xl px-4">
        <div class="mt-6 flex items-center justify-between rounded-2xl bg-white/14 backdrop-blur-lg border border-white/22 px-5 py-3.5 shadow-lg shadow-black/35">
          <div class="flex items-center gap-3">
            <span class="w-10 h-10 rounded-xl bg-white/20 border border-white/30 flex items-center justify-center text-lg font-bold">T</span>
            <div>
              <p class="text-sm text-white font-semibold">TOSS ERP-III</p>
              <p class="text-xs text-slate-100">POS • Stock • Money • People</p>
            </div>
          </div>
          <div class="hidden md:flex items-center gap-3 text-sm text-white font-medium">
            <NuxtLink to="/signin" class="px-3 py-2 rounded-lg hover:bg-white/18 transition">Sign in</NuxtLink>
            <NuxtLink to="/signup" class="px-3 py-2 rounded-lg hover:bg-white/18 transition">Sign up</NuxtLink>
            <NuxtLink to="/pos" class="px-4 py-2 rounded-lg bg-emerald-500 text-slate-900 font-semibold shadow-lg shadow-emerald-500/45 hover:bg-emerald-400 transition">
              Launch POS
            </NuxtLink>
          </div>
        </div>
      </div>
    </header>

    <!-- Hero -->
    <main class="relative z-10 flex-1 flex items-center">
      <div class="mx-auto max-w-6xl w-full px-4 py-10 lg:py-14">
        <div class="grid lg:grid-cols-2 gap-12 items-center">
          <div class="space-y-6">
            <p class="inline-flex items-center gap-2 rounded-full bg-white/14 px-3 py-1 text-sm text-emerald-200 ring-1 ring-white/18">
              Built for township & rural shops
            </p>
            <h1 class="text-4xl lg:text-5xl font-black leading-tight text-white">
              Sell faster. Keep stock tight. See your money clearly.
            </h1>
            <p class="text-slate-100 text-base leading-relaxed">
              Mobile-first POS, stock, and money view designed for spazas, tshisanyamas, bakeries, and butchers. Works great on low-end Android and stays friendly offline.
            </p>
            <div class="flex flex-wrap gap-2 text-sm text-slate-100">
              <span class="rounded-full bg-white/15 px-3 py-1 ring-1 ring-white/20">Offline-friendly POS</span>
              <span class="rounded-full bg-white/15 px-3 py-1 ring-1 ring-white/20">Low-stock alerts</span>
              <span class="rounded-full bg-white/15 px-3 py-1 ring-1 ring-white/20">Cashbook & invoices</span>
            </div>
            <div class="flex flex-wrap gap-3">
              <NuxtLink to="/signup" class="px-5 py-3 rounded-xl bg-white text-slate-900 font-semibold shadow-lg hover:bg-slate-100 transition">
                Create your shop
              </NuxtLink>
              <NuxtLink to="/copilot" class="px-5 py-3 rounded-xl bg-white/15 border border-white/25 text-white font-semibold hover:bg-white/20 transition">
                Try AI Copilot
              </NuxtLink>
            </div>
          </div>

          <div class="grid gap-5" id="login">
            <div class="rounded-2xl bg-white text-slate-900 border border-white/60 shadow-2xl backdrop-blur-md p-6 lg:p-7">
              <div class="flex items-center gap-2 mb-4">
                <i class="material-symbols-rounded text-emerald-600">login</i>
                <h2 class="text-xl font-semibold text-slate-900">Sign in</h2>
              </div>
              <form class="space-y-4" @submit.prevent="handleLogin">
                <div>
                  <label class="text-sm text-slate-700">Email</label>
                  <input v-model="loginEmail" type="email" required class="mt-1 w-full rounded-lg border border-slate-200 bg-white px-3 py-2.5 text-slate-900 placeholder:text-slate-400 focus:outline-none focus:ring-2 focus:ring-emerald-500" />
                </div>
                <div>
                  <label class="text-sm text-slate-700">Password</label>
                  <input v-model="loginPassword" type="password" required class="mt-1 w-full rounded-lg border border-slate-200 bg-white px-3 py-2.5 text-slate-900 placeholder:text-slate-400 focus:outline-none focus:ring-2 focus:ring-emerald-500" />
                </div>
                <p v-if="loginError" class="text-sm text-rose-600">{{ loginError }}</p>
                <button type="submit" :disabled="loginLoading" class="w-full rounded-lg bg-slate-900 hover:bg-slate-800 text-white font-semibold py-2.75 shadow-md disabled:opacity-70 transition">
                  <span v-if="loginLoading">Signing in...</span>
                  <span v-else>Sign in</span>
                </button>
                <p class="text-xs text-slate-600">
                  Use the seeded demo account or the one you create below. Successful login redirects to POS.
                </p>
              </form>
            </div>

            <div class="rounded-2xl bg-white text-slate-900 border border-white/60 shadow-xl backdrop-blur-md p-6 lg:p-7" id="register">
              <div class="flex items-center gap-2 mb-4">
                <i class="material-symbols-rounded text-cyan-600">how_to_reg</i>
                <h2 class="text-xl font-semibold text-slate-900">Register your shop</h2>
              </div>
              <form class="space-y-3" @submit.prevent="handleRegister">
                <div class="grid grid-cols-2 gap-3">
                  <div>
                    <label class="text-sm text-slate-700">Shop Name</label>
                    <input v-model="regShop" required class="mt-1 w-full rounded-lg border border-slate-200 bg-white px-3 py-2.5 text-slate-900 placeholder:text-slate-400 focus:outline-none focus:ring-2 focus:ring-cyan-500" />
                  </div>
                  <div>
                    <label class="text-sm text-slate-700">Area</label>
                    <input v-model="regArea" required class="mt-1 w-full rounded-lg border border-slate-200 bg-white px-3 py-2.5 text-slate-900 placeholder:text-slate-400 focus:outline-none focus:ring-2 focus:ring-cyan-500" />
                  </div>
                </div>
                <div>
                  <label class="text-sm text-slate-700">Address</label>
                  <input v-model="regAddress" required class="mt-1 w-full rounded-lg border border-slate-200 bg-white px-3 py-2.5 text-slate-900 placeholder:text-slate-400 focus:outline-none focus:ring-2 focus:ring-cyan-500" />
                </div>
                <div class="grid grid-cols-2 gap-3">
                  <div>
                    <label class="text-sm text-slate-700">First Name</label>
                    <input v-model="regFirst" required class="mt-1 w-full rounded-lg border border-slate-200 bg-white px-3 py-2.5 text-slate-900 focus:outline-none focus:ring-2 focus:ring-cyan-500" />
                  </div>
                  <div>
                    <label class="text-sm text-slate-700">Last Name</label>
                    <input v-model="regLast" required class="mt-1 w-full rounded-lg border border-slate-200 bg-white px-3 py-2.5 text-slate-900 focus:outline-none focus:ring-2 focus:ring-cyan-500" />
                  </div>
                </div>
                <div>
                  <label class="text-sm text-slate-700">Phone</label>
                  <input v-model="regPhone" required class="mt-1 w-full rounded-lg border border-slate-200 bg-white px-3 py-2.5 text-slate-900 focus:outline-none focus:ring-2 focus:ring-cyan-500" />
                </div>
                <div>
                  <label class="text-sm text-slate-700">Email</label>
                  <input v-model="regEmail" type="email" required class="mt-1 w-full rounded-lg border border-slate-200 bg-white px-3 py-2.5 text-slate-900 focus:outline-none focus:ring-2 focus:ring-cyan-500" />
                </div>
                <div>
                  <label class="text-sm text-slate-700">Password</label>
                  <input v-model="regPassword" type="password" required class="mt-1 w-full rounded-lg border border-slate-200 bg-white px-3 py-2.5 text-slate-900 focus:outline-none focus:ring-2 focus:ring-cyan-500" />
                </div>
                <p v-if="regMessage" class="text-sm text-emerald-700">{{ regMessage }}</p>
                <p v-if="regError" class="text-sm text-rose-600">{{ regError }}</p>
                <button type="submit" :disabled="regLoading" class="w-full rounded-lg bg-cyan-500 hover:bg-cyan-400 text-slate-900 font-semibold py-2.75 shadow-md disabled:opacity-70 transition">
                  <span v-if="regLoading">Creating...</span>
                  <span v-else>Create account</span>
                </button>
              </form>
            </div>
          </div>
        </div>

        <!-- Stats -->
        <div class="mt-12">
          <div class="rounded-2xl bg-white text-slate-900 border border-white/60 shadow-2xl backdrop-blur-md px-6 py-8 lg:px-10">
            <div class="grid md:grid-cols-3 divide-y md:divide-y-0 md:divide-x divide-slate-200 text-center">
              <div class="py-6 px-4">
                <p class="text-4xl font-black text-slate-900">300+</p>
                <p class="mt-2 text-lg font-semibold">Coded Elements</p>
                <p class="mt-2 text-sm text-slate-600">From buttons, to inputs, navbars, alerts or cards, you are covered.</p>
              </div>
              <div class="py-6 px-4">
                <p class="text-4xl font-black text-slate-900">100+</p>
                <p class="mt-2 text-lg font-semibold">Design Blocks</p>
                <p class="mt-2 text-sm text-slate-600">Mix the sections, change the colors and unleash your creativity.</p>
              </div>
              <div class="py-6 px-4">
                <p class="text-4xl font-black text-slate-900">41</p>
                <p class="mt-2 text-lg font-semibold">Pages</p>
                <p class="mt-2 text-sm text-slate-600">Save 3–4 weeks of work when you use our pre-made pages.</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </main>

    <!-- Footer -->
    <footer class="relative z-10 pb-6">
      <div class="mx-auto max-w-6xl px-4">
        <div class="flex flex-wrap items-center justify-between text-sm text-slate-200">
          <p>© {{ new Date().getFullYear() }} TOSS — Built for township & rural SMMEs.</p>
          <div class="flex gap-4">
            <NuxtLink to="/help" class="hover:text-white">Help</NuxtLink>
            <NuxtLink to="/copilot" class="hover:text-white">Copilot</NuxtLink>
          </div>
        </div>
      </div>
    </footer>
  </div>
</template>
