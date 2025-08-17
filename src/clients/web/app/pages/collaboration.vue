<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <!-- Page Header -->
    <div class="bg-white dark:bg-gray-800 shadow">
      <div class="max-w-7xl mx-auto py-6 px-4 sm:px-6 lg:px-8">
        <div class="md:flex md:items-center md:justify-between">
          <div class="flex-1 min-w-0">
            <h2 class="text-2xl font-bold leading-7 text-gray-900 dark:text-white sm:text-3xl sm:truncate">
              Business Collaboration
            </h2>
            <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
              Connect with other businesses for group buying, shared logistics, and community collaboration
            </p>
          </div>
          <div class="mt-4 flex md:mt-0 md:ml-4 space-x-3">
            <button
              @click="showJoinGroupModal = true"
              type="button"
              class="inline-flex items-center px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm text-sm font-medium text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600"
            >
              <svg class="-ml-1 mr-2 h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M18 9v3m0 0v3m0-3h3m-3 0h-3m-2-5a4 4 0 11-8 0 4 4 0 018 0zM3 20a6 6 0 0112 0v1H3v-1z" />
              </svg>
              Join Group
            </button>
            <button
              @click="showCreateGroupModal = true"
              type="button"
              class="inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700"
            >
              <svg class="-ml-1 mr-2 h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
              </svg>
              Create Group
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Collaboration Features -->
    <div class="max-w-7xl mx-auto py-6 px-4 sm:px-6 lg:px-8">
      <!-- Feature Cards -->
      <div class="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3 mb-8">
        <!-- Group Buying -->
        <div class="bg-white dark:bg-gray-800 overflow-hidden shadow rounded-lg">
          <div class="p-6">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <div class="w-8 h-8 bg-blue-500 rounded-md flex items-center justify-center">
                  <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z" />
                  </svg>
                </div>
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 dark:text-gray-400 truncate">
                    Active Group Purchases
                  </dt>
                  <dd class="text-lg font-medium text-gray-900 dark:text-white">
                    {{ activeGroupPurchases }}
                  </dd>
                </dl>
              </div>
            </div>
          </div>
          <div class="bg-gray-50 dark:bg-gray-700 px-6 py-3">
            <div class="text-sm">
              <button
                @click="showGroupBuying = true"
                class="font-medium text-blue-600 dark:text-blue-400 hover:text-blue-500 dark:hover:text-blue-300"
              >
                View all group purchases
              </button>
            </div>
          </div>
        </div>

        <!-- Shared Logistics -->
        <div class="bg-white dark:bg-gray-800 overflow-hidden shadow rounded-lg">
          <div class="p-6">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <div class="w-8 h-8 bg-green-500 rounded-md flex items-center justify-center">
                  <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7h12m0 0l-4-4m4 4l-4 4m0 6H4m0 0l4 4m-4-4l4-4" />
                  </svg>
                </div>
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 dark:text-gray-400 truncate">
                    Shared Deliveries
                  </dt>
                  <dd class="text-lg font-medium text-gray-900 dark:text-white">
                    {{ sharedDeliveries }}
                  </dd>
                </dl>
              </div>
            </div>
          </div>
          <div class="bg-gray-50 dark:bg-gray-700 px-6 py-3">
            <div class="text-sm">
              <button
                @click="showLogistics = true"
                class="font-medium text-green-600 dark:text-green-400 hover:text-green-500 dark:hover:text-green-300"
              >
                Manage logistics
              </button>
            </div>
          </div>
        </div>

        <!-- Community Network -->
        <div class="bg-white dark:bg-gray-800 overflow-hidden shadow rounded-lg">
          <div class="p-6">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <div class="w-8 h-8 bg-purple-500 rounded-md flex items-center justify-center">
                  <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" />
                  </svg>
                </div>
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 dark:text-gray-400 truncate">
                    Network Connections
                  </dt>
                  <dd class="text-lg font-medium text-gray-900 dark:text-white">
                    {{ networkConnections }}
                  </dd>
                </dl>
              </div>
            </div>
          </div>
          <div class="bg-gray-50 dark:bg-gray-700 px-6 py-3">
            <div class="text-sm">
              <button
                @click="showNetwork = true"
                class="font-medium text-purple-600 dark:text-purple-400 hover:text-purple-500 dark:hover:text-purple-300"
              >
                Expand network
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Main Content Grid -->
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <!-- Active Collaborations -->
        <div class="lg:col-span-2 bg-white dark:bg-gray-800 shadow rounded-lg">
          <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
            <div class="flex items-center justify-between">
              <h3 class="text-lg font-medium text-gray-900 dark:text-white">Active Collaborations</h3>
              <div class="flex space-x-2">
                <select
                  v-model="collaborationFilter"
                  class="text-sm border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-700 text-gray-900 dark:text-white rounded-md focus:ring-blue-500 focus:border-blue-500"
                >
                  <option value="all">All Types</option>
                  <option value="group-buying">Group Buying</option>
                  <option value="logistics">Logistics</option>
                  <option value="resource-sharing">Resource Sharing</option>
                </select>
              </div>
            </div>
          </div>
          <div class="p-6">
            <div v-if="loading" class="text-center py-4">
              <div class="inline-block animate-spin rounded-full h-6 w-6 border-b-2 border-blue-600"></div>
              <p class="mt-2 text-sm text-gray-500 dark:text-gray-400">Loading collaborations...</p>
            </div>
            <div v-else-if="error" class="text-center py-4">
              <div class="text-red-500 dark:text-red-400">
                <svg class="mx-auto h-12 w-12 mb-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.704-.833-2.464 0L4.35 16.5c-.77.833.192 2.5 1.732 2.5z" />
                </svg>
                <p class="text-sm">{{ error }}</p>
                <button @click="refreshData" class="mt-2 text-blue-600 hover:text-blue-700 text-sm underline">
                  Try again
                </button>
              </div>
            </div>
            <div v-else class="space-y-4">
              <div v-for="collaboration in filteredCollaborations" :key="collaboration.id" class="flex items-center justify-between p-4 border border-gray-200 dark:border-gray-600 rounded-lg">
                <div class="flex items-center space-x-4">
                  <div class="flex-shrink-0">
                    <div :class="[getCollaborationTypeClass(collaboration.type), 'w-8 h-8 rounded-full flex items-center justify-center']">
                      <svg v-if="collaboration.type === 'group-buying'" class="w-4 h-4 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z" />
                      </svg>
                      <svg v-else-if="collaboration.type === 'logistics'" class="w-4 h-4 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7h12m0 0l-4-4m4 4l-4 4m0 6H4m0 0l4 4m-4-4l4-4" />
                      </svg>
                      <svg v-else class="w-4 h-4 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8.684 13.342C8.886 12.938 9 12.482 9 12c0-.482-.114-.938-.316-1.342m0 2.684a3 3 0 110-2.684m0 2.684l6.632 3.316m-6.632-6l6.632-3.316m0 0a3 3 0 105.367-2.684 3 3 0 00-5.367 2.684zm0 9.316a3 3 0 105.368 2.684 3 3 0 00-5.368-2.684z" />
                      </svg>
                    </div>
                  </div>
                  <div>
                    <p class="text-sm font-medium text-gray-900 dark:text-white">
                      {{ collaboration.title }}
                    </p>
                    <p class="text-xs text-gray-500 dark:text-gray-400">
                      {{ collaboration.participants }} participants • {{ collaboration.status }}
                    </p>
                  </div>
                </div>
                <div class="text-right">
                  <p class="text-sm font-medium text-gray-900 dark:text-white">
                    {{ collaboration.savings ? `R ${formatCurrency(collaboration.savings)} saved` : collaboration.metric }}
                  </p>
                  <p class="text-xs text-gray-500 dark:text-gray-400">
                    {{ formatDate(collaboration.deadline) }}
                  </p>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Collaboration Tools & Opportunities -->
        <div class="space-y-6">
          <!-- Quick Actions -->
          <div class="bg-white dark:bg-gray-800 shadow rounded-lg">
            <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
              <h3 class="text-lg font-medium text-gray-900 dark:text-white">Quick Actions</h3>
            </div>
            <div class="p-6">
              <div class="space-y-3">
                <button
                  @click="showCreateGroupModal = true"
                  class="w-full flex items-center justify-between p-3 border border-gray-200 dark:border-gray-600 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
                >
                  <div class="flex items-center">
                    <svg class="w-5 h-5 text-blue-500 mr-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
                    </svg>
                    <span class="text-sm font-medium text-gray-900 dark:text-white">Start Group Purchase</span>
                  </div>
                  <svg class="w-4 h-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
                  </svg>
                </button>

                <button
                  @click="findLogisticsPartners"
                  class="w-full flex items-center justify-between p-3 border border-gray-200 dark:border-gray-600 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
                >
                  <div class="flex items-center">
                    <svg class="w-5 h-5 text-green-500 mr-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7h12m0 0l-4-4m4 4l-4 4m0 6H4m0 0l4 4m-4-4l4-4" />
                    </svg>
                    <span class="text-sm font-medium text-gray-900 dark:text-white">Find Delivery Partners</span>
                  </div>
                  <svg class="w-4 h-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
                  </svg>
                </button>

                <button
                  @click="shareResources"
                  class="w-full flex items-center justify-between p-3 border border-gray-200 dark:border-gray-600 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
                >
                  <div class="flex items-center">
                    <svg class="w-5 h-5 text-purple-500 mr-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8.684 13.342C8.886 12.938 9 12.482 9 12c0-.482-.114-.938-.316-1.342m0 2.684a3 3 0 110-2.684m0 2.684l6.632 3.316m-6.632-6l6.632-3.316m0 0a3 3 0 105.367-2.684 3 3 0 00-5.367 2.684zm0 9.316a3 3 0 105.368 2.684 3 3 0 00-5.368-2.684z" />
                    </svg>
                    <span class="text-sm font-medium text-gray-900 dark:text-white">Share Resources</span>
                  </div>
                  <svg class="w-4 h-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
                  </svg>
                </button>

                <button
                  @click="showJoinGroupModal = true"
                  class="w-full flex items-center justify-between p-3 border border-gray-200 dark:border-gray-600 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
                >
                  <div class="flex items-center">
                    <svg class="w-5 h-5 text-indigo-500 mr-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M18 9v3m0 0v3m0-3h3m-3 0h-3m-2-5a4 4 0 11-8 0 4 4 0 018 0zM3 20a6 6 0 0112 0v1H3v-1z" />
                    </svg>
                    <span class="text-sm font-medium text-gray-900 dark:text-white">Join Existing Group</span>
                  </div>
                  <svg class="w-4 h-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
                  </svg>
                </button>
              </div>
            </div>
          </div>

          <!-- Available Opportunities -->
          <div class="bg-white dark:bg-gray-800 shadow rounded-lg">
            <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
              <h3 class="text-lg font-medium text-gray-900 dark:text-white">Available Opportunities</h3>
              <p class="text-sm text-gray-500 dark:text-gray-400">Join existing collaborations</p>
            </div>
            <div class="p-6">
              <div class="space-y-4">
                <div v-for="opportunity in availableOpportunities" :key="opportunity.id" class="border border-gray-200 dark:border-gray-600 rounded-lg p-4">
                  <div class="flex items-start justify-between">
                    <div>
                      <h4 class="text-sm font-medium text-gray-900 dark:text-white">{{ opportunity.title }}</h4>
                      <p class="text-xs text-gray-500 dark:text-gray-400 mt-1">
                        {{ opportunity.description }}
                      </p>
                      <div class="flex items-center mt-2">
                        <span class="text-xs text-green-600 dark:text-green-400 font-medium">
                          {{ opportunity.savings }}% savings
                        </span>
                        <span class="mx-1 text-gray-300">•</span>
                        <span class="text-xs text-gray-500 dark:text-gray-400">
                          {{ opportunity.spotsLeft }} spots left
                        </span>
                      </div>
                    </div>
                    <button
                      @click="joinOpportunityHandler(opportunity)"
                      class="ml-4 inline-flex items-center px-3 py-1 border border-transparent text-xs font-medium rounded text-blue-700 bg-blue-100 hover:bg-blue-200 dark:bg-blue-900 dark:text-blue-300 dark:hover:bg-blue-800"
                    >
                      Join
                    </button>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Network Stats -->
          <div class="bg-white dark:bg-gray-800 shadow rounded-lg">
            <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
              <h3 class="text-lg font-medium text-gray-900 dark:text-white">Network Statistics</h3>
            </div>
            <div class="p-6">
              <div class="space-y-4">
                <div class="flex justify-between">
                  <span class="text-sm text-gray-500 dark:text-gray-400">Total Savings</span>
                  <span class="text-sm font-medium text-gray-900 dark:text-white">R {{ formatCurrency(totalSavings) }}</span>
                </div>
                <div class="flex justify-between">
                  <span class="text-sm text-gray-500 dark:text-gray-400">Collaborations Joined</span>
                  <span class="text-sm font-medium text-gray-900 dark:text-white">{{ collaborationsJoined }}</span>
                </div>
                <div class="flex justify-between">
                  <span class="text-sm text-gray-500 dark:text-gray-400">Network Trust Score</span>
                  <span class="text-sm font-medium text-green-600 dark:text-green-400">{{ trustScore }}/100</span>
                </div>
                <div class="flex justify-between">
                  <span class="text-sm text-gray-500 dark:text-gray-400">Active Partnerships</span>
                  <span class="text-sm font-medium text-gray-900 dark:text-white">{{ activePartnerships }}</span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Modals -->
    <!-- Create Group Modal -->
    <div
      v-show="showCreateGroupModal"
      class="fixed inset-0 bg-black bg-opacity-50 z-50 flex items-center justify-center p-4"
      @click.self="showCreateGroupModal = false"
    >
      <div class="bg-white dark:bg-gray-800 rounded-lg max-w-md w-full p-6">
        <div class="flex items-center justify-between mb-4">
          <h3 class="text-lg font-medium text-gray-900 dark:text-white">Create Collaboration Group</h3>
          <button
            @click="showCreateGroupModal = false"
            class="text-gray-400 hover:text-gray-600 dark:hover:text-gray-300"
          >
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>
        <p class="text-gray-500 dark:text-gray-400 mb-4">
          Group collaboration setup form will be implemented here.
        </p>
        <div class="flex justify-end space-x-3">
          <button
            @click="showCreateGroupModal = false"
            class="px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-md text-sm font-medium text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-700"
          >
            Cancel
          </button>
          <button
            class="px-4 py-2 bg-blue-600 text-white rounded-md text-sm font-medium hover:bg-blue-700"
          >
            Create Group
          </button>
        </div>
      </div>
    </div>

    <!-- Join Group Modal -->
    <div
      v-show="showJoinGroupModal"
      class="fixed inset-0 bg-black bg-opacity-50 z-50 flex items-center justify-center p-4"
      @click.self="showJoinGroupModal = false"
    >
      <div class="bg-white dark:bg-gray-800 rounded-lg max-w-md w-full p-6">
        <div class="flex items-center justify-between mb-4">
          <h3 class="text-lg font-medium text-gray-900 dark:text-white">Join Collaboration Group</h3>
          <button
            @click="showJoinGroupModal = false"
            class="text-gray-400 hover:text-gray-600 dark:hover:text-gray-300"
          >
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>
        <p class="text-gray-500 dark:text-gray-400 mb-4">
          Group joining interface will be implemented here.
        </p>
        <div class="flex justify-end space-x-3">
          <button
            @click="showJoinGroupModal = false"
            class="px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-md text-sm font-medium text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-700"
          >
            Cancel
          </button>
          <button
            class="px-4 py-2 bg-blue-600 text-white rounded-md text-sm font-medium hover:bg-blue-700"
          >
            Join Group
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
// Page meta
definePageMeta({
  title: 'Business Collaboration - TOSS ERP',
  description: 'Connect with other businesses for group buying, shared logistics, and community collaboration'
})

