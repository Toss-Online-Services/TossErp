<script setup lang="ts">
const { data: purchaseOrders } = await useFetch('/api/buying/purchase-orders')
</script>

<template>
  <div class="min-vh-100 bg-gray-100 g-sidenav-show">
    <!-- Page Header -->
    <div class="py-4">
      <div class="container-fluid">
        <div class="row align-items-center">
          <div class="col-auto">
            <div class="mb-0">
              <p class="text-sm text-muted-foreground">Buying</p>
              <h1 class="text-3xl font-bold">Purchase Orders</h1>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Purchase Orders Table -->
    <div class="container-fluid py-4">
      <div class="row">
        <div class="col-12">
          <MaterialCard variant="default">
            <div class="overflow-x-auto">
              <table class="min-w-full divide-y divide-slate-200 dark:divide-slate-700">
                <thead class="bg-gray-50 dark:bg-slate-700">
                  <tr>
                    <th class="px-6 py-4 text-left text-xs font-semibold text-slate-700 dark:text-slate-300 uppercase tracking-wider">PO #</th>
                    <th class="px-6 py-4 text-left text-xs font-semibold text-slate-700 dark:text-slate-300 uppercase tracking-wider">Supplier</th>
                    <th class="px-6 py-4 text-left text-xs font-semibold text-slate-700 dark:text-slate-300 uppercase tracking-wider">Total</th>
                    <th class="px-6 py-4 text-left text-xs font-semibold text-slate-700 dark:text-slate-300 uppercase tracking-wider">Status</th>
                  </tr>
                </thead>
                <tbody class="divide-y divide-slate-200 dark:divide-slate-700">
                  <tr
                    v-for="po in purchaseOrders || []"
                    :key="po.id"
                    class="hover:bg-gray-50 dark:hover:bg-slate-600/30 transition-colors"
                  >
                    <td class="px-6 py-4 text-sm font-medium text-primary">#{{ po.id }}</td>
                    <td class="px-6 py-4 text-sm text-slate-900 dark:text-white">{{ po.customer }}</td>
                    <td class="px-6 py-4 text-sm font-semibold text-slate-900 dark:text-white">R {{ po.total.toLocaleString() }}</td>
                    <td class="px-6 py-4 text-sm">
                      <span class="inline-flex px-3 py-1 text-xs font-semibold rounded-full"
                        :class="{
                          'bg-green-100 text-green-800': po.status === 'Delivered',
                          'bg-yellow-100 text-yellow-800': po.status === 'Pending',
                          'bg-blue-100 text-blue-800': po.status === 'In Progress'
                        }"
                      >
                        {{ po.status }}
                      </span>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </MaterialCard>
        </div>
      </div>
    </div>
  </div>
</template>

