<template>
  <AppLayout>
    <div class="p-6 space-y-6">
      <!-- Header -->
      <div class="flex justify-between items-center">
        <div>
          <h1 class="text-2xl font-bold">Financial Reports</h1>
          <p class="text-muted-foreground">Comprehensive financial analysis and reporting for your business</p>
        </div>
        <div class="flex items-center space-x-4">
          <Button variant="outline">
            <Download class="h-4 w-4 mr-2" />
            Export All
          </Button>
          <Button>
            <Calendar class="h-4 w-4 mr-2" />
            Schedule Report
          </Button>
        </div>
      </div>

      <!-- Quick Stats -->
      <div class="grid grid-cols-1 md:grid-cols-4 gap-6">
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg border">
          <div class="flex items-center space-x-3">
            <div class="p-2 bg-green-100 dark:bg-green-900 rounded-lg">
              <TrendingUp class="h-6 w-6 text-green-600" />
            </div>
            <div>
              <p class="text-sm text-muted-foreground">Monthly Profit</p>
              <p class="text-2xl font-bold text-green-600">R {{ monthlyProfit.toLocaleString() }}</p>
              <p class="text-xs text-green-600">+12.5% vs last month</p>
            </div>
          </div>
        </div>
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg border">
          <div class="flex items-center space-x-3">
            <div class="p-2 bg-blue-100 dark:bg-blue-900 rounded-lg">
              <DollarSign class="h-6 w-6 text-blue-600" />
            </div>
            <div>
              <p class="text-sm text-muted-foreground">Total Revenue</p>
              <p class="text-2xl font-bold text-blue-600">R {{ totalRevenue.toLocaleString() }}</p>
              <p class="text-xs text-blue-600">This month</p>
            </div>
          </div>
        </div>
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg border">
          <div class="flex items-center space-x-3">
            <div class="p-2 bg-red-100 dark:bg-red-900 rounded-lg">
              <TrendingDown class="h-6 w-6 text-red-600" />
            </div>
            <div>
              <p class="text-sm text-muted-foreground">Total Expenses</p>
              <p class="text-2xl font-bold text-red-600">R {{ totalExpenses.toLocaleString() }}</p>
              <p class="text-xs text-red-600">This month</p>
            </div>
          </div>
        </div>
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg border">
          <div class="flex items-center space-x-3">
            <div class="p-2 bg-purple-100 dark:bg-purple-900 rounded-lg">
              <Percent class="h-6 w-6 text-purple-600" />
            </div>
            <div>
              <p class="text-sm text-muted-foreground">Profit Margin</p>
              <p class="text-2xl font-bold text-purple-600">{{ profitMargin }}%</p>
              <p class="text-xs text-purple-600">Industry avg: 18%</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Report Filters -->
      <div class="bg-white dark:bg-gray-800 p-6 rounded-lg border">
        <div class="flex flex-wrap items-center gap-4">
          <div>
            <label class="block text-sm font-medium mb-2">Report Period</label>
            <select v-model="selectedPeriod" class="px-3 py-2 border rounded-md">
              <option value="current_month">Current Month</option>
              <option value="last_month">Last Month</option>
              <option value="quarter">This Quarter</option>
              <option value="year">This Year</option>
              <option value="custom">Custom Range</option>
            </select>
          </div>
          <div>
            <label class="block text-sm font-medium mb-2">Report Type</label>
            <select v-model="selectedReportType" class="px-3 py-2 border rounded-md">
              <option value="all">All Reports</option>
              <option value="profit_loss">Profit & Loss</option>
              <option value="balance_sheet">Balance Sheet</option>
              <option value="cash_flow">Cash Flow</option>
              <option value="trial_balance">Trial Balance</option>
            </select>
          </div>
          <div class="flex items-end">
            <Button class="mt-6">
              <Filter class="h-4 w-4 mr-2" />
              Apply Filters
            </Button>
          </div>
        </div>
      </div>

      <!-- Reports Grid -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <!-- Profit & Loss Statement -->
        <div class="bg-white dark:bg-gray-800 rounded-lg border">
          <div class="p-6 border-b">
            <div class="flex justify-between items-center">
              <div class="flex items-center space-x-2">
                <BarChart3 class="h-5 w-5 text-green-600" />
                <h3 class="text-lg font-semibold">Profit & Loss Statement</h3>
              </div>
              <Button variant="outline" size="sm">
                <Download class="h-4 w-4 mr-2" />
                Export
              </Button>
            </div>
          </div>
          <div class="p-6">
            <div class="space-y-4">
              <!-- Revenue Section -->
              <div>
                <p class="font-medium text-blue-600 mb-3">Revenue</p>
                <div class="space-y-2 ml-4">
                  <div v-for="item in profitLossData.revenue" :key="item.name" class="flex justify-between">
                    <span class="text-muted-foreground">{{ item.name }}</span>
                    <span class="text-blue-600">R {{ item.amount.toLocaleString() }}</span>
                  </div>
                  <div class="flex justify-between font-medium border-t pt-2">
                    <span>Total Revenue</span>
                    <span class="text-blue-600">R {{ profitLossData.totalRevenue.toLocaleString() }}</span>
                  </div>
                </div>
              </div>

              <!-- Expenses Section -->
              <div>
                <p class="font-medium text-red-600 mb-3">Expenses</p>
                <div class="space-y-2 ml-4">
                  <div v-for="item in profitLossData.expenses" :key="item.name" class="flex justify-between">
                    <span class="text-muted-foreground">{{ item.name }}</span>
                    <span class="text-red-600">R {{ item.amount.toLocaleString() }}</span>
                  </div>
                  <div class="flex justify-between font-medium border-t pt-2">
                    <span>Total Expenses</span>
                    <span class="text-red-600">R {{ profitLossData.totalExpenses.toLocaleString() }}</span>
                  </div>
                </div>
              </div>

              <!-- Net Income -->
              <div class="border-t pt-4">
                <div class="flex justify-between font-bold text-lg">
                  <span>Net Income</span>
                  <span class="text-green-600">R {{ (profitLossData.totalRevenue - profitLossData.totalExpenses).toLocaleString() }}</span>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Balance Sheet -->
        <div class="bg-white dark:bg-gray-800 rounded-lg border">
          <div class="p-6 border-b">
            <div class="flex justify-between items-center">
              <div class="flex items-center space-x-2">
                <Scale class="h-5 w-5 text-blue-600" />
                <h3 class="text-lg font-semibold">Balance Sheet</h3>
              </div>
              <Button variant="outline" size="sm">
                <Download class="h-4 w-4 mr-2" />
                Export
              </Button>
            </div>
          </div>
          <div class="p-6">
            <div class="space-y-4">
              <!-- Assets Section -->
              <div>
                <p class="font-medium text-green-600 mb-3">Assets</p>
                <div class="space-y-2 ml-4">
                  <div v-for="item in balanceSheetData.assets" :key="item.name" class="flex justify-between">
                    <span class="text-muted-foreground">{{ item.name }}</span>
                    <span class="text-green-600">R {{ item.amount.toLocaleString() }}</span>
                  </div>
                  <div class="flex justify-between font-medium border-t pt-2">
                    <span>Total Assets</span>
                    <span class="text-green-600">R {{ balanceSheetData.totalAssets.toLocaleString() }}</span>
                  </div>
                </div>
              </div>

              <!-- Liabilities Section -->
              <div>
                <p class="font-medium text-red-600 mb-3">Liabilities</p>
                <div class="space-y-2 ml-4">
                  <div v-for="item in balanceSheetData.liabilities" :key="item.name" class="flex justify-between">
                    <span class="text-muted-foreground">{{ item.name }}</span>
                    <span class="text-red-600">R {{ item.amount.toLocaleString() }}</span>
                  </div>
                  <div class="flex justify-between font-medium border-t pt-2">
                    <span>Total Liabilities</span>
                    <span class="text-red-600">R {{ balanceSheetData.totalLiabilities.toLocaleString() }}</span>
                  </div>
                </div>
              </div>

              <!-- Equity Section -->
              <div class="border-t pt-4">
                <div class="flex justify-between font-bold">
                  <span>Owner's Equity</span>
                  <span class="text-blue-600">R {{ (balanceSheetData.totalAssets - balanceSheetData.totalLiabilities).toLocaleString() }}</span>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Cash Flow Statement -->
        <div class="bg-white dark:bg-gray-800 rounded-lg border">
          <div class="p-6 border-b">
            <div class="flex justify-between items-center">
              <div class="flex items-center space-x-2">
                <ArrowUpDown class="h-5 w-5 text-purple-600" />
                <h3 class="text-lg font-semibold">Cash Flow Statement</h3>
              </div>
              <Button variant="outline" size="sm">
                <Download class="h-4 w-4 mr-2" />
                Export
              </Button>
            </div>
          </div>
          <div class="p-6">
            <div class="space-y-4">
              <!-- Operating Activities -->
              <div>
                <p class="font-medium text-blue-600 mb-3">Operating Activities</p>
                <div class="space-y-2 ml-4">
                  <div v-for="item in cashFlowData.operating" :key="item.name" class="flex justify-between">
                    <span class="text-muted-foreground">{{ item.name }}</span>
                    <span :class="item.amount >= 0 ? 'text-green-600' : 'text-red-600'">
                      {{ item.amount >= 0 ? '+' : '' }}R {{ item.amount.toLocaleString() }}
                    </span>
                  </div>
                  <div class="flex justify-between font-medium border-t pt-2">
                    <span>Net Operating Cash Flow</span>
                    <span class="text-blue-600">R {{ cashFlowData.netOperating.toLocaleString() }}</span>
                  </div>
                </div>
              </div>

              <!-- Investing Activities -->
              <div>
                <p class="font-medium text-purple-600 mb-3">Investing Activities</p>
                <div class="space-y-2 ml-4">
                  <div v-for="item in cashFlowData.investing" :key="item.name" class="flex justify-between">
                    <span class="text-muted-foreground">{{ item.name }}</span>
                    <span :class="item.amount >= 0 ? 'text-green-600' : 'text-red-600'">
                      {{ item.amount >= 0 ? '+' : '' }}R {{ item.amount.toLocaleString() }}
                    </span>
                  </div>
                  <div class="flex justify-between font-medium border-t pt-2">
                    <span>Net Investing Cash Flow</span>
                    <span class="text-purple-600">R {{ cashFlowData.netInvesting.toLocaleString() }}</span>
                  </div>
                </div>
              </div>

              <!-- Net Change -->
              <div class="border-t pt-4">
                <div class="flex justify-between font-bold">
                  <span>Net Change in Cash</span>
                  <span class="text-green-600">R {{ (cashFlowData.netOperating + cashFlowData.netInvesting).toLocaleString() }}</span>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Key Financial Ratios -->
        <div class="bg-white dark:bg-gray-800 rounded-lg border">
          <div class="p-6 border-b">
            <div class="flex justify-between items-center">
              <div class="flex items-center space-x-2">
                <Calculator class="h-5 w-5 text-orange-600" />
                <h3 class="text-lg font-semibold">Key Financial Ratios</h3>
              </div>
              <Button variant="outline" size="sm">
                <Eye class="h-4 w-4 mr-2" />
                Details
              </Button>
            </div>
          </div>
          <div class="p-6">
            <div class="grid grid-cols-2 gap-4">
              <div v-for="ratio in financialRatios" :key="ratio.name" class="p-4 border rounded-lg">
                <div class="text-center">
                  <p class="text-2xl font-bold" :class="ratio.status === 'good' ? 'text-green-600' : ratio.status === 'warning' ? 'text-yellow-600' : 'text-red-600'">
                    {{ ratio.value }}{{ ratio.unit }}
                  </p>
                  <p class="text-sm font-medium">{{ ratio.name }}</p>
                  <p class="text-xs text-muted-foreground">{{ ratio.benchmark }}</p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Monthly Trends Chart -->
      <div class="bg-white dark:bg-gray-800 rounded-lg border">
        <div class="p-6 border-b">
          <div class="flex justify-between items-center">
            <h3 class="text-lg font-semibold">Monthly Financial Trends</h3>
            <div class="flex space-x-2">
              <Button variant="outline" size="sm">Revenue</Button>
              <Button variant="outline" size="sm">Expenses</Button>
              <Button variant="outline" size="sm">Profit</Button>
            </div>
          </div>
        </div>
        <div class="p-6">
          <div class="h-64 flex items-center justify-center border rounded-lg bg-gray-50 dark:bg-gray-700">
            <p class="text-muted-foreground">Financial trends chart would go here</p>
          </div>
        </div>
      </div>

      <!-- Tax Summary -->
      <div class="bg-white dark:bg-gray-800 rounded-lg border">
        <div class="p-6 border-b">
          <div class="flex justify-between items-center">
            <div class="flex items-center space-x-2">
              <FileText class="h-5 w-5 text-green-600" />
              <h3 class="text-lg font-semibold">VAT & Tax Summary</h3>
            </div>
            <Button variant="outline">
              <Calendar class="h-4 w-4 mr-2" />
              Tax Calendar
            </Button>
          </div>
        </div>
        <div class="p-6">
          <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
            <div class="p-4 border rounded-lg">
              <div class="text-center">
                <p class="text-2xl font-bold text-blue-600">R {{ vatPayable.toLocaleString() }}</p>
                <p class="text-sm font-medium">VAT Payable</p>
                <p class="text-xs text-muted-foreground">Due: 25th Jan 2024</p>
              </div>
            </div>
            <div class="p-4 border rounded-lg">
              <div class="text-center">
                <p class="text-2xl font-bold text-green-600">R {{ vatRefund.toLocaleString() }}</p>
                <p class="text-sm font-medium">VAT Refund Expected</p>
                <p class="text-xs text-muted-foreground">From previous period</p>
              </div>
            </div>
            <div class="p-4 border rounded-lg">
              <div class="text-center">
                <p class="text-2xl font-bold text-purple-600">R {{ incomeTax.toLocaleString() }}</p>
                <p class="text-sm font-medium">Income Tax Estimate</p>
                <p class="text-xs text-muted-foreground">Annual estimate</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </AppLayout>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { 
  Download,
  Calendar,
  TrendingUp,
  TrendingDown,
  DollarSign,
  Percent,
  Filter,
  BarChart3,
  Scale,
  ArrowUpDown,
  Calculator,
  Eye,
  FileText
} from 'lucide-vue-next'

