<template>
  <div class="space-y-6">
    <!-- Header -->
    <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
      <div class="flex items-center justify-between">
        <div>
          <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
            Collaborative Business Networks
          </h1>
          <p class="mt-2 text-gray-600 dark:text-gray-300">
            Connect, collaborate, and grow with other businesses in your industry and region
          </p>
        </div>
        <div class="flex space-x-2">
          <UButton
            variant="outline"
            icon="i-heroicons-plus-circle"
            @click="showCreateNetwork = true"
          >
            Create Network
          </UButton>
          <UButton
            icon="i-heroicons-magnifying-glass"
            @click="showDiscoverNetworks = true"
          >
            Discover Networks
          </UButton>
        </div>
      </div>
    </div>

    <!-- Network Overview -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-6">
      <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <div class="w-8 h-8 bg-blue-100 dark:bg-blue-900 rounded-md flex items-center justify-center">
              <UIcon name="i-heroicons-user-group" class="w-5 h-5 text-blue-600 dark:text-blue-400" />
            </div>
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-500 dark:text-gray-400">Your Networks</p>
            <p class="text-2xl font-semibold text-gray-900 dark:text-white">{{ yourNetworks.length }}</p>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <div class="w-8 h-8 bg-green-100 dark:bg-green-900 rounded-md flex items-center justify-center">
              <UIcon name="i-heroicons-handshake" class="w-5 h-5 text-green-600 dark:text-green-400" />
            </div>
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-500 dark:text-gray-400">Active Collaborations</p>
            <p class="text-2xl font-semibold text-gray-900 dark:text-white">{{ activeCollaborations }}</p>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <div class="w-8 h-8 bg-yellow-100 dark:bg-yellow-900 rounded-md flex items-center justify-center">
              <UIcon name="i-heroicons-light-bulb" class="w-5 h-5 text-yellow-600 dark:text-yellow-400" />
            </div>
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-500 dark:text-gray-400">Opportunities</p>
            <p class="text-2xl font-semibold text-gray-900 dark:text-white">{{ availableOpportunities }}</p>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <div class="w-8 h-8 bg-purple-100 dark:bg-purple-900 rounded-md flex items-center justify-center">
              <UIcon name="i-heroicons-trophy" class="w-5 h-5 text-purple-600 dark:text-purple-400" />
            </div>
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-500 dark:text-gray-400">Network Score</p>
            <p class="text-2xl font-semibold text-gray-900 dark:text-white">{{ networkScore }}/100</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Your Networks -->
    <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
      <h2 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">
        Your Business Networks
      </h2>
      
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <div
          v-for="network in yourNetworks"
          :key="network.id"
          class="border border-gray-200 dark:border-gray-700 rounded-lg p-4 hover:shadow-md transition-shadow"
        >
          <div class="flex items-start justify-between mb-3">
            <div class="flex items-center">
              <img
                :src="network.logo"
                :alt="network.name"
                class="w-12 h-12 rounded-lg object-cover mr-3"
              />
              <div>
                <h3 class="font-medium text-gray-900 dark:text-white">
                  {{ network.name }}
                </h3>
                <p class="text-sm text-gray-500 dark:text-gray-400">
                  {{ network.industry }} • {{ network.members }} members
                </p>
              </div>
            </div>
            <UBadge
              :color="getNetworkStatusColor(network.status)"
              variant="soft"
            >
              {{ network.status }}
            </UBadge>
          </div>
          
          <p class="text-sm text-gray-600 dark:text-gray-300 mb-4">
            {{ network.description }}
          </p>
          
          <div class="grid grid-cols-2 gap-4 mb-4">
            <div>
              <span class="text-sm text-gray-500">Your Role:</span>
              <p class="font-medium">{{ network.yourRole }}</p>
            </div>
            <div>
              <span class="text-sm text-gray-500">Joined:</span>
              <p class="font-medium">{{ formatDate(network.joinedDate) }}</p>
            </div>
            <div>
              <span class="text-sm text-gray-500">Activity Level:</span>
              <div class="flex items-center">
                <UProgress
                  :value="network.activityLevel"
                  color="green"
                  size="sm"
                  class="flex-1 mr-2"
                />
                <span class="text-sm">{{ network.activityLevel }}%</span>
              </div>
            </div>
            <div>
              <span class="text-sm text-gray-500">Collaborations:</span>
              <p class="font-medium">{{ network.collaborations }}</p>
            </div>
          </div>
          
          <div class="flex space-x-2">
            <UButton
              size="sm"
              @click="viewNetwork(network.id)"
            >
              View Network
            </UButton>
            <UButton
              size="sm"
              variant="outline"
              @click="viewOpportunities(network.id)"
            >
              Opportunities
            </UButton>
            <UButton
              size="sm"
              variant="ghost"
              icon="i-heroicons-chat-bubble-left-right"
              @click="openNetworkChat(network.id)"
            >
              Chat
            </UButton>
          </div>
        </div>
      </div>
    </div>

    <!-- Recent Activities -->
    <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
      <h2 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">
        Recent Network Activities
      </h2>
      
      <div class="space-y-4">
        <div
          v-for="activity in recentActivities"
          :key="activity.id"
          class="flex items-start space-x-3 p-3 bg-gray-50 dark:bg-gray-700 rounded-lg"
        >
          <div class="flex-shrink-0">
            <div
              :class="[
                'w-8 h-8 rounded-full flex items-center justify-center',
                getActivityIconBg(activity.type)
              ]"
            >
              <UIcon
                :name="getActivityIcon(activity.type)"
                :class="getActivityIconColor(activity.type)"
                class="w-4 h-4"
              />
            </div>
          </div>
          <div class="flex-1">
            <p class="text-sm text-gray-900 dark:text-white">
              {{ activity.description }}
            </p>
            <div class="flex items-center mt-1 text-xs text-gray-500 dark:text-gray-400">
              <span>{{ activity.networkName }}</span>
              <span class="mx-2">•</span>
              <span>{{ formatTimeAgo(activity.timestamp) }}</span>
            </div>
          </div>
          <UButton
            v-if="activity.actionable"
            size="sm"
            variant="ghost"
            @click="handleActivity(activity.id)"
          >
            {{ activity.actionText }}
          </UButton>
        </div>
      </div>
    </div>

    <!-- Business Opportunities -->
    <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
      <h2 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">
        Business Opportunities
      </h2>
      
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <div
          v-for="opportunity in businessOpportunities"
          :key="opportunity.id"
          class="border border-gray-200 dark:border-gray-700 rounded-lg p-4"
        >
          <div class="flex items-start justify-between mb-3">
            <div>
              <h3 class="font-medium text-gray-900 dark:text-white">
                {{ opportunity.title }}
              </h3>
              <p class="text-sm text-gray-500 dark:text-gray-400">
                {{ opportunity.networkName }} • Posted {{ formatTimeAgo(opportunity.postedDate) }}
              </p>
            </div>
            <UBadge
              :color="getOpportunityTypeColor(opportunity.type)"
              variant="soft"
            >
              {{ opportunity.type }}
            </UBadge>
          </div>
          
          <p class="text-sm text-gray-600 dark:text-gray-300 mb-3">
            {{ opportunity.description }}
          </p>
          
          <div class="grid grid-cols-2 gap-4 mb-4">
            <div>
              <span class="text-sm text-gray-500">Budget Range:</span>
              <p class="font-medium">{{ opportunity.budgetRange }}</p>
            </div>
            <div>
              <span class="text-sm text-gray-500">Timeline:</span>
              <p class="font-medium">{{ opportunity.timeline }}</p>
            </div>
            <div>
              <span class="text-sm text-gray-500">Skills Needed:</span>
              <p class="font-medium">{{ opportunity.skillsRequired.slice(0, 2).join(', ') }}</p>
            </div>
            <div>
              <span class="text-sm text-gray-500">Applications:</span>
              <p class="font-medium">{{ opportunity.applicants }} interested</p>
            </div>
          </div>
          
          <div class="flex space-x-2">
            <UButton
              size="sm"
              @click="applyToOpportunity(opportunity.id)"
            >
              Express Interest
            </UButton>
            <UButton
              size="sm"
              variant="outline"
              @click="viewOpportunityDetails(opportunity.id)"
            >
              View Details
            </UButton>
          </div>
        </div>
      </div>
    </div>

    <!-- Create Network Modal -->
    <UModal v-model="showCreateNetwork" :ui="{ width: 'sm:max-w-2xl' }">
      <UCard>
        <template #header>
          <div class="flex items-center justify-between">
            <h3 class="text-lg font-semibold">Create Business Network</h3>
          </div>
        </template>

        <UForm
          :schema="networkSchema"
          :state="newNetwork"
          @submit="createNetwork"
        >
          <div class="space-y-4">
            <UFormGroup label="Network Name" name="name">
              <UInput v-model="newNetwork.name" placeholder="e.g., Cape Town Tech Startups" />
            </UFormGroup>

            <UFormGroup label="Description" name="description">
              <UTextarea
                v-model="newNetwork.description"
                placeholder="Describe the purpose and goals of this network..."
                rows="3"
              />
            </UFormGroup>

            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <UFormGroup label="Industry" name="industry">
                <USelectMenu
                  v-model="newNetwork.industry"
                  :options="industries"
                  placeholder="Select industry"
                />
              </UFormGroup>

              <UFormGroup label="Network Type" name="type">
                <USelectMenu
                  v-model="newNetwork.type"
                  :options="networkTypes"
                  placeholder="Select type"
                />
              </UFormGroup>
            </div>

            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <UFormGroup label="Geographic Focus" name="region">
                <USelectMenu
                  v-model="newNetwork.region"
                  :options="regions"
                  placeholder="Select region"
                />
              </UFormGroup>

              <UFormGroup label="Privacy Level" name="privacy">
                <USelectMenu
                  v-model="newNetwork.privacy"
                  :options="privacyLevels"
                  placeholder="Select privacy level"
                />
              </UFormGroup>
            </div>

            <UFormGroup label="Membership Criteria" name="criteria">
              <UTextarea
                v-model="newNetwork.criteria"
                placeholder="What criteria should businesses meet to join this network?"
                rows="3"
              />
            </UFormGroup>

            <UFormGroup label="Network Logo" name="logo">
              <UInput
                type="file"
                accept="image/*"
                @change="handleLogoUpload"
              />
              <p class="text-sm text-gray-500 mt-1">Upload a logo for your network</p>
            </UFormGroup>
          </div>

          <template #footer>
            <div class="flex justify-end space-x-2">
              <UButton
                variant="ghost"
                @click="showCreateNetwork = false"
              >
                Cancel
              </UButton>
              <UButton type="submit">
                Create Network
              </UButton>
            </div>
          </template>
        </UForm>
      </UCard>
    </UModal>

    <!-- Discover Networks Modal -->
    <UModal v-model="showDiscoverNetworks" :ui="{ width: 'sm:max-w-4xl' }">
      <UCard>
        <template #header>
          <div class="flex items-center justify-between">
            <h3 class="text-lg font-semibold">Discover Business Networks</h3>
          </div>
        </template>

        <div class="space-y-4">
          <!-- Search and Filters -->
          <div class="flex flex-col sm:flex-row gap-4">
            <UInput
              v-model="networkSearch"
              icon="i-heroicons-magnifying-glass"
              placeholder="Search networks..."
              class="flex-1"
            />
            <USelectMenu
              v-model="filterIndustry"
              :options="['All Industries', ...industries]"
              placeholder="Industry"
            />
            <USelectMenu
              v-model="filterRegion"
              :options="['All Regions', ...regions]"
              placeholder="Region"
            />
          </div>

          <!-- Available Networks -->
          <div class="max-h-96 overflow-y-auto space-y-4">
            <div
              v-for="network in filteredAvailableNetworks"
              :key="network.id"
              class="border border-gray-200 dark:border-gray-700 rounded-lg p-4"
            >
              <div class="flex items-start justify-between mb-3">
                <div class="flex items-center">
                  <img
                    :src="network.logo"
                    :alt="network.name"
                    class="w-10 h-10 rounded-lg object-cover mr-3"
                  />
                  <div>
                    <h4 class="font-medium text-gray-900 dark:text-white">
                      {{ network.name }}
                    </h4>
                    <p class="text-sm text-gray-500 dark:text-gray-400">
                      {{ network.industry }} • {{ network.region }} • {{ network.members }} members
                    </p>
                  </div>
                </div>
                <UBadge
                  :color="network.privacy === 'Public' ? 'green' : 'yellow'"
                  variant="soft"
                >
                  {{ network.privacy }}
                </UBadge>
              </div>
              
              <p class="text-sm text-gray-600 dark:text-gray-300 mb-3">
                {{ network.description }}
              </p>
              
              <div class="flex justify-between items-center">
                <div class="text-sm text-gray-500">
                  Founded {{ formatDate(network.foundedDate) }}
                </div>
                <UButton
                  size="sm"
                  @click="requestToJoin(network.id)"
                >
                  {{ network.privacy === 'Public' ? 'Join Network' : 'Request to Join' }}
                </UButton>
              </div>
            </div>
          </div>
        </div>

        <template #footer>
          <div class="flex justify-end">
            <UButton
              variant="ghost"
              @click="showDiscoverNetworks = false"
            >
              Close
            </UButton>
          </div>
        </template>
      </UCard>
    </UModal>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { z } from 'zod'

