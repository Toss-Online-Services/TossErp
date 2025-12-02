import { describe, it, expect, vi, beforeEach } from 'vitest'
import { useProductsAPI } from '~/composables/useProductsAPI'

// Mock $fetch
vi.mock('#app', () => ({
  ...vi.importActual('#app'),
}))

global.$fetch = vi.fn()

describe('useProductsAPI', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('getProducts', () => {
    it('should fetch products with correct parameters', async () => {
      // Arrange
      const mockResponse = {
        items: [
          {
            id: 1,
            name: 'Apple',
            sku: 'APPLE-001',
            barcode: '123456',
            categoryName: 'Fresh Produce',
            basePrice: 5.99,
            categoryId: 2,
            isActive: true
          }
        ],
        pageNumber: 1,
        totalPages: 1,
        totalCount: 1
      }
      
      ;(global.$fetch as any).mockResolvedValue(mockResponse)
      const api = useProductsAPI()

      // Act
      const result = await api.getProducts(1)

      // Assert
      expect(global.$fetch).toHaveBeenCalledWith(
        expect.stringContaining('/Inventory/products'),
        expect.objectContaining({
          method: 'GET',
          params: {
            PageNumber: 1,
            PageSize: 1000,
            IsActive: true
          }
        })
      )
      expect(result).toEqual(mockResponse.items)
      expect(Array.isArray(result)).toBe(true)
      expect(result[0]).toHaveProperty('id')
      expect(result[0]).toHaveProperty('categoryId')
    })

    it('should return empty array when no products found', async () => {
      // Arrange
      const mockResponse = {
        items: [],
        pageNumber: 1,
        totalPages: 0,
        totalCount: 0
      }
      
      ;(global.$fetch as any).mockResolvedValue(mockResponse)
      const api = useProductsAPI()

      // Act
      const result = await api.getProducts(1)

      // Assert
      expect(result).toEqual([])
      expect(Array.isArray(result)).toBe(true)
    })

    it('should handle API errors gracefully', async () => {
      // Arrange
      const mockError = new Error('API Error')
      ;(global.$fetch as any).mockRejectedValue(mockError)
      const api = useProductsAPI()

      // Act & Assert
      await expect(api.getProducts(1)).rejects.toThrow('API Error')
    })
  })

  describe('getCategories', () => {
    it('should fetch categories with shopId parameter', async () => {
      // Arrange
      const mockCategories = [
        {
          id: 1,
          name: 'Groceries',
          description: 'Grocery items',
          parentCategoryId: null,
          productCount: 10
        },
        {
          id: 2,
          name: 'Fresh Produce',
          description: 'Fresh fruits and vegetables',
          parentCategoryId: 1,
          productCount: 5
        }
      ]
      
      ;(global.$fetch as any).mockResolvedValue(mockCategories)
      const api = useProductsAPI()

      // Act
      const result = await api.getCategories(1)

      // Assert
      expect(global.$fetch).toHaveBeenCalledWith(
        expect.stringContaining('/Inventory/categories'),
        expect.objectContaining({
          method: 'GET',
          params: { shopId: 1 }
        })
      )
      expect(result).toEqual(mockCategories)
      expect(Array.isArray(result)).toBe(true)
      expect(result[0]).toHaveProperty('id')
      expect(result[0]).toHaveProperty('name')
      expect(result[0]).toHaveProperty('productCount')
    })

    it('should return empty array when no categories found', async () => {
      // Arrange
      ;(global.$fetch as any).mockResolvedValue([])
      const api = useProductsAPI()

      // Act
      const result = await api.getCategories(1)

      // Assert
      expect(result).toEqual([])
      expect(Array.isArray(result)).toBe(true)
    })

    it('should handle API errors gracefully', async () => {
      // Arrange
      const mockError = new Error('Categories API Error')
      ;(global.$fetch as any).mockRejectedValue(mockError)
      const api = useProductsAPI()

      // Act & Assert
      await expect(api.getCategories(1)).rejects.toThrow('Categories API Error')
    })
  })
})

