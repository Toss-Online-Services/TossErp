<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <!-- Service-as-a-Software Header -->
    <div class="bg-gradient-to-r from-purple-600 via-blue-600 to-indigo-700 dark:from-purple-800 dark:via-blue-800 dark:to-indigo-900">
      <div class="max-w-7xl mx-auto py-8 px-4 sm:py-12 sm:px-6 lg:px-8">
        <div class="text-center">
          <div class="flex justify-center mb-4">
            <div class="w-16 h-16 bg-white/20 rounded-2xl flex items-center justify-center">
              <svg class="w-10 h-10 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.663 17h4.673M12 3v1m6.364 1.636l-.707.707M21 12h-1M4 12H3m3.343-5.657l-.707-.707m2.828 9.9a5 5 0 117.072 0l-.548.547A3.374 3.374 0 0014 18.469V19a2 2 0 11-4 0v-.531c0-.895-.356-1.754-.988-2.386l-.548-.547z" />
              </svg>
            </div>
          </div>
          <h1 class="text-4xl font-extrabold text-white sm:text-5xl md:text-6xl">
            <span class="block">Service-as-a-Software</span>
            <span class="block text-purple-200">AI Business Automation</span>
          </h1>
          <p class="mt-6 max-w-3xl mx-auto text-xl text-purple-100">
            Your AI-driven business assistant that delivers outcomes, not just tools. 
            Automated processes, intelligent decisions, minimal human intervention.
          </p>
          <div class="mt-8 flex justify-center items-center space-x-6 text-purple-200">
            <div class="flex items-center">
              <div class="w-3 h-3 bg-green-400 rounded-full mr-2"></div>
              <span class="text-sm">{{ activeAgents }} AI Agents Active</span>
            </div>
            <div class="flex items-center">
              <div class="w-3 h-3 bg-blue-400 rounded-full mr-2"></div>
              <span class="text-sm">{{ autonomousProcesses }} Autonomous Processes</span>
            </div>
            <div class="flex items-center">
              <div class="w-3 h-3 bg-yellow-400 rounded-full mr-2"></div>
              <span class="text-sm">{{ pendingTasks }} Tasks in Queue</span>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div class="max-w-7xl mx-auto py-8 px-4 sm:px-6 lg:px-8">
      <!-- Service Orchestration Overview -->
      <div class="grid grid-cols-1 lg:grid-cols-4 gap-6 mb-8">
        <!-- Service Status Cards -->
        <div class="bg-white dark:bg-gray-800 rounded-xl shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="w-12 h-12 bg-gradient-to-r from-green-400 to-green-600 rounded-lg flex items-center justify-center">
              <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
              </svg>
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Services Delivered</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ servicesDelivered }}</p>
            </div>
          </div>
          <div class="mt-4">
            <div class="flex items-center text-sm text-green-600 dark:text-green-400">
              <svg class="w-4 h-4 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 7h8m0 0v8m0-8l-8 8-4-4-6 6" />
              </svg>
              +{{ serviceGrowth }}% from last month
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 rounded-xl shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="w-12 h-12 bg-gradient-to-r from-blue-400 to-blue-600 rounded-lg flex items-center justify-center">
              <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 10V3L4 14h7v7l9-11h-7z" />
              </svg>
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Automation Rate</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ automationRate }}%</p>
            </div>
          </div>
          <div class="mt-4">
            <div class="flex items-center text-sm text-blue-600 dark:text-blue-400">
              <svg class="w-4 h-4 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 7h8m0 0v8m0-8l-8 8-4-4-6 6" />
              </svg>
              +{{ automationGrowth }}% this quarter
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 rounded-xl shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="w-12 h-12 bg-gradient-to-r from-purple-400 to-purple-600 rounded-lg flex items-center justify-center">
              <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Response Time</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ responseTime }}s</p>
            </div>
          </div>
          <div class="mt-4">
            <div class="flex items-center text-sm text-green-600 dark:text-green-400">
              <svg class="w-4 h-4 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 17h8m0 0V9m0 8l-8-8-4 4-6-6" />
              </svg>
              -{{ responseImprovement }}% faster
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 rounded-xl shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="w-12 h-12 bg-gradient-to-r from-orange-400 to-orange-600 rounded-lg flex items-center justify-center">
              <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Success Rate</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ successRate }}%</p>
            </div>
          </div>
          <div class="mt-4">
            <div class="flex items-center text-sm text-green-600 dark:text-green-400">
              <svg class="w-4 h-4 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 7h8m0 0v8m0-8l-8 8-4-4-6 6" />
              </svg>
              +{{ successImprovement }}% improvement
            </div>
          </div>
        </div>
      </div>

      <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
        <!-- AI Agents & Orchestration -->
        <div class="lg:col-span-2 space-y-6">
          <!-- Active AI Agents -->
          <div class="bg-white dark:bg-gray-800 rounded-xl shadow-sm border border-gray-200 dark:border-gray-700">
            <div class="p-6 border-b border-gray-200 dark:border-gray-700">
              <div class="flex items-center justify-between">
                <div>
                  <h3 class="text-lg font-medium text-gray-900 dark:text-white">AI Agent Orchestration</h3>
                  <p class="text-sm text-gray-500 dark:text-gray-400">Autonomous agents delivering business services</p>
                </div>
                <button @click="refreshAgents" class="inline-flex items-center px-3 py-2 border border-gray-300 dark:border-gray-600 shadow-sm text-sm leading-4 font-medium rounded-md text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600">
                  <svg class="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
                  </svg>
                  Refresh
                </button>
              </div>
            </div>
            <div class="p-6">
              <div class="space-y-4">
                <div v-for="agent in aiAgents" :key="agent.id" class="border border-gray-200 dark:border-gray-700 rounded-lg p-4">
                  <div class="flex items-center justify-between mb-3">
                    <div class="flex items-center">
                      <div class="w-3 h-3 rounded-full mr-3" :class="getStatusColor(agent.status)"></div>
                      <div>
                        <h4 class="text-sm font-medium text-gray-900 dark:text-white">{{ agent.name }}</h4>
                        <p class="text-xs text-gray-500 dark:text-gray-400">{{ agent.type }}</p>
                      </div>
                    </div>
                    <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium" :class="getStatusBadge(agent.status)">
                      {{ agent.status }}
                    </span>
                  </div>
                  <p class="text-sm text-gray-600 dark:text-gray-300 mb-3">{{ agent.currentTask }}</p>
                  <div class="flex items-center justify-between text-xs text-gray-500 dark:text-gray-400">
                    <span>Efficiency: {{ agent.efficiency }}%</span>
                    <span>Tasks: {{ agent.tasksCompleted }}/{{ agent.totalTasks }}</span>
                    <span>Uptime: {{ agent.uptime }}</span>
                  </div>
                  <div class="mt-3">
                    <div class="w-full bg-gray-200 dark:bg-gray-700 rounded-full h-1.5">
                      <div class="bg-gradient-to-r from-purple-500 to-blue-500 h-1.5 rounded-full transition-all duration-300" :style="{ width: `${agent.efficiency}%` }"></div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Service Execution Pipeline -->
          <div class="bg-white dark:bg-gray-800 rounded-xl shadow-sm border border-gray-200 dark:border-gray-700">
            <div class="p-6 border-b border-gray-200 dark:border-gray-700">
              <h3 class="text-lg font-medium text-gray-900 dark:text-white">Service Execution Pipeline</h3>
              <p class="text-sm text-gray-500 dark:text-gray-400">Real-time view of autonomous service delivery</p>
            </div>
            <div class="p-6">
              <div class="space-y-4">
                <div v-for="service in serviceExecution" :key="service.id" class="flex items-center justify-between p-4 bg-gray-50 dark:bg-gray-700 rounded-lg">
                  <div class="flex items-center">
                    <div class="w-8 h-8 rounded-lg flex items-center justify-center mr-3" :class="getServiceIcon(service.type)">
                      <component :is="getServiceIconComponent(service.type)" class="w-4 h-4 text-white" />
                    </div>
                    <div>
                      <h4 class="text-sm font-medium text-gray-900 dark:text-white">{{ service.name }}</h4>
                      <p class="text-xs text-gray-500 dark:text-gray-400">{{ service.description }}</p>
                    </div>
                  </div>
                  <div class="flex items-center space-x-3">
                    <div class="text-right">
                      <p class="text-sm font-medium text-gray-900 dark:text-white">{{ service.progress }}%</p>
                      <p class="text-xs text-gray-500 dark:text-gray-400">{{ service.eta }}</p>
                    </div>
                    <div class="w-16 h-16">
                      <svg class="w-16 h-16 transform -rotate-90" viewBox="0 0 36 36">
                        <path class="text-gray-300 dark:text-gray-600" stroke="currentColor" stroke-width="3" fill="none" d="M18 2.0845 a 15.9155 15.9155 0 0 1 0 31.831 a 15.9155 15.9155 0 0 1 0 -31.831"/>
                        <path class="text-purple-500" stroke="currentColor" stroke-width="3" fill="none" stroke-linecap="round" :stroke-dasharray="`${service.progress}, 100`" d="M18 2.0845 a 15.9155 15.9155 0 0 1 0 31.831 a 15.9155 15.9155 0 0 1 0 -31.831"/>
                      </svg>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Service Controls & Monitoring -->
        <div class="space-y-6">
          <!-- Quick Service Controls -->
          <div class="bg-white dark:bg-gray-800 rounded-xl shadow-sm border border-gray-200 dark:border-gray-700">
            <div class="p-6 border-b border-gray-200 dark:border-gray-700">
              <h3 class="text-lg font-medium text-gray-900 dark:text-white">Service Controls</h3>
              <p class="text-sm text-gray-500 dark:text-gray-400">Manage autonomous services</p>
            </div>
            <div class="p-6 space-y-4">
              <button @click="requestService('inventory')" class="w-full flex items-center justify-between p-3 text-left border border-gray-200 dark:border-gray-600 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors">
                <div class="flex items-center">
                  <div class="w-8 h-8 bg-blue-500 rounded-lg flex items-center justify-center mr-3">
                    <svg class="w-4 h-4 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4" />
                    </svg>
                  </div>
                  <span class="text-sm font-medium text-gray-900 dark:text-white">Inventory Management</span>
                </div>
                <svg class="w-4 h-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
                </svg>
              </button>

              <button @click="requestService('sales')" class="w-full flex items-center justify-between p-3 text-left border border-gray-200 dark:border-gray-600 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors">
                <div class="flex items-center">
                  <div class="w-8 h-8 bg-green-500 rounded-lg flex items-center justify-center mr-3">
                    <svg class="w-4 h-4 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                    </svg>
                  </div>
                  <span class="text-sm font-medium text-gray-900 dark:text-white">Sales Processing</span>
                </div>
                <svg class="w-4 h-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
                </svg>
              </button>

              <button @click="requestService('finance')" class="w-full flex items-center justify-between p-3 text-left border border-gray-200 dark:border-gray-600 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors">
                <div class="flex items-center">
                  <div class="w-8 h-8 bg-yellow-500 rounded-lg flex items-center justify-center mr-3">
                    <svg class="w-4 h-4 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
                    </svg>
                  </div>
                  <span class="text-sm font-medium text-gray-900 dark:text-white">Financial Analysis</span>
                </div>
                <svg class="w-4 h-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
                </svg>
              </button>

              <button @click="requestService('marketing')" class="w-full flex items-center justify-between p-3 text-left border border-gray-200 dark:border-gray-600 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors">
                <div class="flex items-center">
                  <div class="w-8 h-8 bg-purple-500 rounded-lg flex items-center justify-center mr-3">
                    <svg class="w-4 h-4 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5.882V19.24a1.76 1.76 0 01-3.417.592l-2.147-6.15M18 13a3 3 0 100-6M5.436 13.683A4.001 4.001 0 017 6h1.832c4.1 0 7.625-1.234 9.168-3v14c-1.543-1.766-5.067-3-9.168-3H7a3.988 3.988 0 01-1.564-.317z" />
                    </svg>
                  </div>
                  <span class="text-sm font-medium text-gray-900 dark:text-white">Marketing Automation</span>
                </div>
                <svg class="w-4 h-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
                </svg>
              </button>
            </div>
          </div>

          <!-- Recent Service Outcomes -->
          <div class="bg-white dark:bg-gray-800 rounded-xl shadow-sm border border-gray-200 dark:border-gray-700">
            <div class="p-6 border-b border-gray-200 dark:border-gray-700">
              <h3 class="text-lg font-medium text-gray-900 dark:text-white">Recent Outcomes</h3>
              <p class="text-sm text-gray-500 dark:text-gray-400">Services delivered autonomously</p>
            </div>
            <div class="p-6">
              <div class="space-y-4">
                <div v-for="outcome in recentOutcomes" :key="outcome.id" class="flex items-start space-x-3">
                  <div class="w-2 h-2 mt-2 rounded-full" :class="outcome.success ? 'bg-green-400' : 'bg-red-400'"></div>
                  <div class="flex-1 min-w-0">
                    <p class="text-sm text-gray-900 dark:text-white">{{ outcome.description }}</p>
                    <div class="flex items-center mt-1 space-x-2">
                      <span class="text-xs text-gray-500 dark:text-gray-400">{{ formatTime(outcome.timestamp) }}</span>
                      <span class="text-xs text-gray-500 dark:text-gray-400">•</span>
                      <span class="text-xs" :class="outcome.success ? 'text-green-600 dark:text-green-400' : 'text-red-600 dark:text-red-400'">
                        {{ outcome.success ? 'Success' : 'Failed' }}
                      </span>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- System Health -->
          <div class="bg-white dark:bg-gray-800 rounded-xl shadow-sm border border-gray-200 dark:border-gray-700">
            <div class="p-6 border-b border-gray-200 dark:border-gray-700">
              <h3 class="text-lg font-medium text-gray-900 dark:text-white">System Health</h3>
              <p class="text-sm text-gray-500 dark:text-gray-400">AI infrastructure status</p>
            </div>
            <div class="p-6 space-y-4">
              <div class="flex items-center justify-between">
                <span class="text-sm text-gray-600 dark:text-gray-300">AI Models</span>
                <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-300">
                  Operational
                </span>
              </div>
              <div class="flex items-center justify-between">
                <span class="text-sm text-gray-600 dark:text-gray-300">Orchestration Layer</span>
                <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-300">
                  Healthy
                </span>
              </div>
              <div class="flex items-center justify-between">
                <span class="text-sm text-gray-600 dark:text-gray-300">Background Services</span>
                <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-300">
                  Running
                </span>
              </div>
              <div class="flex items-center justify-between">
                <span class="text-sm text-gray-600 dark:text-gray-300">Data Integration</span>
                <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-300">
                  Syncing
                </span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'

