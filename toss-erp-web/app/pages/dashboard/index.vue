<template>
  <div class="flex min-h-[60vh] items-center justify-center">
    <div class="text-center space-y-3">
      <p class="text-sm text-muted-foreground">Redirecting to analytics...</p>
      <NuxtLink
        to="/dashboard/analytics"
        class="inline-flex items-center gap-2 rounded-lg bg-gradient-primary px-4 py-2 text-sm font-semibold text-white shadow-material hover:shadow-material-lg"
      >
        Go to Analytics
        <span class="material-symbols-rounded text-base">arrow_forward</span>
      </NuxtLink>
    </div>
  </div>
</template>

<script setup lang="ts">
// @ts-nocheck
import { definePageMeta } from 'nuxt/app'

definePageMeta({
  layout: 'dashboard',
  middleware: 'auth',
  redirect: '/dashboard/analytics'
})
</script>
