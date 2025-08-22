<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <!-- Header -->
    <div class="bg-white dark:bg-gray-800 shadow">
      <div class="max-w-7xl mx-auto py-6 px-4 sm:px-6 lg:px-8">
        <div class="md:flex md:items-center md:justify-between">
          <div class="flex-1 min-w-0">
            <h2 class="text-2xl font-bold leading-7 text-gray-900 dark:text-white sm:text-3xl sm:truncate">
              Service Outcomes & Metrics
            </h2>
            <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
              Measure business outcomes delivered by AI-powered services
            </p>
          </div>
          <div class="mt-4 flex md:mt-0 md:ml-4 space-x-3">
            <select v-model="selectedTimeframe" class="block w-full pl-3 pr-10 py-2 text-base border border-gray-300 dark:border-gray-600 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm rounded-md bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
              <option value="24h">Last 24 Hours</option>
              <option value="7d">Last 7 Days</option>
              <option value="30d">Last 30 Days</option>
              <option value="90d">Last 90 Days</option>
            </select>
            <button
              @click="exportReport"
              type="button"
              class="inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-indigo-600 hover:bg-indigo-700"
            >
              <svg class="-ml-1 mr-2 h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 10v6m0 0l-3-3m3 3l3-3m2 8H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
              </svg>
              Export Report
            </button>
          </div>
        </div>
      </div>
    </div>

    <div class="max-w-7xl mx-auto py-6 px-4 sm:px-6 lg:px-8">
      <!-- Key Outcomes Summary -->
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
        <div v-for="outcome in keyOutcomes" :key="outcome.id" class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
          <div class="flex items-center">
            <div class="w-12 h-12 rounded-lg flex items-center justify-center mr-4" :class="outcome.iconBg">
              <component :is="outcome.icon" class="w-6 h-6 text-white" />
            </div>
            <div class="flex-1">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">{{ outcome.label }}</p>
              <div class="flex items-center">
                <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ outcome.value }}</p>
                <span class="ml-2 flex items-center text-sm" :class="outcome.trend > 0 ? 'text-green-600 dark:text-green-400' : 'text-red-600 dark:text-red-400'">
                  <svg v-if="outcome.trend > 0" class="w-4 h-4 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 17l9.2-9.2M17 17V7M17 17H7" />
                  </svg>
                  <svg v-else class="w-4 h-4 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 7l-9.2 9.2M7 7v10M7 7h10" />
                  </svg>
                  {{ Math.abs(outcome.trend) }}%
                </span>
              </div>
              <p class="text-xs text-gray-500 dark:text-gray-400 mt-1">{{ outcome.description }}</p>
            </div>
          </div>
        </div>
      </div>

      <div class="grid grid-cols-1 xl:grid-cols-3 gap-6">
        
        <!-- Business Impact Metrics -->
        <div class="xl:col-span-2 space-y-6">
          
          <!-- Revenue Impact -->
          <div class="bg-white dark:bg-gray-800 rounded-lg shadow">
            <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
              <div class="flex items-center justify-between">
                <h3 class="text-lg font-medium text-gray-900 dark:text-white">Revenue Impact</h3>
                <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-300">
                  +{{ revenueGrowth }}% vs {{ selectedTimeframe }}
                </span>
              </div>
            </div>
            <div class="p-6">
              <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-6">
                <div class="text-center">
                  <p class="text-3xl font-bold text-gray-900 dark:text-white">R{{ totalRevenue.toLocaleString() }}</p>
                  <p class="text-sm text-gray-500 dark:text-gray-400">Total Revenue Generated</p>
                </div>
                <div class="text-center">
                  <p class="text-3xl font-bold text-green-600 dark:text-green-400">R{{ revenueFromAI.toLocaleString() }}</p>
                  <p class="text-sm text-gray-500 dark:text-gray-400">AI-Driven Revenue</p>
                </div>
                <div class="text-center">
                  <p class="text-3xl font-bold text-blue-600 dark:text-blue-400">{{ ((revenueFromAI / totalRevenue) * 100).toFixed(1) }}%</p>
                  <p class="text-sm text-gray-500 dark:text-gray-400">AI Contribution</p>
                </div>
              </div>

              <!-- Revenue Sources -->
              <div class="space-y-4">
                <h4 class="text-sm font-medium text-gray-900 dark:text-white">Revenue Sources</h4>
                <div v-for="source in revenueSources" :key="source.id" class="flex items-center justify-between p-3 bg-gray-50 dark:bg-gray-700 rounded-lg">
                  <div class="flex items-center">
                    <div class="w-8 h-8 rounded-lg flex items-center justify-center mr-3" :class="source.iconBg">
                      <component :is="source.icon" class="w-4 h-4 text-white" />
                    </div>
                    <div>
                      <h5 class="text-sm font-medium text-gray-900 dark:text-white">{{ source.name }}</h5>
                      <p class="text-xs text-gray-500 dark:text-gray-400">{{ source.description }}</p>
                    </div>
                  </div>
                  <div class="text-right">
                    <p class="text-sm font-medium text-gray-900 dark:text-white">R{{ source.amount.toLocaleString() }}</p>
                    <div class="flex items-center text-xs" :class="source.growth > 0 ? 'text-green-600 dark:text-green-400' : 'text-red-600 dark:text-red-400'">
                      <svg v-if="source.growth > 0" class="w-3 h-3 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 17l9.2-9.2M17 17V7M17 17H7" />
                      </svg>
                      <svg v-else class="w-3 h-3 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 7l-9.2 9.2M7 7v10M7 7h10" />
                      </svg>
                      {{ Math.abs(source.growth) }}%
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Operational Efficiency -->
          <div class="bg-white dark:bg-gray-800 rounded-lg shadow">
            <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
              <h3 class="text-lg font-medium text-gray-900 dark:text-white">Operational Efficiency</h3>
            </div>
            <div class="p-6">
              <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
                <div v-for="metric in efficiencyMetrics" :key="metric.id" class="border border-gray-200 dark:border-gray-700 rounded-lg p-4">
                  <div class="flex items-center justify-between mb-3">
                    <div class="flex items-center">
                      <div class="w-8 h-8 rounded-lg flex items-center justify-center mr-3" :class="metric.iconBg">
                        <component :is="metric.icon" class="w-4 h-4 text-white" />
                      </div>
                      <h4 class="text-sm font-medium text-gray-900 dark:text-white">{{ metric.name }}</h4>
                    </div>
                    <span class="text-lg font-bold text-gray-900 dark:text-white">{{ metric.value }}{{ metric.unit }}</span>
                  </div>
                  
                  <p class="text-xs text-gray-500 dark:text-gray-400 mb-3">{{ metric.description }}</p>
                  
                  <!-- Progress Bar -->
                  <div class="mb-2">
                    <div class="flex justify-between text-xs text-gray-600 dark:text-gray-300 mb-1">
                      <span>Target: {{ metric.target }}{{ metric.unit }}</span>
                      <span>{{ Math.round((metric.value / metric.target) * 100) }}%</span>
                    </div>
                    <div class="w-full bg-gray-200 dark:bg-gray-700 rounded-full h-2">
                      <div class="h-2 rounded-full transition-all duration-300" :class="getProgressColor(metric.value, metric.target)" :style="{ width: `${Math.min(100, (metric.value / metric.target) * 100)}%` }"></div>
                    </div>
                  </div>
                  
                  <!-- Improvement -->
                  <div class="flex items-center text-xs" :class="metric.improvement > 0 ? 'text-green-600 dark:text-green-400' : 'text-red-600 dark:text-red-400'">
                    <svg v-if="metric.improvement > 0" class="w-3 h-3 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 17l9.2-9.2M17 17V7M17 17H7" />
                    </svg>
                    <svg v-else class="w-3 h-3 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 7l-9.2 9.2M7 7v10M7 7h10" />
                    </svg>
                    {{ Math.abs(metric.improvement) }}% improvement vs last period
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Service Performance -->
          <div class="bg-white dark:bg-gray-800 rounded-lg shadow">
            <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
              <h3 class="text-lg font-medium text-gray-900 dark:text-white">Service Performance</h3>
            </div>
            <div class="p-6">
              <div class="space-y-4">
                <div v-for="service in servicePerformance" :key="service.id" class="border border-gray-200 dark:border-gray-700 rounded-lg p-4">
                  <div class="flex items-center justify-between mb-3">
                    <div class="flex items-center">
                      <div class="w-8 h-8 rounded-lg flex items-center justify-center mr-3" :class="service.statusColor">
                        <component :is="service.icon" class="w-4 h-4 text-white" />
                      </div>
                      <div>
                        <h4 class="text-sm font-medium text-gray-900 dark:text-white">{{ service.name }}</h4>
                        <p class="text-xs text-gray-500 dark:text-gray-400">{{ service.description }}</p>
                      </div>
                    </div>
                    <div class="text-right">
                      <span class="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium" :class="getStatusBadge(service.status)">
                        {{ service.status }}
                      </span>
                      <p class="text-xs text-gray-500 dark:text-gray-400 mt-1">{{ service.sla }}% SLA</p>
                    </div>
                  </div>
                  
                  <!-- Service Metrics -->
                  <div class="grid grid-cols-4 gap-4 text-center">
                    <div>
                      <p class="text-lg font-bold text-gray-900 dark:text-white">{{ service.requests }}</p>
                      <p class="text-xs text-gray-500 dark:text-gray-400">Requests</p>
                    </div>
                    <div>
                      <p class="text-lg font-bold text-gray-900 dark:text-white">{{ service.successRate }}%</p>
                      <p class="text-xs text-gray-500 dark:text-gray-400">Success Rate</p>
                    </div>
                    <div>
                      <p class="text-lg font-bold text-gray-900 dark:text-white">{{ service.avgResponseTime }}ms</p>
                      <p class="text-xs text-gray-500 dark:text-gray-400">Avg Response</p>
                    </div>
                    <div>
                      <p class="text-lg font-bold text-gray-900 dark:text-white">{{ service.cost }}</p>
                      <p class="text-xs text-gray-500 dark:text-gray-400">Cost per Request</p>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Outcome Analytics -->
        <div class="space-y-6">
          
          <!-- ROI Calculator -->
          <div class="bg-white dark:bg-gray-800 rounded-lg shadow">
            <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
              <h3 class="text-lg font-medium text-gray-900 dark:text-white">ROI Analysis</h3>
            </div>
            <div class="p-6">
              <div class="text-center mb-6">
                <p class="text-4xl font-bold text-green-600 dark:text-green-400">{{ roi }}%</p>
                <p class="text-sm text-gray-500 dark:text-gray-400">Return on Investment</p>
              </div>
              
              <div class="space-y-4">
                <div class="flex justify-between">
                  <span class="text-sm text-gray-600 dark:text-gray-400">Total Investment</span>
                  <span class="text-sm font-medium text-gray-900 dark:text-white">R{{ totalInvestment.toLocaleString() }}</span>
                </div>
                <div class="flex justify-between">
                  <span class="text-sm text-gray-600 dark:text-gray-400">Total Returns</span>
                  <span class="text-sm font-medium text-gray-900 dark:text-white">R{{ totalReturns.toLocaleString() }}</span>
                </div>
                <div class="flex justify-between">
                  <span class="text-sm text-gray-600 dark:text-gray-400">Net Profit</span>
                  <span class="text-sm font-medium text-green-600 dark:text-green-400">R{{ (totalReturns - totalInvestment).toLocaleString() }}</span>
                </div>
                <div class="border-t border-gray-200 dark:border-gray-700 pt-3">
                  <div class="flex justify-between">
                    <span class="text-sm font-medium text-gray-900 dark:text-white">Payback Period</span>
                    <span class="text-sm font-medium text-gray-900 dark:text-white">{{ paybackPeriod }} months</span>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Cost Savings -->
          <div class="bg-white dark:bg-gray-800 rounded-lg shadow">
            <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
              <h3 class="text-lg font-medium text-gray-900 dark:text-white">Cost Savings</h3>
            </div>
            <div class="p-6 space-y-4">
              <div v-for="saving in costSavings" :key="saving.id" class="flex items-center justify-between p-3 bg-green-50 dark:bg-green-900/20 border border-green-200 dark:border-green-800 rounded-lg">
                <div>
                  <h4 class="text-sm font-medium text-gray-900 dark:text-white">{{ saving.category }}</h4>
                  <p class="text-xs text-gray-500 dark:text-gray-400">{{ saving.description }}</p>
                </div>
                <div class="text-right">
                  <p class="text-sm font-bold text-green-600 dark:text-green-400">R{{ saving.amount.toLocaleString() }}</p>
                  <p class="text-xs text-gray-500 dark:text-gray-400">{{ saving.percentage }}% saved</p>
                </div>
              </div>
              
              <div class="border-t border-gray-200 dark:border-gray-700 pt-4">
                <div class="flex justify-between">
                  <span class="text-base font-medium text-gray-900 dark:text-white">Total Savings</span>
                  <span class="text-base font-bold text-green-600 dark:text-green-400">R{{ totalSavings.toLocaleString() }}</span>
                </div>
              </div>
            </div>
          </div>

          <!-- Customer Impact -->
          <div class="bg-white dark:bg-gray-800 rounded-lg shadow">
            <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
              <h3 class="text-lg font-medium text-gray-900 dark:text-white">Customer Impact</h3>
            </div>
            <div class="p-6 space-y-4">
              <div v-for="impact in customerImpact" :key="impact.id" class="text-center">
                <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ impact.value }}</p>
                <p class="text-sm text-gray-600 dark:text-gray-400">{{ impact.label }}</p>
                <div class="mt-2">
                  <div class="w-full bg-gray-200 dark:bg-gray-700 rounded-full h-2">
                    <div class="h-2 rounded-full transition-all duration-300" :class="impact.color" :style="{ width: `${impact.percentage}%` }"></div>
                  </div>
                  <p class="text-xs text-gray-500 dark:text-gray-400 mt-1">{{ impact.percentage }}% improvement</p>
                </div>
              </div>
            </div>
          </div>

          <!-- Predictive Insights -->
          <div class="bg-white dark:bg-gray-800 rounded-lg shadow">
            <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
              <h3 class="text-lg font-medium text-gray-900 dark:text-white">Predictive Insights</h3>
            </div>
            <div class="p-6 space-y-4">
              <div v-for="insight in predictiveInsights" :key="insight.id" class="p-3 border border-gray-200 dark:border-gray-700 rounded-lg">
                <div class="flex items-start">
                  <div class="w-8 h-8 rounded-lg flex items-center justify-center mr-3" :class="insight.iconBg">
                    <component :is="insight.icon" class="w-4 h-4 text-white" />
                  </div>
                  <div class="flex-1">
                    <h4 class="text-sm font-medium text-gray-900 dark:text-white">{{ insight.title }}</h4>
                    <p class="text-xs text-gray-500 dark:text-gray-400 mt-1">{{ insight.description }}</p>
                    <div class="mt-2 flex items-center">
                      <span class="text-xs font-medium px-2 py-1 rounded" :class="getConfidenceBadge(insight.confidence)">
                        {{ insight.confidence }}% confidence
                      </span>
                    </div>
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
import { ref, computed } from 'vue'

