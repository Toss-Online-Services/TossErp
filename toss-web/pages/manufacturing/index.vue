<template>
  <div class="p-6 space-y-6">
    <!-- Page Header -->
    <div class="flex justify-between items-center">
      <div>
        <h1 class="text-3xl font-bold text-slate-900 dark:text-white">Manufacturing Dashboard</h1>
        <p class="text-slate-600 dark:text-slate-400 mt-1">Monitor production operations and shop floor performance</p>
      </div>
      <div class="flex gap-3">
        <NuxtLink to="/manufacturing/work-orders" class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors">
          <span class="flex items-center gap-2">
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6"/>
            </svg>
            New Work Order
          </span>
        </NuxtLink>
        <NuxtLink to="/manufacturing/bom" class="px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 transition-colors">
          <span class="flex items-center gap-2">
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2"/>
            </svg>
            New BOM
          </span>
        </NuxtLink>
      </div>
    </div>

    <!-- KPI Cards -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
      <div class="bg-white dark:bg-slate-800 rounded-lg shadow p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-slate-600 dark:text-slate-400">Active Work Orders</p>
            <p class="text-3xl font-bold text-slate-900 dark:text-white mt-2">{{ stats.activeWorkOrders }}</p>
            <p class="text-sm text-blue-600 dark:text-blue-400 mt-2">{{ stats.releasedToday }} released today</p>
          </div>
          <div class="p-3 bg-blue-100 dark:bg-blue-900 rounded-full">
            <svg class="w-8 h-8 text-blue-600 dark:text-blue-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2m-3 7h3m-3 4h3m-6-4h.01M9 16h.01"/>
            </svg>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-slate-800 rounded-lg shadow p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-slate-600 dark:text-slate-400">Units Produced Today</p>
            <p class="text-3xl font-bold text-slate-900 dark:text-white mt-2">{{ stats.unitsProducedToday.toLocaleString() }}</p>
            <p class="text-sm text-green-600 dark:text-green-400 mt-2">Target: {{ stats.targetProduction.toLocaleString() }}</p>
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
            <p class="text-sm font-medium text-slate-600 dark:text-slate-400">Quality Rate</p>
            <p class="text-3xl font-bold text-slate-900 dark:text-white mt-2">{{ stats.qualityRate.toFixed(1) }}%</p>
            <p class="text-sm text-slate-600 dark:text-slate-400 mt-2">Rejected: {{ stats.rejectedToday }}</p>
          </div>
          <div class="p-3 bg-purple-100 dark:bg-purple-900 rounded-full">
            <svg class="w-8 h-8 text-purple-600 dark:text-purple-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4M7.835 4.697a3.42 3.42 0 001.946-.806 3.42 3.42 0 014.438 0 3.42 3.42 0 001.946.806 3.42 3.42 0 013.138 3.138 3.42 3.42 0 00.806 1.946 3.42 3.42 0 010 4.438 3.42 3.42 0 00-.806 1.946 3.42 3.42 0 01-3.138 3.138 3.42 3.42 0 00-1.946.806 3.42 3.42 0 01-4.438 0 3.42 3.42 0 00-1.946-.806 3.42 3.42 0 01-3.138-3.138 3.42 3.42 0 00-.806-1.946 3.42 3.42 0 010-4.438 3.42 3.42 0 00.806-1.946 3.42 3.42 0 013.138-3.138z"/>
            </svg>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-slate-800 rounded-lg shadow p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-slate-600 dark:text-slate-400">Production Cost</p>
            <p class="text-3xl font-bold text-slate-900 dark:text-white mt-2">R{{ (stats.totalCostToday / 100).toFixed(2) }}</p>
            <p class="text-sm text-orange-600 dark:text-orange-400 mt-2">Efficiency: {{ stats.efficiency.toFixed(1) }}%</p>
          </div>
          <div class="p-3 bg-orange-100 dark:bg-orange-900 rounded-full">
            <svg class="w-8 h-8 text-orange-600 dark:text-orange-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z"/>
            </svg>
          </div>
        </div>
      </div>
    </div>

    <!-- Charts Row -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <div class="bg-white dark:bg-slate-800 rounded-lg shadow p-6">
        <h3 class="text-lg font-semibold text-slate-900 dark:text-white mb-4">Production Trend (Last 7 Days)</h3>
        <LineChart
          :labels="productionTrendData.labels"
          :datasets="productionTrendData.datasets"
          height="300px"
          y-axis-label="Units Produced"
        />
      </div>

      <div class="bg-white dark:bg-slate-800 rounded-lg shadow p-6">
        <h3 class="text-lg font-semibold text-slate-900 dark:text-white mb-4">Capacity Utilization</h3>
        <PieChart
          :labels="capacityData.labels"
          :data="capacityData.datasets[0].data"
          :colors="capacityData.datasets[0].backgroundColor"
          type="doughnut"
          height="300px"
        />
      </div>
    </div>

    <!-- Work Order Board -->
    <div class="bg-white dark:bg-slate-800 rounded-lg shadow">
      <div class="px-6 py-4 border-b border-slate-200 dark:border-slate-700">
        <h2 class="text-xl font-semibold text-slate-900 dark:text-white">Work Order Board</h2>
      </div>
      <div class="p-6">
        <div class="grid grid-cols-1 lg:grid-cols-4 gap-4">
          <!-- Draft Column -->
          <div class="bg-slate-50 dark:bg-slate-900 rounded-lg p-4">
            <h3 class="font-semibold text-slate-700 dark:text-slate-300 mb-3">Draft ({{ workOrders.draft.length }})</h3>
            <div class="space-y-3">
              <div v-for="wo in workOrders.draft" :key="wo.id" 
                   class="bg-white dark:bg-slate-800 p-4 rounded border-l-4 border-slate-400 shadow-sm hover:shadow-md transition-shadow cursor-pointer">
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
            <h3 class="font-semibold text-slate-700 dark:text-slate-300 mb-3">Released ({{ workOrders.released.length }})</h3>
            <div class="space-y-3">
              <div v-for="wo in workOrders.released" :key="wo.id"
                   class="bg-white dark:bg-slate-800 p-4 rounded border-l-4 border-blue-400 shadow-sm hover:shadow-md transition-shadow cursor-pointer">
                <div class="font-semibold text-sm text-slate-900 dark:text-white">{{ wo.workOrderNumber }}</div>
                <div class="text-xs text-slate-600 dark:text-slate-400 mt-1">{{ wo.productName }}</div>
                <div class="text-xs text-slate-500 dark:text-slate-500 mt-2">Qty: {{ wo.quantityOrdered }}</div>
                <div class="text-xs text-blue-600 dark:text-blue-400 mt-1">Start: {{ formatDate(wo.plannedStartDate) }}</div>
              </div>
            </div>
          </div>

          <!-- In Progress Column -->
          <div class="bg-green-50 dark:bg-green-900/20 rounded-lg p-4">
            <h3 class="font-semibold text-slate-700 dark:text-slate-300 mb-3">In Progress ({{ workOrders.inProgress.length }})</h3>
            <div class="space-y-3">
              <div v-for="wo in workOrders.inProgress" :key="wo.id"
                   class="bg-white dark:bg-slate-800 p-4 rounded border-l-4 border-green-400 shadow-sm hover:shadow-md transition-shadow cursor-pointer">
                <div class="font-semibold text-sm text-slate-900 dark:text-white">{{ wo.workOrderNumber }}</div>
                <div class="text-xs text-slate-600 dark:text-slate-400 mt-1">{{ wo.productName }}</div>
                <div class="text-xs text-slate-500 dark:text-slate-500 mt-2">
                  {{ wo.quantityProduced }} / {{ wo.quantityOrdered }}
                </div>
                <div class="w-full bg-slate-200 dark:bg-slate-700 rounded-full h-2 mt-2">
                  <div class="bg-green-600 h-2 rounded-full" 
                       :style="{ width: (wo.quantityProduced / wo.quantityOrdered * 100) + '%' }">
                  </div>
                </div>
                <div class="text-xs text-slate-500 dark:text-slate-500 mt-1">
                  {{ ((wo.quantityProduced / wo.quantityOrdered) * 100).toFixed(0) }}% Complete
                </div>
              </div>
            </div>
          </div>

          <!-- Completed Column -->
          <div class="bg-emerald-50 dark:bg-emerald-900/20 rounded-lg p-4">
            <h3 class="font-semibold text-slate-700 dark:text-slate-300 mb-3">Completed ({{ workOrders.completed.length }})</h3>
            <div class="space-y-3">
              <div v-for="wo in workOrders.completed" :key="wo.id"
                   class="bg-white dark:bg-slate-800 p-4 rounded border-l-4 border-emerald-400 shadow-sm hover:shadow-md transition-shadow cursor-pointer">
                <div class="font-semibold text-sm text-slate-900 dark:text-white">{{ wo.workOrderNumber }}</div>
                <div class="text-xs text-slate-600 dark:text-slate-400 mt-1">{{ wo.productName }}</div>
                <div class="text-xs text-emerald-600 dark:text-emerald-400 mt-2">
                  ✓ {{ wo.quantityProduced }} units produced
                </div>
                <div class="text-xs text-slate-500 dark:text-slate-500 mt-1">{{ formatDate(wo.actualEndDate) }}</div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Active BOMs Table -->
    <div class="bg-white dark:bg-slate-800 rounded-lg shadow">
      <div class="px-6 py-4 border-b border-slate-200 dark:border-slate-700">
        <h2 class="text-xl font-semibold text-slate-900 dark:text-white">Active Bills of Materials</h2>
      </div>
      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-slate-200 dark:divide-slate-700">
          <thead class="bg-slate-50 dark:bg-slate-900">
            <tr>
              <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">BOM #</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">Product</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">Version</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">Items</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">Operations</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">Total Cost</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">Status</th>
            </tr>
          </thead>
          <tbody class="bg-white dark:bg-slate-800 divide-y divide-slate-200 dark:divide-slate-700">
            <tr v-for="bom in activeBoms" :key="bom.id" class="hover:bg-slate-50 dark:hover:bg-slate-700 cursor-pointer">
              <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-blue-600 dark:text-blue-400">{{ bom.bomNumber }}</td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-slate-900 dark:text-white">{{ bom.productName }}</td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-slate-600 dark:text-slate-400">v{{ bom.version }}</td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-slate-600 dark:text-slate-400">{{ bom.itemCount }}</td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-slate-600 dark:text-slate-400">{{ bom.operationCount }}</td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-slate-900 dark:text-white">R{{ (bom.totalCost / 100).toFixed(2) }}</td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span class="px-2 py-1 text-xs rounded-full bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200">{{ bom.status }}</span>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Shop Floor Performance -->
    <div class="bg-white dark:bg-slate-800 rounded-lg shadow">
      <div class="px-6 py-4 border-b border-slate-200 dark:border-slate-700">
        <h2 class="text-xl font-semibold text-slate-900 dark:text-white">Shop Floor Performance</h2>
      </div>
      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-slate-200 dark:divide-slate-700">
          <thead class="bg-slate-50 dark:bg-slate-900">
            <tr>
              <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">Work Center</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">Active Orders</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">Produced Today</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">Efficiency</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">Utilization</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">Status</th>
            </tr>
          </thead>
          <tbody class="bg-white dark:bg-slate-800 divide-y divide-slate-200 dark:divide-slate-700">
            <tr v-for="wc in shopFloorPerformance" :key="wc.id" class="hover:bg-slate-50 dark:hover:bg-slate-700">
              <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-slate-900 dark:text-white">{{ wc.name }}</td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-slate-600 dark:text-slate-400">{{ wc.activeOrders }}</td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-slate-600 dark:text-slate-400">{{ wc.producedToday }}</td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-slate-600 dark:text-slate-400">{{ wc.efficiency }}%</td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-slate-600 dark:text-slate-400">{{ wc.utilization }}%</td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span class="px-2 py-1 text-xs rounded-full" :class="getStatusClass(wc.status)">
                  {{ wc.status }}
                </span>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import LineChart from '~/components/charts/LineChart.vue'
