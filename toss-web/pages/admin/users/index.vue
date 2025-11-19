<template>
  <div class="p-6">
    <h1 class="text-2xl font-bold text-gray-900 dark:text-gray-100 mb-6">User Management</h1>

    <!-- Filters -->
    <div class="mb-6 space-y-4 sm:flex sm:space-y-0 sm:space-x-4">
      <div class="flex-1">
        <input
          v-model="searchQuery"
          type="text"
          placeholder="Search users..."
          class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-red-500"
        />
      </div>
      <select
        v-model="selectedRole"
        class="px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-red-500"
      >
        <option value="">All Roles</option>
        <option value="Administrator">Administrator</option>
        <option value="StoreOwner">Retailer</option>
        <option value="Vendor">Vendor</option>
        <option value="Supplier">Supplier</option>
        <option value="Driver">Driver</option>
      </select>
    </div>

    <!-- Users Table -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow overflow-hidden">
      <div v-if="isLoading" class="p-8 text-center">
        <div class="inline-block w-8 h-8 border-4 border-red-600 border-t-transparent rounded-full animate-spin"></div>
        <p class="mt-2 text-gray-600">Loading users...</p>
      </div>

      <table v-else class="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
        <thead class="bg-gray-50 dark:bg-gray-700">
          <tr>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Name</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Email</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Roles</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Status</th>
            <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Actions</th>
          </tr>
        </thead>
        <tbody class="bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700">
          <tr v-for="user in filteredUsers" :key="user.id" class="hover:bg-gray-50 dark:hover:bg-gray-700">
            <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900 dark:text-gray-100">
              {{ user.name }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400">
              {{ user.email }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <div class="flex flex-wrap gap-1">
                <span
                  v-for="role in user.roles"
                  :key="role"
                  class="px-2 py-1 text-xs font-semibold rounded-full bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200"
                >
                  {{ role }}
                </span>
              </div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <span
                :class="[
                  'px-2 py-1 text-xs font-semibold rounded-full',
                  user.isActive
                    ? 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200'
                    : 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200'
                ]"
              >
                {{ user.isActive ? 'Active' : 'Inactive' }}
              </span>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium space-x-2">
              <button
                v-if="user.isActive"
                @click="deactivateUser(user)"
                class="text-red-600 hover:text-red-900 dark:text-red-400"
              >
                Deactivate
              </button>
              <button
                v-else
                @click="activateUser(user)"
                class="text-green-600 hover:text-green-900 dark:text-green-400"
              >
                Activate
              </button>
              <button
                @click="showRoleModal(user)"
                class="text-blue-600 hover:text-blue-900 dark:text-blue-400"
              >
                Edit Roles
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Role Edit Modal -->
    <div v-if="showRoleEditModal" class="fixed inset-0 z-50 flex items-center justify-center bg-black bg-opacity-50">
      <div class="bg-white dark:bg-gray-800 rounded-lg p-6 max-w-md w-full">
        <h3 class="text-lg font-semibold mb-4">Edit Roles for {{ userToEdit?.name }}</h3>
        <div class="space-y-2 mb-6">
          <label
            v-for="role in availableRoles"
            :key="role"
            class="flex items-center"
          >
            <input
              v-model="selectedRoles"
              type="checkbox"
              :value="role"
              class="mr-2"
            />
            <span class="text-sm text-gray-700 dark:text-gray-300">{{ role }}</span>
          </label>
        </div>
        <div class="flex justify-end space-x-4">
          <button
            @click="showRoleEditModal = false"
            class="px-4 py-2 border border-gray-300 rounded-lg hover:bg-gray-50"
          >
            Cancel
          </button>
          <button
            @click="updateUserRoles"
            class="px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700"
          >
            Update Roles
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'admin',
  middleware: 'auth',
  meta: {
    roles: ['Administrator']
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
      params: {
        take: 1000,
        searchTerm: searchQuery.value || undefined,
        role: selectedRole.value || undefined
      }
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

