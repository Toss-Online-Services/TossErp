// Collaboration & Community API Service
import { apiGet, apiPost, apiPut, apiDelete, API_ENDPOINTS, USE_MOCK_DATA, mockDelay } from '~/utils/api'

// Types
export interface CollaborationGroup {
  id: string
  title: string
  description: string
  type: 'group-buying' | 'logistics' | 'resource-sharing' | 'marketing' | 'equipment-sharing'
  status: 'forming' | 'active' | 'completed' | 'cancelled'
  participants: number
  maxParticipants: number
  savings?: number
  metric?: string
  deadline: string
  createdBy: string
  createdAt: string
  updatedAt: string
  location?: string
  requirements?: string[]
  tags?: string[]
}

export interface CollaborationOpportunity {
  id: string
  title: string
  description: string
  type: 'group-buying' | 'logistics' | 'resource-sharing' | 'marketing' | 'equipment-sharing'
  savings: number
  spotsLeft: number
  totalSpots: number
  deadline: string
  location: string
  requirements: string[]
  estimatedValue: number
  organizer: {
    id: string
    name: string
    rating: number
    trustScore: number
  }
  createdAt: string
}

export interface NetworkConnection {
  id: string
  businessName: string
  contactPerson: string
  industry: string
  location: string
  trustScore: number
  collaborations: number
  rating: number
  services: string[]
  connectedAt: string
}

export interface CollaborationStats {
  activeCollaborations: number
  totalSavings: number
  collaborationsJoined: number
  trustScore: number
  activePartnerships: number
  networkConnections: number
  averageSavings: number
  collaborationSuccessRate: number
}

export interface CreateCollaborationRequest {
  title: string
  description: string
  type: 'group-buying' | 'logistics' | 'resource-sharing' | 'marketing' | 'equipment-sharing'
  maxParticipants: number
  deadline: string
  location?: string
  requirements?: string[]
  estimatedValue?: number
  tags?: string[]
}

export interface JoinCollaborationRequest {
  collaborationId: string
  message?: string
  commitment?: string
}

export interface ResourceOffer {
  id: string
  title: string
  description: string
  type: 'space' | 'equipment' | 'service' | 'expertise'
  availability: string
  cost: number
  location: string
  owner: {
    id: string
    name: string
    rating: number
  }
  tags: string[]
  createdAt: string
}

// Mock data
const mockCollaborations: CollaborationGroup[] = [
  {
    id: '1',
    title: 'Electronics Bulk Purchase',
    description: 'Group buying for electronics inventory to get better wholesale prices',
    type: 'group-buying',
    status: 'active',
    participants: 8,
    maxParticipants: 12,
    savings: 2500.00,
    deadline: new Date(Date.now() + 7 * 24 * 60 * 60 * 1000).toISOString(),
    createdBy: 'user123',
    location: 'Soweto',
    requirements: ['Minimum R10,000 order', 'Cash payment'],
    tags: ['electronics', 'wholesale', 'bulk'],
    createdAt: '2024-01-10T08:00:00Z',
    updatedAt: '2024-01-15T14:30:00Z'
  },
  {
    id: '2',
    title: 'Weekly Grocery Delivery',
    description: 'Shared logistics for weekly grocery deliveries to reduce costs',
    type: 'logistics',
    status: 'active',
    participants: 15,
    maxParticipants: 20,
    metric: '15% cost reduction',
    deadline: new Date(Date.now() + 3 * 24 * 60 * 60 * 1000).toISOString(),
    createdBy: 'user456',
    location: 'Alexandria',
    requirements: ['Weekly commitment', 'R500 minimum order'],
    tags: ['delivery', 'groceries', 'weekly'],
    createdAt: '2024-01-05T10:00:00Z',
    updatedAt: '2024-01-16T09:15:00Z'
  },
  {
    id: '3',
    title: 'Storage Space Sharing',
    description: 'Share warehouse space to reduce individual storage costs',
    type: 'resource-sharing',
    status: 'active',
    participants: 6,
    maxParticipants: 10,
    metric: '2000 sqft shared',
    deadline: new Date(Date.now() + 30 * 24 * 60 * 60 * 1000).toISOString(),
    createdBy: 'user789',
    location: 'Johannesburg CBD',
    requirements: ['Security deposit', '6-month minimum'],
    tags: ['storage', 'warehouse', 'sharing'],
    createdAt: '2024-01-01T12:00:00Z',
    updatedAt: '2024-01-16T11:00:00Z'
  }
]