// Mock composables for Nuxt
function definePageMeta(meta: any) {
  // Page meta handling
}

function useHead(options: any) {
  if (typeof document !== 'undefined') {
    document.title = options.title || 'Service-as-a-Software Dashboard - TOSS ERP'
  }
}

// Reactive data
const activeAgents = ref(8)
const autonomousProcesses = ref(12)
const pendingTasks = ref(34)
const servicesDelivered = ref(247)
const serviceGrowth = ref(23)
const automationRate = ref(87)
const automationGrowth = ref(15)
const responseTime = ref(1.2)
const responseImprovement = ref(18)
const successRate = ref(94.2)
const successImprovement = ref(3.8)

// AI Agents data
const aiAgents = ref([
  {
    id: 'agent-1',
    name: 'Inventory AI',
    type: 'Stock Management Agent',
    status: 'active',
    currentTask: 'Optimizing reorder levels for 23 products',
    efficiency: 94,
    tasksCompleted: 847,
    totalTasks: 892,
    uptime: '99.8%'
  },
  {
    id: 'agent-2', 
    name: 'Sales AI',
    type: 'Revenue Optimization Agent',
    status: 'active',
    currentTask: 'Processing customer orders and invoicing',
    efficiency: 91,
    tasksCompleted: 1203,
    totalTasks: 1298,
    uptime: '99.5%'
  },
  {
    id: 'agent-3',
    name: 'Finance AI',
    type: 'Financial Analysis Agent', 
    status: 'processing',
    currentTask: 'Generating monthly financial reports',
    efficiency: 89,
    tasksCompleted: 456,
    totalTasks: 487,
    uptime: '98.9%'
  },
  {
    id: 'agent-4',
    name: 'Marketing AI',
    type: 'Customer Engagement Agent',
    status: 'idle',
    currentTask: 'Waiting for customer segmentation data',
    efficiency: 96,
    tasksCompleted: 234,
    totalTasks: 245,
    uptime: '99.9%'
  }
])

