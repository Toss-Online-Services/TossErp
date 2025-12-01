<script setup lang="ts">
import Card from '@/components/ui/Card.vue'
import CardHeader from '@/components/ui/CardHeader.vue'
import CardTitle from '@/components/ui/CardTitle.vue'
import CardContent from '@/components/ui/CardContent.vue'
import { ShoppingBag, Plus } from 'lucide-vue-next'
</script>

<template>
  <div class="space-y-6">
    <div class="flex items-center justify-between">
      <div>
        <h1 class="text-2xl md:text-3xl font-bold tracking-tight">Sales</h1>
        <p class="text-muted-foreground mt-1">Manage sales, POS, and invoices</p>
      </div>
      <NuxtLink
        to="/sales/pos"
        class="inline-flex items-center gap-2 px-4 py-2 bg-primary text-primary-foreground rounded-md hover:bg-primary/90 transition-colors"
      >
        <Plus :size="18" />
        <span>New Sale</span>
      </NuxtLink>
    </div>

    <div class="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
      <NuxtLink
        to="/sales/pos"
        class="block"
      >
        <Card class="hover:shadow-md transition-shadow cursor-pointer">
          <CardHeader>
            <CardTitle class="flex items-center gap-2">
              <ShoppingBag class="h-5 w-5 text-primary" />
              Point of Sale
            </CardTitle>
          </CardHeader>
          <CardContent>
            <p class="text-sm text-muted-foreground">
              Offline-first POS for quick transactions
            </p>
          </CardContent>
        </Card>
      </NuxtLink>
    </div>
  </div>
</template>

