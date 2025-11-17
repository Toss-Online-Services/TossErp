<script setup lang="ts">
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '~/components/ui/card'
import { Button } from '~/components/ui/button'

const products = [
  { title: 'Satin Wrap Dress', brand: 'Maison Aurore', price: '$215.00' },
  { title: 'Structured Blazer', brand: 'Atelier No. 9', price: '$329.00' },
  { title: 'Chunky Chelsea Boots', brand: 'Linea Forte', price: 'From $180.00' },
]
</script>

<template>
  <section class="space-y-4">
    <div>
      <p class="text-sm text-muted-foreground">ecommerce-sections-01</p>
      <h2 class="text-xl font-semibold">Runway video spotlight with product cards</h2>
    </div>
    <Card class="border-border/70">
      <CardHeader>
        <CardTitle>Runway Spotlight</CardTitle>
        <CardDescription>
          Cinematic runway loop featuring Maison Atelier designs.
        </CardDescription>
      </CardHeader>
      <CardContent class="grid gap-6 lg:grid-cols-[1.3fr,1fr]">
        <div class="space-y-4">
          <div class="aspect-video overflow-hidden rounded-2xl bg-black">
            <div class="flex h-full items-center justify-center text-white">
              <div class="text-center">
                <p class="text-sm uppercase tracking-widest text-white/60">Runway edit</p>
                <p class="text-3xl font-semibold">FW25 Capsule</p>
                <Button variant="secondary" class="mt-4">
                  Watch showreel
                </Button>
              </div>
            </div>
          </div>
          <Button variant="outline" class="w-full">
            View all products
          </Button>
        </div>
        <div class="space-y-4">
          <Card
            v-for="product in products"
            :key="product.title"
            class="border-border/80 bg-muted/20"
          >
            <CardContent class="flex items-center justify-between p-4">
              <div>
                <p class="text-xs uppercase tracking-wide text-muted-foreground">{{ product.brand }}</p>
                <p class="font-semibold">{{ product.title }}</p>
                <p class="text-sm text-muted-foreground">{{ product.price }}</p>
              </div>
              <Button size="sm" variant="ghost">
                View
              </Button>
            </CardContent>
          </Card>
        </div>
      </CardContent>
    </Card>
  </section>
</template>

