import { faker } from '@faker-js/faker'

faker.seed(42)

export interface DashboardKpi {
  label: string
  value: string
  delta: string
  icon: string
}

export const dashboardKpis: DashboardKpi[] = [
  { label: 'Today Sales', value: 'R 15,420', delta: '+12%', icon: 'shopping_cart' },
  { label: 'Cash In', value: 'R 12,300', delta: '+8%', icon: 'payments' },
  { label: 'Cash Out', value: 'R 4,500', delta: '-5%', icon: 'account_balance' },
  { label: 'Low Stock', value: '8 items', delta: 'Act now', icon: 'inventory_2' }
]

export const salesTrend = {
  labels: ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'],
  datasets: [
    {
      label: 'Sales',
      data: [12, 15, 13, 16, 18, 22, 15],
      borderColor: '#2563eb',
      backgroundColor: 'rgba(37, 99, 235, 0.2)',
      tension: 0.4,
      fill: true
    }
  ]
}

export const tasksTrend = {
  labels: ['Week 1', 'Week 2', 'Week 3', 'Week 4'],
  datasets: [
    {
      label: 'Completed Jobs',
      data: [30, 42, 35, 48],
      backgroundColor: '#22c55e'
    }
  ]
}

export const countrySales = [
  { country: 'South Africa', sales: 2500, value: 'R 230,900', bounce: '29.9%', flag: '/assets/za.png' },
  { country: 'Botswana', sales: 900, value: 'R 120,000', bounce: '24.2%', flag: '/assets/bw.png' },
  { country: 'Namibia', sales: 1200, value: 'R 150,700', bounce: '26.4%', flag: '/assets/na.png' }
]

export interface StockItem {
  id: string
  name: string
  sku: string
  qty: number
  uom: string
  minQty: number
  price: number
  location: string
}

export const stockItems: StockItem[] = Array.from({ length: 16 }).map(() => ({
  id: faker.string.uuid(),
  name: faker.commerce.productName(),
  sku: faker.string.alphanumeric(8).toUpperCase(),
  qty: faker.number.int({ min: 0, max: 120 }),
  uom: 'ea',
  minQty: faker.number.int({ min: 5, max: 30 }),
  price: Number(faker.commerce.price({ min: 25, max: 600 })),
  location: faker.helpers.arrayElement(['Store', 'Warehouse', 'Truck'])
}))

export interface OrderLike {
  id: string
  customer: string
  status: string
  total: number
  date: string
}

export const salesOrders: OrderLike[] = Array.from({ length: 8 }).map(() => ({
  id: faker.string.alphanumeric(6).toUpperCase(),
  customer: faker.person.fullName(),
  status: faker.helpers.arrayElement(['Draft', 'Submitted', 'Delivered']),
  total: Number(faker.commerce.price({ min: 500, max: 10000 })),
  date: faker.date.recent({ days: 10 }).toISOString()
}))

export const purchaseOrders: OrderLike[] = Array.from({ length: 6 }).map(() => ({
  id: faker.string.alphanumeric(6).toUpperCase(),
  customer: faker.company.name(),
  status: faker.helpers.arrayElement(['Draft', 'Submitted', 'Received']),
  total: Number(faker.commerce.price({ min: 800, max: 15000 })),
  date: faker.date.recent({ days: 15 }).toISOString()
}))

export const accountsSummary = {
  cashIn: 12300,
  cashOut: 4500,
  netCash: 7800,
  overdueInvoices: 2,
  pendingOrders: 3
}

export const customers = Array.from({ length: 12 }).map(() => ({
  id: faker.string.alphanumeric(6).toUpperCase(),
  name: faker.person.fullName(),
  phone: faker.phone.number(),
  balance: Number(faker.commerce.price({ min: 0, max: 8000 })),
  tier: faker.helpers.arrayElement(['VIP', 'Regular', 'On Account'])
}))

export const leads = Array.from({ length: 6 }).map(() => ({
  id: faker.string.alphanumeric(6).toUpperCase(),
  name: faker.person.fullName(),
  stage: faker.helpers.arrayElement(['New', 'Qualified', 'Converted']),
  source: faker.helpers.arrayElement(['Walk-in', 'Referral', 'Social'])
}))

export const projectTasks = Array.from({ length: 6 }).map(() => ({
  id: faker.string.alphanumeric(6).toUpperCase(),
  title: faker.commerce.productName(),
  assignee: faker.person.firstName(),
  status: faker.helpers.arrayElement(['Planned', 'In Progress', 'Done']),
  due: faker.date.soon({ days: 14 }).toISOString()
}))

