import { resolveComponent, mergeProps, withCtx, createVNode, useSSRContext } from 'vue';
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

const _sfc_main = {
  __name: "index",
  __ssrInlineRender: true,
  setup(__props) {
    useHead({
      title: "Tools & Resources - TOSS ERP III"
    });
    return (_ctx, _push, _parent, _attrs) => {
      const _component_IconBase = resolveComponent("IconBase");
      _push(`<div${ssrRenderAttrs(mergeProps({ class: "min-h-screen bg-gray-50 dark:bg-gray-900" }, _attrs))}><div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8"><div class="mb-8"><h1 class="text-3xl font-bold text-gray-900 dark:text-white">Tools &amp; Resources</h1><p class="mt-2 text-gray-600 dark:text-gray-400">Business tools, calculators, and utilities for SMMEs</p></div><div class="bg-purple-50 dark:bg-purple-900/20 border border-purple-200 dark:border-purple-800 rounded-lg p-6 text-center">`);
      _push(ssrRenderComponent(_component_IconBase, { class: "mx-auto h-12 w-12 text-purple-600 mb-4" }, {
        default: withCtx((_, _push2, _parent2, _scopeId) => {
          if (_push2) {
            _push2(`<path d="M11.78 5.22a.75.75 0 0 1 1.06 0l3 3a.75.75 0 0 1 0 1.06l-3 3a.75.75 0 1 1-1.06-1.06L13.94 9H7.5a.75.75 0 0 1 0-1.5h6.44l-2.16-2.16a.75.75 0 0 1 0-1.06Z"${_scopeId}></path>`);
          } else {
            return [
              createVNode("path", { d: "M11.78 5.22a.75.75 0 0 1 1.06 0l3 3a.75.75 0 0 1 0 1.06l-3 3a.75.75 0 1 1-1.06-1.06L13.94 9H7.5a.75.75 0 0 1 0-1.5h6.44l-2.16-2.16a.75.75 0 0 1 0-1.06Z" })
            ];
          }
        }),
        _: 1
      }, _parent));
      _push(`<h2 class="text-xl font-semibold text-purple-900 dark:text-purple-100 mb-2">Coming Soon</h2><p class="text-purple-700 dark:text-purple-300"> The tools and resources section is currently under development and will be available soon. </p></div><div class="mt-8 grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6"><div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6"><h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-2">Business Calculators</h3><p class="text-gray-600 dark:text-gray-400">ROI, break-even, loan, and tax calculators.</p></div><div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6"><h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-2">Document Templates</h3><p class="text-gray-600 dark:text-gray-400">Contracts, invoices, and business document templates.</p></div><div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6"><h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-2">Compliance Tools</h3><p class="text-gray-600 dark:text-gray-400">BEE scoring, tax compliance, and regulatory tools.</p></div></div></div></div>`);
    };
  }
};
const _sfc_setup = _sfc_main.setup;
_sfc_main.setup = (props, ctx) => {
  const ssrContext = useSSRContext();
  (ssrContext.modules || (ssrContext.modules = /* @__PURE__ */ new Set())).add("pages/tools/index.vue");
  return _sfc_setup ? _sfc_setup(props, ctx) : void 0;
};

export { _sfc_main as default };
//# sourceMappingURL=index-DUiUWuZp.mjs.map
