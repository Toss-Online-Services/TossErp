<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <!-- Page Header -->
    <div class="bg-white dark:bg-gray-800 shadow">
      <div class="max-w-7xl mx-auto py-6 px-4 sm:px-6 lg:px-8">
        <div class="md:flex md:items-center md:justify-between">
          <div class="flex-1 min-w-0">
            <h2 class="text-2xl font-bold leading-7 text-gray-900 dark:text-white sm:text-3xl sm:truncate">
              Automation Workflows
            </h2>
            <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
              Create, manage, and monitor automated business processes and workflows
            </p>
          </div>
          <div class="mt-4 flex md:mt-0 md:ml-4 space-x-3">
            <button
              @click="showCreateModal = true"
              type="button"
              class="inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-orange-600 hover:bg-orange-700"
            >
              <svg class="-ml-1 mr-2 h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
              </svg>
              Create Workflow
            </button>
          </div>
        </div>
      </div>
    </div>

    <div class="max-w-7xl mx-auto py-6 px-4 sm:px-6 lg:px-8">
      <!-- Workflow Stats -->
      <div class="grid grid-cols-1 gap-5 sm:grid-cols-2 lg:grid-cols-4 mb-8">
        <div class="bg-white dark:bg-gray-800 overflow-hidden shadow rounded-lg">
          <div class="p-5">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <div class="w-8 h-8 bg-green-500 rounded-md flex items-center justify-center">
                  <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 10V3L4 14h7v7l9-11h-7z" />
                  </svg>
                </div>
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 dark:text-gray-400 truncate">Active Workflows</dt>
                  <dd class="text-lg font-medium text-gray-900 dark:text-white">{{ activeWorkflows }}</dd>
                </dl>
              </div>
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 overflow-hidden shadow rounded-lg">
          <div class="p-5">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <div class="w-8 h-8 bg-blue-500 rounded-md flex items-center justify-center">
                  <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
                  </svg>
                </div>
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 dark:text-gray-400 truncate">Tasks Automated Today</dt>
                  <dd class="text-lg font-medium text-gray-900 dark:text-white">{{ tasksAutomated }}</dd>
                </dl>
              </div>
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 overflow-hidden shadow rounded-lg">
          <div class="p-5">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <div class="w-8 h-8 bg-purple-500 rounded-md flex items-center justify-center">
                  <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4M7.835 4.697a3.42 3.42 0 001.946-.806 3.42 3.42 0 014.438 0 3.42 3.42 0 001.946.806 3.42 3.42 0 013.138 3.138 3.42 3.42 0 00.806 1.946 3.42 3.42 0 010 4.438 3.42 3.42 0 00-.806 1.946 3.42 3.42 0 01-3.138 3.138 3.42 3.42 0 00-1.946.806 3.42 3.42 0 01-4.438 0 3.42 3.42 0 00-1.946-.806 3.42 3.42 0 01-3.138-3.138 3.42 3.42 0 00-.806-1.946 3.42 3.42 0 010-4.438 3.42 3.42 0 00.806-1.946 3.42 3.42 0 013.138-3.138z" />
                  </svg>
                </div>
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 dark:text-gray-400 truncate">Success Rate</dt>
                  <dd class="text-lg font-medium text-gray-900 dark:text-white">{{ successRate }}%</dd>
                </dl>
              </div>
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 overflow-hidden shadow rounded-lg">
          <div class="p-5">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <div class="w-8 h-8 bg-yellow-500 rounded-md flex items-center justify-center">
                  <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                  </svg>
                </div>
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 dark:text-gray-400 truncate">Time Saved</dt>
                  <dd class="text-lg font-medium text-gray-900 dark:text-white">{{ timeSaved }}h</dd>
                </dl>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
        <!-- Workflows List -->
        <div class="lg:col-span-2 space-y-6">
          <!-- Quick Templates -->
          <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-4">Quick Start Templates</h3>
            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div
                v-for="template in workflowTemplates"
                :key="template.id"
                @click="createFromTemplate(template)"
                class="border-2 border-dashed border-gray-300 dark:border-gray-600 rounded-lg p-4 cursor-pointer hover:border-orange-500 dark:hover:border-orange-400 transition-colors"
              >
                <div class="flex items-center mb-2">
                  <div class="w-8 h-8 rounded-md flex items-center justify-center mr-3" :class="template.color">
                    <svg class="w-4 h-4 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" :d="template.icon" />
                    </svg>
                  </div>
                  <h4 class="text-sm font-medium text-gray-900 dark:text-white">{{ template.name }}</h4>
                </div>
                <p class="text-xs text-gray-500 dark:text-gray-400">{{ template.description }}</p>
              </div>
            </div>
          </div>

          <!-- Active Workflows -->
          <div class="bg-white dark:bg-gray-800 shadow rounded-lg">
            <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
              <h3 class="text-lg font-medium text-gray-900 dark:text-white">Active Workflows</h3>
            </div>
            <div class="divide-y divide-gray-200 dark:divide-gray-700">
              <div
                v-for="workflow in workflows"
                :key="workflow.id"
                class="p-6 hover:bg-gray-50 dark:hover:bg-gray-700"
              >
                <div class="flex items-center justify-between">
                  <div class="flex items-center">
                    <div class="w-10 h-10 rounded-md flex items-center justify-center mr-4" :class="workflow.color">
                      <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" :d="workflow.icon" />
                      </svg>
                    </div>
                    <div>
                      <h4 class="text-sm font-medium text-gray-900 dark:text-white">{{ workflow.name }}</h4>
                      <p class="text-sm text-gray-500 dark:text-gray-400">{{ workflow.description }}</p>
                      <div class="flex items-center mt-1 space-x-4">
                        <span class="text-xs text-gray-500 dark:text-gray-400">
                          Last run: {{ formatTime(workflow.lastRun) }}
                        </span>
                        <span class="text-xs text-gray-500 dark:text-gray-400">
                          {{ workflow.executions }} executions
                        </span>
                      </div>
                    </div>
                  </div>
                  <div class="flex items-center space-x-2">
                    <span
                      class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                      :class="workflow.status === 'active' 
                        ? 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-300' 
                        : 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300'"
                    >
                      {{ workflow.status }}
                    </span>
                    <div class="flex space-x-1">
                      <button @click="toggleWorkflow(workflow)" class="text-gray-400 hover:text-gray-600 dark:hover:text-gray-300">
                        <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M14.828 14.828a4 4 0 01-5.656 0M9 10h1m4 0h1m-6 4h1m4 0h1m2-7H5a2 2 0 00-2 2v8a2 2 0 002 2h14a2 2 0 002-2V9a2 2 0 00-2-2z" />
                        </svg>
                      </button>
                      <button @click="editWorkflow(workflow)" class="text-gray-400 hover:text-gray-600 dark:hover:text-gray-300">
                        <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
                        </svg>
                      </button>
                      <button @click="deleteWorkflow(workflow)" class="text-red-400 hover:text-red-600">
                        <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                        </svg>
                      </button>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Workflow Activity -->
        <div class="space-y-6">
          <!-- Recent Activity -->
          <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-4">Recent Activity</h3>
            <div class="space-y-3">
              <div v-for="activity in recentActivity" :key="activity.id" class="flex items-start space-x-3">
                <div class="flex-shrink-0">
                  <div class="w-2 h-2 mt-2 rounded-full" :class="getActivityColor(activity.type)"></div>
                </div>
                <div class="flex-1 min-w-0">
                  <p class="text-sm text-gray-900 dark:text-white">{{ activity.description }}</p>
                  <p class="text-xs text-gray-500 dark:text-gray-400">{{ formatTime(activity.timestamp) }}</p>
                </div>
              </div>
            </div>
          </div>

          <!-- Workflow Performance -->
          <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-4">Performance</h3>
            <div class="space-y-4">
              <div>
                <div class="flex justify-between text-sm">
                  <span class="text-gray-600 dark:text-gray-400">Inventory Management</span>
                  <span class="text-gray-900 dark:text-white">95%</span>
                </div>
                <div class="mt-1 w-full bg-gray-200 rounded-full h-2">
                  <div class="bg-green-600 h-2 rounded-full" style="width: 95%"></div>
                </div>
              </div>
              
              <div>
                <div class="flex justify-between text-sm">
                  <span class="text-gray-600 dark:text-gray-400">Sales Processing</span>
                  <span class="text-gray-900 dark:text-white">88%</span>
                </div>
                <div class="mt-1 w-full bg-gray-200 rounded-full h-2">
                  <div class="bg-blue-600 h-2 rounded-full" style="width: 88%"></div>
                </div>
              </div>
              
              <div>
                <div class="flex justify-between text-sm">
                  <span class="text-gray-600 dark:text-gray-400">Financial Reports</span>
                  <span class="text-gray-900 dark:text-white">92%</span>
                </div>
                <div class="mt-1 w-full bg-gray-200 rounded-full h-2">
                  <div class="bg-purple-600 h-2 rounded-full" style="width: 92%"></div>
                </div>
              </div>
            </div>
          </div>

          <!-- Quick Actions -->
          <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-4">Quick Actions</h3>
            <div class="space-y-2">
              <button
                @click="runAllWorkflows"
                class="w-full text-left px-3 py-2 text-sm bg-gray-50 dark:bg-gray-700 hover:bg-gray-100 dark:hover:bg-gray-600 rounded-md transition-colors"
              >
                Run All Active Workflows
              </button>
              <button
                @click="pauseAllWorkflows"
                class="w-full text-left px-3 py-2 text-sm bg-gray-50 dark:bg-gray-700 hover:bg-gray-100 dark:hover:bg-gray-600 rounded-md transition-colors"
              >
                Pause All Workflows
              </button>
              <button
                @click="exportWorkflows"
                class="w-full text-left px-3 py-2 text-sm bg-gray-50 dark:bg-gray-700 hover:bg-gray-100 dark:hover:bg-gray-600 rounded-md transition-colors"
              >
                Export Workflow Configs
              </button>
              <button
                @click="viewLogs"
                class="w-full text-left px-3 py-2 text-sm bg-gray-50 dark:bg-gray-700 hover:bg-gray-100 dark:hover:bg-gray-600 rounded-md transition-colors"
              >
                View Execution Logs
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Create Workflow Modal -->
    <div v-if="showCreateModal" class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
      <div class="relative top-20 mx-auto p-5 border w-96 shadow-lg rounded-md bg-white dark:bg-gray-800">
        <div class="mt-3">
          <div class="flex items-center justify-between mb-4">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white">Create New Workflow</h3>
            <button @click="showCreateModal = false" class="text-gray-400 hover:text-gray-600 dark:hover:text-gray-300">
              <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
              </svg>
            </button>
          </div>
          
          <form @submit.prevent="createWorkflow">
            <div class="mb-4">
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Workflow Name</label>
              <input
                v-model="newWorkflow.name"
                type="text"
                class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md focus:outline-none focus:ring-orange-500 focus:border-orange-500 dark:bg-gray-700 dark:text-white"
                placeholder="Enter workflow name"
                required
              />
            </div>
            
            <div class="mb-4">
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Description</label>
              <textarea
                v-model="newWorkflow.description"
                rows="3"
                class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md focus:outline-none focus:ring-orange-500 focus:border-orange-500 dark:bg-gray-700 dark:text-white"
                placeholder="Describe what this workflow does"
              ></textarea>
            </div>
            
            <div class="mb-6">
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Trigger</label>
              <select
                v-model="newWorkflow.trigger"
                class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md focus:outline-none focus:ring-orange-500 focus:border-orange-500 dark:bg-gray-700 dark:text-white"
                required
              >
                <option value="">Select a trigger</option>
                <option value="schedule">Scheduled (Daily/Weekly/Monthly)</option>
                <option value="event">Event-based (Stock Low, Sale Made, etc.)</option>
                <option value="manual">Manual Execution</option>
                <option value="webhook">Webhook/API Call</option>
              </select>
            </div>
            
            <div class="flex justify-end space-x-3">
              <button
                type="button"
                @click="showCreateModal = false"
                class="px-4 py-2 text-sm font-medium text-gray-700 dark:text-gray-300 bg-gray-100 dark:bg-gray-600 hover:bg-gray-200 dark:hover:bg-gray-500 rounded-md"
              >
                Cancel
              </button>
              <button
                type="submit"
                class="px-4 py-2 text-sm font-medium text-white bg-orange-600 hover:bg-orange-700 rounded-md"
              >
                Create Workflow
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'

