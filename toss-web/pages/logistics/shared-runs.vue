<template>
  <div class="min-h-screen bg-gradient-to-br from-slate-50 via-green-50/30 to-slate-100 dark:from-slate-900 dark:via-slate-900 dark:to-slate-800">
    <!-- Page Header -->
    <div class="bg-white/80 dark:bg-slate-800/80 backdrop-blur-xl shadow-sm border-b border-slate-200/50 dark:border-slate-700/50 sticky top-0 z-10">
      <div class="w-full max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4 sm:py-6">
        <div class="flex items-center justify-between">
          <div class="flex-1 min-w-0">
            <h1 class="text-2xl sm:text-3xl font-bold bg-gradient-to-r from-green-600 to-blue-600 bg-clip-text text-transparent">
              Shared Delivery Runs
            </h1>
            <p class="mt-1 text-sm text-slate-600 dark:text-slate-400">
              💰 Share drivers, split costs, save money — coordinate deliveries together
            </p>
          </div>
          <div class="flex space-x-2 sm:space-x-3 flex-shrink-0">
            <button
              @click="showCreateRunModal = true"
              class="inline-flex items-center justify-center px-4 sm:px-6 py-2.5 sm:py-3 bg-gradient-to-r from-green-600 to-blue-600 text-white rounded-xl hover:from-green-700 hover:to-blue-700 shadow-lg hover:shadow-xl transition-all duration-200 transform hover:scale-105 font-semibold text-sm sm:text-base"
            >
              <TruckIcon class="w-5 h-5 mr-2" />
              Create Run
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Main Content -->
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <!-- Stats -->
      <div class="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
        <div class="bg-white dark:bg-slate-800 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700 p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400">Active Runs</p>
              <p class="mt-2 text-3xl font-bold text-slate-900 dark:text-white">{{ stats.activeRuns }}</p>
            </div>
            <div class="p-3 bg-blue-100 dark:bg-blue-900/30 rounded-lg">
              <TruckIcon class="w-8 h-8 text-blue-600 dark:text-blue-400" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700 p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400">Total Savings</p>
              <p class="mt-2 text-3xl font-bold text-green-600 dark:text-green-400">{{ formatCurrency(stats.totalSavings) }}</p>
            </div>
            <div class="p-3 bg-green-100 dark:bg-green-900/30 rounded-lg">
              <CurrencyDollarIcon class="w-8 h-8 text-green-600 dark:text-green-400" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700 p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400">Scheduled</p>
              <p class="mt-2 text-3xl font-bold text-slate-900 dark:text-white">{{ stats.scheduled }}</p>
            </div>
            <div class="p-3 bg-yellow-100 dark:bg-yellow-900/30 rounded-lg">
              <ClockIcon class="w-8 h-8 text-yellow-600 dark:text-yellow-400" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700 p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400">Completed</p>
              <p class="mt-2 text-3xl font-bold text-slate-900 dark:text-white">{{ stats.completed }}</p>
            </div>
            <div class="p-3 bg-purple-100 dark:bg-purple-900/30 rounded-lg">
              <CheckCircleIcon class="w-8 h-8 text-purple-600 dark:text-purple-400" />
            </div>
          </div>
        </div>
      </div>

      <!-- AI Copilot Banner -->
      <div class="mb-6 bg-gradient-to-r from-blue-500 via-green-500 to-emerald-500 rounded-2xl shadow-2xl p-6 text-white relative overflow-hidden">
        <div class="absolute top-0 right-0 w-64 h-64 bg-white/10 rounded-full -mr-32 -mt-32"></div>
        <div class="relative z-10 flex items-start gap-4">
          <div class="p-3 bg-white/20 backdrop-blur-sm rounded-xl flex-shrink-0">
            <SparklesIcon class="w-6 h-6" />
          </div>
          <div class="flex-1">
            <h3 class="text-lg font-bold mb-2">🚚 Shared Run Opportunity</h3>
            <p class="text-white/90 text-sm leading-relaxed">
              3 shops nearby have deliveries from <strong>Metro Cash & Carry</strong> scheduled for tomorrow. Join the shared run to save <strong>R120 (60%)</strong> on delivery fees!
            </p>
            <button class="mt-4 px-5 py-2.5 bg-white/20 hover:bg-white/30 backdrop-blur-sm rounded-lg text-sm font-medium transition-all duration-200">
              Join Shared Run →
            </button>
          </div>
        </div>
      </div>

      <!-- Runs List -->
      <div class="space-y-4">
        <div v-for="run in runs" :key="run.id" 
          class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 overflow-hidden hover:shadow-xl transition-all duration-300"
        >
          <!-- Run Header -->
          <div 
            @click="toggleRunExpansion(run.id)"
            class="bg-gradient-to-r from-green-50 to-blue-50 dark:from-green-900/20 dark:to-blue-900/20 px-6 py-4 border-b border-slate-200 dark:border-slate-600 cursor-pointer hover:from-green-100 hover:to-blue-100 dark:hover:from-green-900/30 dark:hover:to-blue-900/30 transition-colors"
          >
            <div class="flex items-center justify-between">
              <div class="flex items-center space-x-4">
                <div class="flex-shrink-0 h-14 w-14">
                  <div class="h-14 w-14 rounded-xl bg-gradient-to-br from-green-500 to-blue-600 flex items-center justify-center">
                    <TruckIcon class="w-8 h-8 text-white" />
                  </div>
                </div>
                <div>
                  <h3 class="text-xl font-bold text-slate-900 dark:text-white">Run #{{ run.runNumber }}</h3>
                  <p class="text-sm text-slate-600 dark:text-slate-400">Driver: {{ run.driverName }} • {{ run.stops.length }} stops</p>
                </div>
              </div>
              <div class="flex items-center space-x-3">
                <span 
                  class="px-4 py-2 rounded-full text-sm font-medium"
                  :class="getStatusBadge(run.status)"
                >
                  {{ getStatusLabel(run.status) }}
                </span>
                <div class="text-right">
                  <p class="text-2xl font-bold text-slate-900 dark:text-white">R{{ run.baseFee.toFixed(2) }}</p>
                  <p class="text-xs text-slate-500 dark:text-slate-400">
                    {{ expandedRuns.includes(run.id) ? '▲ Collapse' : '▼ Expand' }}
                  </p>
                </div>
              </div>
            </div>
          </div>

          <!-- Run Details -->
          <div class="px-6 py-4">
            <div class="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
              <div class="bg-slate-50 dark:bg-slate-700/50 rounded-xl p-4">
                <p class="text-xs text-slate-500 dark:text-slate-400 mb-1">Pickup</p>
                <p class="text-sm font-bold text-slate-900 dark:text-white">{{ run.pickupLocation }}</p>
                <p class="text-xs text-slate-600 dark:text-slate-400">{{ formatTime(run.pickupTime) }}</p>
              </div>
              <div class="bg-slate-50 dark:bg-slate-700/50 rounded-xl p-4">
                <p class="text-xs text-slate-500 dark:text-slate-400 mb-1">Distance</p>
                <p class="text-sm font-bold text-slate-900 dark:text-white">{{ run.totalDistance }} km</p>
                <p class="text-xs text-slate-600 dark:text-slate-400">Estimated 45 mins</p>
              </div>
              <div class="bg-slate-50 dark:bg-slate-700/50 rounded-xl p-4">
                <p class="text-xs text-slate-500 dark:text-slate-400 mb-1">Your Share</p>
                <p class="text-sm font-bold text-slate-900 dark:text-white">R{{ run.myShare.toFixed(2) }}</p>
                <p class="text-xs text-green-600 dark:text-green-400">Save R{{ run.mySavings.toFixed(2) }}</p>
              </div>
              <div class="bg-slate-50 dark:bg-slate-700/50 rounded-xl p-4">
                <p class="text-xs text-slate-500 dark:text-slate-400 mb-1">Split Rule</p>
                <p class="text-sm font-bold text-slate-900 dark:text-white">{{ run.splitRule }}</p>
                <p class="text-xs text-slate-600 dark:text-slate-400">{{ run.stops.length }} participants</p>
              </div>
            </div>

            <!-- Expandable Drop List -->
            <transition 
              enter-active-class="transition-all duration-300 ease-out"
              enter-from-class="max-h-0 opacity-0"
              enter-to-class="max-h-[1000px] opacity-100"
              leave-active-class="transition-all duration-200 ease-in"
              leave-from-class="max-h-[1000px] opacity-100"
              leave-to-class="max-h-0 opacity-0"
            >
              <div v-if="expandedRuns.includes(run.id)" class="overflow-hidden mb-6">
                <div class="border-t border-slate-200 dark:border-slate-700 pt-6">
                  <h4 class="text-sm font-bold text-slate-900 dark:text-white mb-4 flex items-center">
                    <MapPinIcon class="w-5 h-5 mr-2 text-green-600 dark:text-green-400" />
                    Delivery Stops
                  </h4>
                  <div class="space-y-3">
                    <div 
                      v-for="(stop, index) in run.stops" 
                      :key="stop.id"
                      class="flex items-start gap-4 p-4 bg-slate-50 dark:bg-slate-700/50 rounded-xl"
                    >
                      <div class="flex-shrink-0">
                        <div class="w-10 h-10 rounded-full bg-gradient-to-br from-green-500 to-blue-600 flex items-center justify-center text-white font-bold">
                          {{ index + 1 }}
                        </div>
                      </div>
                      <div class="flex-1">
                        <div class="flex items-center justify-between mb-2">
                          <h5 class="font-semibold text-slate-900 dark:text-white">{{ stop.shopName }}</h5>
                          <span 
                            v-if="stop.proofOfDelivery"
                            class="px-3 py-1 bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400 rounded-full text-xs font-medium"
                          >
                            ✓ Delivered
                          </span>
                          <span 
                            v-else-if="run.status === 'en-route'"
                            class="px-3 py-1 bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400 rounded-full text-xs font-medium"
                          >
                            En Route
                          </span>
                          <span 
                            v-else
                            class="px-3 py-1 bg-slate-100 text-slate-800 dark:bg-slate-900/30 dark:text-slate-400 rounded-full text-xs font-medium"
                          >
                            Pending
                          </span>
                        </div>
                        <p class="text-sm text-slate-600 dark:text-slate-400 mb-2">{{ stop.address }}</p>
                        <div class="flex items-center justify-between text-xs">
                          <span class="text-slate-500 dark:text-slate-400">ETA: {{ formatTime(stop.eta) }}</span>
                          <span class="font-medium text-slate-900 dark:text-white">Fee Share: R{{ stop.feeShare.toFixed(2) }}</span>
                        </div>
                        <div v-if="stop.proofOfDelivery" class="mt-3 p-3 bg-green-50 dark:bg-green-900/20 rounded-lg">
                          <p class="text-xs text-green-800 dark:text-green-400 mb-2">
                            <strong>Proof of Delivery:</strong> {{ stop.proofOfDelivery.type }}
                          </p>
                          <p class="text-xs text-green-700 dark:text-green-500">
                            Delivered at {{ formatTime(stop.proofOfDelivery.timestamp) }} by {{ run.driverName }}
                          </p>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </transition>

            <!-- Actions -->
            <div class="flex items-center justify-between pt-4 border-t border-slate-200 dark:border-slate-700">
              <div class="flex space-x-3">
                <button 
                  v-if="run.status === 'scheduled' && !run.isJoined"
                  @click="joinRun(run)"
                  class="px-6 py-3 bg-gradient-to-r from-green-600 to-blue-600 hover:from-green-700 hover:to-blue-700 text-white rounded-xl font-semibold shadow-lg hover:shadow-xl transition-all duration-200"
                >
                  Join Run - Save R{{ run.mySavings.toFixed(2) }}
                </button>
                <button 
                  v-if="run.isJoined"
                  class="px-6 py-3 bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400 rounded-xl font-semibold"
                  disabled
                >
                  ✓ Joined
                </button>
                <button 
                  @click="shareRun(run)"
                  class="px-6 py-3 bg-blue-600 hover:bg-blue-700 text-white rounded-xl font-semibold transition-colors"
                >
                  <ShareIcon class="w-5 h-5 inline mr-2" />
                  Share on WhatsApp
                </button>
              </div>
              <button 
                v-if="run.isCreator && run.status === 'scheduled'"
                @click="cancelRun(run)"
                class="text-red-600 hover:text-red-800 dark:text-red-400 dark:hover:text-red-200 text-sm font-medium"
              >
                Cancel Run
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Empty State -->
      <div v-if="runs.length === 0" class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-12 text-center">
        <TruckIcon class="w-16 h-16 text-slate-400 mx-auto mb-4" />
        <p class="text-lg font-semibold text-slate-900 dark:text-white mb-2">No delivery runs yet</p>
        <p class="text-slate-600 dark:text-slate-400 mb-6">Create a shared run and invite nearby shops to split delivery costs!</p>
        <button 
          @click="showCreateRunModal = true"
          class="inline-flex items-center px-6 py-3 bg-gradient-to-r from-green-600 to-blue-600 text-white rounded-xl hover:from-green-700 hover:to-blue-700 shadow-lg hover:shadow-xl transition-all duration-200 font-semibold"
        >
          <TruckIcon class="w-5 h-5 mr-2" />
          Create Shared Run
        </button>
      </div>
    </div>

    <!-- Create Run Modal -->
    <CreateRunModal
      :show="showCreateRunModal"
      @close="showCreateRunModal = false"
      @create="createRun"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import {
  TruckIcon,
  CurrencyDollarIcon,
  ClockIcon,
  CheckCircleIcon,
  SparklesIcon,
  ShareIcon,
  MapPinIcon
} from '@heroicons/vue/24/outline'
import CreateRunModal from '~/components/logistics/CreateRunModal.vue'

