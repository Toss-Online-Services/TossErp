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
      title: "Collaboration Hub - TOSS ERP III"
    });
    return (_ctx, _push, _parent, _attrs) => {
      const _component_IconBase = resolveComponent("IconBase");
      _push(`<div${ssrRenderAttrs(mergeProps({ class: "min-h-screen bg-gray-50 dark:bg-gray-900" }, _attrs))}><div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8"><div class="mb-8"><h1 class="text-3xl font-bold text-gray-900 dark:text-white">Collaboration Hub</h1><p class="mt-2 text-gray-600 dark:text-gray-400">Connect, collaborate, and grow together with other SMMEs</p></div><div class="bg-indigo-50 dark:bg-indigo-900/20 border border-indigo-200 dark:border-indigo-800 rounded-lg p-6 text-center">`);
      _push(ssrRenderComponent(_component_IconBase, { class: "mx-auto h-12 w-12 text-indigo-600 mb-4" }, {
        default: withCtx((_, _push2, _parent2, _scopeId) => {
          if (_push2) {
            _push2(`<path d="M15 8a3 3 0 11-6 0 3 3 0 016 0zM6.5 15.5A7.5 7.5 0 0114 8a7.5 7.5 0 01-7.5 7.5z"${_scopeId}></path>`);
          } else {
            return [
              createVNode("path", { d: "M15 8a3 3 0 11-6 0 3 3 0 016 0zM6.5 15.5A7.5 7.5 0 0114 8a7.5 7.5 0 01-7.5 7.5z" })
            ];
          }
        }),
        _: 1
      }, _parent));
      _push(`<h2 class="text-xl font-semibold text-indigo-900 dark:text-indigo-100 mb-2">Coming Soon</h2><p class="text-indigo-700 dark:text-indigo-300"> The collaboration hub is currently under development and will be available soon. </p></div><div class="mt-8 grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6"><div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6"><h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-2">Business Network</h3><p class="text-gray-600 dark:text-gray-400">Connect with other SMMEs in your area and industry.</p></div><div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6"><h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-2">Asset Sharing</h3><p class="text-gray-600 dark:text-gray-400">Share tools, equipment, and resources with other businesses.</p></div><div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6"><h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-2">Joint Ventures</h3><p class="text-gray-600 dark:text-gray-400">Collaborate on projects and joint business opportunities.</p></div></div></div></div>`);
    };
  }
};
const _sfc_setup = _sfc_main.setup;
_sfc_main.setup = (props, ctx) => {
  const ssrContext = useSSRContext();
  (ssrContext.modules || (ssrContext.modules = /* @__PURE__ */ new Set())).add("pages/collaboration/index.vue");
  return _sfc_setup ? _sfc_setup(props, ctx) : void 0;
};
export {
  _sfc_main as default
};
//# sourceMappingURL=index-1AIfD4Wt.js.map
