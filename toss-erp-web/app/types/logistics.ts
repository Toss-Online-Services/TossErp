// Logistics & Delivery Types for TOSS

export interface Location {
  address: string
  coordinates: {
    lat: number
    lon: number
  }
  landmark?: string
  whatsappPin?: string
}

export interface DeliveryStop {
  id: string
  runId: string
  shopId: string
  shopName: string
  shopPhone?: string
  location: Location
  sequenceNumber: number
  estimatedArrival: Date
  actualArrival?: Date
  orderId: string
  orderType: 'purchase-order' | 'sales-order'
  items: DeliveryItem[]
  weight: number
  volume: number
  status: 'pending' | 'in-transit' | 'delivered' | 'failed'
  feeShare: number
  notes?: string
  createdAt: Date
  updatedAt: Date
}

export interface DeliveryItem {
  productId: string
  productName: string
  quantity: number
  unit: string
}

export interface DeliveryRun {
  id: string
  driverId: string
  driverName: string
  driverPhone: string
  vehicleType: 'bakkie' | 'van' | 'truck'
  vehicleRegistration?: string
  pickupLocation: Location
  stops: DeliveryStop[]
  status: 'planned' | 'in-progress' | 'completed' | 'cancelled'
  totalDistance: number
  totalCost: number
  scheduledDate: string
  startedAt?: string
  completedAt?: string
  createdAt: string
  updatedAt: string
}

export interface RouteOptimizationRequest {
  pickupLocation: Location
  stops: Array<{
    location: Location
    priority?: number
    timeWindow?: {
      start: Date
      end: Date
    }
  }>
  vehicleType?: 'bakkie' | 'van' | 'truck'
  maxDistance?: number
  maxStops?: number
}

export interface RouteOptimizationResponse {
  optimizedStops: Array<{
    location: Location
    sequenceNumber: number
    estimatedArrival: Date
    distanceFromPrevious: number
  }>
  totalDistance: number
  totalTime: number
  estimatedCost: number
}

export interface DeliveryCostSplit {
  runId: string
  totalCost: number
  totalDistance: number
  stops: Array<{
    shopId: string
    shopName: string
    distance: number
    feeShare: number
    percentage: number
  }>
}
