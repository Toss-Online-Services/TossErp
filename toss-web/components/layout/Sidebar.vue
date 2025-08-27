<template>
  <aside class="w-64 flex-shrink-0 bg-slate-900 border-r border-slate-800 flex flex-col">
    <!-- Logo Section -->
    <div class="h-16 flex items-center justify-center px-4 border-b border-slate-800">
      <div class="w-8 h-8 bg-gradient-to-r from-blue-500 to-purple-600 rounded-lg flex items-center justify-center mr-3">
        <span class="text-white font-bold text-sm">T</span>
      </div>
      <h1 class="text-xl font-bold text-white">TOSS ERP</h1>
    </div>
    
    <!-- Navigation -->
    <nav class="flex-1 px-3 py-6 space-y-2 overflow-y-auto">
      <NuxtLink 
        to="/" 
        class="nav-link"
        :class="{ 'nav-link-active': route.path === '/' }"
      >
        <HomeIcon class="h-5 w-5 mr-3" />
        Dashboard
      </NuxtLink>
      
      <!-- CRM Section -->
      <div class="space-y-1">
        <button 
          @click="toggleCrmDropdown"
          class="nav-link w-full justify-between"
          :class="{ 'nav-link-active': route.path.startsWith('/crm') }"
        >
          <div class="flex items-center">
            <UsersIcon class="h-5 w-5 mr-3" />
            CRM
          </div>
          <ChevronDownIcon 
            class="h-4 w-4 transition-transform duration-200"
            :class="{ 'transform rotate-180': crmDropdownOpen }"
          />
        </button>
        
        <div 
          v-show="crmDropdownOpen"
          class="ml-6 space-y-1 border-l border-slate-700 pl-3"
        >
          <NuxtLink to="/crm" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/crm' }">
            Dashboard
          </NuxtLink>
          <NuxtLink to="/crm/customers" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/crm/customers' }">
            Customers
          </NuxtLink>
          <NuxtLink to="/crm/leads" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/crm/leads' }">
            Leads
          </NuxtLink>
          <NuxtLink to="/crm/opportunities" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/crm/opportunities' }">
            Opportunities
          </NuxtLink>
          <NuxtLink to="/crm/contacts" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/crm/contacts' }">
            Contacts
          </NuxtLink>
          <NuxtLink to="/crm/automation" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/crm/automation' }">
            Automation
          </NuxtLink>
          <NuxtLink to="/crm/pipeline" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/crm/pipeline' }">
            Pipeline
          </NuxtLink>
        </div>
      </div>

      <!-- Sales & Selling Section -->
      <div class="space-y-1">
        <button 
          @click="toggleSalesDropdown"
          class="nav-link w-full justify-between"
          :class="{ 'nav-link-active': route.path.startsWith('/sales') || route.path.startsWith('/selling') }"
        >
          <div class="flex items-center">
            <ShoppingCartIcon class="h-5 w-5 mr-3" />
            Sales & Selling
          </div>
          <ChevronDownIcon 
            class="h-4 w-4 transition-transform duration-200"
            :class="{ 'transform rotate-180': salesDropdownOpen }"
          />
        </button>
        
        <div 
          v-show="salesDropdownOpen"
          class="ml-6 space-y-1 border-l border-slate-700 pl-3"
        >
          <NuxtLink to="/sales" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/sales' }">
            Sales Dashboard
          </NuxtLink>
          <NuxtLink to="/sales/quotes" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/sales/quotes' }">
            Quotations
          </NuxtLink>
          <NuxtLink to="/sales/orders" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/sales/orders' }">
            Sales Orders
          </NuxtLink>
          <NuxtLink to="/sales/invoices" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/sales/invoices' }">
            Sales Invoices
          </NuxtLink>
          <NuxtLink to="/sales/pos" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/sales/pos' }">
            Point of Sale
          </NuxtLink>
          <NuxtLink to="/sales/analytics" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/sales/analytics' }">
            Sales Analytics
          </NuxtLink>
        </div>
      </div>

      <!-- Purchasing & Buying Section -->
      <div class="space-y-1">
        <button 
          @click="togglePurchasingDropdown"
          class="nav-link w-full justify-between"
          :class="{ 'nav-link-active': route.path.startsWith('/purchasing') || route.path.startsWith('/buying') }"
        >
          <div class="flex items-center">
            <ShoppingBagIcon class="h-5 w-5 mr-3" />
            Purchasing
          </div>
          <ChevronDownIcon 
            class="h-4 w-4 transition-transform duration-200"
            :class="{ 'transform rotate-180': purchasingDropdownOpen }"
          />
        </button>
        
        <div 
          v-show="purchasingDropdownOpen"
          class="ml-6 space-y-1 border-l border-slate-700 pl-3"
        >
          <NuxtLink to="/purchasing" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/purchasing' }">
            Purchase Dashboard
          </NuxtLink>
          <NuxtLink to="/purchasing/suppliers" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/purchasing/suppliers' }">
            Suppliers
          </NuxtLink>
          <NuxtLink to="/purchasing/requests" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/purchasing/requests' }">
            Purchase Requests
          </NuxtLink>
          <NuxtLink to="/purchasing/orders" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/purchasing/orders' }">
            Purchase Orders
          </NuxtLink>
          <NuxtLink to="/purchasing/receipts" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/purchasing/receipts' }">
            Purchase Receipts
          </NuxtLink>
          <NuxtLink to="/purchasing/invoices" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/purchasing/invoices' }">
            Purchase Invoices
          </NuxtLink>
        </div>
      </div>

      <!-- Stock & Inventory Section -->
      <div class="space-y-1">
        <button 
          @click="toggleStockDropdown"
          class="nav-link w-full justify-between"
          :class="{ 'nav-link-active': route.path.startsWith('/stock') }"
        >
          <div class="flex items-center">
            <ArchiveBoxIcon class="h-5 w-5 mr-3" />
            Stock & Inventory
          </div>
          <ChevronDownIcon 
            class="h-4 w-4 transition-transform duration-200"
            :class="{ 'transform rotate-180': stockDropdownOpen }"
          />
        </button>
        
        <div 
          v-show="stockDropdownOpen"
          class="ml-6 space-y-1 border-l border-slate-700 pl-3"
        >
          <NuxtLink to="/stock" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/stock' }">
            Stock Dashboard
          </NuxtLink>
          <NuxtLink to="/stock/items" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/stock/items' }">
            Items
          </NuxtLink>
          <NuxtLink to="/stock/warehouses" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/stock/warehouses' }">
            Warehouses
          </NuxtLink>
          <NuxtLink to="/stock/movements" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/stock/movements' }">
            Stock Movements
          </NuxtLink>
          <NuxtLink to="/stock/reconciliation" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/stock/reconciliation' }">
            Stock Reconciliation
          </NuxtLink>
          <NuxtLink to="/stock/reports" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/stock/reports' }">
            Stock Reports
          </NuxtLink>
        </div>
      </div>

      <!-- Accounting & Finance Section -->
      <div class="space-y-1">
        <button 
          @click="toggleAccountingDropdown"
          class="nav-link w-full justify-between"
          :class="{ 'nav-link-active': route.path.startsWith('/accounting') || route.path.startsWith('/accounts') }"
        >
          <div class="flex items-center">
            <CurrencyDollarIcon class="h-5 w-5 mr-3" />
            Accounting
          </div>
          <ChevronDownIcon 
            class="h-4 w-4 transition-transform duration-200"
            :class="{ 'transform rotate-180': accountingDropdownOpen }"
          />
        </button>
        
        <div 
          v-show="accountingDropdownOpen"
          class="ml-6 space-y-1 border-l border-slate-700 pl-3"
        >
          <NuxtLink to="/accounting" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/accounting' }">
            Accounting Dashboard
          </NuxtLink>
          <NuxtLink to="/accounting/chart-of-accounts" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/accounting/chart-of-accounts' }">
            Chart of Accounts
          </NuxtLink>
          <NuxtLink to="/accounting/journal" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/accounting/journal' }">
            Journal Entries
          </NuxtLink>
          <NuxtLink to="/accounting/ledger" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/accounting/ledger' }">
            General Ledger
          </NuxtLink>
          <NuxtLink to="/accounting/payments" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/accounting/payments' }">
            Payments
          </NuxtLink>
          <NuxtLink to="/accounting/reports" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/accounting/reports' }">
            Financial Reports
          </NuxtLink>
        </div>
      </div>

      <!-- HR & Payroll Section -->
      <div class="space-y-1">
        <button 
          @click="toggleHrDropdown"
          class="nav-link w-full justify-between"
          :class="{ 'nav-link-active': route.path.startsWith('/hr') }"
        >
          <div class="flex items-center">
            <UserGroupIcon class="h-5 w-5 mr-3" />
            HR & Payroll
          </div>
          <ChevronDownIcon 
            class="h-4 w-4 transition-transform duration-200"
            :class="{ 'transform rotate-180': hrDropdownOpen }"
          />
        </button>
        
        <div 
          v-show="hrDropdownOpen"
          class="ml-6 space-y-1 border-l border-slate-700 pl-3"
        >
          <NuxtLink to="/hr" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/hr' }">
            HR Dashboard
          </NuxtLink>
          <NuxtLink to="/hr/employees" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/hr/employees' }">
            Employees
          </NuxtLink>
          <NuxtLink to="/hr/attendance" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/hr/attendance' }">
            Attendance
          </NuxtLink>
          <NuxtLink to="/hr/leaves" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/hr/leaves' }">
            Leave Management
          </NuxtLink>
          <NuxtLink to="/hr/payroll" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/hr/payroll' }">
            Payroll
          </NuxtLink>
          <NuxtLink to="/hr/performance" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/hr/performance' }">
            Performance
          </NuxtLink>
        </div>
      </div>

      <!-- Projects & Tasks Section -->
      <div class="space-y-1">
        <button 
          @click="toggleProjectsDropdown"
          class="nav-link w-full justify-between"
          :class="{ 'nav-link-active': route.path.startsWith('/projects') }"
        >
          <div class="flex items-center">
            <BriefcaseIcon class="h-5 w-5 mr-3" />
            Projects
          </div>
          <ChevronDownIcon 
            class="h-4 w-4 transition-transform duration-200"
            :class="{ 'transform rotate-180': projectsDropdownOpen }"
          />
        </button>
        
        <div 
          v-show="projectsDropdownOpen"
          class="ml-6 space-y-1 border-l border-slate-700 pl-3"
        >
          <NuxtLink to="/projects" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/projects' }">
            Projects Dashboard
          </NuxtLink>
          <NuxtLink to="/projects/list" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/projects/list' }">
            Project List
          </NuxtLink>
          <NuxtLink to="/projects/tasks" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/projects/tasks' }">
            Tasks
          </NuxtLink>
          <NuxtLink to="/projects/timesheet" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/projects/timesheet' }">
            Timesheet
          </NuxtLink>
          <NuxtLink to="/projects/collaboration" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/projects/collaboration' }">
            Collaboration
          </NuxtLink>
        </div>
      </div>

      <!-- Automation Section -->
      <div class="space-y-1">
        <button 
          @click="toggleAutomationDropdown"
          class="nav-link w-full justify-between"
          :class="{ 'nav-link-active': route.path.startsWith('/automation') }"
        >
          <div class="flex items-center">
            <CogIcon class="h-5 w-5 mr-3" />
            Automation
            <span class="ml-2 px-2 py-0.5 text-xs bg-blue-500 text-white rounded-full">AI</span>
          </div>
          <ChevronDownIcon 
            class="h-4 w-4 transition-transform duration-200"
            :class="{ 'transform rotate-180': automationDropdownOpen }"
          />
        </button>
        
        <div 
          v-show="automationDropdownOpen"
          class="ml-6 space-y-1 border-l border-slate-700 pl-3"
        >
          <NuxtLink to="/automation" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/automation' }">
            Automation Hub
          </NuxtLink>
          <NuxtLink to="/automation/workflows" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/automation/workflows' }">
            Workflows
          </NuxtLink>
          <NuxtLink to="/automation/triggers" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/automation/triggers' }">
            Triggers & Rules
          </NuxtLink>
          <NuxtLink to="/automation/ai-assistant" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/automation/ai-assistant' }">
            AI Assistant
          </NuxtLink>
          <NuxtLink to="/automation/reports" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/automation/reports' }">
            Automation Reports
          </NuxtLink>
        </div>
      </div>

      <!-- Collaboration Section -->
      <div class="space-y-1">
        <button 
          @click="toggleCollaborationDropdown"
          class="nav-link w-full justify-between"
          :class="{ 'nav-link-active': route.path.startsWith('/collaboration') }"
        >
          <div class="flex items-center">
            <ChatBubbleLeftRightIcon class="h-5 w-5 mr-3" />
            Collaboration
            <span class="ml-2 px-2 py-0.5 text-xs bg-green-500 text-white rounded-full">Team</span>
          </div>
          <ChevronDownIcon 
            class="h-4 w-4 transition-transform duration-200"
            :class="{ 'transform rotate-180': collaborationDropdownOpen }"
          />
        </button>
        
        <div 
          v-show="collaborationDropdownOpen"
          class="ml-6 space-y-1 border-l border-slate-700 pl-3"
        >
          <NuxtLink to="/collaboration" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/collaboration' }">
            Collaboration Hub
          </NuxtLink>
          <NuxtLink to="/collaboration/chat" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/collaboration/chat' }">
            Team Chat
          </NuxtLink>
          <NuxtLink to="/collaboration/meetings" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/collaboration/meetings' }">
            Meetings
          </NuxtLink>
          <NuxtLink to="/collaboration/documents" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/collaboration/documents' }">
            Document Sharing
          </NuxtLink>
          <NuxtLink to="/collaboration/whiteboard" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/collaboration/whiteboard' }">
            Whiteboard
          </NuxtLink>
          <NuxtLink to="/collaboration/notifications" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/collaboration/notifications' }">
            Notifications
          </NuxtLink>
        </div>
      </div>

      <!-- Manufacturing Section -->
      <div class="space-y-1">
        <button 
          @click="toggleManufacturingDropdown"
          class="nav-link w-full justify-between"
          :class="{ 'nav-link-active': route.path.startsWith('/manufacturing') }"
        >
          <div class="flex items-center">
            <WrenchScrewdriverIcon class="h-5 w-5 mr-3" />
            Manufacturing
          </div>
          <ChevronDownIcon 
            class="h-4 w-4 transition-transform duration-200"
            :class="{ 'transform rotate-180': manufacturingDropdownOpen }"
          />
        </button>
        
        <div 
          v-show="manufacturingDropdownOpen"
          class="ml-6 space-y-1 border-l border-slate-700 pl-3"
        >
          <NuxtLink to="/manufacturing" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/manufacturing' }">
            Manufacturing Dashboard
          </NuxtLink>
          <NuxtLink to="/manufacturing/bom" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/manufacturing/bom' }">
            Bill of Materials
          </NuxtLink>
          <NuxtLink to="/manufacturing/work-orders" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/manufacturing/work-orders' }">
            Work Orders
          </NuxtLink>
          <NuxtLink to="/manufacturing/production" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/manufacturing/production' }">
            Production Planning
          </NuxtLink>
          <NuxtLink to="/manufacturing/quality" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/manufacturing/quality' }">
            Quality Control
          </NuxtLink>
        </div>
      </div>

      <!-- Reports & Analytics Section -->
      <div class="space-y-1">
        <button 
          @click="toggleReportsDropdown"
          class="nav-link w-full justify-between"
          :class="{ 'nav-link-active': route.path.startsWith('/reports') }"
        >
          <div class="flex items-center">
            <ChartBarIcon class="h-5 w-5 mr-3" />
            Reports & Analytics
          </div>
          <ChevronDownIcon 
            class="h-4 w-4 transition-transform duration-200"
            :class="{ 'transform rotate-180': reportsDropdownOpen }"
          />
        </button>
        
        <div 
          v-show="reportsDropdownOpen"
          class="ml-6 space-y-1 border-l border-slate-700 pl-3"
        >
          <NuxtLink to="/reports" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/reports' }">
            Reports Dashboard
          </NuxtLink>
          <NuxtLink to="/reports/financial" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/reports/financial' }">
            Financial Reports
          </NuxtLink>
          <NuxtLink to="/reports/sales" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/reports/sales' }">
            Sales Reports
          </NuxtLink>
          <NuxtLink to="/reports/inventory" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/reports/inventory' }">
            Inventory Reports
          </NuxtLink>
          <NuxtLink to="/reports/analytics" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/reports/analytics' }">
            Business Analytics
          </NuxtLink>
        </div>
      </div>

      <!-- Settings & Administration Section -->
      <div class="space-y-1">
        <button 
          @click="toggleSettingsDropdown"
          class="nav-link w-full justify-between"
          :class="{ 'nav-link-active': route.path.startsWith('/settings') || route.path.startsWith('/admin') }"
        >
          <div class="flex items-center">
            <Cog6ToothIcon class="h-5 w-5 mr-3" />
            Settings
          </div>
          <ChevronDownIcon 
            class="h-4 w-4 transition-transform duration-200"
            :class="{ 'transform rotate-180': settingsDropdownOpen }"
          />
        </button>
        
        <div 
          v-show="settingsDropdownOpen"
          class="ml-6 space-y-1 border-l border-slate-700 pl-3"
        >
          <NuxtLink to="/settings" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/settings' }">
            General Settings
          </NuxtLink>
          <NuxtLink to="/settings/users" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/settings/users' }">
            User Management
          </NuxtLink>
          <NuxtLink to="/settings/permissions" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/settings/permissions' }">
            Permissions
          </NuxtLink>
          <NuxtLink to="/settings/integrations" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/settings/integrations' }">
            Integrations
          </NuxtLink>
          <NuxtLink to="/settings/backup" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/settings/backup' }">
            Backup & Security
          </NuxtLink>
        </div>
      </div>
    </nav>
    
    <!-- Footer -->
    <div class="p-4 border-t border-slate-800">
      <div class="text-xs text-slate-400 text-center">
        TOSS ERP v1.0.0
      </div>
    </div>
  </aside>
