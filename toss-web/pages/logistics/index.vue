<template>
  <div class="p-6">
    <div class="flex items-center justify-between mb-6">
      <div>
        <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
          Logistics Overview
        </h1>
        <p class="text-gray-600 dark:text-gray-400">
          Monitor delivery operations, driver performance and route optimization
        </p>
      </div>
      <button class="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg">
        <Icon name="heroicons:plus" class="w-4 h-4 mr-2" />
        Create Delivery
      </button>
    </div>

    <!-- Logistics Stats -->
    <div class="grid grid-cols-1 md:grid-cols-5 gap-4 mb-6">
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm">
        <div class="flex items-center">
          <div class="p-2 bg-blue-100 rounded-full">
            <Icon name="heroicons:truck" class="w-5 h-5 text-blue-600" />
          </div>
          <div class="ml-3">
            <p class="text-sm text-gray-600 dark:text-gray-400">Active Deliveries</p>
            <p class="text-lg font-semibold text-gray-900 dark:text-white">23</p>
          </div>
        </div>
      </div>
      
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm">
        <div class="flex items-center">
          <div class="p-2 bg-green-100 rounded-full">
            <Icon name="heroicons:users" class="w-5 h-5 text-green-600" />
          </div>
          <div class="ml-3">
            <p class="text-sm text-gray-600 dark:text-gray-400">Active Drivers</p>
            <p class="text-lg font-semibold text-gray-900 dark:text-white">18</p>
          </div>
        </div>
      </div>
      
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm">
        <div class="flex items-center">
          <div class="p-2 bg-purple-100 rounded-full">
            <Icon name="heroicons:clock" class="w-5 h-5 text-purple-600" />
          </div>
          <div class="ml-3">
            <p class="text-sm text-gray-600 dark:text-gray-400">On-Time Rate</p>
            <p class="text-lg font-semibold text-gray-900 dark:text-white">94.2%</p>
          </div>
        </div>
      </div>
      
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm">
        <div class="flex items-center">
          <div class="p-2 bg-orange-100 rounded-full">
            <Icon name="heroicons:banknotes" class="w-5 h-5 text-orange-600" />
          </div>
          <div class="ml-3">
            <p class="text-sm text-gray-600 dark:text-gray-400">Daily Revenue</p>
            <p class="text-lg font-semibold text-gray-900 dark:text-white">R 15.2K</p>
          </div>
        </div>
      </div>
      
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm">
        <div class="flex items-center">
          <div class="p-2 bg-indigo-100 rounded-full">
            <Icon name="heroicons:map-pin" class="w-5 h-5 text-indigo-600" />
          </div>
          <div class="ml-3">
            <p class="text-sm text-gray-600 dark:text-gray-400">Coverage Areas</p>
            <p class="text-lg font-semibold text-gray-900 dark:text-white">15</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Main Content Grid -->
    <div class="grid grid-cols-1 lg:grid-cols-3 gap-6 mb-6">
      <!-- Active Deliveries Map -->
      <div class="lg:col-span-2">
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm p-6">
          <h2 class="text-lg font-medium text-gray-900 dark:text-white mb-4">
            Live Delivery Tracking
          </h2>
          
          <!-- Map Placeholder -->
          <div class="h-96 bg-gray-100 dark:bg-gray-700 rounded-lg flex items-center justify-center mb-4">
            <div class="text-center">
              <Icon name="heroicons:map" class="w-16 h-16 text-gray-400 mx-auto mb-4" />
              <p class="text-gray-600 dark:text-gray-400">Live delivery tracking map</p>
              <p class="text-sm text-gray-500 dark:text-gray-500">Showing 23 active deliveries</p>
            </div>
          </div>
          
          <!-- Map Legend -->
          <div class="flex flex-wrap gap-3">
            <div class="flex items-center">
              <div class="w-3 h-3 bg-green-500 rounded-full mr-2"></div>
              <span class="text-sm text-gray-600 dark:text-gray-400">On Route (15)</span>
            </div>
            <div class="flex items-center">
              <div class="w-3 h-3 bg-orange-500 rounded-full mr-2"></div>
              <span class="text-sm text-gray-600 dark:text-gray-400">Delayed (3)</span>
            </div>
            <div class="flex items-center">
              <div class="w-3 h-3 bg-blue-500 rounded-full mr-2"></div>
              <span class="text-sm text-gray-600 dark:text-gray-400">Loading (5)</span>
            </div>
          </div>
        </div>
      </div>

      <!-- Driver Performance -->
      <div class="space-y-6">
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm p-6">
          <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-4">
            Top Drivers Today
          </h3>
          
          <div class="space-y-3">
            <div v-for="driver in topDrivers" :key="driver.id" 
                 class="flex items-center justify-between">
              <div class="flex items-center">
                <div class="w-8 h-8 bg-gradient-to-br from-blue-500 to-purple-600 rounded-full flex items-center justify-center mr-3">
                  <span class="text-white font-bold text-sm">
                    {{ driver.name.charAt(0) }}
                  </span>
                </div>
                <div>
                  <p class="font-medium text-gray-900 dark:text-white text-sm">{{ driver.name }}</p>
                  <p class="text-xs text-gray-600 dark:text-gray-400">{{ driver.deliveries }} deliveries</p>
                </div>
              </div>
              <div class="text-right">
                <p class="font-medium text-green-600">R {{ driver.earnings }}</p>
                <p class="text-xs text-gray-600 dark:text-gray-400">{{ driver.rating }}/5</p>
              </div>
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm p-6">
          <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-4">
            Delivery Status
          </h3>
          
          <div class="space-y-3">
            <div class="flex justify-between">
              <span class="text-gray-600 dark:text-gray-400">Completed Today</span>
              <span class="font-medium text-gray-900 dark:text-white">127</span>
            </div>
            <div class="flex justify-between">
              <span class="text-gray-600 dark:text-gray-400">In Progress</span>
              <span class="font-medium text-gray-900 dark:text-white">23</span>
            </div>
            <div class="flex justify-between">
              <span class="text-gray-600 dark:text-gray-400">Scheduled</span>
              <span class="font-medium text-gray-900 dark:text-white">45</span>
            </div>
            <div class="flex justify-between">
              <span class="text-gray-600 dark:text-gray-400">Failed Attempts</span>
              <span class="font-medium text-red-600">3</span>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Active Deliveries List -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm">
      <div class="p-6">
        <h2 class="text-lg font-medium text-gray-900 dark:text-white mb-4">
          Current Deliveries
        </h2>
        
        <div class="space-y-4">
          <div v-for="delivery in mockDeliveries" :key="delivery.id" 
               class="border border-gray-200 dark:border-gray-700 rounded-lg p-4">
            <div class="flex items-start justify-between">
              <div class="flex-1">
                <div class="flex items-center mb-3">
                  <div class="w-12 h-12 rounded-lg flex items-center justify-center mr-4"
                       :class="getStatusColor(delivery.status)">
                    <Icon name="heroicons:truck" class="w-6 h-6 text-white" />
                  </div>
                  <div>
                    <h3 class="font-medium text-gray-900 dark:text-white text-lg">
                      Delivery #{{ delivery.deliveryNumber }}
                    </h3>
                    <div class="flex items-center space-x-3">
                      <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                            :class="getStatusClass(delivery.status)">
                        {{ formatStatus(delivery.status) }}
                      </span>
                      <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                            :class="getPriorityClass(delivery.priority)">
                        {{ formatPriority(delivery.priority) }}
                      </span>
                    </div>
                  </div>
                </div>
                
                <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
                  <div>
                    <span class="text-sm font-medium text-gray-700 dark:text-gray-300">Driver:</span>
                    <div class="text-sm text-gray-600 dark:text-gray-400">
                      {{ delivery.driver.name }}<br>
                      {{ delivery.driver.phone }}
                    </div>
                  </div>
                  
                  <div>
                    <span class="text-sm font-medium text-gray-700 dark:text-gray-300">Destination:</span>
                    <div class="text-sm text-gray-600 dark:text-gray-400">
                      {{ delivery.destination.customer }}<br>
                      {{ delivery.destination.area }}
                    </div>
                  </div>
                  
                  <div>
                    <span class="text-sm font-medium text-gray-700 dark:text-gray-300">Schedule:</span>
                    <div class="text-sm text-gray-600 dark:text-gray-400">
                      Started: {{ formatTime(delivery.startTime) }}<br>
                      ETA: {{ formatTime(delivery.estimatedArrival) }}
                    </div>
                  </div>
                  
                  <div>
                    <span class="text-sm font-medium text-gray-700 dark:text-gray-300">Value & Items:</span>
                    <div class="text-sm text-gray-600 dark:text-gray-400">
                      R {{ delivery.value.toLocaleString() }}<br>
                      {{ delivery.items }} item(s)
                    </div>
                  </div>
                </div>
                
                <div class="mt-3">
                  <span class="text-sm font-medium text-gray-700 dark:text-gray-300">Route:</span>
                  <div class="flex items-center space-x-2 mt-1">
                    <span v-for="stop in delivery.route" :key="stop.id" 
                          class="inline-flex items-center px-2 py-1 text-xs rounded"
                          :class="stop.completed ? 
                            'bg-green-100 text-green-700 dark:bg-green-900 dark:text-green-300' : 
                            'bg-gray-100 text-gray-700 dark:bg-gray-700 dark:text-gray-300'">
                      {{ stop.location }}
                      <Icon v-if="stop.completed" name="heroicons:check" class="w-3 h-3 ml-1" />
                    </span>
                  </div>
                </div>
                
                <div v-if="delivery.tracking" class="mt-3">
                  <div class="flex items-center space-x-4">
                    <div class="flex items-center">
                      <Icon name="heroicons:map-pin" class="w-4 h-4 text-blue-600 mr-1" />
                      <span class="text-sm text-gray-600 dark:text-gray-400">
                        {{ delivery.tracking.currentLocation }}
                      </span>
                    </div>
                    <div class="flex items-center">
                      <Icon name="heroicons:clock" class="w-4 h-4 text-green-600 mr-1" />
                      <span class="text-sm text-gray-600 dark:text-gray-400">
                        {{ delivery.tracking.progress }}% complete
                      </span>
                    </div>
                  </div>
                </div>
              </div>
              
              <div class="flex items-center space-x-2 ml-4">
                <button class="text-blue-600 hover:text-blue-800 text-sm px-3 py-1 border border-blue-200 rounded">
                  Track
                </button>
                <button class="text-green-600 hover:text-green-800 text-sm px-3 py-1 border border-green-200 rounded">
                  Contact
                </button>
                <button class="text-purple-600 hover:text-purple-800 text-sm px-3 py-1 border border-purple-200 rounded">
                  Details
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
  title: 'Logistics Overview - TOSS ERP'
})

