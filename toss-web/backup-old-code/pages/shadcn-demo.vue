<script setup lang="ts">
import { Button } from '~/components/ui/button'
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from '~/components/ui/card'

const stats = [
  { title: 'Total Sales', value: 'R45,231.89', change: '+20.1%', trend: 'up' },
  { title: 'Orders', value: '1,234', change: '+10.5%', trend: 'up' },
  { title: 'Customers', value: '2,350', change: '+15.3%', trend: 'up' },
  { title: 'Revenue', value: 'R89,450', change: '+8.2%', trend: 'up' },
]
</script>

<template>
  <div class="min-h-screen bg-background p-6">
    <div class="mx-auto max-w-7xl space-y-6">
      <!-- Header -->
      <div class="flex items-center justify-between">
        <div>
          <h1 class="text-3xl font-bold tracking-tight">
            Dashboard
          </h1>
          <p class="text-muted-foreground">
            Welcome to TOSS ERP III with shadcn-vue
          </p>
        </div>
        <Button>
          <Icon name="mdi:plus" class="mr-2 h-4 w-4" />
          Add New
        </Button>
      </div>

      <!-- Stats Grid -->
      <div class="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
        <Card v-for="stat in stats" :key="stat.title">
          <CardHeader class="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle class="text-sm font-medium">
              {{ stat.title }}
            </CardTitle>
            <Icon 
              :name="stat.trend === 'up' ? 'mdi:trending-up' : 'mdi:trending-down'" 
              class="h-4 w-4 text-muted-foreground"
            />
          </CardHeader>
          <CardContent>
            <div class="text-2xl font-bold">
              {{ stat.value }}
            </div>
            <p class="text-xs text-muted-foreground">
              <span :class="stat.trend === 'up' ? 'text-green-600' : 'text-red-600'">
                {{ stat.change }}
              </span>
              from last month
            </p>
          </CardContent>
        </Card>
      </div>

      <!-- Main Content -->
      <div class="grid gap-4 md:grid-cols-2 lg:grid-cols-7">
        <!-- Recent Activity -->
        <Card class="col-span-4">
          <CardHeader>
            <CardTitle>Recent Activity</CardTitle>
            <CardDescription>
              Your latest transactions and updates
            </CardDescription>
          </CardHeader>
          <CardContent>
            <div class="space-y-4">
              <div v-for="i in 5" :key="i" class="flex items-center">
                <div class="h-9 w-9 rounded-full bg-primary/10 flex items-center justify-center">
                  <Icon name="mdi:cash-register" class="h-4 w-4 text-primary" />
                </div>
                <div class="ml-4 space-y-1">
                  <p class="text-sm font-medium leading-none">
                    Sale #{{ 1000 + i }}
                  </p>
                  <p class="text-sm text-muted-foreground">
                    R{{ (Math.random() * 1000).toFixed(2) }}
                  </p>
                </div>
                <div class="ml-auto font-medium text-sm text-muted-foreground">
                  {{ i }}h ago
                </div>
              </div>
            </div>
          </CardContent>
          <CardFooter>
            <Button variant="outline" class="w-full">
              View All Transactions
            </Button>
          </CardFooter>
        </Card>

        <!-- Quick Actions -->
        <Card class="col-span-3">
          <CardHeader>
            <CardTitle>Quick Actions</CardTitle>
            <CardDescription>
              Frequently used features
            </CardDescription>
          </CardHeader>
          <CardContent class="space-y-2">
            <Button variant="outline" class="w-full justify-start">
              <Icon name="mdi:cash-register" class="mr-2 h-4 w-4" />
              New Sale
            </Button>
            <Button variant="outline" class="w-full justify-start">
              <Icon name="mdi:package-variant" class="mr-2 h-4 w-4" />
              Add Stock
            </Button>
            <Button variant="outline" class="w-full justify-start">
              <Icon name="mdi:account-plus" class="mr-2 h-4 w-4" />
              New Customer
            </Button>
            <Button variant="outline" class="w-full justify-start">
              <Icon name="mdi:chart-line" class="mr-2 h-4 w-4" />
              View Reports
            </Button>
          </CardContent>
          <CardFooter>
            <Button class="w-full">
              <Icon name="mdi:robot" class="mr-2 h-4 w-4" />
              Ask AI Copilot
            </Button>
          </CardFooter>
        </Card>
      </div>

      <!-- Demo Section -->
      <Card>
        <CardHeader>
          <CardTitle>shadcn-vue Integration Complete ✓</CardTitle>
          <CardDescription>
            All components are using shadcn-vue. This is a demonstration page.
          </CardDescription>
        </CardHeader>
        <CardContent>
          <div class="flex flex-wrap gap-2">
            <Button>Default</Button>
            <Button variant="secondary">Secondary</Button>
            <Button variant="destructive">Destructive</Button>
            <Button variant="outline">Outline</Button>
            <Button variant="ghost">Ghost</Button>
            <Button variant="link">Link</Button>
          </div>
        </CardContent>
      </Card>
    </div>
  </div>
</template>
