/**
 * Mock Dashboard Data Service
 * Simulates KPIs, metrics, and analytics for main dashboard
 */

export interface DashboardMetrics {
  totalRevenue: number
  totalOrders: number
  lowStockItems: number
  totalCustomers: number
  revenueGrowth: number
  ordersGrowth: number
}

export interface TopProduct {
  productId: number
  productName: string
  quantitySold: number
  revenue: number
}

export interface RecentActivity {
  id: number
  title: string
  description: string
  date: Date
  type: string
  icon?: string
  color?: string
}

export interface SalesTrendData {
  labels: string[]
  datasets: Array<{
    label: string
    data: number[]
    borderColor: string
    backgroundColor: string
    tension: number
  }>
}

export interface FinancialData {
  labels: string[]
  datasets: Array<{
    label: string
    data: number[]
    backgroundColor: string
  }>
}

const mockMetrics: DashboardMetrics = {
  totalRevenue: 125000000, // R1.25M in cents
  totalOrders: 1247,
  lowStockItems: 23,
  totalCustomers: 1428,
  revenueGrowth: 12.5,
  ordersGrowth: 8.2
}

const mockTopProducts: TopProduct[] = [
  { productId: 1, productName: 'Coca Cola 2L', quantitySold: 245, revenue: 857500 },
  { productId: 2, productName: 'White Bread 700g', quantitySold: 189, revenue: 340200 },
  { productId: 3, productName: 'Simba Chips 125g', quantitySold: 456, revenue: 547200 },
  { productId: 4, productName: 'Milk 1L', quantitySold: 167, revenue: 367400 },
  { productId: 5, productName: 'Sunlight Soap 250g', quantitySold: 134, revenue: 201000 }
]

const mockRecentActivities: RecentActivity[] = [
  {
    id: 1,
    title: 'New Customer Registration',
    description: 'TechCorp Ltd has registered as a new customer',
    date: new Date(),
    type: 'customer',
    color: 'bg-blue-500'
  },
  {
    id: 2,
    title: 'Order Completed',
    description: 'Order #1234 for R45,000 has been completed',
    date: new Date(Date.now() - 3600000),
    type: 'order',
    color: 'bg-green-500'
  },
  {
    id: 3,
    title: 'Quote Sent',
    description: 'Quote sent to BigCorp for enterprise package',
    date: new Date(Date.now() - 7200000),
    type: 'quote',
    color: 'bg-purple-500'
  },
  {
    id: 4,
    title: 'Low Stock Alert',
    description: 'White Bread 700g is running low (15 units)',
    date: new Date(Date.now() - 10800000),
    type: 'alert',
    color: 'bg-yellow-500'
  },
  {
    id: 5,
    title: 'Payment Received',
    description: 'Payment of R67,800 received from Retail Solutions',
    date: new Date(Date.now() - 86400000),
    type: 'payment',
    color: 'bg-green-500'
  }
]

export class MockDashboardService {
  static getMetrics(): DashboardMetrics {
    return mockMetrics
  }

  static getTopProducts(): TopProduct[] {
    return mockTopProducts
  }

  static getRecentActivities(): RecentActivity[] {
    return mockRecentActivities
  }

  static getSalesTrendData(): SalesTrendData {
    return {
      labels: ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'],
      datasets: [
        {
          label: 'Sales',
          data: [12000, 19000, 15000, 25000, 22000, 30000, 28000],
          borderColor: 'rgb(59, 130, 246)',
          backgroundColor: 'rgba(59, 130, 246, 0.1)',
          tension: 0.4
        }
      ]
    }
  }

  static getFinancialData(): FinancialData {
    return {
      labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun'],
      datasets: [
        {
          label: 'Revenue',
          data: [85000, 92000, 78000, 105000, 98000, 112000],
          backgroundColor: 'rgba(34, 197, 94, 0.8)'
        },
        {
          label: 'Expenses',
          data: [65000, 71000, 58000, 82000, 75000, 88000],
          backgroundColor: 'rgba(239, 68, 68, 0.8)'
        }
      ]
    }
  }

  static getSalesFunnel() {
    return [
      { name: 'Leads', count: 124, percentage: 100, color: 'bg-blue-500' },
      { name: 'Qualified', count: 87, percentage: 70, color: 'bg-green-500' },
      { name: 'Proposal', count: 45, percentage: 36, color: 'bg-yellow-500' },
      { name: 'Negotiation', count: 23, percentage: 19, color: 'bg-orange-500' },
      { name: 'Closed Won', count: 12, percentage: 10, color: 'bg-purple-500' }
    ]
  }

  static getQuickStats() {
    return {
      revenue: '1.2M',
      revenueGrowth: '+12.5%',
      customers: 1428,
      customersGrowth: '+8.2%',
      projects: 24,
      projectsDue: 3,
      pipeline: '892K',
      pipelineOpps: 15
    }
  }
}

