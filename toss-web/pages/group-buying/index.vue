<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <!-- Page Header -->
    <div class="bg-white dark:bg-gray-800 shadow-sm border-b border-gray-200 dark:border-gray-700">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="py-4">
          <div class="flex items-center justify-between">
            <div>
              <h1 class="text-2xl font-bold text-gray-900 dark:text-white">Group Buying & Collective Procurement</h1>
              <p class="text-gray-600 dark:text-gray-400">Join forces with other SMMEs to get better prices through bulk purchasing</p>
            </div>
            <div class="flex space-x-3">
              <button @click="showCreateGroupModal = true" class="bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700 transition-colors">
                <PlusIcon class="w-5 h-5 inline mr-2" />
                Create Group Buy
              </button>
              <button @click="showRequestModal = true" class="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors">
                <ShoppingCartIcon class="w-5 h-5 inline mr-2" />
                Request Quote
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Stats Cards -->
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-gray-600 dark:text-gray-400">Active Group Buys</p>
              <p class="text-2xl font-bold text-blue-600">{{ stats.activeGroupBuys }}</p>
            </div>
            <div class="p-3 bg-blue-100 dark:bg-blue-900 rounded-full">
              <UsersIcon class="w-6 h-6 text-blue-600" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-gray-600 dark:text-gray-400">Total Savings</p>
              <p class="text-2xl font-bold text-green-600">R {{ formatCurrency(stats.totalSavings) }}</p>
            </div>
            <div class="p-3 bg-green-100 dark:bg-green-900 rounded-full">
              <CurrencyDollarIcon class="w-6 h-6 text-green-600" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-gray-600 dark:text-gray-400">Partner Businesses</p>
              <p class="text-2xl font-bold text-purple-600">{{ stats.partnerBusinesses }}</p>
            </div>
            <div class="p-3 bg-purple-100 dark:bg-purple-900 rounded-full">
              <BuildingOfficeIcon class="w-6 h-6 text-purple-600" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-gray-600 dark:text-gray-400">Orders This Month</p>
              <p class="text-2xl font-bold text-orange-600">{{ stats.monthlyOrders }}</p>
            </div>
            <div class="p-3 bg-orange-100 dark:bg-orange-900 rounded-full">
              <ShoppingBagIcon class="w-6 h-6 text-orange-600" />
            </div>
          </div>
        </div>
      </div>

      <!-- Active Group Purchases -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 mb-8">
        <div class="p-6 border-b border-gray-200 dark:border-gray-700">
          <div class="flex items-center justify-between">
            <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Active Group Purchases</h3>
            <div class="flex space-x-2">
              <select class="text-sm border border-gray-300 dark:border-gray-600 rounded-lg px-3 py-1 bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
                <option>All Categories</option>
                <option>Food & Beverages</option>
                <option>Office Supplies</option>
                <option>Electronics</option>
                <option>Manufacturing</option>
              </select>
              <select class="text-sm border border-gray-300 dark:border-gray-600 rounded-lg px-3 py-1 bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
                <option>All Locations</option>
                <option>Cape Town</option>
                <option>Johannesburg</option>
                <option>Durban</option>
                <option>Port Elizabeth</option>
              </select>
            </div>
          </div>
        </div>
        <div class="p-6">
          <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            <div v-for="groupBuy in activeGroupBuys" :key="groupBuy.id" class="border border-gray-200 dark:border-gray-700 rounded-lg p-6 hover:shadow-lg transition-shadow cursor-pointer">
              <div class="flex items-center justify-between mb-4">
                <div class="flex items-center space-x-3">
                  <div class="w-12 h-12 bg-blue-100 dark:bg-blue-900 rounded-lg flex items-center justify-center">
                    <component :is="getCategoryIcon(groupBuy.category)" class="w-6 h-6 text-blue-600" />
                  </div>
                  <div>
                    <h4 class="font-semibold text-gray-900 dark:text-white">{{ groupBuy.title }}</h4>
                    <p class="text-sm text-gray-600 dark:text-gray-400">{{ groupBuy.category }}</p>
                  </div>
                </div>
                <span class="bg-green-100 dark:bg-green-900 text-green-800 dark:text-green-400 text-xs font-medium px-2 py-1 rounded-full">
                  {{ groupBuy.savings }}% off
                </span>
              </div>

              <div class="mb-4">
                <div class="flex justify-between text-sm mb-2">
                  <span class="text-gray-600 dark:text-gray-400">Progress</span>
                  <span class="text-gray-900 dark:text-white">{{ groupBuy.currentParticipants }}/{{ groupBuy.targetParticipants }}</span>
                </div>
                <div class="w-full bg-gray-200 dark:bg-gray-700 rounded-full h-2">
                  <div 
                    class="bg-blue-600 h-2 rounded-full transition-all duration-300" 
                    :style="{ width: `${(groupBuy.currentParticipants / groupBuy.targetParticipants) * 100}%` }"
                  ></div>
                </div>
              </div>

              <div class="space-y-2 mb-4">
                <div class="flex justify-between text-sm">
                  <span class="text-gray-600 dark:text-gray-400">Regular Price:</span>
                  <span class="text-gray-500 line-through">R {{ formatCurrency(groupBuy.regularPrice) }}</span>
                </div>
                <div class="flex justify-between">
                  <span class="text-gray-600 dark:text-gray-400">Group Price:</span>
                  <span class="text-green-600 font-semibold">R {{ formatCurrency(groupBuy.groupPrice) }}</span>
                </div>
                <div class="flex justify-between text-sm">
                  <span class="text-gray-600 dark:text-gray-400">Your Savings:</span>
                  <span class="text-green-600 font-medium">R {{ formatCurrency(groupBuy.regularPrice - groupBuy.groupPrice) }}</span>
                </div>
              </div>

              <div class="flex justify-between items-center mb-4">
                <div class="text-sm text-gray-600 dark:text-gray-400">
                  <p>Ends: {{ formatDate(groupBuy.endDate) }}</p>
                  <p>Location: {{ groupBuy.location }}</p>
                </div>
                <div class="text-right">
                  <p class="text-xs text-gray-500 dark:text-gray-400">Organizer</p>
                  <p class="text-sm font-medium text-gray-900 dark:text-white">{{ groupBuy.organizer }}</p>
                </div>
              </div>

              <div class="flex space-x-2">
                <button 
                  @click="joinGroupBuy(groupBuy.id)"
                  :disabled="groupBuy.hasJoined"
                  :class="groupBuy.hasJoined 
                    ? 'bg-gray-300 dark:bg-gray-600 text-gray-500 cursor-not-allowed' 
                    : 'bg-blue-600 hover:bg-blue-700 text-white'"
                  class="flex-1 py-2 px-4 rounded-lg transition-colors text-sm font-medium"
                >
                  {{ groupBuy.hasJoined ? 'Already Joined' : 'Join Group Buy' }}
                </button>
                <button @click="viewDetails(groupBuy.id)" class="px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors text-sm">
                  Details
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- My Participations -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-8">
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
          <div class="p-6 border-b border-gray-200 dark:border-gray-700">
            <h3 class="text-lg font-semibold text-gray-900 dark:text-white">My Participations</h3>
          </div>
          <div class="p-6">
            <div class="space-y-4">
              <div v-for="participation in myParticipations" :key="participation.id" class="flex items-center justify-between p-4 bg-gray-50 dark:bg-gray-700 rounded-lg">
                <div class="flex items-center space-x-3">
                  <div class="w-10 h-10 bg-blue-100 dark:bg-blue-900 rounded-full flex items-center justify-center">
                    <ShoppingBagIcon class="w-5 h-5 text-blue-600" />
                  </div>
                  <div>
                    <p class="font-medium text-gray-900 dark:text-white">{{ participation.title }}</p>
                    <p class="text-sm text-gray-600 dark:text-gray-400">{{ participation.status }} • {{ formatDate(participation.joinedDate) }}</p>
                  </div>
                </div>
                <div class="text-right">
                  <p class="font-semibold text-gray-900 dark:text-white">R {{ formatCurrency(participation.amount) }}</p>
                  <span class="text-xs px-2 py-1 rounded-full" :class="getStatusColor(participation.status)">
                    {{ participation.status }}
                  </span>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Group Buy Requests -->
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
          <div class="p-6 border-b border-gray-200 dark:border-gray-700">
            <h3 class="text-lg font-semibold text-gray-900 dark:text-white">My Requests</h3>
          </div>
          <div class="p-6">
            <div class="space-y-4">
              <div v-for="request in myRequests" :key="request.id" class="flex items-center justify-between p-4 bg-gray-50 dark:bg-gray-700 rounded-lg">
                <div class="flex items-center space-x-3">
                  <div class="w-10 h-10 bg-purple-100 dark:bg-purple-900 rounded-full flex items-center justify-center">
                    <DocumentTextIcon class="w-5 h-5 text-purple-600" />
                  </div>
                  <div>
                    <p class="font-medium text-gray-900 dark:text-white">{{ request.item }}</p>
                    <p class="text-sm text-gray-600 dark:text-gray-400">{{ request.quantity }} units • {{ formatDate(request.requestDate) }}</p>
                  </div>
                </div>
                <div class="text-right">
                  <span class="text-xs px-2 py-1 rounded-full" :class="getRequestStatusColor(request.status)">
                    {{ request.status }}
                  </span>
                  <p class="text-sm text-gray-600 dark:text-gray-400 mt-1">{{ request.responses }} responses</p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- SMME Network -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
        <div class="p-6 border-b border-gray-200 dark:border-gray-700">
          <div class="flex items-center justify-between">
            <h3 class="text-lg font-semibold text-gray-900 dark:text-white">SMME Network</h3>
            <button class="text-blue-600 hover:text-blue-800 text-sm font-medium">Expand Network</button>
          </div>
        </div>
        <div class="p-6">
          <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
            <div v-for="business in networkBusinesses" :key="business.id" class="flex items-center space-x-4 p-4 border border-gray-200 dark:border-gray-700 rounded-lg hover:shadow-md transition-shadow">
              <div class="w-12 h-12 bg-gradient-to-r from-blue-400 to-purple-500 rounded-full flex items-center justify-center">
                <span class="text-white font-bold text-lg">{{ getInitials(business.name) }}</span>
              </div>
              <div class="flex-1">
                <p class="font-medium text-gray-900 dark:text-white">{{ business.name }}</p>
                <p class="text-sm text-gray-600 dark:text-gray-400">{{ business.industry }}</p>
                <p class="text-xs text-gray-500 dark:text-gray-400">{{ business.location }} • {{ business.groupBuysJoined }} group buys</p>
              </div>
              <div class="flex space-x-1">
                <div v-for="i in business.trustRating" :key="i" class="w-3 h-3 bg-yellow-400 rounded-full"></div>
                <div v-for="i in (5 - business.trustRating)" :key="i + business.trustRating" class="w-3 h-3 bg-gray-300 dark:bg-gray-600 rounded-full"></div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