// Mock Nuxt functions
const definePageMeta = (meta: any) => {
  // Meta data handling
}

const useHead = (head: any) => {
  // Head management
}

// Types
interface Workflow {
  id: string
  name: string
  description: string
  status: 'active' | 'paused' | 'error'
  lastRun: Date
  executions: number
  icon: string
  color: string
}

interface WorkflowTemplate {
  id: string
  name: string
  description: string
  icon: string
  color: string
  trigger: string
}

interface Activity {
  id: string
  description: string
  timestamp: Date
  type: 'success' | 'error' | 'warning' | 'info'
}

// Reactive data
const showCreateModal = ref(false)
const activeWorkflows = ref(8)
const tasksAutomated = ref(156)
const successRate = ref(94)
const timeSaved = ref(32)

const newWorkflow = ref({
  name: '',
  description: '',
  trigger: ''
})

// Workflow templates
const workflowTemplates = ref<WorkflowTemplate[]>([
  {
    id: '1',
    name: 'Stock Reorder',
    description: 'Automatically reorder items when stock is low',
    icon: 'M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4',
    color: 'bg-blue-500',
    trigger: 'event'
  },
  {
    id: '2',
    name: 'Daily Sales Report',
    description: 'Generate and email daily sales summary',
    icon: 'M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z',
    color: 'bg-green-500',
    trigger: 'schedule'
  },
  {
    id: '3',
    name: 'Customer Follow-up',
    description: 'Send follow-up messages to recent customers',
    icon: 'M8 12h.01M12 12h.01M16 12h.01M21 12c0 4.418-4.03 8-9 8a9.863 9.863 0 01-4.255-.949L3 20l1.395-3.72C3.512 15.042 3 13.574 3 12c0-4.418 4.03-8 9-8s9 3.582 9 8z',
    color: 'bg-purple-500',
    trigger: 'schedule'
  },
  {
    id: '4',
    name: 'Price Optimization',
    description: 'Adjust prices based on demand and competition',
    icon: 'M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z',
    color: 'bg-yellow-500',
    trigger: 'schedule'
  }
])

