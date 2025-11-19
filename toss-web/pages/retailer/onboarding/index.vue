<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900 flex items-center justify-center p-6">
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-lg max-w-2xl w-full p-8">
      <div class="mb-6">
        <h1 class="text-2xl font-bold text-gray-900 dark:text-gray-100">Welcome to TOSS!</h1>
        <p class="text-gray-600 dark:text-gray-400 mt-2">Let's set up your store in a few quick steps.</p>
      </div>

      <!-- Progress Indicator -->
      <div class="mb-8">
        <div class="flex items-center justify-between">
          <div
            v-for="(step, index) in steps"
            :key="index"
            class="flex items-center flex-1"
          >
            <div
              :class="[
                'w-8 h-8 rounded-full flex items-center justify-center text-sm font-semibold',
                currentStep > index
                  ? 'bg-green-600 text-white'
                  : currentStep === index
                    ? 'bg-blue-600 text-white'
                    : 'bg-gray-200 text-gray-600'
              ]"
            >
              {{ currentStep > index ? '✓' : index + 1 }}
            </div>
            <div
              v-if="index < steps.length - 1"
              :class="[
                'flex-1 h-1 mx-2',
                currentStep > index ? 'bg-green-600' : 'bg-gray-200'
              ]"
            />
          </div>
        </div>
        <div class="flex justify-between mt-2">
          <span
            v-for="(step, index) in steps"
            :key="index"
            :class="[
              'text-xs',
              currentStep === index ? 'font-semibold text-blue-600' : 'text-gray-500'
            ]"
          >
            {{ step.title }}
          </span>
        </div>
      </div>

      <!-- Step 1: Business Profile -->
      <div v-if="currentStep === 0" class="space-y-4">
        <h2 class="text-lg font-semibold mb-4">Step 1: Business Profile</h2>
        <div>
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
            Store Name *
          </label>
          <input
            v-model="form.storeName"
            type="text"
            required
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
          />
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
            Address
          </label>
          <textarea
            v-model="form.address"
            rows="2"
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
          />
        </div>
        <div class="grid grid-cols-2 gap-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
              Phone
            </label>
            <input
              v-model="form.phone"
              type="tel"
              class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
            />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
              Email
            </label>
            <input
              v-model="form.email"
              type="email"
              class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
            />
          </div>
        </div>
      </div>

      <!-- Step 2: Add Products -->
      <div v-if="currentStep === 1" class="space-y-4">
        <h2 class="text-lg font-semibold mb-4">Step 2: Add Your First Products</h2>
        <p class="text-sm text-gray-600 dark:text-gray-400 mb-4">
          Add at least one product to get started. You can add more later.
        </p>
        <div
          v-for="(product, index) in form.products"
          :key="index"
          class="p-4 border border-gray-200 rounded-lg space-y-3"
        >
          <div class="grid grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                Product Name *
              </label>
              <input
                v-model="product.name"
                type="text"
                required
                class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                Price (R) *
              </label>
              <input
                v-model.number="product.price"
                type="number"
                step="0.01"
                min="0"
                required
                class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
              />
            </div>
          </div>
          <button
            v-if="form.products.length > 1"
            type="button"
            @click="removeProduct(index)"
            class="text-sm text-red-600 hover:text-red-800"
          >
            Remove
          </button>
        </div>
        <button
          type="button"
          @click="addProduct"
          class="text-sm text-blue-600 hover:text-blue-800"
        >
          + Add Another Product
        </button>
      </div>

      <!-- Step 3: Optional - Invite Staff -->
      <div v-if="currentStep === 2" class="space-y-4">
        <h2 class="text-lg font-semibold mb-4">Step 3: Invite Staff (Optional)</h2>
        <p class="text-sm text-gray-600 dark:text-gray-400 mb-4">
          You can invite staff members or drivers later. Click "Skip" to continue.
        </p>
        <div class="p-4 bg-blue-50 dark:bg-blue-900/20 rounded-lg">
          <p class="text-sm text-blue-800 dark:text-blue-200">
            Staff invitations can be managed from your dashboard after setup.
          </p>
        </div>
      </div>

      <!-- Navigation Buttons -->
      <div class="flex justify-between mt-8 pt-6 border-t">
        <button
          v-if="currentStep > 0"
          @click="previousStep"
          class="px-6 py-2 border border-gray-300 rounded-lg hover:bg-gray-50"
        >
          Previous
        </button>
        <div v-else></div>
        <div class="flex space-x-4">
          <button
            v-if="currentStep === 2"
            @click="skipOnboarding"
            class="px-6 py-2 border border-gray-300 rounded-lg hover:bg-gray-50"
          >
            Skip
          </button>
          <button
            @click="nextStep"
            :disabled="!canProceed"
            class="px-6 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50"
          >
            {{ currentStep === steps.length - 1 ? 'Complete' : 'Next' }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: false,
  middleware: 'auth',
  meta: {
    roles: ['StoreOwner', 'Vendor']
  }
})

const router = useRouter()
const { put, post } = useApi()
const userStore = useUserStore()

const currentStep = ref(0)
const steps = [
  { title: 'Profile', completed: false },
  { title: 'Products', completed: false },
  { title: 'Staff', completed: false }
]

const form = ref({
  storeName: '',
  address: '',
  phone: '',
  email: '',
  products: [
    { name: '', price: 0 }
  ]
})

const canProceed = computed(() => {
  if (currentStep.value === 0) {
    return form.value.storeName.trim() !== ''
  }
  if (currentStep.value === 1) {
    return form.value.products.some(p => p.name.trim() !== '' && p.price > 0)
  }
  return true
})

const addProduct = () => {
  form.value.products.push({ name: '', price: 0 })
}

const removeProduct = (index: number) => {
  if (form.value.products.length > 1) {
    form.value.products.splice(index, 1)
  }
}

const nextStep = async () => {
  if (currentStep.value === steps.length - 1) {
    await completeOnboarding()
  } else {
    // Save current step progress
    await saveStepProgress()
    currentStep.value++
  }
}

const previousStep = () => {
  if (currentStep.value > 0) {
    currentStep.value--
  }
}

const saveStepProgress = async () => {
  try {
    await put(`/api/onboarding/${userStore.user?.id}`, {
      userId: userStore.user?.id,
      userRole: 'Retailer',
      step1Completed: currentStep.value >= 0,
      step2Completed: currentStep.value >= 1,
      step3Completed: currentStep.value >= 2
    })
  } catch (error) {
    console.error('Failed to save onboarding progress:', error)
  }
}

const completeOnboarding = async () => {
  try {
    // Save final step
    await saveStepProgress()

    // Mark onboarding as complete
    await post(`/api/onboarding/${userStore.user?.id}/complete`)

    // TODO: Save store profile and products to backend
    // For now, just redirect
    router.push('/retailer/dashboard')
  } catch (error) {
    console.error('Failed to complete onboarding:', error)
    alert('Failed to complete onboarding. Please try again.')
  }
}

const skipOnboarding = async () => {
  await completeOnboarding()
}
</script>

