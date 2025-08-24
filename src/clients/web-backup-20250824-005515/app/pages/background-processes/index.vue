<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <!-- Header -->
    <div class="bg-white dark:bg-gray-800 shadow">
      <div class="max-w-7xl mx-auto py-6 px-4 sm:px-6 lg:px-8">
        <div class="md:flex md:items-center md:justify-between">
          <div class="flex-1 min-w-0">
            <h2 class="text-2xl font-bold leading-7 text-gray-900 dark:text-white sm:text-3xl sm:truncate">
              Background Processes
            </h2>
            <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
              Monitor continuous AI-driven business operations running autonomously
            </p>
          </div>
          <div class="mt-4 flex md:mt-0 md:ml-4 space-x-3">
            <div class="flex items-center space-x-2">
              <div class="w-3 h-3 bg-green-400 rounded-full animate-pulse"></div>
              <span class="text-sm text-gray-600 dark:text-gray-300">{{ activeProcesses }} active processes</span>
            </div>
            <button
              @click="toggleAllProcesses"
              type="button"
              class="inline-flex items-center px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm text-sm font-medium text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600"
            >
              <svg class="-ml-1 mr-2 h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 12a9 9 0 01-9 9m9-9a9 9 0 00-9-9m9 9c1.657 0 3-4.03 3-9s-1.343-9-3-9m0 18c-1.657 0-3-4.03-3-9s1.343-9 3-9m-9 9a9 9 0 019-9" />
              </svg>
              {{ allProcessesRunning ? 'Pause All' : 'Resume All' }}
            </button>
          </div>
        </div>
      </div>
    </div>

    <div class="max-w-7xl mx-auto py-6 px-4 sm:px-6 lg:px-8">
      <div class="grid grid-cols-1 xl:grid-cols-3 gap-6">
        
        <!-- Process Categories -->
        <div class="xl:col-span-2 space-y-6">
          
          <!-- Real-time Data Processing -->
          <div class="bg-white dark:bg-gray-800 rounded-lg shadow">
            <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
              <div class="flex items-center justify-between">
                <h3 class="text-lg font-medium text-gray-900 dark:text-white">Real-time Data Processing</h3>
                <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-300">
                  Active
                </span>
              </div>
            </div>
            <div class="p-6">
              <div class="space-y-4">
                <div v-for="process in dataProcesses" :key="process.id" class="border border-gray-200 dark:border-gray-700 rounded-lg p-4">
                  <div class="flex items-center justify-between mb-3">
                    <div class="flex items-center">
                      <div class="w-8 h-8 rounded-lg flex items-center justify-center mr-3" :class="process.statusColor">
                        <component :is="process.icon" class="w-4 h-4 text-white" />
                      </div>
                      <div>
                        <h4 class="text-sm font-medium text-gray-900 dark:text-white">{{ process.name }}</h4>
                        <p class="text-xs text-gray-500 dark:text-gray-400">{{ process.description }}</p>
                      </div>
                    </div>
                    <div class="flex items-center space-x-3">
                      <div class="text-right">
                        <p class="text-sm font-medium text-gray-900 dark:text-white">{{ process.throughput }}</p>
                        <p class="text-xs text-gray-500 dark:text-gray-400">records/min</p>
                      </div>
                      <button @click="toggleProcess(process)" class="text-gray-400 hover:text-gray-600 dark:hover:text-gray-300">
                        <svg v-if="process.running" class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 9v6m4-6v6m7-3a9 9 0 11-18 0 9 9 0 0118 0z" />
                        </svg>
                        <svg v-else class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M14.828 14.828a4 4 0 01-5.656 0M9 10h1m4 0h1M9 16h1m4 0h1M12 3v1m0 16v1m9-9h-1M3 12H2m1.414-5.828l.707.707m14.142 0l.707-.707m-14.142 14.142l.707-.707m14.142 0l.707.707" />
                        </svg>
                      </button>
                    </div>
                  </div>
                  
                  <!-- Process Metrics -->
                  <div class="grid grid-cols-4 gap-4 mb-4">
                    <div class="text-center">
                      <p class="text-xs text-gray-500 dark:text-gray-400">Uptime</p>
                      <p class="text-sm font-medium text-gray-900 dark:text-white">{{ process.uptime }}</p>
                    </div>
                    <div class="text-center">
                      <p class="text-xs text-gray-500 dark:text-gray-400">Processed</p>
                      <p class="text-sm font-medium text-gray-900 dark:text-white">{{ process.processed }}</p>
                    </div>
                    <div class="text-center">
                      <p class="text-xs text-gray-500 dark:text-gray-400">Errors</p>
                      <p class="text-sm font-medium text-gray-900 dark:text-white">{{ process.errors }}</p>
                    </div>
                    <div class="text-center">
                      <p class="text-xs text-gray-500 dark:text-gray-400">CPU Usage</p>
                      <p class="text-sm font-medium text-gray-900 dark:text-white">{{ process.cpuUsage }}%</p>
                    </div>
                  </div>

                  <!-- Activity Timeline -->
                  <div class="bg-gray-50 dark:bg-gray-700 rounded-lg p-3">
                    <h5 class="text-xs font-medium text-gray-700 dark:text-gray-300 mb-2">Recent Activity</h5>
                    <div class="space-y-1">
                      <div v-for="activity in process.recentActivity" :key="activity.id" class="flex items-center justify-between text-xs">
                        <span class="text-gray-600 dark:text-gray-400">{{ activity.action }}</span>
                        <span class="text-gray-500 dark:text-gray-500">{{ formatTime(activity.timestamp) }}</span>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Automated Business Workflows -->
          <div class="bg-white dark:bg-gray-800 rounded-lg shadow">
            <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
              <div class="flex items-center justify-between">
                <h3 class="text-lg font-medium text-gray-900 dark:text-white">Automated Business Workflows</h3>
                <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-300">
                  {{ workflowProcesses.filter(p => p.running).length }} Running
                </span>
              </div>
            </div>
            <div class="p-6">
              <div class="space-y-4">
                <div v-for="workflow in workflowProcesses" :key="workflow.id" class="border border-gray-200 dark:border-gray-700 rounded-lg p-4">
                  <div class="flex items-center justify-between mb-3">
                    <div class="flex items-center">
                      <div class="w-8 h-8 rounded-lg flex items-center justify-center mr-3" :class="workflow.statusColor">
                        <component :is="workflow.icon" class="w-4 h-4 text-white" />
                      </div>
                      <div>
                        <h4 class="text-sm font-medium text-gray-900 dark:text-white">{{ workflow.name }}</h4>
                        <p class="text-xs text-gray-500 dark:text-gray-400">{{ workflow.description }}</p>
                      </div>
                    </div>
                    <div class="flex items-center space-x-3">
                      <span class="text-xs font-medium px-2 py-1 rounded" :class="workflow.running ? 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-300' : 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300'">
                        {{ workflow.running ? 'Active' : 'Paused' }}
                      </span>
                      <button @click="toggleWorkflow(workflow)" class="text-gray-400 hover:text-gray-600 dark:hover:text-gray-300">
                        <svg v-if="workflow.running" class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 9v6m4-6v6m7-3a9 9 0 11-18 0 9 9 0 0118 0z" />
                        </svg>
                        <svg v-else class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M14.828 14.828a4 4 0 01-5.656 0M9 10h1m4 0h1M9 16h1m4 0h1M12 3v1m0 16v1m9-9h-1M3 12H2m1.414-5.828l.707.707m14.142 0l.707-.707m-14.142 14.142l.707-.707m14.142 0l.707.707" />
                        </svg>
                      </button>
                    </div>
                  </div>

                  <!-- Workflow Steps -->
                  <div class="bg-gray-50 dark:bg-gray-700 rounded-lg p-3 mb-3">
                    <h5 class="text-xs font-medium text-gray-700 dark:text-gray-300 mb-2">Workflow Steps</h5>
                    <div class="flex items-center space-x-2 overflow-x-auto pb-2">
                      <div v-for="(step, index) in workflow.steps" :key="index" class="flex items-center space-x-2 flex-shrink-0">
                        <div class="w-6 h-6 rounded-full flex items-center justify-center text-xs font-medium" :class="step.completed ? 'bg-green-500 text-white' : step.active ? 'bg-blue-500 text-white animate-pulse' : 'bg-gray-300 dark:bg-gray-600 text-gray-600 dark:text-gray-400'">
                          {{ index + 1 }}
                        </div>
                        <span class="text-xs text-gray-600 dark:text-gray-400 whitespace-nowrap">{{ step.name }}</span>
                        <svg v-if="index < workflow.steps.length - 1" class="w-4 h-4 text-gray-300 dark:text-gray-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
                        </svg>
                      </div>
                    </div>
                  </div>

                  <!-- Workflow Metrics -->
                  <div class="grid grid-cols-3 gap-4 text-center">
                    <div>
                      <p class="text-xs text-gray-500 dark:text-gray-400">Executions Today</p>
                      <p class="text-sm font-medium text-gray-900 dark:text-white">{{ workflow.executionsToday }}</p>
                    </div>
                    <div>
                      <p class="text-xs text-gray-500 dark:text-gray-400">Success Rate</p>
                      <p class="text-sm font-medium text-gray-900 dark:text-white">{{ workflow.successRate }}%</p>
                    </div>
                    <div>
                      <p class="text-xs text-gray-500 dark:text-gray-400">Avg Duration</p>
                      <p class="text-sm font-medium text-gray-900 dark:text-white">{{ workflow.avgDuration }}</p>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Monitoring & Alerts -->
          <div class="bg-white dark:bg-gray-800 rounded-lg shadow">
            <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
              <div class="flex items-center justify-between">
                <h3 class="text-lg font-medium text-gray-900 dark:text-white">Monitoring & Alerts</h3>
                <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-300">
                  {{ alerts.filter(a => !a.resolved).length }} Active Alerts
                </span>
              </div>
            </div>
            <div class="p-6">
              <div class="space-y-3">
                <div v-for="alert in alerts" :key="alert.id" class="flex items-center justify-between p-3 rounded-lg" :class="alert.resolved ? 'bg-gray-50 dark:bg-gray-700' : getSeverityBg(alert.severity)">
                  <div class="flex items-center">
                    <div class="w-8 h-8 rounded-lg flex items-center justify-center mr-3" :class="getSeverityColor(alert.severity)">
                      <svg v-if="alert.severity === 'critical'" class="w-4 h-4 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L3.732 16.5c-.77.833.192 2.5 1.732 2.5z" />
                      </svg>
                      <svg v-else-if="alert.severity === 'warning'" class="w-4 h-4 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                      </svg>
                      <svg v-else class="w-4 h-4 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                      </svg>
                    </div>
                    <div>
                      <h4 class="text-sm font-medium text-gray-900 dark:text-white">{{ alert.title }}</h4>
                      <p class="text-xs text-gray-500 dark:text-gray-400">{{ alert.message }}</p>
                    </div>
                  </div>
                  <div class="flex items-center space-x-2">
                    <span class="text-xs text-gray-500 dark:text-gray-400">{{ formatTime(alert.timestamp) }}</span>
                    <button v-if="!alert.resolved" @click="resolveAlert(alert)" class="text-green-600 hover:text-green-800 dark:text-green-400 dark:hover:text-green-300">
                      <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
                      </svg>
                    </button>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- System Resources & Performance -->
        <div class="space-y-6">
          <!-- System Health -->
          <div class="bg-white dark:bg-gray-800 rounded-lg shadow">
            <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
              <h3 class="text-lg font-medium text-gray-900 dark:text-white">System Health</h3>
            </div>
            <div class="p-6 space-y-4">
              <div v-for="metric in systemMetrics" :key="metric.name" class="flex items-center justify-between">
                <div>
                  <h4 class="text-sm font-medium text-gray-900 dark:text-white">{{ metric.name }}</h4>
                  <p class="text-xs text-gray-500 dark:text-gray-400">{{ metric.description }}</p>
                </div>
                <div class="text-right">
                  <p class="text-sm font-medium text-gray-900 dark:text-white">{{ metric.value }}{{ metric.unit }}</p>
                  <div class="w-20 h-2 bg-gray-200 dark:bg-gray-700 rounded-full mt-1">
                    <div class="h-2 rounded-full transition-all duration-300" :class="getMetricColor(metric.value, metric.threshold)" :style="{ width: `${Math.min(100, (metric.value / metric.max) * 100)}%` }"></div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Process Statistics -->
          <div class="bg-white dark:bg-gray-800 rounded-lg shadow">
            <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
              <h3 class="text-lg font-medium text-gray-900 dark:text-white">Process Statistics</h3>
            </div>
            <div class="p-6 space-y-4">
              <div class="grid grid-cols-2 gap-4">
                <div class="text-center">
                  <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ totalProcessesRunning }}</p>
                  <p class="text-xs text-gray-500 dark:text-gray-400">Active Processes</p>
                </div>
                <div class="text-center">
                  <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ totalProcessesIdle }}</p>
                  <p class="text-xs text-gray-500 dark:text-gray-400">Idle Processes</p>
                </div>
              </div>
              
              <div class="border-t border-gray-200 dark:border-gray-700 pt-4">
                <h4 class="text-sm font-medium text-gray-900 dark:text-white mb-3">Performance Trends</h4>
                <div class="space-y-2">
                  <div class="flex items-center justify-between text-sm">
                    <span class="text-gray-600 dark:text-gray-400">Throughput</span>
                    <span class="font-medium text-green-600 dark:text-green-400">+12% ↗</span>
                  </div>
                  <div class="flex items-center justify-between text-sm">
                    <span class="text-gray-600 dark:text-gray-400">Error Rate</span>
                    <span class="font-medium text-red-600 dark:text-red-400">-5% ↘</span>
                  </div>
                  <div class="flex items-center justify-between text-sm">
                    <span class="text-gray-600 dark:text-gray-400">Efficiency</span>
                    <span class="font-medium text-blue-600 dark:text-blue-400">+8% ↗</span>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Quick Actions -->
          <div class="bg-white dark:bg-gray-800 rounded-lg shadow">
            <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
              <h3 class="text-lg font-medium text-gray-900 dark:text-white">Quick Actions</h3>
            </div>
            <div class="p-6 space-y-3">
              <button @click="restartAllProcesses" class="w-full flex items-center justify-center px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm text-sm font-medium text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600">
                <svg class="-ml-1 mr-2 h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
                </svg>
                Restart All Processes
              </button>
              
              <button @click="exportLogs" class="w-full flex items-center justify-center px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm text-sm font-medium text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600">
                <svg class="-ml-1 mr-2 h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 10v6m0 0l-3-3m3 3l3-3m2 8H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
                </svg>
                Export Process Logs
              </button>
              
              <button @click="optimizePerformance" class="w-full flex items-center justify-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-indigo-600 hover:bg-indigo-700">
                <svg class="-ml-1 mr-2 h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 10V3L4 14h7v7l9-11h-7z" />
                </svg>
                Optimize Performance
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'

