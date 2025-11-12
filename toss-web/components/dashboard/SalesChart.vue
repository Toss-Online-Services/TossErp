<template>
  <div class="h-80 w-full">
    <div class="p-4 border border-dashed rounded-lg flex items-center justify-center h-full">
      <div class="text-center">
        <TrendingUp class="h-12 w-12 mx-auto text-muted-foreground mb-4" />
        <h3 class="text-lg font-medium text-muted-foreground">Sales Chart</h3>
        <p class="text-sm text-muted-foreground">Chart component would go here</p>
        <p class="text-xs text-muted-foreground mt-2">
          Data: {{ data?.length || 0 }} data points
        </p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { TrendingUp } from 'lucide-vue-next'

interface SalesData {
  month: string
  revenue: number
}

interface Props {
  data: SalesData[]
}

defineProps<Props>()
</script>