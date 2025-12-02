import { describe, it, expect, beforeEach, vi } from 'vitest'
import { mount, flushPromises } from '@vue/test-utils'
import { createRouter, createMemoryHistory } from 'vue-router'
import BuyingOrders from '../../../pages/buying/orders/index.vue'

// Mock composables
const mockOrders = [
  {
    id: 'PO1',
    orderNumber: 'PO-2025-001',
    customer: 'Test Supplier',
    total: 5000,
    status: 'pending',
    orderDate: new Date('2025-01-01'),
    expectedDelivery: new Date('2025-01-10'),
    orderItems: []
  },
  {
    id: 'PO2',
    orderNumber: 'PO-2025-002',
    customer: 'Another Supplier',
    total: 3000,
    status: 'approved',
    orderDate: new Date('2025-01-02'),
    expectedDelivery: new Date('2025-01-12'),
    orderItems: []
  }
]

vi.mock('~/composables/useBuyingAPI', () => ({
  useBuyingAPI: () => ({
    getOrders: vi.fn().mockResolvedValue(mockOrders),
    approveOrder: vi.fn().mockResolvedValue(true),
    cancelOrder: vi.fn().mockResolvedValue(true)
  })
}))

vi.mock('~/composables/useToast', () => ({
  useToast: () => ({
    success: vi.fn(),
    error: vi.fn(),
    warning: vi.fn()
  })
}))

describe('Buying Orders Page', () => {
  let wrapper: any
  let router: any

  beforeEach(async () => {
    router = createRouter({
      history: createMemoryHistory(),
      routes: [
        { path: '/buying/orders', component: BuyingOrders },
        { path: '/buying/orders/create-order', component: { template: '<div>Create Order</div>' } }
      ]
    })

    await router.push('/buying/orders')
    await router.isReady()

    wrapper = mount(BuyingOrders, {
      global: {
        plugins: [router],
        stubs: {
          NuxtLink: false,
          OrderDetailsModal: true,
          PlusIcon: true,
          MagnifyingGlassIcon: true,
          ShoppingCartIcon: true,
          ClockIcon: true,
          TruckIcon: true,
          CheckCircleIcon: true,
          CurrencyDollarIcon: true,
          EyeIcon: true,
          PrinterIcon: true,
          XMarkIcon: true,
          ArrowDownTrayIcon: true
        }
      }
    })

    await flushPromises()
  })

  it('renders the page correctly', () => {
    expect(wrapper.find('h1').text()).toContain('Buy Orders')
    expect(wrapper.text()).toContain('Manage and track your Buy Orders')
  })

  it('displays order statistics', async () => {
    await wrapper.vm.$nextTick()
    
    expect(wrapper.text()).toContain('Total Orders')
    expect(wrapper.text()).toContain('Pending')
    expect(wrapper.text()).toContain('In Transit')
    expect(wrapper.text()).toContain('Delivered')
  })

  it('displays orders list', async () => {
    await wrapper.vm.$nextTick()
    await flushPromises()
    
    expect(wrapper.text()).toContain('PO-2025-001')
    expect(wrapper.text()).toContain('Test Supplier')
  })

  it('has search functionality', async () => {
    await wrapper.vm.$nextTick()
    
    const searchInput = wrapper.find('input[type="text"]')
    expect(searchInput.exists()).toBe(true)
    
    await searchInput.setValue('Test Supplier')
    await wrapper.vm.$nextTick()
    
    expect(wrapper.vm.searchQuery).toBe('Test Supplier')
  })

  it('has status filter', async () => {
    await wrapper.vm.$nextTick()
    
    const statusFilter = wrapper.find('select')
    expect(statusFilter.exists()).toBe(true)
    
    const options = statusFilter.findAll('option')
    expect(options.length).toBeGreaterThan(1)
  })

  it('has create order button', () => {
    const createButton = wrapper.find('[href="/buying/orders/create-order"]')
    expect(createButton.exists()).toBe(true)
  })

  it('displays empty state when no orders', async () => {
    // Remount with empty orders
    vi.clearAllMocks()
    vi.mock('~/composables/useBuyingAPI', () => ({
      useBuyingAPI: () => ({
        getOrders: vi.fn().mockResolvedValue([])
      })
    }))

    const emptyWrapper = mount(BuyingOrders, {
      global: {
        plugins: [router],
        stubs: {
          NuxtLink: false,
          OrderDetailsModal: true
        }
      }
    })

    await flushPromises()
    await emptyWrapper.vm.$nextTick()

    expect(emptyWrapper.text()).toContain('No orders found')
  })

  it('displays order action buttons', async () => {
    await wrapper.vm.$nextTick()
    await flushPromises()
    
    // Check for action buttons in order cards
    expect(wrapper.html()).toContain('View')
    expect(wrapper.html()).toContain('Print')
    expect(wrapper.html()).toContain('Track')
  })

  it('shows approve button for pending orders', async () => {
    await wrapper.vm.$nextTick()
    await flushPromises()
    
    expect(wrapper.html()).toContain('Approve')
  })

  it('shows cancel button for pending/approved orders', async () => {
    await wrapper.vm.$nextTick()
    await flushPromises()
    
    expect(wrapper.html()).toContain('Cancel')
  })

  it('filters orders by search query', async () => {
    await wrapper.vm.$nextTick()
    
    wrapper.vm.searchQuery = 'Test Supplier'
    await wrapper.vm.$nextTick()
    
    const filteredOrders = wrapper.vm.filteredOrders
    expect(filteredOrders.length).toBe(1)
    expect(filteredOrders[0].customer).toBe('Test Supplier')
  })

  it('filters orders by status', async () => {
    await wrapper.vm.$nextTick()
    
    wrapper.vm.statusFilter = 'pending'
    await wrapper.vm.$nextTick()
    
    const filteredOrders = wrapper.vm.filteredOrders
    expect(filteredOrders.length).toBe(1)
    expect(filteredOrders[0].status).toBe('pending')
  })
})

