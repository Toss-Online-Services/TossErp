<template>
  <div class="p-6">
    <div class="flex items-center justify-between mb-6">
      <div>
        <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
          Sales Territories
        </h1>
        <p class="text-gray-600 dark:text-gray-400">
          Manage sales regions, coverage areas and territory assignments
        </p>
      </div>
      <button class="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg">
        <Icon name="heroicons:plus" class="w-4 h-4 mr-2" />
        Add Territory
      </button>
    </div>

    <!-- Territory Stats -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm">
        <div class="flex items-center">
          <div class="p-2 bg-green-100 rounded-full">
            <Icon name="heroicons:map" class="w-5 h-5 text-green-600" />
          </div>
          <div class="ml-3">
            <p class="text-sm text-gray-600 dark:text-gray-400">Total Territories</p>
            <p class="text-lg font-semibold text-gray-900 dark:text-white">18</p>
          </div>
        </div>
      </div>
      
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm">
        <div class="flex items-center">
          <div class="p-2 bg-blue-100 rounded-full">
            <Icon name="heroicons:users" class="w-5 h-5 text-blue-600" />
          </div>
          <div class="ml-3">
            <p class="text-sm text-gray-600 dark:text-gray-400">Active Coverage</p>
            <p class="text-lg font-semibold text-gray-900 dark:text-white">89%</p>
          </div>
        </div>
      </div>
      
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm">
        <div class="flex items-center">
          <div class="p-2 bg-purple-100 rounded-full">
            <Icon name="heroicons:banknotes" class="w-5 h-5 text-purple-600" />
          </div>
          <div class="ml-3">
            <p class="text-sm text-gray-600 dark:text-gray-400">Total Revenue</p>
            <p class="text-lg font-semibold text-gray-900 dark:text-white">R 1.2M</p>
          </div>
        </div>
      </div>
      
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm">
        <div class="flex items-center">
          <div class="p-2 bg-orange-100 rounded-full">
            <Icon name="heroicons:chart-bar" class="w-5 h-5 text-orange-600" />
          </div>
          <div class="ml-3">
            <p class="text-sm text-gray-600 dark:text-gray-400">Avg Performance</p>
            <p class="text-lg font-semibold text-gray-900 dark:text-white">92.3%</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Territory Map & List -->
    <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
      <!-- Map Placeholder -->
      <div class="lg:col-span-2">
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm p-6">
          <h2 class="text-lg font-medium text-gray-900 dark:text-white mb-4">
            Territory Coverage Map
          </h2>
          
          <!-- Simple Map Placeholder -->
          <div class="h-96 bg-gray-100 dark:bg-gray-700 rounded-lg flex items-center justify-center">
            <div class="text-center">
              <Icon name="heroicons:map" class="w-16 h-16 text-gray-400 mx-auto mb-4" />
              <p class="text-gray-600 dark:text-gray-400">Interactive territory map</p>
              <p class="text-sm text-gray-500 dark:text-gray-500">Showing Gauteng coverage areas</p>
            </div>
          </div>
          
          <!-- Quick Territory Legend -->
          <div class="mt-4 flex flex-wrap gap-3">
            <div class="flex items-center">
              <div class="w-3 h-3 bg-green-500 rounded mr-2"></div>
              <span class="text-sm text-gray-600 dark:text-gray-400">High Performance</span>
            </div>
            <div class="flex items-center">
              <div class="w-3 h-3 bg-yellow-500 rounded mr-2"></div>
              <span class="text-sm text-gray-600 dark:text-gray-400">Moderate Performance</span>
            </div>
            <div class="flex items-center">
              <div class="w-3 h-3 bg-red-500 rounded mr-2"></div>
              <span class="text-sm text-gray-600 dark:text-gray-400">Needs Attention</span>
            </div>
            <div class="flex items-center">
              <div class="w-3 h-3 bg-gray-400 rounded mr-2"></div>
              <span class="text-sm text-gray-600 dark:text-gray-400">Unassigned</span>
            </div>
          </div>
        </div>
      </div>

      <!-- Territory Summary -->
      <div class="space-y-6">
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm p-6">
          <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-4">
            Territory Performance
          </h3>
          
          <div class="space-y-4">
            <div v-for="territory in topPerformingTerritories" :key="territory.id" 
                 class="flex items-center justify-between">
              <div>
                <p class="font-medium text-gray-900 dark:text-white">{{ territory.name }}</p>
                <p class="text-sm text-gray-600 dark:text-gray-400">{{ territory.manager }}</p>
              </div>
              <div class="text-right">
                <p class="font-medium text-green-600">R {{ territory.revenue.toLocaleString() }}</p>
                <p class="text-sm text-gray-600 dark:text-gray-400">{{ territory.performance }}%</p>
              </div>
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm p-6">
          <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-4">
            Coverage Status
          </h3>
          
          <div class="space-y-3">
            <div class="flex justify-between">
              <span class="text-gray-600 dark:text-gray-400">Fully Covered</span>
              <span class="font-medium text-gray-900 dark:text-white">12 areas</span>
            </div>
            <div class="flex justify-between">
              <span class="text-gray-600 dark:text-gray-400">Partial Coverage</span>
              <span class="font-medium text-gray-900 dark:text-white">4 areas</span>
            </div>
            <div class="flex justify-between">
              <span class="text-gray-600 dark:text-gray-400">No Coverage</span>
              <span class="font-medium text-gray-900 dark:text-white">2 areas</span>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Territory List -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm mt-6">
      <div class="p-6">
        <h2 class="text-lg font-medium text-gray-900 dark:text-white mb-4">
          Territory Directory
        </h2>
        
        <div class="space-y-4">
          <div v-for="territory in mockTerritories" :key="territory.id" 
               class="border border-gray-200 dark:border-gray-700 rounded-lg p-4">
            <div class="flex items-start justify-between">
              <div class="flex-1">
                <div class="flex items-center mb-3">
                  <div class="w-12 h-12 rounded-lg flex items-center justify-center mr-4"
                       :class="getTerritoryColorClass(territory.performance)">
                    <Icon name="heroicons:map-pin" class="w-6 h-6 text-white" />
                  </div>
                  <div>
                    <h3 class="font-medium text-gray-900 dark:text-white text-lg">
                      {{ territory.name }}
                    </h3>
                    <div class="flex items-center space-x-3">
                      <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                            :class="territory.isActive ? 
                              'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200' : 
                              'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300'">
                        {{ territory.isActive ? 'Active' : 'Inactive' }}
                      </span>
                      <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                            :class="getPerformanceClass(territory.performance)">
                        {{ getPerformanceLabel(territory.performance) }}
                      </span>
                    </div>
                  </div>
                </div>
                
                <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
                  <div>
                    <span class="text-sm font-medium text-gray-700 dark:text-gray-300">Manager:</span>
                    <div class="text-sm text-gray-600 dark:text-gray-400">
                      {{ territory.manager }}<br>
                      {{ territory.contact.phone }}
                    </div>
                  </div>
                  
                  <div>
                    <span class="text-sm font-medium text-gray-700 dark:text-gray-300">Coverage:</span>
                    <div class="text-sm text-gray-600 dark:text-gray-400">
                      {{ territory.coverage.areas.length }} areas<br>
                      {{ territory.coverage.customers }} customers
                    </div>
                  </div>
                  
                  <div>
                    <span class="text-sm font-medium text-gray-700 dark:text-gray-300">Revenue:</span>
                    <div class="text-sm text-gray-600 dark:text-gray-400">
                      R {{ territory.revenue.monthly.toLocaleString() }}/month<br>
                      {{ territory.performance }}% of target
                    </div>
                  </div>
                  
                  <div>
                    <span class="text-sm font-medium text-gray-700 dark:text-gray-300">Metrics:</span>
                    <div class="text-sm text-gray-600 dark:text-gray-400">
                      {{ territory.metrics.conversionRate }}% conversion<br>
                      {{ territory.metrics.satisfaction }}/5 satisfaction
                    </div>
                  </div>
                </div>
                
                <div class="mt-3">
                  <span class="text-sm font-medium text-gray-700 dark:text-gray-300">Areas:</span>
                  <div class="flex flex-wrap gap-2 mt-1">
                    <span v-for="area in territory.coverage.areas" :key="area" 
                          class="inline-flex items-center px-2 py-1 text-xs bg-blue-50 text-blue-700 rounded dark:bg-blue-900 dark:text-blue-300">
                      {{ area }}
                    </span>
                  </div>
                </div>
              </div>
              
              <div class="flex items-center space-x-2 ml-4">
                <button class="text-blue-600 hover:text-blue-800 text-sm px-3 py-1 border border-blue-200 rounded">
                  View Map
                </button>
                <button class="text-green-600 hover:text-green-800 text-sm px-3 py-1 border border-green-200 rounded">
                  Analytics
                </button>
                <button class="text-purple-600 hover:text-purple-800 text-sm px-3 py-1 border border-purple-200 rounded">
                  Assign
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'

