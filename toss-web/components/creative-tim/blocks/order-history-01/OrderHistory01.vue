<script setup lang="ts">
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '~/components/ui/card'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '~/components/ui/table'
import { Button } from '~/components/ui/button'

type Order = {
  id: string
  date: string
  amount: string
  status: 'Delivered' | 'Processing' | 'Refunded'
  product: string
}

const props = withDefaults(
  defineProps<{
    orders?: Order[]
  }>(),
  {
    orders: () => [
      {
        id: '1234',
        date: 'Apr 6, 2024',
        amount: '$2,570',
        status: 'Delivered',
        product: 'Premium Suit',
      },
      {
        id: '1235',
        date: 'Apr 6, 2024',
        amount: '$790',
        status: 'Delivered',
        product: 'Linen Suit',
      },
      {
        id: '1236',
        date: 'Apr 6, 2024',
        amount: '$990',
        status: 'Processing',
        product: 'Tweed Suit',
      },
    ],
  }
)
</script>

<template>
  <section class="space-y-4">
    <div>
      <p class="text-sm text-muted-foreground">order-history-01</p>
      <h2 class="text-xl font-semibold">Order history table</h2>
    </div>
    <Card class="border-border/70">
      <CardHeader class="flex flex-col justify-between gap-4 sm:flex-row sm:items-center">
        <div>
          <CardTitle>Order History</CardTitle>
          <CardDescription>See your recent orders, download invoices.</CardDescription>
        </div>
        <Button variant="outline" size="sm">
          Download statements
        </Button>
      </CardHeader>
      <CardContent class="overflow-x-auto">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Order ID</TableHead>
              <TableHead>Date</TableHead>
              <TableHead>Amount</TableHead>
              <TableHead>Status</TableHead>
              <TableHead>Details</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            <TableRow v-for="order in props.orders" :key="order.id">
              <TableCell class="font-semibold">
                #{{ order.id }}
              </TableCell>
              <TableCell>{{ order.date }}</TableCell>
              <TableCell>{{ order.amount }}</TableCell>
              <TableCell>
                <span
                  class="rounded-full px-3 py-1 text-xs font-semibold"
                  :class="order.status === 'Delivered' ? 'bg-emerald-100 text-emerald-700' : 'bg-amber-100 text-amber-700'"
                >
                  {{ order.status }}
                </span>
              </TableCell>
              <TableCell>
                <Button variant="ghost" size="sm">
                  View
                </Button>
              </TableCell>
            </TableRow>
          </TableBody>
        </Table>
      </CardContent>
    </Card>
  </section>
</template>

