<script setup lang="ts">
import { ref } from 'vue'
import { User } from 'lucide-vue-next'
import AppCard from '~/components/common/AppCard.vue'
import AppTable from '~/components/common/AppTable.vue'
import AppSectionHeader from '~/components/common/AppSectionHeader.vue'
import AppDrawer from '~/components/common/AppDrawer.vue'

useHead({
  title: 'Customers - TOSS Admin',
  meta: [{ name: 'description', content: 'Customer management for TOSS Admin' }]
})

definePageMeta({
  layout: 'dashboard'
})

const customers = ref([
  { id: 1, name: 'John Doe', phone: '+27 82 123 4567', email: 'john@example.com', orders: 12, total: 'R2,450.00' },
  { id: 2, name: 'Jane Smith', phone: '+27 83 234 5678', email: 'jane@example.com', orders: 8, total: 'R1,890.00' },
  { id: 3, name: 'Mike Johnson', phone: '+27 84 345 6789', email: 'mike@example.com', orders: 15, total: 'R3,250.00' }
])

const selectedCustomer = ref(null)
const drawerOpen = ref(false)

const openCustomerDetails = (customer: any) => {
  selectedCustomer.value = customer
  drawerOpen.value = true
}
</script>

<template>
  <div class="space-y-6">
    <AppSectionHeader
      title="Customers"
      description="Manage customer relationships"
    />

    <AppCard>
      <div class="mb-4 flex items-center justify-between">
        <input
          type="text"
          placeholder="Search customers..."
          class="px-3 py-2 border border-border rounded-lg text-sm w-64"
        />
      </div>
      <AppTable :headers="['Name', 'Phone', 'Email', 'Orders', 'Total Spent']">
        <tr
          v-for="customer in customers"
          :key="customer.id"
          class="hover:bg-muted/50 transition-colors cursor-pointer"
          @click="openCustomerDetails(customer)"
        >
          <td class="px-4 py-3 text-sm font-medium text-foreground">{{ customer.name }}</td>
          <td class="px-4 py-3 text-sm text-muted-foreground">{{ customer.phone }}</td>
          <td class="px-4 py-3 text-sm text-muted-foreground">{{ customer.email }}</td>
          <td class="px-4 py-3 text-sm text-foreground">{{ customer.orders }}</td>
          <td class="px-4 py-3 text-sm font-medium text-foreground">{{ customer.total }}</td>
        </tr>
      </AppTable>
    </AppCard>

    <!-- Customer Details Drawer -->
    <AppDrawer v-model:open="drawerOpen" title="Customer Details" v-if="selectedCustomer">
      <div class="space-y-4">
        <div>
          <p class="text-sm text-muted-foreground mb-1">Name</p>
          <p class="text-base font-medium text-foreground">{{ selectedCustomer.name }}</p>
        </div>
        <div>
          <p class="text-sm text-muted-foreground mb-1">Phone</p>
          <p class="text-base text-foreground">{{ selectedCustomer.phone }}</p>
        </div>
        <div>
          <p class="text-sm text-muted-foreground mb-1">Email</p>
          <p class="text-base text-foreground">{{ selectedCustomer.email }}</p>
        </div>
        <div>
          <p class="text-sm text-muted-foreground mb-1">Total Orders</p>
          <p class="text-base font-medium text-foreground">{{ selectedCustomer.orders }}</p>
        </div>
        <div>
          <p class="text-sm text-muted-foreground mb-1">Total Spent</p>
          <p class="text-base font-medium text-foreground">{{ selectedCustomer.total }}</p>
        </div>
      </div>
    </AppDrawer>
  </div>
</template>

