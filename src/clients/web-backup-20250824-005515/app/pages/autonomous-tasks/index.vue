<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <!-- Header -->
    <div class="bg-white dark:bg-gray-800 shadow">
      <div class="max-w-7xl mx-auto py-6 px-4 sm:px-6 lg:px-8">
        <div class="md:flex md:items-center md:justify-between">
          <div class="flex-1 min-w-0">
            <h2 class="text-2xl font-bold leading-7 text-gray-900 dark:text-white sm:text-3xl sm:truncate">
              Autonomous Task Execution
            </h2>
            <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
              AI agents executing business tasks without human intervention
            </p>
          </div>
          <div class="mt-4 flex md:mt-0 md:ml-4 space-x-3">
            <button
              @click="pauseAllTasks"
              type="button"
              class="inline-flex items-center px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm text-sm font-medium text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600"
            >
              <svg class="-ml-1 mr-2 h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 9v6m4-6v6m7-3a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
              Pause All
            </button>
            <button
              @click="createNewTask"
              type="button"
              class="inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-indigo-600 hover:bg-indigo-700"
            >
              <svg class="-ml-1 mr-2 h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
              </svg>
              New Task
            </button>
          </div>
        </div>
      </div>
    </div>

    <div class="max-w-7xl mx-auto py-6 px-4 sm:px-6 lg:px-8">
      <div class="grid grid-cols-1 lg:grid-cols-4 gap-6">
        
        <!-- Task Execution Queue -->
        <div class="lg:col-span-3">
          <!-- Execution Status Overview -->
          <div class="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
            <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
              <div class="flex items-center">
                <div class="w-8 h-8 bg-blue-500 rounded-lg flex items-center justify-center">
                  <svg class="w-4 h-4 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
                  </svg>
                </div>
                <div class="ml-4">
                  <p class="text-sm font-medium text-gray-600 dark:text-gray-400">In Progress</p>
                  <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ inProgressTasks }}</p>
                </div>
              </div>
            </div>

            <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
              <div class="flex items-center">
                <div class="w-8 h-8 bg-yellow-500 rounded-lg flex items-center justify-center">
                  <svg class="w-4 h-4 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
                  </svg>
                </div>
                <div class="ml-4">
                  <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Queued</p>
                  <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ queuedTasks }}</p>
                </div>
              </div>
            </div>

            <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
              <div class="flex items-center">
                <div class="w-8 h-8 bg-green-500 rounded-lg flex items-center justify-center">
                  <svg class="w-4 h-4 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
                  </svg>
                </div>
                <div class="ml-4">
                  <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Completed</p>
                  <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ completedTasks }}</p>
                </div>
              </div>
            </div>

            <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
              <div class="flex items-center">
                <div class="w-8 h-8 bg-red-500 rounded-lg flex items-center justify-center">
                  <svg class="w-4 h-4 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L3.732 16.5c-.77.833.192 2.5 1.732 2.5z" />
                  </svg>
                </div>
                <div class="ml-4">
                  <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Failed</p>
                  <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ failedTasks }}</p>
                </div>
              </div>
            </div>
          </div>

          <!-- Active Tasks -->
          <div class="bg-white dark:bg-gray-800 rounded-lg shadow mb-6">
            <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
              <div class="flex items-center justify-between">
                <h3 class="text-lg font-medium text-gray-900 dark:text-white">Active Autonomous Tasks</h3>
                <div class="flex items-center space-x-2">
                  <span class="text-sm text-gray-500 dark:text-gray-400">Auto-refresh</span>
                  <button
                    @click="toggleAutoRefresh"
                    :class="autoRefresh ? 'bg-indigo-600' : 'bg-gray-200 dark:bg-gray-700'"
                    class="relative inline-flex h-6 w-11 flex-shrink-0 cursor-pointer rounded-full border-2 border-transparent transition-colors duration-200 ease-in-out focus:outline-none"
                  >
                    <span
                      :class="autoRefresh ? 'translate-x-5' : 'translate-x-0'"
                      class="pointer-events-none inline-block h-5 w-5 transform rounded-full bg-white shadow ring-0 transition duration-200 ease-in-out"
                    ></span>
                  </button>
                </div>
              </div>
            </div>
            <div class="p-6">
              <div class="space-y-4">
                <div v-for="task in activeTasks" :key="task.id" class="border border-gray-200 dark:border-gray-700 rounded-lg p-4">
                  <div class="flex items-center justify-between mb-3">
                    <div class="flex items-center">
                      <div class="w-3 h-3 rounded-full mr-3 animate-pulse" :class="getTaskStatusColor(task.status)"></div>
                      <div>
                        <h4 class="text-sm font-medium text-gray-900 dark:text-white">{{ task.name }}</h4>
                        <p class="text-xs text-gray-500 dark:text-gray-400">{{ task.agent }} • {{ task.category }}</p>
                      </div>
                    </div>
                    <div class="flex items-center space-x-2">
                      <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium" :class="getTaskStatusBadge(task.status)">
                        {{ task.status }}
                      </span>
                      <button @click="viewTaskDetails(task)" class="text-gray-400 hover:text-gray-600 dark:hover:text-gray-300">
                        <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
                        </svg>
                      </button>
                    </div>
                  </div>
                  
                  <p class="text-sm text-gray-600 dark:text-gray-300 mb-3">{{ task.description }}</p>
                  
                  <!-- Progress Bar -->
                  <div class="mb-3">
                    <div class="flex justify-between text-sm text-gray-600 dark:text-gray-300 mb-1">
                      <span>Progress</span>
                      <span>{{ task.progress }}%</span>
                    </div>
                    <div class="w-full bg-gray-200 dark:bg-gray-700 rounded-full h-2">
                      <div class="bg-gradient-to-r from-indigo-500 to-purple-500 h-2 rounded-full transition-all duration-300" :style="{ width: `${task.progress}%` }"></div>
                    </div>
                  </div>
                  
                  <!-- Task Steps -->
                  <div class="space-y-2">
                    <div v-for="step in task.steps" :key="step.id" class="flex items-center text-sm">
                      <div class="w-4 h-4 mr-3 flex items-center justify-center">
                        <svg v-if="step.status === 'completed'" class="w-4 h-4 text-green-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
                        </svg>
                        <svg v-else-if="step.status === 'active'" class="w-4 h-4 text-blue-500 animate-spin" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
                        </svg>
                        <div v-else class="w-2 h-2 bg-gray-300 dark:bg-gray-600 rounded-full"></div>
                      </div>
                      <span :class="step.status === 'completed' ? 'text-green-600 dark:text-green-400 line-through' : step.status === 'active' ? 'text-blue-600 dark:text-blue-400 font-medium' : 'text-gray-500 dark:text-gray-400'">
                        {{ step.name }}
                      </span>
                      <span v-if="step.status === 'active'" class="ml-2 text-xs text-gray-400">({{ step.eta }})</span>
                    </div>
                  </div>
                  
                  <!-- Task Metrics -->
                  <div class="mt-4 pt-3 border-t border-gray-200 dark:border-gray-700">
                    <div class="flex items-center justify-between text-xs text-gray-500 dark:text-gray-400">
                      <span>Started: {{ formatTime(task.startTime) }}</span>
                      <span>ETA: {{ task.eta }}</span>
                      <span>Priority: {{ task.priority }}</span>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Task History -->
          <div class="bg-white dark:bg-gray-800 rounded-lg shadow">
            <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
              <h3 class="text-lg font-medium text-gray-900 dark:text-white">Recent Task Completions</h3>
            </div>
            <div class="p-6">
              <div class="space-y-3">
                <div v-for="task in recentTasks" :key="task.id" class="flex items-center justify-between p-3 bg-gray-50 dark:bg-gray-700 rounded-lg">
                  <div class="flex items-center">
                    <div class="w-8 h-8 rounded-lg flex items-center justify-center mr-3" :class="task.success ? 'bg-green-100 dark:bg-green-900' : 'bg-red-100 dark:bg-red-900'">
                      <svg v-if="task.success" class="w-4 h-4 text-green-600 dark:text-green-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
                      </svg>
                      <svg v-else class="w-4 h-4 text-red-600 dark:text-red-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
                      </svg>
                    </div>
                    <div>
                      <h4 class="text-sm font-medium text-gray-900 dark:text-white">{{ task.name }}</h4>
                      <p class="text-xs text-gray-500 dark:text-gray-400">{{ task.agent }} • {{ formatDuration(task.duration) }}</p>
                    </div>
                  </div>
                  <div class="text-right">
                    <p class="text-sm text-gray-600 dark:text-gray-300">{{ formatTime(task.completedAt) }}</p>
                    <p class="text-xs" :class="task.success ? 'text-green-600 dark:text-green-400' : 'text-red-600 dark:text-red-400'">
                      {{ task.success ? 'Success' : 'Failed' }}
                    </p>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Task Controls & Monitoring -->
        <div class="space-y-6">
          <!-- Execution Controls -->
          <div class="bg-white dark:bg-gray-800 rounded-lg shadow">
            <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
              <h3 class="text-lg font-medium text-gray-900 dark:text-white">Execution Controls</h3>
            </div>
            <div class="p-6 space-y-4">
              <button @click="createTask('inventory')" class="w-full flex items-center justify-between p-3 text-left border border-gray-200 dark:border-gray-600 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors">
                <div class="flex items-center">
                  <div class="w-8 h-8 bg-blue-500 rounded-lg flex items-center justify-center mr-3">
                    <svg class="w-4 h-4 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4" />
                    </svg>
                  </div>
                  <span class="text-sm font-medium text-gray-900 dark:text-white">Stock Optimization</span>
                </div>
                <svg class="w-4 h-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
                </svg>
              </button>

              <button @click="createTask('sales')" class="w-full flex items-center justify-between p-3 text-left border border-gray-200 dark:border-gray-600 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors">
                <div class="flex items-center">
                  <div class="w-8 h-8 bg-green-500 rounded-lg flex items-center justify-center mr-3">
                    <svg class="w-4 h-4 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                    </svg>
                  </div>
                  <span class="text-sm font-medium text-gray-900 dark:text-white">Invoice Processing</span>
                </div>
                <svg class="w-4 h-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
                </svg>
              </button>

              <button @click="createTask('finance')" class="w-full flex items-center justify-between p-3 text-left border border-gray-200 dark:border-gray-600 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors">
                <div class="flex items-center">
                  <div class="w-8 h-8 bg-yellow-500 rounded-lg flex items-center justify-center mr-3">
                    <svg class="w-4 h-4 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
                    </svg>
                  </div>
                  <span class="text-sm font-medium text-gray-900 dark:text-white">Financial Analysis</span>
                </div>
                <svg class="w-4 h-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
                </svg>
              </button>

              <button @click="createTask('marketing')" class="w-full flex items-center justify-between p-3 text-left border border-gray-200 dark:border-gray-600 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors">
                <div class="flex items-center">
                  <div class="w-8 h-8 bg-purple-500 rounded-lg flex items-center justify-center mr-3">
                    <svg class="w-4 h-4 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5.882V19.24a1.76 1.76 0 01-3.417.592l-2.147-6.15M18 13a3 3 0 100-6M5.436 13.683A4.001 4.001 0 017 6h1.832c4.1 0 7.625-1.234 9.168-3v14c-1.543-1.766-5.067-3-9.168-3H7a3.988 3.988 0 01-1.564-.317z" />
                    </svg>
                  </div>
                  <span class="text-sm font-medium text-gray-900 dark:text-white">Customer Outreach</span>
                </div>
                <svg class="w-4 h-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
                </svg>
              </button>
            </div>
          </div>

          <!-- Task Queue -->
          <div class="bg-white dark:bg-gray-800 rounded-lg shadow">
            <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
              <h3 class="text-lg font-medium text-gray-900 dark:text-white">Task Queue</h3>
            </div>
            <div class="p-6">
              <div class="space-y-3">
                <div v-for="task in queuedTasksList" :key="task.id" class="flex items-center justify-between p-3 bg-yellow-50 dark:bg-yellow-900/20 border border-yellow-200 dark:border-yellow-800 rounded-lg">
                  <div>
                    <h4 class="text-sm font-medium text-gray-900 dark:text-white">{{ task.name }}</h4>
                    <p class="text-xs text-gray-500 dark:text-gray-400">{{ task.agent }}</p>
                  </div>
                  <div class="text-right">
                    <span class="text-xs text-yellow-600 dark:text-yellow-400">Position {{ task.position }}</span>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Agent Performance -->
          <div class="bg-white dark:bg-gray-800 rounded-lg shadow">
            <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
              <h3 class="text-lg font-medium text-gray-900 dark:text-white">Agent Performance</h3>
            </div>
            <div class="p-6 space-y-4">
              <div v-for="agent in agentPerformance" :key="agent.id" class="flex items-center justify-between">
                <div>
                  <h4 class="text-sm font-medium text-gray-900 dark:text-white">{{ agent.name }}</h4>
                  <p class="text-xs text-gray-500 dark:text-gray-400">{{ agent.tasksCompleted }} tasks today</p>
                </div>
                <div class="text-right">
                  <p class="text-sm font-medium text-gray-900 dark:text-white">{{ agent.successRate }}%</p>
                  <div class="w-16 h-1 bg-gray-200 dark:bg-gray-700 rounded-full mt-1">
                    <div class="h-1 bg-green-500 rounded-full transition-all duration-300" :style="{ width: `${agent.successRate}%` }"></div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'

