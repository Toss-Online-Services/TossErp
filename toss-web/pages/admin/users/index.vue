<template>
  <div class="p-6">
    <h1 class="text-2xl font-bold text-gray-900 dark:text-gray-100 mb-6">User Management</h1>

    <!-- Filters -->
    <div class="mb-6 space-y-4 sm:flex sm:space-y-0 sm:space-x-4">
      <div class="flex-1">
        <MaterialInput
          v-model="searchQuery"
          type="text"
          label="Search users"
          placeholder="Search users..."
          variant="outlined"
          class="w-full"
        />
      </div>
      <MaterialSelect
        v-model="selectedRole"
        label="Role"
        :options="[
          { value: '', label: 'All Roles' },
          { value: 'Administrator', label: 'Administrator' },
          { value: 'StoreOwner', label: 'Retailer' },
          { value: 'Vendor', label: 'Vendor' },
          { value: 'Supplier', label: 'Supplier' },
          { value: 'Driver', label: 'Driver' }
        ]"
        class="w-48"
      />
    </div>

    <!-- Users Table -->
    <MaterialDataTable
      :loading="isLoading"
      :rows="filteredUsers"
      :columns="[
        { key: 'name', label: 'Name', render: (row) => row.name },
        { key: 'email', label: 'Email', render: (row) => row.email },
        { key: 'roles', label: 'Roles', render: (row) => h('div', { class: 'flex flex-wrap gap-1' }, row.roles.map(role => h(UiBadge, { color: 'primary', class: 'text-xs font-semibold' }, () => role))) },
        { key: 'isActive', label: 'Status', render: (row) => h(UiBadge, { color: row.isActive ? 'success' : 'danger', class: 'text-xs font-semibold' }, () => row.isActive ? 'Active' : 'Inactive') },
        { key: 'actions', label: 'Actions', align: 'right', render: (row) => h('div', { class: 'space-x-2' }, [
          row.isActive
            ? h(MaterialButton, { color: 'danger', size: 'sm', onClick: () => deactivateUser(row) }, () => 'Deactivate')
            : h(MaterialButton, { color: 'success', size: 'sm', onClick: () => activateUser(row) }, () => 'Activate'),
          h(MaterialButton, { color: 'primary', size: 'sm', onClick: () => showRoleModal(row) }, () => 'Edit Roles')
        ]) }
      ]"
      class="bg-white dark:bg-gray-800 rounded-lg shadow"
    />

    <MaterialModal v-model="showRoleEditModal" :title="`Edit Roles for ${userToEdit?.name}`">
      <template #body>
        <div class="space-y-2 mb-6">
          <label v-for="role in availableRoles" :key="role" class="flex items-center">
            <UiSwitch v-model="selectedRoles" :value="role" />
            <span class="ml-2 text-sm text-gray-700 dark:text-gray-300">{{ role }}</span>
          </label>
        </div>
      </template>
      <template #footer>
        <div class="flex justify-end space-x-2">
          <MaterialButton variant="text" @click="showRoleEditModal = false">Cancel</MaterialButton>
          <MaterialButton color="primary" @click="updateUserRoles">Update Roles</MaterialButton>
        </div>
      </template>
    </MaterialModal>
  </div>
</template>

<script setup lang="ts">
// eslint-disable-next-line @typescript-eslint/no-undef, no-undef
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

