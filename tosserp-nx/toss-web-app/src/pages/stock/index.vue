<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import Card from '@/components/ui/Card.vue'
import CardHeader from '@/components/ui/CardHeader.vue'
import CardTitle from '@/components/ui/CardTitle.vue'
import CardContent from '@/components/ui/CardContent.vue'
import Breadcrumbs from '@/components/ui/Breadcrumbs.vue'
import KpiCard from '@/components/ui/KpiCard.vue'
import Button from '@/components/ui/Button.vue'
import { Boxes, AlertTriangle, Plus, ArrowRightLeft, TrendingDown, Package, ArrowRight } from 'lucide-vue-next'
import { useInventoryApi } from '@/composables/useInventoryApi'

const { getLowStockAlerts, getStockOnHand, isLoading } = useInventoryApi()

const shopId = ref(1) // TODO: Get from auth/context
const lowStockAlerts = ref<any[]>([])
const stockOnHand = ref<any[]>([])

const totalProducts = computed(() => stockOnHand.value.length)
const totalStockValue = computed(() => 
  stockOnHand.value.reduce((sum, item) => sum + (item.availableStock * item.averageCost), 0)
)
const lowStockCount = computed(() => lowStockAlerts.value.length)

const loadData = async () => {
  try {
    const [alerts, stock] = await Promise.all([
      getLowStockAlerts(shopId.value),
      getStockOnHand({ shopId: shopId.value })
    ])
    lowStockAlerts.value = alerts
    stockOnHand.value = stock
  } catch (error) {
    console.error('Failed to load stock data:', error)
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
        <h1 class="text-2xl md:text-3xl font-bold tracking-tight mt-2">Stock</h1>
        <p class="text-muted-foreground mt-1">Manage inventory and stock levels</p>
      </div>
      <div class="flex gap-2">
        <Button variant="outline">
          <ArrowRightLeft :size="18" class="mr-2" />
          Transfer
        </Button>
        <Button>
          <Plus :size="18" class="mr-2" />
          Add Product
        </Button>
      </div>
    </div>

    <!-- KPIs -->
    <div class="grid gap-4 md:grid-cols-3">
      <KpiCard
        title="Total Products"
        :value="totalProducts"
        :icon="Boxes"
        status="neutral"
      />
      <KpiCard
        title="Stock Value"
        :value="totalStockValue"
        :icon="Package"
        status="neutral"
      />
      <KpiCard
        title="Low Stock Items"
        :value="lowStockCount"
        :icon="AlertTriangle"
        :status="lowStockCount > 0 ? 'warning' : 'good'"
      />
    </div>

    <!-- Low Stock Alerts -->
    <Card v-if="lowStockAlerts.length > 0">
      <CardHeader>
        <CardTitle class="flex items-center gap-2">
          <AlertTriangle class="h-5 w-5 text-amber-600" />
          Low Stock Alerts ({{ lowStockAlerts.length }})
        </CardTitle>
      </CardHeader>
      <CardContent>
        <div class="space-y-3">
          <div
            v-for="alert in lowStockAlerts.slice(0, 5)"
            :key="alert.id"
            class="flex items-center justify-between p-3 border rounded-lg hover:bg-accent/50 transition-colors"
          >
            <div class="flex-1">
              <div class="font-medium text-foreground">{{ alert.productName }}</div>
              <div class="text-sm text-muted-foreground">
                SKU: {{ alert.productSKU }} • Current: {{ alert.currentStock }} • Min: {{ alert.minimumStock }}
              </div>
            </div>
            <div class="flex items-center gap-2">
              <span class="text-sm font-semibold text-amber-600">
                {{ alert.currentStock - alert.minimumStock }} below minimum
              </span>
              <Button variant="outline" size="sm">
                Create PO
              </Button>
            </div>
          </div>
          <div v-if="lowStockAlerts.length > 5" class="text-center pt-2">
            <Button variant="ghost" size="sm">
              View all {{ lowStockAlerts.length }} alerts
              <ArrowRight :size="16" class="ml-1" />
            </Button>
          </div>
        </div>
      </CardContent>
    </Card>

    <!-- Quick Actions -->
    <div class="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
      <Card class="hover:shadow-material-md transition-shadow cursor-pointer">
        <CardHeader>
          <CardTitle class="flex items-center gap-2">
            <Package class="h-5 w-5 text-primary" />
            Stock On Hand
          </CardTitle>
        </CardHeader>
        <CardContent>
          <p class="text-sm text-muted-foreground mb-3">
            View current stock levels for all products
          </p>
          <Button variant="outline" class="w-full">
            View Stock Levels
          </Button>
        </CardContent>
      </Card>

      <Card class="hover:shadow-material-md transition-shadow cursor-pointer">
        <CardHeader>
          <CardTitle class="flex items-center gap-2">
            <TrendingDown class="h-5 w-5 text-primary" />
            Adjust Stock
          </CardTitle>
        </CardHeader>
        <CardContent>
          <p class="text-sm text-muted-foreground mb-3">
            Manually adjust stock quantities
          </p>
          <Button variant="outline" class="w-full">
            Adjust Stock
          </Button>
        </CardContent>
      </Card>

      <Card class="hover:shadow-material-md transition-shadow cursor-pointer">
        <CardHeader>
          <CardTitle class="flex items-center gap-2">
            <ArrowRightLeft class="h-5 w-5 text-primary" />
            Transfer Stock
          </CardTitle>
        </CardHeader>
        <CardContent>
          <p class="text-sm text-muted-foreground mb-3">
            Transfer stock between locations
          </p>
          <Button variant="outline" class="w-full">
            Transfer Stock
          </Button>
        </CardContent>
      </Card>
    </div>
  </div>
</template>