// Use collaboration management composable
const {
  collaborations,
  opportunities,
  connections,
  stats,
  loading,
  error,
  activeCollaborations,
  totalSavings,
  collaborationsJoined,
  trustScore,
  activePartnerships,
  networkConnections,
  availableOpportunities,
  loadStats,
  loadCollaborations,
  loadOpportunities,
  loadConnections,
  joinOpportunity,
  searchPartners,
  connectWithBusiness,
  formatCurrency,
  formatDate,
  getCollaborationTypeClass
} = useCollaborationManagement()

// Reactive data
const collaborationFilter = ref('all')
const showCreateGroupModal = ref(false)
const showJoinGroupModal = ref(false)
const showGroupBuying = ref(false)
const showLogistics = ref(false)
const showNetwork = ref(false)

// Computed for display
const activeGroupPurchases = computed(() => 
  collaborations.value.filter(c => c.type === 'group-buying' && c.status === 'active').length
)

const sharedDeliveries = computed(() => 
  collaborations.value.filter(c => c.type === 'logistics' && c.status === 'active').length
)

// Computed properties
const filteredCollaborations = computed(() => {
  if (collaborationFilter.value === 'all') {
    return collaborations.value
  }
  return collaborations.value.filter(c => c.type === collaborationFilter.value)
})

