<template>
  <div class="space-y-6">
    <PageHeader
      title="Country Management"
      description="Manage countries and regional settings"
    />

    <div class="flex justify-between items-center">
      <div>
        <input
          v-model="searchQuery"
          type="text"
          placeholder="Search countries..."
          class="px-4 py-2 border rounded-lg"
        />
      </div>
      <button
        @click="showModal = true"
        class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700"
      >
        Add Country
      </button>
    </div>

    <div class="bg-white rounded-lg shadow">
      <table class="w-full">
        <thead class="bg-gray-50">
          <tr>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Code</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Country Name</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Currency</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Tax Rate</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Status</th>
            <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">Actions</th>
          </tr>
        </thead>
        <tbody class="divide-y">
          <tr v-for="country in filteredCountries" :key="country.id">
            <td class="px-6 py-4">{{ country.code }}</td>
            <td class="px-6 py-4">{{ country.name }}</td>
            <td class="px-6 py-4">{{ country.currency }}</td>
            <td class="px-6 py-4">{{ country.taxRate }}%</td>
            <td class="px-6 py-4">
              <span :class="country.isActive ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'" class="px-2 py-1 text-xs rounded-full">
                {{ country.isActive ? 'Active' : 'Inactive' }}
              </span>
            </td>
            <td class="px-6 py-4 text-right">
              <button class="text-blue-600 hover:text-blue-800 mr-3">Edit</button>
              <button class="text-red-600 hover:text-red-800">Delete</button>
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
  title: 'Country Management - TOSS ERP',
})

const searchQuery = ref('')
const showModal = ref(false)

const countries = ref([
  { id: 1, code: 'ZA', name: 'South Africa', currency: 'ZAR', taxRate: 15, isActive: true },
  { id: 2, code: 'US', name: 'United States', currency: 'USD', taxRate: 0, isActive: true },
  { id: 3, code: 'GB', name: 'United Kingdom', currency: 'GBP', taxRate: 20, isActive: true },
  { id: 4, code: 'DE', name: 'Germany', currency: 'EUR', taxRate: 19, isActive: true },
])

const filteredCountries = computed(() => {
  if (!searchQuery.value) return countries.value
  return countries.value.filter(c =>
    c.name.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
    c.code.toLowerCase().includes(searchQuery.value.toLowerCase())
  )
})
</script>
