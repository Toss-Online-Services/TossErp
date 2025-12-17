<script setup lang="ts">
const { data: customers } = await useFetch('/api/crm/customers')
</script>

<template>
  <div class="space-y-4">
    <div>
      <p class="text-sm text-muted-foreground">CRM</p>
      <h1 class="text-xl font-semibold">Customers</h1>
    </div>

    <div class="card-surface p-4">
      <div class="overflow-x-auto">
        <table class="min-w-full text-sm">
          <thead class="text-left text-xs uppercase text-muted-foreground">
            <tr>
              <th class="pb-2">Customer</th>
              <th class="pb-2">Phone</th>
              <th class="pb-2">Balance</th>
              <th class="pb-2">Tier</th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="customer in customers || []"
              :key="customer.id"
              class="border-t border-border/60"
            >
              <td class="py-2 font-medium">{{ customer.name }}</td>
              <td class="py-2">{{ customer.phone }}</td>
              <td class="py-2">R {{ customer.balance }}</td>
              <td class="py-2">{{ customer.tier }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

