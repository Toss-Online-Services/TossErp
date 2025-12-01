<script setup lang="ts">
import { ref, onMounted } from 'vue'
import Card from '@/components/ui/Card.vue'
import CardHeader from '@/components/ui/CardHeader.vue'
import CardTitle from '@/components/ui/CardTitle.vue'
import CardContent from '@/components/ui/CardContent.vue'
import Breadcrumbs from '@/components/ui/Breadcrumbs.vue'
import Button from '@/components/ui/Button.vue'
import { Truck, Plus, Search, Filter, MapPin, User, Calendar, CheckCircle2, Clock, XCircle } from 'lucide-vue-next'
import { useLogisticsApi } from '@/composables/useLogisticsApi'

const { getSharedRuns, isLoading } = useLogisticsApi()

const deliveries = ref<any[]>([])
const searchQuery = ref('')
const statusFilter = ref<string | null>(null)

const today = new Date()
const startOfWeek = new Date(today)
startOfWeek.setDate(today.getDate() - today.getDay())
const endOfWeek = new Date(startOfWeek)
endOfWeek.setDate(startOfWeek.getDate() + 6)

const loadDeliveries = async () => {
  try {
    const result = await getSharedRuns({
      startDate: startOfWeek.toISOString(),
      endDate: endOfWeek.toISOString(),
      status: statusFilter.value || undefined
    })
    deliveries.value = result
  } catch (error) {
    console.error('Failed to load deliveries:', error)
  }
}

const getStatusIcon = (status: string) => {
  switch (status.toLowerCase()) {
    case 'completed':
    case 'delivered':
      return CheckCircle2
    case 'inprogress':
    case 'in_transit':
      return Clock
    case 'cancelled':
      return XCircle
    default:
      return Clock
  }
}

const getStatusColor = (status: string) => {
  switch (status.toLowerCase()) {
    case 'completed':
    case 'delivered':
      return 'text-emerald-600'
    case 'inprogress':
    case 'in_transit':
      return 'text-blue-600'
    case 'cancelled':
      return 'text-red-600'
    default:
      return 'text-gray-600'
  }
}

onMounted(() => {
  loadDeliveries()
})
</script>

<template>
  <div class="space-y-6">
    <div class="flex items-center justify-between">
      <div>
        <Breadcrumbs />
        <h1 class="text-2xl md:text-3xl font-bold tracking-tight mt-2">Deliveries</h1>
        <p class="text-muted-foreground mt-1">Track deliveries and driver assignments</p>
      </div>
      <Button>
        <Plus :size="18" class="mr-2" />
        New Delivery Run
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
              placeholder="Search deliveries..."
              class="w-full pl-10 pr-4 py-2 bg-background border rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
            />
          </div>
          <select
            v-model="statusFilter"
            @change="loadDeliveries"
            class="px-4 py-2 bg-background border rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
          >
            <option :value="null">All Statuses</option>
            <option value="Pending">Pending</option>
            <option value="InProgress">In Progress</option>
            <option value="Completed">Completed</option>
            <option value="Cancelled">Cancelled</option>
          </select>
        </div>
      </CardContent>
    </Card>

    <!-- Deliveries List -->
    <div class="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
      <div v-if="isLoading" class="col-span-full text-center py-12 text-muted-foreground">
        Loading deliveries...
      </div>
      <div v-else-if="deliveries.length === 0" class="col-span-full text-center py-12 text-muted-foreground">
        <Truck :size="48" class="mx-auto mb-3 opacity-50" />
        <p>No deliveries found</p>
        <p class="text-sm mt-1">Create a new delivery run to get started</p>
      </div>
      <Card
        v-for="delivery in deliveries"
        :key="delivery.id"
        class="hover:shadow-material-md transition-shadow cursor-pointer"
      >
        <CardHeader>
          <div class="flex items-center justify-between">
            <CardTitle class="text-lg">{{ delivery.runNumber }}</CardTitle>
            <component
              :is="getStatusIcon(delivery.status)"
              :size="20"
              :class="getStatusColor(delivery.status)"
            />
          </div>
        </CardHeader>
        <CardContent>
          <div class="space-y-3">
            <div class="flex items-center gap-2 text-sm text-muted-foreground">
              <Calendar :size="14" />
              {{ new Date(delivery.scheduledDate).toLocaleDateString() }}
            </div>
            <div class="flex items-center gap-2 text-sm text-muted-foreground">
              <MapPin :size="14" />
              {{ delivery.stopCount }} stop{{ delivery.stopCount !== 1 ? 's' : '' }}
            </div>
            <div v-if="delivery.driverName" class="flex items-center gap-2 text-sm text-muted-foreground">
              <User :size="14" />
              {{ delivery.driverName }}
            </div>
            <div class="pt-2 border-t">
              <div class="text-sm text-muted-foreground">Total Cost</div>
              <div class="text-lg font-semibold text-primary">
                R{{ delivery.totalCost.toFixed(2) }}
              </div>
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
  </div>
</template>

