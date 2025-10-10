<template>
  <div class="space-y-6">
    <PageHeader
      title="VAT Report"
      description="South African VAT Return (15% Standard Rate)"
    />

    <!-- Period Selection -->
    <div class="flex flex-wrap gap-4 items-center justify-between">
      <div class="flex gap-4 items-center">
        <div>
          <label class="block text-sm font-medium mb-1">Start Date</label>
          <input
            v-model="startDate"
            type="date"
            class="px-4 py-2 border rounded-lg"
          />
        </div>
        <div>
          <label class="block text-sm font-medium mb-1">End Date</label>
          <input
            v-model="endDate"
            type="date"
            class="px-4 py-2 border rounded-lg"
          />
        </div>
      </div>

      <div class="flex gap-2">
        <ExportButton
          :data="vatReport"
          filename="vat-report"
          :formats="['pdf', 'excel', 'csv']"
        />
        <button
          class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700"
          @click="loadVATReport"
        >
          Generate Report
        </button>
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="isLoading" class="text-center py-12">
      <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto"></div>
      <p class="mt-4 text-gray-600">Generating VAT report...</p>
    </div>

    <!-- Error State -->
    <div v-else-if="error" class="bg-red-50 border border-red-200 rounded-lg p-4">
      <p class="text-red-800">{{ error }}</p>
    </div>

    <!-- VAT Report Content -->
    <div v-else-if="vatReport" class="space-y-6">
      <!-- Summary Cards -->
      <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
        <div class="bg-white rounded-lg shadow p-6">
          <p class="text-sm text-gray-600 mb-1">Output VAT (Sales)</p>
          <p class="text-2xl font-bold text-blue-600">
            {{ formatCurrency(vatReport.sales.total.vatAmount) }}
          </p>
        </div>
        <div class="bg-white rounded-lg shadow p-6">
          <p class="text-sm text-gray-600 mb-1">Input VAT (Purchases)</p>
          <p class="text-2xl font-bold text-green-600">
            {{ formatCurrency(vatReport.purchases.total.vatAmount) }}
          </p>
        </div>
        <div class="bg-white rounded-lg shadow p-6">
          <p class="text-sm text-gray-600 mb-1">Net VAT</p>
          <p class="text-2xl font-bold" :class="vatReport.netVAT >= 0 ? 'text-red-600' : 'text-green-600'">
            {{ formatCurrency(vatReport.netVAT) }}
          </p>
        </div>
        <div class="bg-white rounded-lg shadow p-6">
          <p class="text-sm text-gray-600 mb-1">Total Payable</p>
          <p class="text-2xl font-bold text-red-600">
            {{ formatCurrency(vatReport.totalPayable) }}
          </p>
        </div>
      </div>

      <!-- Sales VAT -->
      <div class="bg-white rounded-lg shadow p-6">
        <h2 class="text-xl font-bold mb-4">Output VAT (Sales)</h2>
        
        <div class="space-y-4">
          <!-- Standard Rate Sales -->
          <div>
            <h3 class="text-lg font-semibold mb-2 text-gray-700">Standard Rate (15%)</h3>
            <div class="grid grid-cols-4 gap-4 text-sm">
              <div>
                <p class="text-gray-600">Transactions</p>
                <p class="font-semibold">{{ vatReport.sales.standard.count }}</p>
              </div>
              <div>
                <p class="text-gray-600">Subtotal</p>
                <p class="font-semibold">{{ formatCurrency(vatReport.sales.standard.subtotal) }}</p>
              </div>
              <div>
                <p class="text-gray-600">VAT Amount</p>
                <p class="font-semibold">{{ formatCurrency(vatReport.sales.standard.vatAmount) }}</p>
              </div>
              <div>
                <p class="text-gray-600">Total</p>
                <p class="font-semibold">{{ formatCurrency(vatReport.sales.standard.total) }}</p>
              </div>
            </div>
          </div>

          <!-- Zero-Rated Sales -->
          <div>
            <h3 class="text-lg font-semibold mb-2 text-gray-700">Zero-Rated (0%)</h3>
            <div class="grid grid-cols-4 gap-4 text-sm">
              <div>
                <p class="text-gray-600">Transactions</p>
                <p class="font-semibold">{{ vatReport.sales.zeroRated.count }}</p>
              </div>
              <div>
                <p class="text-gray-600">Subtotal</p>
                <p class="font-semibold">{{ formatCurrency(vatReport.sales.zeroRated.subtotal) }}</p>
              </div>
              <div>
                <p class="text-gray-600">VAT Amount</p>
                <p class="font-semibold">{{ formatCurrency(vatReport.sales.zeroRated.vatAmount) }}</p>
              </div>
              <div>
                <p class="text-gray-600">Total</p>
                <p class="font-semibold">{{ formatCurrency(vatReport.sales.zeroRated.total) }}</p>
              </div>
            </div>
          </div>

          <!-- Exempt Sales -->
          <div>
            <h3 class="text-lg font-semibold mb-2 text-gray-700">Exempt</h3>
            <div class="grid grid-cols-4 gap-4 text-sm">
              <div>
                <p class="text-gray-600">Transactions</p>
                <p class="font-semibold">{{ vatReport.sales.exempt.count }}</p>
              </div>
              <div>
                <p class="text-gray-600">Subtotal</p>
                <p class="font-semibold">{{ formatCurrency(vatReport.sales.exempt.subtotal) }}</p>
              </div>
              <div>
                <p class="text-gray-600">VAT Amount</p>
                <p class="font-semibold">{{ formatCurrency(vatReport.sales.exempt.vatAmount) }}</p>
              </div>
              <div>
                <p class="text-gray-600">Total</p>
                <p class="font-semibold">{{ formatCurrency(vatReport.sales.exempt.total) }}</p>
              </div>
            </div>
          </div>

          <div class="border-t-2 pt-4">
            <div class="grid grid-cols-4 gap-4 font-bold">
              <div>Total: {{ vatReport.sales.total.count }}</div>
              <div>{{ formatCurrency(vatReport.sales.total.subtotal) }}</div>
              <div>{{ formatCurrency(vatReport.sales.total.vatAmount) }}</div>
              <div>{{ formatCurrency(vatReport.sales.total.total) }}</div>
            </div>
          </div>
        </div>
      </div>

      <!-- Purchases VAT -->
      <div class="bg-white rounded-lg shadow p-6">
        <h2 class="text-xl font-bold mb-4">Input VAT (Purchases)</h2>
        
        <div class="space-y-4">
          <!-- Standard Rate Purchases -->
          <div>
            <h3 class="text-lg font-semibold mb-2 text-gray-700">Standard Rate (15%)</h3>
            <div class="grid grid-cols-4 gap-4 text-sm">
              <div>
                <p class="text-gray-600">Transactions</p>
                <p class="font-semibold">{{ vatReport.purchases.standard.count }}</p>
              </div>
              <div>
                <p class="text-gray-600">Subtotal</p>
                <p class="font-semibold">{{ formatCurrency(vatReport.purchases.standard.subtotal) }}</p>
              </div>
              <div>
                <p class="text-gray-600">VAT Amount</p>
                <p class="font-semibold">{{ formatCurrency(vatReport.purchases.standard.vatAmount) }}</p>
              </div>
              <div>
                <p class="text-gray-600">Total</p>
                <p class="font-semibold">{{ formatCurrency(vatReport.purchases.standard.total) }}</p>
              </div>
            </div>
          </div>

          <!-- Zero-Rated Purchases -->
          <div>
            <h3 class="text-lg font-semibold mb-2 text-gray-700">Zero-Rated (0%)</h3>
            <div class="grid grid-cols-4 gap-4 text-sm">
              <div>
                <p class="text-gray-600">Transactions</p>
                <p class="font-semibold">{{ vatReport.purchases.zeroRated.count }}</p>
              </div>
              <div>
                <p class="text-gray-600">Subtotal</p>
                <p class="font-semibold">{{ formatCurrency(vatReport.purchases.zeroRated.subtotal) }}</p>
              </div>
              <div>
                <p class="text-gray-600">VAT Amount</p>
                <p class="font-semibold">{{ formatCurrency(vatReport.purchases.zeroRated.vatAmount) }}</p>
              </div>
              <div>
                <p class="text-gray-600">Total</p>
                <p class="font-semibold">{{ formatCurrency(vatReport.purchases.zeroRated.total) }}</p>
              </div>
            </div>
          </div>

          <div class="border-t-2 pt-4">
            <div class="grid grid-cols-4 gap-4 font-bold">
              <div>Total: {{ vatReport.purchases.total.count }}</div>
              <div>{{ formatCurrency(vatReport.purchases.total.subtotal) }}</div>
              <div>{{ formatCurrency(vatReport.purchases.total.vatAmount) }}</div>
              <div>{{ formatCurrency(vatReport.purchases.total.total) }}</div>
            </div>
          </div>
        </div>
      </div>

      <!-- VAT Summary -->
      <div class="bg-white rounded-lg shadow p-6">
        <h2 class="text-xl font-bold mb-4">VAT Summary</h2>
        
        <div class="space-y-3">
          <div class="flex justify-between text-lg">
            <span>Output VAT (Sales)</span>
            <span class="font-semibold">{{ formatCurrency(vatReport.sales.total.vatAmount) }}</span>
          </div>
          <div class="flex justify-between text-lg">
            <span>Input VAT (Purchases)</span>
            <span class="font-semibold">{{ formatCurrency(vatReport.purchases.total.vatAmount) }}</span>
          </div>
          <div class="border-t-2 pt-3 flex justify-between text-xl font-bold">
            <span>Net VAT {{ vatReport.netVAT >= 0 ? 'Payable' : 'Refundable' }}</span>
            <span :class="vatReport.netVAT >= 0 ? 'text-red-600' : 'text-green-600'">
              {{ formatCurrency(Math.abs(vatReport.netVAT)) }}
            </span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { definePageMeta, useHead } from '#imports'
import type { VATReport } from '~/types/vat'
import { useVAT } from '~/composables/useVAT'

definePageMeta({
  middleware: ['auth'],
  layout: 'default',
})

useHead({
  title: 'VAT Report - TOSS ERP',
})

const { getVATReport, isLoading, error } = useVAT()

// Default to current month
const now = new Date()
const startDate = ref(new Date(now.getFullYear(), now.getMonth(), 1).toISOString().split('T')[0])
const endDate = ref(new Date(now.getFullYear(), now.getMonth() + 1, 0).toISOString().split('T')[0])
const vatReport = ref<VATReport | null>(null)

const formatCurrency = (amount: number): string => {
  return new Intl.NumberFormat('en-ZA', {
    style: 'currency',
    currency: 'ZAR',
  }).format(amount)
}

const loadVATReport = async () => {
  const start = new Date(startDate.value)
  const end = new Date(endDate.value)
  vatReport.value = await getVATReport(start, end)
}

onMounted(() => {
  loadVATReport()
})
</script>

