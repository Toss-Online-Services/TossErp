<script setup lang="ts">
import { Badge } from '~/components/ui/badge'
import { Button } from '~/components/ui/button'
import { Card, CardContent } from '~/components/ui/card'
import { Plus } from 'lucide-vue-next'

interface Product {
  id: number
  name: string
  sku: string
  barcode: string | null
  basePrice: number
  imageUrl: string | null
  categoryName: string
  availableStock: number
  isTaxable: boolean
}

defineProps<{
  products: Product[]
  loading?: boolean
}>()

const emit = defineEmits<{
  selectProduct: [product: Product]
}>()

const formatPrice = (price: number) => {
  return `R ${price.toFixed(2)}`
}
</script>

<template>
  <div class="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 xl:grid-cols-5 gap-3">
    <Card
      v-for="product in products"
      :key="product.id"
      class="cursor-pointer hover:border-primary transition-colors"
      @click="emit('selectProduct', product)"
    >
      <CardContent class="p-3 space-y-2">
        <div class="aspect-square bg-muted rounded-md flex items-center justify-center mb-2">
          <img
            v-if="product.imageUrl"
            :src="product.imageUrl"
            :alt="product.name"
            class="w-full h-full object-cover rounded-md"
          />
          <span v-else class="text-4xl text-muted-foreground">📦</span>
        </div>
        
        <div class="space-y-1">
          <h4 class="font-medium text-sm line-clamp-2" :title="product.name">
            {{ product.name }}
          </h4>
          <p class="text-xs text-muted-foreground">{{ product.sku }}</p>
        </div>

        <div class="flex items-center justify-between">
          <span class="text-base font-semibold text-primary">
            {{ formatPrice(product.basePrice) }}
          </span>
          <Badge v-if="product.availableStock < 10" variant="destructive" class="text-xs">
            Low: {{ product.availableStock }}
          </Badge>
          <Badge v-else variant="secondary" class="text-xs">
            {{ product.availableStock }}
          </Badge>
        </div>

        <Button size="sm" class="w-full" @click.stop="emit('selectProduct', product)">
          <Plus class="h-4 w-4 mr-1" />
          Add
        </Button>
      </CardContent>
    </Card>

    <div v-if="loading" class="col-span-full flex items-center justify-center py-12">
      <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-primary"></div>
    </div>

    <div v-else-if="products.length === 0" class="col-span-full flex flex-col items-center justify-center py-12 text-muted-foreground">
      <svg xmlns="http://www.w3.org/2000/svg" width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="mb-4 opacity-50">
        <circle cx="11" cy="11" r="8"/>
        <path d="m21 21-4.3-4.3"/>
      </svg>
      <p>No products found</p>
      <p class="text-sm">Try a different search term</p>
    </div>
  </div>
</template>
