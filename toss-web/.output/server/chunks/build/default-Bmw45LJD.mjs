import __nuxt_component_0 from './AppNavigation-8oXhv3oB.mjs';
import _sfc_main$1 from './AICopilotChat-BbZcaCTr.mjs';
import __nuxt_component_2 from './NotificationContainer-DecAytol.mjs';
import { defineComponent, mergeProps, useSSRContext } from 'vue';
import { ssrRenderAttrs, ssrRenderComponent, ssrRenderSlot } from 'vue/server-renderer';
import { u as useHead, e as defineStore } from './server.mjs';
import { u as useUserStore } from './user-B2SCl6g9.mjs';
import './nuxt-link-CzepOQdf.mjs';
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

const useSettingsStore = defineStore("settings", {
  state: () => ({
    darkMode: false,
    language: "en",
    notifications: {
      email: true,
      push: true,
      sms: false
    },
    sidebar: {
      collapsed: false
    }
  }),
  actions: {
    async loadSettings() {
      const saved = localStorage.getItem("toss-settings");
      if (saved) {
        this.$patch(JSON.parse(saved));
      }
    },
    async saveSettings() {
      localStorage.setItem("toss-settings", JSON.stringify(this.$state));
    },
    toggleDarkMode() {
      this.darkMode = !this.darkMode;
      this.saveSettings();
    },
    setLanguage(lang) {
      this.language = lang;
      this.saveSettings();
    },
    toggleSidebar() {
      this.sidebar.collapsed = !this.sidebar.collapsed;
      this.saveSettings();
    }
  }
});
const _sfc_main = /* @__PURE__ */ defineComponent({
  __name: "default",
  __ssrInlineRender: true,
  setup(__props) {
    useHead({
      titleTemplate: "%s - TOSS ERP III",
      meta: [
        { name: "viewport", content: "width=device-width, initial-scale=1" },
        { name: "theme-color", content: "#1d4ed8" }
      ]
    });
    useUserStore();
    useSettingsStore();
    return (_ctx, _push, _parent, _attrs) => {
      const _component_AppNavigation = __nuxt_component_0;
      const _component_AICopilotChat = _sfc_main$1;
      const _component_NotificationContainer = __nuxt_component_2;
      _push(`<div${ssrRenderAttrs(mergeProps({ class: "min-h-screen bg-gray-50 dark:bg-gray-900" }, _attrs))}>`);
      _push(ssrRenderComponent(_component_AppNavigation, null, null, _parent));
      _push(`<main class="flex-1"><div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">`);
      ssrRenderSlot(_ctx.$slots, "default", {}, null, _push, _parent);
      _push(`</div></main>`);
      _push(ssrRenderComponent(_component_AICopilotChat, null, null, _parent));
      _push(ssrRenderComponent(_component_NotificationContainer, null, null, _parent));
      _push(`</div>`);
    };
  }
});
const _sfc_setup = _sfc_main.setup;
_sfc_main.setup = (props, ctx) => {
  const ssrContext = useSSRContext();
  (ssrContext.modules || (ssrContext.modules = /* @__PURE__ */ new Set())).add("layouts/default.vue");
  return _sfc_setup ? _sfc_setup(props, ctx) : void 0;
};

export { _sfc_main as default };
//# sourceMappingURL=default-Bmw45LJD.mjs.map