// Mock Nuxt functions
function definePageMeta(meta: any) {}
function useHead(options: any) {
  if (typeof document !== 'undefined') {
    document.title = options.title || 'Autonomous Task Execution - TOSS ERP'
  }
}

// Reactive data
const inProgressTasks = ref(5)
const queuedTasks = ref(12)
const completedTasks = ref(247)
const failedTasks = ref(3)
const autoRefresh = ref(true)

// Active tasks
const activeTasks = ref([
  {
    id: 'task-1',
    name: 'Inventory Reorder Optimization',
    description: 'Analyzing sales patterns and stock levels to optimize reorder quantities for 23 products',
    agent: 'Inventory AI Agent',
    category: 'Stock Management',
    status: 'executing',
    progress: 67,
    priority: 'high',
    startTime: new Date(Date.now() - 1800000), // 30 minutes ago
    eta: '8 minutes remaining',
    steps: [
      { id: 1, name: 'Collect sales data', status: 'completed', eta: '' },
      { id: 2, name: 'Analyze demand patterns', status: 'completed', eta: '' },
      { id: 3, name: 'Calculate optimal quantities', status: 'active', eta: '5 min' },
      { id: 4, name: 'Generate purchase orders', status: 'pending', eta: '' },
      { id: 5, name: 'Send orders to suppliers', status: 'pending', eta: '' }
    ]
  },
  {
    id: 'task-2',
    name: 'Customer Invoice Processing',
    description: 'Generating and sending invoices for 47 pending orders with automatic payment reminders',
    agent: 'Sales AI Agent',
    category: 'Financial Processing',
    status: 'executing',
    progress: 43,
    priority: 'medium',
    startTime: new Date(Date.now() - 900000), // 15 minutes ago
    eta: '12 minutes remaining',
    steps: [
      { id: 1, name: 'Validate order data', status: 'completed', eta: '' },
      { id: 2, name: 'Calculate totals and taxes', status: 'completed', eta: '' },
      { id: 3, name: 'Generate invoice PDFs', status: 'active', eta: '8 min' },
      { id: 4, name: 'Send via email', status: 'pending', eta: '' },
      { id: 5, name: 'Update accounting records', status: 'pending', eta: '' }
    ]
  },
  {
    id: 'task-3',
    name: 'Financial Report Generation',
    description: 'Compiling monthly financial statements and performance analytics for stakeholder review',
    agent: 'Finance AI Agent', 
    category: 'Business Intelligence',
    status: 'executing',
    progress: 23,
    priority: 'low',
    startTime: new Date(Date.now() - 2700000), // 45 minutes ago
    eta: '25 minutes remaining',
    steps: [
      { id: 1, name: 'Aggregate financial data', status: 'completed', eta: '' },
      { id: 2, name: 'Calculate key metrics', status: 'active', eta: '15 min' },
      { id: 3, name: 'Generate visualizations', status: 'pending', eta: '' },
      { id: 4, name: 'Create executive summary', status: 'pending', eta: '' },
      { id: 5, name: 'Distribute reports', status: 'pending', eta: '' }
    ]
  }
])

