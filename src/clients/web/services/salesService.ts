// Sales & POS API Service
import { apiGet, apiPost, apiPut, apiDelete, API_ENDPOINTS, USE_MOCK_DATA, mockDelay } from '~/utils/api'

// Types
export interface Sale {
  id: string
  orderNumber: string
  customerId?: string
  customer?: Customer
  items: SaleItem[]
  subtotal: number
  tax: number
  total: number
  paymentMethod: string
  status: 'pending' | 'completed' | 'cancelled' | 'refunded'
  createdAt: string
  updatedAt: string
  notes?: string
}

export interface SaleItem {
  id: string
  stockItemId: string
  stockItemName: string
  quantity: number
  unitPrice: number
  total: number
}

export interface Customer {
  id: string
  name: string
  email?: string
  phone?: string
  address?: string
  totalPurchases: number
  lastPurchase?: string
  createdAt: string
}

export interface SalesOverview {
  todaysSales: number
  todaysOrders: number
  averageOrderValue: number
  totalCustomers: number
  topProducts: TopProduct[]
  recentSales: Sale[]
}

export interface TopProduct {
  id: string
  name: string
  sold: number
  revenue: number
}

export interface CreateSaleRequest {
  customerId?: string
  items: {
    stockItemId: string
    quantity: number
    unitPrice: number
  }[]
  paymentMethod: string
  notes?: string
}

export interface CreateCustomerRequest {
  name: string
  email?: string
  phone?: string
  address?: string
}

// Mock data
const mockCustomers: Customer[] = [
  {
    id: '1',
    name: 'John Doe',
    email: 'john.doe@email.com',
    phone: '+27123456789',
    address: '123 Main St, Johannesburg',
    totalPurchases: 5,
    lastPurchase: '2024-01-16T10:30:00Z',
    createdAt: '2024-01-01T08:00:00Z'
  },
  {
    id: '2',
    name: 'Jane Smith',
    email: 'jane.smith@email.com',
    phone: '+27987654321',
    address: '456 Oak Ave, Cape Town',
    totalPurchases: 12,
    lastPurchase: '2024-01-15T14:20:00Z',
    createdAt: '2023-12-15T10:15:00Z'
  }
]

const mockSales: Sale[] = [
  {
    id: '1',
    orderNumber: '2024-0001',
    customerId: '1',
    customer: mockCustomers[0],
    items: [
      {
        id: '1',
        stockItemId: '1',
        stockItemName: 'Samsung Galaxy S21',
        quantity: 1,
        unitPrice: 12999.00,
        total: 12999.00
      }
    ],
    subtotal: 12999.00,
    tax: 1949.85,
    total: 14948.85,
    paymentMethod: 'card',
    status: 'completed',
    createdAt: '2024-01-16T10:30:00Z',
    updatedAt: '2024-01-16T10:35:00Z'
  },
  {
    id: '2',
    orderNumber: '2024-0002',
    items: [
      {
        id: '2',
        stockItemId: '2',
        stockItemName: 'Nike Air Force 1',
        quantity: 2,
        unitPrice: 1899.00,
        total: 3798.00
      }
    ],
    subtotal: 3798.00,
    tax: 569.70,
    total: 4367.70,
    paymentMethod: 'cash',
    status: 'completed',
    createdAt: '2024-01-16T11:15:00Z',
    updatedAt: '2024-01-16T11:20:00Z'
  }
]

const mockTopProducts: TopProduct[] = [
  { id: '1', name: 'Samsung Galaxy S21', sold: 12, revenue: 155988.00 },
  { id: '2', name: 'Nike Air Force 1', sold: 25, revenue: 47475.00 },
  { id: '3', name: 'iPhone 13 Pro', sold: 8, revenue: 175992.00 },
  { id: '4', name: 'Coca Cola 500ml', sold: 156, revenue: 2340.00 }
]

const mockOverview: SalesOverview = {
  todaysSales: 19316.55,
  todaysOrders: 15,
  averageOrderValue: 1287.77,
  totalCustomers: 156,
  topProducts: mockTopProducts,
  recentSales: mockSales
}

