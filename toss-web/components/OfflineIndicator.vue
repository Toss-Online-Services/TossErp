<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'

const isOnline = ref(true)

function updateOnlineStatus() {
  isOnline.value = navigator.onLine
}

onMounted(() => {
  isOnline.value = navigator.onLine
  window.addEventListener('online', updateOnlineStatus)
  window.addEventListener('offline', updateOnlineStatus)
})

onUnmounted(() => {
  window.removeEventListener('online', updateOnlineStatus)
  window.removeEventListener('offline', updateOnlineStatus)
})
</script>

<template>
  <Transition name="slide-fade">
    <div
      v-if="!isOnline"
      class="fixed bottom-4 left-4 right-4 z-50 rounded-xl border border-warning/40 bg-warning/10 px-4 py-3 shadow-lg backdrop-blur md:left-auto md:w-96"
    >
      <div class="flex items-center gap-3">
        <span class="material-symbols-rounded text-2xl text-warning">cloud_off</span>
        <div class="text-sm">
          <p class="font-semibold text-warning">You're offline</p>
          <p class="text-muted-foreground">Changes will sync when you're back online.</p>
        </div>
      </div>
    </div>
  </Transition>
</template>

<style scoped>
.slide-fade-enter-active {
  transition: all 0.3s ease-out;
}
.slide-fade-leave-active {
  transition: all 0.2s ease-in;
}
.slide-fade-enter-from,
.slide-fade-leave-to {
  transform: translateY(20px);
  opacity: 0;
}
</style>
