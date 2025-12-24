<script setup lang="ts">
definePageMeta({
  middleware: 'auth',
  layout: 'default'
})

const invoices = ref([
  { id: 'INV-045', customer: 'Thabo M.', date: '2025-12-18', dueDate: '2025-12-28', amount: 800, paid: 0, status: 'overdue' },
  { id: 'INV-044', customer: 'Sarah K.', date: '2025-12-10', dueDate: '2025-12-20', amount: 400, paid: 0, status: 'overdue' },
  { id: 'INV-046', customer: 'Lucky T.', date: '2025-12-22', dueDate: '2026-01-05', amount: 1200, paid: 0, status: 'unpaid' },
  { id: 'INV-043', customer: 'Mary S.', date: '2025-12-15', dueDate: '2025-12-25', amount: 600, paid: 600, status: 'paid' }
])

const columns = [
  { key: 'id', label: 'Invoice #' },
  { key: 'customer', label: 'Customer' },
  { key: 'date', label: 'Date' },
  { key: 'dueDate', label: 'Due Date' },
  { key: 'amount', label: 'Amount' },
  { key: 'paid', label: 'Paid' },
  { key: 'status', label: 'Status' },
  { key: 'actions', label: 'Actions' }
]

const statusColors = {
  paid: 'success',
  unpaid: 'info',
  overdue: 'error',
  partial: 'warning'
}
</script>

<template>
  <div class="p-4 md:p-6">
    <div class="mb-6 flex items-center justify-between">
      <div>
        <h1 class="text-2xl font-bold mb-2">Sales Invoices</h1>
        <p class="text-muted-foreground">Manage customer invoices and credit sales</p>
      </div>
      <UButton>
        <UIcon name="i-heroicons-plus" class="mr-2" />
        New Invoice
      </UButton>
    </div>

    <!-- Filters -->
    <div class="mb-4 flex gap-2">
      <UButton size="sm" variant="outline">All</UButton>
      <UButton size="sm" variant="ghost">Unpaid</UButton>
      <UButton size="sm" variant="ghost">Overdue</UButton>
      <UButton size="sm" variant="ghost">Paid</UButton>
    </div>

    <!-- Invoices Table -->
    <UCard>
      <UTable :rows="invoices" :columns="columns">
        <template #date-data="{ row }">
          {{ new Date(row.date).toLocaleDateString() }}
        </template>

        <template #dueDate-data="{ row }">
          <span :class="{ 'text-error font-medium': row.status === 'overdue' }">
            {{ new Date(row.dueDate).toLocaleDateString() }}
          </span>
        </template>

        <template #amount-data="{ row }">
          <span class="font-bold">R{{ row.amount }}</span>
        </template>

        <template #paid-data="{ row }">
          <span :class="row.paid < row.amount ? 'text-warning' : 'text-success'">
            R{{ row.paid }}
          </span>
        </template>

        <template #status-data="{ row }">
          <UBadge :color="statusColors[row.status]">
            {{ row.status }}
          </UBadge>
        </template>

        <template #actions-data="{ row }">
          <div class="flex gap-2">
            <UButton size="xs" variant="ghost">View</UButton>
            <UButton v-if="row.status !== 'paid'" size="xs" variant="ghost">Record Payment</UButton>
          </div>
        </template>
      </UTable>
    </UCard>
  </div>
</template>
