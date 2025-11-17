export interface NavNode {
  label: string
  to?: string
  icon?: string
  badge?: string
  exact?: boolean
  children?: NavNode[]
}

export interface NavSection {
  title: string
  items: NavNode[]
}

const overviewSection: NavSection = {
  title: 'Overview',
  items: [
    { label: 'Dashboard', to: '/dashboard', icon: 'mdi:view-dashboard-outline', exact: true },
  ],
}

const salesSection: NavSection = {
  title: 'Sales & CRM',
  items: [
    {
      label: 'Sales Dashboard',
      to: '/sales',
      icon: 'mdi:view-dashboard-outline'
    },
    {
      label: 'Point of Sale',
      to: '/sales/pos',
      icon: 'mdi:cash-register'
    },
    {
      label: 'Ecommerce Blocks',
      to: '/sales/ecommerce-blocks',
      icon: 'mdi:cart-outline'
    },
    {
      label: 'Sales Transactions',
      icon: 'mdi:receipt-outline',
      children: [
        { label: 'Quotations', to: '/sales/quotations', icon: 'mdi:file-document-edit-outline' },
        { label: 'Sales Orders', to: '/sales/orders', icon: 'mdi:clipboard-text-outline' },
        { label: 'Delivery Notes', to: '/sales/delivery-notes', icon: 'mdi:truck-delivery-outline' },
        { label: 'Sales Invoices', to: '/sales/invoices', icon: 'mdi:file-invoice-outline' },
        { label: 'Credit Notes', to: '/sales/credit-notes', icon: 'mdi:file-restore-outline' },
        { label: 'Returns & Refunds', to: '/sales/returns', icon: 'mdi:keyboard-return' },
      ],
    },
    {
      label: 'Customer Management',
      icon: 'mdi:account-group-outline',
      children: [
        { label: 'Customers', to: '/sales/customers', icon: 'mdi:account-multiple-outline' },
        { label: 'Customer Groups', to: '/sales/customer-groups', icon: 'mdi:account-supervisor-outline' },
        { label: 'Sales Territories', to: '/sales/territories', icon: 'mdi:map-marker-radius-outline' },
        { label: 'Sales Partners', to: '/sales/partners', icon: 'mdi:handshake-outline' },
      ],
    },
    {
      label: 'Pricing & Promotions',
      icon: 'mdi:tag-multiple-outline',
      children: [
        { label: 'Pricing Rules', to: '/sales/pricing-rules', icon: 'mdi:currency-usd' },
        { label: 'Promotional Schemes', to: '/sales/promotional-schemes', icon: 'mdi:gift-outline' },
        { label: 'Discount Campaigns', to: '/sales/discounts', icon: 'mdi:percent-outline' },
      ],
    },
    {
      label: 'CRM & Leads',
      icon: 'mdi:account-search-outline',
      children: [
        { label: 'Leads', to: '/crm/leads', icon: 'mdi:account-search-outline' },
        { label: 'Opportunities', to: '/crm/opportunities', icon: 'mdi:target-account' },
        { label: 'Campaigns', to: '/crm/campaigns', icon: 'mdi:megaphone-outline' },
      ],
    },
    {
      label: 'Analytics & Reports',
      icon: 'mdi:chart-line-variant',
      children: [
        { label: 'Sales Analytics', to: '/sales/analytics', icon: 'mdi:chart-line-variant' },
        { label: 'Sales Reports', to: '/sales/reports', icon: 'mdi:file-chart-outline' },
        { label: 'Customer Reports', to: '/sales/reports/customers', icon: 'mdi:account-details-outline' },
      ],
    },
  ],
}

const procurementSection: NavSection = {
  title: 'Procurement',
  items: [
    {
      label: 'Purchasing Workspace',
      icon: 'mdi:cart-arrow-down',
      children: [
        { label: 'Suppliers', to: '/purchasing/suppliers', icon: 'mdi:truck-delivery-outline' },
        { label: 'Receiving', to: '/purchasing/receiving', icon: 'mdi:package-variant-closed' },
        { label: 'Purchase Orders', to: '/buying/orders', icon: 'mdi:clipboard-list-outline' },
      ],
    },
  ],
}

const operationsSection: NavSection = {
  title: 'Inventory & Operations',
  items: [
    {
      label: 'Inventory Management',
      icon: 'mdi:warehouse',
      children: [
        { label: 'Products', to: '/inventory/products', icon: 'mdi:package-variant' },
        { label: 'Adjustments', to: '/inventory/adjustments', icon: 'mdi:swap-horizontal' },
        { label: 'Stock Movements', to: '/stock/movements', icon: 'mdi:transfer' },
      ],
    },
    {
      label: 'Logistics',
      icon: 'mdi:truck-outline',
      children: [
        { label: 'Logistics Overview', to: '/logistics', icon: 'mdi:view-dashboard-outline' },
        { label: 'Delivery Runs', to: '/logistics/runs', icon: 'mdi:map-marker-path' },
      ],
    },
  ],
}

