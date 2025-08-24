<template>
  <div class="bulk-import-export">
    <!-- Import Section -->
    <div class="card mb-6">
      <div class="card-header">
        <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Bulk Import</h3>
        <p class="text-sm text-gray-600 dark:text-gray-400">Import multiple items from CSV or Excel file</p>
      </div>
      <div class="card-body">
        <div class="space-y-4">
          <!-- File Upload -->
          <div>
            <label class="form-label">Select File</label>
            <div class="mt-1 flex justify-center px-6 pt-5 pb-6 border-2 border-gray-300 dark:border-gray-600 border-dashed rounded-lg hover:border-primary-400 dark:hover:border-primary-500 transition-colors">
              <div class="space-y-1 text-center">
                <ArrowUpTrayIcon class="mx-auto h-12 w-12 text-gray-400" />
                <div class="flex text-sm text-gray-600 dark:text-gray-400">
                  <label for="file-upload" class="relative cursor-pointer bg-white dark:bg-gray-800 rounded-md font-medium text-primary-600 hover:text-primary-500 focus-within:outline-none focus-within:ring-2 focus-within:ring-offset-2 focus-within:ring-primary-500">
                    <span>Upload a file</span>
                    <input
                      id="file-upload"
                      ref="fileInput"
                      type="file"
                      class="sr-only"
                      accept=".csv,.xlsx,.xls"
                      @change="handleFileSelect"
                    />
                  </label>
                  <p class="pl-1">or drag and drop</p>
                </div>
                <p class="text-xs text-gray-500 dark:text-gray-400">
                  CSV, Excel files up to 10MB
                </p>
              </div>
            </div>
          </div>

          <!-- File Info -->
          <div v-if="selectedFile" class="bg-gray-50 dark:bg-gray-800 p-4 rounded-lg">
            <div class="flex items-center justify-between">
              <div class="flex items-center space-x-3">
                <DocumentIcon class="h-8 w-8 text-primary-600" />
                <div>
                  <p class="text-sm font-medium text-gray-900 dark:text-white">{{ selectedFile.name }}</p>
                  <p class="text-sm text-gray-500 dark:text-gray-400">
                    {{ formatFileSize(selectedFile.size) }} • {{ selectedFile.type || 'Unknown type' }}
                  </p>
                </div>
              </div>
              <button
                @click="clearFile"
                class="text-gray-400 hover:text-gray-600 dark:hover:text-gray-300"
              >
                <XMarkIcon class="h-5 w-5" />
              </button>
            </div>
          </div>

          <!-- Import Options -->
          <div v-if="selectedFile" class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label class="form-label">Update Existing Items</label>
              <select v-model="importOptions.updateExisting" class="form-input">
                <option value="skip">Skip existing items</option>
                <option value="update">Update existing items</option>
                <option value="replace">Replace existing items</option>
              </select>
            </div>
            <div>
              <label class="form-label">Default Warehouse</label>
              <select v-model="importOptions.defaultWarehouse" class="form-input">
                <option value="">Select warehouse</option>
                <option v-for="warehouse in stockStore.warehouses" :key="warehouse" :value="warehouse">
                  {{ warehouse }}
                </option>
              </select>
            </div>
          </div>

          <!-- Import Button -->
          <div v-if="selectedFile" class="flex justify-end">
            <button
              @click="startImport"
              :disabled="isImporting"
              class="btn-primary disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <ArrowUpTrayIcon v-if="!isImporting" class="w-4 h-4 mr-2" />
              <div v-else class="w-4 h-4 mr-2 border-2 border-white border-t-transparent rounded-full animate-spin"></div>
              {{ isImporting ? 'Importing...' : 'Start Import' }}
            </button>
          </div>

          <!-- Import Progress -->
          <div v-if="importProgress.show" class="bg-blue-50 dark:bg-blue-900/20 p-4 rounded-lg">
            <div class="flex items-center justify-between mb-2">
              <span class="text-sm font-medium text-blue-900 dark:text-blue-100">
                Import Progress
              </span>
              <span class="text-sm text-blue-700 dark:text-blue-300">
                {{ importProgress.processed }}/{{ importProgress.total }}
              </span>
            </div>
            <div class="w-full bg-blue-200 dark:bg-blue-800 rounded-full h-2">
              <div
                class="bg-blue-600 h-2 rounded-full transition-all duration-300"
                :style="{ width: `${(importProgress.processed / importProgress.total) * 100}%` }"
              ></div>
            </div>
            <p class="text-sm text-blue-700 dark:text-blue-300 mt-2">
              {{ importProgress.message }}
            </p>
          </div>

          <!-- Import Results -->
          <div v-if="importResults.show" class="bg-gray-50 dark:bg-gray-800 p-4 rounded-lg">
            <div class="flex items-center justify-between mb-2">
              <h4 class="text-sm font-medium text-gray-900 dark:text-white">Import Results</h4>
              <button
                @click="importResults.show = false"
                class="text-gray-400 hover:text-gray-600 dark:hover:text-gray-300"
              >
                <XMarkIcon class="h-5 w-5" />
              </button>
            </div>
            <div class="space-y-2">
              <div class="flex justify-between text-sm">
                <span class="text-gray-600 dark:text-gray-400">Successfully imported:</span>
                <span class="font-medium text-green-600">{{ importResults.imported }}</span>
              </div>
              <div class="flex justify-between text-sm">
                <span class="text-gray-600 dark:text-gray-400">Errors:</span>
                <span class="font-medium text-red-600">{{ importResults.errors }}</span>
              </div>
              <div v-if="importResults.errors > 0" class="mt-3">
                <button
                  @click="showErrorDetails = !showErrorDetails"
                  class="text-sm text-red-600 hover:text-red-700 dark:text-red-400 dark:hover:text-red-300"
                >
                  {{ showErrorDetails ? 'Hide' : 'Show' }} error details
                </button>
                <div v-if="showErrorDetails" class="mt-2 text-xs text-red-600 dark:text-red-400">
                  <pre class="whitespace-pre-wrap">{{ importResults.errorDetails }}</pre>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Export Section -->
    <div class="card">
      <div class="card-header">
        <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Bulk Export</h3>
        <p class="text-sm text-gray-600 dark:text-gray-400">Export stock data to CSV or Excel format</p>
      </div>
      <div class="card-body">
        <div class="space-y-4">
          <!-- Export Options -->
          <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div>
              <label class="form-label">Format</label>
              <select v-model="exportOptions.format" class="form-input">
                <option value="csv">CSV</option>
                <option value="excel">Excel</option>
              </select>
            </div>
            <div>
              <label class="form-label">Category</label>
              <select v-model="exportOptions.category" class="form-input">
                <option value="">All Categories</option>
                <option v-for="category in stockStore.categories" :key="category" :value="category">
                  {{ category }}
                </option>
              </select>
            </div>
            <div>
              <label class="form-label">Warehouse</label>
              <select v-model="exportOptions.warehouse" class="form-input">
                <option value="">All Warehouses</option>
                <option v-for="warehouse in stockStore.warehouses" :key="warehouse" :value="warehouse">
                  {{ warehouse }}
                </option>
              </select>
            </div>
          </div>

          <!-- Export Button -->
          <div class="flex justify-end">
            <button
              @click="startExport"
              :disabled="isExporting"
              class="btn-outline disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <ArrowDownTrayIcon v-if="!isExporting" class="w-4 h-4 mr-2" />
              <div v-else class="w-4 h-4 mr-2 border-2 border-primary-600 border-t-transparent rounded-full animate-spin"></div>
              {{ isExporting ? 'Exporting...' : 'Export Data' }}
            </button>
          </div>

          <!-- Export Progress -->
          <div v-if="exportProgress.show" class="bg-green-50 dark:bg-green-900/20 p-4 rounded-lg">
            <div class="flex items-center space-x-2">
              <div class="w-4 h-4 border-2 border-green-600 border-t-transparent rounded-full animate-spin"></div>
              <span class="text-sm text-green-900 dark:text-green-100">
                {{ exportProgress.message }}
              </span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import {
  ArrowUpTrayIcon,
  ArrowDownTrayIcon,
  DocumentIcon,
  XMarkIcon
} from '@heroicons/vue/24/outline'
import { useStockStore } from '../stores/stock'
import { getStockApi } from '../services/api'

