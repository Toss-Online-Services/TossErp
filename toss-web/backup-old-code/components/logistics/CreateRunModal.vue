<template>
  <div v-if="show" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
    <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-xl max-w-2xl w-full max-h-[90vh] overflow-y-auto">
      <div class="p-6 border-b border-slate-200 dark:border-slate-700">
        <h3 class="text-2xl font-semibold text-slate-900 dark:text-white">Create Shared Delivery Run</h3>
        <p class="text-sm text-slate-600 dark:text-slate-400 mt-1">Coordinate a shared delivery and split costs</p>
      </div>

      <form @submit.prevent="handleSubmit" class="p-6">
        <div class="space-y-6">
          <!-- Driver Selection -->
          <div>
            <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
              Driver
            </label>
            <select 
              v-model="formData.driverId"
              required
              class="w-full px-4 py-3 border border-slate-300 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-green-500 dark:bg-slate-700 dark:text-white"
            >
              <option value="">Select a driver...</option>
              <option v-for="driver in drivers" :key="driver.id" :value="driver.id">
                {{ driver.name }} - {{ driver.vehicle }}
              </option>
            </select>
          </div>

          <!-- Pickup Location & Time -->
          <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                Pickup Location
              </label>
              <input 
                v-model="formData.pickupLocation"
                type="text"
                required
                placeholder="e.g., Metro Cash & Carry, Soweto"
                class="w-full px-4 py-3 border border-slate-300 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-green-500 dark:bg-slate-700 dark:text-white"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                Pickup Time
              </label>
              <input 
                v-model="formData.pickupTime"
                type="datetime-local"
                required
                :min="minPickupTime"
                class="w-full px-4 py-3 border border-slate-300 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-green-500 dark:bg-slate-700 dark:text-white"
              />
            </div>
          </div>

          <!-- Capacity & Distance -->
          <div class="grid grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                Vehicle Capacity
              </label>
              <input 
                v-model.number="formData.capacity"
                type="number"
                min="1"
                max="10"
                required
                placeholder="Max stops"
                class="w-full px-4 py-3 border border-slate-300 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-green-500 dark:bg-slate-700 dark:text-white"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                Est. Distance (km)
              </label>
              <input 
                v-model.number="formData.estimatedDistance"
                type="number"
                min="1"
                step="0.1"
                required
                placeholder="Total km"
                class="w-full px-4 py-3 border border-slate-300 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-green-500 dark:bg-slate-700 dark:text-white"
              />
            </div>
          </div>

          <!-- Base Fee & Split Rule -->
          <div class="grid grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                Base Delivery Fee
              </label>
              <input 
                v-model.number="formData.baseFee"
                type="number"
                step="0.01"
                min="0"
                required
                placeholder="Total fee"
                class="w-full px-4 py-3 border border-slate-300 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-green-500 dark:bg-slate-700 dark:text-white"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                Split Rule
              </label>
              <select 
                v-model="formData.splitRule"
                required
                class="w-full px-4 py-3 border border-slate-300 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-green-500 dark:bg-slate-700 dark:text-white"
              >
                <option value="equal">Equal Split</option>
                <option value="by-stops">By Stops</option>
                <option value="by-distance">By Distance</option>
                <option value="by-weight">By Weight</option>
              </select>
            </div>
          </div>

          <!-- Cost Preview -->
          <div v-if="formData.baseFee > 0 && formData.capacity > 0" class="p-4 bg-green-50 dark:bg-green-900/20 rounded-xl">
            <div class="flex items-center justify-between">
              <div>
                <p class="text-sm text-green-800 dark:text-green-400">
                  <strong>Per-shop share:</strong> R{{ (formData.baseFee / formData.capacity).toFixed(2) }}
                </p>
                <p class="text-xs text-green-700 dark:text-green-500 mt-1">
                  vs solo: R{{ (formData.baseFee * 1.5).toFixed(2) }} — Save 33%
                </p>
              </div>
              <div class="text-right">
                <p class="text-2xl font-bold text-green-600 dark:text-green-400">
                  -R{{ (formData.baseFee * 0.5).toFixed(2) }}
                </p>
                <p class="text-xs text-green-700 dark:text-green-500">Total savings</p>
              </div>
            </div>
          </div>

          <!-- Delivery Stops -->
          <div>
            <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
              Delivery Stops
            </label>
            <div class="space-y-3">
              <div v-for="(stop, index) in formData.stops" :key="index" class="flex gap-3">
                <div class="flex-1">
                  <input 
                    v-model="stop.address"
                    type="text"
                    required
                    :placeholder="`Stop ${index + 1} address`"
                    class="w-full px-4 py-3 border border-slate-300 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-green-500 dark:bg-slate-700 dark:text-white"
                  />
                </div>
                <button 
                  v-if="formData.stops.length > 1"
                  type="button"
                  @click="removeStop(index)"
                  class="p-3 text-red-600 hover:bg-red-50 dark:hover:bg-red-900/20 rounded-xl transition-colors"
                >
                  <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
                  </svg>
                </button>
              </div>
            </div>
            <button 
              type="button"
              @click="addStop"
              :disabled="formData.stops.length >= formData.capacity"
              class="mt-3 text-sm text-green-600 hover:text-green-700 font-medium disabled:opacity-50 disabled:cursor-not-allowed"
            >
              + Add Stop ({{ formData.stops.length }}/{{ formData.capacity }})
            </button>
          </div>

          <!-- Notes -->
          <div>
            <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
              Notes (Optional)
            </label>
            <textarea 
              v-model="formData.notes"
              rows="3"
              placeholder="Special instructions, vehicle details, etc."
              class="w-full px-4 py-3 border border-slate-300 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-green-500 dark:bg-slate-700 dark:text-white"
            ></textarea>
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
            class="px-8 py-3 bg-gradient-to-r from-green-600 to-blue-600 hover:from-green-700 hover:to-blue-700 text-white rounded-xl font-semibold shadow-lg hover:shadow-xl transition-all duration-200"
          >
            Create Shared Run
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
}>()

const emit = defineEmits<{
  close: []
  create: [runData: any]
}>()

const drivers = ref([
  { id: '1', name: 'Thabo Molefe', vehicle: 'Bakkie' },
  { id: '2', name: 'Sarah Ndlovu', vehicle: 'Van' },
  { id: '3', name: 'John Khumalo', vehicle: 'Truck' }
])

const formData = ref({
  driverId: '',
  pickupLocation: '',
  pickupTime: '',
  capacity: 5,
  estimatedDistance: 30,
  baseFee: 600,
  splitRule: 'equal',
  stops: [{ address: '' }],
  notes: ''
})

const minPickupTime = computed(() => {
  const now = new Date()
  now.setHours(now.getHours() + 2)
  return now.toISOString().slice(0, 16)
})

const addStop = () => {
  if (formData.value.stops.length < formData.value.capacity) {
    formData.value.stops.push({ address: '' })
  }
}

const removeStop = (index: number) => {
  if (formData.value.stops.length > 1) {
    formData.value.stops.splice(index, 1)
  }
}

const handleSubmit = () => {
  emit('create', {
    ...formData.value,
    pickupTime: new Date(formData.value.pickupTime)
  })
}
</script>

