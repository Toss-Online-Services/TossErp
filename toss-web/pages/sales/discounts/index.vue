<template>
  <div class="p-6">
    <div class="flex items-center justify-between mb-6">
      <div>
        <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
          Discount Campaigns
        </h1>
        <p class="text-gray-600 dark:text-gray-400">
          Manage seasonal campaigns and special discount offers for SMME customers
        </p>
      </div>
      <button class="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg">
        <Icon name="heroicons:plus" class="w-4 h-4 mr-2" />
        Create Campaign
      </button>
    </div>

    <!-- Campaign Stats -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm">
        <div class="flex items-center">
          <div class="p-2 bg-green-100 rounded-full">
            <Icon name="heroicons:megaphone" class="w-5 h-5 text-green-600" />
          </div>
          <div class="ml-3">
            <p class="text-sm text-gray-600 dark:text-gray-400">Active Campaigns</p>
            <p class="text-lg font-semibold text-gray-900 dark:text-white">8</p>
          </div>
        </div>
      </div>
      
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm">
        <div class="flex items-center">
          <div class="p-2 bg-blue-100 rounded-full">
            <Icon name="heroicons:users" class="w-5 h-5 text-blue-600" />
          </div>
          <div class="ml-3">
            <p class="text-sm text-gray-600 dark:text-gray-400">Customers Reached</p>
            <p class="text-lg font-semibold text-gray-900 dark:text-white">2,156</p>
          </div>
        </div>
      </div>
      
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm">
        <div class="flex items-center">
          <div class="p-2 bg-purple-100 rounded-full">
            <Icon name="heroicons:banknotes" class="w-5 h-5 text-purple-600" />
          </div>
          <div class="ml-3">
            <p class="text-sm text-gray-600 dark:text-gray-400">Total Savings</p>
            <p class="text-lg font-semibold text-gray-900 dark:text-white">R 67,430</p>
          </div>
        </div>
      </div>
      
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm">
        <div class="flex items-center">
          <div class="p-2 bg-orange-100 rounded-full">
            <Icon name="heroicons:chart-bar" class="w-5 h-5 text-orange-600" />
          </div>
          <div class="ml-3">
            <p class="text-sm text-gray-600 dark:text-gray-400">Conversion Rate</p>
            <p class="text-lg font-semibold text-gray-900 dark:text-white">34.2%</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Campaign List -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm">
      <div class="p-6">
        <h2 class="text-lg font-medium text-gray-900 dark:text-white mb-4">
          Discount Campaigns
        </h2>
        
        <div class="space-y-6">
          <div v-for="campaign in mockCampaigns" :key="campaign.id" 
               class="border border-gray-200 dark:border-gray-700 rounded-lg p-6">
            <div class="flex items-start justify-between mb-4">
              <div class="flex-1">
                <div class="flex items-center mb-2">
                  <h3 class="text-lg font-medium text-gray-900 dark:text-white mr-3">
                    {{ campaign.name }}
                  </h3>
                  <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                        :class="getCampaignStatusClass(campaign.status)">
                    {{ formatCampaignStatus(campaign.status) }}
                  </span>
                </div>
                
                <p class="text-sm text-gray-600 dark:text-gray-400 mb-4">
                  {{ campaign.description }}
                </p>
                
                <!-- Campaign Details -->
                <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
                  <div class="bg-gray-50 dark:bg-gray-700 rounded-lg p-3">
                    <div class="text-sm font-medium text-gray-700 dark:text-gray-300">Discount</div>
                    <div class="text-lg font-bold text-blue-600">
                      {{ formatDiscount(campaign.discount) }}
                    </div>
                  </div>
                  
                  <div class="bg-gray-50 dark:bg-gray-700 rounded-lg p-3">
                    <div class="text-sm font-medium text-gray-700 dark:text-gray-300">Target</div>
                    <div class="text-sm text-gray-600 dark:text-gray-400">
                      {{ campaign.target.customerGroups.join(', ') }}
                    </div>
                  </div>
                  
                  <div class="bg-gray-50 dark:bg-gray-700 rounded-lg p-3">
                    <div class="text-sm font-medium text-gray-700 dark:text-gray-300">Duration</div>
                    <div class="text-sm text-gray-600 dark:text-gray-400">
                      {{ formatDateRange(campaign.validPeriod) }}
                    </div>
                  </div>
                  
                  <div class="bg-gray-50 dark:bg-gray-700 rounded-lg p-3">
                    <div class="text-sm font-medium text-gray-700 dark:text-gray-300">Usage</div>
                    <div class="text-sm text-gray-600 dark:text-gray-400">
                      {{ campaign.usage.used }} / {{ campaign.usage.limit || '∞' }}
                    </div>
                    <div class="w-full bg-gray-200 rounded-full h-1.5 mt-1 dark:bg-gray-600">
                      <div class="bg-blue-600 h-1.5 rounded-full" 
                           :style="{ width: getUsagePercentage(campaign) + '%' }"></div>
                    </div>
                  </div>
                </div>
                
                <!-- Campaign Performance -->
                <div class="mt-4 grid grid-cols-1 md:grid-cols-3 gap-4">
                  <div>
                    <span class="text-sm font-medium text-gray-700 dark:text-gray-300">Reach:</span>
                    <span class="ml-2 text-sm text-gray-600 dark:text-gray-400">
                      {{ campaign.performance.customersReached }} customers
                    </span>
                  </div>
                  
                  <div>
                    <span class="text-sm font-medium text-gray-700 dark:text-gray-300">Conversions:</span>
                    <span class="ml-2 text-sm text-gray-600 dark:text-gray-400">
                      {{ campaign.performance.conversions }} orders ({{ campaign.performance.conversionRate }}%)
                    </span>
                  </div>
                  
                  <div>
                    <span class="text-sm font-medium text-gray-700 dark:text-gray-300">Revenue Impact:</span>
                    <span class="ml-2 text-sm text-green-600">
                      R {{ campaign.performance.revenueImpact.toLocaleString() }}
                    </span>
                  </div>
                </div>
              </div>
              
              <div class="flex items-center space-x-2 ml-4">
                <button class="text-blue-600 hover:text-blue-800 text-sm px-3 py-1 border border-blue-200 rounded">
                  Analytics
                </button>
                <button class="text-green-600 hover:text-green-800 text-sm px-3 py-1 border border-green-200 rounded">
                  Edit
                </button>
                <button :class="campaign.status === 'active' ? 'text-red-600 hover:text-red-800 border-red-200' : 'text-green-600 hover:text-green-800 border-green-200'"
                        class="text-sm px-3 py-1 border rounded">
                  {{ campaign.status === 'active' ? 'Pause' : 'Activate' }}
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
  title: 'Discount Campaigns - TOSS ERP'
})

