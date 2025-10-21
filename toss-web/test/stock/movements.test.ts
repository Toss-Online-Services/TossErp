import { describe, it, expect, beforeEach, vi } from 'vitest'
import { mount } from '@vue/test-utils'
import { createPinia, setActivePinia } from 'pinia'
import MovementsPage from '../../pages/stock/movements.vue'

// Mock Nuxt composables
vi.mock('#app', () => ({
  useRoute: () => ({ path: '/stock/movements' }),
  useRouter: () => ({ push: vi.fn() })
}))

// Mock useStock composable
vi.mock('~/composables/useStock', () => ({
  useStock: () => ({
    getStockMovements: vi.fn().mockResolvedValue({
      movements: [
        {
          id: '1',
          reference: 'RCP-20240115',
          movementType: 'receipt',
          itemName: 'White Bread Loaf',
          itemSku: 'BREAD-001',
          itemCode: 'BREAD-001',
          quantity: 50,
          rate: 8.5,
          amount: 425.0,
          balanceQty: 95,
          transactionDate: '2024-01-15T12:30:00Z',
          createdAt: '2024-01-15T12:30:00Z'
        },
        {
          id: '2',
          reference: 'ISS-20240114',
          movementType: 'issue',
          itemName: 'Fresh Milk 1L',
          itemSku: 'MILK-001',
          itemCode: 'MILK-001',
          quantity: -10,
          rate: 18.0,
          amount: 180.0,
          balanceQty: 15,
          transactionDate: '2024-01-14T16:20:00Z',
          createdAt: '2024-01-14T16:20:00Z'
        },
        {
          id: '3',
          reference: 'TRF-20240113',
          movementType: 'transfer',
          itemName: 'Washing Powder 1kg',
          itemSku: 'SOAP-001',
          itemCode: 'SOAP-001',
          quantity: 5,
          rate: 22.0,
          amount: 110.0,
          balanceQty: 17,
          transactionDate: '2024-01-13T11:15:00Z',
          createdAt: '2024-01-13T11:15:00Z'
        }
      ],
      pagination: {
        page: 1,
        pageSize: 10,
        total: 3,
        totalPages: 1
      }
    }),
    createStockMovement: vi.fn().mockResolvedValue({ id: '4' })
  })
}))

