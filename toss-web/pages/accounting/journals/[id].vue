<script setup lang="ts">
import { onMounted, ref, computed } from 'vue'
import { useAccountingStore, type JournalEntry } from '~/stores/accounting'

definePageMeta({
  layout: 'default',
  ssr: false
})

useHead({
  title: 'Journal Entry Details - TOSS'
})

const route = useRoute()
const accountingStore = useAccountingStore()

const entry = ref<JournalEntry | null>(null)
const loading = ref(false)

const entryId = computed(() => {
  const id = route.params.id
  return Array.isArray(id) ? id[0] : String(id)
})

async function loadEntry() {
  loading.value = true
  try {
    await accountingStore.fetchJournalEntries()
    const id = entryId.value
    
    // Try to find by ID first
    entry.value = accountingStore.getJournalEntryById(id) || null
    
    // If not found by ID, try to find by entryNumber
    if (!entry.value) {
      entry.value = accountingStore.journalEntries.find(
        e => e.entryNumber.toLowerCase() === id.toLowerCase() || 
             e.entryNumber.toLowerCase().replace(/-/g, '') === id.toLowerCase().replace(/-/g, '')
      ) || null
    }
    
    if (!entry.value) {
      console.error('Journal entry not found for ID:', id)
    }
  } catch (error) {
    console.error('Failed to load journal entry:', error)
  } finally {
    loading.value = false
  }
}

function formatCurrency(amount: number) {
  return new Intl.NumberFormat('en-ZA', {
    style: 'currency',
    currency: 'ZAR'
  }).format(amount)
}

function formatDate(date: Date | undefined) {
  if (!date) return 'Never'
  return new Date(date).toLocaleDateString('en-ZA', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  })
}

