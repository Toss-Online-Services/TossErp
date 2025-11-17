<template>
  <div class="p-4 sm:p-6 space-y-6">
    <div class="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
      <div>
        <h1 class="text-2xl font-bold tracking-tight text-foreground">
          {{ $t('sales.quotations.title', 'Sales Quotations') }}
        </h1>
        <p class="text-muted-foreground">
          {{ $t('sales.quotations.subtitle', 'Manage quotes and proposals for customers') }}
        </p>
      </div>
      <div class="flex items-center gap-2">
        <Button variant="outline" @click="handleRefresh" :disabled="pending">
          <RefreshCcw class="mr-2 h-4 w-4" />
          {{ $t('common.refresh', 'Refresh') }}
        </Button>
        <Button as-child>
          <NuxtLink to="/sales/quotations/create">
            <Plus class="mr-2 h-4 w-4" />
            {{ $t('sales.quotations.actions.new', 'New Quotation') }}
          </NuxtLink>
        </Button>
      </div>
    </div>

    <div class="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
      <Card v-for="stat in stats" :key="stat.key">
        <CardHeader class="flex flex-row items-center justify-between space-y-0 pb-2">
          <CardTitle class="text-sm font-medium text-muted-foreground">
            {{ stat.label }}
          </CardTitle>
          <component :is="stat.icon" class="h-5 w-5 text-muted-foreground" />
        </CardHeader>
        <CardContent>
          <p class="text-2xl font-bold">{{ stat.value }}</p>
          <p v-if="stat.caption" class="text-xs text-muted-foreground mt-1">
            {{ stat.caption }}
          </p>
        </CardContent>
      </Card>
    </div>

    <Card>
      <CardHeader>
        <CardTitle>{{ $t('common.filters', 'Filters') }}</CardTitle>
      </CardHeader>
      <CardContent class="space-y-4">
        <div class="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
          <div class="space-y-2">
            <Label for="quotation-search">{{ $t('common.search', 'Search') }}</Label>
            <Input
              id="quotation-search"
              v-model="filters.search"
              :placeholder="$t('sales.quotations.filters.searchPlaceholder', 'Search by customer or number')"
            />
          </div>
          <div class="space-y-2">
            <Label>{{ $t('common.status', 'Status') }}</Label>
            <Select v-model="filters.status">
              <SelectTrigger>
                <SelectValue :placeholder="$t('common.allStatuses', 'All statuses')" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">{{ $t('common.all', 'All') }}</SelectItem>
                <SelectItem v-for="option in statusOptions" :key="option.value" :value="option.value">
                  {{ option.label }}
                </SelectItem>
              </SelectContent>
            </Select>
          </div>
          <div class="space-y-2">
            <Label for="quotation-start-date">{{ $t('common.startDate', 'Start date') }}</Label>
            <Input id="quotation-start-date" type="date" v-model="filters.startDate" />
          </div>
          <div class="space-y-2">
            <Label for="quotation-end-date">{{ $t('common.endDate', 'End date') }}</Label>
            <Input id="quotation-end-date" type="date" v-model="filters.endDate" />
          </div>
        </div>
        <div class="flex flex-wrap items-center gap-2">
          <Button variant="outline" @click="resetFilters">
            <RotateCcw class="mr-2 h-4 w-4" />
            {{ $t('common.reset', 'Reset filters') }}
          </Button>
          <span v-if="activeFiltersCount > 0" class="text-sm text-muted-foreground">
            {{ activeFiltersLabel }}
          </span>
        </div>
      </CardContent>
    </Card>

    <Card>
      <CardHeader>
        <CardTitle>{{ $t('sales.quotations.listTitle', 'Quotations') }}</CardTitle>
        <CardDescription>
          {{
            $t(
              'sales.quotations.listSubtitle',
              'Track quotation progress and follow up with customers'
            )
          }}
        </CardDescription>
      </CardHeader>
      <CardContent class="space-y-4">
        <Alert v-if="error">
          <AlertTitle>{{ $t('common.error', 'Something went wrong') }}</AlertTitle>
          <AlertDescription>{{ errorMessage }}</AlertDescription>
        </Alert>

        <div v-else>
          <Table v-if="!pending && quotations.length">
            <TableHeader>
              <TableRow>
                <TableHead>{{ $t('sales.quotations.number', 'Quotation #') }}</TableHead>
                <TableHead>{{ $t('sales.quotations.customer', 'Customer') }}</TableHead>
                <TableHead>{{ $t('common.date', 'Date') }}</TableHead>
                <TableHead class="text-right">{{ $t('common.amount', 'Amount') }}</TableHead>
                <TableHead class="text-center">{{ $t('common.status', 'Status') }}</TableHead>
                <TableHead class="text-right">{{ $t('common.actions', 'Actions') }}</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              <TableRow v-for="quotation in quotations" :key="quotation.id">
                <TableCell class="font-medium">
                  {{ quotation.number ?? quotation.id }}
                </TableCell>
                <TableCell>
                  <div class="flex flex-col">
                    <span class="font-medium">{{ quotation.customer.name }}</span>
                    <span v-if="quotation.customer.email" class="text-xs text-muted-foreground">
                      {{ quotation.customer.email }}
                    </span>
                  </div>
                </TableCell>
                <TableCell>{{ formatDate(quotation.date) }}</TableCell>
                <TableCell class="text-right">{{ formatCurrency(quotation.grandTotal) }}</TableCell>
                <TableCell class="text-center">
                  <Badge :variant="statusVariant(quotation.status)">
                    {{ formatStatus(quotation.status) }}
                  </Badge>
                </TableCell>
                <TableCell class="text-right">
                  <Button variant="ghost" size="sm" @click="goToDetail(quotation.id)">
                    <Eye class="mr-2 h-4 w-4" />
                    {{ $t('common.view', 'View') }}
                  </Button>
                </TableCell>
              </TableRow>
            </TableBody>
          </Table>

          <div v-else-if="pending" class="space-y-3">
            <Skeleton class="h-12 w-full" />
            <Skeleton class="h-12 w-full" />
            <Skeleton class="h-12 w-full" />
          </div>

          <div v-else class="flex flex-col items-center justify-center gap-2 py-12 text-center">
            <FileQuestion class="h-10 w-10 text-muted-foreground" />
            <p class="text-sm text-muted-foreground">
              {{
                $t(
                  'sales.quotations.emptyState',
                  'No quotations match your filters. Try adjusting them or create a new quotation.'
                )
              }}
            </p>
            <Button as-child>
              <NuxtLink to="/sales/quotations/create">
                <Plus class="mr-2 h-4 w-4" />
                {{ $t('sales.quotations.actions.new', 'New Quotation') }}
              </NuxtLink>
            </Button>
          </div>
        </div>
      </CardContent>
    </Card>
  </div>