definePageMeta({
  layout: 'default'
})

useHead({
  title: 'Shared Delivery Runs - TOSS ERP',
  meta: [
    { name: 'description', content: 'Coordinate deliveries and save on shipping costs' }
  ]
})

// State
const showCreateRunModal = ref(false)
const expandedRuns = ref<string[]>([])

// Mock data with enhanced details
const stats = ref({
  activeRuns: 5,
  totalSavings: 12450,
  scheduled: 8,
  completed: 124
})

const runs = ref([
  {
    id: '1',
    runNumber: 'DR-2025-001',
    driverName: 'Thabo Molefe',
    status: 'scheduled',
    pickupLocation: 'Metro Cash & Carry, Soweto',
    pickupTime: new Date(Date.now() + 86400000 + 9 * 3600000), // Tomorrow 9 AM
    totalDistance: 45,
    baseFee: 850,
    splitRule: 'By Stops',
    myShare: 283.33,
    mySavings: 120.00,
    isJoined: true,
    isCreator: false,
    stops: [
      { 
        id: '1',
        shopName: "Thabo's Spaza",
        address: '123 Main St, Soweto',
        eta: new Date(Date.now() + 86400000 + 9.5 * 3600000),
        feeShare: 283.33,
        proofOfDelivery: null
      },
      { 
        id: '2',
        shopName: "Mama Grace Shop",
        address: '456 Church Rd, Soweto',
        eta: new Date(Date.now() + 86400000 + 10 * 3600000),
        feeShare: 283.33,
        proofOfDelivery: null
      },
      { 
        id: '3',
        shopName: "Lucky's Store",
        address: '789 Market Ave, Soweto',
        eta: new Date(Date.now() + 86400000 + 10.5 * 3600000),
        feeShare: 283.34,
        proofOfDelivery: null
      }
    ]
  },
  {
    id: '2',
    runNumber: 'DR-2025-002',
    driverName: 'Sarah Ndlovu',
    status: 'en-route',
    pickupLocation: 'Makro Warehouse, Alexandra',
    pickupTime: new Date(Date.now() - 2 * 3600000), // 2 hours ago
    totalDistance: 28,
    baseFee: 620,
    splitRule: 'Equal Split',
    myShare: 310.00,
    mySavings: 85.00,
    isJoined: true,
    isCreator: true,
    stops: [
      { 
        id: '4',
        shopName: "Quick Shop",
        address: '101 High St, Alexandra',
        eta: new Date(Date.now() - 1.5 * 3600000),
        feeShare: 310.00,
        proofOfDelivery: {
          type: 'PIN Verified',
          timestamp: new Date(Date.now() - 1.5 * 3600000),
          signature: null
        }
      },
      { 
        id: '5',
        shopName: "City Spaza",
        address: '202 Valley Rd, Alexandra',
        eta: new Date(Date.now() - 0.5 * 3600000),
        feeShare: 310.00,
        proofOfDelivery: null
      }
    ]
  }
])

