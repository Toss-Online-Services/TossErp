<template>
  <MaterialModal
    :model-value="show"
    @update:model-value="$emit('close')"
    title="Create Shared Delivery Run"
    max-width="2xl"
  >
    <template #default>
      <p class="text-sm text-slate-600 dark:text-slate-400 mb-6">Coordinate a shared delivery and split costs</p>
      
      <form @submit.prevent="handleSubmit" class="space-y-6">
        <!-- Driver Selection -->
        <div>
          <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
            Driver
          </label>
          <select 
            v-model="formData.driverId"
            required
            class="w-full px-4 py-3 border-2 border-slate-300 dark:border-slate-600 rounded-lg bg-white dark:bg-slate-800 focus:ring-4 focus:ring-green-500/50 focus:border-green-500 transition-all"
          >
            <option value="">Select a driver...</option>
            <option v-for="driver in drivers" :key="driver.id" :value="driver.id">
              {{ driver.name }} - {{ driver.vehicle }}
            </option>
          </select>
        </div>

        <!-- Pickup Location & Time -->
        <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
          <MaterialInput
            v-model="formData.pickupLocation"
            label="Pickup Location"
            placeholder="e.g., Metro Cash & Carry, Soweto"
            required
          />
          <MaterialInput
            v-model="formData.pickupTime"
            type="datetime-local"
            label="Pickup Time"
            :min="minPickupTime"
            required
          />
        </div>

        <!-- Capacity & Distance -->
        <div class="grid grid-cols-2 gap-4">
          <MaterialInput
            v-model="formData.capacity"
            type="number"
            label="Vehicle Capacity"
            placeholder="Max stops"
            :min="1"
            :max="10"
            required
          />
          <MaterialInput
            v-model="formData.estimatedDistance"
            type="number"
            label="Est. Distance (km)"
            placeholder="Total km"
            :min="1"
            step="0.1"
            required
          />
        </div>

        <!-- Base Fee & Split Rule -->
        <div class="grid grid-cols-2 gap-4">
          <MaterialInput
            v-model="formData.baseFee"
            type="number"
            label="Base Delivery Fee"
            placeholder="Total fee"
            step="0.01"
            :min="0"
            required
          />
          <div>
            <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
              Split Rule
            </label>
            <select 
              v-model="formData.splitRule"
              required
              class="w-full px-4 py-3 border-2 border-slate-300 dark:border-slate-600 rounded-lg bg-white dark:bg-slate-800 focus:ring-4 focus:ring-green-500/50 focus:border-green-500 transition-all"
            >
              <option value="equal">Equal Split</option>
              <option value="by-stops">By Stops</option>
              <option value="by-distance">By Distance</option>
              <option value="by-weight">By Weight</option>
            </select>
          </div>
        </div>

        <!-- Cost Preview -->
        <MaterialCard
          v-if="formData.baseFee > 0 && formData.capacity > 0"
          gradient="green"
          class="p-4"
        >
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
        </MaterialCard>

        <!-- Delivery Stops -->
        <div>
          <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
            Delivery Stops
          </label>
          <div class="space-y-3">
            <div v-for="(stop, index) in formData.stops" :key="index" class="flex gap-3">
              <MaterialInput
                v-model="stop.address"
                :placeholder="`Stop ${index + 1} address`"
                required
                class="flex-1"
              />
              <MaterialButton
                v-if="formData.stops.length > 1"
                type="button"
                variant="text"
                color="error"
                icon
                @click="removeStop(index)"
              >
                <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
                </svg>
              </MaterialButton>
            </div>
          </div>
          <MaterialButton
            type="button"
            variant="text"
            color="success"
            size="sm"
            :disabled="formData.stops.length >= formData.capacity"
            @click="addStop"
            class="mt-3"
          >
            + Add Stop ({{ formData.stops.length }}/{{ formData.capacity }})
          </MaterialButton>
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
            class="w-full px-4 py-3 border-2 border-slate-300 dark:border-slate-600 rounded-lg bg-white dark:bg-slate-800 focus:ring-4 focus:ring-green-500/50 focus:border-green-500 transition-all"
          ></textarea>
        </div>
      </form>
    </template>

    <template #footer>
      <div class="flex justify-end space-x-3">
        <MaterialButton
          variant="outlined"
          color="secondary"
          @click="$emit('close')"
        >
          Cancel
        </MaterialButton>
        <MaterialButton
          color="success"
          @click="handleSubmit"
        >
          Create Shared Run
        </MaterialButton>
      </div>
    </template>
  </MaterialModal>
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

