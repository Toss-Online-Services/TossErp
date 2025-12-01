<script setup lang="ts">
import { ref, onMounted } from 'vue'
import Card from '@/components/ui/Card.vue'
import CardHeader from '@/components/ui/CardHeader.vue'
import CardTitle from '@/components/ui/CardTitle.vue'
import CardContent from '@/components/ui/CardContent.vue'
import Breadcrumbs from '@/components/ui/Breadcrumbs.vue'
import Button from '@/components/ui/Button.vue'
import { MessageSquare, Plus, Search, Filter, AlertCircle, CheckCircle2, Clock, XCircle } from 'lucide-vue-next'
import { useSupportApi } from '@/composables/useSupportApi'

const { getTickets, isLoading } = useSupportApi()

const tickets = ref<any[]>([])
const searchQuery = ref('')
const statusFilter = ref<string | null>(null)
const currentPage = ref(1)
const totalCount = ref(0)

const loadTickets = async () => {
  try {
    const result = await getTickets({
      status: statusFilter.value as any,
      searchTerm: searchQuery.value || undefined,
      pageNumber: currentPage.value,
      pageSize: 20
    })
    tickets.value = result.items
    totalCount.value = result.totalCount
  } catch (error) {
    console.error('Failed to load tickets:', error)
  }
}

const getStatusIcon = (status: string) => {
  switch (status) {
    case 'Resolved':
    case 'Closed':
      return CheckCircle2
    case 'InProgress':
      return Clock
    case 'Open':
      return AlertCircle
    default:
      return AlertCircle
  }
}

const getStatusColor = (status: string) => {
  switch (status) {
    case 'Resolved':
    case 'Closed':
      return 'text-emerald-600'
    case 'InProgress':
      return 'text-blue-600'
    case 'Open':
      return 'text-amber-600'
    default:
      return 'text-gray-600'
  }
}

onMounted(() => {
  loadTickets()
})
</script>

<template>
  <div class="space-y-6">
    <div class="flex items-center justify-between">
      <div>
        <Breadcrumbs />
        <h1 class="text-2xl md:text-3xl font-bold tracking-tight mt-2">Support Tickets</h1>
        <p class="text-muted-foreground mt-1">View and manage support tickets</p>
      </div>
      <Button>
        <Plus :size="18" class="mr-2" />
        New Ticket
      </Button>
    </div>

    <!-- Filters -->
    <Card>
      <CardContent class="pt-6">
        <div class="flex flex-col md:flex-row gap-4">
          <div class="flex-1 relative">
            <Search
              :size="18"
              class="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground"
            />
            <input
              v-model="searchQuery"
              @input="loadTickets"
              type="text"
              placeholder="Search tickets..."
              class="w-full pl-10 pr-4 py-2 bg-background border rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
            />
          </div>
          <select
            v-model="statusFilter"
            @change="loadTickets"
            class="px-4 py-2 bg-background border rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
          >
            <option :value="null">All Statuses</option>
            <option value="Open">Open</option>
            <option value="InProgress">In Progress</option>
            <option value="Resolved">Resolved</option>
            <option value="Closed">Closed</option>
          </select>
        </div>
      </CardContent>
    </Card>

    <!-- Tickets List -->
    <Card>
      <CardHeader>
        <CardTitle>Tickets ({{ totalCount }})</CardTitle>
      </CardHeader>
      <CardContent>
        <div v-if="isLoading" class="text-center py-12 text-muted-foreground">
          Loading tickets...
        </div>
        <div v-else-if="tickets.length === 0" class="text-center py-12 text-muted-foreground">
          <MessageSquare :size="48" class="mx-auto mb-3 opacity-50" />
          <p>No tickets found</p>
          <p class="text-sm mt-1">Create a new ticket to get started</p>
        </div>
        <div v-else class="space-y-3">
          <div
            v-for="ticket in tickets"
            :key="ticket.id"
            class="p-4 border rounded-lg hover:bg-accent/50 transition-colors cursor-pointer"
          >
            <div class="flex items-start justify-between">
              <div class="flex-1">
                <div class="flex items-center gap-2">
                  <div class="font-medium text-lg">{{ ticket.title }}</div>
                  <component
                    :is="getStatusIcon(ticket.status)"
                    :size="16"
                    :class="getStatusColor(ticket.status)"
                  />
                </div>
                <div class="text-sm text-muted-foreground mt-1">
                  {{ ticket.type }} • Priority: {{ ticket.priority }}
                </div>
                <div v-if="ticket.description" class="text-sm text-muted-foreground mt-2 line-clamp-2">
                  {{ ticket.description }}
                </div>
                <div class="text-sm text-muted-foreground mt-2">
                  {{ ticket.noteCount }} note{{ ticket.noteCount !== 1 ? 's' : '' }} • 
                  Created: {{ new Date(ticket.created).toLocaleDateString() }}
                </div>
              </div>
            </div>
          </div>
        </div>
      </CardContent>
    </Card>
  </div>
</template>

