import { ref } from 'vue'

export interface Pool {
  id: string
  poolNumber: string
  productId: string
  productName: string
  targetQuantity: number
  currentQuantity: number
  unit: string
  unitPrice: number
  soloPrice: number
  savingsPercent: number
  area: string
  deadline: Date
  status: 'open' | 'pending' | 'confirmed' | 'fulfilled' | 'cancelled'
  participants: number
  targetParticipants: number
  participantList: Array<{
    id: string
    name: string
    quantity: number
  }>
  splitRule: 'flat' | 'units'
  isJoined: boolean
  isCreator: boolean
  myQuantity?: number
  mySavings?: number
  notes?: string
}

export interface CreatePoolRequest {
  productId: string
  targetQuantity: number
  unit: string
  soloPrice: number
  poolPrice: number
  area: string
  deadline: Date
  splitRule: 'flat' | 'units'
  notes?: string
}

export const useGroupBuying = () => {
  const loading = ref(false)
  const error = ref<string | null>(null)

  // Mock data for development
  const mockPools: Pool[] = [
    {
      id: '1',
      poolNumber: 'P-2025-001',
      productId: 'prod-1',
      productName: 'Cooking Oil 750ml (Case of 12)',
      targetQuantity: 6,
      currentQuantity: 4,
      unit: 'crates',
      unitPrice: 145.50,
      soloPrice: 165.00,
      savingsPercent: 12,
      area: 'soweto',
      deadline: new Date(Date.now() + 4 * 60 * 60 * 1000), // 4 hours from now
      status: 'open',
      participants: 4,
      targetParticipants: 6,
      participantList: [
        { id: '1', name: "Thabo's Spaza", quantity: 1 },
        { id: '2', name: "Mama Grace Shop", quantity: 1 },
        { id: '3', name: "Lucky's Store", quantity: 1 },
        { id: '4', name: "Bright Corner", quantity: 1 }
      ],
      splitRule: 'units',
      isJoined: true,
      isCreator: false,
      myQuantity: 1,
      mySavings: 19.50
    },
    {
      id: '2',
      poolNumber: 'P-2025-002',
      productId: 'prod-2',
      productName: 'White Bread (20 Loaves)',
      targetQuantity: 10,
      currentQuantity: 7,
      unit: 'cases',
      unitPrice: 180.00,
      soloPrice: 210.00,
      savingsPercent: 14,
      area: 'soweto',
      deadline: new Date(Date.now() + 2 * 60 * 60 * 1000), // 2 hours from now
      status: 'open',
      participants: 7,
      targetParticipants: 10,
      participantList: [
        { id: '1', name: "Thabo's Spaza", quantity: 1 },
        { id: '5', name: "Quick Shop", quantity: 1 },
        { id: '6', name: "City Spaza", quantity: 2 },
        { id: '7', name: "Corner Store", quantity: 1 },
        { id: '8', name: "Main Road Shop", quantity: 2 }
      ],
      splitRule: 'units',
      isJoined: true,
      isCreator: true,
      myQuantity: 1,
      mySavings: 30.00
    },
    {
      id: '3',
      poolNumber: 'P-2025-003',
      productId: 'prod-3',
      productName: 'Sugar 2.5kg (10 Bags)',
      targetQuantity: 8,
      currentQuantity: 3,
      unit: 'boxes',
      unitPrice: 425.00,
      soloPrice: 490.00,
      savingsPercent: 13,
      area: 'alexandra',
      deadline: new Date(Date.now() + 6 * 60 * 60 * 1000), // 6 hours from now
      status: 'open',
      participants: 3,
      targetParticipants: 8,
      participantList: [
        { id: '9', name: "Alex Superette", quantity: 1 },
        { id: '10', name: "Township Traders", quantity: 1 },
        { id: '11', name: "Fresh Goods", quantity: 1 }
      ],
      splitRule: 'units',
      isJoined: false,
      isCreator: false
    },
    {
      id: '4',
      poolNumber: 'P-2025-004',
      productId: 'prod-4',
      productName: 'Maize Meal 12.5kg',
      targetQuantity: 12,
      currentQuantity: 12,
      unit: 'bags',
      unitPrice: 95.00,
      soloPrice: 110.00,
      savingsPercent: 14,
      area: 'soweto',
      deadline: new Date(Date.now() - 1 * 60 * 60 * 1000), // 1 hour ago
      status: 'confirmed',
      participants: 12,
      targetParticipants: 12,
      participantList: [
        { id: '1', name: "Thabo's Spaza", quantity: 1 },
        { id: '2', name: "Mama Grace Shop", quantity: 1 },
        { id: '12', name: "Village Store", quantity: 2 }
      ],
      splitRule: 'units',
      isJoined: true,
      isCreator: false,
      myQuantity: 1,
      mySavings: 15.00
    }
  ]

  const getPools = async (): Promise<Pool[]> => {
    loading.value = true
    error.value = null
    
    try {
      // Simulate API call
      await new Promise(resolve => setTimeout(resolve, 500))
      return mockPools
    } catch (err) {
      error.value = 'Failed to load pools'
      throw err
    } finally {
      loading.value = false
    }
  }

  const getPoolById = async (id: string): Promise<Pool | null> => {
    loading.value = true
    error.value = null
    
    try {
      await new Promise(resolve => setTimeout(resolve, 300))
      return mockPools.find(p => p.id === id) || null
    } catch (err) {
      error.value = 'Failed to load pool'
      throw err
    } finally {
      loading.value = false
    }
  }

  const createPool = async (poolData: CreatePoolRequest): Promise<Pool> => {
    loading.value = true
    error.value = null
    
    try {
      // Simulate API call
      await new Promise(resolve => setTimeout(resolve, 800))
      
      const newPool: Pool = {
        id: `pool-${Date.now()}`,
        poolNumber: `P-2025-${String(mockPools.length + 1).padStart(3, '0')}`,
        productId: poolData.productId,
        productName: 'New Product', // Would come from product lookup
        targetQuantity: poolData.targetQuantity,
        currentQuantity: 0,
        unit: poolData.unit,
        unitPrice: poolData.poolPrice,
        soloPrice: poolData.soloPrice,
        savingsPercent: ((poolData.soloPrice - poolData.poolPrice) / poolData.soloPrice) * 100,
        area: poolData.area,
        deadline: poolData.deadline,
        status: 'open',
        participants: 0,
        targetParticipants: 6, // Could be calculated
        participantList: [],
        splitRule: poolData.splitRule,
        isJoined: false,
        isCreator: true,
        notes: poolData.notes
      }
      
      mockPools.push(newPool)
      return newPool
    } catch (err) {
      error.value = 'Failed to create pool'
      throw err
    } finally {
      loading.value = false
    }
  }

  const joinPool = async (poolId: string, quantity: number): Promise<void> => {
    loading.value = true
    error.value = null
    
    try {
      // Simulate API call
      await new Promise(resolve => setTimeout(resolve, 500))
      
      const pool = mockPools.find(p => p.id === poolId)
      if (pool) {
        pool.currentQuantity += quantity
        pool.participants += 1
        pool.isJoined = true
        pool.myQuantity = quantity
        pool.mySavings = (pool.soloPrice - pool.unitPrice) * quantity
        
        pool.participantList.push({
          id: 'current-user',
          name: "Your Shop",
          quantity
        })
      }
    } catch (err) {
      error.value = 'Failed to join pool'
      throw err
    } finally {
      loading.value = false
    }
  }

  const leavePool = async (poolId: string): Promise<void> => {
    loading.value = true
    error.value = null
    
    try {
      await new Promise(resolve => setTimeout(resolve, 500))
      
      const pool = mockPools.find(p => p.id === poolId)
      if (pool && pool.myQuantity) {
        pool.currentQuantity -= pool.myQuantity
        pool.participants -= 1
        pool.isJoined = false
        pool.myQuantity = undefined
        pool.mySavings = undefined
        
        pool.participantList = pool.participantList.filter(p => p.id !== 'current-user')
      }
    } catch (err) {
      error.value = 'Failed to leave pool'
      throw err
    } finally {
      loading.value = false
    }
  }

  const cancelPool = async (poolId: string): Promise<void> => {
    loading.value = true
    error.value = null
    
    try {
      await new Promise(resolve => setTimeout(resolve, 500))
      
      const pool = mockPools.find(p => p.id === poolId)
      if (pool) {
        pool.status = 'cancelled'
      }
    } catch (err) {
      error.value = 'Failed to cancel pool'
      throw err
    } finally {
      loading.value = false
    }
  }

  const confirmPool = async (poolId: string): Promise<void> => {
    loading.value = true
    error.value = null
    
    try {
      await new Promise(resolve => setTimeout(resolve, 500))
      
      const pool = mockPools.find(p => p.id === poolId)
      if (pool) {
        pool.status = 'confirmed'
      }
    } catch (err) {
      error.value = 'Failed to confirm pool'
      throw err
    } finally {
      loading.value = false
    }
  }

  const generateWhatsAppInvite = (pool: Pool): string => {
    const inviteLink = `${window.location.origin}/buying/group-buying/join/${pool.id}`
    return `🛒 Join our Group Buying Pool!\n\n${pool.productName}\nUnit Price: R${pool.unitPrice.toFixed(2)} (Save ${pool.savingsPercent.toFixed(0)}%)\nTarget: ${pool.targetQuantity} ${pool.unit}\nCloses: ${pool.deadline.toLocaleString('en-ZA')}\n\nJoin now: ${inviteLink}`
  }

  return {
    loading,
    error,
    getPools,
    getPoolById,
    createPool,
    joinPool,
    leavePool,
    cancelPool,
    confirmPool,
    generateWhatsAppInvite
  }
}
