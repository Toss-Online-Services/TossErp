<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <!-- Page Header -->
    <div class="bg-white dark:bg-gray-800 shadow">
      <div class="max-w-7xl mx-auto py-6 px-4 sm:px-6 lg:px-8">
        <div class="md:flex md:items-center md:justify-between">
          <div class="flex-1 min-w-0">
            <h2 class="text-2xl font-bold leading-7 text-gray-900 dark:text-white sm:text-3xl sm:truncate">
              Manufacturing & Production
            </h2>
            <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
              Manage production orders, quality control, and manufacturing processes
            </p>
          </div>
          <div class="mt-4 flex md:mt-0 md:ml-4 space-x-3">
            <button
              @click="showNewOrderModal = true"
              type="button"
              class="inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-orange-600 hover:bg-orange-700"
            >
              <svg class="-ml-1 mr-2 h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
              </svg>
              New Production Order
            </button>
          </div>
        </div>
      </div>
    </div>

    <div class="max-w-7xl mx-auto py-6 px-4 sm:px-6 lg:px-8">
      <!-- Manufacturing Stats Cards -->
      <div class="grid grid-cols-1 gap-5 sm:grid-cols-2 lg:grid-cols-4 mb-8">
        <div class="bg-white dark:bg-gray-800 overflow-hidden shadow rounded-lg">
          <div class="p-5">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <svg class="h-6 w-6 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
                </svg>
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 dark:text-gray-400 truncate">Active Orders</dt>
                  <dd class="text-lg font-medium text-gray-900 dark:text-white">{{ activeOrders }}</dd>
                </dl>
              </div>
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 overflow-hidden shadow rounded-lg">
          <div class="p-5">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <svg class="h-6 w-6 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 10V3L4 14h7v7l9-11h-7z" />
                </svg>
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 dark:text-gray-400 truncate">Production Rate</dt>
                  <dd class="text-lg font-medium text-gray-900 dark:text-white">{{ productionRate }}%</dd>
                </dl>
              </div>
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 overflow-hidden shadow rounded-lg">
          <div class="p-5">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <svg class="h-6 w-6 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 dark:text-gray-400 truncate">Quality Score</dt>
                  <dd class="text-lg font-medium text-gray-900 dark:text-white">{{ qualityScore }}%</dd>
                </dl>
              </div>
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 overflow-hidden shadow rounded-lg">
          <div class="p-5">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <svg class="h-6 w-6 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 dark:text-gray-400 truncate">On-Time Delivery</dt>
                  <dd class="text-lg font-medium text-gray-900 dark:text-white">{{ onTimeDelivery }}%</dd>
                </dl>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
        <!-- Main Manufacturing Content -->
        <div class="lg:col-span-2 space-y-6">
          <!-- Production Orders -->
          <div class="bg-white dark:bg-gray-800 shadow rounded-lg">
            <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
              <div class="flex items-center justify-between">
                <h3 class="text-lg font-medium text-gray-900 dark:text-white">Production Orders</h3>
                <div class="flex items-center space-x-3">
                  <select
                    v-model="selectedStatus"
                    class="border border-gray-300 dark:border-gray-600 rounded-md px-3 py-2 text-sm focus:ring-orange-500 focus:border-orange-500 dark:bg-gray-700 dark:text-white"
                  >
                    <option value="">All Status</option>
                    <option value="planned">Planned</option>
                    <option value="in-progress">In Progress</option>
                    <option value="quality-check">Quality Check</option>
                    <option value="completed">Completed</option>
                    <option value="delayed">Delayed</option>
                  </select>
                </div>
              </div>
            </div>
            <div class="overflow-x-auto">
              <table class="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
                <thead class="bg-gray-50 dark:bg-gray-700">
                  <tr>
                    <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Order ID</th>
                    <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Product</th>
                    <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Quantity</th>
                    <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Status</th>
                    <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Progress</th>
                    <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Due Date</th>
                    <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Actions</th>
                  </tr>
                </thead>
                <tbody class="bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700">
                  <tr v-for="order in filteredOrders" :key="order.id" class="hover:bg-gray-50 dark:hover:bg-gray-700">
                    <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900 dark:text-white">{{ order.id }}</td>
                    <td class="px-6 py-4 whitespace-nowrap">
                      <div class="text-sm font-medium text-gray-900 dark:text-white">{{ order.product }}</div>
                      <div class="text-sm text-gray-500 dark:text-gray-400">{{ order.productCode }}</div>
                    </td>
                    <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">{{ order.quantity }} units</td>
                    <td class="px-6 py-4 whitespace-nowrap">
                      <span
                        class="inline-flex px-2 py-1 text-xs font-semibold rounded-full"
                        :class="getStatusColor(order.status)"
                      >
                        {{ order.status }}
                      </span>
                    </td>
                    <td class="px-6 py-4 whitespace-nowrap">
                      <div class="flex items-center">
                        <div class="w-16 bg-gray-200 rounded-full h-2 mr-2">
                          <div class="bg-orange-600 h-2 rounded-full" :style="`width: ${order.progress}%`"></div>
                        </div>
                        <span class="text-sm text-gray-900 dark:text-white">{{ order.progress }}%</span>
                      </div>
                    </td>
                    <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">{{ formatDate(order.dueDate) }}</td>
                    <td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
                      <button
                        @click="viewOrder(order)"
                        class="text-orange-600 hover:text-orange-900 mr-3"
                      >
                        View
                      </button>
                      <button
                        @click="editOrder(order)"
                        class="text-blue-600 hover:text-blue-900"
                      >
                        Edit
                      </button>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>

          <!-- Production Metrics -->
          <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-4">Production Metrics</h3>
            <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
              <div class="text-center">
                <div class="text-2xl font-bold text-gray-900 dark:text-white">{{ dailyOutput }}</div>
                <div class="text-sm text-gray-500 dark:text-gray-400">Daily Output</div>
                <div class="text-xs text-green-600 dark:text-green-400 mt-1">+12% from yesterday</div>
              </div>
              <div class="text-center">
                <div class="text-2xl font-bold text-gray-900 dark:text-white">{{ machineUptime }}%</div>
                <div class="text-sm text-gray-500 dark:text-gray-400">Machine Uptime</div>
                <div class="text-xs text-green-600 dark:text-green-400 mt-1">+2.5% from last week</div>
              </div>
              <div class="text-center">
                <div class="text-2xl font-bold text-gray-900 dark:text-white">{{ defectRate }}%</div>
                <div class="text-sm text-gray-500 dark:text-gray-400">Defect Rate</div>
                <div class="text-xs text-red-600 dark:text-red-400 mt-1">-0.3% from target</div>
              </div>
            </div>
          </div>

          <!-- Quality Control -->
          <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-4">Quality Control</h3>
            <div class="space-y-4">
              <div v-for="check in qualityChecks" :key="check.id" class="border border-gray-200 dark:border-gray-700 rounded-lg p-4">
                <div class="flex items-center justify-between">
                  <div class="flex items-center">
                    <div class="w-10 h-10 rounded-md flex items-center justify-center mr-4" :class="check.color">
                      <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" :d="check.icon" />
                      </svg>
                    </div>
                    <div>
                      <h4 class="text-sm font-medium text-gray-900 dark:text-white">{{ check.product }}</h4>
                      <p class="text-sm text-gray-500 dark:text-gray-400">Batch: {{ check.batchId }}</p>
                    </div>
                  </div>
                  <div class="flex items-center space-x-4">
                    <div class="text-right">
                      <div class="text-sm font-medium text-gray-900 dark:text-white">{{ check.score }}%</div>
                      <div class="text-xs text-gray-500 dark:text-gray-400">Quality Score</div>
                    </div>
                    <span
                      class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                      :class="check.result === 'passed' 
                        ? 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-300' 
                        : check.result === 'failed'
                        ? 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-300'
                        : 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-300'"
                    >
                      {{ check.result }}
                    </span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Manufacturing Management Panel -->
        <div class="space-y-6">
          <!-- Production Planning -->
          <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-4">Production Planning</h3>
            <div class="space-y-3">
              <button
                @click="schedulePlan"
                class="w-full text-left px-4 py-3 border border-gray-200 dark:border-gray-600 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
              >
                <div class="flex items-center">
                  <svg class="w-5 h-5 text-blue-500 mr-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3a2 2 0 012-2h4a2 2 0 012 2v4m-6 0h6m-6 0v10a2 2 0 002 2h4a2 2 0 002-2V7m-6 0h6" />
                  </svg>
                  <div>
                    <div class="text-sm font-medium text-gray-900 dark:text-white">Schedule Production</div>
                    <div class="text-xs text-gray-500 dark:text-gray-400">Plan production timeline</div>
                  </div>
                </div>
              </button>

              <button
                @click="manageMaterials"
                class="w-full text-left px-4 py-3 border border-gray-200 dark:border-gray-600 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
              >
                <div class="flex items-center">
                  <svg class="w-5 h-5 text-green-500 mr-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4" />
                  </svg>
                  <div>
                    <div class="text-sm font-medium text-gray-900 dark:text-white">Material Planning</div>
                    <div class="text-xs text-gray-500 dark:text-gray-400">Manage raw materials</div>
                  </div>
                </div>
              </button>

              <button
                @click="capacityPlanning"
                class="w-full text-left px-4 py-3 border border-gray-200 dark:border-gray-600 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
              >
                <div class="flex items-center">
                  <svg class="w-5 h-5 text-purple-500 mr-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
                  </svg>
                  <div>
                    <div class="text-sm font-medium text-gray-900 dark:text-white">Capacity Planning</div>
                    <div class="text-xs text-gray-500 dark:text-gray-400">Resource allocation</div>
                  </div>
                </div>
              </button>
            </div>
          </div>

          <!-- Machine Status -->
          <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-4">Machine Status</h3>
            <div class="space-y-3">
              <div v-for="machine in machines" :key="machine.id" class="flex items-center justify-between">
                <div class="flex items-center">
                  <div class="w-3 h-3 rounded-full mr-3" :class="machine.color"></div>
                  <div>
                    <span class="text-sm font-medium text-gray-900 dark:text-white">{{ machine.name }}</span>
                    <div class="text-xs text-gray-500 dark:text-gray-400">{{ machine.location }}</div>
                  </div>
                </div>
                <div class="text-right">
                  <div class="text-sm font-medium text-gray-900 dark:text-white">{{ machine.efficiency }}%</div>
                  <div class="text-xs text-gray-500 dark:text-gray-400">{{ machine.status }}</div>
                </div>
              </div>
            </div>
          </div>

          <!-- Recent Activities -->
          <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-4">Recent Activities</h3>
            <div class="space-y-3">
              <div v-for="activity in recentActivities" :key="activity.id" class="flex items-start">
                <div class="flex-shrink-0">
                  <div class="w-8 h-8 rounded-full flex items-center justify-center" :class="activity.color">
                    <svg class="w-4 h-4 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" :d="activity.icon" />
                    </svg>
                  </div>
                </div>
                <div class="ml-3">
                  <p class="text-sm text-gray-900 dark:text-white">{{ activity.description }}</p>
                  <p class="text-xs text-gray-500 dark:text-gray-400">{{ formatTime(activity.timestamp) }}</p>
                </div>
              </div>
            </div>
          </div>

          <!-- Production Targets -->
          <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-4">Production Targets</h3>
            <div class="space-y-4">
              <div>
                <div class="flex justify-between text-sm mb-1">
                  <span class="text-gray-600 dark:text-gray-400">Daily Target</span>
                  <span class="text-gray-900 dark:text-white">{{ Math.round((dailyOutput / 1200) * 100) }}%</span>
                </div>
                <div class="w-full bg-gray-200 rounded-full h-2">
                  <div class="bg-green-600 h-2 rounded-full" :style="`width: ${Math.round((dailyOutput / 1200) * 100)}%`"></div>
                </div>
              </div>
              
              <div>
                <div class="flex justify-between text-sm mb-1">
                  <span class="text-gray-600 dark:text-gray-400">Weekly Target</span>
                  <span class="text-gray-900 dark:text-white">87%</span>
                </div>
                <div class="w-full bg-gray-200 rounded-full h-2">
                  <div class="bg-blue-600 h-2 rounded-full" style="width: 87%"></div>
                </div>
              </div>
              
              <div>
                <div class="flex justify-between text-sm mb-1">
                  <span class="text-gray-600 dark:text-gray-400">Monthly Target</span>
                  <span class="text-gray-900 dark:text-white">92%</span>
                </div>
                <div class="w-full bg-gray-200 rounded-full h-2">
                  <div class="bg-purple-600 h-2 rounded-full" style="width: 92%"></div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- New Production Order Modal -->
    <div v-if="showNewOrderModal" class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
      <div class="relative top-20 mx-auto p-5 border w-96 shadow-lg rounded-md bg-white dark:bg-gray-800">
        <div class="mt-3">
          <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-4">New Production Order</h3>
          <form @submit.prevent="createOrder" class="space-y-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300">Product</label>
              <input
                v-model="newOrder.product"
                type="text"
                required
                class="mt-1 block w-full border border-gray-300 dark:border-gray-600 rounded-md px-3 py-2 text-sm focus:ring-orange-500 focus:border-orange-500 dark:bg-gray-700 dark:text-white"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300">Product Code</label>
              <input
                v-model="newOrder.productCode"
                type="text"
                required
                class="mt-1 block w-full border border-gray-300 dark:border-gray-600 rounded-md px-3 py-2 text-sm focus:ring-orange-500 focus:border-orange-500 dark:bg-gray-700 dark:text-white"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300">Quantity</label>
              <input
                v-model.number="newOrder.quantity"
                type="number"
                required
                class="mt-1 block w-full border border-gray-300 dark:border-gray-600 rounded-md px-3 py-2 text-sm focus:ring-orange-500 focus:border-orange-500 dark:bg-gray-700 dark:text-white"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300">Due Date</label>
              <input
                v-model="newOrder.dueDate"
                type="date"
                required
                class="mt-1 block w-full border border-gray-300 dark:border-gray-600 rounded-md px-3 py-2 text-sm focus:ring-orange-500 focus:border-orange-500 dark:bg-gray-700 dark:text-white"
              />
            </div>
            <div class="flex justify-end space-x-3 pt-4">
              <button
                type="button"
                @click="showNewOrderModal = false"
                class="px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-md text-sm font-medium text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-700"
              >
                Cancel
              </button>
              <button
                type="submit"
                class="px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-orange-600 hover:bg-orange-700"
              >
                Create Order
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'

