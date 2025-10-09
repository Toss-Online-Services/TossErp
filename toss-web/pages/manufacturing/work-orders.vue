<template>
  <div class="p-6 space-y-6">
    <!-- Page Header -->
    <div class="flex justify-between items-center">
      <div>
        <h1 class="text-3xl font-bold text-slate-900 dark:text-white">Work Orders</h1>
        <p class="text-slate-600 dark:text-slate-400 mt-1">Manage production work orders and track progress</p>
      </div>
      <div class="flex gap-3">
        <button @click="showCreateModal = true" class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors">
          <span class="flex items-center gap-2">
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6"/>
            </svg>
            Create Work Order
          </span>
        </button>
        <ExportButton
          :data="filteredWorkOrders"
          filename="work_orders"
          title="Work Orders Report"
          data-type="workOrders"
          @export-start="() => {}"
          @export-complete="(format) => showNotification(`Work orders exported as ${format.toUpperCase()}`, 'success')"
          @export-error="(error) => showNotification(error, 'error')"
        />
      </div>
    </div>

    <!-- Filters and Stats -->
    <div class="grid grid-cols-1 lg:grid-cols-4 gap-6">
      <!-- Filters -->
      <div class="lg:col-span-3 bg-white dark:bg-slate-800 rounded-lg shadow p-6">
        <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
          <div>
            <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Search</label>
            <input
              v-model="searchQuery"
              type="text"
              placeholder="Search work orders..."
              class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white"
            />
          </div>
          <div>
            <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Status</label>
            <select
              v-model="selectedStatus"
              class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white"
            >
              <option value="">All Status</option>
              <option value="Draft">Draft</option>
              <option value="Released">Released</option>
              <option value="In Progress">In Progress</option>
              <option value="Completed">Completed</option>
              <option value="Cancelled">Cancelled</option>
            </select>
          </div>
          <div>
            <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Priority</label>
            <select
              v-model="selectedPriority"
              class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white"
            >
              <option value="">All Priorities</option>
              <option value="1">Critical (1)</option>
              <option value="2">High (2)</option>
              <option value="3">Medium (3)</option>
              <option value="4">Low (4)</option>
            </select>
          </div>
          <div class="flex items-end">
            <button @click="clearFilters" class="px-4 py-2 text-slate-600 dark:text-slate-400 hover:text-slate-900 dark:hover:text-white transition-colors">
              Clear Filters
            </button>
          </div>
        </div>
      </div>

      <!-- Quick Stats -->
      <div class="bg-white dark:bg-slate-800 rounded-lg shadow p-6">
        <h3 class="text-lg font-semibold text-slate-900 dark:text-white mb-4">Quick Stats</h3>
        <div class="space-y-3">
          <div class="flex justify-between">
            <span class="text-slate-600 dark:text-slate-400">Total:</span>
            <span class="font-medium text-slate-900 dark:text-white">{{ workOrders.length }}</span>
          </div>
          <div class="flex justify-between">
            <span class="text-slate-600 dark:text-slate-400">Active:</span>
            <span class="font-medium text-blue-600 dark:text-blue-400">{{ getStatusCount('Released') + getStatusCount('In Progress') }}</span>
          </div>
          <div class="flex justify-between">
            <span class="text-slate-600 dark:text-slate-400">Completed:</span>
            <span class="font-medium text-green-600 dark:text-green-400">{{ getStatusCount('Completed') }}</span>
          </div>
          <div class="flex justify-between">
            <span class="text-slate-600 dark:text-slate-400">Overdue:</span>
            <span class="font-medium text-red-600 dark:text-red-400">{{ getOverdueCount() }}</span>
          </div>
        </div>
      </div>
    </div>

    <!-- Work Orders Kanban Board -->
    <div class="bg-white dark:bg-slate-800 rounded-lg shadow">
      <div class="px-6 py-4 border-b border-slate-200 dark:border-slate-700 flex justify-between items-center">
        <h2 class="text-xl font-semibold text-slate-900 dark:text-white">Work Order Board</h2>
        <div class="flex gap-2">
          <button
            @click="viewMode = 'kanban'"
            :class="viewMode === 'kanban' ? 'bg-blue-600 text-white' : 'text-slate-600 dark:text-slate-400'"
            class="px-3 py-1 rounded text-sm transition-colors"
          >
            Kanban
          </button>
          <button
            @click="viewMode = 'list'"
            :class="viewMode === 'list' ? 'bg-blue-600 text-white' : 'text-slate-600 dark:text-slate-400'"
            class="px-3 py-1 rounded text-sm transition-colors"
          >
            List
          </button>
        </div>
      </div>

      <!-- Kanban View -->
      <div v-if="viewMode === 'kanban'" class="p-6">
        <div class="grid grid-cols-1 lg:grid-cols-4 gap-6">
          <!-- Draft Column -->
          <div class="bg-slate-50 dark:bg-slate-900 rounded-lg p-4">
            <h3 class="font-semibold text-slate-700 dark:text-slate-300 mb-3 flex items-center justify-between">
              Draft
              <span class="bg-slate-200 dark:bg-slate-700 text-slate-600 dark:text-slate-400 text-xs px-2 py-1 rounded-full">
                {{ getWorkOrdersByStatus('Draft').length }}
              </span>
            </h3>
            <div class="space-y-3">
              <div
                v-for="wo in getWorkOrdersByStatus('Draft')"
                :key="wo.id"
                @click="viewWorkOrder(wo)"
                class="bg-white dark:bg-slate-800 p-4 rounded border-l-4 border-slate-400 shadow-sm hover:shadow-md transition-shadow cursor-pointer"
              >
                <div class="font-semibold text-sm text-slate-900 dark:text-white">{{ wo.workOrderNumber }}</div>
                <div class="text-xs text-slate-600 dark:text-slate-400 mt-1">{{ wo.productName }}</div>
                <div class="text-xs text-slate-500 dark:text-slate-500 mt-2">Qty: {{ wo.quantityOrdered }}</div>
                <div class="mt-2 flex gap-2">
                  <span class="text-xs px-2 py-1 bg-slate-100 dark:bg-slate-700 text-slate-700 dark:text-slate-300 rounded">{{ wo.type }}</span>
                  <span class="text-xs px-2 py-1 rounded" :class="getPriorityClass(wo.priority)">P{{ wo.priority }}</span>
                </div>
              </div>
            </div>
          </div>

          <!-- Released Column -->
          <div class="bg-blue-50 dark:bg-blue-900/20 rounded-lg p-4">
            <h3 class="font-semibold text-slate-700 dark:text-slate-300 mb-3 flex items-center justify-between">
              Released
              <span class="bg-blue-200 dark:bg-blue-700 text-blue-600 dark:text-blue-400 text-xs px-2 py-1 rounded-full">
                {{ getWorkOrdersByStatus('Released').length }}
              </span>
            </h3>
            <div class="space-y-3">
              <div
                v-for="wo in getWorkOrdersByStatus('Released')"
                :key="wo.id"
                @click="viewWorkOrder(wo)"
                class="bg-white dark:bg-slate-800 p-4 rounded border-l-4 border-blue-400 shadow-sm hover:shadow-md transition-shadow cursor-pointer"
              >
                <div class="font-semibold text-sm text-slate-900 dark:text-white">{{ wo.workOrderNumber }}</div>
                <div class="text-xs text-slate-600 dark:text-slate-400 mt-1">{{ wo.productName }}</div>
                <div class="text-xs text-slate-500 dark:text-slate-500 mt-2">Qty: {{ wo.quantityOrdered }}</div>
                <div class="text-xs text-blue-600 dark:text-blue-400 mt-1">Start: {{ formatDate(wo.plannedStartDate) }}</div>
                <div class="mt-2 flex gap-1">
                  <button @click.stop="startWorkOrder(wo)" class="text-xs px-2 py-1 bg-green-600 text-white rounded hover:bg-green-700">
                    Start
                  </button>
                </div>
              </div>
            </div>
          </div>

          <!-- In Progress Column -->
          <div class="bg-green-50 dark:bg-green-900/20 rounded-lg p-4">
            <h3 class="font-semibold text-slate-700 dark:text-slate-300 mb-3 flex items-center justify-between">
              In Progress
              <span class="bg-green-200 dark:bg-green-700 text-green-600 dark:text-green-400 text-xs px-2 py-1 rounded-full">
                {{ getWorkOrdersByStatus('In Progress').length }}
              </span>
            </h3>
            <div class="space-y-3">
              <div
                v-for="wo in getWorkOrdersByStatus('In Progress')"
                :key="wo.id"
                @click="viewWorkOrder(wo)"
                class="bg-white dark:bg-slate-800 p-4 rounded border-l-4 border-green-400 shadow-sm hover:shadow-md transition-shadow cursor-pointer"
              >
                <div class="font-semibold text-sm text-slate-900 dark:text-white">{{ wo.workOrderNumber }}</div>
                <div class="text-xs text-slate-600 dark:text-slate-400 mt-1">{{ wo.productName }}</div>
                <div class="text-xs text-slate-500 dark:text-slate-500 mt-2">
                  {{ wo.quantityProduced || 0 }} / {{ wo.quantityOrdered }}
                </div>
                <div class="w-full bg-slate-200 dark:bg-slate-700 rounded-full h-2 mt-2">
                  <div
                    class="bg-green-600 h-2 rounded-full transition-all"
                    :style="{ width: getProgressPercentage(wo) + '%' }"
                  ></div>
                </div>
                <div class="text-xs text-slate-500 dark:text-slate-500 mt-1">
                  {{ getProgressPercentage(wo) }}% Complete
                </div>
                <div class="mt-2 flex gap-1">
                  <button @click.stop="completeWorkOrder(wo)" class="text-xs px-2 py-1 bg-blue-600 text-white rounded hover:bg-blue-700">
                    Complete
                  </button>
                </div>
              </div>
            </div>
          </div>

          <!-- Completed Column -->
          <div class="bg-emerald-50 dark:bg-emerald-900/20 rounded-lg p-4">
            <h3 class="font-semibold text-slate-700 dark:text-slate-300 mb-3 flex items-center justify-between">
              Completed
              <span class="bg-emerald-200 dark:bg-emerald-700 text-emerald-600 dark:text-emerald-400 text-xs px-2 py-1 rounded-full">
                {{ getWorkOrdersByStatus('Completed').length }}
              </span>
            </h3>
            <div class="space-y-3">
              <div
                v-for="wo in getWorkOrdersByStatus('Completed')"
                :key="wo.id"
                @click="viewWorkOrder(wo)"
                class="bg-white dark:bg-slate-800 p-4 rounded border-l-4 border-emerald-400 shadow-sm hover:shadow-md transition-shadow cursor-pointer"
              >
                <div class="font-semibold text-sm text-slate-900 dark:text-white">{{ wo.workOrderNumber }}</div>
                <div class="text-xs text-slate-600 dark:text-slate-400 mt-1">{{ wo.productName }}</div>
                <div class="text-xs text-emerald-600 dark:text-emerald-400 mt-2">
                  ✓ {{ wo.quantityProduced || wo.quantityOrdered }} units produced
                </div>
                <div class="text-xs text-slate-500 dark:text-slate-500 mt-1">{{ formatDate(wo.actualEndDate) }}</div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- List View -->
      <div v-else class="overflow-x-auto">
        <table class="min-w-full divide-y divide-slate-200 dark:divide-slate-700">
          <thead class="bg-slate-50 dark:bg-slate-900">
            <tr>
              <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">Work Order</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">Product</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">Quantity</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">Status</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">Priority</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">Start Date</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">Progress</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">Actions</th>
            </tr>
          </thead>
          <tbody class="bg-white dark:bg-slate-800 divide-y divide-slate-200 dark:divide-slate-700">
            <tr v-for="wo in paginatedWorkOrders" :key="wo.id" class="hover:bg-slate-50 dark:hover:bg-slate-700 transition-colors">
              <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-blue-600 dark:text-blue-400">
                <button @click="viewWorkOrder(wo)" class="hover:underline">{{ wo.workOrderNumber }}</button>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-slate-900 dark:text-white">{{ wo.productName }}</td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-slate-600 dark:text-slate-400">
                {{ wo.quantityProduced || 0 }} / {{ wo.quantityOrdered }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span class="px-2 py-1 text-xs rounded-full" :class="getStatusClass(wo.status)">
                  {{ wo.status }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span class="px-2 py-1 text-xs rounded" :class="getPriorityClass(wo.priority)">
                  P{{ wo.priority }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-slate-600 dark:text-slate-400">
                {{ formatDate(wo.plannedStartDate) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="w-full bg-slate-200 dark:bg-slate-700 rounded-full h-2">
                  <div
                    class="bg-blue-600 h-2 rounded-full transition-all"
                    :style="{ width: getProgressPercentage(wo) + '%' }"
                  ></div>
                </div>
                <div class="text-xs text-slate-500 dark:text-slate-500 mt-1">{{ getProgressPercentage(wo) }}%</div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
                <div class="flex gap-2">
                  <button @click="editWorkOrder(wo)" class="text-blue-600 dark:text-blue-400 hover:text-blue-900 dark:hover:text-blue-300">Edit</button>
                  <button v-if="wo.status === 'Released'" @click="startWorkOrder(wo)" class="text-green-600 dark:text-green-400 hover:text-green-900 dark:hover:text-green-300">Start</button>
                  <button v-if="wo.status === 'In Progress'" @click="completeWorkOrder(wo)" class="text-purple-600 dark:text-purple-400 hover:text-purple-900 dark:hover:text-purple-300">Complete</button>
                  <button @click="deleteWorkOrder(wo)" class="text-red-600 dark:text-red-400 hover:text-red-900 dark:hover:text-red-300">Delete</button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>

        <!-- Pagination -->
        <div class="px-6 py-4 border-t border-slate-200 dark:border-slate-700 flex items-center justify-between">
          <div class="text-sm text-slate-600 dark:text-slate-400">
            Showing {{ ((currentPage - 1) * pageSize) + 1 }} to {{ Math.min(currentPage * pageSize, filteredWorkOrders.length) }} of {{ filteredWorkOrders.length }} work orders
          </div>
          <div class="flex gap-2">
            <button
              @click="currentPage--"
              :disabled="currentPage === 1"
              class="px-3 py-1 text-sm border border-slate-300 dark:border-slate-600 rounded disabled:opacity-50 disabled:cursor-not-allowed hover:bg-slate-50 dark:hover:bg-slate-700 transition-colors"
            >
              Previous
            </button>
            <button
              @click="currentPage++"
              :disabled="currentPage >= totalPages"
              class="px-3 py-1 text-sm border border-slate-300 dark:border-slate-600 rounded disabled:opacity-50 disabled:cursor-not-allowed hover:bg-slate-50 dark:hover:bg-slate-700 transition-colors"
            >
              Next
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Work Order Creation Modal -->
    <div v-if="showCreateModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div class="bg-white dark:bg-slate-800 rounded-lg shadow-xl w-full max-w-2xl max-h-[90vh] overflow-y-auto m-4">
        <div class="px-6 py-4 border-b border-slate-200 dark:border-slate-700">
          <h3 class="text-lg font-semibold text-slate-900 dark:text-white">Create Work Order</h3>
        </div>
        
        <div class="p-6 space-y-6">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Product Name *</label>
              <input
                v-model="newWorkOrder.productName"
                type="text"
                required
                class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white"
                placeholder="Enter product name"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Order Type *</label>
              <select
                v-model="newWorkOrder.type"
                required
                class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white"
              >
                <option value="">Select type</option>
                <option value="Production">Production</option>
                <option value="Assembly">Assembly</option>
                <option value="Rework">Rework</option>
                <option value="Maintenance">Maintenance</option>
              </select>
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Quantity Ordered *</label>
              <input
                v-model.number="newWorkOrder.quantityOrdered"
                type="number"
                min="1"
                required
                class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white"
                placeholder="0"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Priority</label>
              <select
                v-model.number="newWorkOrder.priority"
                class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white"
              >
                <option :value="1">Critical (1)</option>
                <option :value="2">High (2)</option>
                <option :value="3">Medium (3)</option>
                <option :value="4">Low (4)</option>
              </select>
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Planned Start Date</label>
              <input
                v-model="newWorkOrder.plannedStartDate"
                type="date"
                class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Planned End Date</label>
              <input
                v-model="newWorkOrder.plannedEndDate"
                type="date"
                class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white"
              />
            </div>
          </div>

          <div>
            <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Notes</label>
            <textarea
              v-model="newWorkOrder.notes"
              rows="3"
              class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white"
              placeholder="Enter any additional notes or instructions..."
            ></textarea>
          </div>
        </div>

        <div class="px-6 py-4 border-t border-slate-200 dark:border-slate-700 flex justify-end gap-3">
          <button @click="showCreateModal = false" class="px-4 py-2 text-slate-600 dark:text-slate-400 hover:text-slate-900 dark:hover:text-white transition-colors">
            Cancel
          </button>
          <button @click="saveWorkOrder" class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors">
            Create Work Order
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import ExportButton from '~/components/common/ExportButton.vue'

definePageMeta({
  layout: 'default',
  middleware: ['auth']
})

interface WorkOrder {
  id: number
  workOrderNumber: string
  productName: string
  type: string
  quantityOrdered: number
  quantityProduced?: number
  priority: number
  status: string
  plannedStartDate: Date
  plannedEndDate?: Date
  actualStartDate?: Date
  actualEndDate?: Date
  notes?: string
  createdDate: Date
}

// Reactive data
const searchQuery = ref('')
const selectedStatus = ref('')
const selectedPriority = ref('')
const currentPage = ref(1)
const pageSize = ref(10)
const viewMode = ref<'kanban' | 'list'>('kanban')
const showCreateModal = ref(false)

const newWorkOrder = ref({
  productName: '',
  type: '',
  quantityOrdered: 1,
  priority: 3,
  plannedStartDate: '',
  plannedEndDate: '',
  notes: ''
})

// Mock data
const workOrders = ref<WorkOrder[]>([
  {
    id: 1,
    workOrderNumber: 'WO-001',
    productName: 'Widget A',
    type: 'Production',
    quantityOrdered: 100,
    priority: 5,
    status: 'Draft',
    plannedStartDate: new Date('2024-10-15'),
    createdDate: new Date('2024-10-10')
  },
  {
    id: 2,
    workOrderNumber: 'WO-002',
    productName: 'Gadget B',
    type: 'Assembly',
    quantityOrdered: 50,
    priority: 3,
    status: 'Released',
    plannedStartDate: new Date('2024-10-12'),
    createdDate: new Date('2024-10-08')
  },
  {
    id: 3,
    workOrderNumber: 'WO-003',
    productName: 'Component C',
    type: 'Production',
    quantityOrdered: 200,
    quantityProduced: 120,
    priority: 2,
    status: 'In Progress',
    plannedStartDate: new Date('2024-10-10'),
    actualStartDate: new Date('2024-10-10'),
    createdDate: new Date('2024-10-05')
  },
  {
    id: 4,
    workOrderNumber: 'WO-004',
    productName: 'Assembly E',
    type: 'Assembly',
    quantityOrdered: 75,
    quantityProduced: 75,
    priority: 1,
    status: 'Completed',
    plannedStartDate: new Date('2024-10-08'),
    actualStartDate: new Date('2024-10-08'),
    actualEndDate: new Date('2024-10-09'),
    createdDate: new Date('2024-10-03')
  }
])

// Computed properties
const filteredWorkOrders = computed(() => {
  let filtered = workOrders.value

  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    filtered = filtered.filter(wo => 
      wo.workOrderNumber.toLowerCase().includes(query) ||
      wo.productName.toLowerCase().includes(query)
    )
  }

  if (selectedStatus.value) {
    filtered = filtered.filter(wo => wo.status === selectedStatus.value)
  }

  if (selectedPriority.value) {
    filtered = filtered.filter(wo => wo.priority === parseInt(selectedPriority.value))
  }

  return filtered
})

const paginatedWorkOrders = computed(() => {
  const start = (currentPage.value - 1) * pageSize.value
  const end = start + pageSize.value
  return filteredWorkOrders.value.slice(start, end)
})

const totalPages = computed(() => Math.ceil(filteredWorkOrders.value.length / pageSize.value))

// Methods
const formatDate = (date: Date | string) => {
  if (!date) return '-'
  return new Date(date).toLocaleDateString('en-ZA', { month: 'short', day: 'numeric' })
}

const getStatusClass = (status: string) => {
  switch (status) {
    case 'Draft': return 'bg-gray-100 text-gray-800 dark:bg-gray-900 dark:text-gray-200'
    case 'Released': return 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200'
    case 'In Progress': return 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200'
    case 'Completed': return 'bg-emerald-100 text-emerald-800 dark:bg-emerald-900 dark:text-emerald-200'
    case 'Cancelled': return 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200'
    default: return 'bg-gray-100 text-gray-800 dark:bg-gray-900 dark:text-gray-200'
  }
}

const getPriorityClass = (priority: number) => {
  if (priority === 1) return 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200'
  if (priority === 2) return 'bg-orange-100 text-orange-800 dark:bg-orange-900 dark:text-orange-200'
  if (priority === 3) return 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-200'
  return 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200'
}

const getProgressPercentage = (wo: WorkOrder) => {
  if (wo.status === 'Completed') return 100
  if (wo.status === 'Draft' || wo.status === 'Released') return 0
  if (!wo.quantityProduced || wo.quantityOrdered === 0) return 0
  return Math.round((wo.quantityProduced / wo.quantityOrdered) * 100)
}

const getStatusCount = (status: string) => {
  return workOrders.value.filter(wo => wo.status === status).length
}

const getOverdueCount = () => {
  const today = new Date()
  return workOrders.value.filter(wo => 
    wo.status !== 'Completed' && 
    wo.plannedEndDate && 
    new Date(wo.plannedEndDate) < today
  ).length
}

const getWorkOrdersByStatus = (status: string) => {
  return filteredWorkOrders.value.filter(wo => wo.status === status)
}

const clearFilters = () => {
  searchQuery.value = ''
  selectedStatus.value = ''
  selectedPriority.value = ''
  currentPage.value = 1
}

const saveWorkOrder = async () => {
  try {
    // Validate required fields
    if (!newWorkOrder.value.productName || !newWorkOrder.value.type || !newWorkOrder.value.quantityOrdered) {
      alert('Please fill in all required fields')
      return
    }

    // Generate work order number
    const woNumber = `WO-${String(workOrders.value.length + 1).padStart(3, '0')}`
    
    const newWO: WorkOrder = {
      id: workOrders.value.length + 1,
      workOrderNumber: woNumber,
      productName: newWorkOrder.value.productName,
      type: newWorkOrder.value.type,
      quantityOrdered: newWorkOrder.value.quantityOrdered,
      priority: newWorkOrder.value.priority,
      status: 'Draft',
      plannedStartDate: newWorkOrder.value.plannedStartDate ? new Date(newWorkOrder.value.plannedStartDate) : new Date(),
      plannedEndDate: newWorkOrder.value.plannedEndDate ? new Date(newWorkOrder.value.plannedEndDate) : undefined,
      notes: newWorkOrder.value.notes,
      createdDate: new Date()
    }

    workOrders.value.push(newWO)
    
    // Reset form
    newWorkOrder.value = {
      productName: '',
      type: '',
      quantityOrdered: 1,
      priority: 3,
      plannedStartDate: '',
      plannedEndDate: '',
      notes: ''
    }
    
    showCreateModal.value = false
    alert('Work order created successfully!')
  } catch (error) {
    console.error('Error creating work order:', error)
    alert('Failed to create work order. Please try again.')
  }
}

const viewWorkOrder = (wo: WorkOrder) => {
  // TODO: Implement work order detail view
  alert(`View work order details: ${wo.workOrderNumber}`)
}

const editWorkOrder = (wo: WorkOrder) => {
  // TODO: Implement work order editing
  alert(`Edit work order: ${wo.workOrderNumber}`)
}

const startWorkOrder = (wo: WorkOrder) => {
  wo.status = 'In Progress'
  wo.actualStartDate = new Date()
  alert(`Work order ${wo.workOrderNumber} started!`)
}

const completeWorkOrder = (wo: WorkOrder) => {
  wo.status = 'Completed'
  wo.actualEndDate = new Date()
  wo.quantityProduced = wo.quantityOrdered
  alert(`Work order ${wo.workOrderNumber} completed!`)
}

const deleteWorkOrder = (wo: WorkOrder) => {
  if (confirm(`Are you sure you want to delete work order ${wo.workOrderNumber}?`)) {
    const index = workOrders.value.findIndex(w => w.id === wo.id)
    if (index > -1) {
      workOrders.value.splice(index, 1)
      alert('Work order deleted successfully!')
    }
  }
}

const showNotification = (message: string, type: 'success' | 'error') => {
  // Simple notification - in a real app, you'd use a proper notification system
  if (type === 'success') {
    alert(`✅ ${message}`)
  } else {
    alert(`❌ ${message}`)
  }
}
</script>
