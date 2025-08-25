export default defineEventHandler(async (event) => {
  const query = getQuery(event)
  const { period = '30d', module = 'all', metric = 'overview' } = query

  // Calculate date range based on period
  const now = new Date()
  const periodMap = {
    '7d': 7,
    '30d': 30,
    '90d': 90,
    '1y': 365
  }
  const days = periodMap[period] || 30
  const startDate = new Date(now.getTime() - (days * 24 * 60 * 60 * 1000))

  // Demo analytics data
  const demoAnalytics = {
    overview: {
      summary: {
        period: `${days} days`,
        startDate: startDate.toISOString(),
        endDate: now.toISOString(),
        currency: 'ZAR'
      },
      kpis: {
        totalRevenue: 145320.50,
        totalCosts: 98745.30,
        netProfit: 46575.20,
        profitMargin: 32.1,
        avgOrderValue: 1453.21,
        customerGrowth: 18.5,
        inventoryTurnover: 2.8,
        cashFlow: 52890.40
      },
      trends: {
        revenue: [
          { date: '2024-08-01', value: 4200.00 },
          { date: '2024-08-02', value: 3850.00 },
          { date: '2024-08-03', value: 5100.00 },
          { date: '2024-08-04', value: 4650.00 },
          { date: '2024-08-05', value: 6200.00 },
          { date: '2024-08-06', value: 5800.00 },
          { date: '2024-08-07', value: 4300.00 },
          { date: '2024-08-08', value: 4950.00 },
          { date: '2024-08-09', value: 5450.00 },
          { date: '2024-08-10', value: 6100.00 }
        ],
        orders: [
          { date: '2024-08-01', value: 12 },
          { date: '2024-08-02', value: 8 },
          { date: '2024-08-03', value: 15 },
          { date: '2024-08-04', value: 11 },
          { date: '2024-08-05', value: 18 },
          { date: '2024-08-06', value: 16 },
          { date: '2024-08-07', value: 9 },
          { date: '2024-08-08', value: 13 },
          { date: '2024-08-09', value: 14 },
          { date: '2024-08-10', value: 17 }
        ],
        customers: [
          { date: '2024-08-01', value: 45 },
          { date: '2024-08-02', value: 47 },
          { date: '2024-08-03', value: 48 },
          { date: '2024-08-04', value: 49 },
          { date: '2024-08-05', value: 52 },
          { date: '2024-08-06', value: 53 },
          { date: '2024-08-07', value: 54 },
          { date: '2024-08-08', value: 55 },
          { date: '2024-08-09', value: 57 },
          { date: '2024-08-10', value: 58 }
        ]
      }
    },
    sales: {
      performance: {
        totalSales: 145320.50,
        avgDailySales: 4844.02,
        topSalesDay: { date: '2024-08-05', amount: 6200.00 },
        salesGrowth: 23.4, // % vs previous period
        conversionRate: 68.5
      },
      products: {
        topSelling: [
          { name: 'Maize Meal (5kg)', quantity: 245, revenue: 29400.00, growth: 15.2 },
          { name: 'Rice (2kg)', quantity: 189, revenue: 28490.50, growth: 8.7 },
          { name: 'Cooking Oil (750ml)', quantity: 156, revenue: 23712.00, growth: 22.1 },
          { name: 'Sugar (1kg)', quantity: 234, revenue: 10530.00, growth: -5.3 },
          { name: 'Flour (2.5kg)', quantity: 87, revenue: 17835.00, growth: 31.8 }
        ],
        categories: [
          { name: 'Staples', revenue: 89467.50, percentage: 61.6 },
          { name: 'Cooking Oils', revenue: 35780.00, percentage: 24.6 },
          { name: 'Spices & Seasonings', revenue: 12890.00, percentage: 8.9 },
          { name: 'Other', revenue: 7183.00, percentage: 4.9 }
        ]
      },
      customers: {
        topCustomers: [
          { name: 'Corner Café', orders: 18, revenue: 32450.00, growth: 28.5 },
          { name: 'Williams Construction', orders: 12, revenue: 24680.00, growth: 15.2 },
          { name: 'Local Restaurant Co.', orders: 15, revenue: 28970.00, growth: 22.1 },
          { name: 'Sunshine Bakery', orders: 8, revenue: 18320.00, growth: 45.6 }
        ],
        segments: [
          { type: 'Business', count: 23, revenue: 98420.00, percentage: 67.7 },
          { type: 'Individual', count: 35, revenue: 46900.50, percentage: 32.3 }
        ]
      }
    },
    inventory: {
      performance: {
        totalValue: 285600.00,
        turnoverRate: 2.8,
        averageAge: 18.5, // days
        stockAccuracy: 96.8,
        outOfStockEvents: 3
      },
      lowStock: [
        { name: 'Rice (2kg)', currentStock: 3, minimumStock: 10, daysUntilOut: 2 },
        { name: 'Premium Flour', currentStock: 8, minimumStock: 15, daysUntilOut: 5 },
        { name: 'Turmeric Powder', currentStock: 12, minimumStock: 20, daysUntilOut: 8 }
      ],
      fastMoving: [
        { name: 'Maize Meal (5kg)', avgDailySales: 8.2, velocity: 'high' },
        { name: 'Cooking Oil (750ml)', avgDailySales: 5.2, velocity: 'high' },
        { name: 'Rice (2kg)', avgDailySales: 6.3, velocity: 'medium' },
        { name: 'Sugar (1kg)', avgDailySales: 7.8, velocity: 'medium' }
      ],
      movements: {
        totalIn: 1450, // units received
        totalOut: 1285, // units sold
        adjustments: -8, // shrinkage, damage, etc.
        netChange: 157
      }
    },
    finance: {
      cashFlow: {
        inflows: {
          sales: 145320.50,
          groupPurchases: 8500.00,
          otherIncome: 2150.00,
          total: 155970.50
        },
        outflows: {
          inventory: 89400.00,
          salaries: 28500.00,
          rent: 12000.00,
          utilities: 3200.00,
          transport: 4850.00,
          other: 5680.00,
          total: 143630.00
        },
        netCashFlow: 12340.50
      },
      receivables: {
        total: 45680.00,
        current: 32450.00, // 0-30 days
        overdue30: 8950.00, // 31-60 days
        overdue60: 3280.00, // 61-90 days
        overdue90: 1000.00, // 90+ days
        averageCollectionPeriod: 28.5 // days
      },
      payables: {
        total: 28900.00,
        current: 22100.00,
        overdue: 6800.00,
        averagePaymentPeriod: 32.1 // days
      }
    },
    groupBuying: {
      participation: {
        activeGroups: 3,
        totalCommitments: 12450.00,
        estimatedSavings: 1850.00,
        savingsPercentage: 12.9
      },
      groups: [
        { name: 'Flour Purchase', progress: 76, savings: 18, yourCommitment: 4625.00 },
        { name: 'Sugar Purchase', progress: 68, savings: 12.5, yourCommitment: 5950.00 },
        { name: 'Packaging Supplies', progress: 40, savings: 15.9, yourCommitment: 1875.00 }
      ],
      impact: {
        totalSavingsToDate: 15680.00,
        averageSavingsPerOrder: 385.50,
        purchaseVolumeIncrease: 45.2 // % increase in buying power
      }
    },
    ai: {
      insights: [
        {
          type: 'opportunity',
          title: 'Inventory Optimization',
          description: 'Your rice stock is low but sales velocity is high. Consider increasing order quantity by 40% to reduce stockout risk.',
          impact: 'medium',
          actionRequired: true
        },
        {
          type: 'trend',
          title: 'Sales Growth Pattern',
          description: 'Sales have increased 23% this month, with strongest growth in cooking oils category. Consider expanding product range.',
          impact: 'high',
          actionRequired: false
        },
        {
          type: 'efficiency',
          title: 'Group Buying Potential',
          description: 'Based on your purchase patterns, joining the upcoming cooking oil group purchase could save you R1,250.',
          impact: 'medium',
          actionRequired: true
        }
      ],
      predictions: {
        nextWeekSales: 32450.00,
        confidence: 87.5,
        stockoutRisk: [
          { product: 'Rice (2kg)', probability: 85 },
          { product: 'Premium Flour', probability: 45 }
        ],
        optimalReorderPoints: [
          { product: 'Maize Meal (5kg)', current: 15, recommended: 20 },
          { product: 'Cooking Oil (750ml)', current: 12, recommended: 18 }
        ]
      }
    }
  }

  // Filter data based on module
  let responseData = demoAnalytics
  if (module !== 'all') {
    responseData = { [module]: demoAnalytics[module] }
  }

  // Filter by specific metric if requested
  if (metric !== 'overview' && demoAnalytics[metric]) {
    responseData = { [metric]: demoAnalytics[metric] }
  }

  return {
    success: true,
    data: {
      analytics: responseData,
      metadata: {
        period,
        module,
        metric,
        generatedAt: new Date().toISOString(),
        dataFreshness: 'real-time', // In demo, would show actual data age
        lastUpdated: new Date(now.getTime() - 5 * 60 * 1000).toISOString() // 5 minutes ago
      }
    }
  }
})