// Methods
const formatCurrency = (amount: number): string => {
  return new Intl.NumberFormat('en-ZA', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  }).format(amount)
}

const formatTime = (date: Date): string => {
  return new Date(date).toLocaleTimeString('en-ZA', {
    hour: '2-digit',
    minute: '2-digit'
  })
}

const getStatusLabel = (status: string): string => {
  const labels: Record<string, string> = {
    'scheduled': '📅 Scheduled',
    'en-route': '🚚 En Route',
    'completed': '✅ Completed',
    'cancelled': '❌ Cancelled'
  }
  return labels[status] || status
}

const getStatusBadge = (status: string): string => {
  const badges = {
    scheduled: 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-400',
    'en-route': 'bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400',
    completed: 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400',
    cancelled: 'bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400'
  }
  return badges[status as keyof typeof badges] || 'bg-slate-100 text-slate-800'
}

const toggleRunExpansion = (runId: string) => {
  const index = expandedRuns.value.indexOf(runId)
  if (index > -1) {
    expandedRuns.value.splice(index, 1)
  } else {
    expandedRuns.value.push(runId)
  }
}

const createRun = async (runData: any) => {
  try {
    // Implement API call to create run
    console.log('Creating run:', runData)
    showNotification('✓ Shared run created successfully!')
  } catch (error) {
    console.error('Failed to create run:', error)
    showNotification('✗ Failed to create run', 'error')
  }
}

