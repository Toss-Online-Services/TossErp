<template>
  <AppLayout>
    <div class="p-6 space-y-6">
      <!-- Header -->
      <div class="flex justify-between items-center">
        <div>
          <h1 class="text-2xl font-bold">Financial Accounts</h1>
          <p class="text-muted-foreground">Manage your business accounts and financial records</p>
        </div>
        <div class="flex items-center space-x-4">
          <Button variant="outline">
            <Download class="h-4 w-4 mr-2" />
            Export
          </Button>
          <Button>
            <Plus class="h-4 w-4 mr-2" />
            Add Account
          </Button>
        </div>
      </div>

      <!-- Financial Overview -->
      <div class="grid grid-cols-1 md:grid-cols-4 gap-6">
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg border">
          <div class="flex items-center space-x-3">
            <div class="p-2 bg-green-100 dark:bg-green-900 rounded-lg">
              <Wallet class="h-6 w-6 text-green-600" />
            </div>
            <div>
              <p class="text-sm text-muted-foreground">Total Assets</p>
              <p class="text-2xl font-bold">R {{ totalAssets.toLocaleString() }}</p>
              <p class="text-xs text-green-600">+5.2% this month</p>
            </div>
          </div>
        </div>
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg border">
          <div class="flex items-center space-x-3">
            <div class="p-2 bg-blue-100 dark:bg-blue-900 rounded-lg">
              <Banknote class="h-6 w-6 text-blue-600" />
            </div>
            <div>
              <p class="text-sm text-muted-foreground">Cash on Hand</p>
              <p class="text-2xl font-bold">R {{ cashOnHand.toLocaleString() }}</p>
              <p class="text-xs text-blue-600">Available now</p>
            </div>
          </div>
        </div>
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg border">
          <div class="flex items-center space-x-3">
            <div class="p-2 bg-red-100 dark:bg-red-900 rounded-lg">
              <CreditCard class="h-6 w-6 text-red-600" />
            </div>
            <div>
              <p class="text-sm text-muted-foreground">Outstanding Credit</p>
              <p class="text-2xl font-bold">R {{ outstandingCredit.toLocaleString() }}</p>
              <p class="text-xs text-red-600">Customer receivables</p>
            </div>
          </div>
        </div>
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg border">
          <div class="flex items-center space-x-3">
            <div class="p-2 bg-purple-100 dark:bg-purple-900 rounded-lg">
              <TrendingUp class="h-6 w-6 text-purple-600" />
            </div>
            <div>
              <p class="text-sm text-muted-foreground">Monthly Revenue</p>
              <p class="text-2xl font-bold">R {{ monthlyRevenue.toLocaleString() }}</p>
              <p class="text-xs text-purple-600">This month</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Account Categories -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <!-- Assets -->
        <div class="bg-white dark:bg-gray-800 rounded-lg border">
          <div class="p-6 border-b">
            <div class="flex items-center space-x-2">
              <TrendingUp class="h-5 w-5 text-green-600" />
              <h3 class="text-lg font-semibold">Assets</h3>
            </div>
          </div>
          <div class="p-6">
            <div class="space-y-4">
              <div v-for="account in assetAccounts" :key="account.id" class="flex items-center justify-between p-3 border rounded-lg hover:bg-muted/50">
                <div class="flex items-center space-x-3">
                  <div class="w-10 h-10 bg-green-100 dark:bg-green-900 rounded-lg flex items-center justify-center">
                    <component :is="account.icon" class="h-5 w-5 text-green-600" />
                  </div>
                  <div>
                    <p class="font-medium">{{ account.name }}</p>
                    <p class="text-sm text-muted-foreground">{{ account.type }}</p>
                  </div>
                </div>
                <div class="text-right">
                  <p class="font-bold text-green-600">R {{ account.balance.toLocaleString() }}</p>
                  <p class="text-xs text-muted-foreground">{{ account.lastUpdated }}</p>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Liabilities -->
        <div class="bg-white dark:bg-gray-800 rounded-lg border">
          <div class="p-6 border-b">
            <div class="flex items-center space-x-2">
              <TrendingDown class="h-5 w-5 text-red-600" />
              <h3 class="text-lg font-semibold">Liabilities</h3>
            </div>
          </div>
          <div class="p-6">
            <div class="space-y-4">
              <div v-for="account in liabilityAccounts" :key="account.id" class="flex items-center justify-between p-3 border rounded-lg hover:bg-muted/50">
                <div class="flex items-center space-x-3">
                  <div class="w-10 h-10 bg-red-100 dark:bg-red-900 rounded-lg flex items-center justify-center">
                    <component :is="account.icon" class="h-5 w-5 text-red-600" />
                  </div>
                  <div>
                    <p class="font-medium">{{ account.name }}</p>
                    <p class="text-sm text-muted-foreground">{{ account.type }}</p>
                  </div>
                </div>
                <div class="text-right">
                  <p class="font-bold text-red-600">R {{ account.balance.toLocaleString() }}</p>
                  <p class="text-xs text-muted-foreground">{{ account.lastUpdated }}</p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Revenue & Expenses -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <!-- Revenue Accounts -->
        <div class="bg-white dark:bg-gray-800 rounded-lg border">
          <div class="p-6 border-b">
            <div class="flex items-center space-x-2">
              <DollarSign class="h-5 w-5 text-blue-600" />
              <h3 class="text-lg font-semibold">Revenue Accounts</h3>
            </div>
          </div>
          <div class="p-6">
            <div class="space-y-4">
              <div v-for="account in revenueAccounts" :key="account.id" class="flex items-center justify-between p-3 border rounded-lg hover:bg-muted/50">
                <div class="flex items-center space-x-3">
                  <div class="w-10 h-10 bg-blue-100 dark:bg-blue-900 rounded-lg flex items-center justify-center">
                    <component :is="account.icon" class="h-5 w-5 text-blue-600" />
                  </div>
                  <div>
                    <p class="font-medium">{{ account.name }}</p>
                    <p class="text-sm text-muted-foreground">{{ account.type }}</p>
                  </div>
                </div>
                <div class="text-right">
                  <p class="font-bold text-blue-600">R {{ account.balance.toLocaleString() }}</p>
                  <p class="text-xs text-muted-foreground">This month</p>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Expense Accounts -->
        <div class="bg-white dark:bg-gray-800 rounded-lg border">
          <div class="p-6 border-b">
            <div class="flex items-center space-x-2">
              <Minus class="h-5 w-5 text-orange-600" />
              <h3 class="text-lg font-semibold">Expense Accounts</h3>
            </div>
          </div>
          <div class="p-6">
            <div class="space-y-4">
              <div v-for="account in expenseAccounts" :key="account.id" class="flex items-center justify-between p-3 border rounded-lg hover:bg-muted/50">
                <div class="flex items-center space-x-3">
                  <div class="w-10 h-10 bg-orange-100 dark:bg-orange-900 rounded-lg flex items-center justify-center">
                    <component :is="account.icon" class="h-5 w-5 text-orange-600" />
                  </div>
                  <div>
                    <p class="font-medium">{{ account.name }}</p>
                    <p class="text-sm text-muted-foreground">{{ account.type }}</p>
                  </div>
                </div>
                <div class="text-right">
                  <p class="font-bold text-orange-600">R {{ account.balance.toLocaleString() }}</p>
                  <p class="text-xs text-muted-foreground">This month</p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Recent Transactions -->
      <div class="bg-white dark:bg-gray-800 rounded-lg border">
        <div class="p-6 border-b">
          <div class="flex justify-between items-center">
            <h3 class="text-lg font-semibold">Recent Transactions</h3>
            <NuxtLink to="/finance/transactions" class="text-primary hover:underline text-sm">View All</NuxtLink>
          </div>
        </div>
        <div class="overflow-x-auto">
          <table class="w-full">
            <thead>
              <tr class="border-b">
                <th class="text-left p-4 font-medium">Date</th>
                <th class="text-left p-4 font-medium">Description</th>
                <th class="text-left p-4 font-medium">Account</th>
                <th class="text-left p-4 font-medium">Amount</th>
                <th class="text-left p-4 font-medium">Type</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="transaction in recentTransactions" :key="transaction.id" class="border-b hover:bg-muted/50">
                <td class="p-4 text-muted-foreground">{{ transaction.date }}</td>
                <td class="p-4">{{ transaction.description }}</td>
                <td class="p-4 text-muted-foreground">{{ transaction.account }}</td>
                <td class="p-4">
                  <span :class="transaction.type === 'credit' ? 'text-green-600' : 'text-red-600'" class="font-medium">
                    {{ transaction.type === 'credit' ? '+' : '-' }}R {{ transaction.amount.toLocaleString() }}
                  </span>
                </td>
                <td class="p-4">
                  <span class="px-2 py-1 rounded text-xs" :class="transaction.type === 'credit' ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'">
                    {{ transaction.type }}
                  </span>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <!-- Cash Flow Chart -->
      <div class="bg-white dark:bg-gray-800 rounded-lg border">
        <div class="p-6 border-b">
          <h3 class="text-lg font-semibold">Cash Flow Trend</h3>
        </div>
        <div class="p-6">
          <div class="h-64 flex items-center justify-center border rounded-lg bg-gray-50 dark:bg-gray-700">
            <p class="text-muted-foreground">Cash flow chart would go here</p>
          </div>
        </div>
      </div>
    </div>
  </AppLayout>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { 
  Plus,
  Download,
  Wallet,
  Banknote,
  CreditCard,
  TrendingUp,
  TrendingDown,
  DollarSign,
  Minus,
  Building,
  Package,
  Truck,
  Receipt,
  Zap,
  Home,
  ShoppingCart
} from 'lucide-vue-next'

