export default defineEventHandler(async (event) => {
  const query = getQuery(event)
  const { type = 'all', unreadOnly = false, limit = 20, participantId = '' } = query

  // Demo conversations data
  const demoConversations = [
    {
      id: 'conv_001',
      type: 'direct',
      title: 'Order Discussion - Corner Café',
      participants: [
        {
          id: 'USER_CURRENT',
          name: 'You',
          role: 'owner',
          avatar: null,
          isOnline: true
        },
        {
          id: 'CUST-001',
          name: 'Sarah Johnson',
          role: 'customer',
          avatar: 'https://images.unsplash.com/photo-1494790108755-2616b25c4ab4?w=64&h=64&fit=crop&crop=face',
          isOnline: false,
          lastSeen: '2024-08-25T12:30:00Z'
        }
      ],
      lastMessage: {
        id: 'msg_latest_001',
        senderId: 'CUST-001',
        senderName: 'Sarah Johnson',
        content: 'Thanks for the quick delivery! The maize meal quality is excellent as always. Can we schedule a regular weekly order?',
        type: 'text',
        timestamp: '2024-08-25T14:30:00Z',
        isRead: false,
        attachments: []
      },
      unreadCount: 2,
      updatedAt: '2024-08-25T14:30:00Z',
      status: 'active',
      tags: ['customer', 'orders', 'regular_customer'],
      metadata: {
        relatedOrderId: 'ORD-2024-007',
        customerType: 'business',
        priority: 'normal'
      }
    },
    {
      id: 'conv_002',
      type: 'group',
      title: 'Flour Group Purchase Coordination',
      participants: [
        {
          id: 'USER_CURRENT',
          name: 'You',
          role: 'participant',
          avatar: null,
          isOnline: true
        },
        {
          id: 'USER_ALEXANDRA',
          name: 'Alexandra Business Network',
          role: 'organizer',
          avatar: 'https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=64&h=64&fit=crop&crop=face',
          isOnline: true,
          lastSeen: null
        },
        {
          id: 'USER_SUNSHINE',
          name: 'Sunshine Bakery',
          role: 'participant',
          avatar: 'https://images.unsplash.com/photo-1438761681033-6461ffad8d80?w=64&h=64&fit=crop&crop=face',
          isOnline: false,
          lastSeen: '2024-08-25T10:15:00Z'
        },
        {
          id: 'USER_CORNER',
          name: 'Corner Café',
          role: 'participant',
          avatar: 'https://images.unsplash.com/photo-1544725176-7c40e5a71c5e?w=64&h=64&fit=crop&crop=face',
          isOnline: false,
          lastSeen: '2024-08-25T13:45:00Z'
        }
      ],
      lastMessage: {
        id: 'msg_latest_002',
        senderId: 'USER_ALEXANDRA',
        senderName: 'Alexandra Business Network',
        content: 'Great news everyone! We\'ve reached 76% of our target. Just need 24 more bags to trigger the group discount. Deadline is this Thursday.',
        type: 'text',
        timestamp: '2024-08-25T13:15:00Z',
        isRead: true,
        attachments: []
      },
      unreadCount: 0,
      updatedAt: '2024-08-25T13:15:00Z',
      status: 'active',
      tags: ['group_buying', 'flour', 'coordination'],
      metadata: {
        groupPurchaseId: 'gp_flour_001',
        progress: 76,
        deadline: '2024-08-28T23:59:59Z'
      }
    },
    {
      id: 'conv_003',
      type: 'support',
      title: 'Logistics Issue - Late Delivery',
      participants: [
        {
          id: 'USER_CURRENT',
          name: 'You',
          role: 'customer',
          avatar: null,
          isOnline: true
        },
        {
          id: 'SUPPORT_001',
          name: 'FastTrack Support',
          role: 'support_agent',
          avatar: 'https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=64&h=64&fit=crop&crop=face',
          isOnline: true,
          lastSeen: null
        }
      ],
      lastMessage: {
        id: 'msg_latest_003',
        senderId: 'SUPPORT_001',
        senderName: 'FastTrack Support',
        content: 'I\'ve contacted the driver and confirmed your delivery is next on the route. ETA is 15:45. You\'ll receive a tracking update shortly. Sorry for the delay!',
        type: 'text',
        timestamp: '2024-08-25T15:30:00Z',
        isRead: false,
        attachments: []
      },
      unreadCount: 1,
      updatedAt: '2024-08-25T15:30:00Z',
      status: 'resolved',
      tags: ['support', 'logistics', 'delivery'],
      metadata: {
        ticketId: 'TICK-789456',
        shipmentId: 'SHIP-001',
        priority: 'medium',
        category: 'delivery_delay'
      }
    },
    {
      id: 'conv_004',
      type: 'business',
      title: 'Partnership Opportunity - Thabo\'s Hardware',
      participants: [
        {
          id: 'USER_CURRENT',
          name: 'You',
          role: 'business_owner',
          avatar: null,
          isOnline: true
        },
        {
          id: 'USER_THABO',
          name: 'Thabo Mthembu',
          role: 'business_owner',
          avatar: 'https://images.unsplash.com/photo-1500648767791-00dcc994a43e?w=64&h=64&fit=crop&crop=face',
          isOnline: false,
          lastSeen: '2024-08-25T11:00:00Z'
        }
      ],
      lastMessage: {
        id: 'msg_latest_004',
        senderId: 'USER_THABO',
        senderName: 'Thabo Mthembu',
        content: 'I think there\'s a great opportunity for us to combine our customer bases. My construction clients often need bulk food supplies for their sites. What do you think about a referral partnership?',
        type: 'text',
        timestamp: '2024-08-25T11:00:00Z',
        isRead: true,
        attachments: []
      },
      unreadCount: 0,
      updatedAt: '2024-08-25T11:00:00Z',
      status: 'active',
      tags: ['partnership', 'business_development', 'collaboration'],
      metadata: {
        businessType: 'referral_partnership',
        potentialValue: 'high',
        category: 'cross_promotion'
      }
    },
    {
      id: 'conv_005',
      type: 'direct',
      title: 'Payment Overdue - Williams Construction',
      participants: [
        {
          id: 'USER_CURRENT',
          name: 'You',
          role: 'vendor',
          avatar: null,
          isOnline: true
        },
        {
          id: 'CUST-003',
          name: 'John Williams',
          role: 'customer',
          avatar: 'https://images.unsplash.com/photo-1560250097-0b93528c311a?w=64&h=64&fit=crop&crop=face',
          isOnline: false,
          lastSeen: '2024-08-24T16:30:00Z'
        }
      ],
      lastMessage: {
        id: 'msg_latest_005',
        senderId: 'USER_CURRENT',
        senderName: 'You',
        content: 'Hi John, just a friendly reminder that invoice INV-2024-003 for R120.52 was due 2 days ago. Could you please arrange payment when convenient? Thanks!',
        type: 'text',
        timestamp: '2024-08-25T09:00:00Z',
        isRead: true,
        attachments: [
          {
            id: 'att_001',
            name: 'INV-2024-003.pdf',
            type: 'document',
            size: '245KB',
            url: '/documents/INV-2024-003.pdf'
          }
        ]
      },
      unreadCount: 0,
      updatedAt: '2024-08-25T09:00:00Z',
      status: 'waiting_response',
      tags: ['payment', 'overdue', 'follow_up'],
      metadata: {
        invoiceId: 'INV-2024-003',
        amount: 120.52,
        daysPastDue: 2,
        priority: 'medium'
      }
    }
  ]

  // Filter conversations
  let filteredConversations = demoConversations

  if (type !== 'all') {
    filteredConversations = filteredConversations.filter(conv => conv.type === type)
  }

  if (unreadOnly === 'true') {
    filteredConversations = filteredConversations.filter(conv => conv.unreadCount > 0)
  }

  if (participantId) {
    filteredConversations = filteredConversations.filter(conv => 
      conv.participants.some(p => p.id === participantId)
    )
  }

  // Sort by last activity (most recent first)
  filteredConversations.sort((a, b) => new Date(b.updatedAt).getTime() - new Date(a.updatedAt).getTime())

  // Apply limit
  const limitedConversations = filteredConversations.slice(0, Number(limit))

  // Calculate summary statistics
  const stats = {
    total: filteredConversations.length,
    unread: demoConversations.filter(conv => conv.unreadCount > 0).length,
    byType: {
      direct: demoConversations.filter(conv => conv.type === 'direct').length,
      group: demoConversations.filter(conv => conv.type === 'group').length,
      support: demoConversations.filter(conv => conv.type === 'support').length,
      business: demoConversations.filter(conv => conv.type === 'business').length
    },
    byStatus: {
      active: demoConversations.filter(conv => conv.status === 'active').length,
      resolved: demoConversations.filter(conv => conv.status === 'resolved').length,
      waiting_response: demoConversations.filter(conv => conv.status === 'waiting_response').length,
      archived: demoConversations.filter(conv => conv.status === 'archived').length
    },
    totalUnreadMessages: demoConversations.reduce((sum, conv) => sum + conv.unreadCount, 0)
  }

  return {
    success: true,
    data: {
      conversations: limitedConversations,
      stats,
      pagination: {
        total: filteredConversations.length,
        limit: Number(limit),
        page: 1
      }
    }
  }
})
