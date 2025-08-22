<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <!-- Enhanced Page Header -->
    <div class="bg-gradient-to-r from-blue-600 to-indigo-700 shadow-lg">
      <div class="max-w-7xl mx-auto py-8 px-4 sm:px-6 lg:px-8">
        <div class="md:flex md:items-center md:justify-between">
          <div class="flex-1 min-w-0">
            <h1 class="text-3xl font-bold text-white sm:text-4xl">
              TOSS AI Analytics & Insights
            </h1>
            <p class="mt-2 text-lg text-blue-100">
              Advanced business intelligence powered by artificial intelligence and machine learning
            </p>
            <div class="mt-4 flex items-center space-x-6">
              <div class="flex items-center text-blue-100">
                <svg class="h-5 w-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 10V3L4 14h7v7l9-11h-7z" />
                </svg>
                <span class="text-sm">Last updated: {{ lastUpdated }}</span>
              </div>
              <div class="flex items-center text-blue-100">
                <svg class="h-5 w-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.663 17h4.673M12 3v1m6.364 1.636l-.707.707M21 12h-1M4 12H3m3.343-5.657l-.707-.707m2.828 9.9a5 5 0 117.072 0l-.548.547A3.374 3.374 0 0014 18.469V19a2 2 0 11-4 0v-.531c0-.895-.356-1.754-.988-2.386l-.548-.547z" />
                </svg>
                <span class="text-sm">{{ activeModels }} AI models active</span>
              </div>
            </div>
          </div>
          <div class="mt-6 flex md:mt-0 md:ml-4 space-x-3">
            <select
              v-model="selectedTimeframe"
              class="inline-flex items-center px-4 py-2 border border-blue-400 rounded-lg bg-blue-500/20 text-white text-sm font-medium hover:bg-blue-500/30 focus:outline-none focus:ring-2 focus:ring-white"
            >
              <option value="24h">Last 24 Hours</option>
              <option value="7d">Last 7 Days</option>
              <option value="30d">Last 30 Days</option>
              <option value="90d">Last 90 Days</option>
            </select>
            <button
              @click="refreshAnalytics"
              type="button"
              class="inline-flex items-center px-4 py-2 border border-blue-400 rounded-lg bg-blue-500/20 text-white text-sm font-medium hover:bg-blue-500/30 focus:outline-none focus:ring-2 focus:ring-white"
            >
              <svg class="-ml-1 mr-2 h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
              </svg>
              Refresh Data
            </button>
            <button
              @click="generateReport"
              type="button"
              class="inline-flex items-center px-4 py-2 bg-white text-blue-600 rounded-lg shadow-sm text-sm font-medium hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-blue-500"
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
      <!-- Enhanced AI Insights Cards -->
      <div class="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-4 mb-8">
        <div class="bg-white dark:bg-gray-800 overflow-hidden shadow-lg rounded-xl border border-gray-200 dark:border-gray-700 hover:shadow-xl transition-shadow duration-300">
          <div class="p-6">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <div class="w-12 h-12 bg-gradient-to-r from-blue-500 to-blue-600 rounded-xl flex items-center justify-center shadow-lg">
                  <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 7h8m0 0v8m0-8l-8 8-4-4-6 6" />
                  </svg>
                </div>
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 dark:text-gray-400 truncate">AI Predictions Accuracy</dt>
                  <dd class="text-2xl font-bold text-gray-900 dark:text-white">{{ predictionAccuracy }}%</dd>
                  <dd class="text-sm text-green-600 dark:text-green-400 flex items-center">
                    <svg class="w-3 h-3 mr-1" fill="currentColor" viewBox="0 0 20 20">
                      <path fill-rule="evenodd" d="M3.293 9.707a1 1 0 010-1.414l6-6a1 1 0 011.414 0l6 6a1 1 0 01-1.414 1.414L11 5.414V17a1 1 0 11-2 0V5.414L4.707 9.707a1 1 0 01-1.414 0z" clip-rule="evenodd" />
                    </svg>
                    +2.3% from last week
                  </dd>
                </dl>
              </div>
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 overflow-hidden shadow-lg rounded-xl border border-gray-200 dark:border-gray-700 hover:shadow-xl transition-shadow duration-300">
          <div class="p-6">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <div class="w-12 h-12 bg-gradient-to-r from-green-500 to-green-600 rounded-xl flex items-center justify-center shadow-lg">
                  <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.663 17h4.673M12 3v1m6.364 1.636l-.707.707M21 12h-1M4 12H3m3.343-5.657l-.707-.707m2.828 9.9a5 5 0 117.072 0l-.548.547A3.374 3.374 0 0014 18.469V19a2 2 0 11-4 0v-.531c0-.895-.356-1.754-.988-2.386l-.548-.547z" />
                  </svg>
                </div>
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 dark:text-gray-400 truncate">AI Insights Generated</dt>
                  <dd class="text-2xl font-bold text-gray-900 dark:text-white">{{ insightsGenerated }}</dd>
                  <dd class="text-sm text-green-600 dark:text-green-400 flex items-center">
                    <svg class="w-3 h-3 mr-1" fill="currentColor" viewBox="0 0 20 20">
                      <path fill-rule="evenodd" d="M3.293 9.707a1 1 0 010-1.414l6-6a1 1 0 011.414 0l6 6a1 1 0 01-1.414 1.414L11 5.414V17a1 1 0 11-2 0V5.414L4.707 9.707a1 1 0 01-1.414 0z" clip-rule="evenodd" />
                    </svg>
                    +15 today
                  </dd>
                </dl>
              </div>
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 overflow-hidden shadow-lg rounded-xl border border-gray-200 dark:border-gray-700 hover:shadow-xl transition-shadow duration-300">
          <div class="p-6">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <div class="w-12 h-12 bg-gradient-to-r from-purple-500 to-purple-600 rounded-xl flex items-center justify-center shadow-lg">
                  <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                  </svg>
                </div>
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 dark:text-gray-400 truncate">Potential Savings</dt>
                  <dd class="text-2xl font-bold text-gray-900 dark:text-white">{{ formatCurrency(potentialSavings) }}</dd>
                  <dd class="text-sm text-green-600 dark:text-green-400 flex items-center">
                    <svg class="w-3 h-3 mr-1" fill="currentColor" viewBox="0 0 20 20">
                      <path fill-rule="evenodd" d="M3.293 9.707a1 1 0 010-1.414l6-6a1 1 0 011.414 0l6 6a1 1 0 01-1.414 1.414L11 5.414V17a1 1 0 11-2 0V5.414L4.707 9.707a1 1 0 01-1.414 0z" clip-rule="evenodd" />
                    </svg>
                    +R 2,350 this week
                  </dd>
                </dl>
              </div>
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 overflow-hidden shadow-lg rounded-xl border border-gray-200 dark:border-gray-700 hover:shadow-xl transition-shadow duration-300">
          <div class="p-6">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <div class="w-12 h-12 bg-gradient-to-r from-yellow-500 to-orange-500 rounded-xl flex items-center justify-center shadow-lg">
                  <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 10V3L4 14h7v7l9-11h-7z" />
                  </svg>
                </div>
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 dark:text-gray-400 truncate">Automation Rate</dt>
                  <dd class="text-2xl font-bold text-gray-900 dark:text-white">{{ automationRate }}%</dd>
                  <dd class="text-sm text-green-600 dark:text-green-400 flex items-center">
                    <svg class="w-3 h-3 mr-1" fill="currentColor" viewBox="0 0 20 20">
                      <path fill-rule="evenodd" d="M3.293 9.707a1 1 0 010-1.414l6-6a1 1 0 011.414 0l6 6a1 1 0 01-1.414 1.414L11 5.414V17a1 1 0 11-2 0V5.414L4.707 9.707a1 1 0 01-1.414 0z" clip-rule="evenodd" />
                    </svg>
                    +5% this month
                  </dd>
                </dl>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
        <!-- Main Analytics Content -->
        <div class="lg:col-span-2 space-y-6">
          <!-- Enhanced AI Insights with Rich Visual Elements -->
          <div class="bg-white dark:bg-gray-800 shadow-lg rounded-xl p-6 border border-gray-200 dark:border-gray-700">
            <div class="flex items-center justify-between mb-6">
              <h3 class="text-xl font-semibold text-gray-900 dark:text-white">Latest AI Insights</h3>
              <div class="flex items-center space-x-2">
                <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-300">
                  <svg class="w-3 h-3 mr-1" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd" />
                  </svg>
                  {{ aiInsights.length }} Active
                </span>
                <button
                  @click="refreshInsights"
                  class="text-gray-400 hover:text-gray-600 dark:text-gray-500 dark:hover:text-gray-300"
                >
                  <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
                  </svg>
                </button>
              </div>
            </div>
            <div class="space-y-4">
              <div v-for="insight in aiInsights" :key="insight.id" class="border-l-4 pl-4 py-3 hover:bg-gray-50 dark:hover:bg-gray-700/50 rounded-r-lg transition-colors" :class="getInsightBorderColor(insight.type)">
                <div class="flex items-start justify-between">
                  <div class="flex-1">
                    <div class="flex items-center space-x-2 mb-2">
                      <h4 class="text-sm font-semibold text-gray-900 dark:text-white">{{ insight.title }}</h4>
                      <span
                        class="inline-flex items-center px-2 py-0.5 rounded-full text-xs font-medium"
                        :class="getInsightBadgeColor(insight.type)"
                      >
                        {{ insight.type }}
                      </span>
                    </div>
                    <p class="text-sm text-gray-600 dark:text-gray-300 mb-3">{{ insight.description }}</p>
                    <div class="flex items-center justify-between">
                      <div class="flex items-center space-x-4">
                        <div class="flex items-center space-x-1">
                          <svg class="w-4 h-4 text-blue-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
                          </svg>
                          <span class="text-xs text-gray-500 dark:text-gray-400">
                            Confidence: <span class="font-medium">{{ insight.confidence }}%</span>
                          </span>
                        </div>
                        <div class="flex items-center space-x-1">
                          <svg class="w-4 h-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
                          </svg>
                          <span class="text-xs text-gray-500 dark:text-gray-400">
                            {{ formatTime(insight.timestamp) }}
                          </span>
                        </div>
                      </div>
                      <button
                        @click="implementInsight(insight)"
                        class="inline-flex items-center px-3 py-1 border border-orange-200 rounded-md text-xs font-medium text-orange-600 hover:bg-orange-50 hover:border-orange-300 focus:outline-none focus:ring-2 focus:ring-orange-500"
                      >
                        Implement
                      </button>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Predictive Analytics -->
          <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-4">Predictive Analytics</h3>
            <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
              <!-- Sales Forecast -->
              <div class="border border-gray-200 dark:border-gray-700 rounded-lg p-4">
                <h4 class="text-sm font-medium text-gray-900 dark:text-white mb-3">Sales Forecast (Next 30 Days)</h4>
                <div class="space-y-2">
                  <div class="flex justify-between">
                    <span class="text-sm text-gray-600 dark:text-gray-400">Predicted Sales</span>
                    <span class="text-sm font-medium text-gray-900 dark:text-white">{{ formatCurrency(salesForecast.predicted) }}</span>
                  </div>
                  <div class="flex justify-between">
                    <span class="text-sm text-gray-600 dark:text-gray-400">Growth Rate</span>
                    <span class="text-sm font-medium" :class="salesForecast.growth >= 0 ? 'text-green-600' : 'text-red-600'">
                      {{ salesForecast.growth >= 0 ? '+' : '' }}{{ salesForecast.growth }}%
                    </span>
                  </div>
                  <div class="flex justify-between">
                    <span class="text-sm text-gray-600 dark:text-gray-400">Confidence</span>
                    <span class="text-sm font-medium text-gray-900 dark:text-white">{{ salesForecast.confidence }}%</span>
                  </div>
                </div>
              </div>

              <!-- Inventory Optimization -->
              <div class="border border-gray-200 dark:border-gray-700 rounded-lg p-4">
                <h4 class="text-sm font-medium text-gray-900 dark:text-white mb-3">Inventory Optimization</h4>
                <div class="space-y-2">
                  <div class="flex justify-between">
                    <span class="text-sm text-gray-600 dark:text-gray-400">Overstock Items</span>
                    <span class="text-sm font-medium text-red-600">{{ inventoryOptimization.overstock }}</span>
                  </div>
                  <div class="flex justify-between">
                    <span class="text-sm text-gray-600 dark:text-gray-400">Understock Items</span>
                    <span class="text-sm font-medium text-yellow-600">{{ inventoryOptimization.understock }}</span>
                  </div>
                  <div class="flex justify-between">
                    <span class="text-sm text-gray-600 dark:text-gray-400">Optimal Items</span>
                    <span class="text-sm font-medium text-green-600">{{ inventoryOptimization.optimal }}</span>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Machine Learning Models -->
          <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-4">Machine Learning Models</h3>
            <div class="space-y-4">
              <div v-for="model in mlModels" :key="model.id" class="flex items-center justify-between p-4 border border-gray-200 dark:border-gray-700 rounded-lg">
                <div class="flex items-center">
                  <div class="w-10 h-10 rounded-md flex items-center justify-center mr-4" :class="model.color">
                    <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" :d="model.icon" />
                    </svg>
                  </div>
                  <div>
                    <h4 class="text-sm font-medium text-gray-900 dark:text-white">{{ model.name }}</h4>
                    <p class="text-sm text-gray-500 dark:text-gray-400">{{ model.description }}</p>
                  </div>
                </div>
                <div class="flex items-center space-x-4">
                  <div class="text-right">
                    <div class="text-sm font-medium text-gray-900 dark:text-white">{{ model.accuracy }}%</div>
                    <div class="text-xs text-gray-500 dark:text-gray-400">Accuracy</div>
                  </div>
                  <span
                    class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                    :class="model.status === 'active' 
                      ? 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-300' 
                      : 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300'"
                  >
                    {{ model.status }}
                  </span>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- AI Recommendations Panel -->
        <div class="space-y-6">
          <!-- Smart Recommendations -->
          <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-4">Smart Recommendations</h3>
            <div class="space-y-3">
              <div v-for="recommendation in recommendations" :key="recommendation.id" class="border border-gray-200 dark:border-gray-700 rounded-lg p-3">
                <div class="flex items-start justify-between">
                  <div class="flex-1">
                    <h4 class="text-sm font-medium text-gray-900 dark:text-white">{{ recommendation.title }}</h4>
                    <p class="text-xs text-gray-500 dark:text-gray-400 mt-1">{{ recommendation.description }}</p>
                    <div class="flex items-center mt-2">
                      <div class="flex items-center">
                        <svg class="w-3 h-3 text-green-500 mr-1" fill="currentColor" viewBox="0 0 20 20">
                          <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd" />
                        </svg>
                        <span class="text-xs text-green-600 dark:text-green-400">{{ recommendation.impact }}</span>
                      </div>
                    </div>
                  </div>
                  <button
                    @click="applyRecommendation(recommendation)"
                    class="ml-2 text-xs text-orange-600 hover:text-orange-700 font-medium"
                  >
                    Apply
                  </button>
                </div>
              </div>
            </div>
          </div>

          <!-- Data Quality -->
          <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-4">Data Quality</h3>
            <div class="space-y-4">
              <div>
                <div class="flex justify-between text-sm mb-1">
                  <span class="text-gray-600 dark:text-gray-400">Data Completeness</span>
                  <span class="text-gray-900 dark:text-white">{{ dataQuality.completeness }}%</span>
                </div>
                <div class="w-full bg-gray-200 rounded-full h-2">
                  <div class="bg-green-600 h-2 rounded-full" :style="`width: ${dataQuality.completeness}%`"></div>
                </div>
              </div>
              
              <div>
                <div class="flex justify-between text-sm mb-1">
                  <span class="text-gray-600 dark:text-gray-400">Data Accuracy</span>
                  <span class="text-gray-900 dark:text-white">{{ dataQuality.accuracy }}%</span>
                </div>
                <div class="w-full bg-gray-200 rounded-full h-2">
                  <div class="bg-blue-600 h-2 rounded-full" :style="`width: ${dataQuality.accuracy}%`"></div>
                </div>
              </div>
              
              <div>
                <div class="flex justify-between text-sm mb-1">
                  <span class="text-gray-600 dark:text-gray-400">Data Freshness</span>
                  <span class="text-gray-900 dark:text-white">{{ dataQuality.freshness }}%</span>
                </div>
                <div class="w-full bg-gray-200 rounded-full h-2">
                  <div class="bg-purple-600 h-2 rounded-full" :style="`width: ${dataQuality.freshness}%`"></div>
                </div>
              </div>
            </div>
          </div>

          <!-- AI Processing Status -->
          <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-4">AI Processing Status</h3>
            <div class="space-y-3">
              <div class="flex items-center justify-between">
                <span class="text-sm text-gray-600 dark:text-gray-400">Data Processing</span>
                <div class="flex items-center">
                  <div class="w-2 h-2 bg-green-400 rounded-full mr-2"></div>
                  <span class="text-sm text-gray-900 dark:text-white">Active</span>
                </div>
              </div>
              
              <div class="flex items-center justify-between">
                <span class="text-sm text-gray-600 dark:text-gray-400">Model Training</span>
                <div class="flex items-center">
                  <div class="w-2 h-2 bg-blue-400 rounded-full mr-2"></div>
                  <span class="text-sm text-gray-900 dark:text-white">In Progress</span>
                </div>
              </div>
              
              <div class="flex items-center justify-between">
                <span class="text-sm text-gray-600 dark:text-gray-400">Predictions</span>
                <div class="flex items-center">
                  <div class="w-2 h-2 bg-green-400 rounded-full mr-2"></div>
                  <span class="text-sm text-gray-900 dark:text-white">Ready</span>
                </div>
              </div>
              
              <div class="flex items-center justify-between">
                <span class="text-sm text-gray-600 dark:text-gray-400">Optimization</span>
                <div class="flex items-center">
                  <div class="w-2 h-2 bg-yellow-400 rounded-full mr-2"></div>
                  <span class="text-sm text-gray-900 dark:text-white">Scheduled</span>
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
// Import Vue 3 composables
import { ref } from 'vue'

