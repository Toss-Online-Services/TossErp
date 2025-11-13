<template>
  <div class="p-6">
    <div class="flex items-center justify-between mb-6">
      <div>
        <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
          Sales Partners
        </h1>
        <p class="text-gray-600 dark:text-gray-400">
          Manage distribution partners, agents, and channel relationships
        </p>
      </div>
      <button class="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg">
        <Icon name="heroicons:plus" class="w-4 h-4 mr-2" />
        Add Partner
      </button>
    </div>

    <!-- Partner Stats -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm">
        <div class="flex items-center">
          <div class="p-2 bg-blue-100 rounded-full">
            <Icon name="heroicons:user-group" class="w-5 h-5 text-blue-600" />
          </div>
          <div class="ml-3">
            <p class="text-sm text-gray-600 dark:text-gray-400">Active Partners</p>
            <p class="text-lg font-semibold text-gray-900 dark:text-white">24</p>
          </div>
        </div>
      </div>
      
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm">
        <div class="flex items-center">
          <div class="p-2 bg-green-100 rounded-full">
            <Icon name="heroicons:banknotes" class="w-5 h-5 text-green-600" />
          </div>
          <div class="ml-3">
            <p class="text-sm text-gray-600 dark:text-gray-400">Total Revenue</p>
            <p class="text-lg font-semibold text-gray-900 dark:text-white">R 567K</p>
          </div>
        </div>
      </div>
      
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm">
        <div class="flex items-center">
          <div class="p-2 bg-purple-100 rounded-full">
            <Icon name="heroicons:chart-bar" class="w-5 h-5 text-purple-600" />
          </div>
          <div class="ml-3">
            <p class="text-sm text-gray-600 dark:text-gray-400">Avg Commission</p>
            <p class="text-lg font-semibold text-gray-900 dark:text-white">8.5%</p>
          </div>
        </div>
      </div>
      
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm">
        <div class="flex items-center">
          <div class="p-2 bg-orange-100 rounded-full">
            <Icon name="heroicons:map" class="w-5 h-5 text-orange-600" />
          </div>
          <div class="ml-3">
            <p class="text-sm text-gray-600 dark:text-gray-400">Coverage Areas</p>
            <p class="text-lg font-semibold text-gray-900 dark:text-white">12</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Partner List -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm">
      <div class="p-6">
        <h2 class="text-lg font-medium text-gray-900 dark:text-white mb-4">
          Partner Directory
        </h2>
        
        <div class="space-y-4">
          <div v-for="partner in mockPartners" :key="partner.id" 
               class="border border-gray-200 dark:border-gray-700 rounded-lg p-4">
            <div class="flex items-start justify-between">
              <div class="flex-1">
                <div class="flex items-center mb-3">
                  <div class="w-12 h-12 bg-gradient-to-br from-blue-500 to-purple-600 rounded-lg flex items-center justify-center mr-4">
                    <span class="text-white font-bold text-lg">
                      {{ partner.name.charAt(0) }}
                    </span>
                  </div>
                  <div>
                    <h3 class="font-medium text-gray-900 dark:text-white text-lg">
                      {{ partner.name }}
                    </h3>
                    <div class="flex items-center space-x-3">
                      <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                            :class="getPartnerTypeClass(partner.type)">
                        {{ formatPartnerType(partner.type) }}
                      </span>
                      <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                            :class="partner.isActive ? 
                              'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200' : 
                              'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300'">
                        {{ partner.isActive ? 'Active' : 'Inactive' }}
                      </span>
                    </div>
                  </div>
                </div>
                
                <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
                  <div>
                    <span class="text-sm font-medium text-gray-700 dark:text-gray-300">Contact:</span>
                    <div class="text-sm text-gray-600 dark:text-gray-400">
                      {{ partner.contact.name }}<br>
                      {{ partner.contact.phone }}
                    </div>
                  </div>
                  
                  <div>
                    <span class="text-sm font-medium text-gray-700 dark:text-gray-300">Territory:</span>
                    <div class="text-sm text-gray-600 dark:text-gray-400">
                      {{ partner.territory.join(', ') }}
                    </div>
                  </div>
                  
                  <div>
                    <span class="text-sm font-medium text-gray-700 dark:text-gray-300">Commission:</span>
                    <div class="text-sm text-gray-600 dark:text-gray-400">
                      {{ partner.commission.rate }}% ({{ partner.commission.structure }})
                    </div>
                  </div>
                  
                  <div>
                    <span class="text-sm font-medium text-gray-700 dark:text-gray-300">Performance:</span>
                    <div class="text-sm text-gray-600 dark:text-gray-400">
                      R {{ partner.performance.monthlyRevenue.toLocaleString() }}/month<br>
                      {{ partner.performance.customersAcquired }} customers
                    </div>
                  </div>
                </div>
                
                <div v-if="partner.specialties.length" class="mt-3">
                  <span class="text-sm font-medium text-gray-700 dark:text-gray-300">Specialties:</span>
                  <div class="flex flex-wrap gap-2 mt-1">
                    <span v-for="specialty in partner.specialties" :key="specialty" 
                          class="inline-flex items-center px-2 py-1 text-xs bg-blue-50 text-blue-700 rounded dark:bg-blue-900 dark:text-blue-300">
                      {{ specialty }}
                    </span>
                  </div>
                </div>
                
                <div class="mt-3 flex items-center space-x-6">
                  <div class="flex items-center">
                    <Icon name="heroicons:star" class="w-4 h-4 text-yellow-500 mr-1" />
                    <span class="text-sm text-gray-600 dark:text-gray-400">
                      {{ partner.rating }}/5 rating
                    </span>
                  </div>
                  
                  <div class="flex items-center">
                    <Icon name="heroicons:clock" class="w-4 h-4 text-gray-500 mr-1" />
                    <span class="text-sm text-gray-600 dark:text-gray-400">
                      Partner since {{ formatDate(partner.partnershipDate) }}
                    </span>
                  </div>
                </div>
              </div>
              
              <div class="flex items-center space-x-2 ml-4">
                <button class="text-blue-600 hover:text-blue-800 text-sm px-3 py-1 border border-blue-200 rounded">
                  View Details
                </button>
                <button class="text-green-600 hover:text-green-800 text-sm px-3 py-1 border border-green-200 rounded">
                  Performance
                </button>
                <button class="text-purple-600 hover:text-purple-800 text-sm px-3 py-1 border border-purple-200 rounded">
                  Commission
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'