// Service execution pipeline
const serviceExecution = ref([
  {
    id: 'service-1',
    name: 'Stock Reorder Process',
    description: 'Automatically reordering low stock items',
    type: 'inventory',
    progress: 78,
    eta: '5 min remaining'
  },
  {
    id: 'service-2',
    name: 'Customer Invoice Generation',
    description: 'Processing 47 pending invoices',
    type: 'sales',
    progress: 45,
    eta: '12 min remaining'
  },
  {
    id: 'service-3',
    name: 'Financial Report Compilation',
    description: 'Aggregating monthly financial data',
    type: 'finance',
    progress: 23,
    eta: '18 min remaining'
  },
  {
    id: 'service-4',
    name: 'Customer Segmentation',
    description: 'Analyzing customer purchase patterns',
    type: 'marketing',
    progress: 89,
    eta: '2 min remaining'
  }
])

// Recent service outcomes
const recentOutcomes = ref([
  {
    id: 'outcome-1',
    description: 'Successfully processed 23 purchase orders from suppliers',
    success: true,
    timestamp: new Date(Date.now() - 300000)
  },
  {
    id: 'outcome-2',
    description: 'Generated and sent 156 customer invoices via email',
    success: true,
    timestamp: new Date(Date.now() - 600000)
  },
  {
    id: 'outcome-3',
    description: 'Completed inventory valuation and stock optimization',
    success: true,
    timestamp: new Date(Date.now() - 900000)
  },
  {
    id: 'outcome-4',
    description: 'Failed to connect to supplier API for price updates',
    success: false,
    timestamp: new Date(Date.now() - 1200000)
  },
  {
    id: 'outcome-5',
    description: 'Automatically adjusted pricing for 12 products based on market analysis',
    success: true,
    timestamp: new Date(Date.now() - 1800000)
  }
])