// Mock Nuxt functions
function definePageMeta(meta: any) {}
function useHead(options: any) {
  if (typeof document !== 'undefined') {
    document.title = options.title || 'Service Outcomes & Metrics - TOSS ERP'
  }
}

// Reactive data
const selectedTimeframe = ref('30d')

// Key outcomes
const keyOutcomes = ref([
  {
    id: 'outcome-1',
    label: 'Revenue Growth',
    value: 'R2.4M',
    trend: 15.3,
    description: 'AI-driven revenue increase',
    icon: 'chart-icon',
    iconBg: 'bg-green-500'
  },
  {
    id: 'outcome-2',
    label: 'Cost Reduction',
    value: 'R890K',
    trend: 23.7,
    description: 'Operational cost savings',
    icon: 'money-icon',
    iconBg: 'bg-blue-500'
  },
  {
    id: 'outcome-3',
    label: 'Time Saved',
    value: '1,247h',
    trend: 31.2,
    description: 'Staff hours automated',
    icon: 'clock-icon',
    iconBg: 'bg-purple-500'
  },
  {
    id: 'outcome-4',
    label: 'Error Reduction',
    value: '94%',
    trend: 12.8,
    description: 'Fewer processing errors',
    icon: 'check-icon',
    iconBg: 'bg-yellow-500'
  }
])

