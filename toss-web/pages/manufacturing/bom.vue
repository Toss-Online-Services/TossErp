<template>
  <div class="p-6 space-y-6">
    <!-- Page Header -->
    <div class="flex justify-between items-center">
      <div>
        <h1 class="text-3xl font-bold text-slate-900 dark:text-white">Bill of Materials</h1>
        <p class="text-slate-600 dark:text-slate-400 mt-1">Manage product structures and component requirements</p>
      </div>
      <div class="flex gap-3">
        <button @click="showCreateModal = true" class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors">
          <span class="flex items-center gap-2">
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6"/>
            </svg>
            Create BOM
          </span>
        </button>
        <ExportButton
          :data="filteredBOMs"
          filename="bills_of_materials"
          title="Bills of Materials Report"
          data-type="boms"
          @export-start="() => {}"
          @export-complete="(format) => showNotification(`BOMs exported as ${format.toUpperCase()}`, 'success')"
          @export-error="(error) => showNotification(error, 'error')"
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
            placeholder="Search BOMs..."
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
            <option value="Active">Active</option>
            <option value="Draft">Draft</option>
            <option value="Archived">Archived</option>
          </select>
        </div>
        <div>
          <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Product Type</label>
          <select
            v-model="selectedType"
            class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white"
          >
            <option value="">All Types</option>
            <option value="Finished Good">Finished Good</option>
            <option value="Sub-Assembly">Sub-Assembly</option>
            <option value="Component">Component</option>
          </select>
        </div>
        <div class="flex items-end">
          <button @click="clearFilters" class="px-4 py-2 text-slate-600 dark:text-slate-400 hover:text-slate-900 dark:hover:text-white transition-colors">
            Clear Filters
          </button>
        </div>
      </div>
    </div>

    <!-- BOM List -->
    <div class="bg-white dark:bg-slate-800 rounded-lg shadow overflow-hidden">
      <div class="px-6 py-4 border-b border-slate-200 dark:border-slate-700">
        <h2 class="text-xl font-semibold text-slate-900 dark:text-white">Bills of Materials ({{ filteredBOMs.length }})</h2>
      </div>
      
      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-slate-200 dark:divide-slate-700">
          <thead class="bg-slate-50 dark:bg-slate-900">
            <tr>
              <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">BOM #</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">Product</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">Version</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">Type</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">Components</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">Operations</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">Total Cost</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">Status</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">Actions</th>
            </tr>
          </thead>
          <tbody class="bg-white dark:bg-slate-800 divide-y divide-slate-200 dark:divide-slate-700">
            <tr v-for="bom in paginatedBOMs" :key="bom.id" class="hover:bg-slate-50 dark:hover:bg-slate-700 transition-colors">
              <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-blue-600 dark:text-blue-400">
                <button @click="viewBOM(bom)" class="hover:underline">{{ bom.bomNumber }}</button>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-slate-900 dark:text-white">{{ bom.productName }}</td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-slate-600 dark:text-slate-400">v{{ bom.version }}</td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-slate-600 dark:text-slate-400">{{ bom.productType }}</td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-slate-600 dark:text-slate-400">{{ bom.componentCount }}</td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-slate-600 dark:text-slate-400">{{ bom.operationCount }}</td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-slate-900 dark:text-white">R{{ formatCurrency(bom.totalCost) }}</td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span class="px-2 py-1 text-xs rounded-full" :class="getStatusClass(bom.status)">
                  {{ bom.status }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
                <div class="flex gap-2">
                  <button @click="editBOM(bom)" class="text-blue-600 dark:text-blue-400 hover:text-blue-900 dark:hover:text-blue-300">Edit</button>
                  <button @click="duplicateBOM(bom)" class="text-green-600 dark:text-green-400 hover:text-green-900 dark:hover:text-green-300">Copy</button>
                  <button @click="deleteBOM(bom)" class="text-red-600 dark:text-red-400 hover:text-red-900 dark:hover:text-red-300">Delete</button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Pagination -->
      <div class="px-6 py-4 border-t border-slate-200 dark:border-slate-700 flex items-center justify-between">
        <div class="text-sm text-slate-600 dark:text-slate-400">
          Showing {{ ((currentPage - 1) * pageSize) + 1 }} to {{ Math.min(currentPage * pageSize, filteredBOMs.length) }} of {{ filteredBOMs.length }} BOMs
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

    <!-- BOM Creation Modal -->
    <div v-if="showCreateModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div class="bg-white dark:bg-slate-800 rounded-lg shadow-xl w-full max-w-4xl max-h-[90vh] overflow-y-auto m-4">
        <div class="px-6 py-4 border-b border-slate-200 dark:border-slate-700">
          <h3 class="text-lg font-semibold text-slate-900 dark:text-white">Create Bill of Materials</h3>
        </div>
        
        <div class="p-6 space-y-6">
          <!-- Basic Information -->
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Product Name *</label>
              <input
                v-model="newBOM.productName"
                type="text"
                required
                class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white"
                placeholder="Enter product name"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Product Type *</label>
              <select
                v-model="newBOM.productType"
                required
                class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white"
              >
                <option value="">Select type</option>
                <option value="Finished Good">Finished Good</option>
                <option value="Sub-Assembly">Sub-Assembly</option>
                <option value="Component">Component</option>
              </select>
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Version</label>
              <input
                v-model.number="newBOM.version"
                type="number"
                min="1"
                class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white"
                placeholder="1"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Unit of Measure</label>
              <select
                v-model="newBOM.unitOfMeasure"
                class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white"
              >
                <option value="Each">Each</option>
                <option value="Kg">Kilogram</option>
                <option value="Liter">Liter</option>
                <option value="Meter">Meter</option>
              </select>
            </div>
          </div>

          <!-- Components Section -->
          <div>
            <div class="flex justify-between items-center mb-4">
              <h4 class="text-lg font-medium text-slate-900 dark:text-white">Components</h4>
              <button @click="addComponent" class="px-3 py-1 text-sm bg-blue-600 text-white rounded hover:bg-blue-700 transition-colors">
                Add Component
              </button>
            </div>
            
            <div class="space-y-3">
              <div v-for="(component, index) in newBOM.components" :key="index" class="grid grid-cols-12 gap-3 items-end">
                <div class="col-span-4">
                  <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">Component</label>
                  <input
                    v-model="component.name"
                    type="text"
                    placeholder="Component name"
                    class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white"
                  />
                </div>
                <div class="col-span-2">
                  <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">Quantity</label>
                  <input
                    v-model.number="component.quantity"
                    type="number"
                    min="0"
                    step="0.01"
                    placeholder="0"
                    class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white"
                  />
                </div>
                <div class="col-span-2">
                  <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">Unit</label>
                  <select
                    v-model="component.unit"
                    class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white"
                  >
                    <option value="Each">Each</option>
                    <option value="Kg">Kg</option>
                    <option value="Liter">Liter</option>
                    <option value="Meter">Meter</option>
                  </select>
                </div>
                <div class="col-span-3">
                  <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">Unit Cost (R)</label>
                  <input
                    v-model.number="component.unitCost"
                    type="number"
                    min="0"
                    step="0.01"
                    placeholder="0.00"
                    class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white"
                  />
                </div>
                <div class="col-span-1">
                  <button @click="removeComponent(index)" class="p-2 text-red-600 dark:text-red-400 hover:text-red-900 dark:hover:text-red-300">
                    <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"/>
                    </svg>
                  </button>
                </div>
              </div>
            </div>
          </div>

          <!-- Operations Section -->
          <div>
            <div class="flex justify-between items-center mb-4">
              <h4 class="text-lg font-medium text-slate-900 dark:text-white">Operations</h4>
              <button @click="addOperation" class="px-3 py-1 text-sm bg-green-600 text-white rounded hover:bg-green-700 transition-colors">
                Add Operation
              </button>
            </div>
            
            <div class="space-y-3">
              <div v-for="(operation, index) in newBOM.operations" :key="index" class="grid grid-cols-12 gap-3 items-end">
                <div class="col-span-1">
                  <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">Seq</label>
                  <input
                    v-model.number="operation.sequence"
                    type="number"
                    min="1"
                    class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white"
                  />
                </div>
                <div class="col-span-4">
                  <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">Operation</label>
                  <input
                    v-model="operation.name"
                    type="text"
                    placeholder="Operation name"
                    class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white"
                  />
                </div>
                <div class="col-span-3">
                  <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">Work Center</label>
                  <select
                    v-model="operation.workCenter"
                    class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white"
                  >
                    <option value="">Select work center</option>
                    <option value="Assembly Line 1">Assembly Line 1</option>
                    <option value="Assembly Line 2">Assembly Line 2</option>
                    <option value="Packaging">Packaging</option>
                    <option value="Quality Control">Quality Control</option>
                  </select>
                </div>
                <div class="col-span-2">
                  <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">Setup Time (min)</label>
                  <input
                    v-model.number="operation.setupTime"
                    type="number"
                    min="0"
                    placeholder="0"
                    class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white"
                  />
                </div>
                <div class="col-span-2">
                  <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">Run Time (min)</label>
                  <input
                    v-model.number="operation.runTime"
                    type="number"
                    min="0"
                    placeholder="0"
                    class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white"
                  />
                </div>
              </div>
            </div>
          </div>

          <!-- Cost Summary -->
          <div class="bg-slate-50 dark:bg-slate-900 rounded-lg p-4">
            <h4 class="text-lg font-medium text-slate-900 dark:text-white mb-3">Cost Summary</h4>
            <div class="grid grid-cols-2 gap-4 text-sm">
              <div class="flex justify-between">
                <span class="text-slate-600 dark:text-slate-400">Material Cost:</span>
                <span class="font-medium text-slate-900 dark:text-white">R{{ calculateMaterialCost().toFixed(2) }}</span>
              </div>
              <div class="flex justify-between">
                <span class="text-slate-600 dark:text-slate-400">Labor Cost:</span>
                <span class="font-medium text-slate-900 dark:text-white">R{{ calculateLaborCost().toFixed(2) }}</span>
              </div>
              <div class="flex justify-between font-semibold text-lg border-t border-slate-200 dark:border-slate-700 pt-2">
                <span class="text-slate-900 dark:text-white">Total Cost:</span>
                <span class="text-slate-900 dark:text-white">R{{ (calculateMaterialCost() + calculateLaborCost()).toFixed(2) }}</span>
              </div>
            </div>
          </div>
        </div>

        <div class="px-6 py-4 border-t border-slate-200 dark:border-slate-700 flex justify-end gap-3">
          <button @click="showCreateModal = false" class="px-4 py-2 text-slate-600 dark:text-slate-400 hover:text-slate-900 dark:hover:text-white transition-colors">
            Cancel
          </button>
          <button @click="saveBOM" class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors">
            Create BOM
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import ExportButton from '~/components/common/ExportButton.vue'

definePageMeta({
  layout: 'default',
  middleware: ['auth']
})

interface BOMComponent {
  name: string
  quantity: number
  unit: string
  unitCost: number
}

interface BOMOperation {
  sequence: number
  name: string
  workCenter: string
  setupTime: number
  runTime: number
}

interface BOM {
  id: number
  bomNumber: string
  productName: string
  version: number
  productType: string
  componentCount: number
  operationCount: number
  totalCost: number
  status: string
  unitOfMeasure: string
  components: BOMComponent[]
  operations: BOMOperation[]
  createdDate: Date
  lastModified: Date
}

// Reactive data
const searchQuery = ref('')
const selectedStatus = ref('')
const selectedType = ref('')
const currentPage = ref(1)
const pageSize = ref(10)
const showCreateModal = ref(false)

const newBOM = ref({
  productName: '',
  productType: '',
  version: 1,
  unitOfMeasure: 'Each',
  components: [] as BOMComponent[],
  operations: [] as BOMOperation[]
})

// Mock data
const boms = ref<BOM[]>([
  {
    id: 1,
    bomNumber: 'BOM-001',
    productName: 'Widget A',
    version: 2,
    productType: 'Finished Good',
    componentCount: 8,
    operationCount: 5,
    totalCost: 15000,
    status: 'Active',
    unitOfMeasure: 'Each',
    components: [
      { name: 'Steel Rod', quantity: 2, unit: 'Each', unitCost: 25.00 },
      { name: 'Plastic Housing', quantity: 1, unit: 'Each', unitCost: 15.50 }
    ],
    operations: [
      { sequence: 10, name: 'Cut Steel', workCenter: 'Assembly Line 1', setupTime: 15, runTime: 5 },
      { sequence: 20, name: 'Assemble', workCenter: 'Assembly Line 2', setupTime: 10, runTime: 8 }
    ],
    createdDate: new Date('2024-01-15'),
    lastModified: new Date('2024-01-20')
  },
  {
    id: 2,
    bomNumber: 'BOM-002',
    productName: 'Gadget B',
    version: 1,
    productType: 'Sub-Assembly',
    componentCount: 12,
    operationCount: 7,
    totalCost: 22500,
    status: 'Active',
    unitOfMeasure: 'Each',
    components: [],
    operations: [],
    createdDate: new Date('2024-02-01'),
    lastModified: new Date('2024-02-05')
  },
  {
    id: 3,
    bomNumber: 'BOM-003',
    productName: 'Assembly E',
    version: 3,
    productType: 'Finished Good',
    componentCount: 15,
    operationCount: 10,
    totalCost: 35000,
    status: 'Draft',
    unitOfMeasure: 'Each',
    components: [],
    operations: [],
    createdDate: new Date('2024-02-10'),
    lastModified: new Date('2024-02-15')
  }
])

// Computed properties
const filteredBOMs = computed(() => {
  let filtered = boms.value

  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    filtered = filtered.filter(bom => 
      bom.bomNumber.toLowerCase().includes(query) ||
      bom.productName.toLowerCase().includes(query)
    )
  }

  if (selectedStatus.value) {
    filtered = filtered.filter(bom => bom.status === selectedStatus.value)
  }

  if (selectedType.value) {
    filtered = filtered.filter(bom => bom.productType === selectedType.value)
  }

  return filtered
})

