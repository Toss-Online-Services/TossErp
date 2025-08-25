import { ref, computed, readonly } from "vue";
import "C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/node_modules/hookable/dist/index.mjs";
import { b as useNuxtApp, e as defineStore, n as navigateTo } from "../server.mjs";
import { parse } from "C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/node_modules/nuxt/node_modules/cookie-es/dist/index.mjs";
import { getRequestHeader, setCookie, getCookie, deleteCookie } from "C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/node_modules/h3/dist/index.mjs";
import destr from "C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/node_modules/destr/dist/index.mjs";
import { isEqual } from "C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/node_modules/ohash/dist/index.mjs";
import { klona } from "C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/node_modules/klona/dist/index.mjs";
import "C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/node_modules/defu/dist/defu.mjs";
import "#internal/nuxt/paths";
function useRequestEvent(nuxtApp) {
  nuxtApp ||= useNuxtApp();
  return nuxtApp.ssrContext?.event;
}
const CookieDefaults = {
  path: "/",
  watch: true,
  decode: (val) => destr(decodeURIComponent(val)),
  encode: (val) => encodeURIComponent(typeof val === "string" ? val : JSON.stringify(val))
};
function useCookie(name, _opts) {
  const opts = { ...CookieDefaults, ..._opts };
  opts.filter ??= (key) => key === name;
  const cookies = readRawCookies(opts) || {};
  let delay;
  if (opts.maxAge !== void 0) {
    delay = opts.maxAge * 1e3;
  } else if (opts.expires) {
    delay = opts.expires.getTime() - Date.now();
  }
  const hasExpired = delay !== void 0 && delay <= 0;
  const cookieValue = klona(hasExpired ? void 0 : cookies[name] ?? opts.default?.());
  const cookie = ref(cookieValue);
  {
    const nuxtApp = useNuxtApp();
    const writeFinalCookieValue = () => {
      if (opts.readonly || isEqual(cookie.value, cookies[name])) {
        return;
      }
      nuxtApp._cookies ||= {};
      if (name in nuxtApp._cookies) {
        if (isEqual(cookie.value, nuxtApp._cookies[name])) {
          return;
        }
      }
      nuxtApp._cookies[name] = cookie.value;
      writeServerCookie(useRequestEvent(nuxtApp), name, cookie.value, opts);
    };
    const unhook = nuxtApp.hooks.hookOnce("app:rendered", writeFinalCookieValue);
    nuxtApp.hooks.hookOnce("app:error", () => {
      unhook();
      return writeFinalCookieValue();
    });
  }
  return cookie;
}
function readRawCookies(opts = {}) {
  {
    return parse(getRequestHeader(useRequestEvent(), "cookie") || "", opts);
  }
}
function writeServerCookie(event, name, value, opts = {}) {
  if (event) {
    if (value !== null && value !== void 0) {
      return setCookie(event, name, value, opts);
    }
    if (getCookie(event, name) !== void 0) {
      return deleteCookie(event, name, opts);
    }
  }
}
const useUserStore = defineStore("user", () => {
  const user = ref(null);
  const isAuthenticated = ref(false);
  const loading = ref(false);
  const permissions = ref([]);
  const fullName = computed(() => {
    if (!user.value) return "";
    return `${user.value.firstName} ${user.value.lastName}`;
  });
  const hasPermission = computed(() => (permission) => {
    return permissions.value.includes(permission) || permissions.value.includes("admin");
  });
  const userInitials = computed(() => {
    if (!user.value) return "U";
    const firstName = user.value.firstName?.[0] || "";
    const lastName = user.value.lastName?.[0] || "";
    return (firstName + lastName).toUpperCase();
  });
  async function login(credentials) {
    loading.value = true;
    try {
      const response = await $fetch("/api/auth/login", {
        method: "POST",
        body: credentials
      });
      user.value = response.user;
      isAuthenticated.value = true;
      permissions.value = response.permissions || [];
      if (response.token) {
        const token = useCookie("auth-token", {
          default: () => null,
          secure: true,
          sameSite: "strict",
          maxAge: 60 * 60 * 24 * 7
          // 7 days
        });
        token.value = response.token;
      }
      return response;
    } catch (error) {
      throw error;
    } finally {
      loading.value = false;
    }
  }
  async function logout() {
    loading.value = true;
    try {
      await $fetch("/api/auth/logout", {
        method: "POST"
      });
    } catch (error) {
      console.error("Logout error:", error);
    } finally {
      user.value = null;
      isAuthenticated.value = false;
      permissions.value = [];
      loading.value = false;
      const token = useCookie("auth-token");
      token.value = null;
      await navigateTo("/login");
    }
  }
  async function checkAuth() {
    const token = useCookie("auth-token");
    if (!token.value) {
      return false;
    }
    loading.value = true;
    try {
      const response = await $fetch("/api/auth/me");
      user.value = response.user;
      isAuthenticated.value = true;
      permissions.value = response.permissions || [];
      return true;
    } catch (error) {
      await logout();
      return false;
    } finally {
      loading.value = false;
    }
  }
  async function updateProfile(profileData) {
    loading.value = true;
    try {
      const response = await $fetch("/api/user/profile", {
        method: "PATCH",
        body: profileData
      });
      user.value = { ...user.value, ...response.user };
      return response;
    } catch (error) {
      throw error;
    } finally {
      loading.value = false;
    }
  }
  async function changePassword(passwords) {
    loading.value = true;
    try {
      const response = await $fetch("/api/user/change-password", {
        method: "POST",
        body: passwords
      });
      return response;
    } catch (error) {
      throw error;
    } finally {
      loading.value = false;
    }
  }
  return {
    // State
    user: readonly(user),
    isAuthenticated: readonly(isAuthenticated),
    loading: readonly(loading),
    permissions: readonly(permissions),
    // Getters
    fullName,
    hasPermission,
    userInitials,
    // Actions
    login,
    logout,
    checkAuth,
    updateProfile,
    changePassword
  };
});
export {
  useUserStore as u
};
//# sourceMappingURL=user-B2SCl6g9.js.map