// Mock Nuxt functions
function definePageMeta(meta: any) {}
function useHead(options: any) {
  if (typeof document !== 'undefined') {
    document.title = options.title || 'Background Processes - TOSS ERP'
  }
}

// Reactive data
const allProcessesRunning = ref(true)

// Data processing background processes
const dataProcesses = ref([
  {
    id: 'data-1',
    name: 'Sales Data Aggregator',
    description: 'Continuously processing incoming sales transactions and updating analytics',
    icon: 'chart-icon',
    statusColor: 'bg-green-500',
    running: true,
    throughput: 847,
    uptime: '15d 4h',
    processed: '2.4M',
    errors: 0,
    cpuUsage: 23,
    recentActivity: [
      { id: 1, action: 'Processed batch #4782', timestamp: new Date(Date.now() - 120000) },
      { id: 2, action: 'Updated dashboard metrics', timestamp: new Date(Date.now() - 180000) },
      { id: 3, action: 'Validated data integrity', timestamp: new Date(Date.now() - 240000) }
    ]
  },
  {
    id: 'data-2',
    name: 'Inventory Synchronizer',
    description: 'Real-time inventory level updates across all sales channels',
    icon: 'box-icon',
    statusColor: 'bg-blue-500',
    running: true,
    throughput: 342,
    uptime: '8d 12h',
    processed: '945K',
    errors: 2,
    cpuUsage: 18,
    recentActivity: [
      { id: 1, action: 'Synced stock levels', timestamp: new Date(Date.now() - 90000) },
      { id: 2, action: 'Updated product availability', timestamp: new Date(Date.now() - 150000) },
      { id: 3, action: 'Processed reorder alerts', timestamp: new Date(Date.now() - 210000) }
    ]
  },
  {
    id: 'data-3',
    name: 'Financial Data Processor',
    description: 'Automated processing of financial transactions and reconciliation',
    icon: 'money-icon',
    statusColor: 'bg-yellow-500',
    running: true,
    throughput: 156,
    uptime: '22d 8h',
    processed: '1.8M',
    errors: 1,
    cpuUsage: 31,
    recentActivity: [
      { id: 1, action: 'Reconciled bank transactions', timestamp: new Date(Date.now() - 60000) },
      { id: 2, action: 'Generated payment reports', timestamp: new Date(Date.now() - 120000) },
      { id: 3, action: 'Updated GL accounts', timestamp: new Date(Date.now() - 180000) }
    ]
  }
])

