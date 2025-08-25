import { mergeProps, useSSRContext } from "vue";
import { ssrRenderAttrs } from "vue/server-renderer";
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
      title: "Settings - TOSS ERP III"
    });
    return (_ctx, _push, _parent, _attrs) => {
      _push(`<div${ssrRenderAttrs(mergeProps({ class: "min-h-screen bg-gray-50 dark:bg-gray-900" }, _attrs))}><div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8"><div class="mb-8"><h1 class="text-3xl font-bold text-gray-900 dark:text-white">Settings</h1><p class="mt-2 text-gray-600 dark:text-gray-400">Configure your application preferences and system settings</p></div><div class="space-y-6"><div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6"><h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">Appearance</h3><div class="space-y-4"><div><label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Theme</label><select class="block w-full rounded-md border-gray-300 dark:border-gray-600 dark:bg-gray-700 shadow-sm"><option>Light</option><option>Dark</option><option>System</option></select></div><div><label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Language</label><select class="block w-full rounded-md border-gray-300 dark:border-gray-600 dark:bg-gray-700 shadow-sm"><option>English</option><option>Afrikaans</option><option>Zulu</option><option>Xhosa</option></select></div></div></div><div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6"><h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">Notifications</h3><div class="space-y-4"><div class="flex items-center justify-between"><div><p class="text-sm font-medium text-gray-700 dark:text-gray-300">Email Notifications</p><p class="text-xs text-gray-500 dark:text-gray-400">Receive important updates via email</p></div><input type="checkbox" checked class="rounded border-gray-300 text-blue-600 shadow-sm"></div><div class="flex items-center justify-between"><div><p class="text-sm font-medium text-gray-700 dark:text-gray-300">Push Notifications</p><p class="text-xs text-gray-500 dark:text-gray-400">Get instant notifications in your browser</p></div><input type="checkbox" checked class="rounded border-gray-300 text-blue-600 shadow-sm"></div><div class="flex items-center justify-between"><div><p class="text-sm font-medium text-gray-700 dark:text-gray-300">SMS Alerts</p><p class="text-xs text-gray-500 dark:text-gray-400">Receive critical alerts via SMS</p></div><input type="checkbox" class="rounded border-gray-300 text-blue-600 shadow-sm"></div></div></div><div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6"><h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">Security</h3><div class="space-y-4"><div><button class="w-full text-left bg-gray-50 dark:bg-gray-700 p-4 rounded-lg border border-gray-200 dark:border-gray-600 hover:bg-gray-100 dark:hover:bg-gray-600 transition-colors"><p class="text-sm font-medium text-gray-700 dark:text-gray-300">Change Password</p><p class="text-xs text-gray-500 dark:text-gray-400">Update your account password</p></button></div><div><button class="w-full text-left bg-gray-50 dark:bg-gray-700 p-4 rounded-lg border border-gray-200 dark:border-gray-600 hover:bg-gray-100 dark:hover:bg-gray-600 transition-colors"><p class="text-sm font-medium text-gray-700 dark:text-gray-300">Two-Factor Authentication</p><p class="text-xs text-gray-500 dark:text-gray-400">Add an extra layer of security</p></button></div></div></div><div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6"><h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">AI Copilot</h3><div class="space-y-4"><div class="flex items-center justify-between"><div><p class="text-sm font-medium text-gray-700 dark:text-gray-300">Enable AI Assistance</p><p class="text-xs text-gray-500 dark:text-gray-400">Get intelligent suggestions and automation</p></div><input type="checkbox" checked class="rounded border-gray-300 text-blue-600 shadow-sm"></div><div><label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">AI Model</label><select class="block w-full rounded-md border-gray-300 dark:border-gray-600 dark:bg-gray-700 shadow-sm"><option>GPT-4</option><option>Claude</option><option>Gemini</option></select></div></div></div><div class="flex justify-end"><button class="bg-blue-600 text-white px-6 py-2 rounded-lg hover:bg-blue-700 transition-colors"> Save Settings </button></div></div></div></div>`);
    };
  }
};
const _sfc_setup = _sfc_main.setup;
_sfc_main.setup = (props, ctx) => {
  const ssrContext = useSSRContext();
  (ssrContext.modules || (ssrContext.modules = /* @__PURE__ */ new Set())).add("pages/settings/index.vue");
  return _sfc_setup ? _sfc_setup(props, ctx) : void 0;
};
export {
  _sfc_main as default
};
//# sourceMappingURL=index-CN2bdBUQ.js.map
