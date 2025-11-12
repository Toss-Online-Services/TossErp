<template>
  <div class="container mx-auto p-6 max-w-7xl">
    <!-- Header -->
    <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center mb-6">
      <div>
        <h1 class="text-3xl font-bold">{{ $t('crm.contacts.title', 'Customer Contacts') }}</h1>
        <p class="text-muted-foreground mt-1">
          {{ $t('crm.contacts.subtitle', 'Manage your customer relationships') }}
        </p>
      </div>
      <div class="flex gap-2 mt-4 sm:mt-0">
        <Button variant="outline" @click="exportContacts">
          <Download class="mr-2 h-4 w-4" />
          {{ $t('common.export') }}
        </Button>
        <Button @click="showCreateDialog = true">
          <Plus class="mr-2 h-4 w-4" />
          {{ $t('crm.contacts.addContact') }}
        </Button>
      </div>
    </div>

    <!-- Filters and Search -->
    <Card class="mb-6">
      <CardContent class="p-6">
        <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
          <div class="relative">
            <Search class="absolute left-3 top-3 h-4 w-4 text-muted-foreground" />
            <input
              v-model="searchTerm"
              type="text"
              :placeholder="$t('common.search')"
              class="pl-10 w-full h-10 rounded-md border border-input bg-background px-3 py-2 text-sm"
            />
          </div>
          <select v-model="selectedType" class="h-10 rounded-md border border-input bg-background px-3 py-2 text-sm">
            <option value="">{{ $t('crm.contacts.allTypes') }}</option>
            <option value="spaza">{{ $t('crm.contacts.spaza') }}</option>
            <option value="chisa_nyama">{{ $t('crm.contacts.chisaNyama') }}</option>
            <option value="supplier">{{ $t('crm.contacts.supplier') }}</option>
            <option value="individual">{{ $t('crm.contacts.individual') }}</option>
          </select>
          <select v-model="selectedLocation" class="h-10 rounded-md border border-input bg-background px-3 py-2 text-sm">
            <option value="">{{ $t('crm.contacts.allLocations') }}</option>
            <option v-for="location in uniqueLocations" :key="location" :value="location">
              {{ location }}
            </option>
          </select>
          <Button variant="outline" @click="resetFilters" class="h-10">
            <X class="mr-2 h-4 w-4" />
            {{ $t('common.reset') }}
          </Button>
        </div>
      </CardContent>
    </Card>

    <!-- Contacts Grid -->
    <div class="grid grid-cols-1 lg:grid-cols-2 xl:grid-cols-3 gap-6">
      <Card 
        v-for="contact in filteredContacts" 
        :key="contact.id"
        class="hover:shadow-md transition-shadow cursor-pointer"
        @click="selectContact(contact)"
      >
        <CardContent class="p-6">
          <div class="flex items-start justify-between mb-4">
            <div class="flex items-center gap-3">
              <div class="w-12 h-12 rounded-full bg-primary/10 flex items-center justify-center">
                <Component :is="getContactIcon(contact.type)" class="h-6 w-6 text-primary" />
              </div>
              <div>
                <h3 class="font-semibold text-lg">{{ contact.name }}</h3>
                <p class="text-sm text-muted-foreground">{{ contact.businessName }}</p>
              </div>
            </div>
            <Badge :variant="getContactVariant(contact.type)">
              {{ $t(`crm.contacts.${contact.type}`) }}
            </Badge>
          </div>

          <div class="space-y-2 mb-4">
            <div class="flex items-center gap-2 text-sm">
              <Phone class="h-4 w-4 text-muted-foreground" />
              <span>{{ contact.phone }}</span>
            </div>
            <div class="flex items-center gap-2 text-sm">
              <MapPin class="h-4 w-4 text-muted-foreground" />
              <span class="truncate">{{ contact.location }}</span>
            </div>
            <div v-if="contact.creditLimit" class="flex items-center gap-2 text-sm">
              <CreditCard class="h-4 w-4 text-muted-foreground" />
              <span>Credit: {{ formatCurrency(contact.creditLimit) }}</span>
            </div>
          </div>

          <div class="flex items-center justify-between">
            <div class="text-xs text-muted-foreground">
              Last contact: {{ formatDate(contact.lastContact) }}
            </div>
            <div class="flex gap-1">
              <Button size="sm" variant="ghost" @click.stop="callContact(contact.phone)">
                <Phone class="h-4 w-4" />
              </Button>
              <Button size="sm" variant="ghost" @click.stop="sendWhatsApp(contact.phone)">
                <MessageCircle class="h-4 w-4" />
              </Button>
              <Button size="sm" variant="ghost" @click.stop="editContact(contact)">
                <Edit class="h-4 w-4" />
              </Button>
            </div>
          </div>

          <div class="mt-4 pt-4 border-t">
            <div class="flex justify-between text-sm">
              <span class="text-muted-foreground">Total Orders:</span>
              <span class="font-medium">{{ contact.totalOrders }}</span>
            </div>
            <div class="flex justify-between text-sm mt-1">
              <span class="text-muted-foreground">Total Value:</span>
              <span class="font-medium">{{ formatCurrency(contact.totalValue) }}</span>
            </div>
          </div>
        </CardContent>
      </Card>
    </div>

    <!-- Empty State -->
    <Card v-if="filteredContacts.length === 0" class="text-center py-12">
      <CardContent>
        <Users class="h-12 w-12 mx-auto text-muted-foreground mb-4" />
        <h3 class="text-lg font-medium mb-2">{{ $t('crm.contacts.noContacts') }}</h3>
        <p class="text-muted-foreground mb-4">
          {{ $t('crm.contacts.noContactsDesc') }}
        </p>
        <Button @click="showCreateDialog = true">
          {{ $t('crm.contacts.addFirstContact') }}
        </Button>
      </CardContent>
    </Card>

    <!-- Create/Edit Contact Dialog -->
    <ContactDialog 
      v-model:open="showCreateDialog"
      :contact="selectedContactForEdit"
      @save="handleSaveContact"
      @close="handleCloseDialog"
    />

    <!-- Contact Details Modal -->
    <ContactDetails
      v-model:open="showContactDetails"
      :contact="selectedContact"
      @edit="editContact"
      @close="selectedContact = null"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import { 
  Plus, 
  Search, 
  Download,
  X,
  Phone, 
  MapPin, 
  CreditCard,
  MessageCircle,
  Edit,
  Users,
  Store,
  Utensils,
  Truck,
  User
} from 'lucide-vue-next'

