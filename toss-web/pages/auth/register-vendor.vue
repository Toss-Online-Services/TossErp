<script setup lang="ts">
definePageMeta({
  layout: 'auth',
  middleware: 'guest'
})

const router = useRouter()
const currentStep = ref(1)
const isLoading = ref(false)
const error = ref('')

// Form data
const formData = ref({
  // Step 1: Company Information
  companyName: '',
  companyRegNumber: '',
  vatNumber: '',
  description: '',
  website: '',
  
  // Step 2: Contact Person
  contactPerson: '',
  phone: '',
  email: '',
  
  // Step 3: Address & Payment Terms
  addressStreet: '',
  addressCity: '',
  addressProvince: '',
  addressPostalCode: '',
  addressCountry: 'South Africa',
  paymentTermsDays: 30,
  creditLimit: 0,
  
  // Step 4: Account Security
  password: '',
  confirmPassword: ''
})

const canGoNext = computed(() => {
  switch (currentStep.value) {
    case 1:
      return formData.value.companyName && formData.value.contactPerson
    case 2:
      return formData.value.phone && formData.value.email
    case 3:
      return formData.value.addressStreet && formData.value.addressCity
    case 4:
      return formData.value.password && formData.value.confirmPassword && formData.value.password === formData.value.confirmPassword
    default:
      return false
  }
})

const nextStep = () => {
  if (canGoNext.value && currentStep.value < 4) {
    currentStep.value++
  }
}

const previousStep = () => {
  if (currentStep.value > 1) {
    currentStep.value--
  }
}

const handleRegister = async () => {
  try {
    isLoading.value = true
    error.value = ''
    
    if (formData.value.password !== formData.value.confirmPassword) {
      error.value = 'Passwords do not match'
      return
    }
    
    const response = await $fetch('/api/auth/register-vendor', {
      method: 'POST',
      body: {
        companyName: formData.value.companyName,
        companyRegNumber: formData.value.companyRegNumber,
        vatNumber: formData.value.vatNumber,
        description: formData.value.description,
        website: formData.value.website,
        contactPerson: formData.value.contactPerson,
        phone: formData.value.phone,
        email: formData.value.email,
        addressStreet: formData.value.addressStreet,
        addressCity: formData.value.addressCity,
        addressProvince: formData.value.addressProvince,
        addressPostalCode: formData.value.addressPostalCode,
        addressCountry: formData.value.addressCountry,
        paymentTermsDays: formData.value.paymentTermsDays,
        creditLimit: formData.value.creditLimit,
        password: formData.value.password
      }
    })
    
    // Store auth data
    if (response.token) {
      sessionStorage.setItem('token', response.token)
    }
    if (response.user) {
      sessionStorage.setItem('user', JSON.stringify(response.user))
    }
    if (response.vendor) {
      sessionStorage.setItem('vendor', JSON.stringify(response.vendor))
    }
    
    // Navigate to dashboard
    await router.push('/dashboard')
  } catch (err: any) {
    console.error('Registration error:', err)
    error.value = err?.data?.message || err?.message || 'Registration failed. Please check your details and try again.'
  } finally {
    isLoading.value = false
  }
}

const provinces = [
  'Eastern Cape',
  'Free State',
  'Gauteng',
  'KwaZulu-Natal',
  'Limpopo',
  'Mpumalanga',
  'Northern Cape',
  'North West',
  'Western Cape'
]
</script>

