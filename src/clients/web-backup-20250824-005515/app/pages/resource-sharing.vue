<template>
  <div class="space-y-6">
    <!-- Header -->
    <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
      <div class="flex items-center justify-between">
        <div>
          <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
            {{ $t('collaborative.resource_sharing') }}
          </h1>
          <p class="mt-2 text-gray-600 dark:text-gray-300">
            Share assets, tools, and resources with other businesses in your network
          </p>
        </div>
        <UButton
          icon="i-heroicons-plus"
          @click="showShareResource = true"
        >
          Share Resource
        </UButton>
      </div>
    </div>

    <!-- Quick Stats -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-6">
      <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <div class="w-8 h-8 bg-blue-100 dark:bg-blue-900 rounded-md flex items-center justify-center">
              <UIcon name="i-heroicons-cube" class="w-5 h-5 text-blue-600 dark:text-blue-400" />
            </div>
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-500 dark:text-gray-400">Available Resources</p>
            <p class="text-2xl font-semibold text-gray-900 dark:text-white">24</p>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <div class="w-8 h-8 bg-green-100 dark:bg-green-900 rounded-md flex items-center justify-center">
              <UIcon name="i-heroicons-clock" class="w-5 h-5 text-green-600 dark:text-green-400" />
            </div>
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-500 dark:text-gray-400">Your Bookings</p>
            <p class="text-2xl font-semibold text-gray-900 dark:text-white">3</p>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <div class="w-8 h-8 bg-yellow-100 dark:bg-yellow-900 rounded-md flex items-center justify-center">
              <UIcon name="i-heroicons-share" class="w-5 h-5 text-yellow-600 dark:text-yellow-400" />
            </div>
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-500 dark:text-gray-400">Shared by You</p>
            <p class="text-2xl font-semibold text-gray-900 dark:text-white">5</p>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <div class="w-8 h-8 bg-purple-100 dark:bg-purple-900 rounded-md flex items-center justify-center">
              <UIcon name="i-heroicons-currency-dollar" class="w-5 h-5 text-purple-600 dark:text-purple-400" />
            </div>
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-500 dark:text-gray-400">Savings This Month</p>
            <p class="text-2xl font-semibold text-gray-900 dark:text-white">R8,450</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Filter and Search -->
    <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
      <div class="flex flex-col sm:flex-row gap-4">
        <div class="flex-1">
          <UInput
            v-model="searchQuery"
            icon="i-heroicons-magnifying-glass"
            placeholder="Search resources..."
          />
        </div>
        <div class="flex gap-2">
          <USelectMenu
            v-model="selectedCategory"
            :options="categories"
            placeholder="Category"
          />
          <USelectMenu
            v-model="selectedLocation"
            :options="locations"
            placeholder="Location"
          />
          <USelectMenu
            v-model="selectedAvailability"
            :options="availabilityOptions"
            placeholder="Availability"
          />
        </div>
      </div>
    </div>

    <!-- Available Resources -->
    <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
      <h2 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">
        Available Resources
      </h2>
      
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        <div
          v-for="resource in filteredResources"
          :key="resource.id"
          class="border border-gray-200 dark:border-gray-700 rounded-lg p-4 hover:shadow-md transition-shadow"
        >
          <div class="flex items-start justify-between mb-3">
            <div>
              <h3 class="font-medium text-gray-900 dark:text-white">
                {{ resource.name }}
              </h3>
              <p class="text-sm text-gray-500 dark:text-gray-400">
                {{ resource.owner }}
              </p>
            </div>
            <UBadge
              :color="getAvailabilityColor(resource.availability)"
              variant="soft"
            >
              {{ resource.availability }}
            </UBadge>
          </div>
          
          <div class="mb-3">
            <img
              :src="resource.image"
              :alt="resource.name"
              class="w-full h-32 object-cover rounded-md"
            />
          </div>
          
          <p class="text-sm text-gray-600 dark:text-gray-300 mb-3">
            {{ resource.description }}
          </p>
          
          <div class="space-y-2 text-sm">
            <div class="flex justify-between">
              <span class="text-gray-500">Category:</span>
              <span class="font-medium">{{ resource.category }}</span>
            </div>
            <div class="flex justify-between">
              <span class="text-gray-500">Location:</span>
              <span class="font-medium">{{ resource.location }}</span>
            </div>
            <div class="flex justify-between">
              <span class="text-gray-500">Rate:</span>
              <span class="font-medium">R{{ resource.hourlyRate }}/hour</span>
            </div>
            <div class="flex justify-between">
              <span class="text-gray-500">Rating:</span>
              <div class="flex items-center">
                <span class="font-medium mr-1">{{ resource.rating }}</span>
                <div class="flex text-yellow-400">
                  <UIcon
                    v-for="star in 5"
                    :key="star"
                    name="i-heroicons-star"
                    :class="star <= resource.rating ? 'text-yellow-400' : 'text-gray-300'"
                    class="w-4 h-4"
                  />
                </div>
              </div>
            </div>
          </div>
          
          <div class="mt-4 flex space-x-2">
            <UButton
              size="sm"
              @click="bookResource(resource.id)"
              :disabled="resource.availability === 'Unavailable'"
            >
              Book Now
            </UButton>
            <UButton
              size="sm"
              variant="outline"
              @click="viewResource(resource.id)"
            >
              View Details
            </UButton>
            <UButton
              size="sm"
              variant="ghost"
              icon="i-heroicons-heart"
              @click="toggleFavorite(resource.id)"
            />
          </div>
        </div>
      </div>
    </div>

    <!-- Your Shared Resources -->
    <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
      <div class="flex items-center justify-between mb-4">
        <h2 class="text-lg font-semibold text-gray-900 dark:text-white">
          Your Shared Resources
        </h2>
        <UButton
          size="sm"
          variant="outline"
          @click="showShareResource = true"
        >
          Add Resource
        </UButton>
      </div>
      
      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
          <thead class="bg-gray-50 dark:bg-gray-700">
            <tr>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                Resource
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                Category
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                Bookings
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                Revenue
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
              v-for="sharedResource in sharedResources"
              :key="sharedResource.id"
            >
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="flex items-center">
                  <img
                    class="h-10 w-10 rounded-lg object-cover"
                    :src="sharedResource.image"
                    :alt="sharedResource.name"
                  />
                  <div class="ml-4">
                    <div class="text-sm font-medium text-gray-900 dark:text-white">
                      {{ sharedResource.name }}
                    </div>
                    <div class="text-sm text-gray-500 dark:text-gray-400">
                      R{{ sharedResource.hourlyRate }}/hour
                    </div>
                  </div>
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">
                {{ sharedResource.category }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">
                {{ sharedResource.totalBookings }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">
                R{{ formatCurrency(sharedResource.totalRevenue) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <UBadge
                  :color="getAvailabilityColor(sharedResource.status)"
                  variant="soft"
                >
                  {{ sharedResource.status }}
                </UBadge>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                <UDropdown :items="getResourceActions(sharedResource.id)">
                  <UButton variant="ghost" icon="i-heroicons-ellipsis-vertical" />
                </UDropdown>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Share Resource Modal -->
    <UModal v-model="showShareResource" :ui="{ width: 'sm:max-w-2xl' }">
      <UCard>
        <template #header>
          <div class="flex items-center justify-between">
            <h3 class="text-lg font-semibold">Share a Resource</h3>
          </div>
        </template>

        <UForm
          :schema="shareResourceSchema"
          :state="newResource"
          @submit="shareResource"
        >
          <div class="space-y-4">
            <UFormGroup label="Resource Name" name="name">
              <UInput v-model="newResource.name" placeholder="e.g., Industrial Printer" />
            </UFormGroup>

            <UFormGroup label="Description" name="description">
              <UTextarea
                v-model="newResource.description"
                placeholder="Describe the resource and its capabilities..."
                rows="3"
              />
            </UFormGroup>

            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <UFormGroup label="Category" name="category">
                <USelectMenu
                  v-model="newResource.category"
                  :options="categories"
                  placeholder="Select category"
                />
              </UFormGroup>

              <UFormGroup label="Location" name="location">
                <USelectMenu
                  v-model="newResource.location"
                  :options="locations"
                  placeholder="Select location"
                />
              </UFormGroup>
            </div>

            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <UFormGroup label="Hourly Rate (R)" name="hourlyRate">
                <UInput
                  v-model="newResource.hourlyRate"
                  type="number"
                  step="0.01"
                  placeholder="0.00"
                />
              </UFormGroup>

              <UFormGroup label="Minimum Booking (hours)" name="minBooking">
                <UInput
                  v-model="newResource.minBooking"
                  type="number"
                  placeholder="1"
                />
              </UFormGroup>
            </div>

            <UFormGroup label="Availability Schedule" name="schedule">
              <UTextarea
                v-model="newResource.schedule"
                placeholder="e.g., Mon-Fri 8AM-5PM, weekends by arrangement"
                rows="2"
              />
            </UFormGroup>

            <UFormGroup label="Terms & Conditions" name="terms">
              <UTextarea
                v-model="newResource.terms"
                placeholder="Usage guidelines, damage policy, cancellation terms..."
                rows="3"
              />
            </UFormGroup>

            <UFormGroup label="Upload Images" name="images">
              <UInput
                type="file"
                multiple
                accept="image/*"
                @change="handleImageUpload"
              />
              <p class="text-sm text-gray-500 mt-1">Upload up to 5 images of your resource</p>
            </UFormGroup>
          </div>

          <template #footer>
            <div class="flex justify-end space-x-2">
              <UButton
                variant="ghost"
                @click="showShareResource = false"
              >
                Cancel
              </UButton>
              <UButton type="submit">
                Share Resource
              </UButton>
            </div>
          </template>
        </UForm>
      </UCard>
    </UModal>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { z } from 'zod'

// Page meta
definePageMeta({
  title: 'Resource Sharing',
  description: 'Share and access business resources collaboratively'
})

// Reactive data
const showShareResource = ref(false)
const searchQuery = ref('')
const selectedCategory = ref('')
const selectedLocation = ref('')
const selectedAvailability = ref('')

// Options
const categories = [
  'Equipment',
  'Vehicles',
  'Tools',
  'Office Space',
  'Technology',
  'Machinery',
  'Furniture',
  'Other'
]

const locations = [
  'Cape Town',
  'Johannesburg',
  'Durban',
  'Pretoria',
  'Port Elizabeth',
  'Bloemfontein',
  'East London',
  'Pietermaritzburg'
]

const availabilityOptions = [
  'Available',
  'Booked',
  'Unavailable',
  'Maintenance'
]

// Mock data
const resources = ref([
  {
    id: 1,
    name: 'Industrial 3D Printer',
    owner: 'TechMakers Ltd.',
    description: 'High-precision 3D printer for prototypes and small production runs',
    category: 'Equipment',
    location: 'Cape Town',
    hourlyRate: 250,
    rating: 4.8,
    availability: 'Available',
    image: '/api/placeholder/300/200'
  },
  {
    id: 2,
    name: 'Delivery Van',
    owner: 'LogiCorp',
    description: 'Large delivery van perfect for local deliveries and transport',
    category: 'Vehicles',
    location: 'Johannesburg',
    hourlyRate: 180,
    rating: 4.5,
    availability: 'Available',
    image: '/api/placeholder/300/200'
  },
  {
    id: 3,
    name: 'Conference Room',
    owner: 'Business Hub',
    description: 'Modern conference room with video conferencing equipment',
    category: 'Office Space',
    location: 'Cape Town',
    hourlyRate: 120,
    rating: 4.7,
    availability: 'Booked',
    image: '/api/placeholder/300/200'
  }
])

const sharedResources = ref([
  {
    id: 1,
    name: 'Laser Cutter',
    category: 'Equipment',
    hourlyRate: 300,
    totalBookings: 45,
    totalRevenue: 13500,
    status: 'Available',
    image: '/api/placeholder/40/40'
  },
  {
    id: 2,
    name: 'Meeting Room',
    category: 'Office Space',
    hourlyRate: 80,
    totalBookings: 28,
    totalRevenue: 2240,
    status: 'Available',
    image: '/api/placeholder/40/40'
  }
])

// Form data
const newResource = ref({
  name: '',
  description: '',
  category: '',
  location: '',
  hourlyRate: null,
  minBooking: 1,
  schedule: '',
  terms: '',
  images: []
})

// Form schema
const shareResourceSchema = z.object({
  name: z.string().min(1, 'Resource name is required'),
  description: z.string().min(1, 'Description is required'),
  category: z.string().min(1, 'Category is required'),
  location: z.string().min(1, 'Location is required'),
  hourlyRate: z.number().min(0, 'Rate must be non-negative'),
  minBooking: z.number().min(1, 'Minimum booking must be at least 1 hour'),
  schedule: z.string().min(1, 'Schedule is required'),
  terms: z.string().optional()
})

// Computed
const filteredResources = computed(() => {
  let filtered = resources.value

  if (searchQuery.value) {
    filtered = filtered.filter(resource =>
      resource.name.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      resource.description.toLowerCase().includes(searchQuery.value.toLowerCase())
    )
  }

  if (selectedCategory.value) {
    filtered = filtered.filter(resource => resource.category === selectedCategory.value)
  }

  if (selectedLocation.value) {
    filtered = filtered.filter(resource => resource.location === selectedLocation.value)
  }

  if (selectedAvailability.value) {
    filtered = filtered.filter(resource => resource.availability === selectedAvailability.value)
  }

  return filtered
})

// Methods
const formatCurrency = (amount: number): string => {
  return new Intl.NumberFormat('en-ZA', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  }).format(amount)
}

const getAvailabilityColor = (availability: string): string => {
  const colors: Record<string, string> = {
    'Available': 'green',
    'Booked': 'yellow',
    'Unavailable': 'red',
    'Maintenance': 'gray'
  }
  return colors[availability] || 'gray'
}

const bookResource = (id: number) => {
  console.log('Booking resource:', id)
  // Navigate to booking form
}

const viewResource = (id: number) => {
  console.log('Viewing resource:', id)
  // Navigate to resource details
}

const toggleFavorite = (id: number) => {
  console.log('Toggling favorite:', id)
  // Toggle favorite status
}

const getResourceActions = (id: number) => {
  return [
    [{
      label: 'Edit',
      icon: 'i-heroicons-pencil',
      click: () => editResource(id)
    }],
    [{
      label: 'View Bookings',
      icon: 'i-heroicons-calendar',
      click: () => viewBookings(id)
    }],
    [{
      label: 'Disable',
      icon: 'i-heroicons-pause',
      click: () => disableResource(id)
    }],
    [{
      label: 'Delete',
      icon: 'i-heroicons-trash',
      click: () => deleteResource(id)
    }]
  ]
}

const editResource = (id: number) => {
  console.log('Editing resource:', id)
}

const viewBookings = (id: number) => {
  console.log('Viewing bookings for resource:', id)
}

const disableResource = (id: number) => {
  console.log('Disabling resource:', id)
}

const deleteResource = (id: number) => {
  console.log('Deleting resource:', id)
}

const handleImageUpload = (event: Event) => {
  const files = (event.target as HTMLInputElement).files
  if (files) {
    newResource.value.images = Array.from(files)
  }
}

const shareResource = async (data: any) => {
  try {
    console.log('Sharing resource:', data)
    
    // Simulate API call
    await new Promise(resolve => setTimeout(resolve, 1000))
    
    // Reset form and close modal
    newResource.value = {
      name: '',
      description: '',
      category: '',
      location: '',
      hourlyRate: null,
      minBooking: 1,
      schedule: '',
      terms: '',
      images: []
    }
    showShareResource.value = false
    
    // Show success message
    // useToast().add({
    //   title: 'Resource Shared',
    //   description: 'Your resource is now available for booking.',
    //   color: 'green'
    // })
  } catch (error) {
    console.error('Error sharing resource:', error)
  }
}
</script>
