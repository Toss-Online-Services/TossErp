<template>
  <TransitionRoot appear :show="isOpen" as="template">
    <Dialog as="div" @close="closeModal" class="relative z-50">
      <TransitionChild
        as="template"
        enter="duration-300 ease-out"
        enter-from="opacity-0"
        enter-to="opacity-100"
        leave="duration-200 ease-in"
        leave-from="opacity-100"
        leave-to="opacity-0"
      >
        <div class="fixed inset-0 bg-black bg-opacity-50" />
      </TransitionChild>

      <div class="fixed inset-0 overflow-y-auto">
        <div class="flex min-h-full items-center justify-center p-4 text-center">
          <TransitionChild
            as="template"
            enter="duration-300 ease-out"
            enter-from="opacity-0 scale-95"
            enter-to="opacity-100 scale-100"
            leave="duration-200 ease-in"
            leave-from="opacity-100 scale-100"
            leave-to="opacity-0 scale-95"
          >
            <DialogPanel class="w-full max-w-4xl transform overflow-hidden rounded-lg bg-white dark:bg-slate-800 text-left align-middle shadow-xl transition-all">
              <div class="flex items-center justify-between p-6 border-b border-slate-200 dark:border-slate-600">
                <DialogTitle as="h3" class="text-lg font-medium leading-6 text-slate-900 dark:text-white">
                  {{ isEditMode ? 'Edit Contact' : 'Create New Contact' }}
                </DialogTitle>
                <button
                  @click="closeModal"
                  class="text-slate-400 hover:text-slate-500 dark:text-slate-500 dark:hover:text-slate-400 transition-colors"
                >
                  <XMarkIcon class="h-6 w-6" />
                </button>
              </div>

              <form @submit.prevent="submitForm" class="p-6 space-y-6 max-h-[70vh] overflow-y-auto">
                <!-- Personal Information -->
                <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
                  <div>
                    <label for="firstName" class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
                      First Name *
                    </label>
                    <input
                      id="firstName"
                      v-model="form.firstName"
                      type="text"
                      required
                      class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg bg-white dark:bg-slate-700 text-slate-900 dark:text-white focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                      placeholder="Enter first name"
                    />
                  </div>
                  
                  <div>
                    <label for="lastName" class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
                      Last Name *
                    </label>
                    <input
                      id="lastName"
                      v-model="form.lastName"
                      type="text"
                      required
                      class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg bg-white dark:bg-slate-700 text-slate-900 dark:text-white focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                      placeholder="Enter last name"
                    />
                  </div>
                </div>

                <!-- Contact Information -->
                <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
                  <div>
                    <label for="email" class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
                      Email *
                    </label>
                    <input
                      id="email"
                      v-model="form.email"
                      type="email"
                      required
                      class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg bg-white dark:bg-slate-700 text-slate-900 dark:text-white focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                      placeholder="Enter email address"
                    />
                  </div>
                  
                  <div>
                    <label for="phone" class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
                      Phone
                    </label>
                    <input
                      id="phone"
                      v-model="form.phone"
                      type="tel"
                      class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg bg-white dark:bg-slate-700 text-slate-900 dark:text-white focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                      placeholder="Enter phone number"
                    />
                  </div>
                </div>

                <!-- Business Information Section -->
                <div class="border-t border-slate-200 dark:border-slate-600 pt-6">
                  <h4 class="text-md font-medium text-slate-900 dark:text-white mb-4">Business Information</h4>
                  
                  <!-- Company and Job Title -->
                  <div class="grid grid-cols-1 md:grid-cols-2 gap-6 mb-6">
                    <div>
                      <label for="company" class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
                        Company/Business Name
                      </label>
                      <input
                        id="company"
                        v-model="form.company"
                        type="text"
                        class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg bg-white dark:bg-slate-700 text-slate-900 dark:text-white focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                        placeholder="Enter business name"
                      />
                    </div>
                    
                    <div>
                      <label for="jobTitle" class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
                        Job Title/Role
                      </label>
                      <input
                        id="jobTitle"
                        v-model="form.jobTitle"
                        type="text"
                        class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg bg-white dark:bg-slate-700 text-slate-900 dark:text-white focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                        placeholder="Enter role or position"
                      />
                    </div>
                  </div>

                  <!-- Enterprise Type Selection -->
                  <div class="space-y-4 mb-6">
                    <div>
                      <label for="enterpriseSector" class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
                        Business Sector
                      </label>
                      <select
                        id="enterpriseSector"
                        v-model="selectedSector"
                        @change="onSectorChange"
                        class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg bg-white dark:bg-slate-700 text-slate-900 dark:text-white focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                      >
                        <option value="">Select business sector</option>
                        <option v-for="sector in sectors" :key="sector" :value="sector">
                          {{ sector }}
                        </option>
                      </select>
                    </div>

                    <div v-if="selectedSector">
                      <label for="enterpriseType" class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
                        Enterprise Type
                      </label>
                      <select
                        id="enterpriseType"
                        v-model="form.enterpriseType"
                        @change="onEnterpriseTypeChange"
                        class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg bg-white dark:bg-slate-700 text-slate-900 dark:text-white focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                      >
                        <option value="">Select enterprise type</option>
                        <option 
                          v-for="enterprise in filteredEnterprises" 
                          :key="enterprise.enterprise" 
                          :value="enterprise.enterprise"
                        >
                          {{ enterprise.enterprise }}
                        </option>
                      </select>
                      
                      <!-- Enterprise Description -->
                      <div v-if="selectedEnterpriseDetails" class="mt-2 p-3 bg-blue-50 dark:bg-blue-900/20 rounded-lg">
                        <p class="text-sm text-blue-800 dark:text-blue-200">
                          <strong>{{ selectedEnterpriseDetails.typicalScale }}</strong> - {{ selectedEnterpriseDetails.description }}
                        </p>
                      </div>
                    </div>

                    <!-- Quick Select Popular Enterprises -->
                    <div>
                      <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                        Or select from common township businesses:
                      </label>
                      <div class="flex flex-wrap gap-2">
                        <button
                          v-for="popular in popularEnterprises"
                          :key="popular"
                          type="button"
                          @click="selectPopularEnterprise(popular)"
                          class="px-3 py-1 text-xs bg-slate-100 dark:bg-slate-700 text-slate-700 dark:text-slate-300 rounded-full hover:bg-slate-200 dark:hover:bg-slate-600 transition-colors"
                        >
                          {{ popular }}
                        </button>
                      </div>
                    </div>
                  </div>
                </div>

                <!-- Classification Section -->
                <div class="border-t border-slate-200 dark:border-slate-600 pt-6">
                  <h4 class="text-md font-medium text-slate-900 dark:text-white mb-4">Contact Classification</h4>
                  
                  <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
                    <div>
                      <label for="type" class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
                        Type *
                      </label>
                      <select
                        id="type"
                        v-model="form.type"
                        required
                        class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg bg-white dark:bg-slate-700 text-slate-900 dark:text-white focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                      >
                        <option value="">Select type</option>
                        <option value="Lead">Lead</option>
                        <option value="Prospect">Prospect</option>
                        <option value="Customer">Customer</option>
                      </select>
                    </div>
                    
                    <div>
                      <label for="status" class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
                        Status *
                      </label>
                      <select
                        id="status"
                        v-model="form.status"
                        required
                        class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg bg-white dark:bg-slate-700 text-slate-900 dark:text-white focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                      >
                        <option value="">Select status</option>
                        <option value="Active">Active</option>
                        <option value="Inactive">Inactive</option>
                        <option value="Qualified">Qualified</option>
                        <option value="Unqualified">Unqualified</option>
                      </select>
                    </div>
                    
                    <div>
                      <label for="source" class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
                        Source
                      </label>
                      <select
                        id="source"
                        v-model="form.source"
                        class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg bg-white dark:bg-slate-700 text-slate-900 dark:text-white focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                      >
                        <option value="">Select source</option>
                        <option value="Website">Website</option>
                        <option value="Referral">Referral</option>
                        <option value="LinkedIn">LinkedIn</option>
                        <option value="Trade Show">Trade Show</option>
                        <option value="Cold Call">Cold Call</option>
                        <option value="Email Campaign">Email Campaign</option>
                        <option value="Community Network">Community Network</option>
                        <option value="Local Market">Local Market</option>
                        <option value="Word of Mouth">Word of Mouth</option>
                      </select>
                    </div>
                  </div>
                </div>

                <!-- Form Actions -->
                <div class="flex justify-end space-x-3 pt-6 border-t border-slate-200 dark:border-slate-600">
                  <button
                    type="button"
                    @click="closeModal"
                    class="px-4 py-2 text-sm font-medium text-slate-700 dark:text-slate-300 bg-white dark:bg-slate-800 border border-slate-300 dark:border-slate-600 rounded-lg hover:bg-slate-50 dark:hover:bg-slate-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 transition-colors"
                  >
                    Cancel
                  </button>
                  <button
                    type="submit"
                    :disabled="loading"
                    class="px-4 py-2 text-sm font-medium text-white bg-blue-600 border border-transparent rounded-lg hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
                  >
                    <span v-if="loading">{{ isEditMode ? 'Updating...' : 'Creating...' }}</span>
                    <span v-else>{{ isEditMode ? 'Update Contact' : 'Create Contact' }}</span>
                  </button>
                </div>
              </form>
            </DialogPanel>
          </TransitionChild>
        </div>
      </div>
    </Dialog>
  </TransitionRoot>
