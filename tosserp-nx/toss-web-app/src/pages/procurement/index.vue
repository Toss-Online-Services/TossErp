<script setup lang="ts">
import Card from '@/components/ui/Card.vue'
import CardHeader from '@/components/ui/CardHeader.vue'
import CardTitle from '@/components/ui/CardTitle.vue'
import CardContent from '@/components/ui/CardContent.vue'
import Breadcrumbs from '@/components/ui/Breadcrumbs.vue'
import Button from '@/components/ui/Button.vue'
import { ShoppingCart, FileText, PackageCheck, Plus } from 'lucide-vue-next'
</script>

<template>
  <div class="space-y-6">
    <div class="flex items-center justify-between">
      <div>
        <Breadcrumbs />
        <h1 class="text-2xl md:text-3xl font-bold tracking-tight mt-2">Procurement</h1>
        <p class="text-muted-foreground mt-1">Manage purchase requests, orders, and receipts</p>
      </div>
      <Button>
        <Plus :size="18" class="mr-2" />
        New Purchase Request
      </Button>
    </div>

    <div class="grid gap-4 md:grid-cols-3">
      <NuxtLink to="/procurement/requests" class="block">
        <Card class="hover:shadow-material-md transition-shadow cursor-pointer">
          <CardHeader>
            <CardTitle class="flex items-center gap-2">
              <FileText class="h-5 w-5 text-primary" />
              Purchase Requests
            </CardTitle>
          </CardHeader>
          <CardContent>
            <p class="text-sm text-muted-foreground">
              Create and manage purchase requests
            </p>
          </CardContent>
        </Card>
      </NuxtLink>

      <NuxtLink to="/procurement/orders" class="block">
        <Card class="hover:shadow-material-md transition-shadow cursor-pointer">
          <CardHeader>
            <CardTitle class="flex items-center gap-2">
              <ShoppingCart class="h-5 w-5 text-primary" />
              Purchase Orders
            </CardTitle>
          </CardHeader>
          <CardContent>
            <p class="text-sm text-muted-foreground">
              Track purchase orders and approvals
            </p>
          </CardContent>
        </Card>
      </NuxtLink>

      <NuxtLink to="/procurement/receipts" class="block">
        <Card class="hover:shadow-material-md transition-shadow cursor-pointer">
          <CardHeader>
            <CardTitle class="flex items-center gap-2">
              <PackageCheck class="h-5 w-5 text-primary" />
              Goods Receipts
            </CardTitle>
          </CardHeader>
          <CardContent>
            <p class="text-sm text-muted-foreground">
              Record goods received from suppliers
            </p>
          </CardContent>
        </Card>
      </NuxtLink>
    </div>
  </div>
</template>

