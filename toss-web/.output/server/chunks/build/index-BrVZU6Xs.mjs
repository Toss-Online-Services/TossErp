import { _ as __nuxt_component_0 } from './nuxt-link-CzepOQdf.mjs';
import { defineComponent, ref, mergeProps, unref, withCtx, createVNode, toDisplayString, useSSRContext } from 'vue';
import { ssrRenderAttrs, ssrInterpolate, ssrRenderComponent, ssrRenderList, ssrRenderClass } from 'vue/server-renderer';
import { u as useUserStore } from './user-B2SCl6g9.mjs';
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

const _sfc_main = /* @__PURE__ */ defineComponent({
  __name: "index",
  __ssrInlineRender: true,
  setup(__props) {
    const userStore = useUserStore();
    const metrics = ref({
      revenue: 125600,
      customers: 847,
      inventory: 89400,
      savings: 12300
    });
    const lowStockItems = ref(23);
    const activeGroupBuys = ref(8);
    const pendingLeads = ref(12);
    const overduelnvoices = ref(3);
    const recentActivities = ref([
      {
        id: 1,
        type: "sale",
        description: "New sale completed",
        date: /* @__PURE__ */ new Date(),
        user: "John Smith",
        amount: "R 1,250.00"
      },
      {
        id: 2,
        type: "groupbuy",
        description: "Joined group purchase for office supplies",
        date: new Date(Date.now() - 36e5),
        user: "You",
        amount: "R 340.00 saved"
      },
      {
        id: 3,
        type: "inventory",
        description: "Stock level alert for Maize Meal",
        date: new Date(Date.now() - 72e5),
        user: "System",
        amount: "15 remaining"
      },
      {
        id: 4,
        type: "payment",
        description: "Payment received from ABC Corp",
        date: new Date(Date.now() - 864e5),
        user: "ABC Corp",
        amount: "R 8,750.00"
      }
    ]);
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
    function getActivityColor(type) {
      switch (type) {
        case "sale":
          return "bg-green-500";
        case "groupbuy":
          return "bg-blue-500";
        case "inventory":
          return "bg-yellow-500";
        case "payment":
          return "bg-purple-500";
        default:
          return "bg-gray-500";
      }
    }
    function getActivityIcon(type) {
      switch (type) {
        case "sale":
          return "\u{1F4B0}";
        case "groupbuy":
          return "\u{1F91D}";
        case "inventory":
          return "\u{1F4E6}";
        case "payment":
          return "\u{1F4B3}";
        default:
          return "\u{1F4CB}";
      }
    }
    useHead({
      title: "Dashboard - TOSS ERP",
      meta: [
        { name: "description", content: "Business dashboard with key metrics and insights in TOSS ERP" }
      ]
    });
    return (_ctx, _push, _parent, _attrs) => {
      var _a, _b;
      const _component_NuxtLink = __nuxt_component_0;
      _push(`<div${ssrRenderAttrs(mergeProps({ class: "min-h-screen bg-gray-50 dark:bg-gray-900" }, _attrs))}><div class="bg-white dark:bg-gray-800 shadow-sm border-b border-gray-200 dark:border-gray-700"><div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8"><div class="py-4"><div class="flex items-center justify-between"><div><h1 class="text-2xl font-bold text-gray-900 dark:text-white"> Welcome back, ${ssrInterpolate((_a = unref(userStore).user) == null ? void 0 : _a.firstName)}! </h1><p class="text-gray-600 dark:text-gray-400">Overview of ${ssrInterpolate(((_b = unref(userStore).user) == null ? void 0 : _b.businessName) || "your business")} performance and key metrics</p></div><div class="flex space-x-3"><button class="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors"><span class="inline-block w-5 h-5 mr-2">\u{1F4CA}</span> Generate Report </button></div></div></div></div></div><div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6"><div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8"><div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700"><div class="flex items-center justify-between"><div><p class="text-sm text-gray-600 dark:text-gray-400">Revenue This Month</p><p class="text-2xl font-bold text-green-600">R ${ssrInterpolate(formatCurrency(unref(metrics).revenue))}</p><p class="text-sm text-green-600 mt-1">\u2197 +12.5% from last month</p></div><div class="p-3 bg-green-100 dark:bg-green-900 rounded-full"><span class="text-2xl">\u{1F4B0}</span></div></div></div><div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700"><div class="flex items-center justify-between"><div><p class="text-sm text-gray-600 dark:text-gray-400">Active Customers</p><p class="text-2xl font-bold text-blue-600">${ssrInterpolate(unref(metrics).customers)}</p><p class="text-sm text-blue-600 mt-1">\u2197 +8.2% from last month</p></div><div class="p-3 bg-blue-100 dark:bg-blue-900 rounded-full"><span class="text-2xl">\u{1F465}</span></div></div></div><div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700"><div class="flex items-center justify-between"><div><p class="text-sm text-gray-600 dark:text-gray-400">Inventory Value</p><p class="text-2xl font-bold text-purple-600">R ${ssrInterpolate(formatCurrency(unref(metrics).inventory))}</p><p class="text-sm text-red-600 mt-1">\u2198 -3.1% from last month</p></div><div class="p-3 bg-purple-100 dark:bg-purple-900 rounded-full"><span class="text-2xl">\u{1F4E6}</span></div></div></div><div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700"><div class="flex items-center justify-between"><div><p class="text-sm text-gray-600 dark:text-gray-400">Group Buy Savings</p><p class="text-2xl font-bold text-orange-600">R ${ssrInterpolate(formatCurrency(unref(metrics).savings))}</p><p class="text-sm text-orange-600 mt-1">\u2197 +24.8% from last month</p></div><div class="p-3 bg-orange-100 dark:bg-orange-900 rounded-full"><span class="text-2xl">\u{1F91D}</span></div></div></div></div><div class="grid grid-cols-1 lg:grid-cols-3 gap-6 mb-8"><div class="lg:col-span-2"><div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700"><div class="p-6 border-b border-gray-200 dark:border-gray-700"><h3 class="text-lg font-semibold text-gray-900 dark:text-white">Revenue Overview</h3></div><div class="p-6"><div class="h-64 flex items-center justify-center bg-gray-50 dark:bg-gray-700 rounded-lg"><p class="text-gray-500 dark:text-gray-400">Revenue chart would be displayed here</p></div></div></div></div><div><div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700"><div class="p-6 border-b border-gray-200 dark:border-gray-700"><h3 class="text-lg font-semibold text-gray-900 dark:text-white">Quick Actions</h3></div><div class="p-6"><div class="space-y-4">`);
      _push(ssrRenderComponent(_component_NuxtLink, {
        to: "/inventory",
        class: "flex items-center space-x-3 p-3 rounded-lg border border-gray-200 dark:border-gray-600 hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
      }, {
        default: withCtx((_, _push2, _parent2, _scopeId) => {
          if (_push2) {
            _push2(`<span class="text-2xl"${_scopeId}>\u{1F4E6}</span><div${_scopeId}><p class="font-medium text-gray-900 dark:text-white"${_scopeId}>Check Inventory</p><p class="text-sm text-gray-600 dark:text-gray-400"${_scopeId}>${ssrInterpolate(unref(lowStockItems))} items low on stock</p></div>`);
          } else {
            return [
              createVNode("span", { class: "text-2xl" }, "\u{1F4E6}"),
              createVNode("div", null, [
                createVNode("p", { class: "font-medium text-gray-900 dark:text-white" }, "Check Inventory"),
                createVNode("p", { class: "text-sm text-gray-600 dark:text-gray-400" }, toDisplayString(unref(lowStockItems)) + " items low on stock", 1)
              ])
            ];
          }
        }),
        _: 1
      }, _parent));
      _push(ssrRenderComponent(_component_NuxtLink, {
        to: "/group-buying",
        class: "flex items-center space-x-3 p-3 rounded-lg border border-gray-200 dark:border-gray-600 hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
      }, {
        default: withCtx((_, _push2, _parent2, _scopeId) => {
          if (_push2) {
            _push2(`<span class="text-2xl"${_scopeId}>\u{1F91D}</span><div${_scopeId}><p class="font-medium text-gray-900 dark:text-white"${_scopeId}>Join Group Buys</p><p class="text-sm text-gray-600 dark:text-gray-400"${_scopeId}>${ssrInterpolate(unref(activeGroupBuys))} active opportunities</p></div>`);
          } else {
            return [
              createVNode("span", { class: "text-2xl" }, "\u{1F91D}"),
              createVNode("div", null, [
                createVNode("p", { class: "font-medium text-gray-900 dark:text-white" }, "Join Group Buys"),
                createVNode("p", { class: "text-sm text-gray-600 dark:text-gray-400" }, toDisplayString(unref(activeGroupBuys)) + " active opportunities", 1)
              ])
            ];
          }
        }),
        _: 1
      }, _parent));
      _push(ssrRenderComponent(_component_NuxtLink, {
        to: "/crm",
        class: "flex items-center space-x-3 p-3 rounded-lg border border-gray-200 dark:border-gray-600 hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
      }, {
        default: withCtx((_, _push2, _parent2, _scopeId) => {
          if (_push2) {
            _push2(`<span class="text-2xl"${_scopeId}>\u{1F465}</span><div${_scopeId}><p class="font-medium text-gray-900 dark:text-white"${_scopeId}>Manage Customers</p><p class="text-sm text-gray-600 dark:text-gray-400"${_scopeId}>${ssrInterpolate(unref(pendingLeads))} leads need follow-up</p></div>`);
          } else {
            return [
              createVNode("span", { class: "text-2xl" }, "\u{1F465}"),
              createVNode("div", null, [
                createVNode("p", { class: "font-medium text-gray-900 dark:text-white" }, "Manage Customers"),
                createVNode("p", { class: "text-sm text-gray-600 dark:text-gray-400" }, toDisplayString(unref(pendingLeads)) + " leads need follow-up", 1)
              ])
            ];
          }
        }),
        _: 1
      }, _parent));
      _push(ssrRenderComponent(_component_NuxtLink, {
        to: "/accounts",
        class: "flex items-center space-x-3 p-3 rounded-lg border border-gray-200 dark:border-gray-600 hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
      }, {
        default: withCtx((_, _push2, _parent2, _scopeId) => {
          if (_push2) {
            _push2(`<span class="text-2xl"${_scopeId}>\u{1F4B0}</span><div${_scopeId}><p class="font-medium text-gray-900 dark:text-white"${_scopeId}>View Financials</p><p class="text-sm text-gray-600 dark:text-gray-400"${_scopeId}>${ssrInterpolate(unref(overduelnvoices))} overdue invoices</p></div>`);
          } else {
            return [
              createVNode("span", { class: "text-2xl" }, "\u{1F4B0}"),
              createVNode("div", null, [
                createVNode("p", { class: "font-medium text-gray-900 dark:text-white" }, "View Financials"),
                createVNode("p", { class: "text-sm text-gray-600 dark:text-gray-400" }, toDisplayString(unref(overduelnvoices)) + " overdue invoices", 1)
              ])
            ];
          }
        }),
        _: 1
      }, _parent));
      _push(`</div></div></div></div></div><div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700"><div class="p-6 border-b border-gray-200 dark:border-gray-700"><h3 class="text-lg font-semibold text-gray-900 dark:text-white">Recent Activity</h3></div><div class="p-6"><div class="space-y-4"><!--[-->`);
      ssrRenderList(unref(recentActivities), (activity) => {
        _push(`<div class="flex items-center space-x-4 p-4 bg-gray-50 dark:bg-gray-700 rounded-lg"><div class="${ssrRenderClass([getActivityColor(activity.type), "w-10 h-10 rounded-full flex items-center justify-center"])}"><span class="text-white text-sm">${ssrInterpolate(getActivityIcon(activity.type))}</span></div><div class="flex-1"><p class="font-medium text-gray-900 dark:text-white">${ssrInterpolate(activity.description)}</p><p class="text-sm text-gray-600 dark:text-gray-400">${ssrInterpolate(formatDate(activity.date))} \u2022 ${ssrInterpolate(activity.user)}</p></div><div class="text-right"><span class="text-sm font-medium text-gray-900 dark:text-white">${ssrInterpolate(activity.amount)}</span></div></div>`);
      });
      _push(`<!--]--></div></div></div></div></div>`);
    };
  }
});
const _sfc_setup = _sfc_main.setup;
_sfc_main.setup = (props, ctx) => {
  const ssrContext = useSSRContext();
  (ssrContext.modules || (ssrContext.modules = /* @__PURE__ */ new Set())).add("pages/dashboard/index.vue");
  return _sfc_setup ? _sfc_setup(props, ctx) : void 0;
};

export { _sfc_main as default };
//# sourceMappingURL=index-BrVZU6Xs.mjs.map
