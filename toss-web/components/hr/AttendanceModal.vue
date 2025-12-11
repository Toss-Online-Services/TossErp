<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useHrStore, type Attendance } from '~/stores/hr'

interface Props {
  show: boolean
  entry?: Attendance | null
  employeeId?: number | null
}

const props = withDefaults(defineProps<Props>(), {
  entry: null,
  employeeId: null
})

const emit = defineEmits<{
  close: []
  saved: [entry: Attendance]
}>()

const hrStore = useHrStore()

// Form state
const formData = ref({
  employeeId: null as number | null,
  attendanceDate: new Date().toISOString().split('T')[0],
  clockIn: '',
  clockOut: '',
  daysWorked: null as number | null,
  notes: ''
})

const attendanceType = ref<'clock' | 'days'>('clock')
const isEditing = computed(() => !!props.entry)
const isSubmitting = ref(false)
const errors = ref<Record<string, string>>({})

// Watch for entry changes
watch(() => props.entry, (newEntry) => {
  if (newEntry) {
    formData.value = {
      employeeId: newEntry.employeeId,
      attendanceDate: new Date(newEntry.attendanceDate).toISOString().split('T')[0],
      clockIn: newEntry.clockIn ? new Date(newEntry.clockIn).toISOString().slice(0, 16) : '',
      clockOut: newEntry.clockOut ? new Date(newEntry.clockOut).toISOString().slice(0, 16) : '',
      daysWorked: newEntry.daysWorked || null,
      notes: newEntry.notes || ''
    }
    attendanceType.value = newEntry.clockIn ? 'clock' : 'days'
  } else {
    resetForm()
  }
}, { immediate: true })

watch(() => props.employeeId, (newEmployeeId) => {
  if (newEmployeeId && !isEditing.value) {
    formData.value.employeeId = newEmployeeId
  }
}, { immediate: true })

function resetForm() {
  formData.value = {
    employeeId: props.employeeId || null,
    attendanceDate: new Date().toISOString().split('T')[0],
    clockIn: '',
    clockOut: '',
    daysWorked: null,
    notes: ''
  }
  attendanceType.value = 'clock'
  errors.value = {}
}

function validate() {
  errors.value = {}
  
  if (!formData.value.employeeId) {
    errors.value.employeeId = 'Employee is required'
  }
  
  if (!formData.value.attendanceDate) {
    errors.value.attendanceDate = 'Date is required'
  }
  
  if (attendanceType.value === 'clock') {
    if (!formData.value.clockIn) {
      errors.value.clockIn = 'Clock in time is required'
    }
  } else {
    if (!formData.value.daysWorked || formData.value.daysWorked <= 0) {
      errors.value.daysWorked = 'Days worked must be greater than 0'
    }
  }
  
  return Object.keys(errors.value).length === 0
}

async function handleSave() {
  if (!validate()) return
  
  isSubmitting.value = true
  try {
    const attendanceData = {
      employeeId: formData.value.employeeId!,
      attendanceDate: new Date(formData.value.attendanceDate),
      clockIn: attendanceType.value === 'clock' && formData.value.clockIn 
        ? new Date(formData.value.clockIn) 
        : undefined,
      clockOut: attendanceType.value === 'clock' && formData.value.clockOut 
        ? new Date(formData.value.clockOut) 
        : undefined,
      daysWorked: attendanceType.value === 'days' ? formData.value.daysWorked : undefined,
      notes: formData.value.notes.trim() || undefined
    }
    
    const newEntry = await hrStore.recordAttendance(attendanceData)
    emit('saved', newEntry)
    emit('close')
    resetForm()
  } catch (error) {
    console.error('Failed to save attendance:', error)
    alert('Failed to save attendance. Please try again.')
  } finally {
    isSubmitting.value = false
  }
}

function handleClose() {
  emit('close')
  resetForm()
}
</script>

