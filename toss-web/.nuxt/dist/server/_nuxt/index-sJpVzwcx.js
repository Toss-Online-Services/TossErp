import { defineComponent, ref, mergeProps, unref, createVNode, resolveDynamicComponent, useSSRContext } from "vue";
import { ssrRenderAttrs, ssrRenderComponent, ssrInterpolate, ssrRenderList, ssrRenderClass, ssrRenderVNode } from "vue/server-renderer";
import { u as useHead } from "../server.mjs";
import "ofetch";
import "#internal/nuxt/paths";
import "C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/node_modules/hookable/dist/index.mjs";
import "C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/node_modules/unctx/dist/index.mjs";
import "C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/node_modules/h3/dist/index.mjs";
import "vue-router";
import "C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/node_modules/radix3/dist/index.mjs";
import "C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/node_modules/defu/dist/defu.mjs";
import "C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/node_modules/ufo/dist/index.mjs";
import "C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/node_modules/klona/dist/index.mjs";
import "C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/node_modules/@unhead/vue/dist/index.mjs";
const PlusIcon = "svg";
const CubeIcon = "svg";
const ClipboardListIcon = "svg";
const ExclamationTriangleIcon = "svg";
const CurrencyDollarIcon = "svg";
const BuildingStorefrontIcon = "svg";
const _sfc_main = /* @__PURE__ */ defineComponent({
  __name: "index",
  __ssrInlineRender: true,
  setup(__props) {
    const stats = ref({
      totalItems: 1247,
      lowStockItems: 23,
      totalValue: 65e4,
      warehouses: 3
    });
    const inventoryItems = ref([
      {
        id: 1,
        name: "Maize Meal 5kg",
        category: "Food & Beverages",
        currentStock: 15,
        minStock: 20,
        unitPrice: 45.99,
        lastUpdated: /* @__PURE__ */ new Date()
      },
      {
        id: 2,
        name: "Cooking Oil 2L",
        category: "Food & Beverages",
        currentStock: 8,
        minStock: 15,
        unitPrice: 89.99,
        lastUpdated: /* @__PURE__ */ new Date()
      },
      {
        id: 3,
        name: "Office Paper A4",
        category: "Office Supplies",
        currentStock: 50,
        minStock: 10,
        unitPrice: 12.5,
        lastUpdated: /* @__PURE__ */ new Date()
      },
      {
        id: 4,
        name: "USB Flash Drive 32GB",
        category: "Electronics",
        currentStock: 25,
        minStock: 5,
        unitPrice: 125,
        lastUpdated: /* @__PURE__ */ new Date()
      }
    ]);
    const stockAlerts = ref([
      { id: 1, item: "Maize Meal 5kg", current: 15, minimum: 20 },
      { id: 2, item: "Cooking Oil 2L", current: 8, minimum: 15 },
      { id: 3, item: "Sugar 2.5kg", current: 3, minimum: 10 }
    ]);
    const recentMovements = ref([
      { id: 1, item: "Office Paper A4", type: "IN", quantity: 20, date: /* @__PURE__ */ new Date() },
      { id: 2, item: "USB Flash Drive", type: "OUT", quantity: 5, date: new Date(Date.now() - 36e5) },
      { id: 3, item: "Maize Meal 5kg", type: "OUT", quantity: 10, date: new Date(Date.now() - 72e5) }
    ]);
    const groupPurchasingOpportunities = ref([
      {
        id: 1,
        item: "Bulk Maize Meal (50x 5kg)",
        currentPrice: 45.99,
        groupPrice: 35.99,
        savings: 22,
        participants: 8
      },
      {
        id: 2,
        item: "Cooking Oil Case (12x 2L)",
        currentPrice: 89.99,
        groupPrice: 74.99,
        savings: 17,
        participants: 5
      },
      {
        id: 3,
        item: "Office Supplies Bundle",
        currentPrice: 350,
        groupPrice: 275,
        savings: 21,
        participants: 12
      }
    ]);
    ref(false);
    ref(false);
    function formatCurrency(amount) {
      return new Intl.NumberFormat("en-ZA", {
        style: "decimal",
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
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
    function getStockStatus(item) {
      if (item.currentStock <= item.minStock) return "Low Stock";
      if (item.currentStock <= item.minStock * 1.5) return "Reorder Soon";
      return "In Stock";
    }
    function getStockStatusColor(item) {
      if (item.currentStock <= item.minStock) return "bg-red-100 text-red-800";
      if (item.currentStock <= item.minStock * 1.5) return "bg-yellow-100 text-yellow-800";
      return "bg-green-100 text-green-800";
    }
    function getMovementColor(type) {
      return type === "IN" ? "bg-green-500" : "bg-red-500";
    }
    function getMovementIcon(type) {
      return type === "IN" ? "svg" : "svg";
    }
    useHead({
      title: "Inventory - TOSS ERP",
      meta: [
        { name: "description", content: "Inventory management and stock control in TOSS ERP" }
      ]
    });
    return (_ctx, _push, _parent, _attrs) => {
      _push(`<div${ssrRenderAttrs(mergeProps({ class: "min-h-screen bg-gray-50 dark:bg-gray-900" }, _attrs))}><div class="bg-white dark:bg-gray-800 shadow-sm border-b border-gray-200 dark:border-gray-700"><div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8"><div class="py-4"><div class="flex items-center justify-between"><div><h1 class="text-2xl font-bold text-gray-900 dark:text-white">Inventory Management</h1><p class="text-gray-600 dark:text-gray-400">Track stock levels, manage warehouses, and optimize inventory</p></div><div class="flex space-x-3"><button class="bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700 transition-colors">`);
      _push(ssrRenderComponent(PlusIcon, { class: "w-5 h-5 inline mr-2" }, null, _parent));
      _push(` Add Stock </button><button class="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors">`);
      _push(ssrRenderComponent(CubeIcon, { class: "w-5 h-5 inline mr-2" }, null, _parent));
      _push(` New Item </button><button class="bg-purple-600 text-white px-4 py-2 rounded-lg hover:bg-purple-700 transition-colors">`);
      _push(ssrRenderComponent(ClipboardListIcon, { class: "w-5 h-5 inline mr-2" }, null, _parent));
      _push(` Stock Take </button></div></div></div></div></div><div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6"><div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8"><div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700"><div class="flex items-center justify-between"><div><p class="text-sm text-gray-600 dark:text-gray-400">Total Items</p><p class="text-2xl font-bold text-blue-600">${ssrInterpolate(unref(stats).totalItems)}</p></div><div class="p-3 bg-blue-100 dark:bg-blue-900 rounded-full">`);
      _push(ssrRenderComponent(CubeIcon, { class: "w-6 h-6 text-blue-600" }, null, _parent));
      _push(`</div></div></div><div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700"><div class="flex items-center justify-between"><div><p class="text-sm text-gray-600 dark:text-gray-400">Low Stock Items</p><p class="text-2xl font-bold text-red-600">${ssrInterpolate(unref(stats).lowStockItems)}</p></div><div class="p-3 bg-red-100 dark:bg-red-900 rounded-full">`);
      _push(ssrRenderComponent(ExclamationTriangleIcon, { class: "w-6 h-6 text-red-600" }, null, _parent));
      _push(`</div></div></div><div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700"><div class="flex items-center justify-between"><div><p class="text-sm text-gray-600 dark:text-gray-400">Total Value</p><p class="text-2xl font-bold text-green-600">R ${ssrInterpolate(formatCurrency(unref(stats).totalValue))}</p></div><div class="p-3 bg-green-100 dark:bg-green-900 rounded-full">`);
      _push(ssrRenderComponent(CurrencyDollarIcon, { class: "w-6 h-6 text-green-600" }, null, _parent));
      _push(`</div></div></div><div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700"><div class="flex items-center justify-between"><div><p class="text-sm text-gray-600 dark:text-gray-400">Warehouses</p><p class="text-2xl font-bold text-purple-600">${ssrInterpolate(unref(stats).warehouses)}</p></div><div class="p-3 bg-purple-100 dark:bg-purple-900 rounded-full">`);
      _push(ssrRenderComponent(BuildingStorefrontIcon, { class: "w-6 h-6 text-purple-600" }, null, _parent));
      _push(`</div></div></div></div><div class="grid grid-cols-1 lg:grid-cols-3 gap-6 mb-8"><div class="lg:col-span-2"><div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700"><div class="p-6 border-b border-gray-200 dark:border-gray-700"><div class="flex items-center justify-between"><h3 class="text-lg font-semibold text-gray-900 dark:text-white">Inventory Overview</h3><div class="flex space-x-2"><select class="text-sm border border-gray-300 dark:border-gray-600 rounded-lg px-3 py-1 bg-white dark:bg-gray-700 text-gray-900 dark:text-white"><option>All Categories</option><option>Electronics</option><option>Clothing</option><option>Food &amp; Beverages</option><option>Office Supplies</option></select><select class="text-sm border border-gray-300 dark:border-gray-600 rounded-lg px-3 py-1 bg-white dark:bg-gray-700 text-gray-900 dark:text-white"><option>All Warehouses</option><option>Main Warehouse</option><option>Store Front</option><option>Storage Room</option></select></div></div></div><div class="p-6"><div class="overflow-x-auto"><table class="w-full"><thead><tr class="text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider"><th class="pb-3">Item</th><th class="pb-3">Stock Level</th><th class="pb-3">Unit Price</th><th class="pb-3">Value</th><th class="pb-3">Status</th></tr></thead><tbody class="space-y-2"><!--[-->`);
      ssrRenderList(unref(inventoryItems), (item) => {
        _push(`<tr class="border-b border-gray-100 dark:border-gray-700"><td class="py-3"><div class="flex items-center space-x-3"><div class="w-10 h-10 bg-gray-100 dark:bg-gray-700 rounded-lg flex items-center justify-center">`);
        _push(ssrRenderComponent(CubeIcon, { class: "w-5 h-5 text-gray-600" }, null, _parent));
        _push(`</div><div><p class="font-medium text-gray-900 dark:text-white">${ssrInterpolate(item.name)}</p><p class="text-sm text-gray-600 dark:text-gray-400">${ssrInterpolate(item.category)}</p></div></div></td><td class="py-3"><div><p class="font-medium text-gray-900 dark:text-white">${ssrInterpolate(item.currentStock)}</p><p class="text-sm text-gray-600 dark:text-gray-400">Min: ${ssrInterpolate(item.minStock)}</p></div></td><td class="py-3 text-gray-900 dark:text-white">R ${ssrInterpolate(formatCurrency(item.unitPrice))}</td><td class="py-3 font-medium text-gray-900 dark:text-white">R ${ssrInterpolate(formatCurrency(item.currentStock * item.unitPrice))}</td><td class="py-3"><span class="${ssrRenderClass([getStockStatusColor(item), "inline-flex px-2 py-1 text-xs font-semibold rounded-full"])}">${ssrInterpolate(getStockStatus(item))}</span></td></tr>`);
      });
      _push(`<!--]--></tbody></table></div></div></div></div><div class="space-y-6"><div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700"><div class="p-6 border-b border-gray-200 dark:border-gray-700"><h3 class="text-lg font-semibold text-gray-900 dark:text-white">Stock Alerts</h3></div><div class="p-6"><div class="space-y-4"><!--[-->`);
      ssrRenderList(unref(stockAlerts), (alert) => {
        _push(`<div class="flex items-center space-x-3 p-3 bg-red-50 dark:bg-red-900/20 rounded-lg border border-red-200 dark:border-red-800">`);
        _push(ssrRenderComponent(ExclamationTriangleIcon, { class: "w-5 h-5 text-red-600 flex-shrink-0" }, null, _parent));
        _push(`<div class="flex-1"><p class="text-sm font-medium text-red-800 dark:text-red-400">${ssrInterpolate(alert.item)}</p><p class="text-xs text-red-600 dark:text-red-500">${ssrInterpolate(alert.current)} left (Min: ${ssrInterpolate(alert.minimum)})</p></div><button class="text-red-600 hover:text-red-800 text-xs bg-white dark:bg-gray-800 px-2 py-1 rounded border border-red-300 dark:border-red-700"> Reorder </button></div>`);
      });
      _push(`<!--]--></div></div></div><div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700"><div class="p-6 border-b border-gray-200 dark:border-gray-700"><h3 class="text-lg font-semibold text-gray-900 dark:text-white">Recent Movements</h3></div><div class="p-6"><div class="space-y-4"><!--[-->`);
      ssrRenderList(unref(recentMovements), (movement) => {
        _push(`<div class="flex items-center justify-between"><div class="flex items-center space-x-3"><div class="${ssrRenderClass([getMovementColor(movement.type), "w-8 h-8 rounded-full flex items-center justify-center"])}">`);
        ssrRenderVNode(_push, createVNode(resolveDynamicComponent(getMovementIcon(movement.type)), { class: "w-4 h-4 text-white" }, null), _parent);
        _push(`</div><div><p class="text-sm font-medium text-gray-900 dark:text-white">${ssrInterpolate(movement.item)}</p><p class="text-xs text-gray-600 dark:text-gray-400">${ssrInterpolate(movement.type)} • ${ssrInterpolate(formatDate(movement.date))}</p></div></div><div class="text-right"><p class="${ssrRenderClass([movement.type === "IN" ? "text-green-600" : "text-red-600", "text-sm font-medium"])}">${ssrInterpolate(movement.type === "IN" ? "+" : "-")}${ssrInterpolate(movement.quantity)}</p></div></div>`);
      });
      _push(`<!--]--></div></div></div></div></div><div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 mb-8"><div class="p-6 border-b border-gray-200 dark:border-gray-700"><div class="flex items-center justify-between"><h3 class="text-lg font-semibold text-gray-900 dark:text-white">Group Purchasing Opportunities</h3><button class="text-blue-600 hover:text-blue-800 text-sm font-medium">View All</button></div></div><div class="p-6"><div class="grid grid-cols-1 md:grid-cols-3 gap-4"><!--[-->`);
      ssrRenderList(unref(groupPurchasingOpportunities), (opportunity) => {
        _push(`<div class="border border-gray-200 dark:border-gray-700 rounded-lg p-4 hover:shadow-md transition-shadow"><div class="flex items-center justify-between mb-3"><h4 class="font-medium text-gray-900 dark:text-white">${ssrInterpolate(opportunity.item)}</h4><span class="text-xs bg-green-100 dark:bg-green-900 text-green-800 dark:text-green-400 px-2 py-1 rounded-full"> Save ${ssrInterpolate(opportunity.savings)}% </span></div><div class="space-y-2"><div class="flex justify-between text-sm"><span class="text-gray-600 dark:text-gray-400">Current Price:</span><span class="text-gray-900 dark:text-white">R ${ssrInterpolate(formatCurrency(opportunity.currentPrice))}</span></div><div class="flex justify-between text-sm"><span class="text-gray-600 dark:text-gray-400">Group Price:</span><span class="text-green-600 font-medium">R ${ssrInterpolate(formatCurrency(opportunity.groupPrice))}</span></div><div class="flex justify-between text-sm"><span class="text-gray-600 dark:text-gray-400">Participants:</span><span class="text-gray-900 dark:text-white">${ssrInterpolate(opportunity.participants)}</span></div></div><button class="mt-4 w-full bg-blue-600 text-white py-2 rounded-lg hover:bg-blue-700 transition-colors text-sm"> Join Group Purchase </button></div>`);
      });
      _push(`<!--]--></div></div></div></div></div>`);
    };
  }
});
const _sfc_setup = _sfc_main.setup;
_sfc_main.setup = (props, ctx) => {
  const ssrContext = useSSRContext();
  (ssrContext.modules || (ssrContext.modules = /* @__PURE__ */ new Set())).add("pages/inventory/index.vue");
  return _sfc_setup ? _sfc_setup(props, ctx) : void 0;
};
export {
  _sfc_main as default
};
//# sourceMappingURL=index-sJpVzwcx.js.map
