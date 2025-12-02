<template>
  <div class="fixed inset-0 z-50 overflow-y-auto" aria-labelledby="modal-title" role="dialog" aria-modal="true">
    <div class="flex items-end justify-center min-h-screen pt-4 px-4 pb-20 text-center sm:block sm:p-0">
      <div class="fixed inset-0 bg-gray-500 bg-opacity-75 transition-opacity" aria-hidden="true" @click="$emit('close')"></div>

      <span class="hidden sm:inline-block sm:align-middle sm:h-screen" aria-hidden="true">&#8203;</span>

      <div class="relative inline-block align-bottom bg-white dark:bg-gray-800 rounded-lg px-4 pt-5 pb-4 text-left overflow-hidden shadow-xl transform transition-all sm:my-8 sm:align-middle sm:max-w-4xl sm:w-full sm:p-6">
        <div class="sm:flex sm:items-start">
          <div class="w-full">
            <div class="mb-6">
              <h3 class="text-lg leading-6 font-medium text-gray-900 dark:text-white" id="modal-title">
                {{ enterpriseName }} Services
              </h3>
              <p class="mt-2 text-sm text-gray-600 dark:text-gray-400">
                Manage your service offerings and pricing
              </p>
            </div>

            <!-- Services Grid -->
            <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4 mb-6">
              <div
                v-for="service in services"
                :key="service.id"
                class="p-4 border border-gray-200 dark:border-gray-600 rounded-lg hover:shadow-md transition-shadow"
              >
                <div class="flex items-center justify-between mb-3">
                  <h4 class="font-semibold text-gray-900 dark:text-white">{{ service.name }}</h4>
                  <span class="text-lg font-bold text-green-600 dark:text-green-400">
                    {{ formatCurrency(service.basePrice) }}
                  </span>
                </div>
                <p class="text-sm text-gray-600 dark:text-gray-400 mb-3">{{ service.description }}</p>
                <div class="flex items-center justify-between text-xs text-gray-500 dark:text-gray-400">
                  <span class="px-2 py-1 bg-blue-100 dark:bg-blue-900 text-blue-800 dark:text-blue-200 rounded-full">
                    {{ service.category }}
                  </span>
                  <span>{{ formatDuration(service.estimatedDuration) }}</span>
                </div>
                <div class="mt-3 flex space-x-2">
                  <button
                    @click="editService(service)"
                    class="flex-1 px-3 py-1 text-xs font-medium text-blue-600 dark:text-blue-400 border border-blue-200 dark:border-blue-700 rounded hover:bg-blue-50 dark:hover:bg-blue-900/20 transition-colors"
                  >
                    Edit
                  </button>
                  <button
                    @click="bookService(service)"
                    class="flex-1 px-3 py-1 text-xs font-medium text-white bg-blue-600 rounded hover:bg-blue-700 transition-colors"
                  >
                    Book
                  </button>
                </div>
              </div>
            </div>

            <!-- Add New Service -->
            <div class="border-t border-gray-200 dark:border-gray-700 pt-4">
              <button
                @click="showAddService = true"
                class="w-full flex items-center justify-center px-4 py-3 border-2 border-dashed border-gray-300 dark:border-gray-600 rounded-lg text-gray-600 dark:text-gray-400 hover:border-blue-400 hover:text-blue-600 dark:hover:text-blue-400 transition-colors"
              >
                <PlusIcon class="w-5 h-5 mr-2" />
                Add New Service
              </button>
            </div>

            <!-- Service Categories Summary -->
            <div v-if="serviceSummary.length" class="mt-6 border-t border-gray-200 dark:border-gray-700 pt-4">
              <h4 class="font-medium text-gray-900 dark:text-white mb-3">Service Categories</h4>
              <div class="grid grid-cols-2 md:grid-cols-4 gap-3">
                <div
                  v-for="category in serviceSummary"
                  :key="category.name"
                  class="text-center p-3 bg-gray-50 dark:bg-gray-700 rounded-lg"
                >
                  <div class="text-lg font-bold text-gray-900 dark:text-white">{{ category.count }}</div>
                  <div class="text-xs text-gray-600 dark:text-gray-400">{{ category.name }}</div>
                  <div class="text-xs text-green-600 dark:text-green-400">
                    Avg: {{ formatCurrency(category.avgPrice) }}
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <div class="mt-6 sm:mt-6 sm:flex sm:flex-row-reverse">
          <button
            @click="$emit('close')"
            class="w-full inline-flex justify-center rounded-md border border-gray-300 dark:border-gray-600 shadow-sm px-4 py-2 bg-white dark:bg-gray-700 text-base font-medium text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-600 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 sm:w-auto sm:text-sm"
          >
            Close
          </button>
        </div>
      </div>
    </div>

    <!-- Add Service Modal -->
    <div v-if="showAddService" class="fixed inset-0 z-60 overflow-y-auto">
      <div class="flex items-center justify-center min-h-screen p-4">
        <div class="fixed inset-0 bg-black bg-opacity-50" @click="showAddService = false"></div>
        <div class="relative bg-white dark:bg-gray-800 rounded-lg p-6 max-w-md w-full">
          <h4 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">Add New Service</h4>
          <form @submit.prevent="addService" class="space-y-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Service Name</label>
              <input
                v-model="newService.name"
                type="text"
                required
                class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Description</label>
              <textarea
                v-model="newService.description"
                rows="2"
                class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
              ></textarea>
            </div>
            <div class="grid grid-cols-2 gap-3">
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Price (ZAR)</label>
                <input
                  v-model.number="newService.basePrice"
                  type="number"
                  min="0"
                  step="0.01"
                  required
                  class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Duration (min)</label>
                <input
                  v-model.number="newService.estimatedDuration"
                  type="number"
                  min="1"
                  required
                  class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                />
              </div>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Category</label>
              <select
                v-model="newService.category"
                required
                class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
              >
                <option value="">Select category</option>
                <option v-for="category in availableCategories" :key="category" :value="category">
                  {{ category }}
                </option>
              </select>
            </div>
            <div class="flex space-x-3 pt-4">
              <button
                type="button"
                @click="showAddService = false"
                class="flex-1 px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-md text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-700"
              >
                Cancel
              </button>
              <button
                type="submit"
                class="flex-1 px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700"
              >
                Add Service
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useEnterpriseConfig } from '../../composables/useEnterpriseConfig'
import { PlusIcon } from '@heroicons/vue/24/outline'

