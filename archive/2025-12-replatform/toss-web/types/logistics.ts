// Shared Logistics Type Definitions for TOSS
// Resource-sharing delivery system for multi-drop routes

export type RunStatus = 'scheduled' | 'out-for-delivery' | 'completed' | 'cancelled'
export type StopStatus = 'pending' | 'out-for-delivery' | 'delivered' | 'failed'
export type VehicleType = 'bakkie' | 'truck' | 'van' | 'motorcycle'
export type PODType = 'pin' | 'photo' | 'signature'
export type FeeSplitRule = 'by-stops' | 'by-weight' | 'by-distance' | 'flat'

export interface Location {
  address: string
  coordinates: { lat: number; lng: number }
  zone: string // e.g., "soweto-north", "alexandra-central"
  landmark?: string // Helpful for drivers
}

export interface TimeWindow {
  start: Date
  end: Date
}

export interface SharedRun {
  id: string
  tenantId: string
  
  // Route Details
  pickupLocation: Location
  dropList: DeliveryStop[]
  totalDistance: number // km
  estimatedDuration: number // minutes
  actualDistance?: number // Recorded after completion
  actualDuration?: number // Recorded after completion
  
  // Capacity
  vehicleType: VehicleType
  vehicleRegistration?: string
  maxCapacity: number // pallets or cubic meters
  currentLoad: number
  availableSlots: number
  maxWeight: number // kg
  currentWeight: number
  
  // Pricing
  baseFee: number // Base delivery fee for the entire run
  feePerStop: number
  feePerKm?: number
  feePerKg?: number
  feeSplitRule: FeeSplitRule
  totalFee: number // Calculated based on rule
  
  // Driver & Schedule
  driverId: string
  driverName: string
  driverPhone: string
  driverRating: number
  driverPhotoUrl?: string
  scheduledDate: Date
  pickupWindow: TimeWindow
  
  // State
  status: RunStatus
  startedAt?: Date
  completedAt?: Date
  cancelledAt?: Date
  cancellationReason?: string
  
  // Integration
  linkedOrderIds: string[] // POs or Pool IDs this run will deliver
  
  // Metadata
  createdAt: Date
  updatedAt: Date
  createdBy: string // Shop ID that initiated the run
  
  // Notes
  driverNotes?: string
  internalNotes?: string
}

export interface DeliveryStop {
  id: string
  runId: string
  shopId: string
  shopName: string
  shopPhone: string
  location: Location
  sequenceNumber: number // Order in route
  estimatedArrival: Date
  actualArrival?: Date
  
  // Delivery Details
  orderId: string // PO or Pool participation ID
  orderType: 'purchase-order' | 'pool-participation'
  items: DeliveryItem[]
  weight: number // kg
  volume: number // cubic meters
  specialInstructions?: string
  
  // Completion
  status: StopStatus
  deliveredAt?: Date
  failedAt?: Date
  failureReason?: string
  proofOfDelivery?: POD
  feeShare: number // This shop's share of delivery cost
  
  // Customer interaction
  contactName?: string
  contactPhone?: string
  signatureName?: string
  
  // Metadata
  createdAt: Date
  updatedAt: Date
}

export interface DeliveryItem {
  itemId: string
  itemName: string
  sku: string
  quantity: number
  weight: number // kg per unit
  volume: number // cubic meters per unit
  notes?: string
  barcodes?: string[]
}

export interface POD {
  id: string
  stopId: string
  type: PODType
  value: string // PIN code, photo URL, or signature data URL
  capturedAt: Date
  capturedBy: string // Driver ID
  latitude?: number
  longitude?: number
  deviceInfo?: string
  notes?: string
}

// DTOs for API requests/responses

export interface CreateSharedRunDto {
  pickupLocation: Location
  scheduledDate: Date
  pickupWindow: TimeWindow
  vehicleType: VehicleType
  vehicleRegistration?: string
  maxCapacity: number
  maxWeight: number
  baseFee: number
  feePerStop: number
  feePerKm?: number
  feePerKg?: number
  feeSplitRule: FeeSplitRule
  driverId?: string // Optional if driver assigns themselves later
  stops?: CreateDeliveryStopDto[] // Can create run with stops or add later
  notes?: string
}

export interface CreateDeliveryStopDto {
  shopId: string
  location: Location
  orderId: string
  orderType: 'purchase-order' | 'pool-participation'
  items: DeliveryItem[]
  weight: number
  volume: number
  estimatedArrival?: Date
  specialInstructions?: string
  contactName?: string
  contactPhone?: string
}

export interface UpdateRunDto {
  scheduledDate?: Date
  pickupWindow?: TimeWindow
  baseFee?: number
  feePerStop?: number
  status?: RunStatus
  driverNotes?: string
}

export interface AddStopToRunDto {
  runId: string
  stop: CreateDeliveryStopDto
}

export interface RemoveStopFromRunDto {
  runId: string
  stopId: string
  reason?: string
}

export interface CapturePODDto {
  stopId: string
  type: PODType
  value: string // PIN code, base64 image, or signature data
  latitude?: number
  longitude?: number
  notes?: string
}

export interface RunFilterDto {
  status?: RunStatus
  zone?: string
  scheduledDateFrom?: Date
  scheduledDateTo?: Date
  driverId?: string
  hasAvailableCapacity?: boolean
  minAvailableWeight?: number
}

export interface RouteOptimizationRequest {
  pickupLocation: Location
  stops: Array<{
    location: Location
    priority?: number // Higher priority stops first
    timeWindow?: TimeWindow
  }>
  vehicleType: VehicleType
  considerTraffic?: boolean
}

export interface RouteOptimizationResponse {
  optimizedStops: Array<{
    location: Location
    sequenceNumber: number
    estimatedArrival: Date
    distanceFromPrevious: number
  }>
  totalDistance: number
  estimatedDuration: number
  fuelCost?: number
  recommendations?: string[]
}

// Cost calculation

export interface DeliveryCostBreakdown {
  runId: string
  totalFee: number
  baseFee: number
  distanceFee: number
  weightFee: number
  stopCount: number
  perStopFee: number
  feeSplitRule: FeeSplitRule
  stops: Array<{
    stopId: string
    shopName: string
    feeShare: number
    reasoning: string // Explain how fee was calculated
  }>
}

export interface DeliverySavingsCalculation {
  shopId: string
  orderId: string
  soloDeliveryCost: number // What shop would pay for solo delivery
  sharedDeliveryCost: number // What shop pays in shared run
  savings: number
  savingsPercentage: number
  coShoppers: number // Number of other shops in the run
}

// Analytics

export interface LogisticsAnalytics {
  totalRuns: number
  completedRuns: number
  cancelledRuns: number
  onTimeDeliveryRate: number // Percentage delivered within estimated window
  avgStopsPerRun: number
  avgDistancePerRun: number
  totalDistanceSaved: number // vs solo deliveries
  totalCostSavings: number // vs solo deliveries
  avgDriverRating: number
  topDrivers: { driverId: string; driverName: string; completedRuns: number; rating: number }[]
  busyZones: { zone: string; runCount: number }[]
}

export interface DriverStats {
  driverId: string
  completedRuns: number
  cancelledRuns: number
  totalDistance: number
  totalEarnings: number
  avgRating: number
  onTimeRate: number
  totalStops: number
  avgStopsPerRun: number
  reliabilityScore: number
}

