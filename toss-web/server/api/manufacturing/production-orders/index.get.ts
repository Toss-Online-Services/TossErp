export default defineEventHandler(async (event) => {
  const query = getQuery(event)
  const { status = 'all', productType = '', limit = 20, search = '' } = query

  // Demo manufacturing/production data
  const demoProductionOrders = [
    {
      id: 'PROD-001',
      orderNumber: 'PO-2024-001',
      productType: 'food_processing',
      product: {
        id: 'RECIPE-001',
        name: 'Custom Spice Blend - Corner Café',
        description: 'Special spice blend mixture for Corner Café\'s signature dishes',
        recipe: {
          ingredients: [
            { name: 'Paprika', quantity: 2.5, unit: 'kg', cost: 125.00 },
            { name: 'Cumin', quantity: 1.0, unit: 'kg', cost: 180.00 },
            { name: 'Coriander', quantity: 1.5, unit: 'kg', cost: 90.00 },
            { name: 'Black Pepper', quantity: 0.5, unit: 'kg', cost: 250.00 },
            { name: 'Turmeric', quantity: 0.8, unit: 'kg', cost: 95.00 }
          ],
          instructions: [
            'Dry roast coriander and cumin seeds separately',
            'Grind all spices to fine powder',
            'Mix according to proportions',
            'Sieve to ensure consistent texture',
            'Package in airtight containers'
          ],
          yieldQuantity: 6.3,
          yieldUnit: 'kg'
        }
      },
      customer: {
        id: 'CUST-001',
        name: 'Corner Café',
        contact: 'Sarah Johnson',
        specialRequirements: 'Halal certified, no MSG'
      },
      quantity: {
        ordered: 25, // units (6.3kg each = 157.5kg total)
        produced: 18,
        remaining: 7,
        unit: 'batches'
      },
      status: 'in_production',
      priority: 'medium',
      timeline: {
        orderDate: '2024-08-20T10:00:00Z',
        startDate: '2024-08-22T08:00:00Z',
        estimatedCompletion: '2024-08-27T17:00:00Z',
        actualCompletion: null,
        deliveryDate: '2024-08-28T10:00:00Z'
      },
      costs: {
        materials: 18375.00, // 25 batches × 735 per batch
        labor: 3500.00,
        overhead: 1200.00,
        packaging: 650.00,
        total: 23725.00,
        sellingPrice: 31250.00,
        profit: 7525.00,
        margin: 24.1
      },
      production: {
        batchSize: 1, // One recipe = one batch
        batchesPerDay: 4,
        currentBatch: 19,
        qualityChecks: [
          {
            batchNumber: 18,
            checkDate: '2024-08-25T14:00:00Z',
            inspector: 'Quality Controller',
            status: 'passed',
            notes: 'Texture and aroma excellent'
          }
        ],
        equipment: [
          { name: 'Industrial Grinder', status: 'in_use', nextMaintenance: '2024-09-01' },
          { name: 'Commercial Mixer', status: 'available', nextMaintenance: '2024-09-15' },
          { name: 'Packaging Machine', status: 'available', nextMaintenance: '2024-08-30' }
        ]
      },
      materials: {
        allocated: true,
        reservedStock: [
          { itemId: 'SPICE-001', name: 'Paprika', quantityReserved: 62.5, unit: 'kg' },
          { itemId: 'SPICE-002', name: 'Cumin', quantityReserved: 25.0, unit: 'kg' },
          { itemId: 'SPICE-003', name: 'Coriander', quantityReserved: 37.5, unit: 'kg' },
          { itemId: 'SPICE-004', name: 'Black Pepper', quantityReserved: 12.5, unit: 'kg' },
          { itemId: 'SPICE-005', name: 'Turmeric', quantityReserved: 20.0, unit: 'kg' }
        ]
      },
      tracking: [
        {
          timestamp: '2024-08-20T10:00:00Z',
          event: 'order_received',
          description: 'Production order created',
          user: 'Sales Team'
        },
        {
          timestamp: '2024-08-21T14:00:00Z',
          event: 'materials_allocated',
          description: 'All materials reserved from inventory',
          user: 'Production Planner'
        },
        {
          timestamp: '2024-08-22T08:00:00Z',
          event: 'production_started',
          description: 'First batch started',
          user: 'Production Supervisor'
        },
        {
          timestamp: '2024-08-25T15:00:00Z',
          event: 'milestone_reached',
          description: '18 batches completed (72% done)',
          user: 'Production Team'
        }
      ]
    },
    {
      id: 'PROD-002',
      orderNumber: 'PO-2024-002',
      productType: 'packaging',
      product: {
        id: 'PACKAGE-001',
        name: 'Branded Food Containers - Local Restaurant Co.',
        description: 'Custom printed food containers with restaurant branding',
        specifications: {
          material: 'Biodegradable cardboard',
          size: '750ml capacity',
          printing: '2-color logo print',
          finish: 'Food-safe coating'
        }
      },
      customer: {
        id: 'CUST-002',
        name: 'Local Restaurant Co.',
        contact: 'Restaurant Manager',
        specialRequirements: 'Eco-friendly materials only'
      },
      quantity: {
        ordered: 5000,
        produced: 2800,
        remaining: 2200,
        unit: 'pieces'
      },
      status: 'in_production',
      priority: 'high',
      timeline: {
        orderDate: '2024-08-18T09:00:00Z',
        startDate: '2024-08-21T07:00:00Z',
        estimatedCompletion: '2024-08-29T18:00:00Z',
        actualCompletion: null,
        deliveryDate: '2024-08-30T11:00:00Z'
      },
      costs: {
        materials: 8750.00,
        labor: 2800.00,
        overhead: 980.00,
        setup: 1500.00, // Printing setup costs
        total: 14030.00,
        sellingPrice: 18500.00,
        profit: 4470.00,
        margin: 24.2
      },
      production: {
        batchSize: 500,
        batchesPerDay: 6,
        currentBatch: 6,
        qualityChecks: [
          {
            batchNumber: 5,
            checkDate: '2024-08-25T11:00:00Z',
            inspector: 'Print Quality Inspector',
            status: 'passed',
            notes: 'Print alignment and color consistency good'
          }
        ],
        equipment: [
          { name: 'Printing Press', status: 'in_use', nextMaintenance: '2024-09-05' },
          { name: 'Die Cutting Machine', status: 'in_use', nextMaintenance: '2024-09-10' },
          { name: 'Folding Equipment', status: 'scheduled', nextMaintenance: '2024-08-28' }
        ]
      },
      materials: {
        allocated: true,
        reservedStock: [
          { itemId: 'CARDBOARD-001', name: 'Biodegradable Cardboard', quantityReserved: 250, unit: 'sheets' },
          { itemId: 'INK-001', name: 'Food-Safe Ink (Black)', quantityReserved: 2.5, unit: 'liters' },
          { itemId: 'INK-002', name: 'Food-Safe Ink (Red)', quantityReserved: 1.8, unit: 'liters' },
          { itemId: 'COATING-001', name: 'Food-Safe Coating', quantityReserved: 15.0, unit: 'liters' }
        ]
      },
      tracking: [
        {
          timestamp: '2024-08-18T09:00:00Z',
          event: 'order_received',
          description: 'Custom packaging order created',
          user: 'Sales Team'
        },
        {
          timestamp: '2024-08-19T15:00:00Z',
          event: 'design_approved',
          description: 'Customer approved final design proof',
          user: 'Design Team'
        },
        {
          timestamp: '2024-08-21T07:00:00Z',
          event: 'production_started',
          description: 'Printing setup completed, production started',
          user: 'Production Team'
        }
      ]
    },
    {
      id: 'PROD-003',
      orderNumber: 'PO-2024-003',
      productType: 'assembly',
      product: {
        id: 'ASSEMBLY-001',
        name: 'Emergency Food Kits - Williams Construction',
        description: 'Pre-assembled emergency food kits for construction site safety compliance',
        components: [
          { name: 'Instant Noodles', quantity: 4, unit: 'packets' },
          { name: 'Energy Bars', quantity: 6, unit: 'pieces' },
          { name: 'Dried Fruit Mix', quantity: 2, unit: 'packets' },
          { name: 'Water Purification Tablets', quantity: 10, unit: 'tablets' },
          { name: 'Emergency Blanket', quantity: 1, unit: 'piece' },
          { name: 'Waterproof Container', quantity: 1, unit: 'piece' }
        ]
      },
      customer: {
        id: 'CUST-003',
        name: 'Williams Construction',
        contact: 'John Williams',
        specialRequirements: 'Must meet safety regulations, 24-month shelf life'
      },
      quantity: {
        ordered: 100,
        produced: 100,
        remaining: 0,
        unit: 'kits'
      },
      status: 'completed',
      priority: 'medium',
      timeline: {
        orderDate: '2024-08-10T13:00:00Z',
        startDate: '2024-08-12T08:00:00Z',
        estimatedCompletion: '2024-08-18T17:00:00Z',
        actualCompletion: '2024-08-17T15:30:00Z',
        deliveryDate: '2024-08-19T09:00:00Z'
      },
      costs: {
        materials: 15750.00,
        labor: 1200.00,
        overhead: 420.00,
        packaging: 350.00,
        total: 17720.00,
        sellingPrice: 23500.00,
        profit: 5780.00,
        margin: 24.6
      },
      production: {
        batchSize: 20,
        batchesPerDay: 5,
        currentBatch: 5,
        qualityChecks: [
          {
            batchNumber: 5,
            checkDate: '2024-08-17T14:00:00Z',
            inspector: 'Final Assembly Inspector',
            status: 'passed',
            notes: 'All components present and properly sealed'
          }
        ],
        equipment: [
          { name: 'Assembly Workstation', status: 'available', nextMaintenance: '2024-09-01' },
          { name: 'Sealing Machine', status: 'available', nextMaintenance: '2024-08-30' }
        ]
      },
      materials: {
        allocated: false, // Materials consumed
        consumedStock: [
          { itemId: 'FOOD-001', name: 'Instant Noodles', quantityUsed: 400, unit: 'packets' },
          { itemId: 'FOOD-002', name: 'Energy Bars', quantityUsed: 600, unit: 'pieces' },
          { itemId: 'FOOD-003', name: 'Dried Fruit Mix', quantityUsed: 200, unit: 'packets' },
          { itemId: 'SAFETY-001', name: 'Water Purification Tablets', quantityUsed: 1000, unit: 'tablets' },
          { itemId: 'SAFETY-002', name: 'Emergency Blankets', quantityUsed: 100, unit: 'pieces' },
          { itemId: 'CONTAINER-001', name: 'Waterproof Containers', quantityUsed: 100, unit: 'pieces' }
        ]
      },
      tracking: [
        {
          timestamp: '2024-08-10T13:00:00Z',
          event: 'order_received',
          description: 'Emergency kit assembly order created',
          user: 'Sales Team'
        },
        {
          timestamp: '2024-08-11T10:00:00Z',
          event: 'materials_sourced',
          description: 'All components sourced and quality verified',
          user: 'Procurement Team'
        },
        {
          timestamp: '2024-08-12T08:00:00Z',
          event: 'assembly_started',
          description: 'Kit assembly process started',
          user: 'Assembly Team'
        },
        {
          timestamp: '2024-08-17T15:30:00Z',
          event: 'completed',
          description: 'All 100 kits assembled and quality checked',
          user: 'Production Supervisor'
        },
        {
          timestamp: '2024-08-19T09:00:00Z',
          event: 'delivered',
          description: 'Kits delivered to construction site',
          user: 'Logistics Team'
        }
      ]
    }
  ]

  // Filter by status
  let filteredOrders = demoProductionOrders
  if (status !== 'all') {
    filteredOrders = filteredOrders.filter(order => order.status === status)
  }

  // Filter by product type
  if (productType) {
    filteredOrders = filteredOrders.filter(order => order.productType === productType)
  }

  // Search filter
  if (search) {
    const searchLower = search.toLowerCase()
    filteredOrders = filteredOrders.filter(order => 
      order.product.name.toLowerCase().includes(searchLower) ||
      order.customer.name.toLowerCase().includes(searchLower) ||
      order.orderNumber.toLowerCase().includes(searchLower)
    )
  }

  // Apply limit
  const limitedOrders = filteredOrders.slice(0, Number(limit))

  // Calculate summary statistics
  const stats = {
    total: filteredOrders.length,
    byStatus: {
      planning: demoProductionOrders.filter(o => o.status === 'planning').length,
      in_production: demoProductionOrders.filter(o => o.status === 'in_production').length,
      quality_check: demoProductionOrders.filter(o => o.status === 'quality_check').length,
      completed: demoProductionOrders.filter(o => o.status === 'completed').length,
      on_hold: demoProductionOrders.filter(o => o.status === 'on_hold').length
    },
    byType: {
      food_processing: demoProductionOrders.filter(o => o.productType === 'food_processing').length,
      packaging: demoProductionOrders.filter(o => o.productType === 'packaging').length,
      assembly: demoProductionOrders.filter(o => o.productType === 'assembly').length
    },
    totals: {
      revenue: demoProductionOrders.reduce((sum, order) => sum + order.costs.sellingPrice, 0),
      costs: demoProductionOrders.reduce((sum, order) => sum + order.costs.total, 0),
      profit: demoProductionOrders.reduce((sum, order) => sum + order.costs.profit, 0)
    },
    averageMargin: Math.round(demoProductionOrders.reduce((sum, order) => sum + order.costs.margin, 0) / demoProductionOrders.length * 10) / 10
  }

  return {
    success: true,
    data: {
      productionOrders: limitedOrders,
      stats,
      pagination: {
        total: filteredOrders.length,
        limit: Number(limit),
        page: 1
      }
    }
  }
})
