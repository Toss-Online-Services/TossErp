<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useProjectsStore, type ProjectTask, type TaskStatus } from '~/stores/projects'

interface Props {
  show: boolean
  task?: ProjectTask | null
  projectId?: number | null
}

const props = withDefaults(defineProps<Props>(), {
  task: null,
  projectId: null
})

const emit = defineEmits<{
  close: []
  saved: [task: ProjectTask]
}>()

const projectsStore = useProjectsStore()

// Form state
const formData = ref({
  projectId: null as number | null,
  title: '',
  description: '',
  status: 'ToDo' as TaskStatus,
  assigneeId: '',
  dueDate: '',
  estimatedHours: null as number | null
})

const isEditing = computed(() => !!props.task)
const isSubmitting = ref(false)
const errors = ref<Record<string, string>>({})

const statusOptions: TaskStatus[] = ['ToDo', 'InProgress', 'Done', 'Cancelled']

// Watch for task changes
watch(() => props.task, (newTask) => {
  if (newTask) {
    formData.value = {
      projectId: newTask.projectId,
      title: newTask.title,
      description: newTask.description || '',
      status: newTask.status,
      assigneeId: newTask.assigneeId || '',
      dueDate: newTask.dueDate ? new Date(newTask.dueDate).toISOString().split('T')[0] : '',
      estimatedHours: newTask.estimatedHours || null
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

function resetForm() {
  formData.value = {
    projectId: props.projectId || null,
    title: '',
    description: '',
    status: 'ToDo',
    assigneeId: '',
    dueDate: '',
    estimatedHours: null
  }
  errors.value = {}
}

function validate() {
  errors.value = {}
  
  if (!formData.value.title.trim()) {
    errors.value.title = 'Task title is required'
  }
  
  if (!formData.value.projectId) {
    errors.value.projectId = 'Project is required'
  }
  
  return Object.keys(errors.value).length === 0
}

async function handleSave() {
  if (!validate()) return
  
  isSubmitting.value = true
  try {
    const taskData = {
      projectId: formData.value.projectId!,
      title: formData.value.title.trim(),
      description: formData.value.description.trim() || undefined,
      status: formData.value.status,
      assigneeId: formData.value.assigneeId || undefined,
      dueDate: formData.value.dueDate ? new Date(formData.value.dueDate) : undefined,
      estimatedHours: formData.value.estimatedHours || undefined
    }
    
    if (isEditing.value && props.task) {
      await projectsStore.updateTask(props.task.id, taskData)
      emit('saved', { ...props.task, ...taskData } as ProjectTask)
    } else {
      const newTask = await projectsStore.createTask(taskData)
      emit('saved', newTask)
    }
    emit('close')
    resetForm()
  } catch (error) {
    console.error('Failed to save task:', error)
    alert('Failed to save task. Please try again.')
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
                {{ isEditing ? 'Edit Task' : 'New Task' }}
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
                <h4 class="text-sm font-semibold text-gray-900 mb-4">Task Information</h4>
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
                    <label class="block text-sm font-medium text-gray-700 mb-2">
                      Task Title <span class="text-red-500">*</span>
                    </label>
                    <input
                      v-model="formData.title"
                      type="text"
                      class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                      :class="{ 'border-red-500': errors.title }"
                      placeholder="Enter task title"
                    />
                    <p v-if="errors.title" class="mt-1 text-sm text-red-500">{{ errors.title }}</p>
                  </div>

                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-2">Description</label>
                    <textarea
                      v-model="formData.description"
                      rows="3"
                      class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                      placeholder="Enter task description"
                    />
                  </div>

                  <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
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
                      <label class="block text-sm font-medium text-gray-700 mb-2">Due Date</label>
                      <input
                        v-model="formData.dueDate"
                        type="date"
                        class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                      />
                    </div>
                  </div>

                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-2">Estimated Hours</label>
                    <input
                      v-model.number="formData.estimatedHours"
                      type="number"
                      step="0.5"
                      min="0"
                      class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                      placeholder="0.0"
                    />
                  </div>

                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-2">Assignee ID</label>
                    <input
                      v-model="formData.assigneeId"
                      type="text"
                      class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                      placeholder="Enter assignee user ID"
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