// Icons (would normally import from a proper icon library)
const PlusIcon = 'svg'
const ShoppingCartIcon = 'svg'
const UsersIcon = 'svg'
const CurrencyDollarIcon = 'svg'
const BuildingOfficeIcon = 'svg'
const ShoppingBagIcon = 'svg'
const DocumentTextIcon = 'svg'

// Sample data
const stats = ref({
  activeGroupBuys: 24,
  totalSavings: 45600,
  partnerBusinesses: 156,
  monthlyOrders: 43
})

const activeGroupBuys = ref([
  {
    id: 1,
    title: 'Bulk Office Supplies',
    category: 'Office Supplies',
    regularPrice: 450,
    groupPrice: 340,
    savings: 24,
    currentParticipants: 8,
    targetParticipants: 12,
    endDate: new Date(Date.now() + 7 * 24 * 60 * 60 * 1000),
    location: 'Cape Town',
    organizer: 'ABC Stationery',
    hasJoined: false
  },
  {
    id: 2,
    title: 'Catering Supplies Bundle',
    category: 'Food & Beverages',
    regularPrice: 1200,
    groupPrice: 950,
    savings: 21,
    currentParticipants: 15,
    targetParticipants: 20,
    endDate: new Date(Date.now() + 5 * 24 * 60 * 60 * 1000),
    location: 'Johannesburg',
    organizer: 'Metro Foods',
    hasJoined: true
  },
  {
    id: 3,
    title: 'Electronics & IT Equipment',
    category: 'Electronics',
    regularPrice: 2500,
    groupPrice: 1900,
    savings: 24,
    currentParticipants: 6,
    targetParticipants: 10,
    endDate: new Date(Date.now() + 10 * 24 * 60 * 60 * 1000),
    location: 'Durban',
    organizer: 'TechHub SA',
    hasJoined: false
  }
])

