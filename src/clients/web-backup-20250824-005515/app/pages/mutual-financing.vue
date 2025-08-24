<template>
  <div class="space-y-6">
    <!-- Header -->
    <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
      <div class="flex items-center justify-between">
        <div>
          <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
            Mutual Financing
          </h1>
          <p class="mt-2 text-gray-600 dark:text-gray-300">
            Pool resources and provide mutual credit support within your business network
          </p>
        </div>
        <div class="flex space-x-2">
          <UButton
            variant="outline"
            icon="i-heroicons-users"
            @click="showJoinPool = true"
          >
            Join Pool
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

    <!-- Pool Statistics -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-6">
      <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <div class="w-8 h-8 bg-blue-100 dark:bg-blue-900 rounded-md flex items-center justify-center">
              <UIcon name="i-heroicons-building-office" class="w-5 h-5 text-blue-600 dark:text-blue-400" />
            </div>
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-500 dark:text-gray-400">Your Pools</p>
            <p class="text-2xl font-semibold text-gray-900 dark:text-white">{{ memberPools.length }}</p>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <div class="w-8 h-8 bg-green-100 dark:bg-green-900 rounded-md flex items-center justify-center">
              <UIcon name="i-heroicons-currency-dollar" class="w-5 h-5 text-green-600 dark:text-green-400" />
            </div>
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-500 dark:text-gray-400">Total Contributed</p>
            <p class="text-2xl font-semibold text-gray-900 dark:text-white">R{{ formatCurrency(totalContributed) }}</p>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <div class="w-8 h-8 bg-yellow-100 dark:bg-yellow-900 rounded-md flex items-center justify-center">
              <UIcon name="i-heroicons-banknotes" class="w-5 h-5 text-yellow-600 dark:text-yellow-400" />
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
            <div class="w-8 h-8 bg-purple-100 dark:bg-purple-900 rounded-md flex items-center justify-center">
              <UIcon name="i-heroicons-chart-pie" class="w-5 h-5 text-purple-600 dark:text-purple-400" />
            </div>
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-500 dark:text-gray-400">Active Loans</p>
            <p class="text-2xl font-semibold text-gray-900 dark:text-white">{{ activeLoans.length }}</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Your Financing Pools -->
    <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
      <h2 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">
        Your Financing Pools
      </h2>
      
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <div
          v-for="pool in memberPools"
          :key="pool.id"
          class="border border-gray-200 dark:border-gray-700 rounded-lg p-4"
        >
          <div class="flex items-start justify-between mb-3">
            <div>
              <h3 class="font-medium text-gray-900 dark:text-white">
                {{ pool.name }}
              </h3>
              <p class="text-sm text-gray-500 dark:text-gray-400">
                {{ pool.members }} members • Founded {{ formatDate(pool.foundedDate) }}
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
          
          <div class="grid grid-cols-2 gap-4 mb-4">
            <div>
              <span class="text-sm text-gray-500">Pool Size:</span>
              <p class="font-medium">R{{ formatCurrency(pool.totalPool) }}</p>
            </div>
            <div>
              <span class="text-sm text-gray-500">Your Share:</span>
              <p class="font-medium">R{{ formatCurrency(pool.yourContribution) }}</p>
            </div>
            <div>
              <span class="text-sm text-gray-500">Available:</span>
              <p class="font-medium">R{{ formatCurrency(pool.availableFunds) }}</p>
            </div>
            <div>
              <span class="text-sm text-gray-500">Interest Rate:</span>
              <p class="font-medium">{{ pool.interestRate }}% p.a.</p>
            </div>
          </div>
          
          <div class="mb-4">
            <div class="flex justify-between text-sm mb-1">
              <span>Pool Utilization</span>
              <span>{{ Math.round((pool.totalPool - pool.availableFunds) / pool.totalPool * 100) }}%</span>
            </div>
            <UProgress
              :value="(pool.totalPool - pool.availableFunds) / pool.totalPool * 100"
              color="primary"
              size="sm"
            />
          </div>
          
          <div class="flex space-x-2">
            <UButton
              size="sm"
              @click="requestLoan(pool.id)"
              :disabled="pool.availableFunds <= 0"
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
              Contribute
            </UButton>
          </div>
        </div>
      </div>
    </div>

    <!-- Active Loans -->
    <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
      <h2 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">
        Active Loans
      </h2>
      
      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
          <thead class="bg-gray-50 dark:bg-gray-700">
            <tr>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                Loan Details
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                Amount
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                Progress
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                Next Payment
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                Actions
              </th>
            </tr>
          </thead>
          <tbody class="bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700">
            <tr
              v-for="loan in activeLoans"
              :key="loan.id"
            >
              <td class="px-6 py-4 whitespace-nowrap">
                <div>
                  <div class="text-sm font-medium text-gray-900 dark:text-white">
                    {{ loan.purpose }}
                  </div>
                  <div class="text-sm text-gray-500 dark:text-gray-400">
                    From {{ loan.poolName }} • {{ loan.term }} months @ {{ loan.interestRate }}%
                  </div>
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm text-gray-900 dark:text-white">
                  R{{ formatCurrency(loan.originalAmount) }}
                </div>
                <div class="text-sm text-gray-500 dark:text-gray-400">
                  R{{ formatCurrency(loan.remainingBalance) }} remaining
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm text-gray-900 dark:text-white mb-1">
                  {{ loan.paidPayments }}/{{ loan.totalPayments }} payments
                </div>
                <UProgress
                  :value="(loan.paidPayments / loan.totalPayments) * 100"
                  color="green"
                  size="sm"
                />
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm text-gray-900 dark:text-white">
                  R{{ formatCurrency(loan.monthlyPayment) }}
                </div>
                <div class="text-sm text-gray-500 dark:text-gray-400">
                  Due {{ formatDate(loan.nextPaymentDate) }}
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                <UDropdown :items="getLoanActions(loan.id)">
                  <UButton variant="ghost" icon="i-heroicons-ellipsis-vertical" />
                </UDropdown>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Available Pools to Join -->
    <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
      <h2 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">
        Available Pools to Join
      </h2>
      
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <div
          v-for="pool in availablePools"
          :key="pool.id"
          class="border border-gray-200 dark:border-gray-700 rounded-lg p-4"
        >
          <div class="flex items-start justify-between mb-3">
            <div>
              <h3 class="font-medium text-gray-900 dark:text-white">
                {{ pool.name }}
              </h3>
              <p class="text-sm text-gray-500 dark:text-gray-400">
                {{ pool.members }}/{{ pool.maxMembers }} members
              </p>
            </div>
            <UBadge
              color="green"
              variant="soft"
            >
              Open
            </UBadge>
          </div>
          
          <p class="text-sm text-gray-600 dark:text-gray-300 mb-4">
            {{ pool.description }}
          </p>
          
          <div class="grid grid-cols-2 gap-4 mb-4">
            <div>
              <span class="text-sm text-gray-500">Min. Contribution:</span>
              <p class="font-medium">R{{ formatCurrency(pool.minContribution) }}</p>
            </div>
            <div>
              <span class="text-sm text-gray-500">Max. Loan:</span>
              <p class="font-medium">R{{ formatCurrency(pool.maxLoanAmount) }}</p>
            </div>
            <div>
              <span class="text-sm text-gray-500">Interest Rate:</span>
              <p class="font-medium">{{ pool.interestRate }}% p.a.</p>
            </div>
            <div>
              <span class="text-sm text-gray-500">Loan Term:</span>
              <p class="font-medium">{{ pool.maxLoanTerm }} months</p>
            </div>
          </div>
          
          <UButton
            size="sm"
            @click="joinPool(pool.id)"
            class="w-full"
          >
            Request to Join
          </UButton>
        </div>
      </div>
    </div>

    <!-- Create Pool Modal -->
    <UModal v-model="showCreatePool" :ui="{ width: 'sm:max-w-2xl' }">
      <UCard>
        <template #header>
          <div class="flex items-center justify-between">
            <h3 class="text-lg font-semibold">Create Financing Pool</h3>
          </div>
        </template>

        <UForm
          :schema="poolSchema"
          :state="newPool"
          @submit="createPool"
        >
          <div class="space-y-4">
            <UFormGroup label="Pool Name" name="name">
              <UInput v-model="newPool.name" placeholder="e.g., SME Growth Fund" />
            </UFormGroup>

            <UFormGroup label="Description" name="description">
              <UTextarea
                v-model="newPool.description"
                placeholder="Describe the purpose and goals of this financing pool..."
                rows="3"
              />
            </UFormGroup>

            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <UFormGroup label="Minimum Contribution (R)" name="minContribution">
                <UInput
                  v-model="newPool.minContribution"
                  type="number"
                  step="0.01"
                  placeholder="0.00"
                />
              </UFormGroup>

              <UFormGroup label="Maximum Members" name="maxMembers">
                <UInput
                  v-model="newPool.maxMembers"
                  type="number"
                  placeholder="e.g., 20"
                />
              </UFormGroup>
            </div>

            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <UFormGroup label="Interest Rate (%)" name="interestRate">
                <UInput
                  v-model="newPool.interestRate"
                  type="number"
                  step="0.1"
                  placeholder="e.g., 12.5"
                />
              </UFormGroup>

              <UFormGroup label="Max Loan Term (months)" name="maxLoanTerm">
                <UInput
                  v-model="newPool.maxLoanTerm"
                  type="number"
                  placeholder="e.g., 24"
                />
              </UFormGroup>
            </div>

            <UFormGroup label="Pool Rules & Requirements" name="rules">
              <UTextarea
                v-model="newPool.rules"
                placeholder="Outline membership requirements, loan approval criteria, and pool governance..."
                rows="4"
              />
            </UFormGroup>

            <UFormGroup label="Initial Contribution (R)" name="initialContribution">
              <UInput
                v-model="newPool.initialContribution"
                type="number"
                step="0.01"
                placeholder="Your initial contribution to start the pool"
              />
            </UFormGroup>
          </div>

          <template #footer>
            <div class="flex justify-end space-x-2">
              <UButton
                variant="ghost"
                @click="showCreatePool = false"
              >
                Cancel
              </UButton>
              <UButton type="submit">
                Create Pool
              </UButton>
            </div>
          </template>
        </UForm>
      </UCard>
    </UModal>

    <!-- Join Pool Modal -->
    <UModal v-model="showJoinPool" :ui="{ width: 'sm:max-w-lg' }">
      <UCard>
        <template #header>
          <div class="flex items-center justify-between">
            <h3 class="text-lg font-semibold">Join Financing Pool</h3>
          </div>
        </template>

        <div class="space-y-4">
          <UFormGroup label="Pool Code or Name">
            <UInput v-model="joinPoolCode" placeholder="Enter pool invitation code or search by name" />
          </UFormGroup>

          <UFormGroup label="Initial Contribution (R)">
            <UInput
              v-model="joinContribution"
              type="number"
              step="0.01"
              placeholder="0.00"
            />
          </UFormGroup>
        </div>

        <template #footer>
          <div class="flex justify-end space-x-2">
            <UButton
              variant="ghost"
              @click="showJoinPool = false"
            >
              Cancel
            </UButton>
            <UButton @click="submitJoinRequest">
              Submit Request
            </UButton>
          </div>
        </template>
      </UCard>
    </UModal>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { z } from 'zod'

