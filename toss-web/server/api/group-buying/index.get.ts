export default defineEventHandler(async (event) => {
  const query = getQuery(event)
  const { status = 'all', category = '', limit = 20, search = '' } = query

  // Demo group buying opportunities
  const demoGroupPurchases = [
    {
      id: 'gp_flour_001',
      title: 'Premium Flour Bulk Purchase',
      description: 'High-quality premium flour for bakeries and restaurants. Certified organic, stone-ground.',
      organizer: {
        id: 'USER_ALEXANDRA',
        name: 'Alexandra Business Network',
        rating: 4.8,
        completedPurchases: 23,
        trustScore: 95
      },
      product: {
        name: 'Premium Organic Flour',
        brand: 'Mill & Stone Co.',
        specifications: '25kg bags, stone-ground, organic certified',
        unitPrice: 450, // Regular price per 25kg bag
        groupPrice: 370, // Group purchase price
        savings: 18 // Percentage savings
      },
      requirements: {
        minimumQuantity: 100, // Total bags needed for group price
        minimumParticipants: 5,
        maximumQuantity: 500,
        unitSize: '25kg bag'
      },
      progress: {
        currentQuantity: 76,
        currentParticipants: 12,
        progressPercentage: 76
      },
      timeline: {
        createdAt: '2024-08-20T10:00:00Z',
        deadline: '2024-08-28T23:59:59Z',
        deliveryDate: '2024-09-02T09:00:00Z',
        paymentDue: '2024-08-30T23:59:59Z'
      },
      status: 'active',
      category: 'food_ingredients',
      logistics: {
        deliveryMethod: 'centralized_pickup',
        location: 'Alexandra Business Hub, 45 1st Avenue',
        distributionCost: 25, // per participant
        coordinates: { lat: -26.1017, lng: 28.0875 }
      },
      participants: [
        { name: 'Sunshine Bakery', quantity: 20, joinedAt: '2024-08-21T08:00:00Z' },
        { name: 'Corner Café', quantity: 8, joinedAt: '2024-08-21T14:30:00Z' },
        { name: 'Artisan Breads', quantity: 25, joinedAt: '2024-08-22T11:15:00Z' },
        { name: 'Local Restaurant Co.', quantity: 15, joinedAt: '2024-08-23T16:45:00Z' }
      ],
      terms: {
        paymentTerms: 'Full payment required 2 days before delivery',
        cancellationPolicy: 'Free cancellation up to 48 hours before payment due',
        qualityGuarantee: '100% satisfaction guaranteed or full refund',
        liability: 'Organizer responsible for quality assurance'
      }
    },
    {
      id: 'gp_sugar_002',
      title: 'White Sugar - Commercial Grade',
      description: 'Bulk purchase of commercial-grade white sugar for restaurants, cafes, and food businesses.',
      organizer: {
        id: 'USER_MARCUS',
        name: 'Marcus Food Distribution',
        rating: 4.6,
        completedPurchases: 18,
        trustScore: 89
      },
      product: {
        name: 'Commercial White Sugar',
        brand: 'Sweet Valley',
        specifications: '50kg bags, refined sugar, food grade',
        unitPrice: 680,
        groupPrice: 595,
        savings: 12.5
      },
      requirements: {
        minimumQuantity: 50,
        minimumParticipants: 3,
        maximumQuantity: 200,
        unitSize: '50kg bag'
      },
      progress: {
        currentQuantity: 34,
        currentParticipants: 7,
        progressPercentage: 68
      },
      timeline: {
        createdAt: '2024-08-22T14:00:00Z',
        deadline: '2024-08-30T23:59:59Z',
        deliveryDate: '2024-09-05T10:00:00Z',
        paymentDue: '2024-09-01T23:59:59Z'
      },
      status: 'active',
      category: 'food_ingredients',
      logistics: {
        deliveryMethod: 'direct_delivery',
        location: 'Various locations within 25km radius',
        distributionCost: 45,
        coordinates: { lat: -26.2041, lng: 28.0473 }
      },
      participants: [
        { name: 'City Diner', quantity: 10, joinedAt: '2024-08-22T15:00:00Z' },
        { name: 'Sweet Treats Bakery', quantity: 15, joinedAt: '2024-08-23T09:30:00Z' },
        { name: 'Coffee & More', quantity: 5, joinedAt: '2024-08-24T12:00:00Z' }
      ],
      terms: {
        paymentTerms: 'Payment due upon delivery confirmation',
        cancellationPolicy: 'Cancellation fee applies after minimum reached',
        qualityGuarantee: 'Quality guaranteed as per supplier standards',
        liability: 'Shared liability among participants'
      }
    },
    {
      id: 'gp_oil_003',
      title: 'Cooking Oil - Sunflower',
      description: 'Bulk sunflower cooking oil purchase for commercial kitchens and food service businesses.',
      organizer: {
        id: 'USER_FATIMA',
        name: 'Fatima\'s Restaurant Supply',
        rating: 4.9,
        completedPurchases: 31,
        trustScore: 98
      },
      product: {
        name: 'Pure Sunflower Oil',
        brand: 'Golden Fields',
        specifications: '20L containers, cold-pressed, non-GMO',
        unitPrice: 320,
        groupPrice: 275,
        savings: 14.1
      },
      requirements: {
        minimumQuantity: 40,
        minimumParticipants: 4,
        maximumQuantity: 120,
        unitSize: '20L container'
      },
      progress: {
        currentQuantity: 42,
        currentParticipants: 9,
        progressPercentage: 105 // Exceeded minimum
      },
      timeline: {
        createdAt: '2024-08-18T09:00:00Z',
        deadline: '2024-08-26T23:59:59Z',
        deliveryDate: '2024-08-29T08:00:00Z',
        paymentDue: '2024-08-27T23:59:59Z'
      },
      status: 'confirmed', // Minimum reached, now confirmed
      category: 'food_ingredients',
      logistics: {
        deliveryMethod: 'centralized_pickup',
        location: 'Fatima\'s Warehouse, 123 Industrial Road',
        distributionCost: 15,
        coordinates: { lat: -26.1849, lng: 28.0653 }
      },
      participants: [
        { name: 'Golden Spoon Restaurant', quantity: 12, joinedAt: '2024-08-18T10:00:00Z' },
        { name: 'Pizza Palace', quantity: 8, joinedAt: '2024-08-19T14:00:00Z' },
        { name: 'Family Kitchen', quantity: 6, joinedAt: '2024-08-20T11:00:00Z' },
        { name: 'Taste of Home', quantity: 10, joinedAt: '2024-08-21T16:00:00Z' }
      ],
      terms: {
        paymentTerms: 'Payment required within 24 hours of confirmation',
        cancellationPolicy: 'No cancellation after confirmation',
        qualityGuarantee: 'Fresh product guarantee with expiry date check',
        liability: 'Organizer assumes full liability for product quality'
      }
    },
    {
      id: 'gp_packaging_004',
      title: 'Food-Grade Packaging Supplies',
      description: 'Biodegradable food containers and packaging materials for takeaway and delivery services.',
      organizer: {
        id: 'USER_THABO',
        name: 'Thabo\'s Business Solutions',
        rating: 4.5,
        completedPurchases: 12,
        trustScore: 85
      },
      product: {
        name: 'Eco-Friendly Food Containers',
        brand: 'Green Pack',
        specifications: 'Mixed sizes, biodegradable, leak-proof',
        unitPrice: 850, // Per case of 500 containers
        groupPrice: 715,
        savings: 15.9
      },
      requirements: {
        minimumQuantity: 20, // Cases
        minimumParticipants: 5,
        maximumQuantity: 100,
        unitSize: 'Case (500 containers)'
      },
      progress: {
        currentQuantity: 8,
        currentParticipants: 3,
        progressPercentage: 40
      },
      timeline: {
        createdAt: '2024-08-24T13:00:00Z',
        deadline: '2024-09-05T23:59:59Z',
        deliveryDate: '2024-09-10T11:00:00Z',
        paymentDue: '2024-09-07T23:59:59Z'
      },
      status: 'active',
      category: 'packaging',
      logistics: {
        deliveryMethod: 'mixed', // Some pickup, some delivery
        location: 'Various options available',
        distributionCost: 35,
        coordinates: { lat: -26.1368, lng: 28.0881 }
      },
      participants: [
        { name: 'Quick Bites', quantity: 3, joinedAt: '2024-08-24T14:00:00Z' },
        { name: 'Healthy Meals Co.', quantity: 4, joinedAt: '2024-08-25T10:00:00Z' }
      ],
      terms: {
        paymentTerms: 'COD or advance payment options available',
        cancellationPolicy: 'Full refund if minimum not reached',
        qualityGuarantee: 'Quality samples available for inspection',
        liability: 'Standard commercial liability terms'
      }
    }
  ]

  // Filter by status
  let filteredPurchases = demoGroupPurchases
  if (status !== 'all') {
    filteredPurchases = filteredPurchases.filter(gp => gp.status === status)
  }

  // Filter by category
  if (category) {
    filteredPurchases = filteredPurchases.filter(gp => gp.category === category)
  }

  // Search filter
  if (search) {
    const searchLower = search.toLowerCase()
    filteredPurchases = filteredPurchases.filter(gp => 
      gp.title.toLowerCase().includes(searchLower) ||
      gp.description.toLowerCase().includes(searchLower) ||
      gp.product.name.toLowerCase().includes(searchLower)
    )
  }

  // Apply limit
  const limitedPurchases = filteredPurchases.slice(0, Number(limit))

  // Calculate summary statistics
  const stats = {
    total: filteredPurchases.length,
    active: demoGroupPurchases.filter(gp => gp.status === 'active').length,
    confirmed: demoGroupPurchases.filter(gp => gp.status === 'confirmed').length,
    completed: demoGroupPurchases.filter(gp => gp.status === 'completed').length,
    totalSavings: demoGroupPurchases.reduce((sum, gp) => sum + gp.product.savings, 0) / demoGroupPurchases.length,
    categories: {
      food_ingredients: demoGroupPurchases.filter(gp => gp.category === 'food_ingredients').length,
      packaging: demoGroupPurchases.filter(gp => gp.category === 'packaging').length,
      equipment: demoGroupPurchases.filter(gp => gp.category === 'equipment').length,
      supplies: demoGroupPurchases.filter(gp => gp.category === 'supplies').length
    }
  }

  return {
    success: true,
    data: {
      groupPurchases: limitedPurchases,
      stats,
      pagination: {
        total: filteredPurchases.length,
        limit: Number(limit),
        page: 1
      }
    }
  }
})