useHead({
  title: 'Sales Territories - TOSS ERP'
})

// Mock data for demonstration
const mockTerritories = ref([
  {
    id: 'territory-001',
    name: 'Soweto Central',
    manager: 'Themba Mthembu',
    contact: {
      phone: '+27 82 345 6789',
      email: 'themba@toss.co.za'
    },
    isActive: true,
    performance: 105,
    coverage: {
      areas: ['Diepkloof', 'Orlando East', 'Orlando West', 'Meadowlands'],
      customers: 145,
      potential: 220
    },
    revenue: {
      monthly: 156000,
      target: 150000,
      growth: 8.3
    },
    metrics: {
      conversionRate: 68,
      satisfaction: 4.6,
      retention: 87
    }
  },
  {
    id: 'territory-002',
    name: 'Alexandra Township',
    manager: 'Nomsa Dlamini',
    contact: {
      phone: '+27 76 789 0123',
      email: 'nomsa@toss.co.za'
    },
    isActive: true,
    performance: 92,
    coverage: {
      areas: ['Alexandra', 'Wynberg', 'Marlboro Gardens'],
      customers: 89,
      potential: 180
    },
    revenue: {
      monthly: 98000,
      target: 105000,
      growth: 5.2
    },
    metrics: {
      conversionRate: 62,
      satisfaction: 4.4,
      retention: 83
    }
  },
  {
    id: 'territory-003',
    name: 'Tembisa North',
    manager: 'Mpho Molefe',
    contact: {
      phone: '+27 72 123 4567',
      email: 'mpho@toss.co.za'
    },
    isActive: true,
    performance: 78,
    coverage: {
      areas: ['Tembisa', 'Ivory Park', 'Clayville'],
      customers: 67,
      potential: 195
    },
    revenue: {
      monthly: 72000,
      target: 92000,
      growth: -2.1
    },
    metrics: {
      conversionRate: 55,
      satisfaction: 4.2,
      retention: 79
    }
  },
  {
    id: 'territory-004',
    name: 'Orange Farm District',
    manager: 'Pieter Van Der Merwe',
    contact: {
      phone: '+27 83 456 7890',
      email: 'pieter@toss.co.za'
    },
    isActive: true,
    performance: 112,
    coverage: {
      areas: ['Orange Farm', 'Evaton', 'Sebokeng'],
      customers: 178,
      potential: 250
    },
    revenue: {
      monthly: 189000,
      target: 168000,
      growth: 12.7
    },
    metrics: {
      conversionRate: 71,
      satisfaction: 4.8,
      retention: 91
    }
  }
])

const topPerformingTerritories = ref([
  { id: '004', name: 'Orange Farm District', manager: 'Pieter Van Der Merwe', revenue: 189000, performance: 112 },
  { id: '001', name: 'Soweto Central', manager: 'Themba Mthembu', revenue: 156000, performance: 105 },
  { id: '002', name: 'Alexandra Township', manager: 'Nomsa Dlamini', revenue: 98000, performance: 92 }
])

const getTerritoryColorClass = (performance: number) => {
  if (performance >= 100) return 'bg-gradient-to-br from-green-500 to-green-600'
  if (performance >= 80) return 'bg-gradient-to-br from-yellow-500 to-yellow-600'
  return 'bg-gradient-to-br from-red-500 to-red-600'
}

const getPerformanceClass = (performance: number) => {
  if (performance >= 100) return 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200'
  if (performance >= 80) return 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-200'
  return 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200'
}

const getPerformanceLabel = (performance: number) => {
  if (performance >= 100) return 'High Performer'
  if (performance >= 80) return 'On Track'
  return 'Needs Attention'
}
</script>