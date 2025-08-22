<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <!-- Header -->
    <div class="bg-white dark:bg-gray-800 shadow">
      <div class="max-w-7xl mx-auto py-6 px-4 sm:px-6 lg:px-8">
        <div class="md:flex md:items-center md:justify-between">
          <div class="flex-1 min-w-0">
            <h2 class="text-2xl font-bold leading-7 text-gray-900 dark:text-white sm:text-3xl sm:truncate">
              AI Model Configuration
            </h2>
            <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
              Configure and fine-tune AI models for optimal business performance
            </p>
          </div>
          <div class="mt-4 flex md:mt-0 md:ml-4 space-x-3">
            <button
              @click="saveAllConfigurations"
              type="button"
              class="inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-indigo-600 hover:bg-indigo-700"
            >
              <svg class="-ml-1 mr-2 h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7H5a2 2 0 00-2 2v9a2 2 0 002 2h14a2 2 0 002-2V9a2 2 0 00-2-2h-3m-1 4l-3 3m0 0l-3-3m3 3V4" />
              </svg>
              Save All Changes
            </button>
            <button
              @click="deployModels"
              type="button"
              class="inline-flex items-center px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm text-sm font-medium text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600"
            >
              <svg class="-ml-1 mr-2 h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 16a4 4 0 01-.88-7.903A5 5 0 1115.9 6L16 6a5 5 0 011 9.9M9 19l3 3m0 0l3-3m-3 3V10" />
              </svg>
              Deploy Models
            </button>
          </div>
        </div>
      </div>
    </div>

    <div class="max-w-7xl mx-auto py-6 px-4 sm:px-6 lg:px-8">
      <div class="grid grid-cols-1 xl:grid-cols-4 gap-6">
        
        <!-- Model Categories -->
        <div class="xl:col-span-1">
          <div class="bg-white dark:bg-gray-800 rounded-lg shadow">
            <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
              <h3 class="text-lg font-medium text-gray-900 dark:text-white">Model Categories</h3>
            </div>
            <div class="p-6">
              <nav class="space-y-1">
                <button
                  v-for="category in modelCategories"
                  :key="category.id"
                  @click="selectedCategory = category.id"
                  :class="selectedCategory === category.id ? 'bg-indigo-50 dark:bg-indigo-900 border-indigo-500 text-indigo-700 dark:text-indigo-300' : 'border-transparent text-gray-600 dark:text-gray-400 hover:bg-gray-50 dark:hover:bg-gray-700'"
                  class="w-full flex items-center px-3 py-2 text-sm font-medium rounded-md border-l-4 transition-colors"
                >
                  <component :is="category.icon" class="mr-3 h-5 w-5" />
                  {{ category.name }}
                  <span class="ml-auto text-xs bg-gray-100 dark:bg-gray-700 text-gray-600 dark:text-gray-400 px-2 py-1 rounded-full">
                    {{ category.modelCount }}
                  </span>
                </button>
              </nav>
            </div>
          </div>

          <!-- Model Performance Summary -->
          <div class="mt-6 bg-white dark:bg-gray-800 rounded-lg shadow">
            <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
              <h3 class="text-lg font-medium text-gray-900 dark:text-white">Performance Summary</h3>
            </div>
            <div class="p-6 space-y-4">
              <div v-for="metric in performanceSummary" :key="metric.id" class="flex items-center justify-between">
                <div>
                  <p class="text-sm font-medium text-gray-900 dark:text-white">{{ metric.name }}</p>
                  <p class="text-xs text-gray-500 dark:text-gray-400">{{ metric.description }}</p>
                </div>
                <div class="text-right">
                  <p class="text-sm font-bold text-gray-900 dark:text-white">{{ metric.value }}{{ metric.unit }}</p>
                  <div class="w-16 h-1 bg-gray-200 dark:bg-gray-700 rounded-full mt-1">
                    <div class="h-1 rounded-full transition-all duration-300" :class="getPerformanceColor(metric.score)" :style="{ width: `${metric.score}%` }"></div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Model Configuration Panel -->
        <div class="xl:col-span-3">
          <div class="space-y-6">
            
            <!-- Model Grid -->
            <div class="grid grid-cols-1 lg:grid-cols-2 gap-6" v-if="filteredModels.length > 0">
              <div v-for="model in filteredModels" :key="model.id" class="bg-white dark:bg-gray-800 rounded-lg shadow">
                <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
                  <div class="flex items-center justify-between">
                    <div class="flex items-center">
                      <div class="w-10 h-10 rounded-lg flex items-center justify-center mr-3" :class="model.statusColor">
                        <component :is="model.icon" class="w-5 h-5 text-white" />
                      </div>
                      <div>
                        <h3 class="text-lg font-medium text-gray-900 dark:text-white">{{ model.name }}</h3>
                        <p class="text-sm text-gray-500 dark:text-gray-400">{{ model.description }}</p>
                      </div>
                    </div>
                    <div class="flex items-center space-x-2">
                      <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium" :class="getStatusBadge(model.status)">
                        {{ model.status }}
                      </span>
                      <button @click="toggleModel(model)" class="text-gray-400 hover:text-gray-600 dark:hover:text-gray-300">
                        <svg v-if="model.enabled" class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 9v6m4-6v6m7-3a9 9 0 11-18 0 9 9 0 0118 0z" />
                        </svg>
                        <svg v-else class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M14.828 14.828a4 4 0 01-5.656 0M9 10h1m4 0h1M9 16h1m4 0h1M12 3v1m0 16v1m9-9h-1M3 12H2m1.414-5.828l.707.707m14.142 0l.707-.707m-14.142 14.142l.707-.707m14.142 0l.707.707" />
                        </svg>
                      </button>
                    </div>
                  </div>
                </div>
                
                <div class="p-6 space-y-4">
                  <!-- Model Metrics -->
                  <div class="grid grid-cols-3 gap-4 text-center">
                    <div>
                      <p class="text-lg font-bold text-gray-900 dark:text-white">{{ model.accuracy }}%</p>
                      <p class="text-xs text-gray-500 dark:text-gray-400">Accuracy</p>
                    </div>
                    <div>
                      <p class="text-lg font-bold text-gray-900 dark:text-white">{{ model.latency }}ms</p>
                      <p class="text-xs text-gray-500 dark:text-gray-400">Latency</p>
                    </div>
                    <div>
                      <p class="text-lg font-bold text-gray-900 dark:text-white">{{ model.throughput }}</p>
                      <p class="text-xs text-gray-500 dark:text-gray-400">Req/min</p>
                    </div>
                  </div>

                  <!-- Configuration Parameters -->
                  <div class="space-y-3">
                    <h4 class="text-sm font-medium text-gray-900 dark:text-white">Configuration Parameters</h4>
                    
                    <div v-for="param in model.parameters" :key="param.name" class="space-y-1">
                      <div class="flex justify-between">
                        <label class="text-xs font-medium text-gray-700 dark:text-gray-300">{{ param.label }}</label>
                        <span class="text-xs text-gray-500 dark:text-gray-400">{{ param.value }}{{ param.unit }}</span>
                      </div>
                      
                      <div v-if="param.type === 'slider'" class="flex items-center space-x-3">
                        <span class="text-xs text-gray-500 dark:text-gray-400">{{ param.min }}</span>
                        <input
                          type="range"
                          :min="param.min"
                          :max="param.max"
                          :step="param.step"
                          v-model="param.value"
                          class="flex-1 h-2 bg-gray-200 dark:bg-gray-700 rounded-lg appearance-none cursor-pointer"
                        />
                        <span class="text-xs text-gray-500 dark:text-gray-400">{{ param.max }}</span>
                      </div>
                      
                      <select v-else-if="param.type === 'select'" v-model="param.value" class="w-full text-xs border border-gray-300 dark:border-gray-600 rounded-md bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
                        <option v-for="option in param.options" :key="option" :value="option">{{ option }}</option>
                      </select>
                      
                      <input v-else-if="param.type === 'number'" type="number" v-model="param.value" :min="param.min" :max="param.max" class="w-full text-xs border border-gray-300 dark:border-gray-600 rounded-md bg-white dark:bg-gray-700 text-gray-900 dark:text-white px-2 py-1" />
                    </div>
                  </div>

                  <!-- Training Data -->
                  <div class="border-t border-gray-200 dark:border-gray-700 pt-4">
                    <h4 class="text-sm font-medium text-gray-900 dark:text-white mb-2">Training Data</h4>
                    <div class="grid grid-cols-2 gap-4 text-center">
                      <div>
                        <p class="text-sm font-medium text-gray-900 dark:text-white">{{ model.trainingData.samples.toLocaleString() }}</p>
                        <p class="text-xs text-gray-500 dark:text-gray-400">Samples</p>
                      </div>
                      <div>
                        <p class="text-sm font-medium text-gray-900 dark:text-white">{{ model.trainingData.lastUpdate }}</p>
                        <p class="text-xs text-gray-500 dark:text-gray-400">Last Update</p>
                      </div>
                    </div>
                    <button @click="retrainModel(model)" class="w-full mt-3 text-xs bg-gray-100 dark:bg-gray-700 hover:bg-gray-200 dark:hover:bg-gray-600 text-gray-700 dark:text-gray-300 py-2 px-3 rounded-md transition-colors">
                      Retrain Model
                    </button>
                  </div>

                  <!-- Model Actions -->
                  <div class="border-t border-gray-200 dark:border-gray-700 pt-4 flex space-x-2">
                    <button @click="testModel(model)" class="flex-1 text-xs bg-blue-50 dark:bg-blue-900/20 hover:bg-blue-100 dark:hover:bg-blue-900/30 text-blue-700 dark:text-blue-300 py-2 px-3 rounded-md transition-colors">
                      Test Model
                    </button>
                    <button @click="exportModel(model)" class="flex-1 text-xs bg-green-50 dark:bg-green-900/20 hover:bg-green-100 dark:hover:bg-green-900/30 text-green-700 dark:text-green-300 py-2 px-3 rounded-md transition-colors">
                      Export Config
                    </button>
                    <button @click="viewModelDetails(model)" class="flex-1 text-xs bg-gray-50 dark:bg-gray-700 hover:bg-gray-100 dark:hover:bg-gray-600 text-gray-700 dark:text-gray-300 py-2 px-3 rounded-md transition-colors">
                      View Details
                    </button>
                  </div>
                </div>
              </div>
            </div>

            <!-- Model Training & Deployment -->
            <div class="bg-white dark:bg-gray-800 rounded-lg shadow">
              <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
                <h3 class="text-lg font-medium text-gray-900 dark:text-white">Model Training & Deployment</h3>
              </div>
              <div class="p-6">
                <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
                  
                  <!-- Training Queue -->
                  <div>
                    <h4 class="text-sm font-medium text-gray-900 dark:text-white mb-4">Training Queue</h4>
                    <div class="space-y-3">
                      <div v-for="job in trainingQueue" :key="job.id" class="flex items-center justify-between p-3 bg-gray-50 dark:bg-gray-700 rounded-lg">
                        <div class="flex items-center">
                          <div class="w-8 h-8 rounded-lg flex items-center justify-center mr-3" :class="getJobStatusColor(job.status)">
                            <svg v-if="job.status === 'running'" class="w-4 h-4 text-white animate-spin" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
                            </svg>
                            <svg v-else-if="job.status === 'queued'" class="w-4 h-4 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
                            </svg>
                            <svg v-else class="w-4 h-4 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
                            </svg>
                          </div>
                          <div>
                            <p class="text-sm font-medium text-gray-900 dark:text-white">{{ job.modelName }}</p>
                            <p class="text-xs text-gray-500 dark:text-gray-400">{{ job.status }} • {{ job.eta }}</p>
                          </div>
                        </div>
                        <div class="text-right">
                          <p class="text-xs text-gray-500 dark:text-gray-400">{{ job.progress }}%</p>
                          <div class="w-16 h-1 bg-gray-200 dark:bg-gray-600 rounded-full mt-1">
                            <div class="h-1 bg-blue-500 rounded-full transition-all duration-300" :style="{ width: `${job.progress}%` }"></div>
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>

                  <!-- Deployment Status -->
                  <div>
                    <h4 class="text-sm font-medium text-gray-900 dark:text-white mb-4">Deployment Status</h4>
                    <div class="space-y-3">
                      <div v-for="deployment in deploymentStatus" :key="deployment.id" class="flex items-center justify-between p-3 bg-gray-50 dark:bg-gray-700 rounded-lg">
                        <div class="flex items-center">
                          <div class="w-8 h-8 rounded-lg flex items-center justify-center mr-3" :class="getDeploymentStatusColor(deployment.status)">
                            <svg v-if="deployment.status === 'live'" class="w-4 h-4 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
                            </svg>
                            <svg v-else-if="deployment.status === 'deploying'" class="w-4 h-4 text-white animate-spin" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
                            </svg>
                            <svg v-else class="w-4 h-4 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L3.732 16.5c-.77.833.192 2.5 1.732 2.5z" />
                            </svg>
                          </div>
                          <div>
                            <p class="text-sm font-medium text-gray-900 dark:text-white">{{ deployment.modelName }}</p>
                            <p class="text-xs text-gray-500 dark:text-gray-400">{{ deployment.environment }} • v{{ deployment.version }}</p>
                          </div>
                        </div>
                        <div class="text-right">
                          <span class="text-xs font-medium px-2 py-1 rounded" :class="getDeploymentBadge(deployment.status)">
                            {{ deployment.status }}
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
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'

