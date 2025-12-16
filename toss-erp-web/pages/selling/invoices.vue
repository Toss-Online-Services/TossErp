<script setup lang="ts">
const { data: orders } = await useFetch('/api/selling/orders')
</script>

<template>
  <div class="space-y-4">
    <div>
      <p class="text-sm text-muted-foreground">Selling</p>
      <h1 class="text-xl font-semibold">Sales Invoices</h1>
    </div>

    <div class="card-surface p-4">
      <div class="overflow-x-auto">
        <table class="min-w-full text-sm">
          <thead class="text-left text-xs uppercase text-muted-foreground">
            <tr>
              <th class="pb-2">Invoice</th>
              <th class="pb-2">Customer</th>
              <th class="pb-2">Total</th>
              <th class="pb-2">Status</th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="inv in orders || []"
              :key="inv.id"
              class="border-t border-border/60"
            >
              <td class="py-2 font-medium">INV-{{ inv.id }}</td>
              <td class="py-2">{{ inv.customer }}</td>
              <td class="py-2">R {{ inv.total }}</td>
              <td class="py-2">
                <span class="inline-flex items-center rounded-full bg-success/10 px-2 py-1 text-xs text-success">
                  Paid
                </span>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

