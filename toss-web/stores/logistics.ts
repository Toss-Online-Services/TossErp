import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

export interface Driver {
  id: string
  firstName: string
  lastName: string
  fullName: string
  phone: string
  email?: string
  licenseNumber?: string
  vehicleType?: string
  vehicleRegistration?: string
  isActive: boolean
  isAvailable: boolean
  areas?: string[] // Delivery areas
  notes?: string
  createdAt: Date
  updatedAt: Date
}

export type DeliveryStatus = 'scheduled' | 'assigned' | 'in_transit' | 'completed' | 'cancelled'

export interface DeliveryStop {
  id: string
  shopId: string
  shopName: string
  sequenceNumber: number
  address: string
  deliveryInstructions?: string
  costShare: number
  arrivalTime?: Date
  completionTime?: Date
  status: DeliveryStatus
  purchaseOrderId?: string
  purchaseOrderNumber?: string
  notes?: string
}

export interface DeliveryRun {
  id: string
  runNumber: string
  driverId?: string
  driverName?: string
  scheduledDate: Date
  startedAt?: Date
  completedAt?: Date
  assignedDate?: Date
  status: DeliveryStatus
  areaGroup?: string
  totalDeliveryCost: number
  totalDistance: number
  participantCount: number
  costPerStop: number
  notes?: string
  stops: DeliveryStop[]
  createdAt: Date
  updatedAt: Date
}

export interface Route {
  id: string
  routeName: string
  driverId?: string
  driverName?: string
  date: Date
  status: 'planned' | 'active' | 'completed' | 'cancelled'
  totalStops: number
  completedStops: number
  totalDistance: number
  estimatedDuration: number
  deliveryRunIds: string[]
  optimized: boolean
  notes?: string
  createdAt: Date
  updatedAt: Date
}

