<script setup lang="ts">
import { computed, ref } from 'vue'
import { CheckCircleIcon } from '@heroicons/vue/24/outline'

interface RegisterForm {
  shopName: string
  area: string
  zone: string
  address: string
  firstName: string
  lastName: string
  phone: string
  email: string
  password: string
  confirmPassword: string
  whatsappAlerts: boolean
  termsAccepted: boolean
}

const totalSteps = 3
const currentStep = ref<number>(1)
const loading = ref(false)

const form = ref<RegisterForm>({
  shopName: '',
  area: '',
  zone: '',
  address: '',
  firstName: '',
  lastName: '',
  phone: '',
  email: '',
  password: '',
  confirmPassword: '',
  whatsappAlerts: true,
  termsAccepted: false,
})

const progressPercent = computed(() => Math.round((currentStep.value / totalSteps) * 100))

const nextStep = () => {
  if (currentStep.value < totalSteps) {
    currentStep.value += 1
  }
}

const prevStep = () => {
  if (currentStep.value > 1) {
    currentStep.value -= 1
  }
}

// @ts-ignore -- Nuxt auto-injects definePageMeta in setup
definePageMeta({
  layout: false,
  middleware: [],
})

// @ts-ignore -- Nuxt auto-injects useHead composable
useHead({
  title: 'Register - TOSS ERP',
  meta: [{ name: 'description', content: 'Join TOSS and start saving with group buying' }],
})

const persistSession = (response: Record<string, any>) => {
  if (typeof window === 'undefined') {
    return
  }

  if (response.token) {
    sessionStorage.setItem('toss_token', response.token)
  }

  if (response.user) {
    sessionStorage.setItem('toss_user', JSON.stringify(response.user))
  }

  if (response.shop) {
    sessionStorage.setItem('toss_shop', JSON.stringify(response.shop))
  }
}

