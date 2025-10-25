<template>
  <div class="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100 dark:from-gray-900 dark:to-gray-800 flex items-center justify-center p-4">
    <div class="max-w-4xl w-full bg-white dark:bg-gray-800 rounded-2xl shadow-2xl overflow-hidden">
      <!-- Progress Bar -->
      <div class="bg-gray-100 dark:bg-gray-700 px-8 py-4">
        <div class="flex items-center justify-between mb-2">
          <span class="text-sm font-medium text-gray-700 dark:text-gray-300">Step {{ currentStep }} of {{ totalSteps }}</span>
          <span class="text-sm font-medium text-blue-600 dark:text-blue-400">{{ Math.round((currentStep / totalSteps) * 100) }}% Complete</span>
        </div>
        <div class="w-full bg-gray-200 dark:bg-gray-600 rounded-full h-2">
          <div class="bg-blue-600 h-2 rounded-full transition-all duration-300" :style="{ width: `${(currentStep / totalSteps) * 100}%` }"></div>
        </div>
      </div>

      <!-- Step Content -->
      <div class="p-8">
        <!-- Step 1: Welcome -->
        <div v-if="currentStep === 1" class="space-y-6">
          <div class="text-center">
            <div class="w-20 h-20 bg-blue-100 dark:bg-blue-900 rounded-full flex items-center justify-center mx-auto mb-4">
              <svg class="w-10 h-10 text-blue-600 dark:text-blue-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M14 10h4.764a2 2 0 011.789 2.894l-3.5 7A2 2 0 0115.263 21h-4.017c-.163 0-.326-.02-.485-.06L7 20m7-10V5a2 2 0 00-2-2h-.095c-.5 0-.905.405-.905.905 0 .714-.211 1.412-.608 2.006L7 11v9m7-10h-2M7 20H5a2 2 0 01-2-2v-6a2 2 0 012-2h2.5"></path>
              </svg>
            </div>
            <h1 class="text-3xl font-bold text-gray-900 dark:text-white mb-2">Welcome to TOSS ERP III!</h1>
            <p class="text-gray-600 dark:text-gray-400 text-lg">Let's get your business set up in just a few minutes</p>
          </div>

          <div class="grid grid-cols-1 md:grid-cols-3 gap-4 mt-8">
            <div class="p-4 bg-blue-50 dark:bg-blue-900 rounded-lg">
              <div class="w-10 h-10 bg-blue-600 rounded-lg flex items-center justify-center mb-3">
                <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 10V3L4 14h7v7l9-11h-7z"></path>
                </svg>
              </div>
              <h3 class="font-semibold text-gray-900 dark:text-white mb-1">Quick Setup</h3>
              <p class="text-sm text-gray-600 dark:text-gray-400">Get started in under 5 minutes</p>
            </div>

            <div class="p-4 bg-green-50 dark:bg-green-900 rounded-lg">
              <div class="w-10 h-10 bg-green-600 rounded-lg flex items-center justify-center mb-3">
                <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m5.618-4.016A11.955 11.955 0 0112 2.944a11.955 11.955 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.03 9-11.622 0-1.042-.133-2.052-.382-3.016z"></path>
                </svg>
              </div>
              <h3 class="font-semibold text-gray-900 dark:text-white mb-1">Secure & Reliable</h3>
              <p class="text-sm text-gray-600 dark:text-gray-400">Your data is safe with us</p>
            </div>

            <div class="p-4 bg-purple-50 dark:bg-purple-900 rounded-lg">
              <div class="w-10 h-10 bg-purple-600 rounded-lg flex items-center justify-center mb-3">
                <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M18.364 5.636l-3.536 3.536m0 5.656l3.536 3.536M9.172 9.172L5.636 5.636m3.536 9.192l-3.536 3.536M21 12a9 9 0 11-18 0 9 9 0 0118 0zm-5 0a4 4 0 11-8 0 4 4 0 018 0z"></path>
                </svg>
              </div>
              <h3 class="font-semibold text-gray-900 dark:text-white mb-1">24/7 Support</h3>
              <p class="text-sm text-gray-600 dark:text-gray-400">We're here to help anytime</p>
            </div>
          </div>
        </div>

        <!-- Step 2: Company Information -->
        <div v-if="currentStep === 2" class="space-y-6">
          <div>
            <h2 class="text-2xl font-bold text-gray-900 dark:text-white mb-2">Company Information</h2>
            <p class="text-gray-600 dark:text-gray-400">Tell us about your business</p>
          </div>

          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Company Name *</label>
              <input v-model="formData.companyName" type="text" class="w-full px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-gray-700 dark:text-white" placeholder="Your Company Ltd">
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Industry *</label>
              <select v-model="formData.industry" class="w-full px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-gray-700 dark:text-white">
                <option value="">Select Industry</option>
                <option value="retail">Retail</option>
                <option value="manufacturing">Manufacturing</option>
                <option value="services">Services</option>
                <option value="hospitality">Hospitality</option>
                <option value="technology">Technology</option>
                <option value="other">Other</option>
              </select>
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Company Size *</label>
              <select v-model="formData.companySize" class="w-full px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-gray-700 dark:text-white">
                <option value="">Select Size</option>
                <option value="1-10">1-10 employees</option>
                <option value="11-50">11-50 employees</option>
                <option value="51-200">51-200 employees</option>
                <option value="201-500">201-500 employees</option>
                <option value="500+">500+ employees</option>
              </select>
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Country *</label>
              <select v-model="formData.country" class="w-full px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-gray-700 dark:text-white">
                <option value="">Select Country</option>
                <option value="ZA">South Africa</option>
                <option value="US">United States</option>
                <option value="GB">United Kingdom</option>
                <option value="Other">Other</option>
              </select>
            </div>

            <div class="md:col-span-2">
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Currency *</label>
              <select v-model="formData.currency" class="w-full px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-gray-700 dark:text-white">
                <option value="">Select Currency</option>
                <option value="ZAR">South African Rand (ZAR)</option>
                <option value="USD">US Dollar (USD)</option>
                <option value="EUR">Euro (EUR)</option>
                <option value="GBP">British Pound (GBP)</option>
              </select>
            </div>

            <div class="md:col-span-2">
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Tax ID / VAT Number</label>
              <input v-model="formData.taxId" type="text" class="w-full px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-gray-700 dark:text-white" placeholder="Optional">
            </div>
          </div>
        </div>

        <!-- Step 3: User Profile -->
        <div v-if="currentStep === 3" class="space-y-6">
          <div>
            <h2 class="text-2xl font-bold text-gray-900 dark:text-white mb-2">Your Profile</h2>
            <p class="text-gray-600 dark:text-gray-400">Set up your user account</p>
          </div>

          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">First Name *</label>
              <input v-model="formData.firstName" type="text" class="w-full px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-gray-700 dark:text-white">
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Last Name *</label>
              <input v-model="formData.lastName" type="text" class="w-full px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-gray-700 dark:text-white">
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Email *</label>
              <input v-model="formData.email" type="email" class="w-full px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-gray-700 dark:text-white">
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Phone Number</label>
              <input v-model="formData.phone" type="tel" class="w-full px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-gray-700 dark:text-white">
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Job Title *</label>
              <input v-model="formData.jobTitle" type="text" class="w-full px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-gray-700 dark:text-white" placeholder="e.g., CEO, Manager">
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Department</label>
              <select v-model="formData.department" class="w-full px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-gray-700 dark:text-white">
                <option value="">Select Department</option>
                <option value="management">Management</option>
                <option value="sales">Sales</option>
                <option value="finance">Finance</option>
                <option value="operations">Operations</option>
                <option value="hr">Human Resources</option>
                <option value="it">IT</option>
              </select>
            </div>
          </div>
        </div>

        <!-- Step 4: Module Selection -->
        <div v-if="currentStep === 4" class="space-y-6">
          <div>
            <h2 class="text-2xl font-bold text-gray-900 dark:text-white mb-2">Select Modules</h2>
            <p class="text-gray-600 dark:text-gray-400">Choose the modules you want to use</p>
          </div>

          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div v-for="module in modules" :key="module.id" 
                 @click="toggleModule(module.id)"
                 class="p-4 border-2 rounded-lg cursor-pointer transition-all"
                 :class="formData.selectedModules.includes(module.id) ? 'border-blue-600 bg-blue-50 dark:bg-blue-900' : 'border-gray-200 dark:border-gray-600 hover:border-blue-300'">
              <div class="flex items-start">
                <div class="flex-shrink-0">
                  <div class="w-6 h-6 rounded border-2 flex items-center justify-center"
                       :class="formData.selectedModules.includes(module.id) ? 'border-blue-600 bg-blue-600' : 'border-gray-300'">
                    <svg v-if="formData.selectedModules.includes(module.id)" class="w-4 h-4 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7"></path>
                    </svg>
                  </div>
                </div>
                <div class="ml-3">
                  <h3 class="font-semibold text-gray-900 dark:text-white">{{ module.name }}</h3>
                  <p class="text-sm text-gray-600 dark:text-gray-400 mt-1">{{ module.description }}</p>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Step 5: Completion -->
        <div v-if="currentStep === 5" class="space-y-6">
          <div class="text-center">
            <div class="w-20 h-20 bg-green-100 dark:bg-green-900 rounded-full flex items-center justify-center mx-auto mb-4">
              <svg class="w-10 h-10 text-green-600 dark:text-green-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"></path>
              </svg>
            </div>
            <h1 class="text-3xl font-bold text-gray-900 dark:text-white mb-2">You're All Set!</h1>
            <p class="text-gray-600 dark:text-gray-400 text-lg">Your TOSS ERP III account is ready to use</p>
          </div>

          <div class="bg-blue-50 dark:bg-blue-900 rounded-lg p-6 mt-8">
            <h3 class="font-semibold text-gray-900 dark:text-white mb-4">What's Next?</h3>
            <ul class="space-y-3">
              <li class="flex items-start">
                <svg class="w-5 h-5 text-blue-600 dark:text-blue-400 mr-3 mt-0.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"></path>
                </svg>
                <span class="text-gray-700 dark:text-gray-300">Explore your dashboard and familiarize yourself with the interface</span>
              </li>
              <li class="flex items-start">
                <svg class="w-5 h-5 text-blue-600 dark:text-blue-400 mr-3 mt-0.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"></path>
                </svg>
                <span class="text-gray-700 dark:text-gray-300">Invite team members to collaborate</span>
              </li>
              <li class="flex items-start">
                <svg class="w-5 h-5 text-blue-600 dark:text-blue-400 mr-3 mt-0.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"></path>
                </svg>
                <span class="text-gray-700 dark:text-gray-300">Check out our tutorials and documentation</span>
              </li>
              <li class="flex items-start">
                <svg class="w-5 h-5 text-blue-600 dark:text-blue-400 mr-3 mt-0.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"></path>
                </svg>
                <span class="text-gray-700 dark:text-gray-300">Contact support if you need any help</span>
              </li>
            </ul>
          </div>

          <!-- Error Message -->
          <div v-if="errorMessage" class="mt-6 p-4 bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-800 rounded-lg">
            <div class="flex items-start">
              <svg class="w-5 h-5 text-red-600 dark:text-red-400 mr-3 mt-0.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path>
              </svg>
              <div>
                <h4 class="font-semibold text-red-800 dark:text-red-200">Setup Error</h4>
                <p class="text-sm text-red-700 dark:text-red-300 mt-1">{{ errorMessage }}</p>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Navigation Buttons -->
      <div class="bg-gray-50 dark:bg-gray-700 px-8 py-4 flex justify-between">
        <button v-if="currentStep > 1 && currentStep < 5" @click="previousStep" class="px-6 py-2 border border-gray-300 dark:border-gray-600 rounded-lg text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-600 transition-colors">
          Previous
        </button>
        <div v-else></div>

        <button v-if="currentStep < totalSteps" @click="nextStep" :disabled="!canProceed" class="px-6 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:bg-gray-300 disabled:cursor-not-allowed transition-colors">
          {{ currentStep === 1 ? 'Get Started' : 'Continue' }}
        </button>

        <button v-if="currentStep === totalSteps" @click="completeOnboarding" :disabled="isSubmitting" class="px-8 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 disabled:bg-gray-400 disabled:cursor-not-allowed transition-colors flex items-center">
          <svg v-if="isSubmitting" class="animate-spin h-5 w-5 mr-2" fill="none" viewBox="0 0 24 24">
            <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
            <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
          </svg>
          {{ isSubmitting ? 'Setting up...' : 'Go to Dashboard' }}
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
definePageMeta({
  layout: false,
  title: 'Onboarding - TOSS ERP III'
})