// Mock Nuxt functions
function definePageMeta(meta: any) {}
function useHead(options: any) {
  if (typeof document !== 'undefined') {
    document.title = options.title || 'AI Model Configuration - TOSS ERP'
  }
}

// Reactive data
const selectedCategory = ref('nlp')

// Model categories
const modelCategories = ref([
  {
    id: 'nlp',
    name: 'Natural Language Processing',
    icon: 'chat-icon',
    modelCount: 5
  },
  {
    id: 'computer-vision',
    name: 'Computer Vision',
    icon: 'eye-icon',
    modelCount: 3
  },
  {
    id: 'predictive',
    name: 'Predictive Analytics',
    icon: 'chart-icon',
    modelCount: 4
  },
  {
    id: 'recommendation',
    name: 'Recommendation Engine',
    icon: 'star-icon',
    modelCount: 2
  },
  {
    id: 'anomaly',
    name: 'Anomaly Detection',
    icon: 'shield-icon',
    modelCount: 3
  }
])

// AI models
const aiModels = ref([
  // NLP Models
  {
    id: 'nlp-1',
    category: 'nlp',
    name: 'Customer Service Bot',
    description: 'Automated customer inquiry processing and response generation',
    status: 'Active',
    enabled: true,
    statusColor: 'bg-green-500',
    icon: 'chat-icon',
    accuracy: 94.7,
    latency: 245,
    throughput: 847,
    parameters: [
      {
        name: 'temperature',
        label: 'Response Creativity',
        type: 'slider',
        value: 0.7,
        min: 0.1,
        max: 1.0,
        step: 0.1,
        unit: ''
      },
      {
        name: 'max_tokens',
        label: 'Max Response Length',
        type: 'number',
        value: 150,
        min: 50,
        max: 500,
        unit: ' tokens'
      },
      {
        name: 'model_size',
        label: 'Model Size',
        type: 'select',
        value: 'medium',
        options: ['small', 'medium', 'large'],
        unit: ''
      }
    ],
    trainingData: {
      samples: 45780,
      lastUpdate: '2 days ago'
    }
  },
  {
    id: 'nlp-2',
    category: 'nlp',
    name: 'Document Processor',
    description: 'Automated extraction and classification of business documents',
    status: 'Active',
    enabled: true,
    statusColor: 'bg-green-500',
    icon: 'document-icon',
    accuracy: 98.2,
    latency: 1240,
    throughput: 156,
    parameters: [
      {
        name: 'confidence_threshold',
        label: 'Confidence Threshold',
        type: 'slider',
        value: 0.85,
        min: 0.5,
        max: 0.99,
        step: 0.01,
        unit: ''
      },
      {
        name: 'processing_mode',
        label: 'Processing Mode',
        type: 'select',
        value: 'balanced',
        options: ['fast', 'balanced', 'accurate'],
        unit: ''
      }
    ],
    trainingData: {
      samples: 23450,
      lastUpdate: '1 week ago'
    }
  },
  // Predictive Models
  {
    id: 'pred-1',
    category: 'predictive',
    name: 'Sales Forecasting',
    description: 'Predict future sales trends and revenue projections',
    status: 'Active',
    enabled: true,
    statusColor: 'bg-blue-500',
    icon: 'trend-icon',
    accuracy: 91.3,
    latency: 890,
    throughput: 234,
    parameters: [
      {
        name: 'forecast_horizon',
        label: 'Forecast Period',
        type: 'number',
        value: 30,
        min: 7,
        max: 365,
        unit: ' days'
      },
      {
        name: 'seasonality',
        label: 'Seasonality Adjustment',
        type: 'select',
        value: 'auto',
        options: ['none', 'weekly', 'monthly', 'quarterly', 'auto'],
        unit: ''
      }
    ],
    trainingData: {
      samples: 87650,
      lastUpdate: '3 days ago'
    }
  },
  {
    id: 'pred-2',
    category: 'predictive',
    name: 'Inventory Optimizer',
    description: 'Optimize stock levels and predict reorder points',
    status: 'Training',
    enabled: false,
    statusColor: 'bg-yellow-500',
    icon: 'box-icon',
    accuracy: 87.9,
    latency: 450,
    throughput: 567,
    parameters: [
      {
        name: 'safety_stock',
        label: 'Safety Stock Multiplier',
        type: 'slider',
        value: 1.5,
        min: 1.0,
        max: 3.0,
        step: 0.1,
        unit: 'x'
      },
      {
        name: 'lead_time_buffer',
        label: 'Lead Time Buffer',
        type: 'number',
        value: 7,
        min: 1,
        max: 30,
        unit: ' days'
      }
    ],
    trainingData: {
      samples: 156780,
      lastUpdate: '5 days ago'
    }
  },
  // Computer Vision Models
  {
    id: 'cv-1',
    category: 'computer-vision',
    name: 'Receipt Scanner',
    description: 'Automated receipt and invoice data extraction',
    status: 'Active',
    enabled: true,
    statusColor: 'bg-purple-500',
    icon: 'scan-icon',
    accuracy: 96.8,
    latency: 1890,
    throughput: 78,
    parameters: [
      {
        name: 'image_quality',
        label: 'Image Quality Threshold',
        type: 'slider',
        value: 0.8,
        min: 0.5,
        max: 0.95,
        step: 0.05,
        unit: ''
      },
      {
        name: 'ocr_engine',
        label: 'OCR Engine',
        type: 'select',
        value: 'advanced',
        options: ['basic', 'standard', 'advanced'],
        unit: ''
      }
    ],
    trainingData: {
      samples: 34560,
      lastUpdate: '1 week ago'
    }
  },
  // Anomaly Detection Models
  {
    id: 'anom-1',
    category: 'anomaly',
    name: 'Fraud Detection',
    description: 'Detect unusual patterns in financial transactions',
    status: 'Active',
    enabled: true,
    statusColor: 'bg-red-500',
    icon: 'shield-icon',
    accuracy: 99.1,
    latency: 125,
    throughput: 2340,
    parameters: [
      {
        name: 'sensitivity',
        label: 'Detection Sensitivity',
        type: 'slider',
        value: 0.75,
        min: 0.5,
        max: 0.95,
        step: 0.05,
        unit: ''
      },
      {
        name: 'alert_threshold',
        label: 'Alert Threshold',
        type: 'slider',
        value: 0.9,
        min: 0.7,
        max: 0.99,
        step: 0.01,
        unit: ''
      }
    ],
    trainingData: {
      samples: 245670,
      lastUpdate: '1 day ago'
    }
  }
])

