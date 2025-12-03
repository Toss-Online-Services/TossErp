<script setup lang="ts">
import { onMounted, ref, computed } from 'vue'
import { useAccountingStore, type JournalEntry } from '~/stores/accounting'
import AccountingJournalEntryModal from '~/components/accounting/JournalEntryModal.vue'

definePageMeta({
  layout: 'default',
  ssr: false
})

useHead({
  title: 'Journal Entries - TOSS'
})

const accountingStore = useAccountingStore()
const searchQuery = ref('')
const statusFilter = ref<'all' | 'draft' | 'posted' | 'cancelled'>('all')
const showCreateModal = ref(false)
const showEditModal = ref(false)
const selectedEntry = ref<JournalEntry | null>(null)

// Computed
const filteredEntries = computed(() => {
  let filtered = accountingStore.journalEntries

  if (statusFilter.value !== 'all') {
    filtered = filtered.filter(entry => entry.status === statusFilter.value)
  }

  if (searchQuery.value.trim()) {
    const query = searchQuery.value.toLowerCase()
    filtered = filtered.filter(entry =>
      entry.entryNumber.toLowerCase().includes(query) ||
      (entry.reference || '').toLowerCase().includes(query) ||
      (entry.description || '').toLowerCase().includes(query)
    )
  }

  return filtered.sort((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime())
})

const stats = computed(() => {
  const entries = accountingStore.journalEntries
  return {
    total: entries.length,
    draft: entries.filter(e => e.status === 'draft').length,
    posted: entries.filter(e => e.status === 'posted').length,
    unbalanced: entries.filter(e => !e.isBalanced).length
  }
})

// Methods
onMounted(async () => {
  await accountingStore.fetchJournalEntries()
  await accountingStore.fetchAccounts()
})

function handleCreate() {
  selectedEntry.value = null
  showCreateModal.value = true
}

function handleEdit(entry: JournalEntry) {
  if (entry.status === 'posted') {
    alert('Posted journal entries cannot be edited')
    return
  }
  selectedEntry.value = entry
  showEditModal.value = true
}

function handleView(entry: JournalEntry) {
  navigateTo(`/accounting/journals/${entry.id}`)
}

async function handlePost(entry: JournalEntry) {
  if (!entry.isBalanced) {
    alert('Journal entry must be balanced before posting')
    return
  }
  if (confirm('Are you sure you want to post this journal entry? This action cannot be undone.')) {
    await accountingStore.postJournalEntry(entry.id)
    await accountingStore.fetchJournalEntries()
    await accountingStore.fetchAccounts()
  }
}

function handleEntrySaved() {
  showCreateModal.value = false
  showEditModal.value = false
  selectedEntry.value = null
  accountingStore.fetchJournalEntries()
}

function handleCloseCreate() {
  showCreateModal.value = false
}

function handleCloseEdit() {
  showEditModal.value = false
  selectedEntry.value = null
}

function getStatusColor(status: string) {
  const colors: Record<string, string> = {
    draft: 'text-gray-600 bg-gray-100',
    posted: 'text-green-600 bg-green-100',
    cancelled: 'text-red-600 bg-red-100'
  }
  return colors[status] || 'text-gray-600 bg-gray-100'
}

function formatDate(date: Date | undefined) {
  if (!date) return '-'
  return new Date(date).toLocaleDateString('en-ZA', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  })
}

function formatCurrency(amount: number) {
  return new Intl.NumberFormat('en-ZA', {
    style: 'currency',
    currency: 'ZAR'
  }).format(amount)
}
</script>