const router = useRouter()

const currentStep = ref(1)
const totalSteps = 5

const formData = ref({
  companyName: '',
  industry: '',
  companySize: '',
  country: '',
  currency: '',
  taxId: '',
  firstName: '',
  lastName: '',
  email: '',
  phone: '',
  jobTitle: '',
  department: '',
  selectedModules: ['sales', 'inventory', 'accounting']
})

const modules = [
  { id: 'sales', name: 'Sales & CRM', description: 'Manage customers, leads, and sales pipeline' },
  { id: 'inventory', name: 'Inventory', description: 'Track stock levels and manage warehouses' },
  { id: 'accounting', name: 'Accounting', description: 'Financial management and reporting' },
  { id: 'purchasing', name: 'Purchasing', description: 'Supplier management and purchase orders' },
  { id: 'hr', name: 'Human Resources', description: 'Employee management and payroll' },
  { id: 'manufacturing', name: 'Manufacturing', description: 'Production planning and BOM management' },
  { id: 'projects', name: 'Projects', description: 'Project management and time tracking' },
  { id: 'pos', name: 'Point of Sale', description: 'Retail POS and payment processing' }
]

const canProceed = computed(() => {
  if (currentStep.value === 1) return true
  if (currentStep.value === 2) {
    return formData.value.companyName && formData.value.industry && formData.value.companySize && formData.value.country && formData.value.currency
  }
  if (currentStep.value === 3) {
    return formData.value.firstName && formData.value.lastName && formData.value.email && formData.value.jobTitle
  }
  if (currentStep.value === 4) {
    return formData.value.selectedModules.length > 0
  }
  return true
})