// Financial overview stats
const totalAssets = ref(156780)
const cashOnHand = ref(23450)
const outstandingCredit = ref(15680)
const monthlyRevenue = ref(84320)

// Asset accounts
const assetAccounts = ref([
  {
    id: 1,
    name: 'Business Bank Account',
    type: 'Current Asset',
    balance: 23450,
    icon: Building,
    lastUpdated: 'Today'
  },
  {
    id: 2,
    name: 'Cash on Hand',
    type: 'Current Asset',
    balance: 5680,
    icon: Wallet,
    lastUpdated: 'Today'
  },
  {
    id: 3,
    name: 'Inventory',
    type: 'Current Asset',
    balance: 89420,
    icon: Package,
    lastUpdated: 'Yesterday'
  },
  {
    id: 4,
    name: 'Accounts Receivable',
    type: 'Current Asset',
    balance: 15680,
    icon: Receipt,
    lastUpdated: '2 days ago'
  },
  {
    id: 5,
    name: 'Equipment & Fixtures',
    type: 'Fixed Asset',
    balance: 22550,
    icon: Home,
    lastUpdated: 'Last week'
  }
])

// Liability accounts
const liabilityAccounts = ref([
  {
    id: 1,
    name: 'Accounts Payable',
    type: 'Current Liability',
    balance: 8900,
    icon: CreditCard,
    lastUpdated: 'Today'
  },
  {
    id: 2,
    name: 'VAT Payable',
    type: 'Current Liability',
    balance: 3420,
    icon: Receipt,
    lastUpdated: 'Yesterday'
  },
  {
    id: 3,
    name: 'Equipment Loan',
    type: 'Long-term Liability',
    balance: 12000,
    icon: Building,
    lastUpdated: 'Last month'
  },
  {
    id: 4,
    name: 'Supplier Credit',
    type: 'Current Liability',
    balance: 5670,
    icon: Truck,
    lastUpdated: '3 days ago'
  }
])

