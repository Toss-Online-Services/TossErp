<template>
  <div v-if="show" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
    <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-2xl max-w-4xl w-full max-h-[80vh] overflow-hidden flex flex-col">
      <!-- Header -->
      <div class="bg-gradient-to-r from-green-600 to-blue-600 px-6 py-4 flex items-center justify-between">
        <div>
          <h3 class="text-xl font-bold text-white">Order Queue</h3>
          <p class="text-white/80 text-sm">Manage pending orders</p>
        </div>
        <button @click="$emit('close')" class="text-white hover:bg-white/20 rounded-lg p-2 transition-colors">
          <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
          </svg>
        </button>
      </div>

      <!-- Order List -->
      <div class="flex-1 overflow-y-auto p-6">
        <div v-if="orders.length === 0" class="text-center py-12">
          <div class="text-6xl mb-4">📋</div>
          <p class="text-slate-600 dark:text-slate-400">No pending orders</p>
        </div>
        <div v-else class="space-y-4">
          <div v-for="order in orders" :key="order.id" 
            class="bg-slate-50 dark:bg-slate-700 rounded-xl p-4 border-2 border-slate-200 dark:border-slate-600"
          >
            <div class="flex items-start justify-between mb-3">
              <div>
                <div class="flex items-center gap-2 mb-1">
                  <span class="font-bold text-slate-900 dark:text-white">Order #{{ order.orderNumber }}</span>
                  <span 
                    :class="[
                      'px-2 py-1 rounded-full text-xs font-bold',
                      order.status === 'pending' ? 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-400' :
                      order.status === 'in-progress' ? 'bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400' :
                      order.status === 'ready' ? 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400' :
                      'bg-slate-100 text-slate-800'
                    ]"
                  >
                    {{ order.status === 'pending' ? '⏳ Pending' : order.status === 'in-progress' ? '⚙️ In Progress' : '✅ Ready' }}
                  </span>
                </div>
                <p class="text-sm text-slate-600 dark:text-slate-400">{{ order.customerName }} {{ order.customerPhone ? `• ${order.customerPhone}` : '' }}</p>
                <p class="text-xs text-slate-500 dark:text-slate-500 mt-1">{{ formatTime(order.createdAt) }}</p>
              </div>
              <div class="text-right">
                <p class="text-lg font-bold text-slate-900 dark:text-white">R{{ formatCurrency(order.total) }}</p>
                <p class="text-xs text-slate-600 dark:text-slate-400">{{ order.items.length }} items</p>
              </div>
            </div>

            <!-- Items -->
            <div class="mb-3 space-y-1">
              <div v-for="item in order.items" :key="item.id" class="text-sm text-slate-700 dark:text-slate-300">
                {{ item.quantity }}x {{ item.name }}
              </div>
            </div>

            <!-- Notes -->
            <div v-if="order.notes" class="mb-3 text-sm text-slate-600 dark:text-slate-400 italic bg-white dark:bg-slate-800 rounded px-2 py-1">
              "{{ order.notes }}"
            </div>

            <!-- Status Actions -->
            <div class="flex gap-2">
              <button 
                v-if="order.status === 'pending'"
                @click="$emit('updateStatus', order.id, 'in-progress')"
                class="flex-1 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg text-sm font-semibold transition-colors"
              >
                ⚙️ Start
              </button>
              <button 
                v-if="order.status === 'in-progress'"
                @click="$emit('updateStatus', order.id, 'ready')"
                class="flex-1 py-2 bg-green-600 hover:bg-green-700 text-white rounded-lg text-sm font-semibold transition-colors"
              >
                ✅ Mark Ready
              </button>
              <button 
                v-if="order.status === 'ready'"
                @click="$emit('complete', order.id)"
                class="flex-1 py-2 bg-emerald-600 hover:bg-emerald-700 text-white rounded-lg text-sm font-semibold transition-colors"
              >
                ✓ Complete
              </button>
              <button 
                @click="$emit('cancel', order.id)"
                class="py-2 px-4 bg-red-600 hover:bg-red-700 text-white rounded-lg text-sm font-semibold transition-colors"
              >
                ✕ Cancel
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
interface OrderItem {
  id: number | string
  name: string
  quantity: number
  price: number
}

interface Order {
  id: string
  orderNumber: string
  customerName: string
  customerPhone: string
  items: OrderItem[]
  total: number
  notes: string
  status: 'pending' | 'in-progress' | 'ready'
  createdAt: Date
}

interface Props {
  show: boolean
  orders: Order[]
}

defineProps<Props>()

defineEmits<{
  'close': []
  'updateStatus': [id: string, status: string]
  'complete': [id: string]
  'cancel': [id: string]
}>()

const formatTime = (date: Date) => {
  return new Date(date).toLocaleTimeString('en-ZA', { 
    hour: '2-digit', 
    minute: '2-digit' 
  })
}

const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-ZA', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  }).format(amount)
}
</script>

