<template>
  <div class="min-h-screen bg-gradient-to-br from-slate-50 via-orange-50/30 to-slate-100 dark:from-slate-900 dark:via-slate-900 dark:to-slate-800">
    <!-- Page Header -->
    <div class="bg-white/80 dark:bg-slate-800/80 backdrop-blur-xl shadow-sm border-b border-slate-200/50 dark:border-slate-700/50 sticky top-0 z-10">
      <div class="w-full max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4 sm:py-6">
        <div class="flex items-center justify-between">
          <div class="flex-1 min-w-0">
            <h1 class="text-2xl sm:text-3xl font-bold bg-gradient-to-r from-orange-600 to-purple-600 bg-clip-text text-transparent">
              Group Buying Pools
            </h1>
            <p class="mt-1 text-sm text-slate-600 dark:text-slate-400">
              Buy together, save together — Join or create buying pools
            </p>
          </div>
          <div class="flex space-x-2 sm:space-x-3 flex-shrink-0">
            <button
              @click="showCreatePoolModal = true"
              class="inline-flex items-center justify-center px-4 sm:px-6 py-2.5 sm:py-3 bg-gradient-to-r from-orange-600 to-purple-600 text-white rounded-xl hover:from-orange-700 hover:to-purple-700 shadow-lg hover:shadow-xl transition-all duration-200 transform hover:scale-105 font-semibold text-sm sm:text-base"
            >
              <PlusIcon class="w-5 h-5 mr-2" />
              Create Pool
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Main Content -->
    <div class="w-full max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      
      <!-- AI Copilot Banner -->
      <div class="mb-6 bg-gradient-to-r from-blue-500 via-purple-500 to-pink-500 rounded-2xl shadow-2xl p-6 text-white relative overflow-hidden">
        <div class="absolute top-0 right-0 w-64 h-64 bg-white/10 rounded-full -mr-32 -mt-32"></div>
        <div class="relative z-10 flex items-start gap-4">
          <div class="p-3 bg-white/20 backdrop-blur-sm rounded-xl flex-shrink-0">
            <SparklesIcon class="w-6 h-6" />
          </div>
          <div class="flex-1">
            <h3 class="text-lg font-bold mb-2">💡 AI Savings Opportunity</h3>
            <p class="text-white/90 text-sm leading-relaxed">
              You're running low on <strong>Cooking Oil 750ml</strong>. Join Pool #12 with 3 nearby shops to save <strong>R85 (12%)</strong>. Pool closes in 4 hours.
            </p>
            <div class="flex gap-3 mt-4">
              <button 
                @click="quickJoinSuggestedPool"
                class="px-5 py-2.5 bg-white/20 hover:bg-white/30 backdrop-blur-sm rounded-lg text-sm font-medium transition-all duration-200"
              >
                Join Now →
              </button>
              <button class="px-5 py-2.5 bg-white/10 hover:bg-white/20 backdrop-blur-sm rounded-lg text-sm font-medium transition-all duration-200">
                Reorder Solo Instead
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Stats Cards -->
      <div class="grid grid-cols-2 sm:grid-cols-4 gap-4 sm:gap-6 mb-8">
        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">Active Pools</p>
              <p class="text-3xl font-bold text-slate-900 dark:text-white">{{ stats.activePools }}</p>
            </div>
            <div class="p-3 bg-gradient-to-br from-orange-500 to-amber-600 rounded-xl">
              <UserGroupIcon class="w-8 h-8 text-white" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">My Pools</p>
              <p class="text-3xl font-bold text-slate-900 dark:text-white">{{ stats.myPools }}</p>
            </div>
            <div class="p-3 bg-gradient-to-br from-blue-500 to-indigo-600 rounded-xl">
              <ShoppingBagIcon class="w-8 h-8 text-white" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">Total Saved</p>
              <p class="text-3xl font-bold text-slate-900 dark:text-white">R{{ stats.totalSaved }}</p>
            </div>
            <div class="p-3 bg-gradient-to-br from-green-500 to-emerald-600 rounded-xl">
              <CurrencyDollarIcon class="w-8 h-8 text-white" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">Avg Savings</p>
              <p class="text-3xl font-bold text-slate-900 dark:text-white">{{ stats.avgSavingsPercent }}%</p>
            </div>
            <div class="p-3 bg-gradient-to-br from-purple-500 to-pink-600 rounded-xl">
              <ChartBarIcon class="w-8 h-8 text-white" />
            </div>
          </div>
        </div>
      </div>

      <!-- Filters -->
      <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 mb-6">
        <div class="flex flex-col sm:flex-row gap-4">
          <div class="flex-1">
            <input
              v-model="searchQuery"
              type="text"
              placeholder="Search pools by product..."
              class="w-full px-4 py-2.5 border border-slate-300 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-orange-500 dark:bg-slate-700 dark:text-white"
            />
          </div>
          <select
            v-model="statusFilter"
            class="px-4 py-2.5 border border-slate-300 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-orange-500 dark:bg-slate-700 dark:text-white"
          >
            <option value="">All Status</option>
            <option value="open">Open</option>
            <option value="pending">Pending</option>
            <option value="confirmed">Confirmed</option>
            <option value="fulfilled">Fulfilled</option>
          </select>
          <select
            v-model="areaFilter"
            class="px-4 py-2.5 border border-slate-300 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-orange-500 dark:bg-slate-700 dark:text-white"
          >
            <option value="">All Areas</option>
            <option value="soweto">Soweto</option>
            <option value="alexandra">Alexandra</option>
            <option value="katlehong">Katlehong</option>
          </select>
        </div>
      </div>

      <!-- Active Pools -->
      <div class="space-y-4">
        <div v-for="pool in filteredPools" :key="pool.id" 
          class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 overflow-hidden hover:shadow-xl transition-all duration-300"
        >
          <!-- Pool Header -->
          <div class="bg-gradient-to-r from-orange-50 to-purple-50 dark:from-orange-900/20 dark:to-purple-900/20 px-6 py-4 border-b border-slate-200 dark:border-slate-600">
            <div class="flex items-center justify-between">
              <div class="flex items-center space-x-4">
                <div class="flex-shrink-0 h-14 w-14">
                  <div class="h-14 w-14 rounded-xl bg-gradient-to-br from-orange-500 to-purple-600 flex items-center justify-center">
                    <UserGroupIcon class="w-8 h-8 text-white" />
                  </div>
                </div>
                <div>
                  <h3 class="text-xl font-bold text-slate-900 dark:text-white">{{ pool.productName }}</h3>
                  <p class="text-sm text-slate-600 dark:text-slate-400">Pool #{{ pool.poolNumber }} • {{ pool.area }}</p>
                </div>
              </div>
              <div class="flex items-center space-x-3">
                <span 
                  class="px-4 py-2 rounded-full text-sm font-medium"
                  :class="getStatusBadge(pool.status)"
                >
                  {{ getStatusLabel(pool.status) }}
                </span>
              </div>
            </div>
          </div>

          <!-- Pool Body -->
          <div class="px-6 py-6">
            <!-- Progress Bar -->
            <div class="mb-6">
              <div class="flex justify-between text-sm mb-2">
                <span class="font-medium text-slate-900 dark:text-white">
                  Progress: {{ pool.currentQuantity }}/{{ pool.targetQuantity }} {{ pool.unit }}
                </span>
                <span class="text-slate-600 dark:text-slate-400">
                  {{ Math.round((pool.currentQuantity / pool.targetQuantity) * 100) }}%
                </span>
              </div>
              <div class="w-full bg-slate-200 dark:bg-slate-700 rounded-full h-4 overflow-hidden">
                <div 
                  class="bg-gradient-to-r from-orange-500 to-purple-600 h-4 rounded-full transition-all duration-500 flex items-center justify-end pr-2"
                  :style="{ width: `${Math.min((pool.currentQuantity / pool.targetQuantity) * 100, 100)}%` }"
                >
                  <span v-if="(pool.currentQuantity / pool.targetQuantity) >= 0.3" class="text-xs font-bold text-white">
                    {{ pool.participants }}/{{ pool.targetParticipants }} shops
                  </span>
                </div>
              </div>
            </div>

            <!-- Pool Details Grid -->
            <div class="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
              <div class="bg-slate-50 dark:bg-slate-700/50 rounded-xl p-4">
                <p class="text-xs text-slate-500 dark:text-slate-400 mb-1">Unit Price</p>
                <p class="text-lg font-bold text-slate-900 dark:text-white">R{{ pool.unitPrice.toFixed(2) }}</p>
                <p class="text-xs text-green-600 dark:text-green-400">-{{ pool.savingsPercent }}% vs solo</p>
              </div>
              <div class="bg-slate-50 dark:bg-slate-700/50 rounded-xl p-4">
                <p class="text-xs text-slate-500 dark:text-slate-400 mb-1">Your Share</p>
                <p class="text-lg font-bold text-slate-900 dark:text-white">
                  {{ pool.myQuantity || 0 }} {{ pool.unit }}
                </p>
                <p class="text-xs text-slate-600 dark:text-slate-400">
                  R{{ ((pool.myQuantity || 0) * pool.unitPrice).toFixed(2) }}
                </p>
              </div>
              <div class="bg-slate-50 dark:bg-slate-700/50 rounded-xl p-4">
                <p class="text-xs text-slate-500 dark:text-slate-400 mb-1">You Save</p>
                <p class="text-lg font-bold text-green-600 dark:text-green-400">
                  R{{ pool.mySavings?.toFixed(2) || '0.00' }}
                </p>
                <p class="text-xs text-slate-600 dark:text-slate-400">if target met</p>
              </div>
              <div class="bg-slate-50 dark:bg-slate-700/50 rounded-xl p-4">
                <p class="text-xs text-slate-500 dark:text-slate-400 mb-1">Closes In</p>
                <p class="text-lg font-bold text-slate-900 dark:text-white">
                  {{ getTimeRemaining(pool.deadline) }}
                </p>
                <p class="text-xs text-slate-600 dark:text-slate-400">{{ formatDate(pool.deadline) }}</p>
              </div>
            </div>

            <!-- Participants -->
            <div class="mb-6">
              <p class="text-sm font-semibold text-slate-900 dark:text-white mb-3">
                Participants ({{ pool.participants }})
              </p>
              <div class="flex flex-wrap gap-2">
                <div 
                  v-for="participant in pool.participantList" 
                  :key="participant.id"
                  class="flex items-center gap-2 px-3 py-2 bg-orange-50 dark:bg-orange-900/20 rounded-lg"
                >
                  <div class="w-8 h-8 rounded-full bg-gradient-to-br from-orange-500 to-purple-600 flex items-center justify-center text-white text-xs font-bold">
                    {{ participant.name.charAt(0) }}
                  </div>
                  <div>
                    <p class="text-xs font-medium text-slate-900 dark:text-white">{{ participant.name }}</p>
                    <p class="text-xs text-slate-500 dark:text-slate-400">{{ participant.quantity }} {{ pool.unit }}</p>
                  </div>
                </div>
              </div>
            </div>

            <!-- Actions -->
            <div class="flex items-center justify-between pt-4 border-t border-slate-200 dark:border-slate-700">
              <div class="flex space-x-3">
                <button 
                  v-if="!pool.isJoined && pool.status === 'open'"
                  @click="joinPool(pool)" 
                  class="px-6 py-3 bg-gradient-to-r from-orange-600 to-purple-600 hover:from-orange-700 hover:to-purple-700 text-white rounded-xl font-semibold shadow-lg hover:shadow-xl transition-all duration-200"
                >
                  Join Pool
                </button>
                <button 
                  v-if="pool.isJoined"
                  class="px-6 py-3 bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400 rounded-xl font-semibold"
                  disabled
                >
                  ✓ Joined
                </button>
                <button 
                  @click="sharePool(pool)"
                  class="px-6 py-3 bg-blue-600 hover:bg-blue-700 text-white rounded-xl font-semibold transition-colors"
                >
                  <ShareIcon class="w-5 h-5 inline mr-2" />
                  Share on WhatsApp
                </button>
              </div>
              <button 
                v-if="pool.isCreator"
                @click="cancelPool(pool)"
                class="text-red-600 hover:text-red-800 dark:text-red-400 dark:hover:text-red-200 text-sm font-medium"
              >
                Cancel Pool
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Empty State -->
      <div v-if="filteredPools.length === 0" class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-12 text-center">
        <UserGroupIcon class="w-16 h-16 text-slate-400 mx-auto mb-4" />
        <p class="text-lg font-semibold text-slate-900 dark:text-white mb-2">No pools found</p>
        <p class="text-slate-600 dark:text-slate-400 mb-6">Create the first pool and invite nearby shops to join!</p>
        <button 
          @click="showCreatePoolModal = true"
          class="inline-flex items-center px-6 py-3 bg-gradient-to-r from-orange-600 to-purple-600 text-white rounded-xl hover:from-orange-700 hover:to-purple-700 shadow-lg hover:shadow-xl transition-all duration-200 font-semibold"
        >
          <PlusIcon class="w-5 h-5 mr-2" />
          Create Pool
        </button>
      </div>
    </div>

    <!-- Create Pool Modal -->
    <CreatePoolModal
      :show="showCreatePoolModal"
      @close="showCreatePoolModal = false"
      @create="createPool"
    />

    <!-- Join Pool Modal -->
    <JoinPoolModal
      :show="showJoinPoolModal"
      :pool="selectedPool"
      @close="showJoinPoolModal = false"
      @join="confirmJoinPool"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import {
  PlusIcon,
  UserGroupIcon,
  ShoppingBagIcon,
  CurrencyDollarIcon,
  ChartBarIcon,
  SparklesIcon,
  ShareIcon
} from '@heroicons/vue/24/outline'
import CreatePoolModal from '~/components/buying/CreatePoolModal.vue'
import JoinPoolModal from '~/components/buying/JoinPoolModal.vue'
import { useGroupBuying } from '~/composables/useGroupBuying'

