interface ModuleMetric {
  label: string
  value: string
  change: string
  trend?: 'up' | 'down'
  icon?: string
  color?: 'indigo' | 'emerald' | 'amber' | 'purple' | 'rose' | 'cyan'
}

interface ModuleInsight {
  title: string
  description: string
  badge?: string
}

interface ModuleTask {
  title: string
  status: 'on-track' | 'at-risk' | 'due-soon'
  due?: string
}

interface ModuleTable {
  title: string
  columns: string[]
  rows: Array<Record<string, string>>
}

interface ModuleAction {
  label: string
  to: string
}

interface ModuleBusinessNeed {
  title: string
  detail: string
}

interface ModuleErpnextMapping {
  module: string
  capability: string
}

interface ModulePlaybook {
  title: string
  description: string
  status?: 'live' | 'beta'
}

export interface ModuleData {
  slug: string
  title: string
  summary: string
  timeframe: string
  metrics: ModuleMetric[]
  insights: ModuleInsight[]
  tasks: ModuleTask[]
  table: ModuleTable
  alerts: ModuleInsight[]
  actions: ModuleAction[]
  businessNeeds: ModuleBusinessNeed[]
  erpnextMapping: ModuleErpnextMapping[]
  aiPlaybooks: ModulePlaybook[]
}

const baseMetrics: ModuleMetric[] = [
  { label: 'Pipeline Value', value: 'R 4.2M', change: '+8.4%', trend: 'up', icon: 'lucide:trending-up', color: 'indigo' },
  { label: 'Open Items', value: '127', change: '-12%', trend: 'down', icon: 'lucide:clipboard-check', color: 'purple' },
  { label: 'Automation Rate', value: '63%', change: '+4%', trend: 'up', icon: 'lucide:sparkles', color: 'emerald' },
  { label: 'Team SLA', value: '94%', change: '-2%', trend: 'down', icon: 'lucide:gauge', color: 'amber' }
]

const defaultTable: ModuleTable = {
  title: 'Key Workstreams',
  columns: ['Name', 'Owner', 'Status', 'Due'],
  rows: [
    { Name: 'Quarterly forecast sync', Owner: 'Finance', Status: 'In progress', Due: '2 days' },
    { Name: 'Pricing refresh', Owner: 'Merchandising', Status: 'Ready', Due: 'Today' },
    { Name: 'Supplier onboarding', Owner: 'Buying', Status: 'Blocked', Due: '5 days' }
  ]
}

const defaultBusinessNeeds: ModuleBusinessNeed[] = [
  {
    title: 'Daily visibility',
    detail: 'Give township operators a single glance view of sales, stock and alerts.'
  },
  {
    title: 'AI guidance',
    detail: 'Translate telemetry into plain-language tasks the owner can action quickly.'
  }
]

const defaultErpnextMappings: ModuleErpnextMapping[] = [
  { module: 'Selling', capability: 'Quotations, orders and promotion reporting' },
  { module: 'CRM', capability: 'Lead notes, follow ups and customer profiles' }
]

const defaultPlaybooks: ModulePlaybook[] = [
  {
    title: 'Weekly pulse',
    description: 'Summarise key KPIs and send the digest to owners on WhatsApp.',
    status: 'live'
  }
]

const createEntry = (slug: string, overrides: Partial<ModuleData>): ModuleData => ({
  slug,
  title: overrides.title ?? 'Module Overview',
  summary: overrides.summary ?? 'Live telemetry for this ERP capability.',
  timeframe: overrides.timeframe ?? 'Last 7 days',
  metrics: overrides.metrics ?? baseMetrics,
  insights: overrides.insights ?? [
    { title: 'Workflow health', description: 'Automation is covering most repetitive tasks this week.' },
    { title: 'Team focus', description: 'Top 3 squads are exceeding SLA by 12%.' }
  ],
  tasks: overrides.tasks ?? [
    { title: 'Audit escalation', status: 'at-risk', due: 'Today' },
    { title: 'Refresh data connections', status: 'on-track' },
    { title: 'Review alerts queue', status: 'due-soon', due: 'Tomorrow' }
  ],
  table: overrides.table ?? defaultTable,
  alerts: overrides.alerts ?? [
    { title: 'Reminder', description: 'Two low-priority playbooks require approvals.', badge: 'info' }
  ],
  actions: overrides.actions ?? [
    { label: 'View detailed report', to: '/reports' },
    { label: 'Create automation', to: '/automation' }
  ],
  businessNeeds: overrides.businessNeeds ?? defaultBusinessNeeds,
  erpnextMapping: overrides.erpnextMapping ?? defaultErpnextMappings,
  aiPlaybooks: overrides.aiPlaybooks ?? defaultPlaybooks
})

