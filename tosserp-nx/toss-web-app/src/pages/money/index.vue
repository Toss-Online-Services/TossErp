<script setup lang="ts">
import Card from '@/components/ui/Card.vue'
import CardHeader from '@/components/ui/CardHeader.vue'
import CardTitle from '@/components/ui/CardTitle.vue'
import CardContent from '@/components/ui/CardContent.vue'
import { DollarSign, TrendingUp, TrendingDown } from 'lucide-vue-next'
</script>

<template>
  <div class="space-y-6">
    <div>
      <h1 class="text-2xl md:text-3xl font-bold tracking-tight">Money</h1>
      <p class="text-muted-foreground mt-1">Track money in, money out, and what's left</p>
    </div>

    <div class="grid gap-4 md:grid-cols-3">
      <Card>
        <CardHeader>
          <CardTitle class="text-sm font-medium flex items-center gap-2">
            <TrendingUp class="h-4 w-4 text-emerald-600" />
            Money In
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div class="text-2xl font-bold">R 0</div>
          <p class="text-xs text-muted-foreground mt-1">Today</p>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle class="text-sm font-medium flex items-center gap-2">
            <TrendingDown class="h-4 w-4 text-red-600" />
            Money Out
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div class="text-2xl font-bold">R 0</div>
          <p class="text-xs text-muted-foreground mt-1">Today</p>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle class="text-sm font-medium flex items-center gap-2">
            <DollarSign class="h-4 w-4 text-primary" />
            What's Left
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div class="text-2xl font-bold">R 0</div>
          <p class="text-xs text-muted-foreground mt-1">Available</p>
        </CardContent>
      </Card>
    </div>
  </div>
</template>