// Page meta
definePageMeta({
  title: 'Collaborative Business Networks',
  description: 'Connect and collaborate with other businesses'
})

// Reactive data
const showCreateNetwork = ref(false)
const showDiscoverNetworks = ref(false)
const networkSearch = ref('')
const filterIndustry = ref('')
const filterRegion = ref('')

// Mock data
const activeCollaborations = ref(8)
const availableOpportunities = ref(12)
const networkScore = ref(85)

const yourNetworks = ref([
  {
    id: 1,
    name: 'Cape Town Tech Alliance',
    industry: 'Technology',
    members: 145,
    description: 'Connecting technology companies in the Cape Town area for collaboration and growth',
    yourRole: 'Member',
    joinedDate: '2024-06-15',
    activityLevel: 78,
    collaborations: 5,
    status: 'Active',
    logo: '/api/placeholder/48/48'
  },
  {
    id: 2,
    name: 'Small Business Growth Network',
    industry: 'General Business',
    members: 89,
    description: 'Supporting small business owners with resources, mentorship, and collaborative opportunities',
    yourRole: 'Admin',
    joinedDate: '2024-03-20',
    activityLevel: 92,
    collaborations: 3,
    status: 'Active',
    logo: '/api/placeholder/48/48'
  }
])

const recentActivities = ref([
  {
    id: 1,
    type: 'opportunity',
    description: 'New partnership opportunity posted in Cape Town Tech Alliance',
    networkName: 'Cape Town Tech Alliance',
    timestamp: new Date(Date.now() - 2 * 60 * 60 * 1000),
    actionable: true,
    actionText: 'View'
  },
  {
    id: 2,
    type: 'collaboration',
    description: 'Your proposal for joint marketing campaign was accepted',
    networkName: 'Small Business Growth Network',
    timestamp: new Date(Date.now() - 5 * 60 * 60 * 1000),
    actionable: true,
    actionText: 'Continue'
  },
  {
    id: 3,
    type: 'member',
    description: '3 new businesses joined Cape Town Tech Alliance',
    networkName: 'Cape Town Tech Alliance',
    timestamp: new Date(Date.now() - 1 * 24 * 60 * 60 * 1000),
    actionable: false,
    actionText: ''
  }
])