export const useLogisticsStore = defineStore('logistics', () => {
  // State
  const drivers = ref<Driver[]>([])
  const deliveryRuns = ref<DeliveryRun[]>([])
  const routes = ref<Route[]>([])
  const loading = ref(false)

  // Computed
  const activeDrivers = computed(() => {
    return drivers.value.filter(d => d.isActive)
  })

  const availableDrivers = computed(() => {
    return activeDrivers.value.filter(d => d.isAvailable)
  })

  const activeDeliveryRuns = computed(() => {
    return deliveryRuns.value.filter(run => 
      run.status === 'scheduled' || run.status === 'assigned' || run.status === 'in_transit'
    )
  })

  const completedDeliveryRuns = computed(() => {
    return deliveryRuns.value.filter(run => run.status === 'completed')
  })

  const activeRoutes = computed(() => {
    return routes.value.filter(r => r.status === 'active' || r.status === 'planned')
  })

  // Actions
  async function fetchDrivers() {
    loading.value = true
    try {
      // TODO: Replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 500))
      
      // Mock data
      drivers.value = [
        {
          id: '1',
          firstName: 'Thabo',
          lastName: 'Mthembu',
          fullName: 'Thabo Mthembu',
          phone: '+27 82 123 4567',
          email: 'thabo@example.com',
          licenseNumber: 'DL123456',
          vehicleType: 'Bakkie',
          vehicleRegistration: 'ABC 123 GP',
          isActive: true,
          isAvailable: true,
          areas: ['Soweto', 'Alexandra', 'Sandton'],
          notes: 'Experienced driver, specializes in township deliveries',
          createdAt: new Date('2024-01-01'),
          updatedAt: new Date('2024-01-15')
        },
        {
          id: '2',
          firstName: 'Sipho',
          lastName: 'Ndlovu',
          fullName: 'Sipho Ndlovu',
          phone: '+27 83 234 5678',
          licenseNumber: 'DL234567',
          vehicleType: 'Van',
          vehicleRegistration: 'XYZ 789 GP',
          isActive: true,
          isAvailable: false,
          areas: ['Johannesburg CBD', 'Rosebank'],
          createdAt: new Date('2024-01-05'),
          updatedAt: new Date('2024-01-20')
        }
      ]
    } catch (error) {
      console.error('Failed to fetch drivers:', error)
    } finally {
      loading.value = false
    }
  }

  async function fetchDeliveryRuns() {
    loading.value = true
    try {
      // TODO: Replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 500))
      
      // Mock data
      deliveryRuns.value = [
        {
          id: '1',
          runNumber: 'DR-2024-001',
          driverId: '1',
          driverName: 'Thabo Mthembu',
          scheduledDate: new Date('2024-01-25'),
          assignedDate: new Date('2024-01-24'),
          startedAt: new Date('2024-01-25'),
          completedAt: new Date('2024-01-25'),
          status: 'completed',
          areaGroup: 'Soweto',
          totalDeliveryCost: 850,
          totalDistance: 45.5,
          participantCount: 5,
          costPerStop: 170,
          stops: [
            {
              id: '1',
              shopId: '1',
              shopName: 'Spaza Shop A',
              sequenceNumber: 1,
              address: '123 Main St, Soweto',
              costShare: 170,
              status: 'completed',
              arrivalTime: new Date('2024-01-25T10:00:00'),
              completionTime: new Date('2024-01-25T10:30:00'),
              purchaseOrderId: '1',
              purchaseOrderNumber: 'PO-2024-001',
              rating: 5,
              reviewComment: 'Excellent delivery, very professional and on time!',
              reviewDate: new Date('2024-01-25')
            } as any,
            {
              id: '2',
              shopId: '2',
              shopName: 'Spaza Shop B',
              sequenceNumber: 2,
              address: '456 Oak Ave, Soweto',
              costShare: 170,
              status: 'completed',
              arrivalTime: new Date('2024-01-25T11:00:00'),
              completionTime: new Date('2024-01-25T11:30:00'),
              purchaseOrderId: '2',
              purchaseOrderNumber: 'PO-2024-002'
            }
          ],
          createdAt: new Date('2024-01-23'),
          updatedAt: new Date('2024-01-25')
        },
        {
          id: '2',
          runNumber: 'DR-2025-001',
          driverId: '1',
          driverName: 'Thabo Mthembu',
          scheduledDate: new Date('2025-01-25'),
          assignedDate: new Date('2025-01-24'),
          status: 'assigned',
          areaGroup: 'Soweto',
          totalDeliveryCost: 850,
          totalDistance: 45.5,
          participantCount: 5,
          costPerStop: 170,
          stops: [
            {
              id: '3',
              shopId: '1',
              shopName: 'Spaza Shop A',
              sequenceNumber: 1,
              address: '123 Main St, Soweto',
              costShare: 170,
              status: 'scheduled',
              purchaseOrderId: '3',
              purchaseOrderNumber: 'PO-2025-001'
            },
            {
              id: '4',
              shopId: '2',
              shopName: 'Spaza Shop B',
              sequenceNumber: 2,
              address: '456 Oak Ave, Soweto',
              costShare: 170,
              status: 'scheduled',
              purchaseOrderId: '4',
              purchaseOrderNumber: 'PO-2025-002'
            }
          ],
          createdAt: new Date('2025-01-23'),
          updatedAt: new Date('2025-01-24')
        },
        {
          id: '2',
          runNumber: 'DR-2025-002',
          scheduledDate: new Date('2025-01-26'),
          status: 'scheduled',
          areaGroup: 'Alexandra',
          totalDeliveryCost: 1200,
          totalDistance: 62.3,
          participantCount: 8,
          costPerStop: 150,
          stops: [],
          createdAt: new Date('2025-01-24'),
          updatedAt: new Date('2025-01-24')
        }
      ]
    } catch (error) {
      console.error('Failed to fetch delivery runs:', error)
    } finally {
      loading.value = false
    }
  }

  async function fetchRoutes() {
    loading.value = true
    try {
      // TODO: Replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 500))
      
      // Mock data
      routes.value = [
        {
          id: '1',
          routeName: 'Soweto Route - Jan 25',
          driverId: '1',
          driverName: 'Thabo Mthembu',
          date: new Date('2025-01-25'),
          status: 'active',
          totalStops: 5,
          completedStops: 2,
          totalDistance: 45.5,
          estimatedDuration: 180,
          deliveryRunIds: ['1'],
          optimized: true,
          createdAt: new Date('2025-01-24'),
          updatedAt: new Date('2025-01-25')
        },
        {
          id: '2',
          routeName: 'Alexandra Route - Jan 26',
          date: new Date('2025-01-26'),
          status: 'planned',
          totalStops: 8,
          completedStops: 0,
          totalDistance: 62.3,
          estimatedDuration: 240,
          deliveryRunIds: ['2'],
          optimized: false,
          createdAt: new Date('2025-01-24'),
          updatedAt: new Date('2025-01-24')
        }
      ]
    } catch (error) {
      console.error('Failed to fetch routes:', error)
    } finally {
      loading.value = false
    }
  }

  async function createDriver(driver: Omit<Driver, 'id' | 'fullName' | 'createdAt' | 'updatedAt'>): Promise<Driver> {
    loading.value = true
    try {
      // TODO: Replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 500))
      
      const newDriver: Driver = {
        ...driver,
        id: `driver-${Date.now()}`,
        fullName: `${driver.firstName} ${driver.lastName}`,
        createdAt: new Date(),
        updatedAt: new Date()
      }
      
      drivers.value.push(newDriver)
      return newDriver
    } catch (error) {
      console.error('Failed to create driver:', error)
      throw error
    } finally {
      loading.value = false
    }
  }

  async function updateDriver(id: string, updates: Partial<Driver>): Promise<Driver> {
    loading.value = true
    try {
      // TODO: Replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 500))
      
      const index = drivers.value.findIndex(d => d.id === id)
      if (index === -1) throw new Error('Driver not found')
      
      const updated = {
        ...drivers.value[index],
        ...updates,
        fullName: updates.firstName || updates.lastName 
          ? `${updates.firstName || drivers.value[index].firstName} ${updates.lastName || drivers.value[index].lastName}`
          : drivers.value[index].fullName,
        updatedAt: new Date()
      }
      
      drivers.value[index] = updated
      return updated
    } catch (error) {
      console.error('Failed to update driver:', error)
      throw error
    } finally {
      loading.value = false
    }
  }

  async function assignDriverToRun(runId: string, driverId: string): Promise<boolean> {
    loading.value = true
    try {
      // TODO: Replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 500))
      
      const run = deliveryRuns.value.find(r => r.id === runId)
      const driver = drivers.value.find(d => d.id === driverId)
      
      if (!run || !driver) return false
      if (!driver.isAvailable) return false
      
      run.driverId = driverId
      run.driverName = driver.fullName
      run.assignedDate = new Date()
      run.status = 'assigned'
      run.updatedAt = new Date()
      
      return true
    } catch (error) {
      console.error('Failed to assign driver:', error)
      return false
    } finally {
      loading.value = false
    }
  }

  async function updateDeliveryStatus(runId: string, status: DeliveryStatus): Promise<boolean> {
    loading.value = true
    try {
      // TODO: Replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 500))
      
      const run = deliveryRuns.value.find(r => r.id === runId)
      if (!run) return false
      
      run.status = status
      if (status === 'in_transit' && !run.startedAt) {
        run.startedAt = new Date()
      }
      if (status === 'completed' && !run.completedAt) {
        run.completedAt = new Date()
      }
      run.updatedAt = new Date()
      
      return true
    } catch (error) {
      console.error('Failed to update delivery status:', error)
      return false
    } finally {
      loading.value = false
    }
  }

  function getDriverById(id: string): Driver | undefined {
    return drivers.value.find(d => d.id === id)
  }

  function getDeliveryRunById(id: string): DeliveryRun | undefined {
    return deliveryRuns.value.find(r => r.id === id)
  }

  function getRouteById(id: string): Route | undefined {
    return routes.value.find(r => r.id === id)
  }

  return {
    // State
    drivers,
    deliveryRuns,
    routes,
    loading,
    // Computed
    activeDrivers,
    availableDrivers,
    activeDeliveryRuns,
    completedDeliveryRuns,
    activeRoutes,
    // Actions
    fetchDrivers,
    fetchDeliveryRuns,
    fetchRoutes,
    createDriver,
    updateDriver,
    assignDriverToRun,
    updateDeliveryStatus,
    getDriverById,
    getDeliveryRunById,
    getRouteById
  }
})

