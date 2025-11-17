<script setup lang="ts">
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '~/components/ui/card'
import { Button } from '~/components/ui/button'
import { Badge } from '~/components/ui/badge'
import { Tabs, TabsContent, TabsList, TabsTrigger } from '~/components/ui/tabs'

const breadcrumbs = ['Store', 'Designers', 'Pink Blouse']
const colors = ['Soft Rose', 'Midnight', 'Ivory']
const sizes = ['XS', 'S', 'M', 'L']
</script>

<template>
  <section class="space-y-4">
    <div>
      <p class="text-sm text-muted-foreground">product-details-01</p>
      <h2 class="text-xl font-semibold">Product details with image carousel</h2>
    </div>
    <Card class="border-border/70">
      <CardContent class="grid gap-6 lg:grid-cols-[1.2fr,1fr]">
        <div class="space-y-4">
          <div class="flex flex-wrap items-center gap-2 text-xs text-muted-foreground">
            <span v-for="(crumb, idx) in breadcrumbs" :key="crumb" class="flex items-center gap-2">
              <span>{{ crumb }}</span>
              <span v-if="idx < breadcrumbs.length - 1">/</span>
            </span>
          </div>
          <div class="rounded-2xl bg-muted/40 p-4">
            <div class="aspect-square overflow-hidden rounded-xl">
              <img
                src="https://images.unsplash.com/photo-1521572163474-6864f9cf17ab?auto=format&fit=crop&w=600&q=80"
                alt="Product main view"
                class="h-full w-full object-cover"
              >
            </div>
          </div>
          <div class="grid grid-cols-4 gap-2">
            <div v-for="idx in 4" :key="idx" class="aspect-[4/3] rounded-lg bg-muted" />
          </div>
        </div>
        <div class="space-y-4">
          <div>
            <Badge variant="secondary" class="mb-2 w-fit">
              Product 1
            </Badge>
            <CardTitle class="text-3xl">Pink Blouse</CardTitle>
            <CardDescription>There's nothing I really wanted to do in life that I wasn't able to get good at.</CardDescription>
          </div>
          <div class="flex items-center gap-4">
            <p class="text-3xl font-semibold">$1,290</p>
            <p class="text-sm text-muted-foreground">(100 reviews)</p>
          </div>
          <div class="space-y-2">
            <p class="text-xs uppercase text-muted-foreground">Color</p>
            <div class="flex flex-wrap gap-2">
              <Button
                v-for="color in colors"
                :key="color"
                variant="outline"
                size="sm"
                class="rounded-full px-4"
              >
                {{ color }}
              </Button>
            </div>
          </div>
          <div class="space-y-2">
            <p class="text-xs uppercase text-muted-foreground">Size</p>
            <div class="flex gap-2">
              <Button
                v-for="size in sizes"
                :key="size"
                variant="outline"
                size="sm"
                class="rounded-full px-4"
              >
                {{ size }}
              </Button>
            </div>
          </div>
          <div class="grid gap-3 sm:grid-cols-2">
            <Button size="lg">
              Add to Cart
            </Button>
            <Button size="lg" variant="outline">
              Buy it now
            </Button>
          </div>
          <Tabs default-value="details">
            <TabsList class="grid w-full grid-cols-2">
              <TabsTrigger value="details">
                Details
              </TabsTrigger>
              <TabsTrigger value="shipping">
                Shipping
              </TabsTrigger>
            </TabsList>
            <TabsContent value="details">
              <p class="text-sm text-muted-foreground">
                Crafted with breathable cotton and reinforced seams for daily wear.
              </p>
            </TabsContent>
            <TabsContent value="shipping">
              <p class="text-sm text-muted-foreground">
                Free same-day delivery in Gauteng. 48-hour delivery nationwide.
              </p>
            </TabsContent>
          </Tabs>
        </div>
      </CardContent>
    </Card>
  </section>
</template>