// Revenue metrics
const totalRevenue = ref(3240000)
const revenueFromAI = ref(2400000)
const revenueGrowth = ref(18.5)

const revenueSources = ref([
  {
    id: 'source-1',
    name: 'Automated Sales Processing',
    description: 'AI-generated invoices and order processing',
    amount: 890000,
    growth: 22.4,
    icon: 'receipt-icon',
    iconBg: 'bg-green-500'
  },
  {
    id: 'source-2',
    name: 'Predictive Inventory Management',
    description: 'Optimized stock levels and reduced waste',
    amount: 645000,
    growth: 18.7,
    icon: 'box-icon',
    iconBg: 'bg-blue-500'
  },
  {
    id: 'source-3',
    name: 'Customer Service Automation',
    description: 'Improved customer satisfaction and retention',
    amount: 432000,
    growth: 15.2,
    icon: 'heart-icon',
    iconBg: 'bg-purple-500'
  },
  {
    id: 'source-4',
    name: 'Financial Process Optimization',
    description: 'Automated reconciliation and reporting',
    amount: 433000,
    growth: 12.8,
    icon: 'calculator-icon',
    iconBg: 'bg-yellow-500'
  }
])

// Efficiency metrics
const efficiencyMetrics = ref([
  {
    id: 'efficiency-1',
    name: 'Processing Speed',
    value: 247,
    unit: '%',
    target: 200,
    improvement: 47.3,
    description: 'Faster than manual processing',
    icon: 'lightning-icon',
    iconBg: 'bg-blue-500'
  },
  {
    id: 'efficiency-2',
    name: 'Accuracy Rate',
    value: 99.7,
    unit: '%',
    target: 95,
    improvement: 4.9,
    description: 'Data processing accuracy',
    icon: 'target-icon',
    iconBg: 'bg-green-500'
  },
  {
    id: 'efficiency-3',
    name: 'Resource Utilization',
    value: 87,
    unit: '%',
    target: 80,
    improvement: 15.8,
    description: 'Optimal resource allocation',
    icon: 'cpu-icon',
    iconBg: 'bg-purple-500'
  },
  {
    id: 'efficiency-4',
    name: 'Customer Response',
    value: 12,
    unit: 'min',
    target: 30,
    improvement: 60.0,
    description: 'Average response time',
    icon: 'reply-icon',
    iconBg: 'bg-yellow-500'
  }
])

