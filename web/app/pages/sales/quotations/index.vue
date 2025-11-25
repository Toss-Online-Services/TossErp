<script setup lang="ts">
import { ref, computed, h } from 'vue'
import { 
  Plus, 
  Search, 
  Edit,
  Eye
} from 'lucide-vue-next'
import { 
  useVueTable,
  getCoreRowModel,
  getPaginationRowModel,
  getFilteredRowModel,
  getSortedRowModel,
  type ColumnDef,
  type SortingState
} from '@tanstack/vue-table'

useHead({
  title: 'Quotations - Sales - TOSS ERP'
})

// Types
interface Quotation {
  id: string
  name: string
  customer_name: string
  transaction_date: string
  valid_till: string
  grand_total: number
  status: 'Draft' | 'Sent' | 'Accepted' | 'Rejected' | 'Expired'
}

// Mock data - will be replaced with API
const quotations = ref<Quotation[]>([
  {
    id: '1',
    name: 'QUO-00001',
    customer_name: 'ABC Trading',
    transaction_date: '2025-01-20',
    valid_till: '2025-02-20',
    grand_total: 12500,
    status: 'Sent'
  },
  {
    id: '2',
    name: 'QUO-00002',
    customer_name: 'XYZ Stores',
    transaction_date: '2025-01-19',
    valid_till: '2025-02-19',
    grand_total: 8500,
    status: 'Draft'
  },
  {
    id: '3',
    name: 'QUO-00003',
    customer_name: 'Retail Co',
    transaction_date: '2025-01-18',
    valid_till: '2025-02-18',
    grand_total: 15200,
    status: 'Accepted'
  },
  {
    id: '4',
    name: 'QUO-00004',
    customer_name: 'Market Suppliers',
    transaction_date: '2025-01-15',
    valid_till: '2025-02-15',
    grand_total: 9800,
    status: 'Rejected'
  }
])

const searchQuery = ref('')
const statusFilter = ref<string>('all')
const sorting = ref<SortingState>([])

// Filter quotations
const filteredQuotations = computed(() => {
  let filtered = quotations.value

  // Search filter
  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    filtered = filtered.filter(q => 
      q.name.toLowerCase().includes(query) ||
      q.customer_name.toLowerCase().includes(query)
    )
  }

  // Status filter
  if (statusFilter.value !== 'all') {
    filtered = filtered.filter(q => q.status === statusFilter.value)
  }

  return filtered
})

// Stats
const stats = computed(() => ({
  total: quotations.value.length,
  draft: quotations.value.filter(q => q.status === 'Draft').length,
  sent: quotations.value.filter(q => q.status === 'Sent').length,
  accepted: quotations.value.filter(q => q.status === 'Accepted').length,
  rejected: quotations.value.filter(q => q.status === 'Rejected').length
}))

// Table columns
const columns: ColumnDef<Quotation>[] = [
  {
    accessorKey: 'name',
    header: 'Quotation #',
    cell: ({ row }) => {
      const quotation = row.original
      return h('div', { class: 'font-medium' }, quotation.name)
    }
  },
  {
    accessorKey: 'customer_name',
    header: 'Customer',
  },
  {
    accessorKey: 'transaction_date',
    header: 'Date',
    cell: ({ row }) => {
      const date = new Date(row.original.transaction_date)
      return date.toLocaleDateString('en-ZA')
    }
  },
  {
    accessorKey: 'valid_till',
    header: 'Valid Till',
    cell: ({ row }) => {
      const date = new Date(row.original.valid_till)
      return date.toLocaleDateString('en-ZA')
    }
  },
  {
    accessorKey: 'grand_total',
    header: 'Amount',
    cell: ({ row }) => {
      return `R ${row.original.grand_total.toLocaleString()}`
    }
  },
  {
    accessorKey: 'status',
    header: 'Status',
    cell: ({ row }) => {
      const status = row.original.status
      const variantMap: Record<string, string> = {
        'Draft': 'secondary',
        'Sent': 'default',
        'Accepted': 'default',
        'Rejected': 'destructive',
        'Expired': 'secondary'
      }
      return h(Badge, { variant: variantMap[status] as any }, () => status)
    }
  },
  {
    id: 'actions',
    header: 'Actions',
    cell: ({ row }) => {
      return h('div', { class: 'flex items-center gap-2' }, [
        h(Button, {
          variant: 'ghost',
          size: 'icon',
          class: 'h-8 w-8'
        }, {
          default: () => h(Eye, { class: 'h-4 w-4' })
        }),
        h(Button, {
          variant: 'ghost',
          size: 'icon',
          class: 'h-8 w-8'
        }, {
          default: () => h(Edit, { class: 'h-4 w-4' })
        })
      ])
    }
  }
]

// Table instance
const table = useVueTable({
  get data() {
    return filteredQuotations.value
  },
  columns,
  getCoreRowModel: getCoreRowModel(),
  getPaginationRowModel: getPaginationRowModel(),
  getSortedRowModel: getSortedRowModel(),
  getFilteredRowModel: getFilteredRowModel(),
  onSortingChange: (updater) => {
    sorting.value = typeof updater === 'function' ? updater(sorting.value) : updater
  },
  state: {
    get sorting() {
      return sorting.value
    }
  }
})

