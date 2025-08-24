<template>
  <div class="ai-insights-panel">
    <div class="card">
      <div class="card-header">
        <div class="flex items-center space-x-2">
          <div class="w-8 h-8 bg-gradient-to-r from-purple-500 to-pink-500 rounded-lg flex items-center justify-center">
            <SparklesIcon class="w-5 h-5 text-white" />
          </div>
          <div>
            <h3 class="text-lg font-semibold text-gray-900 dark:text-white">AI Insights</h3>
            <p class="text-sm text-gray-600 dark:text-gray-400">Powered by machine learning</p>
          </div>
        </div>
        <button
          @click="refreshInsights"
          :disabled="isRefreshing"
          class="text-primary-600 hover:text-primary-700 dark:text-primary-400 dark:hover:text-primary-300 disabled:opacity-50"
        >
          <ArrowPathIcon v-if="!isRefreshing" class="w-5 h-5" />
          <div v-else class="w-5 h-5 border-2 border-primary-600 border-t-transparent rounded-full animate-spin"></div>
        </button>
      </div>
      
      <div class="card-body">
        <div class="space-y-6">
          <!-- Stock Recommendations -->
          <div>
            <h4 class="text-md font-medium text-gray-900 dark:text-white mb-3 flex items-center">
              <LightBulbIcon class="w-4 h-4 text-yellow-500 mr-2" />
              Stock Recommendations
            </h4>
            <div class="space-y-3">
              <div
                v-for="recommendation in insights.recommendations"
                :key="recommendation.id"
                class="bg-gradient-to-r from-blue-50 to-indigo-50 dark:from-blue-900/20 dark:to-indigo-900/20 p-4 rounded-lg border-l-4 border-blue-500"
              >
                <div class="flex items-start justify-between">
                  <div class="flex-1">
                    <p class="text-sm font-medium text-blue-900 dark:text-blue-100">
                      {{ recommendation.title }}
                    </p>
                    <p class="text-sm text-blue-700 dark:text-blue-300 mt-1">
                      {{ recommendation.description }}
                    </p>
                    <div class="flex items-center space-x-4 mt-2 text-xs text-blue-600 dark:text-blue-400">
                      <span>Confidence: {{ recommendation.confidence }}%</span>
                      <span>Impact: {{ recommendation.impact }}</span>
                    </div>
                  </div>
                  <button
                    @click="applyRecommendation(recommendation)"
                    class="text-xs bg-blue-600 hover:bg-blue-700 text-white px-3 py-1 rounded-md transition-colors"
                  >
                    Apply
                  </button>
                </div>
              </div>
            </div>
          </div>

          <!-- Demand Forecasting -->
          <div>
            <h4 class="text-md font-medium text-gray-900 dark:text-white mb-3 flex items-center">
              <ChartBarIcon class="w-4 h-4 text-green-500 mr-2" />
              Demand Forecasting
            </h4>
            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div
                v-for="forecast in insights.forecasts"
                :key="forecast.id"
                class="bg-gradient-to-r from-green-50 to-emerald-50 dark:from-green-900/20 dark:to-emerald-900/20 p-4 rounded-lg"
              >
                <div class="flex items-center justify-between mb-2">
                  <span class="text-sm font-medium text-green-900 dark:text-green-100">
                    {{ forecast.itemName }}
                  </span>
                  <span class="text-xs text-green-600 dark:text-green-400">
                    {{ forecast.timeframe }}
                  </span>
                </div>
                <div class="space-y-2">
                  <div class="flex justify-between text-sm">
                    <span class="text-green-700 dark:text-green-300">Predicted Demand:</span>
                    <span class="font-medium text-green-900 dark:text-green-100">
                      {{ forecast.predictedDemand }} units
                    </span>
                  </div>
                  <div class="flex justify-between text-sm">
                    <span class="text-green-700 dark:text-green-300">Current Stock:</span>
                    <span class="font-medium text-green-900 dark:text-green-100">
                      {{ forecast.currentStock }} units
                    </span>
                  </div>
                  <div class="flex justify-between text-sm">
                    <span class="text-green-700 dark:text-green-300">Suggested Order:</span>
                    <span class="font-medium text-green-900 dark:text-green-100">
                      {{ forecast.suggestedOrder }} units
                    </span>
                  </div>
                </div>
                <div class="mt-3 pt-2 border-t border-green-200 dark:border-green-700">
                  <div class="flex items-center justify-between text-xs">
                    <span class="text-green-600 dark:text-green-400">Accuracy</span>
                    <span class="text-green-800 dark:text-green-200 font-medium">
                      {{ forecast.accuracy }}%
                    </span>
                  </div>
                  <div class="w-full bg-green-200 dark:bg-green-700 rounded-full h-1.5 mt-1">
                    <div
                      class="bg-green-600 h-1.5 rounded-full transition-all duration-300"
                      :style="{ width: `${forecast.accuracy}%` }"
                    ></div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Anomaly Detection -->
          <div>
            <h4 class="text-md font-medium text-gray-900 dark:text-white mb-3 flex items-center">
              <ExclamationTriangleIcon class="w-4 h-4 text-orange-500 mr-2" />
              Anomaly Detection
            </h4>
            <div class="space-y-3">
              <div
                v-for="anomaly in insights.anomalies"
                :key="anomaly.id"
                class="bg-gradient-to-r from-orange-50 to-red-50 dark:from-orange-900/20 dark:to-red-900/20 p-4 rounded-lg border-l-4 border-orange-500"
              >
                <div class="flex items-start justify-between">
                  <div class="flex-1">
                    <div class="flex items-center space-x-2 mb-1">
                      <span class="text-sm font-medium text-orange-900 dark:text-orange-100">
                        {{ anomaly.title }}
                      </span>
                      <span
                        :class="[
                          'text-xs px-2 py-1 rounded-full',
                          anomaly.severity === 'high' ? 'bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-300' :
                          anomaly.severity === 'medium' ? 'bg-orange-100 text-orange-800 dark:bg-orange-900/30 dark:text-orange-300' :
                          'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-300'
                        ]"
                      >
                        {{ anomaly.severity }}
                      </span>
                    </div>
                    <p class="text-sm text-orange-700 dark:text-orange-300">
                      {{ anomaly.description }}
                    </p>
                    <div class="flex items-center space-x-4 mt-2 text-xs text-orange-600 dark:text-orange-400">
                      <span>Detected: {{ formatTimeAgo(anomaly.detectedAt) }}</span>
                      <span>Confidence: {{ anomaly.confidence }}%</span>
                    </div>
                  </div>
                  <button
                    @click="investigateAnomaly(anomaly)"
                    class="text-xs bg-orange-600 hover:bg-orange-700 text-white px-3 py-1 rounded-md transition-colors"
                  >
                    Investigate
                  </button>
                </div>
              </div>
            </div>
          </div>

          <!-- Performance Metrics -->
          <div>
            <h4 class="text-md font-medium text-gray-900 dark:text-white mb-3 flex items-center">
              <CogIcon class="w-4 h-4 text-purple-500 mr-2" />
              AI Model Performance
            </h4>
            <div class="grid grid-cols-2 md:grid-cols-4 gap-4">
              <div
                v-for="metric in insights.performanceMetrics"
                :key="metric.name"
                class="text-center p-3 bg-gradient-to-br from-purple-50 to-pink-50 dark:from-purple-900/20 dark:to-pink-900/20 rounded-lg"
              >
                <p class="text-2xl font-bold text-purple-600 dark:text-purple-400">
                  {{ metric.value }}
                </p>
                <p class="text-xs text-purple-700 dark:text-purple-300 mt-1">
                  {{ metric.name }}
                </p>
                <div class="flex items-center justify-center mt-1">
                  <ArrowUpIcon
                    v-if="metric.trend === 'up'"
                    class="w-3 h-3 text-green-500"
                  />
                  <ArrowDownIcon
                    v-else-if="metric.trend === 'down'"
                    class="w-3 h-3 text-red-500"
                  />
                  <span
                    :class="[
                      'text-xs ml-1',
                      metric.trend === 'up' ? 'text-green-600 dark:text-green-400' :
                      metric.trend === 'down' ? 'text-red-600 dark:text-red-400' :
                      'text-gray-600 dark:text-gray-400'
                    ]"
                  >
                    {{ metric.change }}
                  </span>
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
import { ref, reactive, onMounted } from 'vue'
import {
  SparklesIcon,
  LightBulbIcon,
  ChartBarIcon,
  ExclamationTriangleIcon,
  CogIcon,
  ArrowPathIcon,
  ArrowUpIcon,
  ArrowDownIcon
} from '@heroicons/vue/24/outline'

