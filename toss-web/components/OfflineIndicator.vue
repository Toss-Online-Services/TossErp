<script setup lang="ts">
import { useOffline } from '~/composables/useOffline'

const { isOnline, queue, isSyncing } = useOffline()
</script>

<template>
  <!-- Offline Banner -->
  <div
    v-if="!isOnline"
    class="fixed top-0 left-0 right-0 z-50 bg-orange-500 text-white px-4 py-2 text-center text-sm font-medium shadow-lg"
  >
    <div class="flex items-center justify-center gap-2">
      <i class="material-symbols-rounded text-lg">cloud_off</i>
      <span>You're offline. Changes will be saved and synced when you reconnect.</span>
      <span v-if="queue.length > 0" class="ml-2 px-2 py-0.5 bg-white/20 rounded-full text-xs">
        {{ queue.length }} pending
      </span>
    </div>
  </div>

  <!-- Syncing Indicator -->
  <div
    v-if="isOnline && isSyncing"
    class="fixed top-0 left-0 right-0 z-50 bg-blue-500 text-white px-4 py-2 text-center text-sm font-medium shadow-lg"
  >
    <div class="flex items-center justify-center gap-2">
      <i class="material-symbols-rounded text-lg animate-spin">sync</i>
      <span>Syncing {{ queue.length }} pending changes...</span>
    </div>
  </div>

  <!-- Back Online Notification -->
  <Transition
    enter-active-class="transition ease-out duration-300"
    enter-from-class="transform translate-y-full opacity-0"
    enter-to-class="transform translate-y-0 opacity-100"
    leave-active-class="transition ease-in duration-200"
    leave-from-class="transform translate-y-0 opacity-100"
    leave-to-class="transform translate-y-full opacity-0"
  >
    <div
      v-if="isOnline && queue.length === 0 && !isSyncing"
      class="fixed bottom-4 right-4 bg-green-500 text-white px-4 py-3 rounded-lg shadow-lg flex items-center gap-2"
    >
      <i class="material-symbols-rounded">check_circle</i>
      <span>All changes synced</span>
    </div>
  </Transition>
</template>

<style scoped>
@keyframes spin {
  from {
    transform: rotate(0deg);
  }
  to {
    transform: rotate(360deg);
  }
}

.animate-spin {
  animation: spin 1s linear infinite;
}
</style>

