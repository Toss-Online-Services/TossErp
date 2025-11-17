<script setup lang="ts">
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '~/components/ui/card'
import { Button } from '~/components/ui/button'
import { Badge } from '~/components/ui/badge'
import { Tabs, TabsContent, TabsList, TabsTrigger } from '~/components/ui/tabs'

const items = [
  {
    title: 'Elegant Ceramic Vase',
    price: '$149.90',
    description: 'Handcrafted ceramic vase for modern spaces.',
  },
  {
    title: 'Wall Art',
    price: '$199.00',
    description: 'Curated art prints for large feature walls.',
  },
  {
    title: 'Table Lamp',
    price: '$129.00',
    description: 'Soft lighting for evening mood.',
  },
]
</script>

<template>
  <section class="space-y-4">
    <div>
      <p class="text-sm text-muted-foreground">interactive-product-preview-01</p>
      <h2 class="text-xl font-semibold">Interactive room product preview</h2>
    </div>
    <Card class="border-border/70">
      <CardHeader class="space-y-2">
        <Badge variant="secondary" class="w-fit">
          Room previewer
        </Badge>
        <CardTitle>Visualize Your Space</CardTitle>
        <CardDescription>
          Easily preview furniture, decor, and more in your space ensuring everything fits perfectly.
        </CardDescription>
      </CardHeader>
      <CardContent class="grid gap-6 lg:grid-cols-[1.3fr,1fr]">
        <div class="space-y-4">
          <div class="aspect-video overflow-hidden rounded-2xl bg-gradient-to-br from-slate-900 to-slate-700">
            <div class="flex h-full flex-col justify-end bg-gradient-to-t from-black/70 to-transparent p-6 text-white">
              <p class="text-sm uppercase tracking-wide">Room preview</p>
              <p class="text-2xl font-semibold">Smart living room set</p>
              <p class="text-sm text-white/70">
                Drag assets into your space and switch materials instantly.
              </p>
            </div>
          </div>
          <Tabs default-value="vase">
            <TabsList class="grid w-full grid-cols-3">
              <TabsTrigger value="vase">
                Vase
              </TabsTrigger>
              <TabsTrigger value="art">
                Wall Art
              </TabsTrigger>
              <TabsTrigger value="lamp">
                Table Lamp
              </TabsTrigger>
            </TabsList>
            <TabsContent value="vase">
              <p class="text-sm text-muted-foreground">
                Elegant Ceramic Vase · Add a touch of sophistication to your home.
              </p>
            </TabsContent>
            <TabsContent value="art">
              <p class="text-sm text-muted-foreground">
                Curated art prints designed for creative studio walls.
              </p>
            </TabsContent>
            <TabsContent value="lamp">
              <p class="text-sm text-muted-foreground">
                Warm-toned lamp for bedside or living room ambiance.
              </p>
            </TabsContent>
          </Tabs>
        </div>
        <div class="space-y-4">
          <div
            v-for="item in items"
            :key="item.title"
            class="rounded-xl border border-border/80 p-4"
          >
            <div class="flex items-center justify-between">
              <div>
                <p class="font-semibold">{{ item.title }}</p>
                <p class="text-xs text-muted-foreground">{{ item.description }}</p>
              </div>
              <p class="font-semibold">{{ item.price }}</p>
            </div>
            <Button variant="outline" size="sm" class="mt-3 w-full">
              Add to cart
            </Button>
          </div>
        </div>
      </CardContent>
    </Card>
  </section>
</template>

