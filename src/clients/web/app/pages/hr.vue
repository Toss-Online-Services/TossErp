<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <!-- Page Header -->
    <div class="bg-white dark:bg-gray-800 shadow">
      <div class="max-w-7xl mx-auto py-6 px-4 sm:px-6 lg:px-8">
        <div class="md:flex md:items-center md:justify-between">
          <div class="flex-1 min-w-0">
            <h2 class="text-2xl font-bold leading-7 text-gray-900 dark:text-white sm:text-3xl sm:truncate">
              Human Resources
            </h2>
            <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
              Manage employees, payroll, benefits, and HR operations
            </p>
          </div>
          <div class="mt-4 flex md:mt-0 md:ml-4 space-x-3">
            <button
              @click="showAddEmployeeModal = true"
              type="button"
              class="inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-orange-600 hover:bg-orange-700"
            >
              <svg class="-ml-1 mr-2 h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
              </svg>
              Add Employee
            </button>
          </div>
        </div>
      </div>
    </div>

    <div class="max-w-7xl mx-auto py-6 px-4 sm:px-6 lg:px-8">
      <!-- HR Stats Cards -->
      <div class="grid grid-cols-1 gap-5 sm:grid-cols-2 lg:grid-cols-4 mb-8">
        <div class="bg-white dark:bg-gray-800 overflow-hidden shadow rounded-lg">
          <div class="p-5">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <svg class="h-6 w-6 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" />
                </svg>
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 dark:text-gray-400 truncate">Total Employees</dt>
                  <dd class="text-lg font-medium text-gray-900 dark:text-white">{{ totalEmployees }}</dd>
                </dl>
              </div>
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 overflow-hidden shadow rounded-lg">
          <div class="p-5">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <svg class="h-6 w-6 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 dark:text-gray-400 truncate">Monthly Payroll</dt>
                  <dd class="text-lg font-medium text-gray-900 dark:text-white">{{ formatCurrency(monthlyPayroll) }}</dd>
                </dl>
              </div>
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 overflow-hidden shadow rounded-lg">
          <div class="p-5">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <svg class="h-6 w-6 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 dark:text-gray-400 truncate">Active Employees</dt>
                  <dd class="text-lg font-medium text-gray-900 dark:text-white">{{ activeEmployees }}</dd>
                </dl>
              </div>
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 overflow-hidden shadow rounded-lg">
          <div class="p-5">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <svg class="h-6 w-6 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3a2 2 0 012-2h4a2 2 0 012 2v4m-6 0h6m-6 0V17a2 2 0 002 2h4a2 2 0 002-2V7m-6 0h6m-6 0V17a2 2 0 002 2h4a2 2 0 002-2V7" />
                </svg>
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 dark:text-gray-400 truncate">On Leave Today</dt>
                  <dd class="text-lg font-medium text-gray-900 dark:text-white">{{ onLeaveToday }}</dd>
                </dl>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
        <!-- Main HR Content -->
        <div class="lg:col-span-2 space-y-6">
          <!-- Employee List -->
          <div class="bg-white dark:bg-gray-800 shadow rounded-lg">
            <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
              <div class="flex items-center justify-between">
                <h3 class="text-lg font-medium text-gray-900 dark:text-white">Employee Directory</h3>
                <div class="flex items-center space-x-3">
                  <div class="relative">
                    <input
                      v-model="searchTerm"
                      type="text"
                      placeholder="Search employees..."
                      class="w-64 pl-10 pr-4 py-2 border border-gray-300 dark:border-gray-600 rounded-md text-sm focus:ring-orange-500 focus:border-orange-500 dark:bg-gray-700 dark:text-white"
                    />
                    <svg class="absolute left-3 top-2.5 h-4 w-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
                    </svg>
                  </div>
                  <select
                    v-model="selectedDepartment"
                    class="border border-gray-300 dark:border-gray-600 rounded-md px-3 py-2 text-sm focus:ring-orange-500 focus:border-orange-500 dark:bg-gray-700 dark:text-white"
                  >
                    <option value="">All Departments</option>
                    <option value="management">Management</option>
                    <option value="sales">Sales</option>
                    <option value="operations">Operations</option>
                    <option value="finance">Finance</option>
                    <option value="admin">Administration</option>
                  </select>
                </div>
              </div>
            </div>
            <div class="overflow-x-auto">
              <table class="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
                <thead class="bg-gray-50 dark:bg-gray-700">
                  <tr>
                    <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Employee</th>
                    <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Position</th>
                    <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Department</th>
                    <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Status</th>
                    <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Salary</th>
                    <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Actions</th>
                  </tr>
                </thead>
                <tbody class="bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700">
                  <tr v-for="employee in filteredEmployees" :key="employee.id" class="hover:bg-gray-50 dark:hover:bg-gray-700">
                    <td class="px-6 py-4 whitespace-nowrap">
                      <div class="flex items-center">
                        <div class="h-10 w-10 flex-shrink-0">
                          <div class="h-10 w-10 rounded-full bg-gray-300 dark:bg-gray-600 flex items-center justify-center">
                            <span class="text-sm font-medium text-gray-700 dark:text-gray-300">
                              {{ employee.name.split(' ').map(n => n[0]).join('') }}
                            </span>
                          </div>
                        </div>
                        <div class="ml-4">
                          <div class="text-sm font-medium text-gray-900 dark:text-white">{{ employee.name }}</div>
                          <div class="text-sm text-gray-500 dark:text-gray-400">{{ employee.email }}</div>
                        </div>
                      </div>
                    </td>
                    <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">{{ employee.position }}</td>
                    <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">{{ employee.department }}</td>
                    <td class="px-6 py-4 whitespace-nowrap">
                      <span
                        class="inline-flex px-2 py-1 text-xs font-semibold rounded-full"
                        :class="getStatusColor(employee.status)"
                      >
                        {{ employee.status }}
                      </span>
                    </td>
                    <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">{{ formatCurrency(employee.salary) }}</td>
                    <td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
                      <button
                        @click="editEmployee(employee)"
                        class="text-orange-600 hover:text-orange-900 mr-3"
                      >
                        Edit
                      </button>
                      <button
                        @click="viewEmployee(employee)"
                        class="text-blue-600 hover:text-blue-900"
                      >
                        View
                      </button>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>

          <!-- Payroll Overview -->
          <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-4">Payroll Overview</h3>
            <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
              <div class="text-center">
                <div class="text-2xl font-bold text-gray-900 dark:text-white">{{ formatCurrency(payrollStats.gross) }}</div>
                <div class="text-sm text-gray-500 dark:text-gray-400">Gross Payroll</div>
              </div>
              <div class="text-center">
                <div class="text-2xl font-bold text-gray-900 dark:text-white">{{ formatCurrency(payrollStats.deductions) }}</div>
                <div class="text-sm text-gray-500 dark:text-gray-400">Total Deductions</div>
              </div>
              <div class="text-center">
                <div class="text-2xl font-bold text-gray-900 dark:text-white">{{ formatCurrency(payrollStats.net) }}</div>
                <div class="text-sm text-gray-500 dark:text-gray-400">Net Payroll</div>
              </div>
            </div>
          </div>
        </div>

        <!-- HR Management Panel -->
        <div class="space-y-6">
          <!-- Quick Actions -->
          <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-4">Quick Actions</h3>
            <div class="space-y-3">
              <button
                @click="processPayroll"
                class="w-full text-left px-4 py-3 border border-gray-200 dark:border-gray-600 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
              >
                <div class="flex items-center">
                  <svg class="w-5 h-5 text-green-500 mr-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                  </svg>
                  <div>
                    <div class="text-sm font-medium text-gray-900 dark:text-white">Process Payroll</div>
                    <div class="text-xs text-gray-500 dark:text-gray-400">Generate monthly payroll</div>
                  </div>
                </div>
              </button>

              <button
                @click="generateReports"
                class="w-full text-left px-4 py-3 border border-gray-200 dark:border-gray-600 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
              >
                <div class="flex items-center">
                  <svg class="w-5 h-5 text-blue-500 mr-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
                  </svg>
                  <div>
                    <div class="text-sm font-medium text-gray-900 dark:text-white">HR Reports</div>
                    <div class="text-xs text-gray-500 dark:text-gray-400">Generate HR analytics</div>
                  </div>
                </div>
              </button>

              <button
                @click="manageLeave"
                class="w-full text-left px-4 py-3 border border-gray-200 dark:border-gray-600 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
              >
                <div class="flex items-center">
                  <svg class="w-5 h-5 text-yellow-500 mr-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3a2 2 0 012-2h4a2 2 0 012 2v4m-6 0h6m-6 0V17a2 2 0 002 2h4a2 2 0 002-2V7m-6 0h6m-6 0V17a2 2 0 002 2h4a2 2 0 002-2V7" />
                  </svg>
                  <div>
                    <div class="text-sm font-medium text-gray-900 dark:text-white">Manage Leave</div>
                    <div class="text-xs text-gray-500 dark:text-gray-400">Leave requests & calendar</div>
                  </div>
                </div>
              </button>

              <button
                @click="manageBenefits"
                class="w-full text-left px-4 py-3 border border-gray-200 dark:border-gray-600 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
              >
                <div class="flex items-center">
                  <svg class="w-5 h-5 text-purple-500 mr-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4.318 6.318a4.5 4.5 0 000 6.364L12 20.364l7.682-7.682a4.5 4.5 0 00-6.364-6.364L12 7.636l-1.318-1.318a4.5 4.5 0 00-6.364 0z" />
                  </svg>
                  <div>
                    <div class="text-sm font-medium text-gray-900 dark:text-white">Employee Benefits</div>
                    <div class="text-xs text-gray-500 dark:text-gray-400">Manage benefits & perks</div>
                  </div>
                </div>
              </button>
            </div>
          </div>

          <!-- Recent Activities -->
          <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-4">Recent HR Activities</h3>
            <div class="space-y-3">
              <div v-for="activity in recentActivities" :key="activity.id" class="flex items-start">
                <div class="flex-shrink-0">
                  <div class="w-8 h-8 rounded-full flex items-center justify-center" :class="activity.color">
                    <svg class="w-4 h-4 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" :d="activity.icon" />
                    </svg>
                  </div>
                </div>
                <div class="ml-3">
                  <p class="text-sm text-gray-900 dark:text-white">{{ activity.description }}</p>
                  <p class="text-xs text-gray-500 dark:text-gray-400">{{ formatTime(activity.timestamp) }}</p>
                </div>
              </div>
            </div>
          </div>

          <!-- Department Overview -->
          <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-4">Department Overview</h3>
            <div class="space-y-3">
              <div v-for="dept in departmentStats" :key="dept.name" class="flex items-center justify-between">
                <div class="flex items-center">
                  <div class="w-3 h-3 rounded-full mr-3" :class="dept.color"></div>
                  <span class="text-sm text-gray-900 dark:text-white">{{ dept.name }}</span>
                </div>
                <span class="text-sm font-medium text-gray-900 dark:text-white">{{ dept.count }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Add Employee Modal -->
    <div v-if="showAddEmployeeModal" class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
      <div class="relative top-20 mx-auto p-5 border w-96 shadow-lg rounded-md bg-white dark:bg-gray-800">
        <div class="mt-3">
          <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-4">Add New Employee</h3>
          <form @submit.prevent="addEmployee" class="space-y-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300">Full Name</label>
              <input
                v-model="newEmployee.name"
                type="text"
                required
                class="mt-1 block w-full border border-gray-300 dark:border-gray-600 rounded-md px-3 py-2 text-sm focus:ring-orange-500 focus:border-orange-500 dark:bg-gray-700 dark:text-white"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300">Email</label>
              <input
                v-model="newEmployee.email"
                type="email"
                required
                class="mt-1 block w-full border border-gray-300 dark:border-gray-600 rounded-md px-3 py-2 text-sm focus:ring-orange-500 focus:border-orange-500 dark:bg-gray-700 dark:text-white"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300">Position</label>
              <input
                v-model="newEmployee.position"
                type="text"
                required
                class="mt-1 block w-full border border-gray-300 dark:border-gray-600 rounded-md px-3 py-2 text-sm focus:ring-orange-500 focus:border-orange-500 dark:bg-gray-700 dark:text-white"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300">Department</label>
              <select
                v-model="newEmployee.department"
                required
                class="mt-1 block w-full border border-gray-300 dark:border-gray-600 rounded-md px-3 py-2 text-sm focus:ring-orange-500 focus:border-orange-500 dark:bg-gray-700 dark:text-white"
              >
                <option value="">Select Department</option>
                <option value="management">Management</option>
                <option value="sales">Sales</option>
                <option value="operations">Operations</option>
                <option value="finance">Finance</option>
                <option value="admin">Administration</option>
              </select>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300">Salary</label>
              <input
                v-model.number="newEmployee.salary"
                type="number"
                required
                class="mt-1 block w-full border border-gray-300 dark:border-gray-600 rounded-md px-3 py-2 text-sm focus:ring-orange-500 focus:border-orange-500 dark:bg-gray-700 dark:text-white"
              />
            </div>
            <div class="flex justify-end space-x-3 pt-4">
              <button
                type="button"
                @click="showAddEmployeeModal = false"
                class="px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-md text-sm font-medium text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-700"
              >
                Cancel
              </button>
              <button
                type="submit"
                class="px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-orange-600 hover:bg-orange-700"
              >
                Add Employee
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'

// Types
interface Employee {
  id: string
  name: string
  email: string
  position: string
  department: string
  status: 'active' | 'inactive' | 'on-leave'
  salary: number
  hireDate: Date
}

interface Activity {
  id: string
  description: string
  timestamp: Date
  icon: string
  color: string
}

// Reactive data
const totalEmployees = ref(45)
const monthlyPayroll = ref(342500)
const activeEmployees = ref(42)
const onLeaveToday = ref(3)

const searchTerm = ref('')
const selectedDepartment = ref('')
const showAddEmployeeModal = ref(false)

// New employee form
const newEmployee = ref({
  name: '',
  email: '',
  position: '',
  department: '',
  salary: 0
})

// Payroll stats
const payrollStats = ref({
  gross: 342500,
  deductions: 68500,
  net: 274000
})

// Sample employees data
const employees = ref<Employee[]>([
  {
    id: '1',
    name: 'Thabo Mthembu',
    email: 'thabo.mthembu@toss.co.za',
    position: 'Store Manager',
    department: 'management',
    status: 'active',
    salary: 35000,
    hireDate: new Date('2022-01-15')
  },
  {
    id: '2',
    name: 'Nomsa Dlamini',
    email: 'nomsa.dlamini@toss.co.za',
    position: 'Sales Associate',
    department: 'sales',
    status: 'active',
    salary: 18000,
    hireDate: new Date('2022-03-20')
  },
  {
    id: '3',
    name: 'Sipho Nkomo',
    email: 'sipho.nkomo@toss.co.za',
    position: 'Inventory Clerk',
    department: 'operations',
    status: 'active',
    salary: 15000,
    hireDate: new Date('2021-11-10')
  },
  {
    id: '4',
    name: 'Lerato Mokgoro',
    email: 'lerato.mokgoro@toss.co.za',
    position: 'Accountant',
    department: 'finance',
    status: 'on-leave',
    salary: 28000,
    hireDate: new Date('2021-08-05')
  },
  {
    id: '5',
    name: 'Mandla Sithole',
    email: 'mandla.sithole@toss.co.za',
    position: 'Cashier',
    department: 'sales',
    status: 'active',
    salary: 12000,
    hireDate: new Date('2023-02-14')
  }
])

// Recent activities
const recentActivities = ref<Activity[]>([
  {
    id: '1',
    description: 'Payroll processed for February 2024',
    timestamp: new Date(Date.now() - 3600000),
    icon: 'M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z',
    color: 'bg-green-500'
  },
  {
    id: '2',
    description: 'New employee Mandla Sithole added',
    timestamp: new Date(Date.now() - 7200000),
    icon: 'M12 6v6m0 0v6m0-6h6m-6 0H6',
    color: 'bg-blue-500'
  },
  {
    id: '3',
    description: 'Leave request approved for Lerato Mokgoro',
    timestamp: new Date(Date.now() - 10800000),
    icon: 'M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z',
    color: 'bg-yellow-500'
  }
])

// Department statistics
const departmentStats = ref([
  { name: 'Management', count: 3, color: 'bg-blue-500' },
  { name: 'Sales', count: 15, color: 'bg-green-500' },
  { name: 'Operations', count: 12, color: 'bg-yellow-500' },
  { name: 'Finance', count: 8, color: 'bg-purple-500' },
  { name: 'Administration', count: 7, color: 'bg-red-500' }
])

// Computed properties
const filteredEmployees = computed(() => {
  return employees.value.filter(employee => {
    const matchesSearch = employee.name.toLowerCase().includes(searchTerm.value.toLowerCase()) ||
                         employee.email.toLowerCase().includes(searchTerm.value.toLowerCase()) ||
                         employee.position.toLowerCase().includes(searchTerm.value.toLowerCase())
    
    const matchesDepartment = selectedDepartment.value === '' || employee.department === selectedDepartment.value
    
    return matchesSearch && matchesDepartment
  })
})

// Methods
const formatCurrency = (amount: number): string => {
  return new Intl.NumberFormat('en-ZA', {
    style: 'currency',
    currency: 'ZAR',
    minimumFractionDigits: 0,
    maximumFractionDigits: 0
  }).format(amount)
}

const formatTime = (date: Date): string => {
  return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
}

const getStatusColor = (status: string): string => {
  switch (status) {
    case 'active': return 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-300'
    case 'inactive': return 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-300'
    case 'on-leave': return 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-300'
    default: return 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300'
  }
}

const addEmployee = () => {
  const employee: Employee = {
    id: Date.now().toString(),
    name: newEmployee.value.name,
    email: newEmployee.value.email,
    position: newEmployee.value.position,
    department: newEmployee.value.department,
    status: 'active',
    salary: newEmployee.value.salary,
    hireDate: new Date()
  }
  
  employees.value.push(employee)
  showAddEmployeeModal.value = false
  
  // Reset form
  newEmployee.value = {
    name: '',
    email: '',
    position: '',
    department: '',
    salary: 0
  }
  
  // Update totals
  totalEmployees.value++
  activeEmployees.value++
  monthlyPayroll.value += employee.salary
}

const editEmployee = (employee: Employee) => {
  console.log('Editing employee:', employee.name)
  // Implement edit functionality
}

const viewEmployee = (employee: Employee) => {
  console.log('Viewing employee:', employee.name)
  // Implement view functionality
}

const processPayroll = () => {
  console.log('Processing payroll...')
  // Implement payroll processing
}

const generateReports = () => {
  console.log('Generating HR reports...')
  // Implement report generation
}

const manageLeave = () => {
  console.log('Managing leave requests...')
  // Implement leave management
}

const manageBenefits = () => {
  console.log('Managing employee benefits...')
  // Implement benefits management
}
</script>

<style scoped>
/* Component-specific styles */
</style>
