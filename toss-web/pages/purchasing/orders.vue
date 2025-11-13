<template>
  <div class="container mx-auto p-6 max-w-7xl">
    <!-- Header -->
    <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center mb-6">
      <div>
        <h1 class="text-3xl font-bold">Purchase Orders</h1>
        <p class="text-muted-foreground mt-1">
          Manage supplier orders and procurement
        </p>
      </div>
      <div class="flex gap-2 mt-4 sm:mt-0">
        <Button variant="outline" @click="showGroupBuyingModal = true">
          <Users class="mr-2 h-4 w-4" />
          Group Buying
        </Button>
        <Button variant="outline" @click="showReorderWizard = true">
          <Zap class="mr-2 h-4 w-4" />
          Smart Reorder
        </Button>
        <Button @click="showCreatePOModal = true">
          <Plus class="mr-2 h-4 w-4" />
          Create PO
        </Button>
      </div>
    </div>

    <!-- Overview Cards -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-6 mb-6">
      <Card>
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-muted-foreground">Active Orders</p>
              <h3 class="text-2xl font-bold">{{ activeOrders.length }}</h3>
            </div>
            <ShoppingCart class="h-8 w-8 text-blue-600" />
          </div>
        </CardContent>
      </Card>
      <Card>
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-muted-foreground">Pending Approvals</p>
              <h3 class="text-2xl font-bold text-orange-600">{{ pendingApprovals.length }}</h3>
            </div>
            <Clock class="h-8 w-8 text-orange-600" />
          </div>
        </CardContent>
      </Card>
      <Card>
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-muted-foreground">This Month</p>
              <h3 class="text-2xl font-bold">{{ formatCurrency(monthlySpend) }}</h3>
            </div>
            <DollarSign class="h-8 w-8 text-green-600" />
          </div>
        </CardContent>
      </Card>
      <Card>
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-muted-foreground">Avg Delivery</p>
              <h3 class="text-2xl font-bold">{{ averageDeliveryDays }} days</h3>
            </div>
            <Truck class="h-8 w-8 text-purple-600" />
          </div>
        </CardContent>
      </Card>
    </div>

    <!-- Smart Recommendations -->
    <Card v-if="smartRecommendations.length > 0" class="mb-6 border-blue-200 bg-blue-50/50">
      <CardContent class="p-6">
        <div class="flex items-start gap-3">
          <Lightbulb class="h-5 w-5 text-blue-600 mt-1" />
          <div class="flex-1">
            <h3 class="font-medium text-blue-800 mb-2">Smart Purchase Recommendations</h3>
            <div class="space-y-3">
              <div v-for="rec in smartRecommendations" :key="rec.id" class="flex items-center justify-between p-3 bg-white rounded-lg border">
                <div class="flex-1">
                  <p class="font-medium text-blue-900">{{ rec.title }}</p>
                  <p class="text-sm text-blue-700">{{ rec.description }}</p>
                  <p class="text-xs text-blue-600 mt-1">Potential savings: {{ formatCurrency(rec.savings) }}</p>
                </div>
                <Button size="sm" @click="applyRecommendation(rec)">
                  Apply
                </Button>
              </div>
            </div>
          </div>
        </div>
      </CardContent>
    </Card>

    <!-- Filters -->
    <Card class="mb-6">
      <CardContent class="p-6">
        <div class="grid grid-cols-1 md:grid-cols-5 gap-4">
          <div class="relative">
            <Search class="absolute left-3 top-3 h-4 w-4 text-muted-foreground" />
            <input
              v-model="searchTerm"
              type="text"
              placeholder="Search orders..."
              class="pl-10 w-full h-10 rounded-md border border-input bg-background px-3 py-2 text-sm"
            />
          </div>
          <select v-model="statusFilter" class="h-10 rounded-md border border-input bg-background px-3 py-2 text-sm">
            <option value="">All Statuses</option>
            <option value="draft">Draft</option>
            <option value="pending">Pending Approval</option>
            <option value="approved">Approved</option>
            <option value="sent">Sent to Supplier</option>
            <option value="delivered">Delivered</option>
            <option value="cancelled">Cancelled</option>
          </select>
          <select v-model="supplierFilter" class="h-10 rounded-md border border-input bg-background px-3 py-2 text-sm">
            <option value="">All Suppliers</option>
            <option v-for="supplier in suppliers" :key="supplier" :value="supplier">
              {{ supplier }}
            </option>
          </select>
          <input
            type="date"
            v-model="dateFilter"
            class="h-10 rounded-md border border-input bg-background px-3 py-2 text-sm"
          />
          <Button variant="outline" @click="resetFilters" class="h-10">
            <X class="mr-2 h-4 w-4" />
            Reset
          </Button>
        </div>
      </CardContent>
    </Card>

    <!-- Purchase Orders Table -->
    <Card>
      <CardContent class="p-0">
        <div class="overflow-x-auto">
          <table class="w-full">
            <thead>
              <tr class="border-b">
                <th class="text-left p-4 font-medium">PO Number</th>
                <th class="text-left p-4 font-medium">Supplier</th>
                <th class="text-left p-4 font-medium">Date</th>
                <th class="text-left p-4 font-medium">Total</th>
                <th class="text-left p-4 font-medium">Status</th>
                <th class="text-left p-4 font-medium">Expected Delivery</th>
                <th class="text-left p-4 font-medium">Progress</th>
                <th class="text-left p-4 font-medium">Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr 
                v-for="order in filteredOrders" 
                :key="order.id"
                class="border-b hover:bg-muted/50"
              >
                <td class="p-4">
                  <div>
                    <p class="font-medium">{{ order.poNumber }}</p>
                    <p class="text-sm text-muted-foreground">{{ order.items.length }} items</p>
                  </div>
                </td>
                <td class="p-4">
                  <div class="flex items-center gap-2">
                    <div class="w-8 h-8 rounded-lg bg-primary/10 flex items-center justify-center">
                      <Building class="h-4 w-4" />
                    </div>
                    <div>
                      <p class="font-medium">{{ order.supplier.name }}</p>
                      <p class="text-sm text-muted-foreground">{{ order.supplier.contact }}</p>
                    </div>
                  </div>
                </td>
                <td class="p-4">{{ formatDate(order.createdDate) }}</td>
                <td class="p-4 font-medium">{{ formatCurrency(order.total) }}</td>
                <td class="p-4">
                  <Badge :variant="getStatusVariant(order.status)">
                    {{ order.status }}
                  </Badge>
                </td>
                <td class="p-4">
                  <span v-if="order.expectedDelivery" class="text-sm">
                    {{ formatDate(order.expectedDelivery) }}
                  </span>
                  <span v-else class="text-sm text-muted-foreground">TBD</span>
                </td>
                <td class="p-4">
                  <div class="w-full bg-muted rounded-full h-2">
                    <div 
                      class="bg-primary rounded-full h-2 transition-all" 
                      :style="{ width: `${getProgressPercentage(order)}%` }"
                    ></div>
                  </div>
                  <p class="text-xs text-muted-foreground mt-1">{{ getProgressPercentage(order) }}%</p>
                </td>
                <td class="p-4">
                  <div class="flex gap-1">
                    <Button size="sm" variant="ghost" @click="viewOrder(order)">
                      <Eye class="h-4 w-4" />
                    </Button>
                    <Button size="sm" variant="ghost" @click="editOrder(order)">
                      <Edit class="h-4 w-4" />
                    </Button>
                    <Button 
                      v-if="order.status === 'approved'"
                      size="sm" 
                      variant="ghost" 
                      @click="sendToSupplier(order)"
                    >
                      <Send class="h-4 w-4" />
                    </Button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </CardContent>
    </Card>

    <!-- Group Buying Opportunities -->
    <Card v-if="groupBuyingOps.length > 0" class="mt-6">
      <CardContent class="p-6">
        <h3 class="font-medium mb-4 flex items-center gap-2">
          <Users class="h-5 w-5" />
          Group Buying Opportunities
        </h3>
        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div v-for="opp in groupBuyingOps" :key="opp.id" class="p-4 border rounded-lg">
            <div class="flex items-center justify-between mb-2">
              <h4 class="font-medium">{{ opp.product }}</h4>
              <Badge variant="secondary">{{ opp.participants }} shops</Badge>
            </div>
            <p class="text-sm text-muted-foreground mb-2">{{ opp.description }}</p>
            <div class="flex items-center justify-between">
              <span class="text-sm font-medium text-green-600">Save {{ opp.savings }}%</span>
              <Button size="sm" @click="joinGroupBuy(opp)">
                Join Group Buy
              </Button>
            </div>
          </div>
        </div>
      </CardContent>
    </Card>

    <!-- Modals -->
    <CreatePurchaseOrderModal
      v-model="showCreatePOModal"
      @created="handlePOCreated"
    />

    <GroupBuyingModal
      v-model="showGroupBuyingModal"
      :opportunities="groupBuyingOps"
      @join="joinGroupBuy"
    />

    <SmartReorderWizard
      v-model="showReorderWizard"
      @complete="handleReorderComplete"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { 
  Plus, 
  Search, 
  X,
  ShoppingCart,
  Clock,
  DollarSign,
  Truck,
  Users,
  Lightbulb,
  Building,
  Eye,
  Edit,
  Send,
  Zap
} from 'lucide-vue-next'

