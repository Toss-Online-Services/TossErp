<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useLogisticsStore, type Driver } from '~/stores/logistics'

interface Props {
  show: boolean
  driver?: Driver | null
}

const props = withDefaults(defineProps<Props>(), {
  driver: null
})

const emit = defineEmits<{
  close: []
  saved: [driver: Driver]
}>()

const logisticsStore = useLogisticsStore()

// Form state
const formData = ref({
  firstName: '',
  lastName: '',
  phone: '',
  email: '',
  licenseNumber: '',
  vehicleType: '',
  vehicleRegistration: '',
  isActive: true,
  isAvailable: true,
  areas: [] as string[],
  notes: ''
})

const isEditing = computed(() => !!props.driver)
const isSubmitting = ref(false)
const errors = ref<Record<string, string>>({})
const newArea = ref('')

// Watch for driver changes
watch(() => props.driver, (newDriver) => {
  if (newDriver) {
    formData.value = {
      firstName: newDriver.firstName,
      lastName: newDriver.lastName,
      phone: newDriver.phone,
      email: newDriver.email || '',
      licenseNumber: newDriver.licenseNumber || '',
      vehicleType: newDriver.vehicleType || '',
      vehicleRegistration: newDriver.vehicleRegistration || '',
      isActive: newDriver.isActive,
      isAvailable: newDriver.isAvailable,
      areas: [...(newDriver.areas || [])],
      notes: newDriver.notes || ''
    }
  } else {
    resetForm()
  }
}, { immediate: true })

function resetForm() {
  formData.value = {
    firstName: '',
    lastName: '',
    phone: '',
    email: '',
    licenseNumber: '',
    vehicleType: '',
    vehicleRegistration: '',
    isActive: true,
    isAvailable: true,
    areas: [],
    notes: ''
  }
  errors.value = {}
  newArea.value = ''
}

function validate() {
  errors.value = {}
  
  if (!formData.value.firstName.trim()) {
    errors.value.firstName = 'First name is required'
  }
  
  if (!formData.value.lastName.trim()) {
    errors.value.lastName = 'Last name is required'
  }
  
  if (!formData.value.phone.trim()) {
    errors.value.phone = 'Phone number is required'
  }
  
  return Object.keys(errors.value).length === 0
}

function addArea() {
  if (newArea.value.trim() && !formData.value.areas.includes(newArea.value.trim())) {
    formData.value.areas.push(newArea.value.trim())
    newArea.value = ''
  }
}

function removeArea(area: string) {
  const index = formData.value.areas.indexOf(area)
  if (index !== -1) {
    formData.value.areas.splice(index, 1)
  }
}

async function handleSave() {
  if (!validate()) return
  
  isSubmitting.value = true
  try {
    if (isEditing.value && props.driver) {
      await logisticsStore.updateDriver(props.driver.id, formData.value)
    } else {
      await logisticsStore.createDriver(formData.value)
    }
    
    emit('saved', props.driver || {} as Driver)
    resetForm()
  } catch (error) {
    console.error('Failed to save driver:', error)
  } finally {
    isSubmitting.value = false
  }
}

