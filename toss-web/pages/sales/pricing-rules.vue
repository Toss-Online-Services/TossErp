<template>
  <div class="min-h-screen bg-slate-50 dark:bg-slate-900">
    <!-- Mobile-First Page Container -->
    <div class="p-4 sm:p-6 space-y-4 sm:space-y-6 pb-20 lg:pb-6">
      <!-- Page Header -->
      <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-3 sm:gap-0">
        <div>
          <h1 class="text-2xl sm:text-3xl font-bold text-slate-900 dark:text-white">Pricing Rules</h1>
          <p class="text-slate-600 dark:text-slate-400 mt-1 text-sm sm:text-base">Automate discounts and special pricing for Thabo's Spaza Shop</p>
        </div>
        <div class="flex flex-wrap gap-2 sm:gap-3">
          <button @click="showNewRuleModal = true" 
                  class="flex-1 sm:flex-none px-4 py-2 sm:px-6 sm:py-3 bg-blue-600 hover:bg-blue-700 text-white rounded-lg transition-colors text-sm sm:text-base">
            <PlusIcon class="w-4 h-4 sm:w-5 sm:h-5 inline mr-2" />
            New Rule
          </button>
        </div>
      </div>

      <!-- Rule Stats -->
      <div class="grid grid-cols-1 xs:grid-cols-2 lg:grid-cols-4 gap-3 sm:gap-6">
        <div class="bg-white dark:bg-slate-800 p-4 sm:p-6 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">Active Rules</p>
              <p class="text-lg sm:text-2xl font-bold text-slate-900 dark:text-white">{{ activeRules }}</p>
              <p class="text-xs sm:text-sm text-green-600">{{ totalRules }} total</p>
            </div>
            <div class="p-2 sm:p-3 bg-green-100 dark:bg-green-900 rounded-full">
              <TagIcon class="w-4 h-4 sm:w-6 sm:h-6 text-green-600" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 p-4 sm:p-6 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">Discounts Applied</p>
              <p class="text-lg sm:text-2xl font-bold text-slate-900 dark:text-white">{{ discountsApplied }}</p>
              <p class="text-xs sm:text-sm text-blue-600">This month</p>
            </div>
            <div class="p-2 sm:p-3 bg-blue-100 dark:bg-blue-900 rounded-full">
              <ReceiptPercentIcon class="w-4 h-4 sm:w-6 sm:h-6 text-blue-600" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 p-4 sm:p-6 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">Total Savings</p>
              <p class="text-lg sm:text-2xl font-bold text-slate-900 dark:text-white">R {{ formatCurrency(totalSavings) }}</p>
              <p class="text-xs sm:text-sm text-purple-600">Customer savings</p>
            </div>
            <div class="p-2 sm:p-3 bg-purple-100 dark:bg-purple-900 rounded-full">
              <CurrencyDollarIcon class="w-4 h-4 sm:w-6 sm:h-6 text-purple-600" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 p-4 sm:p-6 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">Avg Discount</p>
              <p class="text-lg sm:text-2xl font-bold text-slate-900 dark:text-white">{{ avgDiscount }}%</p>
              <p class="text-xs sm:text-sm text-yellow-600">Per transaction</p>
            </div>
            <div class="p-2 sm:p-3 bg-yellow-100 dark:bg-yellow-900 rounded-full">
              <ChartBarIcon class="w-4 h-4 sm:w-6 sm:h-6 text-yellow-600" />
            </div>
          </div>
        </div>
      </div>

      <!-- Filters -->
      <div class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700 p-4 sm:p-6">
        <div class="flex flex-col sm:flex-row gap-3 sm:gap-4">
          <div class="flex-1">
            <input v-model="searchQuery" type="text" placeholder="Search pricing rules..." 
                   class="w-full px-3 sm:px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white">
          </div>
          <div class="flex gap-2 sm:gap-3">
            <select v-model="typeFilter" 
                    class="px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white">
              <option value="">All Types</option>
              <option value="percentage">Percentage</option>
              <option value="fixed">Fixed Amount</option>
              <option value="buy-x-get-y">Buy X Get Y</option>
            </select>
            <select v-model="statusFilter" 
                    class="px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white">
              <option value="">All Status</option>
              <option value="active">Active</option>
              <option value="inactive">Inactive</option>
              <option value="expired">Expired</option>
            </select>
          </div>
        </div>
      </div>

      <!-- Pricing Rules List -->
      <div class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
        <div class="p-4 sm:p-6 border-b border-slate-200 dark:border-slate-700">
          <h3 class="text-base sm:text-lg font-semibold text-slate-900 dark:text-white">Pricing Rules</h3>
        </div>
        <div class="p-4 sm:p-6">
          <div class="space-y-3 sm:space-y-4">
            <div v-for="rule in filteredRules" :key="rule.id" 
                 class="flex items-center justify-between p-4 rounded-lg border border-slate-100 dark:border-slate-700 hover:bg-slate-50 dark:hover:bg-slate-700 transition-colors">
              <div class="flex items-center space-x-3 flex-1 min-w-0">
                <div class="w-10 h-10 rounded-full flex items-center justify-center" :class="getTypeColor(rule.type)">
                  <TagIcon class="w-5 h-5 text-white" />
                </div>
                <div class="flex-1 min-w-0">
                  <div class="flex items-center gap-2">
                    <p class="text-sm font-medium text-slate-900 dark:text-white truncate">{{ rule.name }}</p>
                    <span class="inline-flex px-2 py-1 text-xs rounded-full" :class="getStatusBadge(rule.status)">
                      {{ rule.status }}
                    </span>
                  </div>
                  <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">{{ rule.description }}</p>
                  <p class="text-xs text-slate-500 dark:text-slate-500">Valid: {{ formatDate(rule.validFrom) }} - {{ formatDate(rule.validTo) }}</p>
                </div>
              </div>
              <div class="text-right">
                <p class="text-sm font-semibold text-emerald-600">{{ formatDiscount(rule) }}</p>
                <p class="text-xs text-slate-500 dark:text-slate-500">{{ rule.appliedCount }} times</p>
                <div class="flex gap-1 mt-1">
                  <button @click="editRule(rule)" class="p-1 text-blue-600 hover:bg-blue-50 dark:hover:bg-blue-900 rounded">
                    <PencilIcon class="w-4 h-4" />
                  </button>
                  <button @click="toggleRuleStatus(rule)" class="p-1 text-green-600 hover:bg-green-50 dark:hover:bg-green-900 rounded">
                    <component :is="rule.status === 'active' ? 'PauseIcon' : 'PlayIcon'" class="w-4 h-4" />
                  </button>
                  <button @click="deleteRule(rule)" class="p-1 text-red-600 hover:bg-red-50 dark:hover:bg-red-900 rounded">
                    <TrashIcon class="w-4 h-4" />
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- New Rule Modal -->
    <div v-if="showNewRuleModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div class="bg-white dark:bg-slate-800 rounded-xl sm:rounded-2xl shadow-xl max-w-2xl w-full mx-4 max-h-[90vh] overflow-y-auto">
        <div class="p-4 sm:p-6 border-b border-slate-200 dark:border-slate-700">
          <h3 class="text-lg sm:text-xl font-semibold text-slate-900 dark:text-white">Create Pricing Rule</h3>
        </div>
        <div class="p-4 sm:p-6">
          <form @submit.prevent="createRule">
            <div class="space-y-4">
              <div>
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Rule Name</label>
                <input v-model="newRule.name" type="text" required placeholder="e.g., Weekend Special"
                       class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white">
              </div>

              <div>
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Description</label>
                <textarea v-model="newRule.description" rows="2" placeholder="Describe the pricing rule..."
                          class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white"></textarea>
              </div>

              <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
                <div>
                  <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Rule Type</label>
                  <select v-model="newRule.type" required
                          class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white">
                    <option value="percentage">Percentage Discount</option>
                    <option value="fixed">Fixed Amount</option>
                    <option value="buy-x-get-y">Buy X Get Y Free</option>
                  </select>
                </div>
                <div>
                  <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Discount Value</label>
                  <input v-model.number="newRule.discountValue" type="number" step="0.01" min="0" required
                         :placeholder="newRule.type === 'percentage' ? 'e.g., 10 (%)' : 'e.g., 50 (R)'"
                         class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white">
                </div>
              </div>

              <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
                <div>
                  <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Valid From</label>
                  <input v-model="newRule.validFrom" type="date" required
                         class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white">
                </div>
                <div>
                  <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Valid To</label>
                  <input v-model="newRule.validTo" type="date" required
                         class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white">
                </div>
              </div>

              <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
                <div>
                  <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Min Quantity</label>
                  <input v-model.number="newRule.minQuantity" type="number" min="0" placeholder="0 = No minimum"
                         class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white">
                </div>
                <div>
                  <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Min Amount (R)</label>
                  <input v-model.number="newRule.minAmount" type="number" step="0.01" min="0" placeholder="0 = No minimum"
                         class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white">
                </div>
              </div>

              <div>
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Apply To</label>
                <select v-model="newRule.applyTo" required
                        class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white">
                  <option value="all">All Products</option>
                  <option value="category">Specific Category</option>
                  <option value="product">Specific Products</option>
                  <option value="customer">Specific Customers</option>
                </select>
              </div>
            </div>

            <div class="flex justify-end space-x-3 mt-6">
              <button @click="showNewRuleModal = false" type="button" 
                      class="px-4 py-2 text-slate-600 dark:text-slate-400 hover:text-slate-800 dark:hover:text-slate-200">
                Cancel
              </button>
              <button type="submit" 
                      class="px-6 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg">
                Create Rule
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
import { 
  TagIcon,
  PlusIcon,
  ReceiptPercentIcon,
  CurrencyDollarIcon,
  ChartBarIcon,
  PencilIcon,
  TrashIcon,
  PlayIcon,
  PauseIcon
} from '@heroicons/vue/24/outline'

