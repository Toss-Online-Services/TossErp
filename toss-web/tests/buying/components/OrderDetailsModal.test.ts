import { describe, it, expect, beforeEach, vi } from 'vitest'
import { mount } from '@vue/test-utils'
import OrderDetailsModal from '../../../components/buying/OrderDetailsModal.vue'

describe('OrderDetailsModal Component', () => {
  let wrapper: any

  const mockOrder = {
    id: 'PO1',
    number: 'PO-2025-001',
    supplier: 'Test Supplier',
    status: 'pending',
    orderDate: new Date('2025-01-01'),
    expectedDelivery: new Date('2025-01-10'),
    paymentTerms: 'Net 30',
    totalAmount: 5000,
    items: [
      {
        name: 'Product 1',
        sku: 'SKU001',
        quantity: 10,
        price: 100
      },
      {
        name: 'Product 2',
        sku: 'SKU002',
        quantity: 20,
        price: 150
      }
    ]
  }

  beforeEach(() => {
    wrapper = mount(OrderDetailsModal, {
      props: {
        show: true,
        order: mockOrder
      },
      global: {
        stubs: {
          OrderTimeline: true,
          DocumentTextIcon: true,
          XMarkIcon: true
        }
      }
    })
  })

  it('renders when show prop is true', () => {
    expect(wrapper.isVisible()).toBe(true)
    expect(wrapper.text()).toContain('Order Details')
  })

  it('does not render when show prop is false', async () => {
    await wrapper.setProps({ show: false })
    expect(wrapper.find('.fixed').exists()).toBe(false)
  })

  it('displays order number', () => {
    expect(wrapper.text()).toContain('PO-2025-001')
  })

  it('displays supplier name', () => {
    expect(wrapper.text()).toContain('Test Supplier')
  })

  it('displays order status', () => {
    expect(wrapper.text()).toContain('Awaiting Approval')
  })

  it('displays order date', () => {
    expect(wrapper.text()).toContain('Jan')
    expect(wrapper.text()).toContain('2025')
  })

  it('displays expected delivery date', () => {
    expect(wrapper.text()).toContain('Jan')
  })

  it('displays payment terms', () => {
    expect(wrapper.text()).toContain('Net 30')
  })

  it('displays order items table', () => {
    expect(wrapper.text()).toContain('Order Items')
    expect(wrapper.text()).toContain('Product 1')
    expect(wrapper.text()).toContain('Product 2')
    expect(wrapper.text()).toContain('SKU001')
    expect(wrapper.text()).toContain('SKU002')
  })

  it('displays item quantities and prices', () => {
    expect(wrapper.text()).toContain('10')
    expect(wrapper.text()).toContain('20')
    expect(wrapper.text()).toContain('100')
    expect(wrapper.text()).toContain('150')
  })

  it('displays total amount', () => {
    expect(wrapper.text()).toContain('5000')
  })

  it('emits close event when close button clicked', async () => {
    const closeButton = wrapper.find('button')
    await closeButton.trigger('click')
    
    expect(wrapper.emitted('close')).toBeTruthy()
  })

  it('emits close event when backdrop clicked', async () => {
    const backdrop = wrapper.find('.bg-black\\/50')
    await backdrop.trigger('click')
    
    expect(wrapper.emitted('close')).toBeTruthy()
  })

  it('displays print button', () => {
    const printButton = wrapper.findAll('button').find((btn: any) => 
      btn.text().includes('Print')
    )
    expect(printButton).toBeDefined()
  })

  it('emits print event when print button clicked', async () => {
    const printButton = wrapper.findAll('button').find((btn: any) => 
      btn.text().includes('Print')
    )
    await printButton?.trigger('click')
    
    expect(wrapper.emitted('print')).toBeTruthy()
  })

  it('displays track button', () => {
    const trackButton = wrapper.findAll('button').find((btn: any) => 
      btn.text().includes('Track')
    )
    expect(trackButton).toBeDefined()
  })

  it('emits track event when track button clicked', async () => {
    const trackButton = wrapper.findAll('button').find((btn: any) => 
      btn.text().includes('Track')
    )
    await trackButton?.trigger('click')
    
    expect(wrapper.emitted('track')).toBeTruthy()
  })

  it('displays order items table', () => {
    expect(wrapper.find('table').exists()).toBe(true)
    expect(wrapper.find('thead').exists()).toBe(true)
    expect(wrapper.find('tbody').exists()).toBe(true)
  })

  it('handles missing order data gracefully', async () => {
    await wrapper.setProps({ order: null })
    
    // Should not throw errors
    expect(wrapper.isVisible()).toBe(true)
  })

  it('applies correct status styles', async () => {
    // Test different statuses
    const statuses = ['pending', 'approved', 'in-transit', 'delivered']
    
    for (const status of statuses) {
      await wrapper.setProps({
        order: { ...mockOrder, status }
      })
      await wrapper.vm.$nextTick()
      
      const statusBadge = wrapper.find('.px-3.py-1.rounded-full')
      expect(statusBadge.exists()).toBe(true)
    }
  })

  it('calculates item totals correctly', () => {
    // Product 1: 10 x 100 = 1000
    expect(wrapper.text()).toContain('1000')
    // Product 2: 20 x 150 = 3000
    expect(wrapper.text()).toContain('3000')
  })
})

