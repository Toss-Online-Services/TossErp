// Shared Delivery Composable
// Business logic for shared logistics operations

import { useSharedLogisticsStore } from '~/stores/sharedLogistics'
import type {
  SharedRun,
  DeliveryStop,
  Location,
  DeliveryCostBreakdown
} from '~/types/logistics'
import { calculateDistance } from '~/server/utils/route-optimizer'

export const useSharedDelivery = () => {
  const store = useSharedLogisticsStore()
  
  /**
   * Calculate delivery cost savings compared to solo delivery
   */
  const calculateDeliverySavings = (
    soloDeliveryCost: number,
    sharedDeliveryCost: number
  ): {
    savings: number
    savingsPercentage: number
    message: string
  } => {
    const savings = soloDeliveryCost - sharedDeliveryCost
    const savingsPercentage = (savings / soloDeliveryCost) * 100
    
    return {
      savings,
      savingsPercentage,
      message: `You saved R${savings.toFixed(2)} (${savingsPercentage.toFixed(0)}%) by sharing delivery`
    }
  }
  
  /**
   * Get run status badge info
   */
  const getRunStatusBadge = (run: SharedRun): {
    label: string
    color: string
    icon: string
  } => {
    const statusMap = {
      scheduled: {
        label: 'Scheduled',
        color: 'bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400',
        icon: '📅'
      },
      'out-for-delivery': {
        label: 'Out for Delivery',
        color: 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-400',
        icon: '🚚'
      },
      completed: {
        label: 'Completed',
        color: 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400',
        icon: '✅'
      },
      cancelled: {
        label: 'Cancelled',
        color: 'bg-gray-100 text-gray-800 dark:bg-gray-900/30 dark:text-gray-400',
        icon: '❌'
      }
    }
    
    return statusMap[run.status] || statusMap.scheduled
  }
  
  /**
   * Get stop status badge info
   */
  const getStopStatusBadge = (stop: DeliveryStop): {
    label: string
    color: string
    icon: string
  } => {
    const statusMap = {
      pending: {
        label: 'Pending',
        color: 'bg-gray-100 text-gray-800 dark:bg-gray-900/30 dark:text-gray-400',
        icon: '⏳'
      },
      'out-for-delivery': {
        label: 'On the Way',
        color: 'bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400',
        icon: '🚚'
      },
      delivered: {
        label: 'Delivered',
        color: 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400',
        icon: '✅'
      },
      failed: {
        label: 'Failed',
        color: 'bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400',
        icon: '❌'
      }
    }
    
    return statusMap[stop.status] || statusMap.pending
  }
  
  /**
   * Calculate capacity utilization percentage
   */
  const getCapacityUtilization = (run: SharedRun): number => {
    return (run.currentLoad / run.maxCapacity) * 100
  }
  
  /**
   * Check if run has capacity for additional order
   */
  const hasCapacityFor = (run: SharedRun, weight: number, volume: number): boolean => {
    const hasWeightCapacity = (run.currentWeight + weight) <= run.maxWeight
    const hasSlotAvailable = run.availableSlots > 0
    
    return hasWeightCapacity && hasSlotAvailable
  }
  
  /**
   * Get time until pickup
   */
  const getTimeUntilPickup = (run: SharedRun): string => {
    const now = new Date()
    const pickup = new Date(run.pickupWindow.start)
    const diff = pickup.getTime() - now.getTime()
    
    if (diff <= 0) return 'In progress'
    
    const days = Math.floor(diff / (1000 * 60 * 60 * 24))
    const hours = Math.floor((diff % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60))
    const minutes = Math.floor((diff % (1000 * 60 * 60)) / (1000 * 60))
    
    if (days > 0) return `${days}d ${hours}h`
    if (hours > 0) return `${hours}h ${minutes}m`
    return `${minutes}m`
  }
  
  /**
   * Get ETA for a delivery stop
   */
  const getStopETA = (stop: DeliveryStop): string => {
    const now = new Date()
    const eta = new Date(stop.estimatedArrival)
    const diff = eta.getTime() - now.getTime()
    
    if (diff <= 0) return 'Arrived'
    
    const hours = Math.floor(diff / (1000 * 60 * 60))
    const minutes = Math.floor((diff % (1000 * 60 * 60)) / (1000 * 60))
    
    if (hours > 0) return `${hours}h ${minutes}m`
    return `${minutes}m`
  }
  
  /**
   * Check if stop is nearby (within 10 minutes)
   */
  const isStopNearby = (stop: DeliveryStop): boolean => {
    if (stop.status !== 'out-for-delivery') return false
    
    const now = new Date()
    const eta = new Date(stop.estimatedArrival)
    const diff = eta.getTime() - now.getTime()
    const minutesAway = diff / (1000 * 60)
    
    return minutesAway <= 10 && minutesAway > 0
  }
  
  /**
   * Calculate fee share explanation
   */
  const getFeeShareExplanation = (
    stop: DeliveryStop,
    run: SharedRun
  ): string => {
    switch (run.feeSplitRule) {
      case 'flat':
        return `Equal split among ${run.dropList.length} shops`
      
      case 'by-stops':
        return `Base fee (R${run.baseFee}) + stop fee (R${run.feePerStop})`
      
      case 'by-weight':
        const weightPercentage = (stop.weight / run.currentWeight) * 100
        return `Based on ${weightPercentage.toFixed(0)}% of total weight`
      
      case 'by-distance':
        return `Based on distance to your location`
      
      default:
        return 'Cost share calculated'
    }
  }
  
  /**
   * Get driver rating stars
   */
  const getDriverRatingStars = (rating: number): string => {
    const fullStars = Math.floor(rating)
    const hasHalfStar = rating % 1 >= 0.5
    const emptyStars = 5 - fullStars - (hasHalfStar ? 1 : 0)
    
    return '⭐'.repeat(fullStars) +
           (hasHalfStar ? '✨' : '') +
           '☆'.repeat(emptyStars)
  }
  
  /**
   * Format delivery address for display
   */
  const formatAddress = (location: Location): string => {
    const parts = [location.address]
    if (location.landmark) {
      parts.push(`near ${location.landmark}`)
    }
    return parts.join(', ')
  }
  
  /**
   * Check if user can join run
   */
  const canJoinRun = (
    run: SharedRun,
    orderWeight: number,
    orderVolume: number
  ): {
    canJoin: boolean
    reason?: string
  } => {
    if (run.status !== 'scheduled') {
      return { canJoin: false, reason: 'Run is not open for new stops' }
    }
    
    if (run.availableSlots <= 0) {
      return { canJoin: false, reason: 'Run is full (no slots available)' }
    }
    
    if ((run.currentWeight + orderWeight) > run.maxWeight) {
      return { canJoin: false, reason: 'Exceeds weight capacity' }
    }
    
    // Check if pickup time is still in future
    const now = new Date()
    const pickup = new Date(run.pickupWindow.start)
    if (pickup <= now) {
      return { canJoin: false, reason: 'Pickup time has passed' }
    }
    
    return { canJoin: true }
  }
  
  /**
   * Get AI suggestion for delivery option
   */
  const getDeliverySuggestion = (
    orderZone: string,
    orderWeight: number,
    availableRuns: SharedRun[]
  ): {
    action: 'join-existing' | 'create-new' | 'solo'
    run?: SharedRun
    message: string
    estimatedSavings?: number
  } => {
    // Find runs in same zone with capacity
    const compatibleRuns = availableRuns.filter(run =>
      run.dropList.some(stop => stop.location.zone === orderZone) &&
      hasCapacityFor(run, orderWeight, 0)
    )
    
    if (compatibleRuns.length > 0) {
      // Sort by pickup time (soonest first)
      compatibleRuns.sort((a, b) =>
        new Date(a.pickupWindow.start).getTime() - new Date(b.pickupWindow.start).getTime()
      )
      
      const bestRun = compatibleRuns[0]
      const soloDeliveryCost = 150 // Typical solo delivery
      const sharedCost = bestRun.baseFee / (bestRun.dropList.length + 1) + bestRun.feePerStop
      const savings = soloDeliveryCost - sharedCost
      
      return {
        action: 'join-existing',
        run: bestRun,
        message: `Join shared delivery and save ~R${savings.toFixed(0)}`,
        estimatedSavings: savings
      }
    }
    
    // Check if other pending orders in same zone (could create new run)
    // This would require checking the order database
    // For now, suggest solo delivery
    
    return {
      action: 'solo',
      message: 'No shared deliveries available. Solo delivery recommended.',
      estimatedSavings: 0
    }
  }
  
  /**
   * Format currency
   */
  const formatCurrency = (amount: number): string => {
    return new Intl.NumberFormat('en-ZA', {
      style: 'currency',
      currency: 'ZAR',
      minimumFractionDigits: 2
    }).format(amount)
  }
  
  return {
    // Store
    ...store,
    
    // Computed helpers
    calculateDeliverySavings,
    getRunStatusBadge,
    getStopStatusBadge,
    getCapacityUtilization,
    hasCapacityFor,
    getTimeUntilPickup,
    getStopETA,
    isStopNearby,
    getFeeShareExplanation,
    getDriverRatingStars,
    formatAddress,
    canJoinRun,
    getDeliverySuggestion,
    formatCurrency
  }
}