</template>

<script setup>
import { ref, watch } from 'vue'
import { 
  HomeIcon, 
  UsersIcon, 
  BriefcaseIcon, 
  CurrencyDollarIcon, 
  UserGroupIcon, 
  ArchiveBoxIcon, 
  ShoppingCartIcon, 
  ShoppingBagIcon, 
  ChevronDownIcon,
  CogIcon,
  ChatBubbleLeftRightIcon,
  WrenchScrewdriverIcon,
  ChartBarIcon,
  Cog6ToothIcon
} from '@heroicons/vue/24/outline'

// Ensure router is available
const router = useRouter()
const route = useRoute()

// Dropdown states
const crmDropdownOpen = ref(false)
const salesDropdownOpen = ref(false)
const purchasingDropdownOpen = ref(false)
const stockDropdownOpen = ref(false)
const accountingDropdownOpen = ref(false)
const hrDropdownOpen = ref(false)
const projectsDropdownOpen = ref(false)
const automationDropdownOpen = ref(false)
const collaborationDropdownOpen = ref(false)
const manufacturingDropdownOpen = ref(false)
const reportsDropdownOpen = ref(false)
const settingsDropdownOpen = ref(false)

// Auto-open dropdowns if we're on a page within that section
watch(() => route.path, (newPath) => {
  if (newPath.startsWith('/crm')) {
    crmDropdownOpen.value = true
  }
  if (newPath.startsWith('/sales') || newPath.startsWith('/selling')) {
    salesDropdownOpen.value = true
  }
  if (newPath.startsWith('/purchasing') || newPath.startsWith('/buying')) {
    purchasingDropdownOpen.value = true
  }
  if (newPath.startsWith('/stock')) {
    stockDropdownOpen.value = true
  }
  if (newPath.startsWith('/accounting') || newPath.startsWith('/accounts')) {
    accountingDropdownOpen.value = true
  }
  if (newPath.startsWith('/hr')) {
    hrDropdownOpen.value = true
  }
  if (newPath.startsWith('/projects')) {
    projectsDropdownOpen.value = true
  }
  if (newPath.startsWith('/automation')) {
    automationDropdownOpen.value = true
  }
  if (newPath.startsWith('/collaboration')) {
    collaborationDropdownOpen.value = true
  }
  if (newPath.startsWith('/manufacturing')) {
    manufacturingDropdownOpen.value = true
  }
  if (newPath.startsWith('/reports')) {
    reportsDropdownOpen.value = true
  }
  if (newPath.startsWith('/settings') || newPath.startsWith('/admin')) {
    settingsDropdownOpen.value = true
  }
}, { immediate: true })

