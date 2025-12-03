import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

export interface Customer {
  id: string
  name: string
  email?: string
  phone: string
  address?: string
  city?: string
  postalCode?: string
  customerType: 'individual' | 'business'
  creditLimit: number
  currentBalance: number
  outstandingAmount: number
  totalPurchases: number
  lastPurchaseDate?: Date
  status: 'active' | 'inactive'
  tags: string[]
  notes?: string
  createdAt: Date
  updatedAt: Date
}

export interface Lead {
  id: string
  name: string
  email?: string
  phone: string
  company?: string
  source: 'website' | 'referral' | 'walk-in' | 'phone' | 'social' | 'other'
  status: 'new' | 'contacted' | 'qualified' | 'proposal' | 'negotiation' | 'won' | 'lost'
  interest: string
  estimatedValue?: number
  probability: number
  expectedCloseDate?: Date
  notes?: string
  assignedTo?: string
  createdAt: Date
  updatedAt: Date
}

export interface Communication {
  id: string
  relatedTo: 'customer' | 'lead'
  relatedId: string
  type: 'call' | 'email' | 'meeting' | 'whatsapp' | 'sms' | 'note'
  subject: string
  content: string
  direction: 'inbound' | 'outbound'
  status: 'completed' | 'scheduled' | 'cancelled'
  scheduledAt?: Date
  completedAt?: Date
  createdBy: string
  createdAt: Date
}

export interface Opportunity {
  id: string
  leadId?: string
  customerId?: string
  name: string
  value: number
  stage: 'prospecting' | 'qualification' | 'proposal' | 'negotiation' | 'closed_won' | 'closed_lost'
  probability: number
  expectedCloseDate: Date
  products: string[]
  notes?: string
  createdAt: Date
}

