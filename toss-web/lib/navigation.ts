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
      label: 'Sales Workspace',
      icon: 'mdi:shopping-outline',
      children: [
        { label: 'Sales Analytics', to: '/sales/analytics', icon: 'mdi:chart-line-variant' },
        { label: 'Orders', to: '/sales/orders', icon: 'mdi:clipboard-text-outline' },
        { label: 'Point of Sale', to: '/sales/pos', icon: 'mdi:cash-register' },
        { label: 'Invoices', to: '/sales/invoices', icon: 'mdi:file-invoice-outline' },
      ],
    },
    {
      label: 'Customer Operations',
      icon: 'mdi:account-group-outline',
      children: [
        { label: 'Leads', to: '/crm/leads', icon: 'mdi:account-search-outline' },
        { label: 'Campaigns', to: '/crm/campaigns', icon: 'mdi:megaphone-outline' },
        { label: 'Customers', to: '/crm/customers', icon: 'mdi:account-multiple-outline' },
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
