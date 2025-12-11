<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useProjectsStore, type LabourEntry } from '~/stores/projects'

interface Props {
  show: boolean
  entry?: LabourEntry | null
  projectId?: number | null
}

const props = withDefaults(defineProps<Props>(), {
  entry: null,
  projectId: null
})

const emit = defineEmits<{
  close: []
  saved: [entry: LabourEntry]
}>()

const projectsStore = useProjectsStore()

// Form state
const formData = ref({
  projectId: null as number | null,
  projectTaskId: null as number | null,
  userId: '',
  workDate: new Date().toISOString().split('T')[0],
  hours: 0,
  rate: 150,
  description: ''
})

const isEditing = computed(() => !!props.entry)
const isSubmitting = ref(false)
const errors = ref<Record<string, string>>({})

const totalCost = computed(() => {
  return formData.value.hours * formData.value.rate
})

// Watch for entry changes
watch(() => props.entry, (newEntry) => {
  if (newEntry) {
    formData.value = {
      projectId: newEntry.projectId,
      projectTaskId: newEntry.projectTaskId || null,
      userId: newEntry.userId || '',
      workDate: new Date(newEntry.workDate).toISOString().split('T')[0],
      hours: newEntry.hours,
      rate: newEntry.rate,
      description: newEntry.description || ''
    }
  } else {
    resetForm()
  }
}, { immediate: true })

watch(() => props.projectId, (newProjectId) => {
  if (newProjectId && !isEditing.value) {
    formData.value.projectId = newProjectId
  }
}, { immediate: true })

watch(() => formData.value.hours, () => {
  // Auto-calculate total cost
})

watch(() => formData.value.rate, () => {
  // Auto-calculate total cost
})

function resetForm() {
  formData.value = {
    projectId: props.projectId || null,
    projectTaskId: null,
    userId: '',
    workDate: new Date().toISOString().split('T')[0],
    hours: 0,
    rate: 150,
    description: ''
  }
  errors.value = {}
}

function validate() {
  errors.value = {}
  
  if (!formData.value.projectId) {
    errors.value.projectId = 'Project is required'
  }
  
  if (formData.value.hours <= 0) {
    errors.value.hours = 'Hours must be greater than 0'
  }
  
  if (formData.value.rate <= 0) {
    errors.value.rate = 'Rate must be greater than 0'
  }
  
  if (!formData.value.workDate) {
    errors.value.workDate = 'Work date is required'
  }
  
  return Object.keys(errors.value).length === 0
}

async function handleSave() {
  if (!validate()) return
  
  isSubmitting.value = true
  try {
    const entryData = {
      projectId: formData.value.projectId!,
      projectTaskId: formData.value.projectTaskId || undefined,
      userId: formData.value.userId || undefined,
      workDate: new Date(formData.value.workDate),
      hours: formData.value.hours,
      rate: formData.value.rate,
      totalCost: totalCost.value,
      description: formData.value.description.trim() || undefined
    }
    
    if (isEditing.value && props.entry) {
      // Update not implemented in store yet, but structure is ready
      alert('Update functionality coming soon')
      emit('close')
    } else {
      const newEntry = await projectsStore.createLabourEntry(entryData)
      emit('saved', newEntry)
    }
    emit('close')
    resetForm()
  } catch (error) {
    console.error('Failed to save time entry:', error)
    alert('Failed to save time entry. Please try again.')
  } finally {
    isSubmitting.value = false
  }
}

function handleClose() {
  emit('close')
  resetForm()
}

const availableTasks = computed(() => {
  if (!formData.value.projectId) return []
  return projectsStore.getTasksByProject(formData.value.projectId)
})
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
                {{ isEditing ? 'Edit Time Entry' : 'Log Time' }}
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
              <!-- Project Selection -->
              <div>
                <h4 class="text-sm font-semibold text-gray-900 mb-4">Project & Task</h4>
                <div class="grid grid-cols-1 gap-4">
                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-2">
                      Project <span class="text-red-500">*</span>
                    </label>
                    <select
                      v-model="formData.projectId"
                      class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                      :class="{ 'border-red-500': errors.projectId }"
                    >
                      <option :value="null">Select Project</option>
                      <option
                        v-for="project in projectsStore.projects"
                        :key="project.id"
                        :value="project.id"
                      >
                        {{ project.title }}
                      </option>
                    </select>
                    <p v-if="errors.projectId" class="mt-1 text-sm text-red-500">{{ errors.projectId }}</p>
                  </div>

                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-2">Task (Optional)</label>
                    <select
                      v-model="formData.projectTaskId"
                      class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                      :disabled="!formData.projectId"
                    >
                      <option :value="null">No Task</option>
                      <option
                        v-for="task in availableTasks"
                        :key="task.id"
                        :value="task.id"
                      >
                        {{ task.title }}
                      </option>
                    </select>
                  </div>
                </div>
              </div>

              <!-- Time Entry -->
              <div>
                <h4 class="text-sm font-semibold text-gray-900 mb-4">Time Details</h4>
                <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-2">
                      Work Date <span class="text-red-500">*</span>
                    </label>
                    <input
                      v-model="formData.workDate"
                      type="date"
                      class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                      :class="{ 'border-red-500': errors.workDate }"
                    />
                    <p v-if="errors.workDate" class="mt-1 text-sm text-red-500">{{ errors.workDate }}</p>
                  </div>

                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-2">Worker ID</label>
                    <input
                      v-model="formData.userId"
                      type="text"
                      class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                      placeholder="Enter worker user ID"
                    />
                  </div>

                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-2">
                      Hours <span class="text-red-500">*</span>
                    </label>
                    <input
                      v-model.number="formData.hours"
                      type="number"
                      step="0.25"
                      min="0"
                      class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                      :class="{ 'border-red-500': errors.hours }"
                      placeholder="0.0"
                    />
                    <p v-if="errors.hours" class="mt-1 text-sm text-red-500">{{ errors.hours }}</p>
                  </div>

                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-2">
                      Hourly Rate (R) <span class="text-red-500">*</span>
                    </label>
                    <input
                      v-model.number="formData.rate"
                      type="number"
                      step="0.01"
                      min="0"
                      class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                      :class="{ 'border-red-500': errors.rate }"
                      placeholder="150.00"
                    />
                    <p v-if="errors.rate" class="mt-1 text-sm text-red-500">{{ errors.rate }}</p>
                  </div>
                </div>

                <div class="mt-4 p-4 bg-blue-50 rounded-lg">
                  <div class="flex items-center justify-between">
                    <span class="text-sm font-medium text-gray-700">Total Cost:</span>
                    <span class="text-lg font-bold text-blue-600">
                      R {{ totalCost.toFixed(2) }}
                    </span>
                  </div>
                </div>
              </div>

              <!-- Description -->
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">Description</label>
                <textarea
                  v-model="formData.description"
                  rows="3"
                  class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                  placeholder="Describe the work performed..."
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
                {{ isSubmitting ? 'Saving...' : isEditing ? 'Update' : 'Log Time' }}
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