function formatDateTime(date: Date | undefined) {
  if (!date) return 'Never'
  return new Date(date).toLocaleDateString('en-ZA', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

function getStatusColor(status: string) {
  const colors: Record<string, string> = {
    draft: 'text-gray-600 bg-gray-100',
    posted: 'text-green-600 bg-green-100',
    cancelled: 'text-red-600 bg-red-100'
  }
  return colors[status] || 'text-gray-600 bg-gray-100'
}

function handlePrint() {
  window.print()
}

async function handlePost() {
  if (!entry.value) return
  if (!entry.value.isBalanced) {
    alert('Journal entry must be balanced before posting')
    return
  }
  if (confirm('Are you sure you want to post this journal entry? This action cannot be undone.')) {
    await accountingStore.postJournalEntry(entry.value.id)
    await loadEntry()
    await accountingStore.fetchAccounts()
  }
}

onMounted(async () => {
  await loadEntry()
})
</script>

<template>
  <div class="py-6">
    <div v-if="loading" class="flex items-center justify-center py-12">
      <div class="text-center">
        <i class="material-symbols-rounded text-6xl text-gray-300 mb-4 animate-spin">refresh</i>
        <p class="text-gray-600">Loading journal entry...</p>
      </div>
    </div>

    <div v-else-if="!entry" class="flex items-center justify-center py-12">
      <div class="text-center">
        <i class="material-symbols-rounded text-6xl text-gray-300 mb-4">error</i>
        <h3 class="text-lg font-semibold text-gray-900 mb-2">Journal entry not found</h3>
        <p class="text-gray-600 mb-6">The journal entry you're looking for doesn't exist</p>
        <button
          @click="navigateTo('/accounting/journals')"
          class="inline-flex items-center gap-2 px-4 py-2 bg-gray-900 text-white rounded-lg hover:bg-gray-800 transition-colors"
        >
          <i class="material-symbols-rounded text-lg">arrow_back</i>
          <span>Back to Journal Entries</span>
        </button>
      </div>
    </div>

    <div v-else>
      <!-- Header -->
      <div class="mb-6">
        <button
          @click="navigateTo('/accounting/journals')"
          class="inline-flex items-center gap-2 text-gray-600 hover:text-gray-900 mb-4 transition-colors"
        >
          <i class="material-symbols-rounded text-lg">arrow_back</i>
          <span>Back to Journal Entries</span>
        </button>
        <div class="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
          <div>
            <h3 class="text-3xl font-bold text-gray-900 mb-2">{{ entry.entryNumber }}</h3>
            <p class="text-gray-600 text-sm">{{ entry.description || 'No description' }}</p>
          </div>
          <div class="flex flex-wrap items-center gap-3">
            <span :class="['px-3 py-1 text-sm font-medium rounded-full', getStatusColor(entry.status)]">
              {{ entry.status === 'posted' ? 'Posted' : entry.status === 'draft' ? 'Draft' : 'Cancelled' }}
            </span>
            <span
              v-if="!entry.isBalanced"
              class="px-3 py-1 text-sm font-medium rounded-full text-red-600 bg-red-100"
            >
              Unbalanced
            </span>
            <button
              v-if="entry.status === 'draft' && entry.isBalanced"
              @click="handlePost"
              class="inline-flex items-center gap-2 px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 transition-colors"
            >
              <i class="material-symbols-rounded text-lg">check_circle</i>
              <span>Post Entry</span>
            </button>
            <button
              @click="handlePrint"
              class="inline-flex items-center gap-2 px-4 py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition-colors"
            >
              <i class="material-symbols-rounded text-lg">print</i>
              <span>Print</span>
            </button>
          </div>
        </div>
      </div>

      <!-- Info Cards -->
      <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-6">
        <div class="bg-white rounded-xl shadow-card p-6">
          <h4 class="text-sm font-medium text-gray-600 mb-4 flex items-center gap-2">
            <i class="material-symbols-rounded text-lg">calendar_today</i>
            Entry Information
          </h4>
          <div class="space-y-3">
            <div>
              <p class="text-xs text-gray-500 mb-1">Entry Date</p>
              <p class="text-base font-semibold text-gray-900">{{ formatDate(entry.date) }}</p>
            </div>
            <div v-if="entry.reference">
              <p class="text-xs text-gray-500 mb-1">Reference</p>
              <p class="text-base font-semibold text-gray-900">{{ entry.reference }}</p>
            </div>
            <div>
              <p class="text-xs text-gray-500 mb-1">Created By</p>
              <p class="text-base font-semibold text-gray-900">{{ entry.createdBy }}</p>
            </div>
            <div>
              <p class="text-xs text-gray-500 mb-1">Created At</p>
              <p class="text-sm text-gray-700">{{ formatDateTime(entry.createdAt) }}</p>
            </div>
            <div v-if="entry.postedAt">
              <p class="text-xs text-gray-500 mb-1">Posted At</p>
              <p class="text-sm text-gray-700">{{ formatDateTime(entry.postedAt) }}</p>
            </div>
          </div>
        </div>

        <div class="bg-white rounded-xl shadow-card p-6">
          <h4 class="text-sm font-medium text-gray-600 mb-4 flex items-center gap-2">
            <i class="material-symbols-rounded text-lg">account_balance</i>
            Totals
          </h4>
          <div class="space-y-3">
            <div>
              <p class="text-xs text-gray-500 mb-1">Total Debit</p>
              <p class="text-lg font-bold text-gray-900">{{ formatCurrency(entry.totalDebit) }}</p>
            </div>
            <div>
              <p class="text-xs text-gray-500 mb-1">Total Credit</p>
              <p class="text-lg font-bold text-gray-900">{{ formatCurrency(entry.totalCredit) }}</p>
            </div>
            <div>
              <p class="text-xs text-gray-500 mb-1">Balance Status</p>
              <p
                :class="[
                  'text-base font-bold',
                  entry.isBalanced ? 'text-green-600' : 'text-red-600'
                ]"
              >
                {{ entry.isBalanced ? 'Balanced' : `Difference: ${formatCurrency(Math.abs(entry.totalDebit - entry.totalCredit))}` }}
              </p>
            </div>
          </div>
        </div>

        <div class="bg-white rounded-xl shadow-card p-6">
          <h4 class="text-sm font-medium text-gray-600 mb-4 flex items-center gap-2">
            <i class="material-symbols-rounded text-lg">info</i>
            Status
          </h4>
          <div class="space-y-3">
            <div>
              <p class="text-xs text-gray-500 mb-1">Status</p>
              <span :class="['px-3 py-1 text-sm font-medium rounded-full inline-block', getStatusColor(entry.status)]">
                {{ entry.status === 'posted' ? 'Posted' : entry.status === 'draft' ? 'Draft' : 'Cancelled' }}
              </span>
            </div>
            <div v-if="entry.postedBy">
              <p class="text-xs text-gray-500 mb-1">Posted By</p>
              <p class="text-base font-semibold text-gray-900">{{ entry.postedBy }}</p>
            </div>
            <div>
              <p class="text-xs text-gray-500 mb-1">Number of Lines</p>
              <p class="text-base font-semibold text-gray-900">{{ entry.lines.length }} line{{ entry.lines.length !== 1 ? 's' : '' }}</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Journal Entry Lines -->
      <div class="bg-white rounded-xl shadow-card p-6">
        <h4 class="text-lg font-semibold text-gray-900 mb-6 flex items-center gap-2">
          <i class="material-symbols-rounded text-xl">list</i>
          Entry Lines
        </h4>

        <div class="overflow-x-auto">
          <table class="w-full">
            <thead class="bg-gray-50 border-b border-gray-200">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Account</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Description</th>
                <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Debit</th>
                <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Credit</th>
              </tr>
            </thead>
            <tbody class="bg-white divide-y divide-gray-200">
              <tr
                v-for="line in entry.lines"
                :key="line.id"
                class="hover:bg-gray-50 transition-colors"
              >
                <td class="px-6 py-4">
                  <div>
                    <p class="text-sm font-medium text-gray-900">{{ line.accountName }}</p>
                    <p class="text-xs text-gray-500">{{ line.accountCode }}</p>
                  </div>
                </td>
                <td class="px-6 py-4 text-sm text-gray-600">
                  {{ line.description || '-' }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-semibold text-gray-900">
                  {{ line.debit > 0 ? formatCurrency(line.debit) : '-' }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-semibold text-gray-900">
                  {{ line.credit > 0 ? formatCurrency(line.credit) : '-' }}
                </td>
              </tr>
            </tbody>
            <tfoot class="bg-gray-50 border-t-2 border-gray-300">
              <tr>
                <td colspan="2" class="px-6 py-3 text-sm font-bold text-gray-900">Total</td>
                <td class="px-6 py-3 text-right text-sm font-bold text-gray-900">
                  {{ formatCurrency(entry.totalDebit) }}
                </td>
                <td class="px-6 py-3 text-right text-sm font-bold text-gray-900">
                  {{ formatCurrency(entry.totalCredit) }}
                </td>
              </tr>
            </tfoot>
          </table>
        </div>
      </div>
    </div>
  </div>
</template>

