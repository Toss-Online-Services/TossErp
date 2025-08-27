<template>
  <div class="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100 dark:from-gray-900 dark:to-gray-800">
    <div class="container mx-auto px-4 py-8">
      <!-- Header -->
      <div class="text-center mb-12">
        <h1 class="text-4xl font-bold text-gray-900 dark:text-white mb-4">
          Welcome to TOSS ERP III
        </h1>
        <p class="text-xl text-gray-600 dark:text-gray-300 max-w-3xl mx-auto">
          Service as Software platform designed specifically for South African rural township enterprises. 
          Get a CRM system tailored to your business type.
        </p>
      </div>

      <!-- Enterprise Type Selection -->
      <div v-if="!selectedEnterpriseType" class="max-w-6xl mx-auto">
        <h2 class="text-2xl font-semibold text-gray-900 dark:text-white text-center mb-8">
          What type of business do you run?
        </h2>
        
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 mb-8">
          <div
            v-for="enterprise in featuredEnterprises"
            :key="enterprise.id"
            @click="selectEnterpriseType(enterprise.id)"
            class="cursor-pointer bg-white dark:bg-gray-800 rounded-xl shadow-lg hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1 p-6 border border-gray-200 dark:border-gray-700"
          >
            <div class="flex items-center mb-4">
              <div class="w-12 h-12 bg-blue-100 dark:bg-blue-900 rounded-lg flex items-center justify-center mr-4">
                <component :is="getEnterpriseIcon(enterprise.type)" class="w-6 h-6 text-blue-600 dark:text-blue-400" />
              </div>
              <div>
                <h3 class="text-lg font-semibold text-gray-900 dark:text-white">{{ enterprise.name }}</h3>
                <p class="text-sm text-gray-600 dark:text-gray-400">{{ enterprise.sector }}</p>
              </div>
            </div>
            <p class="text-sm text-gray-700 dark:text-gray-300 mb-4">{{ enterprise.description }}</p>
            <div class="space-y-2">
              <div class="flex items-center text-xs text-gray-600 dark:text-gray-400">
                <CheckIcon class="w-4 h-4 text-green-500 mr-2" />
                Custom fields for your business
              </div>
              <div class="flex items-center text-xs text-gray-600 dark:text-gray-400">
                <CheckIcon class="w-4 h-4 text-green-500 mr-2" />
                {{ enterprise.serviceCount }} pre-configured services
              </div>
              <div class="flex items-center text-xs text-gray-600 dark:text-gray-400">
                <CheckIcon class="w-4 h-4 text-green-500 mr-2" />
                Automation templates included
              </div>
            </div>
          </div>
        </div>

        <!-- All Enterprise Types -->
        <div class="text-center">
          <button
            @click="showAllEnterprises = !showAllEnterprises"
            class="text-blue-600 dark:text-blue-400 hover:text-blue-700 dark:hover:text-blue-300 font-medium"
          >
            {{ showAllEnterprises ? 'Show Less' : 'See All Business Types' }} ({{ allEnterpriseTypes.length }} total)
          </button>
        </div>

        <div v-if="showAllEnterprises" class="mt-6 grid grid-cols-2 md:grid-cols-4 lg:grid-cols-6 gap-3">
          <button
            v-for="enterprise in allEnterpriseTypes"
            :key="enterprise.id"
            @click="selectEnterpriseType(enterprise.id)"
            class="p-3 text-left bg-white dark:bg-gray-800 rounded-lg border border-gray-200 dark:border-gray-700 hover:border-blue-300 dark:hover:border-blue-600 hover:shadow-md transition-all"
          >
            <div class="text-sm font-medium text-gray-900 dark:text-white">{{ enterprise.name }}</div>
            <div class="text-xs text-gray-600 dark:text-gray-400">{{ enterprise.sector }}</div>
          </button>
        </div>
      </div>

      <!-- Configuration Preview -->
      <div v-if="selectedEnterpriseType" class="max-w-4xl mx-auto">
        <div class="bg-white dark:bg-gray-800 rounded-xl shadow-xl p-8">
          <div class="text-center mb-8">
            <div class="w-16 h-16 bg-blue-100 dark:bg-blue-900 rounded-full flex items-center justify-center mx-auto mb-4">
              <component :is="getEnterpriseIcon(selectedEnterprise?.type || '')" class="w-8 h-8 text-blue-600 dark:text-blue-400" />
            </div>
            <h2 class="text-2xl font-bold text-gray-900 dark:text-white mb-2">
              {{ selectedEnterprise?.name }} CRM Configuration
            </h2>
            <p class="text-gray-600 dark:text-gray-400">
              Your personalized CRM system is ready to be configured
            </p>
          </div>

          <!-- Configuration Details -->
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6 mb-8">
            <!-- Custom Fields -->
            <div class="bg-gray-50 dark:bg-gray-700 rounded-lg p-4">
              <h3 class="font-semibold text-gray-900 dark:text-white mb-3 flex items-center">
                <DocumentTextIcon class="w-5 h-5 mr-2 text-blue-600 dark:text-blue-400" />
                Custom Fields ({{ selectedEnterpriseConfig?.customFields.length }})
              </h3>
              <div class="space-y-2">
                <div
                  v-for="field in selectedEnterpriseConfig?.customFields.slice(0, 4)"
                  :key="field.id"
                  class="flex items-center text-sm text-gray-700 dark:text-gray-300"
                >
                  <div class="w-2 h-2 bg-blue-500 rounded-full mr-2"></div>
                  {{ field.name }} ({{ field.type }})
                </div>
                <div v-if="selectedEnterpriseConfig?.customFields && selectedEnterpriseConfig.customFields.length > 4" class="text-xs text-gray-500">
                  +{{ selectedEnterpriseConfig.customFields.length - 4 }} more fields...
                </div>
              </div>
            </div>

            <!-- Service Offerings -->
            <div class="bg-gray-50 dark:bg-gray-700 rounded-lg p-4">
              <h3 class="font-semibold text-gray-900 dark:text-white mb-3 flex items-center">
                <WrenchScrewdriverIcon class="w-5 h-5 mr-2 text-green-600 dark:text-green-400" />
                Services ({{ selectedEnterpriseConfig?.serviceOfferings.length }})
              </h3>
              <div class="space-y-2">
                <div
                  v-for="service in selectedEnterpriseConfig?.serviceOfferings.slice(0, 4)"
                  :key="service.id"
                  class="flex items-center justify-between text-sm"
                >
                  <span class="text-gray-700 dark:text-gray-300">{{ service.name }}</span>
                  <span class="text-green-600 dark:text-green-400 font-medium">R{{ service.basePrice }}</span>
                </div>
                <div v-if="selectedEnterpriseConfig?.serviceOfferings && selectedEnterpriseConfig.serviceOfferings.length > 4" class="text-xs text-gray-500">
                  +{{ selectedEnterpriseConfig.serviceOfferings.length - 4 }} more services...
                </div>
              </div>
            </div>

            <!-- Contact Types -->
            <div class="bg-gray-50 dark:bg-gray-700 rounded-lg p-4">
              <h3 class="font-semibold text-gray-900 dark:text-white mb-3 flex items-center">
                <UsersIcon class="w-5 h-5 mr-2 text-purple-600 dark:text-purple-400" />
                Contact Types ({{ selectedEnterpriseConfig?.contactTypes.length }})
              </h3>
              <div class="space-y-2">
                <div
                  v-for="contactType in selectedEnterpriseConfig?.contactTypes"
                  :key="contactType.id"
                  class="flex items-center text-sm text-gray-700 dark:text-gray-300"
                >
                  <div :class="`w-2 h-2 rounded-full mr-2 bg-${contactType.color}-500`"></div>
                  {{ contactType.name }}
                </div>
              </div>
            </div>

            <!-- Business Hours -->
            <div class="bg-gray-50 dark:bg-gray-700 rounded-lg p-4">
              <h3 class="font-semibold text-gray-900 dark:text-white mb-3 flex items-center">
                <ClockIcon class="w-5 h-5 mr-2 text-orange-600 dark:text-orange-400" />
                Business Hours
              </h3>
              <div class="space-y-1 text-sm text-gray-700 dark:text-gray-300">
                <div class="flex justify-between">
                  <span>Monday - Friday:</span>
                  <span>{{ selectedEnterpriseConfig?.businessHours.monday.open }} - {{ selectedEnterpriseConfig?.businessHours.friday.close }}</span>
                </div>
                <div class="flex justify-between">
                  <span>Saturday:</span>
                  <span>{{ selectedEnterpriseConfig?.businessHours.saturday.open }} - {{ selectedEnterpriseConfig?.businessHours.saturday.close }}</span>
                </div>
                <div class="flex justify-between">
                  <span>Sunday:</span>
                  <span>{{ selectedEnterpriseConfig?.businessHours.sunday.open }} - {{ selectedEnterpriseConfig?.businessHours.sunday.close }}</span>
                </div>
              </div>
            </div>
          </div>

          <!-- Action Buttons -->
          <div class="flex space-x-4 justify-center">
            <button
              @click="selectedEnterpriseType = null"
              class="px-6 py-3 border border-gray-300 dark:border-gray-600 rounded-lg text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
            >
              Choose Different Business
            </button>
            <button
              @click="startWithConfiguration"
              class="px-8 py-3 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors font-semibold"
            >
              Start With This Configuration
            </button>
          </div>
        </div>
      </div>

      <!-- Features Overview -->
      <div v-if="!selectedEnterpriseType" class="max-w-6xl mx-auto mt-16">
        <h2 class="text-2xl font-semibold text-gray-900 dark:text-white text-center mb-8">
          Why Choose TOSS ERP III for Your Township Enterprise?
        </h2>
        
        <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
          <div class="text-center p-6">
            <div class="w-12 h-12 bg-blue-100 dark:bg-blue-900 rounded-lg flex items-center justify-center mx-auto mb-4">
              <CogIcon class="w-6 h-6 text-blue-600 dark:text-blue-400" />
            </div>
            <h3 class="font-semibold text-gray-900 dark:text-white mb-2">Business-Specific</h3>
            <p class="text-sm text-gray-600 dark:text-gray-400">
              Each business type gets custom fields, workflows, and features designed for their specific needs
            </p>
          </div>
          <div class="text-center p-6">
            <div class="w-12 h-12 bg-green-100 dark:bg-green-900 rounded-lg flex items-center justify-center mx-auto mb-4">
              <ChartBarIcon class="w-6 h-6 text-green-600 dark:text-green-400" />
            </div>
            <h3 class="font-semibold text-gray-900 dark:text-white mb-2">Built for Townships</h3>
            <p class="text-sm text-gray-600 dark:text-gray-400">
              Designed for South African rural enterprises with local payment methods, languages, and business practices
            </p>
          </div>
          <div class="text-center p-6">
            <div class="w-12 h-12 bg-purple-100 dark:bg-purple-900 rounded-lg flex items-center justify-center mx-auto mb-4">
              <BoltIcon class="w-6 h-6 text-purple-600 dark:text-purple-400" />
            </div>
            <h3 class="font-semibold text-gray-900 dark:text-white mb-2">Automation Ready</h3>
            <p class="text-sm text-gray-600 dark:text-gray-400">
              Pre-built automation templates for common business processes in your industry
            </p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useEnterpriseConfig } from '../composables/useEnterpriseConfig'
