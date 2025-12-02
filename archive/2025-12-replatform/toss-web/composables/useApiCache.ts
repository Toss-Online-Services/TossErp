/**
 * API Request Caching and Deduplication Composable
 * Prevents duplicate requests and caches responses for better performance
 */

interface CacheEntry<T> {
  data: T
  timestamp: number
  expiresAt: number
}

interface PendingRequest<T> {
  promise: Promise<T>
  timestamp: number
}

class ApiCache {
  private cache = new Map<string, CacheEntry<any>>()
  private pendingRequests = new Map<string, PendingRequest<any>>()
  private readonly defaultTTL = 5 * 60 * 1000 // 5 minutes
  private readonly maxCacheSize = 100 // Maximum number of cached entries

  /**
   * Generate cache key from endpoint and params
   */
  private getCacheKey(endpoint: string, params?: Record<string, any>): string {
    const sortedParams = params ? Object.keys(params)
      .sort()
      .map(key => `${key}=${JSON.stringify(params[key])}`)
      .join('&') : ''
    return `${endpoint}${sortedParams ? `?${sortedParams}` : ''}`
  }

  /**
   * Check if cache entry is still valid
   */
  private isValid(entry: CacheEntry<any>): boolean {
    return Date.now() < entry.expiresAt
  }

  /**
   * Clean expired entries and enforce max size
   */
  private cleanup() {
    const now = Date.now()
    const entries = Array.from(this.cache.entries())
    
    // Remove expired entries
    entries.forEach(([key, entry]) => {
      if (!this.isValid(entry)) {
        this.cache.delete(key)
      }
    })

    // If still over limit, remove oldest entries
    if (this.cache.size > this.maxCacheSize) {
      const sorted = entries
        .filter(([_, entry]) => this.isValid(entry))
        .sort((a, b) => a[1].timestamp - b[1].timestamp)
      
      const toRemove = sorted.slice(0, this.cache.size - this.maxCacheSize)
      toRemove.forEach(([key]) => this.cache.delete(key))
    }

    // Clean up old pending requests (older than 30 seconds)
    const pendingEntries = Array.from(this.pendingRequests.entries())
    pendingEntries.forEach(([key, request]) => {
      if (now - request.timestamp > 30000) {
        this.pendingRequests.delete(key)
      }
    })
  }

  /**
   * Get cached data if available and valid
   */
  get<T>(endpoint: string, params?: Record<string, any>): T | null {
    this.cleanup()
    const key = this.getCacheKey(endpoint, params)
    const entry = this.cache.get(key)
    
    if (entry && this.isValid(entry)) {
      return entry.data as T
    }
    
    return null
  }

  /**
   * Set cache entry
   */
  set<T>(endpoint: string, data: T, params?: Record<string, any>, ttl?: number): void {
    this.cleanup()
    const key = this.getCacheKey(endpoint, params)
    const expiresAt = Date.now() + (ttl || this.defaultTTL)
    
    this.cache.set(key, {
      data,
      timestamp: Date.now(),
      expiresAt
    })
  }

  /**
   * Check if there's a pending request for this endpoint
   */
  getPendingRequest<T>(endpoint: string, params?: Record<string, any>): Promise<T> | null {
    const key = this.getCacheKey(endpoint, params)
    const pending = this.pendingRequests.get(key)
    
    if (pending) {
      return pending.promise as Promise<T>
    }
    
    return null
  }

  /**
   * Register a pending request to prevent duplicates
   */
  setPendingRequest<T>(endpoint: string, params: Record<string, any> | undefined, promise: Promise<T>): void {
    const key = this.getCacheKey(endpoint, params)
    this.pendingRequests.set(key, {
      promise: promise as Promise<any>,
      timestamp: Date.now()
    })

    // Clean up after request completes
    promise.finally(() => {
      this.pendingRequests.delete(key)
    })
  }

  /**
   * Invalidate cache for specific endpoint or all
   */
  invalidate(endpoint?: string, params?: Record<string, any>): void {
    if (endpoint) {
      const key = this.getCacheKey(endpoint, params)
      this.cache.delete(key)
    } else {
      this.cache.clear()
    }
  }

  /**
   * Clear all cache and pending requests
   */
  clear(): void {
    this.cache.clear()
    this.pendingRequests.clear()
  }

  /**
   * Get cache statistics
   */
  getStats() {
    return {
      cacheSize: this.cache.size,
      pendingRequests: this.pendingRequests.size,
      maxSize: this.maxCacheSize
    }
  }
}

// Singleton instance
let cacheInstance: ApiCache | null = null

export const useApiCache = () => {
  if (!cacheInstance) {
    cacheInstance = new ApiCache()
  }
  return cacheInstance
}