import { Button } from '../../components/ui/button'
import { Card, CardContent } from '../../components/ui/card'
import { Badge } from '../../components/ui/badge'

// Types
interface Contact {
  id: string
  name: string
  businessName?: string
  phone: string
  whatsapp?: string
  location: string
  type: 'spaza' | 'chisa_nyama' | 'supplier' | 'individual'
  creditLimit?: number
  creditUsed?: number
  totalOrders: number
  totalValue: number
  lastContact: Date
  notes?: string
  email?: string
}

// State
const { t } = useI18n()
const searchTerm = ref('')
const selectedType = ref('')
const selectedLocation = ref('')
const showCreateDialog = ref(false)
const showContactDetails = ref(false)
const selectedContact = ref<Contact | null>(null)
const selectedContactForEdit = ref<Contact | null>(null)

// Mock data - in real app this would come from API
const contacts = ref<Contact[]>([
  {
    id: '1',
    name: 'Mthunzi Mthembu',
    businessName: 'Mthunzi\'s Spaza Shop',
    phone: '+27821234567',
    whatsapp: '+27821234567',
    location: 'Soweto, Johannesburg',
    type: 'spaza',
    creditLimit: 5000,
    creditUsed: 2300,
    totalOrders: 45,
    totalValue: 23750,
    lastContact: new Date(2024, 0, 15),
    notes: 'Regular customer, good payment history'
  },
  {
    id: '2',
    name: 'Nomsa Dlamini',
    businessName: 'Nomsa\'s Chisa Nyama',
    phone: '+27729876543',
    location: 'Alexandra, Johannesburg',
    type: 'chisa_nyama',
    creditLimit: 3000,
    creditUsed: 1200,
    totalOrders: 28,
    totalValue: 18900,
    lastContact: new Date(2024, 0, 12),
    notes: 'Weekend bulk orders for meat and beverages'
  },
  {
    id: '3',
    name: 'Sipho Mthembu',
    businessName: 'Township Wholesalers',
    phone: '+27114567890',
    location: 'Tembisa, Ekurhuleni',
    type: 'supplier',
    totalOrders: 12,
    totalValue: 125000,
    lastContact: new Date(2024, 0, 10),
    notes: 'Main supplier for dry goods and beverages'
  },
  {
    id: '4',
    name: 'Thandiwe Ndlovu',
    businessName: 'Thandi\'s General Store',
    phone: '+27836547821',
    location: 'Mamelodi, Pretoria',
    type: 'spaza',
    creditLimit: 2500,
    creditUsed: 800,
    totalOrders: 22,
    totalValue: 12400,
    lastContact: new Date(2024, 0, 8),
    notes: 'New customer, growing quickly'
  }
])

