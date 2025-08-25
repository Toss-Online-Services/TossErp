import { ref, computed, readonly } from 'vue';
import { e as defineStore, n as navigateTo, b as useNuxtApp } from './server.mjs';
import { d as destr, F as klona, G as getRequestHeader, H as isEqual, f as setCookie, g as getCookie, I as deleteCookie } from '../_/nitro.mjs';

function parse(str, options) {
  if (typeof str !== "string") {
    throw new TypeError("argument str must be a string");
  }
  const obj = {};
  const opt = options || {};
  const dec = opt.decode || decode;
  let index = 0;
  while (index < str.length) {
    const eqIdx = str.indexOf("=", index);
    if (eqIdx === -1) {
      break;
    }
    let endIdx = str.indexOf(";", index);
    if (endIdx === -1) {
      endIdx = str.length;
    } else if (endIdx < eqIdx) {
      index = str.lastIndexOf(";", eqIdx - 1) + 1;
      continue;
    }
    const key = str.slice(index, eqIdx).trim();
    if (opt?.filter && !opt?.filter(key)) {
      index = endIdx + 1;
      continue;
    }
    if (void 0 === obj[key]) {
      let val = str.slice(eqIdx + 1, endIdx).trim();
      if (val.codePointAt(0) === 34) {
        val = val.slice(1, -1);
      }
      obj[key] = tryDecode(val, dec);
    }
    index = endIdx + 1;
  }
  return obj;
}
function decode(str) {
  return str.includes("%") ? decodeURIComponent(str) : str;
}
function tryDecode(str, decode2) {
  try {
    return decode2(str);
  } catch {
    return str;
  }
}

function useRequestEvent(nuxtApp) {
  var _a;
  nuxtApp || (nuxtApp = useNuxtApp());
  return (_a = nuxtApp.ssrContext) == null ? void 0 : _a.event;
}
const CookieDefaults = {
  path: "/",
  watch: true,
  decode: (val) => destr(decodeURIComponent(val)),
  encode: (val) => encodeURIComponent(typeof val === "string" ? val : JSON.stringify(val))
};
function useCookie(name, _opts) {
  var _a, _b, _c;
  const opts = { ...CookieDefaults, ..._opts };
  (_a = opts.filter) != null ? _a : opts.filter = (key) => key === name;
  const cookies = readRawCookies(opts) || {};
  let delay;
  if (opts.maxAge !== void 0) {
    delay = opts.maxAge * 1e3;
  } else if (opts.expires) {
    delay = opts.expires.getTime() - Date.now();
  }
  const hasExpired = delay !== void 0 && delay <= 0;
  const cookieValue = klona(hasExpired ? void 0 : (_c = cookies[name]) != null ? _c : (_b = opts.default) == null ? void 0 : _b.call(opts));
  const cookie = ref(cookieValue);
  {
    const nuxtApp = useNuxtApp();
    const writeFinalCookieValue = () => {
      if (opts.readonly || isEqual(cookie.value, cookies[name])) {
        return;
      }
      nuxtApp._cookies || (nuxtApp._cookies = {});
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
    var _a, _b;
    if (!user.value) return "U";
    const firstName = ((_a = user.value.firstName) == null ? void 0 : _a[0]) || "";
    const lastName = ((_b = user.value.lastName) == null ? void 0 : _b[0]) || "";
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

export { useUserStore as u };
//# sourceMappingURL=user-B2SCl6g9.mjs.map