// Automated workflow processes
const workflowProcesses = ref([
  {
    id: 'workflow-1',
    name: 'Order Fulfillment Pipeline',
    description: 'End-to-end automated order processing from receipt to shipping',
    icon: 'truck-icon',
    statusColor: 'bg-green-500',
    running: true,
    executionsToday: 127,
    successRate: 98.5,
    avgDuration: '4.2min',
    steps: [
      { name: 'Order Validation', completed: true, active: false },
      { name: 'Inventory Check', completed: true, active: false },
      { name: 'Payment Processing', completed: false, active: true },
      { name: 'Fulfillment', completed: false, active: false },
      { name: 'Shipping', completed: false, active: false }
    ]
  },
  {
    id: 'workflow-2',
    name: 'Customer Service Automation',
    description: 'Automated customer inquiry processing and response generation',
    icon: 'chat-icon',
    statusColor: 'bg-blue-500',
    running: true,
    executionsToday: 89,
    successRate: 94.2,
    avgDuration: '2.8min',
    steps: [
      { name: 'Inquiry Analysis', completed: true, active: false },
      { name: 'Intent Classification', completed: true, active: false },
      { name: 'Response Generation', completed: true, active: false },
      { name: 'Quality Check', completed: false, active: true },
      { name: 'Customer Delivery', completed: false, active: false }
    ]
  },
  {
    id: 'workflow-3',
    name: 'Invoice Generation & Distribution',
    description: 'Automated invoice creation and delivery to customers',
    icon: 'document-icon',
    statusColor: 'bg-purple-500',
    running: false,
    executionsToday: 45,
    successRate: 99.1,
    avgDuration: '1.5min',
    steps: [
      { name: 'Data Collection', completed: false, active: false },
      { name: 'Invoice Creation', completed: false, active: false },
      { name: 'Tax Calculation', completed: false, active: false },
      { name: 'PDF Generation', completed: false, active: false },
      { name: 'Email Delivery', completed: false, active: false }
    ]
  }
])

