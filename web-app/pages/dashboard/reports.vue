<template>
  <div class="space-y-6">
    <div>
      <h1 class="text-3xl font-bold">Reports</h1>
      <p class="text-muted-foreground">Business insights and analytics</p>
    </div>
    
    <!-- Report Cards -->
    <div class="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
      <Card v-for="report in reports" :key="report.id" class="hover:shadow-lg transition-shadow cursor-pointer">
        <CardHeader>
          <div class="flex items-center justify-between">
            <CardTitle class="text-lg">{{ report.name }}</CardTitle>
            <Icon :name="report.icon" class="w-6 h-6 text-primary" />
          </div>
        </CardHeader>
        <CardContent>
          <p class="text-sm text-muted-foreground mb-4">{{ report.description }}</p>
          <Button variant="outline" class="w-full">Generate Report</Button>
        </CardContent>
      </Card>
    </div>
    
    <!-- Quick Stats -->
    <Card>
      <CardHeader>
        <CardTitle>Quick Statistics</CardTitle>
      </CardHeader>
      <CardContent>
        <div class="grid gap-4 md:grid-cols-4">
          <div>
            <p class="text-sm text-muted-foreground">Best Selling Product</p>
            <p class="text-xl font-bold mt-1">White Bread</p>
            <p class="text-sm text-muted-foreground">450 units this month</p>
          </div>
          <div>
            <p class="text-sm text-muted-foreground">Peak Sales Day</p>
            <p class="text-xl font-bold mt-1">Saturday</p>
            <p class="text-sm text-muted-foreground">R 8,500 average</p>
          </div>
          <div>
            <p class="text-sm text-muted-foreground">Top Customer</p>
            <p class="text-xl font-bold mt-1">John Doe</p>
            <p class="text-sm text-muted-foreground">R 4,500 total</p>
          </div>
          <div>
            <p class="text-sm text-muted-foreground">Growth Rate</p>
            <p class="text-xl font-bold mt-1 text-primary">+15.2%</p>
            <p class="text-sm text-muted-foreground">vs last month</p>
          </div>
        </div>
      </CardContent>
    </Card>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'

definePageMeta({
  layout: 'admin'
})

const reports = ref([
  { id: 1, name: 'Sales Report', icon: 'lucide:trending-up', description: 'Detailed sales analysis and trends' },
  { id: 2, name: 'Inventory Report', icon: 'lucide:package', description: 'Stock levels and movement analysis' },
  { id: 3, name: 'Financial Report', icon: 'lucide:wallet', description: 'Profit & loss and cash flow statements' },
  { id: 4, name: 'Customer Report', icon: 'lucide:users', description: 'Customer behavior and purchase patterns' },
  { id: 5, name: 'Supplier Report', icon: 'lucide:truck', description: 'Supplier performance and order history' },
  { id: 6, name: 'Tax Report', icon: 'lucide:file-text', description: 'Tax-ready financial summaries' }
])

useHead({
  title: 'Reports - TOSS'
})
</script>

