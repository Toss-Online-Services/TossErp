<template>
  <div class="space-y-6">
    <PageHeader
      title="Payment Methods"
      description="Configure accepted payment methods and processing"
    />

    <div class="flex justify-end">
      <button class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700">
        Add Payment Method
      </button>
    </div>

    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      <div v-for="method in paymentMethods" :key="method.id" class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
        <div class="flex justify-between items-start mb-4">
          <div class="flex items-center">
            <div :class="method.iconBg" class="w-10 h-10 rounded-lg flex items-center justify-center mr-3">
              <Icon :name="method.icon" class="w-6 h-6" :class="method.iconColor" />
            </div>
            <div>
              <h3 class="font-semibold text-gray-900 dark:text-white">{{ method.name }}</h3>
              <p class="text-xs text-gray-500 dark:text-gray-400">{{ method.type }}</p>
            </div>
          </div>
          <span :class="method.isActive ? 'bg-green-100 text-green-800' : 'bg-gray-100 text-gray-800'" class="px-2 py-1 text-xs rounded-full">
            {{ method.isActive ? 'Active' : 'Inactive' }}
          </span>
        </div>

        <div class="space-y-2 mb-4 text-sm">
          <div class="flex justify-between">
            <span class="text-gray-600 dark:text-gray-400">Processing Fee:</span>
            <span class="font-medium">{{ method.fee }}%</span>
          </div>
          <div class="flex justify-between">
            <span class="text-gray-600 dark:text-gray-400">Transactions:</span>
            <span class="font-medium">{{ method.transactionCount.toLocaleString() }}</span>
          </div>
          <div class="flex justify-between">
            <span class="text-gray-600 dark:text-gray-400">Total Volume:</span>
            <span class="font-medium">R {{ method.totalVolume.toLocaleString() }}</span>
          </div>
        </div>

        <div class="flex gap-2">
          <button class="flex-1 px-3 py-2 text-sm bg-blue-50 text-blue-600 rounded hover:bg-blue-100">
            Configure
          </button>
          <button class="flex-1 px-3 py-2 text-sm bg-gray-50 text-gray-600 rounded hover:bg-gray-100">
            View Trans
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'

definePageMeta({
  layout: 'default'
})

useHead({
  title: 'Payment Methods - TOSS ERP',
})

const paymentMethods = ref([
  { id: 1, name: 'Cash', type: 'Cash Payment', icon: 'heroicons:banknotes', iconBg: 'bg-green-100', iconColor: 'text-green-600', fee: 0, transactionCount: 1245, totalVolume: 125000, isActive: true },
  { id: 2, name: 'Credit Card', type: 'Card Payment', icon: 'heroicons:credit-card', iconBg: 'bg-blue-100', iconColor: 'text-blue-600', fee: 2.5, transactionCount: 3456, totalVolume: 845000, isActive: true },
  { id: 3, name: 'Bank Transfer', type: 'Electronic Transfer', icon: 'heroicons:building-library', iconBg: 'bg-purple-100', iconColor: 'text-purple-600', fee: 0.5, transactionCount: 892, totalVolume: 1250000, isActive: true },
  { id: 4, name: 'Mobile Money', type: 'Mobile Payment', icon: 'heroicons:device-phone-mobile', iconBg: 'bg-orange-100', iconColor: 'text-orange-600', fee: 1.5, transactionCount: 2134, totalVolume: 342000, isActive: true },
  { id: 5, name: 'PayPal', type: 'Online Payment', icon: 'heroicons:globe-alt', iconBg: 'bg-blue-100', iconColor: 'text-blue-600', fee: 3.4, transactionCount: 567, totalVolume: 178000, isActive: false },
])
</script>