// Top drivers data
const topDrivers = ref([
  { id: 1, name: 'Themba Mthembu', deliveries: 8, earnings: '785', rating: 4.9 },
  { id: 2, name: 'Nomsa Dlamini', deliveries: 7, earnings: '692', rating: 4.8 },
  { id: 3, name: 'Mpho Molefe', deliveries: 6, earnings: '578', rating: 4.7 }
])

// Mock deliveries data
const mockDeliveries = ref([
  {
    id: 'del-001',
    deliveryNumber: 'DEL-2024-0156',
    status: 'in-transit',
    priority: 'high',
    driver: {
      name: 'Themba Mthembu',
      phone: '+27 82 345 6789',
      vehicle: 'Toyota Hilux - GP 123 ABC'
    },
    destination: {
      customer: 'Soweto Spaza Shops',
      area: 'Diepkloof Extension',
      address: '123 Main Road, Diepkloof'
    },
    startTime: new Date('2024-01-14T08:30:00'),
    estimatedArrival: new Date('2024-01-14T11:45:00'),
    value: 12450,
    items: 15,
    route: [
      { id: 1, location: 'Warehouse', completed: true },
      { id: 2, location: 'Orlando East', completed: true },
      { id: 3, location: 'Diepkloof', completed: false },
      { id: 4, location: 'Soweto Central', completed: false }
    ],
    tracking: {
      currentLocation: 'Orlando East - En route to Diepkloof',
      progress: 65,
      lastUpdate: new Date('2024-01-14T10:15:00')
    }
  },
  {
    id: 'del-002',
    deliveryNumber: 'DEL-2024-0157',
    status: 'loading',
    priority: 'medium',
    driver: {
      name: 'Nomsa Dlamini',
      phone: '+27 76 789 0123',
      vehicle: 'Nissan NP200 - GP 456 DEF'
    },
    destination: {
      customer: 'Alexandra Mini Markets',
      area: 'Alexandra',
      address: '45 Roosevelt Road, Alexandra'
    },
    startTime: new Date('2024-01-14T09:00:00'),
    estimatedArrival: new Date('2024-01-14T12:30:00'),
    value: 8750,
    items: 12,
    route: [
      { id: 1, location: 'Warehouse', completed: false },
      { id: 2, location: 'Wynberg', completed: false },
      { id: 3, location: 'Alexandra', completed: false }
    ],
    tracking: {
      currentLocation: 'Main Warehouse - Loading dock 3',
      progress: 15,
      lastUpdate: new Date('2024-01-14T09:45:00')
    }
  },
  {
    id: 'del-003',
    deliveryNumber: 'DEL-2024-0158',
    status: 'delayed',
    priority: 'low',
    driver: {
      name: 'Mpho Molefe',
      phone: '+27 72 123 4567',
      vehicle: 'Ford Ranger - GP 789 GHI'
    },
    destination: {
      customer: 'Tembisa Community Store',
      area: 'Tembisa',
      address: '78 Mthembu Street, Tembisa'
    },
    startTime: new Date('2024-01-14T07:45:00'),
    estimatedArrival: new Date('2024-01-14T11:00:00'),
    value: 15600,
    items: 22,
    route: [
      { id: 1, location: 'Warehouse', completed: true },
      { id: 2, location: 'Kempton Park', completed: true },
      { id: 3, location: 'Ivory Park', completed: false },
      { id: 4, location: 'Tembisa', completed: false }
    ],
    tracking: {
      currentLocation: 'Kempton Park - Traffic delay on R21',
      progress: 45,
      lastUpdate: new Date('2024-01-14T10:30:00')
    }
  },
  {
    id: 'del-004',
    deliveryNumber: 'DEL-2024-0159',
    status: 'completed',
    priority: 'high',
    driver: {
      name: 'Lucky Mahlangu',
      phone: '+27 83 456 7890',
      vehicle: 'Isuzu KB - GP 321 JKL'
    },
    destination: {
      customer: 'Orange Farm Markets',
      area: 'Orange Farm',
      address: '12 Extension 7, Orange Farm'
    },
    startTime: new Date('2024-01-14T06:30:00'),
    estimatedArrival: new Date('2024-01-14T09:30:00'),
    value: 9850,
    items: 18,
    route: [
      { id: 1, location: 'Warehouse', completed: true },
      { id: 2, location: 'Lenasia', completed: true },
      { id: 3, location: 'Orange Farm', completed: true }
    ],
    tracking: {
      currentLocation: 'Delivered successfully',
      progress: 100,
      lastUpdate: new Date('2024-01-14T09:15:00')
    }
  }
])