const nextStep = () => {
  if (canProceed.value && currentStep.value < totalSteps) {
    currentStep.value++
  }
}

const previousStep = () => {
  if (currentStep.value > 1) {
    currentStep.value--
  }
}

const toggleModule = (moduleId) => {
  const index = formData.value.selectedModules.indexOf(moduleId)
  if (index > -1) {
    formData.value.selectedModules.splice(index, 1)
  } else {
    formData.value.selectedModules.push(moduleId)
  }
}

const isSubmitting = ref(false)
const errorMessage = ref('')

const completeOnboarding = async () => {
  isSubmitting.value = true
  errorMessage.value = ''
  
  try {
    // Create shop with onboarding data
    const shopData = {
      name: formData.value.companyName,
      email: formData.value.email,
      phone: formData.value.phone,
      address: {
        street: '',
        city: '',
        province: '',
        postalCode: '',
        country: formData.value.country
      },
      taxNumber: formData.value.taxId,
      currency: formData.value.currency,
      industry: formData.value.industry,
      size: formData.value.companySize
    }

    // Save to backend
    const response = await $fetch('/api/shops', {
      method: 'POST',
      body: shopData
    })

    // Update user profile
    await $fetch('/api/auth/profile', {
      method: 'PUT',
      body: {
        firstName: formData.value.firstName,
        lastName: formData.value.lastName,
        email: formData.value.email,
        phone: formData.value.phone,
        jobTitle: formData.value.jobTitle,
        department: formData.value.department
      }
    })

    // Save selected modules to local storage
    localStorage.setItem('toss_modules', JSON.stringify(formData.value.selectedModules))
    localStorage.setItem('toss_onboarding_complete', 'true')
    
    // Redirect to dashboard
    await router.push('/')
  } catch (error) {
    console.error('Onboarding error:', error)
    errorMessage.value = error.message || 'There was an error completing onboarding. Please try again.'
  } finally {
    isSubmitting.value = false
  }
}
</script>


