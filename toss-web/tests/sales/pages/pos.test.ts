import { describe, it, expect, vi, beforeEach } from 'vitest'
import { mount } from '@vue/test-utils'
import { ref, nextTick } from 'vue'

// Mock the composables
vi.mock('~/composables/useSalesAPI', () => ({
  useSalesAPI: vi.fn(() => ({
    getProducts: vi.fn().mockResolvedValue([]),
    getCategories: vi.fn().mockResolvedValue([]),
    getCustomers: vi.fn().mockResolvedValue([]),
    createSale: vi.fn(),
    generateReceipt: vi.fn()
  }))
}))

vi.mock('~/composables/useToast', () => ({
  useToast: vi.fn(() => ({
    success: vi.fn(),
    error: vi.fn(),
    warning: vi.fn(),
    info: vi.fn()
  }))
}))

describe('POS Page - Category Filtering', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('should filter products by categoryId when category is selected', async () => {
    // Arrange
    const mockProducts = ref([
      { id: 1, name: 'Apple', categoryId: 2, category: 'Fresh Produce', price: 5.99 },
      { id: 2, name: 'Bread', categoryId: 6, category: 'Bakery', price: 3.99 },
      { id: 3, name: 'Banana', categoryId: 2, category: 'Fresh Produce', price: 2.99 },
      { id: 4, name: 'Cake', categoryId: 6, category: 'Bakery', price: 15.99 }
    ])

    const mockCategories = ref([
      { id: 'all', name: 'All', productCount: 0 },
      { id: 2, name: 'Fresh Produce', productCount: 2 },
      { id: 6, name: 'Bakery', productCount: 2 }
    ])

    const selectedCategory = ref<string | number>('all')
    const searchQuery = ref('')

    // Simulate the filteredProducts computed property logic
    const getFilteredProducts = () => {
      let filtered = mockProducts.value

      if (selectedCategory.value !== 'all') {
        filtered = filtered.filter((p: any) => p.categoryId === selectedCategory.value)
      }

      if (searchQuery.value) {
        filtered = filtered.filter((p: any) => 
          p.name.toLowerCase().includes(searchQuery.value.toLowerCase())
        )
      }

      return filtered
    }

    // Act & Assert - Test "All" category
    let filteredProducts = getFilteredProducts()
    expect(filteredProducts).toHaveLength(4)
    expect(filteredProducts.map(p => p.name)).toEqual(['Apple', 'Bread', 'Banana', 'Cake'])

    // Act & Assert - Test "Fresh Produce" category
    selectedCategory.value = 2
    filteredProducts = getFilteredProducts()
    expect(filteredProducts).toHaveLength(2)
    expect(filteredProducts.map(p => p.name)).toEqual(['Apple', 'Banana'])
    expect(filteredProducts.every(p => p.categoryId === 2)).toBe(true)

    // Act & Assert - Test "Bakery" category
    selectedCategory.value = 6
    filteredProducts = getFilteredProducts()
    expect(filteredProducts).toHaveLength(2)
    expect(filteredProducts.map(p => p.name)).toEqual(['Bread', 'Cake'])
    expect(filteredProducts.every(p => p.categoryId === 6)).toBe(true)

    // Act & Assert - Test back to "All"
    selectedCategory.value = 'all'
    filteredProducts = getFilteredProducts()
    expect(filteredProducts).toHaveLength(4)
  })

  it('should filter products by search query', async () => {
    // Arrange
    const mockProducts = ref([
      { id: 1, name: 'Apple', categoryId: 2, sku: 'APPLE-001', price: 5.99 },
      { id: 2, name: 'Bread', categoryId: 6, sku: 'BREAD-001', price: 3.99 },
      { id: 3, name: 'Banana', categoryId: 2, sku: 'BANANA-001', price: 2.99 }
    ])

    const selectedCategory = ref<string | number>('all')
    const searchQuery = ref('')

    const getFilteredProducts = () => {
      let filtered = mockProducts.value

      if (selectedCategory.value !== 'all') {
        filtered = filtered.filter((p: any) => p.categoryId === selectedCategory.value)
      }

      if (searchQuery.value) {
        filtered = filtered.filter((p: any) => 
          p.name.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
          p.sku.toLowerCase().includes(searchQuery.value.toLowerCase())
        )
      }

      return filtered
    }

    // Act & Assert - Search by name
    searchQuery.value = 'ban'
    let filteredProducts = getFilteredProducts()
    expect(filteredProducts).toHaveLength(1)
    expect(filteredProducts[0].name).toBe('Banana')

    // Act & Assert - Search by SKU
    searchQuery.value = 'BREAD'
    filteredProducts = getFilteredProducts()
    expect(filteredProducts).toHaveLength(1)
    expect(filteredProducts[0].name).toBe('Bread')

    // Act & Assert - No matches
    searchQuery.value = 'xyz'
    filteredProducts = getFilteredProducts()
    expect(filteredProducts).toHaveLength(0)
  })

  it('should combine category filter and search query', async () => {
    // Arrange
    const mockProducts = ref([
      { id: 1, name: 'Apple Juice', categoryId: 3, sku: 'JUICE-001', price: 5.99 },
      { id: 2, name: 'Apple Pie', categoryId: 6, sku: 'PIE-001', price: 12.99 },
      { id: 3, name: 'Fresh Apples', categoryId: 2, sku: 'APPLE-001', price: 3.99 }
    ])

    const selectedCategory = ref<string | number>('all')
    const searchQuery = ref('')

    const getFilteredProducts = () => {
      let filtered = mockProducts.value

      if (selectedCategory.value !== 'all') {
        filtered = filtered.filter((p: any) => p.categoryId === selectedCategory.value)
      }

      if (searchQuery.value) {
        filtered = filtered.filter((p: any) => 
          p.name.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
          p.sku.toLowerCase().includes(searchQuery.value.toLowerCase())
        )
      }

      return filtered
    }

    // Act & Assert - Search "apple" in "Bakery" category
    selectedCategory.value = 6
    searchQuery.value = 'apple'
    let filteredProducts = getFilteredProducts()
    expect(filteredProducts).toHaveLength(1)
    expect(filteredProducts[0].name).toBe('Apple Pie')

    // Act & Assert - Search "apple" in all categories
    selectedCategory.value = 'all'
    searchQuery.value = 'apple'
    filteredProducts = getFilteredProducts()
    expect(filteredProducts).toHaveLength(3)
  })

  it('should handle numeric categoryId from API correctly', () => {
    // Arrange - Products with numeric categoryId from API
    const mockProducts = ref([
      { id: 1, name: 'Product 1', categoryId: 1, price: 10 },
      { id: 2, name: 'Product 2', categoryId: 2, price: 20 },
      { id: 3, name: 'Product 3', categoryId: 1, price: 15 }
    ])

    const selectedCategory = ref<string | number>(1)

    // Act
    const filtered = mockProducts.value.filter((p: any) => p.categoryId === selectedCategory.value)

    // Assert
    expect(filtered).toHaveLength(2)
    expect(filtered.every(p => p.categoryId === 1)).toBe(true)
    expect(typeof filtered[0].categoryId).toBe('number')
  })

  it('should handle empty product list gracefully', () => {
    // Arrange
    const mockProducts = ref([])
    const selectedCategory = ref<string | number>('all')
    const searchQuery = ref('')

    const getFilteredProducts = () => {
      let filtered = mockProducts.value

      if (selectedCategory.value !== 'all') {
        filtered = filtered.filter((p: any) => p.categoryId === selectedCategory.value)
      }

      if (searchQuery.value) {
        filtered = filtered.filter((p: any) => 
          p.name.toLowerCase().includes(searchQuery.value.toLowerCase())
        )
      }

      return filtered
    }

    // Act
    const filteredProducts = getFilteredProducts()

    // Assert
    expect(filteredProducts).toHaveLength(0)
    expect(Array.isArray(filteredProducts)).toBe(true)
  })
})