const mockOpportunities: CollaborationOpportunity[] = [
  {
    id: '1',
    title: 'Office Supplies Bulk Order',
    description: 'Join 20+ businesses for quarterly office supplies purchase with significant discounts',
    type: 'group-buying',
    savings: 25,
    spotsLeft: 5,
    totalSpots: 25,
    deadline: new Date(Date.now() + 21 * 24 * 60 * 60 * 1000).toISOString(),
    location: 'Johannesburg',
    requirements: ['R2,000 minimum order', 'Quarterly commitment'],
    estimatedValue: 15000,
    organizer: {
      id: 'org123',
      name: 'Township Business Hub',
      rating: 4.8,
      trustScore: 95
    },
    createdAt: '2024-01-12T08:00:00Z'
  },
  {
    id: '2',
    title: 'Shared Marketing Campaign',
    description: 'Collaborative digital marketing campaign for local businesses to increase reach',
    type: 'marketing',
    savings: 40,
    spotsLeft: 3,
    totalSpots: 8,
    deadline: new Date(Date.now() + 14 * 24 * 60 * 60 * 1000).toISOString(),
    location: 'Pretoria',
    requirements: ['R5,000 contribution', 'Social media presence'],
    estimatedValue: 25000,
    organizer: {
      id: 'org456',
      name: 'Local Marketing Co-op',
      rating: 4.6,
      trustScore: 88
    },
    createdAt: '2024-01-14T10:30:00Z'
  },
  {
    id: '3',
    title: 'Equipment Rental Pool',
    description: 'Share construction and maintenance equipment to reduce individual ownership costs',
    type: 'equipment-sharing',
    savings: 60,
    spotsLeft: 8,
    totalSpots: 12,
    deadline: new Date(Date.now() + 45 * 24 * 60 * 60 * 1000).toISOString(),
    location: 'Cape Town',
    requirements: ['Equipment maintenance agreement', 'R10,000 deposit'],
    estimatedValue: 50000,
    organizer: {
      id: 'org789',
      name: 'Equipment Share Network',
      rating: 4.9,
      trustScore: 92
    },
    createdAt: '2024-01-08T14:00:00Z'
  }
]

const mockStats: CollaborationStats = {
  activeCollaborations: 5,
  totalSavings: 8750.50,
  collaborationsJoined: 15,
  trustScore: 92,
  activePartnerships: 8,
  networkConnections: 34,
  averageSavings: 583.37,
  collaborationSuccessRate: 87
}

const mockConnections: NetworkConnection[] = [
  {
    id: '1',
    businessName: 'Township Electronics Hub',
    contactPerson: 'John Mthembu',
    industry: 'Electronics Retail',
    location: 'Soweto',
    trustScore: 95,
    collaborations: 12,
    rating: 4.8,
    services: ['Wholesale Electronics', 'Repair Services', 'Logistics'],
    connectedAt: '2024-01-05T08:00:00Z'
  },
  {
    id: '2',
    businessName: 'Fresh Produce Co-op',
    contactPerson: 'Sarah Ndlovu',
    industry: 'Food & Agriculture',
    location: 'Alexandra',
    trustScore: 88,
    collaborations: 8,
    rating: 4.6,
    services: ['Fresh Produce', 'Bulk Orders', 'Delivery'],
    connectedAt: '2024-01-08T10:15:00Z'
  }
]

// Collaboration Service Class
export class CollaborationService {
  /**
   * Get collaboration statistics/overview
   */
  static async getStats(): Promise<CollaborationStats> {
    if (USE_MOCK_DATA) {
      await mockDelay()
      return mockStats
    }
    
    return await apiGet<CollaborationStats>(`${API_ENDPOINTS.collaboration}/stats`)
  }

