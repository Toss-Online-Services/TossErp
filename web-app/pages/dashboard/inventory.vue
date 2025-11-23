<template>
  <div class="space-y-6">
    <div class="flex items-center justify-between">
      <div>
        <h1 class="text-3xl font-bold">Inventory</h1>
        <p class="text-muted-foreground">Manage your stock and products</p>
      </div>
      <Button>
        <Icon name="lucide:plus" class="w-4 h-4 mr-2" />
        Add Product
      </Button>
    </div>
    
    <!-- Filters -->
    <Card>
      <CardContent class="pt-6">
        <div class="flex flex-wrap gap-4">
          <Input placeholder="Search products..." class="flex-1 min-w-[200px]" />
          <select class="flex h-10 rounded-md border border-input bg-background px-3 py-2 text-sm">
            <option>All Categories</option>
            <option>Food & Beverages</option>
            <option>Household</option>
            <option>Personal Care</option>
          </select>
          <select class="flex h-10 rounded-md border border-input bg-background px-3 py-2 text-sm">
            <option>All Stock Status</option>
            <option>In Stock</option>
            <option>Low Stock</option>
            <option>Out of Stock</option>
          </select>
        </div>
      </CardContent>
    </Card>
    
    <!-- Products Table -->
    <Card>
      <CardHeader>
        <CardTitle>Products</CardTitle>
      </CardHeader>
      <CardContent>
        <div class="overflow-x-auto">
          <table class="w-full">
            <thead>
              <tr class="border-b">
                <th class="text-left p-4 font-medium">Product</th>
                <th class="text-left p-4 font-medium">Category</th>
                <th class="text-left p-4 font-medium">Stock</th>
                <th class="text-left p-4 font-medium">Unit Price</th>
                <th class="text-left p-4 font-medium">Status</th>
                <th class="text-right p-4 font-medium">Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="product in products" :key="product.id" class="border-b hover:bg-muted/50">
                <td class="p-4">
                  <div class="font-medium">{{ product.name }}</div>
                  <div class="text-sm text-muted-foreground">{{ product.sku }}</div>
                </td>
                <td class="p-4">{{ product.category }}</td>
                <td class="p-4">
                  <div class="flex items-center space-x-2">
                    <span>{{ product.stock }} {{ product.unit }}</span>
                    <div class="w-24 h-2 bg-muted rounded-full overflow-hidden">
                      <div class="h-full rounded-full"
                           :class="getStockColor(product.stock, product.minStock)"
                           :style="{ width: `${Math.min((product.stock / product.maxStock) * 100, 100)}%` }">
                      </div>
                    </div>
                  </div>
                </td>
                <td class="p-4">R {{ formatCurrency(product.price) }}</td>
                <td class="p-4">
                  <span class="px-2 py-1 rounded-full text-xs font-medium"
                        :class="getStatusClass(product.stock, product.minStock)">
                    {{ getStockStatus(product.stock, product.minStock) }}
                  </span>
                </td>
                <td class="p-4 text-right">
                  <div class="flex items-center justify-end space-x-2">
                    <Button variant="ghost" size="sm">
                      <Icon name="lucide:edit" class="w-4 h-4" />
                    </Button>
                    <Button variant="ghost" size="sm">
                      <Icon name="lucide:trash" class="w-4 h-4" />
                    </Button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </CardContent>
    </Card>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'admin'
})

const products = ref([
  { id: 1, name: 'White Bread', sku: 'BRD-001', category: 'Food & Beverages', stock: 25, minStock: 30, maxStock: 100, unit: 'loaves', price: 12.50 },
  { id: 2, name: 'Cooking Oil 2L', sku: 'OIL-001', category: 'Food & Beverages', stock: 8, minStock: 10, maxStock: 50, unit: 'bottles', price: 45.00 },
  { id: 3, name: 'Maize Meal 5kg', sku: 'MZM-001', category: 'Food & Beverages', stock: 15, minStock: 20, maxStock: 80, unit: 'bags', price: 65.00 },
  { id: 4, name: 'Sugar 2kg', sku: 'SUG-001', category: 'Food & Beverages', stock: 0, minStock: 15, maxStock: 60, unit: 'bags', price: 35.00 },
  { id: 5, name: 'Milk 2L', sku: 'MLK-001', category: 'Food & Beverages', stock: 12, minStock: 20, maxStock: 50, unit: 'bottles', price: 28.00 }
])

const formatCurrency = (amount: number) => {
  return amount.toLocaleString('en-ZA', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
}

const getStockStatus = (stock: number, minStock: number) => {
  if (stock === 0) return 'Out of Stock'
  if (stock < minStock) return 'Low Stock'
  return 'In Stock'
}

const getStatusClass = (stock: number, minStock: number) => {
  if (stock === 0) return 'bg-destructive/10 text-destructive'
  if (stock < minStock) return 'bg-yellow-500/10 text-yellow-600'
  return 'bg-primary/10 text-primary'
}

const getStockColor = (stock: number, minStock: number) => {
  if (stock === 0) return 'bg-destructive'
  if (stock < minStock) return 'bg-yellow-500'
  return 'bg-primary'
}

useHead({
  title: 'Inventory - TOSS'
})
</script>


