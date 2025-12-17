<script setup lang="ts">
const { data: orders } = await useFetch('/api/selling/orders')
</script>

<template>
  <div class="space-y-4">
    <div>
      <p class="text-sm text-muted-foreground">Selling</p>
      <h1 class="text-xl font-semibold">Sales Orders</h1>
    </div>

    <div class="card-surface p-4">
      <div class="overflow-x-auto">
        <table class="min-w-full text-sm">
          <thead class="text-left text-xs uppercase text-muted-foreground">
            <tr>
              <th class="pb-2">Order</th>
              <th class="pb-2">Customer</th>
              <th class="pb-2">Status</th>
              <th class="pb-2">Total</th>
              <th class="pb-2">Date</th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="order in orders || []"
              :key="order.id"
              class="border-t border-border/60"
            >
              <td class="py-2 font-medium">{{ order.id }}</td>
              <td class="py-2">{{ order.customer }}</td>
              <td class="py-2">
                <span class="inline-flex items-center rounded-full bg-primary/10 px-2 py-1 text-xs text-primary">
                  {{ order.status }}
                </span>
              </td>
              <td class="py-2">R {{ order.total }}</td>
              <td class="py-2 text-muted-foreground">{{ order.date.split('T')[0] }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

