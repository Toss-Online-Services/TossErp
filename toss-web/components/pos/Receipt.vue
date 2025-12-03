<script setup lang="ts">
import type { Sale } from '~/stores/pos'

interface Props {
  sale: Sale
  show?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  show: false
})

const emit = defineEmits<{
  close: []
  print: []
}>()

function formatCurrency(amount: number) {
  return new Intl.NumberFormat('en-ZA', {
    style: 'currency',
    currency: 'ZAR'
  }).format(amount)
}

function formatDate(date: Date) {
  return new Intl.DateTimeFormat('en-ZA', {
    year: 'numeric',
    month: 'long',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  }).format(new Date(date))
}

function handlePrint() {
  emit('print')
  window.print()
}

function handleClose() {
  emit('close')
}
</script>

<template>
  <div
    v-if="show"
    class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4 print:p-0 print:bg-white"
    @click.self="handleClose"
  >
    <div class="bg-white rounded-xl max-w-md w-full max-h-[90vh] overflow-y-auto print:max-h-none print:rounded-none print:shadow-none shadow-2xl">
      <!-- Receipt Header -->
      <div class="p-6 border-b print:border-b-2">
        <div class="flex items-center justify-between mb-4 print:hidden">
          <h2 class="text-2xl font-bold text-gray-900">Receipt</h2>
          <div class="flex items-center gap-2">
            <button
              @click="handlePrint"
              class="p-2 text-gray-600 hover:bg-gray-100 rounded-lg transition-colors"
            >
              <i class="material-symbols-rounded">print</i>
            </button>
            <button
              @click="handleClose"
              class="p-2 text-gray-400 hover:text-gray-600 rounded-lg transition-colors"
            >
              <i class="material-symbols-rounded">close</i>
            </button>
          </div>
        </div>

        <!-- Business Info -->
        <div class="text-center mb-4">
          <h3 class="text-xl font-bold text-gray-900 mb-1">TOSS ERP</h3>
          <p class="text-sm text-gray-600">The One-Stop Solution</p>
          <p class="text-xs text-gray-500 mt-2">123 Main Street, Township, SA</p>
          <p class="text-xs text-gray-500">Tel: +27 11 123 4567</p>
        </div>

        <!-- Invoice Details -->
        <div class="mt-4 pt-4 border-t">
          <div class="flex justify-between text-sm mb-2">
            <span class="text-gray-600">Invoice:</span>
            <span class="font-semibold text-gray-900">{{ sale.invoiceNumber }}</span>
          </div>
          <div class="flex justify-between text-sm mb-2">
            <span class="text-gray-600">Date:</span>
            <span class="font-semibold text-gray-900">{{ formatDate(sale.createdAt) }}</span>
          </div>
          <div v-if="sale.customerName" class="flex justify-between text-sm">
            <span class="text-gray-600">Customer:</span>
            <span class="font-semibold text-gray-900">{{ sale.customerName }}</span>
          </div>
        </div>
      </div>

      <!-- Receipt Items -->
      <div class="p-6">
        <div class="space-y-3 mb-4">
          <div
            v-for="item in sale.items"
            :key="item.id"
            class="flex justify-between items-start pb-3 border-b border-gray-100"
          >
            <div class="flex-1">
              <p class="font-semibold text-gray-900 text-sm">{{ item.name }}</p>
              <p class="text-xs text-gray-600">{{ item.code }}</p>
              <div class="flex items-center gap-2 mt-1">
                <span class="text-xs text-gray-500">{{ item.quantity }} × {{ formatCurrency(item.price) }}</span>
                <span v-if="item.discount > 0" class="text-xs text-red-600">-{{ formatCurrency(item.discount) }}</span>
              </div>
            </div>
            <div class="text-right">
              <p class="font-bold text-gray-900">{{ formatCurrency(item.total) }}</p>
            </div>
          </div>
        </div>

        <!-- Totals -->
        <div class="space-y-2 pt-4 border-t">
          <div class="flex justify-between text-sm">
            <span class="text-gray-600">Subtotal:</span>
            <span class="font-semibold text-gray-900">{{ formatCurrency(sale.subtotal) }}</span>
          </div>
          <div v-if="sale.discount > 0" class="flex justify-between text-sm">
            <span class="text-gray-600">Discount:</span>
            <span class="font-semibold text-red-600">-{{ formatCurrency(sale.discount) }}</span>
          </div>
          <div class="flex justify-between text-sm">
            <span class="text-gray-600">VAT (15%):</span>
            <span class="font-semibold text-gray-900">{{ formatCurrency(sale.tax) }}</span>
          </div>
          <div class="flex justify-between text-lg font-bold pt-2 border-t">
            <span>Total:</span>
            <span class="text-blue-600">{{ formatCurrency(sale.total) }}</span>
          </div>
        </div>

        <!-- Payments -->
        <div class="mt-4 pt-4 border-t">
          <h4 class="text-sm font-semibold text-gray-700 mb-2">Payment</h4>
          <div
            v-for="(payment, index) in sale.payments"
            :key="index"
            class="flex justify-between text-sm mb-1"
          >
            <span class="text-gray-600 capitalize">{{ payment.type }}:</span>
            <span class="font-semibold text-gray-900">{{ formatCurrency(payment.amount) }}</span>
          </div>
          <div v-if="sale.change > 0" class="flex justify-between text-sm mt-2 pt-2 border-t">
            <span class="text-gray-600">Change:</span>
            <span class="font-semibold text-green-600">{{ formatCurrency(sale.change) }}</span>
          </div>
        </div>
      </div>

      <!-- Footer -->
      <div class="p-6 border-t text-center print:border-t-2">
        <p class="text-xs text-gray-500 mb-2">Thank you for your business!</p>
        <p class="text-xs text-gray-400">This is a computer-generated receipt</p>
      </div>
    </div>
  </div>
</template>

<style scoped>
@media print {
  @page {
    size: auto;
    margin: 0;
  }
  
  body {
    margin: 0;
    padding: 0;
  }
}
</style>