</template>

<script setup lang="ts">
import { computed, reactive } from 'vue'
import { useAsyncData } from '#app'
import { useI18n } from 'vue-i18n'
import { useRouter } from 'vue-router'
import {
  RefreshCcw,
  Plus,
  RotateCcw,
  Eye,
  FileQuestion,
  FileText,
  Send,
  CheckCircle2,
  AlertTriangle
} from 'lucide-vue-next'

import { Button } from '~/components/ui/button'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '~/components/ui/card'
import { Badge } from '~/components/ui/badge'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '~/components/ui/table'
import { Input } from '~/components/ui/input'
import { Label } from '~/components/ui/label'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '~/components/ui/select'
import { Alert, AlertDescription, AlertTitle } from '~/components/ui/alert'
import { Skeleton } from '~/components/ui/skeleton'

import type { Quotation } from '~/types/sales'

const { t } = useI18n()
const router = useRouter()

const filters = reactive({
  search: '',
  status: 'all',
  startDate: '',
  endDate: ''
})

const statusOptions = [
  { value: 'draft', label: t('sales.quotations.status.draft', 'Draft') },
  { value: 'sent', label: t('sales.quotations.status.sent', 'Sent') },
  { value: 'accepted', label: t('sales.quotations.status.accepted', 'Accepted') },
  { value: 'rejected', label: t('sales.quotations.status.rejected', 'Rejected') },
  { value: 'expired', label: t('sales.quotations.status.expired', 'Expired') },
  { value: 'converted', label: t('sales.quotations.status.converted', 'Converted') }
]