// Performance summary
const performanceSummary = ref([
  {
    id: 'perf-1',
    name: 'Overall Accuracy',
    description: 'Average model accuracy',
    value: 94.8,
    unit: '%',
    score: 95
  },
  {
    id: 'perf-2',
    name: 'Response Time',
    description: 'Average latency',
    value: 567,
    unit: 'ms',
    score: 78
  },
  {
    id: 'perf-3',
    name: 'Throughput',
    description: 'Requests per minute',
    value: 1247,
    unit: '/min',
    score: 89
  },
  {
    id: 'perf-4',
    name: 'Uptime',
    description: 'Model availability',
    value: 99.7,
    unit: '%',
    score: 97
  }
])

// Training queue
const trainingQueue = ref([
  {
    id: 'train-1',
    modelName: 'Inventory Optimizer',
    status: 'running',
    progress: 67,
    eta: '45 min remaining'
  },
  {
    id: 'train-2',
    modelName: 'Customer Sentiment Analysis',
    status: 'queued',
    progress: 0,
    eta: 'Waiting in queue'
  },
  {
    id: 'train-3',
    modelName: 'Price Optimization',
    status: 'completed',
    progress: 100,
    eta: 'Completed 2h ago'
  }
])

// Deployment status
const deploymentStatus = ref([
  {
    id: 'deploy-1',
    modelName: 'Customer Service Bot',
    environment: 'Production',
    version: '2.1.4',
    status: 'live'
  },
  {
    id: 'deploy-2',
    modelName: 'Sales Forecasting',
    environment: 'Staging',
    version: '1.8.2',
    status: 'deploying'
  },
  {
    id: 'deploy-3',
    modelName: 'Receipt Scanner',
    environment: 'Production',
    version: '3.0.1',
    status: 'failed'
  }
])

