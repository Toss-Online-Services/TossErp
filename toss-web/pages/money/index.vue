<script setup lang="ts">
import { ref } from 'vue'
import { ArrowUpCircle, ArrowDownCircle } from 'lucide-vue-next'
import AppCard from '~/components/common/AppCard.vue'
import AppTable from '~/components/common/AppTable.vue'
import AppSectionHeader from '~/components/common/AppSectionHeader.vue'
import AppKpiCard from '~/components/common/AppKpiCard.vue'

useHead({
  title: 'Money - TOSS Admin',
  meta: [{ name: 'description', content: 'Financial management for TOSS Admin' }]
})

import { computed } from 'vue'

const cashIn = ref(15200)
const cashOut = ref(3200)
const balance = computed(() => cashIn.value - cashOut.value)

definePageMeta({
  layout: 'dashboard'
})

const transactions = ref([
  { id: 1, type: 'Sale', description: 'POS Sale #1234', amount: 'R245.50', date: '2024-01-15', category: 'Income' },
  { id: 2, type: 'Purchase', description: 'Stock from Supplier A', amount: '-R1,250.00', date: '2024-01-15', category: 'Expense' },
  { id: 3, type: 'Sale', description: 'POS Sale #1233', amount: 'R89.00', date: '2024-01-15', category: 'Income' },
  { id: 4, type: 'Expense', description: 'Delivery fee', amount: '-R150.00', date: '2024-01-14', category: 'Expense' }
])
</script>

<template>
  <div class="space-y-6">
    <AppSectionHeader
      title="Money & Cashbook"
      description="Track money in, money out, and what's left"
    />

    <!-- Summary Cards -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
      <AppKpiCard
        title="Money In"
        :value="`R${cashIn.toLocaleString()}`"
        change="+8.3%"
        change-type="positive"
        :icon="ArrowUpCircle"
      />
      <AppKpiCard
        title="Money Out"
        :value="`R${cashOut.toLocaleString()}`"
        change="-5.2%"
        change-type="negative"
        :icon="ArrowDownCircle"
      />
      <AppKpiCard
        title="What's Left"
        :value="`R${balance.toLocaleString()}`"
        change="+12.5%"
        change-type="positive"
      />
    </div>

    <!-- Transactions Table -->
    <AppCard title="Transactions">
      <div class="mb-4 flex items-center justify-between">
        <input
          type="text"
          placeholder="Search transactions..."
          class="px-3 py-2 border border-border rounded-lg text-sm w-64"
        />
        <select class="px-3 py-2 border border-border rounded-lg text-sm">
          <option>All Categories</option>
          <option>Income</option>
          <option>Expense</option>
        </select>
      </div>
      <AppTable :headers="['Type', 'Description', 'Amount', 'Category', 'Date']">
        <tr
          v-for="transaction in transactions"
          :key="transaction.id"
          class="hover:bg-muted/50 transition-colors"
        >
          <td class="px-4 py-3">
            <span
              :class="[
                'inline-flex items-center px-2 py-1 rounded text-xs font-medium',
                transaction.type === 'Sale' ? 'bg-green-100 dark:bg-green-900/20 text-green-700 dark:text-green-400' : 'bg-red-100 dark:bg-red-900/20 text-red-700 dark:text-red-400'
              ]"
            >
              {{ transaction.type }}
            </span>
          </td>
          <td class="px-4 py-3 text-sm text-foreground">{{ transaction.description }}</td>
          <td
            :class="[
              'px-4 py-3 text-sm font-medium',
              transaction.amount.startsWith('-') ? 'text-red-600 dark:text-red-400' : 'text-green-600 dark:text-green-400'
            ]"
          >
            {{ transaction.amount }}
          </td>
          <td class="px-4 py-3 text-sm text-muted-foreground">{{ transaction.category }}</td>
          <td class="px-4 py-3 text-sm text-muted-foreground">{{ transaction.date }}</td>
        </tr>
      </AppTable>
    </AppCard>
  </div>
</template>

