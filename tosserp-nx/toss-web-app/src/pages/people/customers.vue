<script setup lang="ts">
import { ref, onMounted } from 'vue'
import Card from '@/components/ui/Card.vue'
import CardHeader from '@/components/ui/CardHeader.vue'
import CardTitle from '@/components/ui/CardTitle.vue'
import CardContent from '@/components/ui/CardContent.vue'
import Breadcrumbs from '@/components/ui/Breadcrumbs.vue'
import Button from '@/components/ui/Button.vue'
import { Users, Plus, Search, Filter, Phone, Mail } from 'lucide-vue-next'
import { useCrmApi } from '@/composables/useCrmApi'

const { getCustomers, isLoading } = useCrmApi()

const customers = ref<any[]>([])
const searchQuery = ref('')
const currentPage = ref(1)
const totalCount = ref(0)

const loadCustomers = async () => {
  try {
    const result = await getCustomers({
      searchTerm: searchQuery.value || undefined,
      pageNumber: currentPage.value,
      pageSize: 20
    })
    customers.value = result.items
    totalCount.value = result.totalCount
  } catch (error) {
    console.error('Failed to load customers:', error)
  }
}

onMounted(() => {
  loadCustomers()
})
</script>

<template>
  <div class="space-y-6">
    <div class="flex items-center justify-between">
      <div>
        <Breadcrumbs />
        <h1 class="text-2xl md:text-3xl font-bold tracking-tight mt-2">Customers</h1>
        <p class="text-muted-foreground mt-1">Manage customer relationships and interactions</p>
      </div>
      <Button>
        <Plus :size="18" class="mr-2" />
        Add Customer
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
              @input="loadCustomers"
              type="text"
              placeholder="Search customers by name, email, or phone..."
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

    <!-- Customers List -->
    <Card>
      <CardHeader>
        <CardTitle>Customers ({{ totalCount }})</CardTitle>
      </CardHeader>
      <CardContent>
        <div v-if="isLoading" class="text-center py-12 text-muted-foreground">
          Loading customers...
        </div>
        <div v-else-if="customers.length === 0" class="text-center py-12 text-muted-foreground">
          <Users :size="48" class="mx-auto mb-3 opacity-50" />
          <p>No customers found</p>
          <p class="text-sm mt-1">Add your first customer to get started</p>
        </div>
        <div v-else class="space-y-3">
          <div
            v-for="customer in customers"
            :key="customer.id"
            class="p-4 border rounded-lg hover:bg-accent/50 transition-colors cursor-pointer"
          >
            <div class="flex items-start justify-between">
              <div class="flex-1">
                <div class="font-medium text-lg">
                  {{ customer.firstName }} {{ customer.lastName }}
                </div>
                <div class="flex items-center gap-4 mt-2 text-sm text-muted-foreground">
                  <div v-if="customer.phoneNumber" class="flex items-center gap-1">
                    <Phone :size="14" />
                    {{ customer.phoneNumber }}
                  </div>
                  <div v-if="customer.email" class="flex items-center gap-1">
                    <Mail :size="14" />
                    {{ customer.email }}
                  </div>
                </div>
                <div class="mt-2 text-sm">
                  <span class="text-muted-foreground">Total purchases:</span>
                  <span class="font-semibold ml-1">R{{ customer.totalPurchases.toFixed(2) }}</span>
                </div>
              </div>
              <div class="text-right">
                <div v-if="customer.lastPurchaseDate" class="text-sm text-muted-foreground">
                  Last purchase: {{ new Date(customer.lastPurchaseDate).toLocaleDateString() }}
                </div>
              </div>
            </div>
          </div>
        </div>
      </CardContent>
    </Card>
  </div>
</template>

