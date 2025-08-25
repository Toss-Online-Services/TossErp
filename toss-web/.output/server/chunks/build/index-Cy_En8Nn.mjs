import { defineComponent, ref, mergeProps, unref, createVNode, resolveDynamicComponent, useSSRContext } from 'vue';
import { ssrRenderAttrs, ssrRenderComponent, ssrInterpolate, ssrRenderList, ssrRenderClass, ssrRenderVNode } from 'vue/server-renderer';
import { u as useHead } from './server.mjs';
import '../_/nitro.mjs';
import 'node:http';
import 'node:https';
import 'node:events';
import 'node:buffer';
import 'node:fs';
import 'node:path';
import 'node:crypto';
import 'node:url';
import '../routes/renderer.mjs';
import 'vue-bundle-renderer/runtime';
import 'unhead/server';
import 'devalue';
import 'unhead/utils';
import 'unhead/plugins';
import 'vue-router';

const PlusIcon = "svg";
const UserPlusIcon = "svg";
const UsersIcon = "svg";
const ChartBarIcon = "svg";
const TrendingUpIcon = "svg";
const CurrencyDollarIcon = "svg";
const FunnelIcon = "svg";
const _sfc_main = /* @__PURE__ */ defineComponent({
  __name: "index",
  __ssrInlineRender: true,
  setup(__props) {
    const stats = ref({
      totalCustomers: 1247,
      activeLeads: 89,
      conversionRate: 24.5,
      pipelineValue: 45e4
    });
    const pipelineStages = ref([
      {
        name: "Prospecting",
        count: 12,
        opportunities: [
          { id: 1, customer: "ABC Corp", product: "ERP System", value: 25e3 },
          { id: 2, customer: "XYZ Ltd", product: "Inventory Management", value: 15e3 }
        ]
      },
      {
        name: "Qualification",
        count: 8,
        opportunities: [
          { id: 3, customer: "Tech Solutions", product: "CRM Integration", value: 35e3 },
          { id: 4, customer: "Local Store", product: "POS System", value: 8e3 }
        ]
      },
      {
        name: "Proposal",
        count: 5,
        opportunities: [
          { id: 5, customer: "Manufacturing Co", product: "Full ERP", value: 12e4 }
        ]
      },
      {
        name: "Negotiation",
        count: 3,
        opportunities: [
          { id: 6, customer: "Retail Chain", product: "Multi-store System", value: 85e3 }
        ]
      }
    ]);
    const recentActivities = ref([
      { id: 1, type: "call", description: "Called ABC Corp about proposal follow-up", date: /* @__PURE__ */ new Date() },
      { id: 2, type: "email", description: "Sent quote to XYZ Ltd", date: new Date(Date.now() - 36e5) },
      { id: 3, type: "meeting", description: "Demo scheduled with Tech Solutions", date: new Date(Date.now() - 72e5) },
      { id: 4, type: "note", description: "Updated lead status for Local Store", date: new Date(Date.now() - 864e5) }
    ]);
    const customers = ref([
      {
        id: 1,
        name: "John Smith",
        email: "john@abccorp.co.za",
        company: "ABC Corp",
        status: "Active",
        totalSales: 125e3,
        lastContact: new Date(Date.now() - 864e5)
      },
      {
        id: 2,
        name: "Sarah Johnson",
        email: "sarah@xyzltd.co.za",
        company: "XYZ Ltd",
        status: "Lead",
        totalSales: 0,
        lastContact: new Date(Date.now() - 1728e5)
      },
      {
        id: 3,
        name: "Mike Wilson",
        email: "mike@techsol.co.za",
        company: "Tech Solutions",
        status: "Prospect",
        totalSales: 45e3,
        lastContact: new Date(Date.now() - 2592e5)
      }
    ]);
    ref(false);
    ref(false);
    function formatCurrency(amount) {
      return new Intl.NumberFormat("en-ZA", {
        style: "decimal",
        minimumFractionDigits: 0,
        maximumFractionDigits: 0
      }).format(amount);
    }
    function formatDate(date) {
      return new Intl.DateTimeFormat("en-ZA", {
        month: "short",
        day: "numeric",
        hour: "2-digit",
        minute: "2-digit"
      }).format(date);
    }
    function getInitials(name) {
      return name.split(" ").map((n) => n[0]).join("").toUpperCase();
    }
    function getActivityColor(type) {
      switch (type) {
        case "call":
          return "bg-blue-500";
        case "email":
          return "bg-green-500";
        case "meeting":
          return "bg-purple-500";
        case "note":
          return "bg-yellow-500";
        default:
          return "bg-gray-500";
      }
    }
    function getActivityIcon(type) {
      switch (type) {
        case "call":
          return "svg";
        case "email":
          return "svg";
        case "meeting":
          return "svg";
        case "note":
          return "svg";
        default:
          return "svg";
      }
    }
    function getStatusColor(status) {
      switch (status) {
        case "Active":
          return "bg-green-100 text-green-800";
        case "Lead":
          return "bg-yellow-100 text-yellow-800";
        case "Prospect":
          return "bg-blue-100 text-blue-800";
        default:
          return "bg-gray-100 text-gray-800";
      }
    }
    useHead({
      title: "CRM - TOSS ERP",
      meta: [
        { name: "description", content: "Customer relationship management and sales pipeline in TOSS ERP" }
      ]
    });
    return (_ctx, _push, _parent, _attrs) => {
      _push(`<div${ssrRenderAttrs(mergeProps({ class: "min-h-screen bg-gray-50 dark:bg-gray-900" }, _attrs))}><div class="bg-white dark:bg-gray-800 shadow-sm border-b border-gray-200 dark:border-gray-700"><div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8"><div class="py-4"><div class="flex items-center justify-between"><div><h1 class="text-2xl font-bold text-gray-900 dark:text-white">Customer Relationship Management</h1><p class="text-gray-600 dark:text-gray-400">Manage customers, leads, and sales opportunities</p></div><div class="flex space-x-3"><button class="bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700 transition-colors">`);
      _push(ssrRenderComponent(PlusIcon, { class: "w-5 h-5 inline mr-2" }, null, _parent));
      _push(` New Lead </button><button class="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors">`);
      _push(ssrRenderComponent(UserPlusIcon, { class: "w-5 h-5 inline mr-2" }, null, _parent));
      _push(` New Customer </button></div></div></div></div></div><div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6"><div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8"><div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700"><div class="flex items-center justify-between"><div><p class="text-sm text-gray-600 dark:text-gray-400">Total Customers</p><p class="text-2xl font-bold text-blue-600">${ssrInterpolate(unref(stats).totalCustomers)}</p></div><div class="p-3 bg-blue-100 dark:bg-blue-900 rounded-full">`);
      _push(ssrRenderComponent(UsersIcon, { class: "w-6 h-6 text-blue-600" }, null, _parent));
      _push(`</div></div></div><div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700"><div class="flex items-center justify-between"><div><p class="text-sm text-gray-600 dark:text-gray-400">Active Leads</p><p class="text-2xl font-bold text-yellow-600">${ssrInterpolate(unref(stats).activeLeads)}</p></div><div class="p-3 bg-yellow-100 dark:bg-yellow-900 rounded-full">`);
      _push(ssrRenderComponent(ChartBarIcon, { class: "w-6 h-6 text-yellow-600" }, null, _parent));
      _push(`</div></div></div><div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700"><div class="flex items-center justify-between"><div><p class="text-sm text-gray-600 dark:text-gray-400">Conversion Rate</p><p class="text-2xl font-bold text-green-600">${ssrInterpolate(unref(stats).conversionRate)}%</p></div><div class="p-3 bg-green-100 dark:bg-green-900 rounded-full">`);
      _push(ssrRenderComponent(TrendingUpIcon, { class: "w-6 h-6 text-green-600" }, null, _parent));
      _push(`</div></div></div><div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700"><div class="flex items-center justify-between"><div><p class="text-sm text-gray-600 dark:text-gray-400">Pipeline Value</p><p class="text-2xl font-bold text-purple-600">R ${ssrInterpolate(formatCurrency(unref(stats).pipelineValue))}</p></div><div class="p-3 bg-purple-100 dark:bg-purple-900 rounded-full">`);
      _push(ssrRenderComponent(CurrencyDollarIcon, { class: "w-6 h-6 text-purple-600" }, null, _parent));
      _push(`</div></div></div></div><div class="grid grid-cols-1 lg:grid-cols-3 gap-6 mb-8"><div class="lg:col-span-2"><div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700"><div class="p-6 border-b border-gray-200 dark:border-gray-700"><div class="flex items-center justify-between"><h3 class="text-lg font-semibold text-gray-900 dark:text-white">Sales Pipeline</h3><select class="text-sm border border-gray-300 dark:border-gray-600 rounded-lg px-3 py-1 bg-white dark:bg-gray-700 text-gray-900 dark:text-white"><option>All Stages</option><option>Prospecting</option><option>Qualification</option><option>Proposal</option><option>Negotiation</option></select></div></div><div class="p-6"><div class="grid grid-cols-1 md:grid-cols-4 gap-4"><!--[-->`);
      ssrRenderList(unref(pipelineStages), (stage) => {
        _push(`<div class="bg-gray-50 dark:bg-gray-700 rounded-lg p-4"><div class="flex items-center justify-between mb-3"><h4 class="font-medium text-gray-900 dark:text-white">${ssrInterpolate(stage.name)}</h4><span class="text-xs text-gray-500 dark:text-gray-400">${ssrInterpolate(stage.count)}</span></div><div class="space-y-2"><!--[-->`);
        ssrRenderList(stage.opportunities, (opportunity) => {
          _push(`<div class="bg-white dark:bg-gray-800 p-3 rounded border border-gray-200 dark:border-gray-600 cursor-pointer hover:shadow-md transition-shadow"><p class="font-medium text-sm text-gray-900 dark:text-white">${ssrInterpolate(opportunity.customer)}</p><p class="text-xs text-gray-600 dark:text-gray-400">${ssrInterpolate(opportunity.product)}</p><p class="text-sm font-semibold text-green-600 mt-1">R ${ssrInterpolate(formatCurrency(opportunity.value))}</p></div>`);
        });
        _push(`<!--]--></div></div>`);
      });
      _push(`<!--]--></div></div></div></div><div><div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700"><div class="p-6 border-b border-gray-200 dark:border-gray-700"><h3 class="text-lg font-semibold text-gray-900 dark:text-white">Recent Activities</h3></div><div class="p-6"><div class="space-y-4"><!--[-->`);
      ssrRenderList(unref(recentActivities), (activity) => {
        _push(`<div class="flex items-start space-x-3"><div class="${ssrRenderClass([getActivityColor(activity.type), "w-8 h-8 rounded-full flex items-center justify-center"])}">`);
        ssrRenderVNode(_push, createVNode(resolveDynamicComponent(getActivityIcon(activity.type)), { class: "w-4 h-4 text-white" }, null), _parent);
        _push(`</div><div class="flex-1"><p class="text-sm text-gray-900 dark:text-white">${ssrInterpolate(activity.description)}</p><p class="text-xs text-gray-600 dark:text-gray-400">${ssrInterpolate(formatDate(activity.date))}</p></div></div>`);
      });
      _push(`<!--]--></div></div></div></div></div><div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700"><div class="p-6 border-b border-gray-200 dark:border-gray-700"><div class="flex items-center justify-between"><h3 class="text-lg font-semibold text-gray-900 dark:text-white">Recent Customers</h3><div class="flex space-x-2"><input type="text" placeholder="Search customers..." class="px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white text-sm"><button class="bg-gray-100 dark:bg-gray-700 text-gray-600 dark:text-gray-400 px-3 py-2 rounded-lg hover:bg-gray-200 dark:hover:bg-gray-600 transition-colors">`);
      _push(ssrRenderComponent(FunnelIcon, { class: "w-4 h-4" }, null, _parent));
      _push(`</button></div></div></div><div class="overflow-x-auto"><table class="w-full"><thead class="bg-gray-50 dark:bg-gray-700"><tr><th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">Customer</th><th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">Company</th><th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">Status</th><th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">Total Sales</th><th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">Last Contact</th><th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">Actions</th></tr></thead><tbody class="bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700"><!--[-->`);
      ssrRenderList(unref(customers), (customer) => {
        _push(`<tr class="hover:bg-gray-50 dark:hover:bg-gray-700"><td class="px-6 py-4 whitespace-nowrap"><div class="flex items-center"><div class="w-10 h-10 bg-blue-100 dark:bg-blue-900 rounded-full flex items-center justify-center"><span class="text-blue-600 font-medium text-sm">${ssrInterpolate(getInitials(customer.name))}</span></div><div class="ml-4"><div class="text-sm font-medium text-gray-900 dark:text-white">${ssrInterpolate(customer.name)}</div><div class="text-sm text-gray-500 dark:text-gray-400">${ssrInterpolate(customer.email)}</div></div></div></td><td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">${ssrInterpolate(customer.company)}</td><td class="px-6 py-4 whitespace-nowrap"><span class="${ssrRenderClass([getStatusColor(customer.status), "inline-flex px-2 py-1 text-xs font-semibold rounded-full"])}">${ssrInterpolate(customer.status)}</span></td><td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900 dark:text-white"> R ${ssrInterpolate(formatCurrency(customer.totalSales))}</td><td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400">${ssrInterpolate(formatDate(customer.lastContact))}</td><td class="px-6 py-4 whitespace-nowrap text-sm font-medium"><div class="flex space-x-2"><button class="text-blue-600 hover:text-blue-900">View</button><button class="text-green-600 hover:text-green-900">Contact</button></div></td></tr>`);
      });
      _push(`<!--]--></tbody></table></div></div></div></div>`);
    };
  }
});
const _sfc_setup = _sfc_main.setup;
_sfc_main.setup = (props, ctx) => {
  const ssrContext = useSSRContext();
  (ssrContext.modules || (ssrContext.modules = /* @__PURE__ */ new Set())).add("pages/crm/index.vue");
  return _sfc_setup ? _sfc_setup(props, ctx) : void 0;
};

export { _sfc_main as default };
//# sourceMappingURL=index-Cy_En8Nn.mjs.map
