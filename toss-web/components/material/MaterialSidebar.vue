<script setup lang="ts">
import { ref, computed } from 'vue'
import { cn } from '~/lib/utils'
import { 
  HomeIcon, 
  ShoppingBagIcon, 
  ChartBarIcon,
  CogIcon,
  UsersIcon,
  TruckIcon,
  Squares2X2Icon,
  XMarkIcon
} from '@heroicons/vue/24/outline'

interface NavItem {
  name: string
  icon: any
  href: string
  badge?: number
  active?: boolean
}

interface Props {
  items?: NavItem[]
  open?: boolean
  role?: 'admin' | 'retailer' | 'supplier' | 'driver'
  userInfo?: {
    name: string
    email: string
    avatar?: string
  }
}

const props = withDefaults(defineProps<Props>(), {
  items: () => [],
  open: true,
  role: 'retailer'
})

const emit = defineEmits<{
  'update:open': [value: boolean]
  'navigate': [item: NavItem]
}>()

const defaultItems = computed(() => {
  const roleItems: Record<string, NavItem[]> = {
    retailer: [
      { name: 'Dashboard', icon: HomeIcon, href: '/dashboard', active: true },
      { name: 'Sales', icon: ShoppingBagIcon, href: '/sales' },
      { name: 'Products', icon: Squares2X2Icon, href: '/products' },
      { name: 'Inventory', icon: ChartBarIcon, href: '/inventory' },
      { name: 'Orders', icon: TruckIcon, href: '/orders' },
      { name: 'Settings', icon: CogIcon, href: '/settings' }
    ],
    supplier: [
      { name: 'Dashboard', icon: HomeIcon, href: '/supplier/dashboard', active: true },
      { name: 'Orders', icon: ShoppingBagIcon, href: '/supplier/orders', badge: 5 },
      { name: 'Catalog', icon: Squares2X2Icon, href: '/supplier/catalog' },
      { name: 'Analytics', icon: ChartBarIcon, href: '/supplier/analytics' },
      { name: 'Deliveries', icon: TruckIcon, href: '/supplier/deliveries' },
      { name: 'Settings', icon: CogIcon, href: '/supplier/settings' }
    ],
    driver: [
      { name: 'Dashboard', icon: HomeIcon, href: '/driver/dashboard', active: true },
      { name: 'Active Runs', icon: TruckIcon, href: '/driver/runs', badge: 3 },
      { name: 'Earnings', icon: ChartBarIcon, href: '/driver/earnings' },
      { name: 'Schedule', icon: Squares2X2Icon, href: '/driver/schedule' },
      { name: 'Settings', icon: CogIcon, href: '/driver/settings' }
    ],
    admin: [
      { name: 'Dashboard', icon: HomeIcon, href: '/admin/dashboard', active: true },
      { name: 'Users', icon: UsersIcon, href: '/admin/users' },
      { name: 'Analytics', icon: ChartBarIcon, href: '/admin/analytics' },
      { name: 'Shops', icon: ShoppingBagIcon, href: '/admin/shops' },
      { name: 'Deliveries', icon: TruckIcon, href: '/admin/deliveries' },
      { name: 'Settings', icon: CogIcon, href: '/admin/settings' }
    ]
  }
  
  return roleItems[props.role] || roleItems.retailer
})

const navItems = computed(() => props.items.length > 0 ? props.items : defaultItems.value)

const roleColors = {
  admin: 'from-purple-500 to-pink-500',
  retailer: 'from-orange-500 to-red-500',
  supplier: 'from-blue-500 to-cyan-500',
  driver: 'from-green-500 to-emerald-500'
}

const handleNavigate = (item: NavItem) => {
  emit('navigate', item)
  // Close on mobile after navigation
  if (window.innerWidth < 1024) {
    emit('update:open', false)
  }
}
</script>

