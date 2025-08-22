<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <!-- Page Header -->
    <div class="bg-white dark:bg-gray-800 shadow">
      <div class="max-w-7xl mx-auto py-6 px-4 sm:px-6 lg:px-8">
        <div class="md:flex md:items-center md:justify-between">
          <div class="flex-1 min-w-0">
            <h2 class="text-2xl font-bold leading-7 text-gray-900 dark:text-white sm:text-3xl sm:truncate">
              Marketing & Campaigns
            </h2>
            <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
              Manage marketing campaigns, customer outreach, and promotional activities
            </p>
          </div>
          <div class="mt-4 flex md:mt-0 md:ml-4 space-x-3">
            <button
              @click="showNewCampaignModal = true"
              type="button"
              class="inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-orange-600 hover:bg-orange-700"
            >
              <svg class="-ml-1 mr-2 h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
              </svg>
              New Campaign
            </button>
          </div>
        </div>
      </div>
    </div>

    <div class="max-w-7xl mx-auto py-6 px-4 sm:px-6 lg:px-8">
      <!-- Marketing Stats Cards -->
      <div class="grid grid-cols-1 gap-5 sm:grid-cols-2 lg:grid-cols-4 mb-8">
        <div class="bg-white dark:bg-gray-800 overflow-hidden shadow rounded-lg">
          <div class="p-5">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <svg class="h-6 w-6 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
                </svg>
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 dark:text-gray-400 truncate">Active Campaigns</dt>
                  <dd class="text-lg font-medium text-gray-900 dark:text-white">{{ activeCampaigns }}</dd>
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
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 7h8m0 0v8m0-8l-8 8-4-4-6 6" />
                </svg>
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 dark:text-gray-400 truncate">Conversion Rate</dt>
                  <dd class="text-lg font-medium text-gray-900 dark:text-white">{{ conversionRate }}%</dd>
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
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" />
                </svg>
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 dark:text-gray-400 truncate">Total Reach</dt>
                  <dd class="text-lg font-medium text-gray-900 dark:text-white">{{ formatNumber(totalReach) }}</dd>
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
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 dark:text-gray-400 truncate">Marketing ROI</dt>
                  <dd class="text-lg font-medium text-gray-900 dark:text-white">{{ marketingROI }}%</dd>
                </dl>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
        <!-- Main Marketing Content -->
        <div class="lg:col-span-2 space-y-6">
          <!-- Campaign List -->
          <div class="bg-white dark:bg-gray-800 shadow rounded-lg">
            <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
              <div class="flex items-center justify-between">
                <h3 class="text-lg font-medium text-gray-900 dark:text-white">Marketing Campaigns</h3>
                <div class="flex items-center space-x-3">
                  <select
                    v-model="selectedCampaignType"
                    class="border border-gray-300 dark:border-gray-600 rounded-md px-3 py-2 text-sm focus:ring-orange-500 focus:border-orange-500 dark:bg-gray-700 dark:text-white"
                  >
                    <option value="">All Types</option>
                    <option value="email">Email</option>
                    <option value="social">Social Media</option>
                    <option value="print">Print</option>
                    <option value="radio">Radio</option>
                    <option value="digital">Digital Ads</option>
                  </select>
                </div>
              </div>
            </div>
            <div class="overflow-x-auto">
              <table class="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
                <thead class="bg-gray-50 dark:bg-gray-700">
                  <tr>
                    <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Campaign</th>
                    <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Type</th>
                    <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Status</th>
                    <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Budget</th>
                    <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Performance</th>
                    <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Actions</th>
                  </tr>
                </thead>
                <tbody class="bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700">
                  <tr v-for="campaign in filteredCampaigns" :key="campaign.id" class="hover:bg-gray-50 dark:hover:bg-gray-700">
                    <td class="px-6 py-4 whitespace-nowrap">
                      <div class="text-sm font-medium text-gray-900 dark:text-white">{{ campaign.name }}</div>
                      <div class="text-sm text-gray-500 dark:text-gray-400">{{ campaign.description }}</div>
                    </td>
                    <td class="px-6 py-4 whitespace-nowrap">
                      <span
                        class="inline-flex px-2 py-1 text-xs font-semibold rounded-full"
                        :class="getCampaignTypeColor(campaign.type)"
                      >
                        {{ campaign.type }}
                      </span>
                    </td>
                    <td class="px-6 py-4 whitespace-nowrap">
                      <span
                        class="inline-flex px-2 py-1 text-xs font-semibold rounded-full"
                        :class="getStatusColor(campaign.status)"
                      >
                        {{ campaign.status }}
                      </span>
                    </td>
                    <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">{{ formatCurrency(campaign.budget) }}</td>
                    <td class="px-6 py-4 whitespace-nowrap">
                      <div class="flex items-center">
                        <div class="w-16 bg-gray-200 rounded-full h-2 mr-2">
                          <div class="bg-orange-600 h-2 rounded-full" :style="`width: ${campaign.performance}%`"></div>
                        </div>
                        <span class="text-sm text-gray-900 dark:text-white">{{ campaign.performance }}%</span>
                      </div>
                    </td>
                    <td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
                      <button
                        @click="viewCampaign(campaign)"
                        class="text-orange-600 hover:text-orange-900 mr-3"
                      >
                        View
                      </button>
                      <button
                        @click="editCampaign(campaign)"
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

          <!-- Marketing Analytics -->
          <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-4">Campaign Performance</h3>
            <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
              <div class="text-center">
                <div class="text-2xl font-bold text-gray-900 dark:text-white">{{ formatNumber(impressions) }}</div>
                <div class="text-sm text-gray-500 dark:text-gray-400">Total Impressions</div>
                <div class="text-xs text-green-600 dark:text-green-400 mt-1">+15% this month</div>
              </div>
              <div class="text-center">
                <div class="text-2xl font-bold text-gray-900 dark:text-white">{{ formatNumber(clicks) }}</div>
                <div class="text-sm text-gray-500 dark:text-gray-400">Total Clicks</div>
                <div class="text-xs text-green-600 dark:text-green-400 mt-1">+8% this month</div>
              </div>
              <div class="text-center">
                <div class="text-2xl font-bold text-gray-900 dark:text-white">{{ conversions }}</div>
                <div class="text-sm text-gray-500 dark:text-gray-400">Conversions</div>
                <div class="text-xs text-green-600 dark:text-green-400 mt-1">+12% this month</div>
              </div>
            </div>
          </div>

          <!-- Customer Segments -->
          <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-4">Customer Segments</h3>
            <div class="space-y-4">
              <div v-for="segment in customerSegments" :key="segment.id" class="border border-gray-200 dark:border-gray-700 rounded-lg p-4">
                <div class="flex items-center justify-between">
                  <div class="flex items-center">
                    <div class="w-10 h-10 rounded-md flex items-center justify-center mr-4" :class="segment.color">
                      <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" :d="segment.icon" />
                      </svg>
                    </div>
                    <div>
                      <h4 class="text-sm font-medium text-gray-900 dark:text-white">{{ segment.name }}</h4>
                      <p class="text-sm text-gray-500 dark:text-gray-400">{{ segment.description }}</p>
                    </div>
                  </div>
                  <div class="flex items-center space-x-4">
                    <div class="text-right">
                      <div class="text-sm font-medium text-gray-900 dark:text-white">{{ formatNumber(segment.size) }}</div>
                      <div class="text-xs text-gray-500 dark:text-gray-400">Customers</div>
                    </div>
                    <div class="text-right">
                      <div class="text-sm font-medium text-gray-900 dark:text-white">{{ segment.engagement }}%</div>
                      <div class="text-xs text-gray-500 dark:text-gray-400">Engagement</div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Marketing Management Panel -->
        <div class="space-y-6">
          <!-- Quick Actions -->
          <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-4">Quick Actions</h3>
            <div class="space-y-3">
              <button
                @click="createEmail"
                class="w-full text-left px-4 py-3 border border-gray-200 dark:border-gray-600 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
              >
                <div class="flex items-center">
                  <svg class="w-5 h-5 text-blue-500 mr-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 8l7.89 4.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z" />
                  </svg>
                  <div>
                    <div class="text-sm font-medium text-gray-900 dark:text-white">Email Campaign</div>
                    <div class="text-xs text-gray-500 dark:text-gray-400">Send targeted emails</div>
                  </div>
                </div>
              </button>

              <button
                @click="schedulePromotion"
                class="w-full text-left px-4 py-3 border border-gray-200 dark:border-gray-600 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
              >
                <div class="flex items-center">
                  <svg class="w-5 h-5 text-green-500 mr-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z" />
                  </svg>
                  <div>
                    <div class="text-sm font-medium text-gray-900 dark:text-white">Promotion</div>
                    <div class="text-xs text-gray-500 dark:text-gray-400">Schedule promotions</div>
                  </div>
                </div>
              </button>

              <button
                @click="analyzeTrends"
                class="w-full text-left px-4 py-3 border border-gray-200 dark:border-gray-600 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
              >
                <div class="flex items-center">
                  <svg class="w-5 h-5 text-purple-500 mr-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
                  </svg>
                  <div>
                    <div class="text-sm font-medium text-gray-900 dark:text-white">Market Analysis</div>
                    <div class="text-xs text-gray-500 dark:text-gray-400">Analyze market trends</div>
                  </div>
                </div>
              </button>

              <button
                @click="manageSocial"
                class="w-full text-left px-4 py-3 border border-gray-200 dark:border-gray-600 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
              >
                <div class="flex items-center">
                  <svg class="w-5 h-5 text-yellow-500 mr-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 4V2a1 1 0 011-1h8a1 1 0 011 1v2m0 0V3a1 1 0 011 1v1.586a1 1 0 01-.293.707l-6.414 6.414a1 1 0 01-.586.293H6.5A1.5 1.5 0 015 10.5v-4.793A1 1 0 015.293 5L11.707.293A1 1 0 0112 0h0a1 1 0 01.707.293L18.707 6a1 1 0 01.293.707V7a1 1 0 01-1 1H16v10a1 1 0 01-1 1H9a1 1 0 01-1-1V8z" />
                  </svg>
                  <div>
                    <div class="text-sm font-medium text-gray-900 dark:text-white">Social Media</div>
                    <div class="text-xs text-gray-500 dark:text-gray-400">Manage social presence</div>
                  </div>
                </div>
              </button>
            </div>
          </div>

          <!-- Channel Performance -->
          <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-4">Channel Performance</h3>
            <div class="space-y-3">
              <div v-for="channel in channels" :key="channel.name" class="flex items-center justify-between">
                <div class="flex items-center">
                  <div class="w-3 h-3 rounded-full mr-3" :class="channel.color"></div>
                  <span class="text-sm text-gray-900 dark:text-white">{{ channel.name }}</span>
                </div>
                <div class="text-right">
                  <div class="text-sm font-medium text-gray-900 dark:text-white">{{ channel.performance }}%</div>
                  <div class="text-xs text-gray-500 dark:text-gray-400">{{ formatCurrency(channel.revenue) }}</div>
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

          <!-- Budget Overview -->
          <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-4">Budget Overview</h3>
            <div class="space-y-4">
              <div>
                <div class="flex justify-between text-sm mb-1">
                  <span class="text-gray-600 dark:text-gray-400">Monthly Budget</span>
                  <span class="text-gray-900 dark:text-white">75%</span>
                </div>
                <div class="w-full bg-gray-200 rounded-full h-2">
                  <div class="bg-green-600 h-2 rounded-full" style="width: 75%"></div>
                </div>
                <div class="flex justify-between text-xs text-gray-500 dark:text-gray-400 mt-1">
                  <span>{{ formatCurrency(monthlySpent) }} spent</span>
                  <span>{{ formatCurrency(monthlyBudget) }} total</span>
                </div>
              </div>
              
              <div>
                <div class="flex justify-between text-sm mb-1">
                  <span class="text-gray-600 dark:text-gray-400">Quarterly Budget</span>
                  <span class="text-gray-900 dark:text-white">62%</span>
                </div>
                <div class="w-full bg-gray-200 rounded-full h-2">
                  <div class="bg-blue-600 h-2 rounded-full" style="width: 62%"></div>
                </div>
                <div class="flex justify-between text-xs text-gray-500 dark:text-gray-400 mt-1">
                  <span>{{ formatCurrency(quarterlySpent) }} spent</span>
                  <span>{{ formatCurrency(quarterlyBudget) }} total</span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- New Campaign Modal -->
    <div v-if="showNewCampaignModal" class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
      <div class="relative top-20 mx-auto p-5 border w-96 shadow-lg rounded-md bg-white dark:bg-gray-800">
        <div class="mt-3">
          <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-4">New Marketing Campaign</h3>
          <form @submit.prevent="createCampaign" class="space-y-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300">Campaign Name</label>
              <input
                v-model="newCampaign.name"
                type="text"
                required
                class="mt-1 block w-full border border-gray-300 dark:border-gray-600 rounded-md px-3 py-2 text-sm focus:ring-orange-500 focus:border-orange-500 dark:bg-gray-700 dark:text-white"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300">Description</label>
              <textarea
                v-model="newCampaign.description"
                rows="2"
                class="mt-1 block w-full border border-gray-300 dark:border-gray-600 rounded-md px-3 py-2 text-sm focus:ring-orange-500 focus:border-orange-500 dark:bg-gray-700 dark:text-white"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300">Campaign Type</label>
              <select
                v-model="newCampaign.type"
                required
                class="mt-1 block w-full border border-gray-300 dark:border-gray-600 rounded-md px-3 py-2 text-sm focus:ring-orange-500 focus:border-orange-500 dark:bg-gray-700 dark:text-white"
              >
                <option value="">Select Type</option>
                <option value="email">Email</option>
                <option value="social">Social Media</option>
                <option value="print">Print</option>
                <option value="radio">Radio</option>
                <option value="digital">Digital Ads</option>
              </select>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300">Budget</label>
              <input
                v-model.number="newCampaign.budget"
                type="number"
                required
                class="mt-1 block w-full border border-gray-300 dark:border-gray-600 rounded-md px-3 py-2 text-sm focus:ring-orange-500 focus:border-orange-500 dark:bg-gray-700 dark:text-white"
              />
            </div>
            <div class="flex justify-end space-x-3 pt-4">
              <button
                type="button"
                @click="showNewCampaignModal = false"
                class="px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-md text-sm font-medium text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-700"
              >
                Cancel
              </button>
              <button
                type="submit"
                class="px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-orange-600 hover:bg-orange-700"
              >
                Create Campaign
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
interface Campaign {
  id: string
  name: string
  description: string
  type: 'email' | 'social' | 'print' | 'radio' | 'digital'
  status: 'active' | 'paused' | 'completed' | 'draft'
  budget: number
  performance: number
}

