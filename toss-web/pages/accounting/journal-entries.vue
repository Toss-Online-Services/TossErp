<template>
  <div class="space-y-6">
    <PageHeader
      title="Journal Entries"
      description="Record and manage general ledger journal entries"
    />

    <div class="flex justify-between items-center">
      <div class="flex gap-4">
        <input
          v-model="searchQuery"
          type="text"
          placeholder="Search entries..."
          class="px-4 py-2 border rounded-lg dark:bg-gray-700 dark:text-white"
        />
        <input
          v-model="filterDate"
          type="date"
          class="px-4 py-2 border rounded-lg dark:bg-gray-700 dark:text-white"
        />
        <select v-model="filterStatus" class="px-4 py-2 border rounded-lg dark:bg-gray-700 dark:text-white">
          <option value="">All Status</option>
          <option value="Draft">Draft</option>
          <option value="Posted">Posted</option>
          <option value="Cancelled">Cancelled</option>
        </select>
      </div>
      <div class="flex gap-2">
        <button @click="exportEntries" class="px-4 py-2 border border-gray-300 dark:border-gray-600 text-gray-700 dark:text-gray-300 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700">
          Export CSV
        </button>
        <button @click="openCreateModal" class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700">
          New Entry
        </button>
      </div>
    </div>

    <div class="bg-white dark:bg-gray-800 rounded-lg shadow">
      <table class="w-full">
        <thead class="bg-gray-50 dark:bg-gray-700">
          <tr>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Entry #</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Date</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Reference</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Description</th>
            <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Debit</th>
            <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Credit</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Status</th>
            <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Actions</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-gray-200 dark:divide-gray-700">
          <tr v-for="entry in filteredEntries" :key="entry.id" class="dark:text-white hover:bg-gray-50 dark:hover:bg-gray-700">
            <td class="px-6 py-4 font-mono text-sm">{{ entry.entryNumber }}</td>
            <td class="px-6 py-4 text-sm">{{ formatDate(entry.date) }}</td>
            <td class="px-6 py-4 text-sm">{{ entry.reference }}</td>
            <td class="px-6 py-4">
              <div>
                <p class="font-medium">{{ entry.description }}</p>
                <p class="text-xs text-gray-500 dark:text-gray-400">{{ entry.lineItems }} line items</p>
              </div>
            </td>
            <td class="px-6 py-4 text-right font-medium">{{ formatCurrency(entry.totalDebit) }}</td>
            <td class="px-6 py-4 text-right font-medium">{{ formatCurrency(entry.totalCredit) }}</td>
            <td class="px-6 py-4">
              <span :class="getStatusClass(entry.status)" class="px-2 py-1 text-xs rounded-full">
                {{ entry.status }}
              </span>
            </td>
            <td class="px-6 py-4 text-right">
              <button @click="viewEntry(entry)" class="text-blue-600 dark:text-blue-400 hover:text-blue-800 mr-3">View</button>
              <button v-if="entry.status === 'Draft'" @click="editEntry(entry)" class="text-purple-600 dark:text-purple-400 hover:text-purple-800 mr-3">Edit</button>
              <button v-if="entry.status === 'Draft'" @click="postEntry(entry)" class="text-green-600 dark:text-green-400 hover:text-green-800 mr-3">Post</button>
              <button v-if="entry.status === 'Posted'" @click="reverseEntry(entry)" class="text-orange-600 dark:text-orange-400 hover:text-orange-800 mr-3">Reverse</button>
              <button @click="deleteEntry(entry)" class="text-red-600 dark:text-red-400 hover:text-red-800">Delete</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <div class="bg-blue-50 dark:bg-blue-900/20 rounded-lg p-4">
      <div class="flex justify-between items-center">
        <div>
          <p class="text-sm text-gray-600 dark:text-gray-400">Total Debits</p>
          <p class="text-xl font-bold text-gray-900 dark:text-white">{{ formatCurrency(totalDebits) }}</p>
        </div>
        <div>
          <p class="text-sm text-gray-600 dark:text-gray-400">Total Credits</p>
          <p class="text-xl font-bold text-gray-900 dark:text-white">{{ formatCurrency(totalCredits) }}</p>
        </div>
        <div>
          <p class="text-sm text-gray-600 dark:text-gray-400">Difference</p>
          <p class="text-xl font-bold" :class="difference === 0 ? 'text-green-600' : 'text-red-600'">
            {{ formatCurrency(difference) }}
          </p>
        </div>
      </div>
    </div>

    <!-- Journal Entry Modal -->
    <JournalEntryModal
      v-if="showModal"
      :entry="selectedEntry"
      :accounts="accountsList"
      @close="closeModal"
      @save="saveEntry"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useAccounting } from '~/composables/useAccounting'
