import { resolveComponent, mergeProps, withCtx, createVNode, useSSRContext } from "vue";
import { ssrRenderAttrs, ssrRenderComponent } from "vue/server-renderer";
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
const _sfc_main = {
  __name: "index",
  __ssrInlineRender: true,
  setup(__props) {
    useHead({
      title: "Getting Started - TOSS ERP III"
    });
    return (_ctx, _push, _parent, _attrs) => {
      const _component_IconBase = resolveComponent("IconBase");
      _push(`<div${ssrRenderAttrs(mergeProps({ class: "min-h-screen bg-gray-50 dark:bg-gray-900" }, _attrs))}><div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8"><div class="mb-8"><h1 class="text-3xl font-bold text-gray-900 dark:text-white">Getting Started</h1><p class="mt-2 text-gray-600 dark:text-gray-400">Welcome to TOSS ERP III! Let&#39;s get your business set up.</p></div><div class="mb-8"><div class="flex items-center justify-between"><div class="flex items-center"><div class="flex items-center justify-center w-8 h-8 bg-blue-600 text-white rounded-full text-sm font-medium">1</div><span class="ml-3 text-sm font-medium text-gray-900 dark:text-white">Business Profile</span></div><div class="flex-1 h-px bg-gray-300 dark:bg-gray-600 mx-4"></div><div class="flex items-center"><div class="flex items-center justify-center w-8 h-8 bg-gray-300 dark:bg-gray-600 text-gray-600 dark:text-gray-400 rounded-full text-sm font-medium">2</div><span class="ml-3 text-sm font-medium text-gray-500 dark:text-gray-400">Products &amp; Services</span></div><div class="flex-1 h-px bg-gray-300 dark:bg-gray-600 mx-4"></div><div class="flex items-center"><div class="flex items-center justify-center w-8 h-8 bg-gray-300 dark:bg-gray-600 text-gray-600 dark:text-gray-400 rounded-full text-sm font-medium">3</div><span class="ml-3 text-sm font-medium text-gray-500 dark:text-gray-400">First Sale</span></div></div></div><div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6 mb-6"><h2 class="text-xl font-semibold text-gray-900 dark:text-white mb-4">Step 1: Set Up Your Business Profile</h2><p class="text-gray-600 dark:text-gray-400 mb-6"> Let&#39;s start by setting up your business information. This helps other businesses find and connect with you. </p><form class="space-y-4"><div class="grid grid-cols-1 md:grid-cols-2 gap-4"><div><label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Business Name *</label><input type="text" placeholder="Enter your business name" class="w-full rounded-md border-gray-300 dark:border-gray-600 dark:bg-gray-700 shadow-sm"></div><div><label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Industry *</label><select class="w-full rounded-md border-gray-300 dark:border-gray-600 dark:bg-gray-700 shadow-sm"><option value="">Select industry</option><option value="retail">Retail</option><option value="manufacturing">Manufacturing</option><option value="services">Services</option><option value="technology">Technology</option><option value="food">Food &amp; Beverage</option><option value="construction">Construction</option><option value="other">Other</option></select></div></div><div><label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Business Description</label><textarea rows="3" placeholder="Tell us about your business..." class="w-full rounded-md border-gray-300 dark:border-gray-600 dark:bg-gray-700 shadow-sm"></textarea></div><div class="grid grid-cols-1 md:grid-cols-2 gap-4"><div><label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Registration Number</label><input type="text" placeholder="e.g., 2024/123456/23" class="w-full rounded-md border-gray-300 dark:border-gray-600 dark:bg-gray-700 shadow-sm"></div><div><label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">VAT Number</label><input type="text" placeholder="e.g., 4123456789" class="w-full rounded-md border-gray-300 dark:border-gray-600 dark:bg-gray-700 shadow-sm"></div></div><div><label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Business Address</label><input type="text" placeholder="Street address" class="w-full rounded-md border-gray-300 dark:border-gray-600 dark:bg-gray-700 shadow-sm mb-2"><div class="grid grid-cols-2 md:grid-cols-4 gap-2"><input type="text" placeholder="City" class="rounded-md border-gray-300 dark:border-gray-600 dark:bg-gray-700 shadow-sm"><input type="text" placeholder="Province" class="rounded-md border-gray-300 dark:border-gray-600 dark:bg-gray-700 shadow-sm"><input type="text" placeholder="Postal Code" class="rounded-md border-gray-300 dark:border-gray-600 dark:bg-gray-700 shadow-sm"><select class="rounded-md border-gray-300 dark:border-gray-600 dark:bg-gray-700 shadow-sm"><option>South Africa</option></select></div></div></form></div><div class="bg-blue-50 dark:bg-blue-900/20 border border-blue-200 dark:border-blue-800 rounded-lg p-6 mb-6"><h3 class="text-lg font-semibold text-blue-900 dark:text-blue-100 mb-3">`);
      _push(ssrRenderComponent(_component_IconBase, { class: "inline h-5 w-5 mr-2" }, {
        default: withCtx((_, _push2, _parent2, _scopeId) => {
          if (_push2) {
            _push2(`<path d="M12 18v-5.25m0 0a6.01 6.01 0 001.5-.189 6.01 6.01 0 011.699-.686c.85-.34 1.801-.45 2.801-.32v3.97c-.995-.12-1.945-.023-2.801.32a6.01 6.01 0 00-1.699.686A6.01 6.01 0 0112 12.75zm0 0V9a2.25 2.25 0 012.25-2.25h.75c.621 0 1.125-.504 1.125-1.125V4.125C16.125 3.504 15.621 3 15 3H9c-.621 0-1.125.504-1.125 1.125v1.5c0 .621.504 1.125 1.125 1.125h.75A2.25 2.25 0 0112 8.25V9z"${_scopeId}></path>`);
          } else {
            return [
              createVNode("path", { d: "M12 18v-5.25m0 0a6.01 6.01 0 001.5-.189 6.01 6.01 0 011.699-.686c.85-.34 1.801-.45 2.801-.32v3.97c-.995-.12-1.945-.023-2.801.32a6.01 6.01 0 00-1.699.686A6.01 6.01 0 0112 12.75zm0 0V9a2.25 2.25 0 012.25-2.25h.75c.621 0 1.125-.504 1.125-1.125V4.125C16.125 3.504 15.621 3 15 3H9c-.621 0-1.125.504-1.125 1.125v1.5c0 .621.504 1.125 1.125 1.125h.75A2.25 2.25 0 0112 8.25V9z" })
            ];
          }
        }),
        _: 1
      }, _parent));
      _push(` Pro Tips </h3><ul class="space-y-2 text-blue-800 dark:text-blue-200"><li>• Complete your business profile to build trust with other SMME partners</li><li>• Add your registration and VAT numbers to enable formal business transactions</li><li>• A detailed business description helps with AI-powered recommendations</li><li>• You can always update this information later in your profile settings</li></ul></div><div class="flex justify-between"><button class="px-6 py-2 text-gray-600 dark:text-gray-400 hover:text-gray-800 dark:hover:text-gray-200 transition-colors"> Skip for now </button><button class="bg-blue-600 text-white px-6 py-2 rounded-lg hover:bg-blue-700 transition-colors"> Continue to Products &amp; Services </button></div></div></div>`);
    };
  }
};
const _sfc_setup = _sfc_main.setup;
_sfc_main.setup = (props, ctx) => {
  const ssrContext = useSSRContext();
  (ssrContext.modules || (ssrContext.modules = /* @__PURE__ */ new Set())).add("pages/getting-started/index.vue");
  return _sfc_setup ? _sfc_setup(props, ctx) : void 0;
};
export {
  _sfc_main as default
};
//# sourceMappingURL=index-Djn6riIa.js.map
