<template>
  <div class="min-h-screen bg-gradient-to-br from-slate-50 via-green-50/30 to-slate-100 dark:from-slate-900 dark:via-slate-900 dark:to-slate-800">
    <!-- Page Header with Glass Morphism -->
    <div class="bg-white/80 dark:bg-slate-800/80 backdrop-blur-xl shadow-sm border-b border-slate-200/50 dark:border-slate-700/50 sticky top-0 z-10">
      <div class="w-full max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4 sm:py-6">
        <div class="flex items-center justify-between">
          <div class="flex-1 min-w-0">
            <h1 class="text-2xl sm:text-3xl font-bold bg-gradient-to-r from-green-600 to-blue-600 bg-clip-text text-transparent">
              Group Buying
            </h1>
            <p class="mt-1 text-sm text-slate-600 dark:text-slate-400">
              Buy together with others to save money
            </p>
          </div>
          <div class="flex space-x-2 sm:space-x-3 flex-shrink-0">
            <button
              @click="showCreateModal = true"
              class="inline-flex items-center justify-center px-4 sm:px-6 py-2.5 sm:py-3 bg-gradient-to-r from-green-600 to-blue-600 text-white rounded-xl hover:from-green-700 hover:to-blue-700 shadow-lg hover:shadow-xl transition-all duration-200 transform hover:scale-105 font-semibold text-sm sm:text-base"
            >
              <PlusIcon class="w-5 h-5 mr-2" />
              Start Group Buy
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Main Content -->
    <div class="w-full max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      
      <!-- Network Stats -->
      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">Network Members</p>
              <p class="text-3xl font-bold text-slate-900 dark:text-white">{{ networkStats.totalMembers }}</p>
            </div>
            <div class="p-3 bg-gradient-to-br from-blue-500 to-purple-600 rounded-xl">
              <UserGroupIcon class="w-8 h-8 text-white" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">Active Groups</p>
              <p class="text-3xl font-bold text-slate-900 dark:text-white">{{ networkStats.activeGroupBuys }}</p>
            </div>
            <div class="p-3 bg-gradient-to-br from-green-500 to-emerald-600 rounded-xl">
              <ShoppingCartIcon class="w-8 h-8 text-white" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">Total Savings</p>
              <p class="text-3xl font-bold text-green-600">R{{ networkStats.totalSavings }}K</p>
            </div>
            <div class="p-3 bg-gradient-to-br from-yellow-500 to-orange-600 rounded-xl">
              <CurrencyDollarIcon class="w-8 h-8 text-white" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">Trust Score</p>
              <p class="text-3xl font-bold text-slate-900 dark:text-white">{{ networkStats.trustScore }}/5</p>
            </div>
            <div class="p-3 bg-gradient-to-br from-purple-500 to-pink-600 rounded-xl">
              <StarIcon class="w-8 h-8 text-white" />
            </div>
          </div>
        </div>
      </div>

      <!-- Group Buys Tabs -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <!-- My Group Buys -->
        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 overflow-hidden">
          <div class="bg-gradient-to-r from-blue-50 to-purple-50 dark:from-blue-900/20 dark:to-purple-900/20 px-6 py-4 border-b border-slate-200 dark:border-slate-600">
            <div class="flex items-center justify-between">
              <h3 class="text-lg font-bold text-slate-900 dark:text-white">My Active Group Buys</h3>
              <span class="bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400 px-3 py-1 rounded-full text-sm font-medium">
                {{ myGroupBuys.length }} active
              </span>
            </div>
          </div>
          
          <div class="p-6 space-y-4">
            <div v-for="group in myGroupBuys" :key="group.id" 
              class="border border-slate-200 dark:border-slate-600 rounded-xl p-4 hover:shadow-md transition-all duration-200"
            >
              <div class="flex items-center justify-between mb-3">
                <h4 class="font-semibold text-slate-900 dark:text-white">{{ group.title }}</h4>
                <span 
                  class="px-2 py-1 rounded-full text-xs font-medium"
                  :class="getStatusClass(group.status)"
                >
                  {{ group.status }}
                </span>
              </div>
              <p class="text-sm text-slate-600 dark:text-slate-400 mb-3">{{ group.description }}</p>
              
              <!-- Progress Bar -->
              <div class="mb-3">
                <div class="flex justify-between text-sm mb-2">
                  <span class="text-slate-600 dark:text-slate-400">Progress</span>
                  <span class="font-medium text-slate-900 dark:text-white">{{ group.committed }}/{{ group.minQuantity }} units</span>
                </div>
                <div class="w-full bg-slate-200 rounded-full h-2 dark:bg-slate-700">
                  <div 
                    class="bg-gradient-to-r from-blue-600 to-purple-600 h-2 rounded-full transition-all duration-300" 
                    :style="{ width: Math.min((group.committed / group.minQuantity * 100), 100) + '%' }"
                  ></div>
                </div>
              </div>

              <!-- Details -->
              <div class="grid grid-cols-2 gap-4 text-sm mb-3">
                <div>
                  <span class="text-slate-600 dark:text-slate-400">Members:</span>
                  <span class="font-medium ml-1 text-slate-900 dark:text-white">{{ group.memberCount }}</span>
                </div>
                <div>
                  <span class="text-slate-600 dark:text-slate-400">Savings:</span>
                  <span class="font-medium ml-1 text-green-600">R{{ group.estimatedSavings }}</span>
                </div>
                <div>
                  <span class="text-slate-600 dark:text-slate-400">Deadline:</span>
                  <span class="font-medium ml-1 text-slate-900 dark:text-white">{{ formatDate(group.deadline) }}</span>
                </div>
                <div class="flex items-center">
                  <span class="text-slate-600 dark:text-slate-400">Rating:</span>
                  <div class="flex ml-1">
                    <StarIcon v-for="i in 5" :key="i" class="w-3 h-3" :class="i <= group.trustLevel ? 'text-yellow-400' : 'text-slate-300'" />
                  </div>
                </div>
              </div>

              <!-- Actions -->
              <div class="flex space-x-2">
                <button 
                  @click="viewGroupDetails(group)" 
                  class="flex-1 px-3 py-2 bg-gradient-to-r from-blue-600 to-purple-600 text-white rounded-lg hover:from-blue-700 hover:to-purple-700 shadow-md hover:shadow-lg transition-all duration-200 text-sm font-medium"
                >
                  View Details
                </button>
                <button 
                  @click="inviteMembers(group)" 
                  class="px-3 py-2 border-2 border-slate-300 dark:border-slate-600 rounded-lg text-sm font-medium text-slate-700 dark:text-slate-300 hover:bg-slate-50 dark:hover:bg-slate-700 transition-all duration-200"
                >
                  Invite
                </button>
              </div>
            </div>
          </div>
        </div>

        <!-- Available Group Buys -->
        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 overflow-hidden">
          <div class="bg-gradient-to-r from-green-50 to-blue-50 dark:from-green-900/20 dark:to-blue-900/20 px-6 py-4 border-b border-slate-200 dark:border-slate-600">
            <div class="flex items-center justify-between">
              <h3 class="text-lg font-bold text-slate-900 dark:text-white">Available to Join</h3>
              <span class="bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400 px-3 py-1 rounded-full text-sm font-medium">
                {{ availableGroupBuys.length}} opportunities
              </span>
            </div>
          </div>
          
          <div class="p-6 space-y-4">
            <div v-for="group in availableGroupBuys" :key="group.id" 
              class="border border-slate-200 dark:border-slate-600 rounded-xl p-4 hover:shadow-md transition-all duration-200"
            >
              <div class="flex items-center justify-between mb-3">
                <h4 class="font-semibold text-slate-900 dark:text-white">{{ group.title }}</h4>
                <span class="px-2 py-1 rounded-full text-xs font-medium bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400">
                  Open
                </span>
              </div>
              <p class="text-sm text-slate-600 dark:text-slate-400 mb-3">{{ group.description }}</p>
              
              <!-- Lead Organization -->
              <div class="flex items-center mb-3 p-2 bg-slate-50 dark:bg-slate-700/50 rounded-lg">
                <div class="flex-shrink-0 h-8 w-8">
                  <div class="h-8 w-8 rounded-full bg-gradient-to-r from-blue-500 to-purple-600 flex items-center justify-center">
                    <span class="text-sm font-medium text-white">{{ group.leadOrganization.charAt(0) }}</span>
                  </div>
                </div>
                <div class="ml-3 flex-1">
                  <p class="text-sm font-medium text-slate-900 dark:text-white">{{ group.leadOrganization }}</p>
                  <div class="flex items-center">
                    <StarIcon v-for="i in 5" :key="i" class="w-3 h-3" :class="i <= group.leadTrustScore ? 'text-yellow-400' : 'text-slate-300'" />
                  </div>
                </div>
              </div>

              <!-- Join Details -->
              <div class="grid grid-cols-2 gap-3 text-sm mb-4">
                <div>
                  <span class="text-slate-600 dark:text-slate-400">Members:</span>
                  <span class="font-medium ml-1 text-slate-900 dark:text-white">{{ group.memberCount }}</span>
                </div>
                <div>
                  <span class="text-slate-600 dark:text-slate-400">Savings:</span>
                  <span class="font-medium ml-1 text-green-600">{{ group.savingsPercentage }}%</span>
                </div>
                <div>
                  <span class="text-slate-600 dark:text-slate-400">Min Qty:</span>
                  <span class="font-medium ml-1 text-slate-900 dark:text-white">{{ group.minCommitment }} units</span>
                </div>
                <div>
                  <span class="text-slate-600 dark:text-slate-400">Deadline:</span>
                  <span class="font-medium ml-1 text-slate-900 dark:text-white">{{ formatDate(group.deadline) }}</span>
                </div>
              </div>

              <!-- Actions -->
              <div class="flex space-x-2">
                <button 
                  @click="joinGroup(group)" 
                  class="flex-1 px-3 py-2 bg-gradient-to-r from-green-600 to-blue-600 text-white rounded-lg hover:from-green-700 hover:to-blue-700 shadow-md hover:shadow-lg transition-all duration-200 text-sm font-medium"
                >
                  Join Group
                </button>
                <button 
                  @click="viewGroupDetails(group)" 
                  class="px-3 py-2 border-2 border-slate-300 dark:border-slate-600 rounded-lg text-sm font-medium text-slate-700 dark:text-slate-300 hover:bg-slate-50 dark:hover:bg-slate-700 transition-all duration-200"
                >
                  Details
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Create Group Buy Modal (Placeholder) -->
    <div v-if="showCreateModal" class="fixed inset-0 bg-slate-600 bg-opacity-50 overflow-y-auto h-full w-full z-50 flex items-center justify-center p-4">
      <div class="relative bg-white dark:bg-slate-800 rounded-2xl shadow-2xl border border-slate-200 dark:border-slate-700 w-full max-w-2xl p-6">
        <div class="flex items-center justify-between mb-6">
          <h3 class="text-xl font-bold bg-gradient-to-r from-green-600 to-blue-600 bg-clip-text text-transparent">Start New Group Buy</h3>
          <button @click="showCreateModal = false" class="text-slate-400 hover:text-slate-600 dark:hover:text-slate-200">
            <XMarkIcon class="w-6 h-6" />
          </button>
        </div>
        
        <div class="text-center py-12">
          <p class="text-slate-600 dark:text-slate-400">Group buying creation form will be implemented here.</p>
          <p class="text-sm text-slate-500 dark:text-slate-500 mt-2">Collaborate with network partners for better pricing.</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import {
  PlusIcon,
  UserGroupIcon,
  ShoppingCartIcon,
  CurrencyDollarIcon,
  StarIcon,
  XMarkIcon
} from '@heroicons/vue/24/outline'