// Watch filter changes
watch(collaborationFilter, async (newFilter) => {
  if (newFilter === 'all') {
    await loadCollaborations()
  } else {
    await loadCollaborations({ 
      type: newFilter,
      limit: 10
    })
  }
})

// Methods
const findLogisticsPartners = async () => {
  try {
    const partners = await searchPartners({
      type: 'logistics',
      services: ['delivery', 'logistics', 'transport']
    })
    console.log('Found logistics partners:', partners)
    // TODO: Show partners in a modal or navigate to partners page
  } catch (err) {
    console.error('Failed to find logistics partners:', err)
  }
}

const shareResources = () => {
  console.log('Sharing resources...')
  // TODO: Implement resource sharing modal
}

const joinOpportunityHandler = async (opportunity: any) => {
  try {
    const success = await joinOpportunity(opportunity.id, {
      message: 'Interested in joining this collaboration'
    })
    
    if (success) {
      console.log('Successfully joined opportunity:', opportunity.title)
      // TODO: Show success message
    }
  } catch (err) {
    console.error('Failed to join opportunity:', err)
  }
}

const refreshData = async () => {
  await Promise.all([
    loadStats(),
    loadCollaborations({ limit: 10 }),
    loadOpportunities({ limit: 10 }),
    loadConnections({ limit: 10 })
  ])
}

// Load collaboration data on mount
onMounted(async () => {
  await refreshData()
})

// SEO
useHead({
  title: 'Business Collaboration - TOSS ERP',
  meta: [
    { name: 'description', content: 'Connect with other township businesses for group buying, shared logistics, resource sharing, and collaborative growth opportunities' }
  ]
})
</script>

<style scoped>
/* Component-specific styles */
.transition-colors {
  transition-property: color, background-color, border-color;
  transition-timing-function: cubic-bezier(0.4, 0, 0.2, 1);
  transition-duration: 200ms;
}

/* Hover effects */
.hover\:bg-gray-50:hover {
  background-color: #f9fafb;
}

.dark .hover\:bg-gray-700:hover {
  background-color: #374151;
}
</style>
