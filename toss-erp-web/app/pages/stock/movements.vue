<script setup lang="ts">
const { data: items } = await useFetch('/api/stock/items')

const movements = computed(() => {
  const stockItems = items.value || []
  return stockItems.map((item: { id: string; name: string; qty: number; location?: string }) => ({
    id: item.id,
    item: item.name,
    qty: item.qty,
    location: item.location || 'Warehouse',
    direction: item.qty % 2 === 0 ? 'in' : 'out',
    date: new Date().toISOString()
  }))
})
</script>

<template>
  <div class="space-y-4">
    <div>
      <p class="text-sm text-muted-foreground">Stock</p>
      <h1 class="text-xl font-semibold">Movements</h1>
    </div>

    <div class="card-surface p-4">
      <div class="space-y-3">
        <div
          v-for="move in movements"
          :key="move.id"
          class="flex items-center justify-between border-b border-border/60 pb-2 last:border-0"
        >
          <div>
            <p class="font-medium">{{ move.item }}</p>
            <p class="text-xs text-muted-foreground">{{ move.location }}</p>
          </div>
          <div class="flex items-center gap-4 text-sm">
            <span
              class="inline-flex items-center gap-1 rounded-full px-2 py-1 text-xs font-medium"
              :class="move.direction === 'in' ? 'bg-success/10 text-success' : 'bg-destructive/10 text-destructive'"
            >
              {{ move.direction === 'in' ? '↓ In' : '↑ Out' }}
            </span>
            <span class="font-semibold">{{ move.qty }}</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
