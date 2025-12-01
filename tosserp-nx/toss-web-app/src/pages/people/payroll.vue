<script setup lang="ts">
import { ref, onMounted } from 'vue'
import Card from '@/components/ui/Card.vue'
import CardHeader from '@/components/ui/CardHeader.vue'
import CardTitle from '@/components/ui/CardTitle.vue'
import CardContent from '@/components/ui/CardContent.vue'
import Breadcrumbs from '@/components/ui/Breadcrumbs.vue'
import Button from '@/components/ui/Button.vue'
import { DollarSign, Plus, Search, Filter, Calendar, Download } from 'lucide-vue-next'
import { useHrApi } from '@/composables/useHrApi'

const { getPayrollRuns, isLoading } = useHrApi()

const payrollRuns = ref<any[]>([])
const searchQuery = ref('')
const currentPage = ref(1)
const totalCount = ref(0)

const today = new Date()
const startOfMonth = new Date(today.getFullYear(), today.getMonth(), 1)
const endOfMonth = new Date(today.getFullYear(), today.getMonth() + 1, 0)

const loadPayrollRuns = async () => {
  try {
    const result = await getPayrollRuns({
      fromDate: startOfMonth.toISOString(),
      toDate: endOfMonth.toISOString(),
      pageNumber: currentPage.value,
      pageSize: 20
    })
    payrollRuns.value = result.items
    totalCount.value = result.totalCount
  } catch (error) {
    console.error('Failed to load payroll runs:', error)
  }
}

onMounted(() => {
  loadPayrollRuns()
})
</script>

<template>
  <div class="space-y-6">
    <div class="flex items-center justify-between">
      <div>
        <Breadcrumbs />
        <h1 class="text-2xl md:text-3xl font-bold tracking-tight mt-2">Payroll</h1>
        <p class="text-muted-foreground mt-1">Run payroll and view payment history</p>
      </div>
      <Button>
        <Plus :size="18" class="mr-2" />
        Run Payroll
      </Button>
    </div>

    <!-- Filters -->
    <Card>
      <CardContent class="pt-6">
        <div class="flex flex-col md:flex-row gap-4">
          <div class="flex-1 relative">
            <Search
              :size="18"
              class="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground"
            />
            <input
              v-model="searchQuery"
              type="text"
              placeholder="Search payroll runs..."
              class="w-full pl-10 pr-4 py-2 bg-background border rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
            />
          </div>
          <Button variant="outline">
            <Filter :size="18" class="mr-2" />
            Filter
          </Button>
        </div>
      </CardContent>
    </Card>

    <!-- Payroll Runs List -->
    <Card>
      <CardHeader>
        <CardTitle>Payroll Runs ({{ totalCount }})</CardTitle>
      </CardHeader>
      <CardContent>
        <div v-if="isLoading" class="text-center py-12 text-muted-foreground">
          Loading payroll runs...
        </div>
        <div v-else-if="payrollRuns.length === 0" class="text-center py-12 text-muted-foreground">
          <DollarSign :size="48" class="mx-auto mb-3 opacity-50" />
          <p>No payroll runs found</p>
          <p class="text-sm mt-1">Run your first payroll to get started</p>
        </div>
        <div v-else class="space-y-3">
          <div
            v-for="run in payrollRuns"
            :key="run.id"
            class="p-4 border rounded-lg hover:bg-accent/50 transition-colors"
          >
            <div class="flex items-start justify-between">
              <div class="flex-1">
                <div class="font-medium text-lg">{{ run.employeeName }}</div>
                <div class="text-sm text-muted-foreground mt-1">
                  Period: {{ new Date(run.periodStart).toLocaleDateString() }} - 
                  {{ new Date(run.periodEnd).toLocaleDateString() }}
                </div>
                <div class="text-sm text-muted-foreground">
                  Generated: {{ new Date(run.generatedAt).toLocaleDateString() }}
                </div>
              </div>
              <div class="text-right">
                <div class="text-sm text-muted-foreground">Gross: R{{ run.gross.toFixed(2) }}</div>
                <div class="text-sm text-muted-foreground">Deductions: R{{ run.deductions.toFixed(2) }}</div>
                <div class="text-lg font-bold text-primary mt-1">Net: R{{ run.net.toFixed(2) }}</div>
                <Button variant="ghost" size="sm" class="mt-2">
                  <Download :size="14" class="mr-1" />
                  Export
                </Button>
              </div>
            </div>
          </div>
        </div>
      </CardContent>
    </Card>
  </div>
</template>