<template>
  <div>
    <!-- Mobile Overlay -->
    <Transition
      enter-active-class="transition-opacity duration-300"
      enter-from-class="opacity-0"
      enter-to-class="opacity-100"
      leave-active-class="transition-opacity duration-300"
      leave-from-class="opacity-100"
      leave-to-class="opacity-0"
    >
      <div
        v-if="open"
        @click="$emit('update:open', false)"
        class="fixed inset-0 bg-slate-900/80 backdrop-blur-sm z-40 lg:hidden"
      />
    </Transition>

    <!-- Sidebar -->
    <Transition
      enter-active-class="transition-transform duration-300 ease-out"
      enter-from-class="-translate-x-full"
      enter-to-class="translate-x-0"
      leave-active-class="transition-transform duration-300 ease-in"
      leave-from-class="translate-x-0"
      leave-to-class="-translate-x-full"
    >
      <aside
        v-if="open || $route.meta.layout !== 'mobile'"
        :class="cn(
          'fixed top-0 left-0 z-50 h-screen w-72 bg-white dark:bg-slate-900 border-r border-slate-200 dark:border-slate-800 shadow-xl',
          'lg:translate-x-0 lg:static lg:z-auto'
        )"
      >
        <!-- Header -->
        <div class="h-16 flex items-center justify-between px-6 border-b border-slate-200 dark:border-slate-800">
          <NuxtLink to="/" class="flex items-center space-x-3 group">
            <div :class="cn(
              'relative flex items-center justify-center w-10 h-10 rounded-xl bg-gradient-to-br shadow-lg transition-transform group-hover:scale-110',
              roleColors[role]
            )">
              <span class="text-xl font-black text-white">T</span>
            </div>
            <div class="flex flex-col">
              <span class="text-lg font-black text-slate-900 dark:text-white">TOSS</span>
              <span class="text-[8px] font-semibold text-orange-600 dark:text-orange-400 -mt-0.5 tracking-wider uppercase">
                {{ role }}
              </span>
            </div>
          </NuxtLink>
          
          <!-- Close button (mobile only) -->
          <button
            @click="$emit('update:open', false)"
            class="lg:hidden p-2 rounded-lg hover:bg-slate-100 dark:hover:bg-slate-800 transition-colors"
          >
            <XMarkIcon class="w-5 h-5 text-slate-600 dark:text-slate-400" />
          </button>
        </div>

        <!-- User Info -->
        <div v-if="userInfo" class="p-4 border-b border-slate-200 dark:border-slate-800">
          <div class="flex items-center space-x-3">
            <div class="relative">
              <div v-if="userInfo.avatar" class="w-10 h-10 rounded-full overflow-hidden">
                <img :src="userInfo.avatar" :alt="userInfo.name" class="w-full h-full object-cover" />
              </div>
              <div v-else class="w-10 h-10 rounded-full bg-gradient-to-br from-orange-400 to-orange-600 flex items-center justify-center">
                <span class="text-white font-bold text-sm">{{ userInfo.name.charAt(0) }}</span>
              </div>
              <span class="absolute bottom-0 right-0 w-3 h-3 bg-green-500 border-2 border-white dark:border-slate-900 rounded-full"></span>
            </div>
            <div class="flex-1 min-w-0">
              <p class="text-sm font-semibold text-slate-900 dark:text-white truncate">{{ userInfo.name }}</p>
              <p class="text-xs text-slate-500 dark:text-slate-400 truncate">{{ userInfo.email }}</p>
            </div>
          </div>
        </div>

        <!-- Navigation -->
        <nav class="flex-1 overflow-y-auto p-4 space-y-1">
          <NuxtLink
            v-for="item in navItems"
            :key="item.href"
            :to="item.href"
            @click="handleNavigate(item)"
            :class="cn(
              'flex items-center justify-between px-4 py-3 rounded-xl transition-all duration-200',
              'hover:bg-slate-50 dark:hover:bg-slate-800/50',
              item.active 
                ? 'bg-gradient-to-r from-orange-50 to-orange-100/50 dark:from-orange-900/20 dark:to-orange-900/10 text-orange-600 dark:text-orange-400 shadow-sm'
                : 'text-slate-600 dark:text-slate-400 hover:text-slate-900 dark:hover:text-white'
            )"
          >
            <div class="flex items-center space-x-3">
              <component 
                :is="item.icon" 
                :class="cn(
                  'w-5 h-5 transition-transform',
                  item.active && 'scale-110'
                )" 
              />
              <span class="font-medium text-sm">{{ item.name }}</span>
            </div>
            
            <UiBadge
              v-if="item.badge"
              variant="destructive"
              class="min-w-[20px] h-5 flex items-center justify-center px-1.5 text-xs"
            >
              {{ item.badge }}
            </UiBadge>
          </NuxtLink>
        </nav>

        <!-- Footer -->
        <div class="p-4 border-t border-slate-200 dark:border-slate-800">
          <MaterialButton
            variant="outlined"
            color="secondary"
            size="sm"
            full-width
            @click="$router.push('/auth/logout')"
          >
            <svg class="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
            </svg>
            Sign out
          </MaterialButton>
        </div>
      </aside>
    </Transition>
  </div>
</template>
