<template>
  <div class="h-full overflow-y-auto p-6 custom-scrollbar">
    <!-- Page Header -->
    <div class="flex items-center justify-between mb-8">
      <div>
        <h1 class="text-3xl font-bold text-stone-900 dark:text-white mb-2">Customers</h1>
        <p class="text-stone-500 dark:text-stone-400">Manage your customer relationships and credit accounts</p>
      </div>
      <Button class="bg-stone-900 hover:bg-stone-800 dark:bg-white dark:hover:bg-stone-100 dark:text-stone-900">
        <Icon name="lucide:plus" class="w-5 h-5 mr-2" />
        Add Customer
      </Button>
    </div>

    <!-- Stats Cards -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-stone-500 dark:text-stone-400">Total Customers</p>
              <p class="text-2xl font-bold text-stone-900 dark:text-white">1,234</p>
            </div>
            <div class="w-12 h-12 bg-blue-100 dark:bg-blue-900/30 rounded-lg flex items-center justify-center">
              <Icon name="lucide:users" class="w-6 h-6 text-blue-600 dark:text-blue-400" />
            </div>
          </div>
        </CardContent>
      </Card>

      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-stone-500 dark:text-stone-400">Active Credit</p>
              <p class="text-2xl font-bold text-stone-900 dark:text-white">R 45,680</p>
            </div>
            <div class="w-12 h-12 bg-orange-100 dark:bg-orange-900/30 rounded-lg flex items-center justify-center">
              <Icon name="lucide:wallet" class="w-6 h-6 text-orange-600 dark:text-orange-400" />
            </div>
          </div>
        </CardContent>
      </Card>

      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-stone-500 dark:text-stone-400">Overdue</p>
              <p class="text-2xl font-bold text-red-600 dark:text-red-400">R 8,450</p>
            </div>
            <div class="w-12 h-12 bg-red-100 dark:bg-red-900/30 rounded-lg flex items-center justify-center">
              <Icon name="lucide:alert-triangle" class="w-6 h-6 text-red-600 dark:text-red-400" />
            </div>
          </div>
        </CardContent>
      </Card>

      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-stone-500 dark:text-stone-400">New This Month</p>
              <p class="text-2xl font-bold text-green-600 dark:text-green-400">+42</p>
            </div>
            <div class="w-12 h-12 bg-green-100 dark:bg-green-900/30 rounded-lg flex items-center justify-center">
              <Icon name="lucide:user-plus" class="w-6 h-6 text-green-600 dark:text-green-400" />
            </div>
          </div>
        </CardContent>
      </Card>
    </div>

    <!-- Search and Filters -->
    <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700 mb-6">
      <CardContent class="p-4">
        <div class="flex flex-col md:flex-row gap-4">
          <div class="relative flex-1">
            <Icon name="lucide:search" class="absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-stone-400" />
            <input
              v-model="searchQuery"
              type="text"
              placeholder="Search customers by name, phone, or ID..."
              class="w-full pl-10 pr-4 py-2 border border-stone-200 dark:border-stone-700 rounded-lg bg-stone-50 dark:bg-stone-900 text-stone-900 dark:text-white focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            />
          </div>
          <select
            v-model="filterStatus"
            class="px-4 py-2 border border-stone-200 dark:border-stone-700 rounded-lg bg-stone-50 dark:bg-stone-900 text-stone-900 dark:text-white"
          >
            <option value="all">All Status</option>
            <option value="active">Active</option>
            <option value="inactive">Inactive</option>
            <option value="credit">Has Credit</option>
            <option value="overdue">Overdue</option>
          </select>
        </div>
      </CardContent>
    </Card>

    <!-- Customers Table -->
    <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
      <CardHeader class="border-b border-stone-200 dark:border-stone-700">
        <div class="flex items-center justify-between">
          <CardTitle class="text-lg font-semibold text-stone-900 dark:text-white">Customer List</CardTitle>
          <div class="flex items-center text-sm text-stone-500 dark:text-stone-400">
            <div class="w-2 h-2 bg-green-500 rounded-full mr-2" />
            {{ filteredCustomers.length }} customers
          </div>
        </div>
      </CardHeader>

      <CardContent class="p-0">
        <div class="overflow-x-auto">
          <table class="w-full">
            <thead class="bg-stone-50 dark:bg-stone-900">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 dark:text-stone-400 uppercase tracking-wider">
                  Customer
                </th>
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 dark:text-stone-400 uppercase tracking-wider">
                  Contact
                </th>
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 dark:text-stone-400 uppercase tracking-wider">
                  Status
                </th>
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 dark:text-stone-400 uppercase tracking-wider">
                  Credit Balance
                </th>
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 dark:text-stone-400 uppercase tracking-wider">
                  Total Purchases
                </th>
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 dark:text-stone-400 uppercase tracking-wider">
                  Actions
                </th>
              </tr>
            </thead>
            <tbody class="bg-white dark:bg-stone-800 divide-y divide-stone-200 dark:divide-stone-700">
              <tr v-for="customer in filteredCustomers" :key="customer.id" class="hover:bg-stone-50 dark:hover:bg-stone-700">
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="flex items-center">
                    <div class="w-10 h-10 rounded-full bg-stone-200 dark:bg-stone-600 flex items-center justify-center text-stone-600 dark:text-stone-300 font-medium">
                      {{ customer.name.charAt(0) }}
                    </div>
                    <div class="ml-4">
                      <div class="text-sm font-medium text-stone-900 dark:text-white">{{ customer.name }}</div>
                      <div class="text-sm text-stone-500 dark:text-stone-400">ID: {{ customer.id }}</div>
                    </div>
                  </div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="text-sm text-stone-900 dark:text-white">{{ customer.phone }}</div>
                  <div class="text-sm text-stone-500 dark:text-stone-400">{{ customer.area }}</div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <Badge
                    :class="[
                      customer.status === 'active' ? 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400' :
                      customer.status === 'overdue' ? 'bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400' :
                      'bg-stone-100 text-stone-800 dark:bg-stone-700 dark:text-stone-300'
                    ]"
                  >
                    {{ customer.status }}
                  </Badge>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span :class="customer.creditBalance > 0 ? 'text-orange-600 dark:text-orange-400' : 'text-stone-900 dark:text-white'" class="text-sm font-medium">
                    R {{ customer.creditBalance.toFixed(2) }}
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-stone-900 dark:text-white">
                  R {{ customer.totalPurchases.toFixed(2) }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="flex items-center gap-2">
                    <Button variant="ghost" size="sm">
                      <Icon name="lucide:eye" class="w-4 h-4" />
                    </Button>
                    <Button variant="ghost" size="sm">
                      <Icon name="lucide:edit" class="w-4 h-4" />
                    </Button>
                    <Button variant="ghost" size="sm" class="text-red-500 hover:text-red-600">
                      <Icon name="lucide:trash-2" class="w-4 h-4" />
                    </Button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </CardContent>
    </Card>
  </div>
</template>

<script setup lang="ts">
import { Card, CardContent, CardHeader, CardTitle } from '~/components/ui/card'
import { Button } from '~/components/ui/button'
import { Badge } from '~/components/ui/badge'

definePageMeta({
  layout: 'default'
})

const searchQuery = ref('')
const filterStatus = ref('all')

const customers = ref([
  { id: 'C001', name: 'Thabo Molefe', phone: '072 123 4567', area: 'Soweto', status: 'active', creditBalance: 0, totalPurchases: 15680.50 },
  { id: 'C002', name: 'Nomsa Dlamini', phone: '083 234 5678', area: 'Alexandra', status: 'active', creditBalance: 450.00, totalPurchases: 8920.00 },
  { id: 'C003', name: 'Sipho Nkosi', phone: '071 345 6789', area: 'Tembisa', status: 'overdue', creditBalance: 1250.00, totalPurchases: 22450.75 },
  { id: 'C004', name: 'Lindiwe Mthembu', phone: '082 456 7890', area: 'Diepsloot', status: 'active', creditBalance: 0, totalPurchases: 5680.25 },
  { id: 'C005', name: 'Bongani Zulu', phone: '073 567 8901', area: 'Katlehong', status: 'inactive', creditBalance: 0, totalPurchases: 2340.00 },
  { id: 'C006', name: 'Precious Mokoena', phone: '084 678 9012', area: 'Soweto', status: 'active', creditBalance: 780.00, totalPurchases: 18750.50 },
  { id: 'C007', name: 'David Sithole', phone: '076 789 0123', area: 'Orange Farm', status: 'overdue', creditBalance: 2100.00, totalPurchases: 12890.00 },
  { id: 'C008', name: 'Grace Mahlangu', phone: '081 890 1234', area: 'Mamelodi', status: 'active', creditBalance: 320.00, totalPurchases: 9560.75 },
])

const filteredCustomers = computed(() => {
  return customers.value.filter(c => {
    const matchesSearch = c.name.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
                          c.phone.includes(searchQuery.value) ||
                          c.id.toLowerCase().includes(searchQuery.value.toLowerCase())
    
    const matchesStatus = filterStatus.value === 'all' ||
                          filterStatus.value === c.status ||
                          (filterStatus.value === 'credit' && c.creditBalance > 0)
    
    return matchesSearch && matchesStatus
  })
})
</script>

<style scoped>
.custom-scrollbar::-webkit-scrollbar {
  width: 6px;
}

.custom-scrollbar::-webkit-scrollbar-track {
  background: hsl(var(--muted));
}

.custom-scrollbar::-webkit-scrollbar-thumb {
  background: hsl(var(--muted-foreground));
  border-radius: 3px;
}
</style>