// Service performance
const servicePerformance = ref([
  {
    id: 'service-1',
    name: 'Invoice Generation Service',
    description: 'Automated invoice creation and delivery',
    status: 'Operational',
    sla: 99.8,
    requests: '12.4K',
    successRate: 99.8,
    avgResponseTime: 145,
    cost: 'R0.23',
    statusColor: 'bg-green-500',
    icon: 'document-icon'
  },
  {
    id: 'service-2',
    name: 'Inventory Optimization Service',
    description: 'AI-powered stock level optimization',
    status: 'Operational',
    sla: 99.5,
    requests: '8.7K',
    successRate: 98.9,
    avgResponseTime: 230,
    cost: 'R0.45',
    statusColor: 'bg-green-500',
    icon: 'box-icon'
  },
  {
    id: 'service-3',
    name: 'Customer Support Bot',
    description: 'Automated customer inquiry handling',
    status: 'Degraded',
    sla: 97.2,
    requests: '15.6K',
    successRate: 94.7,
    avgResponseTime: 890,
    cost: 'R0.12',
    statusColor: 'bg-yellow-500',
    icon: 'chat-icon'
  }
])

// ROI calculations
const totalInvestment = ref(450000)
const totalReturns = ref(1890000)
const roi = computed(() => Math.round(((totalReturns.value - totalInvestment.value) / totalInvestment.value) * 100))
const paybackPeriod = ref(3.2)

