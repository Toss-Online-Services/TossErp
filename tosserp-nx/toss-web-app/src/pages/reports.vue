<script setup lang="ts">
import Card from '@/components/ui/Card.vue'
import CardHeader from '@/components/ui/CardHeader.vue'
import CardContent from '@/components/ui/CardContent.vue'
import { BarChart3 } from 'lucide-vue-next'
</script>

<template>
  <div class="space-y-6">
    <div class="flex items-center justify-between">
      <div>
        <h1 class="text-xl font-bold tracking-tight">Reports</h1>
        <p class="text-muted-foreground">Analytics and performance metrics</p>
      </div>
    </div>
    <Card>
      <CardHeader>
        <div class="flex items-center gap-2">
          <BarChart3 class="h-4 w-4 text-muted-foreground" />
          <span class="font-semibold">Sales Overview</span>
        </div>
      </CardHeader>
      <CardContent>
        <div class="h-64 flex items-center justify-center text-muted-foreground">
          <!-- Chart.js or vue-chartjs chart will go here -->
          <span>Chart Placeholder</span>
        </div>
      </CardContent>
    </Card>
  </div>
</template>
