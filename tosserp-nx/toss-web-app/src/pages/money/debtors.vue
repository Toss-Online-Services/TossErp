<script setup lang="ts">
import { ref, onMounted } from 'vue'
import Card from '@/components/ui/Card.vue'
import CardHeader from '@/components/ui/CardHeader.vue'
import CardTitle from '@/components/ui/CardTitle.vue'
import CardContent from '@/components/ui/CardContent.vue'
import Breadcrumbs from '@/components/ui/Breadcrumbs.vue'
import Button from '@/components/ui/Button.vue'
import { Users, Search, Filter, AlertCircle } from 'lucide-vue-next'
import { useAccountingApi } from '@/composables/useAccountingApi'

const { getDebtors, isLoading } = useAccountingApi()

const debtors = ref<any[]>([])
const searchQuery = ref('')
const overdueOnly = ref(false)
const currentPage = ref(1)

const loadDebtors = async () => {
  try {
    const result = await getDebtors({
      overdueOnly: overdueOnly.value,
      pageNumber: currentPage.value,
      pageSize: 20
    })
    debtors.value = result.items
  } catch (error) {
    console.error('Failed to load debtors:', error)
  }
}

onMounted(() => {
  loadDebtors()
})
</script>

<template>
  <div class="space-y-6">
    <div class="flex items-center justify-between">
      <div>
        <Breadcrumbs />
        <h1 class="text-2xl md:text-3xl font-bold tracking-tight mt-2">People Who Owe You</h1>
        <p class="text-muted-foreground mt-1">Track outstanding customer invoices</p>
      </div>
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
              placeholder="Search customers..."
              class="w-full pl-10 pr-4 py-2 bg-background border rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
            />
          </div>
          <Button
            variant="outline"
            :class="{ 'bg-amber-50 border-amber-200': overdueOnly }"
            @click="overdueOnly = !overdueOnly; loadDebtors()"
          >
            <AlertCircle :size="18" class="mr-2" />
            Overdue Only
          </Button>
        </div>
      </CardContent>
    </Card>

    <!-- Debtors List -->
    <Card>
      <CardHeader>
        <CardTitle>Outstanding Invoices</CardTitle>
      </CardHeader>
      <CardContent>
        <div v-if="isLoading" class="text-center py-12 text-muted-foreground">
          Loading debtors...
        </div>
        <div v-else-if="debtors.length === 0" class="text-center py-12 text-muted-foreground">
          <Users :size="48" class="mx-auto mb-3 opacity-50" />
          <p>No outstanding invoices</p>
          <p class="text-sm mt-1">All customers are up to date with payments</p>
        </div>
        <div v-else class="space-y-3">
          <div
            v-for="debtor in debtors"
            :key="debtor.customerId"
            class="p-4 border rounded-lg hover:bg-accent/50 transition-colors"
          >
            <div class="flex items-start justify-between">
              <div class="flex-1">
                <div class="font-medium text-lg">{{ debtor.customerName }}</div>
                <div class="text-sm text-muted-foreground mt-1">
                  {{ debtor.invoiceCount }} outstanding invoice{{ debtor.invoiceCount !== 1 ? 's' : '' }}
                </div>
                <div v-if="debtor.customerPhone" class="text-sm text-muted-foreground">
                  {{ debtor.customerPhone }}
                </div>
              </div>
              <div class="text-right">
                <div class="text-xl font-bold text-primary">
                  R{{ debtor.totalOutstanding.toFixed(2) }}
                </div>
                <div v-if="debtor.latestDueDate" class="text-sm text-muted-foreground">
                  Due: {{ new Date(debtor.latestDueDate).toLocaleDateString() }}
                </div>
              </div>
            </div>
          </div>
        </div>
      </CardContent>
    </Card>
  </div>
</template>