const stockStore = useStockStore()
const stockApi = getStockApi()

// File handling
const fileInput = ref<HTMLInputElement>()
const selectedFile = ref<File | null>(null)

// Import options
const importOptions = reactive({
  updateExisting: 'skip',
  defaultWarehouse: ''
})

// Import state
const isImporting = ref(false)
const importProgress = reactive({
  show: false,
  processed: 0,
  total: 0,
  message: ''
})

const importResults = reactive({
  show: false,
  imported: 0,
  errors: 0,
  errorDetails: ''
})

const showErrorDetails = ref(false)

// Export options
const exportOptions = reactive({
  format: 'csv' as 'csv' | 'excel',
  category: '',
  warehouse: ''
})

// Export state
const isExporting = ref(false)
const exportProgress = reactive({
  show: false,
  message: ''
})

// Methods
const handleFileSelect = (event: Event) => {
  const target = event.target as HTMLInputElement
  if (target.files && target.files.length > 0) {
    selectedFile.value = target.files[0]
  }
}

const clearFile = () => {
  selectedFile.value = null
  if (fileInput.value) {
    fileInput.value.value = ''
  }
}

const formatFileSize = (bytes: number): string => {
  if (bytes === 0) return '0 Bytes'
  const k = 1024
  const sizes = ['Bytes', 'KB', 'MB', 'GB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i]
}

const startImport = async () => {
  if (!selectedFile.value) return

  try {
    isImporting.value = true
    importProgress.show = true
    importProgress.processed = 0
    importProgress.total = 100
    importProgress.message = 'Preparing import...'

    // Simulate import progress (replace with actual API call)
    await simulateImportProgress()

    // Mock import results
    importResults.show = true
    importResults.imported = Math.floor(Math.random() * 50) + 10
    importResults.errors = Math.floor(Math.random() * 5)
    importResults.errorDetails = importResults.errors > 0 
      ? 'Sample error details:\n- Invalid SKU format on line 15\n- Missing required field on line 23'
      : ''

    // Clear file after successful import
    clearFile()
  } catch (error) {
    console.error('Import failed:', error)
    // Show error message
  } finally {
    isImporting.value = false
    importProgress.show = false
  }
}

const simulateImportProgress = async () => {
  const steps = [
    { progress: 20, message: 'Reading file...' },
    { progress: 40, message: 'Validating data...' },
    { progress: 60, message: 'Processing items...' },
    { progress: 80, message: 'Saving to database...' },
    { progress: 100, message: 'Import completed!' }
  ]

  for (const step of steps) {
    importProgress.processed = step.progress
    importProgress.message = step.message
    await new Promise(resolve => setTimeout(resolve, 800))
  }
}

const startExport = async () => {
  try {
    isExporting.value = true
    exportProgress.show = true
    exportProgress.message = 'Preparing export...'

    // Simulate export delay
    await new Promise(resolve => setTimeout(resolve, 2000))

    // Mock export completion
    exportProgress.message = 'Export completed! Downloading file...'
    
    // Simulate file download
    setTimeout(() => {
      exportProgress.show = false
      // In real implementation, trigger file download here
    }, 1000)

  } catch (error) {
    console.error('Export failed:', error)
    // Show error message
  } finally {
    isExporting.value = false
  }
}

// Initialize store data
stockStore.loadMockData()
</script>

<style scoped>
.bulk-import-export {
  @apply space-y-6;
}

.card {
  @apply bg-white dark:bg-gray-900 border border-gray-200 dark:border-gray-700 rounded-lg shadow-sm;
}

.card-header {
  @apply px-6 py-4 border-b border-gray-200 dark:border-gray-700;
}

.card-body {
  @apply px-6 py-4;
}

.form-label {
  @apply block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2;
}

.form-input {
  @apply block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm placeholder-gray-400 focus:outline-none focus:ring-primary-500 focus:border-primary-500 dark:bg-gray-800 dark:text-white;
}

.btn-primary {
  @apply inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-primary-600 hover:bg-primary-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary-500 disabled:opacity-50 disabled:cursor-not-allowed;
}

.btn-outline {
  @apply inline-flex items-center px-4 py-2 border border-gray-300 dark:border-gray-600 text-sm font-medium rounded-md text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-800 hover:bg-gray-50 dark:hover:bg-gray-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary-500 disabled:opacity-50 disabled:cursor-not-allowed;
}
</style>
