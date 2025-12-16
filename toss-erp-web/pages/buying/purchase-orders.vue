<script setup lang="ts">
const { data: purchaseOrders } = await useFetch('/api/buying/purchase-orders')
</script>

<template>
  <div class="space-y-4">
    <div>
      <p class="text-sm text-muted-foreground">Buying</p>
      <h1 class="text-xl font-semibold">Purchase Orders</h1>
    </div>

    <div class="card-surface p-4">
      <div class="overflow-x-auto">
        <table class="min-w-full text-sm">
          <thead class="text-left text-xs uppercase text-muted-foreground">
            <tr>
              <th class="pb-2">PO</th>
              <th class="pb-2">Supplier</th>
              <th class="pb-2">Total</th>
              <th class="pb-2">Status</th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="po in purchaseOrders || []"
              :key="po.id"
              class="border-t border-border/60"
            >
              <td class="py-2 font-medium">PO-{{ po.id }}</td>
              <td class="py-2">{{ po.customer }}</td>
              <td class="py-2">R {{ po.total }}</td>
              <td class="py-2 text-muted-foreground">{{ po.status }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

