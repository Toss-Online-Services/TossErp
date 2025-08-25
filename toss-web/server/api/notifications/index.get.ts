export default defineEventHandler(async (event) => {
  const query = getQuery(event)
  const { unreadOnly = false, limit = 20, type = '' } = query

  // Demo notifications data
  const demoNotifications = [
    {
      id: 'notif_1',
      type: 'inventory_alert',
      category: 'inventory',
      title: 'Low Stock Alert',
      message: 'Rice (2kg) is running low with only 3 units remaining. Minimum stock level is 10 units.',
      severity: 'warning',
      isRead: false,
      actionRequired: true,
      actionUrl: '/inventory/5',
      actionText: 'Reorder Now',
      metadata: {
        itemId: '5',
        itemName: 'Rice (2kg)',
        currentStock: 3,
        minimumStock: 10,
        suggestedOrder: 25
      },
      createdAt: '2024-08-25T08:00:00Z',
      readAt: null
    },
    {
      id: 'notif_2',
      type: 'group_purchase',
      category: 'collaboration',
      title: 'Group Purchase Opportunity',
      message: 'Join the bulk flour purchase organized by Alexandra Business Network. Save 18% with minimum 25kg order. Deadline in 3 days.',
      severity: 'info',
      isRead: false,
      actionRequired: true,
      actionUrl: '/group-buying/flour-purchase-aug-2024',
      actionText: 'Join Purchase',
      metadata: {
        purchaseId: 'gp_flour_001',
        organizer: 'Alexandra Business Network',
        savings: 18,
        deadline: '2024-08-28T23:59:59Z',
        minimumQuantity: 25,
        currentProgress: 76
      },
      createdAt: '2024-08-25T07:30:00Z',
      readAt: null
    },
    {
      id: 'notif_3',
      type: 'payment_reminder',
      category: 'finance',
      title: 'Payment Overdue',
      message: 'Williams Construction has an overdue payment of R120.52. Invoice INV-2024-003 was due 2 days ago.',
      severity: 'error',
      isRead: false,
      actionRequired: true,
      actionUrl: '/sales/INV-2024-003',
      actionText: 'Send Reminder',
      metadata: {
        customerId: 'CUST-003',
        customerName: 'Williams Construction',
        invoiceId: 'INV-2024-003',
        amount: 120.52,
        daysPastDue: 2
      },
      createdAt: '2024-08-25T07:00:00Z',
      readAt: null
    },
    {
      id: 'notif_4',
      type: 'ai_insight',
      category: 'insights',
      title: 'Business Performance Insight',
      message: 'Your sales have increased 23% this month! Consider expanding your product range to capitalize on growing demand.',
      severity: 'success',
      isRead: true,
      actionRequired: false,
      actionUrl: '/dashboard/insights',
      actionText: 'View Details',
      metadata: {
        salesGrowth: 23,
        period: 'month',
        recommendedActions: ['expand_product_range', 'increase_inventory']
      },
      createdAt: '2024-08-24T16:00:00Z',
      readAt: '2024-08-24T18:30:00Z'
    },
    {
      id: 'notif_5',
      type: 'tool_request',
      category: 'sharing',
      title: 'Tool Sharing Request',
      message: 'Thabo\'s Hardware wants to borrow your drill for the weekend. Proposed rental: R50/day.',
      severity: 'info',
      isRead: true,
      actionRequired: true,
      actionUrl: '/tools/requests/req_001',
      actionText: 'Respond',
      metadata: {
        requesterId: 'USER_THABO',
        requesterName: 'Thabo\'s Hardware',
        toolId: 'TOOL_DRILL_001',
        toolName: 'Cordless Drill',
        proposedRate: 50,
        startDate: '2024-08-26T08:00:00Z',
        endDate: '2024-08-28T18:00:00Z'
      },
      createdAt: '2024-08-24T14:00:00Z',
      readAt: '2024-08-24T15:15:00Z'
    },
    {
      id: 'notif_6',
      type: 'system_update',
      category: 'system',
      title: 'System Maintenance',
      message: 'Scheduled maintenance tonight from 02:00-04:00. All services will be temporarily unavailable.',
      severity: 'warning',
      isRead: false,
      actionRequired: false,
      actionUrl: '/system/maintenance',
      actionText: 'Learn More',
      metadata: {
        maintenanceStart: '2024-08-26T02:00:00Z',
        maintenanceEnd: '2024-08-26T04:00:00Z',
        affectedServices: ['api', 'web', 'mobile']
      },
      createdAt: '2024-08-24T12:00:00Z',
      readAt: null
    },
    {
      id: 'notif_7',
      type: 'new_feature',
      category: 'system',
      title: 'New Feature: Mobile Payments',
      message: 'You can now accept payments via SnapScan and Zapper! Enable mobile payments in your POS settings.',
      severity: 'success',
      isRead: false,
      actionRequired: false,
      actionUrl: '/settings/payments',
      actionText: 'Enable Now',
      metadata: {
        feature: 'mobile_payments',
        providers: ['SnapScan', 'Zapper'],
        estimatedImpact: '15% sales increase'
      },
      createdAt: '2024-08-24T10:00:00Z',
      readAt: null
    }
  ]

  // Filter notifications
  let filteredNotifications = demoNotifications

  if (unreadOnly === 'true') {
    filteredNotifications = filteredNotifications.filter(notif => !notif.isRead)
  }

  if (type) {
    filteredNotifications = filteredNotifications.filter(notif => notif.type === type)
  }

  // Apply limit
  const limitedNotifications = filteredNotifications.slice(0, Number(limit))

  // Calculate summary stats
  const totalNotifications = demoNotifications.length
  const unreadCount = demoNotifications.filter(n => !n.isRead).length
  const severityCounts = {
    error: demoNotifications.filter(n => n.severity === 'error').length,
    warning: demoNotifications.filter(n => n.severity === 'warning').length,
    info: demoNotifications.filter(n => n.severity === 'info').length,
    success: demoNotifications.filter(n => n.severity === 'success').length
  }
  
  const categoryCounts = {
    inventory: demoNotifications.filter(n => n.category === 'inventory').length,
    finance: demoNotifications.filter(n => n.category === 'finance').length,
    collaboration: demoNotifications.filter(n => n.category === 'collaboration').length,
    insights: demoNotifications.filter(n => n.category === 'insights').length,
    sharing: demoNotifications.filter(n => n.category === 'sharing').length,
    system: demoNotifications.filter(n => n.category === 'system').length
  }

  return {
    success: true,
    data: {
      notifications: limitedNotifications,
      summary: {
        total: totalNotifications,
        unread: unreadCount,
        filtered: filteredNotifications.length,
        severityCounts,
        categoryCounts
      }
    }
  }
})
