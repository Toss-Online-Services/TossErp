<template>
  <div class="space-y-6">
    <!-- Header -->
    <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
      <div class="flex items-center justify-between">
        <div>
          <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
            {{ $t('collaborative.group_buying') }}
          </h1>
          <p class="mt-2 text-gray-600 dark:text-gray-300">
            Pool purchasing power with other businesses to negotiate better prices and terms
          </p>
        </div>
        <UButton
          icon="i-heroicons-plus"
          @click="showCreateGroupBuy = true"
        >
          Create Group Buy
        </UButton>
      </div>
    </div>

    <!-- Active Group Buys -->
    <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
      <h2 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">
        Active Group Buys
      </h2>
      
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        <div
          v-for="groupBuy in activeGroupBuys"
          :key="groupBuy.id"
          class="border border-gray-200 dark:border-gray-700 rounded-lg p-4 hover:shadow-md transition-shadow"
        >
          <div class="flex items-start justify-between mb-3">
            <h3 class="font-medium text-gray-900 dark:text-white">
              {{ groupBuy.title }}
            </h3>
            <UBadge
              :color="getStatusColor(groupBuy.status)"
              variant="soft"
            >
              {{ groupBuy.status }}
            </UBadge>
          </div>
          
          <p class="text-sm text-gray-600 dark:text-gray-300 mb-3">
            {{ groupBuy.description }}
          </p>
          
          <div class="space-y-2 text-sm">
            <div class="flex justify-between">
              <span class="text-gray-500">Organizer:</span>
              <span class="font-medium">{{ groupBuy.organizer }}</span>
            </div>
            <div class="flex justify-between">
              <span class="text-gray-500">Participants:</span>
              <span class="font-medium">{{ groupBuy.participants }}/{{ groupBuy.maxParticipants }}</span>
            </div>
            <div class="flex justify-between">
              <span class="text-gray-500">Target Amount:</span>
              <span class="font-medium">R{{ formatCurrency(groupBuy.targetAmount) }}</span>
            </div>
            <div class="flex justify-between">
              <span class="text-gray-500">Current Total:</span>
              <span class="font-medium">R{{ formatCurrency(groupBuy.currentAmount) }}</span>
            </div>
            <div class="flex justify-between">
              <span class="text-gray-500">Deadline:</span>
              <span class="font-medium">{{ formatDate(groupBuy.deadline) }}</span>
            </div>
          </div>
          
          <div class="mt-4">
            <UProgress
              :value="(groupBuy.currentAmount / groupBuy.targetAmount) * 100"
              color="primary"
              size="sm"
            />
          </div>
          
          <div class="mt-4 flex space-x-2">
            <UButton
              v-if="!groupBuy.userParticipating"
              size="sm"
              @click="joinGroupBuy(groupBuy.id)"
            >
              Join
            </UButton>
            <UButton
              v-else
              size="sm"
              variant="outline"
              @click="viewGroupBuy(groupBuy.id)"
            >
              View Details
            </UButton>
            <UButton
              size="sm"
              variant="ghost"
              icon="i-heroicons-chat-bubble-left-right"
              @click="openChat(groupBuy.id)"
            >
              Chat
            </UButton>
          </div>
        </div>
      </div>
    </div>

    <!-- Your Participation -->
    <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
      <h2 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">
        Your Participation
      </h2>
      
      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
          <thead class="bg-gray-50 dark:bg-gray-700">
            <tr>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                Group Buy
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                Your Commitment
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                Status
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                Actions
              </th>
            </tr>
          </thead>
          <tbody class="bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700">
            <tr
              v-for="participation in userParticipations"
              :key="participation.id"
            >
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm font-medium text-gray-900 dark:text-white">
                  {{ participation.groupBuyTitle }}
                </div>
                <div class="text-sm text-gray-500 dark:text-gray-400">
                  {{ participation.organizer }}
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm text-gray-900 dark:text-white">
                  R{{ formatCurrency(participation.commitmentAmount) }}
                </div>
                <div class="text-sm text-gray-500 dark:text-gray-400">
                  {{ participation.quantity }} units
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <UBadge
                  :color="getStatusColor(participation.status)"
                  variant="soft"
                >
                  {{ participation.status }}
                </UBadge>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                <UButton
                  size="sm"
                  variant="ghost"
                  @click="viewParticipation(participation.id)"
                >
                  View
                </UButton>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Create Group Buy Modal -->
    <UModal v-model="showCreateGroupBuy" :ui="{ width: 'sm:max-w-2xl' }">
      <UCard>
        <template #header>
          <div class="flex items-center justify-between">
            <h3 class="text-lg font-semibold">Create New Group Buy</h3>
          </div>
        </template>

        <UForm
          :schema="groupBuySchema"
          :state="newGroupBuy"
          @submit="createGroupBuy"
        >
          <div class="space-y-4">
            <UFormGroup label="Title" name="title">
              <UInput v-model="newGroupBuy.title" placeholder="e.g., Office Supplies Q1 2025" />
            </UFormGroup>

            <UFormGroup label="Description" name="description">
              <UTextarea
                v-model="newGroupBuy.description"
                placeholder="Describe what you're buying and the benefits..."
                rows="3"
              />
            </UFormGroup>

            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <UFormGroup label="Product/Service" name="product">
                <UInput v-model="newGroupBuy.product" placeholder="e.g., Copy Paper" />
              </UFormGroup>

              <UFormGroup label="Category" name="category">
                <USelectMenu
                  v-model="newGroupBuy.category"
                  :options="categories"
                  placeholder="Select category"
                />
              </UFormGroup>
            </div>

            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <UFormGroup label="Target Quantity" name="targetQuantity">
                <UInput
                  v-model="newGroupBuy.targetQuantity"
                  type="number"
                  placeholder="e.g., 1000"
                />
              </UFormGroup>

              <UFormGroup label="Unit" name="unit">
                <UInput v-model="newGroupBuy.unit" placeholder="e.g., reams, boxes" />
              </UFormGroup>
            </div>

            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <UFormGroup label="Estimated Unit Price" name="estimatedPrice">
                <UInput
                  v-model="newGroupBuy.estimatedPrice"
                  type="number"
                  step="0.01"
                  placeholder="0.00"
                />
              </UFormGroup>

              <UFormGroup label="Max Participants" name="maxParticipants">
                <UInput
                  v-model="newGroupBuy.maxParticipants"
                  type="number"
                  placeholder="e.g., 10"
                />
              </UFormGroup>
            </div>

            <UFormGroup label="Deadline" name="deadline">
              <UInput
                v-model="newGroupBuy.deadline"
                type="date"
              />
            </UFormGroup>

            <UFormGroup label="Requirements & Terms" name="terms">
              <UTextarea
                v-model="newGroupBuy.terms"
                placeholder="Any specific requirements, payment terms, delivery details..."
                rows="3"
              />
            </UFormGroup>
          </div>

          <template #footer>
            <div class="flex justify-end space-x-2">
              <UButton
                variant="ghost"
                @click="showCreateGroupBuy = false"
              >
                Cancel
              </UButton>
              <UButton type="submit">
                Create Group Buy
              </UButton>
            </div>
          </template>
        </UForm>
      </UCard>
    </UModal>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { z } from 'zod'

