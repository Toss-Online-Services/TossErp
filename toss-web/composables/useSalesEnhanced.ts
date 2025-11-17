/**
 * Enhanced Sales Composable
 * Extends the existing sales functionality with ERPNext-inspired features
 * for South African SMME context
 */
import type {
  CustomerGroup,
  CreateCustomerGroupRequest,
  UpdateCustomerGroupRequest,
  SalesTerritory,
  CreateSalesTerritoryRequest,
  CreditNote,
  CreateCreditNoteRequest,
  PromotionalScheme,
  CreatePromotionalSchemeRequest,
  SalesPartner,
  CreateSalesPartnerRequest
} from '~/types/sales'

export const useSalesEnhanced = () => {
  const config = useRuntimeConfig()
  const apiBase = config.public.apiBase || 'https://localhost:5001'
  const baseURL = `${apiBase}/api/sales`

  // ===== CUSTOMER GROUPS =====

  /**
   * Get all customer groups
   */
  const getCustomerGroups = async (): Promise<CustomerGroup[]> => {
    return await $fetch<CustomerGroup[]>(`${baseURL}/customer-groups`)
  }

  /**
   * Get customer group by ID
   */
  const getCustomerGroup = async (id: string): Promise<CustomerGroup> => {
    return await $fetch<CustomerGroup>(`${baseURL}/customer-groups/${id}`)
  }

  /**
   * Create new customer group
   */
  const createCustomerGroup = async (data: CreateCustomerGroupRequest): Promise<CustomerGroup> => {
    return await $fetch<CustomerGroup>(`${baseURL}/customer-groups`, {
      method: 'POST',
      body: data
    })
  }

  /**
   * Update customer group
   */
  const updateCustomerGroup = async (data: UpdateCustomerGroupRequest): Promise<CustomerGroup> => {
    return await $fetch<CustomerGroup>(`${baseURL}/customer-groups/${data.id}`, {
      method: 'PUT',
      body: data
    })
  }

  /**
   * Delete customer group
   */
  const deleteCustomerGroup = async (id: string): Promise<boolean> => {
    return await $fetch<boolean>(`${baseURL}/customer-groups/${id}`, {
      method: 'DELETE'
    })
  }

  /**
   * Get customer group statistics
   */
  const getCustomerGroupStats = async (id: string) => {
    return await $fetch<{
      totalCustomers: number
      totalOrders: number
      totalRevenue: number
      averageOrderValue: number
      topProducts: Array<{
        productId: string
        productName: string
        quantitySold: number
        revenue: number
      }>
    }>(`${baseURL}/customer-groups/${id}/statistics`)
  }

  // ===== SALES TERRITORIES =====

  /**
   * Get all sales territories
   */
  const getSalesTerritories = async (): Promise<SalesTerritory[]> => {
    return await $fetch<SalesTerritory[]>(`${baseURL}/territories`)
  }

  /**
   * Get sales territory by ID
   */
  const getSalesTerritory = async (id: string): Promise<SalesTerritory> => {
    return await $fetch<SalesTerritory>(`${baseURL}/territories/${id}`)
  }

  /**
   * Create new sales territory
   */
  const createSalesTerritory = async (data: CreateSalesTerritoryRequest): Promise<SalesTerritory> => {
    return await $fetch<SalesTerritory>(`${baseURL}/territories`, {
      method: 'POST',
      body: data
    })
  }

  /**
   * Update sales territory
   */
  const updateSalesTerritory = async (id: string, data: Partial<CreateSalesTerritoryRequest>): Promise<SalesTerritory> => {
    return await $fetch<SalesTerritory>(`${baseURL}/territories/${id}`, {
      method: 'PUT',
      body: data
    })
  }

  /**
   * Delete sales territory
   */
  const deleteSalesTerritory = async (id: string): Promise<boolean> => {
    return await $fetch<boolean>(`${baseURL}/territories/${id}`, {
      method: 'DELETE'
    })
  }

  /**
   * Calculate delivery charge for territory and order
   */
  const calculateDeliveryCharge = async (territoryId: string, orderAmount: number, deliveryTime?: string) => {
    return await $fetch<{
      baseCharge: number
      timeSlotCharge: number
      totalCharge: number
      freeDeliveryEligible: boolean
    }>(`${baseURL}/territories/${territoryId}/delivery-charge`, {
      method: 'POST',
      body: { orderAmount, deliveryTime }
    })
  }

  // ===== CREDIT NOTES & RETURNS =====

  /**
   * Get all credit notes
   */
  const getCreditNotes = async (params?: {
    startDate?: string
    endDate?: string
    customerId?: string
    status?: string
  }): Promise<CreditNote[]> => {
    return await $fetch<CreditNote[]>(`${baseURL}/credit-notes`, {
      params
    })
  }

  /**
   * Get credit note by ID
   */
  const getCreditNote = async (id: string): Promise<CreditNote> => {
    return await $fetch<CreditNote>(`${baseURL}/credit-notes/${id}`)
  }

  /**
   * Create new credit note
   */
  const createCreditNote = async (data: CreateCreditNoteRequest): Promise<CreditNote> => {
    return await $fetch<CreditNote>(`${baseURL}/credit-notes`, {
      method: 'POST',
      body: data
    })
  }

  /**
   * Process refund for credit note
   */
  const processRefund = async (creditNoteId: string, refundDetails: {
    refundMethod: 'cash' | 'store_credit' | 'replacement' | 'bank_refund'
    refundReference?: string
    notes?: string
  }) => {
    return await $fetch<{
      success: boolean
      refundReference: string
      refundAmount: number
    }>(`${baseURL}/credit-notes/${creditNoteId}/process-refund`, {
      method: 'POST',
      body: refundDetails
    })
  }

  // ===== PROMOTIONAL SCHEMES =====

  /**
   * Get all promotional schemes
   */
  const getPromotionalSchemes = async (activeOnly: boolean = false): Promise<PromotionalScheme[]> => {
    return await $fetch<PromotionalScheme[]>(`${baseURL}/promotional-schemes`, {
      params: { activeOnly }
    })
  }

  /**
   * Get promotional scheme by ID
   */
  const getPromotionalScheme = async (id: string): Promise<PromotionalScheme> => {
    return await $fetch<PromotionalScheme>(`${baseURL}/promotional-schemes/${id}`)
  }

  /**
   * Create new promotional scheme
   */
  const createPromotionalScheme = async (data: CreatePromotionalSchemeRequest): Promise<PromotionalScheme> => {
    return await $fetch<PromotionalScheme>(`${baseURL}/promotional-schemes`, {
      method: 'POST',
      body: data
    })
  }

  /**
   * Update promotional scheme
   */
  const updatePromotionalScheme = async (id: string, data: Partial<CreatePromotionalSchemeRequest>): Promise<PromotionalScheme> => {
    return await $fetch<PromotionalScheme>(`${baseURL}/promotional-schemes/${id}`, {
      method: 'PUT',
      body: data
    })
  }

  /**
   * Delete promotional scheme
   */
  const deletePromotionalScheme = async (id: string): Promise<boolean> => {
    return await $fetch<boolean>(`${baseURL}/promotional-schemes/${id}`, {
      method: 'DELETE'
    })
  }

  /**
   * Apply promotional scheme to order
   */
  const applyPromotionalScheme = async (schemeId: string, orderData: {
    customerId: string
    customerGroupId: string
    territoryId: string
    items: Array<{
      productId: string
      quantity: number
      unitPrice: number
    }>
    subtotal: number
    paymentMethod: string
  }) => {
    return await $fetch<{
      applicable: boolean
      discountAmount: number
      discountPercentage: number
      newSubtotal: number
      freeItems?: Array<{
        productId: string
        quantity: number
      }>
      message: string
    }>(`${baseURL}/promotional-schemes/${schemeId}/apply`, {
      method: 'POST',
      body: orderData
    })
  }

  // ===== SALES PARTNERS =====

  /**
   * Get all sales partners
   */
  const getSalesPartners = async (params?: {
    partnerType?: string
    territory?: string
    isActive?: boolean
  }): Promise<SalesPartner[]> => {
    return await $fetch<SalesPartner[]>(`${baseURL}/sales-partners`, {
      params
    })
  }

  /**
   * Get sales partner by ID
   */
  const getSalesPartner = async (id: string): Promise<SalesPartner> => {
    return await $fetch<SalesPartner>(`${baseURL}/sales-partners/${id}`)
  }

  /**
   * Create new sales partner
   */
  const createSalesPartner = async (data: CreateSalesPartnerRequest): Promise<SalesPartner> => {
    return await $fetch<SalesPartner>(`${baseURL}/sales-partners`, {
      method: 'POST',
      body: data
    })
  }

  /**
   * Update sales partner
   */
  const updateSalesPartner = async (id: string, data: Partial<CreateSalesPartnerRequest>): Promise<SalesPartner> => {
    return await $fetch<SalesPartner>(`${baseURL}/sales-partners/${id}`, {
      method: 'PUT',
      body: data
    })
  }

  /**
   * Verify sales partner
   */
  const verifySalesPartner = async (id: string, verificationNotes?: string): Promise<boolean> => {
    return await $fetch<boolean>(`${baseURL}/sales-partners/${id}/verify`, {
      method: 'POST',
      body: { verificationNotes }
    })
  }

  /**
   * Calculate partner commission
   */
  const calculatePartnerCommission = async (partnerId: string, transactionAmount: number) => {
    return await $fetch<{
      commissionAmount: number
      commissionRate: number
      paymentDue: string
    }>(`${baseURL}/sales-partners/${partnerId}/commission`, {
      method: 'POST',
      body: { transactionAmount }
    })
  }

  // ===== PRICING RULES =====

  /**
   * Get applicable pricing rules for customer and items
   */
  const getApplicablePricingRules = async (customerId: string, items: Array<{
    productId: string
    quantity: number
  }>) => {
    return await $fetch<Array<{
      ruleId: string
      ruleName: string
      productId: string
      originalPrice: number
      discountedPrice: number
      discountPercentage: number
      discountAmount: number
      savings: number
    }>>(`${baseURL}/pricing-rules/applicable`, {
      method: 'POST',
      body: { customerId, items }
    })
  }

  /**
   * Calculate order totals with all applicable discounts
   */
  const calculateOrderTotals = async (orderData: {
    customerId: string
    customerGroupId: string
    territoryId: string
    items: Array<{
      productId: string
      quantity: number
      unitPrice: number
    }>
    paymentMethod: string
    promotionalSchemes?: string[]
  }) => {
    return await $fetch<{
      subtotal: number
      totalDiscounts: number
      totalPromotions: number
      deliveryCharge: number
      vatAmount: number
      grandTotal: number
      totalSavings: number
      appliedDiscounts: Array<{
        type: 'pricing_rule' | 'promotional_scheme' | 'customer_group' | 'payment_method'
        name: string
        amount: number
      }>
      freeItems?: Array<{
        productId: string
        productName: string
        quantity: number
      }>
    }>(`${baseURL}/orders/calculate-totals`, {
      method: 'POST',
      body: orderData
    })
  }

  // ===== ANALYTICS & REPORTS =====

  /**
   * Get enhanced sales analytics
   */
  const getSalesAnalytics = async (params: {
    startDate: string
    endDate: string
    groupBy?: 'day' | 'week' | 'month'
    territoryId?: string
    customerGroupId?: string
    productCategoryId?: string
  }) => {
    return await $fetch<{
      totalRevenue: number
      totalOrders: number
      averageOrderValue: number
      topCustomerGroups: Array<{
        groupId: string
        groupName: string
        revenue: number
        orders: number
      }>
      topTerritories: Array<{
        territoryId: string
        territoryName: string
        revenue: number
        orders: number
      }>
      topProducts: Array<{
        productId: string
        productName: string
        quantitySold: number
        revenue: number
      }>
      salesTrends: Array<{
        period: string
        revenue: number
        orders: number
        averageValue: number
      }>
      promotionalImpact: Array<{
        schemeId: string
        schemeName: string
        timesUsed: number
        totalSavings: number
        incrementalRevenue: number
      }>
    }>(`${baseURL}/analytics/enhanced`, {
      params
    })
  }

  return {
    // Customer Groups
    getCustomerGroups,
    getCustomerGroup,
    createCustomerGroup,
    updateCustomerGroup,
    deleteCustomerGroup,
    getCustomerGroupStats,

    // Sales Territories
    getSalesTerritories,
    getSalesTerritory,
    createSalesTerritory,
    updateSalesTerritory,
    deleteSalesTerritory,
    calculateDeliveryCharge,

    // Credit Notes & Returns
    getCreditNotes,
    getCreditNote,
    createCreditNote,
    processRefund,

    // Promotional Schemes
    getPromotionalSchemes,
    getPromotionalScheme,
    createPromotionalScheme,
    updatePromotionalScheme,
    deletePromotionalScheme,
    applyPromotionalScheme,

    // Sales Partners
    getSalesPartners,
    getSalesPartner,
    createSalesPartner,
    updateSalesPartner,
    verifySalesPartner,
    calculatePartnerCommission,

    // Pricing & Calculations
    getApplicablePricingRules,
    calculateOrderTotals,

    // Analytics
    getSalesAnalytics,
  }
}