// Recent completed tasks
const recentTasks = ref([
  {
    id: 'recent-1',
    name: 'Customer Segmentation Analysis',
    agent: 'Marketing AI Agent',
    success: true,
    completedAt: new Date(Date.now() - 600000), // 10 minutes ago
    duration: 1800000 // 30 minutes
  },
  {
    id: 'recent-2',
    name: 'Supplier Price Update',
    agent: 'Procurement AI Agent',
    success: true,
    completedAt: new Date(Date.now() - 900000), // 15 minutes ago
    duration: 900000 // 15 minutes
  },
  {
    id: 'recent-3',
    name: 'Low Stock Alert Processing',
    agent: 'Inventory AI Agent',
    success: false,
    completedAt: new Date(Date.now() - 1200000), // 20 minutes ago
    duration: 600000 // 10 minutes
  },
  {
    id: 'recent-4',
    name: 'Payment Reminder Automation',
    agent: 'Finance AI Agent',
    success: true,
    completedAt: new Date(Date.now() - 1800000), // 30 minutes ago
    duration: 1200000 // 20 minutes
  }
])

// Queued tasks
const queuedTasksList = ref([
  {
    id: 'queue-1',
    name: 'Product Catalog Sync',
    agent: 'Data Integration Agent',
    position: 1
  },
  {
    id: 'queue-2',
    name: 'Customer Feedback Analysis',
    agent: 'Analytics AI Agent',
    position: 2
  },
  {
    id: 'queue-3',
    name: 'Seasonal Demand Forecasting',
    agent: 'Predictive AI Agent',
    position: 3
  }
])

