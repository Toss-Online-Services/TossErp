import { describe, it, expect, vi, beforeEach } from 'vitest'
import { useBuyingAPI } from '~/composables/useBuyingAPI'
import { mockNuxtImport } from '@nuxt/test-utils/runtime'

const mockGet = vi.fn()
const mockPost = vi.fn()

mockNuxtImport('useApi', () => {
  return () => ({
    get: mockGet,
    post: mockPost,
  })
})

describe('useBuyingAPI', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('createPurchaseOrder should call post with the correct payload', async () => {
    const poData = {
      supplierId: 1,
      shopId: 1,
      orderDate: '2025-11-12',
      items: [{ productId: 1, quantity: 10, unitPrice: 100 }],
      totalAmount: 1000,
    }
    mockPost.mockResolvedValue({ data: { id: 123 } })

    const { createPurchaseOrder } = useBuyingAPI()
    const result = await createPurchaseOrder(poData)

    expect(mockPost).toHaveBeenCalledWith('/api/buying/purchase-orders', poData)
    expect(result).toEqual({ data: { id: 123 } })
  })

  it('getPurchaseOrderById should call get with the correct URL', async () => {
    const orderId = 456
    mockGet.mockResolvedValue({ data: { id: orderId, status: 'Approved' } })

    const { getPurchaseOrderById } = useBuyingAPI()
    const result = await getPurchaseOrderById(orderId)

    expect(mockGet).toHaveBeenCalledWith(`/api/buying/purchase-orders/${orderId}`)
    expect(result).toEqual({ data: { id: orderId, status: 'Approved' } })
  })

  it('approvePurchaseOrder should call post with the correct URL', async () => {
    const orderId = 789
    mockPost.mockResolvedValue({ data: { success: true } })

    const { approvePurchaseOrder } = useBuyingAPI()
    await approvePurchaseOrder(orderId)

    expect(mockPost).toHaveBeenCalledWith(
      `/api/buying/purchase-orders/${orderId}/approve`,
      {}
    )
  })

  it('getPurchaseOrders should call get with query parameters', async () => {
    const params = { status: 'Pending', shopId: 1 }
    mockGet.mockResolvedValue({ data: [{ id: 1 }, { id: 2 }] })

    const { getPurchaseOrders } = useBuyingAPI()
    await getPurchaseOrders(params)

    expect(mockGet).toHaveBeenCalledWith('/api/buying/purchase-orders', params)
  })
})

describe('useBuyingAPI', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('createPurchaseOrder should call post with the correct payload', async () => {
    const poData = {
      supplierId: 1,
      shopId: 1,
      orderDate: '2025-11-12',
      items: [{ productId: 1, quantity: 10, unitPrice: 100 }],
      totalAmount: 1000,
    }
    mockPost.mockResolvedValue({ data: { id: 123 } })

    const { createPurchaseOrder } = useBuyingAPI()
    const result = await createPurchaseOrder(poData)

    expect(mockPost).toHaveBeenCalledWith('/api/buying/purchase-orders', poData)
    expect(result).toEqual({ data: { id: 123 } })
  })

  it('getPurchaseOrderById should call get with the correct URL', async () => {
    const orderId = 456
    mockGet.mockResolvedValue({ data: { id: orderId, status: 'Approved' } })

    const { getPurchaseOrderById } = useBuyingAPI()
    const result = await getPurchaseOrderById(orderId)

    expect(mockGet).toHaveBeenCalledWith(`/api/buying/purchase-orders/${orderId}`)
    expect(result).toEqual({ data: { id: orderId, status: 'Approved' } })
  })

  it('approvePurchaseOrder should call post with the correct URL', async () => {
    const orderId = 789
    mockPost.mockResolvedValue({ data: { success: true } })

    const { approvePurchaseOrder } = useBuyingAPI()
    await approvePurchaseOrder(orderId)

    expect(mockPost).toHaveBeenCalledWith(
      `/api/buying/purchase-orders/${orderId}/approve`,
      {}
    )
  })

  it('getPurchaseOrders should call get with query parameters', async () => {
    const params = { status: 'Pending', shopId: 1 }
    mockGet.mockResolvedValue({ data: [{ id: 1 }, { id: 2 }] })

    const { getPurchaseOrders } = useBuyingAPI()
    await getPurchaseOrders(params)

    expect(mockGet).toHaveBeenCalledWith('/api/buying/purchase-orders', params)
  })
})
