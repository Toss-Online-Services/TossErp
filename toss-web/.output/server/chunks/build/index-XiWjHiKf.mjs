import { _ as __nuxt_component_0 } from './nuxt-link-CzepOQdf.mjs';
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
      title: "Purchasing - TOSS ERP",
      meta: [
        { name: "description", content: "Purchasing and procurement management in TOSS ERP" }
      ]
    });
    return (_ctx, _push, _parent, _attrs) => {
      const _component_NuxtLink = __nuxt_component_0;
      _push(`<div${ssrRenderAttrs(mergeProps({ class: "min-h-screen bg-gray-50 dark:bg-gray-900" }, _attrs))}><div class="bg-white dark:bg-gray-800 shadow-sm border-b border-gray-200 dark:border-gray-700"><div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8"><div class="py-4"><div class="flex items-center justify-between"><div><h1 class="text-2xl font-bold text-gray-900 dark:text-white">Purchasing &amp; Procurement</h1><p class="text-gray-600 dark:text-gray-400">Manage suppliers, purchase orders, and procurement processes</p></div><div class="flex space-x-3"><button class="bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700 transition-colors"><span class="inline-block w-5 h-5 mr-2">\u{1F4C4}</span> New PO </button><button class="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors"><span class="inline-block w-5 h-5 mr-2">\u{1F91D}</span> Find Group Buy </button></div></div></div></div></div><div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12"><div class="text-center"><div class="w-32 h-32 mx-auto mb-8 bg-gradient-to-r from-purple-400 to-pink-500 rounded-full flex items-center justify-center"><span class="text-6xl text-white">\u{1F6CD}\uFE0F</span></div><h2 class="text-3xl font-bold text-gray-900 dark:text-white mb-4">Purchasing &amp; Procurement</h2><p class="text-xl text-gray-600 dark:text-gray-400 mb-8 max-w-2xl mx-auto"> Advanced procurement features are being developed with focus on collaborative purchasing, supplier networks, and group buying integration for maximum cost savings. </p><div class="flex justify-center space-x-4">`);
      _push(ssrRenderComponent(_component_NuxtLink, {
        to: "/group-buying",
        class: "bg-blue-600 text-white px-6 py-3 rounded-lg hover:bg-blue-700 transition-colors"
      }, {
        default: withCtx((_, _push2, _parent2, _scopeId) => {
          if (_push2) {
            _push2(` Explore Group Buying `);
          } else {
            return [
              createTextVNode(" Explore Group Buying ")
            ];
          }
        }),
        _: 1
      }, _parent));
      _push(ssrRenderComponent(_component_NuxtLink, {
        to: "/dashboard",
        class: "bg-gray-600 text-white px-6 py-3 rounded-lg hover:bg-gray-700 transition-colors"
      }, {
        default: withCtx((_, _push2, _parent2, _scopeId) => {
          if (_push2) {
            _push2(` Back to Dashboard `);
          } else {
            return [
              createTextVNode(" Back to Dashboard ")
            ];
          }
        }),
        _: 1
      }, _parent));
      _push(`</div></div></div></div>`);
    };
  }
});
const _sfc_setup = _sfc_main.setup;
_sfc_main.setup = (props, ctx) => {
  const ssrContext = useSSRContext();
  (ssrContext.modules || (ssrContext.modules = /* @__PURE__ */ new Set())).add("pages/purchasing/index.vue");
  return _sfc_setup ? _sfc_setup(props, ctx) : void 0;
};

export { _sfc_main as default };
//# sourceMappingURL=index-XiWjHiKf.mjs.map
