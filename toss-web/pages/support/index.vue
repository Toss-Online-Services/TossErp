<script setup lang="ts">
definePageMeta({
  middleware: 'auth',
  layout: 'default'
})

const stats = ref([
  { label: 'Open Tickets', value: '8', icon: 'i-heroicons-ticket', color: 'warning' },
  { label: 'In Progress', value: '5', icon: 'i-heroicons-cog-6-tooth', color: 'primary' },
  { label: 'Resolved Today', value: '12', icon: 'i-heroicons-check-circle', color: 'success' },
  { label: 'Avg Response Time', value: '2.5h', icon: 'i-heroicons-clock', color: 'info' }
])

const recentTickets = ref([
  { id: 'TKT-001', title: 'Payment system not working', customer: 'John Doe', priority: 'high', status: 'open', created: '2 hours ago' },
  { id: 'TKT-002', title: 'Need help with order', customer: 'Jane Smith', priority: 'medium', status: 'in-progress', created: '5 hours ago' },
  { id: 'TKT-003', title: 'Product inquiry', customer: 'Mike Johnson', priority: 'low', status: 'resolved', created: 'Yesterday' }
])
</script>

<template>
  <div class="p-4 md:p-6">
    <div class="mb-6">
      <h1 class="text-2xl font-bold mb-2">Customer Support</h1>
      <p class="text-muted-foreground">Manage support tickets and customer inquiries</p>
    </div>

    <!-- Stats Grid -->
    <div class="grid grid-cols-2 md:grid-cols-4 gap-4 mb-6">
      <UCard v-for="stat in stats" :key="stat.label" class="hover:shadow-md transition-shadow">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-muted-foreground">{{ stat.label }}</p>
            <p class="text-2xl font-bold mt-1">{{ stat.value }}</p>
          </div>
          <div :class="`p-3 rounded-lg bg-${stat.color}/10`">
            <UIcon :name="stat.icon" class="w-6 h-6" :class="`text-${stat.color}`" />
          </div>
        </div>
      </UCard>
    </div>

    <!-- Quick Actions -->
    <div class="mb-6 flex gap-3">
      <UButton size="lg">
        <UIcon name="i-heroicons-plus" class="mr-2" />
        New Ticket
      </UButton>
      <UButton to="/support/tickets" size="lg" variant="outline">
        <UIcon name="i-heroicons-queue-list" class="mr-2" />
        All Tickets
      </UButton>
    </div>

    <!-- Recent Tickets -->
    <UCard>
      <template #header>
        <div class="flex items-center justify-between">
          <h2 class="text-lg font-semibold">Recent Tickets</h2>
          <UButton to="/support/tickets" size="sm" variant="ghost">View All</UButton>
        </div>
      </template>

      <div class="space-y-3">
        <div v-for="ticket in recentTickets" :key="ticket.id" class="p-4 border rounded-lg hover:bg-accent/50 transition-colors cursor-pointer" @click="navigateTo(`/support/tickets/${ticket.id}`)">
          <div class="flex items-start justify-between mb-2">
            <div class="flex-1">
              <div class="flex items-center gap-2 mb-1">
                <h3 class="font-semibold">{{ ticket.title }}</h3>
                <UBadge :color="ticket.priority === 'high' ? 'error' : ticket.priority === 'medium' ? 'warning' : 'gray'" size="xs">
                  {{ ticket.priority }}
                </UBadge>
              </div>
              <p class="text-sm text-muted-foreground">{{ ticket.id }} • {{ ticket.customer }}</p>
            </div>
            <UBadge :color="ticket.status === 'open' ? 'warning' : ticket.status === 'in-progress' ? 'primary' : 'success'">
              {{ ticket.status }}
            </UBadge>
          </div>
          <p class="text-xs text-muted-foreground">Created {{ ticket.created }}</p>
        </div>
      </div>
    </UCard>
  </div>
</template>