  /**
   * Get all collaborations with optional filtering
   */
  static async getCollaborations(params?: {
    type?: string
    status?: string
    location?: string
    page?: number
    limit?: number
  }): Promise<CollaborationGroup[]> {
    if (USE_MOCK_DATA) {
      await mockDelay()
      let collaborations = [...mockCollaborations]
      
      if (params?.type && params.type !== 'all') {
        collaborations = collaborations.filter(c => c.type === params.type)
      }
      
      if (params?.status) {
        collaborations = collaborations.filter(c => c.status === params.status)
      }
      
      return collaborations.sort((a, b) => new Date(b.updatedAt).getTime() - new Date(a.updatedAt).getTime())
    }
    
    return await apiGet<CollaborationGroup[]>(`${API_ENDPOINTS.collaboration}/groups`, {
      query: params
    })
  }

  /**
   * Get a single collaboration by ID
   */
  static async getCollaboration(id: string): Promise<CollaborationGroup> {
    if (USE_MOCK_DATA) {
      await mockDelay()
      const collaboration = mockCollaborations.find(c => c.id === id)
      if (!collaboration) {
        throw new Error(`Collaboration with ID ${id} not found`)
      }
      return collaboration
    }
    
    return await apiGet<CollaborationGroup>(`${API_ENDPOINTS.collaboration}/groups/${id}`)
  }

  /**
   * Create a new collaboration
   */
  static async createCollaboration(data: CreateCollaborationRequest): Promise<CollaborationGroup> {
    if (USE_MOCK_DATA) {
      await mockDelay()
      
      const newCollaboration: CollaborationGroup = {
        id: Date.now().toString(),
        ...data,
        status: 'forming',
        participants: 1,
        createdBy: 'current-user',
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString()
      }
      
      mockCollaborations.unshift(newCollaboration)
      return newCollaboration
    }
    
    return await apiPost<CollaborationGroup>(`${API_ENDPOINTS.collaboration}/groups`, data)
  }

  /**
   * Update a collaboration
   */
  static async updateCollaboration(id: string, data: Partial<CreateCollaborationRequest>): Promise<CollaborationGroup> {
    if (USE_MOCK_DATA) {
      await mockDelay()
      const index = mockCollaborations.findIndex(c => c.id === id)
      if (index === -1) {
        throw new Error(`Collaboration with ID ${id} not found`)
      }
      
      mockCollaborations[index] = {
        ...mockCollaborations[index],
        ...data,
        updatedAt: new Date().toISOString()
      }
      
      return mockCollaborations[index]
    }
    
    return await apiPut<CollaborationGroup>(`${API_ENDPOINTS.collaboration}/groups/${id}`, data)
  }

  /**
   * Join a collaboration
   */
  static async joinCollaboration(data: JoinCollaborationRequest): Promise<{ success: boolean; message: string }> {
    if (USE_MOCK_DATA) {
      await mockDelay()
      const collaboration = mockCollaborations.find(c => c.id === data.collaborationId)
      if (!collaboration) {
        throw new Error('Collaboration not found')
      }
      
      if (collaboration.participants >= collaboration.maxParticipants) {
        throw new Error('Collaboration is full')
      }
      
      collaboration.participants += 1
      collaboration.updatedAt = new Date().toISOString()
      
      return {
        success: true,
        message: 'Successfully joined collaboration'
      }
    }
    
    return await apiPost(`${API_ENDPOINTS.collaboration}/groups/join`, data)
  }

  /**
   * Leave a collaboration
   */
  static async leaveCollaboration(collaborationId: string): Promise<{ success: boolean; message: string }> {
    if (USE_MOCK_DATA) {
      await mockDelay()
      const collaboration = mockCollaborations.find(c => c.id === collaborationId)
      if (!collaboration) {
        throw new Error('Collaboration not found')
      }
      
      collaboration.participants = Math.max(0, collaboration.participants - 1)
      collaboration.updatedAt = new Date().toISOString()
      
      return {
        success: true,
        message: 'Successfully left collaboration'
      }
    }
    
    return await apiPost(`${API_ENDPOINTS.collaboration}/groups/${collaborationId}/leave`)
  }

  /**
   * Get available opportunities
   */
  static async getOpportunities(params?: {
    type?: string
    location?: string
    maxSavings?: number
    page?: number
    limit?: number
  }): Promise<CollaborationOpportunity[]> {
    if (USE_MOCK_DATA) {
      await mockDelay()
      let opportunities = [...mockOpportunities]
      
      if (params?.type) {
        opportunities = opportunities.filter(o => o.type === params.type)
      }
      
      return opportunities.sort((a, b) => b.savings - a.savings)
    }
    
    return await apiGet<CollaborationOpportunity[]>(`${API_ENDPOINTS.opportunities}`, {
      query: params
    })
  }