// Active workflows
const workflows = ref<Workflow[]>([
  {
    id: '1',
    name: 'Inventory Sync',
    description: 'Synchronize inventory levels across all channels',
    status: 'active',
    lastRun: new Date(Date.now() - 1800000), // 30 minutes ago
    executions: 247,
    icon: 'M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4',
    color: 'bg-blue-500'
  },
  {
    id: '2',
    name: 'Sales Notification',
    description: 'Send notifications for new sales and updates',
    status: 'active',
    lastRun: new Date(Date.now() - 600000), // 10 minutes ago
    executions: 89,
    icon: 'M15 17h5l-5 5-5-5h5v-5c0-1.1.9-2 2-2s2 .9 2 2v5z',
    color: 'bg-green-500'
  },
  {
    id: '3',
    name: 'Financial Reports',
    description: 'Generate weekly financial summaries and insights',
    status: 'active',
    lastRun: new Date(Date.now() - 3600000), // 1 hour ago
    executions: 34,
    icon: 'M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z',
    color: 'bg-purple-500'
  },
  {
    id: '4',
    name: 'Customer Outreach',
    description: 'Automated customer engagement campaigns',
    status: 'paused',
    lastRun: new Date(Date.now() - 7200000), // 2 hours ago
    executions: 156,
    icon: 'M8 12h.01M12 12h.01M16 12h.01M21 12c0 4.418-4.03 8-9 8a9.863 9.863 0 01-4.255-.949L3 20l1.395-3.72C3.512 15.042 3 13.574 3 12c0-4.418 4.03-8 9-8s9 3.582 9 8z',
    color: 'bg-orange-500'
  }
])

