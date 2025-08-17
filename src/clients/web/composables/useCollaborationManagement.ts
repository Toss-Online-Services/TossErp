// Collaboration Management Composable
import type { 
  CollaborationGroup, 
  CollaborationOpportunity, 
  NetworkConnection,
  CollaborationStats,
  CreateCollaborationRequest,
  JoinCollaborationRequest
} from '~/services/collaborationService'
import { CollaborationService } from '~/services/collaborationService'

export const useCollaborationManagement = () => {
  // Reactive state
  const collaborations = ref<CollaborationGroup[]>([])
  const opportunities = ref<CollaborationOpportunity[]>([])
  const connections = ref<NetworkConnection[]>([])
  const stats = ref<CollaborationStats | null>(null)
  const loading = ref(false)
  const error = ref<string | null>(null)

  // Computed properties
  const activeCollaborations = computed(() => stats.value?.activeCollaborations || 0)
  const totalSavings = computed(() => stats.value?.totalSavings || 0)
  const collaborationsJoined = computed(() => stats.value?.collaborationsJoined || 0)
  const trustScore = computed(() => stats.value?.trustScore || 0)
  const activePartnerships = computed(() => stats.value?.activePartnerships || 0)
  const networkConnections = computed(() => stats.value?.networkConnections || 0)

  // Filtered data
  const activeCollaborationsList = computed(() => 
    collaborations.value.filter(c => c.status === 'active')
  )

  const formingCollaborations = computed(() => 
    collaborations.value.filter(c => c.status === 'forming')
  )

  const availableOpportunities = computed(() => 
    opportunities.value.filter(o => o.spotsLeft > 0)
  )

  const groupBuyingOpportunities = computed(() => 
    opportunities.value.filter(o => o.type === 'group-buying')
  )

  const logisticsOpportunities = computed(() => 
    opportunities.value.filter(o => o.type === 'logistics')
  )

  // Load collaboration statistics
  const loadStats = async () => {
    try {
      loading.value = true
      error.value = null
      stats.value = await CollaborationService.getStats()
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to load collaboration statistics'
      console.error('Error loading collaboration stats:', err)
    } finally {
      loading.value = false
    }
  }

  // Load collaborations
  const loadCollaborations = async (params?: {
    type?: string
    status?: string
    location?: string
    page?: number
    limit?: number
  }) => {
    try {
      loading.value = true
      error.value = null
      collaborations.value = await CollaborationService.getCollaborations(params)
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to load collaborations'
      console.error('Error loading collaborations:', err)
    } finally {
      loading.value = false
    }
  }

  // Get single collaboration
  const getCollaboration = async (id: string): Promise<CollaborationGroup | null> => {
    try {
      loading.value = true
      error.value = null
      return await CollaborationService.getCollaboration(id)
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to load collaboration'
      console.error('Error loading collaboration:', err)
      return null
    } finally {
      loading.value = false
    }
  }

  // Create new collaboration
  const createCollaboration = async (data: CreateCollaborationRequest): Promise<CollaborationGroup | null> => {
    try {
      loading.value = true
      error.value = null
      const newCollaboration = await CollaborationService.createCollaboration(data)
      
      // Add to local collaborations array if loaded
      if (collaborations.value.length > 0) {
        collaborations.value.unshift(newCollaboration)
      }
      
      // Refresh stats
      await loadStats()
      
      return newCollaboration
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to create collaboration'
      console.error('Error creating collaboration:', err)
      return null
    } finally {
      loading.value = false
    }
  }

  // Update collaboration
  const updateCollaboration = async (id: string, data: Partial<CreateCollaborationRequest>): Promise<boolean> => {
    try {
      loading.value = true
      error.value = null
      const updatedCollaboration = await CollaborationService.updateCollaboration(id, data)
      
      // Update local collaborations array
      const index = collaborations.value.findIndex(c => c.id === id)
      if (index !== -1) {
        collaborations.value[index] = updatedCollaboration
      }
      
      return true
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to update collaboration'
      console.error('Error updating collaboration:', err)
      return false
    } finally {
      loading.value = false
    }
  }

  // Join collaboration
  const joinCollaboration = async (data: JoinCollaborationRequest): Promise<boolean> => {
    try {
      loading.value = true
      error.value = null
      const result = await CollaborationService.joinCollaboration(data)
      
      if (result.success) {
        // Update local data
        const collaboration = collaborations.value.find(c => c.id === data.collaborationId)
        if (collaboration) {
          collaboration.participants += 1
        }
        
        // Refresh stats
        await loadStats()
      }
      
      return result.success
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to join collaboration'
      console.error('Error joining collaboration:', err)
      return false
    } finally {
      loading.value = false
    }
  }

  // Leave collaboration
  const leaveCollaboration = async (collaborationId: string): Promise<boolean> => {
    try {
      loading.value = true
      error.value = null
      const result = await CollaborationService.leaveCollaboration(collaborationId)
      
      if (result.success) {
        // Update local data
        const collaboration = collaborations.value.find(c => c.id === collaborationId)
        if (collaboration) {
          collaboration.participants = Math.max(0, collaboration.participants - 1)
        }
        
        // Refresh stats
        await loadStats()
      }
      
      return result.success
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to leave collaboration'
      console.error('Error leaving collaboration:', err)
      return false
    } finally {
      loading.value = false
    }
  }

  // Load opportunities
  const loadOpportunities = async (params?: {
    type?: string
    location?: string
    maxSavings?: number
    page?: number
    limit?: number
  }) => {
    try {
      loading.value = true
      error.value = null
      opportunities.value = await CollaborationService.getOpportunities(params)
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to load opportunities'
      console.error('Error loading opportunities:', err)
    } finally {
      loading.value = false
    }
  }

  // Join opportunity
  const joinOpportunity = async (opportunityId: string, data?: {
    message?: string
    commitment?: string
  }): Promise<boolean> => {
    try {
      loading.value = true
      error.value = null
      const result = await CollaborationService.joinOpportunity(opportunityId, data)
      
      if (result.success) {
        // Update local data
        const opportunity = opportunities.value.find(o => o.id === opportunityId)
        if (opportunity) {
          opportunity.spotsLeft = Math.max(0, opportunity.spotsLeft - 1)
        }
        
        // Refresh stats
        await loadStats()
      }
      
      return result.success
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to join opportunity'
      console.error('Error joining opportunity:', err)
      return false
    } finally {
      loading.value = false
    }
  }

  // Load network connections
  const loadConnections = async (params?: {
    industry?: string
    location?: string
    minTrustScore?: number
    page?: number
    limit?: number
  }) => {
    try {
      loading.value = true
      error.value = null
      connections.value = await CollaborationService.getConnections(params)
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to load connections'
      console.error('Error loading connections:', err)
    } finally {
      loading.value = false
    }
  }

  // Connect with business
  const connectWithBusiness = async (businessId: string, message?: string): Promise<boolean> => {
    try {
      loading.value = true
      error.value = null
      const result = await CollaborationService.connectWithBusiness(businessId, message)
      
      if (result.success) {
        // Refresh stats
        await loadStats()
      }
      
      return result.success
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to connect with business'
      console.error('Error connecting with business:', err)
      return false
    } finally {
      loading.value = false
    }
  }

  // Search for partners
  const searchPartners = async (params: {
    type: string
    location?: string
    industry?: string
    minRating?: number
    services?: string[]
  }): Promise<NetworkConnection[]> => {
    try {
      loading.value = true
      error.value = null
      return await CollaborationService.searchPartners(params)
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to search partners'
      console.error('Error searching partners:', err)
      return []
    } finally {
      loading.value = false
    }
  }

  // Get recommendations
  const getRecommendations = async () => {
    try {
      loading.value = true
      error.value = null
      return await CollaborationService.getRecommendations()
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to get recommendations'
      console.error('Error getting recommendations:', err)
      return null
    } finally {
      loading.value = false
    }
  }

  // Rate collaboration
  const rateCollaboration = async (collaborationId: string, data: {
    rating: number
    review?: string
    categories: {
      communication: number
      reliability: number
      quality: number
      value: number
    }
  }): Promise<boolean> => {
    try {
      loading.value = true
      error.value = null
      const result = await CollaborationService.rateCollaboration(collaborationId, data)
      return result.success
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to rate collaboration'
      console.error('Error rating collaboration:', err)
      return false
    } finally {
      loading.value = false
    }
  }

  // Report issue
  const reportIssue = async (collaborationId: string, data: {
    type: 'quality' | 'communication' | 'payment' | 'other'
    description: string
    severity: 'low' | 'medium' | 'high'
  }): Promise<string | null> => {
    try {
      loading.value = true
      error.value = null
      const result = await CollaborationService.reportIssue(collaborationId, data)
      return result.success ? result.ticketId : null
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to report issue'
      console.error('Error reporting issue:', err)
      return null
    } finally {
      loading.value = false
    }
  }

  // Utility functions
  const formatCurrency = (amount: number): string => {
    return new Intl.NumberFormat('en-ZA', {
      minimumFractionDigits: 2,
      maximumFractionDigits: 2
    }).format(amount)
  }

  const formatDate = (dateString: string): string => {
    const date = new Date(dateString)
    const now = new Date()
    const diffTime = date.getTime() - now.getTime()
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24))
    
    if (diffDays <= 0) return 'Today'
    if (diffDays === 1) return 'Tomorrow'
    if (diffDays <= 7) return `${diffDays} days`
    if (diffDays <= 30) return `${Math.ceil(diffDays / 7)} weeks`
    return date.toLocaleDateString('en-ZA', { month: 'short', day: 'numeric' })
  }

  const formatDateLong = (dateString: string): string => {
    return new Intl.DateTimeFormat('en-ZA', {
      month: 'short',
      day: 'numeric',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    }).format(new Date(dateString))
  }

  const getCollaborationTypeClass = (type: CollaborationGroup['type']): string => {
    switch (type) {
      case 'group-buying':
        return 'bg-blue-500'
      case 'logistics':
        return 'bg-green-500'
      case 'resource-sharing':
        return 'bg-purple-500'
      case 'marketing':
        return 'bg-orange-500'
      case 'equipment-sharing':
        return 'bg-indigo-500'
      default:
        return 'bg-gray-500'
    }
  }

  const getCollaborationTypeText = (type: CollaborationGroup['type']): string => {
    switch (type) {
      case 'group-buying':
        return 'Group Buying'
      case 'logistics':
        return 'Logistics'
      case 'resource-sharing':
        return 'Resource Sharing'
      case 'marketing':
        return 'Marketing'
      case 'equipment-sharing':
        return 'Equipment Sharing'
      default:
        return 'Other'
    }
  }

  const getStatusClass = (status: CollaborationGroup['status']): string => {
    switch (status) {
      case 'active':
        return 'bg-green-100 text-green-600 dark:bg-green-900 dark:text-green-400'
      case 'forming':
        return 'bg-yellow-100 text-yellow-600 dark:bg-yellow-900 dark:text-yellow-400'
      case 'completed':
        return 'bg-blue-100 text-blue-600 dark:bg-blue-900 dark:text-blue-400'
      case 'cancelled':
        return 'bg-red-100 text-red-600 dark:bg-red-900 dark:text-red-400'
      default:
        return 'bg-gray-100 text-gray-600 dark:bg-gray-900 dark:text-gray-400'
    }
  }

  const getStatusText = (status: CollaborationGroup['status']): string => {
    switch (status) {
      case 'active':
        return 'Active'
      case 'forming':
        return 'Forming'
      case 'completed':
        return 'Completed'
      case 'cancelled':
        return 'Cancelled'
      default:
        return 'Unknown'
    }
  }

  const getTrustScoreClass = (score: number): string => {
    if (score >= 90) return 'text-green-600 dark:text-green-400'
    if (score >= 70) return 'text-yellow-600 dark:text-yellow-400'
    return 'text-red-600 dark:text-red-400'
  }

  const getTrustScoreText = (score: number): string => {
    if (score >= 90) return 'Excellent'
    if (score >= 80) return 'Very Good'
    if (score >= 70) return 'Good'
    if (score >= 60) return 'Fair'
    return 'Poor'
  }

  const getCollaborationIcon = (type: CollaborationGroup['type']): string => {
    switch (type) {
      case 'group-buying':
        return 'M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z'
      case 'logistics':
        return 'M8 7h12m0 0l-4-4m4 4l-4 4m0 6H4m0 0l4 4m-4-4l4-4'
      case 'resource-sharing':
        return '8.684 13.342C8.886 12.938 9 12.482 9 12c0-.482-.114-.938-.316-1.342m0 2.684a3 3 0 110-2.684m0 2.684l6.632 3.316m-6.632-6l6.632-3.316m0 0a3 3 0 105.367-2.684 3 3 0 00-5.367 2.684zm0 9.316a3 3 0 105.368 2.684 3 3 0 00-5.368-2.684z'
      case 'marketing':
        return 'M11 5.882V19.24a1.76 1.76 0 01-3.417.592l-2.147-6.15M18 13a3 3 0 100-6M5.436 13.683A4.001 4.001 0 017 6h1.832c4.1 0 7.625-1.234 9.168-3v14c-1.543-1.766-5.067-3-9.168-3H7a3.988 3.988 0 01-1.564-.317z'
      case 'equipment-sharing':
        return 'M19.428 15.428a2 2 0 00-1.022-.547l-2.387-.477a6 6 0 00-3.86.517l-.318.158a6 6 0 01-3.86.517L6.05 15.21a2 2 0 00-1.806.547M8 4h8l-1 1v5.172a2 2 0 00.586 1.414l5 5c1.26 1.26.367 3.414-1.415 3.414H4.828c-1.782 0-2.674-2.154-1.414-3.414l5-5A2 2 0 009 10.172V5L8 4z'
      default:
        return 'M9 5H7a2 2 0 00-2 2v10a2 2 0 002 2h8a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2'
    }
  }

  return {
    // Reactive state
    collaborations: readonly(collaborations),
    opportunities: readonly(opportunities),
    connections: readonly(connections),
    stats: readonly(stats),
    loading: readonly(loading),
    error: readonly(error),
    
    // Computed properties
    activeCollaborations,
    totalSavings,
    collaborationsJoined,
    trustScore,
    activePartnerships,
    networkConnections,
    activeCollaborationsList,
    formingCollaborations,
    availableOpportunities,
    groupBuyingOpportunities,
    logisticsOpportunities,
    
    // Methods
    loadStats,
    loadCollaborations,
    getCollaboration,
    createCollaboration,
    updateCollaboration,
    joinCollaboration,
    leaveCollaboration,
    loadOpportunities,
    joinOpportunity,
    loadConnections,
    connectWithBusiness,
    searchPartners,
    getRecommendations,
    rateCollaboration,
    reportIssue,
    
    // Utility functions
    formatCurrency,
    formatDate,
    formatDateLong,
    getCollaborationTypeClass,
    getCollaborationTypeText,
    getStatusClass,
    getStatusText,
    getTrustScoreClass,
    getTrustScoreText,
    getCollaborationIcon
  }
}