<template>
  <div class="py-6">
    <div class="mb-8">
      <div class="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
        <div>
          <h3 class="text-3xl font-bold text-gray-900 mb-2">Journal Entries</h3>
          <p class="text-gray-600 text-sm">Record financial transactions and adjustments</p>
        </div>
        <button
          @click="handleCreate"
          class="inline-flex items-center gap-2 px-4 py-2 bg-gray-900 text-white rounded-lg hover:bg-gray-800 transition-colors"
        >
          <i class="material-symbols-rounded text-lg">add</i>
          <span>New Entry</span>
        </button>
      </div>
    </div>

    <!-- Stats Cards -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-6 mb-6">
      <div class="bg-white rounded-xl shadow-card p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-gray-600">Total Entries</p>
            <p class="mt-2 text-3xl font-bold text-gray-900">{{ stats.total }}</p>
          </div>
          <div class="p-3 bg-gray-100 rounded-lg">
            <i class="material-symbols-rounded text-2xl text-gray-600">book</i>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-card p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-gray-600">Draft</p>
            <p class="mt-2 text-3xl font-bold text-gray-600">{{ stats.draft }}</p>
          </div>
          <div class="p-3 bg-gray-100 rounded-lg">
            <i class="material-symbols-rounded text-2xl text-gray-600">edit</i>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-card p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-gray-600">Posted</p>
            <p class="mt-2 text-3xl font-bold text-green-600">{{ stats.posted }}</p>
          </div>
          <div class="p-3 bg-green-100 rounded-lg">
            <i class="material-symbols-rounded text-2xl text-green-600">check_circle</i>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-card p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-gray-600">Unbalanced</p>
            <p class="mt-2 text-3xl font-bold text-red-600">{{ stats.unbalanced }}</p>
          </div>
          <div class="p-3 bg-red-100 rounded-lg">
            <i class="material-symbols-rounded text-2xl text-red-600">warning</i>
          </div>
        </div>
      </div>
    </div>

    <!-- Filters -->
    <div class="bg-white rounded-xl shadow-card p-6 mb-6">
      <div class="flex flex-col md:flex-row gap-4">
        <div class="flex-1">
          <div class="relative">
            <i class="material-symbols-rounded absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400">search</i>
            <input
              v-model="searchQuery"
              type="text"
              placeholder="Search by entry number, reference, or description..."
              class="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-gray-900 focus:border-transparent"
            />
          </div>
        </div>
        <div class="md:w-48">
          <select
            v-model="statusFilter"
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-gray-900 focus:border-transparent"
          >
            <option value="all">All Status</option>
            <option value="draft">Draft</option>
            <option value="posted">Posted</option>
            <option value="cancelled">Cancelled</option>
          </select>
        </div>
      </div>
    </div>

    <!-- Journal Entries Table -->
    <div class="bg-white rounded-xl shadow-card overflow-hidden">
      <div v-if="accountingStore.loading" class="flex items-center justify-center py-12">
        <div class="text-center">
          <i class="material-symbols-rounded text-6xl text-gray-300 mb-4 animate-spin">refresh</i>
          <p class="text-gray-600">Loading journal entries...</p>
        </div>
      </div>

      <div v-else-if="filteredEntries.length === 0" class="text-center py-12">
        <i class="material-symbols-rounded text-6xl text-gray-300 mb-4">book</i>
        <h3 class="text-lg font-semibold text-gray-900 mb-2">No journal entries found</h3>
        <p class="text-gray-600 mb-6">Get started by creating your first journal entry</p>
        <button
          @click="handleCreate"
          class="inline-flex items-center gap-2 px-4 py-2 bg-gray-900 text-white rounded-lg hover:bg-gray-800 transition-colors"
        >
          <i class="material-symbols-rounded text-lg">add</i>
          <span>New Entry</span>
        </button>
      </div>

      <div v-else class="overflow-x-auto">
        <table class="w-full">
          <thead class="bg-gray-50 border-b border-gray-200">
            <tr>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Entry #</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Date</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Reference</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Description</th>
              <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Debit</th>
              <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Credit</th>
              <th class="px-6 py-3 text-center text-xs font-medium text-gray-500 uppercase tracking-wider">Status</th>
              <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-gray-200">
            <tr
              v-for="entry in filteredEntries"
              :key="entry.id"
              class="hover:bg-gray-50 transition-colors"
            >
              <td class="px-6 py-4 whitespace-nowrap">
                <span class="text-sm font-semibold text-gray-900">{{ entry.entryNumber }}</span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                {{ formatDate(entry.date) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-600">
                {{ entry.reference || '-' }}
              </td>
              <td class="px-6 py-4 text-sm text-gray-600">
                {{ entry.description || '-' }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-semibold text-gray-900">
                {{ formatCurrency(entry.totalDebit) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-semibold text-gray-900">
                {{ formatCurrency(entry.totalCredit) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-center">
                <div class="flex items-center justify-center gap-2">
                  <span :class="['px-2 py-1 text-xs font-medium rounded-full', getStatusColor(entry.status)]">
                    {{ entry.status === 'posted' ? 'Posted' : entry.status === 'draft' ? 'Draft' : 'Cancelled' }}
                  </span>
                  <span
                    v-if="!entry.isBalanced"
                    class="px-2 py-1 text-xs font-medium rounded-full text-red-600 bg-red-100"
                    title="Unbalanced entry"
                  >
                    !
                  </span>
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                <div class="flex items-center justify-end gap-2">
                  <button
                    @click="handleView(entry)"
                    class="p-2 text-gray-600 hover:text-gray-900 hover:bg-gray-100 rounded-lg transition-colors"
                    title="View"
                  >
                    <i class="material-symbols-rounded text-lg">visibility</i>
                  </button>
                  <button
                    v-if="entry.status === 'draft'"
                    @click="handleEdit(entry)"
                    class="p-2 text-gray-600 hover:text-gray-900 hover:bg-gray-100 rounded-lg transition-colors"
                    title="Edit"
                  >
                    <i class="material-symbols-rounded text-lg">edit</i>
                  </button>
                  <button
                    v-if="entry.status === 'draft' && entry.isBalanced"
                    @click="handlePost(entry)"
                    class="p-2 text-green-600 hover:text-green-900 hover:bg-green-100 rounded-lg transition-colors"
                    title="Post"
                  >
                    <i class="material-symbols-rounded text-lg">check_circle</i>
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Modals -->
    <ClientOnly>
      <AccountingJournalEntryModal
        :show="showCreateModal"
        :entry="null"
        @close="handleCloseCreate"
        @saved="handleEntrySaved"
      />
      <AccountingJournalEntryModal
        :show="showEditModal"
        :entry="selectedEntry"
        @close="handleCloseEdit"
        @saved="handleEntrySaved"
      />
    </ClientOnly>
  </div>
</template>