// Agent performance metrics
const agentPerformance = ref([
  {
    id: 'agent-1',
    name: 'Inventory AI',
    tasksCompleted: 24,
    successRate: 96
  },
  {
    id: 'agent-2',
    name: 'Sales AI',
    tasksCompleted: 31,
    successRate: 94
  },
  {
    id: 'agent-3',
    name: 'Finance AI',
    tasksCompleted: 18,
    successRate: 92
  },
  {
    id: 'agent-4',
    name: 'Marketing AI',
    tasksCompleted: 15,
    successRate: 98
  }
])

// Auto-refresh interval
let refreshInterval: number | null = null

// Methods
const getTaskStatusColor = (status: string) => {
  switch (status) {
    case 'executing':
      return 'bg-blue-400'
    case 'completed':
      return 'bg-green-400'
    case 'failed':
      return 'bg-red-400'
    case 'queued':
      return 'bg-yellow-400'
    default:
      return 'bg-gray-400'
  }
}

const getTaskStatusBadge = (status: string) => {
  switch (status) {
    case 'executing':
      return 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-300'
    case 'completed':
      return 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-300'
    case 'failed':
      return 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-300'
    case 'queued':
      return 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-300'
    default:
      return 'bg-gray-100 text-gray-800 dark:bg-gray-900 dark:text-gray-300'
  }
}

