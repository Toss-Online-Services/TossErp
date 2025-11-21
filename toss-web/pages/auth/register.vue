<template>
  <div class="min-h-screen flex items-center justify-center bg-gradient-to-br from-slate-50 via-orange-50/30 to-slate-100 dark:from-slate-900 dark:via-slate-900 dark:to-slate-800 px-4 py-8">
    <!-- Decorative background elements -->
    <div class="absolute inset-0 overflow-hidden pointer-events-none">
      <div class="absolute top-20 left-10 w-72 h-72 bg-orange-200/20 dark:bg-orange-500/10 rounded-full blur-3xl"></div>
      <div class="absolute bottom-20 right-10 w-96 h-96 bg-orange-300/20 dark:bg-orange-400/10 rounded-full blur-3xl"></div>
    </div>

    <div class="max-w-2xl w-full relative z-10">
      <!-- Logo and Title -->
      <div class="text-center mb-8">
        <NuxtLink to="/" class="inline-flex justify-center mb-6 group">
          <div class="relative flex items-center justify-center w-20 h-20 bg-gradient-to-br from-orange-500 to-orange-600 dark:from-orange-600 dark:to-orange-700 rounded-2xl shadow-xl transition-transform group-hover:scale-110 group-hover:rotate-3">
            <span class="text-4xl font-black text-white">T</span>
            <div class="absolute inset-0 bg-gradient-to-br from-orange-400 to-orange-500 rounded-2xl opacity-0 group-hover:opacity-20 transition-opacity"></div>
          </div>
        </NuxtLink>
        <h1 class="text-4xl font-bold bg-gradient-to-r from-slate-900 to-slate-700 dark:from-white dark:to-slate-300 bg-clip-text text-transparent">Join TOSS</h1>
        <p class="mt-2 text-slate-600 dark:text-slate-400">
          Start saving with group buying & shared logistics
        </p>
      </div>

      <!-- Registration Form -->
      <MaterialCard variant="elevated" class="p-8">
        <form @submit.prevent="handleRegister" class="space-y-6">
          <!-- Progress Indicator -->
          <div class="mb-8">
            <div class="flex items-center justify-between mb-2">
              <span class="text-sm font-medium text-slate-700 dark:text-slate-300">
                Step {{ currentStep }} of 3
              </span>
              <span class="text-sm text-slate-500 dark:text-slate-400">
                {{ Math.round((currentStep / 3) * 100) }}% Complete
              </span>
            </div>
            <div class="w-full bg-slate-200 dark:bg-slate-700 rounded-full h-2">
              <div 
                class="bg-gradient-to-r from-blue-600 to-purple-600 h-2 rounded-full transition-all duration-300"
                :style="{ width: `${(currentStep / 3) * 100}%` }"
              ></div>
            </div>
          </div>

          <!-- Step 1: Shop Information -->
          <div v-if="currentStep === 1" class="space-y-4">
            <h3 class="text-xl font-bold text-slate-900 dark:text-white mb-4">
              Tell us about your shop
            </h3>

            <MaterialInput
              v-model="form.shopName"
              label="Shop Name *"
              placeholder="e.g., Thabo's Spaza Shop"
              required
              variant="outlined"
            />

            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                  Area/Township *
                </label>
                <UiSelect v-model="form.area" required>
                  <UiSelectTrigger class="w-full">
                    <UiSelectValue placeholder="Select area" />
                  </UiSelectTrigger>
                  <UiSelectContent>
                    <UiSelectItem value="soweto">Soweto</UiSelectItem>
                    <UiSelectItem value="alexandra">Alexandra</UiSelectItem>
                    <UiSelectItem value="katlehong">Katlehong</UiSelectItem>
                    <UiSelectItem value="tembisa">Tembisa</UiSelectItem>
                    <UiSelectItem value="diepsloot">Diepsloot</UiSelectItem>
                    <UiSelectItem value="other">Other</UiSelectItem>
                  </UiSelectContent>
                </UiSelect>
              </div>

              <MaterialInput
                v-model="form.zone"
                label="Zone/Section"
                placeholder="e.g., Diepkloof Extension 1"
                variant="outlined"
              />
            </div>

            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                Physical Address *
              </label>
              <UiTextarea
                v-model="form.address"
                required
                rows="2"
                placeholder="Enter your shop's physical address"
                class="w-full"
              />
            </div>
          </div>

          <!-- Step 2: Owner Information -->
          <div v-if="currentStep === 2" class="space-y-4">
            <h3 class="text-xl font-bold text-slate-900 dark:text-white mb-4">
              Your contact details
            </h3>

            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <MaterialInput
                v-model="form.firstName"
                label="First Name *"
                placeholder="First name"
                required
                variant="outlined"
              />

              <MaterialInput
                v-model="form.lastName"
                label="Last Name *"
                placeholder="Last name"
                required
                variant="outlined"
              />
            </div>

            <div>
              <MaterialInput
                v-model="form.phone"
                label="Phone Number *"
                type="tel"
                placeholder="+27 XX XXX XXXX"
                required
                variant="outlined"
              />
              <p class="text-xs text-slate-500 dark:text-slate-400 mt-1">
                For WhatsApp alerts & group buying invites
              </p>
            </div>

            <MaterialInput
              v-model="form.email"
              label="Email Address"
              type="email"
              placeholder="your@email.com"
              variant="outlined"
            />
          </div>

          <!-- Step 3: Account Security -->
          <div v-if="currentStep === 3" class="space-y-4">
            <h3 class="text-xl font-bold text-slate-900 dark:text-white mb-4">
              Secure your account
            </h3>

            <MaterialInput
              v-model="form.password"
              label="Password *"
              type="password"
              placeholder="Create a strong password"
              required
              variant="outlined"
            />

            <MaterialInput
              v-model="form.confirmPassword"
              label="Confirm Password *"
              type="password"
              placeholder="Re-enter password"
              required
              variant="outlined"
            />

            <!-- Preferences -->
            <div class="pt-4 border-t border-slate-200 dark:border-slate-700">
              <h4 class="font-semibold text-slate-900 dark:text-white mb-3">
                Communication Preferences
              </h4>
              
              <label class="flex items-start space-x-3 cursor-pointer group">
                <UiSwitch v-model="form.whatsappAlerts" class="mt-1" />
                <div>
                  <span class="text-sm font-medium text-slate-900 dark:text-white group-hover:text-orange-600 dark:group-hover:text-orange-400 transition-colors">
                    Enable WhatsApp Alerts
                  </span>
                  <p class="text-xs text-slate-500 dark:text-slate-400">
                    Pool updates, delivery notifications & payment links
                  </p>
                </div>
              </label>

              <label class="flex items-start space-x-3 cursor-pointer group mt-3">
                <UiSwitch v-model="form.termsAccepted" class="mt-1" />
                <div>
                  <span class="text-sm font-medium text-slate-900 dark:text-white group-hover:text-orange-600 dark:group-hover:text-orange-400 transition-colors">
                    I agree to the Terms & Conditions *
                  </span>
                  <p class="text-xs text-slate-500 dark:text-slate-400">
                    Including group buying and shared logistics terms
                  </p>
                </div>
              </label>
            </div>
          </div>

          <!-- Navigation Buttons -->
          <div class="flex items-center justify-between pt-6 border-t border-slate-200 dark:border-slate-700">
            <MaterialButton
              v-if="currentStep > 1"
              type="button"
              @click="currentStep--"
              variant="outlined"
              size="lg"
            >
              ← Back
            </MaterialButton>
            <div v-else></div>

            <MaterialButton
              v-if="currentStep < 3"
              type="button"
              @click="currentStep++"
              color="primary"
              size="lg"
            >
              Continue →
            </MaterialButton>

            <MaterialButton
              v-else
              type="submit"
              :disabled="loading"
              :loading="loading"
              color="success"
              size="lg"
            >
              <template v-if="!loading">
                <CheckCircleIcon class="w-5 h-5 mr-2" />
                Complete Registration
              </template>
              <template v-else>
                Creating Account...
              </template>
            </MaterialButton>
          </div>
        </form>
      </MaterialCard>

      <!-- Sign In Link -->
      <p class="text-center text-sm text-slate-600 dark:text-slate-400 mt-6">
        Already have an account?
        <NuxtLink to="/auth/login" class="text-orange-600 hover:text-orange-700 dark:text-orange-400 dark:hover:text-orange-300 font-semibold">
          Sign in
        </NuxtLink>
      </p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { CheckCircleIcon } from '@heroicons/vue/24/outline'