<template>
  <Teleport to="body">
    <Transition name="modal">
      <div
        v-if="show"
        class="fixed inset-0 z-50 overflow-y-auto"
        @click.self="handleClose"
      >
        <div class="flex min-h-screen items-center justify-center p-4">
          <div
            class="relative w-full max-w-2xl rounded-xl bg-white shadow-xl max-h-[90vh] flex flex-col"
            @click.stop
          >
            <!-- Header -->
            <div class="flex items-center justify-between border-b border-gray-200 px-6 py-4">
              <h3 class="text-xl font-bold text-gray-900">
                {{ isEditing ? 'Edit Attendance' : 'Record Attendance' }}
              </h3>
              <button
                @click="handleClose"
                class="text-gray-400 hover:text-gray-600 transition-colors"
              >
                <i class="material-symbols-rounded text-2xl">close</i>
              </button>
            </div>

            <!-- Form -->
            <form @submit.prevent="handleSave" class="flex-1 overflow-y-auto p-6 space-y-6">
              <!-- Employee Selection -->
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">
                  Employee <span class="text-red-500">*</span>
                </label>
                <select
                  v-model="formData.employeeId"
                  class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                  :class="{ 'border-red-500': errors.employeeId }"
                >
                  <option :value="null">Select Employee</option>
                  <option
                    v-for="employee in hrStore.activeEmployees"
                    :key="employee.id"
                    :value="employee.id"
                  >
                    {{ employee.name }}
                  </option>
                </select>
                <p v-if="errors.employeeId" class="mt-1 text-sm text-red-500">{{ errors.employeeId }}</p>
              </div>

              <!-- Attendance Type -->
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">Attendance Type</label>
                <div class="flex gap-4">
                  <label class="flex items-center gap-2">
                    <input
                      v-model="attendanceType"
                      type="radio"
                      value="clock"
                      class="w-4 h-4 text-blue-600 border-gray-300 focus:ring-blue-500"
                    />
                    <span class="text-sm text-gray-700">Clock In/Out</span>
                  </label>
                  <label class="flex items-center gap-2">
                    <input
                      v-model="attendanceType"
                      type="radio"
                      value="days"
                      class="w-4 h-4 text-blue-600 border-gray-300 focus:ring-blue-500"
                    />
                    <span class="text-sm text-gray-700">Days Worked</span>
                  </label>
                </div>
              </div>

              <!-- Date -->
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">
                  Date <span class="text-red-500">*</span>
                </label>
                <input
                  v-model="formData.attendanceDate"
                  type="date"
                  class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                  :class="{ 'border-red-500': errors.attendanceDate }"
                />
                <p v-if="errors.attendanceDate" class="mt-1 text-sm text-red-500">{{ errors.attendanceDate }}</p>
              </div>

              <!-- Clock In/Out -->
              <div v-if="attendanceType === 'clock'">
                <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-2">
                      Clock In <span class="text-red-500">*</span>
                    </label>
                    <input
                      v-model="formData.clockIn"
                      type="datetime-local"
                      class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                      :class="{ 'border-red-500': errors.clockIn }"
                    />
                    <p v-if="errors.clockIn" class="mt-1 text-sm text-red-500">{{ errors.clockIn }}</p>
                  </div>

                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-2">Clock Out</label>
                    <input
                      v-model="formData.clockOut"
                      type="datetime-local"
                      class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    />
                  </div>
                </div>
              </div>

              <!-- Days Worked -->
              <div v-else>
                <label class="block text-sm font-medium text-gray-700 mb-2">
                  Days Worked <span class="text-red-500">*</span>
                </label>
                <input
                  v-model.number="formData.daysWorked"
                  type="number"
                  step="0.5"
                  min="0"
                  class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                  :class="{ 'border-red-500': errors.daysWorked }"
                  placeholder="1.0"
                />
                <p v-if="errors.daysWorked" class="mt-1 text-sm text-red-500">{{ errors.daysWorked }}</p>
              </div>

              <!-- Notes -->
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">Notes</label>
                <textarea
                  v-model="formData.notes"
                  rows="3"
                  class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                  placeholder="Additional notes..."
                />
              </div>
            </form>

            <!-- Footer -->
            <div class="flex items-center justify-end gap-3 border-t border-gray-200 px-6 py-4">
              <button
                @click="handleClose"
                class="px-4 py-2 text-gray-700 bg-gray-100 rounded-lg hover:bg-gray-200 transition-colors"
              >
                Cancel
              </button>
              <button
                @click="handleSave"
                :disabled="isSubmitting"
                class="px-6 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
              >
                {{ isSubmitting ? 'Saving...' : 'Save' }}
              </button>
            </div>
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<style scoped>
.modal-enter-active,
.modal-leave-active {
  transition: opacity 0.3s;
}

.modal-enter-from,
.modal-leave-to {
  opacity: 0;
}
</style>



