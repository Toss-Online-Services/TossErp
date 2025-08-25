import { _ as __nuxt_component_0$1 } from './nuxt-link-CzepOQdf.mjs';
import { defineComponent, ref, mergeProps, withCtx, createVNode, createTextVNode, unref, useSSRContext } from 'vue';
import { ssrRenderAttrs, ssrRenderComponent, ssrRenderStyle } from 'vue/server-renderer';
import { _ as _export_sfc } from './server.mjs';
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
  __name: "AppNavigation",
  __ssrInlineRender: true,
  setup(__props) {
    const showModules = ref(false);
    const showUserMenu = ref(false);
    const showMobileMenu = ref(false);
    const isDark = ref(false);
    return (_ctx, _push, _parent, _attrs) => {
      const _component_NuxtLink = __nuxt_component_0$1;
      _push(`<nav${ssrRenderAttrs(mergeProps({ class: "bg-white dark:bg-gray-800 shadow-sm border-b border-gray-200 dark:border-gray-700" }, _attrs))} data-v-1dce69f1><div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8" data-v-1dce69f1><div class="flex justify-between h-16" data-v-1dce69f1><div class="flex items-center" data-v-1dce69f1>`);
      _push(ssrRenderComponent(_component_NuxtLink, {
        to: "/",
        class: "flex items-center space-x-3"
      }, {
        default: withCtx((_, _push2, _parent2, _scopeId) => {
          if (_push2) {
            _push2(`<div class="w-8 h-8 bg-gradient-to-r from-blue-600 to-blue-800 rounded-lg flex items-center justify-center" data-v-1dce69f1${_scopeId}><span class="text-white font-bold text-sm" data-v-1dce69f1${_scopeId}>T</span></div><span class="text-xl font-bold text-gray-900 dark:text-white" data-v-1dce69f1${_scopeId}>TOSS ERP III</span>`);
          } else {
            return [
              createVNode("div", { class: "w-8 h-8 bg-gradient-to-r from-blue-600 to-blue-800 rounded-lg flex items-center justify-center" }, [
                createVNode("span", { class: "text-white font-bold text-sm" }, "T")
              ]),
              createVNode("span", { class: "text-xl font-bold text-gray-900 dark:text-white" }, "TOSS ERP III")
            ];
          }
        }),
        _: 1
      }, _parent));
      _push(`</div><div class="hidden md:flex items-center space-x-8" data-v-1dce69f1>`);
      _push(ssrRenderComponent(_component_NuxtLink, {
        to: "/dashboard",
        class: "nav-link"
      }, {
        default: withCtx((_, _push2, _parent2, _scopeId) => {
          if (_push2) {
            _push2(`Dashboard`);
          } else {
            return [
              createTextVNode("Dashboard")
            ];
          }
        }),
        _: 1
      }, _parent));
      _push(`<div class="relative" data-v-1dce69f1><button class="nav-link flex items-center" data-v-1dce69f1> Modules <svg class="ml-1 w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24" data-v-1dce69f1><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" data-v-1dce69f1></path></svg></button><div style="${ssrRenderStyle(unref(showModules) ? null : { display: "none" })}" class="absolute top-full left-0 mt-1 w-64 bg-white dark:bg-gray-800 rounded-md shadow-lg border border-gray-200 dark:border-gray-700 z-50" data-v-1dce69f1><div class="py-2" data-v-1dce69f1><h3 class="px-4 py-2 text-xs font-semibold text-gray-500 dark:text-gray-400 uppercase tracking-wider" data-v-1dce69f1>Core Modules</h3>`);
      _push(ssrRenderComponent(_component_NuxtLink, {
        to: "/inventory",
        class: "dropdown-link"
      }, {
        default: withCtx((_, _push2, _parent2, _scopeId) => {
          if (_push2) {
            _push2(`\u{1F4E6} Inventory`);
          } else {
            return [
              createTextVNode("\u{1F4E6} Inventory")
            ];
          }
        }),
        _: 1
      }, _parent));
      _push(ssrRenderComponent(_component_NuxtLink, {
        to: "/sales",
        class: "dropdown-link"
      }, {
        default: withCtx((_, _push2, _parent2, _scopeId) => {
          if (_push2) {
            _push2(`\u{1F4B0} Sales &amp; CRM`);
          } else {
            return [
              createTextVNode("\u{1F4B0} Sales & CRM")
            ];
          }
        }),
        _: 1
      }, _parent));
      _push(ssrRenderComponent(_component_NuxtLink, {
        to: "/pos",
        class: "dropdown-link"
      }, {
        default: withCtx((_, _push2, _parent2, _scopeId) => {
          if (_push2) {
            _push2(`\u{1F6D2} Point of Sale`);
          } else {
            return [
              createTextVNode("\u{1F6D2} Point of Sale")
            ];
          }
        }),
        _: 1
      }, _parent));
      _push(ssrRenderComponent(_component_NuxtLink, {
        to: "/purchasing",
        class: "dropdown-link"
      }, {
        default: withCtx((_, _push2, _parent2, _scopeId) => {
          if (_push2) {
            _push2(`\u{1F6CD}\uFE0F Purchasing`);
          } else {
            return [
              createTextVNode("\u{1F6CD}\uFE0F Purchasing")
            ];
          }
        }),
        _: 1
      }, _parent));
      _push(ssrRenderComponent(_component_NuxtLink, {
        to: "/accounts",
        class: "dropdown-link"
      }, {
        default: withCtx((_, _push2, _parent2, _scopeId) => {
          if (_push2) {
            _push2(`\u{1F4CA} Accounts`);
          } else {
            return [
              createTextVNode("\u{1F4CA} Accounts")
            ];
          }
        }),
        _: 1
      }, _parent));
      _push(`<div class="border-t border-gray-200 dark:border-gray-700 my-2" data-v-1dce69f1></div><h3 class="px-4 py-2 text-xs font-semibold text-gray-500 dark:text-gray-400 uppercase tracking-wider" data-v-1dce69f1>Collaboration</h3>`);
      _push(ssrRenderComponent(_component_NuxtLink, {
        to: "/group-buying",
        class: "dropdown-link"
      }, {
        default: withCtx((_, _push2, _parent2, _scopeId) => {
          if (_push2) {
            _push2(`\u{1F91D} Group Purchasing`);
          } else {
            return [
              createTextVNode("\u{1F91D} Group Purchasing")
            ];
          }
        }),
        _: 1
      }, _parent));
      _push(ssrRenderComponent(_component_NuxtLink, {
        to: "/logistics",
        class: "dropdown-link"
      }, {
        default: withCtx((_, _push2, _parent2, _scopeId) => {
          if (_push2) {
            _push2(`\u{1F69A} Shared Logistics`);
          } else {
            return [
              createTextVNode("\u{1F69A} Shared Logistics")
            ];
          }
        }),
        _: 1
      }, _parent));
      _push(ssrRenderComponent(_component_NuxtLink, {
        to: "/tools",
        class: "dropdown-link"
      }, {
        default: withCtx((_, _push2, _parent2, _scopeId) => {
          if (_push2) {
            _push2(`\u{1F527} Tool Sharing`);
          } else {
            return [
              createTextVNode("\u{1F527} Tool Sharing")
            ];
          }
        }),
        _: 1
      }, _parent));
      _push(ssrRenderComponent(_component_NuxtLink, {
        to: "/credit",
        class: "dropdown-link"
      }, {
        default: withCtx((_, _push2, _parent2, _scopeId) => {
          if (_push2) {
            _push2(`\u{1F3E6} Pooled Credit`);
          } else {
            return [
              createTextVNode("\u{1F3E6} Pooled Credit")
            ];
          }
        }),
        _: 1
      }, _parent));
      _push(`</div></div></div>`);
      _push(ssrRenderComponent(_component_NuxtLink, {
        to: "/collaboration",
        class: "nav-link"
      }, {
        default: withCtx((_, _push2, _parent2, _scopeId) => {
          if (_push2) {
            _push2(`Collaboration`);
          } else {
            return [
              createTextVNode("Collaboration")
            ];
          }
        }),
        _: 1
      }, _parent));
      _push(ssrRenderComponent(_component_NuxtLink, {
        to: "/reports",
        class: "nav-link"
      }, {
        default: withCtx((_, _push2, _parent2, _scopeId) => {
          if (_push2) {
            _push2(`Reports`);
          } else {
            return [
              createTextVNode("Reports")
            ];
          }
        }),
        _: 1
      }, _parent));
      _push(`</div><div class="flex items-center space-x-4" data-v-1dce69f1><button class="p-2 text-gray-500 hover:text-blue-600 dark:text-gray-400 dark:hover:text-blue-400 transition-colors" data-v-1dce69f1><svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24" data-v-1dce69f1><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.663 17h4.673M12 3v1m6.364 1.636l-.707.707M21 12h-1M4 12H3m3.343-5.657l-.707-.707m2.828 9.9a5 5 0 117.072 0l-.548.547A3.374 3.374 0 0014 18.469V19a2 2 0 11-4 0v-.531c0-.895-.356-1.754-.988-2.386l-.548-.547z" data-v-1dce69f1></path></svg></button><button class="relative p-2 text-gray-500 hover:text-blue-600 dark:text-gray-400 dark:hover:text-blue-400 transition-colors" data-v-1dce69f1><svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24" data-v-1dce69f1><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9" data-v-1dce69f1></path></svg><span class="absolute top-0 right-0 block h-2 w-2 rounded-full ring-2 ring-white bg-red-400" data-v-1dce69f1></span></button><button class="p-2 text-gray-500 hover:text-blue-600 dark:text-gray-400 dark:hover:text-blue-400 transition-colors" data-v-1dce69f1>`);
      if (unref(isDark)) {
        _push(`<svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24" data-v-1dce69f1><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 3v1m0 16v1m9-9h-1M4 12H3m15.364 6.364l-.707-.707M6.343 6.343l-.707-.707m12.728 0l-.707.707M6.343 17.657l-.707.707M16 12a4 4 0 11-8 0 4 4 0 018 0z" data-v-1dce69f1></path></svg>`);
      } else {
        _push(`<svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24" data-v-1dce69f1><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20.354 15.354A9 9 0 018.646 3.646 9.003 9.003 0 0012 21a9.003 9.003 0 008.354-5.646z" data-v-1dce69f1></path></svg>`);
      }
      _push(`</button><div class="relative" data-v-1dce69f1><button class="flex items-center space-x-3 p-2 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-700 transition-colors" data-v-1dce69f1><div class="w-8 h-8 bg-blue-600 rounded-full flex items-center justify-center" data-v-1dce69f1><span class="text-white text-sm font-medium" data-v-1dce69f1>TU</span></div><span class="hidden lg:block text-sm font-medium text-gray-900 dark:text-white" data-v-1dce69f1>Test User</span></button><div style="${ssrRenderStyle(unref(showUserMenu) ? null : { display: "none" })}" class="absolute top-full right-0 mt-1 w-48 bg-white dark:bg-gray-800 rounded-md shadow-lg border border-gray-200 dark:border-gray-700 z-50" data-v-1dce69f1><div class="py-1" data-v-1dce69f1>`);
      _push(ssrRenderComponent(_component_NuxtLink, {
        to: "/profile",
        class: "dropdown-link"
      }, {
        default: withCtx((_, _push2, _parent2, _scopeId) => {
          if (_push2) {
            _push2(`\u{1F464} Profile`);
          } else {
            return [
              createTextVNode("\u{1F464} Profile")
            ];
          }
        }),
        _: 1
      }, _parent));
      _push(ssrRenderComponent(_component_NuxtLink, {
        to: "/settings",
        class: "dropdown-link"
      }, {
        default: withCtx((_, _push2, _parent2, _scopeId) => {
          if (_push2) {
            _push2(`\u2699\uFE0F Settings`);
          } else {
            return [
              createTextVNode("\u2699\uFE0F Settings")
            ];
          }
        }),
        _: 1
      }, _parent));
      _push(ssrRenderComponent(_component_NuxtLink, {
        to: "/help",
        class: "dropdown-link"
      }, {
        default: withCtx((_, _push2, _parent2, _scopeId) => {
          if (_push2) {
            _push2(`\u2753 Help`);
          } else {
            return [
              createTextVNode("\u2753 Help")
            ];
          }
        }),
        _: 1
      }, _parent));
      _push(`<div class="border-t border-gray-200 dark:border-gray-700 my-1" data-v-1dce69f1></div><button class="dropdown-link w-full text-left text-red-600 dark:text-red-400" data-v-1dce69f1>\u{1F6AA} Logout</button></div></div></div><button class="md:hidden p-2 text-gray-500 hover:text-blue-600 dark:text-gray-400 dark:hover:text-blue-400" data-v-1dce69f1><svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24" data-v-1dce69f1><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16" data-v-1dce69f1></path></svg></button></div></div></div><div style="${ssrRenderStyle(unref(showMobileMenu) ? null : { display: "none" })}" class="md:hidden bg-white dark:bg-gray-800 border-t border-gray-200 dark:border-gray-700" data-v-1dce69f1><div class="px-4 pt-2 pb-3 space-y-1" data-v-1dce69f1>`);
      _push(ssrRenderComponent(_component_NuxtLink, {
        to: "/dashboard",
        class: "mobile-nav-link"
      }, {
        default: withCtx((_, _push2, _parent2, _scopeId) => {
          if (_push2) {
            _push2(`Dashboard`);
          } else {
            return [
              createTextVNode("Dashboard")
            ];
          }
        }),
        _: 1
      }, _parent));
      _push(ssrRenderComponent(_component_NuxtLink, {
        to: "/inventory",
        class: "mobile-nav-link"
      }, {
        default: withCtx((_, _push2, _parent2, _scopeId) => {
          if (_push2) {
            _push2(`\u{1F4E6} Inventory`);
          } else {
            return [
              createTextVNode("\u{1F4E6} Inventory")
            ];
          }
        }),
        _: 1
      }, _parent));
      _push(ssrRenderComponent(_component_NuxtLink, {
        to: "/sales",
        class: "mobile-nav-link"
      }, {
        default: withCtx((_, _push2, _parent2, _scopeId) => {
          if (_push2) {
            _push2(`\u{1F4B0} Sales`);
          } else {
            return [
              createTextVNode("\u{1F4B0} Sales")
            ];
          }
        }),
        _: 1
      }, _parent));
      _push(ssrRenderComponent(_component_NuxtLink, {
        to: "/pos",
        class: "mobile-nav-link"
      }, {
        default: withCtx((_, _push2, _parent2, _scopeId) => {
          if (_push2) {
            _push2(`\u{1F6D2} POS`);
          } else {
            return [
              createTextVNode("\u{1F6D2} POS")
            ];
          }
        }),
        _: 1
      }, _parent));
      _push(ssrRenderComponent(_component_NuxtLink, {
        to: "/group-buying",
        class: "mobile-nav-link"
      }, {
        default: withCtx((_, _push2, _parent2, _scopeId) => {
          if (_push2) {
            _push2(`\u{1F91D} Group Buying`);
          } else {
            return [
              createTextVNode("\u{1F91D} Group Buying")
            ];
          }
        }),
        _: 1
      }, _parent));
      _push(ssrRenderComponent(_component_NuxtLink, {
        to: "/reports",
        class: "mobile-nav-link"
      }, {
        default: withCtx((_, _push2, _parent2, _scopeId) => {
          if (_push2) {
            _push2(`Reports`);
          } else {
            return [
              createTextVNode("Reports")
            ];
          }
        }),
        _: 1
      }, _parent));
      _push(`</div></div></nav>`);
    };
  }
});
const _sfc_setup = _sfc_main.setup;
_sfc_main.setup = (props, ctx) => {
  const ssrContext = useSSRContext();
  (ssrContext.modules || (ssrContext.modules = /* @__PURE__ */ new Set())).add("components/AppNavigation.vue");
  return _sfc_setup ? _sfc_setup(props, ctx) : void 0;
};
const __nuxt_component_0 = /* @__PURE__ */ _export_sfc(_sfc_main, [["__scopeId", "data-v-1dce69f1"]]);

export { __nuxt_component_0 as default };
//# sourceMappingURL=AppNavigation-8oXhv3oB.mjs.map