interface CustomerSegment {
  id: string
  name: string
  description: string
  size: number
  engagement: number
  icon: string
  color: string
}

interface Channel {
  name: string
  performance: number
  revenue: number
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
const activeCampaigns = ref(8)
const conversionRate = ref(12.5)
const totalReach = ref(24680)
const marketingROI = ref(240)
const impressions = ref(156780)
const clicks = ref(8942)
const conversions = ref(1124)

const selectedCampaignType = ref('')
const showNewCampaignModal = ref(false)

// Budget data
const monthlyBudget = ref(25000)
const monthlySpent = ref(18750)
const quarterlyBudget = ref(75000)
const quarterlySpent = ref(46500)

// New campaign form
const newCampaign = ref({
  name: '',
  description: '',
  type: '',
  budget: 0
})

// Sample campaigns
const campaigns = ref<Campaign[]>([
  {
    id: '1',
    name: 'Spring Sale 2024',
    description: 'Seasonal promotion for kitchen essentials',
    type: 'email',
    status: 'active',
    budget: 5000,
    performance: 87
  },
  {
    id: '2',
    name: 'Social Media Boost',
    description: 'Facebook and Instagram advertising',
    type: 'social',
    status: 'active',
    budget: 8000,
    performance: 92
  },
  {
    id: '3',
    name: 'Local Radio Ads',
    description: 'Radio spots for weekend promotions',
    type: 'radio',
    status: 'active',
    budget: 3000,
    performance: 76
  },
  {
    id: '4',
    name: 'Community Newsletter',
    description: 'Monthly print newsletter distribution',
    type: 'print',
    status: 'completed',
    budget: 1500,
    performance: 65
  },
  {
    id: '5',
    name: 'Google Ads Campaign',
    description: 'Search and display advertising',
    type: 'digital',
    status: 'paused',
    budget: 12000,
    performance: 78
  }
])

// Customer segments
const customerSegments = ref<CustomerSegment[]>([
  {
    id: '1',
    name: 'Regular Customers',
    description: 'Weekly shoppers with high loyalty',
    size: 2450,
    engagement: 89,
    icon: 'M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z',
    color: 'bg-green-500'
  },
  {
    id: '2',
    name: 'New Customers',
    description: 'Recent customers within 3 months',
    size: 1580,
    engagement: 67,
    icon: 'M18 9v3m0 0v3m0-3h3m-3 0h-3m-2-5a4 4 0 11-8 0 4 4 0 018 0zM3 20a6 6 0 0112 0v1H3v-1z',
    color: 'bg-blue-500'
  },
  {
    id: '3',
    name: 'Business Customers',
    description: 'Small businesses and restaurants',
    size: 340,
    engagement: 94,
    icon: 'M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4',
    color: 'bg-purple-500'
  },
  {
    id: '4',
    name: 'Price-Sensitive',
    description: 'Customers focused on deals and discounts',
    size: 3120,
    engagement: 72,
    icon: 'M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z',
    color: 'bg-yellow-500'
  }
])

// Marketing channels
const channels = ref<Channel[]>([
  { name: 'Email Marketing', performance: 92, revenue: 12450, color: 'bg-blue-500' },
  { name: 'Social Media', performance: 85, revenue: 8930, color: 'bg-green-500' },
  { name: 'Digital Ads', performance: 78, revenue: 15680, color: 'bg-purple-500' },
  { name: 'Print Media', performance: 65, revenue: 4200, color: 'bg-yellow-500' },
  { name: 'Radio', performance: 71, revenue: 6800, color: 'bg-red-500' }
])

// Recent activities
const recentActivities = ref<Activity[]>([
  {
    id: '1',
    description: 'Spring Sale campaign launched successfully',
    timestamp: new Date(Date.now() - 1800000),
    icon: 'M15 12a3 3 0 11-6 0 3 3 0 016 0z',
    color: 'bg-green-500'
  },
  {
    id: '2',
    description: 'Google Ads campaign paused for budget review',
    timestamp: new Date(Date.now() - 3600000),
    icon: 'M10 9v6m4-6v6m7-3a9 9 0 11-18 0 9 9 0 0118 0z',
    color: 'bg-yellow-500'
  },
  {
    id: '3',
    description: 'New customer segment analysis completed',
    timestamp: new Date(Date.now() - 5400000),
    icon: 'M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z',
    color: 'bg-blue-500'
  }
])

// Computed properties
const filteredCampaigns = computed(() => {
  return campaigns.value.filter(campaign => {
    return selectedCampaignType.value === '' || campaign.type === selectedCampaignType.value
  })
})

// Methods
const formatCurrency = (amount: number): string => {
  return new Intl.NumberFormat('en-ZA', {
    style: 'currency',
    currency: 'ZAR',
    minimumFractionDigits: 0,
    maximumFractionDigits: 0
  }).format(amount)
}

const formatNumber = (num: number): string => {
  return new Intl.NumberFormat('en-ZA').format(num)
}

const formatTime = (date: Date): string => {
  return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
}

const getCampaignTypeColor = (type: string): string => {
  switch (type) {
    case 'email': return 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-300'
    case 'social': return 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-300'
    case 'print': return 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-300'
    case 'radio': return 'bg-purple-100 text-purple-800 dark:bg-purple-900 dark:text-purple-300'
    case 'digital': return 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-300'
    default: return 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300'
  }
}

const getStatusColor = (status: string): string => {
  switch (status) {
    case 'active': return 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-300'
    case 'paused': return 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-300'
    case 'completed': return 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-300'
    case 'draft': return 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300'
    default: return 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300'
  }
}

const createCampaign = () => {
  const campaign: Campaign = {
    id: Date.now().toString(),
    name: newCampaign.value.name,
    description: newCampaign.value.description,
    type: newCampaign.value.type as Campaign['type'],
    status: 'draft',
    budget: newCampaign.value.budget,
    performance: 0
  }
  
  campaigns.value.push(campaign)
  showNewCampaignModal.value = false
  
  // Reset form
  newCampaign.value = {
    name: '',
    description: '',
    type: '',
    budget: 0
  }
}

const viewCampaign = (campaign: Campaign) => {
  console.log('Viewing campaign:', campaign.name)
  // Implement view functionality
}

const editCampaign = (campaign: Campaign) => {
  console.log('Editing campaign:', campaign.name)
  // Implement edit functionality
}

const createEmail = () => {
  console.log('Creating email campaign...')
  // Implement email campaign creation
}

const schedulePromotion = () => {
  console.log('Scheduling promotion...')
  // Implement promotion scheduling
}

const analyzeTrends = () => {
  console.log('Analyzing market trends...')
  // Implement trend analysis
}

const manageSocial = () => {
  console.log('Managing social media...')
  // Implement social media management
}
</script>

<style scoped>
/* Component-specific styles */
</style>