// Page meta
definePageMeta({
  title: 'Mutual Financing',
  description: 'Collaborative financing and credit pools'
})

// Reactive data
const showCreatePool = ref(false)
const showJoinPool = ref(false)
const joinPoolCode = ref('')
const joinContribution = ref(null)

// Mock data
const totalContributed = ref(75000)
const availableCredit = ref(150000)

const memberPools = ref([
  {
    id: 1,
    name: 'SME Growth Fund',
    description: 'Supporting small business growth through collaborative financing',
    members: 12,
    totalPool: 500000,
    yourContribution: 25000,
    availableFunds: 180000,
    interestRate: 10.5,
    status: 'Active',
    foundedDate: '2024-08-15'
  },
  {
    id: 2,
    name: 'Tech Startup Pool',
    description: 'Mutual financing for technology startups and innovation projects',
    members: 8,
    totalPool: 200000,
    yourContribution: 50000,
    availableFunds: 45000,
    interestRate: 12.0,
    status: 'Active',
    foundedDate: '2024-11-01'
  }
])

const activeLoans = ref([
  {
    id: 1,
    purpose: 'Equipment Purchase',
    poolName: 'SME Growth Fund',
    originalAmount: 45000,
    remainingBalance: 32000,
    monthlyPayment: 2850,
    interestRate: 10.5,
    term: 18,
    paidPayments: 6,
    totalPayments: 18,
    nextPaymentDate: '2025-02-15'
  }
])