// System alerts
const alerts = ref([
  {
    id: 'alert-1',
    title: 'High CPU Usage Detected',
    message: 'Financial Data Processor is using 31% CPU - consider optimization',
    severity: 'warning',
    timestamp: new Date(Date.now() - 300000),
    resolved: false
  },
  {
    id: 'alert-2',
    title: 'Process Error Recovery',
    message: 'Inventory Synchronizer recovered from 2 errors in the last hour',
    severity: 'info',
    timestamp: new Date(Date.now() - 600000),
    resolved: false
  },
  {
    id: 'alert-3',
    title: 'Workflow Completed Successfully',
    message: 'Order Fulfillment Pipeline completed 127 orders today',
    severity: 'info',
    timestamp: new Date(Date.now() - 900000),
    resolved: true
  }
])

// System metrics
const systemMetrics = ref([
  {
    name: 'CPU Usage',
    description: 'Overall system processor utilization',
    value: 45,
    unit: '%',
    max: 100,
    threshold: 80
  },
  {
    name: 'Memory',
    description: 'RAM utilization across all processes',
    value: 62,
    unit: '%',
    max: 100,
    threshold: 85
  },
  {
    name: 'Disk I/O',
    description: 'Storage read/write operations',
    value: 28,
    unit: '%',
    max: 100,
    threshold: 70
  },
  {
    name: 'Network',
    description: 'Network bandwidth utilization',
    value: 15,
    unit: '%',
    max: 100,
    threshold: 75
  }
])

