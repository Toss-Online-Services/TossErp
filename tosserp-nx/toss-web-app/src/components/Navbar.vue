<script setup lang="ts">
import { computed, ref } from 'vue'
import { useRouter } from 'vue-router'
import {
  Search,
  Menu,
  User,
  CreditCard,
  LogOut,
  ChevronDown,
  Sparkles,
  Package,
  ShoppingBasket,
  Wifi,
  WifiOff
} from 'lucide-vue-next'
import { useNetworkStatus } from '@/composables/useNetworkStatus'

defineProps<{
  onToggleSidebar: () => void
}>()

const router = useRouter()
const searchQuery = ref('')
const accountDropdownOpen = ref(false)

const quickActions = [
  {
    label: 'New Sale',
    icon: CreditCard,
    to: '/sales/pos',
    variant: 'primary'
  },
  {
    label: 'Receive Stock',
    icon: Package,
    to: '/stock/receipts',
    variant: 'outline'
  },
  {
    label: 'Ask Copilot',
    icon: Sparkles,
    to: '/copilot',
    variant: 'ghost'
  }
]

const { isOnline, lastChangedAt } = useNetworkStatus()
const networkStatusLabel = computed(() =>
  isOnline.value ? 'Online' : 'Offline – queued actions will sync when back'
)

const lastChangedText = computed(() => {
  if (!lastChangedAt.value) {
    return ''
  }
  return `Updated ${lastChangedAt.value.toLocaleTimeString()}`
})

const handleSearch = (e: Event) => {
  e.preventDefault()
  if (!searchQuery.value.trim()) {
    return
  }
  router.push({
    path: '/search',
    query: { q: searchQuery.value }
  })
}

const triggerAction = (action: (typeof quickActions)[number]) => {
  router.push(action.to)
}

const toggleAccountDropdown = () => {
  accountDropdownOpen.value = !accountDropdownOpen.value
}

const closeAccountDropdown = () => {
  accountDropdownOpen.value = false
}

const navigateToBilling = () => {
  router.push('/billing')
  closeAccountDropdown()
}

const handleLogout = () => {
  // TODO: wire up auth service
  closeAccountDropdown()
}
</script>

<template>
  <nav class="bg-card border-b sticky top-0 z-40">
    <div class="px-4 lg:px-8 space-y-3">
      <div class="flex items-center justify-between h-16 flex-wrap gap-3">
        <div class="flex items-center gap-3">
          <button
            @click="onToggleSidebar"
            class="lg:hidden p-2 hover:bg-accent rounded-md"
            aria-label="Toggle menu"
          >
            <Menu :size="20" />
          </button>

          <form @submit="handleSearch" class="hidden md:block">
            <div class="relative">
              <Search
                :size="18"
                class="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground"
              />
              <input
                v-model="searchQuery"
                type="search"
                placeholder="Search products, customers or invoices"
                class="pl-10 pr-4 py-2 bg-background border rounded-md focus:outline-none focus:ring-2 focus:ring-primary w-64 lg:w-96"
              />
            </div>
          </form>
        </div>

        <div class="flex items-center gap-3">
          <div
            class="flex items-center gap-2 px-3 py-1 border rounded-full text-xs font-medium"
            :class="isOnline ? 'text-emerald-600 border-emerald-200' : 'text-amber-600 border-amber-200'"
          >
            <component :is="isOnline ? Wifi : WifiOff" :size="14" />
            <span>{{ networkStatusLabel }}</span>
          </div>

          <button
            @click="onToggleSidebar"
            class="hidden lg:block p-2 hover:bg-accent rounded-md"
            aria-label="Toggle sidebar"
          >
            <Menu :size="20" />
          </button>

          <div class="relative">
            <button
              @click="toggleAccountDropdown"
              class="flex items-center gap-2 px-3 py-2 hover:bg-accent rounded-md"
              aria-label="Account menu"
            >
              <div class="w-8 h-8 rounded-full bg-primary text-primary-foreground flex items-center justify-center">
                <User :size="18" />
              </div>
              <span class="hidden sm:block text-sm font-medium">Account</span>
              <ChevronDown :size="16" class="hidden sm:block" />
            </button>

            <div
              v-if="accountDropdownOpen"
              @click="closeAccountDropdown"
              class="fixed inset-0 z-40"
            ></div>

            <transition
              enter-active-class="transition ease-out duration-100"
              enter-from-class="transform opacity-0 scale-95"
              enter-to-class="transform opacity-100 scale-100"
              leave-active-class="transition ease-in duration-75"
              leave-from-class="transform opacity-100 scale-100"
              leave-to-class="transform opacity-0 scale-95"
            >
              <div
                v-if="accountDropdownOpen"
                class="absolute right-0 mt-2 w-56 bg-card border rounded-md shadow-lg py-1 z-50"
              >
                <div class="px-4 py-3 border-b">
                  <p class="text-sm font-medium">John Doe</p>
                  <p class="text-xs text-muted-foreground">john@example.com</p>
                </div>

                <button
                  @click="navigateToBilling"
                  class="w-full flex items-center gap-2 px-4 py-2 text-sm hover:bg-accent"
                >
                  <CreditCard :size="16" />
                  <span>Billing</span>
                </button>

                <button
                  @click="handleLogout"
                  class="w-full flex items-center gap-2 px-4 py-2 text-sm hover:bg-accent text-red-600"
                >
                  <LogOut :size="16" />
                  <span>Logout</span>
                </button>
              </div>
            </transition>
          </div>
        </div>
      </div>

      <div class="flex flex-wrap items-center gap-3 text-xs text-muted-foreground">
        <span v-if="lastChangedText">{{ lastChangedText }}</span>
        <span class="hidden md:inline">TOSS ERP III • Helping township businesses stay on track</span>
      </div>

      <div class="flex flex-wrap gap-2">
        <button
          v-for="action in quickActions"
          :key="action.label"
          type="button"
          class="inline-flex items-center gap-2 px-4 py-2 rounded-md text-sm font-medium transition-colors"
          :class="{
            'bg-primary text-primary-foreground hover:bg-primary/90': action.variant === 'primary',
            'border border-input hover:bg-accent': action.variant === 'outline',
            'hover:bg-accent text-muted-foreground': action.variant === 'ghost'
          }"
          @click="triggerAction(action)"
        >
          <component :is="action.icon" :size="16" />
          <span>{{ action.label }}</span>
        </button>
      </div>

      <div class="md:hidden pb-3">
        <form @submit="handleSearch">
          <div class="relative">
            <Search
              :size="18"
              class="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground"
            />
            <input
              v-model="searchQuery"
              type="search"
              placeholder="Search products, customers or invoices"
              class="w-full pl-10 pr-4 py-2 bg-background border rounded-md focus:outline-none focus:ring-2 focus:ring-primary"
            />
          </div>
        </form>
      </div>
    </div>
  </nav>
</template>
