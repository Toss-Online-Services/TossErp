export default defineEventHandler(async (event) => {
  const query = getQuery(event)
  const { status = 'all', type = '', limit = 20, search = '' } = query

  // Demo logistics and shipments data
  const demoShipments = [
    {
      id: 'SHIP-001',
      trackingNumber: 'TN789456123',
      type: 'delivery',
      status: 'in_transit',
      priority: 'standard',
      origin: {
        name: 'Your Warehouse',
        address: '123 Business Street, Alexandra, Johannesburg',
        coordinates: { lat: -26.1017, lng: 28.0875 },
        contactPerson: 'Warehouse Manager',
        contactPhone: '+27 11 123 4567'
      },
      destination: {
        name: 'Corner Café',
        address: '45 Main Road, Sandton, Johannesburg',
        coordinates: { lat: -26.1076, lng: 28.0567 },
        contactPerson: 'Sarah Johnson',
        contactPhone: '+27 82 555 0123'
      },
      items: [
        {
          productId: '1',
          name: 'Maize Meal (5kg)',
          quantity: 10,
          weight: 50,
          value: 120.00
        },
        {
          productId: '2',
          name: 'Rice (2kg)',
          quantity: 5,
          weight: 10,
          value: 75.50
        }
      ],
      timeline: {
        orderDate: '2024-08-25T08:00:00Z',
        pickupDate: '2024-08-25T10:30:00Z',
        estimatedDelivery: '2024-08-25T16:00:00Z',
        actualDelivery: null
      },
      carrier: {
        name: 'FastTrack Logistics',
        driver: 'Michael Sithole',
        vehicle: 'Truck - GP 123 ABC',
        phone: '+27 83 444 5555'
      },
      costs: {
        shipping: 85.00,
        insurance: 5.50,
        fuel: 15.00,
        total: 105.50
      },
      tracking: [
        {
          timestamp: '2024-08-25T08:00:00Z',
          status: 'order_created',
          location: 'Your Warehouse',
          message: 'Shipment order created and assigned to carrier'
        },
        {
          timestamp: '2024-08-25T10:30:00Z',
          status: 'picked_up',
          location: 'Your Warehouse',
          message: 'Items picked up by FastTrack Logistics'
        },
        {
          timestamp: '2024-08-25T12:15:00Z',
          status: 'in_transit',
          location: 'Halfway Point Depot',
          message: 'In transit to destination. ETA: 16:00'
        }
      ],
      specialInstructions: 'Handle with care - fragile items. Call customer 30 minutes before delivery.',
      insuranceValue: 195.50,
      signature: null,
      proofOfDelivery: null
    },
    {
      id: 'SHIP-002',
      trackingNumber: 'TN789456124',
      type: 'pickup',
      status: 'delivered',
      priority: 'express',
      origin: {
        name: 'Sunshine Suppliers',
        address: '78 Industrial Road, Germiston',
        coordinates: { lat: -26.2309, lng: 28.1772 },
        contactPerson: 'Peter Williams',
        contactPhone: '+27 11 987 6543'
      },
      destination: {
        name: 'Your Warehouse',
        address: '123 Business Street, Alexandra, Johannesburg',
        coordinates: { lat: -26.1017, lng: 28.0875 },
        contactPerson: 'Warehouse Manager',
        contactPhone: '+27 11 123 4567'
      },
      items: [
        {
          productId: '3',
          name: 'Cooking Oil (750ml)',
          quantity: 24,
          weight: 18,
          value: 456.00
        }
      ],
      timeline: {
        orderDate: '2024-08-24T14:00:00Z',
        pickupDate: '2024-08-24T16:30:00Z',
        estimatedDelivery: '2024-08-24T18:00:00Z',
        actualDelivery: '2024-08-24T17:45:00Z'
      },
      carrier: {
        name: 'Express Courier Services',
        driver: 'Jenny Mthembu',
        vehicle: 'Van - GP 456 DEF',
        phone: '+27 84 333 2222'
      },
      costs: {
        shipping: 120.00,
        insurance: 8.50,
        fuel: 20.00,
        total: 148.50
      },
      tracking: [
        {
          timestamp: '2024-08-24T14:00:00Z',
          status: 'order_created',
          location: 'Sunshine Suppliers',
          message: 'Express pickup order created'
        },
        {
          timestamp: '2024-08-24T16:30:00Z',
          status: 'picked_up',
          location: 'Sunshine Suppliers',
          message: 'Items collected from supplier'
        },
        {
          timestamp: '2024-08-24T17:45:00Z',
          status: 'delivered',
          location: 'Your Warehouse',
          message: 'Successfully delivered and signed for'
        }
      ],
      specialInstructions: 'Express delivery required for urgent restock',
      insuranceValue: 456.00,
      signature: 'Warehouse Manager - 24/08/2024 17:45',
      proofOfDelivery: 'POD-IMG-789456124.jpg'
    },
    {
      id: 'SHIP-003',
      trackingNumber: 'TN789456125',
      type: 'delivery',
      status: 'scheduled',
      priority: 'standard',
      origin: {
        name: 'Your Warehouse',
        address: '123 Business Street, Alexandra, Johannesburg',
        coordinates: { lat: -26.1017, lng: 28.0875 },
        contactPerson: 'Warehouse Manager',
        contactPhone: '+27 11 123 4567'
      },
      destination: {
        name: 'Williams Construction',
        address: '234 Builder Road, Roodepoort',
        coordinates: { lat: -26.1625, lng: 27.8717 },
        contactPerson: 'John Williams',
        contactPhone: '+27 82 777 8888'
      },
      items: [
        {
          productId: '4',
          name: 'Sugar (1kg)',
          quantity: 15,
          weight: 15,
          value: 67.50
        },
        {
          productId: '5',
          name: 'Rice (2kg)',
          quantity: 8,
          weight: 16,
          value: 120.52
        }
      ],
      timeline: {
        orderDate: '2024-08-25T09:00:00Z',
        pickupDate: '2024-08-26T08:00:00Z',
        estimatedDelivery: '2024-08-26T12:00:00Z',
        actualDelivery: null
      },
      carrier: {
        name: 'Reliable Transport',
        driver: 'TBD',
        vehicle: 'TBD',
        phone: '+27 11 555 6666'
      },
      costs: {
        shipping: 95.00,
        insurance: 6.00,
        fuel: 18.00,
        total: 119.00
      },
      tracking: [
        {
          timestamp: '2024-08-25T09:00:00Z',
          status: 'order_created',
          location: 'Your Warehouse',
          message: 'Delivery scheduled for tomorrow morning'
        }
      ],
      specialInstructions: 'Construction site delivery - call before arrival. Gate access required.',
      insuranceValue: 188.02,
      signature: null,
      proofOfDelivery: null
    },
    {
      id: 'SHIP-004',
      trackingNumber: 'TN789456126',
      type: 'return',
      status: 'processing',
      priority: 'standard',
      origin: {
        name: 'Local Restaurant Co.',
        address: '567 Food Street, Braamfontein',
        coordinates: { lat: -26.1929, lng: 28.0367 },
        contactPerson: 'Restaurant Manager',
        contactPhone: '+27 82 999 1111'
      },
      destination: {
        name: 'Your Warehouse',
        address: '123 Business Street, Alexandra, Johannesburg',
        coordinates: { lat: -26.1017, lng: 28.0875 },
        contactPerson: 'Returns Department',
        contactPhone: '+27 11 123 4567'
      },
      items: [
        {
          productId: '6',
          name: 'Damaged Flour Bags',
          quantity: 3,
          weight: 15,
          value: 85.00
        }
      ],
      timeline: {
        orderDate: '2024-08-25T11:00:00Z',
        pickupDate: null,
        estimatedDelivery: '2024-08-26T14:00:00Z',
        actualDelivery: null
      },
      carrier: {
        name: 'ReturnFast Logistics',
        driver: 'TBD',
        vehicle: 'TBD',
        phone: '+27 11 444 7777'
      },
      costs: {
        shipping: 0, // Free returns
        insurance: 0,
        fuel: 0,
        total: 0
      },
      tracking: [
        {
          timestamp: '2024-08-25T11:00:00Z',
          status: 'return_requested',
          location: 'Local Restaurant Co.',
          message: 'Return request submitted for damaged items'
        },
        {
          timestamp: '2024-08-25T14:00:00Z',
          status: 'processing',
          location: 'Returns Department',
          message: 'Return approved, scheduling pickup'
        }
      ],
      specialInstructions: 'Quality inspection required. Document damage for supplier claim.',
      insuranceValue: 85.00,
      signature: null,
      proofOfDelivery: null,
      returnReason: 'Damaged products - water damage during transport'
    }
  ]

  // Filter by status
  let filteredShipments = demoShipments
  if (status !== 'all') {
    filteredShipments = filteredShipments.filter(ship => ship.status === status)
  }

  // Filter by type
  if (type) {
    filteredShipments = filteredShipments.filter(ship => ship.type === type)
  }

  // Search filter
  if (search) {
    const searchLower = search.toLowerCase()
    filteredShipments = filteredShipments.filter(ship => 
      ship.trackingNumber.toLowerCase().includes(searchLower) ||
      ship.destination.name.toLowerCase().includes(searchLower) ||
      ship.carrier.name.toLowerCase().includes(searchLower)
    )
  }

  // Apply limit
  const limitedShipments = filteredShipments.slice(0, Number(limit))

  // Calculate summary statistics
  const stats = {
    total: filteredShipments.length,
    byStatus: {
      scheduled: demoShipments.filter(s => s.status === 'scheduled').length,
      in_transit: demoShipments.filter(s => s.status === 'in_transit').length,
      delivered: demoShipments.filter(s => s.status === 'delivered').length,
      processing: demoShipments.filter(s => s.status === 'processing').length
    },
    byType: {
      delivery: demoShipments.filter(s => s.type === 'delivery').length,
      pickup: demoShipments.filter(s => s.type === 'pickup').length,
      return: demoShipments.filter(s => s.type === 'return').length
    },
    totalValue: demoShipments.reduce((sum, ship) => sum + ship.items.reduce((itemSum, item) => itemSum + item.value, 0), 0),
    totalCosts: demoShipments.reduce((sum, ship) => sum + ship.costs.total, 0)
  }

  return {
    success: true,
    data: {
      shipments: limitedShipments,
      stats,
      pagination: {
        total: filteredShipments.length,
        limit: Number(limit),
        page: 1
      }
    }
  }
})
