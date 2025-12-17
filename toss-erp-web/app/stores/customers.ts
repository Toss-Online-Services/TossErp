import { defineStore } from 'pinia'

export const useCustomerStore = defineStore('customers', () => {
  const {
    getCustomers,
    getCustomerProfile,
    createCustomer: createCustomerAPI,
    searchCustomers: searchCustomersAPI
  } = useCustomers()

  // State
  const customers = ref<any[]>([])
  const currentCustomer = ref<any | null>(null)
  const loading = ref(false)
  const error = ref<string | null>(null)

  // Computed
  const filteredCustomers = computed(() => customers.value)
  
  const customerStats = computed(() => {
    const total = customers.value.length
    const active = customers.value.filter((c: any) => c.status === 'Active').length
    const leads = customers.value.filter((c: any) => c.status === 'Lead').length
    const inactive = customers.value.filter((c: any) => c.status === 'Inactive').length
    const totalValue = customers.value.reduce((sum: number, c: any) => sum + (c.lifetimeValue || 0), 0)

    return { total, active, leads, inactive, totalValue }
  })

  // Actions
  const fetchCustomers = async (params?: any) => {
    loading.value = true
    error.value = null
    try {
      const response = await getCustomers(params)
      customers.value = response.items || response || []
      return response
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch customers'
      throw err
    } finally {
      loading.value = false
    }
  }

  const fetchCustomerById = async (id: number) => {
    loading.value = true
    error.value = null
    try {
      const customer = await getCustomerProfile(id)
      currentCustomer.value = customer
      return customer
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch customer'
      throw err
    } finally {
      loading.value = false
    }
  }

  const getCustomerById = (id: number) => {
    return customers.value.find((c: any) => c.id === id) || currentCustomer.value
  }

  const createCustomer = async (customerData: any) => {
    loading.value = true
    error.value = null
    try {
      const result = await createCustomerAPI(customerData)
      await fetchCustomers({ shopId: customerData.shopId })
      return result
    } catch (err: any) {
      error.value = err.message || 'Failed to create customer'
      throw err
    } finally {
      loading.value = false
    }
  }

  const searchCustomers = async (searchTerm: string, shopId?: number) => {
    loading.value = true
    error.value = null
    try {
      const results = await searchCustomersAPI(searchTerm, shopId)
      return results
    } catch (err: any) {
      error.value = err.message || 'Failed to search customers'
      throw err
    } finally {
      loading.value = false
    }
  }

  const clearError = () => {
    error.value = null
  }

  return {
    // State
    customers,
    currentCustomer,
    loading,
    error,
    
    // Computed
    filteredCustomers,
    customerStats,
    
    // Actions
    fetchCustomers,
    fetchCustomerById,
    getCustomerById,
    createCustomer,
    searchCustomers,
    clearError
  }
})
