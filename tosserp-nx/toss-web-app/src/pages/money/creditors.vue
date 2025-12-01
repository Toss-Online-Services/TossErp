<script setup lang="ts">
import { ref, onMounted } from 'vue'
import Card from '@/components/ui/Card.vue'
import CardHeader from '@/components/ui/CardHeader.vue'
import CardTitle from '@/components/ui/CardTitle.vue'
import CardContent from '@/components/ui/CardContent.vue'
import Breadcrumbs from '@/components/ui/Breadcrumbs.vue'
import Button from '@/components/ui/Button.vue'
import { Building2, Search, Filter, AlertCircle } from 'lucide-vue-next'
import { useAccountingApi } from '@/composables/useAccountingApi'

const { getCreditors, isLoading } = useAccountingApi()

const creditors = ref<any[]>([])
const searchQuery = ref('')
const overdueOnly = ref(false)
const currentPage = ref(1)

const loadCreditors = async () => {
  try {
    const result = await getCreditors({
      overdueOnly: overdueOnly.value,
      pageNumber: currentPage.value,
      pageSize: 20
    })
    creditors.value = result.items
  } catch (error) {
    console.error('Failed to load creditors:', error)
  }
}

onMounted(() => {
  loadCreditors()
})
</script>

<template>
  <div class="space-y-6">
    <div class="flex items-center justify-between">
      <div>
        <Breadcrumbs />
        <h1 class="text-2xl md:text-3xl font-bold tracking-tight mt-2">You Owe Suppliers</h1>
        <p class="text-muted-foreground mt-1">Track outstanding vendor invoices</p>
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
              placeholder="Search vendors..."
              class="w-full pl-10 pr-4 py-2 bg-background border rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
            />
          </div>
          <Button
            variant="outline"
            :class="{ 'bg-amber-50 border-amber-200': overdueOnly }"
            @click="overdueOnly = !overdueOnly; loadCreditors()"
          >
            <AlertCircle :size="18" class="mr-2" />
            Overdue Only
          </Button>
        </div>
      </CardContent>
    </Card>

    <!-- Creditors List -->
    <Card>
      <CardHeader>
        <CardTitle>Outstanding Invoices</CardTitle>
      </CardHeader>
      <CardContent>
        <div v-if="isLoading" class="text-center py-12 text-muted-foreground">
          Loading creditors...
        </div>
        <div v-else-if="creditors.length === 0" class="text-center py-12 text-muted-foreground">
          <Building2 :size="48" class="mx-auto mb-3 opacity-50" />
          <p>No outstanding invoices</p>
          <p class="text-sm mt-1">All vendor invoices are paid</p>
        </div>
        <div v-else class="space-y-3">
          <div
            v-for="creditor in creditors"
            :key="creditor.vendorId"
            class="p-4 border rounded-lg hover:bg-accent/50 transition-colors"
          >
            <div class="flex items-start justify-between">
              <div class="flex-1">
                <div class="font-medium text-lg">{{ creditor.vendorName }}</div>
                <div class="text-sm text-muted-foreground mt-1">
                  {{ creditor.invoiceCount }} outstanding invoice{{ creditor.invoiceCount !== 1 ? 's' : '' }}
                </div>
              </div>
              <div class="text-right">
                <div class="text-xl font-bold text-red-600">
                  R{{ creditor.totalOutstanding.toFixed(2) }}
                </div>
                <div v-if="creditor.latestDueDate" class="text-sm text-muted-foreground">
                  Due: {{ new Date(creditor.latestDueDate).toLocaleDateString() }}
                </div>
              </div>
            </div>
          </div>
        </div>
      </CardContent>
    </Card>
  </div>
</template>