</template>

<script setup lang="ts">
import { ref, watch, computed } from 'vue'
import {
  Dialog,
  DialogPanel,
  DialogTitle,
  TransitionChild,
  TransitionRoot,
} from '@headlessui/vue'
import { XMarkIcon } from '@heroicons/vue/24/outline'

// Props
interface Props {
  isOpen: boolean
  contact?: any
  isEditMode?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  isEditMode: false
})

// Emits
const emit = defineEmits<{
  close: []
  save: [contact: any]
}>()

// Enterprise data composable (auto-imported in Nuxt 3)
// Temporarily embedded for immediate functionality
const useEnterpriseData = () => {
  const enterpriseData = [
    // Retail & Trade
    { sector: "Retail & Trade", enterprise: "Spaza / Convenience shop", type: "P/H", typicalScale: "Micro/Informal", description: "Small local shop selling groceries, airtime, household goods." },
    { sector: "Retail & Trade", enterprise: "Hawkers / Street traders", type: "P/H", typicalScale: "Micro/Informal", description: "Mobile sellers of food, fruit, small goods." },
    { sector: "Retail & Trade", enterprise: "Pavement / Market stall sellers", type: "P/H", typicalScale: "Micro/Informal", description: "Fixed-day stalls selling clothing, utensils, produce." },
    { sector: "Retail & Trade", enterprise: "Second-hand clothing (thrift) stalls", type: "P", typicalScale: "Micro", description: "Buy/resell used clothes." },
    { sector: "Retail & Trade", enterprise: "Small kiosks (tuck-shops)", type: "P", typicalScale: "Micro", description: "Single-item focused retail (snacks, cold drinks)." },
    
    // Food & Hospitality
    { sector: "Food & Hospitality", enterprise: "Shebeen / informal tavern", type: "S/H", typicalScale: "Micro/Informal", description: "Local drinking/social venue often serving snacks." },
    { sector: "Food & Hospitality", enterprise: "Township restaurant / Imbizo / braai stalls", type: "S/H", typicalScale: "Micro/Small", description: "Cooked-food outlets, often outdoor." },
    { sector: "Food & Hospitality", enterprise: "Home bakeries & confectioneries", type: "P/H", typicalScale: "Micro", description: "Bread, cakes made at home for sale." },
    { sector: "Food & Hospitality", enterprise: "Prepared-food vendors / street food", type: "S/H", typicalScale: "Micro/Informal", description: "Ready-to-eat meals (pap, vetkoek, samosas)." },
    
    // Personal & Household Services
    { sector: "Personal & Household Services", enterprise: "Hair salons / barbershops", type: "S", typicalScale: "Micro", description: "Haircuts and styling from small shops or at home." },
    { sector: "Personal & Household Services", enterprise: "Beauty & nail services", type: "S", typicalScale: "Micro", description: "Manicure, pedicure and beauty treatments." },
    { sector: "Personal & Household Services", enterprise: "Laundry / wash-and-fold", type: "S", typicalScale: "Micro/Small", description: "Manual or small-machine based laundry services." },
    { sector: "Personal & Household Services", enterprise: "Domestic cleaning / housekeepers", type: "S", typicalScale: "Micro", description: "Home cleaning and chores." },
    
    // Trades & Technical Services
    { sector: "Trades & Technical Services", enterprise: "Electricians", type: "S", typicalScale: "Micro/Small", description: "Residential/commercial wiring, repairs." },
    { sector: "Trades & Technical Services", enterprise: "Plumbers", type: "S", typicalScale: "Micro/Small", description: "Drainage, piping, toilet installation and repair." },
    { sector: "Trades & Technical Services", enterprise: "Carpenters & joiners", type: "P/H", typicalScale: "Micro/Small", description: "Furniture manufacture and installation." },
    { sector: "Trades & Technical Services", enterprise: "Welders & metalworkers", type: "P/H", typicalScale: "Micro/Small", description: "Gates, repairs, fabrication." },
    { sector: "Trades & Technical Services", enterprise: "Mechanics (vehicle repairs)", type: "S/H", typicalScale: "Micro/Small", description: "Engine, brake, general repairs." },
    
    // Automotive & Mobility Services
    { sector: "Automotive & Mobility Services", enterprise: "Car washes (fixed bays)", type: "S", typicalScale: "Micro/Small", description: "Manual/pressure cleaning." },
    { sector: "Automotive & Mobility Services", enterprise: "Mobile car wash", type: "S", typicalScale: "Micro", description: "Door-to-door vehicle cleaning." },
    { sector: "Automotive & Mobility Services", enterprise: "Minibus taxi / shared transport", type: "S", typicalScale: "Small", description: "Passenger transport services." },
    
    // Agriculture & Agribusiness
    { sector: "Agriculture & Agribusiness", enterprise: "Small-scale vegetable gardening", type: "P", typicalScale: "Micro", description: "Backyard or community-plot produce." },
    { sector: "Agriculture & Agribusiness", enterprise: "Poultry (chicken) farming", type: "P", typicalScale: "Micro/Small", description: "Eggs and broiler production." },
    { sector: "Agriculture & Agribusiness", enterprise: "Small livestock (goats, sheep)", type: "P", typicalScale: "Micro/Small", description: "Meat and breeding stock." },
    
    // Financial Services & Group Savings
    { sector: "Financial Services & Group Savings", enterprise: "Stokvels / rotating savings groups", type: "S/H", typicalScale: "Micro", description: "Collective savings and bulk buying." },
    { sector: "Financial Services & Group Savings", enterprise: "Agent banking / airtime & voucher shops", type: "S/H", typicalScale: "Micro", description: "Financial transactions and airtime sales." }
  ]
  
  const sectors = computed(() => {
    const uniqueSectors = [...new Set(enterpriseData.map(item => item.sector))]
    return uniqueSectors.sort()
  })
  
  const getEnterprisesBySector = (sector: string) => {
    return enterpriseData.filter(item => item.sector === sector)
  }
  
  const allEnterprises = computed(() => enterpriseData)
  
  const popularEnterprises = computed(() => [
    "Spaza / Convenience shop",
    "Hair salons / barbershops", 
    "Mobile car wash",
    "Small-scale vegetable gardening",
    "Agent banking / airtime & voucher shops",
    "Township restaurant / Imbizo / braai stalls",
    "Electricians",
    "Plumbers",
    "Mechanics (vehicle repairs)",
    "Stokvels / rotating savings groups"
  ])
  
  return {
    sectors,
    allEnterprises,
    getEnterprisesBySector,
    popularEnterprises
  }
}