const getStatusColor = (status: string) => {
  const colorMap: Record<string, string> = {
    loading: 'bg-gradient-to-br from-blue-500 to-blue-600',
    'in-transit': 'bg-gradient-to-br from-purple-500 to-purple-600',
    delayed: 'bg-gradient-to-br from-orange-500 to-orange-600',
    completed: 'bg-gradient-to-br from-green-500 to-green-600',
    failed: 'bg-gradient-to-br from-red-500 to-red-600'
  }
  return colorMap[status] || 'bg-gradient-to-br from-gray-500 to-gray-600'
}

const getStatusClass = (status: string) => {
  const classMap: Record<string, string> = {
    loading: 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200',
    'in-transit': 'bg-purple-100 text-purple-800 dark:bg-purple-900 dark:text-purple-200',
    delayed: 'bg-orange-100 text-orange-800 dark:bg-orange-900 dark:text-orange-200',
    completed: 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200',
    failed: 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200'
  }
  return classMap[status] || 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300'
}

const getPriorityClass = (priority: string) => {
  const classMap: Record<string, string> = {
    high: 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200',
    medium: 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-200',
    low: 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200'
  }
  return classMap[priority] || 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300'
}

const formatStatus = (status: string) => {
  const statusMap: Record<string, string> = {
    loading: 'Loading',
    'in-transit': 'In Transit',
    delayed: 'Delayed',
    completed: 'Completed',
    failed: 'Failed'
  }
  return statusMap[status] || status
}

const formatPriority = (priority: string) => {
  const priorityMap: Record<string, string> = {
    high: 'High Priority',
    medium: 'Medium Priority',
    low: 'Low Priority'
  }
  return priorityMap[priority] || priority
}

const formatTime = (date: Date) => {
  return new Date(date).toLocaleTimeString('en-ZA', { 
    hour: '2-digit', 
    minute: '2-digit' 
  })
}
</script>