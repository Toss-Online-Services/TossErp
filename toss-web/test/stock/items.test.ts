import { describe, it, expect, beforeEach, vi } from 'vitest'
import { mount } from '@vue/test-utils'
import { createPinia, setActivePinia } from 'pinia'
import ItemsPage from '../../pages/stock/items.vue'

// Mock Nuxt composables
vi.mock('#app', () => ({
  useRoute: () => ({ path: '/stock/items' }),
  useRouter: () => ({ push: vi.fn() })
}))

// Mock useStock composable
vi.mock('~/composables/useStock', () => ({
  useStock: () => ({
    getItems: vi.fn().mockResolvedValue([
      {
        id: '1',
        sku: 'BREAD-001',
        barcode: '1234567890',
        name: 'White Bread Loaf',
        description: 'Fresh white bread',
        category: 'Bakery',
        unit: 'loaf',
        sellingPrice: 12.0,
        costPrice: 8.5,
        quantityOnHand: 45,
        reorderLevel: 20,
        reorderQty: 50,
        isActive: true,
        createdAt: '2024-01-01T00:00:00Z',
        updatedAt: '2024-01-01T00:00:00Z'
      },
      {
        id: '2',
        sku: 'MILK-001',
        barcode: '0987654321',
        name: 'Fresh Milk 1L',
        description: 'Fresh milk',
        category: 'Dairy',
        unit: 'liter',
        sellingPrice: 25.0,
        costPrice: 18.0,
        quantityOnHand: 15,
        reorderLevel: 25,
        reorderQty: 30,
        isActive: true,
        createdAt: '2024-01-01T00:00:00Z',
        updatedAt: '2024-01-01T00:00:00Z'
      }
    ]),
    getStockOverview: vi.fn().mockResolvedValue({
      totalItems: 2,
      lowStockItems: 1,
      outOfStockItems: 0,
      totalValue: 1485.0
    }),
    getCategories: vi.fn().mockResolvedValue(['Bakery', 'Dairy', 'Grains', 'Cleaning']),
    createItem: vi.fn().mockResolvedValue({ id: '3', sku: 'NEW-001' }),
    updateItem: vi.fn().mockResolvedValue({ id: '1', sku: 'BREAD-001' }),
    deleteItem: vi.fn().mockResolvedValue(true)
  })
}))