const myParticipations = ref([
  {
    id: 1,
    title: 'Catering Supplies Bundle',
    amount: 950,
    status: 'Active',
    joinedDate: new Date(Date.now() - 3 * 24 * 60 * 60 * 1000)
  },
  {
    id: 2,
    title: 'Cleaning Supplies Bulk Order',
    amount: 340,
    status: 'Completed',
    joinedDate: new Date(Date.now() - 14 * 24 * 60 * 60 * 1000)
  },
  {
    id: 3,
    title: 'Packaging Materials',
    amount: 680,
    status: 'Pending',
    joinedDate: new Date(Date.now() - 2 * 24 * 60 * 60 * 1000)
  }
])

const myRequests = ref([
  {
    id: 1,
    item: 'Industrial Printer Paper',
    quantity: 500,
    status: 'Open',
    responses: 3,
    requestDate: new Date(Date.now() - 24 * 60 * 60 * 1000)
  },
  {
    id: 2,
    item: 'Coffee & Tea Supplies',
    quantity: 100,
    status: 'In Progress',
    responses: 7,
    requestDate: new Date(Date.now() - 5 * 24 * 60 * 60 * 1000)
  }
])

const networkBusinesses = ref([
  {
    id: 1,
    name: 'ABC Stationery',
    industry: 'Retail',
    location: 'Cape Town',
    groupBuysJoined: 12,
    trustRating: 5
  },
  {
    id: 2,
    name: 'Metro Foods',
    industry: 'Food Service',
    location: 'Johannesburg',
    groupBuysJoined: 8,
    trustRating: 4
  },
  {
    id: 3,
    name: 'TechHub SA',
    industry: 'Technology',
    location: 'Durban',
    groupBuysJoined: 15,
    trustRating: 5
  }
])

