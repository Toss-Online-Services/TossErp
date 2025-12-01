<script setup lang="ts">
import { ref, onMounted } from 'vue'
import Card from '@/components/ui/Card.vue'
import CardHeader from '@/components/ui/CardHeader.vue'
import CardTitle from '@/components/ui/CardTitle.vue'
import CardContent from '@/components/ui/CardContent.vue'
import Breadcrumbs from '@/components/ui/Breadcrumbs.vue'
import Button from '@/components/ui/Button.vue'
import { FileText, Plus, Search, Filter, TrendingUp, TrendingDown } from 'lucide-vue-next'
import { useAccountingApi } from '@/composables/useAccountingApi'

const { getCashbookEntries, isLoading } = useAccountingApi()

const entries = ref<any[]>([])
const searchQuery = ref('')
const currentPage = ref(1)
const pageSize = ref(20)

const loadEntries = async () => {
  try {
    const today = new Date()
    const startOfMonth = new Date(today.getFullYear(), today.getMonth(), 1)
    const endOfMonth = new Date(today.getFullYear(), today.getMonth() + 1, 0, 23, 59, 59)

    const result = await getCashbookEntries({
      fromDate: startOfMonth.toISOString(),
      toDate: endOfMonth.toISOString(),
      pageNumber: currentPage.value,
      pageSize: pageSize.value
    })
    entries.value = result.items
  } catch (error) {
    console.error('Failed to load cashbook entries:', error)
  }
}

onMounted(() => {
  loadEntries()
})
</script>

<template>
  <div class="space-y-6">
    <div class="flex items-center justify-between">
      <div>
        <Breadcrumbs />
        <h1 class="text-2xl md:text-3xl font-bold tracking-tight mt-2">Cashbook</h1>
        <p class="text-muted-foreground mt-1">All money in and money out transactions</p>
      </div>
      <Button>
        <Plus :size="18" class="mr-2" />
        New Entry
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
              placeholder="Search cashbook entries..."
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

    <!-- Entries List -->
    <Card>
      <CardHeader>
        <CardTitle>Cashbook Entries</CardTitle>
      </CardHeader>
      <CardContent>
        <div v-if="isLoading" class="text-center py-12 text-muted-foreground">
          Loading cashbook entries...
        </div>
        <div v-else-if="entries.length === 0" class="text-center py-12 text-muted-foreground">
          <FileText :size="48" class="mx-auto mb-3 opacity-50" />
          <p>No cashbook entries found</p>
          <p class="text-sm mt-1">Transactions will appear here as they are recorded</p>
        </div>
        <div v-else class="space-y-2">
          <!-- Entry items will go here -->
        </div>
      </CardContent>
    </Card>
  </div>
</template>