import { useEnterpriseData } from '../composables/useEnterpriseData'
import {
  CheckIcon,
  DocumentTextIcon,
  WrenchScrewdriverIcon,
  UsersIcon,
  ClockIcon,
  CogIcon,
  ChartBarIcon,
  BoltIcon,
  ScissorsIcon,
  ShoppingBagIcon,
  BuildingStorefrontIcon,
  TruckIcon,
  UserGroupIcon,
  HeartIcon,
  AcademicCapIcon,
  HomeIcon
} from '@heroicons/vue/24/outline'

const { getAvailableEnterpriseTypes, enterpriseConfigs, setCurrentEnterprise } = useEnterpriseConfig()
const { allEnterprises } = useEnterpriseData()

const selectedEnterpriseType = ref<string | null>(null)
const showAllEnterprises = ref(false)

const allEnterpriseTypes = computed(() => {
  return allEnterprises.value.map(enterprise => ({
    id: enterprise.enterprise.toLowerCase().replace(/\s+/g, '_').replace(/[^\w]/g, ''),
    name: enterprise.enterprise,
    sector: enterprise.sector,
    description: `Manage your ${enterprise.enterprise.toLowerCase()} business with specialized tools`
  }))
})

const featuredEnterprises = computed(() => {
  return [
    {
      id: 'beauty_salon',
      name: 'Hair & Beauty Salon',
      type: 'Hair salons / barbershops',
      sector: 'Personal & Household Services',
      description: 'Manage client appointments, beauty services, loyalty programs, and specialist treatments',
      serviceCount: 6
    },
    {
      id: 'plumbing_service',
      name: 'Plumbing Services',
      type: 'Plumbers',
      sector: 'Trades & Technical Services', 
      description: 'Handle emergency calls, schedule maintenance, track service history, and manage quotes',
      serviceCount: 6
    },
    {
      id: 'spaza_shop',
      name: 'Spaza Shop',
      type: 'Spaza / Convenience shop',
      sector: 'Retail & Trade',
      description: 'Manage inventory, customer credit, loyalty programs, and local delivery services',
      serviceCount: 4
    }
  ]
})