interface ServiceOffering {
  id: string
  name: string
  description: string
  category: string
  estimatedDuration: number
  basePrice: number
  currency: string
}

const props = defineProps<{
  enterpriseName: string
}>()

const emit = defineEmits<{
  close: []
  serviceBooked: [service: ServiceOffering]
}>()

const { currentEnterprise, getServiceOfferingsForEnterprise } = useEnterpriseConfig()

const showAddService = ref(false)
const services = ref<ServiceOffering[]>([])

const newService = ref({
  name: '',
  description: '',
  category: '',
  estimatedDuration: 60,
  basePrice: 0
})

const availableCategories = computed(() => {
  const categories = [...new Set(services.value.map(s => s.category))]
  return categories.length > 0 ? categories : ['Basic Services', 'Premium Services', 'Special Services']
})

const serviceSummary = computed(() => {
  const categoryMap = new Map()
  
  services.value.forEach(service => {
    if (!categoryMap.has(service.category)) {
      categoryMap.set(service.category, { count: 0, totalPrice: 0 })
    }
    const cat = categoryMap.get(service.category)
    cat.count++
    cat.totalPrice += service.basePrice
  })
  
  return Array.from(categoryMap.entries()).map(([name, data]) => ({
    name,
    count: data.count,
    avgPrice: data.totalPrice / data.count
  }))
})

const formatCurrency = (amount: number): string => {
  return new Intl.NumberFormat('en-ZA', {
    style: 'currency',
    currency: 'ZAR'
  }).format(amount)
}

const formatDuration = (minutes: number): string => {
  if (minutes < 60) return `${minutes}min`
  const hours = Math.floor(minutes / 60)
  const mins = minutes % 60
  return mins > 0 ? `${hours}h ${mins}min` : `${hours}h`
}

const editService = (service: ServiceOffering) => {
  // Implement edit functionality
  console.log('Edit service:', service)
}

const bookService = (service: ServiceOffering) => {
  emit('serviceBooked', service)
}

const addService = () => {
  const service: ServiceOffering = {
    id: Date.now().toString(),
    name: newService.value.name,
    description: newService.value.description,
    category: newService.value.category,
    estimatedDuration: newService.value.estimatedDuration,
    basePrice: newService.value.basePrice,
    currency: 'ZAR'
  }
  
  services.value.push(service)
  
  // Reset form
  newService.value = {
    name: '',
    description: '',
    category: '',
    estimatedDuration: 60,
    basePrice: 0
  }
  
  showAddService.value = false
}

onMounted(() => {
  if (currentEnterprise.value) {
    services.value = getServiceOfferingsForEnterprise.value
  }
})
</script>
