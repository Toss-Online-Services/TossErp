import { describe, it, expect, beforeEach, vi } from 'vitest'
import { mount } from '@vue/test-utils'
import { createRouter, createMemoryHistory } from 'vue-router'
import BuyingDashboard from '../../../pages/buying/index.vue'

// Mock composables
vi.mock('~/composables/useBuyingAPI', () => ({
  useBuyingAPI: () => ({
    getStatistics: vi.fn().mockResolvedValue({
      totalOrders: 24,
      pendingOrders: 18,
      activeSuppliers: 42
    })
  })
}))

describe('Buying Dashboard', () => {
  let wrapper: any
  let router: any

  beforeEach(async () => {
    router = createRouter({
      history: createMemoryHistory(),
      routes: [
        { path: '/buying', component: BuyingDashboard },
        { path: '/buying/create-order', component: { template: '<div>Create Order</div>' } },
        { path: '/buying/orders', component: { template: '<div>Orders</div>' } },
        { path: '/buying/suppliers', component: { template: '<div>Suppliers</div>' } },
        { path: '/buying/group-buying', component: { template: '<div>Group Buying</div>' } }
      ]
    })

    await router.push('/buying')
    await router.isReady()

    wrapper = mount(BuyingDashboard, {
      global: {
        plugins: [router],
        stubs: {
          NuxtLink: false,
          LineChart: true,
          BarChart: true,
          UserGroupIcon: true,
          ShoppingCartIcon: true,
          CurrencyDollarIcon: true,
          ClockIcon: true,
          TruckIcon: true
        }
      }
    })
  })

  it('renders the dashboard correctly', () => {
    expect(wrapper.find('h1').text()).toContain('Buying')
    expect(wrapper.text()).toContain('Manage orders, suppliers, and group buying')
  })

  it('displays key metrics', async () => {
    await wrapper.vm.$nextTick()
    
    const metrics = wrapper.findAll('.text-3xl')
    expect(metrics.length).toBeGreaterThan(0)
  })

  it('has a create order button', () => {
    const createButton = wrapper.find('[href="/buying/create-order"]')
    expect(createButton.exists()).toBe(true)
    expect(createButton.text()).toContain('Create Order')
  })

  it('displays spending trends chart', () => {
    expect(wrapper.text()).toContain('Spending Trends')
  })

  it('displays order status distribution', () => {
    expect(wrapper.text()).toContain('Order Status')
  })

  it('displays quick action cards', () => {
    expect(wrapper.text()).toContain('Buy Orders')
    expect(wrapper.text()).toContain('Suppliers')
    expect(wrapper.text()).toContain('Group Buying')
  })

  it('has navigation links to key pages', async () => {
    const ordersLink = wrapper.find('[href="/buying/orders"]')
    const suppliersLink = wrapper.find('[href="/buying/suppliers"]')
    const groupBuyingLink = wrapper.find('[href="/buying/group-buying"]')

    expect(ordersLink.exists()).toBe(true)
    expect(suppliersLink.exists()).toBe(true)
    expect(groupBuyingLink.exists()).toBe(true)
  })

  it('displays category spending breakdown', () => {
    expect(wrapper.text()).toContain('Spending by Category')
    expect(wrapper.text()).toContain('Food & Beverages')
  })

  it('displays top suppliers chart', () => {
    expect(wrapper.text()).toContain('Top Suppliers by Spend')
  })
})