const paginatedBOMs = computed(() => {
  const start = (currentPage.value - 1) * pageSize.value
  const end = start + pageSize.value
  return filteredBOMs.value.slice(start, end)
})

const totalPages = computed(() => Math.ceil(filteredBOMs.value.length / pageSize.value))

// Methods
const formatCurrency = (amount: number) => {
  return (amount / 100).toFixed(2)
}

const getStatusClass = (status: string) => {
  switch (status) {
    case 'Active': return 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200'
    case 'Draft': return 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-200'
    case 'Archived': return 'bg-gray-100 text-gray-800 dark:bg-gray-900 dark:text-gray-200'
    default: return 'bg-gray-100 text-gray-800 dark:bg-gray-900 dark:text-gray-200'
  }
}

const clearFilters = () => {
  searchQuery.value = ''
  selectedStatus.value = ''
  selectedType.value = ''
  currentPage.value = 1
}

const addComponent = () => {
  newBOM.value.components.push({
    name: '',
    quantity: 1,
    unit: 'Each',
    unitCost: 0
  })
}

const removeComponent = (index: number) => {
  newBOM.value.components.splice(index, 1)
}

const addOperation = () => {
  const nextSequence = (newBOM.value.operations.length + 1) * 10
  newBOM.value.operations.push({
    sequence: nextSequence,
    name: '',
    workCenter: '',
    setupTime: 0,
    runTime: 0
  })
}