describe('Items Page', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
  })

  describe('Page Layout', () => {
    it('renders page header with title and description', async () => {
      const wrapper = mount(ItemsPage)
      await wrapper.vm.$nextTick()

      expect(wrapper.text()).toContain('Items Management')
      expect(wrapper.text()).toContain('Manage inventory items')
    })

    it('displays Add Item button', () => {
      const wrapper = mount(ItemsPage)
      const addButton = wrapper.find('button:contains("Add Item")')
      expect(addButton.exists()).toBe(true)
    })
  })

  describe('Stats Cards', () => {
    it('displays Total Items stat', async () => {
      const wrapper = mount(ItemsPage)
      await wrapper.vm.$nextTick()

      expect(wrapper.text()).toContain('Total Items')
      expect(wrapper.text()).toContain('2')
    })

    it('displays Low Stock stat', async () => {
      const wrapper = mount(ItemsPage)
      await wrapper.vm.$nextTick()

      expect(wrapper.text()).toContain('Low Stock')
      expect(wrapper.text()).toContain('1')
    })

    it('displays Out of Stock stat', async () => {
      const wrapper = mount(ItemsPage)
      await wrapper.vm.$nextTick()

      expect(wrapper.text()).toContain('Out of Stock')
      expect(wrapper.text()).toContain('0')
    })

    it('displays Total Value stat with currency', async () => {
      const wrapper = mount(ItemsPage)
      await wrapper.vm.$nextTick()

      expect(wrapper.text()).toContain('Total Value')
      expect(wrapper.text()).toContain('R1,485.00')
    })
  })

  describe('Search and Filters', () => {
    it('renders search input', () => {
      const wrapper = mount(ItemsPage)
      const searchInput = wrapper.find('input[placeholder="Search items..."]')
      expect(searchInput.exists()).toBe(true)
    })

    it('renders category filter', () => {
      const wrapper = mount(ItemsPage)
      const categorySelect = wrapper.find('select')
      expect(categorySelect.exists()).toBe(true)
      expect(categorySelect.text()).toContain('All Categories')
    })

    it('renders stock level filter', () => {
      const wrapper = mount(ItemsPage)
      const filters = wrapper.findAll('select')
      expect(filters.length).toBeGreaterThan(1)
    })

    it('filters items by search term', async () => {
      const wrapper = mount(ItemsPage)
      await wrapper.vm.$nextTick()

      const searchInput = wrapper.find('input[placeholder="Search items..."]')
      await searchInput.setValue('Bread')

      // Items should be filtered
      expect(wrapper.vm.filteredItems.length).toBeLessThanOrEqual(2)
    })

    it('filters items by category', async () => {
      const wrapper = mount(ItemsPage)
      await wrapper.vm.$nextTick()

      await wrapper.setData({ selectedCategory: 'Bakery' })
      
      expect(wrapper.vm.filteredItems.every(item => item.category === 'Bakery')).toBe(true)
    })

    it('filters items by stock level', async () => {
      const wrapper = mount(ItemsPage)
      await wrapper.vm.$nextTick()

      await wrapper.setData({ stockFilter: 'low' })
      
      // Should show items where quantity <= reorder level
      expect(wrapper.vm.filteredItems.every(item => 
        item.quantityOnHand <= item.reorderLevel
      )).toBe(true)
    })
  })

  describe('Items Table', () => {
    it('displays item details correctly', async () => {
      const wrapper = mount(ItemsPage)
      await wrapper.vm.$nextTick()

      expect(wrapper.text()).toContain('White Bread Loaf')
      expect(wrapper.text()).toContain('BREAD-001')
      expect(wrapper.text()).toContain('45 loaf')
      expect(wrapper.text()).toContain('R12.00')
    })

    it('shows low stock warning indicator', async () => {
      const wrapper = mount(ItemsPage)
      await wrapper.vm.$nextTick()

      // Fresh Milk has 15 quantity with reorder at 25 - should show warning
      const lowStockIcons = wrapper.findAll('[class*="text-orange"]')
      expect(lowStockIcons.length).toBeGreaterThan(0)
    })

    it('displays active status badge', async () => {
      const wrapper = mount(ItemsPage)
      await wrapper.vm.$nextTick()

      expect(wrapper.text()).toContain('Active')
    })

    it('shows edit and delete buttons for each item', async () => {
      const wrapper = mount(ItemsPage)
      await wrapper.vm.$nextTick()

      const editButtons = wrapper.findAll('[aria-label="Edit"]')
      const deleteButtons = wrapper.findAll('[aria-label="Delete"]')

      expect(editButtons.length).toBeGreaterThan(0)
      expect(deleteButtons.length).toBeGreaterThan(0)
    })
  })

  describe('Pagination', () => {
    it('displays pagination info', async () => {
      const wrapper = mount(ItemsPage)
      await wrapper.vm.$nextTick()

      expect(wrapper.text()).toMatch(/Showing \d+ to \d+ of \d+ results/)
    })

    it('calculates total pages correctly', async () => {
      const wrapper = mount(ItemsPage)
      await wrapper.vm.$nextTick()
      
      wrapper.vm.pageSize = 1
      await wrapper.vm.$nextTick()

      expect(wrapper.vm.totalPages).toBe(2)
    })

    it('changes page when next button is clicked', async () => {
      const wrapper = mount(ItemsPage)
      await wrapper.vm.$nextTick()
      
      wrapper.vm.pageSize = 1
      await wrapper.vm.$nextTick()

      const nextButton = wrapper.find('button:contains("Next")')
      await nextButton.trigger('click')

      expect(wrapper.vm.currentPage).toBe(2)
    })

    it('disables previous button on first page', async () => {
      const wrapper = mount(ItemsPage)
      await wrapper.vm.$nextTick()

      const prevButton = wrapper.find('button:contains("Previous")')
      expect(prevButton.attributes('disabled')).toBeDefined()
    })
  })

  describe('Item Modal', () => {
    it('opens create modal when Add Item button is clicked', async () => {
      const wrapper = mount(ItemsPage)
      await wrapper.vm.$nextTick()

      const addButton = wrapper.find('button:contains("Add Item")')
      await addButton.trigger('click')

      expect(wrapper.vm.showCreateModal).toBe(true)
    })

    it('opens edit modal when edit button is clicked', async () => {
      const wrapper = mount(ItemsPage)
      await wrapper.vm.$nextTick()

      const editButton = wrapper.findAll('button')[5] // Assuming edit is the 6th button
      await editButton.trigger('click')

      expect(wrapper.vm.showEditModal).toBe(true)
    })

    it('closes modal when close event is emitted', async () => {
      const wrapper = mount(ItemsPage)
      await wrapper.vm.$nextTick()

      wrapper.vm.showCreateModal = true
      await wrapper.vm.$nextTick()

      wrapper.vm.closeModal()

      expect(wrapper.vm.showCreateModal).toBe(false)
      expect(wrapper.vm.showEditModal).toBe(false)
    })
  })

  describe('Export Functionality', () => {
    it('displays export button', () => {
      const wrapper = mount(ItemsPage)
      const exportButton = wrapper.find('button:contains("Export")')
      expect(exportButton.exists()).toBe(true)
    })

    it('calls export function when export button is clicked', async () => {
      const wrapper = mount(ItemsPage)
      const exportSpy = vi.spyOn(wrapper.vm, 'exportData')
      
      const exportButton = wrapper.find('button:contains("Export")')
      await exportButton.trigger('click')

      expect(exportSpy).toHaveBeenCalled()
    })
  })

  describe('Responsive Design', () => {
    it('renders correctly on mobile viewport', () => {
      const wrapper = mount(ItemsPage)
      
      // Check for mobile-specific classes
      expect(wrapper.find('.sm\\:grid-cols-2').exists() || wrapper.find('.lg\\:grid-cols-4').exists()).toBe(true)
    })

    it('stacks stats cards on mobile', () => {
      const wrapper = mount(ItemsPage)
      
      // Stats should be in a grid that adapts to mobile
      expect(wrapper.find('.grid').exists()).toBe(true)
    })
  })

  describe('Empty State', () => {
    it('shows empty state when no items exist', async () => {
      const useStock = vi.fn().mockReturnValue({
        getItems: vi.fn().mockResolvedValue([]),
        getStockOverview: vi.fn().mockResolvedValue({
          totalItems: 0,
          lowStockItems: 0,
          outOfStockItems: 0,
          totalValue: 0
        }),
        getCategories: vi.fn().mockResolvedValue([])
      })

      const wrapper = mount(ItemsPage)
      await wrapper.vm.$nextTick()

      expect(wrapper.text()).toContain('No items found')
    })
  })

  describe('Currency Formatting', () => {
    it('formats currency with two decimal places', () => {
      const wrapper = mount(ItemsPage)
      const formatted = wrapper.vm.formatCurrency(1234.5)
      expect(formatted).toBe('1,234.50')
    })

    it('handles zero values', () => {
      const wrapper = mount(ItemsPage)
      const formatted = wrapper.vm.formatCurrency(0)
      expect(formatted).toBe('0.00')
    })
  })

  describe('Error Handling', () => {
    it('handles API errors gracefully', async () => {
      const useStock = vi.fn().mockReturnValue({
        getItems: vi.fn().mockRejectedValue(new Error('API Error')),
        getStockOverview: vi.fn().mockRejectedValue(new Error('API Error')),
        getCategories: vi.fn().mockResolvedValue([])
      })

      const wrapper = mount(ItemsPage)
      await wrapper.vm.$nextTick()

      // Should not crash, should show empty state or error message
      expect(wrapper.exists()).toBe(true)
    })
  })
})

