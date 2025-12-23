<script setup lang="ts">
import { ref } from 'vue'
import { Building2 } from 'lucide-vue-next'
import AppCard from '~/components/common/AppCard.vue'
import AppTable from '~/components/common/AppTable.vue'
import AppSectionHeader from '~/components/common/AppSectionHeader.vue'
import AppDrawer from '~/components/common/AppDrawer.vue'

useHead({
  title: 'Suppliers - TOSS Admin',
  meta: [{ name: 'description', content: 'Supplier management for TOSS Admin' }]
})

definePageMeta({
  layout: 'dashboard'
})

const suppliers = ref([
  { id: 1, name: 'Supplier A', contact: 'John Supplier', phone: '+27 82 111 2222', email: 'suppliera@example.com', orders: 12, total: 'R45,250.00' },
  { id: 2, name: 'Supplier B', contact: 'Jane Vendor', phone: '+27 83 222 3333', email: 'supplierb@example.com', orders: 8, total: 'R28,900.00' },
  { id: 3, name: 'Supplier C', contact: 'Mike Wholesale', phone: '+27 84 333 4444', email: 'supplierc@example.com', orders: 5, total: 'R15,600.00' }
])

const selectedSupplier = ref(null)
const drawerOpen = ref(false)

const openSupplierDetails = (supplier: any) => {
  selectedSupplier.value = supplier
  drawerOpen.value = true
}
</script>

<template>
  <div class="space-y-6">
    <AppSectionHeader
      title="Suppliers"
      description="Manage supplier relationships"
    />

    <AppCard>
      <div class="mb-4 flex items-center justify-between">
        <input
          type="text"
          placeholder="Search suppliers..."
          class="px-3 py-2 border border-border rounded-lg text-sm w-64"
        />
      </div>
      <AppTable :headers="['Name', 'Contact', 'Phone', 'Email', 'Orders', 'Total']">
        <tr
          v-for="supplier in suppliers"
          :key="supplier.id"
          class="hover:bg-muted/50 transition-colors cursor-pointer"
          @click="openSupplierDetails(supplier)"
        >
          <td class="px-4 py-3 text-sm font-medium text-foreground">{{ supplier.name }}</td>
          <td class="px-4 py-3 text-sm text-muted-foreground">{{ supplier.contact }}</td>
          <td class="px-4 py-3 text-sm text-muted-foreground">{{ supplier.phone }}</td>
          <td class="px-4 py-3 text-sm text-muted-foreground">{{ supplier.email }}</td>
          <td class="px-4 py-3 text-sm text-foreground">{{ supplier.orders }}</td>
          <td class="px-4 py-3 text-sm font-medium text-foreground">{{ supplier.total }}</td>
        </tr>
      </AppTable>
    </AppCard>

    <!-- Supplier Details Drawer -->
    <AppDrawer v-model:open="drawerOpen" title="Supplier Details" v-if="selectedSupplier">
      <div class="space-y-4">
        <div>
          <p class="text-sm text-muted-foreground mb-1">Name</p>
          <p class="text-base font-medium text-foreground">{{ selectedSupplier.name }}</p>
        </div>
        <div>
          <p class="text-sm text-muted-foreground mb-1">Contact Person</p>
          <p class="text-base text-foreground">{{ selectedSupplier.contact }}</p>
        </div>
        <div>
          <p class="text-sm text-muted-foreground mb-1">Phone</p>
          <p class="text-base text-foreground">{{ selectedSupplier.phone }}</p>
        </div>
        <div>
          <p class="text-sm text-muted-foreground mb-1">Email</p>
          <p class="text-base text-foreground">{{ selectedSupplier.email }}</p>
        </div>
        <div>
          <p class="text-sm text-muted-foreground mb-1">Total Orders</p>
          <p class="text-base font-medium text-foreground">{{ selectedSupplier.orders }}</p>
        </div>
        <div>
          <p class="text-sm text-muted-foreground mb-1">Total Value</p>
          <p class="text-base font-medium text-foreground">{{ selectedSupplier.total }}</p>
        </div>
      </div>
    </AppDrawer>
  </div>
</template>