// State
const isRefreshing = ref(false)

// Mock insights data
const insights = reactive({
  recommendations: [
    {
      id: 1,
      title: 'Increase Reorder Level for Electronics',
      description: 'Based on recent sales patterns, consider increasing reorder levels for electronics category by 15% to prevent stockouts.',
      confidence: 87,
      impact: 'High'
    },
    {
      id: 2,
      title: 'Optimize Warehouse Layout',
      description: 'AI analysis suggests reorganizing warehouse layout could reduce picking time by 23% and improve efficiency.',
      confidence: 92,
      impact: 'Medium'
    },
    {
      id: 3,
      title: 'Seasonal Stock Preparation',
      description: 'Prepare for upcoming holiday season by increasing stock levels for popular gift items by 40%.',
      confidence: 78,
      impact: 'High'
    }
  ],
  forecasts: [
    {
      id: 1,
      itemName: 'Laptop Computer',
      timeframe: 'Next 30 days',
      predictedDemand: 45,
      currentStock: 25,
      suggestedOrder: 30,
      accuracy: 89
    },
    {
      id: 2,
      itemName: 'Wireless Mouse',
      timeframe: 'Next 30 days',
      predictedDemand: 180,
      currentStock: 150,
      suggestedOrder: 50,
      accuracy: 94
    },
    {
      id: 3,
      itemName: 'Office Chair',
      timeframe: 'Next 30 days',
      predictedDemand: 15,
      currentStock: 8,
      suggestedOrder: 12,
      accuracy: 82
    },
    {
      id: 4,
      itemName: 'Coffee Mug',
      timeframe: 'Next 30 days',
      predictedDemand: 75,
      currentStock: 0,
      suggestedOrder: 100,
      accuracy: 91
    }
  ],
  anomalies: [
    {
      id: 1,
      title: 'Unusual Stock Movement Pattern',
      description: 'Detected abnormal stock movement pattern for Product A during off-peak hours. Possible inventory discrepancy.',
      severity: 'medium',
      confidence: 76,
      detectedAt: new Date(Date.now() - 2 * 60 * 60 * 1000)
    },
    {
      id: 2,
      title: 'Demand Spike Detection',
      description: 'Unexpected 300% increase in demand for Product B. Consider investigating marketing campaigns or external factors.',
      severity: 'high',
      confidence: 89,
      detectedAt: new Date(Date.now() - 6 * 60 * 60 * 1000)
    },
    {
      id: 3,
      title: 'Supplier Delay Pattern',
      description: 'Consistent delays detected from Supplier C. Consider alternative suppliers or adjusting lead times.',
      severity: 'low',
      confidence: 67,
      detectedAt: new Date(Date.now() - 24 * 60 * 60 * 1000)
    }
  ],
  performanceMetrics: [
    {
      name: 'Forecast Accuracy',
      value: '89.2%',
      trend: 'up',
      change: '+2.1%'
    },
    {
      name: 'Anomaly Detection',
      value: '94.7%',
      trend: 'up',
      change: '+1.8%'
    },
    {
      name: 'Recommendation Success',
      value: '87.3%',
      trend: 'up',
      change: '+3.2%'
    },
    {
      name: 'Model Confidence',
      value: '91.5%',
      trend: 'stable',
      change: '0.0%'
    }
  ]
})

