<script setup lang="ts">
definePageMeta({
  middleware: 'auth',
  layout: 'default'
})

// Simple view for township businesses - Money In / Money Out / What's Left
const stats = ref([
  { label: 'Money In (This Month)', value: 'R50,000', icon: 'i-heroicons-arrow-trending-up', color: 'success' },
  { label: 'Money Out (This Month)', value: 'R40,000', icon: 'i-heroicons-arrow-trending-down', color: 'error' },
  { label: "What's Left (Profit)", value: 'R10,000', icon: 'i-heroicons-banknotes', color: 'primary' },
  { label: 'Cash Balance', value: 'R8,500', icon: 'i-heroicons-wallet', color: 'info' }
])

const unpaidInvoices = ref([
  { customer: 'Thabo M.', amount: 800, daysOverdue: 5, invoice: 'INV-045' },
  { customer: 'Sarah K.', amount: 400, daysOverdue: 12, invoice: 'INV-042' }
])

const upcomingBills = ref([
  { supplier: 'Main Supplier', amount: 5000, dueDate: '2025-12-30', invoice: 'SUPP-123' },
  { supplier: 'Rent (Landlord)', amount: 2500, dueDate: '2026-01-05', invoice: 'RENT-JAN' }
])

const recentTransactions = ref([
  { date: '2025-12-23', description: 'Cash Sale - POS', type: 'income', amount: 1250 },
  { date: '2025-12-23', description: 'Paid Electricity Bill', type: 'expense', amount: 500 },
  { date: '2025-12-22', description: 'Customer Payment - Thabo', type: 'income', amount: 300 },
  { date: '2025-12-22', description: 'Fuel for Generator', type: 'expense', amount: 200 }
])
</script>

<template>
  <div class="p-4 md:p-6">
    <div class="mb-6">
      <h1 class="text-2xl font-bold mb-2">Money (Finance)</h1>
      <p class="text-muted-foreground">Track your income and expenses</p>
    </div>

    <!-- Stats Grid - Simplified View -->
    <div class="grid grid-cols-2 md:grid-cols-4 gap-4 mb-6">
      <UCard v-for="stat in stats" :key="stat.label" class="hover:shadow-md transition-shadow">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-muted-foreground">{{ stat.label }}</p>
            <p class="text-2xl font-bold mt-1">{{ stat.value }}</p>
          </div>
          <div :class="`p-3 rounded-lg bg-${stat.color}/10`">
            <UIcon :name="stat.icon" class="w-6 h-6" :class="`text-${stat.color}`" />
          </div>
        </div>
      </UCard>
    </div>

    <!-- Quick Actions -->
    <div class="mb-6 flex flex-wrap gap-3">
      <UButton size="lg">
        <UIcon name="i-heroicons-plus" class="mr-2" />
        Record Expense
      </UButton>
      <UButton to="/accounting/invoices" size="lg" variant="outline">
        <UIcon name="i-heroicons-document-text" class="mr-2" />
        Invoices
      </UButton>
      <UButton to="/accounting/payments" size="lg" variant="outline">
        <UIcon name="i-heroicons-banknotes" class="mr-2" />
        Payments
      </UButton>
      <UButton to="/accounting/reports" size="lg" variant="outline">
        <UIcon name="i-heroicons-chart-bar" class="mr-2" />
        Reports
      </UButton>
    </div>

    <!-- Unpaid Invoices (Owed to You) -->
    <UCard class="mb-6">
      <template #header>
        <div class="flex items-center justify-between">
          <h2 class="text-lg font-semibold">People Who Owe You (Book/Credit)</h2>
          <UBadge color="warning">{{ unpaidInvoices.length }}</UBadge>
        </div>
      </template>

      <div class="space-y-3">
        <div v-for="item in unpaidInvoices" :key="item.invoice" class="p-3 border rounded-lg hover:bg-accent/50 transition-colors">
          <div class="flex items-center justify-between">
            <div>
              <p class="font-semibold">{{ item.customer }}</p>
              <p class="text-sm text-muted-foreground">{{ item.invoice }} • {{ item.daysOverdue }} days overdue</p>
            </div>
            <div class="text-right">
              <p class="font-bold text-lg text-warning">R{{ item.amount }}</p>
              <UButton size="xs" variant="ghost">Record Payment</UButton>
            </div>
          </div>
        </div>
      </div>
    </UCard>

    <!-- Upcoming Bills (What You Owe) -->
    <UCard class="mb-6">
      <template #header>
        <div class="flex items-center justify-between">
          <h2 class="text-lg font-semibold">Bills You Need to Pay</h2>
          <UBadge color="info">{{ upcomingBills.length }}</UBadge>
        </div>
      </template>

      <div class="space-y-3">
        <div v-for="bill in upcomingBills" :key="bill.invoice" class="p-3 border rounded-lg hover:bg-accent/50 transition-colors">
          <div class="flex items-center justify-between">
            <div>
              <p class="font-semibold">{{ bill.supplier }}</p>
              <p class="text-sm text-muted-foreground">{{ bill.invoice }} • Due: {{ new Date(bill.dueDate).toLocaleDateString() }}</p>
            </div>
            <div class="text-right">
              <p class="font-bold text-lg text-error">R{{ bill.amount }}</p>
              <UButton size="xs" variant="ghost">Pay Now</UButton>
            </div>
          </div>
        </div>
      </div>
    </UCard>

    <!-- Recent Transactions -->
    <UCard>
      <template #header>
        <h2 class="text-lg font-semibold">Recent Money Movement</h2>
      </template>

      <div class="space-y-2">
        <div v-for="(tx, index) in recentTransactions" :key="index" class="flex items-center justify-between p-3 border-b last:border-0">
          <div class="flex items-center gap-3">
            <div :class="`p-2 rounded-lg ${tx.type === 'income' ? 'bg-success/10' : 'bg-error/10'}`">
              <UIcon :name="tx.type === 'income' ? 'i-heroicons-arrow-down-left' : 'i-heroicons-arrow-up-right'" 
                     :class="`w-4 h-4 ${tx.type === 'income' ? 'text-success' : 'text-error'}`" />
            </div>
            <div>
              <p class="font-medium">{{ tx.description }}</p>
              <p class="text-xs text-muted-foreground">{{ new Date(tx.date).toLocaleDateString() }}</p>
            </div>
          </div>
          <p :class="`font-bold ${tx.type === 'income' ? 'text-success' : 'text-error'}`">
            {{ tx.type === 'income' ? '+' : '-' }}R{{ tx.amount }}
          </p>
        </div>
      </div>
    </UCard>
  </div>
</template>
