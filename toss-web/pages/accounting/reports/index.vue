<script setup lang="ts">
definePageMeta({
  middleware: 'auth',
  layout: 'default'
})

const reportType = ref('profit-loss')

// Profit & Loss (Income minus Expenses)
const profitLoss = ref({
  period: 'December 2025',
  income: {
    sales: 50000,
    otherIncome: 2000,
    total: 52000
  },
  expenses: {
    purchases: 30000,
    wages: 3200,
    utilities: 1500,
    transport: 800,
    rent: 2500,
    other: 2000,
    total: 40000
  },
  netProfit: 12000
})

// Balance Sheet (What You Have vs What You Owe)
const balanceSheet = ref({
  date: '31 December 2025',
  assets: {
    cash: 8500,
    bank: 12000,
    inventory: 25000,
    receivables: 1200,
    equipment: 35000,
    total: 81700
  },
  liabilities: {
    payables: 7500,
    loans: 15000,
    total: 22500
  },
  netWorth: 59200
})

// Cash Flow
const cashFlow = ref({
  period: 'December 2025',
  opening: 5000,
  cashIn: 48000,
  cashOut: 44500,
  closing: 8500
})
</script>

<template>
  <div class="p-4 md:p-6">
    <div class="mb-6">
      <h1 class="text-2xl font-bold mb-2">Financial Reports</h1>
      <p class="text-muted-foreground">View your business financial statements</p>
    </div>

    <!-- Report Type Selector -->
    <div class="mb-6 flex gap-2">
      <UButton :variant="reportType === 'profit-loss' ? 'solid' : 'outline'" @click="reportType = 'profit-loss'">
        Income vs Expenses
      </UButton>
      <UButton :variant="reportType === 'balance-sheet' ? 'solid' : 'outline'" @click="reportType = 'balance-sheet'">
        What You Have & Owe
      </UButton>
      <UButton :variant="reportType === 'cash-flow' ? 'solid' : 'outline'" @click="reportType = 'cash-flow'">
        Cash Flow
      </UButton>
    </div>

    <!-- Profit & Loss Report -->
    <UCard v-if="reportType === 'profit-loss'">
      <template #header>
        <div class="flex items-center justify-between">
          <h2 class="text-lg font-semibold">Income minus Expenses Report</h2>
          <p class="text-sm text-muted-foreground">{{ profitLoss.period }}</p>
        </div>
      </template>

      <div class="space-y-6">
        <!-- Income Section -->
        <div>
          <h3 class="font-semibold text-success mb-3">Money In (Income)</h3>
          <div class="space-y-2 pl-4">
            <div class="flex justify-between">
              <span>Sales</span>
              <span class="font-medium">R{{ profitLoss.income.sales.toLocaleString() }}</span>
            </div>
            <div class="flex justify-between">
              <span>Other Income</span>
              <span class="font-medium">R{{ profitLoss.income.otherIncome.toLocaleString() }}</span>
            </div>
            <div class="flex justify-between pt-2 border-t font-bold text-success">
              <span>Total Income</span>
              <span>R{{ profitLoss.income.total.toLocaleString() }}</span>
            </div>
          </div>
        </div>

        <!-- Expenses Section -->
        <div>
          <h3 class="font-semibold text-error mb-3">Money Out (Expenses)</h3>
          <div class="space-y-2 pl-4">
            <div class="flex justify-between">
              <span>Purchases</span>
              <span class="font-medium">R{{ profitLoss.expenses.purchases.toLocaleString() }}</span>
            </div>
            <div class="flex justify-between">
              <span>Wages</span>
              <span class="font-medium">R{{ profitLoss.expenses.wages.toLocaleString() }}</span>
            </div>
            <div class="flex justify-between">
              <span>Utilities</span>
              <span class="font-medium">R{{ profitLoss.expenses.utilities.toLocaleString() }}</span>
            </div>
            <div class="flex justify-between">
              <span>Transport</span>
              <span class="font-medium">R{{ profitLoss.expenses.transport.toLocaleString() }}</span>
            </div>
            <div class="flex justify-between">
              <span>Rent</span>
              <span class="font-medium">R{{ profitLoss.expenses.rent.toLocaleString() }}</span>
            </div>
            <div class="flex justify-between">
              <span>Other Expenses</span>
              <span class="font-medium">R{{ profitLoss.expenses.other.toLocaleString() }}</span>
            </div>
            <div class="flex justify-between pt-2 border-t font-bold text-error">
              <span>Total Expenses</span>
              <span>R{{ profitLoss.expenses.total.toLocaleString() }}</span>
            </div>
          </div>
        </div>

        <!-- Net Profit -->
        <div class="p-4 bg-primary/10 rounded-lg">
          <div class="flex justify-between items-center">
            <span class="text-lg font-bold">Net Profit (What's Left)</span>
            <span class="text-2xl font-bold text-primary">R{{ profitLoss.netProfit.toLocaleString() }}</span>
          </div>
        </div>
      </div>
    </UCard>

    <!-- Balance Sheet Report -->
    <UCard v-if="reportType === 'balance-sheet'">
      <template #header>
        <div class="flex items-center justify-between">
          <h2 class="text-lg font-semibold">What You Have & What You Owe</h2>
          <p class="text-sm text-muted-foreground">As of {{ balanceSheet.date }}</p>
        </div>
      </template>

      <div class="space-y-6">
        <!-- Assets Section -->
        <div>
          <h3 class="font-semibold text-success mb-3">What You Have (Assets)</h3>
          <div class="space-y-2 pl-4">
            <div class="flex justify-between">
              <span>Cash</span>
              <span class="font-medium">R{{ balanceSheet.assets.cash.toLocaleString() }}</span>
            </div>
            <div class="flex justify-between">
              <span>Bank</span>
              <span class="font-medium">R{{ balanceSheet.assets.bank.toLocaleString() }}</span>
            </div>
            <div class="flex justify-between">
              <span>Stock (Inventory)</span>
              <span class="font-medium">R{{ balanceSheet.assets.inventory.toLocaleString() }}</span>
            </div>
            <div class="flex justify-between">
              <span>People Owe You</span>
              <span class="font-medium">R{{ balanceSheet.assets.receivables.toLocaleString() }}</span>
            </div>
            <div class="flex justify-between">
              <span>Equipment</span>
              <span class="font-medium">R{{ balanceSheet.assets.equipment.toLocaleString() }}</span>
            </div>
            <div class="flex justify-between pt-2 border-t font-bold text-success">
              <span>Total Assets</span>
              <span>R{{ balanceSheet.assets.total.toLocaleString() }}</span>
            </div>
          </div>
        </div>

        <!-- Liabilities Section -->
        <div>
          <h3 class="font-semibold text-error mb-3">What You Owe (Liabilities)</h3>
          <div class="space-y-2 pl-4">
            <div class="flex justify-between">
              <span>Bills to Pay</span>
              <span class="font-medium">R{{ balanceSheet.liabilities.payables.toLocaleString() }}</span>
            </div>
            <div class="flex justify-between">
              <span>Loans</span>
              <span class="font-medium">R{{ balanceSheet.liabilities.loans.toLocaleString() }}</span>
            </div>
            <div class="flex justify-between pt-2 border-t font-bold text-error">
              <span>Total Liabilities</span>
              <span>R{{ balanceSheet.liabilities.total.toLocaleString() }}</span>
            </div>
          </div>
        </div>

        <!-- Net Worth -->
        <div class="p-4 bg-primary/10 rounded-lg">
          <div class="flex justify-between items-center">
            <span class="text-lg font-bold">Your Business Net Worth</span>
            <span class="text-2xl font-bold text-primary">R{{ balanceSheet.netWorth.toLocaleString() }}</span>
          </div>
        </div>
      </div>
    </UCard>

    <!-- Cash Flow Report -->
    <UCard v-if="reportType === 'cash-flow'">
      <template #header>
        <div class="flex items-center justify-between">
          <h2 class="text-lg font-semibold">Cash Flow</h2>
          <p class="text-sm text-muted-foreground">{{ cashFlow.period }}</p>
        </div>
      </template>

      <div class="space-y-4">
        <div class="flex justify-between p-3 bg-info/10 rounded">
          <span class="font-medium">Cash at Start of Month</span>
          <span class="font-bold">R{{ cashFlow.opening.toLocaleString() }}</span>
        </div>

        <div class="flex justify-between p-3 bg-success/10 rounded">
          <span class="font-medium">Money Received</span>
          <span class="font-bold text-success">+R{{ cashFlow.cashIn.toLocaleString() }}</span>
        </div>

        <div class="flex justify-between p-3 bg-error/10 rounded">
          <span class="font-medium">Money Paid Out</span>
          <span class="font-bold text-error">-R{{ cashFlow.cashOut.toLocaleString() }}</span>
        </div>

        <div class="flex justify-between p-4 bg-primary/20 rounded-lg">
          <span class="text-lg font-bold">Cash Now</span>
          <span class="text-2xl font-bold text-primary">R{{ cashFlow.closing.toLocaleString() }}</span>
        </div>
      </div>
    </UCard>
  </div>
</template>
