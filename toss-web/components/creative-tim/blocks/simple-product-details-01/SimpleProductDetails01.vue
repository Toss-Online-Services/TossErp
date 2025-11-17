<script setup lang="ts">
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '~/components/ui/card'
import { Badge } from '~/components/ui/badge'
import { Button } from '~/components/ui/button'
import { Separator } from '~/components/ui/separator'
import { Tabs, TabsContent, TabsList, TabsTrigger } from '~/components/ui/tabs'

const product = {
  title: 'Pink Blouse',
  headline: 'Simple product details grid',
  price: '1,490',
  rating: 4.8,
  reviews: 100,
  description:
    "Whether you're closing deals or attending formal events, its breathable lining and natural stretch keep you comfortable and sharp from day to night.",
  colors: ['Rose Quartz', 'Onyx', 'Sable'],
  sizes: ['XS', 'S', 'M', 'L', 'XL', 'XXL'],
  highlights: [
    'Soft touch mercerised cotton',
    'Hand finished cuffs',
    'Wrinkle resistant finish',
    '15-day free returns',
  ],
}

const similarItems = [
  {
    title: 'Black Bustier Top',
    price: '49.95',
    note: 'Available in 5 sizes',
  },
  {
    title: 'High-Waist Jeans',
    price: '49.95',
    note: 'Available in 8 sizes',
  },
]
</script>

<template>
  <section class="grid gap-6 lg:grid-cols-2">
    <Card class="border-border/70">
      <CardContent class="space-y-4 p-6">
        <div class="rounded-2xl bg-muted/50 p-6">
          <div class="aspect-square overflow-hidden rounded-xl bg-gradient-to-br from-primary/10 to-secondary/20">
            <img
              src="https://images.unsplash.com/photo-1503341455253-b2e723bb3dbb?auto=format&fit=crop&w=600&q=80"
              alt="Pink blouse"
              class="h-full w-full object-cover"
            >
          </div>
        </div>
        <div class="grid grid-cols-3 gap-3">
          <div
            v-for="idx in 3"
            :key="idx"
            class="aspect-square rounded-xl border border-border/80 bg-muted/50"
          />
        </div>
      </CardContent>
    </Card>

    <Card class="border-border/70">
      <CardHeader class="space-y-4">
        <div>
          <Badge variant="secondary" class="mb-2 w-fit">
            simple-product-details-01
          </Badge>
          <CardTitle class="text-2xl">{{ product.title }}</CardTitle>
          <CardDescription>{{ product.headline }}</CardDescription>
        </div>
        <div class="flex flex-wrap items-center gap-3">
          <p class="text-3xl font-semibold">R {{ product.price }}</p>
          <div class="flex items-center gap-2 text-sm text-muted-foreground">
            <span class="font-medium text-foreground">{{ product.rating }}</span>
            <span>({{ product.reviews }} reviews)</span>
          </div>
        </div>
        <p class="text-sm text-muted-foreground">
          {{ product.description }}
        </p>
      </CardHeader>
      <CardContent class="space-y-6">
        <div class="space-y-3">
          <p class="text-sm font-semibold uppercase tracking-wide text-muted-foreground">Size</p>
          <div class="flex flex-wrap gap-2">
            <Button
              v-for="size in product.sizes"
              :key="size"
              variant="outline"
              size="sm"
              class="rounded-full px-4"
            >
              {{ size }}
            </Button>
          </div>
        </div>

        <div class="space-y-3">
          <p class="text-sm font-semibold uppercase tracking-wide text-muted-foreground">Color</p>
          <div class="flex flex-wrap gap-2">
            <Button
              v-for="color in product.colors"
              :key="color"
              variant="outline"
              size="sm"
              class="rounded-full px-4"
            >
              {{ color }}
            </Button>
          </div>
        </div>

        <div class="grid gap-3 sm:grid-cols-2">
          <Button size="lg">
            Add to Cart
          </Button>
          <Button variant="outline" size="lg">
            Save to Wishlist
          </Button>
        </div>

        <Tabs default-value="info" class="w-full">
          <TabsList class="grid w-full grid-cols-3">
            <TabsTrigger value="info">
              More Info
            </TabsTrigger>
            <TabsTrigger value="description">
              Description
            </TabsTrigger>
            <TabsTrigger value="delivery">
              Delivery & Returns
            </TabsTrigger>
          </TabsList>
          <TabsContent value="info">
            <ul class="list-disc space-y-1 pl-5 text-sm text-muted-foreground">
              <li v-for="highlight in product.highlights" :key="highlight">
                {{ highlight }}
              </li>
            </ul>
          </TabsContent>
          <TabsContent value="description">
            <p class="text-sm text-muted-foreground">
              Whether you're navigating city streets or embarking on outdoor adventures, this blouse ensures you stay
              comfortable wherever you go.
            </p>
          </TabsContent>
          <TabsContent value="delivery">
            <p class="text-sm text-muted-foreground">
              Free delivery nationwide on orders above R1,200. Collect in-store within 2 hours.
            </p>
          </TabsContent>
        </Tabs>

        <Separator />

        <div class="space-y-3">
          <p class="text-sm font-semibold text-muted-foreground">Similar Items</p>
          <div class="space-y-3">
            <div
              v-for="item in similarItems"
              :key="item.title"
              class="flex items-center justify-between rounded-lg border border-dashed border-border/70 p-3 text-sm"
            >
              <div>
                <p class="font-medium">{{ item.title }}</p>
                <p class="text-xs text-muted-foreground">{{ item.note }}</p>
              </div>
              <p class="font-semibold">€{{ item.price }}</p>
            </div>
          </div>
        </div>
      </CardContent>
    </Card>
  </section>
</template>

