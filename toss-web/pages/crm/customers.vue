<script setup lang="ts">
import { Button } from '~/components/ui/button'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '~/components/ui/card'
import { Input } from '~/components/ui/input'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '~/components/ui/table'
import { Badge } from '~/components/ui/badge'
import { Avatar, AvatarFallback, AvatarImage } from '~/components/ui/avatar'

definePageMeta({
  middleware: 'auth',
})

const searchQuery = ref('')

const customers = [
  { id: 1, name: 'Thabo Mbeki', email: 'thabo@email.com', phone: '+27 71 234 5678', totalSpent: 2500.00, visits: 12, status: 'active' },
  { id: 2, name: 'Nomsa Dlamini', email: 'nomsa@email.com', phone: '+27 82 345 6789', totalSpent: 1800.50, visits: 8, status: 'active' },
  { id: 3, name: 'Sipho Ndlovu', email: 'sipho@email.com', phone: '+27 73 456 7890', totalSpent: 950.00, visits: 5, status: 'active' },
  { id: 4, name: 'Lerato Khumalo', email: 'lerato@email.com', phone: '+27 84 567 8901', totalSpent: 3200.75, visits: 15, status: 'vip' },
  { id: 5, name: 'Bongani Mthembu', email: 'bongani@email.com', phone: '+27 76 678 9012', totalSpent: 450.00, visits: 2, status: 'new' },
]

const getStatusVariant = (status: string) => {
  switch (status) {
    case 'vip': return 'default'
    case 'active': return 'secondary'
    case 'new': return 'outline'
    default: return 'outline'
  }
}

const getStatusLabel = (status: string) => {
  return status.toUpperCase()
}

const getInitials = (name: string) => {
  return name.split(' ').map(n => n[0]).join('').toUpperCase()
}
</script>

<template>
  <div class="flex-1 space-y-4 p-4 md:p-8 pt-6">
    <div class="flex items-center justify-between">
      <h2 class="text-3xl font-bold tracking-tight">
        Customers
      </h2>
      <div class="flex items-center space-x-2">
        <Button>
          <Icon name="mdi:plus" class="mr-2 h-4 w-4" />
          Add Customer
        </Button>
      </div>
    </div>

    <!-- Stats Cards -->
    <div class="grid gap-4 md:grid-cols-4">
      <Card>
        <CardHeader class="flex flex-row items-center justify-between space-y-0 pb-2">
          <CardTitle class="text-sm font-medium">
            Total Customers
          </CardTitle>
          <Icon name="mdi:account-group" class="h-4 w-4 text-muted-foreground" />
        </CardHeader>
        <CardContent>
          <div class="text-2xl font-bold">{{ customers.length }}</div>
        </CardContent>
      </Card>

      <Card>
        <CardHeader class="flex flex-row items-center justify-between space-y-0 pb-2">
          <CardTitle class="text-sm font-medium">
            VIP Customers
          </CardTitle>
          <Icon name="mdi:star" class="h-4 w-4 text-yellow-600" />
        </CardHeader>
        <CardContent>
          <div class="text-2xl font-bold">{{ customers.filter(c => c.status === 'vip').length }}</div>
        </CardContent>
      </Card>

      <Card>
        <CardHeader class="flex flex-row items-center justify-between space-y-0 pb-2">
          <CardTitle class="text-sm font-medium">
            New This Month
          </CardTitle>
          <Icon name="mdi:account-plus" class="h-4 w-4 text-green-600" />
        </CardHeader>
        <CardContent>
          <div class="text-2xl font-bold">{{ customers.filter(c => c.status === 'new').length }}</div>
        </CardContent>
      </Card>

      <Card>
        <CardHeader class="flex flex-row items-center justify-between space-y-0 pb-2">
          <CardTitle class="text-sm font-medium">
            Total Revenue
          </CardTitle>
          <Icon name="mdi:currency-usd" class="h-4 w-4 text-muted-foreground" />
        </CardHeader>
        <CardContent>
          <div class="text-2xl font-bold">R {{ customers.reduce((sum, c) => sum + c.totalSpent, 0).toFixed(2) }}</div>
        </CardContent>
      </Card>
    </div>

    <!-- Customers Table -->
    <Card>
      <CardHeader>
        <div class="flex items-center justify-between">
          <div>
            <CardTitle>Customer Directory</CardTitle>
            <CardDescription>Manage your customer relationships</CardDescription>
          </div>
          <Input
            v-model="searchQuery"
            placeholder="Search customers..."
            class="max-w-sm"
          />
        </div>
      </CardHeader>
      <CardContent>
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Customer</TableHead>
              <TableHead>Contact</TableHead>
              <TableHead>Total Spent</TableHead>
              <TableHead>Visits</TableHead>
              <TableHead>Status</TableHead>
              <TableHead class="text-right">Actions</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            <TableRow v-for="customer in customers" :key="customer.id">
              <TableCell>
                <div class="flex items-center gap-3">
                  <Avatar>
                    <AvatarFallback>{{ getInitials(customer.name) }}</AvatarFallback>
                  </Avatar>
                  <div>
                    <p class="font-medium">{{ customer.name }}</p>
                    <p class="text-sm text-muted-foreground">{{ customer.email }}</p>
                  </div>
                </div>
              </TableCell>
              <TableCell>{{ customer.phone }}</TableCell>
              <TableCell>R {{ customer.totalSpent.toFixed(2) }}</TableCell>
              <TableCell>{{ customer.visits }}</TableCell>
              <TableCell>
                <Badge :variant="getStatusVariant(customer.status)">
                  {{ getStatusLabel(customer.status) }}
                </Badge>
              </TableCell>
              <TableCell class="text-right">
                <Button variant="ghost" size="sm">
                  <Icon name="mdi:eye" class="h-4 w-4" />
                </Button>
                <Button variant="ghost" size="sm">
                  <Icon name="mdi:pencil" class="h-4 w-4" />
                </Button>
              </TableCell>
            </TableRow>
          </TableBody>
        </Table>
      </CardContent>
    </Card>
  </div>
</template>
