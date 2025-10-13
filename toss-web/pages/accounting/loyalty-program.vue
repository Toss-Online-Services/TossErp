<template>
  <div class="space-y-6">
    <PageHeader
      title="Loyalty Programs"
      description="Manage customer loyalty and rewards programs"
    />

    <div class="flex justify-end">
      <button class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700">
        Create Program
      </button>
    </div>

    <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
      <div v-for="program in loyaltyPrograms" :key="program.id" class="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
        <div class="flex justify-between items-start mb-4">
          <div>
            <h3 class="text-lg font-semibold text-gray-900 dark:text-white">{{ program.name }}</h3>
            <p class="text-sm text-gray-600 dark:text-gray-400">{{ program.description }}</p>
          </div>
          <span :class="program.isActive ? 'bg-green-100 text-green-800' : 'bg-gray-100 text-gray-800'" class="px-2 py-1 text-xs rounded-full">
            {{ program.isActive ? 'Active' : 'Inactive' }}
          </span>
        </div>

        <div class="grid grid-cols-3 gap-3 mb-4">
          <div class="text-center p-3 bg-blue-50 dark:bg-blue-900 rounded">
            <p class="text-sm text-gray-600 dark:text-gray-400">Members</p>
            <p class="text-xl font-bold text-blue-600 dark:text-blue-300">{{ program.members.toLocaleString() }}</p>
          </div>
          <div class="text-center p-3 bg-green-50 dark:bg-green-900 rounded">
            <p class="text-sm text-gray-600 dark:text-gray-400">Points Issued</p>
            <p class="text-xl font-bold text-green-600 dark:text-green-300">{{ program.pointsIssued.toLocaleString() }}</p>
          </div>
          <div class="text-center p-3 bg-purple-50 dark:bg-purple-900 rounded">
            <p class="text-sm text-gray-600 dark:text-gray-400">Redeemed</p>
            <p class="text-xl font-bold text-purple-600 dark:text-purple-300">{{ program.pointsRedeemed.toLocaleString() }}</p>
          </div>
        </div>

        <div class="space-y-2 mb-4">
          <div class="flex justify-between text-sm">
            <span class="text-gray-600 dark:text-gray-400">Point Value:</span>
            <span class="font-medium">R {{ program.pointValue }} per point</span>
          </div>
          <div class="flex justify-between text-sm">
            <span class="text-gray-600 dark:text-gray-400">Earning Rate:</span>
            <span class="font-medium">{{ program.earningRate }}% of purchase</span>
          </div>
          <div class="flex justify-between text-sm">
            <span class="text-gray-600 dark:text-gray-400">Expiry:</span>
            <span class="font-medium">{{ program.expiryMonths }} months</span>
          </div>
        </div>

        <div class="flex gap-2">
          <button class="flex-1 px-3 py-2 text-sm bg-blue-50 text-blue-600 rounded hover:bg-blue-100">
            View Members
          </button>
          <button class="flex-1 px-3 py-2 text-sm bg-gray-50 text-gray-600 rounded hover:bg-gray-100">
            Edit
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'

definePageMeta({
  layout: 'default'
})

useHead({
  title: 'Loyalty Programs - TOSS ERP',
})

const loyaltyPrograms = ref([
  {
    id: 1,
    name: 'Gold Rewards',
    description: 'Premium customer rewards program',
    isActive: true,
    members: 1250,
    pointsIssued: 45000,
    pointsRedeemed: 18500,
    pointValue: 0.10,
    earningRate: 5,
    expiryMonths: 12
  },
  {
    id: 2,
    name: 'Silver Plus',
    description: 'Standard loyalty program for regular customers',
    isActive: true,
    members: 3840,
    pointsIssued: 120000,
    pointsRedeemed: 67000,
    pointValue: 0.05,
    earningRate: 2,
    expiryMonths: 6
  }
])
</script>