// Types
interface AIInsight {
  id: string
  title: string
  description: string
  type: 'optimization' | 'prediction' | 'anomaly' | 'opportunity'
  confidence: number
  timestamp: Date
}

interface MLModel {
  id: string
  name: string
  description: string
  accuracy: number
  status: 'active' | 'training' | 'inactive'
  icon: string
  color: string
}

interface Recommendation {
  id: string
  title: string
  description: string
  impact: string
}

// Reactive data
const predictionAccuracy = ref(94)
const insightsGenerated = ref(127)
const potentialSavings = ref(15420)
const automationRate = ref(78)
const lastUpdated = ref('2 minutes ago')
const activeModels = ref(5)
const selectedTimeframe = ref('24h')

// Mock refresh function
const refreshInsights = async () => {
  console.log('Refreshing insights...')
}

// Sales forecast data
const salesForecast = ref({
  predicted: 156780,
  growth: 12.5,
  confidence: 89
})

// Inventory optimization data
const inventoryOptimization = ref({
  overstock: 12,
  understock: 7,
  optimal: 156
})

// Data quality metrics
const dataQuality = ref({
  completeness: 94,
  accuracy: 91,
  freshness: 88
})

// AI Insights
const aiInsights = ref<AIInsight[]>([
  {
    id: '1',
    title: 'Stock Optimization Opportunity',
    description: 'Reducing inventory of slow-moving items could free up R 23,400 in capital while maintaining service levels.',
    type: 'optimization',
    confidence: 92,
    timestamp: new Date(Date.now() - 1800000)
  },
  {
    id: '2',
    title: 'Demand Spike Prediction',
    description: 'Expected 35% increase in cooking oil demand next week based on seasonal patterns and local events.',
    type: 'prediction',
    confidence: 87,
    timestamp: new Date(Date.now() - 3600000)
  },
  {
    id: '3',
    title: 'Price Anomaly Detected',
    description: 'Competitor price changes suggest you could increase margins on 8 products by an average of 12%.',
    type: 'anomaly',
    confidence: 78,
    timestamp: new Date(Date.now() - 5400000)
  },
  {
    id: '4',
    title: 'Customer Retention Opportunity',
    description: 'Implementing a loyalty program for your top 50 customers could increase repeat purchases by 28%.',
    type: 'opportunity',
    confidence: 85,
    timestamp: new Date(Date.now() - 7200000)
  }
])

