<script setup lang="ts">
definePageMeta({
  middleware: 'auth',
  layout: 'default'
})

const payments = ref([
  { id: 'PAY-101', date: '2025-12-23', type: 'received', party: 'Thabo M.', amount: 300, method: 'cash', invoice: 'INV-045' },
  { id: 'PAY-102', date: '2025-12-23', type: 'made', party: 'Main Supplier', amount: 5000, method: 'bank', invoice: 'SUPP-123' },
  { id: 'PAY-103', date: '2025-12-22', type: 'received', party: 'Sarah K.', amount: 200, method: 'mobile_money', invoice: 'INV-042' },
  { id: 'PAY-104', date: '2025-12-21', type: 'made', party: 'Electricity Company', amount: 500, method: 'bank', invoice: '-' }
])

const columns = [
  { key: 'id', label: 'Payment #' },
  { key: 'date', label: 'Date' },
  { key: 'type', label: 'Type' },
  { key: 'party', label: 'Customer/Supplier' },
  { key: 'amount', label: 'Amount' },
  { key: 'method', label: 'Method' },
  { key: 'invoice', label: 'Invoice' },
  { key: 'actions', label: 'Actions' }
]

const methodLabels = {
  cash: 'Cash',
  bank: 'Bank Transfer',
  mobile_money: 'Mobile Money',
  card: 'Card'
}
</script>

<template>
  <div class="p-4 md:p-6">
    <div class="mb-6 flex items-center justify-between">
      <div>
        <h1 class="text-2xl font-bold mb-2">Payments</h1>
        <p class="text-muted-foreground">Track money received and paid</p>
      </div>
      <div class="flex gap-2">
        <UButton>
          <UIcon name="i-heroicons-arrow-down-tray" class="mr-2" />
          Receive Payment
        </UButton>
        <UButton variant="outline">
          <UIcon name="i-heroicons-arrow-up-tray" class="mr-2" />
          Make Payment
        </UButton>
      </div>
    </div>

    <!-- Filters -->
    <div class="mb-4 flex gap-2">
      <UButton size="sm" variant="outline">All</UButton>
      <UButton size="sm" variant="ghost">Received</UButton>
      <UButton size="sm" variant="ghost">Made</UButton>
      <UButton size="sm" variant="ghost">Cash</UButton>
      <UButton size="sm" variant="ghost">Bank</UButton>
    </div>

    <!-- Payments Table -->
    <UCard>
      <UTable :rows="payments" :columns="columns">
        <template #date-data="{ row }">
          {{ new Date(row.date).toLocaleDateString() }}
        </template>

        <template #type-data="{ row }">
          <UBadge :color="row.type === 'received' ? 'success' : 'info'">
            {{ row.type }}
          </UBadge>
        </template>

        <template #amount-data="{ row }">
          <span :class="`font-bold ${row.type === 'received' ? 'text-success' : 'text-error'}`">
            {{ row.type === 'received' ? '+' : '-' }}R{{ row.amount }}
          </span>
        </template>

        <template #method-data="{ row }">
          {{ methodLabels[row.method] }}
        </template>

        <template #actions-data="{ row }">
          <div class="flex gap-2">
            <UButton size="xs" variant="ghost">View</UButton>
            <UButton size="xs" variant="ghost">Print Receipt</UButton>
          </div>
        </template>
      </UTable>
    </UCard>
  </div>
</template>
