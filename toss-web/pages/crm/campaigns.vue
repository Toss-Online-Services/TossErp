<template>
  <AppLayout>
    <div class="p-6 space-y-6">
      <!-- Header -->
      <div class="flex justify-between items-center">
        <div>
          <h1 class="text-2xl font-bold">Marketing Campaigns</h1>
          <p class="text-muted-foreground">Plan, execute, and track your marketing campaigns</p>
        </div>
        <div class="flex items-center space-x-4">
          <Button variant="outline">
            <BarChart class="h-4 w-4 mr-2" />
            Analytics
          </Button>
          <Button>
            <Plus class="h-4 w-4 mr-2" />
            New Campaign
          </Button>
        </div>
      </div>

      <!-- Campaign Performance -->
      <div class="grid grid-cols-1 md:grid-cols-4 gap-6">
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg border">
          <div class="flex items-center space-x-3">
            <div class="p-2 bg-blue-100 dark:bg-blue-900 rounded-lg">
              <Megaphone class="h-6 w-6 text-blue-600" />
            </div>
            <div>
              <p class="text-sm text-muted-foreground">Active Campaigns</p>
              <p class="text-2xl font-bold">{{ activeCampaigns }}</p>
            </div>
          </div>
        </div>
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg border">
          <div class="flex items-center space-x-3">
            <div class="p-2 bg-green-100 dark:bg-green-900 rounded-lg">
              <Users class="h-6 w-6 text-green-600" />
            </div>
            <div>
              <p class="text-sm text-muted-foreground">Total Reach</p>
              <p class="text-2xl font-bold">{{ totalReach.toLocaleString() }}</p>
            </div>
          </div>
        </div>
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg border">
          <div class="flex items-center space-x-3">
            <div class="p-2 bg-yellow-100 dark:bg-yellow-900 rounded-lg">
              <Target class="h-6 w-6 text-yellow-600" />
            </div>
            <div>
              <p class="text-sm text-muted-foreground">Conversion Rate</p>
              <p class="text-2xl font-bold">{{ conversionRate }}%</p>
            </div>
          </div>
        </div>
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg border">
          <div class="flex items-center space-x-3">
            <div class="p-2 bg-purple-100 dark:bg-purple-900 rounded-lg">
              <DollarSign class="h-6 w-6 text-purple-600" />
            </div>
            <div>
              <p class="text-sm text-muted-foreground">ROI</p>
              <p class="text-2xl font-bold">{{ roi }}%</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Quick Actions -->
      <div class="bg-white dark:bg-gray-800 p-6 rounded-lg border">
        <h3 class="text-lg font-semibold mb-4">Quick Campaign Actions</h3>
        <div class="grid grid-cols-1 md:grid-cols-3 lg:grid-cols-5 gap-4">
          <Button variant="outline" class="h-20 flex-col space-y-2">
            <MessageSquare class="h-6 w-6" />
            <span class="text-sm">WhatsApp Blast</span>
          </Button>
          <Button variant="outline" class="h-20 flex-col space-y-2">
            <Mail class="h-6 w-6" />
            <span class="text-sm">Email Campaign</span>
          </Button>
          <Button variant="outline" class="h-20 flex-col space-y-2">
            <Smartphone class="h-6 w-6" />
            <span class="text-sm">SMS Campaign</span>
          </Button>
          <Button variant="outline" class="h-20 flex-col space-y-2">
            <Users class="h-6 w-6" />
            <span class="text-sm">Community Event</span>
          </Button>
          <Button variant="outline" class="h-20 flex-col space-y-2">
            <Gift class="h-6 w-6" />
            <span class="text-sm">Loyalty Program</span>
          </Button>
        </div>
      </div>

      <!-- Active Campaigns -->
      <div class="bg-white dark:bg-gray-800 rounded-lg border">
        <div class="p-6 border-b">
          <div class="flex justify-between items-center">
            <h3 class="text-lg font-semibold">Campaign Overview</h3>
            <div class="flex items-center space-x-4">
              <select class="px-3 py-2 border rounded-md text-sm">
                <option value="all">All Campaigns</option>
                <option value="active">Active</option>
                <option value="paused">Paused</option>
                <option value="completed">Completed</option>
              </select>
            </div>
          </div>
        </div>
        <div class="p-6">
          <div class="space-y-4">
            <div v-for="campaign in campaigns" :key="campaign.id" class="border rounded-lg p-4">
              <div class="flex items-start justify-between">
                <div class="flex-1">
                  <div class="flex items-center space-x-3 mb-2">
                    <h4 class="text-lg font-medium">{{ campaign.name }}</h4>
                    <span class="px-2 py-1 rounded text-xs font-medium" :class="getStatusColor(campaign.status)">
                      {{ campaign.status }}
                    </span>
                  </div>
                  <p class="text-muted-foreground text-sm mb-3">{{ campaign.description }}</p>
                  
                  <!-- Campaign Metrics -->
                  <div class="grid grid-cols-1 md:grid-cols-4 gap-4 mb-4">
                    <div class="text-center">
                      <p class="text-2xl font-bold text-blue-600">{{ campaign.metrics.reach.toLocaleString() }}</p>
                      <p class="text-xs text-muted-foreground">Reach</p>
                    </div>
                    <div class="text-center">
                      <p class="text-2xl font-bold text-green-600">{{ campaign.metrics.engagement }}%</p>
                      <p class="text-xs text-muted-foreground">Engagement</p>
                    </div>
                    <div class="text-center">
                      <p class="text-2xl font-bold text-yellow-600">{{ campaign.metrics.conversions }}</p>
                      <p class="text-xs text-muted-foreground">Conversions</p>
                    </div>
                    <div class="text-center">
                      <p class="text-2xl font-bold text-purple-600">R {{ campaign.metrics.revenue.toLocaleString() }}</p>
                      <p class="text-xs text-muted-foreground">Revenue</p>
                    </div>
                  </div>

                  <!-- Progress Bar -->
                  <div class="mb-3">
                    <div class="flex justify-between text-sm mb-1">
                      <span>Campaign Progress</span>
                      <span>{{ campaign.progress }}%</span>
                    </div>
                    <div class="w-full bg-gray-200 rounded-full h-2">
                      <div 
                        class="bg-primary h-2 rounded-full transition-all duration-300"
                        :style="{ width: campaign.progress + '%' }"
                      ></div>
                    </div>
                  </div>

                  <!-- Campaign Details -->
                  <div class="flex items-center space-x-4 text-sm text-muted-foreground">
                    <span class="flex items-center space-x-1">
                      <Calendar class="h-4 w-4" />
                      <span>{{ campaign.startDate }} - {{ campaign.endDate }}</span>
                    </span>
                    <span class="flex items-center space-x-1">
                      <DollarSign class="h-4 w-4" />
                      <span>Budget: R {{ campaign.budget.toLocaleString() }}</span>
                    </span>
                    <span class="flex items-center space-x-1">
                      <MessageSquare class="h-4 w-4" />
                      <span>{{ campaign.channel }}</span>
                    </span>
                  </div>
                </div>
                
                <!-- Action Buttons -->
                <div class="flex items-center space-x-2 ml-4">
                  <Button size="sm" variant="outline">
                    <Eye class="h-4 w-4" />
                  </Button>
                  <Button size="sm" variant="outline">
                    <Edit class="h-4 w-4" />
                  </Button>
                  <Button size="sm" variant="outline" v-if="campaign.status === 'active'">
                    <Pause class="h-4 w-4" />
                  </Button>
                  <Button size="sm" variant="outline" v-else-if="campaign.status === 'paused'">
                    <Play class="h-4 w-4" />
                  </Button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </AppLayout>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { 
  Plus, 
  BarChart,
  Megaphone,
  Users,
  Target,
  DollarSign,
  MessageSquare,
  Mail,
  Smartphone,
  Gift,
  Calendar,
  Eye,
  Edit,
  Pause,
  Play
} from 'lucide-vue-next'