import PieChart from '~/components/charts/PieChart.vue'

definePageMeta({
  layout: 'default',
  middleware: ['auth']
})

useHead({
  title: 'Manufacturing Dashboard - TOSS ERP III'
})

const { getProductionTrendData, getCapacityUtilizationData } = useCharts()

// Chart data
const productionTrendData = getProductionTrendData()
const capacityData = getCapacityUtilizationData()

// Mock data - would be fetched from API
const stats = ref({
  activeWorkOrders: 23,
  releasedToday: 5,
  unitsProducedToday: 1250,
  targetProduction: 1500,
  qualityRate: 96.5,
  rejectedToday: 45,
  totalCostToday: 2850000, // R28,500.00
  efficiency: 89.2
})

const workOrders = ref({
  draft: [
    { id: 1, workOrderNumber: 'WO-001', productName: 'Widget A', quantityOrdered: 100, type: 'Production', priority: 5 },
    { id: 2, workOrderNumber: 'WO-002', productName: 'Gadget B', quantityOrdered: 50, type: 'Assembly', priority: 3 }
  ],
  released: [
    { id: 3, workOrderNumber: 'WO-003', productName: 'Component C', quantityOrdered: 200, plannedStartDate: new Date() },
    { id: 4, workOrderNumber: 'WO-004', productName: 'Part D', quantityOrdered: 75, plannedStartDate: new Date() }
  ],
  inProgress: [
    { id: 5, workOrderNumber: 'WO-005', productName: 'Assembly E', quantityOrdered: 150, quantityProduced: 120 },
    { id: 6, workOrderNumber: 'WO-006', productName: 'Product F', quantityOrdered: 80, quantityProduced: 50 }
  ],
  completed: [
    { id: 7, workOrderNumber: 'WO-007', productName: 'Widget G', quantityOrdered: 100, quantityProduced: 100, actualEndDate: new Date() }
  ]
})

