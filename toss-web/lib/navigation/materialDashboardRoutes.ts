export type NavItemType = 'collapse' | 'title' | 'divider'

export type NavCollapseItem = {
  type: 'collapse'
  name: string
  key: string
  icon?: string
  route?: string
  href?: string
  noCollapse?: boolean
  collapse?: NavCollapseItem[]
}

export type NavTitleItem = {
  type: 'title'
  title: string
  key: string
}

export type NavDividerItem = {
  type: 'divider'
  key: string
}

export type NavItem = NavCollapseItem | NavTitleItem | NavDividerItem

export type BreadcrumbItem = {
  label: string
  to?: string
}

export const materialDashboardRoutes: NavItem[] = [
  {
    type: 'collapse',
    name: 'Dashboard',
    key: 'dashboard',
    icon: 'dashboard',
    route: '/dashboard',
    noCollapse: true,
  },
  { type: 'divider', key: 'divider-0' },
  { type: 'title', title: 'Core Modules', key: 'title-core' },
  {
    type: 'collapse',
    name: 'Money (Finance)',
    key: 'accounting',
    icon: 'account_balance_wallet',
    collapse: [
      {
        type: 'collapse',
        name: 'Overview',
        key: 'accounting-overview',
        route: '/accounting',
      },
      {
        type: 'collapse',
        name: 'Invoices',
        key: 'accounting-invoices',
        route: '/accounting/invoices',
      },
      {
        type: 'collapse',
        name: 'Payments',
        key: 'accounting-payments',
        route: '/accounting/payments',
      },
      {
        type: 'collapse',
        name: 'Expenses',
        key: 'accounting-expenses',
        route: '/accounting/expenses',
      },
      {
        type: 'collapse',
        name: 'Reports',
        key: 'accounting-reports',
        route: '/accounting/reports',
      },
    ],
  },
  {
    type: 'collapse',
    name: 'Sales & CRM',
    key: 'sales',
    icon: 'shopping_cart',
    collapse: [
      {
        type: 'collapse',
        name: 'Overview',
        key: 'sales-overview',
        route: '/sales',
      },
      {
        type: 'collapse',
        name: 'Quotations',
        key: 'sales-quotations',
        route: '/sales/quotations',
      },
      {
        type: 'collapse',
        name: 'Orders',
        key: 'sales-orders',
        route: '/sales/orders',
      },
      {
        type: 'collapse',
        name: 'Deliveries',
        key: 'sales-deliveries',
        route: '/sales/delivery-notes',
      },
      {
        type: 'collapse',
        name: 'Invoices',
        key: 'sales-invoices',
        route: '/sales/invoices',
      },
      {
        type: 'collapse',
        name: 'Returns',
        key: 'sales-returns',
        route: '/sales/returns',
      },
    ],
  },
  {
    type: 'collapse',
    name: 'Stock',
    key: 'stock',
    icon: 'inventory_2',
    collapse: [
      {
        type: 'collapse',
        name: 'Overview',
        key: 'stock-overview',
        route: '/stock',
      },
      {
        type: 'collapse',
        name: 'Items',
        key: 'stock-items',
        route: '/stock/items',
      },
      {
        type: 'collapse',
        name: 'Transfers',
        key: 'stock-transfers',
        route: '/stock/transfers',
      },
      {
        type: 'collapse',
        name: 'Reconciliation',
        key: 'stock-reconciliation',
        route: '/stock/reconciliation',
      },
      {
        type: 'collapse',
        name: 'Reports',
        key: 'stock-reports',
        route: '/stock/reports',
      },
    ],
  },
  {
    type: 'collapse',
    name: 'Point of Sale',
    key: 'pos',
    icon: 'point_of_sale',
    route: '/pos',
    noCollapse: true,
  },
  {
    type: 'collapse',
    name: 'Purchasing',
    key: 'purchasing',
    icon: 'shopping_bag',
    collapse: [
      {
        type: 'collapse',
        name: 'Overview',
        key: 'purchasing-overview',
        route: '/purchasing',
      },
      {
        type: 'collapse',
        name: 'Purchase Orders',
        key: 'purchasing-orders',
        route: '/purchasing/orders',
      },
      {
        type: 'collapse',
        name: 'Suppliers',
        key: 'purchasing-suppliers',
        route: '/purchasing/suppliers',
      },
    ],
  },
  { type: 'divider', key: 'divider-1' },
  { type: 'title', title: 'Operations', key: 'title-operations' },
  {
    type: 'collapse',
    name: 'Manufacturing',
    key: 'manufacturing',
    icon: 'precision_manufacturing',
    collapse: [
      {
        type: 'collapse',
        name: 'Overview',
        key: 'manufacturing-overview',
        route: '/manufacturing',
      },
      {
        type: 'collapse',
        name: 'Work Orders',
        key: 'manufacturing-work-orders',
        route: '/manufacturing/work-orders',
      },
      {
        type: 'collapse',
        name: 'Bills of Materials',
        key: 'manufacturing-bom',
        route: '/manufacturing/bom',
      },
    ],
  },
  {
    type: 'collapse',
    name: 'Projects',
    key: 'projects',
    icon: 'folder_open',
    collapse: [
      {
        type: 'collapse',
        name: 'Overview',
        key: 'projects-overview',
        route: '/projects',
      },
      {
        type: 'collapse',
        name: 'Tasks',
        key: 'projects-tasks',
        route: '/projects/tasks',
      },
    ],
  },
  {
    type: 'collapse',
    name: 'Quality',
    key: 'quality',
    icon: 'verified',
    collapse: [
      {
        type: 'collapse',
        name: 'Overview',
        key: 'quality-overview',
        route: '/quality',
      },
      {
        type: 'collapse',
        name: 'Inspections',
        key: 'quality-inspections',
        route: '/quality/inspections',
      },
      {
        type: 'collapse',
        name: 'Procedures',
        key: 'quality-procedures',
        route: '/quality/procedures',
      },
    ],
  },
  {
    type: 'collapse',
    name: 'Assets',
    key: 'assets',
    icon: 'domain',
    collapse: [
      {
        type: 'collapse',
        name: 'Overview',
        key: 'assets-overview',
        route: '/assets',
      },
      {
        type: 'collapse',
        name: 'Assets List',
        key: 'assets-list',
        route: '/assets/list',
      },
      {
        type: 'collapse',
        name: 'Maintenance',
        key: 'assets-maintenance',
        route: '/assets/maintenance',
      },
    ],
  },
  { type: 'divider', key: 'divider-2' },
  { type: 'title', title: 'People & Support', key: 'title-people' },
  {
    type: 'collapse',
    name: 'HR & Payroll',
    key: 'hr',
    icon: 'badge',
    collapse: [
      {
        type: 'collapse',
        name: 'Overview',
        key: 'hr-overview',
        route: '/hr',
      },
      {
        type: 'collapse',
        name: 'Employees',
        key: 'hr-employees',
        route: '/hr/employees',
      },
      {
        type: 'collapse',
        name: 'Attendance',
        key: 'hr-attendance',
        route: '/hr/attendance',
      },
      {
        type: 'collapse',
        name: 'Leave',
        key: 'hr-leave',
        route: '/hr/leave',
      },
      {
        type: 'collapse',
        name: 'Payroll',
        key: 'hr-payroll',
        route: '/hr/payroll',
      },
    ],
  },
  {
    type: 'collapse',
    name: 'Support',
    key: 'support',
    icon: 'support_agent',
    collapse: [
      {
        type: 'collapse',
        name: 'Overview',
        key: 'support-overview',
        route: '/support',
      },
      {
        type: 'collapse',
        name: 'Tickets',
        key: 'support-tickets',
        route: '/support/tickets',
      },
    ],
  },
  { type: 'divider', key: 'divider-3' },
  { type: 'title', title: 'Collaboration', key: 'title-collaboration' },
  {
    type: 'collapse',
    name: 'Group Buying',
    key: 'group-buying',
    icon: 'groups',
    route: '/purchasing/group-buying',
    noCollapse: true,
  },
  {
    type: 'collapse',
    name: 'Shared Logistics',
    key: 'logistics',
    icon: 'local_shipping',
    route: '/logistics',
    noCollapse: true,
  },
]

const normalizePath = (path: string) => {
  const withoutHash = path.split('#')[0] ?? ''
  const withoutQuery = withoutHash.split('?')[0] ?? ''
  return withoutQuery.startsWith('/') ? withoutQuery : `/${withoutQuery}`
}

export const resolveBreadcrumbsFromNav = (currentPath: string): BreadcrumbItem[] => {
  const target = normalizePath(currentPath)

  const visit = (items: NavItem[], parents: BreadcrumbItem[]): BreadcrumbItem[] | null => {
    for (const item of items) {
      if (item.type !== 'collapse') continue

      const current: BreadcrumbItem = { label: item.name }
      if (item.route) current.to = item.route

      if (item.route && normalizePath(item.route) === target) {
        return [...parents, current]
      }

      if (item.collapse?.length) {
        const result = visit(item.collapse, [...parents, current])
        if (result) return result
      }
    }

    return null
  }

  return visit(materialDashboardRoutes, []) ?? []
}
