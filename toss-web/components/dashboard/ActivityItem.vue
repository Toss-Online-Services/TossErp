<template>
  <div class="flex items-center space-x-3 p-3 rounded-lg border hover:bg-accent/50 transition-colors">
    <div class="p-2 rounded-full bg-secondary">
      <Component :is="activity.icon" class="h-4 w-4" />
    </div>
    <div class="flex-1 min-w-0">
      <p class="text-sm font-medium">{{ activity.description }}</p>
      <div class="flex items-center justify-between mt-1">
        <p class="text-xs text-muted-foreground">{{ activity.time }}</p>
        <p class="text-sm font-medium text-primary">{{ activity.amount }}</p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
interface Activity {
  id: number
  type: string
  description: string
  amount: string
  time: string
  icon: any
}

interface Props {
  activity: Activity
}

defineProps<Props>()
</script>