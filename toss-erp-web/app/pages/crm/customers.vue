<script setup lang="ts">
const { data: customers } = await useFetch('/api/crm/customers')
</script>

<template>
  <div class="min-vh-100 bg-gray-100 g-sidenav-show">
    <!-- Page Header -->
    <div class="py-4">
      <div class="container-fluid">
        <div class="row align-items-center">
          <div class="col-auto">
            <div class="mb-0">
              <p class="text-sm text-muted-foreground">CRM</p>
              <h1 class="text-3xl font-bold">Customers</h1>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Customers Table -->
    <div class="container-fluid py-4">
      <div class="row">
        <div class="col-12">
          <MaterialCard variant="default">
            <div class="overflow-x-auto">
              <table class="min-w-full divide-y divide-slate-200 dark:divide-slate-700">
                <thead class="bg-gray-50 dark:bg-slate-700">
                  <tr>
                    <th class="px-6 py-4 text-left text-xs font-semibold text-slate-700 dark:text-slate-300 uppercase tracking-wider">Customer</th>
                    <th class="px-6 py-4 text-left text-xs font-semibold text-slate-700 dark:text-slate-300 uppercase tracking-wider">Phone</th>
                    <th class="px-6 py-4 text-left text-xs font-semibold text-slate-700 dark:text-slate-300 uppercase tracking-wider">Balance</th>
                    <th class="px-6 py-4 text-left text-xs font-semibold text-slate-700 dark:text-slate-300 uppercase tracking-wider">Tier</th>
                  </tr>
                </thead>
                <tbody class="divide-y divide-slate-200 dark:divide-slate-700">
                  <tr
                    v-for="customer in customers || []"
                    :key="customer.id"
                    class="hover:bg-gray-50 dark:hover:bg-slate-600/30 transition-colors"
                  >
                    <td class="px-6 py-4 text-sm font-medium text-slate-900 dark:text-white">{{ customer.name }}</td>
                    <td class="px-6 py-4 text-sm text-slate-600 dark:text-slate-400">{{ customer.phone }}</td>
                    <td class="px-6 py-4 text-sm font-semibold text-slate-900 dark:text-white">R {{ customer.balance.toLocaleString() }}</td>
                    <td class="px-6 py-4 text-sm">
                      <span class="inline-flex px-3 py-1 text-xs font-semibold rounded-full"
                        :class="{
                          'bg-gold-100 text-gold-800': customer.tier === 'Gold',
                          'bg-blue-100 text-blue-800': customer.tier === 'Silver',
                          'bg-gray-100 text-gray-800': customer.tier === 'Bronze'
                        }"
                      >
                        {{ customer.tier }}
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

