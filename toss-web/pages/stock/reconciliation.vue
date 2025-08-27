<template>
  <div class="space-y-6">
    <!-- Header -->
    <div class="border-b border-gray-200 dark:border-gray-700 pb-4">
      <div class="flex items-center justify-between">
        <div>
          <h1 class="text-2xl font-bold text-gray-900 dark:text-white">Stock Reconciliation</h1>
          <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
            Reconcile physical stock counts with system records
          </p>
        </div>
        <div class="flex space-x-3">
          <button class="inline-flex items-center px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg text-sm font-medium text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600">
            <DocumentArrowDownIcon class="w-4 h-4 mr-2" />
            Export
          </button>
          <button class="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-lg text-white bg-blue-600 hover:bg-blue-700">
            <PlusIcon class="w-4 h-4 mr-2" />
            New Reconciliation
          </button>
        </div>
      </div>
    </div>

    <!-- Quick Stats -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-6">
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <div class="flex items-center">
          <div class="w-10 h-10 bg-blue-100 dark:bg-blue-900 rounded-lg flex items-center justify-center">
            <ClipboardDocumentCheckIcon class="w-6 h-6 text-blue-600 dark:text-blue-400" />
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-500 dark:text-gray-400">Pending Reconciliations</p>
            <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ pendingCount }}</p>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <div class="flex items-center">
          <div class="w-10 h-10 bg-green-100 dark:bg-green-900 rounded-lg flex items-center justify-center">
            <CheckCircleIcon class="w-6 h-6 text-green-600 dark:text-green-400" />
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-500 dark:text-gray-400">Completed This Month</p>
            <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ completedCount }}</p>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <div class="flex items-center">
          <div class="w-10 h-10 bg-yellow-100 dark:bg-yellow-900 rounded-lg flex items-center justify-center">
            <ExclamationTriangleIcon class="w-6 h-6 text-yellow-600 dark:text-yellow-400" />
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-500 dark:text-gray-400">Discrepancies</p>
            <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ discrepancyCount }}</p>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <div class="flex items-center">
          <div class="w-10 h-10 bg-purple-100 dark:bg-purple-900 rounded-lg flex items-center justify-center">
            <CurrencyDollarIcon class="w-6 h-6 text-purple-600 dark:text-purple-400" />
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-500 dark:text-gray-400">Value Adjustment</p>
            <p class="text-2xl font-bold text-gray-900 dark:text-white">${{ valueAdjustment.toLocaleString() }}</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Filters and Search -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-4">
      <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
        <div>
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Warehouse</label>
          <select v-model="selectedWarehouse" class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
            <option value="">All Warehouses</option>
            <option v-for="warehouse in warehouses" :key="warehouse.id" :value="warehouse.id">
              {{ warehouse.name }}
            </option>
          </select>
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Status</label>
          <select v-model="selectedStatus" class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
            <option value="">All Status</option>
            <option value="draft">Draft</option>
            <option value="in-progress">In Progress</option>
            <option value="completed">Completed</option>
            <option value="cancelled">Cancelled</option>
          </select>
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Date Range</label>
          <input v-model="dateRange" type="date" class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Search</label>
          <div class="relative">
            <MagnifyingGlassIcon class="w-5 h-5 absolute left-3 top-2.5 text-gray-400" />
            <input v-model="searchQuery" type="text" placeholder="Search reconciliations..." class="w-full pl-10 pr-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white placeholder-gray-500 dark:placeholder-gray-400">
          </div>
        </div>
      </div>
    </div>

    <!-- Reconciliation List -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
      <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
        <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Reconciliation Records</h3>
      </div>
      <div class="overflow-x-auto">
        <table class="w-full">
          <thead class="bg-gray-50 dark:bg-gray-700">
            <tr>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                Reconciliation ID
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                Warehouse
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                Date
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                Items
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                Discrepancies
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                Value Impact
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                Status
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                Actions
              </th>
            </tr>
          </thead>
          <tbody class="divide-y divide-gray-200 dark:divide-gray-700">
            <tr v-for="reconciliation in filteredReconciliations" :key="reconciliation.id" class="hover:bg-gray-50 dark:hover:bg-gray-700">
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="flex items-center">
                  <div class="w-8 h-8 bg-blue-100 dark:bg-blue-900 rounded-lg flex items-center justify-center mr-3">
                    <ClipboardDocumentCheckIcon class="w-4 h-4 text-blue-600 dark:text-blue-400" />
                  </div>
                  <div>
                    <div class="text-sm font-medium text-gray-900 dark:text-white">{{ reconciliation.id }}</div>
                    <div class="text-sm text-gray-500 dark:text-gray-400">{{ reconciliation.reference }}</div>
                  </div>
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm text-gray-900 dark:text-white">{{ reconciliation.warehouse }}</div>
                <div class="text-sm text-gray-500 dark:text-gray-400">{{ reconciliation.location }}</div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm text-gray-900 dark:text-white">{{ reconciliation.date }}</div>
                <div class="text-sm text-gray-500 dark:text-gray-400">{{ reconciliation.time }}</div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">
                {{ reconciliation.itemsCount }} items
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="flex items-center">
                  <div v-if="reconciliation.discrepancies > 0" class="text-sm text-red-600 dark:text-red-400">
                    {{ reconciliation.discrepancies }} issues
                  </div>
                  <div v-else class="text-sm text-green-600 dark:text-green-400">
                    No issues
                  </div>
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm" :class="reconciliation.valueImpact >= 0 ? 'text-green-600 dark:text-green-400' : 'text-red-600 dark:text-red-400'">
                  {{ reconciliation.valueImpact >= 0 ? '+' : '' }}${{ reconciliation.valueImpact.toLocaleString() }}
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span class="inline-flex px-2 py-1 text-xs font-semibold rounded-full" :class="getStatusClass(reconciliation.status)">
                  {{ reconciliation.status }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                <div class="flex space-x-2">
                  <button class="text-blue-600 dark:text-blue-400 hover:text-blue-700" title="View Details">
                    <EyeIcon class="w-4 h-4" />
                  </button>
                  <button v-if="reconciliation.status === 'draft'" class="text-green-600 dark:text-green-400 hover:text-green-700" title="Start Reconciliation">
                    <PlayIcon class="w-4 h-4" />
                  </button>
                  <button class="text-gray-600 dark:text-gray-400 hover:text-gray-700" title="Edit">
                    <PencilIcon class="w-4 h-4" />
                  </button>
                  <button class="text-red-600 dark:text-red-400 hover:text-red-700" title="Delete">
                    <TrashIcon class="w-4 h-4" />
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
      
      <!-- Empty State -->
      <div v-if="filteredReconciliations.length === 0" class="text-center py-12">
        <ClipboardDocumentCheckIcon class="w-12 h-12 text-gray-400 mx-auto mb-4" />
        <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-2">No reconciliations found</h3>
        <p class="text-gray-500 dark:text-gray-400 mb-4">Get started by creating your first stock reconciliation.</p>
        <button class="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-lg text-white bg-blue-600 hover:bg-blue-700">
          <PlusIcon class="w-4 h-4 mr-2" />
          New Reconciliation
        </button>
      </div>
    </div>

    <!-- Reconciliation Process Help -->
    <div class="bg-blue-50 dark:bg-blue-900/20 border border-blue-200 dark:border-blue-800 rounded-lg p-6">
      <div class="flex items-start">
        <InformationCircleIcon class="w-6 h-6 text-blue-600 dark:text-blue-400 mt-0.5 mr-3 flex-shrink-0" />
        <div>
          <h3 class="text-lg font-semibold text-blue-900 dark:text-blue-100 mb-2">Stock Reconciliation Process</h3>
          <div class="text-blue-800 dark:text-blue-200 space-y-2">
            <p><strong>1. Create Reconciliation:</strong> Start a new reconciliation for a specific warehouse or location</p>
            <p><strong>2. Physical Count:</strong> Conduct physical stock counting and record actual quantities</p>
            <p><strong>3. Review Discrepancies:</strong> Compare physical counts with system records</p>
            <p><strong>4. Adjust Stock:</strong> Apply corrections to resolve discrepancies</p>
            <p><strong>5. Complete:</strong> Finalize the reconciliation and update stock levels</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import {
  PlusIcon,
  DocumentArrowDownIcon,
  ClipboardDocumentCheckIcon,
  CheckCircleIcon,
  ExclamationTriangleIcon,
  CurrencyDollarIcon,
  MagnifyingGlassIcon,
  EyeIcon,
  PlayIcon,
  PencilIcon,
  TrashIcon,
  InformationCircleIcon
} from '@heroicons/vue/24/outline'

useHead({
  title: 'Stock Reconciliation - TOSS ERP III'
})

// Reactive data
const selectedWarehouse = ref('')
const selectedStatus = ref('')
const dateRange = ref('')
const searchQuery = ref('')

// Sample data
const pendingCount = ref(5)
const completedCount = ref(28)
const discrepancyCount = ref(12)
const valueAdjustment = ref(2456)

const warehouses = ref([
  { id: 1, name: 'Main Warehouse' },
  { id: 2, name: 'Secondary Storage' },
  { id: 3, name: 'Retail Location A' },
  { id: 4, name: 'Retail Location B' }
])

const reconciliations = ref([
  {
    id: 'REC-2024-001',
    reference: 'Monthly Count - Jan',
    warehouse: 'Main Warehouse',
    location: 'Section A-C',
    date: '2024-01-15',
    time: '09:30 AM',
    itemsCount: 156,
    discrepancies: 3,
    valueImpact: -245,
    status: 'completed'
  },
  {
    id: 'REC-2024-002',
    reference: 'Cycle Count - Electronics',
    warehouse: 'Secondary Storage',
    location: 'Electronics Bay',
    date: '2024-01-18',
    time: '02:15 PM',
    itemsCount: 89,
    discrepancies: 0,
    valueImpact: 0,
    status: 'completed'
  },
  {
    id: 'REC-2024-003',
    reference: 'Annual Inventory',
    warehouse: 'Main Warehouse',
    location: 'All Sections',
    date: '2024-01-20',
    time: '08:00 AM',
    itemsCount: 1243,
    discrepancies: 8,
    valueImpact: 1890,
    status: 'in-progress'
  },
  {
    id: 'REC-2024-004',
    reference: 'Spot Check - High Value',
    warehouse: 'Retail Location A',
    location: 'Security Cage',
    date: '2024-01-22',
    time: '10:45 AM',
    itemsCount: 24,
    discrepancies: 1,
    valueImpact: -567,
    status: 'draft'
  },
  {
    id: 'REC-2024-005',
    reference: 'Weekly Count - Consumables',
    warehouse: 'Main Warehouse',
    location: 'Section D',
    date: '2024-01-25',
    time: '01:30 PM',
    itemsCount: 78,
    discrepancies: 0,
    valueImpact: 0,
    status: 'completed'
  }
])

// Computed properties
const filteredReconciliations = computed(() => {
  let filtered = reconciliations.value

  if (selectedWarehouse.value) {
    filtered = filtered.filter(rec => rec.warehouse.includes(selectedWarehouse.value))
  }

  if (selectedStatus.value) {
    filtered = filtered.filter(rec => rec.status === selectedStatus.value)
  }

  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    filtered = filtered.filter(rec => 
      rec.id.toLowerCase().includes(query) ||
      rec.reference.toLowerCase().includes(query) ||
      rec.warehouse.toLowerCase().includes(query) ||
      rec.location.toLowerCase().includes(query)
    )
  }

  return filtered
})

// Methods
const getStatusClass = (status: string) => {
  const classes = {
    'draft': 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-200',
    'in-progress': 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200',
    'completed': 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200',
    'cancelled': 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200'
  }
  return classes[status as keyof typeof classes] || classes.draft
}
</script>
