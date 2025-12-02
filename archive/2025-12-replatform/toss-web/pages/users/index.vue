<template>
  <div class="min-h-screen bg-gradient-to-br from-slate-50 via-purple-50/30 to-slate-100 dark:from-slate-900 dark:via-slate-900 dark:to-slate-800">
    <!-- Page Header -->
    <div class="bg-white/80 dark:bg-slate-800/80 backdrop-blur-xl shadow-sm border-b border-slate-200/50 dark:border-slate-700/50 sticky top-0 z-10">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4 sm:py-6">
        <div class="flex items-center justify-between">
          <div class="flex-1 min-w-0">
            <h1 class="text-2xl sm:text-3xl font-bold bg-gradient-to-r from-purple-600 to-pink-600 bg-clip-text text-transparent">
              User Management
            </h1>
            <p class="mt-1 text-sm text-slate-600 dark:text-slate-400">
              Manage shop staff and access permissions
            </p>
          </div>
          <div class="flex space-x-2 sm:space-x-3 flex-shrink-0">
            <button
              @click="showAddModal = true"
              class="inline-flex items-center justify-center px-4 sm:px-6 py-2.5 sm:py-3 bg-gradient-to-r from-purple-600 to-pink-600 text-white rounded-xl hover:from-purple-700 hover:to-pink-700 shadow-lg hover:shadow-xl transition-all duration-200 transform hover:scale-105 font-semibold text-sm sm:text-base"
            >
              <PlusIcon class="w-5 h-5 mr-2" />
              Add User
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Main Content -->
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <!-- Stats Cards -->
      <div class="grid grid-cols-2 sm:grid-cols-4 gap-4 sm:gap-6 mb-8">
        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">Total Users</p>
              <p class="text-3xl font-bold text-slate-900 dark:text-white">{{ users.length }}</p>
            </div>
            <div class="p-3 bg-gradient-to-br from-purple-500 to-pink-600 rounded-xl">
              <UsersIcon class="w-8 h-8 text-white" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">Active</p>
              <p class="text-3xl font-bold text-slate-900 dark:text-white">{{ activeUsers }}</p>
            </div>
            <div class="p-3 bg-gradient-to-br from-green-500 to-emerald-600 rounded-xl">
              <CheckCircleIcon class="w-8 h-8 text-white" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">Owners</p>
              <p class="text-3xl font-bold text-slate-900 dark:text-white">{{ ownerCount }}</p>
            </div>
            <div class="p-3 bg-gradient-to-br from-blue-500 to-indigo-600 rounded-xl">
              <KeyIcon class="w-8 h-8 text-white" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">Cashiers</p>
              <p class="text-3xl font-bold text-slate-900 dark:text-white">{{ cashierCount }}</p>
            </div>
            <div class="p-3 bg-gradient-to-br from-orange-500 to-amber-600 rounded-xl">
              <UserIcon class="w-8 h-8 text-white" />
            </div>
          </div>
        </div>
      </div>

      <!-- Filters -->
      <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 mb-6">
        <div class="flex flex-col sm:flex-row gap-4">
          <div class="flex-1">
            <input
              v-model="searchQuery"
              type="text"
              placeholder="Search users by name, email, or phone..."
              class="w-full px-4 py-2.5 border border-slate-300 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-purple-500 dark:bg-slate-700 dark:text-white"
            />
          </div>
          <select
            v-model="roleFilter"
            class="px-4 py-2.5 border border-slate-300 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-purple-500 dark:bg-slate-700 dark:text-white"
          >
            <option value="">All Roles</option>
            <option value="owner">Owner</option>
            <option value="cashier">Cashier</option>
            <option value="driver">Driver</option>
          </select>
          <select
            v-model="statusFilter"
            class="px-4 py-2.5 border border-slate-300 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-purple-500 dark:bg-slate-700 dark:text-white"
          >
            <option value="">All Status</option>
            <option value="active">Active</option>
            <option value="inactive">Inactive</option>
          </select>
        </div>
      </div>

      <!-- Users List -->
      <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 overflow-hidden">
        <div class="overflow-x-auto">
          <table class="min-w-full divide-y divide-slate-200 dark:divide-slate-700">
            <thead class="bg-slate-50 dark:bg-slate-700">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-300 uppercase tracking-wider">
                  User
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-300 uppercase tracking-wider">
                  Contact
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-300 uppercase tracking-wider">
                  Role
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-300 uppercase tracking-wider">
                  Status
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-300 uppercase tracking-wider">
                  Last Active
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-300 uppercase tracking-wider">
                  Actions
                </th>
              </tr>
            </thead>
            <tbody class="bg-white dark:bg-slate-800 divide-y divide-slate-200 dark:divide-slate-700">
              <tr
                v-for="user in filteredUsers"
                :key="user.id"
                class="hover:bg-slate-50 dark:hover:bg-slate-700 transition-colors"
              >
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="flex items-center">
                    <div class="flex-shrink-0 h-10 w-10 bg-purple-100 dark:bg-purple-900 rounded-full flex items-center justify-center">
                      <span class="text-purple-600 dark:text-purple-300 font-semibold">
                        {{ getInitials(user.name) }}
                      </span>
                    </div>
                    <div class="ml-4">
                      <div class="text-sm font-medium text-slate-900 dark:text-white">{{ user.name }}</div>
                      <div class="text-sm text-slate-500 dark:text-slate-400">ID: {{ user.id }}</div>
                    </div>
                  </div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="text-sm text-slate-900 dark:text-white">{{ user.email }}</div>
                  <div class="text-sm text-slate-500 dark:text-slate-400">{{ user.phone }}</div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span
                    class="px-3 py-1 inline-flex text-xs leading-5 font-semibold rounded-full"
                    :class="getRoleBadge(user.role)"
                  >
                    {{ getRoleLabel(user.role) }}
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span
                    class="px-3 py-1 inline-flex text-xs leading-5 font-semibold rounded-full"
                    :class="getStatusBadge(user.status)"
                  >
                    {{ user.status }}
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-slate-500 dark:text-slate-400">
                  {{ formatDate(user.lastActive) }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm font-medium space-x-2">
                  <button
                    @click="editUser(user)"
                    class="text-blue-600 dark:text-blue-400 hover:text-blue-900 dark:hover:text-blue-300"
                  >
                    Edit
                  </button>
                  <button
                    v-if="user.status === 'active'"
                    @click="toggleUserStatus(user)"
                    class="text-orange-600 dark:text-orange-400 hover:text-orange-900 dark:hover:text-orange-300"
                  >
                    Deactivate
                  </button>
                  <button
                    v-else
                    @click="toggleUserStatus(user)"
                    class="text-green-600 dark:text-green-400 hover:text-green-900 dark:hover:text-green-300"
                  >
                    Activate
                  </button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>

    <!-- Add/Edit User Modal -->
    <transition name="modal">
      <div
        v-if="showAddModal"
        class="fixed inset-0 bg-black/50 backdrop-blur-sm z-50 overflow-y-auto"
        @click="showAddModal = false"
      >
        <div class="flex min-h-full items-center justify-center p-4">
          <div
            class="relative bg-white dark:bg-slate-800 rounded-2xl shadow-2xl border border-slate-200 dark:border-slate-700 w-full max-w-2xl"
            @click.stop
          >
            <!-- Header -->
            <div class="bg-gradient-to-r from-purple-600 to-pink-600 px-6 py-4 flex items-center justify-between rounded-t-2xl">
              <div>
                <h3 class="text-xl font-bold text-white">{{ editingUser ? 'Edit User' : 'Add New User' }}</h3>
                <p class="text-sm text-white/80">{{ editingUser ? 'Update user details' : 'Create a new staff member' }}</p>
              </div>
              <button
                @click="showAddModal = false"
                class="p-2 hover:bg-white/20 rounded-lg transition-colors"
              >
                <XMarkIcon class="w-6 h-6 text-white" />
              </button>
            </div>

            <!-- Form -->
            <form @submit.prevent="saveUser" class="p-6 space-y-4">
              <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                  <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                    Full Name *
                  </label>
                  <input
                    v-model="userForm.name"
                    type="text"
                    required
                    class="w-full px-4 py-3 border-2 border-slate-200 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-purple-500 focus:border-purple-500 bg-white dark:bg-slate-900 text-slate-900 dark:text-white"
                  />
                </div>

                <div>
                  <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                    Role *
                  </label>
                  <select
                    v-model="userForm.role"
                    required
                    class="w-full px-4 py-3 border-2 border-slate-200 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-purple-500 bg-white dark:bg-slate-900 text-slate-900 dark:text-white"
                  >
                    <option value="owner">Owner</option>
                    <option value="cashier">Cashier</option>
                    <option value="driver">Driver</option>
                  </select>
                </div>

                <div>
                  <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                    Email
                  </label>
                  <input
                    v-model="userForm.email"
                    type="email"
                    class="w-full px-4 py-3 border-2 border-slate-200 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-purple-500 bg-white dark:bg-slate-900 text-slate-900 dark:text-white"
                  />
                </div>

                <div>
                  <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                    Phone Number *
                  </label>
                  <input
                    v-model="userForm.phone"
                    type="tel"
                    required
                    class="w-full px-4 py-3 border-2 border-slate-200 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-purple-500 bg-white dark:bg-slate-900 text-slate-900 dark:text-white"
                  />
                </div>
              </div>

              <div v-if="!editingUser">
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                  Temporary Password *
                </label>
                <input
                  v-model="userForm.password"
                  type="password"
                  :required="!editingUser"
                  placeholder="User will be prompted to change"
                  class="w-full px-4 py-3 border-2 border-slate-200 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-purple-500 bg-white dark:bg-slate-900 text-slate-900 dark:text-white"
                />
              </div>

              <!-- Permissions -->
              <div class="pt-4 border-t border-slate-200 dark:border-slate-700">
                <h4 class="font-semibold text-slate-900 dark:text-white mb-3">Permissions</h4>
                <div class="space-y-2">
                  <label class="flex items-center space-x-3 cursor-pointer">
                    <input
                      v-model="userForm.permissions.sales"
                      type="checkbox"
                      class="w-4 h-4 text-purple-600 border-slate-300 rounded focus:ring-purple-500"
                    />
                    <span class="text-sm text-slate-900 dark:text-white">Sales & POS</span>
                  </label>
                  <label class="flex items-center space-x-3 cursor-pointer">
                    <input
                      v-model="userForm.permissions.stock"
                      type="checkbox"
                      class="w-4 h-4 text-purple-600 border-slate-300 rounded focus:ring-purple-500"
                    />
                    <span class="text-sm text-slate-900 dark:text-white">Stock Management</span>
                  </label>
                  <label class="flex items-center space-x-3 cursor-pointer">
                    <input
                      v-model="userForm.permissions.buying"
                      type="checkbox"
                      class="w-4 h-4 text-purple-600 border-slate-300 rounded focus:ring-purple-500"
                    />
                    <span class="text-sm text-slate-900 dark:text-white">Buying & Orders</span>
                  </label>
                  <label class="flex items-center space-x-3 cursor-pointer">
                    <input
                      v-model="userForm.permissions.reports"
                      type="checkbox"
                      class="w-4 h-4 text-purple-600 border-slate-300 rounded focus:ring-purple-500"
                    />
                    <span class="text-sm text-slate-900 dark:text-white">View Reports</span>
                  </label>
                </div>
              </div>

              <!-- Actions -->
              <div class="flex items-center justify-end space-x-3 pt-6 border-t border-slate-200 dark:border-slate-700">
                <button
                  type="button"
                  @click="showAddModal = false"
                  class="px-6 py-3 border-2 border-slate-300 dark:border-slate-600 rounded-xl text-slate-700 dark:text-slate-300 font-semibold hover:bg-slate-50 dark:hover:bg-slate-700 transition-all"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  class="px-6 py-3 bg-gradient-to-r from-purple-600 to-pink-600 text-white rounded-xl font-semibold hover:from-purple-700 hover:to-pink-700 shadow-lg hover:shadow-xl transition-all"
                >
                  {{ editingUser ? 'Update User' : 'Add User' }}
                </button>
              </div>
            </form>
          </div>
        </div>
      </div>
    </transition>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import {
  PlusIcon,
  UsersIcon,
  CheckCircleIcon,
  KeyIcon,
  UserIcon,
  XMarkIcon
} from '@heroicons/vue/24/outline'

useHead({
  title: 'User Management - TOSS ERP',
  meta: [
    { name: 'description', content: 'Manage staff and access permissions' }
  ]
})

// State
const searchQuery = ref('')
const roleFilter = ref('')
const statusFilter = ref('')
const showAddModal = ref(false)
const editingUser = ref<any>(null)

const userForm = ref({
  name: '',
  email: '',
  phone: '',
  role: 'cashier',
  password: '',
  permissions: {
    sales: true,
    stock: false,
    buying: false,
    reports: false
  }
})

// Mock Data
const users = ref([
  {
    id: 'USR-001',
    name: 'Thabo Mthembu',
    email: 'thabo@thabos-spaza.co.za',
    phone: '+27 71 234 5678',
    role: 'owner',
    status: 'active',
    lastActive: new Date('2025-10-23')
  },
  {
    id: 'USR-002',
    name: 'Nomsa Dlamini',
    email: 'nomsa@example.com',
    phone: '+27 82 345 6789',
    role: 'cashier',
    status: 'active',
    lastActive: new Date('2025-10-23')
  },
  {
    id: 'USR-003',
    name: 'Sipho Ndlovu',
    email: 'sipho@example.com',
    phone: '+27 83 456 7890',
    role: 'cashier',
    status: 'active',
    lastActive: new Date('2025-10-22')
  },
  {
    id: 'USR-004',
    name: 'Lindiwe Khumalo',
    email: 'lindiwe@example.com',
    phone: '+27 84 567 8901',
    role: 'driver',
    status: 'inactive',
    lastActive: new Date('2025-10-15')
  }
])

// Computed
const activeUsers = computed(() => users.value.filter((u: any) => u.status === 'active').length)
const ownerCount = computed(() => users.value.filter((u: any) => u.role === 'owner').length)
const cashierCount = computed(() => users.value.filter((u: any) => u.role === 'cashier').length)

const filteredUsers = computed(() => {
  return users.value.filter((user: any) => {
    const matchesSearch = !searchQuery.value ||
      user.name.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      user.email.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      user.phone.includes(searchQuery.value)

    const matchesRole = !roleFilter.value || user.role === roleFilter.value
    const matchesStatus = !statusFilter.value || user.status === statusFilter.value

    return matchesSearch && matchesRole && matchesStatus
  })
})

// Methods
const getInitials = (name: string) => {
  return name.split(' ').map(n => n[0]).join('').toUpperCase()
}

const getRoleLabel = (role: string) => {
  const labels: Record<string, string> = {
    owner: 'Owner',
    cashier: 'Cashier',
    driver: 'Driver'
  }
  return labels[role] || role
}

const getRoleBadge = (role: string) => {
  const badges: Record<string, string> = {
    owner: 'bg-purple-100 text-purple-800 dark:bg-purple-900/30 dark:text-purple-400',
    cashier: 'bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400',
    driver: 'bg-orange-100 text-orange-800 dark:bg-orange-900/30 dark:text-orange-400'
  }
  return badges[role] || 'bg-slate-100 text-slate-800'
}

const getStatusBadge = (status: string) => {
  return status === 'active'
    ? 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400'
    : 'bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400'
}

const formatDate = (date: Date) => {
  return new Date(date).toLocaleDateString('en-ZA', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  })
}