import { Button } from '../../components/ui/button'
import { Card, CardContent } from '../../components/ui/card'
import { Badge } from '../../components/ui/badge'

// Types
interface PurchaseOrder {
  id: string
  poNumber: string
  supplier: {
    id: string
    name: string
    contact: string
  }
  items: {
    id: string
    name: string
    quantity: number
    unitPrice: number
    total: number
  }[]
  total: number
  status: 'draft' | 'pending' | 'approved' | 'sent' | 'delivered' | 'cancelled'
  createdDate: Date
  expectedDelivery?: Date
  progress: number
}

interface SmartRecommendation {
  id: string
  title: string
  description: string
  savings: number
  type: 'group_buy' | 'bulk_discount' | 'seasonal'
}

interface GroupBuyingOpportunity {
  id: string
  product: string
  description: string
  participants: number
  savings: number
  minQuantity: number
  deadline: Date
}

// State
const searchTerm = ref('')
const statusFilter = ref('')
const supplierFilter = ref('')
const dateFilter = ref('')
const showCreatePOModal = ref(false)
const showGroupBuyingModal = ref(false)
const showReorderWizard = ref(false)

// Mock data
const purchaseOrders = ref<PurchaseOrder[]>([
  {
    id: '1',
    poNumber: 'PO-2024-001',
    supplier: {
      id: 'sup1',
      name: 'Township Wholesalers',
      contact: '+27114567890'
    },
    items: [
      { id: '1', name: 'Maize Meal 5kg', quantity: 50, unitPrice: 38.50, total: 1925 },
      { id: '2', name: 'Cooking Oil 750ml', quantity: 24, unitPrice: 28.00, total: 672 }
    ],
    total: 2597,
    status: 'delivered',
    createdDate: new Date(2024, 0, 10),
    expectedDelivery: new Date(2024, 0, 12),
    progress: 100
  },
  {
    id: '2',
    poNumber: 'PO-2024-002',
    supplier: {
      id: 'sup2',
      name: 'Albany Bakeries',
      contact: '+27118887766'
    },
    items: [
      { id: '3', name: 'White Bread', quantity: 100, unitPrice: 8.50, total: 850 }
    ],
    total: 850,
    status: 'sent',
    createdDate: new Date(2024, 0, 15),
    expectedDelivery: new Date(2024, 0, 17),
    progress: 75
  },
  {
    id: '3',
    poNumber: 'PO-2024-003',
    supplier: {
      id: 'sup3',
      name: 'Sunfoil Distributors',
      contact: '+27124569988'
    },
    items: [
      { id: '4', name: 'Cooking Oil 2L', quantity: 30, unitPrice: 65.00, total: 1950 }
    ],
    total: 1950,
    status: 'pending',
    createdDate: new Date(2024, 0, 16),
    progress: 25
  }
])