// Toggle functions
const toggleCrmDropdown = () => {
  crmDropdownOpen.value = !crmDropdownOpen.value
}

const toggleSalesDropdown = () => {
  salesDropdownOpen.value = !salesDropdownOpen.value
}

const togglePurchasingDropdown = () => {
  purchasingDropdownOpen.value = !purchasingDropdownOpen.value
}

const toggleStockDropdown = () => {
  stockDropdownOpen.value = !stockDropdownOpen.value
}

const toggleAccountingDropdown = () => {
  accountingDropdownOpen.value = !accountingDropdownOpen.value
}

const toggleHrDropdown = () => {
  hrDropdownOpen.value = !hrDropdownOpen.value
}

const toggleProjectsDropdown = () => {
  projectsDropdownOpen.value = !projectsDropdownOpen.value
}

const toggleAutomationDropdown = () => {
  automationDropdownOpen.value = !automationDropdownOpen.value
}

const toggleCollaborationDropdown = () => {
  collaborationDropdownOpen.value = !collaborationDropdownOpen.value
}

const toggleManufacturingDropdown = () => {
  manufacturingDropdownOpen.value = !manufacturingDropdownOpen.value
}

const toggleReportsDropdown = () => {
  reportsDropdownOpen.value = !reportsDropdownOpen.value
}

