<template>
  <div class="space-y-6">
    <!-- Welcome Header -->
    <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
      <div class="sm:flex sm:items-center">
        <div class="sm:flex-auto">
          <h1 class="text-2xl font-semibold text-gray-900 dark:text-white">
            {{ t('dashboard.welcome') }}
          </h1>
          <p class="mt-2 text-sm text-gray-700 dark:text-gray-300">
            Your comprehensive business management dashboard for TOSS ERP III
          </p>
        </div>
        <div class="mt-4 sm:mt-0 sm:ml-16 sm:flex-none">
          <UButton
            icon="i-heroicons-sparkles"
            size="lg"
            @click="openAICopilot"
          >
            Ask AI Co-Pilot
          </UButton>
        </div>
      </div>
    </div>

    <!-- Quick Statistics -->
    <div class="grid grid-cols-1 gap-5 sm:grid-cols-2 lg:grid-cols-4">
      <!-- Total Revenue -->
      <UCard>
        <template #header>
          <div class="flex items-center">
            <div class="flex-shrink-0">
              <UIcon name="i-heroicons-banknotes" class="w-8 h-8 text-green-500" />
            </div>
            <div class="ml-5 w-0 flex-1">
              <dl>
                <dt class="text-sm font-medium text-gray-500 dark:text-gray-400 truncate">
                  Total Revenue
                </dt>
                <dd class="text-lg font-medium text-gray-900 dark:text-white">
                  R {{ formatCurrency(totalRevenue) }}
                </dd>
              </dl>
            </div>
          </div>
        </template>
      </UCard>

      <!-- Active Customers -->
      <UCard>
        <template #header>
          <div class="flex items-center">
            <div class="flex-shrink-0">
              <UIcon name="i-heroicons-users" class="w-8 h-8 text-blue-500" />
            </div>
            <div class="ml-5 w-0 flex-1">
              <dl>
                <dt class="text-sm font-medium text-gray-500 dark:text-gray-400 truncate">
                  Active Customers
                </dt>
                <dd class="text-lg font-medium text-gray-900 dark:text-white">
                  {{ activeCustomers }}
                </dd>
              </dl>
            </div>
          </div>
        </template>
      </UCard>

      <!-- Low Stock Items -->
      <UCard>
        <template #header>
          <div class="flex items-center">
            <div class="flex-shrink-0">
              <UIcon name="i-heroicons-exclamation-triangle" class="w-8 h-8 text-yellow-500" />
            </div>
            <div class="ml-5 w-0 flex-1">
              <dl>
                <dt class="text-sm font-medium text-gray-500 dark:text-gray-400 truncate">
                  Low Stock Alerts
                </dt>
                <dd class="text-lg font-medium text-gray-900 dark:text-white">
                  {{ lowStockCount }}
                </dd>
              </dl>
            </div>
          </div>
        </template>
      </UCard>

      <!-- Pending Orders -->
      <UCard>
        <template #header>
          <div class="flex items-center">
            <div class="flex-shrink-0">
              <UIcon name="i-heroicons-clock" class="w-8 h-8 text-purple-500" />
            </div>
            <div class="ml-5 w-0 flex-1">
              <dl>
                <dt class="text-sm font-medium text-gray-500 dark:text-gray-400 truncate">
                  Pending Orders
                </dt>
                <dd class="text-lg font-medium text-gray-900 dark:text-white">
                  {{ pendingOrders }}
                </dd>
              </dl>
            </div>
          </div>
        </template>
      </UCard>
    </div>

    <!-- Main Content Grid -->
    <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
      <!-- Recent Activities -->
      <div class="lg:col-span-2">
        <UCard>
          <template #header>
            <h3 class="text-lg font-medium text-gray-900 dark:text-white">
              {{ t('dashboard.recent_activities') }}
            </h3>
          </template>

          <div class="space-y-4">
            <div class="flex items-start space-x-3" v-for="activity in recentActivities" :key="activity.id">
              <div class="flex-shrink-0">
                <UAvatar :src="activity.user.avatar" :alt="activity.user.name" size="sm" />
              </div>
              <div class="min-w-0 flex-1">
                <p class="text-sm text-gray-900 dark:text-white">
                  <span class="font-medium">{{ activity.user.name }}</span>
                  {{ activity.description }}
                </p>
                <p class="text-xs text-gray-500 dark:text-gray-400">
                  {{ formatTime(activity.timestamp) }}
                </p>
              </div>
            </div>
          </div>

          <template #footer>
            <UButton variant="ghost" class="w-full">
              View All Activities
            </UButton>
          </template>
        </UCard>
      </div>

      <!-- AI Insights & Quick Actions -->
      <div class="space-y-6">
        <!-- AI Insights -->
        <UCard>
          <template #header>
            <div class="flex items-center">
              <UIcon name="i-heroicons-sparkles" class="w-5 h-5 text-purple-500 mr-2" />
              <h3 class="text-lg font-medium text-gray-900 dark:text-white">
                {{ t('dashboard.ai_insights') }}
              </h3>
            </div>
          </template>

          <div class="space-y-3">
            <UAlert
              icon="i-heroicons-light-bulb"
              color="blue"
              variant="soft"
              title="Stock Optimization"
              description="Consider ordering more maize meal - sales increased 30% this month."
            />
            <UAlert
              icon="i-heroicons-chart-bar"
              color="green"
              variant="soft"
              title="Sales Trend"
              description="Weekend sales are 40% higher. Consider weekend promotions."
            />
            <UAlert
              icon="i-heroicons-users"
              color="yellow"
              variant="soft"
              title="Customer Retention"
              description="5 customers haven't visited in 2 weeks. Send follow-up messages."
            />
          </div>
        </UCard>

        <!-- Quick Actions -->
        <UCard>
          <template #header>
            <h3 class="text-lg font-medium text-gray-900 dark:text-white">
              Quick Actions
            </h3>
          </template>

          <div class="space-y-3">
            <UButton
              icon="i-heroicons-plus-circle"
              variant="outline"
              class="w-full justify-start"
              @click="router.push('/sales/new')"
            >
              New Sale
            </UButton>
            <UButton
              icon="i-heroicons-cube"
              variant="outline"
              class="w-full justify-start"
              @click="router.push('/inventory/add')"
            >
              Add Product
            </UButton>
            <UButton
              icon="i-heroicons-user-plus"
              variant="outline"
              class="w-full justify-start"
              @click="router.push('/sales/customers/new')"
            >
              Add Customer
            </UButton>
            <UButton
              icon="i-heroicons-document-chart-bar"
              variant="outline"
              class="w-full justify-start"
              @click="router.push('/reports')"
            >
              View Reports
            </UButton>
          </div>
        </UCard>
      </div>
    </div>

    <!-- Charts Section -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <!-- Sales Chart -->
      <UCard>
        <template #header>
          <h3 class="text-lg font-medium text-gray-900 dark:text-white">
            Sales Overview
          </h3>
        </template>
        
        <div class="h-64 flex items-center justify-center text-gray-500 dark:text-gray-400">
          <div class="text-center">
            <UIcon name="i-heroicons-chart-bar" class="w-12 h-12 mx-auto mb-2" />
            <p>Sales chart will be implemented here</p>
            <p class="text-sm">Connect to analytics service</p>
          </div>
        </div>
      </UCard>

      <!-- Inventory Chart -->
      <UCard>
        <template #header>
          <h3 class="text-lg font-medium text-gray-900 dark:text-white">
            Inventory Status
          </h3>
        </template>
        
        <div class="h-64 flex items-center justify-center text-gray-500 dark:text-gray-400">
          <div class="text-center">
            <UIcon name="i-heroicons-cube" class="w-12 h-12 mx-auto mb-2" />
            <p>Inventory chart will be implemented here</p>
            <p class="text-sm">Stock levels visualization</p>
          </div>
        </div>
      </UCard>
    </div>
  </div>
