<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900 flex items-center justify-center p-6">
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-lg max-w-2xl w-full p-8">
      <div class="mb-6">
        <h1 class="text-2xl font-bold text-gray-900 dark:text-gray-100">Welcome, Driver!</h1>
        <p class="text-gray-600 dark:text-gray-400 mt-2">Let's set up your driver profile.</p>
      </div>

      <div class="space-y-4">
        <div>
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
            Full Name *
          </label>
          <input
            v-model="form.name"
            type="text"
            required
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500"
          />
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
            Phone Number *
          </label>
          <input
            v-model="form.phone"
            type="tel"
            required
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500"
          />
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
            Vehicle Type *
          </label>
          <select
            v-model="form.vehicleType"
            required
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500"
          >
            <option value="">Select Vehicle Type</option>
            <option value="Motorcycle">Motorcycle</option>
            <option value="Car">Car</option>
            <option value="Van">Van</option>
            <option value="Truck">Truck</option>
            <option value="Bicycle">Bicycle</option>
          </select>
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
      userRole: 'Driver',
      step1Completed: true,
      step2Completed: true,
      step3Completed: true
    })
    await post(`/api/onboarding/${userStore.user?.id}/complete`)
    router.push('/driver/deliveries')
  } catch (error) {
    console.error('Failed to complete onboarding:', error)
    alert('Failed to complete onboarding. Please try again.')
  }
}
</script>