const showCreateGroupModal = ref(false)
const showRequestModal = ref(false)

// Helper functions
function formatCurrency(amount: number): string {
  return new Intl.NumberFormat('en-ZA', {
    style: 'decimal',
    minimumFractionDigits: 0,
    maximumFractionDigits: 0
  }).format(amount)
}

function formatDate(date: Date): string {
  return new Intl.DateTimeFormat('en-ZA', {
    month: 'short',
    day: 'numeric'
  }).format(date)
}

function getInitials(name: string): string {
  return name.split(' ').map(n => n[0]).join('').toUpperCase()
}

function getCategoryIcon(category: string) {
  switch (category) {
    case 'Office Supplies': return 'svg'
    case 'Food & Beverages': return 'svg'
    case 'Electronics': return 'svg'
    default: return 'svg'
  }
}

function getStatusColor(status: string): string {
  switch (status) {
    case 'Active': return 'bg-blue-100 text-blue-800'
    case 'Completed': return 'bg-green-100 text-green-800'
    case 'Pending': return 'bg-yellow-100 text-yellow-800'
    default: return 'bg-gray-100 text-gray-800'
  }
}

function getRequestStatusColor(status: string): string {
  switch (status) {
    case 'Open': return 'bg-green-100 text-green-800'
    case 'In Progress': return 'bg-blue-100 text-blue-800'
    case 'Closed': return 'bg-gray-100 text-gray-800'
    default: return 'bg-gray-100 text-gray-800'
  }
}

function joinGroupBuy(groupBuyId: number) {
  // TODO: Implement join group buy functionality
  console.log('Joining group buy:', groupBuyId)
}

function viewDetails(groupBuyId: number) {
  // TODO: Implement view details functionality
  console.log('Viewing details for group buy:', groupBuyId)
}

// Page metadata
useHead({
  title: 'Group Buying - TOSS ERP',
  meta: [
    { name: 'description', content: 'Collaborative procurement and group buying for SMMEs in TOSS ERP' }
  ]
})
</script>
