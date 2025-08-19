<template>
  <div class="min-h-screen bg-gray-50 py-8">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <!-- Page Header -->
      <div class="mb-8">
        <h1 class="text-3xl font-bold text-gray-900">Component Library</h1>
        <p class="mt-2 text-gray-600">Reusable UI components for TOSS ERP III platform</p>
      </div>

      <!-- Card Component Demo -->
      <section class="mb-12">
        <h2 class="text-2xl font-semibold text-gray-900 mb-6">Card Component</h2>
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          <!-- Basic Card -->
          <Card title="Basic Card" subtitle="Simple card with title and content">
            <p class="text-gray-600">This is a basic card component with title, subtitle, and content area.</p>
          </Card>

          <!-- Card with Header Actions -->
          <Card title="Card with Actions" subtitle="Card with header actions">
            <template #headerActions>
              <button class="px-3 py-1 text-sm bg-orange-600 text-white rounded hover:bg-orange-700">
                Action
              </button>
            </template>
            <p class="text-gray-600">This card has header actions and custom content.</p>
          </Card>

          <!-- Card with Footer -->
          <Card title="Card with Footer" subtitle="Card with footer content">
            <p class="text-gray-600">This card includes a footer section with additional actions.</p>
            <template #footer>
              <div class="flex justify-end space-x-2">
                <button class="px-3 py-1 text-sm bg-gray-500 text-white rounded hover:bg-gray-600">
                  Cancel
                </button>
                <button class="px-3 py-1 text-sm bg-orange-600 text-white rounded hover:bg-orange-700">
                  Save
                </button>
              </div>
            </template>
          </Card>
        </div>
      </section>

      <!-- Table Component Demo -->
      <section class="mb-12">
        <h2 class="text-2xl font-semibold text-gray-900 mb-6">Table Component</h2>
        <Table 
          title="Sample Data Table" 
          subtitle="Reusable table component with sorting and formatting"
          :columns="tableColumns"
          :data="tableData"
        >
          <template #headerActions>
            <button class="px-3 py-1 text-sm bg-orange-600 text-white rounded hover:bg-orange-700">
              Add Item
            </button>
          </template>
          
          <!-- Custom cell formatting -->
          <template #cell-status="{ value }">
            <span 
              class="px-2 py-1 text-xs font-medium rounded-full"
              :class="value === 'active' ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'"
            >
              {{ value }}
            </span>
          </template>
        </Table>
      </section>

      <!-- Modal Component Demo -->
      <section class="mb-12">
        <h2 class="text-2xl font-semibold text-gray-900 mb-6">Modal Component</h2>
        <div class="space-x-4">
          <button 
            @click="showSmallModal = true"
            class="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700"
          >
            Small Modal
          </button>
          <button 
            @click="showMediumModal = true"
            class="px-4 py-2 bg-green-600 text-white rounded hover:bg-green-700"
          >
            Medium Modal
          </button>
          <button 
            @click="showLargeModal = true"
            class="px-4 py-2 bg-purple-600 text-white rounded hover:bg-purple-700"
          >
            Large Modal
          </button>
        </div>

        <!-- Small Modal -->
        <Modal 
          v-model:is-open="showSmallModal" 
          title="Small Modal" 
          size="small"
        >
          <p class="text-gray-600">This is a small modal with limited content.</p>
          <template #footer>
            <button @click="showSmallModal = false" class="px-3 py-1 text-sm bg-gray-500 text-white rounded hover:bg-gray-600">
              Close
            </button>
          </template>
        </Modal>

        <!-- Medium Modal -->
        <Modal 
          v-model:is-open="showMediumModal" 
          title="Medium Modal" 
          size="medium"
        >
          <p class="text-gray-600">This is a medium-sized modal with more content space.</p>
          <div class="mt-4 p-4 bg-gray-50 rounded">
            <p class="text-sm text-gray-600">Additional content can be placed here.</p>
          </div>
          <template #footer>
            <button @click="showMediumModal = false" class="px-3 py-1 text-sm bg-gray-500 text-white rounded hover:bg-gray-600">
              Cancel
            </button>
            <button @click="showMediumModal = false" class="px-3 py-1 text-sm bg-orange-600 text-white rounded hover:bg-orange-700">
              Confirm
            </button>
          </template>
        </Modal>

        <!-- Large Modal -->
        <Modal 
          v-model:is-open="showLargeModal" 
          title="Large Modal" 
          size="large"
        >
          <p class="text-gray-600">This is a large modal with extensive content space.</p>
          <div class="mt-4 grid grid-cols-2 gap-4">
            <div class="p-4 bg-gray-50 rounded">
              <h4 class="font-medium text-gray-900 mb-2">Section 1</h4>
              <p class="text-sm text-gray-600">Content for the first section.</p>
            </div>
            <div class="p-4 bg-gray-50 rounded">
              <h4 class="font-medium text-gray-900 mb-2">Section 2</h4>
              <p class="text-sm text-gray-600">Content for the second section.</p>
            </div>
          </div>
          <template #footer>
            <button @click="showLargeModal = false" class="px-3 py-1 text-sm bg-gray-500 text-white rounded hover:bg-gray-600">
              Cancel
            </button>
            <button @click="showLargeModal = false" class="px-3 py-1 text-sm bg-orange-600 text-white rounded hover:bg-orange-700">
              Save Changes
            </button>
          </template>
        </Modal>
      </section>

      <!-- Chart Component Demo -->
      <section class="mb-12">
        <h2 class="text-2xl font-semibold text-gray-900 mb-6">Chart Component</h2>
        <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
          <!-- Basic Chart -->
          <Chart title="Revenue Chart" subtitle="Monthly revenue data">
            <div class="h-64 flex items-center justify-center bg-gray-50 rounded">
              <p class="text-gray-500">Chart implementation placeholder</p>
            </div>
          </Chart>

          <!-- Chart with Loading State -->
          <Chart title="Loading Chart" :loading="chartLoading" subtitle="Chart with loading state">
            <div class="h-64 flex items-center justify-center bg-gray-50 rounded">
              <p class="text-gray-500">Chart content</p>
            </div>
          </Chart>
        </div>

        <!-- Chart Controls -->
        <div class="mt-4 flex space-x-4">
          <button 
            @click="chartLoading = !chartLoading"
            class="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700"
          >
            Toggle Loading
          </button>
          <button 
            @click="showChartError = !showChartError"
            class="px-4 py-2 bg-red-600 text-white rounded hover:bg-red-700"
          >
            Toggle Error
          </button>
        </div>

        <!-- Chart with Error State -->
        <div class="mt-6">
          <Chart 
            title="Error Chart" 
            :error="showChartError ? 'Failed to load chart data' : ''"
            subtitle="Chart with error handling"
          >
            <div class="h-64 flex items-center justify-center bg-gray-50 rounded">
              <p class="text-gray-500">Chart content</p>
            </div>
          </Chart>
        </div>
      </section>

      <!-- Notification Component Demo -->
      <section class="mb-12">
        <h2 class="text-2xl font-semibold text-gray-900 mb-6">Notification Component</h2>
        <div class="space-x-4">
          <button 
            @click="showNotification('success', 'Success!', 'Operation completed successfully.')"
            class="px-4 py-2 bg-green-600 text-white rounded hover:bg-green-700"
          >
            Success Notification
          </button>
          <button 
            @click="showNotification('error', 'Error!', 'Something went wrong.')"
            class="px-4 py-2 bg-red-600 text-white rounded hover:bg-red-700"
          >
            Error Notification
          </button>
          <button 
            @click="showNotification('warning', 'Warning!', 'Please review your input.')"
            class="px-4 py-2 bg-yellow-600 text-white rounded hover:bg-yellow-700"
          >
            Warning Notification
          </button>
          <button 
            @click="showNotification('info', 'Info', 'Here is some information.')"
            class="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700"
          >
            Info Notification
          </button>
        </div>

        <!-- Notification with Actions -->
        <div class="mt-4">
          <button 
            @click="showNotificationWithActions"
            class="px-4 py-2 bg-purple-600 text-white rounded hover:bg-purple-700"
          >
            Notification with Actions
          </button>
        </div>
      </section>

      <!-- Notification Instances -->
      <Notification
        v-model:is-visible="notifications.success.isVisible"
        :type="notifications.success.type"
        :title="notifications.success.title"
        :message="notifications.success.message"
        :duration="notifications.success.duration"
      >
        <template #actions>
          <button 
            @click="notifications.success.isVisible = false"
            class="text-sm font-medium text-green-800 hover:text-green-900"
          >
            Dismiss
          </button>
        </template>
      </Notification>

      <Notification
        v-model:is-visible="notifications.error.isVisible"
        :type="notifications.error.type"
        :title="notifications.error.title"
        :message="notifications.error.message"
        :duration="notifications.error.duration"
      />

      <Notification
        v-model:is-visible="notifications.warning.isVisible"
        :type="notifications.warning.type"
        :title="notifications.warning.title"
        :message="notifications.warning.message"
        :duration="notifications.warning.duration"
      />

      <Notification
        v-model:is-visible="notifications.info.isVisible"
        :type="notifications.info.type"
        :title="notifications.info.title"
        :message="notifications.info.message"
        :duration="notifications.info.duration"
      />

      <Notification
        v-model:is-visible="notifications.withActions.isVisible"
        :type="notifications.withActions.type"
        :title="notifications.withActions.title"
        :message="notifications.withActions.message"
        :duration="notifications.withActions.duration"
      >
        <template #actions>
          <button 
            @click="handleNotificationAction('undo')"
            class="text-sm font-medium text-blue-800 hover:text-blue-900 mr-2"
          >
            Undo
          </button>
          <button 
            @click="notifications.withActions.isVisible = false"
            class="text-sm font-medium text-gray-800 hover:text-gray-900"
          >
            Dismiss
          </button>
        </template>
      </Notification>
    </div>
  </div>