// Cost savings
const costSavings = ref([
  {
    id: 'saving-1',
    category: 'Staff Costs',
    description: 'Reduced manual processing',
    amount: 340000,
    percentage: 45
  },
  {
    id: 'saving-2',
    category: 'Error Correction',
    description: 'Fewer processing mistakes',
    amount: 125000,
    percentage: 78
  },
  {
    id: 'saving-3',
    category: 'Infrastructure',
    description: 'Optimized resource usage',
    amount: 89000,
    percentage: 23
  },
  {
    id: 'saving-4',
    category: 'Compliance',
    description: 'Automated reporting',
    amount: 67000,
    percentage: 56
  }
])

const totalSavings = computed(() => costSavings.value.reduce((sum, saving) => sum + saving.amount, 0))

// Customer impact
const customerImpact = ref([
  {
    id: 'impact-1',
    label: 'Customer Satisfaction',
    value: '4.8/5',
    percentage: 96,
    color: 'bg-green-500'
  },
  {
    id: 'impact-2',
    label: 'Response Time',
    value: '12 min',
    percentage: 85,
    color: 'bg-blue-500'
  },
  {
    id: 'impact-3',
    label: 'Resolution Rate',
    value: '94.7%',
    percentage: 95,
    color: 'bg-purple-500'
  }
])

