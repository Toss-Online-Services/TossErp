<script setup lang="ts">
const { data: purchaseOrders } = await useFetch('/api/buying/purchase-orders')

const suppliers = computed(() => {
  const map = new Map<string, number>()
  const orders = purchaseOrders.value || []
  for (const po of orders) {
    map.set(po.customer, (map.get(po.customer) || 0) + po.total)
  }
  return Array.from(map.entries()).map(([name, spend]) => ({ name, spend }))
})
</script>

<template>
  <div class="space-y-4">
    <div>
      <p class="text-sm text-muted-foreground">Buying</p>
      <h1 class="text-xl font-semibold">Suppliers</h1>
    </div>

    <div class="card-surface p-4">
      <div class="overflow-x-auto">
        <table class="min-w-full text-sm">
          <thead class="text-left text-xs uppercase text-muted-foreground">
            <tr>
              <th class="pb-2">Supplier</th>
              <th class="pb-2">Spend</th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="sup in suppliers"
              :key="sup.name"
              class="border-t border-border/60"
            >
              <td class="py-2 font-medium">{{ sup.name }}</td>
              <td class="py-2">R {{ sup.spend.toLocaleString() }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>