// Stats
const activeCampaigns = ref(7)
const totalReach = ref(15420)
const conversionRate = ref(8.5)
const roi = ref(245)

// Mock campaigns data
const campaigns = ref([
  {
    id: 1,
    name: 'Back to School Special',
    description: 'Target families preparing for the new school year with stationery and uniform specials.',
    status: 'active',
    startDate: '2024-01-01',
    endDate: '2024-01-31',
    budget: 5000,
    channel: 'WhatsApp & Facebook',
    progress: 65,
    metrics: {
      reach: 2340,
      engagement: 12.5,
      conversions: 47,
      revenue: 12450
    }
  },
  {
    id: 2,
    name: 'Weekend Fresh Produce',
    description: 'Promote weekend vegetable deliveries to township communities.',
    status: 'active',
    startDate: '2024-01-15',
    endDate: '2024-02-15',
    budget: 3000,
    channel: 'SMS & Community Radio',
    progress: 45,
    metrics: {
      reach: 1820,
      engagement: 8.7,
      conversions: 23,
      revenue: 6780
    }
  },
  {
    id: 3,
    name: 'Loyalty Card Launch',
    description: 'Introduction of customer loyalty program with rewards and discounts.',
    status: 'paused',
    startDate: '2024-01-10',
    endDate: '2024-02-10',
    budget: 2500,
    channel: 'In-store & WhatsApp',
    progress: 30,
    metrics: {
      reach: 890,
      engagement: 15.2,
      conversions: 67,
      revenue: 8920
    }
  },
  {
    id: 4,
    name: 'Township Business Network',
    description: 'Connect local businesses for cross-promotion and bulk purchasing.',
    status: 'active',
    startDate: '2024-01-05',
    endDate: '2024-03-05',
    budget: 4000,
    channel: 'Community Events',
    progress: 55,
    metrics: {
      reach: 1560,
      engagement: 22.1,
      conversions: 34,
      revenue: 15600
    }
  },
  {
    id: 5,
    name: 'Digital Payment Adoption',
    description: 'Encourage customers to use digital payment methods with special discounts.',
    status: 'completed',
    startDate: '2023-12-01',
    endDate: '2023-12-31',
    budget: 1800,
    channel: 'SMS & In-store',
    progress: 100,
    metrics: {
      reach: 3240,
      engagement: 18.9,
      conversions: 156,
      revenue: 18950
    }
  }
])

// Methods
const getStatusColor = (status: string) => {
  const colors = {
    'active': 'bg-green-100 text-green-800',
    'paused': 'bg-yellow-100 text-yellow-800',
    'completed': 'bg-blue-100 text-blue-800',
    'draft': 'bg-gray-100 text-gray-800'
  }
  return colors[status as keyof typeof colors] || 'bg-gray-100 text-gray-800'
}
</script>