// Mock data for demonstration
const mockCampaigns = ref([
  {
    id: 'camp-001',
    name: 'Festive Season 2025',
    description: 'Special holiday discounts for spaza shops and taverns during festive season',
    status: 'active',
    discount: {
      type: 'percentage',
      value: 15,
      minimumPurchase: 1000
    },
    target: {
      customerGroups: ['spaza-shops', 'taverns'],
      territories: ['all'],
      productCategories: ['groceries', 'beverages']
    },
    validPeriod: {
      startDate: new Date('2024-12-01'),
      endDate: new Date('2025-01-31')
    },
    usage: {
      used: 234,
      limit: 500
    },
    performance: {
      customersReached: 567,
      conversions: 234,
      conversionRate: 41.3,
      revenueImpact: 28750
    }
  },
  {
    id: 'camp-002',
    name: 'Back to School Special',
    description: 'Support local spaza shops with school supplies and snacks discounts',
    status: 'scheduled',
    discount: {
      type: 'buy_x_get_y',
      buyQuantity: 5,
      getQuantity: 1,
      freeProduct: true
    },
    target: {
      customerGroups: ['spaza-shops'],
      territories: ['soweto', 'alexandra', 'diepkloof'],
      productCategories: ['snacks', 'stationery']
    },
    validPeriod: {
      startDate: new Date('2025-01-20'),
      endDate: new Date('2025-02-15')
    },
    usage: {
      used: 0,
      limit: 300
    },
    performance: {
      customersReached: 0,
      conversions: 0,
      conversionRate: 0,
      revenueImpact: 0
    }
  },
  {
    id: 'camp-003',
    name: 'Stokvel Month Promotion',
    description: 'Encourage stokvel bulk orders with additional group discounts',
    status: 'active',
    discount: {
      type: 'percentage',
      value: 20,
      minimumPurchase: 2500,
      minimumMembers: 8
    },
    target: {
      customerGroups: ['stokvels'],
      territories: ['all'],
      productCategories: ['all']
    },
    validPeriod: {
      startDate: new Date('2025-01-01'),
      endDate: new Date('2025-01-31')
    },
    usage: {
      used: 45,
      limit: 100
    },
    performance: {
      customersReached: 89,
      conversions: 45,
      conversionRate: 50.6,
      revenueImpact: 15680
    }
  },
  {
    id: 'camp-004',
    name: 'New Customer Welcome',
    description: 'First-time customer incentive to build loyalty',
    status: 'active',
    discount: {
      type: 'percentage',
      value: 25,
      minimumPurchase: 300,
      firstOrderOnly: true
    },
    target: {
      customerGroups: ['new-customers'],
      territories: ['all'],
      productCategories: ['all']
    },
    validPeriod: {
      startDate: new Date('2025-01-01'),
      endDate: new Date('2025-12-31')
    },
    usage: {
      used: 89,
      limit: null
    },
    performance: {
      customersReached: 156,
      conversions: 89,
      conversionRate: 57.1,
      revenueImpact: 23000
    }
  }
])

const formatCampaignStatus = (status: string) => {
  const statusMap: Record<string, string> = {
    active: 'Active',
    scheduled: 'Scheduled',
    paused: 'Paused',
    completed: 'Completed',
    expired: 'Expired'
  }
  return statusMap[status] || status
}

const getCampaignStatusClass = (status: string) => {
  const classMap: Record<string, string> = {
    active: 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200',
    scheduled: 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200',
    paused: 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-200',
    completed: 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300',
    expired: 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200'
  }
  return classMap[status] || 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300'
}

const formatDiscount = (discount: any) => {
  if (discount.type === 'percentage') {
    return `${discount.value}% OFF`
  }
  if (discount.type === 'fixed_amount') {
    return `R ${discount.value} OFF`
  }
  if (discount.type === 'buy_x_get_y') {
    return `Buy ${discount.buyQuantity} Get ${discount.getQuantity} FREE`
  }
  return 'Special Offer'
}

const formatDateRange = (period: any) => {
  const start = new Date(period.startDate).toLocaleDateString('en-ZA', { month: 'short', day: 'numeric' })
  const end = new Date(period.endDate).toLocaleDateString('en-ZA', { month: 'short', day: 'numeric' })
  return `${start} - ${end}`
}

const getUsagePercentage = (campaign: any) => {
  if (!campaign.usage.limit) return 0
  return Math.min(100, (campaign.usage.used / campaign.usage.limit) * 100)
}
</script>