<script setup lang="ts">
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '~/components/ui/card'
import { Button } from '~/components/ui/button'
import { Badge } from '~/components/ui/badge'

const gallery = Array.from({ length: 4 }, (_, idx) => ({
  id: idx,
  image: 'https://images.unsplash.com/photo-1518544801976-3e159e0050bd?auto=format&fit=crop&w=400&q=80',
}))
</script>

<template>
  <section class="space-y-4">
    <div>
      <p class="text-sm text-muted-foreground">ecommerce-sections-02</p>
      <h2 class="text-xl font-semibold">Luxury product detail page with gallery</h2>
    </div>
    <Card class="border-border/70">
      <CardContent class="grid gap-6 lg:grid-cols-[1.1fr,1fr]">
        <div class="space-y-4">
          <div class="aspect-square overflow-hidden rounded-2xl bg-muted/40">
            <img
              src="https://images.unsplash.com/photo-1518544801976-3e159e0050bd?auto=format&fit=crop&w=800&q=80"
              alt="Artisan Leather Crossbody"
              class="h-full w-full object-cover"
            >
          </div>
          <div class="grid grid-cols-4 gap-3">
            <div
              v-for="thumb in gallery"
              :key="thumb.id"
              class="aspect-square rounded-xl border border-border/80"
            />
          </div>
        </div>
        <div class="space-y-4">
          <Badge variant="secondary" class="w-fit">
            Trending now
          </Badge>
          <CardTitle class="text-3xl">Artisan Leather Crossbody</CardTitle>
          <CardDescription>
            Elegant crossbody bag crafted from premium vegetable-tanned leather with a minimalist silhouette.
          </CardDescription>
          <div class="flex items-center justify-between">
            <p class="text-3xl font-semibold">$385.00</p>
            <p class="text-sm text-muted-foreground">(4.7)</p>
          </div>
          <p class="text-sm text-muted-foreground">
            This is a demo store. To buy this product, visit Maison Atelier official store.
          </p>
          <div class="flex gap-3">
            <Button class="flex-1">
              Add to cart
            </Button>
            <Button variant="outline" class="flex-1">
              Buy it now
            </Button>
          </div>
          <div class="grid gap-2 text-sm text-muted-foreground">
            <p>Crossbody style · Premium leather · Adjustable strap · Crafted in France</p>
          </div>
        </div>
      </CardContent>
    </Card>
  </section>
</template>