// Computed properties
const activeProcesses = computed(() => {
  return dataProcesses.value.filter(p => p.running).length + 
         workflowProcesses.value.filter(p => p.running).length
})

const totalProcessesRunning = computed(() => {
  return dataProcesses.value.filter(p => p.running).length + 
         workflowProcesses.value.filter(p => p.running).length
})

const totalProcessesIdle = computed(() => {
  return dataProcesses.value.filter(p => !p.running).length + 
         workflowProcesses.value.filter(p => !p.running).length
})

// Auto-refresh interval
let refreshInterval: number | null = null

// Methods
const formatTime = (date: Date): string => {
  return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
}

const getSeverityColor = (severity: string): string => {
  switch (severity) {
    case 'critical':
      return 'bg-red-500'
    case 'warning':
      return 'bg-yellow-500'
    case 'info':
      return 'bg-blue-500'
    default:
      return 'bg-gray-500'
  }
}

const getSeverityBg = (severity: string): string => {
  switch (severity) {
    case 'critical':
      return 'bg-red-50 dark:bg-red-900/20 border-red-200 dark:border-red-800'
    case 'warning':
      return 'bg-yellow-50 dark:bg-yellow-900/20 border-yellow-200 dark:border-yellow-800'
    case 'info':
      return 'bg-blue-50 dark:bg-blue-900/20 border-blue-200 dark:border-blue-800'
    default:
      return 'bg-gray-50 dark:bg-gray-700'
  }
}

