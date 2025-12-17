// GET /api/runs - List all delivery runs with filters
import { defineEventHandler, getQuery } from 'h3'
import type { SharedRun, RunFilterDto } from '../../types/logistics'

export default defineEventHandler(async (event) => {
  const query = getQuery(event) as Partial<RunFilterDto>
  
  // Mock data for MVP - replace with actual database queries
  const allRuns: SharedRun[] = [
    {
      id: 'run-1',
      tenantId: 'tenant1',
      pickupLocation: {
        address: 'Johannesburg Fresh Produce Market, City Deep',
        coordinates: { lat: -26.2418, lng: 28.0473 },
        zone: 'city-center'
      },
      dropList: [
        {
          id: 'stop-1',
          runId: 'run-1',
          shopId: 'shop-1',
          shopName: 'Thabo\'s Spaza',
          shopPhone: '+27821234567',
          location: {
            address: '123 Vilakazi Street, Soweto',
            coordinates: { lat: -26.2477, lng: 27.9089 },
            zone: 'soweto-north'
          },
          sequenceNumber: 1,
          estimatedArrival: new Date(Date.now() + 2 * 60 * 60 * 1000),
          orderId: 'pool-1',
          orderType: 'pool-participation',
          items: [
            {
              itemId: 'item-1',
              itemName: 'White Bread Loaf',
              sku: 'BREAD-001',
              quantity: 20,
              weight: 10,
              volume: 0.08
            }
          ],
          weight: 10,
          volume: 0.08,
          status: 'pending',
          feeShare: 45,
          createdAt: new Date(),
          updatedAt: new Date()
        },
        {
          id: 'stop-2',
          runId: 'run-1',
          shopId: 'shop-2',
          shopName: 'Nomsa\'s Store',
          shopPhone: '+27821234568',
          location: {
            address: '456 Mandela Avenue, Soweto',
            coordinates: { lat: -26.2520, lng: 27.9120 },
            zone: 'soweto-north'
          },
          sequenceNumber: 2,
          estimatedArrival: new Date(Date.now() + 2.5 * 60 * 60 * 1000),
          orderId: 'pool-1',
          orderType: 'pool-participation',
          items: [
            {
              itemId: 'item-1',
              itemName: 'White Bread Loaf',
              sku: 'BREAD-001',
              quantity: 15,
              weight: 7.5,
              volume: 0.06
            }
          ],
          weight: 7.5,
          volume: 0.06,
          status: 'pending',
          feeShare: 45,
          createdAt: new Date(),
          updatedAt: new Date()
        }
      ],
      totalDistance: 45.3,
      estimatedDuration: 180,
      vehicleType: 'bakkie',
      maxCapacity: 500,
      currentLoad: 17.5,
      availableSlots: 8,
      maxWeight: 500,
      currentWeight: 17.5,
      baseFee: 150,
      feePerStop: 30,
      feeSplitRule: 'by-stops',
      totalFee: 210,
      driverId: 'driver-1',
      driverName: 'Sipho Dlamini',
      driverPhone: '+27821234560',
      driverRating: 4.8,
      scheduledDate: new Date(Date.now() + 24 * 60 * 60 * 1000),
      pickupWindow: {
        start: new Date(Date.now() + 24 * 60 * 60 * 1000),
        end: new Date(Date.now() + 24 * 60 * 60 * 1000 + 2 * 60 * 60 * 1000)
      },
      status: 'scheduled',
      linkedOrderIds: ['pool-1'],
      createdAt: new Date(),
      updatedAt: new Date(),
      createdBy: 'shop-1'
    }
  ]
  
  // Apply filters
  let filteredRuns = allRuns
  
  if (query.status) {
    filteredRuns = filteredRuns.filter(r => r.status === query.status)
  }
  
  if (query.zone) {
    filteredRuns = filteredRuns.filter(r =>
      r.dropList.some((stop: any) => stop.location.zone === query.zone)
    )
  }
  
  if (query.scheduledDateFrom) {
    const fromDate = new Date(query.scheduledDateFrom)
    filteredRuns = filteredRuns.filter(r => new Date(r.scheduledDate) >= fromDate)
  }
  
  if (query.scheduledDateTo) {
    const toDate = new Date(query.scheduledDateTo)
    filteredRuns = filteredRuns.filter(r => new Date(r.scheduledDate) <= toDate)
  }
  
  if (query.driverId) {
    filteredRuns = filteredRuns.filter(r => r.driverId === query.driverId)
  }
  
  if (query.hasAvailableCapacity) {
    filteredRuns = filteredRuns.filter(r => r.availableSlots > 0)
  }
  
  if (query.minAvailableWeight) {
    filteredRuns = filteredRuns.filter(r =>
      (r.maxWeight - r.currentWeight) >= (query.minAvailableWeight || 0)
    )
  }
  
  return {
    runs: filteredRuns,
    totalCount: filteredRuns.length
  }
})