// Computed properties
const filteredModels = computed(() => {
  return aiModels.value.filter(model => model.category === selectedCategory.value)
})

// Methods
const getStatusBadge = (status: string): string => {
  switch (status.toLowerCase()) {
    case 'active':
      return 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-300'
    case 'training':
      return 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-300'
    case 'inactive':
      return 'bg-gray-100 text-gray-800 dark:bg-gray-900 dark:text-gray-300'
    case 'error':
      return 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-300'
    default:
      return 'bg-gray-100 text-gray-800 dark:bg-gray-900 dark:text-gray-300'
  }
}

const getPerformanceColor = (score: number): string => {
  if (score >= 90) return 'bg-green-500'
  if (score >= 70) return 'bg-yellow-500'
  return 'bg-red-500'
}

const getJobStatusColor = (status: string): string => {
  switch (status) {
    case 'running':
      return 'bg-blue-500'
    case 'queued':
      return 'bg-yellow-500'
    case 'completed':
      return 'bg-green-500'
    default:
      return 'bg-gray-500'
  }
}

const getDeploymentStatusColor = (status: string): string => {
  switch (status) {
    case 'live':
      return 'bg-green-500'
    case 'deploying':
      return 'bg-blue-500'
    case 'failed':
      return 'bg-red-500'
    default:
      return 'bg-gray-500'
  }
}

