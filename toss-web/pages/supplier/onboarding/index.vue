<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900 flex items-center justify-center p-6">
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-lg max-w-2xl w-full p-8">
      <div class="mb-6">
        <h1 class="text-2xl font-bold text-gray-900 dark:text-gray-100">Welcome, Supplier!</h1>
        <p class="text-gray-600 dark:text-gray-400 mt-2">Let's set up your supplier profile.</p>
      </div>

      <!-- Step 1: Business Profile -->
      <div v-if="currentStep === 0" class="space-y-4">
        <h2 class="text-lg font-semibold mb-4">Business Profile</h2>
        <div>
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
            Business Name *
          </label>
          <input
            v-model="form.businessName"
            type="text"
            required
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500"
          />
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
            Address
          </label>
          <textarea
            v-model="form.address"
            rows="2"
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500"
          />
        </div>
        <div class="grid grid-cols-2 gap-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
              Phone *
            </label>
            <input
              v-model="form.phone"
              type="tel"
              required
              class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500"
            />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
              Email
            </label>
            <input
              v-model="form.email"
              type="email"
              class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500"
            />
          </div>
        </div>
      </div>

      <!-- Step 2: Product Categories -->
      <div v-if="currentStep === 1" class="space-y-4">
        <h2 class="text-lg font-semibold mb-4">Product Categories</h2>
        <p class="text-sm text-gray-600 dark:text-gray-400 mb-4">
          What categories of products do you supply? (You can add products later)
        </p>
        <div class="space-y-2">
          <label
            v-for="category in availableCategories"
            :key="category"
            class="flex items-center"
          >
            <input
              v-model="form.categories"
              type="checkbox"
              :value="category"
              class="mr-2"
            />
            <span class="text-sm text-gray-700 dark:text-gray-300">{{ category }}</span>
          </label>
        </div>
      </div>

      <!-- Navigation -->
      <div class="flex justify-between mt-8 pt-6 border-t">
        <button
          v-if="currentStep > 0"
          @click="currentStep--"
          class="px-6 py-2 border border-gray-300 rounded-lg hover:bg-gray-50"
        >
          Previous
        </button>
        <div v-else></div>
        <button
          @click="handleNext"
          :disabled="!canProceed"
          class="px-6 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 disabled:opacity-50"
        >
          {{ currentStep === 1 ? 'Complete' : 'Next' }}
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: false,
  middleware: 'auth',
  meta: {
    roles: ['Supplier']
  }
})

const router = useRouter()
const { put, post } = useApi()
const userStore = useUserStore()

const currentStep = ref(0)
const availableCategories = ['Groceries', 'Beverages', 'Snacks', 'Household', 'Personal Care', 'Other']

const form = ref({
  businessName: '',
  address: '',
  phone: '',
  email: '',
  categories: [] as string[]
})

const canProceed = computed(() => {
  if (currentStep.value === 0) {
    return form.value.businessName.trim() !== '' && form.value.phone.trim() !== ''
  }
  return true
})

const handleNext = async () => {
  if (currentStep.value === 1) {
    await completeOnboarding()
  } else {
    await saveStepProgress()
    currentStep.value++
  }
}

const saveStepProgress = async () => {
  try {
    const completedSteps: string[] = []
    if (currentStep.value >= 0) completedSteps.push('step1')
    if (currentStep.value >= 1) completedSteps.push('step2')

    await put(`/api/onboarding/${userStore.user?.id}`, {
      userId: userStore.user?.id,
      role: 'Supplier',
      completedSteps,
      currentStep: currentStep.value,
      onboardingData: JSON.stringify(form.value)
    })
  } catch (error) {
    console.error('Failed to save onboarding progress:', error)
  }
}

const completeOnboarding = async () => {
  try {
    await saveStepProgress()
    await post(`/api/onboarding/${userStore.user?.id}/complete?role=Supplier`)
    router.push('/supplier/dashboard')
  } catch (error) {
    console.error('Failed to complete onboarding:', error)
    alert('Failed to complete onboarding. Please try again.')
  }
}
</script>