const toggleSettingsDropdown = () => {
  settingsDropdownOpen.value = !settingsDropdownOpen.value
}
</script>

<style scoped>
.nav-link {
  display: flex;
  align-items: center;
  padding: 0.625rem 0.75rem;
  font-size: 0.875rem;
  font-weight: 500;
  color: rgb(203 213 225);
  border-radius: 0.5rem;
  transition: all 0.2s;
}

.nav-link:hover {
  background-color: rgb(30 41 59);
  color: white;
}

.nav-link-active {
  background: linear-gradient(to right, rgb(59 130 246), rgb(147 51 234));
  color: white;
  box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1);
}

.nav-link-active:hover {
  background: linear-gradient(to right, rgb(37 99 235), rgb(126 34 206));
}

.nav-sub-link {
  display: block;
  padding: 0.5rem 0.75rem;
  font-size: 0.8125rem;
  font-weight: 400;
  color: rgb(148 163 184);
  border-radius: 0.375rem;
  transition: all 0.2s;
}

.nav-sub-link:hover {
  background-color: rgb(30 41 59);
  color: rgb(203 213 225);
}

.nav-sub-link-active {
  background-color: rgba(59, 130, 246, 0.2);
  color: rgb(147 197 253);
  font-weight: 500;
}

.nav-sub-link-active:hover {
  background-color: rgba(59, 130, 246, 0.3);
}
</style>
