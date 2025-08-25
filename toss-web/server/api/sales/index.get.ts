export default defineEventHandler(async (event) => {
  const query = getQuery(event)
  const { page = 1, limit = 20, status = '', customerId = '', startDate = '', endDate = '' } = query

  // Demo sales data
  const demoSales = [
    {
      id: 'INV-2024-001',
      invoiceNumber: 'INV-2024-001',
      customerId: 'CUST-001',
      customerName: 'Sipho Mthembu',
      customerEmail: 'sipho@email.com',
      customerPhone: '+27 82 123 4567',
      saleDate: '2024-08-24T10:30:00Z',
      dueDate: '2024-09-07T23:59:59Z',
      status: 'paid',
      paymentMethod: 'cash',
      paymentStatus: 'completed',
      subtotal: 156.50,
      taxAmount: 22.98,
      discountAmount: 0,
      totalAmount: 179.48,
      paidAmount: 179.48,
      balanceAmount: 0,
      items: [
        {
          id: '1',
          name: 'Maize Meal (1kg)',
          sku: 'MM001',
          quantity: 3,
          unitPrice: 22.99,
          totalPrice: 68.97
        },
        {
          id: '2',
          name: 'Sunflower Oil (750ml)',
          sku: 'SO002',
          quantity: 2,
          unitPrice: 39.99,
          totalPrice: 79.98
        },
        {
          id: '4',
          name: 'Coca Cola (500ml)',
          sku: 'CC004',
          quantity: 1,
          unitPrice: 14.99,
          totalPrice: 14.99
        }
      ],
      notes: 'Regular customer - monthly grocery shopping',
      salesPersonId: 'USER-001',
      salesPersonName: 'Nomsa Dlamini',
      createdAt: '2024-08-24T10:30:00Z',
      updatedAt: '2024-08-24T10:35:00Z'
    },
    {
      id: 'INV-2024-002',
      invoiceNumber: 'INV-2024-002',
      customerId: 'CUST-002',
      customerName: 'Mary Johnson',
      customerEmail: 'mary@email.com',
      customerPhone: '+27 71 987 6543',
      saleDate: '2024-08-24T14:15:00Z',
      dueDate: '2024-09-07T23:59:59Z',
      status: 'pending',
      paymentMethod: 'credit',
      paymentStatus: 'pending',
      subtotal: 89.97,
      taxAmount: 13.50,
      discountAmount: 5.00,
      totalAmount: 98.47,
      paidAmount: 0,
      balanceAmount: 98.47,
      items: [
        {
          id: '3',
          name: 'Bread (White Loaf)',
          sku: 'BR003',
          quantity: 2,
          unitPrice: 16.99,
          totalPrice: 33.98
        },
        {
          id: '5',
          name: 'Rice (2kg)',
          sku: 'RC005',
          quantity: 1,
          unitPrice: 65.99,
          totalPrice: 65.99
        }
      ],
      notes: 'Credit customer - 30 days payment terms',
      salesPersonId: 'USER-002',
      salesPersonName: 'Thabo Molefe',
      createdAt: '2024-08-24T14:15:00Z',
      updatedAt: '2024-08-24T14:20:00Z'
    },
    {
      id: 'INV-2024-003',
      invoiceNumber: 'INV-2024-003',
      customerId: 'CUST-003',
      customerName: 'David Williams',
      customerEmail: 'david@email.com',
      customerPhone: '+27 83 456 7890',
      saleDate: '2024-08-23T16:45:00Z',
      dueDate: '2024-09-06T23:59:59Z',
      status: 'partially_paid',
      paymentMethod: 'card',
      paymentStatus: 'partial',
      subtotal: 245.89,
      taxAmount: 36.88,
      discountAmount: 12.25,
      totalAmount: 270.52,
      paidAmount: 150.00,
      balanceAmount: 120.52,
      items: [
        {
          id: '1',
          name: 'Maize Meal (1kg)',
          sku: 'MM001',
          quantity: 5,
          unitPrice: 22.99,
          totalPrice: 114.95
        },
        {
          id: '2',
          name: 'Sunflower Oil (750ml)',
          sku: 'SO002',
          quantity: 3,
          unitPrice: 39.99,
          totalPrice: 119.97
        },
        {
          id: '4',
          name: 'Coca Cola (500ml)',
          sku: 'CC004',
          quantity: 4,
          unitPrice: 14.99,
          totalPrice: 59.96
        }
      ],
      notes: 'Bulk purchase - regular business customer',
      salesPersonId: 'USER-001',
      salesPersonName: 'Nomsa Dlamini',
      createdAt: '2024-08-23T16:45:00Z',
      updatedAt: '2024-08-24T09:15:00Z'
    }
  ]

  // Filter sales based on query parameters
  let filteredSales = demoSales

  if (status) {
    filteredSales = filteredSales.filter(sale => sale.status === status)
  }

  if (customerId) {
    filteredSales = filteredSales.filter(sale => sale.customerId === customerId)
  }

  if (startDate) {
    filteredSales = filteredSales.filter(sale => 
      new Date(sale.saleDate) >= new Date(startDate.toString())
    )
  }

  if (endDate) {
    filteredSales = filteredSales.filter(sale => 
      new Date(sale.saleDate) <= new Date(endDate.toString())
    )
  }

  // Calculate pagination
  const totalItems = filteredSales.length
  const totalPages = Math.ceil(totalItems / Number(limit))
  const startIndex = (Number(page) - 1) * Number(limit)
  const endIndex = startIndex + Number(limit)
  const paginatedSales = filteredSales.slice(startIndex, endIndex)

  // Calculate summary statistics
  const totalSalesAmount = filteredSales.reduce((sum, sale) => sum + sale.totalAmount, 0)
  const totalPaidAmount = filteredSales.reduce((sum, sale) => sum + sale.paidAmount, 0)
  const totalOutstandingAmount = filteredSales.reduce((sum, sale) => sum + sale.balanceAmount, 0)
  const averageOrderValue = filteredSales.length > 0 ? totalSalesAmount / filteredSales.length : 0

  const statusCounts = {
    paid: filteredSales.filter(s => s.status === 'paid').length,
    pending: filteredSales.filter(s => s.status === 'pending').length,
    partially_paid: filteredSales.filter(s => s.status === 'partially_paid').length,
    cancelled: filteredSales.filter(s => s.status === 'cancelled').length
  }

  return {
    success: true,
    data: {
      sales: paginatedSales,
      pagination: {
        currentPage: Number(page),
        totalPages,
        totalItems,
        itemsPerPage: Number(limit),
        hasNextPage: Number(page) < totalPages,
        hasPreviousPage: Number(page) > 1
      },
      summary: {
        totalSales: filteredSales.length,
        totalSalesAmount,
        totalPaidAmount,
        totalOutstandingAmount,
        averageOrderValue,
        statusCounts
      }
    }
  }
})
