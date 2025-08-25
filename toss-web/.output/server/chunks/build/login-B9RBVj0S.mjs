import { _ as __nuxt_component_0 } from './nuxt-link-CzepOQdf.mjs';
import { defineComponent, ref, mergeProps, unref, withCtx, createTextVNode, useSSRContext } from 'vue';
import { ssrRenderAttrs, ssrRenderAttr, ssrIncludeBooleanAttr, ssrLooseContain, ssrRenderComponent, ssrInterpolate } from 'vue/server-renderer';
import { _ as _export_sfc, u as useHead } from './server.mjs';
import { u as useUserStore } from './user-B2SCl6g9.mjs';
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
  __name: "login",
  __ssrInlineRender: true,
  setup(__props) {
    useHead({
      title: "Sign In"
    });
    useUserStore();
    const loading = ref(false);
    const form = ref({
      email: "",
      password: "",
      rememberMe: false
    });
    return (_ctx, _push, _parent, _attrs) => {
      const _component_NuxtLink = __nuxt_component_0;
      _push(`<div${ssrRenderAttrs(mergeProps({ class: "min-h-screen flex items-center justify-center bg-gray-50 dark:bg-gray-900 py-12 px-4 sm:px-6 lg:px-8" }, _attrs))} data-v-dc7e54e5><div class="max-w-md w-full space-y-8" data-v-dc7e54e5><div data-v-dc7e54e5><div class="mx-auto h-12 w-12 flex items-center justify-center rounded-full bg-gradient-to-r from-green-400 to-green-600" data-v-dc7e54e5><svg class="h-8 w-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24" data-v-dc7e54e5><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 10V3L4 14h7v7l9-11h-7z" data-v-dc7e54e5></path></svg></div><h2 class="mt-6 text-center text-3xl font-extrabold text-gray-900 dark:text-white" data-v-dc7e54e5> Sign in to TOSS ERP </h2><p class="mt-2 text-center text-sm text-gray-600 dark:text-gray-400" data-v-dc7e54e5> Your collaborative business platform </p></div><form class="mt-8 space-y-6" data-v-dc7e54e5><div class="rounded-md shadow-sm -space-y-px" data-v-dc7e54e5><div data-v-dc7e54e5><label for="email" class="sr-only" data-v-dc7e54e5>Email address</label><input id="email"${ssrRenderAttr("value", unref(form).email)} name="email" type="email" autocomplete="email" required class="appearance-none rounded-none relative block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 placeholder-gray-500 dark:placeholder-gray-400 text-gray-900 dark:text-white rounded-t-md focus:outline-none focus:ring-green-500 focus:border-green-500 focus:z-10 sm:text-sm bg-white dark:bg-gray-800" placeholder="Email address" data-v-dc7e54e5></div><div data-v-dc7e54e5><label for="password" class="sr-only" data-v-dc7e54e5>Password</label><input id="password"${ssrRenderAttr("value", unref(form).password)} name="password" type="password" autocomplete="current-password" required class="appearance-none rounded-none relative block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 placeholder-gray-500 dark:placeholder-gray-400 text-gray-900 dark:text-white rounded-b-md focus:outline-none focus:ring-green-500 focus:border-green-500 focus:z-10 sm:text-sm bg-white dark:bg-gray-800" placeholder="Password" data-v-dc7e54e5></div></div><div class="flex items-center justify-between" data-v-dc7e54e5><div class="flex items-center" data-v-dc7e54e5><input id="remember-me"${ssrIncludeBooleanAttr(Array.isArray(unref(form).rememberMe) ? ssrLooseContain(unref(form).rememberMe, null) : unref(form).rememberMe) ? " checked" : ""} name="remember-me" type="checkbox" class="h-4 w-4 text-green-600 focus:ring-green-500 border-gray-300 dark:border-gray-600 rounded bg-white dark:bg-gray-800" data-v-dc7e54e5><label for="remember-me" class="ml-2 block text-sm text-gray-900 dark:text-white" data-v-dc7e54e5> Remember me </label></div><div class="text-sm" data-v-dc7e54e5>`);
      _push(ssrRenderComponent(_component_NuxtLink, {
        to: "/forgot-password",
        class: "font-medium text-green-600 hover:text-green-500 dark:text-green-400 dark:hover:text-green-300"
      }, {
        default: withCtx((_, _push2, _parent2, _scopeId) => {
          if (_push2) {
            _push2(` Forgot your password? `);
          } else {
            return [
              createTextVNode(" Forgot your password? ")
            ];
          }
        }),
        _: 1
      }, _parent));
      _push(`</div></div><div data-v-dc7e54e5><button type="submit"${ssrIncludeBooleanAttr(unref(loading)) ? " disabled" : ""} class="group relative w-full flex justify-center py-2 px-4 border border-transparent text-sm font-medium rounded-md text-white bg-green-600 hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-500 disabled:opacity-50 disabled:cursor-not-allowed" data-v-dc7e54e5><span class="absolute left-0 inset-y-0 flex items-center pl-3" data-v-dc7e54e5>`);
      if (!unref(loading)) {
        _push(`<svg class="h-5 w-5 text-green-500 group-hover:text-green-400" fill="currentColor" viewBox="0 0 20 20" data-v-dc7e54e5><path fill-rule="evenodd" d="M5 9V7a5 5 0 0110 0v2a2 2 0 012 2v5a2 2 0 01-2 2H5a2 2 0 01-2-2v-5a2 2 0 012-2zm8-2v2H7V7a3 3 0 016 0z" clip-rule="evenodd" data-v-dc7e54e5></path></svg>`);
      } else {
        _push(`<svg class="animate-spin h-5 w-5 text-green-500" fill="none" viewBox="0 0 24 24" data-v-dc7e54e5><circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4" data-v-dc7e54e5></circle><path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z" data-v-dc7e54e5></path></svg>`);
      }
      _push(`</span> ${ssrInterpolate(unref(loading) ? "Signing in..." : "Sign in")}</button></div><div class="mt-6 p-4 bg-blue-50 dark:bg-blue-900/20 rounded-lg" data-v-dc7e54e5><h3 class="text-sm font-medium text-blue-800 dark:text-blue-200 mb-2" data-v-dc7e54e5>Demo Credentials</h3><div class="space-y-1 text-xs text-blue-600 dark:text-blue-300" data-v-dc7e54e5><p data-v-dc7e54e5><strong data-v-dc7e54e5>Business Owner:</strong> owner@demo.toss.co.za / password123</p><p data-v-dc7e54e5><strong data-v-dc7e54e5>Manager:</strong> manager@demo.toss.co.za / password123</p><p data-v-dc7e54e5><strong data-v-dc7e54e5>Employee:</strong> employee@demo.toss.co.za / password123</p></div><button type="button" class="mt-2 text-xs text-blue-600 dark:text-blue-400 hover:text-blue-500 dark:hover:text-blue-300 underline" data-v-dc7e54e5> Use demo credentials </button></div><div class="text-center" data-v-dc7e54e5><p class="text-sm text-gray-600 dark:text-gray-400" data-v-dc7e54e5> Don&#39;t have an account? `);
      _push(ssrRenderComponent(_component_NuxtLink, {
        to: "/register",
        class: "font-medium text-green-600 hover:text-green-500 dark:text-green-400 dark:hover:text-green-300"
      }, {
        default: withCtx((_, _push2, _parent2, _scopeId) => {
          if (_push2) {
            _push2(` Sign up for free `);
          } else {
            return [
              createTextVNode(" Sign up for free ")
            ];
          }
        }),
        _: 1
      }, _parent));
      _push(`</p></div></form><div class="mt-8 grid grid-cols-2 gap-4 text-center" data-v-dc7e54e5><div class="p-3 bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700" data-v-dc7e54e5><div class="text-green-600 dark:text-green-400 mb-1" data-v-dc7e54e5><svg class="w-6 h-6 mx-auto" fill="none" stroke="currentColor" viewBox="0 0 24 24" data-v-dc7e54e5><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" data-v-dc7e54e5></path></svg></div><p class="text-xs font-medium text-gray-900 dark:text-white" data-v-dc7e54e5>Group Buying</p><p class="text-xs text-gray-500 dark:text-gray-400" data-v-dc7e54e5>Save together</p></div><div class="p-3 bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700" data-v-dc7e54e5><div class="text-green-600 dark:text-green-400 mb-1" data-v-dc7e54e5><svg class="w-6 h-6 mx-auto" fill="none" stroke="currentColor" viewBox="0 0 24 24" data-v-dc7e54e5><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.663 17h4.673M12 3v1m6.364 1.636l-.707.707M21 12h-1M4 12H3m3.343-5.657l-.707-.707m2.828 9.9a5 5 0 117.072 0l-.548.547A3.374 3.374 0 0014 18.469V19a2 2 0 11-4 0v-.531c0-.895-.356-1.754-.988-2.386l-.548-.547z" data-v-dc7e54e5></path></svg></div><p class="text-xs font-medium text-gray-900 dark:text-white" data-v-dc7e54e5>AI Assistant</p><p class="text-xs text-gray-500 dark:text-gray-400" data-v-dc7e54e5>Smart insights</p></div></div></div></div>`);
    };
  }
});
const _sfc_setup = _sfc_main.setup;
_sfc_main.setup = (props, ctx) => {
  const ssrContext = useSSRContext();
  (ssrContext.modules || (ssrContext.modules = /* @__PURE__ */ new Set())).add("pages/login.vue");
  return _sfc_setup ? _sfc_setup(props, ctx) : void 0;
};
const login = /* @__PURE__ */ _export_sfc(_sfc_main, [["__scopeId", "data-v-dc7e54e5"]]);

export { login as default };
//# sourceMappingURL=login-B9RBVj0S.mjs.map
