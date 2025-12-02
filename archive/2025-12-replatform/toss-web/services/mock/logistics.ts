/**
 * Mock Logistics Data Service
 * Simulates delivery jobs, drivers, and tracking for community logistics
 */

export interface Driver {
  id: string
  fullName: string
  phone: string
  vehicleType: 'bakkie' | 'truck' | 'van' | 'bike'
  isAvailable: boolean
  rating: number
  completedDeliveries: number
  location?: { lat: number; lng: number }
}

export interface DeliveryJob {
  id: string
  status: 'open' | 'assigned' | 'in-transit' | 'delivered' | 'cancelled'
  pickup: {
    name: string
    address: string
    lat: number
    lng: number
  }
  dropoff: {
    name: string
    address: string
    lat: number
    lng: number
  }
  weightKg: number
  payout: number
  driverId?: string
  driverName?: string
  createdAt: Date
  assignedAt?: Date
  deliveredAt?: Date
  customerName: string
  customerPhone: string
}

export interface TrackingInfo {
  jobId: string
  status: string
  driverLocation: { lat: number; lng: number }
  estimatedArrival: Date
  lastUpdate: string
}

const mockDrivers: Driver[] = [
  { id: 'DRV-001', fullName: 'Sipho Mthembu', phone: '+27 82 555 1234', vehicleType: 'bakkie', isAvailable: true, rating: 4.8, completedDeliveries: 156, location: { lat: -26.2041, lng: 28.0473 } },
  { id: 'DRV-002', fullName: 'Thandi Nkosi', phone: '+27 83 555 2345', vehicleType: 'van', isAvailable: true, rating: 4.9, completedDeliveries: 203, location: { lat: -26.1876, lng: 28.0368 } },
  { id: 'DRV-003', fullName: 'Lucky Dube', phone: '+27 84 555 3456', vehicleType: 'bike', isAvailable: false, rating: 4.7, completedDeliveries: 89, location: { lat: -26.2175, lng: 28.0422 } }
]

const mockDeliveryJobs: DeliveryJob[] = [
  {
    id: 'JOB-001',
    status: 'open',
    pickup: { name: 'Soweto Wholesalers', address: '123 Main Rd, Soweto', lat: -26.2678, lng: 27.8585 },
    dropoff: { name: 'Mama Thandi Spaza', address: '45 Freedom Ave, Orlando East', lat: -26.2412, lng: 27.8976 },
    weightKg: 25,
    payout: 80,
    createdAt: new Date(Date.now() - 600000),
    customerName: 'Thandi Mokoena',
    customerPhone: '+27 82 999 1111'
  },
  {
    id: 'JOB-002',
    status: 'open',
    pickup: { name: 'Fresh Produce Market', address: '78 Market St, Diepsloot', lat: -25.9321, lng: 28.0109 },
    dropoff: { name: 'Kasi Corner Shop', address: '12 Community Rd, Diepsloot', lat: -25.9412, lng: 28.0156 },
    weightKg: 15,
    payout: 60,
    createdAt: new Date(Date.now() - 1200000),
    customerName: 'John Maleka',
    customerPhone: '+27 83 999 2222'
  },
  {
    id: 'JOB-003',
    status: 'assigned',
    pickup: { name: 'City Cash & Carry', address: '90 Industrial Ave, Johannesburg', lat: -26.2044, lng: 28.0456 },
    dropoff: { name: 'Alexandra Mini Market', address: '34 Main Rd, Alexandra', lat: -26.1034, lng: 28.0889 },
    weightKg: 40,
    payout: 120,
    driverId: 'DRV-001',
    driverName: 'Sipho Mthembu',
    createdAt: new Date(Date.now() - 1800000),
    assignedAt: new Date(Date.now() - 600000),
    customerName: 'Sarah Khumalo',
    customerPhone: '+27 84 999 3333'
  }
]

export class MockLogisticsService {
  static getDrivers(): Driver[] {
    return mockDrivers
  }

  static getAvailableJobs(): DeliveryJob[] {
    return mockDeliveryJobs.filter(job => job.status === 'open')
  }

  static getAssignedJobs(driverId?: string): DeliveryJob[] {
    return mockDeliveryJobs.filter(job => 
      job.status === 'assigned' && (!driverId || job.driverId === driverId)
    )
  }

  static registerDriver(driver: Omit<Driver, 'id' | 'rating' | 'completedDeliveries' | 'isAvailable'>): Driver {
    const newDriver: Driver = {
      ...driver,
      id: `DRV-${String(mockDrivers.length + 1).padStart(3, '0')}`,
      rating: 5.0,
      completedDeliveries: 0,
      isAvailable: false
    }
    mockDrivers.push(newDriver)
    return newDriver
  }

  static setDriverAvailability(driverId: string, available: boolean): boolean {
    const driver = mockDrivers.find(d => d.id === driverId)
    if (driver) {
      driver.isAvailable = available
      return true
    }
    return false
  }

  static acceptJob(jobId: string, driverId: string): DeliveryJob | null {
    const job = mockDeliveryJobs.find(j => j.id === jobId)
    const driver = mockDrivers.find(d => d.id === driverId)
    
    if (job && driver && job.status === 'open') {
      job.status = 'assigned'
      job.driverId = driverId
      job.driverName = driver.fullName
      job.assignedAt = new Date()
      driver.isAvailable = false
      return job
    }
    return null
  }

  static getJobTracking(jobId: string): TrackingInfo | null {
    const job = mockDeliveryJobs.find(j => j.id === jobId)
    if (!job || !job.driverId) return null

    const driver = mockDrivers.find(d => d.id === job.driverId)
    
    return {
      jobId: job.id,
      status: job.status,
      driverLocation: driver?.location || { lat: -26.2041, lng: 28.0473 },
      estimatedArrival: new Date(Date.now() + 1800000),
      lastUpdate: new Date().toLocaleTimeString()
    }
  }

  static markJobDelivered(jobId: string): boolean {
    const job = mockDeliveryJobs.find(j => j.id === jobId)
    if (job && job.status === 'assigned') {
      job.status = 'delivered'
      job.deliveredAt = new Date()
      
      // Update driver
      if (job.driverId) {
        const driver = mockDrivers.find(d => d.id === job.driverId)
        if (driver) {
          driver.isAvailable = true
          driver.completedDeliveries++
        }
      }
      return true
    }
    return false
  }

  static createDeliveryJob(job: Omit<DeliveryJob, 'id' | 'status' | 'createdAt'>): DeliveryJob {
    const newJob: DeliveryJob = {
      ...job,
      id: `JOB-${String(mockDeliveryJobs.length + 1).padStart(3, '0')}`,
      status: 'open',
      createdAt: new Date()
    }
    mockDeliveryJobs.unshift(newJob)
    return newJob
  }
}

