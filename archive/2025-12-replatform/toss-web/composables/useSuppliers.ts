/**
 * Suppliers API Composable
 * Connects to TOSS backend /api/suppliers endpoints
 */
export const useSuppliers = () => {
  const { get, post, put } = useApi()

  /**
   * Get all suppliers with pagination
   */
  const getSuppliers = async (params?: {
    searchTerm?: string
    isActive?: boolean
    pageNumber?: number
    pageSize?: number
  }) => {
    return await get<any>('/api/suppliers', params)
  }

  /**
   * Get supplier by ID
   */
  const getSupplierById = async (id: number) => {
    return await get<any>(`/api/suppliers/${id}`)
  }

  /**
   * Create a new supplier
   */
  const createSupplier = async (supplierData: {
    name: string
    contactPerson?: string
    email?: string
    phone?: string
    address?: string
    isActive: boolean
  }) => {
    return await post<{ id: number }>('/api/suppliers', supplierData)
  }

  /**
   * Get products for a supplier
   */
  const getSupplierProducts = async (supplierId: number) => {
    return await get<any>(`/api/suppliers/${supplierId}/products`)
  }

  /**
   * Link a product to a supplier with pricing
   */
  const linkSupplierProduct = async (supplierId: number, productData: {
    productId: number
    supplierSku?: string
    unitPrice: number
    minOrderQuantity?: number
    leadTimeDays?: number
  }) => {
    return await post<{ id: number }>(`/api/suppliers/${supplierId}/products`, productData)
  }

  /**
   * Update supplier product pricing
   */
  const updateSupplierPricing = async (supplierProductId: number, pricingData: {
    unitPrice: number
    minOrderQuantity?: number
    leadTimeDays?: number
  }) => {
    return await put<{ id: number }>(`/api/suppliers/products/${supplierProductId}/pricing`, pricingData)
  }

  return {
    getSuppliers,
    getSupplierById,
    createSupplier,
    getSupplierProducts,
    linkSupplierProduct,
    updateSupplierPricing
  }
}