const editUser = (user: any) => {
  editingUser.value = user
  userForm.value = {
    name: user.name,
    email: user.email,
    phone: user.phone,
    role: user.role,
    password: '',
    permissions: user.permissions || {
      sales: true,
      stock: false,
      buying: false,
      reports: false
    }
  }
  showAddModal.value = true
}

const saveUser = () => {
  if (editingUser.value) {
    // Update existing user
    const index = users.value.findIndex((u: any) => u.id === editingUser.value.id)
    if (index !== -1) {
      users.value[index] = {
        ...users.value[index],
        name: userForm.value.name,
        email: userForm.value.email,
        phone: userForm.value.phone,
        role: userForm.value.role
      }
    }
    alert('✓ User updated successfully!')
  } else {
    // Add new user
    const newUser = {
      id: `USR-${String(users.value.length + 1).padStart(3, '0')}`,
      name: userForm.value.name,
      email: userForm.value.email,
      phone: userForm.value.phone,
      role: userForm.value.role,
      status: 'active',
      lastActive: new Date()
    }
    users.value.unshift(newUser)
    alert(`✓ User added successfully! ID: ${newUser.id}`)
  }

  // Reset form
  showAddModal.value = false
  editingUser.value = null
  userForm.value = {
    name: '',
    email: '',
    phone: '',
    role: 'cashier',
    password: '',
    permissions: {
      sales: true,
      stock: false,
      buying: false,
      reports: false
    }
  }
}

const toggleUserStatus = (user: any) => {
  const newStatus = user.status === 'active' ? 'inactive' : 'active'
  const index = users.value.findIndex((u: any) => u.id === user.id)
  if (index !== -1) {
    users.value[index].status = newStatus
    alert(`✓ User ${newStatus === 'active' ? 'activated' : 'deactivated'}!`)
  }
}
</script>

<style scoped>
.modal-enter-active,
.modal-leave-active {
  transition: opacity 0.3s ease;
}

.modal-enter-from,
.modal-leave-to {
  opacity: 0;
}
</style>

