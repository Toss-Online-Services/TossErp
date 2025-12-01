<script setup lang="ts">
import Card from '@/components/ui/Card.vue'
import CardHeader from '@/components/ui/CardHeader.vue'
import CardTitle from '@/components/ui/CardTitle.vue'
import CardContent from '@/components/ui/CardContent.vue'
import { Boxes, AlertTriangle } from 'lucide-vue-next'
</script>

<template>
  <div class="space-y-6">
    <div>
      <h1 class="text-2xl md:text-3xl font-bold tracking-tight">Stock</h1>
      <p class="text-muted-foreground mt-1">Manage inventory and stock levels</p>
    </div>

    <Card>
      <CardHeader>
        <CardTitle class="flex items-center gap-2">
          <AlertTriangle class="h-5 w-5 text-amber-600" />
          Low Stock Alerts
        </CardTitle>
      </CardHeader>
      <CardContent>
        <p class="text-sm text-muted-foreground">
          Stock management features coming soon...
        </p>
      </CardContent>
    </Card>
  </div>
</template>

