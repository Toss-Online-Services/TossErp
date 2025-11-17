<template>
  <div class="p-6">
    <div class="flex items-center justify-between mb-6">
      <div>
        <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
          Promotional Schemes
        </h1>
        <p class="text-gray-600 dark:text-gray-400">
          Manage volume discounts and promotional offers for SMME customers
        </p>
      </div>
      <button class="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg">
        <Icon name="heroicons:plus" class="w-4 h-4 mr-2" />
        New Promotion
      </button>
    </div>

    <!-- Stats -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm">
        <div class="flex items-center">
          <div class="p-2 bg-green-100 rounded-full">
            <Icon name="heroicons:gift" class="w-5 h-5 text-green-600" />
          </div>
          <div class="ml-3">
            <p class="text-sm text-gray-600 dark:text-gray-400">Active Promotions</p>
            <p class="text-lg font-semibold text-gray-900 dark:text-white">12</p>
          </div>
        </div>
      </div>
      
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm">
        <div class="flex items-center">
          <div class="p-2 bg-blue-100 rounded-full">
            <Icon name="heroicons:users" class="w-5 h-5 text-blue-600" />
          </div>
          <div class="ml-3">
            <p class="text-sm text-gray-600 dark:text-gray-400">Group Discounts</p>
            <p class="text-lg font-semibold text-gray-900 dark:text-white">8</p>
          </div>
        </div>
      </div>
      
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm">
        <div class="flex items-center">
          <div class="p-2 bg-purple-100 rounded-full">
            <Icon name="heroicons:chart-bar" class="w-5 h-5 text-purple-600" />
          </div>
          <div class="ml-3">
            <p class="text-sm text-gray-600 dark:text-gray-400">Total Usage</p>
            <p class="text-lg font-semibold text-gray-900 dark:text-white">245</p>
          </div>
        </div>
      </div>
      
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm">
        <div class="flex items-center">
          <div class="p-2 bg-orange-100 rounded-full">
            <Icon name="heroicons:banknotes" class="w-5 h-5 text-orange-600" />
          </div>
          <div class="ml-3">
            <p class="text-sm text-gray-600 dark:text-gray-400">Total Savings</p>
            <p class="text-lg font-semibold text-gray-900 dark:text-white">R 15,240</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Promotions List -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm">
      <div class="p-6">
        <h2 class="text-lg font-medium text-gray-900 dark:text-white mb-4">
          Current Promotional Schemes
        </h2>
        
        <div class="space-y-4">
          <div v-for="promotion in mockPromotions" :key="promotion.id" 
               class="border border-gray-200 dark:border-gray-700 rounded-lg p-4">
            <div class="flex items-center justify-between">
              <div class="flex-1">
                <h3 class="font-medium text-gray-900 dark:text-white">
                  {{ promotion.name }}
                </h3>
                <p class="text-sm text-gray-600 dark:text-gray-400 mt-1">
                  {{ promotion.description }}
                </p>
                <div class="flex items-center mt-2 space-x-4">
                  <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                        :class="promotion.validity.isActive ? 
                          'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200' : 
                          'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300'">
                    {{ promotion.validity.isActive ? 'Active' : 'Inactive' }}
                  </span>
                  <span class="text-sm text-gray-500">
                    Type: {{ formatType(promotion.type) }}
                  </span>
                  <span class="text-sm text-gray-500">
                    Used: {{ promotion.limits.currentUses }} / {{ promotion.limits.maxUsesTotal || '∞' }}
                  </span>
                </div>
              </div>
              <div class="text-right">
                <div class="text-lg font-bold text-blue-600">
                  {{ formatDiscount(promotion) }}
                </div>
                <div class="text-sm text-gray-500">
                  Valid until {{ formatDate(promotion.validity.validTo) }}
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
import { ref } from 'vue'

useHead({
  title: 'Promotional Schemes - TOSS ERP'
})

// Mock data for demonstration
const mockPromotions = ref([
  {
    id: '1',
    name: 'Spaza Shop Volume Discount',
    description: 'Progressive discounts for high-volume purchases by spaza shops',
    type: 'bulk_discount',
    validity: {
      validFrom: new Date('2025-01-01'),
      validTo: new Date('2025-12-31'),
      isActive: true
    },
    applicableCustomerGroups: ['spaza-shops'],
    applicableTerritories: ['soweto', 'alexandra'],
    conditions: {
      minimumQuantity: 10,
      minimumAmount: 500
    },
    discount: {
      tieredDiscounts: [
        { threshold: 10, discount: 5 },
        { threshold: 25, discount: 10 },
        { threshold: 50, discount: 15 }
      ]
    },
    limits: {
      maxUsesPerCustomer: 5,
      maxUsesTotal: 100,
      currentUses: 45
    },
    statistics: {
      totalSaved: 12500,
      timesUsed: 45,
      averageOrderValue: 1250
    }
  },
  {
    id: '2',
    name: 'Stokvel Group Buying Special',
    description: 'Special pricing for stokvel bulk purchases with delivery',
    type: 'group_discount',
    validity: {
      validFrom: new Date('2025-01-15'),
      validTo: new Date('2025-06-30'),
      isActive: true
    },
    applicableCustomerGroups: ['stokvels'],
    applicableTerritories: ['all'],
    conditions: {
      minimumQuantity: 100,
      minimumAmount: 2000,
      minimumGroupSize: 5
    },
    discount: {
      percentage: 12
    },
    limits: {
      maxUsesPerCustomer: 2,
      maxUsesTotal: 50,
      currentUses: 8
    },
    statistics: {
      totalSaved: 8750,
      timesUsed: 8,
      averageOrderValue: 3200
    }
  },
  {
    id: '3',
    name: 'New Customer Welcome Offer',
    description: '20% off first purchase for new township businesses',
    type: 'percentage',
    validity: {
      validFrom: new Date('2025-01-01'),
      validTo: new Date('2025-03-31'),
      isActive: true
    },
    applicableCustomerGroups: ['new-customers'],
    applicableTerritories: ['all'],
    conditions: {
      minimumAmount: 200
    },
    discount: {
      percentage: 20
    },
    limits: {
      maxUsesPerCustomer: 1,
      currentUses: 23
    },
    statistics: {
      totalSaved: 4600,
      timesUsed: 23,
      averageOrderValue: 450
    }
  }
])

const formatType = (type: string) => {
  const typeMap: Record<string, string> = {
    percentage: 'Percentage Discount',
    amount: 'Fixed Amount',
    buy_x_get_y: 'Buy X Get Y',
    bulk_discount: 'Volume Discount',
    group_discount: 'Group Discount'
  }
  return typeMap[type] || type
}

const formatDiscount = (promotion: any) => {
  if (promotion.discount.percentage) {
    return `${promotion.discount.percentage}% OFF`
  }
  if (promotion.discount.fixedAmount) {
    return `R ${promotion.discount.fixedAmount} OFF`
  }
  if (promotion.discount.tieredDiscounts) {
    const maxDiscount = Math.max(...promotion.discount.tieredDiscounts.map((t: any) => t.discount))
    return `Up to ${maxDiscount}% OFF`
  }
  return 'Special Offer'
}

const formatDate = (date: Date) => {
  return new Date(date).toLocaleDateString('en-ZA')
}
</script>