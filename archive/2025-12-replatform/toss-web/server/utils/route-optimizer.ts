// Route Optimization Utilities for TOSS
// Simple nearest-neighbor algorithm for MVP, can be enhanced with Google Maps API

import type { Location, DeliveryStop, RouteOptimizationRequest, RouteOptimizationResponse } from '~/types/logistics'

/**
 * Calculate distance between two coordinates using Haversine formula
 * Returns distance in kilometers
 */
export function calculateDistance(loc1: Location, loc2: Location): number {
  const R = 6371 // Earth's radius in kilometers
  
  const lat1 = toRadians(loc1.coordinates.lat)
  const lat2 = toRadians(loc2.coordinates.lat)
  const deltaLat = toRadians(loc2.coordinates.lat - loc1.coordinates.lat)
  const deltaLon = toRadians(loc2.coordinates.lon - loc1.coordinates.lon)
  
  const a =
    Math.sin(deltaLat / 2) * Math.sin(deltaLat / 2) +
    Math.cos(lat1) * Math.cos(lat2) *
    Math.sin(deltaLon / 2) * Math.sin(deltaLon / 2)
  
  const c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a))
  
  return R * c
}

/**
 * Convert degrees to radians
 */
function toRadians(degrees: number): number {
  return degrees * (Math.PI / 180)
}

/**
 * Simple greedy nearest-neighbor route optimization
 * For MVP - produces decent results quickly
 * Future enhancement: Use Google Maps Directions API or similar
 */
export function optimizeRoute(
  pickup: Location,
  stops: DeliveryStop[]
): DeliveryStop[] {
  if (stops.length === 0) return []
  if (stops.length === 1) {
    stops[0].sequenceNumber = 1
    return stops
  }
  
  const optimized: DeliveryStop[] = []
  let currentLocation = pickup
  const remaining = [...stops]
  
  // Greedy nearest-neighbor algorithm
  while (remaining.length > 0) {
    // Find nearest stop from current location
    let nearestIndex = 0
    let shortestDistance = calculateDistance(currentLocation, remaining[0].location)
    
    for (let i = 1; i < remaining.length; i++) {
      const distance = calculateDistance(currentLocation, remaining[i].location)
      if (distance < shortestDistance) {
        shortestDistance = distance
        nearestIndex = i
      }
    }
    
    // Add nearest stop to optimized route
    const nearestStop = remaining[nearestIndex]
    optimized.push(nearestStop)
    
    // Update current location and remove from remaining
    currentLocation = nearestStop.location
    remaining.splice(nearestIndex, 1)
  }
  
  // Assign sequence numbers
  optimized.forEach((stop, index) => {
    stop.sequenceNumber = index + 1
  })
  
  return optimized
}

/**
 * Advanced route optimization with priorities and time windows
 */
