<template>
  <div class="space-y-6">
    <!-- Header -->
    <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
      <div class="flex items-center justify-between">
        <div>
          <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
            Pooled Credit
          </h1>
          <p class="mt-2 text-gray-600 dark:text-gray-300">
            Manage collaborative credit pools and lending within your business network
          </p>
        </div>
        <div class="flex space-x-2">
          <UButton
            variant="outline"
            icon="i-heroicons-plus-circle"
            @click="showJoinPool = true"
          >
            Join Credit Pool
          </UButton>
          <UButton
            icon="i-heroicons-plus"
            @click="showCreatePool = true"
          >
            Create Pool
          </UButton>
        </div>
      </div>
    </div>

    <!-- Credit Pool Stats -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-6">
      <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <div class="w-8 h-8 bg-blue-100 dark:bg-blue-900 rounded-md flex items-center justify-center">
              <UIcon name="i-heroicons-currency-dollar" class="w-5 h-5 text-blue-600 dark:text-blue-400" />
            </div>
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-500 dark:text-gray-400">Available Credit</p>
            <p class="text-2xl font-semibold text-gray-900 dark:text-white">R{{ formatCurrency(availableCredit) }}</p>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <div class="w-8 h-8 bg-green-100 dark:bg-green-900 rounded-md flex items-center justify-center">
              <UIcon name="i-heroicons-banknotes" class="w-5 h-5 text-green-600 dark:text-green-400" />
            </div>
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-500 dark:text-gray-400">Your Contributions</p>
            <p class="text-2xl font-semibold text-gray-900 dark:text-white">R{{ formatCurrency(yourContributions) }}</p>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <div class="w-8 h-8 bg-yellow-100 dark:bg-yellow-900 rounded-md flex items-center justify-center">
              <UIcon name="i-heroicons-chart-pie" class="w-5 h-5 text-yellow-600 dark:text-yellow-400" />
            </div>
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-500 dark:text-gray-400">Active Loans</p>
            <p class="text-2xl font-semibold text-gray-900 dark:text-white">{{ activeLoans }}</p>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <div class="w-8 h-8 bg-purple-100 dark:bg-purple-900 rounded-md flex items-center justify-center">
              <UIcon name="i-heroicons-user-group" class="w-5 h-5 text-purple-600 dark:text-purple-400" />
            </div>
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-500 dark:text-gray-400">Pool Members</p>
            <p class="text-2xl font-semibold text-gray-900 dark:text-white">{{ totalMembers }}</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Your Credit Pools -->
    <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
      <h2 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">
        Your Credit Pools
      </h2>
      
      <div class="space-y-4">
        <div
          v-for="pool in creditPools"
          :key="pool.id"
          class="border border-gray-200 dark:border-gray-700 rounded-lg p-4"
        >
          <div class="flex items-start justify-between mb-3">
            <div>
              <h3 class="font-medium text-gray-900 dark:text-white">
                {{ pool.name }}
              </h3>
              <p class="text-sm text-gray-500 dark:text-gray-400">
                {{ pool.members }} members • {{ pool.interestRate }}% interest rate
              </p>
            </div>
            <UBadge
              :color="getPoolStatusColor(pool.status)"
              variant="soft"
            >
              {{ pool.status }}
            </UBadge>
          </div>
          
          <p class="text-sm text-gray-600 dark:text-gray-300 mb-4">
            {{ pool.description }}
          </p>
          
          <div class="grid grid-cols-2 md:grid-cols-4 gap-4 mb-4">
            <div>
              <span class="text-sm text-gray-500">Total Pool Size:</span>
              <p class="font-medium">R{{ formatCurrency(pool.totalSize) }}</p>
            </div>
            <div>
              <span class="text-sm text-gray-500">Available:</span>
              <p class="font-medium">R{{ formatCurrency(pool.available) }}</p>
            </div>
            <div>
              <span class="text-sm text-gray-500">Your Share:</span>
              <p class="font-medium">R{{ formatCurrency(pool.yourContribution) }}</p>
            </div>
            <div>
              <span class="text-sm text-gray-500">Max Loan:</span>
              <p class="font-medium">R{{ formatCurrency(pool.maxLoan) }}</p>
            </div>
          </div>
          
          <div class="mb-4">
            <div class="flex justify-between text-sm mb-1">
              <span>Pool Utilization</span>
              <span>{{ Math.round(((pool.totalSize - pool.available) / pool.totalSize) * 100) }}%</span>
            </div>
            <UProgress
              :value="((pool.totalSize - pool.available) / pool.totalSize) * 100"
              color="primary"
              size="sm"
            />
          </div>
          
          <div class="flex space-x-2">
            <UButton
              size="sm"
              @click="requestLoan(pool.id)"
              :disabled="pool.available <= 0"
            >
              Request Loan
            </UButton>
            <UButton
              size="sm"
              variant="outline"
              @click="viewPool(pool.id)"
            >
              View Details
            </UButton>
            <UButton
              size="sm"
              variant="ghost"
              @click="contribute(pool.id)"
            >
              Contribute More
            </UButton>
          </div>
        </div>
      </div>
    </div>

    <!-- Create Pool Modal -->
    <UModal v-model="showCreatePool">
      <UCard>
        <template #header>
          <h3 class="text-lg font-semibold">Create Credit Pool</h3>
        </template>

        <div class="space-y-4">
          <UFormGroup label="Pool Name">
            <UInput v-model="newPoolName" placeholder="e.g., Small Business Growth Fund" />
          </UFormGroup>

          <UFormGroup label="Description">
            <UTextarea v-model="newPoolDescription" placeholder="Describe the purpose of this credit pool..." />
          </UFormGroup>

          <div class="grid grid-cols-2 gap-4">
            <UFormGroup label="Initial Contribution (R)">
              <UInput v-model="newPoolContribution" type="number" placeholder="0.00" />
            </UFormGroup>

            <UFormGroup label="Interest Rate (%)">
              <UInput v-model="newPoolInterestRate" type="number" placeholder="10.5" />
            </UFormGroup>
          </div>
        </div>

        <template #footer>
          <div class="flex justify-end space-x-2">
            <UButton variant="ghost" @click="showCreatePool = false">Cancel</UButton>
            <UButton @click="createPool">Create Pool</UButton>
          </div>
        </template>
      </UCard>
    </UModal>

    <!-- Join Pool Modal -->
    <UModal v-model="showJoinPool">
      <UCard>
        <template #header>
          <h3 class="text-lg font-semibold">Join Credit Pool</h3>
        </template>

        <div class="space-y-4">
          <UFormGroup label="Pool Code">
            <UInput v-model="joinPoolCode" placeholder="Enter pool invitation code" />
          </UFormGroup>

          <UFormGroup label="Initial Contribution (R)">
            <UInput v-model="joinPoolContribution" type="number" placeholder="0.00" />
          </UFormGroup>
        </div>

        <template #footer>
          <div class="flex justify-end space-x-2">
            <UButton variant="ghost" @click="showJoinPool = false">Cancel</UButton>
            <UButton @click="joinPool">Join Pool</UButton>
          </div>
        </template>
      </UCard>
    </UModal>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'