// Methods
const getStatusColor = (status: string) => {
  switch (status) {
    case 'active':
      return 'bg-green-400'
    case 'processing':
      return 'bg-blue-400'
    case 'idle':
      return 'bg-yellow-400'
    case 'error':
      return 'bg-red-400'
    default:
      return 'bg-gray-400'
  }
}

const getStatusBadge = (status: string) => {
  switch (status) {
    case 'active':
      return 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-300'
    case 'processing':
      return 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-300'
    case 'idle':
      return 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-300'
    case 'error':
      return 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-300'
    default:
      return 'bg-gray-100 text-gray-800 dark:bg-gray-900 dark:text-gray-300'
  }
}

const getServiceIcon = (type: string) => {
  switch (type) {
    case 'inventory':
      return 'bg-blue-500'
    case 'sales':
      return 'bg-green-500'
    case 'finance':
      return 'bg-yellow-500'
    case 'marketing':
      return 'bg-purple-500'
    default:
      return 'bg-gray-500'
  }
}

const getServiceIconComponent = (type: string) => {
  // Return appropriate SVG component for each service type
  const icons = {
    inventory: 'svg',
    sales: 'svg', 
    finance: 'svg',
    marketing: 'svg'
  }
  return icons[type as keyof typeof icons] || 'svg'
}

