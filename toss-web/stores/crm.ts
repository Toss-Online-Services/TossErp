import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { useCrmApi } from '~/composables/useCrmApi'

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
  const crmApi = useCrmApi()
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
  async function fetchCustomers(storeId?: number) {
    loading.value = true
    try {
      const { data, error } = await crmApi.getCustomers(storeId)
      if (error.value) {
        console.error('Failed to fetch customers:', error.value)
        return
      }
      const items = data.value?.items ?? data.value ?? []
      customers.value = items.map(mapCustomerFromApi)
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

  async function fetchCommunications(customerId: string) {
    loading.value = true
    try {
      const { data, error } = await crmApi.getInteractions(Number(customerId))
      if (error.value) {
        console.error('Failed to fetch communications:', error.value)
        return
      }
      const items = data.value?.items ?? data.value ?? []
      communications.value = items.map(mapCommunicationFromApi(customerId))
    } finally {
      loading.value = false
    }
  }

  async function createCustomer(data: Omit<Customer, 'id' | 'createdAt' | 'updatedAt'>) {
    loading.value = true
    try {
      const { data: created, error } = await crmApi.createCustomer(data)
      if (error.value) {
        console.error('Failed to create customer:', error.value)
        throw error.value
      }
      const id = created.value?.id ? String(created.value.id) : `cust_${Date.now()}`
      const customer: Customer = {
        ...data,
        id,
        createdAt: new Date(),
        updatedAt: new Date()
      }
      
      customers.value.unshift(customer)
      return customer
    } finally {
      loading.value = false
    }
  }

  async function updateCustomer(id: string, updates: Partial<Customer>) {
    loading.value = true
    try {
      const { error } = await crmApi.updateCustomer(Number(id), updates)
      if (error.value) {
        console.error('Failed to update customer:', error.value)
        throw error.value
      }
      
      const index = customers.value.findIndex(c => c.id === id)
      if (index !== -1) {
        customers.value[index] = {
          ...customers.value[index],
          ...updates,
          updatedAt: new Date()
        }
      }
    } finally {
      loading.value = false
    }
  }

  async function deleteCustomer(id: string) {
    loading.value = true
    try {
      const { error } = await crmApi.deleteCustomer(Number(id))
      if (error.value) {
        console.error('Failed to delete customer:', error.value)
        throw error.value
      }
      customers.value = customers.value.filter(c => c.id !== id)
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

  function mapCustomerFromApi(customer: any): Customer {
    const fallbackName = `${customer.firstName ?? ''} ${customer.lastName ?? ''}`.trim() || 'Customer'
    return {
      id: String(customer.id ?? customer.customerId ?? crypto.randomUUID()),
      name: customer.name ?? fallbackName,
      email: customer.email,
      phone: customer.phone ?? customer.mobileNumber,
      address: customer.address,
      city: customer.city,
      postalCode: customer.postalCode,
      customerType: customer.customerType ?? 'business',
      creditLimit: customer.creditLimit ?? 0,
      currentBalance: customer.currentBalance ?? customer.balance ?? 0,
      outstandingAmount: customer.outstandingAmount ?? customer.balance ?? 0,
      totalPurchases: customer.totalPurchases ?? 0,
      lastPurchaseDate: customer.lastPurchaseDate ? new Date(customer.lastPurchaseDate) : undefined,
      status: customer.status ?? 'active',
      tags: customer.tags ?? [],
      notes: customer.notes,
      createdAt: customer.createdAt ? new Date(customer.createdAt) : new Date(),
      updatedAt: customer.updatedAt ? new Date(customer.updatedAt) : new Date()
    }
  }

  function mapCommunicationFromApi(customerId: string) {
    return (comm: any): Communication => ({
      id: String(comm.id ?? crypto.randomUUID()),
      relatedTo: 'customer',
      relatedId: customerId,
      type: comm.type ?? 'note',
      subject: comm.subject ?? 'Interaction',
      content: comm.content ?? '',
      direction: comm.direction ?? 'outbound',
      status: comm.status ?? 'completed',
      scheduledAt: comm.scheduledAt ? new Date(comm.scheduledAt) : undefined,
      completedAt: comm.completedAt ? new Date(comm.completedAt) : undefined,
      createdBy: comm.createdBy ?? 'system',
      createdAt: comm.createdAt ? new Date(comm.createdAt) : new Date()
    })
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
    deleteCustomer,
    createLead,
    convertLeadToCustomer,
    logCommunication,
    searchCustomers,
    getCustomerById
  }
})

