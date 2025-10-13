<template>
  <div class="fixed inset-0 bg-gray-500 bg-opacity-75 flex items-center justify-center p-4 z-50">
    <div class="bg-white dark:bg-gray-800 rounded-xl shadow-xl max-w-5xl w-full max-h-[90vh] overflow-hidden">
      <!-- Header -->
      <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
        <div class="flex items-center justify-between">
          <div class="flex items-center">
            <div class="w-10 h-10 bg-blue-100 dark:bg-blue-900 rounded-lg flex items-center justify-center mr-4">
              <BuildingStorefrontIcon class="w-6 h-6 text-blue-600 dark:text-blue-400" />
            </div>
            <div>
              <h3 class="text-lg font-semibold text-gray-900 dark:text-white">
                {{ warehouse?.name }}
              </h3>
              <p class="text-sm text-gray-500 dark:text-gray-400">
                {{ warehouse?.code }} • {{ warehouse?.type }}
              </p>
            </div>
          </div>
          <div class="flex items-center space-x-2">
            <button
              @click="warehouse && $emit('edit', warehouse)"
              class="inline-flex items-center px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg text-sm font-medium text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600"
            >
              <PencilIcon class="w-4 h-4 mr-1" />
              Edit
            </button>
            <button
              @click="$emit('close')"
              class="text-gray-400 hover:text-gray-600 dark:hover:text-gray-300"
            >
              <XMarkIcon class="w-6 h-6" />
            </button>
          </div>
        </div>
      </div>

      <!-- Content -->
      <div class="p-6 overflow-y-auto max-h-[calc(90vh-120px)]">
        <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
          <!-- Main Details -->
          <div class="lg:col-span-2 space-y-6">
            <!-- Basic Information -->
            <div>
              <h4 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">Warehouse Information</h4>
              <div class="bg-gray-50 dark:bg-gray-700 rounded-lg p-4">
                <dl class="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div>
                    <dt class="text-sm font-medium text-gray-500 dark:text-gray-400">Code</dt>
                    <dd class="mt-1 text-sm font-mono text-gray-900 dark:text-white">{{ warehouse?.code }}</dd>
                  </div>
                  <div>
                    <dt class="text-sm font-medium text-gray-500 dark:text-gray-400">Type</dt>
                    <dd class="mt-1 text-sm text-gray-900 dark:text-white capitalize">{{ warehouse?.type }}</dd>
                  </div>
                  <div>
                    <dt class="text-sm font-medium text-gray-500 dark:text-gray-400">Status</dt>
                    <dd class="mt-1">
                      <span 
                        class="inline-flex px-2 py-1 text-xs font-semibold rounded-full"
                        :class="{
                          'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200': warehouse?.isActive,
                          'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200': !warehouse?.isActive
                        }"
                      >
                        {{ warehouse?.isActive ? 'Active' : 'Inactive' }}
                      </span>
                    </dd>
                  </div>
                  <div>
                    <dt class="text-sm font-medium text-gray-500 dark:text-gray-400">Type</dt>
                    <dd class="mt-1">
                      <span 
                        class="inline-flex px-2 py-1 text-xs font-semibold rounded-full"
                        :class="{
                          'bg-purple-100 text-purple-800 dark:bg-purple-900 dark:text-purple-200': warehouse?.isGroup,
                          'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200': !warehouse?.isGroup
                        }"
                      >
                        {{ warehouse?.isGroup ? 'Warehouse Group' : 'Single Warehouse' }}
                      </span>
                    </dd>
                  </div>
                </dl>
                <div v-if="warehouse?.description" class="mt-4">
                  <dt class="text-sm font-medium text-gray-500 dark:text-gray-400">Description</dt>
                  <dd class="mt-1 text-sm text-gray-900 dark:text-white">{{ warehouse.description }}</dd>
                </div>
                <div v-if="warehouse?.address" class="mt-4">
                  <dt class="text-sm font-medium text-gray-500 dark:text-gray-400">Address</dt>
                  <dd class="mt-1 text-sm text-gray-900 dark:text-white">{{ warehouse.address }}</dd>
                </div>
              </div>
            </div>

            <!-- Stock Summary -->
            <div>
              <h4 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">Stock Summary</h4>
              <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
                <div class="bg-blue-50 dark:bg-blue-900/20 border border-blue-200 dark:border-blue-800 rounded-lg p-4">
                  <div class="flex items-center">
                    <CubeIcon class="w-8 h-8 text-blue-600 dark:text-blue-400 mr-3" />
                    <div>
                      <p class="text-sm font-medium text-blue-800 dark:text-blue-200">Total Items</p>
                      <p class="text-2xl font-bold text-blue-900 dark:text-blue-100">{{ warehouse?.itemCount || 0 }}</p>
                    </div>
                  </div>
                </div>
                <div class="bg-green-50 dark:bg-green-900/20 border border-green-200 dark:border-green-800 rounded-lg p-4">
                  <div class="flex items-center">
                    <CurrencyDollarIcon class="w-8 h-8 text-green-600 dark:text-green-400 mr-3" />
                    <div>
                      <p class="text-sm font-medium text-green-800 dark:text-green-200">Stock Value</p>
                      <p class="text-2xl font-bold text-green-900 dark:text-green-100">R{{ formatCurrency(warehouse?.stockValue || 0) }}</p>
                    </div>
                  </div>
                </div>
                <div class="bg-purple-50 dark:bg-purple-900/20 border border-purple-200 dark:border-purple-800 rounded-lg p-4">
                  <div class="flex items-center">
                    <ChartBarIcon class="w-8 h-8 text-purple-600 dark:text-purple-400 mr-3" />
                    <div>
                      <p class="text-sm font-medium text-purple-800 dark:text-purple-200">Avg. Value/Item</p>
                      <p class="text-2xl font-bold text-purple-900 dark:text-purple-100">
                        R{{ formatCurrency((warehouse?.stockValue || 0) / (warehouse?.itemCount || 1)) }}
                      </p>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <!-- Recent Movements -->
            <div>
              <h4 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">Recent Movements</h4>
              <div class="bg-white dark:bg-gray-800 border border-gray-200 dark:border-gray-700 rounded-lg">
                <div class="p-4 text-center text-gray-500 dark:text-gray-400">
                  <ArrowsRightLeftIcon class="w-8 h-8 mx-auto mb-2" />
                  <p class="text-sm">Movement history will be displayed here</p>
                  <p class="text-xs">Feature coming soon</p>
                </div>
              </div>
            </div>

            <!-- Items in Warehouse -->
            <div>
              <h4 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">Items in Warehouse</h4>
              <div class="bg-white dark:bg-gray-800 border border-gray-200 dark:border-gray-700 rounded-lg">
                <div class="p-4 text-center text-gray-500 dark:text-gray-400">
                  <CubeIcon class="w-8 h-8 mx-auto mb-2" />
                  <p class="text-sm">Item list will be displayed here</p>
                  <p class="text-xs">Feature coming soon</p>
                </div>
              </div>
            </div>
          </div>

          <!-- Sidebar Actions -->
          <div class="space-y-6">
            <!-- Quick Actions -->
            <div>
              <h4 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">Quick Actions</h4>
              <div class="space-y-3">
                <button
                  @click="viewStock"
                  class="w-full inline-flex items-center justify-center px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg text-sm font-medium text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600"
                >
                  <CubeIcon class="w-4 h-4 mr-2" />
                  View All Stock
                </button>
                <button
                  @click="receiveStock"
                  class="w-full inline-flex items-center justify-center px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg text-sm font-medium text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600"
                >
                  <ArrowDownIcon class="w-4 h-4 mr-2" />
                  Receive Stock
                </button>
                <button
                  @click="issueStock"
                  class="w-full inline-flex items-center justify-center px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg text-sm font-medium text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600"
                >
                  <ArrowUpIcon class="w-4 h-4 mr-2" />
                  Issue Stock
                </button>
                <button
                  @click="transferStock"
                  class="w-full inline-flex items-center justify-center px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg text-sm font-medium text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600"
                >
                  <ArrowRightLeftIcon class="w-4 h-4 mr-2" />
                  Transfer Stock
                </button>
              </div>
            </div>

            <!-- Warehouse Image Placeholder -->
            <div>
              <h4 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">Warehouse Image</h4>
              <div class="aspect-square bg-gray-100 dark:bg-gray-700 rounded-lg flex items-center justify-center">
                <div class="text-center">
                  <PhotoIcon class="w-16 h-16 text-gray-400 mx-auto mb-2" />
                  <p class="text-sm text-gray-500 dark:text-gray-400">No image available</p>
                </div>
              </div>
            </div>

            <!-- Key Information -->
            <div>
              <h4 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">Key Information</h4>
              <div class="space-y-4">
                <div class="bg-gray-50 dark:bg-gray-700 rounded-lg p-3">
                  <div class="text-sm font-medium text-gray-700 dark:text-gray-300">Created</div>
                  <div class="text-sm text-gray-600 dark:text-gray-400">
                    {{ warehouse?.createdAt ? formatDate(warehouse.createdAt) : 'N/A' }}
                  </div>
                </div>
                <div class="bg-gray-50 dark:bg-gray-700 rounded-lg p-3">
                  <div class="text-sm font-medium text-gray-700 dark:text-gray-300">Last Updated</div>
                  <div class="text-sm text-gray-600 dark:text-gray-400">
                    {{ warehouse?.updatedAt ? formatDate(warehouse.updatedAt) : 'N/A' }}
                  </div>
                </div>
              </div>
            </div>

            <!-- Danger Zone -->
            <div v-if="warehouse?.isActive">
              <h4 class="text-lg font-semibold text-red-600 dark:text-red-400 mb-4">Danger Zone</h4>
              <button
                @click="deactivate"
                class="w-full inline-flex items-center justify-center px-4 py-2 border border-red-300 dark:border-red-600 rounded-lg text-sm font-medium text-red-700 dark:text-red-300 bg-red-50 dark:bg-red-900/20 hover:bg-red-100 dark:hover:bg-red-900/30"
              >
                <XCircleIcon class="w-4 h-4 mr-2" />
                Deactivate Warehouse
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import {
  XMarkIcon,
  BuildingStorefrontIcon,
  PencilIcon,
  CubeIcon,
  CurrencyDollarIcon,
  ChartBarIcon,
  ArrowsRightLeftIcon,
  ArrowDownIcon,
  ArrowUpIcon,
  ArrowRightLeftIcon,
  PhotoIcon,
  XCircleIcon
} from '@heroicons/vue/24/outline'
import type { WarehouseDto } from '../../composables/useStock'

// Props
interface Props {
  warehouse?: WarehouseDto | null
}

const props = withDefaults(defineProps<Props>(), {
  warehouse: null
})

// Emits
const emit = defineEmits<{
  close: []
  edit: [warehouse: WarehouseDto]
  deactivate: [warehouse: WarehouseDto]
}>()

// Methods
const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-ZA', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  }).format(amount)
}

const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString('en-ZA', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  })
}

const viewStock = () => {
  alert('View stock feature - Navigate to stock levels filtered by this warehouse')
}

const receiveStock = () => {
  alert('Receive stock feature - Open stock receipt modal for this warehouse')
}

const issueStock = () => {
  alert('Issue stock feature - Open stock issue modal for this warehouse')
}

const transferStock = () => {
  alert('Transfer stock feature - Open stock transfer modal with this warehouse as source')
}

const deactivate = () => {
  if (!props.warehouse) return
  
  if (confirm(`Are you sure you want to deactivate ${props.warehouse.name}? This will prevent new stock movements.`)) {
    emit('deactivate', props.warehouse)
  }
}
</script>

