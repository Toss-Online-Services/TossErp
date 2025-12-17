<script setup lang="ts">
const { data: orders } = await useFetch('/api/selling/orders')
</script>

<template>
  <div class="space-y-4">
    <div>
      <p class="text-sm text-muted-foreground">Selling</p>
      <h1 class="text-xl font-semibold">Deliveries</h1>
    </div>

    <div class="card-surface p-4">
      <div class="overflow-x-auto">
        <table class="min-w-full text-sm">
          <thead class="text-left text-xs uppercase text-muted-foreground">
            <tr>
              <th class="pb-2">Delivery</th>
              <th class="pb-2">Customer</th>
              <th class="pb-2">Status</th>
              <th class="pb-2">Date</th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="del in orders || []"
              :key="del.id"
              class="border-t border-border/60"
            >
              <td class="py-2 font-medium">DN-{{ del.id }}</td>
              <td class="py-2">{{ del.customer }}</td>
              <td class="py-2">
                <span class="inline-flex items-center rounded-full bg-secondary/10 px-2 py-1 text-xs text-secondary-foreground">
                  {{ del.status === 'Delivered' ? 'Delivered' : 'In transit' }}
                </span>
              </td>
              <td class="py-2 text-muted-foreground">{{ del.date.split('T')[0] }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

