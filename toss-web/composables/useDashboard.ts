/**
 * Dashboard/Analytics API Composable
 * Connects to TOSS backend /api/dashboard endpoints
 */
export const useDashboard = () => {
  const { get } = useApi()

  /**
   * Get comprehensive dashboard summary
   */
  const getDashboardSummary = async (shopId: number) => {
    return await get<{
      todaySales: number
      todayRevenue: number
      lowStockCount: number
      pendingOrders: number
      activePoolsCount: number
      completedDeliveries: number
    }>('/api/dashboard/summary', { shopId })
  }

  /**
   * Get sales trends for charts
   */
  const getSalesTrends = async (params: {
    shopId: number
    days?: number
    startDate?: string
    endDate?: string
  }) => {
    return await get<any>('/api/dashboard/sales-trends', params)
  }

  /**
   * Get top selling products
   */
  const getTopProducts = async (params: {
    shopId: number
    limit?: number
    period?: string
  }) => {
    return await get<any>('/api/dashboard/top-products', params)
  }

  /**
   * Get cash flow summary
   */
  const getCashFlowSummary = async (params: {
    shopId: number
    startDate?: string
    endDate?: string
  }) => {
    return await get<{
      totalInflow: number
      totalOutflow: number
      netCashFlow: number
      projectedCashFlow: number
    }>('/api/dashboard/cash-flow', params)
  }

  return {
    getDashboardSummary,
    getSalesTrends,
    getTopProducts,
    getCashFlowSummary
  }
}
