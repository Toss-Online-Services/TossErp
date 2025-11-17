<template>
  <AppLayout>
    <div class="p-6 space-y-6">
      <!-- Header -->
      <div class="flex justify-between items-center">
        <div>
          <h1 class="text-2xl font-bold">Leads</h1>
          <p class="text-muted-foreground">Manage potential customers and sales opportunities</p>
        </div>
        <div class="flex items-center space-x-4">
          <Button variant="outline">
            <Filter class="h-4 w-4 mr-2" />
            Filter
          </Button>
          <Button>
            <Plus class="h-4 w-4 mr-2" />
            Add Lead
          </Button>
        </div>
      </div>

      <!-- Stats -->
      <div class="grid grid-cols-1 md:grid-cols-4 gap-6">
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg border">
          <div class="flex items-center space-x-3">
            <div class="p-2 bg-blue-100 dark:bg-blue-900 rounded-lg">
              <Users class="h-6 w-6 text-blue-600" />
            </div>
            <div>
              <p class="text-sm text-muted-foreground">Total Leads</p>
              <p class="text-2xl font-bold">{{ totalLeads }}</p>
            </div>
          </div>
        </div>
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg border">
          <div class="flex items-center space-x-3">
            <div class="p-2 bg-green-100 dark:bg-green-900 rounded-lg">
              <TrendingUp class="h-6 w-6 text-green-600" />
            </div>
            <div>
              <p class="text-sm text-muted-foreground">Conversion Rate</p>
              <p class="text-2xl font-bold">{{ conversionRate }}%</p>
            </div>
          </div>
        </div>
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg border">
          <div class="flex items-center space-x-3">
            <div class="p-2 bg-yellow-100 dark:bg-yellow-900 rounded-lg">
              <Clock class="h-6 w-6 text-yellow-600" />
            </div>
            <div>
              <p class="text-sm text-muted-foreground">Hot Leads</p>
              <p class="text-2xl font-bold">{{ hotLeads }}</p>
            </div>
          </div>
        </div>
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg border">
          <div class="flex items-center space-x-3">
            <div class="p-2 bg-purple-100 dark:bg-purple-900 rounded-lg">
              <DollarSign class="h-6 w-6 text-purple-600" />
            </div>
            <div>
              <p class="text-sm text-muted-foreground">Pipeline Value</p>
              <p class="text-2xl font-bold">R {{ pipelineValue.toLocaleString() }}</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Filters and Search -->
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg border">
        <div class="flex flex-col md:flex-row md:items-center space-y-4 md:space-y-0 md:space-x-4">
          <div class="relative flex-1">
            <Search class="absolute left-3 top-3 h-4 w-4 text-muted-foreground" />
            <input
              v-model="searchQuery"
              type="text"
              placeholder="Search leads..."
              class="pl-10 pr-4 py-2 w-full rounded-md border border-input bg-background"
            />
          </div>
          <select v-model="statusFilter" class="px-3 py-2 border rounded-md">
            <option value="">All Statuses</option>
            <option value="new">New</option>
            <option value="contacted">Contacted</option>
            <option value="qualified">Qualified</option>
            <option value="proposal">Proposal</option>
            <option value="negotiation">Negotiation</option>
            <option value="converted">Converted</option>
            <option value="lost">Lost</option>
          </select>
          <select v-model="sourceFilter" class="px-3 py-2 border rounded-md">
            <option value="">All Sources</option>
            <option value="website">Website</option>
            <option value="referral">Referral</option>
            <option value="social">Social Media</option>
            <option value="event">Event</option>
            <option value="cold-call">Cold Call</option>
          </select>
        </div>
      </div>

      <!-- Leads Table -->
      <div class="bg-white dark:bg-gray-800 rounded-lg border">
        <div class="overflow-x-auto">
          <table class="w-full">
            <thead>
              <tr class="border-b">
                <th class="text-left p-4 font-medium">Lead</th>
                <th class="text-left p-4 font-medium">Company</th>
                <th class="text-left p-4 font-medium">Status</th>
                <th class="text-left p-4 font-medium">Source</th>
                <th class="text-left p-4 font-medium">Value</th>
                <th class="text-left p-4 font-medium">Last Contact</th>
                <th class="text-left p-4 font-medium">Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="lead in filteredLeads" :key="lead.id" class="border-b hover:bg-muted/50">
                <td class="p-4">
                  <div class="flex items-center space-x-3">
                    <div class="w-10 h-10 bg-primary/10 rounded-full flex items-center justify-center">
                      <span class="text-primary font-medium">{{ lead.name.charAt(0) }}</span>
                    </div>
                    <div>
                      <p class="font-medium">{{ lead.name }}</p>
                      <p class="text-sm text-muted-foreground">{{ lead.email }}</p>
                    </div>
                  </div>
                </td>
                <td class="p-4">
                  <div>
                    <p class="font-medium">{{ lead.company }}</p>
                    <p class="text-sm text-muted-foreground">{{ lead.position }}</p>
                  </div>
                </td>
                <td class="p-4">
                  <span class="px-2 py-1 rounded text-xs font-medium" :class="getStatusColor(lead.status)">
                    {{ lead.status }}
                  </span>
                </td>
                <td class="p-4 text-muted-foreground">{{ lead.source }}</td>
                <td class="p-4 font-medium">R {{ lead.value.toLocaleString() }}</td>
                <td class="p-4 text-muted-foreground">{{ lead.lastContact }}</td>
                <td class="p-4">
                  <div class="flex items-center space-x-2">
                    <Button size="sm" variant="ghost">
                      <Edit class="h-4 w-4" />
                    </Button>
                    <Button size="sm" variant="ghost">
                      <Phone class="h-4 w-4" />
                    </Button>
                    <Button size="sm" variant="ghost">
                      <Mail class="h-4 w-4" />
                    </Button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <!-- Pagination -->
      <div class="flex items-center justify-between">
        <p class="text-sm text-muted-foreground">
          Showing {{ filteredLeads.length }} of {{ leads.length }} leads
        </p>
        <div class="flex items-center space-x-2">
          <Button variant="outline" size="sm">Previous</Button>
          <Button variant="outline" size="sm">1</Button>
          <Button variant="outline" size="sm">2</Button>
          <Button variant="outline" size="sm">3</Button>
          <Button variant="outline" size="sm">Next</Button>
        </div>
      </div>
    </div>
  </AppLayout>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { 
  Plus, 
  Filter, 
  Search, 
  Edit, 
  Phone, 
  Mail,
  Users,
  TrendingUp,
  Clock,
  DollarSign
} from 'lucide-vue-next'

