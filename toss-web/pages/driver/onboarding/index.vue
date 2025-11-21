<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900 flex items-center justify-center p-6">
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-lg max-w-2xl w-full p-8">
      <template>
        <div class="min-h-screen bg-gray-100 dark:bg-gray-900 py-8 px-4">
          <div class="max-w-xl mx-auto">
            <h1 class="text-2xl font-bold mb-6">Driver Onboarding</h1>
            <MaterialStepper :activeStep="step - 1" :steps="['Profile', 'Vehicle', 'Areas', 'Complete']" class="mb-6" />

            <MaterialCard v-if="step === 1" variant="elevated" class="mb-6">
              <h2 class="text-lg font-semibold mb-4">Step 1: Profile</h2>
              <div class="mb-4">
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Full Name</label>
                <MaterialInput v-model="form.name" type="text" placeholder="Enter your name" />
              </div>
              <div class="mb-4">
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Phone Number</label>
                <MaterialInput v-model="form.phone" type="text" placeholder="Enter your phone number" />
              </div>
              <MaterialButton @click="nextStep" color="primary" class="w-full">Next</MaterialButton>
            </MaterialCard>

            <MaterialCard v-else-if="step === 2" variant="elevated" class="mb-6">
              <h2 class="text-lg font-semibold mb-4">Step 2: Vehicle Details</h2>
              <div class="mb-4">
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Vehicle Type</label>
                <MaterialInput v-model="form.vehicleType" type="text" placeholder="e.g. Car, Bike" />
              </div>
              <div class="mb-4">
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Vehicle Registration</label>
                <MaterialInput v-model="form.vehicleReg" type="text" placeholder="Enter registration number" />
              </div>
              <MaterialButton @click="nextStep" color="primary" class="w-full">Next</MaterialButton>
            </MaterialCard>

            <MaterialCard v-else-if="step === 3" variant="elevated" class="mb-6">
              <h2 class="text-lg font-semibold mb-4">Step 3: Delivery Areas</h2>
              <div class="mb-4">
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Service Areas</label>
                <MaterialInput v-model="form.areas" type="text" placeholder="e.g. Soweto, Alexandra" />
              </div>
              <MaterialButton @click="submitOnboarding" color="success" class="w-full">Finish</MaterialButton>
            </MaterialCard>

            <MaterialCard v-else-if="step === 4" variant="elevated" class="bg-green-50 dark:bg-green-900/20">
              <h2 class="text-lg font-semibold mb-4 text-green-800 dark:text-green-200">Onboarding Complete!</h2>
              <p class="mb-4">Thank you for registering as a driver. You can now view your assigned deliveries.</p>
              <MaterialButton to="/driver/deliveries" color="primary" as="NuxtLink" class="w-full">Go to Deliveries</MaterialButton>
            </MaterialCard>
          </div>
        </div>
      </template>
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
            Vehicle Registration *
          </label>
          <input
            v-model="form.vehicleRegistration"
            type="text"
            required
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500"
          />
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
            Typical Delivery Area
          </label>
          <input
            v-model="form.deliveryArea"
            type="text"
            placeholder="e.g., Soweto, Johannesburg"
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500"
          />
        </div>
      </div>

      <div class="flex justify-end mt-8 pt-6 border-t">
        <button
          @click="completeOnboarding"
          :disabled="!canProceed"
          class="px-6 py-2 bg-purple-600 text-white rounded-lg hover:bg-purple-700 disabled:opacity-50"
        >
          Complete Setup
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
    roles: ['Driver']
  }
})

const router = useRouter()
const { put, post } = useApi()
const userStore = useUserStore()

const form = ref({
  name: '',
  phone: '',
  vehicleType: '',
  vehicleRegistration: '',
  deliveryArea: ''
})

const canProceed = computed(() => {
  return form.value.name.trim() !== '' &&
         form.value.phone.trim() !== '' &&
         form.value.vehicleType !== '' &&
         form.value.vehicleRegistration.trim() !== ''
})

const completeOnboarding = async () => {
  try {
    await put(`/api/onboarding/${userStore.user?.id}`, {
      userId: userStore.user?.id,
      role: 'Driver',
      completedSteps: ['step1'],
      currentStep: 1,
      onboardingData: JSON.stringify(form.value)
    })
    await post(`/api/onboarding/${userStore.user?.id}/complete?role=Driver`)
    router.push('/driver/deliveries')
  } catch (error) {
    console.error('Failed to complete onboarding:', error)
    alert('Failed to complete onboarding. Please try again.')
  }
}
</script>

