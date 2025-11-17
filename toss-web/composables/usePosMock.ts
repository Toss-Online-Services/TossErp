/**
 * usePosMock - POS Mock API Composable
 * 
 * Wraps MockSalesService POS methods and provides additional offline queue integration.
 * This is the data layer for POS operations, handling product lookup, customer search,
 * sale creation, parked sales, and returns.
 */

import { MockSalesService } from '~/services/mock/sales'
import { mockProducts, mockCategories, searchMockProducts, getMockProductByBarcode } from '~/services/mock/products'
import type { PosSale, PosParkedSale, PosSaleItem, PosPaymentEntry } from '~/types/sales'

export interface CartItem {
  productId: number
  productName: string
  sku: string
  barcode?: string
  quantity: number
  unitPrice: number
  discount: number
  discountType: 'fixed' | 'percentage'
  taxRate: number
  isTaxable: boolean
}

export interface Customer {
  id: number
  name: string
  phone?: string
  email?: string
  creditLimit?: number
  creditBalance?: number
}

export const usePosMock = () => {
  const productsAPI = useProductsAPI()
  
  /**
   * Fetch products for POS (with optional search/filter)
   */
  const fetchProducts = async (shopId: number, searchTerm?: string, categoryId?: number) => {
    try {
      console.log('[usePosMock] fetchProducts called:', { shopId, searchTerm, categoryId, mockProductsLength: mockProducts.length })
      
      // Use mock products for testing/development
      let filteredProducts = [...mockProducts]
      
      // Filter by search term
      if (searchTerm) {
        const term = searchTerm.toLowerCase()
        filteredProducts = filteredProducts.filter(p =>
          p.name.toLowerCase().includes(term) ||
          p.sku.toLowerCase().includes(term) ||
          (p.barcode && p.barcode.toLowerCase().includes(term))
        )
      }
      
      // Filter by category
      if (categoryId) {
        filteredProducts = filteredProducts.filter(p => p.categoryId === categoryId)
      }
      
      const result = filteredProducts.map(p => ({
        id: p.id,
        name: p.name,
        sku: p.sku,
        barcode: p.barcode,
        basePrice: p.basePrice,
        imageUrl: p.imageUrl,
        categoryId: p.categoryId,
        categoryName: mockCategories.find((c: any) => c.id === p.categoryId)?.name || 'Unknown',
        availableStock: p.currentStock || 100,
        isActive: true,
        isTaxable: p.isTaxable
      }))
      
      console.log('[usePosMock] fetchProducts returning:', result.length, 'products')
      return result
    } catch (error) {
      console.error('Error fetching products:', error)
      return []
    }
  }

  /**
   * Fetch product categories
   */
  const fetchCategories = async (shopId: number) => {
    try {
      // Return mock categories for testing/development
      return mockCategories.map((c: any) => ({
        id: c.id,
        name: c.name
      }))
    } catch (error) {
      console.error('Error fetching categories:', error)
      return []
    }
  }

  /**
   * Get product by barcode
   */
  const getProductByBarcode = async (barcode: string, shopId: number) => {
    try {
      // Use mock data for testing/development
      const product = getMockProductByBarcode(barcode)
      if (!product) return null
      
      return {
        id: product.id,
        name: product.name,
        sku: product.sku,
        barcode: product.barcode,
        basePrice: product.basePrice,
        categoryId: product.categoryId,
        categoryName: mockCategories.find((c: any) => c.id === product.categoryId)?.name || 'Unknown',
        availableStock: product.currentStock || 100,
        unit: product.unit,
        isTaxable: product.isTaxable
      }
    } catch (error) {
      console.error('Error fetching product by barcode:', error)
      return null
    }
  }

  /**
   * Search customer by phone or name
   */
  const fetchCustomerByPhoneOrName = async (query: string): Promise<Customer | null> => {
    // Mock implementation - in real app would call API
    const mockCustomers: Customer[] = [
      { id: 1, name: "Jabu's Spaza", phone: '+27821234567', creditLimit: 5000, creditBalance: 1200 },
      { id: 2, name: "Sipho's Tavern", phone: '+27829876543', creditLimit: 10000, creditBalance: 3500 },
      { id: 3, name: "The Gogo Shop", phone: '+27826543210', creditLimit: 3000, creditBalance: 800 }
    ]
    
    const found = mockCustomers.find(c => 
      c.phone?.includes(query) || c.name.toLowerCase().includes(query.toLowerCase())
    )
    return found || null
  }

  /**
   * Create a POS sale from cart
   */
  const createSaleFromCart = (
    sessionId: string,
    cartItems: CartItem[],
    payments: PosPaymentEntry[],
    customerId?: number,
    customerName?: string,
    notes?: string
  ): PosSale => {
    // Convert cart items to POS sale items
    const items: PosSaleItem[] = cartItems.map(item => ({
      productId: item.productId,
      productName: item.productName,
      sku: item.sku,
      barcode: item.barcode,
      quantity: item.quantity,
      unitPrice: item.unitPrice,
      discount: item.discount,
      discountType: item.discountType,
      lineTotal: calculateLineTotal(item),
      taxRate: item.taxRate,
      taxAmount: calculateLineTax(item),
      isTaxable: item.isTaxable
    }))

    // Calculate totals
    const subtotal = items.reduce((sum, item) => sum + (item.quantity * item.unitPrice - item.discount), 0)
    const totalTax = items.reduce((sum, item) => sum + (item.taxAmount || 0), 0)
    const totalDiscount = items.reduce((sum, item) => sum + item.discount, 0)
    const total = subtotal + totalTax

    // Create the sale
    const normalizedPayments = payments.map(payment => ({
      ...payment,
      mode: (payment as any).method ?? payment.mode ?? 'cash'
    }))

    const sale = MockSalesService.createPosSale({
      sessionId,
      customerId,
      customerName,
      items,
      payments: normalizedPayments,
      subtotal,
      discount: totalDiscount,
      tax: totalTax,
      total,
      notes
    })

    return sale
  }

  /**
   * Hold/park a sale for later
   */
  const holdSale = (
    cartItems: CartItem[],
    customerId?: number,
    notes?: string
  ): PosParkedSale => {
    const items: PosSaleItem[] = cartItems.map(item => ({
      productId: item.productId,
      productName: item.productName,
      sku: item.sku,
      barcode: item.barcode,
      quantity: item.quantity,
      unitPrice: item.unitPrice,
      discount: item.discount,
      discountType: item.discountType,
      lineTotal: calculateLineTotal(item),
      taxRate: item.taxRate,
      taxAmount: calculateLineTax(item),
      isTaxable: item.isTaxable
    }))

    const subtotal = items.reduce((sum, item) => sum + (item.quantity * item.unitPrice - item.discount), 0)
    const totalTax = items.reduce((sum, item) => sum + (item.taxAmount || 0), 0)
    const totalDiscount = items.reduce((sum, item) => sum + item.discount, 0)
    const total = subtotal + totalTax

    return MockSalesService.parkPosSale({
      customerId,
      items,
      subtotal,
      totalTax,
      totalDiscount,
      total,
      notes
    })
  }

  /**
   * List held/parked sales
   */
  const listHeldSales = (customerId?: number): PosParkedSale[] => {
    return MockSalesService.listPosParkedSales(customerId ? { customerId } : undefined)
  }

  /**
   * Remove/recall a parked sale
   */
  const removeParkedSale = (reference: string): void => {
    MockSalesService.removePosParkedSale(reference)
  }

  /**
   * List recent POS sales
   */
  const listRecentSales = (sessionId?: string, limit: number = 20): PosSale[] => {
    return MockSalesService.listPosSales(sessionId ? { sessionId, limit } : { limit })
  }

  /**
   * Get a specific sale by ID or reference
   */
  const getSale = (idOrRef: string): PosSale | undefined => {
    return MockSalesService.getPosSale(idOrRef)
  }

  /**
   * Record a return (simplified - creates a sales return record)
   */
  const recordReturn = async (
    saleReference: string,
    items: Array<{ productId: number; quantity: number; reason: string }>
  ) => {
    // In a real app, this would call the sales returns API
    // For now, we'll use the mock service
    const originalSale = getSale(saleReference)
    if (!originalSale) {
      throw new Error('Original sale not found')
    }

    // Create a return using the mock service
    const returnItems = items.map(item => {
      const originalItem = originalSale.items.find((i: PosSaleItem) => i.productId === item.productId)
      if (!originalItem) {
        throw new Error(`Product ${item.productId} not found in original sale`)
      }
      return {
        ...originalItem,
        quantity: item.quantity,
        returnReason: item.reason
      }
    })

    return MockSalesService.createSalesReturn({
      customerId: originalSale.customerId,
      salesInvoiceId: originalSale.id,
      returnType: 'full',
      items: returnItems.map(item => ({
        productId: item.productId,
        productName: item.productName,
        quantityReturned: item.quantity,
        unitPrice: item.unitPrice,
        lineTotal: item.quantity * item.unitPrice,
        returnReason: item.returnReason || 'Customer request'
      })),
      subtotal: returnItems.reduce((sum, item) => sum + (item.quantity * item.unitPrice), 0),
      totalTax: returnItems.reduce((sum, item) => sum + (item.taxAmount || 0), 0),
      total: returnItems.reduce((sum, item) => sum + item.lineTotal, 0),
      refundMethod: 'cash',
      reason: 'Customer return'
    })
  }

  // Helper functions
  const calculateLineTotal = (item: CartItem): number => {
    const subtotal = item.quantity * item.unitPrice
    const discount = item.discountType === 'percentage' 
      ? (subtotal * item.discount) / 100 
      : item.discount
    const afterDiscount = subtotal - discount
    const tax = item.isTaxable ? (afterDiscount * item.taxRate) / 100 : 0
    return afterDiscount + tax
  }

  const calculateLineTax = (item: CartItem): number => {
    if (!item.isTaxable) return 0
    const subtotal = item.quantity * item.unitPrice
    const discount = item.discountType === 'percentage' 
      ? (subtotal * item.discount) / 100 
      : item.discount
    const afterDiscount = subtotal - discount
    return (afterDiscount * item.taxRate) / 100
  }

  return {
    fetchProducts,
    fetchCategories,
    getProductByBarcode,
    fetchCustomerByPhoneOrName,
    createSaleFromCart,
    holdSale,
    listHeldSales,
    removeParkedSale,
    listRecentSales,
    getSale,
    recordReturn
  }
}
