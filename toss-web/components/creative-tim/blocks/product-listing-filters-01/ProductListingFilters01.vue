<script setup lang="ts">
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '~/components/ui/card'
import { Button } from '~/components/ui/button'
import { Badge } from '~/components/ui/badge'
import { Checkbox } from '~/components/ui/checkbox'
import { Label } from '~/components/ui/label'

const filters = [
  { label: 'Exclusive', count: 18 },
  { label: 'New Arrival', count: 24 },
  { label: 'On Sale', count: 32 },
]

const products = [
  { name: 'Cable-knit cashmere cardigan', brand: 'Zegna', price: '€3,450', badge: 'Exclusive' },
  { name: 'Cotton and cashmere shirt', brand: 'Zegna', price: '€675' },
  { name: 'Wool straight pants', brand: 'Zegna', price: '€1,450', badge: 'Exclusive' },
  { name: 'Cashmere sweater', brand: 'Zegna', price: '€1,950', badge: 'New Arrival' },
]
</script>

<template>
  <section class="space-y-4">
    <div>
      <p class="text-sm text-muted-foreground">product-listing-filters-01</p>
      <h2 class="text-xl font-semibold">Product listing with filters & sort</h2>
    </div>
    <Card class="border-border/70">
      <CardHeader class="flex flex-col gap-2 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <CardTitle>462 Products</CardTitle>
          <CardDescription>Filter by collection, style, or promotion.</CardDescription>
        </div>
        <Button variant="outline" size="sm">
          Sort: Featured
        </Button>
      </CardHeader>
      <CardContent class="grid gap-6 lg:grid-cols-[280px,1fr]">
        <div class="rounded-xl border border-border/80 p-4">
          <p class="text-sm font-semibold text-muted-foreground">Filters</p>
          <div class="mt-4 space-y-3">
            <div
              v-for="filter in filters"
              :key="filter.label"
              class="flex items-center justify-between rounded-lg border border-dashed border-border/70 p-3 text-sm"
            >
              <Label class="flex items-center gap-2 font-medium">
                <Checkbox :checked="filter.label === 'Exclusive'" />
                {{ filter.label }}
              </Label>
              <span class="text-xs text-muted-foreground">{{ filter.count }}</span>
            </div>
          </div>
          <Button variant="secondary" class="mt-4 w-full" size="sm">
            Clear filters
          </Button>
        </div>

        <div class="grid gap-4 sm:grid-cols-2">
          <Card
            v-for="product in products"
            :key="product.name"
            class="border-border/70"
          >
            <CardContent class="space-y-3 p-4">
              <div class="aspect-[4/3] rounded-xl bg-muted" />
              <div class="space-y-1">
                <div class="flex items-center justify-between">
                  <p class="text-sm text-muted-foreground">{{ product.brand }}</p>
                  <Badge v-if="product.badge" variant="outline">
                    {{ product.badge }}
                  </Badge>
                </div>
                <p class="font-semibold">{{ product.name }}</p>
                <p class="text-sm text-muted-foreground">{{ product.price }}</p>
              </div>
              <Button variant="outline" class="w-full">
                View details
              </Button>
            </CardContent>
          </Card>
        </div>
      </CardContent>
    </Card>
  </section>
</template>

