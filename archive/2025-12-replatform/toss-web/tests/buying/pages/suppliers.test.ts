import { describe, it, expect, beforeEach, vi } from 'vitest'
import { mount, flushPromises } from '@vue/test-utils'
import { createRouter, createMemoryHistory } from 'vue-router'
import Suppliers from '../../../pages/buying/suppliers.vue'

// Mock composables
const mockSuppliers = [
  {
    id: 'S1',
    name: 'Test Supplier',
    category: 'Beverages',
    contact: 'John Doe',
    phone: '+27 11 123 4567',
    email: 'john@testsupplier.com',
    address: '123 Test Street',
    rating: 4.5
  },
  {
    id: 'S2',
    name: 'Another Supplier',
    category: 'Groceries',
    contact: 'Jane Smith',
    phone: '+27 11 987 6543',
    email: 'jane@anothersupplier.com',
    address: '456 Another Street',
    rating: 4.8
  }
]

vi.mock('~/composables/useBuyingAPI', () => ({
  useBuyingAPI: () => ({
    getSuppliers: vi.fn().mockResolvedValue(mockSuppliers),
    addSupplier: vi.fn().mockResolvedValue({ id: 'S3', name: 'New Supplier' })
  })
}))

describe('Suppliers Page', () => {
  let wrapper: any
  let router: any

  beforeEach(async () => {
    router = createRouter({
      history: createMemoryHistory(),
      routes: [
        { path: '/buying/suppliers', component: Suppliers },
        { path: '/buying/orders/create-order', component: { template: '<div>Create Order</div>' } }
      ]
    })

    await router.push('/buying/suppliers')
    await router.isReady()

    wrapper = mount(Suppliers, {
      global: {
        plugins: [router],
        stubs: {
          NuxtLink: false,
          PlusIcon: true,
          MagnifyingGlassIcon: true,
          StarIcon: true,
          UserIcon: true,
          PhoneIcon: true,
          EnvelopeIcon: true,
          MapPinIcon: true,
          BuildingStorefrontIcon: true,
          XMarkIcon: true
        }
      }
    })

    await flushPromises()
  })

  it('renders the page correctly', () => {
    expect(wrapper.find('h1').text()).toContain('Suppliers')
    expect(wrapper.text()).toContain('Manage your vendor relationships')
  })

  it('displays suppliers grid', async () => {
    await wrapper.vm.$nextTick()
    
    expect(wrapper.text()).toContain('Test Supplier')
    expect(wrapper.text()).toContain('Another Supplier')
  })

  it('has add supplier button', () => {
    const addButton = wrapper.findAll('button').find((btn: any) => 
      btn.text().includes('Add Supplier')
    )
    expect(addButton).toBeDefined()
  })

  it('has search functionality', async () => {
    const searchInput = wrapper.find('input[type="text"]')
    expect(searchInput.exists()).toBe(true)
    
    await searchInput.setValue('Test Supplier')
    await wrapper.vm.$nextTick()
    
    expect(wrapper.vm.searchQuery).toBe('Test Supplier')
  })

  it('has category filter', async () => {
    const categoryFilter = wrapper.find('select')
    expect(categoryFilter.exists()).toBe(true)
    
    const options = categoryFilter.findAll('option')
    expect(options.length).toBeGreaterThan(1)
    expect(options.some((opt: any) => opt.text() === 'Beverages')).toBe(true)
  })

  it('displays supplier contact information', async () => {
    await wrapper.vm.$nextTick()
    
    expect(wrapper.text()).toContain('John Doe')
    expect(wrapper.text()).toContain('+27 11 123 4567')
    expect(wrapper.text()).toContain('john@testsupplier.com')
  })

  it('displays supplier rating', async () => {
    await wrapper.vm.$nextTick()
    
    expect(wrapper.text()).toContain('4.5')
    expect(wrapper.text()).toContain('4.8')
  })

  it('has action buttons for each supplier', async () => {
    await wrapper.vm.$nextTick()
    
    const buttons = wrapper.findAll('button')
    const viewButtons = buttons.filter((btn: any) => btn.text() === 'View Details')
    const orderButtons = buttons.filter((btn: any) => btn.text() === 'Order')
    
    expect(viewButtons.length).toBeGreaterThan(0)
    expect(orderButtons.length).toBeGreaterThan(0)
  })

  it('opens add supplier modal', async () => {
    const addButton = wrapper.findAll('button').find((btn: any) => 
      btn.text().includes('Add Supplier')
    )
    
    await addButton?.trigger('click')
    await wrapper.vm.$nextTick()
    
    expect(wrapper.vm.showAddSupplierModal).toBe(true)
  })

  it('filters suppliers by search query', async () => {
    await wrapper.vm.$nextTick()
    
    wrapper.vm.searchQuery = 'Test Supplier'
    await wrapper.vm.$nextTick()
    
    const filteredSuppliers = wrapper.vm.filteredSuppliers
    expect(filteredSuppliers.length).toBe(1)
    expect(filteredSuppliers[0].name).toBe('Test Supplier')
  })

  it('filters suppliers by category', async () => {
    await wrapper.vm.$nextTick()
    
    wrapper.vm.categoryFilter = 'Beverages'
    await wrapper.vm.$nextTick()
    
    const filteredSuppliers = wrapper.vm.filteredSuppliers
    expect(filteredSuppliers.length).toBe(1)
    expect(filteredSuppliers[0].category).toBe('Beverages')
  })

  it('displays empty state when no suppliers', async () => {
    // Set suppliers to empty array
    wrapper.vm.suppliers = []
    await wrapper.vm.$nextTick()
    
    expect(wrapper.text()).toContain('No suppliers found')
  })
})