const removeOperation = (index: number) => {
  newBOM.value.operations.splice(index, 1)
}

const calculateMaterialCost = () => {
  return newBOM.value.components.reduce((total, component) => {
    return total + (component.quantity * component.unitCost)
  }, 0)
}

const calculateLaborCost = () => {
  // Assuming labor rate of R2.50 per minute
  const laborRate = 2.50
  return newBOM.value.operations.reduce((total, operation) => {
    return total + ((operation.setupTime + operation.runTime) * laborRate)
  }, 0)
}

const saveBOM = async () => {
  try {
    // Validate required fields
    if (!newBOM.value.productName || !newBOM.value.productType) {
      alert('Please fill in all required fields')
      return
    }

    // Generate BOM number
    const bomNumber = `BOM-${String(boms.value.length + 1).padStart(3, '0')}`
    
    const newBOMData: BOM = {
      id: boms.value.length + 1,
      bomNumber,
      productName: newBOM.value.productName,
      version: newBOM.value.version,
      productType: newBOM.value.productType,
      componentCount: newBOM.value.components.length,
      operationCount: newBOM.value.operations.length,
      totalCost: Math.round((calculateMaterialCost() + calculateLaborCost()) * 100),
      status: 'Draft',
      unitOfMeasure: newBOM.value.unitOfMeasure,
      components: [...newBOM.value.components],
      operations: [...newBOM.value.operations],
      createdDate: new Date(),
      lastModified: new Date()
    }

    boms.value.push(newBOMData)
    
    // Reset form
    newBOM.value = {
      productName: '',
      productType: '',
      version: 1,
      unitOfMeasure: 'Each',
      components: [],
      operations: []
    }
    
    showCreateModal.value = false
    alert('BOM created successfully!')
  } catch (error) {
    console.error('Error creating BOM:', error)
    alert('Failed to create BOM. Please try again.')
  }
}

const viewBOM = (bom: BOM) => {
  // TODO: Implement BOM detail view
  alert(`View BOM details: ${bom.bomNumber}`)
}

const editBOM = (bom: BOM) => {
  // TODO: Implement BOM editing
  alert(`Edit BOM: ${bom.bomNumber}`)
}

const duplicateBOM = (bom: BOM) => {
  // TODO: Implement BOM duplication
  alert(`Duplicate BOM: ${bom.bomNumber}`)
}

const deleteBOM = (bom: BOM) => {
  if (confirm(`Are you sure you want to delete BOM ${bom.bomNumber}?`)) {
    const index = boms.value.findIndex(b => b.id === bom.id)
    if (index > -1) {
      boms.value.splice(index, 1)
      alert('BOM deleted successfully!')
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

// Initialize with some components for demo
onMounted(() => {
  addComponent()
  addOperation()
})
</script>
