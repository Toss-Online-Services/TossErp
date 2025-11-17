<script setup lang="ts">
import { Button } from '~/components/ui/button'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '~/components/ui/card'
import { Input } from '~/components/ui/input'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '~/components/ui/table'
import { Badge } from '~/components/ui/badge'
import { Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle, DialogTrigger } from '~/components/ui/dialog'

definePageMeta({
  middleware: 'auth',
})

const searchQuery = ref('')

const products = [
  { id: 1, name: 'Bread - White Loaf', sku: 'BRD001', stock: 25, reorderLevel: 10, price: 12.50, status: 'in-stock' },
  { id: 2, name: 'Milk - 2L Full Cream', sku: 'MLK001', stock: 8, reorderLevel: 15, price: 25.99, status: 'low-stock' },
  { id: 3, name: 'Sugar - 2.5kg', sku: 'SGR001', stock: 0, reorderLevel: 5, price: 45.00, status: 'out-of-stock' },
  { id: 4, name: 'Cooking Oil - 750ml', sku: 'OIL001', stock: 18, reorderLevel: 10, price: 32.50, status: 'in-stock' },
  { id: 5, name: 'Maize Meal - 5kg', sku: 'MAZ001', stock: 12, reorderLevel: 8, price: 55.00, status: 'in-stock' },
]

const getStatusVariant = (status: string) => {
  switch (status) {
    case 'in-stock': return 'default'
    case 'low-stock': return 'secondary'
    case 'out-of-stock': return 'destructive'
    default: return 'default'
  }
}

const getStatusLabel = (status: string) => {
  return status.split('-').map(word => word.charAt(0).toUpperCase() + word.slice(1)).join(' ')
}
</script>

<template>
  <div class="flex-1 space-y-4 p-4 md:p-8 pt-6">
    <div class="flex items-center justify-between">
      <h2 class="text-3xl font-bold tracking-tight">
        Inventory
      </h2>
      <div class="flex items-center space-x-2">
        <Dialog>
          <DialogTrigger as-child>
            <Button>
              <Icon name="mdi:plus" class="mr-2 h-4 w-4" />
              Add Product
            </Button>
          </DialogTrigger>
          <DialogContent>
            <DialogHeader>
              <DialogTitle>Add New Product</DialogTitle>
              <DialogDescription>
                Add a new product to your inventory
              </DialogDescription>
            </DialogHeader>
            <div class="space-y-4 py-4">
              <p class="text-sm text-muted-foreground">Product form will go here</p>
            </div>
            <DialogFooter>
              <Button variant="outline">Cancel</Button>
              <Button>Save Product</Button>
            </DialogFooter>
          </DialogContent>
        </Dialog>
      </div>
    </div>

    <!-- Stats Cards -->
    <div class="grid gap-4 md:grid-cols-4">
      <Card>
        <CardHeader class="flex flex-row items-center justify-between space-y-0 pb-2">
          <CardTitle class="text-sm font-medium">
            Total Products
          </CardTitle>
          <Icon name="mdi:package-variant" class="h-4 w-4 text-muted-foreground" />
        </CardHeader>
        <CardContent>
          <div class="text-2xl font-bold">{{ products.length }}</div>
        </CardContent>
      </Card>

      <Card>
        <CardHeader class="flex flex-row items-center justify-between space-y-0 pb-2">
          <CardTitle class="text-sm font-medium">
            In Stock
          </CardTitle>
          <Icon name="mdi:check-circle" class="h-4 w-4 text-green-600" />
        </CardHeader>
        <CardContent>
          <div class="text-2xl font-bold">{{ products.filter(p => p.status === 'in-stock').length }}</div>
        </CardContent>
      </Card>

      <Card>
        <CardHeader class="flex flex-row items-center justify-between space-y-0 pb-2">
          <CardTitle class="text-sm font-medium">
            Low Stock
          </CardTitle>
          <Icon name="mdi:alert-circle" class="h-4 w-4 text-orange-600" />
        </CardHeader>
        <CardContent>
          <div class="text-2xl font-bold">{{ products.filter(p => p.status === 'low-stock').length }}</div>
        </CardContent>
      </Card>

      <Card>
        <CardHeader class="flex flex-row items-center justify-between space-y-0 pb-2">
          <CardTitle class="text-sm font-medium">
            Out of Stock
          </CardTitle>
          <Icon name="mdi:close-circle" class="h-4 w-4 text-red-600" />
        </CardHeader>
        <CardContent>
          <div class="text-2xl font-bold">{{ products.filter(p => p.status === 'out-of-stock').length }}</div>
        </CardContent>
      </Card>
    </div>

    <!-- Products Table -->
    <Card>
      <CardHeader>
        <div class="flex items-center justify-between">
          <div>
            <CardTitle>Products</CardTitle>
            <CardDescription>Manage your inventory and stock levels</CardDescription>
          </div>
          <Input
            v-model="searchQuery"
            placeholder="Search products..."
            class="max-w-sm"
          />
        </div>
      </CardHeader>
      <CardContent>
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Product</TableHead>
              <TableHead>SKU</TableHead>
              <TableHead>Stock</TableHead>
              <TableHead>Reorder Level</TableHead>
              <TableHead>Price</TableHead>
              <TableHead>Status</TableHead>
              <TableHead class="text-right">Actions</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            <TableRow v-for="product in products" :key="product.id">
              <TableCell class="font-medium">{{ product.name }}</TableCell>
              <TableCell>{{ product.sku }}</TableCell>
              <TableCell>{{ product.stock }}</TableCell>
              <TableCell>{{ product.reorderLevel }}</TableCell>
              <TableCell>R {{ product.price.toFixed(2) }}</TableCell>
              <TableCell>
                <Badge :variant="getStatusVariant(product.status)">
                  {{ getStatusLabel(product.status) }}
                </Badge>
              </TableCell>
              <TableCell class="text-right">
                <Button variant="ghost" size="sm">
                  <Icon name="mdi:pencil" class="h-4 w-4" />
                </Button>
                <Button variant="ghost" size="sm">
                  <Icon name="mdi:delete" class="h-4 w-4" />
                </Button>
              </TableCell>
            </TableRow>
          </TableBody>
        </Table>
      </CardContent>
    </Card>
  </div>
</template>