const getDeploymentBadge = (status: string): string => {
  switch (status) {
    case 'live':
      return 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-300'
    case 'deploying':
      return 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-300'
    case 'failed':
      return 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-300'
    default:
      return 'bg-gray-100 text-gray-800 dark:bg-gray-900 dark:text-gray-300'
  }
}

const toggleModel = (model: any) => {
  model.enabled = !model.enabled
  console.log(`${model.enabled ? 'Enabling' : 'Disabling'} model: ${model.name}`)
}

const retrainModel = (model: any) => {
  console.log(`Starting retraining for: ${model.name}`)
  // Implementation would start model retraining
}

const testModel = (model: any) => {
  console.log(`Testing model: ${model.name}`)
  // Implementation would run model tests
}

const exportModel = (model: any) => {
  console.log(`Exporting configuration for: ${model.name}`)
  // Implementation would export model configuration
}

const viewModelDetails = (model: any) => {
  console.log(`Viewing details for: ${model.name}`)
  // Implementation would show detailed model view
}

const saveAllConfigurations = () => {
  console.log('Saving all model configurations...')
  // Implementation would save all changes
}

const deployModels = () => {
  console.log('Deploying models...')
  // Implementation would deploy updated models
}

// Page meta
definePageMeta({
  title: 'AI Model Configuration',
  description: 'Configure and fine-tune AI models for optimal business performance'
})

