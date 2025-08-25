import { _ as __nuxt_component_0 } from './nuxt-link-CzepOQdf.mjs';
import { defineComponent, computed, mergeProps, unref, withCtx, createVNode, toDisplayString, createBlock, openBlock, useSSRContext } from 'vue';
import { ssrRenderComponent, ssrInterpolate, ssrRenderClass } from 'vue/server-renderer';
import '../_/nitro.mjs';
import 'node:http';
import 'node:https';
import 'node:events';
import 'node:buffer';
import 'node:fs';
import 'node:path';
import 'node:crypto';
import 'node:url';
import './server.mjs';
import '../routes/renderer.mjs';
import 'vue-bundle-renderer/runtime';
import 'unhead/server';
import 'devalue';
import 'unhead/utils';
import 'unhead/plugins';
import 'vue-router';

const _sfc_main = /* @__PURE__ */ defineComponent({
  __name: "ModuleCard",
  __ssrInlineRender: true,
  props: {
    title: {},
    description: {},
    icon: {},
    link: {},
    color: { default: "blue" },
    status: { default: "active" }
  },
  setup(__props) {
    const props = __props;
    const colorClasses = {
      blue: "bg-white dark:bg-gray-800 border-gray-200 dark:border-gray-700 hover:border-blue-300 dark:hover:border-blue-600",
      green: "bg-white dark:bg-gray-800 border-gray-200 dark:border-gray-700 hover:border-green-300 dark:hover:border-green-600",
      purple: "bg-white dark:bg-gray-800 border-gray-200 dark:border-gray-700 hover:border-purple-300 dark:hover:border-purple-600",
      orange: "bg-white dark:bg-gray-800 border-gray-200 dark:border-gray-700 hover:border-orange-300 dark:hover:border-orange-600",
      teal: "bg-white dark:bg-gray-800 border-gray-200 dark:border-gray-700 hover:border-teal-300 dark:hover:border-teal-600",
      indigo: "bg-white dark:bg-gray-800 border-gray-200 dark:border-gray-700 hover:border-indigo-300 dark:hover:border-indigo-600",
      gray: "bg-white dark:bg-gray-800 border-gray-200 dark:border-gray-700 hover:border-gray-300 dark:hover:border-gray-600",
      emerald: "bg-white dark:bg-gray-800 border-gray-200 dark:border-gray-700 hover:border-emerald-300 dark:hover:border-emerald-600",
      red: "bg-white dark:bg-gray-800 border-gray-200 dark:border-gray-700 hover:border-red-300 dark:hover:border-red-600",
      yellow: "bg-white dark:bg-gray-800 border-gray-200 dark:border-gray-700 hover:border-yellow-300 dark:hover:border-yellow-600",
      pink: "bg-white dark:bg-gray-800 border-gray-200 dark:border-gray-700 hover:border-pink-300 dark:hover:border-pink-600"
    };
    const statusClasses = {
      active: "bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-300",
      beta: "bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-300",
      "coming-soon": "bg-gray-100 text-gray-800 dark:bg-gray-900 dark:text-gray-300"
    };
    const statusText = {
      active: "Active",
      beta: "Beta",
      "coming-soon": "Coming Soon"
    };
    const cardClass = computed(() => colorClasses[props.color] || colorClasses.blue);
    const badgeClass = computed(() => statusClasses[props.status]);
    const badgeText = computed(() => statusText[props.status]);
    return (_ctx, _push, _parent, _attrs) => {
      const _component_NuxtLink = __nuxt_component_0;
      _push(ssrRenderComponent(_component_NuxtLink, mergeProps({
        to: _ctx.link,
        class: [unref(cardClass), "group block p-6 rounded-lg shadow-sm border transition-all duration-200 hover:shadow-lg hover:scale-105"]
      }, _attrs), {
        default: withCtx((_, _push2, _parent2, _scopeId) => {
          if (_push2) {
            _push2(`<div class="flex items-center mb-3"${_scopeId}><div class="text-3xl mr-3"${_scopeId}>${ssrInterpolate(_ctx.icon)}</div><h3 class="text-lg font-semibold text-gray-900 dark:text-white group-hover:text-blue-600 dark:group-hover:text-blue-400 transition-colors"${_scopeId}>${ssrInterpolate(_ctx.title)}</h3></div><p class="text-sm text-gray-600 dark:text-gray-400 mb-4"${_scopeId}>${ssrInterpolate(_ctx.description)}</p><div class="flex items-center justify-between"${_scopeId}><span class="${ssrRenderClass([unref(badgeClass), "px-2 py-1 rounded-full text-xs font-medium"])}"${_scopeId}>${ssrInterpolate(unref(badgeText))}</span><svg class="w-5 h-5 text-gray-400 group-hover:text-blue-600 dark:group-hover:text-blue-400 transition-colors" fill="none" stroke="currentColor" viewBox="0 0 24 24"${_scopeId}><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7"${_scopeId}></path></svg></div>`);
          } else {
            return [
              createVNode("div", { class: "flex items-center mb-3" }, [
                createVNode("div", { class: "text-3xl mr-3" }, toDisplayString(_ctx.icon), 1),
                createVNode("h3", { class: "text-lg font-semibold text-gray-900 dark:text-white group-hover:text-blue-600 dark:group-hover:text-blue-400 transition-colors" }, toDisplayString(_ctx.title), 1)
              ]),
              createVNode("p", { class: "text-sm text-gray-600 dark:text-gray-400 mb-4" }, toDisplayString(_ctx.description), 1),
              createVNode("div", { class: "flex items-center justify-between" }, [
                createVNode("span", {
                  class: [unref(badgeClass), "px-2 py-1 rounded-full text-xs font-medium"]
                }, toDisplayString(unref(badgeText)), 3),
                (openBlock(), createBlock("svg", {
                  class: "w-5 h-5 text-gray-400 group-hover:text-blue-600 dark:group-hover:text-blue-400 transition-colors",
                  fill: "none",
                  stroke: "currentColor",
                  viewBox: "0 0 24 24"
                }, [
                  createVNode("path", {
                    "stroke-linecap": "round",
                    "stroke-linejoin": "round",
                    "stroke-width": "2",
                    d: "M9 5l7 7-7 7"
                  })
                ]))
              ])
            ];
          }
        }),
        _: 1
      }, _parent));
    };
  }
});
const _sfc_setup = _sfc_main.setup;
_sfc_main.setup = (props, ctx) => {
  const ssrContext = useSSRContext();
  (ssrContext.modules || (ssrContext.modules = /* @__PURE__ */ new Set())).add("components/ModuleCard.vue");
  return _sfc_setup ? _sfc_setup(props, ctx) : void 0;
};

export { _sfc_main as default };
//# sourceMappingURL=ModuleCard-aDOCvYGa.mjs.map