const getMetricColor = (value: number, threshold: number): string => {
  if (value >= threshold) {
    return 'bg-red-500'
  } else if (value >= threshold * 0.8) {
    return 'bg-yellow-500'
  } else {
    return 'bg-green-500'
  }
}

const toggleAllProcesses = () => {
  allProcessesRunning.value = !allProcessesRunning.value
  
  dataProcesses.value.forEach(process => {
    process.running = allProcessesRunning.value
  })
  
  workflowProcesses.value.forEach(workflow => {
    workflow.running = allProcessesRunning.value
  })
  
  console.log(`${allProcessesRunning.value ? 'Resuming' : 'Pausing'} all background processes...`)
}

const toggleProcess = (process: any) => {
  process.running = !process.running
  console.log(`${process.running ? 'Starting' : 'Stopping'} process: ${process.name}`)
}

const toggleWorkflow = (workflow: any) => {
  workflow.running = !workflow.running
  
  if (workflow.running) {
    // Reset workflow steps when starting
    workflow.steps.forEach((step: any, index: number) => {
      step.completed = false
      step.active = index === 0
    })
  } else {
    // Stop all workflow steps
    workflow.steps.forEach((step: any) => {
      step.active = false
    })
  }
  
  console.log(`${workflow.running ? 'Starting' : 'Stopping'} workflow: ${workflow.name}`)
}