// SEO
useHead({
  title: 'AI Model Configuration - TOSS ERP',
  meta: [
    { name: 'description', content: 'Advanced AI model configuration interface for optimizing business automation and performance.' }
  ]
})
</script>

<style scoped>
/* Range slider styling */
input[type="range"] {
  -webkit-appearance: none;
  appearance: none;
  background: transparent;
  cursor: pointer;
}

input[type="range"]::-webkit-slider-track {
  background: #e5e7eb;
  height: 8px;
  border-radius: 4px;
}

input[type="range"]::-webkit-slider-thumb {
  -webkit-appearance: none;
  appearance: none;
  background: #4f46e5;
  height: 20px;
  width: 20px;
  border-radius: 50%;
  cursor: pointer;
}

input[type="range"]::-moz-range-track {
  background: #e5e7eb;
  height: 8px;
  border-radius: 4px;
  border: none;
}

input[type="range"]::-moz-range-thumb {
  background: #4f46e5;
  height: 20px;
  width: 20px;
  border-radius: 50%;
  cursor: pointer;
  border: none;
}

/* Dark mode slider */
.dark input[type="range"]::-webkit-slider-track {
  background: #374151;
}

.dark input[type="range"]::-moz-range-track {
  background: #374151;
}

/* Transition animations */
.transition-colors {
  transition-property: color, background-color, border-color, text-decoration-color, fill, stroke;
  transition-timing-function: cubic-bezier(0.4, 0, 0.2, 1);
  transition-duration: 150ms;
}

.transition-all {
  transition-property: all;
  transition-timing-function: cubic-bezier(0.4, 0, 0.2, 1);
  transition-duration: 300ms;
}

/* Animation for spinning elements */
@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

.animate-spin {
  animation: spin 1s linear infinite;
}

/* Custom scrollbar for parameter lists */
.parameter-list {
  max-height: 300px;
  overflow-y: auto;
}

.parameter-list::-webkit-scrollbar {
  width: 4px;
}

.parameter-list::-webkit-scrollbar-track {
  background: #f1f1f1;
}

.parameter-list::-webkit-scrollbar-thumb {
  background: #c1c1c1;
  border-radius: 2px;
}

.dark .parameter-list::-webkit-scrollbar-track {
  background: #374151;
}

.dark .parameter-list::-webkit-scrollbar-thumb {
  background: #6b7280;
}
</style>