// Machine Learning Models
const mlModels = ref<MLModel[]>([
  {
    id: '1',
    name: 'Demand Forecasting',
    description: 'Predicts product demand based on historical data and external factors',
    accuracy: 94,
    status: 'active',
    icon: 'M13 7h8m0 0v8m0-8l-8 8-4-4-6 6',
    color: 'bg-blue-500'
  },
  {
    id: '2',
    name: 'Price Optimization',
    description: 'Suggests optimal pricing strategies for maximum profitability',
    accuracy: 87,
    status: 'active',
    icon: 'M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z',
    color: 'bg-green-500'
  },
  {
    id: '3',
    name: 'Customer Segmentation',
    description: 'Automatically groups customers based on behavior and preferences',
    accuracy: 91,
    status: 'active',
    icon: 'M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z',
    color: 'bg-purple-500'
  },
  {
    id: '4',
    name: 'Fraud Detection',
    description: 'Identifies suspicious transactions and potential fraud attempts',
    accuracy: 98,
    status: 'training',
    icon: 'M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z',
    color: 'bg-red-500'
  }
])

// Smart Recommendations
const recommendations = ref<Recommendation[]>([
  {
    id: '1',
    title: 'Optimize Stock Levels',
    description: 'Reduce inventory holding costs by 15%',
    impact: '+R 12,300 monthly savings'
  },
  {
    id: '2',
    title: 'Dynamic Pricing',
    description: 'Implement AI-driven pricing strategy',
    impact: '+8% profit margin'
  },
  {
    id: '3',
    title: 'Customer Campaigns',
    description: 'Launch targeted marketing campaigns',
    impact: '+25% customer retention'
  },
  {
    id: '4',
    title: 'Supplier Optimization',
    description: 'Renegotiate supplier contracts',
    impact: '+R 5,600 monthly savings'
  }
])

