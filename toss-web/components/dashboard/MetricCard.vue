<template>
  <Card class="h-full">
    <CardContent class="p-6">
      <div class="flex items-center space-x-4">
        <div class="p-3 rounded-full" :class="iconColorClass">
          <Component :is="icon" class="h-6 w-6" />
        </div>
        <div class="flex-1 min-w-0">
          <p class="text-sm font-medium text-muted-foreground">{{ title }}</p>
          <div class="flex items-baseline space-x-2">
            <h3 class="text-2xl font-bold">{{ value }}</h3>
            <Badge 
              :variant="trend === 'up' ? 'default' : 'secondary'"
              class="text-xs"
            >
              <Component 
                :is="trend === 'up' ? TrendingUp : TrendingDown" 
                class="h-3 w-3 mr-1" 
              />
              {{ change }}
            </Badge>
          </div>
        </div>
      </div>
    </CardContent>
  </Card>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { TrendingUp, TrendingDown } from 'lucide-vue-next'
import { Card, CardContent } from '../ui/card'
import { Badge } from '../ui/badge'

interface Props {
  title: string
  value: string
  change: string
  trend: 'up' | 'down'
  icon: any
  color: 'green' | 'blue' | 'purple' | 'orange' | 'red'
}

const props = defineProps<Props>()

const iconColorClass = computed(() => {
  const colorMap = {
    green: 'bg-green-100 text-green-600 dark:bg-green-900/20 dark:text-green-400',
    blue: 'bg-blue-100 text-blue-600 dark:bg-blue-900/20 dark:text-blue-400',
    purple: 'bg-purple-100 text-purple-600 dark:bg-purple-900/20 dark:text-purple-400',
    orange: 'bg-orange-100 text-orange-600 dark:bg-orange-900/20 dark:text-orange-400',
    red: 'bg-red-100 text-red-600 dark:bg-red-900/20 dark:text-red-400'
  }
  return colorMap[props.color]
})
</script>