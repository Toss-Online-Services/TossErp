import { f as defineNuxtRouteMiddleware, n as navigateTo, g as createError } from './server.mjs';
import { u as useUserStore } from './user-B2SCl6g9.mjs';
import 'vue';
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
import 'vue/server-renderer';
import 'unhead/server';
import 'devalue';
import 'unhead/utils';
import 'unhead/plugins';
import 'vue-router';

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

export { auth as default };
//# sourceMappingURL=auth-bHeqSRVD.mjs.map
