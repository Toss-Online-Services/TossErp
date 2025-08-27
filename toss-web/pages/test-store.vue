<template>
  <div class="p-4">
    <h1 class="text-2xl font-bold mb-4">Store Test</h1>
    <div v-if="loading">Loading...</div>
    <div v-else>
      <h2 class="text-lg font-semibold mb-2">Customer Stats:</h2>
      <pre>{{ JSON.stringify(customerStats, null, 2) }}</pre>
      
      <h2 class="text-lg font-semibold mb-2 mt-4">Customers ({{ customers.length }}):</h2>
      <div v-for="customer in customers" :key="customer.id" class="border p-2 mb-2">
        <p><strong>{{ customer.fullName }}</strong> - {{ customer.email }}</p>
        <p>Status: {{ customer.status }} | Segment: {{ customer.segment }}</p>
        <p>Total Spent: R {{ customer.totalSpent }}</p>
      </div>
    </div>
  </div>
</template>

<script setup>
import { useCustomerStore } from '~/stores/customers'
import { storeToRefs } from 'pinia'

const customersStore = useCustomerStore()
const { customers, loading, customerStats } = storeToRefs(customersStore)

// Test fetch on mount
onMounted(async () => {
  try {
    await customersStore.fetchCustomers()
  } catch (error) {
    console.error('Failed to fetch customers:', error)
  }
})
</script>