// Reactive data
const searchQuery = ref('')
const statusFilter = ref('')
const sourceFilter = ref('')

// Stats
const totalLeads = ref(156)
const conversionRate = ref(23.5)
const hotLeads = ref(18)
const pipelineValue = ref(245000)

// Mock leads data
const leads = ref([
  {
    id: 1,
    name: 'Nomsa Mbeki',
    email: 'nomsa@example.com',
    company: 'Soweto Spaza',
    position: 'Owner',
    status: 'qualified',
    source: 'referral',
    value: 15000,
    lastContact: '2024-01-15'
  },
  {
    id: 2,
    name: 'Thabo Mthembu',
    email: 'thabo@example.com',
    company: 'Khayelitsha Market',
    position: 'Manager',
    status: 'proposal',
    source: 'website',
    value: 25000,
    lastContact: '2024-01-14'
  },
  {
    id: 3,
    name: 'Zanele Dlamini',
    email: 'zanele@example.com',
    company: 'Tembisa Traders',
    position: 'Director',
    status: 'new',
    source: 'social',
    value: 8000,
    lastContact: '2024-01-13'
  },
  {
    id: 4,
    name: 'Sipho Ndaba',
    email: 'sipho@example.com',
    company: 'Alexandra Food Store',
    position: 'Owner',
    status: 'negotiation',
    source: 'event',
    value: 35000,
    lastContact: '2024-01-12'
  },
  {
    id: 5,
    name: 'Lerato Molefe',
    email: 'lerato@example.com',
    company: 'Orange Farm Co-op',
    position: 'Secretary',
    status: 'contacted',
    source: 'cold-call',
    value: 12000,
    lastContact: '2024-01-11'
  }
])

// Computed properties
const filteredLeads = computed(() => {
  return leads.value.filter(lead => {
    const matchesSearch = lead.name.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
                         lead.email.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
                         lead.company.toLowerCase().includes(searchQuery.value.toLowerCase())
    const matchesStatus = !statusFilter.value || lead.status === statusFilter.value
    const matchesSource = !sourceFilter.value || lead.source === sourceFilter.value
    
    return matchesSearch && matchesStatus && matchesSource
  })
})

// Methods
const getStatusColor = (status: string) => {
  const colors = {
    'new': 'bg-blue-100 text-blue-800',
    'contacted': 'bg-yellow-100 text-yellow-800',
    'qualified': 'bg-green-100 text-green-800',
    'proposal': 'bg-purple-100 text-purple-800',
    'negotiation': 'bg-orange-100 text-orange-800',
    'converted': 'bg-emerald-100 text-emerald-800',
    'lost': 'bg-red-100 text-red-800'
  }
  return colors[status as keyof typeof colors] || 'bg-gray-100 text-gray-800'
}
</script>