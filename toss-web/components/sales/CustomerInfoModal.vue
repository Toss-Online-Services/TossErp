<template>
  <div v-if="show" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
    <div class="bg-white dark:bg-slate-800 rounded-2xl p-6 max-w-md w-full">
      <h3 class="text-xl font-bold text-slate-900 dark:text-white mb-4">Customer Information</h3>
      <div class="space-y-4">
        <div>
          <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Customer Name *</label>
          <input 
            v-model="localCustomer.name"
            type="text"
            placeholder="Enter customer name"
            class="w-full px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-green-500 dark:bg-slate-700 dark:text-white"
            required
          />
        </div>
        <div>
          <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Phone Number</label>
          <input 
            v-model="localCustomer.phone"
            type="tel"
            placeholder="Enter phone number"
            class="w-full px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-green-500 dark:bg-slate-700 dark:text-white"
          />
        </div>
        <div>
          <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Notes (Optional)</label>
          <textarea 
            v-model="localCustomer.notes"
            placeholder="Special instructions..."
            rows="2"
            class="w-full px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-green-500 dark:bg-slate-700 dark:text-white"
          ></textarea>
        </div>
      </div>
      <div class="flex gap-3 mt-6">
        <button 
          @click="$emit('close')"
          class="flex-1 py-2 border-2 border-slate-300 dark:border-slate-600 text-slate-700 dark:text-slate-300 rounded-xl font-medium hover:bg-slate-50 dark:hover:bg-slate-700 transition-colors"
        >
          Cancel
        </button>
        <button 
          @click="handleSave"
          class="flex-1 py-2 bg-gradient-to-r from-green-600 to-blue-600 hover:from-green-700 hover:to-blue-700 text-white rounded-xl font-semibold transition-colors"
        >
          Save
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'

interface CustomerInfo {
  name: string
  phone: string
  notes: string
}

interface Props {
  show: boolean
  customerInfo?: CustomerInfo
}

const props = withDefaults(defineProps<Props>(), {
  customerInfo: () => ({ name: '', phone: '', notes: '' })
})

const emit = defineEmits<{
  'close': []
  'save': [customer: CustomerInfo]
}>()

const localCustomer = ref<CustomerInfo>({ ...props.customerInfo })

watch(() => props.customerInfo, (newVal) => {
  localCustomer.value = { ...newVal }
}, { deep: true })

const handleSave = () => {
  if (!localCustomer.value.name.trim()) {
    alert('Please enter customer name')
    return
  }
  emit('save', { ...localCustomer.value })
}
</script>

