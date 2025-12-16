<script setup lang="ts">
const { data: orders } = await useFetch('/api/selling/orders')
</script>

<template>
  <div class="space-y-4">
    <div>
      <p class="text-sm text-muted-foreground">Selling</p>
      <h1 class="text-xl font-semibold">Quotations</h1>
    </div>

    <div class="card-surface p-4">
      <p class="text-sm text-muted-foreground mb-3">
        Using sample data; convert to orders once backend is ready.
      </p>
      <div class="overflow-x-auto">
        <table class="min-w-full text-sm">
          <thead class="text-left text-xs uppercase text-muted-foreground">
            <tr>
              <th class="pb-2">Quote</th>
              <th class="pb-2">Customer</th>
              <th class="pb-2">Amount</th>
              <th class="pb-2">Date</th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="quote in orders || []"
              :key="quote.id"
              class="border-t border-border/60"
            >
              <td class="py-2 font-medium">Q-{{ quote.id }}</td>
              <td class="py-2">{{ quote.customer }}</td>
              <td class="py-2">R {{ quote.total }}</td>
              <td class="py-2 text-muted-foreground">{{ quote.date.split('T')[0] }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