const getStatusBadgeVariant = (status: string) => {
  const variants: Record<string, string> = {
    'Draft': 'secondary',
    'Sent': 'default',
    'Accepted': 'default',
    'Rejected': 'destructive',
    'Expired': 'secondary'
  }
  return variants[status] || 'secondary'
}
</script>

<template>
  <div class="space-y-6">
    <!-- Page Header -->
    <div class="flex items-center justify-between">
      <div>
        <h1 class="text-3xl font-bold tracking-tight">Quotations</h1>
        <p class="text-muted-foreground mt-2">Manage customer quotations and quotes</p>
      </div>
      <NuxtLink to="/sales/quotations/create">
        <Button>
          <Plus class="mr-2 h-4 w-4" />
          New Quotation
        </Button>
      </NuxtLink>
    </div>

    <!-- Stats Cards -->
    <div class="grid gap-4 md:grid-cols-5">
      <Card>
        <CardHeader class="pb-2">
          <CardTitle class="text-sm font-medium">Total</CardTitle>
        </CardHeader>
        <CardContent>
          <div class="text-2xl font-bold">{{ stats.total }}</div>
        </CardContent>
      </Card>
      <Card>
        <CardHeader class="pb-2">
          <CardTitle class="text-sm font-medium">Draft</CardTitle>
        </CardHeader>
        <CardContent>
          <div class="text-2xl font-bold">{{ stats.draft }}</div>
        </CardContent>
      </Card>
      <Card>
        <CardHeader class="pb-2">
          <CardTitle class="text-sm font-medium">Sent</CardTitle>
        </CardHeader>
        <CardContent>
          <div class="text-2xl font-bold">{{ stats.sent }}</div>
        </CardContent>
      </Card>
      <Card>
        <CardHeader class="pb-2">
          <CardTitle class="text-sm font-medium">Accepted</CardTitle>
        </CardHeader>
        <CardContent>
          <div class="text-2xl font-bold">{{ stats.accepted }}</div>
        </CardContent>
      </Card>
      <Card>
        <CardHeader class="pb-2">
          <CardTitle class="text-sm font-medium">Rejected</CardTitle>
        </CardHeader>
        <CardContent>
          <div class="text-2xl font-bold">{{ stats.rejected }}</div>
        </CardContent>
      </Card>
    </div>

    <!-- Filters and Search -->
    <Card>
      <CardHeader>
        <div class="flex items-center gap-4">
          <div class="relative flex-1 max-w-sm">
            <Search class="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
            <Input
              v-model="searchQuery"
              placeholder="Search quotations..."
              class="pl-10"
            />
          </div>
          <Select v-model="statusFilter">
            <SelectTrigger class="w-[180px]">
              <SelectValue placeholder="Filter by status" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="all">All Status</SelectItem>
              <SelectItem value="Draft">Draft</SelectItem>
              <SelectItem value="Sent">Sent</SelectItem>
              <SelectItem value="Accepted">Accepted</SelectItem>
              <SelectItem value="Rejected">Rejected</SelectItem>
              <SelectItem value="Expired">Expired</SelectItem>
            </SelectContent>
          </Select>
        </div>
      </CardHeader>
      <CardContent>
        <div class="rounded-md border">
          <Table>
            <TableHeader>
              <TableRow v-for="headerGroup in table.getHeaderGroups()" :key="headerGroup.id">
                <TableHead v-for="header in headerGroup.headers" :key="header.id">
                  <div v-if="!header.isPlaceholder">
                    <button
                      v-if="header.column.getCanSort()"
                      @click="header.column.toggleSorting()"
                      class="flex items-center gap-2 hover:opacity-70"
                    >
                      {{ (header.column.columnDef.header as string) || header.id }}
                      <span v-if="header.column.getIsSorted()">
                        {{ header.column.getIsSorted() === 'asc' ? '↑' : '↓' }}
                      </span>
                    </button>
                    <span v-else>
                      {{ (header.column.columnDef.header as string) || header.id }}
                    </span>
                  </div>
                </TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              <TableRow
                v-for="row in table.getRowModel().rows"
                :key="row.id"
                class="cursor-pointer hover:bg-muted/50"
              >
                <TableCell v-for="cell in row.getVisibleCells()" :key="cell.id">
                  <component :is="cell.column.columnDef.cell" :props="cell.getContext()" />
                </TableCell>
              </TableRow>
              <TableRow v-if="table.getRowModel().rows.length === 0">
                <TableCell :col-span="columns.length" class="h-24 text-center">
                  No quotations found.
                </TableCell>
              </TableRow>
            </TableBody>
          </Table>
        </div>

        <!-- Pagination -->
        <div class="flex items-center justify-between px-2 py-4">
          <div class="text-sm text-muted-foreground">
            Showing {{ table.getState().pagination.pageIndex * table.getState().pagination.pageSize + 1 }} to
            {{ Math.min((table.getState().pagination.pageIndex + 1) * table.getState().pagination.pageSize, table.getFilteredRowModel().rows.length) }}
            of {{ table.getFilteredRowModel().rows.length }} quotations
          </div>
          <div class="flex items-center gap-2">
            <Button
              variant="outline"
              size="sm"
              :disabled="!table.getCanPreviousPage()"
              @click="table.previousPage()"
            >
              Previous
            </Button>
            <Button
              variant="outline"
              size="sm"
              :disabled="!table.getCanNextPage()"
              @click="table.nextPage()"
            >
              Next
            </Button>
          </div>
        </div>
      </CardContent>
    </Card>
  </div>
</template>

