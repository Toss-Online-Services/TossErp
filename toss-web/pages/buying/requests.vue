<template>
  <div class="space-y-6">
    <PageHeader
      title="Purchase Requests"
      description="Manage and approve purchase requisitions"
    />

    <div class="flex justify-between items-center">
      <div class="flex gap-4">
        <input
          v-model="searchQuery"
          type="text"
          placeholder="Search requests..."
          class="px-4 py-2 border rounded-lg dark:bg-gray-700 dark:text-white"
        />
        <select v-model="filterStatus" class="px-4 py-2 border rounded-lg dark:bg-gray-700 dark:text-white">
          <option value="">All Status</option>
          <option value="Pending">Pending</option>
          <option value="Approved">Approved</option>
          <option value="Rejected">Rejected</option>
          <option value="Ordered">Ordered</option>
        </select>
        <select v-model="filterDepartment" class="px-4 py-2 border rounded-lg dark:bg-gray-700 dark:text-white">
          <option value="">All Departments</option>
          <option value="Manufacturing">Manufacturing</option>
          <option value="IT">IT</option>
          <option value="Sales">Sales</option>
          <option value="HR">HR</option>
        </select>
      </div>
      <button class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700">
        New Request
      </button>
    </div>

    <!-- Stats Cards -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-4">
        <p class="text-sm text-gray-600 dark:text-gray-400 mb-1">Total Requests</p>
        <p class="text-2xl font-bold dark:text-white">{{ requests.length }}</p>
      </div>
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-4">
        <p class="text-sm text-gray-600 dark:text-gray-400 mb-1">Pending Approval</p>
        <p class="text-2xl font-bold text-yellow-600">{{ requests.filter(r => r.status === 'Pending').length }}</p>
      </div>
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-4">
        <p class="text-sm text-gray-600 dark:text-gray-400 mb-1">Approved</p>
        <p class="text-2xl font-bold text-green-600">{{ requests.filter(r => r.status === 'Approved').length }}</p>
      </div>
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-4">
        <p class="text-sm text-gray-600 dark:text-gray-400 mb-1">Total Value</p>
        <p class="text-2xl font-bold text-blue-600">{{ formatCurrency(totalValue) }}</p>
      </div>
    </div>

    <div class="bg-white dark:bg-gray-800 rounded-lg shadow">
      <table class="w-full">
        <thead class="bg-gray-50 dark:bg-gray-700">
          <tr>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Request #</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Requestor</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Department</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Date</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Items</th>
            <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Est. Cost</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Priority</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Status</th>
            <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Actions</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-gray-200 dark:divide-gray-700">
          <tr v-for="request in filteredRequests" :key="request.id" class="dark:text-white hover:bg-gray-50 dark:hover:bg-gray-700">
            <td class="px-6 py-4 font-mono text-sm">{{ request.requestNumber }}</td>
            <td class="px-6 py-4">
              <div>
                <p class="font-medium">{{ request.requestor }}</p>
                <p class="text-xs text-gray-500 dark:text-gray-400">{{ request.requestorEmail }}</p>
              </div>
            </td>
            <td class="px-6 py-4 text-sm">{{ request.department }}</td>
            <td class="px-6 py-4 text-sm">{{ formatDate(request.date) }}</td>
            <td class="px-6 py-4 text-sm">
              <div>
                <p>{{ request.itemCount }} items</p>
                <p class="text-xs text-gray-500 dark:text-gray-400">{{ request.description }}</p>
              </div>
            </td>
            <td class="px-6 py-4 text-right font-medium">{{ formatCurrency(request.estimatedCost) }}</td>
            <td class="px-6 py-4">
              <span :class="getPriorityClass(request.priority)" class="px-2 py-1 text-xs rounded-full">
                {{ request.priority }}
              </span>
            </td>
            <td class="px-6 py-4">
              <span :class="getStatusClass(request.status)" class="px-2 py-1 text-xs rounded-full">
                {{ request.status }}
              </span>
            </td>
            <td class="px-6 py-4 text-right">
              <button class="text-blue-600 hover:text-blue-800 mr-3">View</button>
              <button v-if="request.status === 'Pending'" class="text-green-600 hover:text-green-800 mr-3">Approve</button>
              <button v-if="request.status === 'Approved'" class="text-purple-600 hover:text-purple-800">Create PO</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'

definePageMeta({
  middleware: ['auth'],
  layout: 'default',
})

useHead({
  title: 'Purchase Requests - TOSS ERP',
})

const searchQuery = ref('')
const filterStatus = ref('')
const filterDepartment = ref('')

const requests = ref([
  { id: 1, requestNumber: 'PR-2024-001', requestor: 'John Smith', requestorEmail: 'john@toss.com', department: 'Manufacturing', date: '2024-01-15', itemCount: 5, description: 'Raw materials', estimatedCost: 35000, priority: 'High', status: 'Approved' },
  { id: 2, requestNumber: 'PR-2024-002', requestor: 'Sarah Johnson', requestorEmail: 'sarah@toss.com', department: 'IT', date: '2024-01-18', itemCount: 3, description: 'Computer equipment', estimatedCost: 45000, priority: 'Medium', status: 'Pending' },
  { id: 3, requestNumber: 'PR-2024-003', requestor: 'Mike Williams', requestorEmail: 'mike@toss.com', department: 'Sales', date: '2024-01-22', itemCount: 8, description: 'Marketing materials', estimatedCost: 12000, priority: 'Low', status: 'Approved' },
  { id: 4, requestNumber: 'PR-2024-004', requestor: 'Emma Brown', requestorEmail: 'emma@toss.com', department: 'HR', date: '2024-02-01', itemCount: 2, description: 'Office furniture', estimatedCost: 25000, priority: 'Medium', status: 'Pending' },
  { id: 5, requestNumber: 'PR-2024-005', requestor: 'David Lee', requestorEmail: 'david@toss.com', department: 'Manufacturing', date: '2024-02-05', itemCount: 12, description: 'Machine parts', estimatedCost: 67000, priority: 'High', status: 'Ordered' },
])

const filteredRequests = computed(() => {
  let result = requests.value

  if (filterStatus.value) {
    result = result.filter(r => r.status === filterStatus.value)
  }

  if (filterDepartment.value) {
    result = result.filter(r => r.department === filterDepartment.value)
  }

  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    result = result.filter(r => 
      r.requestNumber.toLowerCase().includes(query) ||
      r.requestor.toLowerCase().includes(query) ||
      r.description.toLowerCase().includes(query)
    )
  }

  return result
})

const totalValue = computed(() => requests.value.reduce((sum, r) => sum + r.estimatedCost, 0))

const getStatusClass = (status: string) => {
  const classes = {
    'Pending': 'bg-yellow-100 text-yellow-800',
    'Approved': 'bg-green-100 text-green-800',
    'Rejected': 'bg-red-100 text-red-800',
    'Ordered': 'bg-blue-100 text-blue-800',
  }
  return classes[status as keyof typeof classes] || 'bg-gray-100 text-gray-800'
}

const getPriorityClass = (priority: string) => {
  const classes = {
    'High': 'bg-red-100 text-red-800',
    'Medium': 'bg-yellow-100 text-yellow-800',
    'Low': 'bg-blue-100 text-blue-800',
  }
  return classes[priority as keyof typeof classes] || 'bg-gray-100 text-gray-800'
}

const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-ZA', { style: 'currency', currency: 'ZAR' }).format(amount)
}

const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString('en-ZA')
}
</script>