const selectedEnterprise = computed(() => {
  return featuredEnterprises.value.find(e => e.id === selectedEnterpriseType.value) ||
         allEnterpriseTypes.value.find(e => e.id === selectedEnterpriseType.value)
})

const selectedEnterpriseConfig = computed(() => {
  return selectedEnterpriseType.value ? enterpriseConfigs[selectedEnterpriseType.value] : null
})

const getEnterpriseIcon = (enterpriseType: string) => {
  const iconMap: Record<string, any> = {
    'Hair salons / barbershops': ScissorsIcon,
    'Plumbers': WrenchScrewdriverIcon,
    'Spaza / Convenience shop': ShoppingBagIcon,
    'Electricians': WrenchScrewdriverIcon,
    'Mechanics (vehicle repairs)': TruckIcon,
    'Mobile car wash': TruckIcon,
    'Daycare / ECD centres / crèches': UserGroupIcon,
    'Township restaurant / Imbizo / braai stalls': BuildingStorefrontIcon,
    'Agent banking / airtime & voucher shops': BuildingStorefrontIcon
  }
  return iconMap[enterpriseType] || BuildingStorefrontIcon
}

const selectEnterpriseType = (enterpriseId: string) => {
  selectedEnterpriseType.value = enterpriseId
}

const startWithConfiguration = () => {
  if (selectedEnterpriseType.value) {
    // Set the enterprise configuration
    setCurrentEnterprise(selectedEnterpriseType.value)
    
    // Navigate to the CRM contacts page
    window.location.href = '/crm/contacts'
  }
}
</script>