// Page metadata
useHead({
  title: 'Group Buying - TOSS ERP',
  meta: [
    { name: 'description', content: 'Collaborative procurement and group buying platform for TOSS network members' }
  ]
})

// State
const showCreateModal = ref(false)

// Network Statistics
const networkStats = ref({
  totalMembers: 1247,
  activeGroupBuys: 23,
  totalSavings: 485,
  trustScore: 4.3
})

// Mock data
const myGroupBuys = ref([
  {
    id: 1,
    title: 'Office Supplies Bulk Purchase',
    description: 'High-quality office supplies for Q2 2025',
    status: 'collecting',
    committed: 850,
    minQuantity: 1000,
    memberCount: 12,
    estimatedSavings: 1250,
    deadline: new Date('2025-09-15'),
    trustLevel: 4
  },
  {
    id: 2,
    title: 'Industrial Cleaning Equipment',
    description: 'Professional cleaning equipment for warehouses',
    status: 'negotiating',
    committed: 5,
    minQuantity: 8,
    memberCount: 5,
    estimatedSavings: 3200,
    deadline: new Date('2025-09-30'),
    trustLevel: 5
  }
])

const availableGroupBuys = ref([
  {
    id: 3,
    title: 'IT Hardware Upgrade Bundle',
    description: 'Enterprise-grade servers and networking equipment',
    leadOrganization: 'Tech Solutions Network',
    leadTrustScore: 4,
    memberCount: 8,
    savingsPercentage: 18,
    minCommitment: 2,
    deadline: new Date('2025-09-20')
  },
  {
    id: 4,
    title: 'Food Supplies - Bulk Order',
    description: 'Fresh produce and packaged goods for retail',
    leadOrganization: 'Fresh Market Co-op',
    leadTrustScore: 5,
    memberCount: 15,
    savingsPercentage: 25,
    minCommitment: 1,
    deadline: new Date('2025-10-01')
  }
])

// Methods
const getStatusClass = (status: string) => {
  const classes: Record<string, string> = {
    'collecting': 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-400',
    'negotiating': 'bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400',
    'confirmed': 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400',
    'completed': 'bg-slate-100 text-slate-800 dark:bg-slate-900/30 dark:text-slate-400'
  }
  return classes[status] || 'bg-slate-100 text-slate-800'
}

const formatDate = (date: Date) => {
  return date.toLocaleDateString('en-US', { 
    month: 'short', 
    day: 'numeric',
    year: 'numeric'
  })
}

const viewGroupDetails = (group: any) => {
  alert(`Viewing details for: ${group.title}`)
}

const inviteMembers = (group: any) => {
  alert(`Inviting members to: ${group.title}`)
}

const joinGroup = (group: any) => {
  alert(`Joining group: ${group.title}`)
}
</script>
