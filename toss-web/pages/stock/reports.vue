<template>
  <div class="space-y-6">
    <!-- Header -->
    <div class="border-b border-gray-200 dark:border-gray-700 pb-4">
      <div class="flex items-center justify-between">
        <div>
          <h1 class="text-2xl font-bold text-gray-900 dark:text-white">Stock Reports</h1>
          <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
            Comprehensive inventory analytics and reporting
          </p>
        </div>
        <div class="flex space-x-3">
          <button @click="scheduleReport" class="inline-flex items-center px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg text-sm font-medium text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600">
            <CalendarIcon class="w-4 h-4 mr-2" />
            Schedule Report
          </button>
          <button @click="exportAllReports" class="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-lg text-white bg-blue-600 hover:bg-blue-700">
            <DocumentArrowDownIcon class="w-4 h-4 mr-2" />
            Export All
          </button>
        </div>
      </div>
    </div>

    <!-- Report Categories -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
      <!-- Inventory Reports -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <div class="flex items-center mb-4">
          <div class="w-10 h-10 bg-blue-100 dark:bg-blue-900 rounded-lg flex items-center justify-center mr-3">
            <ArchiveBoxIcon class="w-6 h-6 text-blue-600 dark:text-blue-400" />
          </div>
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Inventory Reports</h3>
        </div>
        <div class="space-y-3">
          <button @click="generateSpecificReport('Stock Balance Report', 'Inventory')" class="w-full text-left p-3 rounded-lg border border-gray-200 dark:border-gray-600 hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors">
            <div class="font-medium text-gray-900 dark:text-white">Stock Balance Report</div>
            <div class="text-sm text-gray-500 dark:text-gray-400">Current stock levels by item and warehouse</div>
          </button>
          <button @click="generateSpecificReport('Low Stock Report', 'Inventory')" class="w-full text-left p-3 rounded-lg border border-gray-200 dark:border-gray-600 hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors">
            <div class="font-medium text-gray-900 dark:text-white">Low Stock Report</div>
            <div class="text-sm text-gray-500 dark:text-gray-400">Items below minimum stock levels</div>
          </button>
          <button @click="generateSpecificReport('Stock Aging Report', 'Inventory')" class="w-full text-left p-3 rounded-lg border border-gray-200 dark:border-gray-600 hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors">
            <div class="font-medium text-gray-900 dark:text-white">Stock Aging Report</div>
            <div class="text-sm text-gray-500 dark:text-gray-400">Analysis of slow-moving inventory</div>
          </button>
          <button @click="generateSpecificReport('ABC Analysis', 'Inventory')" class="w-full text-left p-3 rounded-lg border border-gray-200 dark:border-gray-600 hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors">
            <div class="font-medium text-gray-900 dark:text-white">ABC Analysis</div>
            <div class="text-sm text-gray-500 dark:text-gray-400">Inventory categorization by value</div>
          </button>
        </div>
      </div>

      <!-- Movement Reports -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <div class="flex items-center mb-4">
          <div class="w-10 h-10 bg-green-100 dark:bg-green-900 rounded-lg flex items-center justify-center mr-3">
            <ArrowsRightLeftIcon class="w-6 h-6 text-green-600 dark:text-green-400" />
          </div>
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Movement Reports</h3>
        </div>
        <div class="space-y-3">
          <button @click="generateSpecificReport('Stock Movement History', 'Movement')" class="w-full text-left p-3 rounded-lg border border-gray-200 dark:border-gray-600 hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors">
            <div class="font-medium text-gray-900 dark:text-white">Stock Movement History</div>
            <div class="text-sm text-gray-500 dark:text-gray-400">Detailed transaction history</div>
          </button>
          <button @click="generateSpecificReport('Consumption Report', 'Movement')" class="w-full text-left p-3 rounded-lg border border-gray-200 dark:border-gray-600 hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors">
            <div class="font-medium text-gray-900 dark:text-white">Consumption Report</div>
            <div class="text-sm text-gray-500 dark:text-gray-400">Material consumption patterns</div>
          </button>
          <button @click="generateSpecificReport('Transfer Report', 'Movement')" class="w-full text-left p-3 rounded-lg border border-gray-200 dark:border-gray-600 hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors">
            <div class="font-medium text-gray-900 dark:text-white">Transfer Report</div>
            <div class="text-sm text-gray-500 dark:text-gray-400">Inter-warehouse transfers</div>
          </button>
          <button @click="generateSpecificReport('Adjustment Report', 'Movement')" class="w-full text-left p-3 rounded-lg border border-gray-200 dark:border-gray-600 hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors">
            <div class="font-medium text-gray-900 dark:text-white">Adjustment Report</div>
            <div class="text-sm text-gray-500 dark:text-gray-400">Stock adjustments and reconciliations</div>
          </button>
        </div>
      </div>

      <!-- Valuation Reports -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <div class="flex items-center mb-4">
          <div class="w-10 h-10 bg-purple-100 dark:bg-purple-900 rounded-lg flex items-center justify-center mr-3">
            <CurrencyDollarIcon class="w-6 h-6 text-purple-600 dark:text-purple-400" />
          </div>
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Valuation Reports</h3>
        </div>
        <div class="space-y-3">
          <button @click="generateSpecificReport('Stock Valuation', 'Valuation')" class="w-full text-left p-3 rounded-lg border border-gray-200 dark:border-gray-600 hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors">
            <div class="font-medium text-gray-900 dark:text-white">Stock Valuation</div>
            <div class="text-sm text-gray-500 dark:text-gray-400">Total inventory value by location</div>
          </button>
          <button @click="generateSpecificReport('Cost Analysis', 'Valuation')" class="w-full text-left p-3 rounded-lg border border-gray-200 dark:border-gray-600 hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors">
            <div class="font-medium text-gray-900 dark:text-white">Cost Analysis</div>
            <div class="text-sm text-gray-500 dark:text-gray-400">Cost breakdown and trends</div>
          </button>
          <button @click="generateSpecificReport('Price Variance Report', 'Valuation')" class="w-full text-left p-3 rounded-lg border border-gray-200 dark:border-gray-600 hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors">
            <div class="font-medium text-gray-900 dark:text-white">Price Variance Report</div>
            <div class="text-sm text-gray-500 dark:text-gray-400">Purchase price variance analysis</div>
          </button>
          <button @click="generateSpecificReport('Profitability Analysis', 'Valuation')" class="w-full text-left p-3 rounded-lg border border-gray-200 dark:border-gray-600 hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors">
            <div class="font-medium text-gray-900 dark:text-white">Profitability Analysis</div>
            <div class="text-sm text-gray-500 dark:text-gray-400">Item-wise profit margins</div>
          </button>
        </div>
      </div>
    </div>

    <!-- Report Filters -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
      <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">Generate Custom Report</h3>
      <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
        <div>
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Report Type</label>
          <select v-model="selectedReportType" class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
            <option value="">Select Report</option>
            <option value="stock-balance">Stock Balance</option>
            <option value="movement-history">Movement History</option>
            <option value="valuation">Stock Valuation</option>
            <option value="aging">Stock Aging</option>
          </select>
        </div>
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
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Date Range</label>
          <select v-model="selectedDateRange" class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
            <option value="today">Today</option>
            <option value="week">This Week</option>
            <option value="month">This Month</option>
            <option value="quarter">This Quarter</option>
            <option value="year">This Year</option>
            <option value="custom">Custom Range</option>
          </select>
        </div>
        <div class="flex items-end">
          <button @click="generateReport" class="w-full px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors">
            Generate Report
          </button>
        </div>
      </div>
    </div>

    <!-- Recent Reports -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
      <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
        <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Recent Reports</h3>
      </div>
      <div class="overflow-x-auto">
        <table class="w-full">
          <thead class="bg-gray-50 dark:bg-gray-700">
            <tr>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                Report Name
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                Type
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                Generated
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                Size
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
            <tr v-for="report in recentReports" :key="report.id" class="hover:bg-gray-50 dark:hover:bg-gray-700">
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="flex items-center">
                  <div class="w-8 h-8 bg-blue-100 dark:bg-blue-900 rounded-lg flex items-center justify-center mr-3">
                    <DocumentTextIcon class="w-4 h-4 text-blue-600 dark:text-blue-400" />
                  </div>
                  <div>
                    <div class="text-sm font-medium text-gray-900 dark:text-white">{{ report.name }}</div>
                    <div class="text-sm text-gray-500 dark:text-gray-400">{{ report.description }}</div>
                  </div>
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">
                {{ report.type }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm text-gray-900 dark:text-white">{{ report.generated }}</div>
                <div class="text-sm text-gray-500 dark:text-gray-400">{{ report.generatedBy }}</div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">
                {{ report.size }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span class="inline-flex px-2 py-1 text-xs font-semibold rounded-full" :class="getStatusClass(report.status)">
                  {{ report.status }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                <div class="flex space-x-2">
                  <button @click="downloadReport(report)" class="text-blue-600 dark:text-blue-400 hover:text-blue-700" title="Download">
                    <ArrowDownTrayIcon class="w-4 h-4" />
                  </button>
                  <button @click="viewReport(report)" class="text-green-600 dark:text-green-400 hover:text-green-700" title="View">
                    <EyeIcon class="w-4 h-4" />
                  </button>
                  <button @click="shareReport(report)" class="text-gray-600 dark:text-gray-400 hover:text-gray-700" title="Share">
                    <ShareIcon class="w-4 h-4" />
                  </button>
                  <button @click="deleteReport(report)" class="text-red-600 dark:text-red-400 hover:text-red-700" title="Delete">
                    <TrashIcon class="w-4 h-4" />
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Report Analytics Dashboard -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <!-- Top Items by Value -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">Top Items by Value</h3>
        <div class="space-y-3">
          <div v-for="item in topItemsByValue" :key="item.id" class="flex items-center justify-between p-3 bg-gray-50 dark:bg-gray-700 rounded-lg">
            <div>
              <div class="font-medium text-gray-900 dark:text-white">{{ item.name }}</div>
              <div class="text-sm text-gray-500 dark:text-gray-400">{{ item.sku }}</div>
            </div>
            <div class="text-right">
              <div class="font-medium text-gray-900 dark:text-white">${{ item.value.toLocaleString() }}</div>
              <div class="text-sm text-gray-500 dark:text-gray-400">{{ item.quantity }} units</div>
            </div>
          </div>
        </div>
      </div>

      <!-- Stock Trends -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">Stock Level Trends</h3>
        <div class="space-y-4">
          <div class="flex items-center justify-between">
            <span class="text-sm text-gray-600 dark:text-gray-400">Total Stock Value</span>
            <span class="text-lg font-semibold text-gray-900 dark:text-white">${{ totalStockValue.toLocaleString() }}</span>
          </div>
          <div class="flex items-center justify-between">
            <span class="text-sm text-gray-600 dark:text-gray-400">Items Below Reorder Level</span>
            <span class="text-lg font-semibold text-red-600 dark:text-red-400">{{ lowStockItems }}</span>
          </div>
          <div class="flex items-center justify-between">
            <span class="text-sm text-gray-600 dark:text-gray-400">Fast Moving Items</span>
            <span class="text-lg font-semibold text-green-600 dark:text-green-400">{{ fastMovingItems }}</span>
          </div>
          <div class="flex items-center justify-between">
            <span class="text-sm text-gray-600 dark:text-gray-400">Dead Stock Items</span>
            <span class="text-lg font-semibold text-yellow-600 dark:text-yellow-400">{{ deadStockItems }}</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import {
  CalendarIcon,
  DocumentArrowDownIcon,
  ArchiveBoxIcon,
  ArrowsRightLeftIcon,
  CurrencyDollarIcon,
  DocumentTextIcon,
  ArrowDownTrayIcon,
  EyeIcon,
  ShareIcon,
  TrashIcon
} from '@heroicons/vue/24/outline'

useHead({
  title: 'Stock Reports - TOSS ERP III'
})

// Reactive data
const selectedReportType = ref('')
const selectedWarehouse = ref('')
const selectedDateRange = ref('month')

// Sample data
const warehouses = ref([
  { id: 1, name: 'Main Warehouse' },
  { id: 2, name: 'Secondary Storage' },
  { id: 3, name: 'Retail Location A' },
  { id: 4, name: 'Retail Location B' }
])

const recentReports = ref([
  {
    id: 1,
    name: 'Monthly Stock Balance',
    description: 'Complete stock balance as of Jan 31, 2024',
    type: 'Stock Balance',
    generated: '2024-01-31',
    generatedBy: 'John Doe',
    size: '2.3 MB',
    status: 'completed'
  },
  {
    id: 2,
    name: 'Q1 Movement Analysis',
    description: 'Stock movement analysis for Q1 2024',
    type: 'Movement History',
    generated: '2024-01-30',
    generatedBy: 'Jane Smith',
    size: '5.1 MB',
    status: 'completed'
  },
  {
    id: 3,
    name: 'Weekly Valuation Report',
    description: 'Current week stock valuation',
    type: 'Valuation',
    generated: '2024-01-29',
    generatedBy: 'Mike Wilson',
    size: '1.8 MB',
    status: 'processing'
  },
  {
    id: 4,
    name: 'Low Stock Alert Report',
    description: 'Items requiring immediate reorder',
    type: 'Low Stock',
    generated: '2024-01-28',
    generatedBy: 'Sarah Lee',
    size: '890 KB',
    status: 'completed'
  }
])

const topItemsByValue = ref([
  { id: 1, name: 'iPhone 15 Pro', sku: 'IPH-15-PRO', value: 125000, quantity: 125 },
  { id: 2, name: 'MacBook Air M2', sku: 'MBA-M2-13', value: 89000, quantity: 89 },
  { id: 3, name: 'Samsung Galaxy S24', sku: 'SGS-24-ULT', value: 67500, quantity: 75 },
  { id: 4, name: 'iPad Pro 12.9"', sku: 'IPD-PRO-129', value: 45600, quantity: 57 },
  { id: 5, name: 'AirPods Pro 2', sku: 'APP-2-USB', value: 32400, quantity: 162 }
])

// Analytics data
const totalStockValue = ref(2456789)
const lowStockItems = ref(23)
const fastMovingItems = ref(87)
const deadStockItems = ref(12)

// Methods
const getStatusClass = (status: string) => {
  const classes = {
    'completed': 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200',
    'processing': 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200',
    'failed': 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200'
  }
  return classes[status as keyof typeof classes] || classes.completed
}

const generateReport = async () => {
  if (!selectedReportType.value) {
    alert('Please select a report type')
    return
  }
  
  alert(`Generating ${selectedReportType.value} report...\nThis will create a PDF/Excel report based on your selections.`)
  
  // Simulate report generation
  await new Promise(resolve => setTimeout(resolve, 1000))
  
  // Add to recent reports
  const reportTypes: Record<string, string> = {
    'stock-balance': 'Stock Balance',
    'movement-history': 'Movement History',
    'valuation': 'Stock Valuation',
    'aging': 'Stock Aging'
  }
  
  const newReport = {
    id: recentReports.value.length + 1,
    name: `${reportTypes[selectedReportType.value] || 'Stock'} Report`,
    description: `Generated on ${new Date().toLocaleDateString()}`,
    type: reportTypes[selectedReportType.value] || 'Stock Report',
    generated: new Date().toLocaleDateString(),
    generatedBy: 'Current User',
    size: `${Math.floor(Math.random() * 5 + 1)}.${Math.floor(Math.random() * 9)} MB`,
    status: 'completed'
  }
  
  recentReports.value.unshift(newReport)
  alert('Report generated successfully!')
}

const generateSpecificReport = (reportType: string, category: string) => {
  alert(`Generating ${category} - ${reportType}...\nThis will create a detailed report with charts and tables.`)
}

const downloadReport = (report: any) => {
  alert(`Downloading ${report.name}...\nFile: ${report.name.replace(/ /g, '_')}.pdf`)
}

const viewReport = (report: any) => {
  alert(`Opening ${report.name} in viewer...`)
}

const shareReport = (report: any) => {
  alert(`Share ${report.name} via email or link...`)
}

const deleteReport = (report: any) => {
  if (confirm(`Delete ${report.name}?`)) {
    const index = recentReports.value.findIndex(r => r.id === report.id)
    if (index > -1) {
      recentReports.value.splice(index, 1)
      alert('Report deleted successfully!')
    }
  }
}

const scheduleReport = () => {
  alert('Schedule Report Feature\n\nSetup automated report generation:\n- Daily/Weekly/Monthly schedules\n- Email delivery\n- Multiple recipients\n\nFeature coming soon!')
}

const exportAllReports = async () => {
  const exportData = recentReports.value.map(report => ({
    'Report Name': report.name,
    'Type': report.type,
    'Generated': report.generated,
    'Generated By': report.generatedBy,
    'Size': report.size,
    'Status': report.status
  }))

  const csvContent = [
    Object.keys(exportData[0]).join(','),
    ...exportData.map(row => Object.values(row).map(v => `"${v}"`).join(','))
  ].join('\n')

  const blob = new Blob([csvContent], { type: 'text/csv' })
  const url = window.URL.createObjectURL(blob)
  const a = document.createElement('a')
  a.href = url
  a.download = `stock-reports-index-${new Date().toISOString().split('T')[0]}.csv`
  a.click()
  window.URL.revokeObjectURL(url)
  
  alert('Reports index exported successfully!')
}
</script>
