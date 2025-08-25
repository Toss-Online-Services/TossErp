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
      title: "Manufacturing - TOSS ERP III"
    });
    return (_ctx, _push, _parent, _attrs) => {
      const _component_IconBase = resolveComponent("IconBase");
      _push(`<div${ssrRenderAttrs(mergeProps({ class: "min-h-screen bg-gray-50 dark:bg-gray-900" }, _attrs))}><div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8"><div class="mb-8"><h1 class="text-3xl font-bold text-gray-900 dark:text-white">Manufacturing</h1><p class="mt-2 text-gray-600 dark:text-gray-400">Production planning, quality control, and manufacturing operations</p></div><div class="bg-gray-50 dark:bg-gray-900/20 border border-gray-200 dark:border-gray-800 rounded-lg p-6 text-center">`);
      _push(ssrRenderComponent(_component_IconBase, { class: "mx-auto h-12 w-12 text-gray-600 mb-4" }, {
        default: withCtx((_, _push2, _parent2, _scopeId) => {
          if (_push2) {
            _push2(`<path d="M11.42 15.17L17.25 21A2.652 2.652 0 0 0 21 17.25l-5.877-5.877M11.42 15.17l2.496-3.03c.317-.384.74-.626 1.208-.766M11.42 15.17l-4.655-5.653a2.548 2.548 0 0 1-.1-3.528l.893-.893a2.548 2.548 0 0 1 3.528-.1l5.653 4.655M15.125 8.375l-2.496 3.03M15.125 8.375a2.548 2.548 0 0 1 .1 3.528l-.893.893a2.548 2.548 0 0 1-3.528.1M12.629 11.405l-2.496 3.03"${_scopeId}></path>`);
          } else {
            return [
              createVNode("path", { d: "M11.42 15.17L17.25 21A2.652 2.652 0 0 0 21 17.25l-5.877-5.877M11.42 15.17l2.496-3.03c.317-.384.74-.626 1.208-.766M11.42 15.17l-4.655-5.653a2.548 2.548 0 0 1-.1-3.528l.893-.893a2.548 2.548 0 0 1 3.528-.1l5.653 4.655M15.125 8.375l-2.496 3.03M15.125 8.375a2.548 2.548 0 0 1 .1 3.528l-.893.893a2.548 2.548 0 0 1-3.528.1M12.629 11.405l-2.496 3.03" })
            ];
          }
        }),
        _: 1
      }, _parent));
      _push(`<h2 class="text-xl font-semibold text-gray-900 dark:text-gray-100 mb-2">Coming Soon</h2><p class="text-gray-700 dark:text-gray-300"> The manufacturing management module is currently under development and will be available soon. </p></div><div class="mt-8 grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6"><div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6"><h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-2">Production Planning</h3><p class="text-gray-600 dark:text-gray-400">Schedule production runs, manage work orders, and optimize resource allocation.</p></div><div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6"><h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-2">Quality Control</h3><p class="text-gray-600 dark:text-gray-400">Track quality metrics, manage inspections, and ensure compliance standards.</p></div><div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6"><h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-2">Equipment Management</h3><p class="text-gray-600 dark:text-gray-400">Monitor machinery, schedule maintenance, and track equipment performance.</p></div></div></div></div>`);
    };
  }
};
const _sfc_setup = _sfc_main.setup;
_sfc_main.setup = (props, ctx) => {
  const ssrContext = useSSRContext();
  (ssrContext.modules || (ssrContext.modules = /* @__PURE__ */ new Set())).add("pages/manufacturing/index.vue");
  return _sfc_setup ? _sfc_setup(props, ctx) : void 0;
};

export { _sfc_main as default };
//# sourceMappingURL=index-CItc06IY.mjs.map
