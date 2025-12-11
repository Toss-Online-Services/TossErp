<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useStockStore } from '~/stores/stock'
import { useDashboardStore } from '~/stores/dashboard'

useHead({ title: 'AI Copilot - TOSS' })

const stockStore = useStockStore()
const dashboardStore = useDashboardStore()

const suggestions = ref([
  {
    id: 1,
    type: 'low_stock',
    title: 'Low Stock Alert',
    message: 'You are running low on sugar. Current stock: 15 units (minimum: 30)',
    priority: 'high',
    action: 'Create Purchase Order',
    icon: 'inventory_2',
    color: 'red'
  },
  {
    id: 2,
    type: 'cashflow',
    title: 'Cashflow Warning',
    message: 'Money out this week (R 4,500) is higher than money in (R 2,300)',
    priority: 'medium',
    action: 'Review Expenses',
    icon: 'account_balance_wallet',
    color: 'orange'
  },
  {
    id: 3,
    type: 'reorder',
    title: 'Reorder Suggestion',
    message: 'Consider reordering cooking oil. You have 8 bottles left.',
    priority: 'low',
    action: 'View Stock',
    icon: 'shopping_cart',
    color: 'blue'
  }
])

const insights = ref([
  {
    id: 1,
    title: 'Sales Trend',
    description: 'Your sales have increased by 15% compared to last week',
    type: 'positive',
    icon: 'trending_up'
  },
  {
    id: 2,
    title: 'Best Selling Item',
    description: 'Cement 50kg is your top seller this month',
    type: 'info',
    icon: 'star'
  },
  {
    id: 3,
    title: 'Customer Activity',
    description: '3 new customers added this week',
    type: 'positive',
    icon: 'people'
  }
])

const router = useRouter()

function handleSuggestionAction(suggestion: any) {
  if (suggestion.type === 'low_stock') {
    router.push('/stock/items')
  } else if (suggestion.type === 'cashflow') {
    router.push('/money')
  } else if (suggestion.type === 'reorder') {
    router.push('/stock/items')
  }
}

function dismissSuggestion(id: number) {
  suggestions.value = suggestions.value.filter(s => s.id !== id)
}

onMounted(() => {
  stockStore.fetchItems()
  dashboardStore.fetchDashboardData()
})
</script>