<template>
  <div class="auth-container">
    <div class="auth-card">
      <div class="auth-header">
        <h1 class="text-3xl font-bold text-gray-900 dark:text-white">Register as Vendor</h1>
        <p class="text-gray-600 dark:text-gray-400 mt-2">Join our vendor network</p>
        <div class="mt-4 text-sm text-gray-500">
          Step {{ currentStep }} of 4
        </div>
      </div>

      <div v-if="error" class="error-message">
        {{ error }}
      </div>

      <form @submit.prevent="handleRegister" class="auth-form">
        <!-- Step 1: Company Information -->
        <div v-if="currentStep === 1" class="space-y-4">
          <h3 class="text-xl font-semibold text-gray-900 dark:text-white">Company Information</h3>
          
          <div>
            <label class="form-label">Company Name *</label>
            <input
              v-model="formData.companyName"
              type="text"
              placeholder="Fresh Foods Suppliers"
              class="form-input"
              required
            />
          </div>
          
          <div>
            <label class="form-label">Company Registration Number</label>
            <input
              v-model="formData.companyRegNumber"
              type="text"
              placeholder="2024/123456/07"
              class="form-input"
            />
          </div>
          
          <div>
            <label class="form-label">VAT Number</label>
            <input
              v-model="formData.vatNumber"
              type="text"
              placeholder="4567890123"
              class="form-input"
            />
          </div>
          
          <div>
            <label class="form-label">Website</label>
            <input
              v-model="formData.website"
              type="url"
              placeholder="https://yourcompany.co.za"
              class="form-input"
            />
          </div>
          
          <div>
            <label class="form-label">Description</label>
            <textarea
              v-model="formData.description"
              placeholder="Brief description of your business"
              class="form-input"
              rows="3"
            ></textarea>
          </div>
        </div>

        <!-- Step 2: Contact Person -->
        <div v-if="currentStep === 2" class="space-y-4">
          <h3 class="text-xl font-semibold text-gray-900 dark:text-white">Contact Person Details</h3>
          
          <div>
            <label class="form-label">Contact Person *</label>
            <input
              v-model="formData.contactPerson"
              type="text"
              placeholder="Jane Doe"
              class="form-input"
              required
            />
          </div>
          
          <div>
            <label class="form-label">Phone Number *</label>
            <input
              v-model="formData.phone"
              type="tel"
              placeholder="+27 82 123 4567"
              class="form-input"
              required
            />
          </div>
          
          <div>
            <label class="form-label">Email *</label>
            <input
              v-model="formData.email"
              type="email"
              placeholder="contact@company.co.za"
              class="form-input"
              required
            />
          </div>
        </div>

        <!-- Step 3: Address & Payment Terms -->
        <div v-if="currentStep === 3" class="space-y-4">
          <h3 class="text-xl font-semibold text-gray-900 dark:text-white">Address & Payment Terms</h3>
          
          <div>
            <label class="form-label">Street Address *</label>
            <input
              v-model="formData.addressStreet"
              type="text"
              placeholder="456 Business Avenue"
              class="form-input"
              required
            />
          </div>
          
          <div class="grid grid-cols-2 gap-4">
            <div>
              <label class="form-label">City *</label>
              <input
                v-model="formData.addressCity"
                type="text"
                placeholder="Johannesburg"
                class="form-input"
                required
              />
            </div>
            
            <div>
              <label class="form-label">Province</label>
              <select v-model="formData.addressProvince" class="form-input">
                <option value="">Select Province</option>
                <option v-for="province in provinces" :key="province" :value="province">
                  {{ province }}
                </option>
              </select>
            </div>
          </div>
          
          <div class="grid grid-cols-2 gap-4">
            <div>
              <label class="form-label">Postal Code</label>
              <input
                v-model="formData.addressPostalCode"
                type="text"
                placeholder="2000"
                class="form-input"
              />
            </div>
            
            <div>
              <label class="form-label">Country</label>
              <input
                v-model="formData.addressCountry"
                type="text"
                class="form-input"
                readonly
              />
            </div>
          </div>
          
          <div class="grid grid-cols-2 gap-4">
            <div>
              <label class="form-label">Payment Terms (Days)</label>
              <input
                v-model.number="formData.paymentTermsDays"
                type="number"
                min="0"
                class="form-input"
              />
            </div>
            
            <div>
              <label class="form-label">Credit Limit (R)</label>
              <input
                v-model.number="formData.creditLimit"
                type="number"
                min="0"
                step="100"
                placeholder="0"
                class="form-input"
              />
            </div>
          </div>
        </div>

        <!-- Step 4: Account Security -->
        <div v-if="currentStep === 4" class="space-y-4">
          <h3 class="text-xl font-semibold text-gray-900 dark:text-white">Secure Your Account</h3>
          
          <div>
            <label class="form-label">Password *</label>
            <input
              v-model="formData.password"
              type="password"
              placeholder="Create a strong password"
              class="form-input"
              required
            />
          </div>
          
          <div>
            <label class="form-label">Confirm Password *</label>
            <input
              v-model="formData.confirmPassword"
              type="password"
              placeholder="Re-enter password"
              class="form-input"
              required
            />
          </div>
        </div>

        <!-- Navigation Buttons -->
        <div class="flex gap-4 mt-6">
          <button
            v-if="currentStep > 1"
            type="button"
            @click="previousStep"
            class="btn-secondary flex-1"
          >
            ← Back
          </button>
          
          <button
            v-if="currentStep < 4"
            type="button"
            @click="nextStep"
            :disabled="!canGoNext"
            class="btn-primary flex-1"
          >
            Continue →
          </button>
          
          <button
            v-if="currentStep === 4"
            type="submit"
            :disabled="!canGoNext || isLoading"
            class="btn-primary flex-1"
          >
            <span v-if="!isLoading">Complete Registration</span>
            <span v-else>Registering...</span>
          </button>
        </div>
      </form>

      <div class="auth-footer">
        <p class="text-sm text-gray-600 dark:text-gray-400">
          Already have an account?
          <NuxtLink to="/auth/login" class="text-blue-600 hover:text-blue-700 dark:text-blue-400">
            Sign in
          </NuxtLink>
        </p>
      </div>
    </div>
  </div>
</template>

<style scoped>
.auth-container {
  @apply min-h-screen flex items-center justify-center bg-gray-50 dark:bg-gray-900 py-12 px-4 sm:px-6 lg:px-8;
}

.auth-card {
  @apply max-w-2xl w-full space-y-8 bg-white dark:bg-gray-800 p-8 rounded-lg shadow-lg;
}

.auth-header {
  @apply text-center;
}

.auth-form {
  @apply mt-8 space-y-6;
}

.form-label {
  @apply block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1;
}

.form-input {
  @apply appearance-none relative block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 
         placeholder-gray-500 dark:placeholder-gray-400 text-gray-900 dark:text-white 
         rounded-md focus:outline-none focus:ring-blue-500 focus:border-blue-500 focus:z-10 sm:text-sm
         bg-white dark:bg-gray-700;
}

.btn-primary {
  @apply group relative w-full flex justify-center py-2 px-4 border border-transparent 
         text-sm font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700 
         focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500
         disabled:opacity-50 disabled:cursor-not-allowed;
}

.btn-secondary {
  @apply group relative w-full flex justify-center py-2 px-4 border border-gray-300 dark:border-gray-600
         text-sm font-medium rounded-md text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-700
         hover:bg-gray-50 dark:hover:bg-gray-600 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500;
}

.error-message {
  @apply bg-red-50 dark:bg-red-900/30 border border-red-400 dark:border-red-800 text-red-700 dark:text-red-400 px-4 py-3 rounded relative;
}

.auth-footer {
  @apply mt-6 text-center;
}
</style>

