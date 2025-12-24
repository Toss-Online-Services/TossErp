import { describe, it, expect, beforeEach } from 'vitest'
import { mount } from '@vue/test-utils'
import { createRouter, createWebHistory } from 'vue-router'
import MaterialSidebar from '~/components/material/MaterialSidebar.vue'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: '/', component: { template: '<div>Home</div>' } },
    { path: '/retailer/dashboard', component: { template: '<div>Dashboard</div>' } }
  ]
})

describe('MaterialSidebar', () => {
  const defaultProps = {
    role: 'retailer' as const,
    userInfo: {
      name: 'Test User',
      email: 'test@example.com'
    }
  }

  it('renders correctly with default props', async () => {
    await router.push('/retailer/dashboard')
    const wrapper = mount(MaterialSidebar, {
      props: defaultProps,
      global: {
        plugins: [router]
      }
    })

    expect(wrapper.find('.text-foreground').text()).toContain('TOSS')
    expect(wrapper.text()).toContain('Dashboard')
  })

  it('shows user info when provided', () => {
    const wrapper = mount(MaterialSidebar, {
      props: defaultProps,
      global: {
        plugins: [router]
      }
    })

    expect(wrapper.text()).toContain('Test User')
    expect(wrapper.text()).toContain('test@example.com')
  })

  it('renders navigation items based on role', () => {
    const wrapper = mount(MaterialSidebar, {
      props: { ...defaultProps, role: 'retailer' },
      global: {
        plugins: [router]
      }
    })

    expect(wrapper.text()).toContain('POS & Store Solutions')
    expect(wrapper.text()).toContain('Inventory Management')
    expect(wrapper.text()).toContain('Warehouse Management')
  })

  it('renders supplier navigation when role is supplier', () => {
    const wrapper = mount(MaterialSidebar, {
      props: { ...defaultProps, role: 'supplier' },
      global: {
        plugins: [router]
      }
    })

    // Supplier currently uses the same module groups as other roles.
    expect(wrapper.text()).toContain('Dashboard')
    expect(wrapper.text()).toContain('Sales & Marketing')
    expect(wrapper.text()).toContain('Inventory Management')
  })
})









