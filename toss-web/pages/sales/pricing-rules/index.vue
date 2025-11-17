<template>
  <div class="p-6">
    <div class="flex items-center justify-between mb-6">
      <div>
        <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
          Pricing Rules
        </h1>
        <p class="text-gray-600 dark:text-gray-400">
          Configure dynamic pricing strategies for different customer groups and products
        </p>
      </div>
      <button class="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg">
        <Icon name="heroicons:plus" class="w-4 h-4 mr-2" />
        New Pricing Rule
      </button>
    </div>

    <!-- Stats -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm">
        <div class="flex items-center">
          <div class="p-2 bg-green-100 rounded-full">
            <Icon name="heroicons:currency-dollar" class="w-5 h-5 text-green-600" />
          </div>
          <div class="ml-3">
            <p class="text-sm text-gray-600 dark:text-gray-400">Active Rules</p>
            <p class="text-lg font-semibold text-gray-900 dark:text-white">18</p>
          </div>
        </div>
      </div>
      
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm">
        <div class="flex items-center">
          <div class="p-2 bg-blue-100 rounded-full">
            <Icon name="heroicons:chart-bar" class="w-5 h-5 text-blue-600" />
          </div>
          <div class="ml-3">
            <p class="text-sm text-gray-600 dark:text-gray-400">Products Covered</p>
            <p class="text-lg font-semibold text-gray-900 dark:text-white">1,247</p>
          </div>
        </div>
      </div>
      
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm">
        <div class="flex items-center">
          <div class="p-2 bg-purple-100 rounded-full">
            <Icon name="heroicons:users" class="w-5 h-5 text-purple-600" />
          </div>
          <div class="ml-3">
            <p class="text-sm text-gray-600 dark:text-gray-400">Customer Groups</p>
            <p class="text-lg font-semibold text-gray-900 dark:text-white">6</p>
          </div>
        </div>
      </div>
      
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm">
        <div class="flex items-center">
          <div class="p-2 bg-yellow-100 rounded-full">
            <Icon name="heroicons:clock" class="w-5 h-5 text-yellow-600" />
          </div>
          <div class="ml-3">
            <p class="text-sm text-gray-600 dark:text-gray-400">Time-Based Rules</p>
            <p class="text-lg font-semibold text-gray-900 dark:text-white">4</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Pricing Rules List -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm">
      <div class="p-6">
        <h2 class="text-lg font-medium text-gray-900 dark:text-white mb-4">
          Active Pricing Rules
        </h2>
        
        <div class="space-y-4">
          <div v-for="rule in mockPricingRules" :key="rule.id" 
               class="border border-gray-200 dark:border-gray-700 rounded-lg p-4">
            <div class="flex items-start justify-between">
              <div class="flex-1">
                <div class="flex items-center mb-2">
                  <h3 class="font-medium text-gray-900 dark:text-white mr-3">
                    {{ rule.name }}
                  </h3>
                  <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                        :class="rule.isActive ? 
                          'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200' : 
                          'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300'">
                    {{ rule.isActive ? 'Active' : 'Inactive' }}
                  </span>
                  <span class="ml-2 inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                        :class="getPriorityClass(rule.priority)">
                    Priority {{ rule.priority }}
                  </span>
                </div>
                
                <p class="text-sm text-gray-600 dark:text-gray-400 mb-3">
                  {{ rule.description }}
                </p>
                
                <div class="grid grid-cols-1 md:grid-cols-3 gap-4 text-sm">
                  <div>
                    <span class="font-medium text-gray-700 dark:text-gray-300">Applies to:</span>
                    <div class="text-gray-600 dark:text-gray-400">
                      {{ formatRuleScope(rule) }}
                    </div>
                  </div>
                  
                  <div>
                    <span class="font-medium text-gray-700 dark:text-gray-300">Adjustment:</span>
                    <div class="text-gray-600 dark:text-gray-400">
                      {{ formatPriceAdjustment(rule) }}
                    </div>
                  </div>
                  
                  <div>
                    <span class="font-medium text-gray-700 dark:text-gray-300">Valid:</span>
                    <div class="text-gray-600 dark:text-gray-400">
                      {{ formatValidityPeriod(rule) }}
                    </div>
                  </div>
                </div>
                
                <div v-if="rule.conditions.length" class="mt-3">
                  <span class="text-sm font-medium text-gray-700 dark:text-gray-300">Conditions:</span>
                  <div class="flex flex-wrap gap-2 mt-1">
                    <span v-for="condition in rule.conditions" :key="condition" 
                          class="inline-flex items-center px-2 py-1 text-xs bg-blue-50 text-blue-700 rounded dark:bg-blue-900 dark:text-blue-300">
                      {{ condition }}
                    </span>
                  </div>
                </div>
              </div>
              
              <div class="flex items-center space-x-2 ml-4">
                <button class="text-blue-600 hover:text-blue-800 text-sm px-3 py-1 border border-blue-200 rounded">
                  Edit
                </button>
                <button class="text-gray-600 hover:text-gray-800 text-sm px-3 py-1 border border-gray-200 rounded">
                  Clone
                </button>
                <button :class="rule.isActive ? 'text-red-600 hover:text-red-800 border-red-200' : 'text-green-600 hover:text-green-800 border-green-200'"
                        class="text-sm px-3 py-1 border rounded">
                  {{ rule.isActive ? 'Disable' : 'Enable' }}
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'

