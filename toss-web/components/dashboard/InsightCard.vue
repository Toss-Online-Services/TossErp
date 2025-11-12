<template>
  <div class="p-4 border rounded-lg" :class="borderColor">
    <div class="flex items-start justify-between">
      <div class="flex-1">
        <h4 class="text-sm font-medium">{{ insight.title }}</h4>
        <p class="text-xs text-muted-foreground mt-1">{{ insight.description }}</p>
      </div>
      <Badge :variant="badgeVariant" class="text-xs">
        {{ insight.priority }}
      </Badge>
    </div>
    <Button size="sm" variant="ghost" class="mt-3 h-8 px-3 text-xs">
      {{ insight.action }}
    </Button>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { Button } from '../ui/button'
import { Badge } from '../ui/badge'

interface Insight {
  id: number
  type: 'opportunity' | 'trend' | 'risk'
  title: string
  description: string
  action: string
  priority: 'high' | 'medium' | 'low'
}

interface Props {
  insight: Insight
}

const props = defineProps<Props>()

const borderColor = computed(() => {
  switch (props.insight.type) {
    case 'opportunity':
      return 'border-green-200 bg-green-50/50 dark:border-green-800 dark:bg-green-900/20'
    case 'trend':
      return 'border-blue-200 bg-blue-50/50 dark:border-blue-800 dark:bg-blue-900/20'
    case 'risk':
      return 'border-red-200 bg-red-50/50 dark:border-red-800 dark:bg-red-900/20'
    default:
      return 'border-border'
  }
})

const badgeVariant = computed(() => {
  switch (props.insight.priority) {
    case 'high':
      return 'destructive'
    case 'medium':
      return 'secondary'
    case 'low':
      return 'outline'
    default:
      return 'secondary'
  }
})
</script>