const availablePools = ref([
  {
    id: 3,
    name: 'Local Business Network',
    description: 'Community-focused financing for local businesses',
    members: 15,
    maxMembers: 25,
    minContribution: 10000,
    maxLoanAmount: 100000,
    interestRate: 9.5,
    maxLoanTerm: 36
  },
  {
    id: 4,
    name: 'Green Energy Fund',
    description: 'Financing sustainable and renewable energy projects',
    members: 6,
    maxMembers: 15,
    minContribution: 20000,
    maxLoanAmount: 250000,
    interestRate: 8.5,
    maxLoanTerm: 60
  }
])

// Form data
const newPool = ref({
  name: '',
  description: '',
  minContribution: null,
  maxMembers: null,
  interestRate: null,
  maxLoanTerm: null,
  rules: '',
  initialContribution: null
})

// Form schema
const poolSchema = z.object({
  name: z.string().min(1, 'Pool name is required'),
  description: z.string().min(1, 'Description is required'),
  minContribution: z.number().min(1, 'Minimum contribution must be greater than 0'),
  maxMembers: z.number().min(2, 'Must allow at least 2 members'),
  interestRate: z.number().min(0, 'Interest rate must be non-negative'),
  maxLoanTerm: z.number().min(1, 'Loan term must be at least 1 month'),
  rules: z.string().min(1, 'Pool rules are required'),
  initialContribution: z.number().min(1, 'Initial contribution is required')
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
  // Navigate to loan request form
}

const viewPool = (poolId: number) => {
  console.log('Viewing pool details:', poolId)
  // Navigate to pool details page
}

const contribute = (poolId: number) => {
  console.log('Contributing to pool:', poolId)
  // Open contribution modal
}

const joinPool = (poolId: number) => {
  console.log('Joining pool:', poolId)
  // Open join pool form
}

const getLoanActions = (loanId: number) => {
  return [
    [{
      label: 'Make Payment',
      icon: 'i-heroicons-credit-card',
      click: () => makePayment(loanId)
    }],
    [{
      label: 'View Schedule',
      icon: 'i-heroicons-calendar',
      click: () => viewPaymentSchedule(loanId)
    }],
    [{
      label: 'Request Extension',
      icon: 'i-heroicons-clock',
      click: () => requestExtension(loanId)
    }]
  ]
}

const makePayment = (loanId: number) => {
  console.log('Making payment for loan:', loanId)
}

const viewPaymentSchedule = (loanId: number) => {
  console.log('Viewing payment schedule for loan:', loanId)
}

const requestExtension = (loanId: number) => {
  console.log('Requesting extension for loan:', loanId)
}

const createPool = async (data: any) => {
  try {
    console.log('Creating financing pool:', data)
    
    // Simulate API call
    await new Promise(resolve => setTimeout(resolve, 1000))
    
    // Reset form and close modal
    newPool.value = {
      name: '',
      description: '',
      minContribution: null,
      maxMembers: null,
      interestRate: null,
      maxLoanTerm: null,
      rules: '',
      initialContribution: null
    }
    showCreatePool.value = false
    
    // Show success message
    // useToast().add({
    //   title: 'Pool Created',
    //   description: 'Your financing pool has been created successfully.',
    //   color: 'green'
    // })
  } catch (error) {
    console.error('Error creating pool:', error)
  }
}

const submitJoinRequest = async () => {
  try {
    console.log('Submitting join request:', { code: joinPoolCode.value, contribution: joinContribution.value })
    
    // Simulate API call
    await new Promise(resolve => setTimeout(resolve, 1000))
    
    // Reset form and close modal
    joinPoolCode.value = ''
    joinContribution.value = null
    showJoinPool.value = false
    
    // Show success message
    // useToast().add({
    //   title: 'Join Request Submitted',
    //   description: 'Your request to join the pool has been submitted for approval.',
    //   color: 'green'
    // })
  } catch (error) {
    console.error('Error submitting join request:', error)
  }
}
</script>
