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

const hideChrome = computed(() => {
  if (route.meta?.hideChrome) {
    return true
  }
  return route.path.startsWith('/auth')
})

const pageTitle = computed(() => (route.meta?.title as string) || 'ERP Overview')
const pageSubtitle = computed(() => (route.meta?.subtitle as string) || 'Manage your operations')

const resolvedRole = computed<Role>(() => {
  const metaRole = route.meta?.role as Role | undefined
  const userRole = user.value?.role as Role | undefined
  return metaRole || userRole || 'retailer'
})
</script>
