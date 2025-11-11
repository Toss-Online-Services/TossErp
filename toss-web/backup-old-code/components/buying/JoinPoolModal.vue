<template>
  <div v-if="show && pool" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
    <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-xl max-w-lg w-full">
      <div class="p-6 border-b border-slate-200 dark:border-slate-700">
        <h3 class="text-2xl font-semibold text-slate-900 dark:text-white">Join Pool</h3>
        <p class="text-sm text-slate-600 dark:text-slate-400 mt-1">{{ pool.productName }}</p>
      </div>

      <form @submit.prevent="handleSubmit" class="p-6">
        <div class="space-y-6">
          <!-- Pool Summary -->
          <div class="bg-gradient-to-r from-orange-50 to-purple-50 dark:from-orange-900/20 dark:to-purple-900/20 rounded-xl p-4">
            <div class="grid grid-cols-2 gap-4">
              <div>
                <p class="text-xs text-slate-600 dark:text-slate-400">Unit Price</p>
                <p class="text-lg font-bold text-slate-900 dark:text-white">R{{ pool.unitPrice.toFixed(2) }}</p>
                <p class="text-xs text-green-600 dark:text-green-400">-{{ pool.savingsPercent }}% vs solo</p>
              </div>
              <div>
                <p class="text-xs text-slate-600 dark:text-slate-400">Progress</p>
                <p class="text-lg font-bold text-slate-900 dark:text-white">
                  {{ pool.currentQuantity }}/{{ pool.targetQuantity }} {{ pool.unit }}
                </p>
                <p class="text-xs text-slate-600 dark:text-slate-400">
                  {{ pool.participants }} participants
                </p>
              </div>
            </div>
          </div>

          <!-- Quantity Input -->
          <div>
            <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
              How many {{ pool.unit }} do you want?
            </label>
            <input 
              v-model.number="quantity"
              type="number"
              min="1"
              :max="pool.targetQuantity - pool.currentQuantity"
              required
              class="w-full px-4 py-3 border border-slate-300 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-orange-500 dark:bg-slate-700 dark:text-white text-lg font-semibold"
            />
            <p class="text-xs text-slate-500 dark:text-slate-400 mt-1">
              Available: {{ pool.targetQuantity - pool.currentQuantity }} {{ pool.unit }}
            </p>
          </div>

          <!-- Cost Breakdown -->
          <div class="bg-slate-50 dark:bg-slate-700/50 rounded-xl p-4 space-y-3">
            <div class="flex justify-between">
              <span class="text-sm text-slate-600 dark:text-slate-400">Your quantity:</span>
              <span class="text-sm font-medium text-slate-900 dark:text-white">{{ quantity }} {{ pool.unit }}</span>
            </div>
            <div class="flex justify-between">
              <span class="text-sm text-slate-600 dark:text-slate-400">Unit price:</span>
              <span class="text-sm font-medium text-slate-900 dark:text-white">R{{ pool.unitPrice.toFixed(2) }}</span>
            </div>
            <div class="flex justify-between pt-3 border-t border-slate-200 dark:border-slate-600">
              <span class="text-base font-semibold text-slate-900 dark:text-white">Your share:</span>
              <span class="text-base font-bold text-slate-900 dark:text-white">R{{ totalCost.toFixed(2) }}</span>
            </div>
            <div v-if="savings > 0" class="flex justify-between pt-2 border-t border-green-200 dark:border-green-800">
              <span class="text-sm font-semibold text-green-600 dark:text-green-400">💰 You save:</span>
              <span class="text-sm font-bold text-green-600 dark:text-green-400">R{{ savings.toFixed(2) }}</span>
            </div>
          </div>

          <!-- Payment Info -->
          <div class="bg-blue-50 dark:bg-blue-900/20 rounded-xl p-4">
            <p class="text-sm text-blue-800 dark:text-blue-400">
              <strong>Payment:</strong> You'll receive a payment link via WhatsApp when the pool is confirmed.
            </p>
          </div>

          <!-- Delivery Info -->
          <div class="bg-purple-50 dark:bg-purple-900/20 rounded-xl p-4">
            <p class="text-sm text-purple-800 dark:text-purple-400">
              <strong>Delivery:</strong> Shared delivery with other participants. You'll be notified of the delivery schedule.
            </p>
          </div>
        </div>

        <!-- Actions -->
        <div class="flex justify-end space-x-3 mt-8 pt-6 border-t border-slate-200 dark:border-slate-700">
          <button 
            type="button"
            @click="$emit('close')"
            class="px-6 py-3 text-slate-600 dark:text-slate-400 hover:text-slate-800 dark:hover:text-slate-200 font-medium"
          >
            Cancel
          </button>
          <button 
            type="submit"
            class="px-8 py-3 bg-gradient-to-r from-orange-600 to-purple-600 hover:from-orange-700 hover:to-purple-700 text-white rounded-xl font-semibold shadow-lg hover:shadow-xl transition-all duration-200"
          >
            Join Pool & Save R{{ savings.toFixed(2) }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'

const props = defineProps<{
  show: boolean
  pool: any
}>()

const emit = defineEmits<{
  close: []
  join: [poolId: string, quantity: number]
}>()

const quantity = ref(1)

const totalCost = computed(() => {
  return quantity.value * (props.pool?.unitPrice || 0)
})

const savings = computed(() => {
  if (!props.pool) return 0
  const soloPrice = props.pool.unitPrice / (1 - props.pool.savingsPercent / 100)
  return (soloPrice - props.pool.unitPrice) * quantity.value
})

const handleSubmit = () => {
  emit('join', props.pool.id, quantity.value)
}
</script>

