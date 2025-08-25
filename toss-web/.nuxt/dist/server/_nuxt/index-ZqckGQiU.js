import { defineComponent, ref, mergeProps, unref, createVNode, resolveDynamicComponent, useSSRContext } from "vue";
import { ssrRenderAttrs, ssrRenderComponent, ssrInterpolate, ssrRenderList, ssrRenderVNode, ssrRenderClass } from "vue/server-renderer";
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
const DocumentIcon = "svg";
const TrendingUpIcon = "svg";
const TrendingDownIcon = "svg";
const CurrencyDollarIcon = "svg";
const ChartBarIcon = "svg";
const _sfc_main = /* @__PURE__ */ defineComponent({
  __name: "index",
  __ssrInlineRender: true,
  setup(__props) {
    const stats = ref({
      totalAssets: 245e3,
      totalLiabilities: 89e3,
      monthlyRevenue: 45e3,
      netProfit: 12e3
    });
    const accounts = ref([
      { id: 1, name: "Business Checking", type: "Asset", number: "1001", balance: 25e3, change: 5.2 },
      { id: 2, name: "Accounts Receivable", type: "Asset", number: "1200", balance: 15e3, change: -2.1 },
      { id: 3, name: "Inventory", type: "Asset", number: "1300", balance: 85e3, change: 8.3 },
      { id: 4, name: "Accounts Payable", type: "Liability", number: "2001", balance: -12e3, change: 3.1 },
      { id: 5, name: "Sales Revenue", type: "Income", number: "4001", balance: 45e3, change: 12.5 }
    ]);
    const recentTransactions = ref([
      { id: 1, description: "Payment from Customer ABC", type: "income", amount: 2500, date: /* @__PURE__ */ new Date() },
      { id: 2, description: "Office Supplies Purchase", type: "expense", amount: -450, date: new Date(Date.now() - 864e5) },
      { id: 3, description: "Bank Transfer", type: "transfer", amount: 1e3, date: new Date(Date.now() - 1728e5) }
    ]);
    const profitLoss = ref({
      revenue: 45e3,
      cogs: 18e3,
      expenses: 15e3,
      netProfit: 12e3
    });
    const balanceSheet = ref({
      currentAssets: 125e3,
      fixedAssets: 12e4,
      currentLiabilities: 45e3,
      longTermDebt: 44e3,
      equity: 156e3
    });
    ref(false);
    ref(false);
    function formatCurrency(amount) {
      return new Intl.NumberFormat("en-ZA", {
        style: "decimal",
        minimumFractionDigits: 0,
        maximumFractionDigits: 0
      }).format(Math.abs(amount));
    }
    function formatDate(date) {
      return new Intl.DateTimeFormat("en-ZA", {
        month: "short",
        day: "numeric",
        hour: "2-digit",
        minute: "2-digit"
      }).format(date);
    }
    function getAccountIcon(type) {
      switch (type) {
        case "Asset":
          return "svg";
        case "Liability":
          return "svg";
        case "Income":
          return "svg";
        case "Expense":
          return "svg";
        default:
          return "svg";
      }
    }
    function getTransactionIcon(type) {
      switch (type) {
        case "income":
          return "svg";
        case "expense":
          return "svg";
        case "transfer":
          return "svg";
        default:
          return "svg";
      }
    }
    useHead({
      title: "Accounts - TOSS ERP",
      meta: [
        { name: "description", content: "Manage financial transactions and accounting in TOSS ERP" }
      ]
    });
    return (_ctx, _push, _parent, _attrs) => {
      _push(`<div${ssrRenderAttrs(mergeProps({ class: "min-h-screen bg-gray-50 dark:bg-gray-900" }, _attrs))}><div class="bg-white dark:bg-gray-800 shadow-sm border-b border-gray-200 dark:border-gray-700"><div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8"><div class="py-4"><div class="flex items-center justify-between"><div><h1 class="text-2xl font-bold text-gray-900 dark:text-white">Accounts</h1><p class="text-gray-600 dark:text-gray-400">Manage your financial transactions and accounting</p></div><div class="flex space-x-3"><button class="bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700 transition-colors">`);
      _push(ssrRenderComponent(PlusIcon, { class: "w-5 h-5 inline mr-2" }, null, _parent));
      _push(` New Account </button><button class="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors">`);
      _push(ssrRenderComponent(DocumentIcon, { class: "w-5 h-5 inline mr-2" }, null, _parent));
      _push(` Journal Entry </button></div></div></div></div></div><div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6"><div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8"><div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700"><div class="flex items-center justify-between"><div><p class="text-sm text-gray-600 dark:text-gray-400">Total Assets</p><p class="text-2xl font-bold text-green-600">R ${ssrInterpolate(formatCurrency(unref(stats).totalAssets))}</p></div><div class="p-3 bg-green-100 dark:bg-green-900 rounded-full">`);
      _push(ssrRenderComponent(TrendingUpIcon, { class: "w-6 h-6 text-green-600" }, null, _parent));
      _push(`</div></div></div><div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700"><div class="flex items-center justify-between"><div><p class="text-sm text-gray-600 dark:text-gray-400">Total Liabilities</p><p class="text-2xl font-bold text-red-600">R ${ssrInterpolate(formatCurrency(unref(stats).totalLiabilities))}</p></div><div class="p-3 bg-red-100 dark:bg-red-900 rounded-full">`);
      _push(ssrRenderComponent(TrendingDownIcon, { class: "w-6 h-6 text-red-600" }, null, _parent));
      _push(`</div></div></div><div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700"><div class="flex items-center justify-between"><div><p class="text-sm text-gray-600 dark:text-gray-400">Monthly Revenue</p><p class="text-2xl font-bold text-blue-600">R ${ssrInterpolate(formatCurrency(unref(stats).monthlyRevenue))}</p></div><div class="p-3 bg-blue-100 dark:bg-blue-900 rounded-full">`);
      _push(ssrRenderComponent(CurrencyDollarIcon, { class: "w-6 h-6 text-blue-600" }, null, _parent));
      _push(`</div></div></div><div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700"><div class="flex items-center justify-between"><div><p class="text-sm text-gray-600 dark:text-gray-400">Net Profit</p><p class="text-2xl font-bold text-green-600">R ${ssrInterpolate(formatCurrency(unref(stats).netProfit))}</p></div><div class="p-3 bg-green-100 dark:bg-green-900 rounded-full">`);
      _push(ssrRenderComponent(ChartBarIcon, { class: "w-6 h-6 text-green-600" }, null, _parent));
      _push(`</div></div></div></div><div class="grid grid-cols-1 lg:grid-cols-3 gap-6 mb-8"><div class="lg:col-span-2"><div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700"><div class="p-6 border-b border-gray-200 dark:border-gray-700"><h3 class="text-lg font-semibold text-gray-900 dark:text-white">Chart of Accounts</h3></div><div class="p-6"><div class="space-y-4"><!--[-->`);
      ssrRenderList(unref(accounts), (account) => {
        _push(`<div class="flex items-center justify-between p-4 bg-gray-50 dark:bg-gray-700 rounded-lg"><div class="flex items-center space-x-3"><div class="w-10 h-10 bg-blue-100 dark:bg-blue-900 rounded-full flex items-center justify-center">`);
        ssrRenderVNode(_push, createVNode(resolveDynamicComponent(getAccountIcon(account.type)), { class: "w-5 h-5 text-blue-600" }, null), _parent);
        _push(`</div><div><p class="font-medium text-gray-900 dark:text-white">${ssrInterpolate(account.name)}</p><p class="text-sm text-gray-600 dark:text-gray-400">${ssrInterpolate(account.type)} • ${ssrInterpolate(account.number)}</p></div></div><div class="text-right"><p class="font-semibold text-gray-900 dark:text-white">R ${ssrInterpolate(formatCurrency(account.balance))}</p><p class="${ssrRenderClass([account.balance >= 0 ? "text-green-600" : "text-red-600", "text-sm"])}">${ssrInterpolate(account.balance >= 0 ? "+" : "")}${ssrInterpolate(account.change)}% </p></div></div>`);
      });
      _push(`<!--]--></div></div></div></div><div><div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700"><div class="p-6 border-b border-gray-200 dark:border-gray-700"><h3 class="text-lg font-semibold text-gray-900 dark:text-white">Recent Transactions</h3></div><div class="p-6"><div class="space-y-4"><!--[-->`);
      ssrRenderList(unref(recentTransactions), (transaction) => {
        _push(`<div class="flex items-center justify-between"><div class="flex items-center space-x-3"><div class="w-8 h-8 bg-gray-100 dark:bg-gray-700 rounded-full flex items-center justify-center">`);
        ssrRenderVNode(_push, createVNode(resolveDynamicComponent(getTransactionIcon(transaction.type)), { class: "w-4 h-4 text-gray-600" }, null), _parent);
        _push(`</div><div><p class="text-sm font-medium text-gray-900 dark:text-white">${ssrInterpolate(transaction.description)}</p><p class="text-xs text-gray-600 dark:text-gray-400">${ssrInterpolate(formatDate(transaction.date))}</p></div></div><div class="text-right"><p class="${ssrRenderClass([transaction.amount >= 0 ? "text-green-600" : "text-red-600", "text-sm font-semibold"])}">${ssrInterpolate(transaction.amount >= 0 ? "+" : "-")}R ${ssrInterpolate(formatCurrency(Math.abs(transaction.amount)))}</p></div></div>`);
      });
      _push(`<!--]--></div></div></div></div></div><div class="grid grid-cols-1 lg:grid-cols-2 gap-6"><div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700"><div class="p-6 border-b border-gray-200 dark:border-gray-700"><div class="flex items-center justify-between"><h3 class="text-lg font-semibold text-gray-900 dark:text-white">Profit &amp; Loss</h3><select class="text-sm border border-gray-300 dark:border-gray-600 rounded-lg px-3 py-1 bg-white dark:bg-gray-700 text-gray-900 dark:text-white"><option>This Month</option><option>Last Month</option><option>This Quarter</option><option>This Year</option></select></div></div><div class="p-6"><div class="space-y-4"><div class="flex justify-between"><span class="text-gray-600 dark:text-gray-400">Revenue</span><span class="font-semibold text-green-600">R ${ssrInterpolate(formatCurrency(unref(profitLoss).revenue))}</span></div><div class="flex justify-between"><span class="text-gray-600 dark:text-gray-400">Cost of Goods Sold</span><span class="font-semibold text-red-600">R ${ssrInterpolate(formatCurrency(unref(profitLoss).cogs))}</span></div><div class="flex justify-between"><span class="text-gray-600 dark:text-gray-400">Operating Expenses</span><span class="font-semibold text-red-600">R ${ssrInterpolate(formatCurrency(unref(profitLoss).expenses))}</span></div><hr class="border-gray-200 dark:border-gray-700"><div class="flex justify-between"><span class="font-semibold text-gray-900 dark:text-white">Net Profit</span><span class="font-bold text-green-600">R ${ssrInterpolate(formatCurrency(unref(profitLoss).netProfit))}</span></div></div></div></div><div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700"><div class="p-6 border-b border-gray-200 dark:border-gray-700"><h3 class="text-lg font-semibold text-gray-900 dark:text-white">Balance Sheet Summary</h3></div><div class="p-6"><div class="space-y-4"><div><h4 class="font-medium text-gray-900 dark:text-white mb-2">Assets</h4><div class="space-y-2 ml-4"><div class="flex justify-between"><span class="text-gray-600 dark:text-gray-400">Current Assets</span><span class="font-semibold text-gray-900 dark:text-white">R ${ssrInterpolate(formatCurrency(unref(balanceSheet).currentAssets))}</span></div><div class="flex justify-between"><span class="text-gray-600 dark:text-gray-400">Fixed Assets</span><span class="font-semibold text-gray-900 dark:text-white">R ${ssrInterpolate(formatCurrency(unref(balanceSheet).fixedAssets))}</span></div></div></div><div><h4 class="font-medium text-gray-900 dark:text-white mb-2">Liabilities</h4><div class="space-y-2 ml-4"><div class="flex justify-between"><span class="text-gray-600 dark:text-gray-400">Current Liabilities</span><span class="font-semibold text-gray-900 dark:text-white">R ${ssrInterpolate(formatCurrency(unref(balanceSheet).currentLiabilities))}</span></div><div class="flex justify-between"><span class="text-gray-600 dark:text-gray-400">Long-term Debt</span><span class="font-semibold text-gray-900 dark:text-white">R ${ssrInterpolate(formatCurrency(unref(balanceSheet).longTermDebt))}</span></div></div></div><hr class="border-gray-200 dark:border-gray-700"><div class="flex justify-between"><span class="font-semibold text-gray-900 dark:text-white">Equity</span><span class="font-bold text-blue-600">R ${ssrInterpolate(formatCurrency(unref(balanceSheet).equity))}</span></div></div></div></div></div></div></div>`);
    };
  }
});
const _sfc_setup = _sfc_main.setup;
_sfc_main.setup = (props, ctx) => {
  const ssrContext = useSSRContext();
  (ssrContext.modules || (ssrContext.modules = /* @__PURE__ */ new Set())).add("pages/accounts/index.vue");
  return _sfc_setup ? _sfc_setup(props, ctx) : void 0;
};
export {
  _sfc_main as default
};
//# sourceMappingURL=index-ZqckGQiU.js.map