const businessOpportunities = ref([
  {
    id: 1,
    title: 'Joint Software Development Project',
    type: 'Partnership',
    description: 'Looking for development partners for a new SaaS platform targeting small businesses',
    networkName: 'Cape Town Tech Alliance',
    budgetRange: 'R100k - R500k',
    timeline: '6-9 months',
    skillsRequired: ['React', 'Node.js', 'AWS', 'UI/UX Design'],
    applicants: 7,
    postedDate: new Date(Date.now() - 3 * 24 * 60 * 60 * 1000)
  },
  {
    id: 2,
    title: 'Collaborative Marketing Campaign',
    type: 'Marketing',
    description: 'Joint marketing initiative for local businesses to increase visibility during holiday season',
    networkName: 'Small Business Growth Network',
    budgetRange: 'R25k - R75k',
    timeline: '2-3 months',
    skillsRequired: ['Digital Marketing', 'Content Creation', 'Social Media'],
    applicants: 12,
    postedDate: new Date(Date.now() - 1 * 24 * 60 * 60 * 1000)
  }
])

const availableNetworks = ref([
  {
    id: 3,
    name: 'Western Cape Manufacturing Network',
    industry: 'Manufacturing',
    region: 'Western Cape',
    members: 78,
    description: 'Connecting manufacturers for supply chain optimization and collaborative growth',
    privacy: 'Public',
    foundedDate: '2023-11-10',
    logo: '/api/placeholder/40/40'
  },
  {
    id: 4,
    name: 'Johannesburg Finance Professionals',
    industry: 'Finance',
    region: 'Gauteng',
    members: 156,
    description: 'Professional network for finance and accounting professionals in Johannesburg',
    privacy: 'Private',
    foundedDate: '2023-08-22',
    logo: '/api/placeholder/40/40'
  }
])