</template>

<script setup lang="ts">
// Composables
const { t } = useI18n()
const router = useRouter()

// Page meta
definePageMeta({
  title: 'Dashboard - TOSS ERP III',
  description: 'Business management dashboard'
})

// Mock data - replace with real API calls
const totalRevenue = ref(125750.00)
const activeCustomers = ref(342)
const lowStockCount = ref(8)
const pendingOrders = ref(23)

const recentActivities = ref([
  {
    id: 1,
    user: { name: 'John Doe', avatar: '/avatars/john.jpg' },
    description: 'created a new sale for R450.00',
    timestamp: new Date(Date.now() - 1000 * 60 * 5) // 5 minutes ago
  },
  {
    id: 2,
    user: { name: 'Jane Smith', avatar: '/avatars/jane.jpg' },
    description: 'added 50 units of maize meal to inventory',
    timestamp: new Date(Date.now() - 1000 * 60 * 15) // 15 minutes ago
  },
  {
    id: 3,
    user: { name: 'Mike Johnson', avatar: '/avatars/mike.jpg' },
    description: 'generated monthly financial report',
    timestamp: new Date(Date.now() - 1000 * 60 * 30) // 30 minutes ago
  },
  {
    id: 4,
    user: { name: 'Sarah Williams', avatar: '/avatars/sarah.jpg' },
    description: 'processed customer refund of R120.00',
    timestamp: new Date(Date.now() - 1000 * 60 * 45) // 45 minutes ago
  }
])

// Utility functions
const formatCurrency = (amount: number): string => {
  return new Intl.NumberFormat('en-ZA', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  }).format(amount)
}

const formatTime = (date: Date): string => {
  return new Intl.RelativeTimeFormat('en-ZA', { numeric: 'auto' }).format(
    Math.floor((date.getTime() - Date.now()) / (1000 * 60)),
    'minute'
  )
}

const openAICopilot = () => {
  // This would integrate with the AI copilot functionality
  console.log('Opening AI Co-Pilot...')
}

// Load dashboard data
onMounted(async () => {
  try {
    // TODO: Replace with actual API calls
    console.log('Loading dashboard data...')
  } catch (error) {
    console.error('Failed to load dashboard data:', error)
  }
})
</script>