const modules: Record<string, ModuleData> = {
  selling: createEntry('selling', {
    title: 'Sales & Marketing',
    summary: 'Campaign performance, revenue pacing and lead acceleration across all channels.',
    metrics: [
      { label: 'Active Campaigns', value: '8', change: '+2', trend: 'up', icon: 'lucide:megaphone', color: 'indigo' },
      { label: 'Community Reach', value: '28k', change: '+9%', trend: 'up', icon: 'lucide:users', color: 'purple' },
      { label: 'Referral Leads', value: '384', change: '+6%', trend: 'up', icon: 'lucide:share-2', color: 'emerald' },
      { label: 'Spend Efficiency', value: '5.2x ROAS', change: '+0.4x', trend: 'up', icon: 'lucide:wallet', color: 'amber' }
    ],
    table: {
      title: 'Active campaigns',
      columns: ['Name', 'Stage', 'Owner', 'Spend'],
      rows: [
        { Name: 'Festive retail push', Stage: 'Live', Owner: 'Zama', Spend: 'R 210k' },
        { Name: 'Wholesale nurture', Stage: 'Build', Owner: 'Neo', Spend: 'R 80k' },
        { Name: 'Refer-a-shop', Stage: 'Optimising', Owner: 'Naledi', Spend: 'R 45k' }
      ]
    },
    businessNeeds: [
      {
        title: 'Hyperlocal awareness',
        detail: 'Boost township foot traffic with campaigns tied to AI detected product trends.'
      },
      {
        title: 'Community referrals',
        detail: 'Reward shoppers who refer neighbours and push qualified leads to supplier partners.'
      },
      {
        title: 'Supplier storytelling',
        detail: 'Share live sales results with suppliers so they co-fund group bundles.'
      }
    ],
    erpnextMapping: [
      { module: 'CRM', capability: 'Lead intake, loyalty tiers and follow up cadences.' },
      { module: 'Selling', capability: 'Campaign ROI, quotations and recurring bookings.' },
      { module: 'Communication', capability: 'Broadcast WhatsApp/SMS templates straight from ERPNext.' }
    ],
    aiPlaybooks: [
      {
        title: 'WhatsApp Hustle Boost',
        description: 'Auto-target top spenders with a weekend bundle before demand peaks.',
        status: 'live'
      },
      {
        title: 'Referral Heatmap',
        description: 'Highlight wards where referral momentum slows and suggest incentives.',
        status: 'beta'
      },
      {
        title: 'Supplier Launchpad',
        description: 'Compile campaign readouts for suppliers and request co-op budget in one tap.',
        status: 'live'
      }
    ]
  }),
  'selling-pos': createEntry('selling-pos', {
    title: 'POS & Store Solutions',
    summary: 'Omni-channel transactions, device availability and shift performance.',
    metrics: [
      { label: 'Offline Checkouts', value: '7,842', change: '+9%', trend: 'up', icon: 'lucide:scan-line', color: 'emerald' },
      { label: 'Average Basket', value: 'R 356', change: '+3%', trend: 'up', icon: 'lucide:shopping-bag', color: 'indigo' },
      { label: 'Offline Uptime', value: '99.2%', change: '+0.2%', trend: 'up', icon: 'lucide:wifi-off', color: 'amber' },
      { label: 'New Devices', value: '34', change: '+12%', trend: 'up', icon: 'lucide:tablet', color: 'purple' }
    ],
    table: {
      title: 'Store readiness',
      columns: ['Branch', 'Status', 'Last Sync', 'Manager'],
      rows: [
        { Branch: 'Soweto Hub', Status: 'Healthy', 'Last Sync': '08:14', Manager: 'Letlhogonolo' },
        { Branch: 'Ga-Rankuwa', Status: 'Warning', 'Last Sync': '07:32', Manager: 'Busisiwe' },
        { Branch: 'Khayelitsha', Status: 'Healthy', 'Last Sync': '08:05', Manager: 'Mandla' }
      ]
    },
    businessNeeds: [
      {
        title: 'Offline-first POS',
        detail: 'Record every cash or card sale even when the network drops, then sync later.'
      },
      {
        title: 'Smart replenishment',
        detail: 'Tie POS scans to stock levels so staples auto-reorder before shelves empty.'
      },
      {
        title: 'Daily cash confidence',
        detail: 'Simplify cash-up, variance checks and digital payouts for shop owners.'
      }
    ],
    erpnextMapping: [
      { module: 'Point of Sale', capability: 'Device management, shift tracking and offline queues.' },
      { module: 'Stock', capability: 'Auto-reserve inventory and trigger replenishment workflows.' },
      { module: 'Accounts', capability: 'Synchronise cash-up summaries and settlements.' }
    ],
    aiPlaybooks: [
      {
        title: 'Auto Reorder Essentials',
        description: 'Generate supplier carts for fast movers when coverage drops below target.',
        status: 'live'
      },
      {
        title: 'Till Variance Guard',
        description: 'Alert the owner if cash vs. digital intake deviates from pattern.',
        status: 'beta'
      },
      {
        title: 'Shift Coaching',
        description: 'Nudge clerks with tips on attachment items and credit capture.',
        status: 'live'
      }
    ]
  }),
  'selling-cross-commerce': createEntry('selling-cross-commerce', {
    title: 'Cross Commerce Solutions',
    summary: 'Marketplace assortment, channel profitability and fulfilment orchestration.',
    metrics: [
      { label: 'Marketplace GMV', value: 'R 640k', change: '+11%', trend: 'up', icon: 'lucide:store', color: 'purple' },
      { label: 'Channel Margin', value: '18.3%', change: '+0.5%', trend: 'up', icon: 'lucide:percent', color: 'emerald' },
      { label: 'Fulfilment SLA', value: '96%', change: '-1%', trend: 'down', icon: 'lucide:timer', color: 'amber' },
      { label: 'Return Rate', value: '2.1%', change: '-0.3%', trend: 'up', icon: 'lucide:rotate-ccw', color: 'rose' }
    ],
    businessNeeds: [
      {
        title: 'Unified catalogue',
        detail: 'Expose marketplace offers next to in-store stock so owners can expand assortment quickly.'
      },
      {
        title: 'Fulfilment orchestration',
        detail: 'Coordinate suppliers, depots and drivers for one pooled delivery plan.'
      },
      {
        title: 'Margin visibility',
        detail: 'Track profitability per channel and auto-flag loss making assortments.'
      }
    ],
    erpnextMapping: [
      { module: 'Selling', capability: 'Marketplace orders, dropship fulfilment and commissions.' },
      { module: 'Buying', capability: 'Consolidated procurement and vendor managed inventory.' },
      { module: 'ERPNext Integrations', capability: 'Sync listings to partner storefronts.' }
    ],
    aiPlaybooks: [
      {
        title: 'Marketplace Price Watch',
        description: 'Compare supplier quotes vs. marketplace fees and suggest profitable bundles.',
        status: 'live'
      },
      {
        title: 'Group Buy Trigger',
        description: 'Detect shared demand for bulk items and open a pooled order window.',
        status: 'beta'
      },
      {
        title: 'Fulfilment SLA Monitor',
        description: 'Escalate when a delivery leg risks missing township service levels.',
        status: 'live'
      }
    ]
  }),
  'planning-assortment': createEntry('planning-assortment', {
    title: 'Planning & Assortment',
    summary: 'Top-down merchandise planning, cluster demand and allocation scenarios.',
    metrics: [
      { label: 'Planned Buy', value: 'R 3.4M', change: '+5%', trend: 'up', icon: 'lucide:coins', color: 'indigo' },
      { label: 'In-season Shift', value: 'R 420k', change: '-3%', trend: 'down', icon: 'lucide:repeat', color: 'amber' },
      { label: 'SKU Coverage', value: '92%', change: '+4%', trend: 'up', icon: 'lucide:layout-grid', color: 'emerald' },
      { label: 'Allocation Accuracy', value: '88%', change: '+6%', trend: 'up', icon: 'lucide:crosshair', color: 'purple' }
    ],
    businessNeeds: [
      { title: 'Demand clarity', detail: 'Blend historical POS data with AI signals to size buys per community cluster.' },
      { title: 'Cash discipline', detail: 'Guard working capital by simulating how much stock the network can carry.' },
      { title: 'Ready-to-ship packs', detail: 'Build smart assortments that suppliers can box and deliver without repacking.' }
    ],
    erpnextMapping: [
      { module: 'Buying', capability: 'Material requests and blanket orders for pooled procurement.' },
      { module: 'Stock', capability: 'Reorder level automation plus substitute item logic.' },
      { module: 'Projects', capability: 'Scenario planning tasks for seasonal drops.' }
    ],
    aiPlaybooks: [
      { title: 'Neighbourhood Demand Pulse', description: 'Detect spikes from POS feeds and bump allocations automatically.', status: 'live' },
      { title: 'Budget Guardrails', description: 'Alert planners when a basket blows through cash constraints.', status: 'beta' },
      { title: 'Supplier Prep Checklist', description: 'Share assortment files + ETA with vendors ahead of delivery week.', status: 'live' }
    ]
  }),
  'planning-merchandising': createEntry('planning-merchandising', {
    title: 'Merchandising Management',
    summary: 'Line reviews, pricing ladders and floor-set readiness.',
    metrics: [
      { label: 'Line Cards', value: '58', change: '+7%', trend: 'up', icon: 'lucide:files', color: 'purple' },
      { label: 'Markdown Impact', value: 'R 74k', change: '-6%', trend: 'down', icon: 'lucide:tag', color: 'rose' },
      { label: 'Planograms Published', value: '42', change: '+3', trend: 'up', icon: 'lucide:layout', color: 'indigo' },
      { label: 'Supplier Compliance', value: '91%', change: '+5%', trend: 'up', icon: 'lucide:badge-check', color: 'emerald' }
    ],
    businessNeeds: [
      { title: 'Consistent shelf stories', detail: 'Translate planograms into simple tasks store owners can execute quickly.' },
      { title: 'Margin defence', detail: 'Track markdown recovery so price cuts still protect township margins.' },
      { title: 'Supplier playbooks', detail: 'Share readiness checklists to cut cycle time on promotional displays.' }
    ],
    erpnextMapping: [
      { module: 'Projects', capability: 'Floor-set timelines, dependencies and task ownership.' },
      { module: 'Quality Management', capability: 'Compliance checks for planograms and brand assets.' },
      { module: 'Stock', capability: 'Variant attributes and item bundles for curated displays.' }
    ],
    aiPlaybooks: [
      { title: 'Planogram Coach', description: 'Send annotated layouts and auto-check for missing hero items.', status: 'live' },
      { title: 'Markdown Watcher', description: 'Alert when price drops erode margin beyond the safe band.', status: 'beta' },
      { title: 'Supplier Handover Bot', description: 'Compile display instructions + sell-through targets straight from ERPNext.', status: 'live' }
    ]
  }),
  'planning-business-intelligence': createEntry('planning-business-intelligence', {
    title: 'Business Intelligence',
    summary: 'Core dashboards, anomaly detection and decision playbooks.',
    metrics: [
      { label: 'Insights Shipped', value: '27', change: '+10%', trend: 'up', icon: 'lucide:lightbulb', color: 'amber' },
      { label: 'Data Freshness', value: '99.6%', change: '+0.1%', trend: 'up', icon: 'lucide:refresh-cw', color: 'cyan' },
      { label: 'Alert Accuracy', value: '93%', change: '+2%', trend: 'up', icon: 'lucide:bell', color: 'purple' },
      { label: 'Playbooks Running', value: '14', change: '+4', trend: 'up', icon: 'lucide:workflow', color: 'emerald' }
    ],
    businessNeeds: [
      { title: 'Owner-friendly analytics', detail: 'Convert complex KPIs into human language updates on WhatsApp.' },
      { title: 'Network learning', detail: 'Spot wins from one township and share with every other shop instantly.' },
      { title: 'Trustworthy data', detail: 'Keep freshness + uptime high even on low bandwidth devices.' }
    ],
    erpnextMapping: [
      { module: 'Support', capability: 'Surface ticket sentiment and SLA breaches in dashboards.' },
      { module: 'Utilities', capability: 'Use built-in Data Import/Export, Scheduler and Reports.' },
      { module: 'Communication', capability: 'Deliver digests through ERPNext email/SMS channels.' }
    ],
    aiPlaybooks: [
      { title: 'Township Pulse Report', description: 'Push a daily mini-report summarising sales, stock and deliveries.', status: 'live' },
      { title: 'Anomaly Radar', description: 'Pinpoint deviations (e.g. sudden stock-outs) and create tickets.', status: 'beta' },
      { title: 'Opportunity Broadcast', description: 'Notify shops when similar peers grew sales via a specific lever.', status: 'live' }
    ]
  }),
  'relationships-account': createEntry('relationships-account', {
    title: 'Account Management',
    summary: 'Customer lifecycle milestones, collections and engagement cadences.',
    metrics: [
      { label: 'Active Accounts', value: '312', change: '+4%', trend: 'up', icon: 'lucide:id-card', color: 'indigo' },
      { label: 'Expansion Pipeline', value: 'R 860k', change: '+9%', trend: 'up', icon: 'lucide:trending-up', color: 'emerald' },
      { label: 'Churn Risk', value: '2.4%', change: '-0.6%', trend: 'down', icon: 'lucide:alert-triangle', color: 'rose' },
      { label: 'Collection Rate', value: '98%', change: '+1%', trend: 'up', icon: 'lucide:wallet-cards', color: 'purple' }
    ],
    businessNeeds: [
      { title: 'Credit visibility', detail: 'Use transaction history to build light-weight credit profiles for every shop.' },
      { title: 'Engagement cadences', detail: 'Automate nudges so owners never miss renewal or expansion moments.' },
      { title: 'Collections transparency', detail: 'Give both parties a shared view of invoices, settlements and disputes.' }
    ],
    erpnextMapping: [
      { module: 'Accounts', capability: 'Customer statements, payment reminders and credit limits.' },
      { module: 'CRM', capability: 'Account plans, next best actions and upsell journeys.' },
      { module: 'Communication', capability: 'Structured follow-ups through ERPNext email/sms templates.' }
    ],
    aiPlaybooks: [
      { title: 'Credit Builder', description: 'Summarise positive repayment behaviour and prep loan paperwork.', status: 'live' },
      { title: 'Expansion Radar', description: 'Highlight accounts with growing GMV who should unlock new services.', status: 'beta' },
      { title: 'Promise to Pay Tracker', description: 'Chase overdue agreements and schedule field agent support.', status: 'live' }
    ]
  }),
  'relationships-invoice': createEntry('relationships-invoice', {
    title: 'Invoice Management',
    summary: 'Billing runs, disputes and payment allocations across all entities.',
    metrics: [
      { label: 'Invoices Issued', value: '1,238', change: '+3%', trend: 'up', icon: 'lucide:file-text', color: 'indigo' },
      { label: 'Average DSO', value: '34 days', change: '-2 days', trend: 'down', icon: 'lucide:clock-9', color: 'amber' },
      { label: 'Disputes', value: '7', change: '-4', trend: 'up', icon: 'lucide:scales', color: 'rose' },
      { label: 'Auto-matched', value: '86%', change: '+5%', trend: 'up', icon: 'lucide:check-circle', color: 'emerald' }
    ],
    businessNeeds: [
      { title: 'Paperless billing', detail: 'Replace WhatsApp PDF chaos with structured invoices + receipts.' },
      { title: 'Cash flow clarity', detail: 'Show owners exactly when supplier payments are due and collected.' },
      { title: 'Dispute transparency', detail: 'Log issues (like damaged goods) with photos + decisions for both sides.' }
    ],
    erpnextMapping: [
      { module: 'Accounts', capability: 'Sales/Purchase Invoices, Payment Entries and Journals.' },
      { module: 'Bulk Transaction', capability: 'Batch posting for recurring township buyers.' },
      { module: 'EDI', capability: 'Supplier-friendly API/NAS integrations for invoice exchange.' }
    ],
    aiPlaybooks: [
      { title: 'Cash-up to Ledger', description: 'Auto-generate invoices from POS data and push to ERPNext nightly.', status: 'live' },
      { title: 'Dispute Mediator', description: 'Summarise evidence from both sides and propose a resolution timer.', status: 'beta' },
      { title: 'Working Capital Meter', description: 'Forecast cash gaps and suggest when to tap embedded finance.', status: 'live' }
    ]
  }),
  'relationships-vendor': createEntry('relationships-vendor', {
    title: 'Vendor Relationship',
    summary: 'Supplier performance, rebate tracking and collaboration loops.',
    metrics: [
      { label: 'Active Suppliers', value: '142', change: '+5%', trend: 'up', icon: 'lucide:truck', color: 'indigo' },
      { label: 'OTIF', value: '95%', change: '+1%', trend: 'up', icon: 'lucide:badge-check', color: 'emerald' },
      { label: 'Rebates Due', value: 'R 310k', change: '+8%', trend: 'up', icon: 'lucide:gift', color: 'purple' },
      { label: 'Contracts Expiring', value: '4', change: 'This month', trend: 'down', icon: 'lucide:alert-octagon', color: 'amber' }
    ],
    businessNeeds: [
      { title: 'Group-buy leverage', detail: 'Bundle township demand so suppliers give better pricing + credit.' },
      { title: 'Performance visibility', detail: 'Share OTIF + rebate dashboards with suppliers in real time.' },
      { title: 'Digital paperwork', detail: 'Automate contracts, invoices and proof-of-delivery flows.' }
    ],
    erpnextMapping: [
      { module: 'Buying', capability: 'Supplier scorecards, price lists and blanket purchase orders.' },
      { module: 'Subcontracting', capability: 'Track outsourced packing or manufacturing runs.' },
      { module: 'Communication', capability: 'Vendor collaboration portal + notifications.' }
    ],
    aiPlaybooks: [
      { title: 'Group Order Brief', description: 'Summarise consolidated demand + delivery routes for suppliers.', status: 'live' },
      { title: 'Rebate Guardian', description: 'Verify if agreed rebates or discounts have been applied.', status: 'beta' },
      { title: 'Vendor Risk Radar', description: 'Flag falling OTIF or quality dips before they hurt stock.', status: 'live' }
    ]
  }),
  'relationships-customer': createEntry('relationships-customer', {
    title: 'Customer Relationship',
    summary: 'Service levels, loyalty contributions and engagement journeys.',
    metrics: [
      { label: 'Net Satisfaction', value: '82', change: '+6', trend: 'up', icon: 'lucide:smile', color: 'emerald' },
      { label: 'Open Cases', value: '24', change: '-12%', trend: 'down', icon: 'lucide:inbox', color: 'purple' },
      { label: 'Loyalty Share', value: '36%', change: '+3%', trend: 'up', icon: 'lucide:star', color: 'amber' },
      { label: 'Escalations', value: '1', change: '-2', trend: 'down', icon: 'lucide:alarm-clock', color: 'rose' }
    ],
    businessNeeds: [
      { title: 'Neighbourhood trust', detail: 'Capture informal promises, credit slips and disputes transparently.' },
      { title: 'Service visibility', detail: 'Keep delivery ETAs + stock promises visible to shop owners.' },
      { title: 'Community loyalty', detail: 'Reward repeat buyers and offer micro-promotions tied to AI insights.' }
    ],
    erpnextMapping: [
      { module: 'CRM', capability: 'Customer issues, loyalty tiers and journey automation.' },
      { module: 'Support', capability: 'Ticket SLAs, knowledge base and maintenance schedules.' },
      { module: 'Portal', capability: 'Let shop owners self-serve statements and place requests.' }
    ],
    aiPlaybooks: [
      { title: 'Promise Keeper', description: 'Log community promises (e.g. credit sales) and remind both parties.', status: 'live' },
      { title: 'Loyalty Booster', description: 'Recommend micro rewards for the top quartile of spenders.', status: 'beta' },
      { title: 'Customer Health Digest', description: 'Summarise service issues + deliveries for each owner weekly.', status: 'live' }
    ]
  }),
  'operations-audits': createEntry('operations-audits', {
    title: 'Audits & Operations',
    summary: 'Internal controls, SOP adherence and compliance dashboards.',
    metrics: [
      { label: 'Audits in Flight', value: '11', change: '+2', trend: 'up', icon: 'lucide:clipboard-list', color: 'indigo' },
      { label: 'Findings Resolved', value: '78%', change: '+9%', trend: 'up', icon: 'lucide:check-square', color: 'emerald' },
      { label: 'Critical Issues', value: '0', change: '—', trend: 'up', icon: 'lucide:shield', color: 'purple' },
      { label: 'SOP Refresh', value: '6 docs', change: '+3', trend: 'up', icon: 'lucide:book', color: 'amber' }
    ],
    businessNeeds: [
      { title: 'Trusted operations', detail: 'Provide township operators with simple checklists to stay compliant.' },
      { title: 'Evidence trail', detail: 'Capture photos + notes per audit so remote teams can verify.' },
      { title: 'Continuous improvement', detail: 'Loop findings back into AI playbooks + SOP updates.' }
    ],
    erpnextMapping: [
      { module: 'Quality Management', capability: 'Non-conformance logging and CAPA workflows.' },
      { module: 'Maintenance', capability: 'Schedule store inspections and asset checks.' },
      { module: 'Support', capability: 'Route escalations into service queues with SLA timers.' }
    ],
    aiPlaybooks: [
      { title: 'SOP Whisperer', description: 'Deliver step-by-step actions during audits via mobile prompts.', status: 'live' },
      { title: 'Risk Heatmap', description: 'Score locations by unresolved findings and trigger visits.', status: 'beta' },
      { title: 'Auto-CAPA Draft', description: 'Generate corrective plans referencing ERPNext tasks + owners.', status: 'live' }
    ]
  }),
  'operations-inventory': createEntry('operations-inventory', {
    title: 'Inventory Management',
    summary: 'Stock accuracy, replenishment rhythm and ageing exposure.',
    metrics: [
      { label: 'Stock Value', value: 'R 5.7M', change: '+4%', trend: 'up', icon: 'lucide:cubes', color: 'indigo' },
      { label: 'Fill Rate', value: '97%', change: '+1%', trend: 'up', icon: 'lucide:gauge', color: 'emerald' },
      { label: 'Aged >60 days', value: '6%', change: '-0.8%', trend: 'down', icon: 'lucide:hourglass', color: 'amber' },
      { label: 'Replenishment Cycle', value: '5.2 days', change: '-0.3', trend: 'down', icon: 'lucide:cycle', color: 'purple' }
    ],
    businessNeeds: [
      { title: 'No stockouts', detail: 'Alert shops before hero items vanish by blending POS + supplier ETAs.' },
      { title: 'Fresh stock', detail: 'Flag ageing SKUs and suggest promotions or pooled returns.' },
      { title: 'Mobile visibility', detail: 'Give owners a simple mobile dashboard summarising stock health.' }
    ],
    erpnextMapping: [
      { module: 'Stock', capability: 'Bin-level balances, valuation and stock reconciliation.' },
      { module: 'Buying', capability: 'Auto-trigger purchase orders from reorder rules.' },
      { module: 'Assets', capability: 'Track fridges, freezers and other assets tied to SKU performance.' }
    ],
    aiPlaybooks: [
      { title: 'Low Stock SOS', description: 'Push reorder suggestions + one-click supplier carts.', status: 'live' },
      { title: 'Expiry Guardian', description: 'Warn when perishables are about to expire and propose bundles.', status: 'beta' },
      { title: 'Shared Delivery Planner', description: 'Group neighbouring shops that can share a replenishment run.', status: 'live' }
    ]
  }),
  'operations-warehouse': createEntry('operations-warehouse', {
    title: 'Warehouse Management',
    summary: 'Throughput, labour efficiency and utilisation across hubs.',
    metrics: [
      { label: 'Orders Processed', value: '4,812', change: '+7%', trend: 'up', icon: 'lucide:package', color: 'indigo' },
      { label: 'Pick Accuracy', value: '99.3%', change: '+0.5%', trend: 'up', icon: 'lucide:target', color: 'emerald' },
      { label: 'Dock Utilisation', value: '78%', change: '+4%', trend: 'up', icon: 'lucide:columns-4', color: 'purple' },
      { label: 'Labour Hours', value: '1,420', change: '-6%', trend: 'down', icon: 'lucide:clock', color: 'amber' }
    ],
    businessNeeds: [
      { title: 'Community hubs', detail: 'Optimise township fulfilment centres that feed drivers + shops.' },
      { title: 'Pick-pack excellence', detail: 'Track accuracy and labour to keep pooled orders profitable.' },
      { title: 'Driver coordination', detail: 'Align loading windows with last-mile driver availability.' }
    ],
    erpnextMapping: [
      { module: 'Stock', capability: 'Warehouse transfers, pick lists and packing slips.' },
      { module: 'Manufacturing', capability: 'Simple kitting for bundle offers and pre-packed hampers.' },
      { module: 'Telephony', capability: 'Notify drivers of dock readiness via ERPNext communications.' }
    ],
    aiPlaybooks: [
      { title: 'Wave Planner', description: 'Sequence picks by driver route to cut double handling.', status: 'live' },
      { title: 'Labour Optimiser', description: 'Forecast staffing for high-volume days (e.g. month-end).', status: 'beta' },
      { title: 'Dock Flow Guard', description: 'Alert when trucks dwell too long and reprioritise loads.', status: 'live' }
    ]
  }),
  'operations-supply-chain': createEntry('operations-supply-chain', {
    title: 'Supply & Chain Integration',
    summary: 'Transportation orchestration, partner visibility and risk tracking.',
    metrics: [
      { label: 'Routes Active', value: '86', change: '+5%', trend: 'up', icon: 'lucide:map', color: 'indigo' },
      { label: 'On-time Delivery', value: '94%', change: '+2%', trend: 'up', icon: 'lucide:clock-4', color: 'emerald' },
      { label: 'Cost per Drop', value: 'R 118', change: '-4%', trend: 'down', icon: 'lucide:fuel', color: 'amber' },
      { label: 'Carbon Index', value: '2.3 t/tonne', change: '-0.1', trend: 'down', icon: 'lucide:leaf', color: 'purple' }
    ],
    businessNeeds: [
      { title: 'Shared last mile', detail: 'Pool deliveries so multiple shops share one bakkie trip.' },
      { title: 'Network visibility', detail: 'Track each leg from wholesaler to depot to shop with live ETAs.' },
      { title: 'Cost relief', detail: 'Highlight opportunities to cut fuel spend or consolidate orders.' }
    ],
    erpnextMapping: [
      { module: 'Buying', capability: 'Drop-shipped purchase orders tied to route planning.' },
      { module: 'Stock', capability: 'Delivery notes, pick sheets and route stops.' },
      { module: 'Telephony', capability: 'Driver notifications and proof-of-delivery checkpoints.' }
    ],
    aiPlaybooks: [
      { title: 'Route Share Suggestion', description: 'Suggest neighbours that can join an existing delivery route.', status: 'live' },
      { title: 'ETA Truth Meter', description: 'Alert shops if a delivery leg is trending late.', status: 'beta' },
      { title: 'Cost per Drop Optimiser', description: 'Recommend route adjustments when cost creeps above target.', status: 'live' }
    ]
  })
}

export default modules