export const useCrmStore = defineStore('crm', () => {
  // State
  const customers = ref<Customer[]>([])
  const leads = ref<Lead[]>([])
  const communications = ref<Communication[]>([])
  const opportunities = ref<Opportunity[]>([])
  const loading = ref(false)

  // Computed
  const activeCustomers = computed(() => {
    return customers.value.filter(c => c.status === 'active')
  })

  const customersWithCredit = computed(() => {
    return customers.value.filter(c => c.currentBalance > 0)
  })

  const totalOutstanding = computed(() => {
    return customers.value.reduce((sum, c) => sum + c.outstandingAmount, 0)
  })

  const activeLeads = computed(() => {
    return leads.value.filter(l => 
      l.status !== 'won' && l.status !== 'lost'
    )
  })

  const leadsByStage = computed(() => {
    const stages: Record<string, Lead[]> = {}
    leads.value.forEach(lead => {
      if (!stages[lead.status]) {
        stages[lead.status] = []
      }
      stages[lead.status].push(lead)
    })
    return stages
  })

  const topCustomers = computed(() => {
    return [...customers.value]
      .sort((a, b) => b.totalPurchases - a.totalPurchases)
      .slice(0, 10)
  })

  const recentCommunications = computed(() => {
    return [...communications.value]
      .sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
      .slice(0, 20)
  })

  // Actions
  async function fetchCustomers() {
    loading.value = true
    try {
      // TODO: Replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 500))
      
      // Mock data
      customers.value = [
        {
          id: '1',
          name: 'Thabo Builders',
          phone: '+27 82 123 4567',
          email: 'thabo@builders.co.za',
          address: '123 Main Road, Soweto',
          city: 'Johannesburg',
          postalCode: '1809',
          customerType: 'business',
          creditLimit: 50000,
          currentBalance: 15000,
          outstandingAmount: 15000,
          totalPurchases: 125000,
          lastPurchaseDate: new Date(Date.now() - 5 * 24 * 60 * 60 * 1000),
          status: 'active',
          tags: ['wholesale', 'vip'],
          notes: 'Regular customer, always pays on time',
          createdAt: new Date(Date.now() - 180 * 24 * 60 * 60 * 1000),
          updatedAt: new Date()
        },
        {
          id: '2',
          name: 'Sipho Grocery',
          phone: '+27 83 234 5678',
          customerType: 'business',
          creditLimit: 20000,
          currentBalance: 5000,
          outstandingAmount: 5000,
          totalPurchases: 45000,
          lastPurchaseDate: new Date(Date.now() - 2 * 24 * 60 * 60 * 1000),
          status: 'active',
          tags: ['retail'],
          createdAt: new Date(Date.now() - 90 * 24 * 60 * 60 * 1000),
          updatedAt: new Date()
        }
      ]
    } catch (error) {
      console.error('Failed to fetch customers:', error)
    } finally {
      loading.value = false
    }
  }

  async function fetchLeads() {
    loading.value = true
    try {
      // TODO: Replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 500))
      
      // Mock data
      leads.value = [
        {
          id: '1',
          name: 'Nomsa Catering',
          phone: '+27 84 345 6789',
          email: 'nomsa@catering.co.za',
          company: 'Nomsa Events',
          source: 'referral',
          status: 'qualified',
          interest: 'Bulk food supplies for events',
          estimatedValue: 30000,
          probability: 60,
          expectedCloseDate: new Date(Date.now() + 14 * 24 * 60 * 60 * 1000),
          notes: 'Referred by Thabo Builders',
          createdAt: new Date(Date.now() - 7 * 24 * 60 * 60 * 1000),
          updatedAt: new Date()
        }
      ]
    } catch (error) {
      console.error('Failed to fetch leads:', error)
    } finally {
      loading.value = false
    }
  }

  async function fetchCommunications(relatedId?: string) {
    loading.value = true
    try {
      // TODO: Replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 500))
      
      // Mock data
      communications.value = []
    } catch (error) {
      console.error('Failed to fetch communications:', error)
    } finally {
      loading.value = false
    }
  }

  async function createCustomer(data: Omit<Customer, 'id' | 'createdAt' | 'updatedAt'>) {
    loading.value = true
    try {
      // TODO: Replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 500))
      
      const customer: Customer = {
        ...data,
        id: `cust_${Date.now()}`,
        createdAt: new Date(),
        updatedAt: new Date()
      }
      
      customers.value.unshift(customer)
      return customer
    } catch (error) {
      console.error('Failed to create customer:', error)
      throw error
    } finally {
      loading.value = false
    }
  }

  async function updateCustomer(id: string, updates: Partial<Customer>) {
    loading.value = true
    try {
      // TODO: Replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 500))
      
      const index = customers.value.findIndex(c => c.id === id)
      if (index !== -1) {
        customers.value[index] = {
          ...customers.value[index],
          ...updates,
          updatedAt: new Date()
        }
      }
    } catch (error) {
      console.error('Failed to update customer:', error)
      throw error
    } finally {
      loading.value = false
    }
  }

  async function createLead(data: Omit<Lead, 'id' | 'createdAt' | 'updatedAt'>) {
    loading.value = true
    try {
      // TODO: Replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 500))
      
      const lead: Lead = {
        ...data,
        id: `lead_${Date.now()}`,
        createdAt: new Date(),
        updatedAt: new Date()
      }
      
      leads.value.unshift(lead)
      return lead
    } catch (error) {
      console.error('Failed to create lead:', error)
      throw error
    } finally {
      loading.value = false
    }
  }

  async function convertLeadToCustomer(leadId: string) {
    const lead = leads.value.find(l => l.id === leadId)
    if (!lead) throw new Error('Lead not found')

    loading.value = true
    try {
      // TODO: Replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 500))
      
      const customer: Customer = {
        id: `cust_${Date.now()}`,
        name: lead.name,
        email: lead.email,
        phone: lead.phone,
        customerType: lead.company ? 'business' : 'individual',
        creditLimit: 0,
        currentBalance: 0,
        outstandingAmount: 0,
        totalPurchases: 0,
        status: 'active',
        tags: [],
        notes: `Converted from lead: ${lead.interest}`,
        createdAt: new Date(),
        updatedAt: new Date()
      }
      
      customers.value.unshift(customer)
      lead.status = 'won'
      
      return customer
    } catch (error) {
      console.error('Failed to convert lead:', error)
      throw error
    } finally {
      loading.value = false
    }
  }

  async function logCommunication(data: Omit<Communication, 'id' | 'createdAt'>) {
    loading.value = true
    try {
      // TODO: Replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 500))
      
      const communication: Communication = {
        ...data,
        id: `comm_${Date.now()}`,
        createdAt: new Date()
      }
      
      communications.value.unshift(communication)
      return communication
    } catch (error) {
      console.error('Failed to log communication:', error)
      throw error
    } finally {
      loading.value = false
    }
  }

  function searchCustomers(query: string): Customer[] {
    const lowerQuery = query.toLowerCase()
    return customers.value.filter(customer => 
      customer.name.toLowerCase().includes(lowerQuery) ||
      customer.phone.includes(query) ||
      customer.email?.toLowerCase().includes(lowerQuery)
    )
  }

  function getCustomerById(id: string): Customer | undefined {
    return customers.value.find(c => c.id === id)
  }

  return {
    // State
    customers,
    leads,
    communications,
    opportunities,
    loading,
    // Computed
    activeCustomers,
    customersWithCredit,
    totalOutstanding,
    activeLeads,
    leadsByStage,
    topCustomers,
    recentCommunications,
    // Actions
    fetchCustomers,
    fetchLeads,
    fetchCommunications,
    createCustomer,
    updateCustomer,
    createLead,
    convertLeadToCustomer,
    logCommunication,
    searchCustomers,
    getCustomerById
  }
})