const smartRecommendations = ref<SmartRecommendation[]>([
  {
    id: '1',
    title: 'Group Buy: Maize Meal',
    description: 'Join 3 other shops to buy maize meal in bulk. Minimum order: 200 units.',
    savings: 450,
    type: 'group_buy'
  },
  {
    id: '2',
    title: 'Seasonal Discount: Soft Drinks',
    description: 'Summer season approaching. Stock up on beverages with 15% early bird discount.',
    savings: 320,
    type: 'seasonal'
  }
])

const groupBuyingOps = ref<GroupBuyingOpportunity[]>([
  {
    id: '1',
    product: 'Maize Meal 5kg',
    description: 'Bulk purchase with 3 other township shops. Minimum 200 units total.',
    participants: 3,
    savings: 15,
    minQuantity: 200,
    deadline: new Date(2024, 1, 25)
  },
  {
    id: '2',
    product: 'Cooking Oil 750ml',
    description: 'Weekly group purchase every Monday. Join standing order.',
    participants: 5,
    savings: 12,
    minQuantity: 100,
    deadline: new Date(2024, 1, 20)
  }
])

// Computed
const activeOrders = computed(() => 
  purchaseOrders.value.filter(po => !['delivered', 'cancelled'].includes(po.status))
)

const pendingApprovals = computed(() =>
  purchaseOrders.value.filter(po => po.status === 'pending')
)

