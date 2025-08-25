import { _ as __nuxt_component_0 } from './nuxt-link-CzepOQdf.mjs';
import _sfc_main$1 from './ModuleCard-aDOCvYGa.mjs';
import { defineComponent, mergeProps, withCtx, createTextVNode, useSSRContext } from 'vue';
import { ssrRenderAttrs, ssrRenderComponent } from 'vue/server-renderer';
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
    useHead({
      title: "TOSS ERP III - Township One-Stop Solution",
      meta: [
        { name: "description", content: "AI-powered collaborative business platform for South African SMMEs. Manage inventory, sales, purchasing, and collaborate with other businesses." },
        { property: "og:title", content: "TOSS ERP III - Township One-Stop Solution" },
        { property: "og:description", content: "AI-powered collaborative business platform for South African SMMEs" },
        { property: "og:type", content: "website" }
      ]
    });
    return (_ctx, _push, _parent, _attrs) => {
      const _component_NuxtLink = __nuxt_component_0;
      const _component_ModuleCard = _sfc_main$1;
      _push(`<div${ssrRenderAttrs(mergeProps({ class: "space-y-8" }, _attrs))}><div class="bg-gradient-to-r from-blue-600 to-blue-800 rounded-lg p-8 text-white"><div class="max-w-4xl"><h1 class="text-4xl font-bold mb-4"> Welcome to TOSS ERP III </h1><p class="text-xl mb-6 text-blue-100"> AI-powered collaborative business platform for South African SMMEs </p><div class="flex flex-wrap gap-4">`);
      _push(ssrRenderComponent(_component_NuxtLink, {
        to: "/dashboard",
        class: "bg-white text-blue-600 px-6 py-3 rounded-lg font-semibold hover:bg-blue-50 transition-colors"
      }, {
        default: withCtx((_, _push2, _parent2, _scopeId) => {
          if (_push2) {
            _push2(` Go to Dashboard `);
          } else {
            return [
              createTextVNode(" Go to Dashboard ")
            ];
          }
        }),
        _: 1
      }, _parent));
      _push(ssrRenderComponent(_component_NuxtLink, {
        to: "/getting-started",
        class: "border border-white text-white px-6 py-3 rounded-lg font-semibold hover:bg-white hover:text-blue-600 transition-colors"
      }, {
        default: withCtx((_, _push2, _parent2, _scopeId) => {
          if (_push2) {
            _push2(` Getting Started `);
          } else {
            return [
              createTextVNode(" Getting Started ")
            ];
          }
        }),
        _: 1
      }, _parent));
      _push(`</div></div></div><div class="bg-white dark:bg-gray-800 rounded-lg p-6 shadow-sm border border-gray-200 dark:border-gray-700"><div class="flex items-center mb-4"><div class="w-12 h-12 bg-green-100 dark:bg-green-900 rounded-lg flex items-center justify-center mr-4"><svg class="w-6 h-6 text-green-600 dark:text-green-400" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.663 17h4.673M12 3v1m6.364 1.636l-.707.707M21 12h-1M4 12H3m3.343-5.657l-.707-.707m2.828 9.9a5 5 0 117.072 0l-.548.547A3.374 3.374 0 0014 18.469V19a2 2 0 11-4 0v-.531c0-.895-.356-1.754-.988-2.386l-.548-.547z"></path></svg></div><div><h2 class="text-xl font-semibold text-gray-900 dark:text-white">AI Business Co-Pilot</h2><p class="text-gray-600 dark:text-gray-400">Your intelligent business assistant</p></div></div><p class="text-gray-700 dark:text-gray-300 mb-4"> Chat with your AI assistant to manage inventory, check sales, coordinate with suppliers, and automate routine tasks. Available in English, Afrikaans, Zulu, Xhosa, and more. </p><button class="bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700 transition-colors"> Start Conversation </button></div><div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6"><div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700"><div class="flex items-center"><div class="p-2 bg-blue-100 dark:bg-blue-900 rounded-lg"><svg class="w-6 h-6 text-blue-600 dark:text-blue-400" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4"></path></svg></div><div class="ml-4"><p class="text-sm font-medium text-gray-600 dark:text-gray-400">Inventory Items</p><p class="text-2xl font-semibold text-gray-900 dark:text-white">1,247</p></div></div></div><div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700"><div class="flex items-center"><div class="p-2 bg-green-100 dark:bg-green-900 rounded-lg"><svg class="w-6 h-6 text-green-600 dark:text-green-400" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1"></path></svg></div><div class="ml-4"><p class="text-sm font-medium text-gray-600 dark:text-gray-400">Today&#39;s Sales</p><p class="text-2xl font-semibold text-gray-900 dark:text-white">R 8,450</p></div></div></div><div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700"><div class="flex items-center"><div class="p-2 bg-yellow-100 dark:bg-yellow-900 rounded-lg"><svg class="w-6 h-6 text-yellow-600 dark:text-yellow-400" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z"></path></svg></div><div class="ml-4"><p class="text-sm font-medium text-gray-600 dark:text-gray-400">Active Customers</p><p class="text-2xl font-semibold text-gray-900 dark:text-white">342</p></div></div></div><div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700"><div class="flex items-center"><div class="p-2 bg-purple-100 dark:bg-purple-900 rounded-lg"><svg class="w-6 h-6 text-purple-600 dark:text-purple-400" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 11H5m14 0a2 2 0 012 2v6a2 2 0 01-2 2H5a2 2 0 01-2-2v-6a2 2 0 012-2m14 0V9a2 2 0 00-2-2M5 11V9a2 2 0 012-2m0 0V5a2 2 0 012-2h6a2 2 0 012 2v2M7 7h10"></path></svg></div><div class="ml-4"><p class="text-sm font-medium text-gray-600 dark:text-gray-400">Group Orders</p><p class="text-2xl font-semibold text-gray-900 dark:text-white">12</p></div></div></div></div><div><h2 class="text-2xl font-bold text-gray-900 dark:text-white mb-6">Business Modules</h2><div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">`);
      _push(ssrRenderComponent(_component_ModuleCard, {
        title: "Inventory Management",
        description: "Track stock, manage suppliers, automate reordering",
        icon: "\u{1F4E6}",
        link: "/inventory",
        color: "blue"
      }, null, _parent));
      _push(ssrRenderComponent(_component_ModuleCard, {
        title: "Sales & CRM",
        description: "Manage customers, orders, and sales tracking",
        icon: "\u{1F4B0}",
        link: "/sales",
        color: "green"
      }, null, _parent));
      _push(ssrRenderComponent(_component_ModuleCard, {
        title: "Point of Sale",
        description: "Fast checkout, receipt printing, inventory sync",
        icon: "\u{1F6D2}",
        link: "/pos",
        color: "purple"
      }, null, _parent));
      _push(ssrRenderComponent(_component_ModuleCard, {
        title: "Purchasing",
        description: "Group buying, supplier management, cost optimization",
        icon: "\u{1F6CD}\uFE0F",
        link: "/purchasing",
        color: "orange"
      }, null, _parent));
      _push(ssrRenderComponent(_component_ModuleCard, {
        title: "Group Purchasing",
        description: "Bulk buying with other businesses for better prices",
        icon: "\u{1F91D}",
        link: "/group-buying",
        color: "teal"
      }, null, _parent));
      _push(ssrRenderComponent(_component_ModuleCard, {
        title: "Shared Logistics",
        description: "Collaborative delivery and warehousing",
        icon: "\u{1F69A}",
        link: "/logistics",
        color: "indigo"
      }, null, _parent));
      _push(ssrRenderComponent(_component_ModuleCard, {
        title: "Tool Sharing",
        description: "Rent and share equipment with network members",
        icon: "\u{1F527}",
        link: "/tools",
        color: "gray"
      }, null, _parent));
      _push(ssrRenderComponent(_component_ModuleCard, {
        title: "Pooled Credit",
        description: "Community savings and micro-loans",
        icon: "\u{1F3E6}",
        link: "/credit",
        color: "emerald"
      }, null, _parent));
      _push(ssrRenderComponent(_component_ModuleCard, {
        title: "Accounts & Finance",
        description: "Bookkeeping, cash flow, financial reports",
        icon: "\u{1F4CA}",
        link: "/accounts",
        color: "blue"
      }, null, _parent));
      _push(ssrRenderComponent(_component_ModuleCard, {
        title: "Manufacturing",
        description: "Production planning and quality management",
        icon: "\u{1F3ED}",
        link: "/manufacturing",
        color: "red"
      }, null, _parent));
      _push(ssrRenderComponent(_component_ModuleCard, {
        title: "Projects",
        description: "Project management and resource allocation",
        icon: "\u{1F4CB}",
        link: "/projects",
        color: "yellow"
      }, null, _parent));
      _push(ssrRenderComponent(_component_ModuleCard, {
        title: "Communication",
        description: "Team chat, notifications, and announcements",
        icon: "\u{1F4AC}",
        link: "/communication",
        color: "pink"
      }, null, _parent));
      _push(`</div></div><div class="bg-white dark:bg-gray-800 rounded-lg p-6 shadow-sm border border-gray-200 dark:border-gray-700"><h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">Recent Activity</h3><div class="space-y-4"><div class="flex items-center space-x-3"><div class="w-2 h-2 bg-green-500 rounded-full"></div><p class="text-sm text-gray-700 dark:text-gray-300"><span class="font-medium">Group purchase</span> completed - 5 businesses saved R2,340 on bulk flour order </p><span class="text-xs text-gray-500">2 hours ago</span></div><div class="flex items-center space-x-3"><div class="w-2 h-2 bg-blue-500 rounded-full"></div><p class="text-sm text-gray-700 dark:text-gray-300"><span class="font-medium">Inventory alert</span> - AI reordered 50kg maize meal automatically </p><span class="text-xs text-gray-500">4 hours ago</span></div><div class="flex items-center space-x-3"><div class="w-2 h-2 bg-purple-500 rounded-full"></div><p class="text-sm text-gray-700 dark:text-gray-300"><span class="font-medium">Tool sharing</span> - Drill borrowed by Thabo&#39;s Hardware for weekend project </p><span class="text-xs text-gray-500">1 day ago</span></div></div></div></div>`);
    };
  }
});
const _sfc_setup = _sfc_main.setup;
_sfc_main.setup = (props, ctx) => {
  const ssrContext = useSSRContext();
  (ssrContext.modules || (ssrContext.modules = /* @__PURE__ */ new Set())).add("pages/index.vue");
  return _sfc_setup ? _sfc_setup(props, ctx) : void 0;
};

export { _sfc_main as default };
//# sourceMappingURL=index-oKc0deLM.mjs.map
