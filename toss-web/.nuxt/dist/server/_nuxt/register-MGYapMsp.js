import { _ as __nuxt_component_0 } from "./nuxt-link-CzepOQdf.js";
import { defineComponent, ref, computed, mergeProps, unref, withCtx, createTextVNode, useSSRContext } from "vue";
import { ssrRenderAttrs, ssrRenderAttr, ssrIncludeBooleanAttr, ssrLooseContain, ssrLooseEqual, ssrRenderComponent, ssrInterpolate } from "vue/server-renderer";
import "C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/node_modules/hookable/dist/index.mjs";
import { u as useHead, _ as _export_sfc } from "../server.mjs";
import "C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/node_modules/klona/dist/index.mjs";
import "C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/node_modules/defu/dist/defu.mjs";
import "#internal/nuxt/paths";
import { u as useUserStore } from "./user-B2SCl6g9.js";
import "C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/node_modules/ufo/dist/index.mjs";
import "ofetch";
import "C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/node_modules/unctx/dist/index.mjs";
import "C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/node_modules/h3/dist/index.mjs";
import "vue-router";
import "C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/node_modules/radix3/dist/index.mjs";
import "C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/node_modules/@unhead/vue/dist/index.mjs";
import "C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/node_modules/nuxt/node_modules/cookie-es/dist/index.mjs";
import "C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/node_modules/destr/dist/index.mjs";
import "C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/node_modules/ohash/dist/index.mjs";
const _sfc_main = /* @__PURE__ */ defineComponent({
  __name: "register",
  __ssrInlineRender: true,
  setup(__props) {
    useHead({
      title: "Sign Up - Join TOSS ERP"
    });
    const loading = ref(false);
    const form = ref({
      businessName: "",
      firstName: "",
      lastName: "",
      email: "",
      phone: "",
      password: "",
      confirmPassword: "",
      businessType: "",
      agreeTerms: false
    });
    const isFormValid = computed(() => {
      return form.value.businessName && form.value.firstName && form.value.lastName && form.value.email && form.value.phone && form.value.password && form.value.confirmPassword && form.value.businessType && form.value.agreeTerms && form.value.password === form.value.confirmPassword && form.value.password.length >= 8;
    });
    useUserStore();
    return (_ctx, _push, _parent, _attrs) => {
      const _component_NuxtLink = __nuxt_component_0;
      _push(`<div${ssrRenderAttrs(mergeProps({ class: "min-h-screen flex items-center justify-center bg-gray-50 dark:bg-gray-900 py-12 px-4 sm:px-6 lg:px-8" }, _attrs))} data-v-f629ca36><div class="max-w-md w-full space-y-8" data-v-f629ca36><div data-v-f629ca36><div class="mx-auto h-12 w-12 flex items-center justify-center rounded-full bg-gradient-to-r from-green-400 to-green-600" data-v-f629ca36><svg class="h-8 w-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24" data-v-f629ca36><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 10V3L4 14h7v7l9-11h-7z" data-v-f629ca36></path></svg></div><h2 class="mt-6 text-center text-3xl font-extrabold text-gray-900 dark:text-white" data-v-f629ca36> Join TOSS ERP </h2><p class="mt-2 text-center text-sm text-gray-600 dark:text-gray-400" data-v-f629ca36> Create your free business account today </p></div><form class="mt-8 space-y-6" data-v-f629ca36><div class="space-y-4" data-v-f629ca36><div data-v-f629ca36><label for="businessName" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1" data-v-f629ca36> Business Name </label><input id="businessName"${ssrRenderAttr("value", unref(form).businessName)} name="businessName" type="text" required class="appearance-none relative block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 placeholder-gray-500 dark:placeholder-gray-400 text-gray-900 dark:text-white rounded-md focus:outline-none focus:ring-green-500 focus:border-green-500 sm:text-sm bg-white dark:bg-gray-800" placeholder="e.g., Thabo&#39;s Spaza Shop" data-v-f629ca36></div><div class="grid grid-cols-2 gap-4" data-v-f629ca36><div data-v-f629ca36><label for="firstName" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1" data-v-f629ca36> First Name </label><input id="firstName"${ssrRenderAttr("value", unref(form).firstName)} name="firstName" type="text" required class="appearance-none relative block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 placeholder-gray-500 dark:placeholder-gray-400 text-gray-900 dark:text-white rounded-md focus:outline-none focus:ring-green-500 focus:border-green-500 sm:text-sm bg-white dark:bg-gray-800" placeholder="First name" data-v-f629ca36></div><div data-v-f629ca36><label for="lastName" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1" data-v-f629ca36> Last Name </label><input id="lastName"${ssrRenderAttr("value", unref(form).lastName)} name="lastName" type="text" required class="appearance-none relative block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 placeholder-gray-500 dark:placeholder-gray-400 text-gray-900 dark:text-white rounded-md focus:outline-none focus:ring-green-500 focus:border-green-500 sm:text-sm bg-white dark:bg-gray-800" placeholder="Last name" data-v-f629ca36></div></div><div data-v-f629ca36><label for="email" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1" data-v-f629ca36> Email Address </label><input id="email"${ssrRenderAttr("value", unref(form).email)} name="email" type="email" autocomplete="email" required class="appearance-none relative block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 placeholder-gray-500 dark:placeholder-gray-400 text-gray-900 dark:text-white rounded-md focus:outline-none focus:ring-green-500 focus:border-green-500 sm:text-sm bg-white dark:bg-gray-800" placeholder="your@email.com" data-v-f629ca36></div><div data-v-f629ca36><label for="phone" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1" data-v-f629ca36> Phone Number </label><input id="phone"${ssrRenderAttr("value", unref(form).phone)} name="phone" type="tel" required class="appearance-none relative block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 placeholder-gray-500 dark:placeholder-gray-400 text-gray-900 dark:text-white rounded-md focus:outline-none focus:ring-green-500 focus:border-green-500 sm:text-sm bg-white dark:bg-gray-800" placeholder="+27 XX XXX XXXX" data-v-f629ca36></div><div data-v-f629ca36><label for="password" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1" data-v-f629ca36> Password </label><input id="password"${ssrRenderAttr("value", unref(form).password)} name="password" type="password" required class="appearance-none relative block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 placeholder-gray-500 dark:placeholder-gray-400 text-gray-900 dark:text-white rounded-md focus:outline-none focus:ring-green-500 focus:border-green-500 sm:text-sm bg-white dark:bg-gray-800" placeholder="Create a strong password" data-v-f629ca36></div><div data-v-f629ca36><label for="confirmPassword" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1" data-v-f629ca36> Confirm Password </label><input id="confirmPassword"${ssrRenderAttr("value", unref(form).confirmPassword)} name="confirmPassword" type="password" required class="appearance-none relative block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 placeholder-gray-500 dark:placeholder-gray-400 text-gray-900 dark:text-white rounded-md focus:outline-none focus:ring-green-500 focus:border-green-500 sm:text-sm bg-white dark:bg-gray-800" placeholder="Confirm your password" data-v-f629ca36></div><div data-v-f629ca36><label for="businessType" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1" data-v-f629ca36> Business Type </label><select id="businessType" name="businessType" required class="appearance-none relative block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 text-gray-900 dark:text-white rounded-md focus:outline-none focus:ring-green-500 focus:border-green-500 sm:text-sm bg-white dark:bg-gray-800" data-v-f629ca36><option value="" data-v-f629ca36${ssrIncludeBooleanAttr(Array.isArray(unref(form).businessType) ? ssrLooseContain(unref(form).businessType, "") : ssrLooseEqual(unref(form).businessType, "")) ? " selected" : ""}>Select your business type</option><option value="spaza" data-v-f629ca36${ssrIncludeBooleanAttr(Array.isArray(unref(form).businessType) ? ssrLooseContain(unref(form).businessType, "spaza") : ssrLooseEqual(unref(form).businessType, "spaza")) ? " selected" : ""}>Spaza Shop</option><option value="retail" data-v-f629ca36${ssrIncludeBooleanAttr(Array.isArray(unref(form).businessType) ? ssrLooseContain(unref(form).businessType, "retail") : ssrLooseEqual(unref(form).businessType, "retail")) ? " selected" : ""}>Retail Store</option><option value="restaurant" data-v-f629ca36${ssrIncludeBooleanAttr(Array.isArray(unref(form).businessType) ? ssrLooseContain(unref(form).businessType, "restaurant") : ssrLooseEqual(unref(form).businessType, "restaurant")) ? " selected" : ""}>Restaurant/Takeaway</option><option value="service" data-v-f629ca36${ssrIncludeBooleanAttr(Array.isArray(unref(form).businessType) ? ssrLooseContain(unref(form).businessType, "service") : ssrLooseEqual(unref(form).businessType, "service")) ? " selected" : ""}>Service Business</option><option value="wholesale" data-v-f629ca36${ssrIncludeBooleanAttr(Array.isArray(unref(form).businessType) ? ssrLooseContain(unref(form).businessType, "wholesale") : ssrLooseEqual(unref(form).businessType, "wholesale")) ? " selected" : ""}>Wholesale</option><option value="manufacturing" data-v-f629ca36${ssrIncludeBooleanAttr(Array.isArray(unref(form).businessType) ? ssrLooseContain(unref(form).businessType, "manufacturing") : ssrLooseEqual(unref(form).businessType, "manufacturing")) ? " selected" : ""}>Manufacturing</option><option value="other" data-v-f629ca36${ssrIncludeBooleanAttr(Array.isArray(unref(form).businessType) ? ssrLooseContain(unref(form).businessType, "other") : ssrLooseEqual(unref(form).businessType, "other")) ? " selected" : ""}>Other</option></select></div></div><div class="flex items-center" data-v-f629ca36><input id="agreeTerms"${ssrIncludeBooleanAttr(Array.isArray(unref(form).agreeTerms) ? ssrLooseContain(unref(form).agreeTerms, null) : unref(form).agreeTerms) ? " checked" : ""} name="agreeTerms" type="checkbox" required class="h-4 w-4 text-green-600 focus:ring-green-500 border-gray-300 dark:border-gray-600 rounded bg-white dark:bg-gray-800" data-v-f629ca36><label for="agreeTerms" class="ml-2 block text-sm text-gray-900 dark:text-white" data-v-f629ca36> I agree to the `);
      _push(ssrRenderComponent(_component_NuxtLink, {
        to: "/terms",
        class: "text-green-600 hover:text-green-500 dark:text-green-400 dark:hover:text-green-300 underline"
      }, {
        default: withCtx((_, _push2, _parent2, _scopeId) => {
          if (_push2) {
            _push2(` Terms of Service `);
          } else {
            return [
              createTextVNode(" Terms of Service ")
            ];
          }
        }),
        _: 1
      }, _parent));
      _push(` and `);
      _push(ssrRenderComponent(_component_NuxtLink, {
        to: "/privacy",
        class: "text-green-600 hover:text-green-500 dark:text-green-400 dark:hover:text-green-300 underline"
      }, {
        default: withCtx((_, _push2, _parent2, _scopeId) => {
          if (_push2) {
            _push2(` Privacy Policy `);
          } else {
            return [
              createTextVNode(" Privacy Policy ")
            ];
          }
        }),
        _: 1
      }, _parent));
      _push(`</label></div><div data-v-f629ca36><button type="submit"${ssrIncludeBooleanAttr(unref(loading) || !unref(isFormValid)) ? " disabled" : ""} class="group relative w-full flex justify-center py-2 px-4 border border-transparent text-sm font-medium rounded-md text-white bg-green-600 hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-500 disabled:opacity-50 disabled:cursor-not-allowed" data-v-f629ca36><span class="absolute left-0 inset-y-0 flex items-center pl-3" data-v-f629ca36>`);
      if (!unref(loading)) {
        _push(`<svg class="h-5 w-5 text-green-500 group-hover:text-green-400" fill="currentColor" viewBox="0 0 20 20" data-v-f629ca36><path d="M8 9a3 3 0 100-6 3 3 0 000 6zM8 11a6 6 0 016 6H2a6 6 0 016-6zM16 7a1 1 0 10-2 0v1h-1a1 1 0 100 2h1v1a1 1 0 102 0v-1h1a1 1 0 100-2h-1V7z" data-v-f629ca36></path></svg>`);
      } else {
        _push(`<svg class="animate-spin h-5 w-5 text-green-500" fill="none" viewBox="0 0 24 24" data-v-f629ca36><circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4" data-v-f629ca36></circle><path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z" data-v-f629ca36></path></svg>`);
      }
      _push(`</span> ${ssrInterpolate(unref(loading) ? "Creating account..." : "Create account")}</button></div><div class="text-center" data-v-f629ca36><p class="text-sm text-gray-600 dark:text-gray-400" data-v-f629ca36> Already have an account? `);
      _push(ssrRenderComponent(_component_NuxtLink, {
        to: "/login",
        class: "font-medium text-green-600 hover:text-green-500 dark:text-green-400 dark:hover:text-green-300"
      }, {
        default: withCtx((_, _push2, _parent2, _scopeId) => {
          if (_push2) {
            _push2(` Sign in `);
          } else {
            return [
              createTextVNode(" Sign in ")
            ];
          }
        }),
        _: 1
      }, _parent));
      _push(`</p></div></form><div class="mt-8 bg-green-50 dark:bg-green-900/20 rounded-lg p-4" data-v-f629ca36><h3 class="text-sm font-medium text-green-800 dark:text-green-200 mb-3" data-v-f629ca36>Why join TOSS ERP?</h3><div class="space-y-2 text-xs text-green-700 dark:text-green-300" data-v-f629ca36><div class="flex items-center" data-v-f629ca36><svg class="w-4 h-4 mr-2" fill="currentColor" viewBox="0 0 20 20" data-v-f629ca36><path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" data-v-f629ca36></path></svg> Group buying to save money together </div><div class="flex items-center" data-v-f629ca36><svg class="w-4 h-4 mr-2" fill="currentColor" viewBox="0 0 20 20" data-v-f629ca36><path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" data-v-f629ca36></path></svg> AI-powered business insights </div><div class="flex items-center" data-v-f629ca36><svg class="w-4 h-4 mr-2" fill="currentColor" viewBox="0 0 20 20" data-v-f629ca36><path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" data-v-f629ca36></path></svg> Complete inventory &amp; sales management </div><div class="flex items-center" data-v-f629ca36><svg class="w-4 h-4 mr-2" fill="currentColor" viewBox="0 0 20 20" data-v-f629ca36><path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" data-v-f629ca36></path></svg> Community collaboration tools </div></div></div></div></div>`);
    };
  }
});
const _sfc_setup = _sfc_main.setup;
_sfc_main.setup = (props, ctx) => {
  const ssrContext = useSSRContext();
  (ssrContext.modules || (ssrContext.modules = /* @__PURE__ */ new Set())).add("pages/register.vue");
  return _sfc_setup ? _sfc_setup(props, ctx) : void 0;
};
const register = /* @__PURE__ */ _export_sfc(_sfc_main, [["__scopeId", "data-v-f629ca36"]]);
export {
  register as default
};
//# sourceMappingURL=register-MGYapMsp.js.map