// Sales Service Class
export class SalesService {
  /**
   * Get sales overview/summary
   */
  static async getOverview(period?: 'today' | 'week' | 'month'): Promise<SalesOverview> {
    if (USE_MOCK_DATA) {
      await mockDelay()
      return mockOverview
    }
    
    return await apiGet<SalesOverview>(API_ENDPOINTS.sales, {
      query: { period }
    })
  }
  
  /**
   * Get all sales with optional filtering
   */
  static async getSales(params?: {
    customerId?: string
    status?: string
    startDate?: string
    endDate?: string
    page?: number
    limit?: number
  }): Promise<Sale[]> {
    if (USE_MOCK_DATA) {
      await mockDelay()
      let sales = [...mockSales]
      
      if (params?.customerId) {
        sales = sales.filter(sale => sale.customerId === params.customerId)
      }
      
      if (params?.status) {
        sales = sales.filter(sale => sale.status === params.status)
      }
      
      return sales
    }
    
    return await apiGet<Sale[]>(API_ENDPOINTS.orders, {
      query: params
    })
  }
  
  /**
   * Get a single sale by ID
   */
  static async getSale(id: string): Promise<Sale> {
    if (USE_MOCK_DATA) {
      await mockDelay()
      const sale = mockSales.find(sale => sale.id === id)
      if (!sale) {
        throw new Error(`Sale with ID ${id} not found`)
      }
      return sale
    }
    
    return await apiGet<Sale>(`${API_ENDPOINTS.orders}/${id}`)
  }
  
  /**
   * Create a new sale
   */
  static async createSale(data: CreateSaleRequest): Promise<Sale> {
    if (USE_MOCK_DATA) {
      await mockDelay()
      
      const saleItems: SaleItem[] = data.items.map((item, index) => ({
        id: (index + 1).toString(),
        stockItemId: item.stockItemId,
        stockItemName: `Product ${item.stockItemId}`, // In real app, fetch from stock
        quantity: item.quantity,
        unitPrice: item.unitPrice,
        total: item.quantity * item.unitPrice
      }))
      
      const subtotal = saleItems.reduce((sum, item) => sum + item.total, 0)
      const tax = subtotal * 0.15 // 15% VAT
      const total = subtotal + tax
      
      const newSale: Sale = {
        id: Date.now().toString(),
        orderNumber: `2024-${(mockSales.length + 1).toString().padStart(4, '0')}`,
        customerId: data.customerId,
        customer: data.customerId ? mockCustomers.find(c => c.id === data.customerId) : undefined,
        items: saleItems,
        subtotal,
        tax,
        total,
        paymentMethod: data.paymentMethod,
        status: 'completed',
        notes: data.notes,
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString()
      }
      
      mockSales.unshift(newSale)
      return newSale
    }
    
    return await apiPost<Sale>(API_ENDPOINTS.orders, data)
  }
  
  /**
   * Update sale status
   */
  static async updateSaleStatus(id: string, status: Sale['status']): Promise<Sale> {
    if (USE_MOCK_DATA) {
      await mockDelay()
      const index = mockSales.findIndex(sale => sale.id === id)
      if (index === -1) {
        throw new Error(`Sale with ID ${id} not found`)
      }
      
      mockSales[index] = {
        ...mockSales[index],
        status,
        updatedAt: new Date().toISOString()
      }
      
      return mockSales[index]
    }
    
    return await apiPut<Sale>(`${API_ENDPOINTS.orders}/${id}/status`, { status })
  }
  
  /**
   * Cancel a sale
   */
  static async cancelSale(id: string, reason?: string): Promise<Sale> {
    if (USE_MOCK_DATA) {
      await mockDelay()
      return await this.updateSaleStatus(id, 'cancelled')
    }
    
    return await apiPost<Sale>(`${API_ENDPOINTS.orders}/${id}/cancel`, { reason })
  }
  