useHead({
  title: 'Sales Partners - TOSS ERP'
})

// Mock data for demonstration
const mockPartners = ref([
  {
    id: 'partner-001',
    name: 'Township Distribution Co.',
    type: 'distributor',
    isActive: true,
    contact: {
      name: 'Themba Mthembu',
      phone: '+27 82 345 6789',
      email: 'themba@township-dist.co.za'
    },
    territory: ['Soweto', 'Diepkloof', 'Orlando'],
    commission: {
      rate: 8.5,
      structure: 'graduated'
    },
    specialties: ['FMCG', 'Beverages', 'Household Items'],
    performance: {
      monthlyRevenue: 125000,
      customersAcquired: 45,
      conversionRate: 62.3
    },
    rating: 4.7,
    partnershipDate: new Date('2023-03-15'),
    agreements: {
      exclusiveTerritory: true,
      minimumVolume: 50000
    }
  },
  {
    id: 'partner-002',
    name: 'Spaza Connect Network',
    type: 'agent',
    isActive: true,
    contact: {
      name: 'Nomsa Dlamini',
      phone: '+27 76 789 0123',
      email: 'nomsa@spazaconnect.com'
    },
    territory: ['Alexandra', 'Tembisa'],
    commission: {
      rate: 6.0,
      structure: 'flat'
    },
    specialties: ['Spaza Shop Onboarding', 'Digital Payments', 'Training'],
    performance: {
      monthlyRevenue: 78000,
      customersAcquired: 32,
      conversionRate: 58.7
    },
    rating: 4.9,
    partnershipDate: new Date('2023-08-20'),
    agreements: {
      exclusiveTerritory: false,
      minimumVolume: 25000
    }
  },
  {
    id: 'partner-003',
    name: 'Rural Logistics Solutions',
    type: 'logistics',
    isActive: true,
    contact: {
      name: 'Pieter Van Der Merwe',
      phone: '+27 83 456 7890',
      email: 'pieter@rurallogistics.co.za'
    },
    territory: ['Orange Farm', 'Evaton', 'Sebokeng'],
    commission: {
      rate: 12.0,
      structure: 'performance-based'
    },
    specialties: ['Last-Mile Delivery', 'Rural Areas', 'Cold Chain'],
    performance: {
      monthlyRevenue: 156000,
      customersAcquired: 28,
      conversionRate: 71.4
    },
    rating: 4.5,
    partnershipDate: new Date('2022-11-10'),
    agreements: {
      exclusiveTerritory: true,
      minimumVolume: 75000
    }
  },
  {
    id: 'partner-004',
    name: 'Stokvel Solutions Hub',
    type: 'specialist',
    isActive: true,
    contact: {
      name: 'Mpho Molefe',
      phone: '+27 72 123 4567',
      email: 'mpho@stokvelsolutions.co.za'
    },
    territory: ['All Areas'],
    commission: {
      rate: 10.5,
      structure: 'tiered'
    },
    specialties: ['Stokvel Management', 'Group Buying', 'Financial Services'],
    performance: {
      monthlyRevenue: 89000,
      customersAcquired: 15,
      conversionRate: 85.2
    },
    rating: 4.8,
    partnershipDate: new Date('2023-05-03'),
    agreements: {
      exclusiveTerritory: false,
      minimumVolume: 40000
    }
  }
])

const formatPartnerType = (type: string) => {
  const typeMap: Record<string, string> = {
    distributor: 'Distributor',
    agent: 'Sales Agent',
    logistics: 'Logistics Partner',
    specialist: 'Specialist Partner',
    reseller: 'Reseller'
  }
  return typeMap[type] || type
}

const getPartnerTypeClass = (type: string) => {
  const classMap: Record<string, string> = {
    distributor: 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200',
    agent: 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200',
    logistics: 'bg-orange-100 text-orange-800 dark:bg-orange-900 dark:text-orange-200',
    specialist: 'bg-purple-100 text-purple-800 dark:bg-purple-900 dark:text-purple-200',
    reseller: 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-200'
  }
  return classMap[type] || 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300'
}

const formatDate = (date: Date) => {
  return new Date(date).toLocaleDateString('en-ZA', { year: 'numeric', month: 'long' })
}
</script>