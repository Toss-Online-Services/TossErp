<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import { useProjectsStore, type Project, type ProjectStatus } from '~/stores/projects'
import { useCrmStore } from '~/stores/crm'

interface Props {
  show: boolean
  project?: Project | null
}

const props = withDefaults(defineProps<Props>(), {
  project: null
})

const emit = defineEmits<{
  close: []
  saved: [project: Project]
}>()

const projectsStore = useProjectsStore()
const crmStore = useCrmStore()

// Form state
const formData = ref({
  title: '',
  description: '',
  status: 'New' as ProjectStatus,
  customerId: null as number | null,
  shopId: null as number | null,
  startDate: '',
  expectedCompletionDate: '',
  totalCost: 0,
  totalRevenue: 0
})

const isEditing = computed(() => !!props.project)
const isSubmitting = ref(false)
const errors = ref<Record<string, string>>({})

const statusOptions: ProjectStatus[] = ['New', 'InProgress', 'OnHold', 'Completed', 'Closed', 'Cancelled']

// Watch for project changes
watch(() => props.project, (newProject) => {
  if (newProject) {
    formData.value = {
      title: newProject.title,
      description: newProject.description || '',
      status: newProject.status,
      customerId: newProject.customerId || null,
      shopId: newProject.shopId || null,
      startDate: newProject.startDate ? new Date(newProject.startDate).toISOString().split('T')[0] : '',
      expectedCompletionDate: newProject.expectedCompletionDate ? new Date(newProject.expectedCompletionDate).toISOString().split('T')[0] : '',
      totalCost: newProject.totalCost || 0,
      totalRevenue: newProject.totalRevenue || 0
    }
  } else {
    resetForm()
  }
}, { immediate: true })

function resetForm() {
  formData.value = {
    title: '',
    description: '',
    status: 'New',
    customerId: null,
    shopId: null,
    startDate: '',
    expectedCompletionDate: '',
    totalCost: 0,
    totalRevenue: 0
  }
  errors.value = {}
}

function validate() {
  errors.value = {}
  
  if (!formData.value.title.trim()) {
    errors.value.title = 'Project title is required'
  }
  
  return Object.keys(errors.value).length === 0
}

async function handleSave() {
  if (!validate()) return
  
  isSubmitting.value = true
  try {
    const projectData = {
      title: formData.value.title.trim(),
      description: formData.value.description.trim() || undefined,
      status: formData.value.status,
      customerId: formData.value.customerId || undefined,
      shopId: formData.value.shopId || undefined,
      startDate: formData.value.startDate ? new Date(formData.value.startDate) : undefined,
      expectedCompletionDate: formData.value.expectedCompletionDate ? new Date(formData.value.expectedCompletionDate) : undefined,
      totalCost: formData.value.totalCost,
      totalRevenue: formData.value.totalRevenue || undefined
    }
    
    if (isEditing.value && props.project) {
      await projectsStore.updateProject(props.project.id, projectData)
      emit('saved', { ...props.project, ...projectData } as Project)
    } else {
      const newProject = await projectsStore.createProject(projectData)
      emit('saved', newProject)
    }
    emit('close')
    resetForm()
  } catch (error) {
    console.error('Failed to save project:', error)
    alert('Failed to save project. Please try again.')
  } finally {
    isSubmitting.value = false
  }
}

function handleClose() {
  emit('close')
  resetForm()
}

// Load customers for dropdown
onMounted(() => {
  crmStore.fetchCustomers()
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
            class="relative w-full max-w-3xl rounded-xl bg-white shadow-xl max-h-[90vh] flex flex-col"
            @click.stop
          >
            <!-- Header -->
            <div class="flex items-center justify-between border-b border-gray-200 px-6 py-4">
              <h3 class="text-xl font-bold text-gray-900">
                {{ isEditing ? 'Edit Project' : 'New Project' }}
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
                      Project Title <span class="text-red-500">*</span>
                    </label>
                    <input
                      v-model="formData.title"
                      type="text"
                      class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                      :class="{ 'border-red-500': errors.title }"
                      placeholder="Enter project title"
                    />
                    <p v-if="errors.title" class="mt-1 text-sm text-red-500">{{ errors.title }}</p>
                  </div>

                  <div class="md:col-span-2">
                    <label class="block text-sm font-medium text-gray-700 mb-2">Description</label>
                    <textarea
                      v-model="formData.description"
                      rows="3"
                      class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                      placeholder="Enter project description"
                    />
                  </div>

                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-2">Status</label>
                    <select
                      v-model="formData.status"
                      class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    >
                      <option v-for="status in statusOptions" :key="status" :value="status">
                        {{ status }}
                      </option>
                    </select>
                  </div>

                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-2">Customer</label>
                    <select
                      v-model="formData.customerId"
                      class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    >
                      <option :value="null">No Customer</option>
                      <option
                        v-for="customer in crmStore.customers"
                        :key="customer.id"
                        :value="customer.id"
                      >
                        {{ customer.name }}
                      </option>
                    </select>
                  </div>
                </div>
              </div>

              <!-- Dates -->
              <div>
                <h4 class="text-sm font-semibold text-gray-900 mb-4">Timeline</h4>
                <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-2">Start Date</label>
                    <input
                      v-model="formData.startDate"
                      type="date"
                      class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    />
                  </div>

                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-2">Expected Completion</label>
                    <input
                      v-model="formData.expectedCompletionDate"
                      type="date"
                      class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    />
                  </div>
                </div>
              </div>

              <!-- Financial -->
              <div>
                <h4 class="text-sm font-semibold text-gray-900 mb-4">Financial</h4>
                <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-2">Total Cost</label>
                    <input
                      v-model.number="formData.totalCost"
                      type="number"
                      step="0.01"
                      min="0"
                      class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                      placeholder="0.00"
                    />
                  </div>

                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-2">Total Revenue</label>
                    <input
                      v-model.number="formData.totalRevenue"
                      type="number"
                      step="0.01"
                      min="0"
                      class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                      placeholder="0.00"
                    />
                  </div>
                </div>
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