  /**
   * Process a refund
   */
  static async refundSale(id: string, amount?: number, reason?: string): Promise<Sale> {
    if (USE_MOCK_DATA) {
      await mockDelay()
      return await this.updateSaleStatus(id, 'refunded')
    }
    
    return await apiPost<Sale>(`${API_ENDPOINTS.orders}/${id}/refund`, { amount, reason })
  }
  
  /**
   * Get all customers
   */
  static async getCustomers(params?: {
    search?: string
    page?: number
    limit?: number
  }): Promise<Customer[]> {
    if (USE_MOCK_DATA) {
      await mockDelay()
      let customers = [...mockCustomers]
      
      if (params?.search) {
        const search = params.search.toLowerCase()
        customers = customers.filter(customer =>
          customer.name.toLowerCase().includes(search) ||
          customer.email?.toLowerCase().includes(search) ||
          customer.phone?.includes(search)
        )
      }
      
      return customers
    }
    
    return await apiGet<Customer[]>(API_ENDPOINTS.customers, {
      query: params
    })
  }
  
  /**
   * Get a single customer by ID
   */
  static async getCustomer(id: string): Promise<Customer> {
    if (USE_MOCK_DATA) {
      await mockDelay()
      const customer = mockCustomers.find(customer => customer.id === id)
      if (!customer) {
        throw new Error(`Customer with ID ${id} not found`)
      }
      return customer
    }
    
    return await apiGet<Customer>(`${API_ENDPOINTS.customers}/${id}`)
  }
  
  /**
   * Create a new customer
   */
  static async createCustomer(data: CreateCustomerRequest): Promise<Customer> {
    if (USE_MOCK_DATA) {
      await mockDelay()
      const newCustomer: Customer = {
        id: Date.now().toString(),
        ...data,
        totalPurchases: 0,
        createdAt: new Date().toISOString()
      }
      mockCustomers.push(newCustomer)
      return newCustomer
    }
    
    return await apiPost<Customer>(API_ENDPOINTS.customers, data)
  }
  
  /**
   * Update customer information
   */
  static async updateCustomer(id: string, data: Partial<CreateCustomerRequest>): Promise<Customer> {
    if (USE_MOCK_DATA) {
      await mockDelay()
      const index = mockCustomers.findIndex(customer => customer.id === id)
      if (index === -1) {
        throw new Error(`Customer with ID ${id} not found`)
      }
      
      mockCustomers[index] = {
        ...mockCustomers[index],
        ...data
      }
      
      return mockCustomers[index]
    }
    
    return await apiPut<Customer>(`${API_ENDPOINTS.customers}/${id}`, data)
  }
  
  /**
   * Delete a customer
   */
  static async deleteCustomer(id: string): Promise<void> {
    if (USE_MOCK_DATA) {
      await mockDelay()
      const index = mockCustomers.findIndex(customer => customer.id === id)
      if (index === -1) {
        throw new Error(`Customer with ID ${id} not found`)
      }
      mockCustomers.splice(index, 1)
      return
    }
    
    return await apiDelete(`${API_ENDPOINTS.customers}/${id}`)
  }
  
  /**
   * Get top selling products
   */
  static async getTopProducts(params?: {
    period?: 'today' | 'week' | 'month'
    limit?: number
  }): Promise<TopProduct[]> {
    if (USE_MOCK_DATA) {
      await mockDelay()
      return mockTopProducts.slice(0, params?.limit || 10)
    }
    
    return await apiGet<TopProduct[]>(`${API_ENDPOINTS.sales}/top-products`, {
      query: params
    })
  }
  
  /**
   * Generate sales report
   */
  static async generateReport(params: {
    startDate: string
    endDate: string
    format?: 'json' | 'csv' | 'pdf'
  }): Promise<any> {
    if (USE_MOCK_DATA) {
      await mockDelay()
      return {
        totalSales: 45750.50,
        totalOrders: 156,
        averageOrderValue: 293.27,
        reportUrl: '#'
      }
    }
    
    return await apiPost(`${API_ENDPOINTS.sales}/reports`, params)
  }
}

// Export default service
export default SalesService