// Options
const industries = [
  'Technology',
  'Manufacturing',
  'Finance',
  'Healthcare',
  'Retail',
  'Construction',
  'Education',
  'Agriculture',
  'Tourism',
  'General Business'
]

const networkTypes = [
  'Professional',
  'Industry-Specific',
  'Regional',
  'Collaborative',
  'Peer-to-Peer',
  'Mentorship'
]

const regions = [
  'Western Cape',
  'Gauteng',
  'KwaZulu-Natal',
  'Eastern Cape',
  'Free State',
  'Limpopo',
  'Mpumalanga',
  'Northern Cape',
  'North West',
  'National'
]

const privacyLevels = [
  { label: 'Public - Anyone can join', value: 'Public' },
  { label: 'Private - Invitation only', value: 'Private' },
  { label: 'Application - Requires approval', value: 'Application' }
]

// Form data
const newNetwork = ref({
  name: '',
  description: '',
  industry: '',
  type: '',
  region: '',
  privacy: '',
  criteria: '',
  logo: null
})

// Form schema
const networkSchema = z.object({
  name: z.string().min(1, 'Network name is required'),
  description: z.string().min(1, 'Description is required'),
  industry: z.string().min(1, 'Industry is required'),
  type: z.string().min(1, 'Network type is required'),
  region: z.string().min(1, 'Region is required'),
  privacy: z.string().min(1, 'Privacy level is required'),
  criteria: z.string().min(1, 'Membership criteria are required')
})

