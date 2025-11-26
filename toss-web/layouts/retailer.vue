<template>
  <DashboardLayout role="retailer" :page-title="pageTitle" :page-subtitle="pageSubtitle">
    <slot />
  </DashboardLayout>
</template>

<script setup lang="ts">
import DashboardLayout from '~/layouts/dashboard.vue'

interface Props {
  pageTitle?: string
  pageSubtitle?: string
}

withDefaults(defineProps<Props>(), {
  pageTitle: 'Retailer Dashboard',
  pageSubtitle: 'Manage your shop'
})
</script>

