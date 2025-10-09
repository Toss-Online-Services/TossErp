<template>
  <div class="p-6 space-y-6">
    <!-- Page Header -->
    <div class="flex justify-between items-center">
      <div>
        <h1 class="text-3xl font-bold text-slate-900 dark:text-white">Quality Control</h1>
        <p class="text-slate-600 dark:text-slate-400 mt-1">Manage quality inspections, defect tracking, and quality metrics</p>
      </div>
      <div class="flex gap-3">
        <button @click="showInspectionModal = true" class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors">
          <span class="flex items-center gap-2">
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6"/>
            </svg>
            New Inspection
          </span>
        </button>
        <ExportButton
          :data="filteredInspections"
          filename="quality_inspections"
          title="Quality Inspections Report"
          data-type="quality"
          @export-start="() => {}"
          @export-complete="(format) => showNotification(`Quality data exported as ${format.toUpperCase()}`, 'success')"
          @export-error="(error) => showNotification(error, 'error')"
        />
      </div>
    </div>

    <!-- Quality Metrics Cards -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
      <div class="bg-white dark:bg-slate-800 rounded-lg shadow p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-slate-600 dark:text-slate-400">Quality Rate</p>
            <p class="text-3xl font-bold text-slate-900 dark:text-white mt-2">{{ qualityMetrics.qualityRate.toFixed(1) }}%</p>
            <p class="text-sm text-green-600 dark:text-green-400 mt-2">Target: 95%</p>
          </div>
          <div class="p-3 bg-green-100 dark:bg-green-900 rounded-full">
            <svg class="w-8 h-8 text-green-600 dark:text-green-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"/>
            </svg>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-slate-800 rounded-lg shadow p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-slate-600 dark:text-slate-400">Inspections Today</p>
            <p class="text-3xl font-bold text-slate-900 dark:text-white mt-2">{{ qualityMetrics.inspectionsToday }}</p>
            <p class="text-sm text-blue-600 dark:text-blue-400 mt-2">{{ qualityMetrics.passedToday }} passed</p>
          </div>
          <div class="p-3 bg-blue-100 dark:bg-blue-900 rounded-full">
            <svg class="w-8 h-8 text-blue-600 dark:text-blue-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2"/>
            </svg>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-slate-800 rounded-lg shadow p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-slate-600 dark:text-slate-400">Defects Found</p>
            <p class="text-3xl font-bold text-slate-900 dark:text-white mt-2">{{ qualityMetrics.defectsToday }}</p>
            <p class="text-sm text-red-600 dark:text-red-400 mt-2">{{ qualityMetrics.criticalDefects }} critical</p>
          </div>
          <div class="p-3 bg-red-100 dark:bg-red-900 rounded-full">
            <svg class="w-8 h-8 text-red-600 dark:text-red-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L3.732 16.5c-.77.833.192 2.5 1.732 2.5z"/>
            </svg>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-slate-800 rounded-lg shadow p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-slate-600 dark:text-slate-400">Cost of Quality</p>
            <p class="text-3xl font-bold text-slate-900 dark:text-white mt-2">R{{ formatCurrency(qualityMetrics.costOfQuality) }}</p>
            <p class="text-sm text-orange-600 dark:text-orange-400 mt-2">This month</p>
          </div>
          <div class="p-3 bg-orange-100 dark:bg-orange-900 rounded-full">
            <svg class="w-8 h-8 text-orange-600 dark:text-orange-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1"/>
            </svg>
          </div>
        </div>
      </div>
    </div>

    <!-- Quality Charts -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <div class="bg-white dark:bg-slate-800 rounded-lg shadow p-6">
        <h3 class="text-lg font-semibold text-slate-900 dark:text-white mb-4">Quality Trend (Last 30 Days)</h3>
        <LineChart
          :labels="qualityTrendData.labels"
          :datasets="qualityTrendData.datasets"
          height="300px"
          y-axis-label="Quality Rate (%)"
        />
      </div>

      <div class="bg-white dark:bg-slate-800 rounded-lg shadow p-6">
        <h3 class="text-lg font-semibold text-slate-900 dark:text-white mb-4">Defect Distribution</h3>
        <PieChart
          :labels="defectDistributionData.labels"
          :data="defectDistributionData.datasets[0].data"
          :colors="defectDistributionData.datasets[0].backgroundColor"
          height="300px"
        />
      </div>
    </div>

    <!-- Filters -->
    <div class="bg-white dark:bg-slate-800 rounded-lg shadow p-6">
      <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
        <div>
          <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Search</label>
          <input
            v-model="searchQuery"
            type="text"
            placeholder="Search inspections..."
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
            <option value="Passed">Passed</option>
            <option value="Failed">Failed</option>
            <option value="Pending">Pending</option>
          </select>
        </div>
        <div>
          <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Severity</label>
          <select
            v-model="selectedSeverity"
            class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white"
          >
            <option value="">All Severities</option>
            <option value="Critical">Critical</option>
            <option value="Major">Major</option>
            <option value="Minor">Minor</option>
          </select>
        </div>
        <div class="flex items-end">
          <button @click="clearFilters" class="px-4 py-2 text-slate-600 dark:text-slate-400 hover:text-slate-900 dark:hover:text-white transition-colors">
            Clear Filters
          </button>
        </div>
      </div>
    </div>

    <!-- Inspections List -->
    <div class="bg-white dark:bg-slate-800 rounded-lg shadow overflow-hidden">
      <div class="px-6 py-4 border-b border-slate-200 dark:border-slate-700">
        <h2 class="text-xl font-semibold text-slate-900 dark:text-white">Quality Inspections ({{ filteredInspections.length }})</h2>
      </div>
      
      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-slate-200 dark:divide-slate-700">
          <thead class="bg-slate-50 dark:bg-slate-900">
            <tr>
              <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">Inspection #</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">Product</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">Work Order</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">Inspector</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">Date</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">Result</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">Defects</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">Actions</th>
            </tr>
          </thead>
          <tbody class="bg-white dark:bg-slate-800 divide-y divide-slate-200 dark:divide-slate-700">
            <tr v-for="inspection in paginatedInspections" :key="inspection.id" class="hover:bg-slate-50 dark:hover:bg-slate-700 transition-colors">
              <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-blue-600 dark:text-blue-400">
                <button @click="viewInspection(inspection)" class="hover:underline">{{ inspection.inspectionNumber }}</button>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-slate-900 dark:text-white">{{ inspection.productName }}</td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-slate-600 dark:text-slate-400">{{ inspection.workOrderNumber }}</td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-slate-600 dark:text-slate-400">{{ inspection.inspector }}</td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-slate-600 dark:text-slate-400">{{ formatDate(inspection.inspectionDate) }}</td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span class="px-2 py-1 text-xs rounded-full" :class="getStatusClass(inspection.result)">
                  {{ inspection.result }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-slate-600 dark:text-slate-400">
                <span v-if="inspection.defects.length > 0" class="text-red-600 dark:text-red-400">{{ inspection.defects.length }} defect(s)</span>
                <span v-else class="text-green-600 dark:text-green-400">No defects</span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
                <div class="flex gap-2">
                  <button @click="viewInspection(inspection)" class="text-blue-600 dark:text-blue-400 hover:text-blue-900 dark:hover:text-blue-300">View</button>
                  <button @click="editInspection(inspection)" class="text-green-600 dark:text-green-400 hover:text-green-900 dark:hover:text-green-300">Edit</button>
                  <button @click="deleteInspection(inspection)" class="text-red-600 dark:text-red-400 hover:text-red-900 dark:hover:text-red-300">Delete</button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Pagination -->
      <div class="px-6 py-4 border-t border-slate-200 dark:border-slate-700 flex items-center justify-between">
        <div class="text-sm text-slate-600 dark:text-slate-400">
          Showing {{ ((currentPage - 1) * pageSize) + 1 }} to {{ Math.min(currentPage * pageSize, filteredInspections.length) }} of {{ filteredInspections.length }} inspections
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

    <!-- Inspection Modal -->
    <div v-if="showInspectionModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div class="bg-white dark:bg-slate-800 rounded-lg shadow-xl w-full max-w-4xl max-h-[90vh] overflow-y-auto m-4">
        <div class="px-6 py-4 border-b border-slate-200 dark:border-slate-700">
          <h3 class="text-lg font-semibold text-slate-900 dark:text-white">New Quality Inspection</h3>
        </div>
        
        <div class="p-6 space-y-6">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Work Order *</label>
              <select
                v-model="newInspection.workOrderNumber"
                required
                class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white"
              >
                <option value="">Select work order</option>
                <option value="WO-001">WO-001 - Widget A</option>
                <option value="WO-002">WO-002 - Gadget B</option>
                <option value="WO-003">WO-003 - Component C</option>
              </select>
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Product Name *</label>
              <input
                v-model="newInspection.productName"
                type="text"
                required
                class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white"
                placeholder="Enter product name"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Inspector *</label>
              <input
                v-model="newInspection.inspector"
                type="text"
                required
                class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white"
                placeholder="Inspector name"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Inspection Date</label>
              <input
                v-model="newInspection.inspectionDate"
                type="date"
                class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white"
              />
            </div>
          </div>

          <!-- Inspection Checklist -->
          <div>
            <h4 class="text-lg font-medium text-slate-900 dark:text-white mb-4">Inspection Checklist</h4>
            <div class="space-y-3">
              <div v-for="(item, index) in newInspection.checklist" :key="index" class="grid grid-cols-12 gap-3 items-center">
                <div class="col-span-6">
                  <input
                    v-model="item.description"
                    type="text"
                    placeholder="Inspection point description"
                    class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white"
                  />
                </div>
                <div class="col-span-2">
                  <select
                    v-model="item.result"
                    class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white"
                  >
                    <option value="Pass">Pass</option>
                    <option value="Fail">Fail</option>
                    <option value="N/A">N/A</option>
                  </select>
                </div>
                <div class="col-span-3">
                  <input
                    v-model="item.notes"
                    type="text"
                    placeholder="Notes"
                    class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white"
                  />
                </div>
                <div class="col-span-1">
                  <button @click="removeChecklistItem(index)" class="p-2 text-red-600 dark:text-red-400 hover:text-red-900 dark:hover:text-red-300">
                    <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"/>
                    </svg>
                  </button>
                </div>
              </div>
            </div>
            <button @click="addChecklistItem" class="mt-3 px-3 py-1 text-sm bg-blue-600 text-white rounded hover:bg-blue-700 transition-colors">
              Add Inspection Point
            </button>
          </div>

          <!-- Overall Result -->
          <div>
            <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Overall Result</label>
            <div class="flex gap-4">
              <label class="flex items-center">
                <input v-model="newInspection.result" type="radio" value="Passed" class="mr-2" />
                <span class="text-green-600 dark:text-green-400">Passed</span>
              </label>
              <label class="flex items-center">
                <input v-model="newInspection.result" type="radio" value="Failed" class="mr-2" />
                <span class="text-red-600 dark:text-red-400">Failed</span>
              </label>
              <label class="flex items-center">
                <input v-model="newInspection.result" type="radio" value="Pending" class="mr-2" />
                <span class="text-yellow-600 dark:text-yellow-400">Pending</span>
              </label>
            </div>
          </div>

          <div>
            <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Notes</label>
            <textarea
              v-model="newInspection.notes"
              rows="3"
              class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white"
              placeholder="Additional inspection notes..."
            ></textarea>
          </div>
        </div>

        <div class="px-6 py-4 border-t border-slate-200 dark:border-slate-700 flex justify-end gap-3">
          <button @click="showInspectionModal = false" class="px-4 py-2 text-slate-600 dark:text-slate-400 hover:text-slate-900 dark:hover:text-white transition-colors">
            Cancel
          </button>
          <button @click="saveInspection" class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors">
            Save Inspection
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import ExportButton from '~/components/common/ExportButton.vue'
import LineChart from '~/components/charts/LineChart.vue'
import PieChart from '~/components/charts/PieChart.vue'

definePageMeta({
  layout: 'default',
  middleware: ['auth']
})

interface ChecklistItem {
  description: string
  result: 'Pass' | 'Fail' | 'N/A'
  notes: string
}

interface Defect {
  id: number
  type: string
  severity: 'Critical' | 'Major' | 'Minor'
  description: string
  location: string
}

interface QualityInspection {
  id: number
  inspectionNumber: string
  workOrderNumber: string
  productName: string
  inspector: string
  inspectionDate: Date
  result: 'Passed' | 'Failed' | 'Pending'
  checklist: ChecklistItem[]
  defects: Defect[]
  notes: string
  createdDate: Date
}

// Reactive data
const searchQuery = ref('')
const selectedStatus = ref('')
const selectedSeverity = ref('')
const currentPage = ref(1)
const pageSize = ref(10)
const showInspectionModal = ref(false)

const newInspection = ref({
  workOrderNumber: '',
  productName: '',
  inspector: '',
  inspectionDate: new Date().toISOString().split('T')[0],
  result: 'Pending' as 'Passed' | 'Failed' | 'Pending',
  checklist: [] as ChecklistItem[],
  notes: ''
})

// Quality metrics
const qualityMetrics = ref({
  qualityRate: 96.5,
  inspectionsToday: 24,
  passedToday: 23,
  defectsToday: 8,
  criticalDefects: 2,
  costOfQuality: 1250000 // R12,500.00
})

// Chart data
const { getQualityMetricsData, getHRAttendanceData } = useCharts()
const defectDistributionData = getQualityMetricsData()
const qualityTrendData = getHRAttendanceData() // Reusing attendance chart structure for quality trend

// Mock data
const inspections = ref<QualityInspection[]>([
  {
    id: 1,
    inspectionNumber: 'QI-001',
    workOrderNumber: 'WO-001',
    productName: 'Widget A',
    inspector: 'John Smith',
    inspectionDate: new Date('2024-10-09'),
    result: 'Passed',
    checklist: [
      { description: 'Dimensional accuracy', result: 'Pass', notes: 'Within tolerance' },
      { description: 'Surface finish', result: 'Pass', notes: 'Good quality' }
    ],
    defects: [],
    notes: 'All checks passed successfully',
    createdDate: new Date('2024-10-09')
  },
  {
    id: 2,
    inspectionNumber: 'QI-002',
    workOrderNumber: 'WO-002',
    productName: 'Gadget B',
    inspector: 'Sarah Johnson',
    inspectionDate: new Date('2024-10-09'),
    result: 'Failed',
    checklist: [
      { description: 'Dimensional accuracy', result: 'Pass', notes: 'OK' },
      { description: 'Surface finish', result: 'Fail', notes: 'Scratches found' }
    ],
    defects: [
      { id: 1, type: 'Surface Defect', severity: 'Minor', description: 'Scratches on surface', location: 'Top panel' }
    ],
    notes: 'Requires rework due to surface defects',
    createdDate: new Date('2024-10-09')
  }
])

// Computed properties
const filteredInspections = computed(() => {
  let filtered = inspections.value

  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    filtered = filtered.filter(inspection => 
      inspection.inspectionNumber.toLowerCase().includes(query) ||
      inspection.productName.toLowerCase().includes(query) ||
      inspection.workOrderNumber.toLowerCase().includes(query)
    )
  }

  if (selectedStatus.value) {
    filtered = filtered.filter(inspection => inspection.result === selectedStatus.value)
  }

  if (selectedSeverity.value) {
    filtered = filtered.filter(inspection => 
      inspection.defects.some(defect => defect.severity === selectedSeverity.value)
    )
  }

  return filtered
})