  /**
   * Join an opportunity
   */
  static async joinOpportunity(opportunityId: string, data?: {
    message?: string
    commitment?: string
  }): Promise<{ success: boolean; message: string }> {
    if (USE_MOCK_DATA) {
      await mockDelay()
      const opportunity = mockOpportunities.find(o => o.id === opportunityId)
      if (!opportunity) {
        throw new Error('Opportunity not found')
      }
      
      if (opportunity.spotsLeft <= 0) {
        throw new Error('No spots available')
      }
      
      opportunity.spotsLeft -= 1
      
      return {
        success: true,
        message: 'Successfully joined opportunity'
      }
    }
    
    return await apiPost(`${API_ENDPOINTS.opportunities}/${opportunityId}/join`, data)
  }

  /**
   * Get network connections
   */
  static async getConnections(params?: {
    industry?: string
    location?: string
    minTrustScore?: number
    page?: number
    limit?: number
  }): Promise<NetworkConnection[]> {
    if (USE_MOCK_DATA) {
      await mockDelay()
      return [...mockConnections]
    }
    
    return await apiGet<NetworkConnection[]>(`${API_ENDPOINTS.collaboration}/network`, {
      query: params
    })
  }

  /**
   * Connect with a business
   */
  static async connectWithBusiness(businessId: string, message?: string): Promise<{ success: boolean; message: string }> {
    if (USE_MOCK_DATA) {
      await mockDelay()
      return {
        success: true,
        message: 'Connection request sent successfully'
      }
    }
    
    return await apiPost(`${API_ENDPOINTS.collaboration}/network/connect`, {
      businessId,
      message
    })
  }

  /**
   * Search for collaboration partners
   */
  static async searchPartners(params: {
    type: string
    location?: string
    industry?: string
    minRating?: number
    services?: string[]
  }): Promise<NetworkConnection[]> {
    if (USE_MOCK_DATA) {
      await mockDelay()
      return mockConnections.filter(conn => {
        if (params.location && !conn.location.toLowerCase().includes(params.location.toLowerCase())) {
          return false
        }
        if (params.industry && !conn.industry.toLowerCase().includes(params.industry.toLowerCase())) {
          return false
        }
        if (params.minRating && conn.rating < params.minRating) {
          return false
        }
        return true
      })
    }
    
    return await apiGet<NetworkConnection[]>(`${API_ENDPOINTS.collaboration}/search-partners`, {
      query: params
    })
  }

  /**
   * Get collaboration recommendations
   */
  static async getRecommendations(): Promise<{
    collaborations: CollaborationGroup[]
    opportunities: CollaborationOpportunity[]
    partners: NetworkConnection[]
  }> {
    if (USE_MOCK_DATA) {
      await mockDelay()
      return {
        collaborations: mockCollaborations.slice(0, 3),
        opportunities: mockOpportunities.slice(0, 2),
        partners: mockConnections.slice(0, 2)
      }
    }
    
    return await apiGet(`${API_ENDPOINTS.collaboration}/recommendations`)
  }

  /**
   * Report collaboration issue
   */
  static async reportIssue(collaborationId: string, data: {
    type: 'quality' | 'communication' | 'payment' | 'other'
    description: string
    severity: 'low' | 'medium' | 'high'
  }): Promise<{ success: boolean; ticketId: string }> {
    if (USE_MOCK_DATA) {
      await mockDelay()
      return {
        success: true,
        ticketId: `TICKET-${Date.now()}`
      }
    }
    
    return await apiPost(`${API_ENDPOINTS.collaboration}/groups/${collaborationId}/report`, data)
  }

  /**
   * Rate collaboration experience
   */
  static async rateCollaboration(collaborationId: string, data: {
    rating: number // 1-5
    review?: string
    categories: {
      communication: number
      reliability: number
      quality: number
      value: number
    }
  }): Promise<{ success: boolean; message: string }> {
    if (USE_MOCK_DATA) {
      await mockDelay()
      return {
        success: true,
        message: 'Rating submitted successfully'
      }
    }
    
    return await apiPost(`${API_ENDPOINTS.collaboration}/groups/${collaborationId}/rate`, data)
  }
}

// Export default service
export default CollaborationService