const { 
  sectors, 
  getEnterprisesBySector, 
  allEnterprises,
  popularEnterprises 
} = useEnterpriseData()

// Form state
const loading = ref(false)
const selectedSector = ref('')
const form = ref({
  firstName: '',
  lastName: '',
  email: '',
  phone: '',
  company: '',
  jobTitle: '',
  type: '',
  status: '',
  source: '',
  enterpriseType: '',
  enterpriseSector: ''
})

// Computed properties for enterprise selection
const filteredEnterprises = computed(() => {
  if (!selectedSector.value) return []
  return getEnterprisesBySector(selectedSector.value)
})

const selectedEnterpriseDetails = computed(() => {
  if (!form.value.enterpriseType) return null
  return allEnterprises.value.find(e => e.enterprise === form.value.enterpriseType)
})

// Methods for enterprise selection
const onSectorChange = () => {
  form.value.enterpriseType = ''
  form.value.enterpriseSector = selectedSector.value
}

const onEnterpriseTypeChange = () => {
  const enterprise = allEnterprises.value.find(e => e.enterprise === form.value.enterpriseType)
  if (enterprise) {
    selectedSector.value = enterprise.sector
    form.value.enterpriseSector = enterprise.sector
  }
}

const selectPopularEnterprise = (enterpriseName: string) => {
  const enterprise = allEnterprises.value.find(e => e.enterprise === enterpriseName)
  if (enterprise) {
    selectedSector.value = enterprise.sector
    form.value.enterpriseType = enterprise.enterprise
    form.value.enterpriseSector = enterprise.sector
  }
}

