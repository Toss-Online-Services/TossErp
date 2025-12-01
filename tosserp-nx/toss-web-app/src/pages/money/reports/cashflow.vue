<script setup lang="ts">
import { ref, onMounted } from 'vue'
import Card from '@/components/ui/Card.vue'
import CardHeader from '@/components/ui/CardHeader.vue'
import CardTitle from '@/components/ui/CardTitle.vue'
import CardContent from '@/components/ui/CardContent.vue'
import Breadcrumbs from '@/components/ui/Breadcrumbs.vue'
import { TrendingUp, TrendingDown, DollarSign } from 'lucide-vue-next'
import { useAccountingApi } from '@/composables/useAccountingApi'

const { getCashflowSummary, isLoading } = useAccountingApi()

const cashflow = ref<any>(null)

const today = new Date()
const startOfMonth = new Date(today.getFullYear(), today.getMonth(), 1)
const endOfMonth = new Date(today.getFullYear(), today.getMonth() + 1, 0, 23, 59, 59)

const loadReport = async () => {
  try {
    const result = await getCashflowSummary({
      fromDate: startOfMonth.toISOString(),
      toDate: endOfMonth.toISOString()
    })
    cashflow.value = result
  } catch (error) {
    console.error('Failed to load cashflow report:', error)
  }
}

onMounted(() => {
  loadReport()
})
</script>

<template>
  <div class="space-y-6">
    <div>
      <Breadcrumbs />
      <h1 class="text-2xl md:text-3xl font-bold tracking-tight mt-2">Cashflow Report</h1>
      <p class="text-muted-foreground mt-1">Money in, money out, and balance summary</p>
    </div>

    <div v-if="isLoading" class="text-center py-12 text-muted-foreground">
      Loading cashflow report...
    </div>

    <div v-else-if="cashflow" class="space-y-6">
      <!-- Summary Cards -->
      <div class="grid gap-4 md:grid-cols-4">
        <Card>
          <CardHeader>
            <CardTitle class="text-sm font-medium flex items-center gap-2">
              <DollarSign class="h-4 w-4 text-primary" />
              Opening Balance
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div class="text-2xl font-bold">
              R{{ cashflow.openingBalance.toFixed(2) }}
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle class="text-sm font-medium flex items-center gap-2">
              <TrendingUp class="h-4 w-4 text-emerald-600" />
              Total Cash In
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div class="text-2xl font-bold text-emerald-600">
              R{{ cashflow.totalCashIn.toFixed(2) }}
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle class="text-sm font-medium flex items-center gap-2">
              <TrendingDown class="h-4 w-4 text-red-600" />
              Total Cash Out
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div class="text-2xl font-bold text-red-600">
              R{{ cashflow.totalCashOut.toFixed(2) }}
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle class="text-sm font-medium flex items-center gap-2">
              <DollarSign class="h-4 w-4 text-primary" />
              Closing Balance
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div class="text-2xl font-bold" :class="cashflow.closingBalance >= 0 ? 'text-emerald-600' : 'text-red-600'">
              R{{ cashflow.closingBalance.toFixed(2) }}
            </div>
          </CardContent>
        </Card>
      </div>

      <!-- Cash In Breakdown -->
      <Card>
        <CardHeader>
          <CardTitle>Money In</CardTitle>
        </CardHeader>
        <CardContent>
          <div v-if="cashflow.cashInItems.length === 0" class="text-center py-8 text-muted-foreground">
            No cash in transactions for this period
          </div>
          <div v-else class="space-y-2">
            <div
              v-for="item in cashflow.cashInItems"
              :key="item.source"
              class="flex items-center justify-between p-3 border rounded-lg"
            >
              <div>
                <div class="font-medium">{{ item.source }}</div>
                <div class="text-sm text-muted-foreground">{{ item.count }} transactions</div>
              </div>
              <div class="text-lg font-semibold text-emerald-600">
                R{{ item.amount.toFixed(2) }}
              </div>
            </div>
          </div>
        </CardContent>
      </Card>

      <!-- Cash Out Breakdown -->
      <Card>
        <CardHeader>
          <CardTitle>Money Out</CardTitle>
        </CardHeader>
        <CardContent>
          <div v-if="cashflow.cashOutItems.length === 0" class="text-center py-8 text-muted-foreground">
            No cash out transactions for this period
          </div>
          <div v-else class="space-y-2">
            <div
              v-for="item in cashflow.cashOutItems"
              :key="item.source"
              class="flex items-center justify-between p-3 border rounded-lg"
            >
              <div>
                <div class="font-medium">{{ item.source }}</div>
                <div class="text-sm text-muted-foreground">{{ item.count }} transactions</div>
              </div>
              <div class="text-lg font-semibold text-red-600">
                R{{ item.amount.toFixed(2) }}
              </div>
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
  </div>
</template>

