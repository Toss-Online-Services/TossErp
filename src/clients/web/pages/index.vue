<template>
  <div class="space-y-6">
    <!-- Page header -->
    <div class="flex items-center justify-between">
      <div>
        <h1 class="text-2xl font-bold text-gray-900 dark:text-white">Dashboard</h1>
        <p class="text-gray-600 dark:text-gray-400">Stock management overview and insights</p>
      </div>
      <div class="flex space-x-3">
        <button class="btn-outline">
          <ArrowDownTrayIcon class="w-4 h-4 mr-2" />
          Export
        </button>
        <button class="btn-primary">
          <PlusIcon class="w-4 h-4 mr-2" />
          Add Item
        </button>
      </div>
    </div>

    <!-- Stats cards -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
      <div class="stat-card">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <div class="w-8 h-8 bg-primary-100 dark:bg-primary-900 rounded-lg flex items-center justify-center">
              <CubeIcon class="w-5 h-5 text-primary-600 dark:text-primary-400" />
            </div>
          </div>
          <div class="ml-4">
            <p class="stat-label">Total Items</p>
            <p class="stat-value">1,234</p>
            <p class="stat-change-positive">
              <ArrowUpIcon class="w-4 h-4 inline mr-1" />
              +12.5%
            </p>
          </div>
        </div>
      </div>

      <div class="stat-card">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <div class="w-8 h-8 bg-secondary-100 dark:bg-secondary-900 rounded-lg flex items-center justify-center">
              <TruckIcon class="w-5 h-5 text-secondary-600 dark:text-secondary-400" />
            </div>
          </div>
          <div class="ml-4">
            <p class="stat-label">Warehouses</p>
            <p class="stat-value">8</p>
            <p class="stat-change-positive">
              <ArrowUpIcon class="w-4 h-4 inline mr-1" />
              +2
            </p>
          </div>
        </div>
      </div>

      <div class="stat-card">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <div class="w-8 h-8 bg-accent-100 dark:bg-accent-900 rounded-lg flex items-center justify-center">
              <ExclamationTriangleIcon class="w-5 h-5 text-accent-600 dark:text-accent-400" />
            </div>
          </div>
          <div class="ml-4">
            <p class="stat-label">Low Stock</p>
            <p class="stat-value">23</p>
            <p class="stat-change-negative">
              <ArrowDownIcon class="w-4 h-4 inline mr-1" />
              +5
            </p>
          </div>
        </div>
      </div>

      <div class="stat-card">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <div class="w-8 h-8 bg-green-100 dark:bg-green-900 rounded-lg flex items-center justify-center">
              <CurrencyDollarIcon class="w-5 h-5 text-green-600 dark:text-green-400" />
            </div>
          </div>
          <div class="ml-4">
            <p class="stat-label">Total Value</p>
            <p class="stat-value">$45.2K</p>
            <p class="stat-change-positive">
              <ArrowUpIcon class="w-4 h-4 inline mr-1" />
              +8.3%
            </p>
          </div>
        </div>
      </div>
    </div>

    <!-- Charts and tables -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <!-- Stock movement chart -->
      <div class="card">
        <div class="card-header">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Stock Movement</h3>
          <p class="text-sm text-gray-600 dark:text-gray-400">Last 30 days</p>
        </div>
        <div class="card-body">
          <div class="h-64 flex items-center justify-center text-gray-500 dark:text-gray-400">
            Chart placeholder - Stock movement over time
          </div>
        </div>
      </div>

      <!-- Recent activities -->
      <div class="card">
        <div class="card-header">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Recent Activities</h3>
        </div>
        <div class="card-body">
          <div class="space-y-4">
            <div v-for="activity in recentActivities" :key="activity.id" class="flex items-start space-x-3">
              <div class="flex-shrink-0">
                <div :class="[
                  'w-8 h-8 rounded-full flex items-center justify-center',
                  activity.type === 'in' ? 'bg-green-100 dark:bg-green-900' : 'bg-red-100 dark:bg-red-900'
                ]">
                  <component 
                    :is="activity.type === 'in' ? ArrowDownIcon : ArrowUpIcon" 
                    :class="[
                      'w-4 h-4',
                      activity.type === 'in' ? 'text-green-600 dark:text-green-400' : 'text-red-600 dark:text-red-400'
                    ]"
                  />
                </div>
              </div>
              <div class="flex-1 min-w-0">
                <p class="text-sm font-medium text-gray-900 dark:text-white">
                  {{ activity.description }}
                </p>
                <p class="text-sm text-gray-500 dark:text-gray-400">
                  {{ activity.time }}
                </p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Quick actions -->
    <div class="card">
      <div class="card-header">
        <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Quick Actions</h3>
      </div>
      <div class="card-body">
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
          <button class="flex items-center p-4 border border-gray-200 dark:border-gray-700 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-800 transition-colors">
            <PlusIcon class="w-6 h-6 text-primary-600 dark:text-primary-400 mr-3" />
            <div class="text-left">
              <p class="text-sm font-medium text-gray-900 dark:text-white">Add Item</p>
              <p class="text-xs text-gray-500 dark:text-gray-400">Create new inventory item</p>
            </div>
          </button>
          
          <button class="flex items-center p-4 border border-gray-200 dark:border-gray-700 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-800 transition-colors">
            <ArrowDownTrayIcon class="w-6 h-6 text-secondary-600 dark:text-secondary-400 mr-3" />
            <div class="text-left">
              <p class="text-sm font-medium text-gray-900 dark:text-white">Receive Stock</p>
              <p class="text-xs text-gray-500 dark:text-gray-400">Record incoming inventory</p>
            </div>
          </button>
          
          <button class="flex items-center p-4 border border-gray-200 dark:border-gray-700 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-800 transition-colors">
            <ArrowUpTrayIcon class="w-6 h-6 text-accent-600 dark:text-accent-400 mr-3" />
            <div class="text-left">
              <p class="text-sm font-medium text-gray-900 dark:text-white">Issue Stock</p>
              <p class="text-xs text-gray-500 dark:text-gray-400">Record outgoing inventory</p>
            </div>
          </button>
          
          <button class="flex items-center p-4 border border-gray-200 dark:border-gray-700 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-800 transition-colors">
            <ChartBarIcon class="w-6 h-6 text-green-600 dark:text-green-400 mr-3" />
            <div class="text-left">
              <p class="text-sm font-medium text-gray-900 dark:text-white">Generate Report</p>
              <p class="text-xs text-gray-500 dark:text-gray-400">Create stock reports</p>
            </div>
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import {
  ArrowDownTrayIcon,
  PlusIcon,
  CubeIcon,
  TruckIcon,
  ExclamationTriangleIcon,
  CurrencyDollarIcon,
  ArrowUpIcon,
  ArrowDownIcon,
  ArrowUpTrayIcon,
  ChartBarIcon
} from '@heroicons/vue/24/outline'

// Mock data
const recentActivities = ref([
  {
    id: 1,
    type: 'in',
    description: 'Received 50 units of Product A',
    time: '2 hours ago'
  },
  {
    id: 2,
    type: 'out',
    description: 'Issued 25 units of Product B',
    time: '4 hours ago'
  },
  {
    id: 3,
    type: 'in',
    description: 'Received 100 units of Product C',
    time: '6 hours ago'
  },
  {
    id: 4,
    type: 'out',
    description: 'Issued 10 units of Product D',
    time: '8 hours ago'
  }
])
</script>