// Page metadata
useHead({
  title: 'Pricing Rules - TOSS ERP',
  meta: [
    { name: 'description', content: 'Automate discounts and special pricing' }
  ]
})

// Layout
definePageMeta({
  layout: 'default'
})

// Reactive data
const showNewRuleModal = ref(false)
const searchQuery = ref('')
const typeFilter = ref('')
const statusFilter = ref('')

// Rule statistics
const activeRules = ref(8)
const totalRules = ref(12)
const discountsApplied = ref(234)
const totalSavings = ref(12450)
const avgDiscount = ref(8.5)

// Sample pricing rules
const rules = ref([
  {
    id: '1',
    name: 'Weekend Special',
    description: '10% off all purchases on weekends',
    type: 'percentage',
    discountValue: 10,
    status: 'active',
    validFrom: new Date('2025-01-01'),
    validTo: new Date('2025-12-31'),
    minQuantity: 0,
    minAmount: 0,
    applyTo: 'all',
    appliedCount: 89
  },
  {
    id: '2',
    name: 'Bulk Buy Discount',
    description: 'R50 off purchases over R500',
    type: 'fixed',
    discountValue: 50,
    status: 'active',
    validFrom: new Date('2025-01-01'),
    validTo: new Date('2025-06-30'),
    minQuantity: 0,
    minAmount: 500,
    applyTo: 'all',
    appliedCount: 45
  },
  {
    id: '3',
    name: 'Buy 2 Get 1 Free',
    description: 'Buy 2 items, get 1 free on selected products',
    type: 'buy-x-get-y',
    discountValue: 1,
    status: 'active',
    validFrom: new Date('2025-01-15'),
    validTo: new Date('2025-02-15'),
    minQuantity: 2,
    minAmount: 0,
    applyTo: 'category',
    appliedCount: 67
  }
])

