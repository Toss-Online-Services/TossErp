<script setup lang="ts">
import { useThemeStore } from '~/stores/theme'
import { useOfflineStatus } from '~/composables/useOfflineStatus'

defineProps<{
  onToggleSidebar?: () => void
}>()

const theme = useThemeStore()
const { isOnline } = useOfflineStatus()
</script>

<template>
  <div class="flex items-center justify-between px-4 py-3 gap-3">
    <div class="flex items-center gap-3">
      <button
        class="lg:hidden inline-flex items-center justify-center rounded-lg border border-border px-2.5 py-2 text-sm hover:bg-muted"
        @click="onToggleSidebar?.()"
      >
        <span class="material-symbols-rounded">menu</span>
      </button>
      <div>
        <p class="text-sm text-muted-foreground">ERP-III Analytics</p>
        <p class="text-lg font-semibold">Today view</p>
      </div>
    </div>

    <div class="flex items-center gap-2">
      <span
        class="inline-flex items-center gap-1 rounded-full px-2 py-1 text-xs"
        :class="isOnline ? 'bg-success/10 text-success' : 'bg-warning/10 text-warning'"
        title="Connectivity status"
      >
        <span class="material-symbols-rounded text-base">
          {{ isOnline ? 'wifi' : 'cloud_off' }}
        </span>
        {{ isOnline ? 'Online' : 'Offline' }}
      </span>

      <button
        class="inline-flex items-center justify-center rounded-lg border border-border px-2.5 py-2 text-sm hover:bg-muted"
        @click="theme.toggle()"
        :title="theme.isDark ? 'Switch to light mode' : 'Switch to dark mode'"
      >
        <span class="material-symbols-rounded">
          {{ theme.isDark ? 'light_mode' : 'dark_mode' }}
        </span>
      </button>

      <div class="h-10 w-10 rounded-full bg-primary/10 text-primary flex items-center justify-center font-semibold">
        TU
      </div>
    </div>
  </div>
</template>