const activeBoms = ref([
  { id: 1, bomNumber: 'BOM-001', productName: 'Widget A', version: 2, itemCount: 8, operationCount: 5, totalCost: 15000, status: 'Active' },
  { id: 2, bomNumber: 'BOM-002', productName: 'Gadget B', version: 1, itemCount: 12, operationCount: 7, totalCost: 22500, status: 'Active' },
  { id: 3, bomNumber: 'BOM-003', productName: 'Assembly E', version: 3, itemCount: 15, operationCount: 10, totalCost: 35000, status: 'Active' }
])

const shopFloorPerformance = ref([
  { id: 1, name: 'Assembly Line 1', activeOrders: 3, producedToday: 450, efficiency: 92, utilization: 85, status: 'Running' },
  { id: 2, name: 'Assembly Line 2', activeOrders: 2, producedToday: 380, efficiency: 88, utilization: 78, status: 'Running' },
  { id: 3, name: 'Packaging Station', activeOrders: 4, producedToday: 620, efficiency: 95, utilization: 90, status: 'Running' },
  { id: 4, name: 'Quality Control', activeOrders: 1, producedToday: 150, efficiency: 98, utilization: 65, status: 'Running' }
])

function getPriorityClass(priority: number) {
  if (priority >= 8) return 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200'
  if (priority >= 5) return 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-200'
  return 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200'
}

function getStatusClass(status: string) {
  if (status === 'Running') return 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200'
  if (status === 'Idle') return 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-200'
  return 'bg-slate-100 text-slate-800 dark:bg-slate-900 dark:text-slate-200'
}

function formatDate(date: Date) {
  return new Date(date).toLocaleDateString('en-ZA', { month: 'short', day: 'numeric' })
}
</script>