const resolveAlert = (alert: any) => {
  alert.resolved = true
  console.log(`Resolved alert: ${alert.title}`)
}

const restartAllProcesses = () => {
  console.log('Restarting all background processes...')
  // Implementation would restart all processes
}

const exportLogs = () => {
  console.log('Exporting process logs...')
  // Implementation would export logs
}

const optimizePerformance = () => {
  console.log('Optimizing system performance...')
  // Implementation would run performance optimization
}

const startAutoRefresh = () => {
  refreshInterval = setInterval(() => {
    // Simulate process updates
    dataProcesses.value.forEach(process => {
      if (process.running) {
        // Update throughput with some variation
        process.throughput = Math.max(0, process.throughput + Math.floor(Math.random() * 20 - 10))
        
        // Simulate CPU usage fluctuation
        process.cpuUsage = Math.max(5, Math.min(50, process.cpuUsage + Math.floor(Math.random() * 6 - 3)))
        
        // Add recent activity
        if (Math.random() > 0.7) {
          process.recentActivity.unshift({
            id: Date.now(),
            action: 'Processed data batch',
            timestamp: new Date()
          })
          
          if (process.recentActivity.length > 5) {
            process.recentActivity.pop()
          }
        }
      }
    })
    
    // Simulate workflow progress
    workflowProcesses.value.forEach(workflow => {
      if (workflow.running) {
        const activeStepIndex = workflow.steps.findIndex((step: any) => step.active)
        
        if (activeStepIndex !== -1 && Math.random() > 0.6) {
          // Complete current step and move to next
          workflow.steps[activeStepIndex].completed = true
          workflow.steps[activeStepIndex].active = false
          
          if (activeStepIndex < workflow.steps.length - 1) {
            workflow.steps[activeStepIndex + 1].active = true
          } else {
            // Workflow completed, restart
            workflow.steps.forEach((step: any, index: number) => {
              step.completed = false
              step.active = index === 0
            })
            workflow.executionsToday++
          }
        }
      }
    })
    
    // Update system metrics
    systemMetrics.value.forEach(metric => {
      metric.value = Math.max(0, Math.min(metric.max, metric.value + Math.floor(Math.random() * 10 - 5)))
    })
    
  }, 5000) as unknown as number // Update every 5 seconds
}

const stopAutoRefresh = () => {
  if (refreshInterval) {
    clearInterval(refreshInterval)
    refreshInterval = null
  }
}

// Lifecycle
onMounted(() => {
  startAutoRefresh()
})

onUnmounted(() => {
  stopAutoRefresh()
})

// Page meta
definePageMeta({
  title: 'Background Processes',
  description: 'Monitor and control continuous AI-driven business operations'
})

// SEO
useHead({
  title: 'Background Processes - TOSS ERP',
  meta: [
    { name: 'description', content: 'Real-time monitoring of autonomous background processes powering continuous business operations.' }
  ]
})
</script>

<style scoped>
/* Custom animations */
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

/* Workflow step indicators */
.flex-shrink-0 {
  flex-shrink: 0;
}

/* Scrollbar styling for workflow steps */
.overflow-x-auto::-webkit-scrollbar {
  height: 4px;
}

.overflow-x-auto::-webkit-scrollbar-track {
  background: #f1f1f1;
}

.overflow-x-auto::-webkit-scrollbar-thumb {
  background: #c1c1c1;
  border-radius: 2px;
}

.overflow-x-auto::-webkit-scrollbar-thumb:hover {
  background: #a8a8a8;
}

/* Dark mode scrollbar */
.dark .overflow-x-auto::-webkit-scrollbar-track {
  background: #374151;
}

.dark .overflow-x-auto::-webkit-scrollbar-thumb {
  background: #6b7280;
}

.dark .overflow-x-auto::-webkit-scrollbar-thumb:hover {
  background: #9ca3af;
}
</style>