const financeSection: NavSection = {
  title: 'Finance',
  items: [
    {
      label: 'Financial Management',
      icon: 'mdi:finance',
      children: [
        { label: 'Accounts', to: '/finance/accounts', icon: 'mdi:bank-outline' },
        { label: 'Reports', to: '/finance/reports', icon: 'mdi:file-chart-outline' },
        { label: 'Receivables', to: '/accounting/receivables', icon: 'mdi:cash-clock' },
        { label: 'Payables', to: '/accounting/payables', icon: 'mdi:cash-fast' },
      ],
    },
  ],
}

const hrSection: NavSection = {
  title: 'HR & People',
  items: [
    {
      label: 'Human Resources',
      icon: 'mdi:account-group',
      children: [
        { label: 'HR Dashboard', to: '/hr', icon: 'mdi:view-dashboard-outline' },
        { label: 'Employees', to: '/hr/employees', icon: 'mdi:account-multiple' },
        { label: 'Attendance', to: '/hr/attendance', icon: 'mdi:clock-check-outline' },
        { label: 'Leave Requests', to: '/hr/leave', icon: 'mdi:calendar-remove' },
        { label: 'Payroll', to: '/hr/payroll', icon: 'mdi:cash-multiple' },
        { label: 'Performance', to: '/hr/performance', icon: 'mdi:chart-line-variant' },
      ],
    },
  ],
}

const aiSection: NavSection = {
  title: 'AI & Automation',
  items: [
    {
      label: 'AI Assistant',
      icon: 'mdi:robot-excited-outline',
      children: [
        { label: 'AI Chat', to: '/ai/chat', icon: 'mdi:message-text-outline' },
        { label: 'Automations', to: '/ai/automations', icon: 'mdi:cog-sync-outline' },
      ],
    },
  ],
}

const adminSection: NavSection = {
  title: 'Administration',
  items: [
    { label: 'Settings', to: '/settings', icon: 'mdi:cog-outline' },
    { label: 'Profile', to: '/profile', icon: 'mdi:account-outline' },
  ],
}

const authSection: NavSection = {
  title: 'Auth Pages',
  items: [
    { label: 'Sign In', to: '/auth/login', icon: 'mdi:login', exact: true },
    { label: 'Register', to: '/auth/register', icon: 'mdi:account-plus-outline', exact: true },
  ],
}

export const navigation: NavSection[] = [
  overviewSection,
  salesSection,
  procurementSection,
  operationsSection,
  financeSection,
  hrSection,
  aiSection,
  adminSection,
]

export const secondaryNavigation: NavSection[] = [authSection]

export interface FlattenedNavItem extends NavNode {
  parents: string[]
}

export const flattenNavigation = (sections: NavSection[]): FlattenedNavItem[] => {
  const flattenNodes = (items: NavNode[], parents: string[]): FlattenedNavItem[] => {
    return items.flatMap((item) => {
      const currentParents = item.children ? [...parents, item.label] : parents

      if (!item.children?.length) {
        return [{ ...item, parents: parents.slice() }]
      }

      const childResults = flattenNodes(item.children, currentParents)
      if (item.to) {
        return [{ ...item, parents: parents.slice() }, ...childResults]
      }

      return childResults
    })
  }

  return sections.flatMap((section) => flattenNodes(section.items, [section.title]))
}

const navigationIndex = flattenNavigation([...navigation, ...secondaryNavigation])

export const findNavItemByPath = (path: string): FlattenedNavItem | undefined => {
  const normalizedPath = path?.length > 1 && path.endsWith('/') ? path.slice(0, -1) : path
  return navigationIndex.find((item) => item.to && isPathActive(normalizedPath, item))
}

export const isPathActive = (path: string, item: NavNode): boolean => {
  if (!item.to) {
    return false
  }

  if (item.exact) {
    return path === item.to
  }

  if (path === item.to) {
    return true
  }

  return path.startsWith(`${item.to}/`)
}

export const listPrimaryRoutes = (): string[] => {
  return navigationIndex.filter((item) => Boolean(item.to)).map((item) => item.to!)
}

export const getSectionByPath = (path: string): NavSection | undefined => {
  const match = findNavItemByPath(path)
  if (!match) {
    return undefined
  }

  const sectionTitle = match.parents[0]
  return [...navigation, ...secondaryNavigation].find((section) => section.title === sectionTitle)
}