// Computed
const filteredAvailableNetworks = computed(() => {
  let filtered = availableNetworks.value

  if (networkSearch.value) {
    filtered = filtered.filter(network =>
      network.name.toLowerCase().includes(networkSearch.value.toLowerCase()) ||
      network.description.toLowerCase().includes(networkSearch.value.toLowerCase())
    )
  }

  if (filterIndustry.value && filterIndustry.value !== 'All Industries') {
    filtered = filtered.filter(network => network.industry === filterIndustry.value)
  }

  if (filterRegion.value && filterRegion.value !== 'All Regions') {
    filtered = filtered.filter(network => network.region === filterRegion.value)
  }

  return filtered
})

// Methods
const formatDate = (dateString: string): string => {
  return new Date(dateString).toLocaleDateString('en-ZA')
}

const formatTimeAgo = (date: Date): string => {
  const now = new Date()
  const diff = now.getTime() - date.getTime()
  const hours = Math.floor(diff / (1000 * 60 * 60))
  const days = Math.floor(hours / 24)

  if (days > 0) {
    return `${days} day${days > 1 ? 's' : ''} ago`
  } else if (hours > 0) {
    return `${hours} hour${hours > 1 ? 's' : ''} ago`
  } else {
    return 'Recently'
  }
}

const getNetworkStatusColor = (status: string): string => {
  const colors: Record<string, string> = {
    'Active': 'green',
    'Inactive': 'gray',
    'Pending': 'yellow'
  }
  return colors[status] || 'gray'
}