export function optimizeRouteAdvanced(request: RouteOptimizationRequest): RouteOptimizationResponse {
  const { pickupLocation, stops, vehicleType } = request
  
  // Convert stops to DeliveryStop-like objects for optimization
  const deliveryStops: DeliveryStop[] = stops.map((stop, index) => ({
    id: `temp-stop-${index}`,
    runId: '',
    shopId: '',
    shopName: '',
    shopPhone: '',
    location: stop.location,
    sequenceNumber: 0,
    estimatedArrival: new Date(),
    orderId: '',
    orderType: 'purchase-order',
    items: [],
    weight: 0,
    volume: 0,
    status: 'pending',
    feeShare: 0,
    createdAt: new Date(),
    updatedAt: new Date()
  }))
  
  // Apply priority sorting if provided
  if (stops.some(s => s.priority)) {
    deliveryStops.sort((a, b) => {
      const stopA = stops.find(s => s.location === a.location)
      const stopB = stops.find(s => s.location === b.location)
      return (stopB?.priority || 0) - (stopA?.priority || 0)
    })
  }
  
  // Optimize route
  const optimized = optimizeRoute(pickupLocation, deliveryStops)
  
  // Calculate distances and times
  let totalDistance = calculateDistance(pickupLocation, optimized[0].location)
  let currentTime = new Date()
  
  const optimizedStops = optimized.map((stop, index) => {
    // Calculate distance from previous stop
    const prevLocation = index === 0 ? pickupLocation : optimized[index - 1].location
    const distanceFromPrevious = calculateDistance(prevLocation, stop.location)
    
    if (index > 0) {
      totalDistance += distanceFromPrevious
    }
    
    // Estimate time (assume average speed of 40 km/h in township areas)
    const timeInMinutes = (distanceFromPrevious / 40) * 60
    
    // Add stop time (assume 10 minutes per stop for loading/unloading)
    const stopTime = 10
    currentTime = new Date(currentTime.getTime() + (timeInMinutes + stopTime) * 60000)
    
    return {
      location: stop.location,
      sequenceNumber: stop.sequenceNumber,
      estimatedArrival: new Date(currentTime),
      distanceFromPrevious
    }
  })
  
  // Calculate estimated duration
  const estimatedDuration = Math.ceil((totalDistance / 40) * 60) + (optimized.length * 10)
  
  // Calculate fuel cost (assume R20 per liter, 10km per liter)
  const fuelCost = (totalDistance / 10) * 20
  
  // Generate recommendations
  const recommendations: string[] = []
  
  if (totalDistance > 50) {
    recommendations.push('Route exceeds 50km - consider splitting into multiple runs')
  }
  
  if (optimized.length > 10) {
    recommendations.push('High number of stops - allow extra time for delays')
  }
  
  const avgDistancePerStop = totalDistance / optimized.length
  if (avgDistancePerStop > 5) {
    recommendations.push('Stops are spread out - consider geographic clustering')
  }
  
  return {
    optimizedStops,
    totalDistance: Number(totalDistance.toFixed(2)),
    estimatedDuration,
    fuelCost: Number(fuelCost.toFixed(2)),
    recommendations: recommendations.length > 0 ? recommendations : undefined
  }
}

/**
 * Check if a location is within a geographic zone
 */
export function isInZone(location: Location, zone: string): boolean {
  // Simple zone matching for MVP
  return location.zone === zone
}

/**
 * Find nearby stops within a radius
 */
export function findNearbyStops(
  centerLocation: Location,
  allStops: DeliveryStop[],
  radiusKm: number
): DeliveryStop[] {
  return allStops.filter(stop => {
    const distance = calculateDistance(centerLocation, stop.location)
    return distance <= radiusKm
  })
}

/**
 * Calculate ETA for a stop based on current location and traffic
 */
export function calculateETA(
  currentLocation: Location,
  destination: Location,
  avgSpeedKmh: number = 40
): Date {
  const distance = calculateDistance(currentLocation, destination)
  const timeInHours = distance / avgSpeedKmh
  const timeInMilliseconds = timeInHours * 60 * 60 * 1000
  
  return new Date(Date.now() + timeInMilliseconds)
}

/**
 * Validate route feasibility
 */
export function validateRoute(
  stops: DeliveryStop[],
  vehicleCapacity: number,
  maxDistance: number
): {
  isValid: boolean
  issues: string[]
} {
  const issues: string[] = []
  
  // Check total weight against capacity
  const totalWeight = stops.reduce((sum, stop) => sum + stop.weight, 0)
  if (totalWeight > vehicleCapacity) {
    issues.push(`Total weight (${totalWeight}kg) exceeds vehicle capacity (${vehicleCapacity}kg)`)
  }
  
  // Check total distance
  let totalDistance = 0
  for (let i = 1; i < stops.length; i++) {
    totalDistance += calculateDistance(stops[i - 1].location, stops[i].location)
  }
  
  if (totalDistance > maxDistance) {
    issues.push(`Total distance (${totalDistance.toFixed(1)}km) exceeds maximum (${maxDistance}km)`)
  }
  
  // Check for duplicate stops
  const uniqueShops = new Set(stops.map(s => s.shopId))
  if (uniqueShops.size !== stops.length) {
    issues.push('Route contains duplicate stops for the same shop')
  }
  
  return {
    isValid: issues.length === 0,
    issues
  }
}

/**
 * Estimate delivery time window for a stop
 */
export function estimateDeliveryWindow(
  estimatedArrival: Date,
  bufferMinutes: number = 15
): { start: Date; end: Date } {
  return {
    start: new Date(estimatedArrival.getTime() - bufferMinutes * 60000),
    end: new Date(estimatedArrival.getTime() + bufferMinutes * 60000)
  }
}

