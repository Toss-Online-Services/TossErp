<script setup lang="ts">
const { data: alerts } = await useFetch('/api/stock/alerts')
</script>

<template>
  <div class="space-y-4">
    <div>
      <p class="text-sm text-muted-foreground">Stock</p>
      <h1 class="text-xl font-semibold">Low stock alerts</h1>
    </div>

    <div class="card-surface p-4">
      <div class="overflow-x-auto">
        <table class="min-w-full text-sm">
          <thead class="text-left text-xs uppercase text-muted-foreground">
            <tr>
              <th class="pb-2">Item</th>
              <th class="pb-2">Qty</th>
              <th class="pb-2">Min</th>
              <th class="pb-2">Location</th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="item in alerts || []"
              :key="item.id"
              class="border-t border-border/60"
            >
              <td class="py-2 font-medium">{{ item.name }}</td>
              <td class="py-2">{{ item.qty }}</td>
              <td class="py-2">{{ item.minQty }}</td>
              <td class="py-2">{{ item.location }}</td>
            </tr>
            <tr v-if="(alerts || []).length === 0">
              <td colspan="4" class="py-4 text-center text-muted-foreground">No alerts 🎉</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

