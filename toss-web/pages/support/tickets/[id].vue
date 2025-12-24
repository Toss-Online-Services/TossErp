<script setup lang="ts">
definePageMeta({
  middleware: 'auth',
  layout: 'default'
})

const route = useRoute()
const ticketId = route.params.id

const ticket = ref({
  id: ticketId,
  title: 'Payment system not working',
  customer: 'John Doe',
  priority: 'high',
  status: 'open',
  created: '2025-12-23 14:30',
  description: 'The payment system is showing an error when I try to complete my purchase. I have tried multiple times but it keeps failing.',
  responses: [
    { user: 'Support Agent', message: 'Thank you for reporting this. We are investigating the issue.', time: '2025-12-23 14:45' },
    { user: 'John Doe', message: 'Any update on this? I really need to complete my order.', time: '2025-12-23 15:30' }
  ]
})
</script>

<template>
  <div class="p-4 md:p-6">
    <div class="mb-6">
      <UButton to="/support/tickets" variant="ghost" size="sm" class="mb-4">
        <UIcon name="i-heroicons-arrow-left" class="mr-2" />
        Back to Tickets
      </UButton>
      <div class="flex items-start justify-between">
        <div>
          <h1 class="text-2xl font-bold mb-2">{{ ticket.title }}</h1>
          <p class="text-muted-foreground">{{ ticket.id }} • Created {{ new Date(ticket.created).toLocaleString() }}</p>
        </div>
        <div class="flex gap-2">
          <UBadge :color="ticket.priority === 'high' ? 'error' : ticket.priority === 'medium' ? 'warning' : 'gray'" size="lg">
            {{ ticket.priority }} priority
          </UBadge>
          <UBadge :color="ticket.status === 'open' ? 'warning' : 'primary'" size="lg">
            {{ ticket.status }}
          </UBadge>
        </div>
      </div>
    </div>

    <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
      <!-- Ticket Details -->
      <div class="md:col-span-2 space-y-6">
        <!-- Description -->
        <UCard>
          <template #header>
            <h2 class="text-lg font-semibold">Description</h2>
          </template>
          <p class="text-muted-foreground">{{ ticket.description }}</p>
        </UCard>

        <!-- Responses -->
        <UCard>
          <template #header>
            <h2 class="text-lg font-semibold">Conversation</h2>
          </template>
          <div class="space-y-4">
            <div v-for="(response, index) in ticket.responses" :key="index" class="p-4 border-l-4" :class="response.user === 'Support Agent' ? 'border-l-primary bg-primary/5' : 'border-l-gray-300'">
              <div class="flex items-center justify-between mb-2">
                <span class="font-semibold">{{ response.user }}</span>
                <span class="text-xs text-muted-foreground">{{ new Date(response.time).toLocaleString() }}</span>
              </div>
              <p class="text-sm">{{ response.message }}</p>
            </div>
          </div>

          <!-- Reply Form -->
          <div class="mt-4 pt-4 border-t">
            <UTextarea placeholder="Type your response..." rows="4" class="mb-3" />
            <UButton>
              <UIcon name="i-heroicons-paper-airplane" class="mr-2" />
              Send Response
            </UButton>
          </div>
        </UCard>
      </div>

      <!-- Sidebar -->
      <div class="space-y-6">
        <!-- Customer Info -->
        <UCard>
          <template #header>
            <h2 class="text-lg font-semibold">Customer</h2>
          </template>
          <div class="space-y-2">
            <p class="font-medium">{{ ticket.customer }}</p>
            <UButton size="sm" variant="outline" block>
              <UIcon name="i-heroicons-user" class="mr-2" />
              View Profile
            </UButton>
          </div>
        </UCard>

        <!-- Actions -->
        <UCard>
          <template #header>
            <h2 class="text-lg font-semibold">Actions</h2>
          </template>
          <div class="space-y-2">
            <UButton v-if="ticket.status === 'open'" block variant="outline">
              <UIcon name="i-heroicons-play" class="mr-2" />
              Start Working
            </UButton>
            <UButton v-if="ticket.status !== 'resolved'" block variant="outline" color="green">
              <UIcon name="i-heroicons-check" class="mr-2" />
              Mark as Resolved
            </UButton>
            <UButton block variant="outline" color="red">
              <UIcon name="i-heroicons-x-mark" class="mr-2" />
              Close Ticket
            </UButton>
          </div>
        </UCard>
      </div>
    </div>
  </div>
</template>
