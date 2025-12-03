<script setup lang="ts">
import { onMounted, ref, computed } from 'vue'
import { useRoute } from 'vue-router'
import { useLogisticsStore, type Driver, type DeliveryRun } from '~/stores/logistics'
import { useSalesStore, type DeliveryNote } from '~/stores/sales'

definePageMeta({
  layout: 'default',
  ssr: false
})

useHead({
  title: 'Driver Details - TOSS'
})

const route = useRoute()
const logisticsStore = useLogisticsStore()
const salesStore = useSalesStore()

const driver = ref<Driver | null>(null)
const deliveryRuns = ref<DeliveryRun[]>([])
const salesDeliveries = ref<DeliveryNote[]>([])
const loading = ref(false)
const showReviewModal = ref(false)
const selectedDeliveryForReview = ref<DeliveryNote | null>(null)
const reviewRating = ref(5)
const reviewComment = ref('')

// Mock ratings data - in real app, this would come from the store
const driverRatings = ref([
  {
    id: '1',
    deliveryId: '1',
    deliveryNumber: 'DN-001',
    customerName: 'John Doe',
    rating: 5,
    comment: 'Excellent service, very professional and on time!',
    createdAt: new Date('2024-01-15')
  },
  {
    id: '2',
    deliveryId: '2',
    deliveryNumber: 'DN-002',
    customerName: 'Jane Smith',
    rating: 4,
    comment: 'Good delivery, driver was friendly.',
    createdAt: new Date('2024-01-10')
  },
  {
    id: '3',
    deliveryId: '3',
    deliveryNumber: 'DN-003',
    customerName: 'Bob Johnson',
    rating: 5,
    comment: 'Perfect! Delivered exactly on time.',
    createdAt: new Date('2024-01-05')
  }
])

const driverId = computed(() => {
  const id = route.params.id
  return Array.isArray(id) ? id[0] : String(id)
})

const averageRating = computed(() => {
  if (driverRatings.value.length === 0) return 0
  const sum = driverRatings.value.reduce((acc, r) => acc + r.rating, 0)
  return (sum / driverRatings.value.length).toFixed(1)
})

const totalRatings = computed(() => driverRatings.value.length)

const ratingDistribution = computed(() => {
  const dist = { 5: 0, 4: 0, 3: 0, 2: 0, 1: 0 }
  driverRatings.value.forEach(r => {
    dist[r.rating as keyof typeof dist]++
  })
  return dist
})

async function loadDriver() {
  loading.value = true
  try {
    await logisticsStore.fetchDrivers()
    driver.value = logisticsStore.getDriverById(driverId.value) || null
    
    if (driver.value) {
      await logisticsStore.fetchDeliveryRuns()
      deliveryRuns.value = logisticsStore.deliveryRuns.filter(
        run => run.driverId === driver.value?.id
      )
      
      // Load sales deliveries for this driver
      await salesStore.fetchDeliveries()
      salesDeliveries.value = salesStore.deliveries.filter(
        d => d.driverName === driver.value?.fullName
      )
    }
  } catch (error) {
    console.error('Failed to load driver:', error)
  } finally {
    loading.value = false
  }
}

function formatDate(date: Date | undefined) {
  if (!date) return '-'
  return new Date(date).toLocaleDateString('en-ZA', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  })
}

