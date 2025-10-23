import { describe, it, expect, beforeEach } from 'vitest'
import { mount } from '@vue/test-utils'
import OrderTimeline from '../../../components/buying/OrderTimeline.vue'

describe('OrderTimeline Component', () => {
  let wrapper: any

  const baseProps = {
    status: 'pending',
    orderNumber: 'PO-2025-001',
    orderDate: new Date('2025-01-01'),
    expectedDelivery: new Date('2025-01-10')
  }

  beforeEach(() => {
    wrapper = mount(OrderTimeline, {
      props: baseProps,
      global: {
        stubs: {
          CheckCircleIcon: true,
          ClockIcon: true,
          TruckIcon: true,
          CubeIcon: true
        }
      }
    })
  })

  it('renders timeline correctly', () => {
    expect(wrapper.find('.relative').exists()).toBe(true)
  })

  it('displays order number', () => {
    expect(wrapper.text()).toContain('PO-2025-001')
  })

  it('displays order placed status', () => {
    expect(wrapper.text()).toContain('Order Placed')
  })

  it('displays order confirmed status when status is approved', async () => {
    await wrapper.setProps({ status: 'approved' })
    expect(wrapper.text()).toContain('Order Confirmed')
  })

  it('displays out for delivery status when status is in-transit', async () => {
    await wrapper.setProps({ status: 'in-transit' })
    expect(wrapper.text()).toContain('Out for Delivery')
  })

  it('displays delivered status when status is delivered', async () => {
    await wrapper.setProps({ status: 'delivered' })
    expect(wrapper.text()).toContain('Delivered')
  })

  it('shows active status with colored icon', () => {
    const activeIcon = wrapper.find('.bg-green-500')
    expect(activeIcon.exists()).toBe(true)
  })

  it('shows inactive statuses with gray icon', () => {
    const inactiveIcons = wrapper.findAll('.bg-slate-300')
    expect(inactiveIcons.length).toBeGreaterThan(0)
  })

  it('displays timestamp for completed stages', async () => {
    await wrapper.setProps({ status: 'approved' })
    
    // Should display dates for pending and approved stages
    expect(wrapper.text()).toContain('Jan')
  })

  it('displays expected delivery date when in-transit', async () => {
    await wrapper.setProps({ status: 'in-transit' })
    expect(wrapper.text()).toContain('Jan')
  })

  it('shows all timeline stages', () => {
    // Should show all 5 stages
    expect(wrapper.text()).toContain('Order Placed')
    expect(wrapper.text()).toContain('Order Confirmed')
    expect(wrapper.text()).toContain('Preparing for Shipment')
    expect(wrapper.text()).toContain('Out for Delivery')
    expect(wrapper.text()).toContain('Delivered')
  })

  it('updates visual state based on status progression', async () => {
    // Test progression through statuses
    const statuses = ['pending', 'approved', 'in-transit', 'delivered']
    
    for (const status of statuses) {
      await wrapper.setProps({ status })
      await wrapper.vm.$nextTick()
      
      // Each status should have appropriate visual indicators
      const activeElements = wrapper.findAll('.bg-blue-500, .bg-green-500, .bg-orange-500, .bg-purple-500')
      expect(activeElements.length).toBeGreaterThan(0)
    }
  })

  it('shows waiting message for future stages', () => {
    // When status is pending, other stages should show waiting messages
    expect(wrapper.text()).toMatch(/Not .* yet|Awaiting|Pending/)
  })

  it('displays vertical connecting line', () => {
    // Timeline should have connecting lines between stages
    const connectingLine = wrapper.find('.absolute')
    expect(connectingLine.exists()).toBe(true)
  })

  it('formats dates correctly', () => {
    // Dates should be formatted in a readable way
    const dateText = wrapper.text()
    expect(dateText).toMatch(/Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec/)
  })

  it('handles missing expected delivery date gracefully', async () => {
    await wrapper.setProps({ expectedDelivery: null })
    
    // Should not throw errors
    expect(wrapper.isVisible()).toBe(true)
  })

  it('displays stage descriptions', () => {
    // Each stage should have descriptive text
    expect(wrapper.text()).toMatch(/placed|confirmed|shipped|delivered|awaiting/i)
  })
})