const joinRun = async (run: any) => {
  try {
    // Implement API call to join run
    run.isJoined = true
    showNotification(`✓ Joined run! You'll save R${run.mySavings.toFixed(2)}`)
  } catch (error) {
    console.error('Failed to join run:', error)
    showNotification('✗ Failed to join run', 'error')
  }
}

const shareRun = (run: any) => {
  const inviteLink = `${window.location.origin}/logistics/shared-runs/join/${run.id}`
  const message = `🚚 Join our Shared Delivery Run!\n\nDriver: ${run.driverName}\nPickup: ${run.pickupLocation}\nTime: ${formatTime(run.pickupTime)}\nYour share: R${run.myShare.toFixed(2)}\n\nSave up to R${run.mySavings.toFixed(2)}!\n\nJoin now: ${inviteLink}`
  
  const whatsappUrl = `https://wa.me/?text=${encodeURIComponent(message)}`
  window.open(whatsappUrl, '_blank')
}

const cancelRun = async (run: any) => {
  if (confirm(`Are you sure you want to cancel this delivery run?`)) {
    try {
      // Implement API call to cancel run
      run.status = 'cancelled'
      showNotification('Run cancelled')
    } catch (error) {
      console.error('Failed to cancel run:', error)
      showNotification('✗ Failed to cancel run', 'error')
    }
  }
}

const showNotification = (message: string, type: 'success' | 'error' = 'success') => {
  const notification = document.createElement('div')
  notification.textContent = message
  notification.className = `fixed top-20 right-4 ${type === 'success' ? 'bg-green-600' : 'bg-red-600'} text-white px-4 py-2 rounded-lg shadow-lg z-50 animate-fade-in`
  document.body.appendChild(notification)
  setTimeout(() => notification.remove(), 3000)
}
</script>