// Page meta
definePageMeta({
  title: 'Group Buying',
  description: 'Collaborative purchasing with other businesses'
})

// Reactive data
const showCreateGroupBuy = ref(false)

// Mock data
const activeGroupBuys = ref([
  {
    id: 1,
    title: 'Office Supplies Q1 2025',
    description: 'Bulk purchase of office supplies including paper, pens, and folders',
    organizer: 'ABC Trading Co.',
    participants: 7,
    maxParticipants: 12,
    targetAmount: 50000,
    currentAmount: 32500,
    deadline: '2025-02-15',
    status: 'Active',
    userParticipating: false
  },
  {
    id: 2,
    title: 'Industrial Cleaning Supplies',
    description: 'Cleaning products for offices and workshops',
    organizer: 'CleanCorp Ltd.',
    participants: 5,
    maxParticipants: 8,
    targetAmount: 25000,
    currentAmount: 18750,
    deadline: '2025-01-30',
    status: 'Active',
    userParticipating: true
  },
  {
    id: 3,
    title: 'Computer Equipment Bundle',
    description: 'Laptops, monitors, and accessories for small businesses',
    organizer: 'TechFlow Solutions',
    participants: 12,
    maxParticipants: 15,
    targetAmount: 150000,
    currentAmount: 125000,
    deadline: '2025-03-01',
    status: 'Active',
    userParticipating: false
  }
])

