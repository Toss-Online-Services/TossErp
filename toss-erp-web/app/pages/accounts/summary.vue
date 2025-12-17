<script setup lang="ts">
const { data: summary } = await useFetch('/api/accounts/summary')
</script>

<template>
  <div class="space-y-4">
    <div>
      <p class="text-sm text-muted-foreground">Accounts</p>
      <h1 class="text-xl font-semibold">Summary</h1>
    </div>

    <div class="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
      <div class="card-surface p-4">
        <p class="text-sm text-muted-foreground">Cash In</p>
        <p class="text-2xl font-semibold">R {{ summary?.cashIn }}</p>
      </div>
      <div class="card-surface p-4">
        <p class="text-sm text-muted-foreground">Cash Out</p>
        <p class="text-2xl font-semibold">R {{ summary?.cashOut }}</p>
      </div>
      <div class="card-surface p-4">
        <p class="text-sm text-muted-foreground">Net Cash</p>
        <p class="text-2xl font-semibold">R {{ summary?.netCash }}</p>
      </div>
    </div>

    <div class="card-surface p-4">
      <p class="text-sm text-muted-foreground">Receivables</p>
      <p class="text-xl font-semibold">{{ summary?.overdueInvoices }} overdue invoices</p>
    </div>
  </div>
</template>