definePageMeta({
  layout: false,
  middleware: []
})

useHead({
  title: 'Register - TOSS ERP',
  meta: [
    { name: 'description', content: 'Join TOSS and start saving with group buying' }
  ]
})

const currentStep = ref(1)
const loading = ref(false)

const form = ref({
  // Step 1
  shopName: '',
  area: '',
  zone: '',
  address: '',
  
  // Step 2
  firstName: '',
  lastName: '',
  phone: '',
  email: '',
  
  // Step 3
  password: '',
  confirmPassword: '',
  whatsappAlerts: true,
  termsAccepted: false
})

const handleRegister = async () => {
  // Validate passwords match
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
    // Register user
    const response = await $fetch('/api/auth/register', {
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
        whatsappAlerts: form.value.whatsappAlerts
      }
    }) as any

    // Store authentication data in session storage
    if (response.success && response.token) {
      sessionStorage.setItem('toss_token', response.token)
      sessionStorage.setItem('toss_user', JSON.stringify(response.user))
      sessionStorage.setItem('toss_shop', JSON.stringify(response.shop))
      
      console.log('✅ Registration successful:', response.message)
      console.log('User:', response.user)
      console.log('Shop:', response.shop)
    }

    // Navigate to dashboard
    alert('✅ Registration successful! Welcome to TOSS!')
    await navigateTo('/dashboard')
  } catch (error: any) {
    console.error('Registration failed:', error)
    
    // Extract error message from response
    const errorMessage = error?.data?.message || 
                        error?.data?.statusMessage || 
                        error?.message || 
                        'Registration failed. Please try again.'
    
    alert(`❌ ${errorMessage}`)
  } finally {
    loading.value = false
  }
}
</script>