// Form data
const newRule = ref({
  name: '',
  description: '',
  type: 'percentage',
  discountValue: 0,
  validFrom: new Date().toISOString().split('T')[0],
  validTo: new Date(Date.now() + 30 * 24 * 60 * 60 * 1000).toISOString().split('T')[0],
  minQuantity: 0,
  minAmount: 0,
  applyTo: 'all'
})

// Computed
const filteredRules = computed(() => {
  let filtered = rules.value

  if (searchQuery.value) {
    filtered = filtered.filter(rule => 
      rule.name.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      rule.description.toLowerCase().includes(searchQuery.value.toLowerCase())
    )
  }

  if (typeFilter.value) {
    filtered = filtered.filter(rule => rule.type === typeFilter.value)
  }

  if (statusFilter.value) {
    filtered = filtered.filter(rule => rule.status === statusFilter.value)
  }

  return filtered
})

// Helper functions
const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-ZA', {
    minimumFractionDigits: 0,
    maximumFractionDigits: 0
  }).format(amount)
}

const formatDate = (date: Date) => {
  return new Intl.DateTimeFormat('en-ZA', {
    day: 'numeric',
    month: 'short',
    year: 'numeric'
  }).format(date)
}

const formatDiscount = (rule: any) => {
  if (rule.type === 'percentage') {
    return `${rule.discountValue}% off`
  } else if (rule.type === 'fixed') {
    return `R${rule.discountValue} off`
  } else {
    return `Buy ${rule.minQuantity} Get ${rule.discountValue}`
  }
}

