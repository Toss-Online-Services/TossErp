<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <!-- Page Header -->
    <div class="bg-white dark:bg-gray-800 shadow-sm border-b border-gray-200 dark:border-gray-700">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="py-4">
          <div class="flex items-center justify-between">
            <div>
              <h1 class="text-2xl font-bold text-gray-900 dark:text-white">Employee Management</h1>
              <p class="text-gray-600 dark:text-gray-400">Manage staff records and employment details</p>
            </div>
            <div class="flex space-x-3">
              <button @click="showAddEmployeeModal = true" class="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors">
                <UserPlusIcon class="w-5 h-5 inline mr-2" />
                Add Employee
              </button>
              <button @click="exportEmployees" class="bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700 transition-colors">
                <DocumentIcon class="w-5 h-5 inline mr-2" />
                Export
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Stats Cards -->
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
      <div class="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
          <div class="flex items-center">
            <UsersIcon class="w-8 h-8 text-blue-600" />
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Total Employees</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ stats.totalEmployees }}</p>
            </div>
          </div>
        </div>
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
          <div class="flex items-center">
            <CheckCircleIcon class="w-8 h-8 text-green-600" />
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Active</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ stats.activeEmployees }}</p>
            </div>
          </div>
        </div>
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
          <div class="flex items-center">
            <ClockIcon class="w-8 h-8 text-yellow-600" />
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">On Leave</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ stats.onLeave }}</p>
            </div>
          </div>
        </div>
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
          <div class="flex items-center">
            <CurrencyDollarIcon class="w-8 h-8 text-purple-600" />
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Avg Salary</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">R{{ stats.avgSalary.toLocaleString() }}</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Filters -->
      <div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 mb-6">
        <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Search</label>
            <input v-model="searchTerm" placeholder="Search employees..." class="w-full p-2 border border-gray-300 rounded-lg dark:bg-gray-700 dark:border-gray-600 dark:text-white">
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Department</label>
            <select v-model="selectedDepartment" class="w-full p-2 border border-gray-300 rounded-lg dark:bg-gray-700 dark:border-gray-600 dark:text-white">
              <option value="">All Departments</option>
              <option value="management">Management</option>
              <option value="sales">Sales</option>
              <option value="operations">Operations</option>
              <option value="finance">Finance</option>
            </select>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Status</label>
            <select v-model="selectedStatus" class="w-full p-2 border border-gray-300 rounded-lg dark:bg-gray-700 dark:border-gray-600 dark:text-white">
              <option value="">All Status</option>
              <option value="active">Active</option>
              <option value="on-leave">On Leave</option>
              <option value="probation">Probation</option>
            </select>
          </div>
          <div class="flex items-end">
            <button @click="clearFilters" class="w-full bg-gray-500 text-white px-4 py-2 rounded-lg hover:bg-gray-600 transition-colors">
              Clear Filters
            </button>
          </div>
        </div>
      </div>

      <!-- Employees Table -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 overflow-hidden">
        <div class="overflow-x-auto">
          <table class="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
            <thead class="bg-gray-50 dark:bg-gray-700">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Employee</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Position</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Department</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Salary</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Status</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Hire Date</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Actions</th>
              </tr>
            </thead>
            <tbody class="bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700">
              <tr v-for="employee in filteredEmployees" :key="employee.id" class="hover:bg-gray-50 dark:hover:bg-gray-700">
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="flex items-center">
                    <div class="flex-shrink-0 h-10 w-10">
                      <div class="h-10 w-10 rounded-full bg-blue-500 flex items-center justify-center">
                        <span class="text-white font-medium">{{ employee.name.split(' ').map(n => n[0]).join('') }}</span>
                      </div>
                    </div>
                    <div class="ml-4">
                      <div class="text-sm font-medium text-gray-900 dark:text-white">{{ employee.name }}</div>
                      <div class="text-sm text-gray-500 dark:text-gray-400">{{ employee.email }}</div>
                    </div>
                  </div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">{{ employee.position }}</td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400 capitalize">{{ employee.department }}</td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">R{{ employee.salary.toLocaleString() }}</td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span :class="getStatusColor(employee.status)" class="px-2 inline-flex text-xs leading-5 font-semibold rounded-full">
                    {{ employee.status }}
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400">
                  {{ formatDate(employee.hireDate) }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                  <button @click="editEmployee(employee)" class="text-blue-600 hover:text-blue-900 mr-3">Edit</button>
                  <button @click="viewEmployee(employee)" class="text-green-600 hover:text-green-900">View</button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>

    <!-- Add Employee Modal -->
    <div v-if="showAddEmployeeModal" class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
      <div class="relative top-20 mx-auto p-5 border w-96 shadow-lg rounded-md bg-white dark:bg-gray-800">
        <div class="mt-3">
          <h3 class="text-lg font-medium text-gray-900 dark:text-white text-center">Add New Employee</h3>
          <div class="mt-4 space-y-4">
            <input v-model="newEmployee.name" placeholder="Full Name" class="w-full p-2 border border-gray-300 rounded-lg dark:bg-gray-700 dark:border-gray-600 dark:text-white">
            <input v-model="newEmployee.email" placeholder="Email" type="email" class="w-full p-2 border border-gray-300 rounded-lg dark:bg-gray-700 dark:border-gray-600 dark:text-white">
            <input v-model="newEmployee.position" placeholder="Position" class="w-full p-2 border border-gray-300 rounded-lg dark:bg-gray-700 dark:border-gray-600 dark:text-white">
            <select v-model="newEmployee.department" class="w-full p-2 border border-gray-300 rounded-lg dark:bg-gray-700 dark:border-gray-600 dark:text-white">
              <option value="">Select Department</option>
              <option value="management">Management</option>
              <option value="sales">Sales</option>
              <option value="operations">Operations</option>
              <option value="finance">Finance</option>
            </select>
            <input v-model="newEmployee.salary" placeholder="Salary" type="number" class="w-full p-2 border border-gray-300 rounded-lg dark:bg-gray-700 dark:border-gray-600 dark:text-white">
          </div>
          <div class="items-center px-4 py-3">
            <button @click="addEmployee" class="px-4 py-2 bg-blue-500 text-white text-base font-medium rounded-md w-full shadow-sm hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-300">
              Add Employee
            </button>
            <button @click="showAddEmployeeModal = false" class="mt-3 px-4 py-2 bg-gray-500 text-white text-base font-medium rounded-md w-full shadow-sm hover:bg-gray-600 focus:outline-none focus:ring-2 focus:ring-gray-300">
              Cancel
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'

definePageMeta({
  title: 'Employee Management - TOSS ERP',
  description: 'Manage employee records and HR information for your business'
})

// Icons (using HeroIcons naming convention)
const UserPlusIcon = 'svg'
const DocumentIcon = 'svg'
const UsersIcon = 'svg'
const CheckCircleIcon = 'svg'
const ClockIcon = 'svg'
const CurrencyDollarIcon = 'svg'

// Reactive data
const searchTerm = ref('')
const selectedDepartment = ref('')
const selectedStatus = ref('')
const showAddEmployeeModal = ref(false)

const stats = ref({
  totalEmployees: 8,
  activeEmployees: 6,
  onLeave: 2,
  avgSalary: 22500
})

const employees = ref([
  {
    id: '1',
    name: 'Thabo Mthembu',
    email: 'thabo@thabosspaza.co.za',
    position: 'Store Owner/Manager',
    department: 'management',
    status: 'active',
    salary: 35000,
    hireDate: new Date('2020-01-15')
  },
  {
    id: '2',
    name: 'Nomsa Khumalo',
    email: 'nomsa@thabosspaza.co.za',
    position: 'Assistant Manager',
    department: 'management',
    status: 'active',
    salary: 22000,
    hireDate: new Date('2021-03-20')
  },
  {
    id: '3',
    name: 'Sipho Dube',
    email: 'sipho@thabosspaza.co.za',
    position: 'Sales Clerk',
    department: 'sales',
    status: 'active',
    salary: 15000,
    hireDate: new Date('2022-06-10')
  },
  {
    id: '4',
    name: 'Lerato Masango',
    email: 'lerato@thabosspaza.co.za',
    position: 'Cashier',
    department: 'sales',
    status: 'active',
    salary: 12000,
    hireDate: new Date('2022-08-05')
  },
  {
    id: '5',
    name: 'Mandla Ndaba',
    email: 'mandla@thabosspaza.co.za',
    position: 'Stock Controller',
    department: 'operations',
    status: 'active',
    salary: 18000,
    hireDate: new Date('2021-11-12')
  },
  {
    id: '6',
    name: 'Grace Mthethwa',
    email: 'grace@thabosspaza.co.za',
    position: 'Cleaner',
    department: 'operations',
    status: 'on-leave',
    salary: 8000,
    hireDate: new Date('2023-02-01')
  },
  {
    id: '7',
    name: 'Bongani Sithole',
    email: 'bongani@thabosspaza.co.za',
    position: 'Security Guard',
    department: 'operations',
    status: 'active',
    salary: 10000,
    hireDate: new Date('2022-09-15')
  },
  {
    id: '8',
    name: 'Zinhle Mpofu',
    email: 'zinhle@thabosspaza.co.za',
    position: 'Bookkeeper',
    department: 'finance',
    status: 'on-leave',
    salary: 20000,
    hireDate: new Date('2021-07-20')
  }
])

const newEmployee = ref({
  name: '',
  email: '',
  position: '',
  department: '',
  salary: 0
})

// Computed properties
const filteredEmployees = computed(() => {
  return employees.value.filter(employee => {
    const matchesSearch = !searchTerm.value || 
      employee.name.toLowerCase().includes(searchTerm.value.toLowerCase()) ||
      employee.email.toLowerCase().includes(searchTerm.value.toLowerCase()) ||
      employee.position.toLowerCase().includes(searchTerm.value.toLowerCase())
    
    const matchesDepartment = !selectedDepartment.value || employee.department === selectedDepartment.value
    const matchesStatus = !selectedStatus.value || employee.status === selectedStatus.value
    
    return matchesSearch && matchesDepartment && matchesStatus
  })
})

// Methods
const getStatusColor = (status) => {
  switch (status) {
    case 'active':
      return 'bg-green-100 text-green-800'
    case 'on-leave':
      return 'bg-yellow-100 text-yellow-800'
    case 'probation':
      return 'bg-blue-100 text-blue-800'
    default:
      return 'bg-gray-100 text-gray-800'
  }
}

const formatDate = (date) => {
  return new Date(date).toLocaleDateString('en-ZA')
}

const addEmployee = () => {
  if (newEmployee.value.name && newEmployee.value.email && newEmployee.value.position) {
    const employee = {
      id: (employees.value.length + 1).toString(),
      ...newEmployee.value,
      status: 'probation',
      hireDate: new Date()
    }
    employees.value.push(employee)
    newEmployee.value = { name: '', email: '', position: '', department: '', salary: 0 }
    showAddEmployeeModal.value = false
    // Update stats
    stats.value.totalEmployees++
  }
}

const editEmployee = (employee) => {
  // Implementation for editing employee
  console.log('Edit employee:', employee)
}

const viewEmployee = (employee) => {
  // Implementation for viewing employee details
  console.log('View employee:', employee)
}

const exportEmployees = () => {
  // Implementation for exporting employee data
  console.log('Exporting employees...')
}

const clearFilters = () => {
  searchTerm.value = ''
  selectedDepartment.value = ''
  selectedStatus.value = ''
}

useHead({
  title: 'Employee Management - TOSS ERP',
  meta: [
    { name: 'description', content: 'Employee management and HR records in TOSS ERP' }
  ]
})
</script>