// Revenue accounts
const revenueAccounts = ref([
  {
    id: 1,
    name: 'Product Sales',
    type: 'Revenue',
    balance: 76850,
    icon: ShoppingCart,
    lastUpdated: 'Today'
  },
  {
    id: 2,
    name: 'Service Revenue',
    type: 'Revenue',
    balance: 5230,
    icon: Zap,
    lastUpdated: 'Yesterday'
  },
  {
    id: 3,
    name: 'Delivery Charges',
    type: 'Revenue',
    balance: 2240,
    icon: Truck,
    lastUpdated: 'Today'
  }
])

// Expense accounts
const expenseAccounts = ref([
  {
    id: 1,
    name: 'Cost of Goods Sold',
    type: 'Direct Expense',
    balance: 45680,
    icon: Package,
    lastUpdated: 'Today'
  },
  {
    id: 2,
    name: 'Rent Expense',
    type: 'Operating Expense',
    balance: 3500,
    icon: Home,
    lastUpdated: 'This month'
  },
  {
    id: 3,
    name: 'Utilities',
    type: 'Operating Expense',
    balance: 890,
    icon: Zap,
    lastUpdated: 'Last week'
  },
  {
    id: 4,
    name: 'Transport Costs',
    type: 'Operating Expense',
    balance: 1250,
    icon: Truck,
    lastUpdated: 'Yesterday'
  },
  {
    id: 5,
    name: 'Marketing Expense',
    type: 'Operating Expense',
    balance: 650,
    icon: Receipt,
    lastUpdated: 'This week'
  }
])

// Recent transactions
const recentTransactions = ref([
  {
    id: 1,
    date: '2024-01-15',
    description: 'Daily sales deposit',
    account: 'Business Bank Account',
    amount: 2450,
    type: 'credit'
  },
  {
    id: 2,
    date: '2024-01-15',
    description: 'Supplier payment - Fresh Produce',
    account: 'Accounts Payable',
    amount: 1800,
    type: 'debit'
  },
  {
    id: 3,
    date: '2024-01-14',
    description: 'Rent payment',
    account: 'Rent Expense',
    amount: 3500,
    type: 'debit'
  },
  {
    id: 4,
    date: '2024-01-14',
    description: 'Customer payment - Credit sale',
    account: 'Accounts Receivable',
    amount: 890,
    type: 'credit'
  },
  {
    id: 5,
    date: '2024-01-13',
    description: 'Utilities bill payment',
    account: 'Utilities',
    amount: 220,
    type: 'debit'
  }
])
</script>