const formatTime = (date: Date): string => {
  return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
}

const formatDuration = (milliseconds: number): string => {
  const minutes = Math.floor(milliseconds / 60000)
  const hours = Math.floor(minutes / 60)
  
  if (hours > 0) {
    return `${hours}h ${minutes % 60}m`
  }
  return `${minutes}m`
}

const pauseAllTasks = () => {
  console.log('Pausing all autonomous tasks...')
  // Implementation would pause all active AI agents
}

const createNewTask = () => {
  console.log('Creating new autonomous task...')
  // Implementation would show task creation modal
}

const createTask = (type: string) => {
  console.log(`Creating ${type} task...`)
  // Implementation would create specific task type
}

const viewTaskDetails = (task: any) => {
  console.log('Viewing task details:', task.name)
  // Implementation would show detailed task view
}

const toggleAutoRefresh = () => {
  autoRefresh.value = !autoRefresh.value
  if (autoRefresh.value) {
    startAutoRefresh()
  } else {
    stopAutoRefresh()
  }
}

const startAutoRefresh = () => {
  refreshInterval = setInterval(() => {
    // Simulate task progress updates
    activeTasks.value.forEach(task => {
      if (task.status === 'executing' && task.progress < 100) {
        task.progress = Math.min(100, task.progress + Math.random() * 5)
        
        // Update step status based on progress
        const completedSteps = Math.floor((task.progress / 100) * task.steps.length)
        task.steps.forEach((step, index) => {
          if (index < completedSteps) {
            step.status = 'completed'
          } else if (index === completedSteps) {
            step.status = 'active'
          } else {
            step.status = 'pending'
          }
        })
      }
    })
  }, 3000) as unknown as number // Update every 3 seconds
}

