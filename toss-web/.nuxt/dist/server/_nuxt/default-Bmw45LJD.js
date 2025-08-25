import __nuxt_component_0 from "./AppNavigation-8oXhv3oB.js";
import _sfc_main$1 from "./AICopilotChat-BbZcaCTr.js";
import __nuxt_component_2 from "./NotificationContainer-DecAytol.js";
import { defineComponent, mergeProps, useSSRContext } from "vue";
import { ssrRenderAttrs, ssrRenderComponent, ssrRenderSlot } from "vue/server-renderer";
import { e as defineStore, u as useHead } from "../server.mjs";
import { u as useUserStore } from "./user-B2SCl6g9.js";
import "C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/node_modules/hookable/dist/index.mjs";
import "C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/node_modules/klona/dist/index.mjs";
import "C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/node_modules/defu/dist/defu.mjs";
import "#internal/nuxt/paths";
import "./nuxt-link-CzepOQdf.js";
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
export {
  _sfc_main as default
};
//# sourceMappingURL=default-Bmw45LJD.js.map