import type { JournalEntry, Account } from '~/composables/useAccounting'
import JournalEntryModal from '~/components/accounting/JournalEntryModal.vue'

definePageMeta({
  layout: 'default'
})

useHead({
  title: 'Journal Entries - TOSS ERP',
})

const { getJournalEntries, getAccounts, createJournalEntry, postJournalEntry, reverseJournalEntry, isLoading } = useAccounting()

const searchQuery = ref('')
const filterDate = ref('')
const filterStatus = ref('')
const entries = ref<JournalEntry[]>([])
const accountsList = ref<Account[]>([])
const showModal = ref(false)
const selectedEntry = ref<JournalEntry | null>(null)

const filteredEntries = computed(() => {
  let result = entries.value

  if (filterStatus.value) {
    result = result.filter(e => e.status === filterStatus.value)
  }

  if (filterDate.value) {
    result = result.filter(e => e.date === filterDate.value)
  }

  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    result = result.filter(e => 
      e.entryNumber.toLowerCase().includes(query) ||
      e.description.toLowerCase().includes(query) ||
      e.reference.toLowerCase().includes(query)
    )
  }

  return result
})

const totalDebits = computed(() => filteredEntries.value.reduce((sum, e) => sum + e.totalDebit, 0))
const totalCredits = computed(() => filteredEntries.value.reduce((sum, e) => sum + e.totalCredit, 0))
const difference = computed(() => totalDebits.value - totalCredits.value)

const getStatusClass = (status: string) => {
  const classes = {
    'Posted': 'bg-green-100 text-green-800',
    'Draft': 'bg-yellow-100 text-yellow-800',
    'Cancelled': 'bg-red-100 text-red-800',
  }
  return classes[status as keyof typeof classes] || 'bg-gray-100 text-gray-800'
}

const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-ZA', { style: 'currency', currency: 'ZAR' }).format(amount)
}

const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString('en-ZA')
}

// Methods
const loadEntries = async () => {
  try {
    entries.value = await getJournalEntries()
  } catch (error) {
    console.error('Error loading journal entries:', error)
    alert('Failed to load journal entries. Please try again.')
  }
}

const loadAccounts = async () => {
  try {
    accountsList.value = await getAccounts()
  } catch (error) {
    console.error('Error loading accounts:', error)
  }
}

const openCreateModal = () => {
  selectedEntry.value = null
  showModal.value = true
}

const editEntry = (entry: JournalEntry) => {
  if (entry.status !== 'Draft') {
    alert('Only draft entries can be edited')
    return
  }
  selectedEntry.value = entry
  showModal.value = true
}

const viewEntry = (entry: JournalEntry) => {
  const lineItemsText = entry.lineItems.map((item, i) => 
    `${i + 1}. ${item.accountCode} - ${item.accountName}\n   Debit: R${item.debit.toFixed(2)} | Credit: R${item.credit.toFixed(2)}`
  ).join('\n')

  const details = `
Journal Entry: ${entry.entryNumber}
Date: ${formatDate(entry.date)}
Reference: ${entry.reference}
Status: ${entry.status}
${entry.postedBy ? `Posted by: ${entry.postedBy} at ${new Date(entry.postedAt!).toLocaleString('en-ZA')}` : ''}

Description: ${entry.description}

Line Items:
${lineItemsText}

Total Debit: R${entry.totalDebit.toFixed(2)}
Total Credit: R${entry.totalCredit.toFixed(2)}
  `
  alert(details)
}

