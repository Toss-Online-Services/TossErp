<template>
  <div v-if="hideChrome" class="min-h-screen bg-background text-foreground">
    <slot />
  </div>
  <div v-else class="min-h-screen bg-background text-foreground">
    <DashboardLayout
      :role="resolvedRole"
      :page-title="pageTitle"
      :page-subtitle="pageSubtitle"
    >
      <slot />
    </DashboardLayout>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useRoute } from 'vue-router'
import DashboardLayout from '~/layouts/dashboard.vue'
import { useAuth } from '~/composables/useAuth'

type Role = 'admin' | 'retailer' | 'supplier' | 'driver'

const route = useRoute()
const { user } = useAuth()

// Get the layout name from the route - Nuxt sets this when definePageMeta specifies a layout
const currentLayout = computed(() => {
  // In Nuxt 3, when layout is specified in definePageMeta, it's available via route.meta.layout
  // But we also check the path as a fallback
  return route.meta?.layout || (route.path === '/' ? 'landing' : 'default')
})

const hideChrome = computed(() => {
  // If the page is using landing layout, hide chrome
  if (currentLayout.value === 'landing' || currentLayout.value === false) {
    return true
  }
  // Check if page explicitly wants to hide chrome
  if (route.meta?.hideChrome) {
    return true
  }
  // Root path and auth pages should hide chrome
  if (route.path === '/' || route.path.startsWith('/auth')) {
    return true
  }
  return false
})

const pageTitle = computed(() => (route.meta?.title as string) || 'ERP Overview')
const pageSubtitle = computed(() => (route.meta?.subtitle as string) || 'Manage your operations')

const resolvedRole = computed<Role>(() => {
  const metaRole = route.meta?.role as Role | undefined
  const userRole = user.value?.role as Role | undefined
  return metaRole || userRole || 'retailer'
})
</script>