const refreshAgents = () => {
  console.log('Refreshing AI agents...')
  // Simulate refresh with slight variations
  aiAgents.value.forEach(agent => {
    agent.efficiency = Math.max(85, Math.min(99, agent.efficiency + (Math.random() - 0.5) * 5))
    agent.tasksCompleted += Math.floor(Math.random() * 5)
  })
}

const requestService = (serviceType: string) => {
  console.log(`Requesting ${serviceType} service...`)
  // In a real implementation, this would trigger the AI orchestration layer
  // to start a new autonomous service process
}

const formatTime = (date: Date): string => {
  const now = new Date()
  const diffMs = now.getTime() - date.getTime()
  const diffMinutes = Math.floor(diffMs / (1000 * 60))
  
  if (diffMinutes < 1) return 'Just now'
  if (diffMinutes === 1) return '1 minute ago'
  if (diffMinutes < 60) return `${diffMinutes} minutes ago`
  
  const diffHours = Math.floor(diffMinutes / 60)
  if (diffHours === 1) return '1 hour ago'
  if (diffHours < 24) return `${diffHours} hours ago`
  
  const diffDays = Math.floor(diffHours / 24)
  if (diffDays === 1) return '1 day ago'
  return `${diffDays} days ago`
}

// Initialize data on mount
onMounted(() => {
  console.log('Service-as-a-Software Dashboard initialized')
})

// Page meta
definePageMeta({
  title: 'Service-as-a-Software Dashboard',
  description: 'AI-driven business automation and service orchestration dashboard'
})

// SEO
useHead({
  title: 'Service-as-a-Software Dashboard - TOSS ERP',
  meta: [
    { name: 'description', content: 'Monitor and control AI-driven business automation. Service-as-a-Software dashboard for autonomous business process management.' }
  ]
})
</script>

<style scoped>
/* Custom animations for service indicators */
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

/* Gradient backgrounds */
.bg-gradient-to-r {
  background: linear-gradient(to right, var(--tw-gradient-stops));
}

/* Service progress circles */
svg path {
  transition: stroke-dasharray 0.5s ease-in-out;
}

/* Hover effects */
.transition-colors {
  transition-property: color, background-color, border-color;
  transition-timing-function: cubic-bezier(0.4, 0, 0.2, 1);
  transition-duration: 200ms;
}
</style>