</template>

<script setup lang="ts">
// Reactive state
const showSmallModal = ref(false)
const showMediumModal = ref(false)
const showLargeModal = ref(false)
const chartLoading = ref(false)
const showChartError = ref(false)

// Table data
const tableColumns = [
  { key: 'id', label: 'ID', class: 'w-16' },
  { key: 'name', label: 'Name' },
  { key: 'email', label: 'Email' },
  { key: 'status', label: 'Status', class: 'w-24' },
  { key: 'createdAt', label: 'Created', formatter: (value: string) => new Date(value).toLocaleDateString() }
]

const tableData = [
  { id: 1, name: 'John Doe', email: 'john@example.com', status: 'active', createdAt: '2024-01-15' },
  { id: 2, name: 'Jane Smith', email: 'jane@example.com', status: 'inactive', createdAt: '2024-01-10' },
  { id: 3, name: 'Bob Johnson', email: 'bob@example.com', status: 'active', createdAt: '2024-01-05' },
  { id: 4, name: 'Alice Brown', email: 'alice@example.com', status: 'pending', createdAt: '2024-01-01' }
]

// Notification system
const notifications = reactive({
  success: { isVisible: false, type: 'success', title: '', message: '', duration: 5000 },
  error: { isVisible: false, type: 'error', title: '', message: '', duration: 5000 },
  warning: { isVisible: false, type: 'warning', title: '', message: '', duration: 5000 },
  info: { isVisible: false, type: 'info', title: '', message: '', duration: 5000 },
  withActions: { isVisible: false, type: 'info', title: '', message: '', duration: 10000 }
})

const showNotification = (type: 'success' | 'error' | 'warning' | 'info', title: string, message: string) => {
  notifications[type].title = title
  notifications[type].message = message
  notifications[type].isVisible = true
}

const showNotificationWithActions = () => {
  notifications.withActions.title = 'Action Required'
  notifications.withActions.message = 'Please review the changes before proceeding.'
  notifications.withActions.isVisible = true
}

const handleNotificationAction = (action: string) => {
  console.log(`Notification action: ${action}`)
  notifications.withActions.isVisible = false
}
</script>

<style scoped>
/* Page-specific styles */
</style>