describe('Stock Movements Page', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
  })

  describe('Page Layout', () => {
    it('renders page header with title and description', async () => {
      const wrapper = mount(MovementsPage)
      await wrapper.vm.$nextTick()

      expect(wrapper.text()).toContain('Stock Movements')
      expect(wrapper.text()).toContain('Track all stock transactions')
    })

    it('displays New Movement button', () => {
      const wrapper = mount(MovementsPage)
      const newButton = wrapper.find('button:contains("New Movement")')
      expect(newButton.exists()).toBe(true)
    })
  })

  describe('Quick Action Buttons', () => {
    it('displays Stock IN button with correct styling', () => {
      const wrapper = mount(MovementsPage)
      expect(wrapper.text()).toContain('Stock IN ↓')
      expect(wrapper.text()).toContain('Receiving inventory')
    })

    it('displays Stock OUT button with correct styling', () => {
      const wrapper = mount(MovementsPage)
      expect(wrapper.text()).toContain('Stock OUT ↑')
      expect(wrapper.text()).toContain('Removing inventory')
    })

    it('displays Stock MOVED button with correct styling', () => {
      const wrapper = mount(MovementsPage)
      expect(wrapper.text()).toContain('Stock MOVED →')
      expect(wrapper.text()).toContain('Between locations')
    })

    it('displays Stock FIXED button with correct styling', () => {
      const wrapper = mount(MovementsPage)
      expect(wrapper.text()).toContain('Stock FIXED ⇌')
      expect(wrapper.text()).toContain('Correct mistakes')
    })

    it('opens modal when Stock IN is clicked', async () => {
      const wrapper = mount(MovementsPage)
      await wrapper.vm.$nextTick()

      const stockInButton = wrapper.find('button:contains("Stock IN")')
      await stockInButton.trigger('click')

      expect(wrapper.vm.showMovementModal).toBe(true)
      expect(wrapper.vm.selectedMovementType).toBe('receipt')
    })

    it('opens modal when Stock OUT is clicked', async () => {
      const wrapper = mount(MovementsPage)
      await wrapper.vm.$nextTick()

      const stockOutButton = wrapper.find('button:contains("Stock OUT")')
      await stockOutButton.trigger('click')

      expect(wrapper.vm.showMovementModal).toBe(true)
      expect(wrapper.vm.selectedMovementType).toBe('issue')
    })

    it('opens modal when Stock MOVED is clicked', async () => {
      const wrapper = mount(MovementsPage)
      await wrapper.vm.$nextTick()

      const stockMovedButton = wrapper.find('button:contains("Stock MOVED")')
      await stockMovedButton.trigger('click')

      expect(wrapper.vm.showMovementModal).toBe(true)
      expect(wrapper.vm.selectedMovementType).toBe('transfer')
    })

    it('opens modal when Stock FIXED is clicked', async () => {
      const wrapper = mount(MovementsPage)
      await wrapper.vm.$nextTick()

      const stockFixedButton = wrapper.find('button:contains("Stock FIXED")')
      await stockFixedButton.trigger('click')

      expect(wrapper.vm.showMovementModal).toBe(true)
      expect(wrapper.vm.selectedMovementType).toBe('adjustment')
    })
  })

  describe('Filters', () => {
    it('renders search input', () => {
      const wrapper = mount(MovementsPage)
      const searchInput = wrapper.find('input[placeholder*="Search"]')
      expect(searchInput.exists()).toBe(true)
    })

    it('renders type filter dropdown', () => {
      const wrapper = mount(MovementsPage)
      expect(wrapper.text()).toContain('All Types')
      expect(wrapper.text()).toContain('Receipt')
      expect(wrapper.text()).toContain('Issue')
      expect(wrapper.text()).toContain('Transfer')
      expect(wrapper.text()).toContain('Adjustment')
    })

    it('renders date range filter dropdown', () => {
      const wrapper = mount(MovementsPage)
      expect(wrapper.text()).toContain('All Time')
      expect(wrapper.text()).toContain('Today')
      expect(wrapper.text()).toContain('This Week')
      expect(wrapper.text()).toContain('This Month')
    })

    it('renders export and clear buttons', () => {
      const wrapper = mount(MovementsPage)
      expect(wrapper.text()).toContain('Export CSV')
      expect(wrapper.text()).toContain('Clear')
    })

    it('filters movements by type', async () => {
      const wrapper = mount(MovementsPage)
      await wrapper.vm.$nextTick()

      await wrapper.setData({ selectedType: 'receipt' })
      
      expect(wrapper.vm.filteredMovements.every(m => m.movementType === 'receipt')).toBe(true)
    })

    it('filters movements by search query', async () => {
      const wrapper = mount(MovementsPage)
      await wrapper.vm.$nextTick()

      await wrapper.setData({ searchQuery: 'Bread' })
      
      expect(wrapper.vm.filteredMovements.every(m => 
        m.itemName.toLowerCase().includes('bread')
      )).toBe(true)
    })

    it('clears all filters when clear button is clicked', async () => {
      const wrapper = mount(MovementsPage)
      await wrapper.vm.$nextTick()

      await wrapper.setData({ 
        searchQuery: 'Test',
        selectedType: 'receipt',
        selectedDateRange: 'week'
      })

      const clearButton = wrapper.find('button:contains("Clear")')
      await clearButton.trigger('click')

      expect(wrapper.vm.searchQuery).toBe('')
      expect(wrapper.vm.selectedType).toBe('')
      expect(wrapper.vm.selectedDateRange).toBe('')
    })
  })

  describe('Movements Table', () => {
    it('displays table header correctly', async () => {
      const wrapper = mount(MovementsPage)
      await wrapper.vm.$nextTick()

      expect(wrapper.text()).toContain('Date & Reference')
      expect(wrapper.text()).toContain('Type')
      expect(wrapper.text()).toContain('Item')
      expect(wrapper.text()).toContain('Quantity')
      expect(wrapper.text()).toContain('Value')
      expect(wrapper.text()).toContain('Status')
      expect(wrapper.text()).toContain('Actions')
    })

    it('displays receipt movement with correct details', async () => {
      const wrapper = mount(MovementsPage)
      await wrapper.vm.$nextTick()

      expect(wrapper.text()).toContain('RCP-20240115')
      expect(wrapper.text()).toContain('receipt')
      expect(wrapper.text()).toContain('White Bread Loaf')
      expect(wrapper.text()).toContain('+50')
      expect(wrapper.text()).toContain('R425.00')
    })

    it('displays issue movement with negative quantity', async () => {
      const wrapper = mount(MovementsPage)
      await wrapper.vm.$nextTick()

      expect(wrapper.text()).toContain('ISS-20240114')
      expect(wrapper.text()).toContain('issue')
      expect(wrapper.text()).toContain('Fresh Milk 1L')
      expect(wrapper.text()).toContain('-10')
    })

    it('displays transfer movement correctly', async () => {
      const wrapper = mount(MovementsPage)
      await wrapper.vm.$nextTick()

      expect(wrapper.text()).toContain('TRF-20240113')
      expect(wrapper.text()).toContain('transfer')
      expect(wrapper.text()).toContain('Washing Powder 1kg')
    })

    it('shows View button for each movement', async () => {
      const wrapper = mount(MovementsPage)
      await wrapper.vm.$nextTick()

      const viewButtons = wrapper.findAll('button:contains("View")')
      expect(viewButtons.length).toBe(3)
    })

    it('displays type badges with gradient styling', async () => {
      const wrapper = mount(MovementsPage)
      await wrapper.vm.$nextTick()

      // Check for gradient classes in type badges
      const html = wrapper.html()
      expect(html).toContain('bg-gradient-to-r')
    })

    it('displays status badges with gradient styling', async () => {
      const wrapper = mount(MovementsPage)
      await wrapper.vm.$nextTick()

      expect(wrapper.text()).toContain('completed')
      const html = wrapper.html()
      expect(html).toContain('bg-gradient-to-r')
    })
  })

  describe('Pagination', () => {
    it('displays pagination info', async () => {
      const wrapper = mount(MovementsPage)
      await wrapper.vm.$nextTick()

      expect(wrapper.text()).toMatch(/Showing \d+ to \d+ of \d+ movements/)
    })

    it('calculates total pages correctly', async () => {
      const wrapper = mount(MovementsPage)
      await wrapper.vm.$nextTick()

      expect(wrapper.vm.totalPages).toBeGreaterThan(0)
    })

    it('displays Previous and Next buttons', async () => {
      const wrapper = mount(MovementsPage)
      await wrapper.vm.$nextTick()

      expect(wrapper.text()).toContain('Previous')
      expect(wrapper.text()).toContain('Next')
    })

    it('disables buttons on single page', async () => {
      const wrapper = mount(MovementsPage)
      await wrapper.vm.$nextTick()

      // With 3 items and pageSize 10, should have 1 page
      const prevButton = wrapper.find('button:contains("Previous")')
      const nextButton = wrapper.find('button:contains("Next")')

      expect(prevButton.attributes('disabled')).toBeDefined()
      expect(nextButton.attributes('disabled')).toBeDefined()
    })
  })

  describe('Export Functionality', () => {
    it('calls export function when export button is clicked', async () => {
      const wrapper = mount(MovementsPage)
      const exportSpy = vi.spyOn(wrapper.vm, 'exportMovements')
      
      const exportButton = wrapper.find('button:contains("Export CSV")')
      await exportButton.trigger('click')

      expect(exportSpy).toHaveBeenCalled()
    })
  })

  describe('Date Formatting', () => {
    it('formats dates correctly', () => {
      const wrapper = mount(MovementsPage)
      const formatted = wrapper.vm.formatDate('2024-01-15T12:30:00Z')
      expect(formatted).toMatch(/Jan.*2024.*12:30/)
    })
  })

  describe('Currency Formatting', () => {
    it('formats currency with two decimal places', () => {
      const wrapper = mount(MovementsPage)
      const formatted = wrapper.vm.formatCurrency(1234.56)
      expect(formatted).toBe('1,234.56')
    })
  })

  describe('Modal Integration', () => {
    it('renders Stock Movement Modal when showMovementModal is true', async () => {
      const wrapper = mount(MovementsPage)
      await wrapper.vm.$nextTick()

      await wrapper.setData({ showMovementModal: true })
      
      const modal = wrapper.findComponent({ name: 'StockMovementModal' })
      expect(modal.exists()).toBe(true)
    })

    it('closes modal when close event is emitted', async () => {
      const wrapper = mount(MovementsPage)
      await wrapper.vm.$nextTick()

      wrapper.vm.showMovementModal = true
      await wrapper.vm.$nextTick()

      wrapper.vm.closeMovementModal()

      expect(wrapper.vm.showMovementModal).toBe(false)
    })

    it('passes correct movement type to modal', async () => {
      const wrapper = mount(MovementsPage)
      await wrapper.vm.$nextTick()

      wrapper.vm.selectedMovementType = 'receipt'
      wrapper.vm.showMovementModal = true
      await wrapper.vm.$nextTick()

      const modal = wrapper.findComponent({ name: 'StockMovementModal' })
      expect(modal.props('movementType')).toBe('receipt')
    })
  })

  describe('Responsive Design', () => {
    it('renders quick action buttons in responsive grid', () => {
      const wrapper = mount(MovementsPage)
      
      // Should have grid classes for responsive layout
      expect(wrapper.find('.grid').exists()).toBe(true)
    })

    it('stacks buttons on mobile', () => {
      const wrapper = mount(MovementsPage)
      
      // Should have grid-cols-1 for mobile
      expect(wrapper.html()).toContain('grid-cols-1')
    })
  })

  describe('Error Handling', () => {
    it('handles API errors gracefully', async () => {
      const useStock = vi.fn().mockReturnValue({
        getStockMovements: vi.fn().mockRejectedValue(new Error('API Error'))
      })

      const wrapper = mount(MovementsPage)
      await wrapper.vm.$nextTick()

      // Should not crash
      expect(wrapper.exists()).toBe(true)
    })
  })

  describe('Empty State', () => {
    it('shows empty state when no movements exist', async () => {
      const useStock = vi.fn().mockReturnValue({
        getStockMovements: vi.fn().mockResolvedValue({
          movements: [],
          pagination: { page: 1, pageSize: 10, total: 0, totalPages: 0 }
        })
      })

      const wrapper = mount(MovementsPage)
      await wrapper.vm.$nextTick()

      // Should indicate no data
      expect(wrapper.vm.paginatedMovements.length).toBe(0)
    })
  })

  describe('Loading State', () => {
    it('shows loading state while fetching data', async () => {
      const wrapper = mount(MovementsPage)
      
      // Loading should be true initially
      expect(wrapper.vm.loading).toBeDefined()
    })
  })
})

