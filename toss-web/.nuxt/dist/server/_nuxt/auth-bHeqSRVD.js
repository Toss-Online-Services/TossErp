import { f as defineNuxtRouteMiddleware, n as navigateTo, g as createError } from "../server.mjs";
import { u as useUserStore } from "./user-B2SCl6g9.js";
import "vue";
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
import "vue/server-renderer";
import "C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/node_modules/nuxt/node_modules/cookie-es/dist/index.mjs";
import "C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/node_modules/destr/dist/index.mjs";
import "C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/node_modules/ohash/dist/index.mjs";
const auth = defineNuxtRouteMiddleware((to) => {
  const userStore = useUserStore();
  if (!userStore.isAuthenticated) {
    return navigateTo("/login");
  }
  if (to.meta.requiresPermission) {
    const requiredPermission = to.meta.requiresPermission;
    if (!userStore.hasPermission.value(requiredPermission)) {
      throw createError({
        statusCode: 403,
        statusMessage: "Access denied. Insufficient permissions."
      });
    }
  }
});
export {
  auth as default
};
//# sourceMappingURL=auth-bHeqSRVD.js.map
