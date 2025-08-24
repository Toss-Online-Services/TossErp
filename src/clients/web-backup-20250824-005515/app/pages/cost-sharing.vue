<template>
  <div class="space-y-6">
    <!-- Header -->
    <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
      <div class="flex items-center justify-between">
        <div>
          <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
            Cost Sharing
          </h1>
          <p class="mt-2 text-gray-600 dark:text-gray-300">
            Split expenses and costs with other businesses in your network
          </p>
        </div>
        <UButton
          icon="i-heroicons-plus"
          @click="showCreateExpense = true"
        >
          Create Shared Expense
        </UButton>
      </div>
    </div>

    <!-- Quick Stats -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-6">
      <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <div class="w-8 h-8 bg-green-100 dark:bg-green-900 rounded-md flex items-center justify-center">
              <UIcon name="i-heroicons-currency-dollar" class="w-5 h-5 text-green-600 dark:text-green-400" />
            </div>
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-500 dark:text-gray-400">Total Shared</p>
            <p class="text-2xl font-semibold text-gray-900 dark:text-white">R{{ formatCurrency(totalShared) }}</p>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <div class="w-8 h-8 bg-blue-100 dark:bg-blue-900 rounded-md flex items-center justify-center">
              <UIcon name="i-heroicons-credit-card" class="w-5 h-5 text-blue-600 dark:text-blue-400" />
            </div>
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-500 dark:text-gray-400">You Owe</p>
            <p class="text-2xl font-semibold text-gray-900 dark:text-white">R{{ formatCurrency(youOwe) }}</p>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <div class="w-8 h-8 bg-yellow-100 dark:bg-yellow-900 rounded-md flex items-center justify-center">
              <UIcon name="i-heroicons-arrow-down-left" class="w-5 h-5 text-yellow-600 dark:text-yellow-400" />
            </div>
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-500 dark:text-gray-400">Owed to You</p>
            <p class="text-2xl font-semibold text-gray-900 dark:text-white">R{{ formatCurrency(owedToYou) }}</p>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <div class="w-8 h-8 bg-purple-100 dark:bg-purple-900 rounded-md flex items-center justify-center">
              <UIcon name="i-heroicons-calculator" class="w-5 h-5 text-purple-600 dark:text-purple-400" />
            </div>
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-500 dark:text-gray-400">Active Expenses</p>
            <p class="text-2xl font-semibold text-gray-900 dark:text-white">{{ activeExpenses.length }}</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Active Shared Expenses -->
    <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
      <h2 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">
        Active Shared Expenses
      </h2>
      
      <div class="space-y-4">
        <div
          v-for="expense in activeExpenses"
          :key="expense.id"
          class="border border-gray-200 dark:border-gray-700 rounded-lg p-4"
        >
          <div class="flex items-start justify-between mb-3">
            <div>
              <h3 class="font-medium text-gray-900 dark:text-white">
                {{ expense.title }}
              </h3>
              <p class="text-sm text-gray-500 dark:text-gray-400">
                Created by {{ expense.createdBy }} • {{ formatDate(expense.createdAt) }}
              </p>
            </div>
            <UBadge
              :color="getStatusColor(expense.status)"
              variant="soft"
            >
              {{ expense.status }}
            </UBadge>
          </div>
          
          <p class="text-sm text-gray-600 dark:text-gray-300 mb-3">
            {{ expense.description }}
          </p>
          
          <div class="grid grid-cols-1 md:grid-cols-3 gap-4 mb-4">
            <div>
              <span class="text-sm text-gray-500">Total Amount:</span>
              <p class="font-medium">R{{ formatCurrency(expense.totalAmount) }}</p>
            </div>
            <div>
              <span class="text-sm text-gray-500">Your Share:</span>
              <p class="font-medium">R{{ formatCurrency(expense.yourShare) }}</p>
            </div>
            <div>
              <span class="text-sm text-gray-500">Participants:</span>
              <p class="font-medium">{{ expense.participants.length }} members</p>
            </div>
          </div>
          
          <!-- Participants breakdown -->
          <div class="bg-gray-50 dark:bg-gray-700 rounded-lg p-3 mb-3">
            <h4 class="font-medium text-gray-900 dark:text-white mb-2">Expense Breakdown</h4>
            <div class="grid grid-cols-1 md:grid-cols-2 gap-2">
              <div
                v-for="participant in expense.participants"
                :key="participant.id"
                class="flex justify-between text-sm"
              >
                <span class="text-gray-600 dark:text-gray-300">{{ participant.name }}:</span>
                <span class="font-medium">R{{ formatCurrency(participant.amount) }}</span>
              </div>
            </div>
          </div>
          
          <div class="flex space-x-2">
            <UButton
              size="sm"
              @click="viewExpense(expense.id)"
            >
              View Details
            </UButton>
            <UButton
              v-if="expense.status === 'Pending Payment'"
              size="sm"
              variant="outline"
              @click="makePayment(expense.id)"
            >
              Pay Your Share
            </UButton>
            <UButton
              size="sm"
              variant="ghost"
              icon="i-heroicons-chat-bubble-left-right"
              @click="openExpenseChat(expense.id)"
            >
              Chat
            </UButton>
          </div>
        </div>
      </div>
    </div>

    <!-- Settlement Summary -->
    <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
      <h2 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">
        Settlement Summary
      </h2>
      
      <div class="space-y-4">
        <div
          v-for="settlement in settlements"
          :key="settlement.id"
          class="flex items-center justify-between p-3 bg-gray-50 dark:bg-gray-700 rounded-lg"
        >
          <div class="flex items-center">
            <div class="flex-shrink-0">
              <div
                :class="[
                  'w-3 h-3 rounded-full',
                  settlement.type === 'owe' ? 'bg-red-400' : 'bg-green-400'
                ]"
              />
            </div>
            <div class="ml-3">
              <p class="text-sm font-medium text-gray-900 dark:text-white">
                {{ settlement.type === 'owe' ? 'You owe' : 'Owes you' }}
                {{ settlement.companyName }}
              </p>
              <p class="text-xs text-gray-500 dark:text-gray-400">
                {{ settlement.expenseCount }} expenses
              </p>
            </div>
          </div>
          <div class="flex items-center space-x-3">
            <span class="font-medium text-gray-900 dark:text-white">
              R{{ formatCurrency(settlement.amount) }}
            </span>
            <UButton
              size="sm"
              :variant="settlement.type === 'owe' ? 'solid' : 'outline'"
              @click="settleAmount(settlement.id)"
            >
              {{ settlement.type === 'owe' ? 'Pay' : 'Request' }}
            </UButton>
          </div>
        </div>
      </div>
    </div>

    <!-- Create Expense Modal -->
    <UModal v-model="showCreateExpense" :ui="{ width: 'sm:max-w-2xl' }">
      <UCard>
        <template #header>
          <div class="flex items-center justify-between">
            <h3 class="text-lg font-semibold">Create Shared Expense</h3>
          </div>
        </template>

        <UForm
          :schema="expenseSchema"
          :state="newExpense"
          @submit="createExpense"
        >
          <div class="space-y-4">
            <UFormGroup label="Expense Title" name="title">
              <UInput v-model="newExpense.title" placeholder="e.g., Office Rent - January 2025" />
            </UFormGroup>

            <UFormGroup label="Description" name="description">
              <UTextarea
                v-model="newExpense.description"
                placeholder="Describe what this expense is for..."
                rows="3"
              />
            </UFormGroup>

            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <UFormGroup label="Total Amount (R)" name="totalAmount">
                <UInput
                  v-model="newExpense.totalAmount"
                  type="number"
                  step="0.01"
                  placeholder="0.00"
                />
              </UFormGroup>

              <UFormGroup label="Category" name="category">
                <USelectMenu
                  v-model="newExpense.category"
                  :options="expenseCategories"
                  placeholder="Select category"
                />
              </UFormGroup>
            </div>

            <UFormGroup label="Split Method" name="splitMethod">
              <USelectMenu
                v-model="newExpense.splitMethod"
                :options="splitMethods"
                placeholder="How to split the expense"
              />
            </UFormGroup>

            <UFormGroup label="Participants" name="participants">
              <div class="space-y-2">
                <div
                  v-for="(participant, index) in newExpense.participants"
                  :key="index"
                  class="flex items-center space-x-2"
                >
                  <UInput
                    v-model="participant.email"
                    placeholder="Business email"
                    class="flex-1"
                  />
                  <UInput
                    v-if="newExpense.splitMethod === 'custom'"
                    v-model="participant.amount"
                    type="number"
                    step="0.01"
                    placeholder="Amount"
                    class="w-32"
                  />
                  <UButton
                    size="sm"
                    variant="ghost"
                    icon="i-heroicons-trash"
                    @click="removeParticipant(index)"
                  />
                </div>
                <UButton
                  size="sm"
                  variant="outline"
                  @click="addParticipant"
                >
                  Add Participant
                </UButton>
              </div>
            </UFormGroup>

            <UFormGroup label="Due Date" name="dueDate">
              <UInput
                v-model="newExpense.dueDate"
                type="date"
              />
            </UFormGroup>

            <UFormGroup label="Receipt/Invoice" name="receipt">
              <UInput
                type="file"
                accept="image/*,.pdf"
                @change="handleReceiptUpload"
              />
              <p class="text-sm text-gray-500 mt-1">Upload receipt or invoice for transparency</p>
            </UFormGroup>
          </div>

          <template #footer>
            <div class="flex justify-end space-x-2">
              <UButton
                variant="ghost"
                @click="showCreateExpense = false"
              >
                Cancel
              </UButton>
              <UButton type="submit">
                Create Expense
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
  title: 'Cost Sharing',
  description: 'Split expenses and costs collaboratively'
})

