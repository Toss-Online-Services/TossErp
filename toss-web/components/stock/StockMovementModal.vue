<template>
  <div class="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center p-4 z-50 animate-fadeIn" @click.self="$emit('close')">
    <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-2xl max-w-2xl w-full max-h-[90vh] overflow-hidden border border-slate-200/50 dark:border-slate-700/50 animate-slideUp">
      <!-- Header with Gradient -->
      <div :class="headerClass" class="relative px-6 py-6 overflow-hidden">
        <div class="absolute inset-0 bg-black/10"></div>
        <div class="relative z-10 flex items-center justify-between">
          <div class="flex items-center space-x-3">
            <div class="p-2 bg-white/20 backdrop-blur-sm rounded-lg">
              <component :is="iconComponent" class="w-6 h-6 text-white" />
            </div>
            <div>
              <h3 class="text-xl font-bold text-white">
                {{ title }}
              </h3>
              <p class="text-sm text-white/80 mt-0.5">
                {{ subtitle }}
              </p>
            </div>
          </div>
          <button
            @click="$emit('close')"
            class="p-2 text-white/80 hover:text-white hover:bg-white/20 rounded-lg transition-all duration-200"
          >
            <XMarkIcon class="w-6 h-6" />
          </button>
        </div>
      </div>

      <!-- Form -->
      <form @submit.prevent="handleSubmit" class="p-6 overflow-y-auto max-h-[calc(90vh-180px)]">
        <div class="space-y-6">
          <!-- Item Selection -->
          <div>
            <label for="item" class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
              Select Item <span class="text-red-500">*</span>
            </label>
            <select
              id="item"
              v-model="formData.itemId"
              required
              class="w-full px-4 py-2.5 border border-slate-300 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-purple-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white"
            >
              <option value="">Choose an item...</option>
              <option v-for="item in items" :key="item.id" :value="item.id">
                {{ item.name }} ({{ item.sku }})
              </option>
            </select>
          </div>

          <!-- Quantity -->
          <div>
            <label for="quantity" class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
              Quantity <span class="text-red-500">*</span>
            </label>
            <input
              id="quantity"
              v-model.number="formData.quantity"
              type="number"
              min="1"
              required
              class="w-full px-4 py-2.5 border border-slate-300 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-purple-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white"
              placeholder="Enter quantity"
            />
          </div>

          <!-- Reference -->
          <div>
            <label for="reference" class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
              Reference Number
            </label>
            <input
              id="reference"
              v-model="formData.reference"
              type="text"
              class="w-full px-4 py-2.5 border border-slate-300 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-purple-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white"
              placeholder="e.g., PO-12345, INV-678"
            />
          </div>

          <!-- Notes/Reason -->
          <div>
            <label for="notes" class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
              {{ movementType === 'adjustment' ? 'Reason for Adjustment' : 'Notes' }}
              <span v-if="movementType === 'adjustment'" class="text-red-500">*</span>
            </label>
            <textarea
              id="notes"
              v-model="formData.notes"
              :required="movementType === 'adjustment'"
              rows="3"
              class="w-full px-4 py-2.5 border border-slate-300 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-purple-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white"
              :placeholder="movementType === 'adjustment' ? 'Why are you fixing this? e.g., Found damaged items' : 'Optional notes about this movement'"
            ></textarea>
          </div>

          <!-- Transfer Location (only for transfer type) -->
          <div v-if="movementType === 'transfer'">
            <label for="fromLocation" class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
              From Location
            </label>
            <input
              id="fromLocation"
              v-model="formData.fromLocation"
              type="text"
              class="w-full px-4 py-2.5 border border-slate-300 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-purple-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white"
              placeholder="e.g., Main Store"
            />
          </div>

          <div v-if="movementType === 'transfer'">
            <label for="toLocation" class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
              To Location <span class="text-red-500">*</span>
            </label>
            <input
              id="toLocation"
              v-model="formData.toLocation"
              type="text"
              :required="movementType === 'transfer'"
              class="w-full px-4 py-2.5 border border-slate-300 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-purple-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white"
              placeholder="e.g., Back Room"
            />
          </div>
        </div>

        <!-- Form Actions -->
        <div class="flex flex-col sm:flex-row justify-end space-y-3 sm:space-y-0 sm:space-x-4 mt-8 pt-6 border-t border-slate-200 dark:border-slate-700">
          <button
            type="button"
            @click="$emit('close')"
            class="px-6 py-3 border-2 border-slate-300 dark:border-slate-600 rounded-xl text-slate-700 dark:text-slate-300 hover:bg-slate-50 dark:hover:bg-slate-700 hover:border-slate-400 dark:hover:border-slate-500 transition-all duration-200 font-medium"
          >
            Cancel
          </button>
          <button
            type="submit"
            :class="buttonClass"
            class="relative px-8 py-3 text-white rounded-xl font-semibold shadow-lg hover:shadow-xl transform hover:scale-105 transition-all duration-200 overflow-hidden group"
          >
            <span class="relative z-10 flex items-center justify-center">
              <component :is="iconComponent" class="w-5 h-5 mr-2" />
              {{ submitButtonText }}
            </span>
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import {
  XMarkIcon,
  ArrowDownIcon,
  ArrowUpIcon,
  ArrowRightIcon,
  AdjustmentsHorizontalIcon
} from '@heroicons/vue/24/outline'
import { useStock, type ItemDto } from '~/composables/useStock'