const paginatedInspections = computed(() => {
  const start = (currentPage.value - 1) * pageSize.value
  const end = start + pageSize.value
  return filteredInspections.value.slice(start, end)
})

const totalPages = computed(() => Math.ceil(filteredInspections.value.length / pageSize.value))

// Methods
const formatCurrency = (amount: number) => {
  return (amount / 100).toFixed(2)
}

const formatDate = (date: Date | string) => {
  return new Date(date).toLocaleDateString('en-ZA', { 
    year: 'numeric', 
    month: 'short', 
    day: 'numeric' 
  })
}

const getStatusClass = (status: string) => {
  switch (status) {
    case 'Passed': return 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200'
    case 'Failed': return 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200'
    case 'Pending': return 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-200'
    default: return 'bg-gray-100 text-gray-800 dark:bg-gray-900 dark:text-gray-200'
  }
}

const clearFilters = () => {
  searchQuery.value = ''
  selectedStatus.value = ''
  selectedSeverity.value = ''
  currentPage.value = 1
}

const addChecklistItem = () => {
  newInspection.value.checklist.push({
    description: '',
    result: 'Pass',
    notes: ''
  })
}

const removeChecklistItem = (index: number) => {
  newInspection.value.checklist.splice(index, 1)
}

const saveInspection = async () => {
  try {
    // Validate required fields
    if (!newInspection.value.workOrderNumber || !newInspection.value.productName || !newInspection.value.inspector) {
      alert('Please fill in all required fields')
      return
    }

    // Generate inspection number
    const inspectionNumber = `QI-${String(inspections.value.length + 1).padStart(3, '0')}`
    
    const newInspectionData: QualityInspection = {
      id: inspections.value.length + 1,
      inspectionNumber,
      workOrderNumber: newInspection.value.workOrderNumber,
      productName: newInspection.value.productName,
      inspector: newInspection.value.inspector,
      inspectionDate: new Date(newInspection.value.inspectionDate),
      result: newInspection.value.result,
      checklist: [...newInspection.value.checklist],
      defects: [], // Defects would be added based on failed checklist items
      notes: newInspection.value.notes,
      createdDate: new Date()
    }

    inspections.value.push(newInspectionData)
    
    // Reset form
    newInspection.value = {
      workOrderNumber: '',
      productName: '',
      inspector: '',
      inspectionDate: new Date().toISOString().split('T')[0],
      result: 'Pending',
      checklist: [],
      notes: ''
    }
    
    showInspectionModal.value = false
    showNotification('Quality inspection created successfully!', 'success')
  } catch (error) {
    console.error('Error creating inspection:', error)
    showNotification('Failed to create inspection. Please try again.', 'error')
  }
}

const viewInspection = (inspection: QualityInspection) => {
  // TODO: Implement inspection detail view
  alert(`View inspection details: ${inspection.inspectionNumber}`)
}

const editInspection = (inspection: QualityInspection) => {
  // TODO: Implement inspection editing
  alert(`Edit inspection: ${inspection.inspectionNumber}`)
}

const deleteInspection = (inspection: QualityInspection) => {
  if (confirm(`Are you sure you want to delete inspection ${inspection.inspectionNumber}?`)) {
    const index = inspections.value.findIndex(i => i.id === inspection.id)
    if (index > -1) {
      inspections.value.splice(index, 1)
      showNotification('Inspection deleted successfully!', 'success')
    }
  }
}

const showNotification = (message: string, type: 'success' | 'error') => {
  if (type === 'success') {
    alert(`✅ ${message}`)
  } else {
    alert(`❌ ${message}`)
  }
}

// Initialize with some checklist items for demo
onMounted(() => {
  addChecklistItem()
  addChecklistItem()
})
</script>
