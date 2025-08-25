export default defineEventHandler(async (event) => {
  const query = getQuery(event)
  const { page = 1, limit = 20, search = '', status = 'active' } = query

  // Demo customers data
  const demoCustomers = [
    {
      id: 'CUST-001',
      customerNumber: 'CUST-001',
      type: 'individual',
      firstName: 'Sipho',
      lastName: 'Mthembu',
      companyName: null,
      email: 'sipho@email.com',
      phone: '+27 82 123 4567',
      alternatePhone: null,
      idNumber: '8501015800084',
      vatNumber: null,
      status: 'active',
      creditLimit: 1000,
      currentBalance: 0,
      totalPurchases: 2547.85,
      lastPurchaseDate: '2024-08-24T10:30:00Z',
      registrationDate: '2024-01-15T09:00:00Z',
      billingAddress: {
        street: '123 Main Street',
        suburb: 'Alexandra',
        city: 'Johannesburg',
        province: 'Gauteng',
        postalCode: '2090',
        country: 'South Africa'
      },
      shippingAddress: {
        street: '123 Main Street',
        suburb: 'Alexandra',
        city: 'Johannesburg',
        province: 'Gauteng',
        postalCode: '2090',
        country: 'South Africa'
      },
      paymentTerms: 'Cash',
      preferredPaymentMethod: 'cash',
      notes: 'Regular customer, very reliable',
      tags: ['regular', 'local'],
      createdAt: '2024-01-15T09:00:00Z',
      updatedAt: '2024-08-24T10:30:00Z'
    },
    {
      id: 'CUST-002',
      customerNumber: 'CUST-002',
      type: 'individual',
      firstName: 'Mary',
      lastName: 'Johnson',
      companyName: null,
      email: 'mary@email.com',
      phone: '+27 71 987 6543',
      alternatePhone: '+27 11 555 0123',
      idNumber: '7203125600089',
      vatNumber: null,
      status: 'active',
      creditLimit: 500,
      currentBalance: 98.47,
      totalPurchases: 1245.67,
      lastPurchaseDate: '2024-08-24T14:15:00Z',
      registrationDate: '2024-02-20T11:30:00Z',
      billingAddress: {
        street: '456 Oak Avenue',
        suburb: 'Soweto',
        city: 'Johannesburg',
        province: 'Gauteng',
        postalCode: '1804',
        country: 'South Africa'
      },
      shippingAddress: {
        street: '456 Oak Avenue',
        suburb: 'Soweto',
        city: 'Johannesburg',
        province: 'Gauteng',
        postalCode: '1804',
        country: 'South Africa'
      },
      paymentTerms: '30 Days',
      preferredPaymentMethod: 'credit',
      notes: 'Credit customer, sometimes late with payments',
      tags: ['credit', 'monthly'],
      createdAt: '2024-02-20T11:30:00Z',
      updatedAt: '2024-08-24T14:15:00Z'
    },
    {
      id: 'CUST-003',
      customerNumber: 'CUST-003',
      type: 'business',
      firstName: 'David',
      lastName: 'Williams',
      companyName: 'Williams Construction',
      email: 'david@williamsconst.co.za',
      phone: '+27 83 456 7890',
      alternatePhone: '+27 11 789 0123',
      idNumber: null,
      vatNumber: '4123456789',
      status: 'active',
      creditLimit: 5000,
      currentBalance: 120.52,
      totalPurchases: 15847.32,
      lastPurchaseDate: '2024-08-23T16:45:00Z',
      registrationDate: '2024-01-10T14:00:00Z',
      billingAddress: {
        street: '789 Industrial Road',
        suburb: 'Midrand',
        city: 'Johannesburg',
        province: 'Gauteng',
        postalCode: '1685',
        country: 'South Africa'
      },
      shippingAddress: {
        street: '789 Industrial Road',
        suburb: 'Midrand',
        city: 'Johannesburg',
        province: 'Gauteng',
        postalCode: '1685',
        country: 'South Africa'
      },
      paymentTerms: '30 Days',
      preferredPaymentMethod: 'eft',
      notes: 'Major business customer, bulk purchases',
      tags: ['business', 'bulk', 'vip'],
      createdAt: '2024-01-10T14:00:00Z',
      updatedAt: '2024-08-23T16:45:00Z'
    },
    {
      id: 'CUST-004',
      customerNumber: 'CUST-004',
      type: 'individual',
      firstName: 'Thandi',
      lastName: 'Ndlovu',
      companyName: null,
      email: 'thandi@email.com',
      phone: '+27 76 234 5678',
      alternatePhone: null,
      idNumber: '9008125700091',
      vatNumber: null,
      status: 'inactive',
      creditLimit: 0,
      currentBalance: 0,
      totalPurchases: 345.20,
      lastPurchaseDate: '2024-06-15T12:20:00Z',
      registrationDate: '2024-03-05T10:15:00Z',
      billingAddress: {
        street: '321 Park Lane',
        suburb: 'Randburg',
        city: 'Johannesburg',
        province: 'Gauteng',
        postalCode: '2125',
        country: 'South Africa'
      },
      shippingAddress: {
        street: '321 Park Lane',
        suburb: 'Randburg',
        city: 'Johannesburg',
        province: 'Gauteng',
        postalCode: '2125',
        country: 'South Africa'
      },
      paymentTerms: 'Cash',
      preferredPaymentMethod: 'cash',
      notes: 'Moved away, account inactive',
      tags: ['inactive'],
      createdAt: '2024-03-05T10:15:00Z',
      updatedAt: '2024-06-15T12:20:00Z'
    }
  ]

  // Filter customers based on query parameters
  let filteredCustomers = demoCustomers

  if (search) {
    const searchLower = search.toString().toLowerCase()
    filteredCustomers = filteredCustomers.filter(customer => 
      customer.firstName.toLowerCase().includes(searchLower) ||
      customer.lastName.toLowerCase().includes(searchLower) ||
      (customer.companyName && customer.companyName.toLowerCase().includes(searchLower)) ||
      customer.email.toLowerCase().includes(searchLower) ||
      customer.phone.includes(searchLower) ||
      customer.customerNumber.toLowerCase().includes(searchLower)
    )
  }

  if (status && status !== 'all') {
    filteredCustomers = filteredCustomers.filter(customer => 
      customer.status === status.toString()
    )
  }

  // Calculate pagination
  const totalItems = filteredCustomers.length
  const totalPages = Math.ceil(totalItems / Number(limit))
  const startIndex = (Number(page) - 1) * Number(limit)
  const endIndex = startIndex + Number(limit)
  const paginatedCustomers = filteredCustomers.slice(startIndex, endIndex)

  // Calculate summary statistics
  const totalCustomers = filteredCustomers.length
  const activeCustomers = filteredCustomers.filter(c => c.status === 'active').length
  const totalOutstanding = filteredCustomers.reduce((sum, customer) => sum + customer.currentBalance, 0)
  const totalLifetimeValue = filteredCustomers.reduce((sum, customer) => sum + customer.totalPurchases, 0)
  const averageCustomerValue = totalCustomers > 0 ? totalLifetimeValue / totalCustomers : 0

  return {
    success: true,
    data: {
      customers: paginatedCustomers,
      pagination: {
        currentPage: Number(page),
        totalPages,
        totalItems,
        itemsPerPage: Number(limit),
        hasNextPage: Number(page) < totalPages,
        hasPreviousPage: Number(page) > 1
      },
      summary: {
        totalCustomers,
        activeCustomers,
        totalOutstanding,
        totalLifetimeValue,
        averageCustomerValue
      }
    }
  }
})
