import { describe, it, expect } from 'vitest'
import { useCartMath } from '~/composables/useCartMath'
import type { CartLine, CartPaymentResult } from '~/composables/useCartMath'

describe('useCartMath', () => {
  const cartMath = useCartMath()

  describe('calculateLineSubtotal', () => {
    it('calculates correct subtotal for positive values', () => {
      const line: CartLine = {
        productId: 1,
        name: 'Test Product',
        quantity: 3,
        unitPrice: 10.50
      }
      expect(cartMath.calculateLineSubtotal(line)).toBe(31.50)
    })

    it('handles zero quantity', () => {
      const line: CartLine = {
        productId: 1,
        name: 'Test Product',
        quantity: 0,
        unitPrice: 10.50
      }
      expect(cartMath.calculateLineSubtotal(line)).toBe(0)
    })
  
    it('returns zero for negative quantity', () => {
      const line: CartLine = {
        productId: 1,
        name: 'Test Product',
        quantity: -2,
        unitPrice: 10.50
      }
      expect(cartMath.calculateLineSubtotal(line)).toBe(0)
    })

    it('returns zero for negative price', () => {
      const line: CartLine = {
        productId: 1,
        name: 'Test Product',
        quantity: 2,
        unitPrice: -10.50
      }
      expect(cartMath.calculateLineSubtotal(line)).toBe(0)
    })
  })

  describe('calculateLineDiscount', () => {
    it('calculates percentage discount correctly', () => {
      const line: CartLine = {
        productId: 1,
        name: 'Test Product',
        quantity: 2,
        unitPrice: 100,
        discountPercent: 10
      }
      expect(cartMath.calculateLineDiscount(line)).toBe(20)
    })

    it('calculates fixed amount discount correctly', () => {
      const line: CartLine = {
        productId: 1,
        name: 'Test Product',
        quantity: 2,
        unitPrice: 100,
        discountAmount: 30
      }
      expect(cartMath.calculateLineDiscount(line)).toBe(30)
    })

    it('caps discount at subtotal for fixed amount', () => {
      const line: CartLine = {
        productId: 1,
        name: 'Test Product',
        quantity: 2,
        unitPrice: 100,
        discountAmount: 250
      }
      expect(cartMath.calculateLineDiscount(line)).toBe(200)
    })

    it('caps percentage discount at 100%', () => {
      const line: CartLine = {
        productId: 1,
        name: 'Test Product',
        quantity: 2,
        unitPrice: 100,
        discountPercent: 150
      }
      expect(cartMath.calculateLineDiscount(line)).toBe(200)
    })

    it('returns zero when no discount specified', () => {
      const line: CartLine = {
        productId: 1,
        name: 'Test Product',
        quantity: 2,
        unitPrice: 100
      }
      expect(cartMath.calculateLineDiscount(line)).toBe(0)
    })

    it('prefers fixed amount over percentage when both present', () => {
      const line: CartLine = {
        productId: 1,
        name: 'Test Product',
        quantity: 2,
        unitPrice: 100,
        discountPercent: 10,
        discountAmount: 30
      }
      expect(cartMath.calculateLineDiscount(line)).toBe(30)
    })
  })

  describe('calculateLineTax', () => {
    it('calculates tax on discounted amount', () => {
      const line: CartLine = {
        productId: 1,
        name: 'Test Product',
        quantity: 2,
        unitPrice: 100,
        discountPercent: 10,
        taxRate: 15
      }
      // Subtotal: 200, Discount: 20, Taxable: 180, Tax: 27
      expect(cartMath.calculateLineTax(line)).toBe(27)
    })

    it('calculates tax without discount', () => {
      const line: CartLine = {
        productId: 1,
        name: 'Test Product',
        quantity: 2,
        unitPrice: 100,
        taxRate: 15
      }
      // Subtotal: 200, Tax: 30
      expect(cartMath.calculateLineTax(line)).toBe(30)
    })

    it('returns zero when taxRate is zero', () => {
      const line: CartLine = {
        productId: 1,
        name: 'Test Product',
        quantity: 2,
        unitPrice: 100,
        taxRate: 0
      }
      expect(cartMath.calculateLineTax(line)).toBe(0)
    })

    it('returns zero when taxRate is not specified', () => {
      const line: CartLine = {
        productId: 1,
        name: 'Test Product',
        quantity: 2,
        unitPrice: 100
      }
      expect(cartMath.calculateLineTax(line)).toBe(0)
    })
  })

  describe('calculateLineTotal', () => {
    it('calculates complete line total with discount and tax', () => {
      const line: CartLine = {
        productId: 1,
        name: 'Test Product',
        quantity: 2,
        unitPrice: 100,
        discountPercent: 10,
        taxRate: 15
      }
      // Subtotal: 200, Discount: 20, Taxable: 180, Tax: 27, Total: 207
      expect(cartMath.calculateLineTotal(line)).toBe(207)
    })

    it('calculates total with only tax', () => {
      const line: CartLine = {
        productId: 1,
        name: 'Test Product',
        quantity: 2,
        unitPrice: 100,
        taxRate: 15
      }
      // Subtotal: 200, Tax: 30, Total: 230
      expect(cartMath.calculateLineTotal(line)).toBe(230)
    })

    it('calculates total with only discount', () => {
      const line: CartLine = {
        productId: 1,
        name: 'Test Product',
        quantity: 2,
        unitPrice: 100,
        discountPercent: 10
      }
      // Subtotal: 200, Discount: 20, Total: 180
      expect(cartMath.calculateLineTotal(line)).toBe(180)
    })

    it('handles zero total correctly', () => {
      const line: CartLine = {
        productId: 1,
        name: 'Test Product',
        quantity: 2,
        unitPrice: 100,
        discountPercent: 100
      }
      expect(cartMath.calculateLineTotal(line)).toBe(0)
    })
  })

  describe('calculateCartTotals', () => {
    it('aggregates multiple line totals correctly', () => {
      const lines: CartLine[] = [
        {
          productId: 1,
          name: 'Product A',
          quantity: 2,
          unitPrice: 100,
          discountPercent: 10,
          taxRate: 15
        },
        {
          productId: 2,
          name: 'Product B',
          quantity: 1,
          unitPrice: 50,
          taxRate: 15
        }
      ]

      const totals = cartMath.calculateCartTotals(lines)
      
      // Line 1: Subtotal 200, Discount 20, Tax 27, Total 207
      // Line 2: Subtotal 50, Discount 0, Tax 7.5, Total 57.5
      // Cart: Subtotal 250, Discount 20, Tax 34.5, Total 264.5
      
      expect(totals.subtotal).toBe(250)
      expect(totals.discountTotal).toBe(20)
      expect(totals.taxTotal).toBe(34.5)
      expect(totals.grandTotal).toBe(264.5)
    })

    it('handles empty cart', () => {
      const totals = cartMath.calculateCartTotals([])
      
      expect(totals.subtotal).toBe(0)
      expect(totals.discountTotal).toBe(0)
      expect(totals.taxTotal).toBe(0)
      expect(totals.grandTotal).toBe(0)
    })

    it('handles cart with mixed discount types', () => {
      const lines: CartLine[] = [
        {
          productId: 1,
          name: 'Product A',
          quantity: 1,
          unitPrice: 100,
          discountPercent: 10,
          taxRate: 15
        },
        {
          productId: 2,
          name: 'Product B',
          quantity: 1,
          unitPrice: 100,
          discountAmount: 20,
          taxRate: 15
        }
      ]

      const totals = cartMath.calculateCartTotals(lines)
      
      // Line 1: Discount 10, Tax 13.5, Total 103.5
      // Line 2: Discount 20, Tax 12, Total 92
      // Cart: Subtotal 200, Discount 30, Tax 25.5, Total 195.5
      
      expect(totals.subtotal).toBe(200)
      expect(totals.discountTotal).toBe(30)
      expect(totals.taxTotal).toBe(25.5)
      expect(totals.grandTotal).toBe(195.5)
    })
  })

  describe('calculateChange', () => {
    it('calculates correct change when payment is sufficient', () => {
      const result = cartMath.calculateChange(100, 150)
      
      expect(result.paid).toBe(150)
      expect(result.change).toBe(50)
      expect(result.insufficient).toBe(false)
      expect(result.shortfall).toBe(0)
    })

    it('calculates zero change when payment is exact', () => {
      const result = cartMath.calculateChange(100, 100)
      
      expect(result.paid).toBe(100)
      expect(result.change).toBe(0)
      expect(result.insufficient).toBe(false)
      expect(result.shortfall).toBe(0)
    })

    it('identifies insufficient payment', () => {
      const result = cartMath.calculateChange(100, 80)
      
      expect(result.paid).toBe(80)
      expect(result.change).toBe(0)
      expect(result.insufficient).toBe(true)
      expect(result.shortfall).toBe(20)
    })

    it('handles decimal amounts correctly', () => {
      const result = cartMath.calculateChange(99.99, 100)
      
      expect(result.paid).toBe(100)
      expect(result.change).toBe(0.01)
      expect(result.insufficient).toBe(false)
      expect(result.shortfall).toBe(0)
    })

    it('rounds to ZAR precision', () => {
      const result = cartMath.calculateChange(10.555, 20)
      
      expect(result.paid).toBe(20)
      expect(result.change).toBe(9.44)
      expect(result.insufficient).toBe(false)
    })
  })

  describe('roundToZAR', () => {
    it('rounds to 2 decimal places', () => {
      expect(cartMath.roundToZAR(10.555)).toBe(10.56)
      expect(cartMath.roundToZAR(10.554)).toBe(10.55)
      expect(cartMath.roundToZAR(10.125)).toBe(10.13)
    })

    it('handles whole numbers', () => {
      expect(cartMath.roundToZAR(100)).toBe(100)
    })

    it('handles negative numbers', () => {
      expect(cartMath.roundToZAR(-10.555)).toBe(-10.55) // JS Math.round floors negatives
    })
  })

  describe('formatCurrency', () => {
    it('formats ZAR with 2 decimals', () => {
      const formatted = cartMath.formatCurrency(1234.5)
      // Should be "1 234.50" or "1,234.50" depending on locale
      expect(formatted).toMatch(/1[\s,]234[.,]50/)
    })

    it('formats zero correctly', () => {
      const formatted = cartMath.formatCurrency(0)
      expect(formatted).toMatch(/0[.,]00/)
    })

    it('formats negative amounts', () => {
      const formatted = cartMath.formatCurrency(-50.25)
      expect(formatted).toMatch(/-50[.,]25/)
    })
  })

  describe('applyStandardVAT', () => {
    it('applies 15% VAT to lines without taxRate', () => {
      const lines: CartLine[] = [
        {
          productId: 1,
          name: 'Product A',
          quantity: 1,
          unitPrice: 100
        },
        {
          productId: 2,
          name: 'Product B',
          quantity: 1,
          unitPrice: 50
        }
      ]

      const result = cartMath.applyStandardVAT(lines)
      
      expect(result[0]?.taxRate).toBe(15)
      expect(result[1]?.taxRate).toBe(15)
    })

    it('preserves existing taxRate when specified', () => {
      const lines: CartLine[] = [
        {
          productId: 1,
          name: 'Product A',
          quantity: 1,
          unitPrice: 100,
          taxRate: 0
        }
      ]

      const result = cartMath.applyStandardVAT(lines)
      
      expect(result[0]?.taxRate).toBe(0)
    })

    it('handles empty array', () => {
      const result = cartMath.applyStandardVAT([])
      expect(result).toEqual([])
    })
  })

  describe('Edge cases and real-world scenarios', () => {
    it('handles typical spaza shop transaction', () => {
      const lines: CartLine[] = [
        {
          productId: 'bread-001',
          name: 'Albany White Bread',
          quantity: 2,
          unitPrice: 15.99,
          taxRate: 0 // Zero-rated food item
        },
        {
          productId: 'airtime-001',
          name: 'MTN Airtime R30',
          quantity: 1,
          unitPrice: 30,
          taxRate: 0 // Airtime zero-rated
        },
        {
          productId: 'cooldrink-001',
          name: 'Coca-Cola 2L',
          quantity: 3,
          unitPrice: 19.99,
          taxRate: 15
        }
      ]

      const totals = cartMath.calculateCartTotals(lines)
      
      // Bread: 31.98 (no tax)
      // Airtime: 30 (no tax)
      // Cooldrink: 59.97 + 8.9955 tax = 68.97
      // Total: 130.95
      
      expect(totals.subtotal).toBe(121.95)
      expect(totals.taxTotal).toBeCloseTo(9, 0) // Approx 9 ZAR VAT
      expect(totals.grandTotal).toBeCloseTo(130.95, 1)
    })

    it('handles bulk discount scenario', () => {
      const lines: CartLine[] = [
        {
          productId: 'sugar-001',
          name: 'White Sugar 2.5kg',
          quantity: 10,
          unitPrice: 45,
          discountPercent: 5, // Bulk discount
          taxRate: 0
        }
      ]

      const totals = cartMath.calculateCartTotals(lines)
      
      // Subtotal: 450, Discount: 22.5, Total: 427.5
      expect(totals.subtotal).toBe(450)
      expect(totals.discountTotal).toBe(22.5)
      expect(totals.grandTotal).toBe(427.5)
    })

    it('handles return transaction with negative quantity', () => {
      const lines: CartLine[] = [
        {
          productId: 'chips-001',
          name: 'Simba Chips',
          quantity: -2, // Return
          unitPrice: 12.50,
          taxRate: 15
        }
      ]

      // Negative quantity should result in zero per current implementation
      const totals = cartMath.calculateCartTotals(lines)
      expect(totals.grandTotal).toBe(0)
    })
  })
})