const fetchQuotations = () => {
  const params: Record<string, string> = {}
  if (filters.search) params.search = filters.search
  if (filters.status && filters.status !== 'all') params.status = filters.status
  if (filters.startDate) params.dateFrom = filters.startDate
  if (filters.endDate) params.dateTo = filters.endDate
  return $fetch('/api/sales/quotations', { params })
}

const { data, pending, error, refresh } = await useAsyncData('sales-quotations-list', fetchQuotations, {
  watch: [
    () => filters.search,
    () => filters.status,
    () => filters.startDate,
    () => filters.endDate
  ]
})

const quotations = computed<Quotation[]>(() => data.value?.data ?? [])

const statusCounts = computed(() => {
  const counts: Record<string, number> = {
    draft: 0,
    sent: 0,
    accepted: 0,
    rejected: 0,
    expired: 0,
    converted: 0
  }
  quotations.value.forEach((quotation) => {
    const status = quotation.status ?? 'draft'
    counts[status] = (counts[status] ?? 0) + 1
  })
  return counts
})

const stats = computed(() => [
  {
    key: 'draft',
    label: t('sales.quotations.status.draft', 'Draft'),
    value: statusCounts.value.draft ?? 0,
    icon: FileText
  },
  {
    key: 'sent',
    label: t('sales.quotations.status.sent', 'Sent'),
    value: statusCounts.value.sent ?? 0,
    icon: Send
  },
  {
    key: 'accepted',
    label: t('sales.quotations.status.accepted', 'Accepted'),
    value: statusCounts.value.accepted ?? 0,
    icon: CheckCircle2
  },
  {
    key: 'expired',
    label: t('sales.quotations.status.expired', 'Expired'),
    value: (statusCounts.value.expired ?? 0) + (statusCounts.value.rejected ?? 0),
    icon: AlertTriangle,
    caption: t('sales.quotations.expiredCaption', 'Includes rejected or expired quotations')
  }
])

const activeFiltersCount = computed(() => {
  let count = 0
  if (filters.search) count += 1
  if (filters.status !== 'all') count += 1
  if (filters.startDate) count += 1
  if (filters.endDate) count += 1
  return count
})

const activeFiltersLabel = computed(() => {
  if (!activeFiltersCount.value) return ''
  return t('sales.quotations.activeFilters', '{count} filters applied', {
    count: activeFiltersCount.value
  })
})

const errorMessage = computed(
  () => (error.value as Error | null)?.message ?? t('common.unexpectedError', 'Unexpected error loading quotations.')
)

const formatStatus = (status: string) => {
  switch (status) {
    case 'draft':
      return t('sales.quotations.status.draft', 'Draft')
    case 'sent':
      return t('sales.quotations.status.sent', 'Sent')
    case 'accepted':
      return t('sales.quotations.status.accepted', 'Accepted')
    case 'rejected':
      return t('sales.quotations.status.rejected', 'Rejected')
    case 'expired':
      return t('sales.quotations.status.expired', 'Expired')
    case 'converted':
      return t('sales.quotations.status.converted', 'Converted')
    default:
      return status
  }
}

const statusVariant = (status: string) => {
  switch (status) {
    case 'draft':
      return 'outline'
    case 'sent':
      return 'secondary'
    case 'accepted':
    case 'converted':
      return 'default'
    case 'rejected':
    case 'expired':
      return 'destructive'
    default:
      return 'secondary'
  }
}

const formatCurrency = (value: number) =>
  new Intl.NumberFormat('en-ZA', { style: 'currency', currency: 'ZAR' }).format(value ?? 0)

const formatDate = (value: string) =>
  new Date(value).toLocaleDateString('en-ZA', { year: 'numeric', month: 'short', day: 'numeric' })

const goToDetail = (id: string) => {
  router.push(`/sales/quotations/${id}`)
}

const resetFilters = () => {
  filters.search = ''
  filters.status = 'all'
  filters.startDate = ''
  filters.endDate = ''
  refresh()
}

const handleRefresh = () => {
  refresh()
}
</script>

<style scoped>
.table-header {
  @apply px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider dark:text-gray-300;
}
.table-cell {
  @apply px-6 py-4 whitespace-nowrap text-sm text-gray-600 dark:text-gray-300;
}
.btn {
  @apply inline-flex items-center justify-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-offset-2;
}
.btn-primary {
  @apply text-white bg-blue-600 hover:bg-blue-700 focus:ring-blue-500;
}
</style>
