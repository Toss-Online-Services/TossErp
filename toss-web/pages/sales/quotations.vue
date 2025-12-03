<script setup lang="ts">
import { onMounted } from 'vue'
import { useSalesStore } from '~/stores/sales'

useHead({
  title: 'Quotations - TOSS'
})

const salesStore = useSalesStore()

onMounted(() => {
  salesStore.fetchQuotations()
})

function formatCurrency(amount: number) {
  return new Intl.NumberFormat('en-ZA', {
    style: 'currency',
    currency: 'ZAR'
  }).format(amount)
}

function formatDate(date: Date) {
  return new Date(date).toLocaleDateString('en-ZA', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  })
}

function getStatusColor(status: string) {
  const colors: Record<string, string> = {
    draft: 'bg-gray-100 text-gray-800',
    sent: 'bg-blue-100 text-blue-800',
    accepted: 'bg-green-100 text-green-800',
    rejected: 'bg-red-100 text-red-800',
    expired: 'bg-orange-100 text-orange-800'
  }
  return colors[status] || 'bg-gray-100 text-gray-800'
}
</script>

<template>
  <div class="py-6">
    <!-- Page Header -->
    <div class="mb-8 flex items-center justify-between">
      <div>
        <h3 class="text-3xl font-bold text-gray-900 mb-2">Quotations</h3>
        <p class="text-gray-600 text-sm">
          Create and manage sales quotations
        </p>
      </div>
      <button class="flex items-center gap-2 px-4 py-2 bg-gradient-to-br from-blue-500 to-blue-600 text-white rounded-lg hover:shadow-lg transition-shadow">
        <i class="material-symbols-rounded text-xl">add</i>
        <span>New Quotation</span>
      </button>
    </div>

    <!-- Stats Cards -->
    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
      <div class="bg-white rounded-xl shadow-sm p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-600 mb-1">Total Quotations</p>
            <h4 class="text-2xl font-bold text-gray-900">{{ salesStore.quotations.length }}</h4>
          </div>
          <div class="w-12 h-12 rounded-lg bg-gradient-to-br from-blue-500 to-blue-600 flex items-center justify-center text-white">
            <i class="material-symbols-rounded text-2xl">description</i>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-sm p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-600 mb-1">Pending</p>
            <h4 class="text-2xl font-bold text-gray-900">{{ salesStore.pendingQuotations.length }}</h4>
          </div>
          <div class="w-12 h-12 rounded-lg bg-gradient-to-br from-orange-500 to-orange-600 flex items-center justify-center text-white">
            <i class="material-symbols-rounded text-2xl">pending</i>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-sm p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-600 mb-1">Accepted</p>
            <h4 class="text-2xl font-bold text-gray-900">
              {{ salesStore.quotations.filter(q => q.status === 'accepted').length }}
            </h4>
          </div>
          <div class="w-12 h-12 rounded-lg bg-gradient-to-br from-green-500 to-green-600 flex items-center justify-center text-white">
            <i class="material-symbols-rounded text-2xl">check_circle</i>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-sm p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-600 mb-1">Total Value</p>
            <h4 class="text-2xl font-bold text-gray-900">
              {{ formatCurrency(salesStore.quotations.reduce((sum, q) => sum + q.total, 0)) }}
            </h4>
          </div>
          <div class="w-12 h-12 rounded-lg bg-gradient-to-br from-purple-500 to-purple-600 flex items-center justify-center text-white">
            <i class="material-symbols-rounded text-2xl">payments</i>
          </div>
        </div>
      </div>
    </div>

    <!-- Quotations Table -->
    <div class="bg-white rounded-xl shadow-sm overflow-hidden">
      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-gray-200">
          <thead class="bg-gray-50">
            <tr>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Quotation #
              </th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Customer
              </th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Date
              </th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Valid Until
              </th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Total
              </th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Status
              </th>
              <th scope="col" class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                Actions
              </th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-gray-200">
            <tr v-for="quotation in salesStore.quotations" :key="quotation.id" class="hover:bg-gray-50">
              <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                {{ quotation.quotationNumber }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                {{ quotation.customerName }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                {{ formatDate(quotation.date) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                {{ formatDate(quotation.validUntil) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm font-semibold text-gray-900">
                {{ formatCurrency(quotation.total) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span :class="['px-2 py-1 inline-flex text-xs leading-5 font-semibold rounded-full', getStatusColor(quotation.status)]">
                  {{ quotation.status }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                <button class="text-blue-600 hover:text-blue-900 mr-3">
                  <i class="material-symbols-rounded text-lg">visibility</i>
                </button>
                <button class="text-gray-600 hover:text-gray-900">
                  <i class="material-symbols-rounded text-lg">more_vert</i>
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Empty State -->
      <div v-if="salesStore.quotations.length === 0" class="text-center py-12">
        <i class="material-symbols-rounded text-6xl text-gray-400 mb-4">description</i>
        <h3 class="text-lg font-medium text-gray-900 mb-2">No quotations yet</h3>
        <p class="text-gray-600 mb-4">Get started by creating your first quotation</p>
        <button class="inline-flex items-center gap-2 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700">
          <i class="material-symbols-rounded">add</i>
          <span>Create Quotation</span>
        </button>
      </div>
    </div>
  </div>
</template>

