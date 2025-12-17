<script setup lang="ts">
import { computed, inject, type Ref } from 'vue'
import { useRoute } from '#imports'
import {
  LayoutDashboard,
  FileSignature,
  ShoppingBag,
  ReceiptText,
  Truck,
  FileSpreadsheet,
  Package,
  Users,
  Boxes,
  Bell,
  ArrowLeftRight,
  Wallet,
  Banknote,
  BarChart3,
  ContactRound,
  CircleUserRound,
  FolderKanban,
  CheckSquare,
  CreditCard,
  Settings,
  ShieldCheck,
  ChevronLeft,
  ChevronRight
} from 'lucide-vue-next'

export interface NavLink {
  label: string
  to: string
  icon?: string
}

export interface NavSection {
  label: string
  items: NavLink[]
}

const props = defineProps<{
  sections: NavSection[]
  collapsed?: boolean
  onNavigate?: () => void
  onToggleCollapse?: () => void
}>()

const sidebarCollapsed = inject<Ref<boolean>>('sidebarCollapsed')
const toggleCollapse = inject<() => void>('toggleCollapse')

const isCollapsed = computed(() => props.collapsed ?? sidebarCollapsed?.value ?? false)
const handleToggle = () => props.onToggleCollapse?.() ?? toggleCollapse?.()

const iconMap: Record<string, any> = {
  dashboard: LayoutDashboard,
  quotation: FileSignature,
  salesorder: ShoppingBag,
  invoice: ReceiptText,
  delivery: Truck,
  po: FileSpreadsheet,
  receipt: Package,
  supplier: Users,
  items: Boxes,
  alerts: Bell,
  movement: ArrowLeftRight,
  accounts: Wallet,
  cashbook: Banknote,
  reports: BarChart3,
  customers: ContactRound,
  leads: CircleUserRound,
  projects: FolderKanban,
  tasks: CheckSquare,
  pos: CreditCard,
  settings: Settings,
  users: ShieldCheck
}

const route = useRoute()

const isActive = (to: string) => computed(() => route.path === to || (to !== '/' && route.path.startsWith(to)))
</script>

<template>
  <div class="h-full flex flex-col bg-white dark:bg-stone-900">
    <!-- Header with collapse button -->
    <div class="p-6 pb-0 flex items-center justify-between">
      <h1 v-if="!isCollapsed" class="text-lg font-semibold text-stone-900 dark:text-white">
        TOSS ERP
      </h1>
      <button
        @click="handleToggle"
        class="p-1.5 hover:bg-stone-100 dark:hover:bg-stone-800 rounded-md transition-colors hidden lg:flex items-center justify-center text-stone-600 dark:text-stone-400 hover:text-stone-900 dark:hover:text-white"
        :class="{ 'mx-auto': isCollapsed }"
        :title="isCollapsed ? 'Expand sidebar' : 'Collapse sidebar'"
      >
        <ChevronRight v-if="isCollapsed" :size="20" />
        <ChevronLeft v-else :size="20" />
      </button>
    </div>

    <!-- Navigation -->
    <nav class="flex-1 overflow-y-auto p-4 space-y-2">
      <div
        v-for="section in sections"
        :key="section.label"
        class="space-y-1"
      >
        <p
          v-if="!isCollapsed"
          class="px-4 text-xs font-semibold text-stone-500 dark:text-stone-400 uppercase tracking-wide mb-2"
        >
          {{ section.label }}
        </p>
        <div v-else class="border-t border-stone-200 dark:border-stone-700 my-2" />
        
        <ul class="space-y-1">
          <li v-for="item in section.items" :key="item.to">
            <NuxtLink
              :to="item.to"
              class="nav-link flex items-center gap-3 rounded-lg cursor-pointer transition-all duration-300 group relative"
              :class="[
                isActive(item.to).value
                  ? 'nav-active px-3 py-2 shadow-sm hover:shadow-md bg-stone-800 hover:bg-stone-700 border border-stone-900 text-stone-50'
                  : 'nav-inactive px-3 py-2 text-stone-700 dark:text-stone-300 hover:bg-stone-100 dark:hover:bg-stone-800 border border-transparent',
                { 'justify-center': isCollapsed }
              ]"
              @click="onNavigate?.()"
              :title="isCollapsed ? item.label : undefined"
            >
              <component
                v-if="item.icon && iconMap[item.icon]"
                :is="iconMap[item.icon]"
                class="w-4 h-4 shrink-0"
              />
              <span v-if="!isCollapsed" class="text-sm font-normal truncate">{{ item.label }}</span>
              
              <!-- Tooltip for collapsed state -->
              <div
                v-if="isCollapsed"
                class="absolute left-full ml-2 px-2 py-1 bg-stone-800 text-stone-50 text-xs rounded-md shadow-lg opacity-0 invisible group-hover:opacity-100 group-hover:visible transition-all duration-200 whitespace-nowrap z-50"
              >
                {{ item.label }}
              </div>
            </NuxtLink>
          </li>
        </ul>
      </div>
    </nav>
  </div>
</template>

<style scoped>
.nav-active {
  background-image: linear-gradient(to bottom, rgb(68 64 60), rgb(41 37 36));
  position: relative;
}

.nav-active::after {
  content: '';
  position: absolute;
  inset: 0;
  border-radius: inherit;
  box-shadow: inset 0 1px 0px rgba(255, 255, 255, 0.25), inset 0 -2px 0px rgba(0, 0, 0, 0.35);
  pointer-events: none;
}

.nav-active:hover {
  background-image: linear-gradient(to bottom, rgb(41 37 36), rgb(41 37 36));
}
</style>
