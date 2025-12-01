<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import Card from '@/components/ui/Card.vue'
import CardHeader from '@/components/ui/CardHeader.vue'
import CardTitle from '@/components/ui/CardTitle.vue'
import CardContent from '@/components/ui/CardContent.vue'
import Breadcrumbs from '@/components/ui/Breadcrumbs.vue'
import KpiCard from '@/components/ui/KpiCard.vue'
import Button from '@/components/ui/Button.vue'
import { DollarSign, TrendingUp, TrendingDown, FileText, Users, Building2, ArrowRight } from 'lucide-vue-next'
import { useAccountingApi } from '@/composables/useAccountingApi'

const { getCashflowSummary, getAccounts, isLoading } = useAccountingApi()

const shopId = ref(1) // TODO: Get from auth/context
const cashflow = ref<any>(null)
const accounts = ref<any[]>([])

const today = new Date()
const startOfToday = new Date(today.getFullYear(), today.getMonth(), today.getDate())
const endOfToday = new Date(today.getFullYear(), today.getMonth(), today.getDate(), 23, 59, 59)

const moneyIn = computed(() => cashflow.value?.totalCashIn || 0)
const moneyOut = computed(() => cashflow.value?.totalCashOut || 0)
const whatsLeft = computed(() => cashflow.value?.closingBalance || 0)
const totalAccounts = computed(() => accounts.value.length)
const totalAccountBalance = computed(() => 
  accounts.value.reduce((sum, acc) => sum + acc.balance, 0)
)

const loadData = async () => {
  try {
    const [cashflowData, accountsData] = await Promise.all([
      getCashflowSummary({
        fromDate: startOfToday.toISOString(),
        toDate: endOfToday.toISOString()
      }),
      getAccounts(shopId.value)
    ])
    cashflow.value = cashflowData
    accounts.value = accountsData
  } catch (error) {
    console.error('Failed to load money data:', error)
  }
}

onMounted(() => {
  loadData()
})
</script>

<template>
  <div class="space-y-6">
    <div class="flex items-center justify-between">
      <div>
        <Breadcrumbs />
        <h1 class="text-2xl md:text-3xl font-bold tracking-tight mt-2">Money</h1>
        <p class="text-muted-foreground mt-1">Track money in, money out, and what's left</p>
      </div>
    </div>

    <!-- KPIs -->
    <div class="grid gap-4 md:grid-cols-3">
      <KpiCard
        title="Money In"
        :value="moneyIn"
        :icon="TrendingUp"
        status="good"
        change-label="Today"
      />
      <KpiCard
        title="Money Out"
        :value="moneyOut"
        :icon="TrendingDown"
        status="bad"
        change-label="Today"
      />
      <KpiCard
        title="What's Left"
        :value="whatsLeft"
        :icon="DollarSign"
        :status="whatsLeft >= 0 ? 'good' : 'bad'"
        change-label="Available"
      />
    </div>

    <!-- Accounts Summary -->
    <div class="grid gap-4 md:grid-cols-2">
      <Card>
        <CardHeader>
          <CardTitle class="flex items-center gap-2">
            <DollarSign class="h-5 w-5 text-primary" />
            Accounts
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div class="space-y-3">
            <div class="flex items-center justify-between">
              <span class="text-sm text-muted-foreground">Total Accounts</span>
              <span class="font-semibold">{{ totalAccounts }}</span>
            </div>
            <div class="flex items-center justify-between">
              <span class="text-sm text-muted-foreground">Total Balance</span>
              <span class="font-semibold text-primary">
                R{{ totalAccountBalance.toFixed(2) }}
              </span>
            </div>
            <Button variant="outline" class="w-full mt-4">
              View All Accounts
              <ArrowRight :size="16" class="ml-2" />
            </Button>
          </div>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle class="flex items-center gap-2">
            <FileText class="h-5 w-5 text-primary" />
            Quick Reports
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div class="space-y-2">
            <NuxtLink to="/money/reports/profit-loss" class="block">
              <Button variant="outline" class="w-full justify-start">
                <TrendingUp :size="16" class="mr-2" />
                Profit & Loss
              </Button>
            </NuxtLink>
            <NuxtLink to="/money/reports/cashflow" class="block">
              <Button variant="outline" class="w-full justify-start">
                <DollarSign :size="16" class="mr-2" />
                Cashflow Report
              </Button>
            </NuxtLink>
          </div>
        </CardContent>
      </Card>
    </div>

    <!-- Debtors & Creditors -->
    <div class="grid gap-4 md:grid-cols-2">
      <Card>
        <CardHeader>
          <CardTitle class="flex items-center gap-2">
            <Users class="h-5 w-5 text-primary" />
            People Who Owe You
          </CardTitle>
        </CardHeader>
        <CardContent>
          <p class="text-sm text-muted-foreground mb-3">
            Track outstanding customer invoices
          </p>
          <NuxtLink to="/money/debtors">
            <Button variant="outline" class="w-full">
              View Debtors
              <ArrowRight :size="16" class="ml-2" />
            </Button>
          </NuxtLink>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle class="flex items-center gap-2">
            <Building2 class="h-5 w-5 text-primary" />
            You Owe Suppliers
          </CardTitle>
        </CardHeader>
        <CardContent>
          <p class="text-sm text-muted-foreground mb-3">
            Track outstanding vendor invoices
          </p>
          <NuxtLink to="/money/creditors">
            <Button variant="outline" class="w-full">
              View Creditors
              <ArrowRight :size="16" class="ml-2" />
            </Button>
          </NuxtLink>
        </CardContent>
      </Card>
    </div>

    <!-- Cashbook -->
    <Card>
      <CardHeader>
        <CardTitle class="flex items-center gap-2">
          <FileText class="h-5 w-5 text-primary" />
          Recent Cashbook Entries
        </CardTitle>
      </CardHeader>
      <CardContent>
        <p class="text-sm text-muted-foreground mb-3">
          View all money in and money out transactions
        </p>
        <NuxtLink to="/money/cashbook">
          <Button variant="outline" class="w-full">
            View Cashbook
            <ArrowRight :size="16" class="ml-2" />
          </Button>
        </NuxtLink>
      </CardContent>
    </Card>
  </div>
</template>