// Types
interface ProductionOrder {
  id: string
  product: string
  productCode: string
  quantity: number
  status: 'planned' | 'in-progress' | 'quality-check' | 'completed' | 'delayed'
  progress: number
  dueDate: Date
}

interface QualityCheck {
  id: string
  product: string
  batchId: string
  score: number
  result: 'passed' | 'failed' | 'pending'
  icon: string
  color: string
}

interface Machine {
  id: string
  name: string
  location: string
  status: 'running' | 'idle' | 'maintenance'
  efficiency: number
  color: string
}

interface Activity {
  id: string
  description: string
  timestamp: Date
  icon: string
  color: string
}

// Reactive data
const activeOrders = ref(12)
const productionRate = ref(87)
const qualityScore = ref(94)
const onTimeDelivery = ref(91)
const dailyOutput = ref(1085)
const machineUptime = ref(94.5)
const defectRate = ref(2.3)

const selectedStatus = ref('')
const showNewOrderModal = ref(false)

// New order form
const newOrder = ref({
  product: '',
  productCode: '',
  quantity: 0,
  dueDate: ''
})

// Sample production orders
const productionOrders = ref<ProductionOrder[]>([
  {
    id: 'PO-2024-001',
    product: 'Cooking Oil 5L',
    productCode: 'CO-5L-001',
    quantity: 500,
    status: 'in-progress',
    progress: 65,
    dueDate: new Date('2024-03-15')
  },
  {
    id: 'PO-2024-002',
    product: 'Rice 10kg Bags',
    productCode: 'RC-10K-002',
    quantity: 200,
    status: 'quality-check',
    progress: 90,
    dueDate: new Date('2024-03-12')
  },
  {
    id: 'PO-2024-003',
    product: 'Laundry Detergent 2L',
    productCode: 'LD-2L-003',
    quantity: 300,
    status: 'planned',
    progress: 0,
    dueDate: new Date('2024-03-20')
  },
  {
    id: 'PO-2024-004',
    product: 'Toilet Paper 12-pack',
    productCode: 'TP-12P-004',
    quantity: 800,
    status: 'completed',
    progress: 100,
    dueDate: new Date('2024-03-10')
  },
  {
    id: 'PO-2024-005',
    product: 'Cleaning Supplies Kit',
    productCode: 'CS-KIT-005',
    quantity: 150,
    status: 'delayed',
    progress: 45,
    dueDate: new Date('2024-03-08')
  }
])

