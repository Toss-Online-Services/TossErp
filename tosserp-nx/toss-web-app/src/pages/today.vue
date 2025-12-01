<script setup lang="ts">
import { ref, onMounted } from 'vue'
import Card from '@/components/ui/Card.vue'
import CardHeader from '@/components/ui/CardHeader.vue'
import CardTitle from '@/components/ui/CardTitle.vue'
import CardContent from '@/components/ui/CardContent.vue'
import Breadcrumbs from '@/components/ui/Breadcrumbs.vue'
import KpiCard from '@/components/ui/KpiCard.vue'
import { 
  Activity,
  Clock,
  User,
  FileText,
  ShoppingBag,
  DollarSign,
  TrendingUp
} from 'lucide-vue-next'
import { useAnalyticsApi } from '@/composables/useAnalyticsApi'
import { useAccountingApi } from '@/composables/useAccountingApi'

const { getActivityToday, getAnalyticsSummary, isLoading: isActivityLoading } = useAnalyticsApi()
const { getCashflowSummary, isLoading: isCashflowLoading } = useAccountingApi()

const activities = ref<any[]>([])
const analyticsSummary = ref<any>(null)
const cashflow = ref<any>(null)

const today = new Date()
const startOfToday = new Date(today.getFullYear(), today.getMonth(), today.getDate())
const endOfToday = new Date(today.getFullYear(), today.getMonth(), today.getDate(), 23, 59, 59)

const loadTodayData = async () => {
  try {
    const [activitiesData, summaryData, cashflowData] = await Promise.all([
      getActivityToday({ date: today.toISOString() }),
      getAnalyticsSummary({
        fromDate: startOfToday.toISOString(),
        toDate: endOfToday.toISOString()
      }).catch(() => null),
      getCashflowSummary({
        fromDate: startOfToday.toISOString(),
        toDate: endOfToday.toISOString()
      }).catch(() => null)
    ])
    activities.value = activitiesData
    analyticsSummary.value = summaryData
    cashflow.value = cashflowData
  } catch (error) {
    console.error('Failed to load today data:', error)
  }
}

const isLoading = computed(() => isActivityLoading.value || isCashflowLoading.value)

onMounted(() => {
  loadTodayData()
})
</script>

<template>
  <div class="space-y-6">
    <div>
      <Breadcrumbs />
      <h1 class="text-2xl md:text-3xl font-bold tracking-tight mt-2">Today</h1>
      <p class="text-muted-foreground mt-1">Today's activity and summary</p>
    </div>

    <div v-if="isLoading" class="text-center py-12 text-muted-foreground">
      Loading today's data...
    </div>

    <div v-else class="space-y-6">
      <!-- Today's KPIs -->
      <div class="grid gap-4 md:grid-cols-3">
        <KpiCard
          v-if="analyticsSummary"
          title="Today's Sales"
          :value="analyticsSummary.totalSalesAmount || 0"
          :icon="ShoppingBag"
          status="good"
          :format-currency="true"
        />
        <KpiCard
          v-if="cashflow"
          title="Money In"
          :value="cashflow.totalCashIn || 0"
          :icon="TrendingUp"
          status="good"
          :format-currency="true"
        />
        <KpiCard
          v-if="analyticsSummary"
          title="POS Sales"
          :value="analyticsSummary.totalPosSales || 0"
          :icon="FileText"
          status="neutral"
        />
      </div>

      <!-- Today's Activity -->
      <Card>
        <CardHeader>
          <CardTitle class="flex items-center gap-2">
            <Activity class="h-5 w-5 text-primary" />
            Today's Activity
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div v-if="activities.length === 0" class="text-center py-12 text-muted-foreground">
            <Activity :size="48" class="mx-auto mb-3 opacity-50" />
            <p>No activity recorded today</p>
          </div>
          <div v-else class="space-y-3">
            <div
              v-for="activity in activities"
              :key="activity.id"
              class="flex items-start gap-4 p-4 border rounded-lg hover:bg-accent/50 transition-colors"
            >
              <div class="flex-shrink-0 mt-1">
                <div class="h-2 w-2 rounded-full bg-primary"></div>
              </div>
              <div class="flex-1 min-w-0">
                <div class="flex items-center justify-between">
                  <div>
                    <div class="font-medium">
                      {{ activity.action }} - {{ activity.entityType }}
                    </div>
                    <div class="text-sm text-muted-foreground mt-1">
                      <span v-if="activity.userName">{{ activity.userName }}</span>
                      <span v-else-if="activity.userId">User {{ activity.userId }}</span>
                    </div>
                  </div>
                  <div class="text-sm text-muted-foreground">
                    <Clock :size="14" class="inline mr-1" />
                    {{ new Date(activity.created).toLocaleTimeString() }}
                  </div>
                </div>
                <div v-if="activity.notes" class="text-sm text-muted-foreground mt-2">
                  {{ activity.notes }}
                </div>
              </div>
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
  </div>
</template>