<template>
  <div class="py-6">
    <!-- Header -->
    <div class="mb-8">
      <h3 class="text-3xl font-bold text-gray-900 mb-2">AI Copilot</h3>
      <p class="text-gray-600 text-sm">Your intelligent business assistant - Ask me anything about your business</p>
    </div>

    <!-- Main Layout: Chatbot + Sidebar -->
    <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
      <!-- Chatbot (Main) -->
      <div class="lg:col-span-2">
        <div class="h-[calc(100vh-250px)] min-h-[600px]">
          <ClientOnly>
            <CopilotChatbotEmbedded />
            <template #fallback>
              <div class="flex items-center justify-center h-full bg-white rounded-xl shadow-sm">
                <div class="text-center">
                  <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto mb-4"></div>
                  <p class="text-gray-600">Loading chatbot...</p>
                </div>
              </div>
            </template>
          </ClientOnly>
        </div>
      </div>

      <!-- Sidebar: Suggestions & Quick Actions -->
      <div class="space-y-6">
        <!-- Suggestions -->
        <div>
          <h4 class="text-lg font-semibold text-gray-900 mb-4">Suggestions & Alerts</h4>
          <div class="space-y-4">
            <div
              v-for="suggestion in suggestions"
              :key="suggestion.id"
              class="bg-white rounded-xl shadow-sm p-4 border-l-4"
              :class="{
                'border-red-500': suggestion.color === 'red',
                'border-orange-500': suggestion.color === 'orange',
                'border-blue-500': suggestion.color === 'blue'
              }"
            >
              <div class="flex items-start justify-between mb-3">
                <div class="flex items-center gap-2">
                  <div
                    class="w-8 h-8 rounded-lg flex items-center justify-center"
                    :class="{
                      'bg-red-100': suggestion.color === 'red',
                      'bg-orange-100': suggestion.color === 'orange',
                      'bg-blue-100': suggestion.color === 'blue'
                    }"
                  >
                    <i
                      class="material-symbols-rounded text-sm"
                      :class="{
                        'text-red-600': suggestion.color === 'red',
                        'text-orange-600': suggestion.color === 'orange',
                        'text-blue-600': suggestion.color === 'blue'
                      }"
                    >
                      {{ suggestion.icon }}
                    </i>
                  </div>
                  <div>
                    <h5 class="text-sm font-semibold text-gray-900">{{ suggestion.title }}</h5>
                    <span
                      class="text-xs px-2 py-0.5 rounded-full"
                      :class="{
                        'bg-red-100 text-red-800': suggestion.priority === 'high',
                        'bg-orange-100 text-orange-800': suggestion.priority === 'medium',
                        'bg-blue-100 text-blue-800': suggestion.priority === 'low'
                      }"
                    >
                      {{ suggestion.priority }}
                    </span>
                  </div>
                </div>
                <button
                  @click="dismissSuggestion(suggestion.id)"
                  class="text-gray-400 hover:text-gray-600"
                >
                  <i class="material-symbols-rounded text-sm">close</i>
                </button>
              </div>
              <p class="text-xs text-gray-600 mb-3">{{ suggestion.message }}</p>
              <button
                @click="handleSuggestionAction(suggestion)"
                class="w-full px-3 py-1.5 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors text-xs"
              >
                {{ suggestion.action }}
              </button>
            </div>
          </div>
        </div>

        <!-- Insights -->
        <div>
          <h4 class="text-lg font-semibold text-gray-900 mb-4">Business Insights</h4>
          <div class="space-y-3">
            <div
              v-for="insight in insights"
              :key="insight.id"
              class="bg-white rounded-xl shadow-sm p-4"
            >
              <div class="flex items-center gap-2 mb-2">
                <div
                  class="w-8 h-8 rounded-lg flex items-center justify-center"
                  :class="{
                    'bg-green-100': insight.type === 'positive',
                    'bg-blue-100': insight.type === 'info'
                  }"
                >
                  <i
                    class="material-symbols-rounded text-sm"
                    :class="{
                      'text-green-600': insight.type === 'positive',
                      'text-blue-600': insight.type === 'info'
                    }"
                  >
                    {{ insight.icon }}
                  </i>
                </div>
                <h5 class="text-sm font-semibold text-gray-900">{{ insight.title }}</h5>
              </div>
              <p class="text-xs text-gray-600">{{ insight.description }}</p>
            </div>
          </div>
        </div>

        <!-- Quick Actions -->
        <div>
          <h4 class="text-lg font-semibold text-gray-900 mb-4">Quick Actions</h4>
          <div class="grid grid-cols-2 gap-3">
            <button
              @click="router.push('/stock/items')"
              class="bg-white rounded-xl shadow-sm p-4 hover:shadow-md transition-shadow text-center"
            >
              <i class="material-symbols-rounded text-3xl text-blue-600 mb-1">inventory_2</i>
              <p class="text-xs font-medium text-gray-900">Check Stock</p>
            </button>
            <button
              @click="router.push('/money')"
              class="bg-white rounded-xl shadow-sm p-4 hover:shadow-md transition-shadow text-center"
            >
              <i class="material-symbols-rounded text-3xl text-green-600 mb-1">account_balance_wallet</i>
              <p class="text-xs font-medium text-gray-900">View Money</p>
            </button>
            <button
              @click="router.push('/sales/pos')"
              class="bg-white rounded-xl shadow-sm p-4 hover:shadow-md transition-shadow text-center"
            >
              <i class="material-symbols-rounded text-3xl text-purple-600 mb-1">point_of_sale</i>
              <p class="text-xs font-medium text-gray-900">New Sale</p>
            </button>
            <button
              @click="router.push('/people/customers')"
              class="bg-white rounded-xl shadow-sm p-4 hover:shadow-md transition-shadow text-center"
            >
              <i class="material-symbols-rounded text-3xl text-orange-600 mb-1">people</i>
              <p class="text-xs font-medium text-gray-900">Customers</p>
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

