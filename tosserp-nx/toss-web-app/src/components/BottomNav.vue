<script setup lang="ts">
import { computed } from 'vue'
import { useRoute } from 'vue-router'
import {
  LayoutDashboard,
  ShoppingBag,
  Boxes,
  DollarSign,
  Users,
  Truck,
  Settings
} from 'lucide-vue-next'

const route = useRoute()

const navItems = [
  {
    name: 'Home',
    path: '/dashboard',
    icon: LayoutDashboard
  },
  {
    name: 'Sales',
    path: '/sales',
    icon: ShoppingBag
  },
  {
    name: 'Stock',
    path: '/stock',
    icon: Boxes
  },
  {
    name: 'Money',
    path: '/money',
    icon: DollarSign
  },
  {
    name: 'People',
    path: '/people',
    icon: Users
  },
  {
    name: 'Jobs',
    path: '/jobs',
    icon: Truck
  },
  {
    name: 'Settings',
    path: '/settings',
    icon: Settings
  }
]

const isActive = (path: string) => {
  return route.path.startsWith(path)
}
</script>

<template>
  <nav class="fixed bottom-0 left-0 right-0 bg-card border-t z-50 lg:hidden safe-area-bottom">
    <div class="flex items-center justify-around h-16 px-2">
      <NuxtLink
        v-for="item in navItems"
        :key="item.path"
        :to="item.path"
        :class="[
          'flex flex-col items-center justify-center gap-1 flex-1 h-full rounded-t-lg transition-colors min-w-0',
          isActive(item.path)
            ? 'text-primary bg-primary/10'
            : 'text-muted-foreground hover:text-foreground hover:bg-accent'
        ]"
      >
        <component :is="item.icon" :size="20" />
        <span class="text-[10px] font-medium truncate w-full text-center">{{ item.name }}</span>
      </NuxtLink>
    </div>
  </nav>
</template>

<style scoped>
/* Safe area for devices with notches/home indicators */
.safe-area-bottom {
  padding-bottom: env(safe-area-inset-bottom);
}
</style>

