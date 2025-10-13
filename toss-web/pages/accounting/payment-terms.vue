<template>
  <div class="space-y-6">
    <PageHeader
      title="Payment Terms"
      description="Define payment terms and credit conditions"
    />

    <div class="flex justify-end">
      <button class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700">
        Add Payment Term
      </button>
    </div>

    <div class="bg-white dark:bg-gray-800 rounded-lg shadow">
      <table class="w-full">
        <thead class="bg-gray-50 dark:bg-gray-700">
          <tr>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Term Name</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Credit Days</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Discount</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Usage Count</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Status</th>
            <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Actions</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-gray-200 dark:divide-gray-700">
          <tr v-for="term in paymentTerms" :key="term.id" class="dark:text-white">
            <td class="px-6 py-4">
              <div>
                <p class="font-medium">{{ term.name }}</p>
                <p class="text-sm text-gray-500 dark:text-gray-400">{{ term.description }}</p>
              </div>
            </td>
            <td class="px-6 py-4">{{ term.creditDays }} days</td>
            <td class="px-6 py-4">
              <span v-if="term.discountPercent">
                {{ term.discountPercent }}% in {{ term.discountDays }} days
              </span>
              <span v-else class="text-gray-400">-</span>
            </td>
            <td class="px-6 py-4">{{ term.usageCount.toLocaleString() }}</td>
            <td class="px-6 py-4">
              <span :class="term.isActive ? 'bg-green-100 text-green-800' : 'bg-gray-100 text-gray-800'" class="px-2 py-1 text-xs rounded-full">
                {{ term.isActive ? 'Active' : 'Inactive' }}
              </span>
            </td>
            <td class="px-6 py-4 text-right">
              <button class="text-blue-600 hover:text-blue-800 mr-3">Edit</button>
              <button class="text-red-600 hover:text-red-800">Delete</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'

definePageMeta({
  layout: 'default'
})

useHead({
  title: 'Payment Terms - TOSS ERP',
})

const paymentTerms = ref([
  { id: 1, name: 'Net 30', description: 'Payment due in 30 days', creditDays: 30, discountPercent: null, discountDays: null, usageCount: 245, isActive: true },
  { id: 2, name: '2/10 Net 30', description: '2% discount if paid within 10 days, net 30', creditDays: 30, discountPercent: 2, discountDays: 10, usageCount: 89, isActive: true },
  { id: 3, name: 'Net 60', description: 'Payment due in 60 days', creditDays: 60, discountPercent: null, discountDays: null, usageCount: 123, isActive: true },
  { id: 4, name: 'Immediate', description: 'Payment due upon receipt', creditDays: 0, discountPercent: null, discountDays: null, usageCount: 567, isActive: true },
  { id: 5, name: 'Net 90', description: 'Payment due in 90 days', creditDays: 90, discountPercent: null, discountDays: null, usageCount: 45, isActive: true },
])
</script>