// Watch for contact prop changes
watch(() => props.contact, (newContact) => {
  if (newContact && props.isEditMode) {
    form.value = {
      firstName: newContact.firstName || '',
      lastName: newContact.lastName || '',
      email: newContact.email || '',
      phone: newContact.phone || '',
      company: newContact.company || '',
      jobTitle: newContact.jobTitle || '',
      type: newContact.type || '',
      status: newContact.status || '',
      source: newContact.source || '',
      enterpriseType: newContact.enterpriseType || '',
      enterpriseSector: newContact.enterpriseSector || ''
    }
    
    // Set selected sector for editing
    if (newContact.enterpriseSector) {
      selectedSector.value = newContact.enterpriseSector
    }
  }
}, { immediate: true })

// Watch for modal open/close to reset form
watch(() => props.isOpen, (isOpen) => {
  if (isOpen && !props.isEditMode) {
    // Reset form for new contact
    form.value = {
      firstName: '',
      lastName: '',
      email: '',
      phone: '',
      company: '',
      jobTitle: '',
      type: '',
      status: '',
      source: '',
      enterpriseType: '',
      enterpriseSector: ''
    }
    selectedSector.value = ''
  }
})

// Methods
const closeModal = () => {
  emit('close')
}

const submitForm = async () => {
  loading.value = true
  
  try {
    // Create contact object
    const contactData = {
      ...form.value,
      id: props.isEditMode ? props.contact?.id : undefined
    }
    
    emit('save', contactData)
  } catch (error) {
    console.error('Error saving contact:', error)
  } finally {
    loading.value = false
  }
}
</script>
