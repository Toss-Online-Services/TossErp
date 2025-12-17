<script setup lang="ts">
const { data: items, pending } = await useFetch('/api/stock/items')
</script>

<template>
  <div class="space-y-4">
    <div>
      <p class="text-sm text-muted-foreground">Stock</p>
      <h1 class="text-xl font-semibold">Items</h1>
    </div>

    <div class="card-surface p-4">
      <div class="overflow-x-auto">
        <table class="min-w-full text-sm">
          <thead class="text-left text-xs uppercase text-muted-foreground">
            <tr>
              <th class="pb-2">Item</th>
              <th class="pb-2">SKU</th>
              <th class="pb-2">Qty</th>
              <th class="pb-2">Min</th>
              <th class="pb-2">Location</th>
              <th class="pb-2">Price</th>
            </tr>
          </thead>
          <tbody>
            <tr v-if="pending">
              <td colspan="6" class="py-4 text-center text-muted-foreground">Loading…</td>
            </tr>
            <tr
              v-for="item in items || []"
              :key="item.id"
              class="border-t border-border/60"
            >
              <td class="py-2 font-medium">{{ item.name }}</td>
              <td class="py-2">{{ item.sku }}</td>
              <td class="py-2">{{ item.qty }} {{ item.uom }}</td>
              <td class="py-2">{{ item.minQty }}</td>
              <td class="py-2">{{ item.location }}</td>
              <td class="py-2">R {{ item.price }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