// Methods
const refreshAnalytics = () => {
  console.log('Refreshing AI analytics...')
  // Simulate data refresh
  predictionAccuracy.value = Math.floor(Math.random() * 10) + 90
  insightsGenerated.value = Math.floor(Math.random() * 50) + 100
}

const generateReport = () => {
  console.log('Generating AI analytics report...')
  // Implement report generation
}

const implementInsight = (insight: AIInsight) => {
  console.log('Implementing insight:', insight.title)
  // Implement insight application logic
}

const applyRecommendation = (recommendation: Recommendation) => {
  console.log('Applying recommendation:', recommendation.title)
  // Implement recommendation application logic
}

const getInsightBorderColor = (type: string): string => {
  switch (type) {
    case 'optimization': return 'border-blue-400'
    case 'prediction': return 'border-green-400'
    case 'anomaly': return 'border-red-400'
    case 'opportunity': return 'border-yellow-400'
    default: return 'border-gray-400'
  }
}

const getInsightBadgeColor = (type: string): string => {
  switch (type) {
    case 'optimization': return 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-300'
    case 'prediction': return 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-300'
    case 'anomaly': return 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-300'
    case 'opportunity': return 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-300'
    default: return 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300'
  }
}

const formatCurrency = (amount: number): string => {
  return new Intl.NumberFormat('en-ZA', {
    style: 'currency',
    currency: 'ZAR',
    minimumFractionDigits: 0,
    maximumFractionDigits: 0
  }).format(amount)
}

const formatTime = (date: Date): string => {
  return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
}
</script>

<style scoped>
/* Component-specific styles */
</style>