function formatDateTime(date: Date | undefined) {
  if (!date) return '-'
  return new Date(date).toLocaleDateString('en-ZA', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

function formatCurrency(amount: number) {
  return new Intl.NumberFormat('en-ZA', {
    style: 'currency',
    currency: 'ZAR'
  }).format(amount)
}

function getStatusColor(status: string) {
  const colors: Record<string, string> = {
    scheduled: 'text-gray-600 bg-gray-100',
    assigned: 'text-blue-600 bg-blue-100',
    in_transit: 'text-orange-600 bg-orange-100',
    completed: 'text-green-600 bg-green-100',
    cancelled: 'text-red-600 bg-red-100',
    pending: 'text-gray-600 bg-gray-100',
    delivered: 'text-green-600 bg-green-100',
    failed: 'text-red-600 bg-red-100'
  }
  return colors[status] || 'text-gray-600 bg-gray-100'
}

function getStatusLabel(status: string) {
  const labels: Record<string, string> = {
    scheduled: 'Scheduled',
    assigned: 'Assigned',
    in_transit: 'In Transit',
    completed: 'Completed',
    cancelled: 'Cancelled',
    pending: 'Pending',
    delivered: 'Delivered',
    failed: 'Failed'
  }
  return labels[status] || status
}

function openReviewModal(delivery: DeliveryNote) {
  selectedDeliveryForReview.value = delivery
  reviewRating.value = 5
  reviewComment.value = ''
  showReviewModal.value = true
}

function submitReview() {
  if (!selectedDeliveryForReview.value) return
  
  // In real app, this would call an API
  const newReview = {
    id: String(driverRatings.value.length + 1),
    deliveryId: selectedDeliveryForReview.value.id,
    deliveryNumber: selectedDeliveryForReview.value.deliveryNumber,
    customerName: selectedDeliveryForReview.value.customerName,
    rating: reviewRating.value,
    comment: reviewComment.value,
    createdAt: new Date()
  }
  
  driverRatings.value.unshift(newReview)
  showReviewModal.value = false
  selectedDeliveryForReview.value = null
  reviewRating.value = 5
  reviewComment.value = ''
  
  // Show success message
  alert('Review submitted successfully!')
}

function renderStars(rating: number) {
  return Array.from({ length: 5 }, (_, i) => i < rating)
}

onMounted(() => {
  loadDriver()
})
</script>

<template>
  <div class="py-6">
    <div v-if="loading" class="flex items-center justify-center py-12">
      <div class="text-center">
        <i class="material-symbols-rounded text-6xl text-gray-300 mb-4 animate-spin">refresh</i>
        <p class="text-gray-600">Loading driver details...</p>
      </div>
    </div>

    <div v-else-if="!driver" class="text-center py-12">
      <i class="material-symbols-rounded text-6xl text-gray-300 mb-4">person_off</i>
      <h3 class="text-lg font-semibold text-gray-900 mb-2">Driver not found</h3>
      <p class="text-gray-600 mb-6">The driver you're looking for doesn't exist</p>
      <NuxtLink
        to="/logistics/drivers"
        class="inline-flex items-center gap-2 px-4 py-2 bg-gray-900 text-white rounded-lg hover:bg-gray-800 transition-colors"
      >
        <i class="material-symbols-rounded text-lg">arrow_back</i>
        <span>Back to Drivers</span>
      </NuxtLink>
    </div>

    <div v-else>
      <!-- Header -->
      <div class="mb-8">
        <div class="flex items-center gap-4 mb-4">
          <NuxtLink
            to="/logistics/drivers"
            class="p-2 text-gray-600 hover:text-gray-900 hover:bg-gray-100 rounded-lg transition-colors"
          >
            <i class="material-symbols-rounded text-xl">arrow_back</i>
          </NuxtLink>
          <div>
            <h3 class="text-3xl font-bold text-gray-900">{{ driver.fullName }}</h3>
            <p class="text-gray-600 text-sm">Driver profile and delivery history</p>
          </div>
        </div>
      </div>

      <!-- Stats Cards -->
      <div class="grid grid-cols-1 md:grid-cols-4 gap-6 mb-6">
        <div class="bg-white rounded-xl shadow-card p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-gray-600">Status</p>
              <p class="mt-2">
                <span
                  :class="[
                    'px-2 py-1 text-xs font-medium rounded-full',
                    driver.isAvailable ? 'text-green-600 bg-green-100' : 'text-orange-600 bg-orange-100'
                  ]"
                >
                  {{ driver.isAvailable ? 'Available' : 'On Delivery' }}
                </span>
              </p>
            </div>
            <div class="p-3 bg-gray-100 rounded-lg">
              <i class="material-symbols-rounded text-2xl text-gray-600">person</i>
            </div>
          </div>
        </div>

        <div class="bg-white rounded-xl shadow-card p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-gray-600">Total Deliveries</p>
              <p class="mt-2 text-3xl font-bold text-gray-900">{{ deliveryRuns.length + salesDeliveries.length }}</p>
            </div>
            <div class="p-3 bg-blue-100 rounded-lg">
              <i class="material-symbols-rounded text-2xl text-blue-600">local_shipping</i>
            </div>
          </div>
        </div>

        <div class="bg-white rounded-xl shadow-card p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-gray-600">Average Rating</p>
              <div class="mt-2 flex items-center gap-2">
                <span class="text-3xl font-bold text-gray-900">{{ averageRating }}</span>
                <div class="flex items-center">
                  <i class="material-symbols-rounded text-yellow-500 text-lg">star</i>
                </div>
                <span class="text-sm text-gray-600">({{ totalRatings }})</span>
              </div>
            </div>
            <div class="p-3 bg-yellow-100 rounded-lg">
              <i class="material-symbols-rounded text-2xl text-yellow-600">star</i>
            </div>
          </div>
        </div>

        <div class="bg-white rounded-xl shadow-card p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-gray-600">Completed</p>
              <p class="mt-2 text-3xl font-bold text-green-600">
                {{ deliveryRuns.filter(r => r.status === 'completed').length + salesDeliveries.filter(d => d.status === 'delivered').length }}
              </p>
            </div>
            <div class="p-3 bg-green-100 rounded-lg">
              <i class="material-symbols-rounded text-2xl text-green-600">check_circle</i>
            </div>
          </div>
        </div>
      </div>

      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <!-- Driver Information -->
        <div class="lg:col-span-2 space-y-6">
          <!-- Contact Information -->
          <div class="bg-white rounded-xl shadow-card p-6">
            <h4 class="text-lg font-semibold text-gray-900 mb-4">Contact Information</h4>
            <div class="space-y-4">
              <div class="flex items-start gap-3">
                <i class="material-symbols-rounded text-gray-400 mt-1">phone</i>
                <div>
                  <p class="text-sm text-gray-600">Phone</p>
                  <p class="text-sm font-medium text-gray-900">{{ driver.phone }}</p>
                </div>
              </div>
              <div v-if="driver.email" class="flex items-start gap-3">
                <i class="material-symbols-rounded text-gray-400 mt-1">email</i>
                <div>
                  <p class="text-sm text-gray-600">Email</p>
                  <p class="text-sm font-medium text-gray-900">{{ driver.email }}</p>
                </div>
              </div>
            </div>
          </div>

          <!-- Vehicle Information -->
          <div v-if="driver.vehicleType || driver.vehicleRegistration" class="bg-white rounded-xl shadow-card p-6">
            <h4 class="text-lg font-semibold text-gray-900 mb-4">Vehicle Information</h4>
            <div class="space-y-4">
              <div v-if="driver.vehicleType" class="flex items-start gap-3">
                <i class="material-symbols-rounded text-gray-400 mt-1">directions_car</i>
                <div>
                  <p class="text-sm text-gray-600">Vehicle Type</p>
                  <p class="text-sm font-medium text-gray-900">{{ driver.vehicleType }}</p>
                </div>
              </div>
              <div v-if="driver.vehicleRegistration" class="flex items-start gap-3">
                <i class="material-symbols-rounded text-gray-400 mt-1">badge</i>
                <div>
                  <p class="text-sm text-gray-600">Registration</p>
                  <p class="text-sm font-medium text-gray-900">{{ driver.vehicleRegistration }}</p>
                </div>
              </div>
              <div v-if="driver.licenseNumber" class="flex items-start gap-3">
                <i class="material-symbols-rounded text-gray-400 mt-1">credit_card</i>
                <div>
                  <p class="text-sm text-gray-600">License Number</p>
                  <p class="text-sm font-medium text-gray-900">{{ driver.licenseNumber }}</p>
                </div>
              </div>
            </div>
          </div>

          <!-- Delivery Areas -->
          <div v-if="driver.areas && driver.areas.length > 0" class="bg-white rounded-xl shadow-card p-6">
            <h4 class="text-lg font-semibold text-gray-900 mb-4">Delivery Areas</h4>
            <div class="flex flex-wrap gap-2">
              <span
                v-for="area in driver.areas"
                :key="area"
                class="px-3 py-1 text-sm font-medium text-gray-700 bg-gray-100 rounded-lg"
              >
                {{ area }}
              </span>
            </div>
          </div>

          <!-- Ratings & Reviews -->
          <div class="bg-white rounded-xl shadow-card p-6">
            <div class="flex items-center justify-between mb-4">
              <h4 class="text-lg font-semibold text-gray-900">Ratings & Reviews</h4>
              <NuxtLink
                to="/sales/deliveries"
                class="text-sm text-gray-600 hover:text-gray-900 flex items-center gap-1"
              >
                <span>View All Deliveries</span>
                <i class="material-symbols-rounded text-sm">arrow_forward</i>
              </NuxtLink>
            </div>
            
            <div v-if="totalRatings > 0" class="mb-6">
              <div class="flex items-center gap-6 mb-4">
                <div class="text-center">
                  <div class="text-4xl font-bold text-gray-900">{{ averageRating }}</div>
                  <div class="flex items-center justify-center gap-1 mt-1">
                    <i
                      v-for="i in 5"
                      :key="i"
                      class="material-symbols-rounded text-yellow-500"
                      :class="i <= Math.round(Number(averageRating)) ? 'fill' : ''"
                    >
                      star
                    </i>
                  </div>
                  <p class="text-sm text-gray-600 mt-2">{{ totalRatings }} {{ totalRatings === 1 ? 'review' : 'reviews' }}</p>
                </div>
                <div class="flex-1 space-y-2">
                  <div
                    v-for="rating in [5, 4, 3, 2, 1]"
                    :key="rating"
                    class="flex items-center gap-2"
                  >
                    <span class="text-sm text-gray-600 w-8">{{ rating }}</span>
                    <i class="material-symbols-rounded text-yellow-500 text-sm">star</i>
                    <div class="flex-1 bg-gray-200 rounded-full h-2">
                      <div
                        class="bg-yellow-500 h-2 rounded-full"
                        :style="{ width: `${(ratingDistribution[rating] / totalRatings) * 100}%` }"
                      ></div>
                    </div>
                    <span class="text-sm text-gray-600 w-8 text-right">{{ ratingDistribution[rating] }}</span>
                  </div>
                </div>
              </div>
            </div>
            <div v-else class="text-center py-8">
              <i class="material-symbols-rounded text-6xl text-gray-300 mb-4">star</i>
              <p class="text-gray-600">No ratings yet</p>
            </div>

            <!-- Recent Reviews -->
            <div v-if="driverRatings.length > 0" class="mt-6 space-y-4">
              <h5 class="text-sm font-semibold text-gray-900 mb-3">Recent Reviews</h5>
              <div
                v-for="review in driverRatings.slice(0, 5)"
                :key="review.id"
                class="border border-gray-200 rounded-lg p-4"
              >
                <div class="flex items-start justify-between mb-2">
                  <div>
                    <p class="text-sm font-medium text-gray-900">{{ review.customerName }}</p>
                    <p class="text-xs text-gray-600">{{ review.deliveryNumber }}</p>
                  </div>
                  <div class="flex items-center gap-1">
                    <i
                      v-for="i in 5"
                      :key="i"
                      class="material-symbols-rounded text-sm"
                      :class="i <= review.rating ? 'text-yellow-500 fill' : 'text-gray-300'"
                    >
                      star
                    </i>
                  </div>
                </div>
                <p class="text-sm text-gray-700 mb-2">{{ review.comment }}</p>
                <p class="text-xs text-gray-500">{{ formatDate(review.createdAt) }}</p>
              </div>
            </div>
          </div>

          <!-- Notes -->
          <div v-if="driver.notes" class="bg-white rounded-xl shadow-card p-6">
            <h4 class="text-lg font-semibold text-gray-900 mb-4">Notes</h4>
            <p class="text-sm text-gray-700 whitespace-pre-wrap">{{ driver.notes }}</p>
          </div>
        </div>

        <!-- Sidebar -->
        <div class="space-y-6">
          <!-- Quick Actions -->
          <div class="bg-white rounded-xl shadow-card p-6">
            <h4 class="text-lg font-semibold text-gray-900 mb-4">Quick Actions</h4>
            <div class="space-y-2">
              <button
                @click="navigateTo(`/logistics/drivers?edit=${driver.id}`)"
                class="w-full flex items-center gap-2 px-4 py-2 text-sm text-gray-700 bg-gray-50 rounded-lg hover:bg-gray-100 transition-colors"
              >
                <i class="material-symbols-rounded text-lg">edit</i>
                <span>Edit Driver</span>
              </button>
              <button
                @click="logisticsStore.updateDriver(driver.id, { isAvailable: !driver.isAvailable })"
                class="w-full flex items-center gap-2 px-4 py-2 text-sm text-gray-700 bg-gray-50 rounded-lg hover:bg-gray-100 transition-colors"
              >
                <i class="material-symbols-rounded text-lg">{{ driver.isAvailable ? 'pause_circle' : 'play_circle' }}</i>
                <span>{{ driver.isAvailable ? 'Mark Unavailable' : 'Mark Available' }}</span>
              </button>
              <NuxtLink
                to="/sales/deliveries"
                class="w-full flex items-center gap-2 px-4 py-2 text-sm text-gray-700 bg-gray-50 rounded-lg hover:bg-gray-100 transition-colors"
              >
                <i class="material-symbols-rounded text-lg">local_shipping</i>
                <span>View Deliveries</span>
              </NuxtLink>
            </div>
          </div>

          <!-- Driver Stats -->
          <div class="bg-white rounded-xl shadow-card p-6">
            <h4 class="text-lg font-semibold text-gray-900 mb-4">Statistics</h4>
            <div class="space-y-3">
              <div class="flex justify-between items-center">
                <span class="text-sm text-gray-600">Active</span>
                <span class="text-sm font-semibold text-gray-900">
                  {{ driver.isActive ? 'Yes' : 'No' }}
                </span>
              </div>
              <div class="flex justify-between items-center">
                <span class="text-sm text-gray-600">Available</span>
                <span class="text-sm font-semibold text-gray-900">
                  {{ driver.isAvailable ? 'Yes' : 'No' }}
                </span>
              </div>
              <div class="flex justify-between items-center">
                <span class="text-sm text-gray-600">Total Runs</span>
                <span class="text-sm font-semibold text-gray-900">{{ deliveryRuns.length }}</span>
              </div>
              <div class="flex justify-between items-center">
                <span class="text-sm text-gray-600">Total Ratings</span>
                <span class="text-sm font-semibold text-gray-900">{{ totalRatings }}</span>
              </div>
              <div class="flex justify-between items-center">
                <span class="text-sm text-gray-600">Created</span>
                <span class="text-sm font-semibold text-gray-900">{{ formatDate(driver.createdAt) }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Sales Deliveries -->
      <div class="mt-6 bg-white rounded-xl shadow-card overflow-hidden">
        <div class="p-6 border-b border-gray-200 flex items-center justify-between">
          <h4 class="text-lg font-semibold text-gray-900">Sales Deliveries</h4>
          <NuxtLink
            to="/sales/deliveries"
            class="text-sm text-gray-600 hover:text-gray-900 flex items-center gap-1"
          >
            <span>View All</span>
            <i class="material-symbols-rounded text-sm">arrow_forward</i>
          </NuxtLink>
        </div>
        <div v-if="salesDeliveries.length === 0" class="p-12 text-center">
          <i class="material-symbols-rounded text-6xl text-gray-300 mb-4">local_shipping</i>
          <p class="text-gray-600">No sales deliveries assigned yet</p>
        </div>
        <div v-else class="overflow-x-auto">
          <table class="w-full">
            <thead class="bg-gray-50 border-b border-gray-200">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Delivery #</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Customer</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Date</th>
                <th class="px-6 py-3 text-center text-xs font-medium text-gray-500 uppercase tracking-wider">Status</th>
                <th class="px-6 py-3 text-center text-xs font-medium text-gray-500 uppercase tracking-wider">Rating</th>
                <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
              </tr>
            </thead>
            <tbody class="bg-white divide-y divide-gray-200">
              <tr
                v-for="delivery in salesDeliveries"
                :key="delivery.id"
                class="hover:bg-gray-50 transition-colors"
              >
                <td class="px-6 py-4 whitespace-nowrap">
                  <span class="text-sm font-semibold text-gray-900">{{ delivery.deliveryNumber }}</span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                  {{ delivery.customerName }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                  {{ formatDate(delivery.deliveryDate) }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-center">
                  <span :class="['px-2 py-1 text-xs font-medium rounded-full', getStatusColor(delivery.status)]">
                    {{ getStatusLabel(delivery.status) }}
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-center">
                  <div v-if="driverRatings.find(r => r.deliveryId === delivery.id)" class="flex items-center justify-center gap-1">
                    <i
                      v-for="i in 5"
                      :key="i"
                      class="material-symbols-rounded text-sm"
                      :class="i <= (driverRatings.find(r => r.deliveryId === delivery.id)?.rating || 0) ? 'text-yellow-500 fill' : 'text-gray-300'"
                    >
                      star
                    </i>
                  </div>
                  <span v-else class="text-xs text-gray-400">Not rated</span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                  <div class="flex items-center justify-end gap-2">
                    <button
                      @click="navigateTo(`/sales/deliveries/${delivery.id}`)"
                      class="p-2 text-gray-600 hover:text-gray-900 hover:bg-gray-100 rounded-lg transition-colors"
                      title="View"
                    >
                      <i class="material-symbols-rounded text-lg">visibility</i>
                    </button>
                    <button
                      v-if="delivery.status === 'delivered' && !driverRatings.find(r => r.deliveryId === delivery.id)"
                      @click="openReviewModal(delivery)"
                      class="p-2 text-gray-600 hover:text-gray-900 hover:bg-gray-100 rounded-lg transition-colors"
                      title="Review"
                    >
                      <i class="material-symbols-rounded text-lg">rate_review</i>
                    </button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <!-- Delivery History (Logistics Runs) -->
      <div class="mt-6 bg-white rounded-xl shadow-card overflow-hidden">
        <div class="p-6 border-b border-gray-200">
          <h4 class="text-lg font-semibold text-gray-900">Delivery Runs</h4>
        </div>
        <div v-if="deliveryRuns.length === 0" class="p-12 text-center">
          <i class="material-symbols-rounded text-6xl text-gray-300 mb-4">local_shipping</i>
          <p class="text-gray-600">No delivery runs assigned yet</p>
        </div>
        <div v-else class="overflow-x-auto">
          <table class="w-full">
            <thead class="bg-gray-50 border-b border-gray-200">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Run #</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Date</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Stops</th>
                <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Cost</th>
                <th class="px-6 py-3 text-center text-xs font-medium text-gray-500 uppercase tracking-wider">Status</th>
                <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
              </tr>
            </thead>
            <tbody class="bg-white divide-y divide-gray-200">
              <tr
                v-for="run in deliveryRuns"
                :key="run.id"
                class="hover:bg-gray-50 transition-colors"
              >
                <td class="px-6 py-4 whitespace-nowrap">
                  <span class="text-sm font-semibold text-gray-900">{{ run.runNumber }}</span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                  {{ formatDate(run.scheduledDate) }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                  {{ run.stops.length }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-semibold text-gray-900">
                  {{ formatCurrency(run.totalDeliveryCost) }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-center">
                  <span :class="['px-2 py-1 text-xs font-medium rounded-full', getStatusColor(run.status)]">
                    {{ getStatusLabel(run.status) }}
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                  <button
                    @click="navigateTo(`/logistics/deliveries/${run.id}`)"
                    class="p-2 text-gray-600 hover:text-gray-900 hover:bg-gray-100 rounded-lg transition-colors"
                    title="View"
                  >
                    <i class="material-symbols-rounded text-lg">visibility</i>
                  </button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>

    <!-- Review Modal -->
    <div
      v-if="showReviewModal"
      class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4"
      @click.self="showReviewModal = false"
    >
      <div class="bg-white rounded-xl shadow-xl max-w-md w-full p-6">
        <div class="flex items-center justify-between mb-4">
          <h3 class="text-lg font-semibold text-gray-900">Review Delivery</h3>
          <button
            @click="showReviewModal = false"
            class="p-2 text-gray-400 hover:text-gray-600 rounded-lg transition-colors"
          >
            <i class="material-symbols-rounded text-xl">close</i>
          </button>
        </div>
        
        <div v-if="selectedDeliveryForReview" class="space-y-4">
          <div>
            <p class="text-sm text-gray-600">Delivery</p>
            <p class="text-sm font-medium text-gray-900">{{ selectedDeliveryForReview.deliveryNumber }}</p>
            <p class="text-xs text-gray-500">{{ selectedDeliveryForReview.customerName }}</p>
          </div>
          
          <div>
            <p class="text-sm text-gray-600 mb-2">Rating</p>
            <div class="flex items-center gap-2">
              <button
                v-for="i in 5"
                :key="i"
                @click="reviewRating = i"
                class="p-1 transition-colors"
              >
                <i
                  class="material-symbols-rounded text-3xl"
                  :class="i <= reviewRating ? 'text-yellow-500 fill' : 'text-gray-300'"
                >
                  star
                </i>
              </button>
            </div>
          </div>
          
          <div>
            <label class="block text-sm text-gray-600 mb-2">Comment</label>
            <textarea
              v-model="reviewComment"
              rows="4"
              class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-gray-900 focus:border-transparent"
              placeholder="Share your experience..."
            ></textarea>
          </div>
          
          <div class="flex items-center gap-3 pt-4">
            <button
              @click="showReviewModal = false"
              class="flex-1 px-4 py-2 text-sm text-gray-700 bg-gray-100 rounded-lg hover:bg-gray-200 transition-colors"
            >
              Cancel
            </button>
            <button
              @click="submitReview"
              class="flex-1 px-4 py-2 text-sm text-white bg-gray-900 rounded-lg hover:bg-gray-800 transition-colors"
            >
              Submit Review
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