// Computed
const filteredContacts = computed(() => {
  return contacts.value.filter(contact => {
    const matchesSearch = !searchTerm.value || 
      contact.name.toLowerCase().includes(searchTerm.value.toLowerCase()) ||
      contact.businessName?.toLowerCase().includes(searchTerm.value.toLowerCase())
    
    const matchesType = !selectedType.value || contact.type === selectedType.value
    const matchesLocation = !selectedLocation.value || contact.location.includes(selectedLocation.value)
    
    return matchesSearch && matchesType && matchesLocation
  })
})

const uniqueLocations = computed(() => {
  const locations = contacts.value.map(c => c.location.split(', ')[1] || c.location)
  return [...new Set(locations)].sort()
})

// Methods
const getContactIcon = (type: string) => {
  switch (type) {
    case 'spaza': return Store
    case 'chisa_nyama': return Utensils
    case 'supplier': return Truck
    case 'individual': return User
    default: return User
  }
}

const getContactVariant = (type: string) => {
  switch (type) {
    case 'spaza': return 'default'
    case 'chisa_nyama': return 'secondary'
    case 'supplier': return 'outline'
    case 'individual': return 'outline'
    default: return 'outline'
  }
}

const selectContact = (contact: Contact) => {
  selectedContact.value = contact
  showContactDetails.value = true
}

const editContact = (contact: Contact) => {
  selectedContactForEdit.value = contact
  showCreateDialog.value = true
}

const resetFilters = () => {
  searchTerm.value = ''
  selectedType.value = ''
  selectedLocation.value = ''
}

const callContact = (phone: string) => {
  window.open(`tel:${phone}`)
}

const sendWhatsApp = (phone: string) => {
  const message = encodeURIComponent('Hello from TOSS ERP!')
  window.open(`https://wa.me/${phone.replace('+', '')}?text=${message}`)
}

const exportContacts = () => {
  const csvData = contacts.value.map(c => ({
    Name: c.name,
    Business: c.businessName || '',
    Phone: c.phone,
    Location: c.location,
    Type: c.type,
    CreditLimit: c.creditLimit || 0,
    TotalOrders: c.totalOrders,
    TotalValue: c.totalValue
  }))
  
  // Convert to CSV and download
  const csv = [
    Object.keys(csvData[0]).join(','),
    ...csvData.map(row => Object.values(row).join(','))
  ].join('\n')
  
  const blob = new Blob([csv], { type: 'text/csv' })
  const url = window.URL.createObjectURL(blob)
  const a = document.createElement('a')
  a.href = url
  a.download = 'contacts.csv'
  a.click()
  window.URL.revokeObjectURL(url)
}

const handleSaveContact = (contact: Contact) => {
  if (selectedContactForEdit.value) {
    // Update existing contact
    const index = contacts.value.findIndex(c => c.id === contact.id)
    if (index !== -1) {
      contacts.value[index] = contact
    }
  } else {
    // Add new contact
    contact.id = Date.now().toString()
    contacts.value.push(contact)
  }
  
  handleCloseDialog()
}

const handleCloseDialog = () => {
  showCreateDialog.value = false
  selectedContactForEdit.value = null
}

const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-ZA', {
    style: 'currency',
    currency: 'ZAR',
    minimumFractionDigits: 0
  }).format(amount)
}

const formatDate = (date: Date) => {
  return new Intl.DateTimeFormat('en-ZA', {
    day: 'numeric',
    month: 'short',
    year: 'numeric'
  }).format(date)
}

onMounted(() => {
  console.log('CRM Contacts loaded')
})
</script>