// Page metadata
useHead({
  title: 'Group Buying - TOSS ERP',
  meta: [
    { name: 'description', content: 'Join buying pools to save on bulk purchases' }
  ]
})

definePageMeta({
  layout: 'default'
})

// Composable
const { getPools, createPool: createPoolAPI, joinPool: joinPoolAPI } = useGroupBuying()

// State
const showCreatePoolModal = ref(false)
const showJoinPoolModal = ref(false)
const selectedPool = ref<any>(null)
const searchQuery = ref('')
const statusFilter = ref('')
const areaFilter = ref('')
const pools = ref<any[]>([])
const loading = ref(false)

// Stats
const stats = ref({
  activePools: 12,
  myPools: 3,
  totalSaved: 1250,
  avgSavingsPercent: 14
})

// Load pools on mount
onMounted(async () => {
  await loadPools()
})

const loadPools = async () => {
  loading.value = true
  try {
    pools.value = await getPools()
  } catch (error) {
    console.error('Failed to load pools:', error)
  } finally {
    loading.value = false
  }
}

// Computed
const filteredPools = computed(() => {
  let filtered = pools.value

  if (searchQuery.value) {
    filtered = filtered.filter((pool: any) => 
      pool.productName.toLowerCase().includes(searchQuery.value.toLowerCase())
    )
  }

  if (statusFilter.value) {
    filtered = filtered.filter((pool: any) => pool.status === statusFilter.value)
  }

  if (areaFilter.value) {
    filtered = filtered.filter((pool: any) => pool.area === areaFilter.value)
  }

  return filtered
})