const getTypeColor = (type: string) => {
  const colors = {
    percentage: 'bg-blue-600',
    fixed: 'bg-green-600',
    'buy-x-get-y': 'bg-purple-600'
  }
  return colors[type as keyof typeof colors] || 'bg-slate-600'
}

const getStatusBadge = (status: string) => {
  const badges = {
    active: 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200',
    inactive: 'bg-slate-100 text-slate-800 dark:bg-slate-900 dark:text-slate-200',
    expired: 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200'
  }
  return badges[status as keyof typeof badges] || 'bg-slate-100 text-slate-800'
}

// Actions
const createRule = async () => {
  try {
    const rule = {
      id: Date.now().toString(),
      ...newRule.value,
      status: 'active',
      validFrom: new Date(newRule.value.validFrom),
      validTo: new Date(newRule.value.validTo),
      appliedCount: 0
    }

    rules.value.unshift(rule)
    totalRules.value += 1
    activeRules.value += 1
    
    showNewRuleModal.value = false
    
    // Reset form
    newRule.value = {
      name: '',
      description: '',
      type: 'percentage',
      discountValue: 0,
      validFrom: new Date().toISOString().split('T')[0],
      validTo: new Date(Date.now() + 30 * 24 * 60 * 60 * 1000).toISOString().split('T')[0],
      minQuantity: 0,
      minAmount: 0,
      applyTo: 'all'
    }
    
    alert('Pricing rule created successfully!')
  } catch (error) {
    console.error('Error creating rule:', error)
    alert('Failed to create pricing rule. Please try again.')
  }
}

const editRule = (rule: any) => {
  alert(`Editing rule: ${rule.name}`)
}

const toggleRuleStatus = (rule: any) => {
  rule.status = rule.status === 'active' ? 'inactive' : 'active'
  if (rule.status === 'active') {
    activeRules.value += 1
  } else {
    activeRules.value -= 1
  }
  alert(`Rule ${rule.name} is now ${rule.status}`)
}

const deleteRule = (rule: any) => {
  if (confirm(`Delete pricing rule "${rule.name}"?`)) {
    const index = rules.value.findIndex(r => r.id === rule.id)
    if (index !== -1) {
      rules.value.splice(index, 1)
      totalRules.value -= 1
      if (rule.status === 'active') {
        activeRules.value -= 1
      }
      alert('Pricing rule deleted successfully!')
    }
  }
}
</script>


