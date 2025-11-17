<template>
  <div class="p-4 border rounded-lg" :class="urgencyColor">
    <div class="flex items-center justify-between mb-2">
      <h4 class="text-sm font-medium">{{ item.name }}</h4>
      <Badge :variant="urgencyVariant" class="text-xs">
        {{ item.urgency }}
      </Badge>
    </div>
    <div class="space-y-1 text-xs text-muted-foreground">
      <p>Stock: {{ item.currentStock }} | Reorder at: {{ item.reorderLevel }}</p>
      <p>Supplier: {{ item.supplier }}</p>
    </div>
    <div class="mt-3">
      <Button size="sm" variant="outline" class="h-7 px-3 text-xs w-full">
        Order Now
      </Button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { Button } from '../ui/button'
import { Badge } from '../ui/badge'

interface LowStockItem {
  id: number
  name: string
  currentStock: number
  reorderLevel: number
  supplier: string
  urgency: 'high' | 'medium' | 'low'
}

interface Props {
  item: LowStockItem
}

const props = defineProps<Props>()

const urgencyColor = computed(() => {
  switch (props.item.urgency) {
    case 'high':
      return 'border-red-200 bg-red-50/50 dark:border-red-800 dark:bg-red-900/20'
    case 'medium':
      return 'border-orange-200 bg-orange-50/50 dark:border-orange-800 dark:bg-orange-900/20'
    case 'low':
      return 'border-yellow-200 bg-yellow-50/50 dark:border-yellow-800 dark:bg-yellow-900/20'
    default:
      return 'border-border'
  }
})

const urgencyVariant = computed(() => {
  switch (props.item.urgency) {
    case 'high':
      return 'destructive'
    case 'medium':
      return 'secondary'
    case 'low':
      return 'outline'
    default:
      return 'outline'
  }
})
</script>