// Reactive data
const showCreateExpense = ref(false)

// Mock data
const totalShared = ref(125000)
const youOwe = ref(8750)
const owedToYou = ref(4200)

const activeExpenses = ref([
  {
    id: 1,
    title: 'Office Rent - February 2025',
    description: 'Shared office space rental for co-working arrangement',
    totalAmount: 15000,
    yourShare: 3750,
    createdBy: 'ABC Corp',
    createdAt: '2025-02-01',
    status: 'Pending Payment',
    participants: [
      { id: 1, name: 'ABC Corp', amount: 7500 },
      { id: 2, name: 'Your Company', amount: 3750 },
      { id: 3, name: 'XYZ Ltd', amount: 3750 }
    ]
  },
  {
    id: 2,
    title: 'Shared Internet & Utilities',
    description: 'Monthly internet and utility bills for shared office',
    totalAmount: 2400,
    yourShare: 800,
    createdBy: 'Your Company',
    createdAt: '2025-01-15',
    status: 'Settled',
    participants: [
      { id: 1, name: 'Your Company', amount: 800 },
      { id: 2, name: 'Partner Co', amount: 800 },
      { id: 3, name: 'Tech Solutions', amount: 800 }
    ]
  }
])

const settlements = ref([
  {
    id: 1,
    type: 'owe',
    companyName: 'ABC Corp',
    amount: 3750,
    expenseCount: 2
  },
  {
    id: 2,
    type: 'owed',
    companyName: 'Partner Co',
    amount: 1200,
    expenseCount: 1
  }
])