const handleRegister = async () => {
  if (form.value.password !== form.value.confirmPassword) {
    alert('Passwords do not match!')
    return
  }

  if (!form.value.termsAccepted) {
    alert('Please accept the terms and conditions')
    return
  }

  loading.value = true

  try {
  // @ts-ignore -- Nuxt injects $fetch globally at runtime
  const response = (await $fetch('/api/auth/register', {
      method: 'POST',
      body: {
        shopName: form.value.shopName,
        area: form.value.area,
        zone: form.value.zone,
        address: form.value.address,
        firstName: form.value.firstName,
        lastName: form.value.lastName,
        phone: form.value.phone,
        email: form.value.email,
        password: form.value.password,
        whatsappAlerts: form.value.whatsappAlerts,
      },
    })) as Record<string, any>

    if (response?.success && response?.token) {
      persistSession(response)
    }

    alert('✅ Registration successful! Welcome to TOSS!')
    // @ts-ignore -- Nuxt auto-injects navigateTo helper
    await navigateTo('/dashboard')
  } catch (error: any) {
    console.error('Registration failed:', error)
    const errorMessage =
      error?.data?.message ||
      error?.data?.statusMessage ||
      error?.message ||
      'Registration failed. Please try again.'

    alert(`❌ ${errorMessage}`)
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="min-h-screen flex items-center justify-center bg-gradient-to-br from-slate-50 via-blue-50/30 to-slate-100 dark:from-slate-900 dark:via-slate-900 dark:to-slate-800 px-4 py-8">
    <div class="max-w-2xl w-full">
      <div class="text-center mb-8">
        <div class="flex justify-center mb-4">
          <div class="w-20 h-20 bg-gradient-to-r from-blue-500 to-purple-600 rounded-2xl flex items-center justify-center shadow-lg">
            <span class="text-4xl font-bold text-white">T</span>
          </div>
        </div>
        <h1 class="text-4xl font-bold text-slate-900 dark:text-white">Join TOSS</h1>
        <p class="mt-2 text-slate-600 dark:text-slate-400">
          Start saving with group buying &amp; shared logistics
        </p>
      </div>

      <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-2xl border border-slate-200 dark:border-slate-700 p-8">
        <form class="space-y-6" @submit.prevent="handleRegister">
          <div class="mb-8">
            <div class="flex items-center justify-between mb-2">
              <span class="text-sm font-medium text-slate-700 dark:text-slate-300">
                Step {{ currentStep }} of {{ totalSteps }}
              </span>
              <span class="text-sm text-slate-500 dark:text-slate-400">
                {{ progressPercent }}% Complete
              </span>
            </div>
            <div class="w-full bg-slate-200 dark:bg-slate-700 rounded-full h-2">
              <div
                class="bg-gradient-to-r from-blue-600 to-purple-600 h-2 rounded-full transition-all duration-300"
                :style="{ width: `${progressPercent}%` }"
              ></div>
            </div>
          </div>

          <div v-if="currentStep === 1" class="space-y-4">
            <h3 class="text-xl font-bold text-slate-900 dark:text-white mb-4">
              Tell us about your shop
            </h3>

            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                Shop Name *
              </label>
              <input
                v-model="form.shopName"
                type="text"
                required
                placeholder="e.g., Thabo's Spaza Shop"
                class="w-full px-4 py-3 border-2 border-slate-200 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-blue-500 bg-white dark:bg-slate-900 text-slate-900 dark:text-white transition-all"
              />
            </div>

            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                  Area/Township *
                </label>
                <select
                  v-model="form.area"
                  required
                  class="w-full px-4 py-3 border-2 border-slate-200 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-blue-500 bg-white dark:bg-slate-900 text-slate-900 dark:text-white"
                >
                  <option value="">Select area</option>
                  <option value="soweto">Soweto</option>
                  <option value="alexandra">Alexandra</option>
                  <option value="katlehong">Katlehong</option>
                  <option value="tembisa">Tembisa</option>
                  <option value="diepsloot">Diepsloot</option>
                  <option value="other">Other</option>
                </select>
              </div>

              <div>
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                  Zone/Section
                </label>
                <input
                  v-model="form.zone"
                  type="text"
                  placeholder="e.g., Diepkloof Extension 1"
                  class="w-full px-4 py-3 border-2 border-slate-200 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-blue-500 bg-white dark:bg-slate-900 text-slate-900 dark:text-white"
                />
              </div>
            </div>

            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                Physical Address *
              </label>
              <textarea
                v-model="form.address"
                required
                rows="2"
                placeholder="Enter your shop's physical address"
                class="w-full px-4 py-3 border-2 border-slate-200 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-blue-500 bg-white dark:bg-slate-900 text-slate-900 dark:text-white"
              ></textarea>
            </div>
          </div>

          <div v-if="currentStep === 2" class="space-y-4">
            <h3 class="text-xl font-bold text-slate-900 dark:text-white mb-4">
              Your contact details
            </h3>

            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                  First Name *
                </label>
                <input
                  v-model="form.firstName"
                  type="text"
                  required
                  placeholder="First name"
                  class="w-full px-4 py-3 border-2 border-slate-200 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-blue-500 bg-white dark:bg-slate-900 text-slate-900 dark:text-white"
                />
              </div>

              <div>
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                  Last Name *
                </label>
                <input
                  v-model="form.lastName"
                  type="text"
                  required
                  placeholder="Last name"
                  class="w-full px-4 py-3 border-2 border-slate-200 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-blue-500 bg-white dark:bg-slate-900 text-slate-900 dark:text-white"
                />
              </div>
            </div>

            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                Phone Number *
              </label>
              <input
                v-model="form.phone"
                type="tel"
                required
                placeholder="+27 XX XXX XXXX"
                class="w-full px-4 py-3 border-2 border-slate-200 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-blue-500 bg-white dark:bg-slate-900 text-slate-900 dark:text-white"
              />
              <p class="text-xs text-slate-500 dark:text-slate-400 mt-1">
                For WhatsApp alerts &amp; group buying invites
              </p>
            </div>

            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                Email Address
              </label>
              <input
                v-model="form.email"
                type="email"
                placeholder="your@email.com"
                class="w-full px-4 py-3 border-2 border-slate-200 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-blue-500 bg-white dark:bg-slate-900 text-slate-900 dark:text-white"
              />
            </div>
          </div>

          <div v-if="currentStep === 3" class="space-y-4">
            <h3 class="text-xl font-bold text-slate-900 dark:text-white mb-4">
              Secure your account
            </h3>

            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                Password *
              </label>
              <input
                v-model="form.password"
                type="password"
                required
                placeholder="Create a strong password"
                class="w-full px-4 py-3 border-2 border-slate-200 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-blue-500 bg-white dark:bg-slate-900 text-slate-900 dark:text-white"
              />
            </div>

            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                Confirm Password *
              </label>
              <input
                v-model="form.confirmPassword"
                type="password"
                required
                placeholder="Re-enter password"
                class="w-full px-4 py-3 border-2 border-slate-200 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-blue-500 bg-white dark:bg-slate-900 text-slate-900 dark:text-white"
              />
            </div>

            <div class="pt-4 border-t border-slate-200 dark:border-slate-700">
              <h4 class="font-semibold text-slate-900 dark:text-white mb-3">
                Communication Preferences
              </h4>

              <label class="flex items-start space-x-3 cursor-pointer">
                <input
                  v-model="form.whatsappAlerts"
                  type="checkbox"
                  class="w-4 h-4 mt-1 text-blue-600 border-slate-300 rounded focus:ring-blue-500"
                />
                <div>
                  <span class="text-sm font-medium text-slate-900 dark:text-white">
                    Enable WhatsApp Alerts
                  </span>
                  <p class="text-xs text-slate-500 dark:text-slate-400">
                    Pool updates, delivery notifications &amp; payment links
                  </p>
                </div>
              </label>

              <label class="flex items-start space-x-3 cursor-pointer mt-3">
                <input
                  v-model="form.termsAccepted"
                  type="checkbox"
                  required
                  class="w-4 h-4 mt-1 text-blue-600 border-slate-300 rounded focus:ring-blue-500"
                />
                <div>
                  <span class="text-sm font-medium text-slate-900 dark:text-white">
                    I agree to the Terms &amp; Conditions *
                  </span>
                  <p class="text-xs text-slate-500 dark:text-slate-400">
                    Including group buying and shared logistics terms
                  </p>
                </div>
              </label>
            </div>
          </div>

          <div class="flex items-center justify-between pt-6 border-t border-slate-200 dark:border-slate-700">
            <button
              v-if="currentStep > 1"
              type="button"
              @click="prevStep"
              class="px-6 py-3 border-2 border-slate-300 dark:border-slate-600 text-slate-700 dark:text-slate-300 rounded-xl font-semibold hover:bg-slate-50 dark:hover:bg-slate-700 transition-all"
            >
              ← Back
            </button>
            <div v-else></div>

            <button
              v-if="currentStep < totalSteps"
              type="button"
              @click="nextStep"
              class="px-6 py-3 bg-gradient-to-r from-blue-600 to-purple-600 text-white rounded-xl font-semibold hover:from-blue-700 hover:to-purple-700 shadow-lg hover:shadow-xl transition-all"
            >
              Continue →
            </button>

            <button
              v-else
              type="submit"
              :disabled="loading"
              class="px-6 py-3 bg-gradient-to-r from-green-600 to-emerald-600 text-white rounded-xl font-semibold hover:from-green-700 hover:to-emerald-700 shadow-lg hover:shadow-xl transition-all disabled:opacity-50 disabled:cursor-not-allowed flex items-center"
            >
              <CheckCircleIcon v-if="!loading" class="w-5 h-5 mr-2" />
              <div v-else class="w-5 h-5 mr-2 border-2 border-white border-t-transparent rounded-full animate-spin"></div>
              {{ loading ? 'Creating Account...' : 'Complete Registration' }}
            </button>
          </div>
        </form>
      </div>

      <p class="text-center text-sm text-slate-600 dark:text-slate-400 mt-6">
        Already have an account?
        <NuxtLink to="/auth/login" class="text-blue-600 hover:text-blue-700 dark:text-blue-400 dark:hover:text-blue-300 font-medium">
          Sign in
        </NuxtLink>
      </p>
    </div>
  </div>
</template>
