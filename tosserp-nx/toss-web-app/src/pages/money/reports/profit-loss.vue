<script setup lang="ts">
import { ref, onMounted } from 'vue'
import Card from '@/components/ui/Card.vue'
import CardHeader from '@/components/ui/CardHeader.vue'
import CardTitle from '@/components/ui/CardTitle.vue'
import CardContent from '@/components/ui/CardContent.vue'
import Breadcrumbs from '@/components/ui/Breadcrumbs.vue'
import Button from '@/components/ui/Button.vue'
import { TrendingUp, TrendingDown, DollarSign } from 'lucide-vue-next'
import { useAccountingApi } from '@/composables/useAccountingApi'

const { getProfitAndLoss, isLoading } = useAccountingApi()

const pnl = ref<any>(null)
const shopId = ref(1) // TODO: Get from auth/context

const today = new Date()
const startOfMonth = new Date(today.getFullYear(), today.getMonth(), 1)
const endOfMonth = new Date(today.getFullYear(), today.getMonth() + 1, 0, 23, 59, 59)

const loadReport = async () => {
  try {
    const result = await getProfitAndLoss({
      fromDate: startOfMonth.toISOString(),
      toDate: endOfMonth.toISOString(),
      shopId: shopId.value
    })
    pnl.value = result
  } catch (error) {
    console.error('Failed to load profit and loss:', error)
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
      <h1 class="text-2xl md:text-3xl font-bold tracking-tight mt-2">Profit & Loss</h1>
      <p class="text-muted-foreground mt-1">Revenue, expenses, and profit summary</p>
    </div>

    <div v-if="isLoading" class="text-center py-12 text-muted-foreground">
      Loading profit and loss report...
    </div>

    <div v-else-if="pnl" class="space-y-6">
      <!-- Summary Cards -->
      <div class="grid gap-4 md:grid-cols-4">
        <Card>
          <CardHeader>
            <CardTitle class="text-sm font-medium flex items-center gap-2">
              <TrendingUp class="h-4 w-4 text-emerald-600" />
              Total Revenue
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div class="text-2xl font-bold text-emerald-600">
              R{{ pnl.totalRevenue.toFixed(2) }}
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle class="text-sm font-medium flex items-center gap-2">
              <TrendingDown class="h-4 w-4 text-red-600" />
              Total Expenses
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div class="text-2xl font-bold text-red-600">
              R{{ pnl.totalExpenses.toFixed(2) }}
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle class="text-sm font-medium flex items-center gap-2">
              <DollarSign class="h-4 w-4 text-primary" />
              Gross Profit
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div class="text-2xl font-bold" :class="pnl.grossProfit >= 0 ? 'text-emerald-600' : 'text-red-600'">
              R{{ pnl.grossProfit.toFixed(2) }}
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle class="text-sm font-medium flex items-center gap-2">
              <DollarSign class="h-4 w-4 text-primary" />
              Net Profit
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div class="text-2xl font-bold" :class="pnl.netProfit >= 0 ? 'text-emerald-600' : 'text-red-600'">
              R{{ pnl.netProfit.toFixed(2) }}
            </div>
          </CardContent>
        </Card>
      </div>

      <!-- Revenue Breakdown -->
      <Card>
        <CardHeader>
          <CardTitle>Revenue Breakdown</CardTitle>
        </CardHeader>
        <CardContent>
          <div v-if="pnl.revenueItems.length === 0" class="text-center py-8 text-muted-foreground">
            No revenue items for this period
          </div>
          <div v-else class="space-y-2">
            <div
              v-for="item in pnl.revenueItems"
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

      <!-- Expense Breakdown -->
      <Card>
        <CardHeader>
          <CardTitle>Expense Breakdown</CardTitle>
        </CardHeader>
        <CardContent>
          <div v-if="pnl.expenseItems.length === 0" class="text-center py-8 text-muted-foreground">
            No expense items for this period
          </div>
          <div v-else class="space-y-2">
            <div
              v-for="item in pnl.expenseItems"
              :key="item.category"
              class="flex items-center justify-between p-3 border rounded-lg"
            >
              <div>
                <div class="font-medium">{{ item.category }}</div>
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