const expenseCategories = [
  'Office Rent',
  'Utilities',
  'Equipment',
  'Software Licenses',
  'Marketing',
  'Travel',
  'Meals',
  'Other'
]

const splitMethods = [
  { label: 'Equal Split', value: 'equal' },
  { label: 'Custom Amounts', value: 'custom' },
  { label: 'Percentage Based', value: 'percentage' }
]

// Form data
const newExpense = ref({
  title: '',
  description: '',
  totalAmount: null,
  category: '',
  splitMethod: 'equal',
  participants: [{ email: '', amount: null }],
  dueDate: '',
  receipt: null
})

// Form schema
const expenseSchema = z.object({
  title: z.string().min(1, 'Title is required'),
  description: z.string().min(1, 'Description is required'),
  totalAmount: z.number().min(0.01, 'Amount must be greater than 0'),
  category: z.string().min(1, 'Category is required'),
  splitMethod: z.string().min(1, 'Split method is required'),
  dueDate: z.string().min(1, 'Due date is required')
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
    'Pending Payment': 'yellow',
    'Settled': 'green',
    'Overdue': 'red',
    'Cancelled': 'gray'
  }
  return colors[status] || 'gray'
}

const viewExpense = (id: number) => {
  console.log('Viewing expense:', id)
  // Navigate to expense details
}

const makePayment = (id: number) => {
  console.log('Making payment for expense:', id)
  // Open payment modal or navigate to payment page
}

const openExpenseChat = (id: number) => {
  console.log('Opening chat for expense:', id)
  // Open chat modal
}

const settleAmount = (id: number) => {
  console.log('Settling amount:', id)
  // Handle settlement
}

const addParticipant = () => {
  newExpense.value.participants.push({ email: '', amount: null })
}

const removeParticipant = (index: number) => {
  if (newExpense.value.participants.length > 1) {
    newExpense.value.participants.splice(index, 1)
  }
}

const handleReceiptUpload = (event: Event) => {
  const file = (event.target as HTMLInputElement).files?.[0]
  if (file) {
    newExpense.value.receipt = file
  }
}

const createExpense = async (data: any) => {
  try {
    console.log('Creating shared expense:', data)
    
    // Simulate API call
    await new Promise(resolve => setTimeout(resolve, 1000))
    
    // Reset form and close modal
    newExpense.value = {
      title: '',
      description: '',
      totalAmount: null,
      category: '',
      splitMethod: 'equal',
      participants: [{ email: '', amount: null }],
      dueDate: '',
      receipt: null
    }
    showCreateExpense.value = false
    
    // Show success message
    // useToast().add({
    //   title: 'Expense Created',
    //   description: 'Your shared expense has been created and participants will be notified.',
    //   color: 'green'
    // })
  } catch (error) {
    console.error('Error creating expense:', error)
  }
}
</script>
