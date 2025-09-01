import { defineEventHandler, readBody } from 'h3'

export default defineEventHandler(async (event) => {
  const body = await readBody<{ message?: string; context?: { module?: string } }>(event)
  const text = (body?.message || '').toLowerCase()
  const module = (body?.context?.module || 'dashboard').toLowerCase()

  const canned = (content: string, data?: any) => ({ content, data })

  if (module.includes('sales') || text.includes('sales') || text.includes('revenue')) {
    if (text.includes('forecast') || text.includes('predict')) {
      return canned('Projected next week revenue: R19,600 (p95). Growth +12.5%.', {
        metrics: [
          { label: 'Next Week', value: 'R19,600' },
          { label: 'Confidence', value: '87%' },
          { label: 'Growth', value: '+12.5%' }
        ],
        actions: [
          { title: 'View Sales Analytics', route: '/sales/analytics' },
          { title: 'Check Inventory', route: '/inventory' }
        ]
      })
    }
    return canned('Top products: White Bread (156), Coca Cola 2L (89), Milk 1L (78).', {
      metrics: [
        { label: 'Top Seller', value: 'White Bread' },
        { label: 'Units Sold', value: '156' },
        { label: 'Revenue Share', value: '32%' }
      ]
    })
  }

  if (module.includes('inventory') || text.includes('inventory') || text.includes('stock')) {
    return canned('3 items need attention: Milk, White Bread, Coca Cola. Reorder suggested.', {
      metrics: [
        { label: 'Total Items', value: '156' },
        { label: 'Low Stock Alerts', value: '3' },
        { label: 'Critical Items', value: '3' }
      ],
      actions: [
        { title: 'View Inventory', route: '/inventory' },
        { title: 'Create Purchase Order', route: '/purchasing/orders' }
      ]
    })
  }

  if (module.includes('crm') || text.includes('customer')) {
    return canned('Customers: 234 total, 23 active today, 68% repeat rate.', {
      metrics: [
        { label: 'Total Customers', value: '234' },
        { label: 'Repeat Rate', value: '68%' },
        { label: 'Active Today', value: '23' }
      ]
    })
  }

  if (module.includes('finance') || module.includes('accounts') || text.includes('financial')) {
    return canned('Cash R43,500; Profit margin 28%; Receivables R12,300.', {
      metrics: [
        { label: 'Cash on Hand', value: 'R43,500' },
        { label: 'Profit Margin', value: '28%' },
        { label: 'Receivables', value: 'R12,300' }
      ]
    })
  }

  return canned("I'm here to help with your business operations. Ask me anything.", {
    actions: [
      { title: 'View Dashboard', route: '/' },
      { title: 'Open Inventory', route: '/inventory' }
    ]
  })
})

