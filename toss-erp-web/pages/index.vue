<script setup lang="ts">
import StatCard from '~/components/ui/StatCard.vue'
import ChartCard from '~/components/ui/ChartCard.vue'
import LineChart from '~/components/charts/LineChart.vue'
import BarChart from '~/components/charts/BarChart.vue'

const { data: dashboard } = await useFetch('/api/dashboard')
</script>

<template>
  <div class="space-y-6">
    <div>
      <p class="text-sm text-muted-foreground">ERP-III pulse</p>
      <h1 class="text-2xl font-semibold">Today</h1>
    </div>

    <div class="kpi-grid">
      <StatCard
        v-for="kpi in dashboard?.kpis || []"
        :key="kpi.label"
        :label="kpi.label"
        :value="kpi.value"
        :delta="kpi.delta"
        :icon="kpi.icon"
      />
    </div>

    <div class="grid gap-4 lg:grid-cols-2">
      <ChartCard title="Daily sales" subtitle="Last 7 days">
        <ClientOnly>
          <LineChart v-if="dashboard?.salesTrend" :data="dashboard.salesTrend" />
        </ClientOnly>
      </ChartCard>
      <ChartCard title="Completed jobs" subtitle="Weekly">
        <ClientOnly>
          <BarChart v-if="dashboard?.tasksTrend" :data="dashboard.tasksTrend" />
        </ClientOnly>
      </ChartCard>
    </div>

    <div class="card-surface p-4 space-y-3">
      <div class="flex items-center justify-between">
        <h3 class="section-title">Sales by country</h3>
        <p class="text-sm text-muted-foreground">Recent period</p>
      </div>
      <div class="overflow-x-auto">
        <table class="min-w-full text-sm">
          <thead class="text-left text-xs uppercase text-muted-foreground">
            <tr>
              <th class="pb-2">Country</th>
              <th class="pb-2">Sales</th>
              <th class="pb-2">Value</th>
              <th class="pb-2">Bounce</th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="row in dashboard?.countrySales || []"
              :key="row.country"
              class="border-t border-border/60"
            >
              <td class="py-2 font-medium">{{ row.country }}</td>
              <td class="py-2">{{ row.sales }}</td>
              <td class="py-2">{{ row.value }}</td>
              <td class="py-2 text-muted-foreground">{{ row.bounce }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