const userParticipations = ref([
  {
    id: 1,
    groupBuyTitle: 'Industrial Cleaning Supplies',
    organizer: 'CleanCorp Ltd.',
    commitmentAmount: 3750,
    quantity: 50,
    status: 'Committed'
  },
  {
    id: 2,
    groupBuyTitle: 'Marketing Materials Q4',
    organizer: 'PrintPro Services',
    commitmentAmount: 2500,
    quantity: 25,
    status: 'Completed'
  }
])

const categories = [
  'Office Supplies',
  'Cleaning Supplies',
  'Technology',
  'Marketing Materials',
  'Raw Materials',
  'Tools & Equipment',
  'Food & Beverages',
  'Other'
]

// Form data
const newGroupBuy = ref({
  title: '',
  description: '',
  product: '',
  category: '',
  targetQuantity: null,
  unit: '',
  estimatedPrice: null,
  maxParticipants: null,
  deadline: '',
  terms: ''
})

// Form schema
const groupBuySchema = z.object({
  title: z.string().min(1, 'Title is required'),
  description: z.string().min(1, 'Description is required'),
  product: z.string().min(1, 'Product is required'),
  category: z.string().min(1, 'Category is required'),
  targetQuantity: z.number().min(1, 'Target quantity must be greater than 0'),
  unit: z.string().min(1, 'Unit is required'),
  estimatedPrice: z.number().min(0, 'Price must be non-negative'),
  maxParticipants: z.number().min(1, 'Must allow at least 1 participant'),
  deadline: z.string().min(1, 'Deadline is required'),
  terms: z.string().optional()
})

// Methods
const formatCurrency = (amount: number): string => {
  return new Intl.NumberFormat('en-ZA', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  }).format(amount)
}

const formatDate = (dateString: string): string => {
  return new Date(dateString).toLocaleDateString('en-ZA')
}

const getStatusColor = (status: string): string => {
  const colors: Record<string, string> = {
    'Active': 'green',
    'Committed': 'blue',
    'Completed': 'gray',
    'Cancelled': 'red',
    'Pending': 'yellow'
  }
  return colors[status] || 'gray'
}

const joinGroupBuy = (id: number) => {
  // Implementation for joining a group buy
  console.log('Joining group buy:', id)
  // Navigate to join form or show join modal
}

const viewGroupBuy = (id: number) => {
  // Implementation for viewing group buy details
  console.log('Viewing group buy:', id)
  // Navigate to detail view
}

const openChat = (id: number) => {
  // Implementation for opening group chat
  console.log('Opening chat for group buy:', id)
  // Open chat modal or navigate to chat page
}

const viewParticipation = (id: number) => {
  // Implementation for viewing participation details
  console.log('Viewing participation:', id)
}

const createGroupBuy = async (data: any) => {
  try {
    // Implementation for creating group buy
    console.log('Creating group buy:', data)
    
    // Simulate API call
    await new Promise(resolve => setTimeout(resolve, 1000))
    
    // Reset form and close modal
    newGroupBuy.value = {
      title: '',
      description: '',
      product: '',
      category: '',
      targetQuantity: null,
      unit: '',
      estimatedPrice: null,
      maxParticipants: null,
      deadline: '',
      terms: ''
    }
    showCreateGroupBuy.value = false
    
    // Show success message
    // useToast().add({
    //   title: 'Group Buy Created',
    //   description: 'Your group buy has been created successfully.',
    //   color: 'green'
    // })
  } catch (error) {
    console.error('Error creating group buy:', error)
  }
}
</script>