// Page meta
definePageMeta({
  title: 'Pooled Credit',
  description: 'Collaborative credit pools and lending'
})

// Reactive data
const showCreatePool = ref(false)
const showJoinPool = ref(false)
const newPoolName = ref('')
const newPoolDescription = ref('')
const newPoolContribution = ref(null)
const newPoolInterestRate = ref(null)
const joinPoolCode = ref('')
const joinPoolContribution = ref(null)

// Mock data
const availableCredit = ref(125000)
const yourContributions = ref(50000)
const activeLoans = ref(3)
const totalMembers = ref(24)

const creditPools = ref([
  {
    id: 1,
    name: 'SME Growth Fund',
    description: 'Supporting small and medium enterprises with flexible credit solutions',
    members: 15,
    interestRate: 8.5,
    totalSize: 500000,
    available: 185000,
    yourContribution: 25000,
    maxLoan: 100000,
    status: 'Active'
  },
  {
    id: 2,
    name: 'Tech Startup Pool',
    description: 'Collaborative funding for technology startups and innovation projects',
    members: 9,
    interestRate: 12.0,
    totalSize: 200000,
    available: 45000,
    yourContribution: 25000,
    maxLoan: 50000,
    status: 'Active'
  }
])

// Methods
const formatCurrency = (amount: number): string => {
  return new Intl.NumberFormat('en-ZA', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  }).format(amount)
}

const getPoolStatusColor = (status: string): string => {
  const colors: Record<string, string> = {
    'Active': 'green',
    'Inactive': 'gray',
    'Full': 'yellow',
    'Closed': 'red'
  }
  return colors[status] || 'gray'
}

const requestLoan = (poolId: number) => {
  console.log('Requesting loan from pool:', poolId)
}

const viewPool = (poolId: number) => {
  console.log('Viewing pool:', poolId)
}

const contribute = (poolId: number) => {
  console.log('Contributing to pool:', poolId)
}

const createPool = () => {
  console.log('Creating pool:', {
    name: newPoolName.value,
    description: newPoolDescription.value,
    contribution: newPoolContribution.value,
    interestRate: newPoolInterestRate.value
  })
  showCreatePool.value = false
}

const joinPool = () => {
  console.log('Joining pool:', {
    code: joinPoolCode.value,
    contribution: joinPoolContribution.value
  })
  showJoinPool.value = false
}
</script>
