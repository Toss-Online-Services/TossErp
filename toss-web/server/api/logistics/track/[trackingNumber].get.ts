export default defineEventHandler(async (event) => {
  const trackingNumber = getRouterParam(event, 'trackingNumber')
  
  if (!trackingNumber) {
    throw createError({
      statusCode: 400,
      statusMessage: 'Tracking number is required'
    })
  }

  // Demo tracking data - in real app, fetch from logistics API
  const demoTrackingData = {
    'TN789456123': {
      id: 'SHIP-001',
      trackingNumber: 'TN789456123',
      status: 'in_transit',
      currentLocation: {
        name: 'Halfway Point Depot',
        address: 'Highway Junction, Johannesburg',
        coordinates: { lat: -26.1250, lng: 28.0100 },
        timestamp: '2024-08-25T12:15:00Z'
      },
      estimatedDelivery: '2024-08-25T16:00:00Z',
      destination: {
        name: 'Corner Café',
        address: '45 Main Road, Sandton, Johannesburg'
      },
      carrier: {
        name: 'FastTrack Logistics',
        driver: 'Michael Sithole',
        phone: '+27 83 444 5555'
      },
      timeline: [
        {
          timestamp: '2024-08-25T08:00:00Z',
          status: 'order_created',
          location: 'Your Warehouse',
          message: 'Shipment order created and assigned to carrier',
          icon: 'package'
        },
        {
          timestamp: '2024-08-25T10:30:00Z',
          status: 'picked_up',
          location: 'Your Warehouse',
          message: 'Items picked up by FastTrack Logistics',
          icon: 'truck'
        },
        {
          timestamp: '2024-08-25T12:15:00Z',
          status: 'in_transit',
          location: 'Halfway Point Depot',
          message: 'In transit to destination. ETA: 16:00',
          icon: 'route',
          current: true
        },
        {
          timestamp: null,
          status: 'out_for_delivery',
          location: 'Local Distribution Center',
          message: 'Out for delivery - final leg',
          icon: 'delivery',
          estimated: '2024-08-25T15:30:00Z'
        },
        {
          timestamp: null,
          status: 'delivered',
          location: 'Corner Café',
          message: 'Package delivered',
          icon: 'check',
          estimated: '2024-08-25T16:00:00Z'
        }
      ],
      items: [
        { name: 'Maize Meal (5kg)', quantity: 10 },
        { name: 'Rice (2kg)', quantity: 5 }
      ],
      specialInstructions: 'Handle with care - fragile items. Call customer 30 minutes before delivery.',
      nextUpdate: '2024-08-25T14:00:00Z'
    },
    'TN789456124': {
      id: 'SHIP-002',
      trackingNumber: 'TN789456124',
      status: 'delivered',
      currentLocation: {
        name: 'Your Warehouse',
        address: '123 Business Street, Alexandra, Johannesburg',
        coordinates: { lat: -26.1017, lng: 28.0875 },
        timestamp: '2024-08-24T17:45:00Z'
      },
      estimatedDelivery: '2024-08-24T18:00:00Z',
      destination: {
        name: 'Your Warehouse',
        address: '123 Business Street, Alexandra, Johannesburg'
      },
      carrier: {
        name: 'Express Courier Services',
        driver: 'Jenny Mthembu',
        phone: '+27 84 333 2222'
      },
      timeline: [
        {
          timestamp: '2024-08-24T14:00:00Z',
          status: 'order_created',
          location: 'Sunshine Suppliers',
          message: 'Express pickup order created',
          icon: 'package'
        },
        {
          timestamp: '2024-08-24T16:30:00Z',
          status: 'picked_up',
          location: 'Sunshine Suppliers',
          message: 'Items collected from supplier',
          icon: 'truck'
        },
        {
          timestamp: '2024-08-24T17:45:00Z',
          status: 'delivered',
          location: 'Your Warehouse',
          message: 'Successfully delivered and signed for',
          icon: 'check',
          current: true
        }
      ],
      items: [
        { name: 'Cooking Oil (750ml)', quantity: 24 }
      ],
      signature: 'Warehouse Manager - 24/08/2024 17:45',
      proofOfDelivery: 'POD-IMG-789456124.jpg',
      deliveryConfirmation: {
        signedBy: 'Warehouse Manager',
        signedAt: '2024-08-24T17:45:00Z',
        notes: 'All items received in good condition'
      }
    }
  }

  const tracking = demoTrackingData[trackingNumber]
  
  if (!tracking) {
    throw createError({
      statusCode: 404,
      statusMessage: 'Tracking number not found'
    })
  }

  // Calculate progress percentage
  const completedSteps = tracking.timeline.filter(step => step.timestamp).length
  const totalSteps = tracking.timeline.length
  const progressPercentage = Math.round((completedSteps / totalSteps) * 100)

  // Determine if delivery is delayed
  const now = new Date()
  const estimatedDelivery = new Date(tracking.estimatedDelivery)
  const isDelayed = tracking.status !== 'delivered' && now > estimatedDelivery

  return {
    success: true,
    data: {
      ...tracking,
      progress: {
        percentage: progressPercentage,
        completedSteps,
        totalSteps,
        isDelayed,
        delayMinutes: isDelayed ? Math.round((now.getTime() - estimatedDelivery.getTime()) / (1000 * 60)) : 0
      },
      lastUpdated: new Date().toISOString()
    }
  }
})