const getOpportunityTypeColor = (type: string): string => {
  const colors: Record<string, string> = {
    'Partnership': 'blue',
    'Marketing': 'purple',
    'Contract': 'green',
    'Investment': 'yellow'
  }
  return colors[type] || 'gray'
}

const getActivityIconBg = (type: string): string => {
  const backgrounds: Record<string, string> = {
    'opportunity': 'bg-blue-100 dark:bg-blue-900',
    'collaboration': 'bg-green-100 dark:bg-green-900',
    'member': 'bg-purple-100 dark:bg-purple-900'
  }
  return backgrounds[type] || 'bg-gray-100 dark:bg-gray-700'
}

const getActivityIcon = (type: string): string => {
  const icons: Record<string, string> = {
    'opportunity': 'i-heroicons-light-bulb',
    'collaboration': 'i-heroicons-handshake',
    'member': 'i-heroicons-user-plus'
  }
  return icons[type] || 'i-heroicons-information-circle'
}

const getActivityIconColor = (type: string): string => {
  const colors: Record<string, string> = {
    'opportunity': 'text-blue-600 dark:text-blue-400',
    'collaboration': 'text-green-600 dark:text-green-400',
    'member': 'text-purple-600 dark:text-purple-400'
  }
  return colors[type] || 'text-gray-600 dark:text-gray-400'
}

const viewNetwork = (networkId: number) => {
  console.log('Viewing network:', networkId)
  // Navigate to network details
}

const viewOpportunities = (networkId: number) => {
  console.log('Viewing opportunities for network:', networkId)
  // Navigate to network opportunities
}

const openNetworkChat = (networkId: number) => {
  console.log('Opening chat for network:', networkId)
  // Open network chat
}

const handleActivity = (activityId: number) => {
  console.log('Handling activity:', activityId)
  // Handle activity action
}

const applyToOpportunity = (opportunityId: number) => {
  console.log('Applying to opportunity:', opportunityId)
  // Open application modal
}

const viewOpportunityDetails = (opportunityId: number) => {
  console.log('Viewing opportunity details:', opportunityId)
  // Navigate to opportunity details
}

const requestToJoin = (networkId: number) => {
  console.log('Requesting to join network:', networkId)
  // Send join request
}

const handleLogoUpload = (event: Event) => {
  const file = (event.target as HTMLInputElement).files?.[0]
  if (file) {
    newNetwork.value.logo = file
  }
}

const createNetwork = async (data: any) => {
  try {
    console.log('Creating network:', data)
    
    // Simulate API call
    await new Promise(resolve => setTimeout(resolve, 1000))
    
    // Reset form and close modal
    newNetwork.value = {
      name: '',
      description: '',
      industry: '',
      type: '',
      region: '',
      privacy: '',
      criteria: '',
      logo: null
    }
    showCreateNetwork.value = false
    
    // Show success message
    // useToast().add({
    //   title: 'Network Created',
    //   description: 'Your business network has been created successfully.',
    //   color: 'green'
    // })
  } catch (error) {
    console.error('Error creating network:', error)
  }
}
</script>
