/**
 * Mock Services Export
 * Central export for all mock data services
 */

export { MockStockService, mockStockItems, mockStockMovements, mockWarehouses } from './stock'
export { MockLogisticsService } from './logistics'
export { MockPurchasingService } from './purchasing'
export { MockSalesService } from './sales'
export { MockAutomationService } from './automation'
export { MockDashboardService } from './dashboard'

export type { StockItem, StockMovement, Warehouse } from './stock'
export type { Driver, DeliveryJob, TrackingInfo } from './logistics'
export type { Supplier, PurchaseOrder, PurchaseInvoice, GroupBuyOpportunity } from './purchasing'
export type {
  SalesOrder,
  QuotationSummary,
  QuotationDetail,
  SalesInvoice,
  POSTransaction,
  Product,
  SalesCustomer
} from './sales'
export type { Workflow, Trigger, WorkflowExecution, AIRecommendation } from './automation'
export type { DashboardMetrics, TopProduct, RecentActivity } from './dashboard'

/**
 * Check if app is in mock mode
 */
export function useMockMode() {
  const isOffline = !navigator.onLine
  const isDevelopment = process.env.NODE_ENV === 'development'
  const forceMock = process.env.ENABLE_MOCK_DATA === 'true'
  
  return isOffline || isDevelopment || forceMock
}

