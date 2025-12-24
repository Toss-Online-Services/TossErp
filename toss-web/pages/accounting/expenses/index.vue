<script setup lang="ts">
definePageMeta({
  middleware: 'auth',
  layout: 'default'
})

const expenses = ref([
  { id: 'EXP-025', date: '2025-12-23', category: 'Utilities/Fuel', description: 'Generator Diesel', amount: 200, paidBy: 'Cash' },
  { id: 'EXP-026', date: '2025-12-23', category: 'Electricity', description: 'Monthly Electricity Bill', amount: 500, paidBy: 'Bank' },
  { id: 'EXP-024', date: '2025-12-22', category: 'Transport', description: 'Delivery Fuel', amount: 150, paidBy: 'Cash' },
  { id: 'EXP-023', date: '2025-12-20', category: 'Wages', description: 'Staff Wages - Week', amount: 800, paidBy: 'Cash' }
])

const columns = [
  { key: 'id', label: 'ID' },
  { key: 'date', label: 'Date' },
  { key: 'category', label: 'Category' },
  { key: 'description', label: 'Description' },
  { key: 'amount', label: 'Amount' },
  { key: 'paidBy', label: 'Paid By' },
  { key: 'actions', label: 'Actions' }
]

const categories = ['Utilities/Fuel', 'Electricity', 'Transport', 'Wages', 'Rent', 'Supplies', 'Maintenance', 'Other']
</script>

<template>
  <div class="p-4 md:p-6">
    <div class="mb-6 flex items-center justify-between">
      <div>
        <h1 class="text-2xl font-bold mb-2">Expenses</h1>
        <p class="text-muted-foreground">Record business expenses and petty cash</p>
      </div>
      <UButton>
        <UIcon name="i-heroicons-plus" class="mr-2" />
        Record Expense
      </UButton>
    </div>

    <!-- Category Filters -->
    <div class="mb-4 flex gap-2 flex-wrap">
      <UButton size="sm" variant="outline">All</UButton>
      <UButton v-for="cat in categories.slice(0, 5)" :key="cat" size="sm" variant="ghost">{{ cat }}</UButton>
    </div>

    <!-- Expenses Table -->
    <UCard>
      <UTable :rows="expenses" :columns="columns">
        <template #date-data="{ row }">
          {{ new Date(row.date).toLocaleDateString() }}
        </template>

        <template #amount-data="{ row }">
          <span class="font-bold text-error">R{{ row.amount }}</span>
        </template>

        <template #actions-data="{ row }">
          <div class="flex gap-2">
            <UButton size="xs" variant="ghost">View</UButton>
            <UButton size="xs" variant="ghost">Edit</UButton>
          </div>
        </template>
      </UTable>
    </UCard>
  </div>
</template>