const postEntry = async (entry: JournalEntry) => {
  if (confirm(`Post journal entry ${entry.entryNumber}? This will update the general ledger.`)) {
    try {
      await postJournalEntry(entry.id)
      await loadEntries()
      alert(`Journal entry ${entry.entryNumber} posted successfully!`)
    } catch (error) {
      console.error('Error posting entry:', error)
      alert('Failed to post journal entry. Please try again.')
    }
  }
}

const reverseEntry = async (entry: JournalEntry) => {
  const reversalDate = prompt(`Enter reversal date (YYYY-MM-DD) for entry ${entry.entryNumber}:`, new Date().toISOString().split('T')[0])
  
  if (reversalDate) {
    try {
      await reverseJournalEntry(entry.id, reversalDate)
      await loadEntries()
      alert(`Journal entry ${entry.entryNumber} reversed successfully! Reversal entry created.`)
    } catch (error) {
      console.error('Error reversing entry:', error)
      alert('Failed to reverse journal entry. Please try again.')
    }
  }
}

const deleteEntry = async (entry: JournalEntry) => {
  if (entry.status === 'Posted') {
    alert('Cannot delete a posted entry. Please reverse it instead.')
    return
  }

  if (confirm(`Delete journal entry ${entry.entryNumber}?`)) {
    try {
      const index = entries.value.findIndex(e => e.id === entry.id)
      if (index > -1) {
        entries.value.splice(index, 1)
        alert(`Journal entry ${entry.entryNumber} deleted successfully!`)
      }
    } catch (error) {
      console.error('Error deleting entry:', error)
      alert('Failed to delete journal entry. Please try again.')
    }
  }
}

const closeModal = () => {
  showModal.value = false
  selectedEntry.value = null
}

const saveEntry = async (data: any) => {
  try {
    if (selectedEntry.value) {
      // Update existing entry (only if Draft)
      const index = entries.value.findIndex(e => e.id === selectedEntry.value!.id)
      if (index > -1) {
        const totalDebit = data.lineItems.reduce((sum: number, item: any) => sum + item.debit, 0)
        const totalCredit = data.lineItems.reduce((sum: number, item: any) => sum + item.credit, 0)
        
        entries.value[index] = {
          ...entries.value[index],
          date: data.date,
          reference: data.reference,
          description: data.description,
          lineItems: data.lineItems,
          totalDebit,
          totalCredit
        }
        alert('Journal entry updated successfully!')
      }
    } else {
      // Create new entry
      const newEntry = await createJournalEntry(data)
      entries.value.unshift(newEntry)
      alert('Journal entry created successfully!')
    }
    closeModal()
  } catch (error) {
    console.error('Error saving entry:', error)
    alert('Failed to save journal entry. Please try again.')
  }
}

const exportEntries = () => {
  const exportData = filteredEntries.value.map(entry => ({
    'Entry Number': entry.entryNumber,
    'Date': formatDate(entry.date),
    'Reference': entry.reference,
    'Description': entry.description,
    'Line Items': entry.lineItems.length,
    'Total Debit (R)': entry.totalDebit,
    'Total Credit (R)': entry.totalCredit,
    'Status': entry.status,
    'Posted By': entry.postedBy || '',
    'Posted At': entry.postedAt ? new Date(entry.postedAt).toLocaleString('en-ZA') : ''
  }))

  const csvContent = [
    Object.keys(exportData[0]).join(','),
    ...exportData.map(row => Object.values(row).map(v => `"${v}"`).join(','))
  ].join('\n')

  const blob = new Blob([csvContent], { type: 'text/csv' })
  const url = URL.createObjectURL(blob)
  const a = document.createElement('a')
  a.href = url
  a.download = `journal-entries-${new Date().toISOString().split('T')[0]}.csv`
  a.click()
  URL.revokeObjectURL(url)
}

// Lifecycle
onMounted(() => {
  loadEntries()
  loadAccounts()
})
</script>
