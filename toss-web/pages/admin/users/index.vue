<template>
  <div class="space-y-6">
    <!-- Page Header -->
    <div>
      <h1 class="text-2xl font-bold text-gray-900 dark:text-white">User Management</h1>
      <p class="text-sm text-gray-600 dark:text-gray-400 mt-1">Manage system users and their roles</p>
    </div>

    <!-- Filters -->
    <MaterialCard variant="elevated" class="p-4">
      <div class="flex flex-col sm:flex-row gap-4">
        <div class="flex-1">
          <input
            v-model="searchQuery"
            type="text"
            placeholder="Search users..."
            class="w-full px-4 py-2 bg-gray-50 dark:bg-gray-700 border border-gray-200 dark:border-gray-600 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-transparent"
          />
        </div>
        <select
          v-model="selectedRole"
          class="px-4 py-2 bg-gray-50 dark:bg-gray-700 border border-gray-200 dark:border-gray-600 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-transparent"
        >
          <option value="">All Roles</option>
          <option value="Administrator">Administrator</option>
          <option value="StoreOwner">Retailer</option>
          <option value="Vendor">Vendor</option>
          <option value="Supplier">Supplier</option>
          <option value="Driver">Driver</option>
        </select>
      </div>
    </MaterialCard>

    <!-- Users Table -->
    <MaterialCard variant="elevated" class="overflow-hidden">
      <div class="overflow-x-auto">
        <table class="w-full">
          <thead class="bg-gray-50 dark:bg-gray-700/50">
            <tr>
              <th class="text-left py-4 px-6 text-xs font-semibold text-gray-600 dark:text-gray-400 uppercase tracking-wider">Name</th>
              <th class="text-left py-4 px-6 text-xs font-semibold text-gray-600 dark:text-gray-400 uppercase tracking-wider">Email</th>
              <th class="text-left py-4 px-6 text-xs font-semibold text-gray-600 dark:text-gray-400 uppercase tracking-wider">Roles</th>
              <th class="text-left py-4 px-6 text-xs font-semibold text-gray-600 dark:text-gray-400 uppercase tracking-wider">Status</th>
              <th class="text-right py-4 px-6 text-xs font-semibold text-gray-600 dark:text-gray-400 uppercase tracking-wider">Actions</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-gray-200 dark:divide-gray-700">
            <tr v-if="isLoading">
              <td colspan="5" class="py-12 text-center">
                <div class="flex items-center justify-center">
                  <div class="w-8 h-8 border-4 border-indigo-500 border-t-transparent rounded-full animate-spin" />
                  <span class="ml-3 text-sm text-gray-600 dark:text-gray-400">Loading users...</span>
                </div>
              </td>
            </tr>
            <tr
              v-else-if="filteredUsers.length === 0"
              class="hover:bg-gray-50 dark:hover:bg-gray-700/50"
            >
              <td colspan="5" class="py-12 text-center text-gray-500 dark:text-gray-400">
                No users found
              </td>
            </tr>
            <tr
              v-for="user in filteredUsers"
              :key="user.id"
              class="hover:bg-gray-50 dark:hover:bg-gray-700/50 transition-colors"
            >
              <td class="py-4 px-6">
                <div class="flex items-center space-x-3">
                  <img
                    :src="user.avatar || `https://ui-avatars.com/api/?name=${encodeURIComponent(user.name)}&background=6366f1&color=fff`"
                    :alt="user.name"
                    class="w-10 h-10 rounded-full"
                  />
                  <span class="text-sm font-medium text-gray-900 dark:text-white">{{ user.name }}</span>
                </div>
              </td>
              <td class="py-4 px-6 text-sm text-gray-900 dark:text-white">{{ user.email }}</td>
              <td class="py-4 px-6">
                <div class="flex flex-wrap gap-1">
                  <span
                    v-for="role in user.roles"
                    :key="role"
                    class="px-2 py-1 text-xs font-semibold bg-indigo-100 dark:bg-indigo-900/30 text-indigo-800 dark:text-indigo-300 rounded-full"
                  >
                    {{ role }}
                  </span>
                </div>
              </td>
              <td class="py-4 px-6">
                <span
                  :class="[
                    'px-2 py-1 text-xs font-semibold rounded-full',
                    user.isActive
                      ? 'bg-green-100 dark:bg-green-900/30 text-green-800 dark:text-green-300'
                      : 'bg-red-100 dark:bg-red-900/30 text-red-800 dark:text-red-300'
                  ]"
                >
                  {{ user.isActive ? 'Active' : 'Inactive' }}
                </span>
              </td>
              <td class="py-4 px-6 text-right">
                <div class="flex items-center justify-end space-x-2">
                  <button
                    @click="user.isActive ? deactivateUser(user) : activateUser(user)"
                    :class="[
                      'px-3 py-1.5 text-xs font-medium rounded-lg transition-colors',
                      user.isActive
                        ? 'bg-red-100 dark:bg-red-900/30 text-red-700 dark:text-red-300 hover:bg-red-200 dark:hover:bg-red-900/50'
                        : 'bg-green-100 dark:bg-green-900/30 text-green-700 dark:text-green-300 hover:bg-green-200 dark:hover:bg-green-900/50'
                    ]"
                  >
                    {{ user.isActive ? 'Deactivate' : 'Activate' }}
                  </button>
                  <button
                    @click="showRoleModal(user)"
                    class="px-3 py-1.5 text-xs font-medium bg-indigo-100 dark:bg-indigo-900/30 text-indigo-700 dark:text-indigo-300 rounded-lg hover:bg-indigo-200 dark:hover:bg-indigo-900/50 transition-colors"
                  >
                    Edit Roles
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </MaterialCard>

    <!-- Role Edit Modal -->
    <MaterialModal v-model="showRoleEditModal" title="Edit Roles">
      <template #body>
        <div v-if="userToEdit" class="space-y-4">
          <p class="text-sm text-gray-600 dark:text-gray-400">
            Edit roles for <span class="font-semibold text-gray-900 dark:text-white">{{ userToEdit.name }}</span>
          </p>
          <div class="space-y-2">
            <label
              v-for="role in availableRoles"
              :key="role"
              class="flex items-center space-x-3 p-3 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700/50 cursor-pointer"
            >
              <input
                v-model="selectedRoles"
                type="checkbox"
                :value="role"
                class="w-4 h-4 text-indigo-600 border-gray-300 rounded focus:ring-indigo-500"
              />
              <span class="text-sm text-gray-900 dark:text-white">{{ role }}</span>
            </label>
          </div>
        </div>
      </template>
      <template #footer>
        <div class="flex justify-end space-x-3">
          <button
            @click="showRoleEditModal = false"
            class="px-4 py-2 text-sm font-medium text-gray-700 dark:text-gray-300 bg-gray-100 dark:bg-gray-700 rounded-lg hover:bg-gray-200 dark:hover:bg-gray-600 transition-colors"
          >
            Cancel
          </button>
          <button
            @click="updateUserRoles"
            class="px-4 py-2 text-sm font-medium text-white bg-indigo-600 rounded-lg hover:bg-indigo-700 transition-colors"
          >
            Update Roles
          </button>
        </div>
      </template>
    </MaterialModal>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'default',
  middleware: 'auth',
  meta: {
    roles: ['Administrator'],
    role: 'admin',
    title: 'Users',
    subtitle: 'Manage platform access and roles'
  }
})

