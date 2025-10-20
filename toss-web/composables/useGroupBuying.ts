// Group Buying Composable
// Business logic for pool operations

import { useGroupBuyingStore } from '~/stores/groupBuying'
import type {
  Pool,
  CreatePoolDto,
  PoolSavingsCalculation
} from '~/types/group-buying'

export const useGroupBuying = () => {
  const store = useGroupBuyingStore()
  const { $fetch } = useNuxtApp()
  
  /**
   * Calculate pool fill percentage
   */
  const calculateFillPercentage = (pool: Pool): number => {
    return (pool.currentCommitment / pool.targetQuantity) * 100
  }
  
  /**
   * Check if pool has reached target
   */
  const hasReachedTarget = (pool: Pool): boolean => {
    return pool.currentCommitment >= pool.targetQuantity
  }
  
  /**
   * Check if pool deadline has passed
   */
  const isDeadlinePassed = (pool: Pool): boolean => {
    return new Date() > new Date(pool.deadline)
  }
  
  /**
   * Check if pool can be extended (only once)
   */
  const canExtend = (pool: Pool): boolean => {
    const fillPercentage = calculateFillPercentage(pool)
    return !pool.extendedOnce && fillPercentage >= 70 && isDeadlinePassed(pool)
  }
  
  /**
   * Get time remaining until deadline
   */
  const getTimeRemaining = (pool: Pool): string => {
    const now = new Date()
    const deadline = new Date(pool.deadline)
    const diff = deadline.getTime() - now.getTime()
    
    if (diff <= 0) return 'Expired'
    
    const days = Math.floor(diff / (1000 * 60 * 60 * 24))
    const hours = Math.floor((diff % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60))
    const minutes = Math.floor((diff % (1000 * 60 * 60)) / (1000 * 60))
    
    if (days > 0) return `${days}d ${hours}h`
    if (hours > 0) return `${hours}h ${minutes}m`
    return `${minutes}m`
  }
  
  /**
   * Calculate savings for a participant
   */
  const calculateParticipantSavings = (
    pool: Pool,
    quantityCommitted: number
  ): {
    regularCost: number
    poolCost: number
    savings: number
    savingsPercentage: number
  } => {
    const regularCost = quantityCommitted * pool.currentPrice
    const poolCost = quantityCommitted * pool.poolPrice
    const savings = regularCost - poolCost
    const savingsPercentage = (savings / regularCost) * 100
    
    return {
      regularCost,
      poolCost,
      savings,
      savingsPercentage
    }
  }
  
  /**
   * Check if user can join pool
   */
  const canJoinPool = (pool: Pool, shopId: string): {
    canJoin: boolean
    reason?: string
  } => {
    if (pool.status !== 'open') {
      return { canJoin: false, reason: 'Pool is no longer open' }
    }
    
    if (pool.participants.some(p => p.shopId === shopId)) {
      return { canJoin: false, reason: 'Already in this pool' }
    }
    
    if (pool.participantCount >= pool.maxParticipants) {
      return { canJoin: false, reason: 'Pool is full' }
    }
    
    if (isDeadlinePassed(pool)) {
      return { canJoin: false, reason: 'Deadline has passed' }
    }
    
    return { canJoin: true }
  }
  
  /**
   * Get pool status badge info
   */
  const getPoolStatusBadge = (pool: Pool): {
    label: string
    color: string
    icon: string
  } => {
    const statusMap = {
      open: {
        label: 'Open',
        color: 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400',
        icon: '🟢'
      },
      pending: {
        label: 'Pending Confirmation',
        color: 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-400',
        icon: '⏳'
      },
      confirmed: {
        label: 'Confirmed',
        color: 'bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400',
        icon: '✅'
      },
      fulfilled: {
        label: 'Fulfilled',
        color: 'bg-purple-100 text-purple-800 dark:bg-purple-900/30 dark:text-purple-400',
        icon: '📦'
      },
      cancelled: {
        label: 'Cancelled',
        color: 'bg-gray-100 text-gray-800 dark:bg-gray-900/30 dark:text-gray-400',
        icon: '❌'
      }
    }
    
    return statusMap[pool.status] || statusMap.open
  }
  
  /**
   * Format currency (South African Rand)
   */
  const formatCurrency = (amount: number): string => {
    return new Intl.NumberFormat('en-ZA', {
      style: 'currency',
      currency: 'ZAR',
      minimumFractionDigits: 2
    }).format(amount)
  }
  
  /**
   * Get AI suggestion for pool action
   */
  const getPoolSuggestion = (
    pool: Pool,
    shopId: string,
    lowStockItems: string[]
  ): {
    action: 'join' | 'create' | 'wait' | 'none'
    message: string
    priority: 'high' | 'medium' | 'low'
  } => {
    // Check if pool item is in low stock
    const needsItem = lowStockItems.includes(pool.sku)
    const fillPercentage = calculateFillPercentage(pool)
    const timeRemaining = new Date(pool.deadline).getTime() - Date.now()
    const hoursRemaining = timeRemaining / (1000 * 60 * 60)
    
    // High priority: item low stock + pool nearly full
    if (needsItem && fillPercentage > 80 && hoursRemaining < 24) {
      return {
        action: 'join',
        message: `${pool.title} is ${fillPercentage.toFixed(0)}% full and closes soon. Save ${pool.savingsPercentage}%!`,
        priority: 'high'
      }
    }
    
    // Medium priority: item low stock + pool open
    if (needsItem && fillPercentage < 50) {
      return {
        action: 'join',
        message: `Save ${pool.savingsPercentage}% on ${pool.title}. ${pool.targetQuantity - pool.currentCommitment} units needed.`,
        priority: 'medium'
      }
    }
    
    // Low priority: good deal but not urgent
    if (pool.savingsPercentage > 15 && hoursRemaining > 48) {
      return {
        action: 'wait',
        message: `${pool.title} offers ${pool.savingsPercentage}% savings. Watch this pool.`,
        priority: 'low'
      }
    }
    
    return {
      action: 'none',
      message: '',
      priority: 'low'
    }
  }
  
  /**
   * Validate pool creation data
   */
  const validatePoolData = (data: Partial<CreatePoolDto>): {
    isValid: boolean
    errors: string[]
  } => {
    const errors: string[] = []
    
    if (!data.title || data.title.trim().length < 5) {
      errors.push('Title must be at least 5 characters')
    }
    
    if (!data.description || data.description.trim().length < 10) {
      errors.push('Description must be at least 10 characters')
    }
    
    if (!data.targetQuantity || data.targetQuantity <= 0) {
      errors.push('Target quantity must be greater than 0')
    }
    
    if (!data.deadline || new Date(data.deadline) <= new Date()) {
      errors.push('Deadline must be in the future')
    }
    
    if (!data.currentPrice || data.currentPrice <= 0) {
      errors.push('Current price must be greater than 0')
    }
    
    if (!data.poolPrice || data.poolPrice <= 0) {
      errors.push('Pool price must be greater than 0')
    }
    
    if (data.poolPrice && data.currentPrice && data.poolPrice >= data.currentPrice) {
      errors.push('Pool price must be less than current price')
    }
    
    if (!data.area || data.area.trim().length === 0) {
      errors.push('Area is required')
    }
    
    if (!data.supplierId || data.supplierId.trim().length === 0) {
      errors.push('Supplier is required')
    }
    
    return {
      isValid: errors.length === 0,
      errors
    }
  }
  
  return {
    // Store
    ...store,
    
    // Computed helpers
    calculateFillPercentage,
    hasReachedTarget,
    isDeadlinePassed,
    canExtend,
    getTimeRemaining,
    calculateParticipantSavings,
    canJoinPool,
    getPoolStatusBadge,
    formatCurrency,
    getPoolSuggestion,
    validatePoolData
  }
}