useHead({
  title: 'Pricing Rules - TOSS ERP'
})

// Mock data for demonstration
const mockPricingRules = ref([
  {
    id: 'rule-001',
    name: 'Spaza Shop Volume Pricing',
    description: 'Tiered pricing for spaza shops based on order volume',
    isActive: true,
    priority: 1,
    type: 'volume_based',
    scope: {
      customerGroups: ['spaza-shops'],
      productCategories: ['groceries'],
      territories: ['soweto', 'alexandra']
    },
    adjustment: {
      type: 'tiered_percentage',
      tiers: [
        { minimumQuantity: 50, discount: 5 },
        { minimumQuantity: 100, discount: 10 },
        { minimumQuantity: 200, discount: 15 }
      ]
    },
    validity: {
      startDate: new Date('2025-01-01'),
      endDate: new Date('2025-12-31')
    },
    conditions: ['Minimum order R500', 'Same-day delivery']
  },
  {
    id: 'rule-002',
    name: 'Stokvel Group Discount',
    description: 'Special pricing for stokvel bulk orders',
    isActive: true,
    priority: 2,
    type: 'group_discount',
    scope: {
      customerGroups: ['stokvels'],
      productCategories: ['all'],
      territories: ['all']
    },
    adjustment: {
      type: 'flat_percentage',
      percentage: 12
    },
    validity: {
      startDate: new Date('2025-01-01'),
      endDate: new Date('2025-06-30')
    },
    conditions: ['Minimum 5 members', 'Monthly order commitment']
  },
  {
    id: 'rule-003',
    name: 'Weekend Tavern Special',
    description: 'Weekend pricing for taverns and shebeens',
    isActive: true,
    priority: 3,
    type: 'time_based',
    scope: {
      customerGroups: ['taverns'],
      productCategories: ['beverages'],
      territories: ['all']
    },
    adjustment: {
      type: 'flat_percentage',
      percentage: 8
    },
    validity: {
      startDate: new Date('2025-01-01'),
      endDate: new Date('2025-12-31')
    },
    conditions: ['Weekends only', 'Minimum R1000 order']
  },
  {
    id: 'rule-004',
    name: 'New Customer Incentive',
    description: 'First-time customer pricing incentive',
    isActive: true,
    priority: 4,
    type: 'customer_based',
    scope: {
      customerGroups: ['new-customers'],
      productCategories: ['all'],
      territories: ['all']
    },
    adjustment: {
      type: 'flat_percentage',
      percentage: 15
    },
    validity: {
      startDate: new Date('2025-01-01'),
      endDate: new Date('2025-03-31')
    },
    conditions: ['First order only', 'Valid for 30 days']
  }
])

const formatRuleScope = (rule: any) => {
  const parts = []
  if (rule.scope.customerGroups?.length) {
    parts.push(`${rule.scope.customerGroups.length} customer groups`)
  }
  if (rule.scope.productCategories?.length) {
    const categories = rule.scope.productCategories.includes('all') ? 'All products' : `${rule.scope.productCategories.length} categories`
    parts.push(categories)
  }
  if (rule.scope.territories?.length) {
    const territories = rule.scope.territories.includes('all') ? 'All areas' : `${rule.scope.territories.length} territories`
    parts.push(territories)
  }
  return parts.join(', ')
}

const formatPriceAdjustment = (rule: any) => {
  if (rule.adjustment.type === 'flat_percentage') {
    return `${rule.adjustment.percentage}% discount`
  }
  if (rule.adjustment.type === 'tiered_percentage') {
    const maxDiscount = Math.max(...rule.adjustment.tiers.map((t: any) => t.discount))
    return `Up to ${maxDiscount}% volume discount`
  }
  if (rule.adjustment.type === 'fixed_amount') {
    return `R ${rule.adjustment.amount} off`
  }
  return 'Custom adjustment'
}

const formatValidityPeriod = (rule: any) => {
  const start = new Date(rule.validity.startDate).toLocaleDateString('en-ZA')
  const end = new Date(rule.validity.endDate).toLocaleDateString('en-ZA')
  return `${start} - ${end}`
}

const getPriorityClass = (priority: number) => {
  if (priority <= 2) {
    return 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200'
  }
  if (priority <= 4) {
    return 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-200'
  }
  return 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300'
}
</script>