const { get, post } = useApi()
const users = ref<any[]>([])
const isLoading = ref(true)
const searchQuery = ref('')
const selectedRole = ref('')
const showRoleEditModal = ref(false)
const userToEdit = ref<any>(null)
const selectedRoles = ref<string[]>([])

const availableRoles = ['Administrator', 'StoreOwner', 'Vendor', 'Supplier', 'Driver']

const loadUsers = async () => {
  isLoading.value = true
  try {
    const response = await get('/api/users/list', {
      take: 1000,
      searchTerm: searchQuery.value || undefined,
      role: selectedRole.value || undefined
    })
    users.value = response || []
  } catch (error) {
    console.error('Failed to load users:', error)
  } finally {
    isLoading.value = false
  }
}

const filteredUsers = computed(() => {
  return users.value
})

const activateUser = async (user: any) => {
  if (!confirm(`Activate user ${user.name}?`)) return

  try {
    await post(`/api/users/${user.id}/activate`)
    await loadUsers()
  } catch (error) {
    console.error('Failed to activate user:', error)
    alert('Failed to activate user. Please try again.')
  }
}

const deactivateUser = async (user: any) => {
  if (!confirm(`Deactivate user ${user.name}?`)) return

  try {
    await post(`/api/users/${user.id}/deactivate`)
    await loadUsers()
  } catch (error) {
    console.error('Failed to deactivate user:', error)
    alert('Failed to deactivate user. Please try again.')
  }
}

const showRoleModal = (user: any) => {
  userToEdit.value = user
  selectedRoles.value = [...(user.roles || [])]
  showRoleEditModal.value = true
}

const updateUserRoles = async () => {
  if (!userToEdit.value) return

  try {
    await post(`/api/users/${userToEdit.value.id}/roles`, {
      roles: selectedRoles.value
    })
    await loadUsers()
    showRoleEditModal.value = false
    userToEdit.value = null
  } catch (error) {
    console.error('Failed to update user roles:', error)
    alert('Failed to update user roles. Please try again.')
  }
}

watch([searchQuery, selectedRole], () => {
  loadUsers()
})

onMounted(() => {
  loadUsers()
})
</script>
