<template>
  <div class="enterprise-selector">
    <!-- Current Enterprise Display -->
    <div v-if="currentEnterprise" class="current-enterprise mb-6 p-4 bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
      <div class="flex items-center justify-between">
        <div class="flex items-center space-x-3">
          <div class="w-12 h-12 bg-blue-100 dark:bg-blue-900 rounded-lg flex items-center justify-center">
            <component :is="getEnterpriseIcon(currentEnterprise.type)" class="w-6 h-6 text-blue-600 dark:text-blue-400" />
          </div>
          <div>
            <h3 class="text-lg font-semibold text-gray-900 dark:text-white">{{ currentEnterprise.name }}</h3>
            <p class="text-sm text-gray-600 dark:text-gray-400">{{ currentEnterprise.sector }}</p>
          </div>
        </div>
        <button
          @click="showSelector = true"
          class="px-4 py-2 text-sm font-medium text-blue-600 dark:text-blue-400 hover:text-blue-700 dark:hover:text-blue-300 border border-blue-200 dark:border-blue-700 rounded-md hover:bg-blue-50 dark:hover:bg-blue-900/20 transition-colors"
        >
          Change Enterprise Type
        </button>
      </div>
    </div>

    <!-- Enterprise Selector Modal -->
    <div v-if="showSelector || !currentEnterprise" class="fixed inset-0 z-50 overflow-y-auto" aria-labelledby="modal-title" role="dialog" aria-modal="true">
      <div class="flex items-end justify-center min-h-screen pt-4 px-4 pb-20 text-center sm:block sm:p-0">
        <div class="fixed inset-0 bg-gray-500 bg-opacity-75 transition-opacity" aria-hidden="true" @click="showSelector = false"></div>

        <span class="hidden sm:inline-block sm:align-middle sm:h-screen" aria-hidden="true">&#8203;</span>

        <div class="relative inline-block align-bottom bg-white dark:bg-gray-800 rounded-lg px-4 pt-5 pb-4 text-left overflow-hidden shadow-xl transform transition-all sm:my-8 sm:align-middle sm:max-w-4xl sm:w-full sm:p-6">
          <div class="sm:flex sm:items-start">
            <div class="w-full">
              <div class="mb-6">
                <h3 class="text-lg leading-6 font-medium text-gray-900 dark:text-white" id="modal-title">
                  Select Your Enterprise Type
                </h3>
                <p class="mt-2 text-sm text-gray-600 dark:text-gray-400">
                  Choose the type of business you run to customize your CRM experience
                </p>
              </div>

              <!-- Enterprise Type Grid -->
              <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4 mb-6">
                <div
                  v-for="enterprise in availableEnterprises"
                  :key="enterprise.id"
                  @click="selectEnterprise(enterprise.id)"
                  class="cursor-pointer p-4 border-2 border-gray-200 dark:border-gray-600 rounded-lg hover:border-blue-500 dark:hover:border-blue-400 hover:shadow-md transition-all"
                  :class="{ 'border-blue-500 dark:border-blue-400 bg-blue-50 dark:bg-blue-900/20': selectedEnterpriseId === enterprise.id }"
                >
                  <div class="flex items-center space-x-3 mb-3">
                    <div class="w-10 h-10 bg-blue-100 dark:bg-blue-900 rounded-lg flex items-center justify-center">
                      <component :is="getEnterpriseIcon(enterprise.type)" class="w-5 h-5 text-blue-600 dark:text-blue-400" />
                    </div>
                    <div>
                      <h4 class="font-semibold text-gray-900 dark:text-white">{{ enterprise.name }}</h4>
                      <p class="text-xs text-gray-600 dark:text-gray-400">{{ enterprise.sector }}</p>
                    </div>
                  </div>
                  <p class="text-sm text-gray-700 dark:text-gray-300">{{ getEnterpriseDescription(enterprise.id) }}</p>
                </div>
              </div>

              <!-- Preview Selected Enterprise -->
              <div v-if="selectedEnterpriseId" class="border-t border-gray-200 dark:border-gray-700 pt-6">
                <h4 class="font-medium text-gray-900 dark:text-white mb-4">What you'll get with {{ getSelectedEnterpriseName() }}:</h4>
                <div class="grid grid-cols-1 md:grid-cols-2 gap-4 text-sm">
                  <div>
                    <h5 class="font-medium text-gray-700 dark:text-gray-300 mb-2">Custom Fields:</h5>
                    <ul class="space-y-1 text-gray-600 dark:text-gray-400">
                      <li v-for="field in getSelectedEnterpriseConfig()?.customFields.slice(0, 3)" :key="field.id" class="flex items-center">
                        <CheckIcon class="w-4 h-4 text-green-500 mr-2" />
                        {{ field.name }}
                      </li>
                      <li v-if="getSelectedEnterpriseConfig()?.customFields.length > 3" class="text-gray-500">
                        +{{ getSelectedEnterpriseConfig()?.customFields.length - 3 }} more fields...
                      </li>
                    </ul>
                  </div>
                  <div>
                    <h5 class="font-medium text-gray-700 dark:text-gray-300 mb-2">Service Offerings:</h5>
                    <ul class="space-y-1 text-gray-600 dark:text-gray-400">
                      <li v-for="service in getSelectedEnterpriseConfig()?.serviceOfferings.slice(0, 3)" :key="service.id" class="flex items-center">
                        <CheckIcon class="w-4 h-4 text-green-500 mr-2" />
                        {{ service.name }}
                      </li>
                      <li v-if="getSelectedEnterpriseConfig()?.serviceOfferings.length > 3" class="text-gray-500">
                        +{{ getSelectedEnterpriseConfig()?.serviceOfferings.length - 3 }} more services...
                      </li>
                    </ul>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div class="mt-6 sm:mt-6 sm:flex sm:flex-row-reverse">
            <button
              @click="confirmSelection"
              :disabled="!selectedEnterpriseId"
              class="w-full inline-flex justify-center rounded-md border border-transparent shadow-sm px-4 py-2 bg-blue-600 text-base font-medium text-white hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 sm:ml-3 sm:w-auto sm:text-sm disabled:opacity-50 disabled:cursor-not-allowed"
            >
              Apply Configuration
            </button>
            <button
              v-if="currentEnterprise"
              @click="showSelector = false"
              class="mt-3 w-full inline-flex justify-center rounded-md border border-gray-300 dark:border-gray-600 shadow-sm px-4 py-2 bg-white dark:bg-gray-700 text-base font-medium text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-600 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 sm:mt-0 sm:w-auto sm:text-sm"
            >
              Cancel
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useEnterpriseConfig } from '@/composables/useEnterpriseConfig'
import { 
  CheckIcon,
  ScissorsIcon,
  WrenchScrewdriverIcon,
  ShoppingBagIcon,
  BuildingStorefrontIcon,
  TruckIcon,
  UserGroupIcon,
  HeartIcon,
  AcademicCapIcon,
  HomeIcon
} from '@heroicons/vue/24/outline'