// Methods
const refreshInsights = async () => {
  isRefreshing.value = true
  
  try {
    // Simulate API call delay
    await new Promise(resolve => setTimeout(resolve, 2000))
    
    // Update insights with new mock data
    updateInsights()
    
    // Show success message (in real app, this would be a toast notification)
    console.log('Insights refreshed successfully')
  } catch (error) {
    console.error('Failed to refresh insights:', error)
  } finally {
    isRefreshing.value = false
  }
}

const updateInsights = () => {
  // Update recommendations with slight variations
  insights.recommendations.forEach(rec => {
    rec.confidence = Math.max(70, Math.min(95, rec.confidence + (Math.random() - 0.5) * 10))
  })
  
  // Update forecasts with new predictions
  insights.forecasts.forEach(forecast => {
    forecast.predictedDemand = Math.max(10, Math.round(forecast.predictedDemand * (0.9 + Math.random() * 0.2)))
    forecast.accuracy = Math.max(75, Math.min(98, Math.round(forecast.accuracy + (Math.random() - 0.5) * 10)))
  })
  
  // Update performance metrics
  insights.performanceMetrics.forEach(metric => {
    if (metric.trend !== 'stable') {
      const change = (Math.random() - 0.5) * 5
      metric.value = (parseFloat(metric.value) + change).toFixed(1) + '%'
      metric.change = (change > 0 ? '+' : '') + change.toFixed(1) + '%'
    }
  })
}

const applyRecommendation = (recommendation: any) => {
  console.log('Applying recommendation:', recommendation.title)
  // In real app, this would trigger an action or show a form
  alert(`Applying recommendation: ${recommendation.title}`)
}

const investigateAnomaly = (anomaly: any) => {
  console.log('Investigating anomaly:', anomaly.title)
  // In real app, this would open a detailed investigation view
  alert(`Investigating anomaly: ${anomaly.title}`)
}

const formatTimeAgo = (date: Date): string => {
  const now = new Date()
  const diffMs = now.getTime() - date.getTime()
  const diffHours = Math.floor(diffMs / (1000 * 60 * 60))
  
  if (diffHours < 1) return 'Just now'
  if (diffHours === 1) return '1 hour ago'
  if (diffHours < 24) return `${diffHours} hours ago`
  
  const diffDays = Math.floor(diffHours / 24)
  if (diffDays === 1) return '1 day ago'
  return `${diffDays} days ago`
}

// Initialize insights on mount
onMounted(() => {
  // Insights are already initialized with mock data
})
</script>

<style scoped>
.ai-insights-panel {
  @apply space-y-6;
}

.card {
  @apply bg-white dark:bg-gray-900 border border-gray-200 dark:border-gray-700 rounded-lg shadow-sm;
}

.card-header {
  @apply px-6 py-4 border-b border-gray-200 dark:border-gray-700 flex items-center justify-between;
}

.card-body {
  @apply px-6 py-4;
}

/* Custom gradients for better visual appeal */
.bg-gradient-to-r {
  background: linear-gradient(to right, var(--tw-gradient-stops));
}

.bg-gradient-to-br {
  background: linear-gradient(to bottom right, var(--tw-gradient-stops));
}

/* Animation for refreshing */
@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

.animate-spin {
  animation: spin 1s linear infinite;
}
</style>
