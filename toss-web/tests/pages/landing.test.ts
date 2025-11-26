import { describe, it, expect } from 'vitest'
import { mount } from '@vue/test-utils'
import LandingPage from '~/pages/index.vue'

describe('Landing Page', () => {
  it('renders the hero section', () => {
    const wrapper = mount(LandingPage)

    expect(wrapper.text()).toContain('Run your hustle like')
    expect(wrapper.text()).toContain('a big enterprise')
  })

  it('renders navigation links', () => {
    const wrapper = mount(LandingPage)

    expect(wrapper.text()).toContain('How it works')
    expect(wrapper.text()).toContain('For shops')
    expect(wrapper.text()).toContain('For suppliers & drivers')
    expect(wrapper.text()).toContain('FAQ')
  })

  it('renders call-to-action buttons', () => {
    const wrapper = mount(LandingPage)

    expect(wrapper.text()).toContain('Join TOSS')
    expect(wrapper.text()).toContain('Sign in')
  })

  it('renders problem/solution section', () => {
    const wrapper = mount(LandingPage)

    expect(wrapper.text()).toContain('The everyday struggle')
    expect(wrapper.text()).toContain('The Problems')
    expect(wrapper.text()).toContain('How TOSS Helps')
  })

  it('renders features section', () => {
    const wrapper = mount(LandingPage)

    expect(wrapper.text()).toContain('Simple POS on your phone')
    expect(wrapper.text()).toContain('See your stock in one place')
    expect(wrapper.text()).toContain('Order from suppliers in a few taps')
  })

  it('renders FAQ section', () => {
    const wrapper = mount(LandingPage)

    expect(wrapper.text()).toContain('Common questions')
    expect(wrapper.text()).toContain('How much does TOSS cost?')
  })
})