// Quick stats
const monthlyProfit = ref(38640)
const totalRevenue = ref(84320)
const totalExpenses = ref(45680)
const profitMargin = ref(22.1)

// Filter states
const selectedPeriod = ref('current_month')
const selectedReportType = ref('all')

// Profit & Loss data
const profitLossData = ref({
  revenue: [
    { name: 'Product Sales', amount: 76850 },
    { name: 'Service Revenue', amount: 5230 },
    { name: 'Delivery Charges', amount: 2240 }
  ],
  expenses: [
    { name: 'Cost of Goods Sold', amount: 45680 },
    { name: 'Rent Expense', amount: 3500 },
    { name: 'Utilities', amount: 890 },
    { name: 'Transport Costs', amount: 1250 },
    { name: 'Marketing Expense', amount: 650 }
  ],
  totalRevenue: 84320,
  totalExpenses: 51970
})

// Balance Sheet data
const balanceSheetData = ref({
  assets: [
    { name: 'Cash & Bank', amount: 29130 },
    { name: 'Accounts Receivable', amount: 15680 },
    { name: 'Inventory', amount: 89420 },
    { name: 'Equipment', amount: 22550 }
  ],
  liabilities: [
    { name: 'Accounts Payable', amount: 8900 },
    { name: 'VAT Payable', amount: 3420 },
    { name: 'Loans', amount: 17670 }
  ],
  totalAssets: 156780,
  totalLiabilities: 29990
})

// Cash Flow data
const cashFlowData = ref({
  operating: [
    { name: 'Net Income', amount: 32350 },
    { name: 'Accounts Receivable Change', amount: -2340 },
    { name: 'Inventory Change', amount: -5600 },
    { name: 'Accounts Payable Change', amount: 1890 }
  ],
  investing: [
    { name: 'Equipment Purchase', amount: -8500 },
    { name: 'Asset Sale', amount: 2100 }
  ],
  netOperating: 26300,
  netInvesting: -6400
})

// Financial ratios
const financialRatios = ref([
  { name: 'Current Ratio', value: '5.2', unit: ':1', benchmark: 'Good: >2.0', status: 'good' },
  { name: 'Gross Margin', value: '45.8', unit: '%', benchmark: 'Target: >40%', status: 'good' },
  { name: 'ROI', value: '28.4', unit: '%', benchmark: 'Excellent: >20%', status: 'good' },
  { name: 'Debt Ratio', value: '19.1', unit: '%', benchmark: 'Low: <30%', status: 'good' }
])

// Tax data
const vatPayable = ref(3420)
const vatRefund = ref(890)
const incomeTax = ref(12500)
</script>