// Quality checks
const qualityChecks = ref<QualityCheck[]>([
  {
    id: '1',
    product: 'Cooking Oil 5L',
    batchId: 'B-2024-0312',
    score: 96,
    result: 'passed',
    icon: 'M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z',
    color: 'bg-green-500'
  },
  {
    id: '2',
    product: 'Rice 10kg Bags',
    batchId: 'B-2024-0311',
    score: 89,
    result: 'passed',
    icon: 'M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z',
    color: 'bg-green-500'
  },
  {
    id: '3',
    product: 'Laundry Detergent 2L',
    batchId: 'B-2024-0310',
    score: 78,
    result: 'failed',
    icon: 'M6 18L18 6M6 6l12 12',
    color: 'bg-red-500'
  },
  {
    id: '4',
    product: 'Cleaning Supplies Kit',
    batchId: 'B-2024-0309',
    score: 85,
    result: 'pending',
    icon: 'M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z',
    color: 'bg-yellow-500'
  }
])

// Machines
const machines = ref<Machine[]>([
  { id: '1', name: 'Production Line A', location: 'Floor 1', status: 'running', efficiency: 95, color: 'bg-green-400' },
  { id: '2', name: 'Production Line B', location: 'Floor 1', status: 'running', efficiency: 87, color: 'bg-green-400' },
  { id: '3', name: 'Packaging Unit 1', location: 'Floor 2', status: 'idle', efficiency: 0, color: 'bg-yellow-400' },
  { id: '4', name: 'Quality Control Station', location: 'Floor 2', status: 'running', efficiency: 92, color: 'bg-green-400' },
  { id: '5', name: 'Labeling Machine', location: 'Floor 2', status: 'maintenance', efficiency: 0, color: 'bg-red-400' }
])