// Recent activity
const recentActivity = ref<Activity[]>([
  {
    id: '1',
    description: 'Inventory Sync workflow completed successfully',
    timestamp: new Date(Date.now() - 600000),
    type: 'success'
  },
  {
    id: '2',
    description: 'Sales Notification workflow executed',
    timestamp: new Date(Date.now() - 900000),
    type: 'success'
  },
  {
    id: '3',
    description: 'Customer Outreach workflow paused by user',
    timestamp: new Date(Date.now() - 1200000),
    type: 'warning'
  },
  {
    id: '4',
    description: 'Financial Reports workflow generated monthly summary',
    timestamp: new Date(Date.now() - 1800000),
    type: 'info'
  }
])

// Methods
const createFromTemplate = (template: WorkflowTemplate) => {
  newWorkflow.value = {
    name: template.name,
    description: template.description,
    trigger: template.trigger
  }
  showCreateModal.value = true
}

const createWorkflow = () => {
  // Implement workflow creation logic
  console.log('Creating workflow:', newWorkflow.value)
  
  // Add to workflows list
  const workflow: Workflow = {
    id: Date.now().toString(),
    name: newWorkflow.value.name,
    description: newWorkflow.value.description,
    status: 'active',
    lastRun: new Date(),
    executions: 0,
    icon: 'M13 10V3L4 14h7v7l9-11h-7z',
    color: 'bg-indigo-500'
  }
  
  workflows.value.unshift(workflow)
  activeWorkflows.value++
  
  // Reset form and close modal
  newWorkflow.value = { name: '', description: '', trigger: '' }
  showCreateModal.value = false
  
  // Add activity
  recentActivity.value.unshift({
    id: Date.now().toString(),
    description: `Created new workflow: ${workflow.name}`,
    timestamp: new Date(),
    type: 'info'
  })
}