const {
  currentEnterprise,
  setCurrentEnterprise,
  loadEnterpriseFromStorage,
  getAvailableEnterpriseTypes,
  enterpriseConfigs
} = useEnterpriseConfig()

const showSelector = ref(false)
const selectedEnterpriseId = ref<string>('')

const availableEnterprises = computed(() => getAvailableEnterpriseTypes.value)

const getEnterpriseIcon = (enterpriseType: string) => {
  const iconMap: Record<string, any> = {
    'Hair salon': ScissorsIcon,
    'Plumbers': WrenchScrewdriverIcon,
    'Spaza / Convenience shop': ShoppingBagIcon,
    'General shop': BuildingStorefrontIcon,
    'Taxi services': TruckIcon,
    'Hairdresser': ScissorsIcon,
    'Crèche': UserGroupIcon,
    'Traditional healer': HeartIcon,
    'Tutoring': AcademicCapIcon,
    'Accommodation': HomeIcon
  }
  return iconMap[enterpriseType] || BuildingStorefrontIcon
}

const getEnterpriseDescription = (enterpriseId: string) => {
  const descriptions: Record<string, string> = {
    beauty_salon: 'Manage appointments, track client preferences, loyalty programs, and specialized beauty services',
    plumbing_service: 'Handle emergency calls, schedule maintenance, track service history, and manage quotes',
    spaza_shop: 'Manage inventory, customer credit, loyalty programs, and local delivery services'
  }
  return descriptions[enterpriseId] || 'Customize your CRM for your specific business needs'
}

const getSelectedEnterpriseName = () => {
  const selected = availableEnterprises.value.find(e => e.id === selectedEnterpriseId.value)
  return selected?.name || ''
}

const getSelectedEnterpriseConfig = () => {
  return enterpriseConfigs[selectedEnterpriseId.value]
}

const selectEnterprise = (enterpriseId: string) => {
  selectedEnterpriseId.value = enterpriseId
}

const confirmSelection = () => {
  if (selectedEnterpriseId.value) {
    setCurrentEnterprise(selectedEnterpriseId.value)
    showSelector.value = false
    
    // Emit event to parent components
    emit('enterpriseChanged', selectedEnterpriseId.value)
    
    // Show success message
    window.dispatchEvent(new CustomEvent('show-notification', {
      detail: {
        type: 'success',
        title: 'Enterprise Type Updated',
        message: `Your CRM is now configured for ${getSelectedEnterpriseName()}`
      }
    }))
  }
}

const emit = defineEmits<{
  enterpriseChanged: [enterpriseType: string]
}>()

onMounted(() => {
  loadEnterpriseFromStorage()
  if (!currentEnterprise.value) {
    showSelector.value = true
  }
})
</script>