function handleClose() {
  resetForm()
  emit('close')
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
        <div class="flex items-center justify-center min-h-screen px-4 pt-4 pb-20 text-center sm:block sm:p-0">
          <div class="fixed inset-0 transition-opacity bg-gray-500 bg-opacity-75" @click="handleClose"></div>

          <div class="inline-block align-bottom bg-white rounded-lg text-left overflow-hidden shadow-xl transform transition-all sm:my-8 sm:align-middle sm:max-w-2xl sm:w-full">
            <div class="bg-white px-4 pt-5 pb-4 sm:p-6">
              <div class="flex items-center justify-between mb-4">
                <h3 class="text-xl font-bold text-gray-900">
                  {{ isEditing ? 'Edit Driver' : 'Add Driver' }}
                </h3>
                <button
                  @click="handleClose"
                  class="text-gray-400 hover:text-gray-600 transition-colors"
                >
                  <i class="material-symbols-rounded text-2xl">close</i>
                </button>
              </div>

              <div class="space-y-4">
                <div class="grid grid-cols-2 gap-4">
                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-1">
                      First Name <span class="text-red-500">*</span>
                    </label>
                    <input
                      v-model="formData.firstName"
                      type="text"
                      class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-gray-900 focus:border-transparent"
                      :class="{ 'border-red-500': errors.firstName }"
                    />
                    <p v-if="errors.firstName" class="mt-1 text-xs text-red-500">{{ errors.firstName }}</p>
                  </div>

                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-1">
                      Last Name <span class="text-red-500">*</span>
                    </label>
                    <input
                      v-model="formData.lastName"
                      type="text"
                      class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-gray-900 focus:border-transparent"
                      :class="{ 'border-red-500': errors.lastName }"
                    />
                    <p v-if="errors.lastName" class="mt-1 text-xs text-red-500">{{ errors.lastName }}</p>
                  </div>
                </div>

                <div class="grid grid-cols-2 gap-4">
                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-1">
                      Phone <span class="text-red-500">*</span>
                    </label>
                    <input
                      v-model="formData.phone"
                      type="tel"
                      class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-gray-900 focus:border-transparent"
                      :class="{ 'border-red-500': errors.phone }"
                    />
                    <p v-if="errors.phone" class="mt-1 text-xs text-red-500">{{ errors.phone }}</p>
                  </div>

                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-1">
                      Email
                    </label>
                    <input
                      v-model="formData.email"
                      type="email"
                      class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-gray-900 focus:border-transparent"
                    />
                  </div>
                </div>

                <div class="grid grid-cols-2 gap-4">
                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-1">
                      License Number
                    </label>
                    <input
                      v-model="formData.licenseNumber"
                      type="text"
                      class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-gray-900 focus:border-transparent"
                    />
                  </div>

                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-1">
                      Vehicle Type
                    </label>
                    <select
                      v-model="formData.vehicleType"
                      class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-gray-900 focus:border-transparent"
                    >
                      <option value="">Select vehicle type...</option>
                      <option value="Bakkie">Bakkie</option>
                      <option value="Van">Van</option>
                      <option value="Truck">Truck</option>
                      <option value="Motorcycle">Motorcycle</option>
                    </select>
                  </div>
                </div>

                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-1">
                    Vehicle Registration
                  </label>
                  <input
                    v-model="formData.vehicleRegistration"
                    type="text"
                    placeholder="ABC 123 GP"
                    class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-gray-900 focus:border-transparent"
                  />
                </div>

                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-1">
                    Delivery Areas
                  </label>
                  <div class="flex gap-2 mb-2">
                    <input
                      v-model="newArea"
                      type="text"
                      placeholder="Add area (e.g., Soweto)"
                      class="flex-1 px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-gray-900 focus:border-transparent"
                      @keyup.enter="addArea"
                    />
                    <button
                      @click="addArea"
                      type="button"
                      class="px-4 py-2 bg-gray-900 text-white rounded-lg hover:bg-gray-800 transition-colors"
                    >
                      Add
                    </button>
                  </div>
                  <div v-if="formData.areas.length > 0" class="flex flex-wrap gap-2">
                    <span
                      v-for="area in formData.areas"
                      :key="area"
                      class="inline-flex items-center gap-1 px-3 py-1 text-sm text-gray-700 bg-gray-100 rounded-lg"
                    >
                      {{ area }}
                      <button
                        @click="removeArea(area)"
                        type="button"
                        class="text-gray-500 hover:text-gray-700"
                      >
                        <i class="material-symbols-rounded text-sm">close</i>
                      </button>
                    </span>
                  </div>
                </div>

                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-1">
                    Notes
                  </label>
                  <textarea
                    v-model="formData.notes"
                    rows="3"
                    class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-gray-900 focus:border-transparent"
                  ></textarea>
                </div>

                <div class="flex items-center gap-4">
                  <label class="flex items-center gap-2">
                    <input
                      v-model="formData.isActive"
                      type="checkbox"
                      class="w-4 h-4 text-gray-900 border-gray-300 rounded focus:ring-gray-900"
                    />
                    <span class="text-sm text-gray-700">Active</span>
                  </label>
                  <label class="flex items-center gap-2">
                    <input
                      v-model="formData.isAvailable"
                      type="checkbox"
                      class="w-4 h-4 text-gray-900 border-gray-300 rounded focus:ring-gray-900"
                    />
                    <span class="text-sm text-gray-700">Available</span>
                  </label>
                </div>
              </div>
            </div>

            <div class="bg-gray-50 px-4 py-3 sm:px-6 sm:flex sm:flex-row-reverse">
              <button
                @click="handleSave"
                :disabled="isSubmitting"
                class="w-full inline-flex justify-center rounded-lg border border-transparent shadow-sm px-4 py-2 bg-gray-900 text-base font-medium text-white hover:bg-gray-800 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-gray-900 sm:ml-3 sm:w-auto sm:text-sm disabled:opacity-50 disabled:cursor-not-allowed"
              >
                {{ isSubmitting ? 'Saving...' : isEditing ? 'Update' : 'Create' }}
              </button>
              <button
                @click="handleClose"
                class="mt-3 w-full inline-flex justify-center rounded-lg border border-gray-300 shadow-sm px-4 py-2 bg-white text-base font-medium text-gray-700 hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-gray-900 sm:mt-0 sm:ml-3 sm:w-auto sm:text-sm"
              >
                Cancel
              </button>
            </div>
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<style scoped>
.modal-enter-active, .modal-leave-active {
  transition: opacity 0.3s;
}
.modal-enter-from, .modal-leave-to {
  opacity: 0;
}
</style>