const toggleWorkflow = (workflow: Workflow) => {
  workflow.status = workflow.status === 'active' ? 'paused' : 'active'
  
  recentActivity.value.unshift({
    id: Date.now().toString(),
    description: `${workflow.status === 'active' ? 'Activated' : 'Paused'} workflow: ${workflow.name}`,
    timestamp: new Date(),
    type: 'info'
  })
}

const editWorkflow = (workflow: Workflow) => {
  console.log('Editing workflow:', workflow)
  // Implement edit functionality
}

const deleteWorkflow = (workflow: Workflow) => {
  if (confirm(`Are you sure you want to delete "${workflow.name}"?`)) {
    const index = workflows.value.findIndex((w: Workflow) => w.id === workflow.id)
    if (index > -1) {
      workflows.value.splice(index, 1)
      activeWorkflows.value--
      
      recentActivity.value.unshift({
        id: Date.now().toString(),
        description: `Deleted workflow: ${workflow.name}`,
        timestamp: new Date(),
        type: 'warning'
      })
    }
  }
}

const runAllWorkflows = () => {
  console.log('Running all active workflows')
  recentActivity.value.unshift({
    id: Date.now().toString(),
    description: 'Executed all active workflows manually',
    timestamp: new Date(),
    type: 'success'
  })
}

const pauseAllWorkflows = () => {
  workflows.value.forEach((workflow: Workflow) => {
    if (workflow.status === 'active') {
      workflow.status = 'paused'
    }
  })
  
  recentActivity.value.unshift({
    id: Date.now().toString(),
    description: 'Paused all active workflows',
    timestamp: new Date(),
    type: 'warning'
  })
}

const exportWorkflows = () => {
  const dataStr = JSON.stringify(workflows.value, null, 2)
  const dataBlob = new Blob([dataStr], { type: 'application/json' })
  const url = URL.createObjectURL(dataBlob)
  const link = document.createElement('a')
  link.href = url
  link.download = 'workflows.json'
  link.click()
  URL.revokeObjectURL(url)
}

const viewLogs = () => {
  console.log('Opening workflow execution logs')
  // Implement logs view
}

const getActivityColor = (type: string): string => {
  switch (type) {
    case 'success': return 'bg-green-400'
    case 'error': return 'bg-red-400'
    case 'warning': return 'bg-yellow-400'
    case 'info': return 'bg-blue-400'
    default: return 'bg-gray-400'
  }
}

const formatTime = (date: Date): string => {
  return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
}

// Page meta
definePageMeta({
  title: 'Automation Workflows',
  description: 'Create, manage, and monitor automated business processes and workflows'
})

// SEO
useHead({
  title: 'Automation Workflows - TOSS ERP',
  meta: [
    { name: 'description', content: 'Create and manage automated business workflows. Streamline your operations with intelligent automation and monitoring.' }
  ]
})
</script>

<style scoped>
/* Component-specific styles */
</style>
