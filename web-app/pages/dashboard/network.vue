<template>
  <div class="space-y-6">
    <div>
      <h1 class="text-3xl font-bold">Collaborative Network</h1>
      <p class="text-muted-foreground">Connect with other businesses and leverage group buying power</p>
    </div>
    
    <!-- Network Stats -->
    <div class="grid gap-4 md:grid-cols-4">
      <Card>
        <CardHeader class="pb-2">
          <CardTitle class="text-sm font-medium">Network Members</CardTitle>
        </CardHeader>
        <CardContent>
          <div class="text-2xl font-bold">{{ networkMembers }}</div>
        </CardContent>
      </Card>
      <Card>
        <CardHeader class="pb-2">
          <CardTitle class="text-sm font-medium">Active Group Orders</CardTitle>
        </CardHeader>
        <CardContent>
          <div class="text-2xl font-bold">{{ activeGroupOrders }}</div>
        </CardContent>
      </Card>
      <Card>
        <CardHeader class="pb-2">
          <CardTitle class="text-sm font-medium">Savings This Month</CardTitle>
        </CardHeader>
        <CardContent>
          <div class="text-2xl font-bold text-primary">R {{ formatCurrency(totalSavings) }}</div>
        </CardContent>
      </Card>
      <Card>
        <CardHeader class="pb-2">
          <CardTitle class="text-sm font-medium">Shared Deliveries</CardTitle>
        </CardHeader>
        <CardContent>
          <div class="text-2xl font-bold">{{ sharedDeliveries }}</div>
        </CardContent>
      </Card>
    </div>
    
    <!-- Active Group Orders -->
    <Card>
      <CardHeader>
        <CardTitle>Active Group Orders</CardTitle>
      </CardHeader>
      <CardContent>
        <div class="space-y-4">
          <div v-for="order in groupOrders" :key="order.id" class="p-4 border rounded-lg">
            <div class="flex items-start justify-between mb-3">
              <div>
                <p class="font-medium">{{ order.product }}</p>
                <p class="text-sm text-muted-foreground">{{ order.organizer }} is organizing</p>
              </div>
              <div class="text-right">
                <p class="text-sm text-muted-foreground line-through">R {{ formatCurrency(order.regularPrice) }}</p>
                <p class="font-medium text-primary text-lg">R {{ formatCurrency(order.groupPrice) }}</p>
                <p class="text-xs text-muted-foreground">Save R {{ formatCurrency(order.regularPrice - order.groupPrice) }}</p>
              </div>
            </div>
            <div class="flex items-center justify-between text-sm mb-3">
              <span class="text-muted-foreground">{{ order.participants }}/{{ order.minParticipants }} shops joined</span>
              <span class="text-muted-foreground">Deadline: {{ order.deadline }}</span>
            </div>
            <div class="w-full bg-muted rounded-full h-2 mb-3">
              <div class="bg-primary h-2 rounded-full"
                   :style="{ width: `${(order.participants / order.minParticipants) * 100}%` }">
              </div>
            </div>
            <div class="flex gap-2">
              <Button v-if="!order.joined" class="flex-1">Join Order</Button>
              <Button v-else variant="outline" class="flex-1">View Details</Button>
              <Button variant="outline">Share</Button>
            </div>
          </div>
        </div>
      </CardContent>
    </Card>
    
    <!-- Nearby Businesses -->
    <Card>
      <CardHeader>
        <CardTitle>Nearby Network Members</CardTitle>
      </CardHeader>
      <CardContent>
        <div class="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
          <div v-for="business in nearbyBusinesses" :key="business.id" class="p-4 border rounded-lg">
            <div class="flex items-start justify-between mb-2">
              <div>
                <p class="font-medium">{{ business.name }}</p>
                <p class="text-sm text-muted-foreground">{{ business.type }}</p>
              </div>
              <Icon name="lucide:map-pin" class="w-4 h-4 text-muted-foreground" />
            </div>
            <p class="text-sm text-muted-foreground mb-3">{{ business.distance }} away</p>
            <Button variant="outline" size="sm" class="w-full">Connect</Button>
          </div>
        </div>
      </CardContent>
    </Card>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'

definePageMeta({
  layout: 'admin'
})

const networkMembers = ref(156)
const activeGroupOrders = ref(8)
const totalSavings = ref(3200)
const sharedDeliveries = ref(24)

const groupOrders = ref([
  { id: 1, product: 'Cooking Oil 2L', organizer: 'Mama Dlamini', regularPrice: 45, groupPrice: 38, participants: 8, minParticipants: 10, deadline: '2025-01-20', joined: false },
  { id: 2, product: 'Maize Meal 5kg', organizer: 'Spaza Shop ABC', regularPrice: 65, groupPrice: 55, participants: 12, minParticipants: 15, deadline: '2025-01-22', joined: true }
])

const nearbyBusinesses = ref([
  { id: 1, name: 'Mama Dlamini\'s Spaza', type: 'Spaza Shop', distance: '0.5 km' },
  { id: 2, name: 'Corner Store', type: 'General Dealer', distance: '1.2 km' },
  { id: 3, name: 'Local Bakery', type: 'Bakery', distance: '2.0 km' }
])

const formatCurrency = (amount: number) => {
  return amount.toLocaleString('en-ZA', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
}

useHead({
  title: 'Network - TOSS'
})
</script>