// Methods
const formatDate = (date: Date) => {
  return new Date(date).toLocaleDateString('en-ZA', {
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

const getTimeRemaining = (deadline: Date) => {
  const now = new Date()
  const end = new Date(deadline)
  const diff = end.getTime() - now.getTime()
  
  if (diff <= 0) return 'Closed'
  
  const hours = Math.floor(diff / (1000 * 60 * 60))
  const minutes = Math.floor((diff % (1000 * 60 * 60)) / (1000 * 60))
  
  if (hours > 24) {
    const days = Math.floor(hours / 24)
    return `${days}d ${hours % 24}h`
  }
  
  return `${hours}h ${minutes}m`
}

const getStatusLabel = (status: string) => {
  const labels: Record<string, string> = {
    'open': '🟢 Open',
    'pending': '🟡 Pending',
    'confirmed': '✅ Confirmed',
    'fulfilled': '📦 Fulfilled',
    'cancelled': '❌ Cancelled'
  }
  return labels[status] || status
}

const getStatusBadge = (status: string) => {
  const badges = {
    open: 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400',
    pending: 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-400',
    confirmed: 'bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400',
    fulfilled: 'bg-emerald-100 text-emerald-800 dark:bg-emerald-900/30 dark:text-emerald-400',
    cancelled: 'bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400'
  }
  return badges[status as keyof typeof badges] || 'bg-slate-100 text-slate-800'
}

const joinPool = (pool: any) => {
  selectedPool.value = pool
  showJoinPoolModal.value = true
}

const confirmJoinPool = async (poolId: string, quantity: number) => {
  try {
    await joinPoolAPI(poolId, quantity)
    await loadPools()
    showJoinPoolModal.value = false
    showNotification(`✓ Joined pool successfully! You'll save R${selectedPool.value.mySavings?.toFixed(2)}`)
  } catch (error) {
    console.error('Failed to join pool:', error)
    showNotification('✗ Failed to join pool', 'error')
  }
}

const createPool = async (poolData: any) => {
  try {
    await createPoolAPI(poolData)
    await loadPools()
    showCreatePoolModal.value = false
    showNotification('✓ Pool created! Share the link to invite others')
  } catch (error) {
    console.error('Failed to create pool:', error)
    showNotification('✗ Failed to create pool', 'error')
  }
}

const sharePool = (pool: any) => {
  const inviteLink = `${window.location.origin}/buying/group-buying/join/${pool.id}`
  const message = `🛒 Join our Group Buying Pool!\n\n${pool.productName}\nUnit Price: R${pool.unitPrice} (Save ${pool.savingsPercent}%)\nCloses: ${formatDate(pool.deadline)}\n\nJoin now: ${inviteLink}`
  
  const whatsappUrl = `https://wa.me/?text=${encodeURIComponent(message)}`
  window.open(whatsappUrl, '_blank')
}

const cancelPool = async (pool: any) => {
  if (confirm(`Are you sure you want to cancel this pool?`)) {
    // Implement cancel logic
    showNotification('Pool cancelled')
  }
}

const quickJoinSuggestedPool = () => {
  // Implement AI suggested pool join
  showNotification('Joining suggested pool...')
}

const showNotification = (message: string, type: 'success' | 'error' = 'success') => {
  const notification = document.createElement('div')
  notification.textContent = message
  notification.className = `fixed top-20 right-4 ${type === 'success' ? 'bg-green-600' : 'bg-red-600'} text-white px-4 py-2 rounded-lg shadow-lg z-50 animate-fade-in`
  document.body.appendChild(notification)
  setTimeout(() => notification.remove(), 3000)
}
</script>