// Predictive insights
const predictiveInsights = ref([
  {
    id: 'insight-1',
    title: 'Revenue Projection',
    description: 'Expected 25% revenue increase next quarter based on current AI performance trends',
    confidence: 87,
    icon: 'trend-icon',
    iconBg: 'bg-green-500'
  },
  {
    id: 'insight-2',
    title: 'Cost Optimization',
    description: 'Additional R180K savings possible with expanded automation in accounts payable',
    confidence: 92,
    icon: 'savings-icon',
    iconBg: 'bg-blue-500'
  },
  {
    id: 'insight-3',
    title: 'Service Scaling',
    description: 'Customer support bot efficiency will improve 15% with additional training data',
    confidence: 78,
    icon: 'scale-icon',
    iconBg: 'bg-purple-500'
  }
])

// Methods
const getProgressColor = (value: number, target: number): string => {
  const percentage = (value / target) * 100
  if (percentage >= 100) return 'bg-green-500'
  if (percentage >= 80) return 'bg-yellow-500'
  return 'bg-red-500'
}

const getStatusBadge = (status: string): string => {
  switch (status.toLowerCase()) {
    case 'operational':
      return 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-300'
    case 'degraded':
      return 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-300'
    case 'down':
      return 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-300'
    default:
      return 'bg-gray-100 text-gray-800 dark:bg-gray-900 dark:text-gray-300'
  }
}

const getConfidenceBadge = (confidence: number): string => {
  if (confidence >= 90) return 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-300'
  if (confidence >= 70) return 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-300'
  return 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-300'
}

const exportReport = () => {
  console.log('Exporting service outcomes report...')
  // Implementation would generate and download report
}

// Page meta
definePageMeta({
  title: 'Service Outcomes & Metrics',
  description: 'Track and measure business outcomes delivered by AI-powered services'
})

// SEO
useHead({
  title: 'Service Outcomes & Metrics - TOSS ERP',
  meta: [
    { name: 'description', content: 'Comprehensive metrics and analytics for measuring the business impact and ROI of Service-as-a-Software implementations.' }
  ]
})
</script>

<style scoped>
/* Progress bar animations */
.transition-all {
  transition-property: all;
  transition-timing-function: cubic-bezier(0.4, 0, 0.2, 1);
  transition-duration: 300ms;
}

/* Custom gradient backgrounds */
.bg-gradient-to-r {
  background-image: linear-gradient(to right, var(--tw-gradient-stops));
}

/* Number animation */
@keyframes countUp {
  from {
    opacity: 0;
    transform: translateY(10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.animate-countUp {
  animation: countUp 0.5s ease-out;
}

/* Hover effects */
.hover-lift {
  transition: transform 0.2s ease-in-out;
}

.hover-lift:hover {
  transform: translateY(-2px);
}

/* Success indicators */
.success-indicator {
  position: relative;
}

.success-indicator::after {
  content: '';
  position: absolute;
  top: 0;
  right: 0;
  width: 8px;
  height: 8px;
  background-color: #10b981;
  border-radius: 50%;
  animation: pulse 2s infinite;
}

@keyframes pulse {
  0%, 100% {
    opacity: 1;
  }
  50% {
    opacity: 0.5;
  }
}
</style>