// Recent activities
const recentActivities = ref<Activity[]>([
  {
    id: '1',
    description: 'Production order PO-2024-004 completed',
    timestamp: new Date(Date.now() - 1800000),
    icon: 'M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z',
    color: 'bg-green-500'
  },
  {
    id: '2',
    description: 'Quality check failed for batch B-2024-0310',
    timestamp: new Date(Date.now() - 3600000),
    icon: 'M6 18L18 6M6 6l12 12',
    color: 'bg-red-500'
  },
  {
    id: '3',
    description: 'Labeling machine scheduled for maintenance',
    timestamp: new Date(Date.now() - 5400000),
    icon: 'M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z',
    color: 'bg-blue-500'
  }
])

// Computed properties
const filteredOrders = computed(() => {
  return productionOrders.value.filter(order => {
    return selectedStatus.value === '' || order.status === selectedStatus.value
  })
})

// Methods
const formatDate = (date: Date): string => {
  return date.toLocaleDateString('en-ZA')
}

const formatTime = (date: Date): string => {
  return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
}

const getStatusColor = (status: string): string => {
  switch (status) {
    case 'planned': return 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-300'
    case 'in-progress': return 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-300'
    case 'quality-check': return 'bg-purple-100 text-purple-800 dark:bg-purple-900 dark:text-purple-300'
    case 'completed': return 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-300'
    case 'delayed': return 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-300'
    default: return 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300'
  }
}

const createOrder = () => {
  const order: ProductionOrder = {
    id: `PO-2024-${String(productionOrders.value.length + 1).padStart(3, '0')}`,
    product: newOrder.value.product,
    productCode: newOrder.value.productCode,
    quantity: newOrder.value.quantity,
    status: 'planned',
    progress: 0,
    dueDate: new Date(newOrder.value.dueDate)
  }
  
  productionOrders.value.push(order)
  showNewOrderModal.value = false
  
  // Reset form
  newOrder.value = {
    product: '',
    productCode: '',
    quantity: 0,
    dueDate: ''
  }
  
  // Update active orders count
  activeOrders.value++
}

const viewOrder = (order: ProductionOrder) => {
  console.log('Viewing order:', order.id)
  // Implement view functionality
}

const editOrder = (order: ProductionOrder) => {
  console.log('Editing order:', order.id)
  // Implement edit functionality
}

const schedulePlan = () => {
  console.log('Opening production scheduler...')
  // Implement production planning
}

const manageMaterials = () => {
  console.log('Opening material planning...')
  // Implement material management
}

const capacityPlanning = () => {
  console.log('Opening capacity planning...')
  // Implement capacity planning
}
</script>

<style scoped>
/* Component-specific styles */
</style>
