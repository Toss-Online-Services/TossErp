<script setup lang="ts">
import { useOfflineStatus } from '~/composables/useOfflineStatus'

const { isOnline } = useOfflineStatus()
</script>

<template>
  <transition name="fade">
    <div
      v-if="!isOnline"
      class="fixed bottom-4 left-4 right-4 z-40 rounded-xl border border-warning/40 bg-warning/10 px-4 py-3 text-warning shadow-lg backdrop-blur md:left-auto md:w-96"
    >
      <div class="flex items-center gap-2">
        <span class="material-symbols-rounded text-lg">cloud_off</span>
        <div class="text-sm">
          <p class="font-semibold">Offline mode</p>
          <p class="text-muted-foreground">You can continue working; changes will sync when back online.</p>
        </div>
      </div>
    </div>
  </transition>
</template>

<style scoped>
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.2s ease;
}
.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>

