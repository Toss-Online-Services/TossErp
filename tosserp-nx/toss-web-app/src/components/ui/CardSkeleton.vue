<script setup lang="ts">
import Skeleton from './Skeleton.vue'
</script>

<template>
  <div class="rounded-lg border bg-card p-6 space-y-4">
    <div class="flex items-center justify-between">
      <Skeleton class="h-4 w-24" />
      <Skeleton class="h-4 w-4 rounded-full" />
    </div>
    <Skeleton class="h-8 w-32" />
    <Skeleton class="h-3 w-20" />
  </div>
</template>