const monthlySpend = computed(() =>
  purchaseOrders.value
    .filter(po => {
      const poMonth = po.createdDate.getMonth()
      const currentMonth = new Date().getMonth()
      return poMonth === currentMonth && po.status !== 'cancelled'
    })
    .reduce((sum, po) => sum + po.total, 0)
)

const averageDeliveryDays = computed(() => {
  const deliveredOrders = purchaseOrders.value.filter(po => po.status === 'delivered' && po.expectedDelivery)
  if (deliveredOrders.length === 0) return 0
  
  const totalDays = deliveredOrders.reduce((sum, po) => {
    const days = Math.ceil((po.expectedDelivery!.getTime() - po.createdDate.getTime()) / (1000 * 60 * 60 * 24))
    return sum + days
  }, 0)
  
  return Math.round(totalDays / deliveredOrders.length)
})

const suppliers = computed(() => {
  const sups = purchaseOrders.value.map(po => po.supplier.name)
  return [...new Set(sups)].sort()
})

const filteredOrders = computed(() => {
  return purchaseOrders.value.filter(order => {
    const matchesSearch = !searchTerm.value || 
      order.poNumber.toLowerCase().includes(searchTerm.value.toLowerCase()) ||
      order.supplier.name.toLowerCase().includes(searchTerm.value.toLowerCase())
    
    const matchesStatus = !statusFilter.value || order.status === statusFilter.value
    const matchesSupplier = !supplierFilter.value || order.supplier.name === supplierFilter.value
    
    let matchesDate = true
    if (dateFilter.value) {
      const filterDate = new Date(dateFilter.value)
      matchesDate = order.createdDate.toDateString() === filterDate.toDateString()
    }
    
    return matchesSearch && matchesStatus && matchesSupplier && matchesDate
  })
})

// Methods
const getStatusVariant = (status: string) => {
  switch (status) {
    case 'delivered': return 'default'
    case 'sent': return 'secondary'
    case 'approved': return 'outline'
    case 'pending': return 'destructive'
    case 'draft': return 'secondary'
    case 'cancelled': return 'outline'
    default: return 'outline'
  }
}

const getProgressPercentage = (order: PurchaseOrder) => {
  const statusProgress = {
    draft: 10,
    pending: 25,
    approved: 50,
    sent: 75,
    delivered: 100,
    cancelled: 0
  }
  return statusProgress[order.status] || 0
}

const viewOrder = (order: PurchaseOrder) => {
  console.log('Viewing order:', order.poNumber)
}

const editOrder = (order: PurchaseOrder) => {
  console.log('Editing order:', order.poNumber)
}

const sendToSupplier = (order: PurchaseOrder) => {
  console.log('Sending to supplier:', order.poNumber)
}

const applyRecommendation = (rec: SmartRecommendation) => {
  console.log('Applying recommendation:', rec.title)
}

const joinGroupBuy = (opp: GroupBuyingOpportunity) => {
  console.log('Joining group buy:', opp.product)
}

const resetFilters = () => {
  searchTerm.value = ''
  statusFilter.value = ''
  supplierFilter.value = ''
  dateFilter.value = ''
}

const handlePOCreated = (po: any) => {
  console.log('PO created:', po)
}

const handleReorderComplete = (orders: any) => {
  console.log('Reorder complete:', orders)
}

const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-ZA', {
    style: 'currency',
    currency: 'ZAR',
    minimumFractionDigits: 0
  }).format(amount)
}

const formatDate = (date: Date) => {
  return new Intl.DateTimeFormat('en-ZA', {
    day: 'numeric',
    month: 'short',
    year: 'numeric'
  }).format(date)
}

onMounted(() => {
  console.log('Purchase Orders loaded')
})
</script>