import { describe, it, expect, beforeEach } from 'vitest'
import { mount } from '@vue/test-utils'
import ModuleCard from '~/components/ModuleCard.vue'

describe('ModuleCard', () => {
  const defaultProps = {
    title: 'Test Module',
    description: 'Test description',
    icon: '📊',
    href: '/test',
    color: 'blue'
  }

  beforeEach(() => {
    // Reset all mocks before each test
    vi.clearAllMocks()
  })

  it('renders correctly with basic props', () => {
    const wrapper = mount(ModuleCard, {
      props: defaultProps
    })

    expect(wrapper.find('h3').text()).toBe('Test Module')
    expect(wrapper.find('p').text()).toBe('Test description')
    expect(wrapper.text()).toContain('📊')
  })

  it('applies correct color classes', () => {
    const wrapper = mount(ModuleCard, {
      props: { ...defaultProps, color: 'green' }
    })

    const cardElement = wrapper.find('.bg-green-50')
    expect(cardElement.exists()).toBe(true)
  })

  it('handles click events', async () => {
    const wrapper = mount(ModuleCard, {
      props: defaultProps
    })

    await wrapper.trigger('click')
    expect(global.navigateTo).toHaveBeenCalledWith('/test')
  })

  it('displays coming soon badge when comingSoon is true', () => {
    const wrapper = mount(ModuleCard, {
      props: { ...defaultProps, comingSoon: true }
    })

    expect(wrapper.text()).toContain('Coming Soon')
  })

  it('shows correct stats when provided', () => {
    const wrapper = mount(ModuleCard, {
      props: { 
        ...defaultProps, 
        stats: { label: 'Items', value: '150' }
      }
    })

    expect(wrapper.text()).toContain('150')
    expect(wrapper.text()).toContain('Items')
  })
})
