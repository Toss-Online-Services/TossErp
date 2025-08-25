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
      title: "User Profile - TOSS ERP III"
    });
    return (_ctx, _push, _parent, _attrs) => {
      _push(`<div${ssrRenderAttrs(mergeProps({ class: "min-h-screen bg-gray-50 dark:bg-gray-900" }, _attrs))}><div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8"><div class="mb-8"><h1 class="text-3xl font-bold text-gray-900 dark:text-white">User Profile</h1><p class="mt-2 text-gray-600 dark:text-gray-400">Manage your account information and preferences</p></div><div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6 mb-6"><div class="flex items-center space-x-6"><div class="w-20 h-20 bg-blue-500 rounded-full flex items-center justify-center text-white text-2xl font-bold"> JD </div><div><h2 class="text-xl font-semibold text-gray-900 dark:text-white">John Doe</h2><p class="text-gray-600 dark:text-gray-400">john.doe@example.com</p><p class="text-sm text-gray-500 dark:text-gray-500">Member since January 2024</p></div></div></div><div class="space-y-6"><div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6"><h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">Personal Information</h3><div class="grid grid-cols-1 md:grid-cols-2 gap-4"><div><label class="block text-sm font-medium text-gray-700 dark:text-gray-300">First Name</label><input type="text" value="John" class="mt-1 block w-full rounded-md border-gray-300 dark:border-gray-600 dark:bg-gray-700 shadow-sm"></div><div><label class="block text-sm font-medium text-gray-700 dark:text-gray-300">Last Name</label><input type="text" value="Doe" class="mt-1 block w-full rounded-md border-gray-300 dark:border-gray-600 dark:bg-gray-700 shadow-sm"></div><div><label class="block text-sm font-medium text-gray-700 dark:text-gray-300">Email</label><input type="email" value="john.doe@example.com" class="mt-1 block w-full rounded-md border-gray-300 dark:border-gray-600 dark:bg-gray-700 shadow-sm"></div><div><label class="block text-sm font-medium text-gray-700 dark:text-gray-300">Phone</label><input type="tel" value="+27 82 123 4567" class="mt-1 block w-full rounded-md border-gray-300 dark:border-gray-600 dark:bg-gray-700 shadow-sm"></div></div></div><div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6"><h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">Business Information</h3><div class="grid grid-cols-1 md:grid-cols-2 gap-4"><div><label class="block text-sm font-medium text-gray-700 dark:text-gray-300">Business Name</label><input type="text" value="Doe Enterprises" class="mt-1 block w-full rounded-md border-gray-300 dark:border-gray-600 dark:bg-gray-700 shadow-sm"></div><div><label class="block text-sm font-medium text-gray-700 dark:text-gray-300">Industry</label><select class="mt-1 block w-full rounded-md border-gray-300 dark:border-gray-600 dark:bg-gray-700 shadow-sm"><option>Retail</option><option>Manufacturing</option><option>Services</option><option>Technology</option></select></div><div><label class="block text-sm font-medium text-gray-700 dark:text-gray-300">Registration Number</label><input type="text" value="2024/123456/23" class="mt-1 block w-full rounded-md border-gray-300 dark:border-gray-600 dark:bg-gray-700 shadow-sm"></div><div><label class="block text-sm font-medium text-gray-700 dark:text-gray-300">VAT Number</label><input type="text" value="4123456789" class="mt-1 block w-full rounded-md border-gray-300 dark:border-gray-600 dark:bg-gray-700 shadow-sm"></div></div></div><div class="flex justify-end"><button class="bg-blue-600 text-white px-6 py-2 rounded-lg hover:bg-blue-700 transition-colors"> Save Changes </button></div></div></div></div>`);
    };
  }
};
const _sfc_setup = _sfc_main.setup;
_sfc_main.setup = (props, ctx) => {
  const ssrContext = useSSRContext();
  (ssrContext.modules || (ssrContext.modules = /* @__PURE__ */ new Set())).add("pages/profile/index.vue");
  return _sfc_setup ? _sfc_setup(props, ctx) : void 0;
};
export {
  _sfc_main as default
};
//# sourceMappingURL=index--obkZLIu.js.map
