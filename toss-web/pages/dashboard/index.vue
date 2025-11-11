<script setup lang="ts">
import { Button } from '~/components/ui/button'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '~/components/ui/card'
import { Tabs, TabsContent, TabsList, TabsTrigger } from '~/components/ui/tabs'
import { Avatar, AvatarFallback, AvatarImage } from '~/components/ui/avatar'

definePageMeta({
  middleware: 'auth',
})

const stats = [
  {
    title: 'Total Revenue',
    value: 'R 45,231.89',
    change: '+20.1% from last month',
    icon: 'mdi:currency-usd',
  },
  {
    title: 'Subscriptions',
    value: '+2,350',
    change: '+180.1% from last month',
    icon: 'mdi:account-multiple',
  },
  {
    title: 'Sales',
    value: '+12,234',
    change: '+19% from last month',
    icon: 'mdi:shopping',
  },
  {
    title: 'Active Now',
    value: '+573',
    change: '+201 since last hour',
    icon: 'mdi:chart-line',
  },
]

const recentSales = [
  {
    name: 'Olivia Martin',
    email: 'olivia.martin@email.com',
    amount: '+R 1,999.00',
    avatar: '/avatars/01.png',
  },
  {
    name: 'Jackson Lee',
    email: 'jackson.lee@email.com',
    amount: '+R 39.00',
    avatar: '/avatars/02.png',
  },
  {
    name: 'Isabella Nguyen',
    email: 'isabella.nguyen@email.com',
    amount: '+R 299.00',
    avatar: '/avatars/03.png',
  },
  {
    name: 'William Kim',
    email: 'will@email.com',
    amount: '+R 99.00',
    avatar: '/avatars/04.png',
  },
  {
    name: 'Sofia Davis',
    email: 'sofia.davis@email.com',
    amount: '+R 39.00',
    avatar: '/avatars/05.png',
  },
]
</script>

<template>
  <div class="flex-1 space-y-4 p-4 md:p-8 pt-6">
    <div class="flex items-center justify-between space-y-2">
      <h2 class="text-3xl font-bold tracking-tight">
        Dashboard
      </h2>
      <div class="flex items-center space-x-2">
        <Button>Download</Button>
      </div>
    </div>
    
    <Tabs default-value="overview" class="space-y-4">
      <TabsList>
        <TabsTrigger value="overview">
          Overview
        </TabsTrigger>
        <TabsTrigger value="analytics">
          Analytics
        </TabsTrigger>
        <TabsTrigger value="reports">
          Reports
        </TabsTrigger>
        <TabsTrigger value="notifications">
          Notifications
        </TabsTrigger>
      </TabsList>
      
      <TabsContent value="overview" class="space-y-4">
        <!-- Stats Grid -->
        <div class="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
          <Card v-for="stat in stats" :key="stat.title">
            <CardHeader class="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle class="text-sm font-medium">
                {{ stat.title }}
              </CardTitle>
              <Icon :name="stat.icon" class="h-4 w-4 text-muted-foreground" />
            </CardHeader>
            <CardContent>
              <div class="text-2xl font-bold">{{ stat.value }}</div>
              <p class="text-xs text-muted-foreground">
                {{ stat.change }}
              </p>
            </CardContent>
          </Card>
        </div>

        <!-- Overview and Recent Sales -->
        <div class="grid gap-4 md:grid-cols-2 lg:grid-cols-7">
          <!-- Overview Chart -->
          <Card class="col-span-4">
            <CardHeader>
              <CardTitle>Overview</CardTitle>
            </CardHeader>
            <CardContent class="pl-2">
              <div class="h-[350px] flex items-center justify-center text-muted-foreground">
                Chart placeholder - Add your chart component here
              </div>
            </CardContent>
          </Card>

          <!-- Recent Sales -->
          <Card class="col-span-3">
            <CardHeader>
              <CardTitle>Recent Sales</CardTitle>
              <CardDescription>
                You made 265 sales this month.
              </CardDescription>
            </CardHeader>
            <CardContent>
              <div class="space-y-8">
                <div
                  v-for="sale in recentSales"
                  :key="sale.email"
                  class="flex items-center"
                >
                  <Avatar class="h-9 w-9">
                    <AvatarImage :src="sale.avatar" :alt="sale.name" />
                    <AvatarFallback>{{ sale.name.substring(0, 2) }}</AvatarFallback>
                  </Avatar>
                  <div class="ml-4 space-y-1">
                    <p class="text-sm font-medium leading-none">
                      {{ sale.name }}
                    </p>
                    <p class="text-sm text-muted-foreground">
                      {{ sale.email }}
                    </p>
                  </div>
                  <div class="ml-auto font-medium">
                    {{ sale.amount }}
                  </div>
                </div>
              </div>
            </CardContent>
          </Card>
        </div>
      </TabsContent>

      <TabsContent value="analytics" class="space-y-4">
        <Card>
          <CardHeader>
            <CardTitle>Analytics</CardTitle>
            <CardDescription>
              Detailed analytics and insights for your business
            </CardDescription>
          </CardHeader>
          <CardContent>
            <div class="h-[400px] flex items-center justify-center text-muted-foreground">
              Analytics content goes here
            </div>
          </CardContent>
        </Card>
      </TabsContent>

      <TabsContent value="reports" class="space-y-4">
        <Card>
          <CardHeader>
            <CardTitle>Reports</CardTitle>
            <CardDescription>
              Generate and view business reports
            </CardDescription>
          </CardHeader>
          <CardContent>
            <div class="h-[400px] flex items-center justify-center text-muted-foreground">
              Reports content goes here
            </div>
          </CardContent>
        </Card>
      </TabsContent>

      <TabsContent value="notifications" class="space-y-4">
        <Card>
          <CardHeader>
            <CardTitle>Notifications</CardTitle>
            <CardDescription>
              Manage your notifications and alerts
            </CardDescription>
          </CardHeader>
          <CardContent>
            <div class="h-[400px] flex items-center justify-center text-muted-foreground">
              Notifications content goes here
            </div>
          </CardContent>
        </Card>
      </TabsContent>
    </Tabs>
  </div>
</template>