const stopAutoRefresh = () => {
  if (refreshInterval) {
    clearInterval(refreshInterval)
    refreshInterval = null
  }
}

// Lifecycle
onMounted(() => {
  if (autoRefresh.value) {
    startAutoRefresh()
  }
})

onUnmounted(() => {
  stopAutoRefresh()
})

// Page meta
definePageMeta({
  title: 'Autonomous Task Execution',
  description: 'Monitor and control AI agents executing business tasks autonomously'
})

// SEO
useHead({
  title: 'Autonomous Task Execution - TOSS ERP',
  meta: [
    { name: 'description', content: 'Real-time monitoring and control of autonomous AI agents executing business tasks without human intervention.' }
  ]
})
</script>

<style scoped>
/* Custom animations */
@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

.animate-spin {
  animation: spin 1s linear infinite;
}

@keyframes pulse {
  0%, 100% {
    opacity: 1;
  }
  50% {
    opacity: 0.5;
  }
}

.animate-pulse {
  animation: pulse 2s cubic-bezier(0.4, 0, 0.6, 1) infinite;
}

/* Progress bar animations */
.transition-all {
  transition-property: all;
  transition-timing-function: cubic-bezier(0.4, 0, 0.2, 1);
  transition-duration: 300ms;
}

/* Toggle switch */
.transform {
  transform: var(--tw-transform);
}

.translate-x-0 {
  --tw-translate-x: 0px;
  transform: translate(var(--tw-translate-x), var(--tw-translate-y)) rotate(var(--tw-rotate)) skewX(var(--tw-skew-x)) skewY(var(--tw-skew-y)) scaleX(var(--tw-scale-x)) scaleY(var(--tw-scale-y));
}

.translate-x-5 {
  --tw-translate-x: 1.25rem;
  transform: translate(var(--tw-translate-x), var(--tw-translate-y)) rotate(var(--tw-rotate)) skewX(var(--tw-skew-x)) skewY(var(--tw-skew-y)) scaleX(var(--tw-scale-x)) scaleY(var(--tw-scale-y));
}
</style>
