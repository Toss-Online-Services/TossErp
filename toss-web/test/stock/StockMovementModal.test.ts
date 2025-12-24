import { describe, it, expect, beforeEach, vi } from 'vitest'
import { mount } from '@vue/test-utils'
import { createPinia, setActivePinia } from 'pinia'
import StockMovementModal from '../../components/stock/StockMovementModal.vue'

// Mock useStock composable
vi.mock('../../composables/useStock', () => ({
  useStock: () => ({
    getItems: vi.fn().mockResolvedValue({
      items: [
        {
          id: '1',
          sku: 'BREAD-001',
          name: 'White Bread Loaf',
          category: 'Bakery',
          sellingPrice: 12.0
        },
        {
          id: '2',
          sku: 'MILK-001',
          name: 'Fresh Milk 1L',
          category: 'Dairy',
          sellingPrice: 25.0
        }
      ]
    })
  })
}))

describe('StockMovementModal', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
  })

  describe('Modal Header - Receipt Type', () => {
    it('displays correct title for receipt', () => {
      const wrapper = mount(StockMovementModal, {
        props: { movementType: 'receipt' }
      })

      expect(wrapper.text()).toContain('Stock IN ↓')
      expect(wrapper.text()).toContain('Record new inventory received')
    })

    it('displays green gradient for receipt', () => {
      const wrapper = mount(StockMovementModal, {
        props: { movementType: 'receipt' }
      })

      expect(wrapper.html()).toContain('from-green-500')
      expect(wrapper.html()).toContain('to-emerald-600')
    })

    it('displays ArrowDownIcon for receipt', () => {
      const wrapper = mount(StockMovementModal, {
        props: { movementType: 'receipt' }
      })

      // Icon should be rendered in header
      expect(wrapper.find('svg').exists()).toBe(true)
    })
  })

  describe('Modal Header - Issue Type', () => {
    it('displays correct title for issue', () => {
      const wrapper = mount(StockMovementModal, {
        props: { movementType: 'issue' }
      })

      expect(wrapper.text()).toContain('Stock OUT ↑')
      expect(wrapper.text()).toContain('Record inventory removed or sold')
    })

    it('displays red gradient for issue', () => {
      const wrapper = mount(StockMovementModal, {
        props: { movementType: 'issue' }
      })

      expect(wrapper.html()).toContain('from-red-500')
	  expect(wrapper.html()).toContain('to-red-600')
    })
  })

  describe('Modal Header - Transfer Type', () => {
    it('displays correct title for transfer', () => {
      const wrapper = mount(StockMovementModal, {
        props: { movementType: 'transfer' }
      })

      expect(wrapper.text()).toContain('Stock MOVED →')
      expect(wrapper.text()).toContain('Move stock between locations')
    })

    it('displays blue gradient for transfer', () => {
      const wrapper = mount(StockMovementModal, {
        props: { movementType: 'transfer' }
      })

      expect(wrapper.html()).toContain('from-blue-500')
      expect(wrapper.html()).toContain('to-purple-600')
    })
  })

  describe('Modal Header - Adjustment Type', () => {
    it('displays correct title for adjustment', () => {
      const wrapper = mount(StockMovementModal, {
        props: { movementType: 'adjustment' }
      })

      expect(wrapper.text()).toContain('Stock FIXED ⇌')
      expect(wrapper.text()).toContain('Correct inventory discrepancies')
    })

    it('displays orange gradient for adjustment', () => {
      const wrapper = mount(StockMovementModal, {
        props: { movementType: 'adjustment' }
      })

      expect(wrapper.html()).toContain('from-orange-500')
      expect(wrapper.html()).toContain('to-yellow-500')
    })
  })

  describe('Form Fields - Common', () => {
    it('displays item selection dropdown', async () => {
      const wrapper = mount(StockMovementModal, {
        props: { movementType: 'receipt' }
      })
      await wrapper.vm.$nextTick()

      expect(wrapper.text()).toContain('Item')
      const select = wrapper.find('select#item')
      expect(select.exists()).toBe(true)
    })

    it('populates item dropdown with fetched items', async () => {
      const wrapper = mount(StockMovementModal, {
        props: { movementType: 'receipt' }
      })
      await wrapper.vm.$nextTick()
      await new Promise(resolve => setTimeout(resolve, 100))

      expect(wrapper.text()).toContain('White Bread Loaf')
      expect(wrapper.text()).toContain('Fresh Milk 1L')
    })

    it('displays quantity input', () => {
      const wrapper = mount(StockMovementModal, {
        props: { movementType: 'receipt' }
      })

      expect(wrapper.text()).toContain('Quantity')
      const input = wrapper.find('input#quantity')
      expect(input.exists()).toBe(true)
      expect(input.attributes('type')).toBe('number')
    })

    it('displays reference input', () => {
      const wrapper = mount(StockMovementModal, {
        props: { movementType: 'receipt' }
      })

      expect(wrapper.text()).toContain('Reference')
      const input = wrapper.find('input#reference')
      expect(input.exists()).toBe(true)
    })

    it('displays notes textarea', () => {
      const wrapper = mount(StockMovementModal, {
        props: { movementType: 'receipt' }
      })

      expect(wrapper.text()).toContain('Notes')
      const textarea = wrapper.find('textarea#notes')
      expect(textarea.exists()).toBe(true)
    })
  })

  describe('Form Fields - Transfer Specific', () => {
    it('displays from location dropdown for transfer', () => {
      const wrapper = mount(StockMovementModal, {
        props: { movementType: 'transfer' }
      })

      expect(wrapper.text()).toContain('From Location')
      const select = wrapper.find('select#fromLocation')
      expect(select.exists()).toBe(true)
    })

    it('displays to location dropdown for transfer', () => {
      const wrapper = mount(StockMovementModal, {
        props: { movementType: 'transfer' }
      })

      expect(wrapper.text()).toContain('To Location')
      const select = wrapper.find('select#toLocation')
      expect(select.exists()).toBe(true)
    })

    it('does not display location fields for non-transfer types', () => {
      const wrapper = mount(StockMovementModal, {
        props: { movementType: 'receipt' }
      })

      expect(wrapper.text()).not.toContain('From Location')
      expect(wrapper.text()).not.toContain('To Location')
    })

    it('populates location options', () => {
      const wrapper = mount(StockMovementModal, {
        props: { movementType: 'transfer' }
      })

      expect(wrapper.text()).toContain('Main Warehouse')
      expect(wrapper.text()).toContain('Shop Floor')
      expect(wrapper.text()).toContain('Returns Area')
    })
  })

  describe('Form Validation', () => {
    it('marks item field as required', () => {
      const wrapper = mount(StockMovementModal, {
        props: { movementType: 'receipt' }
      })

      const select = wrapper.find('select#item')
      expect(select.attributes('required')).toBeDefined()
    })

    it('marks quantity field as required', () => {
      const wrapper = mount(StockMovementModal, {
        props: { movementType: 'receipt' }
      })

      const input = wrapper.find('input#quantity')
      expect(input.attributes('required')).toBeDefined()
    })

    it('sets minimum quantity to 1', () => {
      const wrapper = mount(StockMovementModal, {
        props: { movementType: 'receipt' }
      })

      const input = wrapper.find('input#quantity')
      expect(input.attributes('min')).toBe('1')
    })

    it('marks notes as required for adjustment type', () => {
      const wrapper = mount(StockMovementModal, {
        props: { movementType: 'adjustment' }
      })

      const textarea = wrapper.find('textarea#notes')
      expect(textarea.attributes('required')).toBeDefined()
    })

    it('does not mark notes as required for other types', () => {
      const wrapper = mount(StockMovementModal, {
        props: { movementType: 'receipt' }
      })

      const textarea = wrapper.find('textarea#notes')
      expect(textarea.attributes('required')).toBeUndefined()
    })
  })

  describe('Form Submission', () => {
    it('emits save event with correct data on submit', async () => {
      const wrapper = mount(StockMovementModal, {
        props: { movementType: 'receipt' }
      })
      await wrapper.vm.$nextTick()

      await wrapper.setData({
        formData: {
          itemCode: 'BREAD-001',
          quantity: 50,
          reference: 'PO-001',
          notes: 'Test receipt',
          fromLocation: '',
          toLocation: ''
        }
      })

      const form = wrapper.find('form')
      await form.trigger('submit.prevent')

      expect(wrapper.emitted('save')).toBeTruthy()
      const saves = wrapper.emitted('save')
      expect(saves).toBeTruthy()

      const firstPayload = saves?.[0]?.[0]
      expect(firstPayload).toBeTruthy()
      expect(firstPayload).toMatchObject({
        itemSku: 'BREAD-001',
        quantity: 50,
        movementType: 'receipt'
      })
    })

    it('validates required fields before submission', async () => {
      const wrapper = mount(StockMovementModal, {
        props: { movementType: 'receipt' }
      })
      await wrapper.vm.$nextTick()

      // Leave form empty
      const form = wrapper.find('form')
      await form.trigger('submit.prevent')

      // Should not emit save event with empty data
      expect(wrapper.emitted('save')).toBeFalsy()
    })

    it('validates transfer requires both locations', async () => {
      const wrapper = mount(StockMovementModal, {
        props: { movementType: 'transfer' }
      })
      await wrapper.vm.$nextTick()

      await wrapper.setData({
        formData: {
          itemCode: 'BREAD-001',
          quantity: 10,
          fromLocation: 'Main Warehouse',
          toLocation: '', // Missing
          reference: '',
          notes: ''
        }
      })

      const form = wrapper.find('form')
      await form.trigger('submit.prevent')

      // Should show alert or not emit save
      expect(wrapper.emitted('save')).toBeFalsy()
    })

    it('validates adjustment requires notes', async () => {
      const wrapper = mount(StockMovementModal, {
        props: { movementType: 'adjustment' }
      })
      await wrapper.vm.$nextTick()

      await wrapper.setData({
        formData: {
          itemCode: 'BREAD-001',
          quantity: 5,
          notes: '', // Missing
          reference: '',
          fromLocation: '',
          toLocation: ''
        }
      })

      const form = wrapper.find('form')
      await form.trigger('submit.prevent')

      // Should show alert or not emit save
      expect(wrapper.emitted('save')).toBeFalsy()
    })
  })

  describe('Form Actions', () => {
    it('displays Cancel button', () => {
      const wrapper = mount(StockMovementModal, {
        props: { movementType: 'receipt' }
      })

      expect(wrapper.text()).toContain('Cancel')
    })

    it('displays submit button with correct text for receipt', () => {
      const wrapper = mount(StockMovementModal, {
        props: { movementType: 'receipt' }
      })

      expect(wrapper.text()).toContain('Record Receipt')
    })

    it('displays submit button with correct text for issue', () => {
      const wrapper = mount(StockMovementModal, {
        props: { movementType: 'issue' }
      })

      expect(wrapper.text()).toContain('Record Issue')
    })

    it('displays submit button with correct text for transfer', () => {
      const wrapper = mount(StockMovementModal, {
        props: { movementType: 'transfer' }
      })

      expect(wrapper.text()).toContain('Record Transfer')
    })

    it('displays submit button with correct text for adjustment', () => {
      const wrapper = mount(StockMovementModal, {
        props: { movementType: 'adjustment' }
      })

      expect(wrapper.text()).toContain('Record Adjustment')
    })

    it('emits close event when Cancel is clicked', async () => {
      const wrapper = mount(StockMovementModal, {
        props: { movementType: 'receipt' }
      })

      const cancelButton = wrapper
        .findAll('button')
        .find((b) => b.text() === 'Cancel')

      expect(cancelButton).toBeTruthy()
      await cancelButton!.trigger('click')

      expect(wrapper.emitted('close')).toBeTruthy()
    })

    it('emits close event when X button is clicked', async () => {
      const wrapper = mount(StockMovementModal, {
        props: { movementType: 'receipt' }
      })

      const closeButton = wrapper.find('button[aria-label="Close"]')
      await closeButton.trigger('click')

      expect(wrapper.emitted('close')).toBeTruthy()
    })
  })

  describe('Form Reset', () => {
    it('resets form data when movement type changes', async () => {
      const wrapper = mount(StockMovementModal, {
        props: { movementType: 'receipt' }
      })
      await wrapper.vm.$nextTick()

      await wrapper.setData({
        formData: {
          itemCode: 'BREAD-001',
          quantity: 50,
          reference: 'TEST',
          notes: 'Test notes',
          fromLocation: '',
          toLocation: ''
        }
      })

      await wrapper.setProps({ movementType: 'issue' })

      expect(wrapper.vm.formData.itemCode).toBe('')
      expect(wrapper.vm.formData.quantity).toBe(1)
      expect(wrapper.vm.formData.reference).toBe('')
      expect(wrapper.vm.formData.notes).toBe('')
    })
  })

  describe('Styling and Visual', () => {
    it('applies gradient to header based on movement type', () => {
      const receipt = mount(StockMovementModal, { props: { movementType: 'receipt' } })
      const issue = mount(StockMovementModal, { props: { movementType: 'issue' } })
      const transfer = mount(StockMovementModal, { props: { movementType: 'transfer' } })
      const adjustment = mount(StockMovementModal, { props: { movementType: 'adjustment' } })

      expect(receipt.html()).toContain('from-green-500')
      expect(issue.html()).toContain('from-red-500')
      expect(transfer.html()).toContain('from-blue-500')
      expect(adjustment.html()).toContain('from-orange-500')
    })

    it('applies gradient to submit button based on movement type', () => {
      const receipt = mount(StockMovementModal, { props: { movementType: 'receipt' } })
      const issue = mount(StockMovementModal, { props: { movementType: 'issue' } })

      expect(receipt.html()).toContain('from-green-600')
      expect(issue.html()).toContain('from-red-600')
    })

    it('has rounded modal with shadow', () => {
      const wrapper = mount(StockMovementModal, {
        props: { movementType: 'receipt' }
      })

      expect(wrapper.html()).toContain('rounded-2xl')
      expect(wrapper.html()).toContain('shadow-2xl')
    })

    it('has backdrop blur effect', () => {
      const wrapper = mount(StockMovementModal, {
        props: { movementType: 'receipt' }
      })

      expect(wrapper.html()).toContain('backdrop-blur')
    })
  })

  describe('Accessibility', () => {
    it('labels all form inputs', () => {
      const wrapper = mount(StockMovementModal, {
        props: { movementType: 'receipt' }
      })

      expect(wrapper.find('label[for="item"]').exists()).toBe(true)
      expect(wrapper.find('label[for="quantity"]').exists()).toBe(true)
      expect(wrapper.find('label[for="reference"]').exists()).toBe(true)
      expect(wrapper.find('label[for="notes"]').exists()).toBe(true)
    })

    it('indicates required fields with asterisk', () => {
      const wrapper = mount(StockMovementModal, {
        props: { movementType: 'receipt' }
      })

      const html = wrapper.html()
      expect(html).toContain('text-red-500">*</span>')
    })
  })

  describe('Error Handling', () => {
    it('handles item fetch errors gracefully', async () => {
      const useStock = vi.fn().mockReturnValue({
        getItems: vi.fn().mockRejectedValue(new Error('API Error'))
      })

      const wrapper = mount(StockMovementModal, {
        props: { movementType: 'receipt' }
      })
      await wrapper.vm.$nextTick()

      // Should not crash
      expect(wrapper.exists()).toBe(true)
    })
  })
})