const props = defineProps<{
  movementType: 'receipt' | 'issue' | 'transfer' | 'adjustment'
}>()

const emit = defineEmits<{
  close: []
  save: [movement: any]
}>()

// Composables
const { getItems, createStockMovement } = useStock()

// State
const items = ref<ItemDto[]>([])
const formData = ref({
  itemId: '',
  quantity: 1,
  reference: '',
  notes: '',
  fromLocation: '',
  toLocation: ''
})

// Computed properties
const title = computed(() => {
  const titles = {
    receipt: 'Stock IN ↓',
    issue: 'Stock OUT ↑',
    transfer: 'Stock MOVED →',
    adjustment: 'Stock FIXED ⇌'
  }
  return titles[props.movementType]
})

const subtitle = computed(() => {
  const subtitles = {
    receipt: 'Record items coming into your store',
    issue: 'Record items going out of your store',
    transfer: 'Move items between locations',
    adjustment: 'Fix stock count mistakes'
  }
  return subtitles[props.movementType]
})

const headerClass = computed(() => {
  const classes = {
    receipt: 'bg-gradient-to-r from-green-500 to-emerald-600',
    issue: 'bg-gradient-to-r from-red-500 to-pink-600',
    transfer: 'bg-gradient-to-r from-blue-500 to-purple-600',
    adjustment: 'bg-gradient-to-r from-orange-500 to-yellow-500'
  }
  return classes[props.movementType]
})

const buttonClass = computed(() => {
  const classes = {
    receipt: 'bg-gradient-to-r from-green-600 to-emerald-600 hover:from-green-700 hover:to-emerald-700',
    issue: 'bg-gradient-to-r from-red-600 to-pink-600 hover:from-red-700 hover:to-pink-700',
    transfer: 'bg-gradient-to-r from-blue-600 to-purple-600 hover:from-blue-700 hover:to-purple-700',
    adjustment: 'bg-gradient-to-r from-orange-600 to-yellow-600 hover:from-orange-700 hover:to-yellow-700'
  }
  return classes[props.movementType]
})

const iconComponent = computed(() => {
  const icons = {
    receipt: ArrowDownIcon,
    issue: ArrowUpIcon,
    transfer: ArrowRightIcon,
    adjustment: AdjustmentsHorizontalIcon
  }
  return icons[props.movementType]
})

const submitButtonText = computed(() => {
  const texts = {
    receipt: 'Record Stock IN',
    issue: 'Record Stock OUT',
    transfer: 'Move Stock',
    adjustment: 'Fix Stock Count'
  }
  return texts[props.movementType]
})

// Methods
const loadItems = async () => {
  try {
    items.value = await getItems()
  } catch (error) {
    console.error('Failed to load items:', error)
  }
}

const handleSubmit = async () => {
  try {
    const movement = {
      type: props.movementType,
      itemId: formData.value.itemId,
      quantity: props.movementType === 'issue' ? -formData.value.quantity : formData.value.quantity,
      reference: formData.value.reference,
      notes: formData.value.notes,
      fromLocation: formData.value.fromLocation,
      toLocation: formData.value.toLocation,
      status: 'completed',
      createdAt: new Date().toISOString()
    }

    await createStockMovement(movement)
    emit('save', movement)
    emit('close')
  } catch (error) {
    console.error('Failed to create movement:', error)
    alert('Failed to record stock movement. Please try again.')
  }
}

// Lifecycle
onMounted(() => {
  loadItems()
})
</script>

<style scoped>
@keyframes fadeIn {
  from {
    opacity: 0;
  }
  to {
    opacity: 1;
  }
}

@keyframes slideUp {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.animate-fadeIn {
  animation: fadeIn 0.2s ease-out;
}

.animate-slideUp {
  animation: slideUp 0.3s ease-out;
}
</style>
