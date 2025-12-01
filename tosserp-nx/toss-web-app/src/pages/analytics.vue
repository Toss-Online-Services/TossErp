<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import Card from '@/components/ui/Card.vue'
import CardHeader from '@/components/ui/CardHeader.vue'
import CardTitle from '@/components/ui/CardTitle.vue'
import CardContent from '@/components/ui/CardContent.vue'
import Breadcrumbs from '@/components/ui/Breadcrumbs.vue'
import KpiCard from '@/components/ui/KpiCard.vue'
import { 
  TrendingUp, 
  TrendingDown, 
  DollarSign, 
  ShoppingBag,
  LogIn,
  AlertCircle,
  CheckCircle2,
  Package,
  BarChart3
} from 'lucide-vue-next'
import { useAnalyticsApi } from '@/composables/useAnalyticsApi'

const { getAnalyticsSummary, getWeeklyAnalytics, isLoading } = useAnalyticsApi()

const summary = ref<any>(null)
const weeklyData = ref<any[]>([])

const today = new Date()
const last7Days = new Date(today)
last7Days.setDate(today.getDate() - 7)

const loadAnalytics = async () => {
  try {
    const [summaryData, weeklyDataResult] = await Promise.all([
      getAnalyticsSummary({
        fromDate: last7Days.toISOString(),
        toDate: today.toISOString()
      }),
      getWeeklyAnalytics({ weeks: 4 })
    ])
    summary.value = summaryData
    weeklyData.value = weeklyDataResult
  } catch (error) {
    console.error('Failed to load analytics:', error)
  }
}

const moduleUsageEntries = computed(() => {
  if (!summary.value?.moduleUsage) return []
  return Object.entries(summary.value.moduleUsage)
    .map(([module, count]) => ({ module, count: count as number }))
    .sort((a, b) => b.count - a.count)
})

onMounted(() => {
  loadAnalytics()
})
</script>

<template>
  <div class="space-y-6">
    <div>
      <Breadcrumbs />
      <h1 class="text-2xl md:text-3xl font-bold tracking-tight mt-2">Analytics</h1>
      <p class="text-muted-foreground mt-1">Business insights and performance metrics</p>
    </div>

    <div v-if="isLoading" class="text-center py-12 text-muted-foreground">
      Loading analytics...
    </div>

    <div v-else-if="summary" class="space-y-6">
      <!-- KPI Cards -->
      <div class="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
        <KpiCard
          title="Total Logins"
          :value="summary.totalLogins"
          :icon="LogIn"
          status="neutral"
        />
        <KpiCard
          title="POS Sales"
          :value="summary.totalPosSales"
          :icon="ShoppingBag"
          status="good"
        />
        <KpiCard
          title="Sales Amount"
          :value="summary.totalSalesAmount"
          :icon="DollarSign"
          status="good"
          :format-currency="true"
        />
        <KpiCard
          title="Stock Alerts Resolved"
          :value="summary.stockAlertsResolved"
          :icon="CheckCircle2"
          status="good"
        />
      </div>

      <!-- Weekly Trends -->
      <Card>
        <CardHeader>
          <CardTitle class="flex items-center gap-2">
            <BarChart3 class="h-5 w-5 text-primary" />
            Weekly Trends (Last 4 Weeks)
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div v-if="weeklyData.length === 0" class="text-center py-8 text-muted-foreground">
            No weekly data available
          </div>
          <div v-else class="space-y-4">
            <div
              v-for="week in weeklyData"
              :key="week.weekStart"
              class="p-4 border rounded-lg"
            >
              <div class="flex items-center justify-between mb-3">
                <div>
                  <div class="font-medium">
                    {{ new Date(week.weekStart).toLocaleDateString() }} - 
                    {{ new Date(week.weekEnd).toLocaleDateString() }}
                  </div>
                </div>
              </div>
              <div class="grid grid-cols-2 md:grid-cols-5 gap-4 text-sm">
                <div>
                  <div class="text-muted-foreground">Logins</div>
                  <div class="font-semibold">{{ week.logins }}</div>
                </div>
                <div>
                  <div class="text-muted-foreground">POS Sales</div>
                  <div class="font-semibold">{{ week.posSales }}</div>
                </div>
                <div>
                  <div class="text-muted-foreground">Sales Amount</div>
                  <div class="font-semibold text-primary">R{{ week.salesAmount.toFixed(2) }}</div>
                </div>
                <div>
                  <div class="text-muted-foreground">Alerts Resolved</div>
                  <div class="font-semibold text-emerald-600">{{ week.stockAlertsResolved }}</div>
                </div>
                <div>
                  <div class="text-muted-foreground">Stock Outs</div>
                  <div class="font-semibold text-red-600">{{ week.stockOuts }}</div>
                </div>
              </div>
            </div>
          </div>
        </CardContent>
      </Card>

      <!-- Module Usage -->
      <Card>
        <CardHeader>
          <CardTitle>Module Usage</CardTitle>
        </CardHeader>
        <CardContent>
          <div v-if="moduleUsageEntries.length === 0" class="text-center py-8 text-muted-foreground">
            No module usage data available
          </div>
          <div v-else class="space-y-3">
            <div
              v-for="entry in moduleUsageEntries"
              :key="entry.module"
              class="flex items-center justify-between p-3 border rounded-lg"
            >
              <div class="flex items-center gap-3">
                <Package class="h-5 w-5 text-primary" />
                <div>
                  <div class="font-medium">{{ entry.module }}</div>
                </div>
              </div>
              <div class="text-lg font-semibold text-primary">
                {{ entry.count }} usage{{ entry.count !== 1 ? 's' : '' }}
              </div>
            </div>
          </div>
        </CardContent>
      </Card>

      <!-- Event Type Breakdown -->
      <Card>
        <CardHeader>
          <CardTitle>Event Type Breakdown</CardTitle>
        </CardHeader>
        <CardContent>
          <div v-if="!summary.eventTypeCounts || Object.keys(summary.eventTypeCounts).length === 0" 
               class="text-center py-8 text-muted-foreground">
            No event data available
          </div>
          <div v-else class="space-y-2">
            <div
              v-for="[eventType, count] in Object.entries(summary.eventTypeCounts)"
              :key="eventType"
              class="flex items-center justify-between p-3 border rounded-lg"
            >
              <div class="font-medium">{{ eventType }}</div>
              <div class="text-lg font-semibold text-primary">
                {{ count }}
              </div>
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
  </div>
</template>

