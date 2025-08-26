export interface Customer {
  id: number
  firstName: string
  lastName: string
  email: string
  phone: string
  company: string
  jobTitle: string
  createdAt: string
  updatedAt: string
  status: string
  source: string
  lifetimeValue: number
  lastContactDate: string
  notes: string
  subscriptions: Subscription[]
}

export interface Subscription {
  id: number
  customerId: number
  serviceName: string
  status: string
  monthlyFee: number
  startDate: string
  endDate?: string
  billingCycle: string
}

export interface CustomerFilters {
  search: string
  status: string
  page: number
  pageSize: number
}

export const useCustomerStore = defineStore('customers', () => {
  // State
  const customers = ref<Customer[]>([
    {
      id: 1,
      firstName: "John",
      lastName: "Doe",
      email: "john.doe@example.com",
      phone: "+1-555-1234",
      company: "Acme Corp",
      jobTitle: "Manager",
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
      status: "Active",
      source: "Website",
      lifetimeValue: 15000,
      lastContactDate: new Date(Date.now() - 5 * 24 * 60 * 60 * 1000).toISOString(),
      notes: "VIP customer, prefers email communication",
      subscriptions: [
        {
          id: 1,
          customerId: 1,
          serviceName: "Premium Support",
          status: "Active",
          monthlyFee: 299.99,
          startDate: new Date(Date.now() - 6 * 30 * 24 * 60 * 60 * 1000).toISOString(),
          billingCycle: "Monthly"
        }
      ]
    },
    {
      id: 2,
      firstName: "Jane",
      lastName: "Smith",
      email: "jane.smith@techco.com",
      phone: "+1-555-5678",
      company: "TechCo",
      jobTitle: "Director",
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
      status: "Active",
      source: "Referral",
      lifetimeValue: 25000,
      lastContactDate: new Date(Date.now() - 2 * 24 * 60 * 60 * 1000).toISOString(),
      notes: "Looking to expand services",
      subscriptions: [
        {
          id: 2,
          customerId: 2,
          serviceName: "Enterprise Package",
          status: "Active",
          monthlyFee: 999.99,
          startDate: new Date(Date.now() - 12 * 30 * 24 * 60 * 60 * 1000).toISOString(),
          billingCycle: "Annual"
        }
      ]
    },
    {
      id: 3,
      firstName: "Mike",
      lastName: "Johnson",
      email: "mike.johnson@startup.co",
      phone: "+1-555-9999",
      company: "StartupCo",
      jobTitle: "CEO",
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
      status: "Lead",
      source: "Cold Call",
      lifetimeValue: 0,
      lastContactDate: new Date(Date.now() - 1 * 24 * 60 * 60 * 1000).toISOString(),
      notes: "Interested in starter package",
      subscriptions: []
    },
    {
      id: 4,
      firstName: "Sarah",
      lastName: "Williams",
      email: "sarah.williams@corp.com",
      phone: "+1-555-7777",
      company: "BigCorp",
      jobTitle: "VP Sales",
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
      status: "Inactive",
      source: "Trade Show",
      lifetimeValue: 50000,
      lastContactDate: new Date(Date.now() - 30 * 24 * 60 * 60 * 1000).toISOString(),
      notes: "Contract expired, needs follow-up",
      subscriptions: [
        {
          id: 3,
          customerId: 4,
          serviceName: "Enterprise Package",
          status: "Expired",
          monthlyFee: 1999.99,
          startDate: new Date(Date.now() - 24 * 30 * 24 * 60 * 60 * 1000).toISOString(),
          endDate: new Date(Date.now() - 30 * 24 * 60 * 60 * 1000).toISOString(),
          billingCycle: "Annual"
        }
      ]
    }
  ])

  const loading = ref(false)
  const error = ref<string | null>(null)

  // Getters
  const filteredCustomers = computed(() => {
    return customers.value
  })

  const customerStats = computed(() => {
    const total = customers.value.length
    const active = customers.value.filter(c => c.status === 'Active').length
    const leads = customers.value.filter(c => c.status === 'Lead').length
    const inactive = customers.value.filter(c => c.status === 'Inactive').length
    const totalValue = customers.value.reduce((sum, c) => sum + c.lifetimeValue, 0)

    return { total, active, leads, inactive, totalValue }
  })

  // Actions
  const fetchCustomers = async (filters: Partial<CustomerFilters> = {}) => {
    loading.value = true
    error.value = null
    
    try {
      // Simulate API delay
      await new Promise(resolve => setTimeout(resolve, 500))
      
      let filtered = [...customers.value]
      
      if (filters.search) {
        const search = filters.search.toLowerCase()
        filtered = filtered.filter(c => 
          c.firstName.toLowerCase().includes(search) ||
          c.lastName.toLowerCase().includes(search) ||
          c.email.toLowerCase().includes(search) ||
          c.company.toLowerCase().includes(search)
        )
      }
      
      if (filters.status) {
        filtered = filtered.filter(c => c.status === filters.status)
      }
      
      // Pagination
      const page = filters.page || 1
      const pageSize = filters.pageSize || 10
      const start = (page - 1) * pageSize
      const end = start + pageSize
      
      return {
        data: filtered.slice(start, end),
        total: filtered.length,
        page,
        pageSize
      }
    } catch (err) {
      error.value = 'Failed to fetch customers'
      throw err
    } finally {
      loading.value = false
    }
  }

  const getCustomerById = (id: number) => {
    return customers.value.find(c => c.id === id)
  }

  const createCustomer = async (customerData: Omit<Customer, 'id' | 'createdAt' | 'updatedAt'>) => {
    loading.value = true
    error.value = null
    
    try {
      // Simulate API delay
      await new Promise(resolve => setTimeout(resolve, 300))
      
      const newCustomer: Customer = {
        ...customerData,
        id: Math.max(...customers.value.map(c => c.id)) + 1,
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString()
      }
      
      customers.value.push(newCustomer)
      return newCustomer
    } catch (err) {
      error.value = 'Failed to create customer'
      throw err
    } finally {
      loading.value = false
    }
  }

  const updateCustomer = async (id: number, customerData: Partial<Customer>) => {
    loading.value = true
    error.value = null
    
    try {
      // Simulate API delay
      await new Promise(resolve => setTimeout(resolve, 300))
      
      const index = customers.value.findIndex(c => c.id === id)
      if (index === -1) {
        throw new Error('Customer not found')
      }
      
      customers.value[index] = {
        ...customers.value[index],
        ...customerData,
        updatedAt: new Date().toISOString()
      }
      
      return customers.value[index]
    } catch (err) {
      error.value = 'Failed to update customer'
      throw err
    } finally {
      loading.value = false
    }
  }

  const deleteCustomer = async (id: number) => {
    loading.value = true
    error.value = null
    
    try {
      // Simulate API delay
      await new Promise(resolve => setTimeout(resolve, 300))
      
      const index = customers.value.findIndex(c => c.id === id)
      if (index === -1) {
        throw new Error('Customer not found')
      }
      
      customers.value.splice(index, 1)
    } catch (err) {
      error.value = 'Failed to delete customer'
      throw err
    } finally {
      loading.value = false
    }
  }

  return {
    // State
    customers: readonly(customers),
    loading: readonly(loading),
    error: readonly(error),
    
    // Getters
    filteredCustomers,
    customerStats,
    
    // Actions
    fetchCustomers,
    getCustomerById,
    createCustomer,
    updateCustomer,
    deleteCustomer
  }
})
