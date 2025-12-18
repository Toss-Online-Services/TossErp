<script setup lang="ts">
const { data: orders } = await useFetch('/api/selling/orders')
</script>

<template>
  <div class="container-fluid py-4">
    <div class="row mb-4">
      <div class="col-12">
        <MaterialCard variant="default">
          <div class="card-body p-4">
            <div class="overflow-x-auto">
              <table class="min-w-full text-sm">
                <thead class="text-left text-xs uppercase text-muted-foreground border-b border-border">
                  <tr>
                    <th class="pb-3 font-semibold">Order</th>
                    <th class="pb-3 font-semibold">Customer</th>
                    <th class="pb-3 font-semibold">Status</th>
                    <th class="pb-3 font-semibold">Total</th>
                    <th class="pb-3 font-semibold">Date</th>
                  </tr>
                </thead>
                <tbody>
                  <tr
                    v-for="order in orders || []"
                    :key="order.id"
                    class="border-t border-border/60 hover:bg-gray-50 dark:hover:bg-slate-800/50 transition-colors"
                  >
                    <td class="py-3 font-medium text-primary">#{{ order.id }}</td>
                    <td class="py-3">{{ order.customer }}</td>
                    <td class="py-3">
                      <span class="inline-flex items-center rounded-full bg-primary/10 px-2.5 py-1 text-xs font-medium text-primary border border-primary/20">
                        {{ order.status }}
                      </span>
                    </td>
                    <td class="py-3 font-medium">R {{ order.total.toLocaleString() }}</td>
                    <td class="py-3 text-muted-foreground">{{ order.date.split('T')[0] }}</td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </MaterialCard>
      </div>
    </div>
  </div>
</template>
