<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useHrStore, type Employee, type EmployeeRateType } from '~/stores/hr'

interface Props {
  show: boolean
  employee?: Employee | null
}

const props = withDefaults(defineProps<Props>(), {
  employee: null
})

const emit = defineEmits<{
  close: []
  saved: [employee: Employee]
}>()

const hrStore = useHrStore()

// Form state
const formData = ref({
  name: '',
  email: '',
  phone: '',
  idNumber: '',
  rateType: 'Hourly' as EmployeeRateType,
  rate: 0,
  isActive: true,
  notes: ''
})

const isEditing = computed(() => !!props.employee)
const isSubmitting = ref(false)
const errors = ref<Record<string, string>>({})

const rateTypeOptions: EmployeeRateType[] = ['Hourly', 'Daily', 'Monthly']

// Watch for employee changes
watch(() => props.employee, (newEmployee) => {
  if (newEmployee) {
    formData.value = {
      name: newEmployee.name,
      email: newEmployee.email || '',
      phone: newEmployee.phone || '',
      idNumber: newEmployee.idNumber || '',
      rateType: newEmployee.rateType,
      rate: newEmployee.rate,
      isActive: newEmployee.isActive,
      notes: newEmployee.notes || ''
    }
  } else {
    resetForm()
  }
}, { immediate: true })

function resetForm() {
  formData.value = {
    name: '',
    email: '',
    phone: '',
    idNumber: '',
    rateType: 'Hourly',
    rate: 0,
    isActive: true,
    notes: ''
  }
  errors.value = {}
}

function validate() {
  errors.value = {}
  
  if (!formData.value.name.trim()) {
    errors.value.name = 'Employee name is required'
  }
  
  if (formData.value.rate <= 0) {
    errors.value.rate = 'Rate must be greater than 0'
  }
  
  if (formData.value.email && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.value.email)) {
    errors.value.email = 'Invalid email format'
  }
  
  return Object.keys(errors.value).length === 0
}

async function handleSave() {
  if (!validate()) return
  
  isSubmitting.value = true
  try {
    const employeeData = {
      name: formData.value.name.trim(),
      email: formData.value.email.trim() || undefined,
      phone: formData.value.phone.trim() || undefined,
      idNumber: formData.value.idNumber.trim() || undefined,
      rateType: formData.value.rateType,
      rate: formData.value.rate,
      isActive: formData.value.isActive,
      notes: formData.value.notes.trim() || undefined
    }
    
    if (isEditing.value && props.employee) {
      await hrStore.updateEmployee(props.employee.id, employeeData)
      emit('saved', { ...props.employee, ...employeeData } as Employee)
    } else {
      const newEmployee = await hrStore.createEmployee(employeeData)
      emit('saved', newEmployee)
    }
    emit('close')
    resetForm()
  } catch (error) {
    console.error('Failed to save employee:', error)
    alert('Failed to save employee. Please try again.')
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
                {{ isEditing ? 'Edit Employee' : 'Add Employee' }}
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
              <!-- Basic Information -->
              <div>
                <h4 class="text-sm font-semibold text-gray-900 mb-4">Basic Information</h4>
                <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div class="md:col-span-2">
                    <label class="block text-sm font-medium text-gray-700 mb-2">
                      Name <span class="text-red-500">*</span>
                    </label>
                    <input
                      v-model="formData.name"
                      type="text"
                      class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                      :class="{ 'border-red-500': errors.name }"
                      placeholder="Enter employee name"
                    />
                    <p v-if="errors.name" class="mt-1 text-sm text-red-500">{{ errors.name }}</p>
                  </div>

                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-2">Email</label>
                    <input
                      v-model="formData.email"
                      type="email"
                      class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                      :class="{ 'border-red-500': errors.email }"
                      placeholder="email@example.com"
                    />
                    <p v-if="errors.email" class="mt-1 text-sm text-red-500">{{ errors.email }}</p>
                  </div>

                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-2">Phone</label>
                    <input
                      v-model="formData.phone"
                      type="tel"
                      class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                      placeholder="+27 82 123 4567"
                    />
                  </div>

                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-2">ID Number</label>
                    <input
                      v-model="formData.idNumber"
                      type="text"
                      class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                      placeholder="850101 5800 08 5"
                    />
                  </div>
                </div>
              </div>

              <!-- Rate Information -->
              <div>
                <h4 class="text-sm font-semibold text-gray-900 mb-4">Compensation</h4>
                <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-2">Rate Type</label>
                    <select
                      v-model="formData.rateType"
                      class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    >
                      <option v-for="type in rateTypeOptions" :key="type" :value="type">
                        {{ type }}
                      </option>
                    </select>
                  </div>

                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-2">
                      Rate (R) <span class="text-red-500">*</span>
                    </label>
                    <input
                      v-model.number="formData.rate"
                      type="number"
                      step="0.01"
                      min="0"
                      class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                      :class="{ 'border-red-500': errors.rate }"
                      placeholder="0.00"
                    />
                    <p v-if="errors.rate" class="mt-1 text-sm text-red-500">{{ errors.rate }}</p>
                  </div>
                </div>
              </div>

              <!-- Status -->
              <div>
                <label class="flex items-center gap-2">
                  <input
                    v-model="formData.isActive"
                    type="checkbox"
                    class="w-4 h-4 text-blue-600 border-gray-300 rounded focus:ring-blue-500"
                  />
                  <span class="text-sm font-medium text-gray-700">Active Employee</span>
                </label>
              </div>

              <!-- Notes -->
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">Notes</label>
                <textarea
                  v-model="formData.notes"
                  rows="3"
                  class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                  placeholder="Additional notes about the employee..."
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
                {{ isSubmitting ? 'Saving...' : isEditing ? 'Update' : 'Create' }}
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



