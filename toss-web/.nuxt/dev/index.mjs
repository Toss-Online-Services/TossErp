import process from 'node:process';globalThis._importMeta_={url:import.meta.url,env:process.env};import { tmpdir } from 'node:os';
import { Server } from 'node:http';
import { resolve, dirname, join } from 'node:path';
import nodeCrypto from 'node:crypto';
import { parentPort, threadId } from 'node:worker_threads';
import { defineEventHandler, handleCacheHeaders, splitCookiesString, createEvent, fetchWithEvent, isEvent, eventHandler, setHeaders, sendRedirect, proxyRequest, getRequestHeader, setResponseHeaders, setResponseStatus, send, getRequestHeaders, setResponseHeader, appendResponseHeader, getRequestURL, getResponseHeader, removeResponseHeader, createError, getQuery as getQuery$1, readBody, createApp, createRouter as createRouter$1, toNodeListener, lazyEventHandler, getResponseStatus, getRouterParam, getHeader, getCookie, setCookie, getMethod, getResponseStatusText } from 'file://C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/node_modules/h3/dist/index.mjs';
import { escapeHtml } from 'file://C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/node_modules/@vue/shared/dist/shared.cjs.js';
import { createRenderer, getRequestDependencies, getPreloadLinks, getPrefetchLinks } from 'file://C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/node_modules/vue-bundle-renderer/dist/runtime.mjs';
import { parseURL, withoutBase, joinURL, getQuery, withQuery, withTrailingSlash, decodePath, withLeadingSlash, withoutTrailingSlash, joinRelativeURL } from 'file://C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/node_modules/ufo/dist/index.mjs';
import { renderToString } from 'file://C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/node_modules/vue/server-renderer/index.mjs';
import { klona } from 'file://C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/node_modules/klona/dist/index.mjs';
import defu, { defuFn } from 'file://C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/node_modules/defu/dist/defu.mjs';
import destr, { destr as destr$1 } from 'file://C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/node_modules/destr/dist/index.mjs';
import { snakeCase } from 'file://C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/node_modules/scule/dist/index.mjs';
import { createHead as createHead$1, propsToString, renderSSRHead } from 'file://C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/node_modules/unhead/dist/server.mjs';
import { stringify, uneval } from 'file://C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/node_modules/devalue/index.js';
import { isVNode, toValue, isRef } from 'file://C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/node_modules/vue/index.mjs';
import { DeprecationsPlugin, PromisesPlugin, TemplateParamsPlugin, AliasSortingPlugin } from 'file://C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/node_modules/unhead/dist/plugins.mjs';
import { createHooks } from 'file://C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/node_modules/hookable/dist/index.mjs';
import { createFetch, Headers as Headers$1 } from 'file://C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/node_modules/ofetch/dist/node.mjs';
import { fetchNodeRequestHandler, callNodeRequestHandler } from 'file://C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/node_modules/node-mock-http/dist/index.mjs';
import { createStorage, prefixStorage } from 'file://C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/node_modules/unstorage/dist/index.mjs';
import unstorage_47drivers_47fs from 'file://C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/node_modules/unstorage/drivers/fs.mjs';
import { digest } from 'file://C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/node_modules/ohash/dist/index.mjs';
import { toRouteMatcher, createRouter } from 'file://C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/node_modules/radix3/dist/index.mjs';
import { readFile } from 'node:fs/promises';
import consola, { consola as consola$1 } from 'file://C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/node_modules/consola/dist/index.mjs';
import { ErrorParser } from 'file://C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/node_modules/youch-core/build/index.js';
import { Youch } from 'file://C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/node_modules/nitropack/node_modules/youch/build/index.js';
import { SourceMapConsumer } from 'file://C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/node_modules/source-map/source-map.js';
import { AsyncLocalStorage } from 'node:async_hooks';
import { getContext } from 'file://C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/node_modules/unctx/dist/index.mjs';
import { captureRawStackTrace, parseRawStackTrace } from 'file://C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/node_modules/errx/dist/index.js';
import { promises } from 'node:fs';
import { fileURLToPath } from 'node:url';
import { dirname as dirname$1, resolve as resolve$1 } from 'file://C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/node_modules/pathe/dist/index.mjs';
import { walkResolver } from 'file://C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/node_modules/unhead/dist/utils.mjs';

const serverAssets = [{"baseName":"server","dir":"C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/server/assets"}];

const assets$1 = createStorage();

for (const asset of serverAssets) {
  assets$1.mount(asset.baseName, unstorage_47drivers_47fs({ base: asset.dir, ignore: (asset?.ignore || []) }));
}

const storage = createStorage({});

storage.mount('/assets', assets$1);

storage.mount('root', unstorage_47drivers_47fs({"driver":"fs","readOnly":true,"base":"C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web","watchOptions":{"ignored":[null]}}));
storage.mount('src', unstorage_47drivers_47fs({"driver":"fs","readOnly":true,"base":"C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/server","watchOptions":{"ignored":[null]}}));
storage.mount('build', unstorage_47drivers_47fs({"driver":"fs","readOnly":false,"base":"C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/.nuxt"}));
storage.mount('cache', unstorage_47drivers_47fs({"driver":"fs","readOnly":false,"base":"C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/.nuxt/cache"}));
storage.mount('data', unstorage_47drivers_47fs({"driver":"fs","base":"C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/.data/kv"}));

function useStorage(base = "") {
  return base ? prefixStorage(storage, base) : storage;
}

const Hasher = /* @__PURE__ */ (() => {
  class Hasher2 {
    buff = "";
    #context = /* @__PURE__ */ new Map();
    write(str) {
      this.buff += str;
    }
    dispatch(value) {
      const type = value === null ? "null" : typeof value;
      return this[type](value);
    }
    object(object) {
      if (object && typeof object.toJSON === "function") {
        return this.object(object.toJSON());
      }
      const objString = Object.prototype.toString.call(object);
      let objType = "";
      const objectLength = objString.length;
      objType = objectLength < 10 ? "unknown:[" + objString + "]" : objString.slice(8, objectLength - 1);
      objType = objType.toLowerCase();
      let objectNumber = null;
      if ((objectNumber = this.#context.get(object)) === void 0) {
        this.#context.set(object, this.#context.size);
      } else {
        return this.dispatch("[CIRCULAR:" + objectNumber + "]");
      }
      if (typeof Buffer !== "undefined" && Buffer.isBuffer && Buffer.isBuffer(object)) {
        this.write("buffer:");
        return this.write(object.toString("utf8"));
      }
      if (objType !== "object" && objType !== "function" && objType !== "asyncfunction") {
        if (this[objType]) {
          this[objType](object);
        } else {
          this.unknown(object, objType);
        }
      } else {
        const keys = Object.keys(object).sort();
        const extraKeys = [];
        this.write("object:" + (keys.length + extraKeys.length) + ":");
        const dispatchForKey = (key) => {
          this.dispatch(key);
          this.write(":");
          this.dispatch(object[key]);
          this.write(",");
        };
        for (const key of keys) {
          dispatchForKey(key);
        }
        for (const key of extraKeys) {
          dispatchForKey(key);
        }
      }
    }
    array(arr, unordered) {
      unordered = unordered === void 0 ? false : unordered;
      this.write("array:" + arr.length + ":");
      if (!unordered || arr.length <= 1) {
        for (const entry of arr) {
          this.dispatch(entry);
        }
        return;
      }
      const contextAdditions = /* @__PURE__ */ new Map();
      const entries = arr.map((entry) => {
        const hasher = new Hasher2();
        hasher.dispatch(entry);
        for (const [key, value] of hasher.#context) {
          contextAdditions.set(key, value);
        }
        return hasher.toString();
      });
      this.#context = contextAdditions;
      entries.sort();
      return this.array(entries, false);
    }
    date(date) {
      return this.write("date:" + date.toJSON());
    }
    symbol(sym) {
      return this.write("symbol:" + sym.toString());
    }
    unknown(value, type) {
      this.write(type);
      if (!value) {
        return;
      }
      this.write(":");
      if (value && typeof value.entries === "function") {
        return this.array(
          [...value.entries()],
          true
          /* ordered */
        );
      }
    }
    error(err) {
      return this.write("error:" + err.toString());
    }
    boolean(bool) {
      return this.write("bool:" + bool);
    }
    string(string) {
      this.write("string:" + string.length + ":");
      this.write(string);
    }
    function(fn) {
      this.write("fn:");
      if (isNativeFunction(fn)) {
        this.dispatch("[native]");
      } else {
        this.dispatch(fn.toString());
      }
    }
    number(number) {
      return this.write("number:" + number);
    }
    null() {
      return this.write("Null");
    }
    undefined() {
      return this.write("Undefined");
    }
    regexp(regex) {
      return this.write("regex:" + regex.toString());
    }
    arraybuffer(arr) {
      this.write("arraybuffer:");
      return this.dispatch(new Uint8Array(arr));
    }
    url(url) {
      return this.write("url:" + url.toString());
    }
    map(map) {
      this.write("map:");
      const arr = [...map];
      return this.array(arr, false);
    }
    set(set) {
      this.write("set:");
      const arr = [...set];
      return this.array(arr, false);
    }
    bigint(number) {
      return this.write("bigint:" + number.toString());
    }
  }
  for (const type of [
    "uint8array",
    "uint8clampedarray",
    "unt8array",
    "uint16array",
    "unt16array",
    "uint32array",
    "unt32array",
    "float32array",
    "float64array"
  ]) {
    Hasher2.prototype[type] = function(arr) {
      this.write(type + ":");
      return this.array([...arr], false);
    };
  }
  function isNativeFunction(f) {
    if (typeof f !== "function") {
      return false;
    }
    return Function.prototype.toString.call(f).slice(
      -15
      /* "[native code] }".length */
    ) === "[native code] }";
  }
  return Hasher2;
})();
function serialize(object) {
  const hasher = new Hasher();
  hasher.dispatch(object);
  return hasher.buff;
}
function hash(value) {
  return digest(typeof value === "string" ? value : serialize(value)).replace(/[-_]/g, "").slice(0, 10);
}

function defaultCacheOptions() {
  return {
    name: "_",
    base: "/cache",
    swr: true,
    maxAge: 1
  };
}
function defineCachedFunction(fn, opts = {}) {
  opts = { ...defaultCacheOptions(), ...opts };
  const pending = {};
  const group = opts.group || "nitro/functions";
  const name = opts.name || fn.name || "_";
  const integrity = opts.integrity || hash([fn, opts]);
  const validate = opts.validate || ((entry) => entry.value !== void 0);
  async function get(key, resolver, shouldInvalidateCache, event) {
    const cacheKey = [opts.base, group, name, key + ".json"].filter(Boolean).join(":").replace(/:\/$/, ":index");
    let entry = await useStorage().getItem(cacheKey).catch((error) => {
      console.error(`[cache] Cache read error.`, error);
      useNitroApp().captureError(error, { event, tags: ["cache"] });
    }) || {};
    if (typeof entry !== "object") {
      entry = {};
      const error = new Error("Malformed data read from cache.");
      console.error("[cache]", error);
      useNitroApp().captureError(error, { event, tags: ["cache"] });
    }
    const ttl = (opts.maxAge ?? 0) * 1e3;
    if (ttl) {
      entry.expires = Date.now() + ttl;
    }
    const expired = shouldInvalidateCache || entry.integrity !== integrity || ttl && Date.now() - (entry.mtime || 0) > ttl || validate(entry) === false;
    const _resolve = async () => {
      const isPending = pending[key];
      if (!isPending) {
        if (entry.value !== void 0 && (opts.staleMaxAge || 0) >= 0 && opts.swr === false) {
          entry.value = void 0;
          entry.integrity = void 0;
          entry.mtime = void 0;
          entry.expires = void 0;
        }
        pending[key] = Promise.resolve(resolver());
      }
      try {
        entry.value = await pending[key];
      } catch (error) {
        if (!isPending) {
          delete pending[key];
        }
        throw error;
      }
      if (!isPending) {
        entry.mtime = Date.now();
        entry.integrity = integrity;
        delete pending[key];
        if (validate(entry) !== false) {
          let setOpts;
          if (opts.maxAge && !opts.swr) {
            setOpts = { ttl: opts.maxAge };
          }
          const promise = useStorage().setItem(cacheKey, entry, setOpts).catch((error) => {
            console.error(`[cache] Cache write error.`, error);
            useNitroApp().captureError(error, { event, tags: ["cache"] });
          });
          if (event?.waitUntil) {
            event.waitUntil(promise);
          }
        }
      }
    };
    const _resolvePromise = expired ? _resolve() : Promise.resolve();
    if (entry.value === void 0) {
      await _resolvePromise;
    } else if (expired && event && event.waitUntil) {
      event.waitUntil(_resolvePromise);
    }
    if (opts.swr && validate(entry) !== false) {
      _resolvePromise.catch((error) => {
        console.error(`[cache] SWR handler error.`, error);
        useNitroApp().captureError(error, { event, tags: ["cache"] });
      });
      return entry;
    }
    return _resolvePromise.then(() => entry);
  }
  return async (...args) => {
    const shouldBypassCache = await opts.shouldBypassCache?.(...args);
    if (shouldBypassCache) {
      return fn(...args);
    }
    const key = await (opts.getKey || getKey)(...args);
    const shouldInvalidateCache = await opts.shouldInvalidateCache?.(...args);
    const entry = await get(
      key,
      () => fn(...args),
      shouldInvalidateCache,
      args[0] && isEvent(args[0]) ? args[0] : void 0
    );
    let value = entry.value;
    if (opts.transform) {
      value = await opts.transform(entry, ...args) || value;
    }
    return value;
  };
}
function cachedFunction(fn, opts = {}) {
  return defineCachedFunction(fn, opts);
}
function getKey(...args) {
  return args.length > 0 ? hash(args) : "";
}
function escapeKey(key) {
  return String(key).replace(/\W/g, "");
}
function defineCachedEventHandler(handler, opts = defaultCacheOptions()) {
  const variableHeaderNames = (opts.varies || []).filter(Boolean).map((h) => h.toLowerCase()).sort();
  const _opts = {
    ...opts,
    getKey: async (event) => {
      const customKey = await opts.getKey?.(event);
      if (customKey) {
        return escapeKey(customKey);
      }
      const _path = event.node.req.originalUrl || event.node.req.url || event.path;
      let _pathname;
      try {
        _pathname = escapeKey(decodeURI(parseURL(_path).pathname)).slice(0, 16) || "index";
      } catch {
        _pathname = "-";
      }
      const _hashedPath = `${_pathname}.${hash(_path)}`;
      const _headers = variableHeaderNames.map((header) => [header, event.node.req.headers[header]]).map(([name, value]) => `${escapeKey(name)}.${hash(value)}`);
      return [_hashedPath, ..._headers].join(":");
    },
    validate: (entry) => {
      if (!entry.value) {
        return false;
      }
      if (entry.value.code >= 400) {
        return false;
      }
      if (entry.value.body === void 0) {
        return false;
      }
      if (entry.value.headers.etag === "undefined" || entry.value.headers["last-modified"] === "undefined") {
        return false;
      }
      return true;
    },
    group: opts.group || "nitro/handlers",
    integrity: opts.integrity || hash([handler, opts])
  };
  const _cachedHandler = cachedFunction(
    async (incomingEvent) => {
      const variableHeaders = {};
      for (const header of variableHeaderNames) {
        const value = incomingEvent.node.req.headers[header];
        if (value !== void 0) {
          variableHeaders[header] = value;
        }
      }
      const reqProxy = cloneWithProxy(incomingEvent.node.req, {
        headers: variableHeaders
      });
      const resHeaders = {};
      let _resSendBody;
      const resProxy = cloneWithProxy(incomingEvent.node.res, {
        statusCode: 200,
        writableEnded: false,
        writableFinished: false,
        headersSent: false,
        closed: false,
        getHeader(name) {
          return resHeaders[name];
        },
        setHeader(name, value) {
          resHeaders[name] = value;
          return this;
        },
        getHeaderNames() {
          return Object.keys(resHeaders);
        },
        hasHeader(name) {
          return name in resHeaders;
        },
        removeHeader(name) {
          delete resHeaders[name];
        },
        getHeaders() {
          return resHeaders;
        },
        end(chunk, arg2, arg3) {
          if (typeof chunk === "string") {
            _resSendBody = chunk;
          }
          if (typeof arg2 === "function") {
            arg2();
          }
          if (typeof arg3 === "function") {
            arg3();
          }
          return this;
        },
        write(chunk, arg2, arg3) {
          if (typeof chunk === "string") {
            _resSendBody = chunk;
          }
          if (typeof arg2 === "function") {
            arg2(void 0);
          }
          if (typeof arg3 === "function") {
            arg3();
          }
          return true;
        },
        writeHead(statusCode, headers2) {
          this.statusCode = statusCode;
          if (headers2) {
            if (Array.isArray(headers2) || typeof headers2 === "string") {
              throw new TypeError("Raw headers  is not supported.");
            }
            for (const header in headers2) {
              const value = headers2[header];
              if (value !== void 0) {
                this.setHeader(
                  header,
                  value
                );
              }
            }
          }
          return this;
        }
      });
      const event = createEvent(reqProxy, resProxy);
      event.fetch = (url, fetchOptions) => fetchWithEvent(event, url, fetchOptions, {
        fetch: useNitroApp().localFetch
      });
      event.$fetch = (url, fetchOptions) => fetchWithEvent(event, url, fetchOptions, {
        fetch: globalThis.$fetch
      });
      event.waitUntil = incomingEvent.waitUntil;
      event.context = incomingEvent.context;
      event.context.cache = {
        options: _opts
      };
      const body = await handler(event) || _resSendBody;
      const headers = event.node.res.getHeaders();
      headers.etag = String(
        headers.Etag || headers.etag || `W/"${hash(body)}"`
      );
      headers["last-modified"] = String(
        headers["Last-Modified"] || headers["last-modified"] || (/* @__PURE__ */ new Date()).toUTCString()
      );
      const cacheControl = [];
      if (opts.swr) {
        if (opts.maxAge) {
          cacheControl.push(`s-maxage=${opts.maxAge}`);
        }
        if (opts.staleMaxAge) {
          cacheControl.push(`stale-while-revalidate=${opts.staleMaxAge}`);
        } else {
          cacheControl.push("stale-while-revalidate");
        }
      } else if (opts.maxAge) {
        cacheControl.push(`max-age=${opts.maxAge}`);
      }
      if (cacheControl.length > 0) {
        headers["cache-control"] = cacheControl.join(", ");
      }
      const cacheEntry = {
        code: event.node.res.statusCode,
        headers,
        body
      };
      return cacheEntry;
    },
    _opts
  );
  return defineEventHandler(async (event) => {
    if (opts.headersOnly) {
      if (handleCacheHeaders(event, { maxAge: opts.maxAge })) {
        return;
      }
      return handler(event);
    }
    const response = await _cachedHandler(
      event
    );
    if (event.node.res.headersSent || event.node.res.writableEnded) {
      return response.body;
    }
    if (handleCacheHeaders(event, {
      modifiedTime: new Date(response.headers["last-modified"]),
      etag: response.headers.etag,
      maxAge: opts.maxAge
    })) {
      return;
    }
    event.node.res.statusCode = response.code;
    for (const name in response.headers) {
      const value = response.headers[name];
      if (name === "set-cookie") {
        event.node.res.appendHeader(
          name,
          splitCookiesString(value)
        );
      } else {
        if (value !== void 0) {
          event.node.res.setHeader(name, value);
        }
      }
    }
    return response.body;
  });
}
function cloneWithProxy(obj, overrides) {
  return new Proxy(obj, {
    get(target, property, receiver) {
      if (property in overrides) {
        return overrides[property];
      }
      return Reflect.get(target, property, receiver);
    },
    set(target, property, value, receiver) {
      if (property in overrides) {
        overrides[property] = value;
        return true;
      }
      return Reflect.set(target, property, value, receiver);
    }
  });
}
const cachedEventHandler = defineCachedEventHandler;

const inlineAppConfig = {
  "nuxt": {}
};



const appConfig = defuFn(inlineAppConfig);

function getEnv(key, opts) {
  const envKey = snakeCase(key).toUpperCase();
  return destr(
    process.env[opts.prefix + envKey] ?? process.env[opts.altPrefix + envKey]
  );
}
function _isObject(input) {
  return typeof input === "object" && !Array.isArray(input);
}
function applyEnv(obj, opts, parentKey = "") {
  for (const key in obj) {
    const subKey = parentKey ? `${parentKey}_${key}` : key;
    const envValue = getEnv(subKey, opts);
    if (_isObject(obj[key])) {
      if (_isObject(envValue)) {
        obj[key] = { ...obj[key], ...envValue };
        applyEnv(obj[key], opts, subKey);
      } else if (envValue === void 0) {
        applyEnv(obj[key], opts, subKey);
      } else {
        obj[key] = envValue ?? obj[key];
      }
    } else {
      obj[key] = envValue ?? obj[key];
    }
    if (opts.envExpansion && typeof obj[key] === "string") {
      obj[key] = _expandFromEnv(obj[key]);
    }
  }
  return obj;
}
const envExpandRx = /\{\{([^{}]*)\}\}/g;
function _expandFromEnv(value) {
  return value.replace(envExpandRx, (match, key) => {
    return process.env[key] || match;
  });
}

const _inlineRuntimeConfig = {
  "app": {
    "baseURL": "/",
    "buildId": "dev",
    "buildAssetsDir": "/_nuxt/",
    "cdnURL": ""
  },
  "nitro": {
    "envPrefix": "NUXT_",
    "routeRules": {
      "/__nuxt_error": {
        "cache": false
      },
      "/_nuxt/builds/meta/**": {
        "headers": {
          "cache-control": "public, max-age=31536000, immutable"
        }
      },
      "/_nuxt/builds/**": {
        "headers": {
          "cache-control": "public, max-age=1, immutable"
        }
      }
    }
  },
  "public": {
    "apiBase": "/api"
  },
  "apiSecret": ""
};
const envOptions = {
  prefix: "NITRO_",
  altPrefix: _inlineRuntimeConfig.nitro.envPrefix ?? process.env.NITRO_ENV_PREFIX ?? "_",
  envExpansion: _inlineRuntimeConfig.nitro.envExpansion ?? process.env.NITRO_ENV_EXPANSION ?? false
};
const _sharedRuntimeConfig = _deepFreeze(
  applyEnv(klona(_inlineRuntimeConfig), envOptions)
);
function useRuntimeConfig(event) {
  if (!event) {
    return _sharedRuntimeConfig;
  }
  if (event.context.nitro.runtimeConfig) {
    return event.context.nitro.runtimeConfig;
  }
  const runtimeConfig = klona(_inlineRuntimeConfig);
  applyEnv(runtimeConfig, envOptions);
  event.context.nitro.runtimeConfig = runtimeConfig;
  return runtimeConfig;
}
_deepFreeze(klona(appConfig));
function _deepFreeze(object) {
  const propNames = Object.getOwnPropertyNames(object);
  for (const name of propNames) {
    const value = object[name];
    if (value && typeof value === "object") {
      _deepFreeze(value);
    }
  }
  return Object.freeze(object);
}
new Proxy(/* @__PURE__ */ Object.create(null), {
  get: (_, prop) => {
    console.warn(
      "Please use `useRuntimeConfig()` instead of accessing config directly."
    );
    const runtimeConfig = useRuntimeConfig();
    if (prop in runtimeConfig) {
      return runtimeConfig[prop];
    }
    return void 0;
  }
});

const config = useRuntimeConfig();
const _routeRulesMatcher = toRouteMatcher(
  createRouter({ routes: config.nitro.routeRules })
);
function createRouteRulesHandler(ctx) {
  return eventHandler((event) => {
    const routeRules = getRouteRules(event);
    if (routeRules.headers) {
      setHeaders(event, routeRules.headers);
    }
    if (routeRules.redirect) {
      let target = routeRules.redirect.to;
      if (target.endsWith("/**")) {
        let targetPath = event.path;
        const strpBase = routeRules.redirect._redirectStripBase;
        if (strpBase) {
          targetPath = withoutBase(targetPath, strpBase);
        }
        target = joinURL(target.slice(0, -3), targetPath);
      } else if (event.path.includes("?")) {
        const query = getQuery(event.path);
        target = withQuery(target, query);
      }
      return sendRedirect(event, target, routeRules.redirect.statusCode);
    }
    if (routeRules.proxy) {
      let target = routeRules.proxy.to;
      if (target.endsWith("/**")) {
        let targetPath = event.path;
        const strpBase = routeRules.proxy._proxyStripBase;
        if (strpBase) {
          targetPath = withoutBase(targetPath, strpBase);
        }
        target = joinURL(target.slice(0, -3), targetPath);
      } else if (event.path.includes("?")) {
        const query = getQuery(event.path);
        target = withQuery(target, query);
      }
      return proxyRequest(event, target, {
        fetch: ctx.localFetch,
        ...routeRules.proxy
      });
    }
  });
}
function getRouteRules(event) {
  event.context._nitro = event.context._nitro || {};
  if (!event.context._nitro.routeRules) {
    event.context._nitro.routeRules = getRouteRulesForPath(
      withoutBase(event.path.split("?")[0], useRuntimeConfig().app.baseURL)
    );
  }
  return event.context._nitro.routeRules;
}
function getRouteRulesForPath(path) {
  return defu({}, ..._routeRulesMatcher.matchAll(path).reverse());
}

function _captureError(error, type) {
  console.error(`[${type}]`, error);
  useNitroApp().captureError(error, { tags: [type] });
}
function trapUnhandledNodeErrors() {
  process.on(
    "unhandledRejection",
    (error) => _captureError(error, "unhandledRejection")
  );
  process.on(
    "uncaughtException",
    (error) => _captureError(error, "uncaughtException")
  );
}
function joinHeaders(value) {
  return Array.isArray(value) ? value.join(", ") : String(value);
}
function normalizeFetchResponse(response) {
  if (!response.headers.has("set-cookie")) {
    return response;
  }
  return new Response(response.body, {
    status: response.status,
    statusText: response.statusText,
    headers: normalizeCookieHeaders(response.headers)
  });
}
function normalizeCookieHeader(header = "") {
  return splitCookiesString(joinHeaders(header));
}
function normalizeCookieHeaders(headers) {
  const outgoingHeaders = new Headers();
  for (const [name, header] of headers) {
    if (name === "set-cookie") {
      for (const cookie of normalizeCookieHeader(header)) {
        outgoingHeaders.append("set-cookie", cookie);
      }
    } else {
      outgoingHeaders.set(name, joinHeaders(header));
    }
  }
  return outgoingHeaders;
}

function isJsonRequest(event) {
  if (hasReqHeader(event, "accept", "text/html")) {
    return false;
  }
  return hasReqHeader(event, "accept", "application/json") || hasReqHeader(event, "user-agent", "curl/") || hasReqHeader(event, "user-agent", "httpie/") || hasReqHeader(event, "sec-fetch-mode", "cors") || event.path.startsWith("/api/") || event.path.endsWith(".json");
}
function hasReqHeader(event, name, includes) {
  const value = getRequestHeader(event, name);
  return value && typeof value === "string" && value.toLowerCase().includes(includes);
}

const errorHandler$0 = (async function errorhandler(error, event, { defaultHandler }) {
  if (event.handled || isJsonRequest(event)) {
    return;
  }
  const defaultRes = await defaultHandler(error, event, { json: true });
  const statusCode = error.statusCode || 500;
  if (statusCode === 404 && defaultRes.status === 302) {
    setResponseHeaders(event, defaultRes.headers);
    setResponseStatus(event, defaultRes.status, defaultRes.statusText);
    return send(event, JSON.stringify(defaultRes.body, null, 2));
  }
  if (typeof defaultRes.body !== "string" && Array.isArray(defaultRes.body.stack)) {
    defaultRes.body.stack = defaultRes.body.stack.join("\n");
  }
  const errorObject = defaultRes.body;
  const url = new URL(errorObject.url);
  errorObject.url = withoutBase(url.pathname, useRuntimeConfig(event).app.baseURL) + url.search + url.hash;
  errorObject.message ||= "Server Error";
  errorObject.data ||= error.data;
  errorObject.statusMessage ||= error.statusMessage;
  delete defaultRes.headers["content-type"];
  delete defaultRes.headers["content-security-policy"];
  setResponseHeaders(event, defaultRes.headers);
  const reqHeaders = getRequestHeaders(event);
  const isRenderingError = event.path.startsWith("/__nuxt_error") || !!reqHeaders["x-nuxt-error"];
  const res = isRenderingError ? null : await useNitroApp().localFetch(
    withQuery(joinURL(useRuntimeConfig(event).app.baseURL, "/__nuxt_error"), errorObject),
    {
      headers: { ...reqHeaders, "x-nuxt-error": "true" },
      redirect: "manual"
    }
  ).catch(() => null);
  if (event.handled) {
    return;
  }
  if (!res) {
    const { template } = await Promise.resolve().then(function () { return errorDev; }) ;
    {
      errorObject.description = errorObject.message;
    }
    setResponseHeader(event, "Content-Type", "text/html;charset=UTF-8");
    return send(event, template(errorObject));
  }
  const html = await res.text();
  for (const [header, value] of res.headers.entries()) {
    if (header === "set-cookie") {
      appendResponseHeader(event, header, value);
      continue;
    }
    setResponseHeader(event, header, value);
  }
  setResponseStatus(event, res.status && res.status !== 200 ? res.status : defaultRes.status, res.statusText || defaultRes.statusText);
  return send(event, html);
});

function defineNitroErrorHandler(handler) {
  return handler;
}

const errorHandler$1 = defineNitroErrorHandler(
  async function defaultNitroErrorHandler(error, event) {
    const res = await defaultHandler(error, event);
    if (!event.node?.res.headersSent) {
      setResponseHeaders(event, res.headers);
    }
    setResponseStatus(event, res.status, res.statusText);
    return send(
      event,
      typeof res.body === "string" ? res.body : JSON.stringify(res.body, null, 2)
    );
  }
);
async function defaultHandler(error, event, opts) {
  const isSensitive = error.unhandled || error.fatal;
  const statusCode = error.statusCode || 500;
  const statusMessage = error.statusMessage || "Server Error";
  const url = getRequestURL(event, { xForwardedHost: true, xForwardedProto: true });
  if (statusCode === 404) {
    const baseURL = "/";
    if (/^\/[^/]/.test(baseURL) && !url.pathname.startsWith(baseURL)) {
      const redirectTo = `${baseURL}${url.pathname.slice(1)}${url.search}`;
      return {
        status: 302,
        statusText: "Found",
        headers: { location: redirectTo },
        body: `Redirecting...`
      };
    }
  }
  await loadStackTrace(error).catch(consola.error);
  const youch = new Youch();
  if (isSensitive && !opts?.silent) {
    const tags = [error.unhandled && "[unhandled]", error.fatal && "[fatal]"].filter(Boolean).join(" ");
    const ansiError = await (await youch.toANSI(error)).replaceAll(process.cwd(), ".");
    consola.error(
      `[request error] ${tags} [${event.method}] ${url}

`,
      ansiError
    );
  }
  const useJSON = opts?.json || !getRequestHeader(event, "accept")?.includes("text/html");
  const headers = {
    "content-type": useJSON ? "application/json" : "text/html",
    // Prevent browser from guessing the MIME types of resources.
    "x-content-type-options": "nosniff",
    // Prevent error page from being embedded in an iframe
    "x-frame-options": "DENY",
    // Prevent browsers from sending the Referer header
    "referrer-policy": "no-referrer",
    // Disable the execution of any js
    "content-security-policy": "script-src 'self' 'unsafe-inline'; object-src 'none'; base-uri 'self';"
  };
  if (statusCode === 404 || !getResponseHeader(event, "cache-control")) {
    headers["cache-control"] = "no-cache";
  }
  const body = useJSON ? {
    error: true,
    url,
    statusCode,
    statusMessage,
    message: error.message,
    data: error.data,
    stack: error.stack?.split("\n").map((line) => line.trim())
  } : await youch.toHTML(error, {
    request: {
      url: url.href,
      method: event.method,
      headers: getRequestHeaders(event)
    }
  });
  return {
    status: statusCode,
    statusText: statusMessage,
    headers,
    body
  };
}
async function loadStackTrace(error) {
  if (!(error instanceof Error)) {
    return;
  }
  const parsed = await new ErrorParser().defineSourceLoader(sourceLoader).parse(error);
  const stack = error.message + "\n" + parsed.frames.map((frame) => fmtFrame(frame)).join("\n");
  Object.defineProperty(error, "stack", { value: stack });
  if (error.cause) {
    await loadStackTrace(error.cause).catch(consola.error);
  }
}
async function sourceLoader(frame) {
  if (!frame.fileName || frame.fileType !== "fs" || frame.type === "native") {
    return;
  }
  if (frame.type === "app") {
    const rawSourceMap = await readFile(`${frame.fileName}.map`, "utf8").catch(() => {
    });
    if (rawSourceMap) {
      const consumer = await new SourceMapConsumer(rawSourceMap);
      const originalPosition = consumer.originalPositionFor({ line: frame.lineNumber, column: frame.columnNumber });
      if (originalPosition.source && originalPosition.line) {
        frame.fileName = resolve(dirname(frame.fileName), originalPosition.source);
        frame.lineNumber = originalPosition.line;
        frame.columnNumber = originalPosition.column || 0;
      }
    }
  }
  const contents = await readFile(frame.fileName, "utf8").catch(() => {
  });
  return contents ? { contents } : void 0;
}
function fmtFrame(frame) {
  if (frame.type === "native") {
    return frame.raw;
  }
  const src = `${frame.fileName || ""}:${frame.lineNumber}:${frame.columnNumber})`;
  return frame.functionName ? `at ${frame.functionName} (${src}` : `at ${src}`;
}

const errorHandlers = [errorHandler$0, errorHandler$1];

async function errorHandler(error, event) {
  for (const handler of errorHandlers) {
    try {
      await handler(error, event, { defaultHandler });
      if (event.handled) {
        return; // Response handled
      }
    } catch(error) {
      // Handler itself thrown, log and continue
      console.error(error);
    }
  }
  // H3 will handle fallback
}

const script$1 = `
if (!window.__NUXT_DEVTOOLS_TIME_METRIC__) {
  Object.defineProperty(window, '__NUXT_DEVTOOLS_TIME_METRIC__', {
    value: {},
    enumerable: false,
    configurable: true,
  })
}
window.__NUXT_DEVTOOLS_TIME_METRIC__.appInit = Date.now()
`;

const _7qpQRfKT8qr0TzYqctsNO4maeK5v4ZoupIrE2s4Lk = (function(nitro) {
  nitro.hooks.hook("render:html", (htmlContext) => {
    htmlContext.head.push(`<script>${script$1}<\/script>`);
  });
});

const rootDir = "C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web";

const appHead = {"meta":[{"charset":"utf-8"},{"name":"viewport","content":"width=device-width, initial-scale=1"},{"name":"description","content":"TOSS ERP III - AI-powered collaborative business platform for South African SMMEs"}],"link":[{"rel":"icon","type":"image/x-icon","href":"/favicon.ico"}],"style":[],"script":[],"noscript":[],"title":"TOSS ERP III - Township One-Stop Solution"};

const appRootTag = "div";

const appRootAttrs = {"id":"__nuxt"};

const appTeleportTag = "div";

const appTeleportAttrs = {"id":"teleports"};

const appId = "nuxt-app";

const devReducers = {
  VNode: (data) => isVNode(data) ? { type: data.type, props: data.props } : void 0,
  URL: (data) => data instanceof URL ? data.toString() : void 0
};
const asyncContext = getContext("nuxt-dev", { asyncContext: true, AsyncLocalStorage });
const _Bofk4Vy6MniwTqHdd_79UbkchYI2GKSdaOhr1Xyco = (nitroApp) => {
  const handler = nitroApp.h3App.handler;
  nitroApp.h3App.handler = (event) => {
    return asyncContext.callAsync({ logs: [], event }, () => handler(event));
  };
  onConsoleLog((_log) => {
    const ctx = asyncContext.tryUse();
    if (!ctx) {
      return;
    }
    const rawStack = captureRawStackTrace();
    if (!rawStack || rawStack.includes("runtime/vite-node.mjs")) {
      return;
    }
    const trace = [];
    let filename = "";
    for (const entry of parseRawStackTrace(rawStack)) {
      if (entry.source === globalThis._importMeta_.url) {
        continue;
      }
      if (EXCLUDE_TRACE_RE.test(entry.source)) {
        continue;
      }
      filename ||= entry.source.replace(withTrailingSlash(rootDir), "");
      trace.push({
        ...entry,
        source: entry.source.startsWith("file://") ? entry.source.replace("file://", "") : entry.source
      });
    }
    const log = {
      ..._log,
      // Pass along filename to allow the client to display more info about where log comes from
      filename,
      // Clean up file names in stack trace
      stack: trace
    };
    ctx.logs.push(log);
  });
  nitroApp.hooks.hook("afterResponse", () => {
    const ctx = asyncContext.tryUse();
    if (!ctx) {
      return;
    }
    return nitroApp.hooks.callHook("dev:ssr-logs", { logs: ctx.logs, path: ctx.event.path });
  });
  nitroApp.hooks.hook("render:html", (htmlContext) => {
    const ctx = asyncContext.tryUse();
    if (!ctx) {
      return;
    }
    try {
      const reducers = Object.assign(/* @__PURE__ */ Object.create(null), devReducers, ctx.event.context._payloadReducers);
      htmlContext.bodyAppend.unshift(`<script type="application/json" data-nuxt-logs="${appId}">${stringify(ctx.logs, reducers)}<\/script>`);
    } catch (e) {
      const shortError = e instanceof Error && "toString" in e ? ` Received \`${e.toString()}\`.` : "";
      console.warn(`[nuxt] Failed to stringify dev server logs.${shortError} You can define your own reducer/reviver for rich types following the instructions in https://nuxt.com/docs/api/composables/use-nuxt-app#payload.`);
    }
  });
};
const EXCLUDE_TRACE_RE = /\/node_modules\/(?:.*\/)?(?:nuxt|nuxt-nightly|nuxt-edge|nuxt3|consola|@vue)\/|core\/runtime\/nitro/;
function onConsoleLog(callback) {
  consola$1.addReporter({
    log(logObj) {
      callback(logObj);
    }
  });
  consola$1.wrapConsole();
}

const script = "\"use strict\";(()=>{const t=window,e=document.documentElement,c=[\"dark\",\"light\"],n=getStorageValue(\"localStorage\",\"nuxt-color-mode\")||\"system\";let i=n===\"system\"?u():n;const r=e.getAttribute(\"data-color-mode-forced\");r&&(i=r),l(i),t[\"__NUXT_COLOR_MODE__\"]={preference:n,value:i,getColorScheme:u,addColorScheme:l,removeColorScheme:d};function l(o){const s=\"\"+o+\"\",a=\"\";e.classList?e.classList.add(s):e.className+=\" \"+s,a&&e.setAttribute(\"data-\"+a,o)}function d(o){const s=\"\"+o+\"\",a=\"\";e.classList?e.classList.remove(s):e.className=e.className.replace(new RegExp(s,\"g\"),\"\"),a&&e.removeAttribute(\"data-\"+a)}function f(o){return t.matchMedia(\"(prefers-color-scheme\"+o+\")\")}function u(){if(t.matchMedia&&f(\"\").media!==\"not all\"){for(const o of c)if(f(\":\"+o).matches)return o}return\"light\"}})();function getStorageValue(t,e){switch(t){case\"localStorage\":return window.localStorage.getItem(e);case\"sessionStorage\":return window.sessionStorage.getItem(e);case\"cookie\":return getCookie(e);default:return null}}function getCookie(t){const c=(\"; \"+window.document.cookie).split(\"; \"+t+\"=\");if(c.length===2)return c.pop()?.split(\";\").shift()}";

const _UVjoGPPKFNMtJDOvIYNdIOwWRmPnOEZPfoLKZHdd8VY = (function(nitro) {
  nitro.hooks.hook("render:html", (htmlContext) => {
    htmlContext.head.push(`<script>${script}<\/script>`);
  });
});

const plugins = [
  _7qpQRfKT8qr0TzYqctsNO4maeK5v4ZoupIrE2s4Lk,
_Bofk4Vy6MniwTqHdd_79UbkchYI2GKSdaOhr1Xyco,
_UVjoGPPKFNMtJDOvIYNdIOwWRmPnOEZPfoLKZHdd8VY
];

const assets = {};

function readAsset (id) {
  const serverDir = dirname$1(fileURLToPath(globalThis._importMeta_.url));
  return promises.readFile(resolve$1(serverDir, assets[id].path))
}

const publicAssetBases = {"/_nuxt/builds/meta/":{"maxAge":31536000},"/_nuxt/builds/":{"maxAge":1}};

function isPublicAssetURL(id = '') {
  if (assets[id]) {
    return true
  }
  for (const base in publicAssetBases) {
    if (id.startsWith(base)) { return true }
  }
  return false
}

function getAsset (id) {
  return assets[id]
}

const METHODS = /* @__PURE__ */ new Set(["HEAD", "GET"]);
const EncodingMap = { gzip: ".gz", br: ".br" };
const _11TZDX = eventHandler((event) => {
  if (event.method && !METHODS.has(event.method)) {
    return;
  }
  let id = decodePath(
    withLeadingSlash(withoutTrailingSlash(parseURL(event.path).pathname))
  );
  let asset;
  const encodingHeader = String(
    getRequestHeader(event, "accept-encoding") || ""
  );
  const encodings = [
    ...encodingHeader.split(",").map((e) => EncodingMap[e.trim()]).filter(Boolean).sort(),
    ""
  ];
  if (encodings.length > 1) {
    appendResponseHeader(event, "Vary", "Accept-Encoding");
  }
  for (const encoding of encodings) {
    for (const _id of [id + encoding, joinURL(id, "index.html" + encoding)]) {
      const _asset = getAsset(_id);
      if (_asset) {
        asset = _asset;
        id = _id;
        break;
      }
    }
  }
  if (!asset) {
    if (isPublicAssetURL(id)) {
      removeResponseHeader(event, "Cache-Control");
      throw createError({ statusCode: 404 });
    }
    return;
  }
  const ifNotMatch = getRequestHeader(event, "if-none-match") === asset.etag;
  if (ifNotMatch) {
    setResponseStatus(event, 304, "Not Modified");
    return "";
  }
  const ifModifiedSinceH = getRequestHeader(event, "if-modified-since");
  const mtimeDate = new Date(asset.mtime);
  if (ifModifiedSinceH && asset.mtime && new Date(ifModifiedSinceH) >= mtimeDate) {
    setResponseStatus(event, 304, "Not Modified");
    return "";
  }
  if (asset.type && !getResponseHeader(event, "Content-Type")) {
    setResponseHeader(event, "Content-Type", asset.type);
  }
  if (asset.etag && !getResponseHeader(event, "ETag")) {
    setResponseHeader(event, "ETag", asset.etag);
  }
  if (asset.mtime && !getResponseHeader(event, "Last-Modified")) {
    setResponseHeader(event, "Last-Modified", mtimeDate.toUTCString());
  }
  if (asset.encoding && !getResponseHeader(event, "Content-Encoding")) {
    setResponseHeader(event, "Content-Encoding", asset.encoding);
  }
  if (asset.size > 0 && !getResponseHeader(event, "Content-Length")) {
    setResponseHeader(event, "Content-Length", asset.size);
  }
  return readAsset(id);
});

const VueResolver = (_, value) => {
  return isRef(value) ? toValue(value) : value;
};

const headSymbol = "usehead";
function vueInstall(head) {
  const plugin = {
    install(app) {
      app.config.globalProperties.$unhead = head;
      app.config.globalProperties.$head = head;
      app.provide(headSymbol, head);
    }
  };
  return plugin.install;
}

function resolveUnrefHeadInput(input) {
  return walkResolver(input, VueResolver);
}

function createHead(options = {}) {
  const head = createHead$1({
    ...options,
    propResolvers: [VueResolver]
  });
  head.install = vueInstall(head);
  return head;
}

const unheadOptions = {
  disableDefaults: true,
  disableCapoSorting: false,
  plugins: [DeprecationsPlugin, PromisesPlugin, TemplateParamsPlugin, AliasSortingPlugin],
};

function createSSRContext(event) {
  const ssrContext = {
    url: event.path,
    event,
    runtimeConfig: useRuntimeConfig(event),
    noSSR: event.context.nuxt?.noSSR || (false),
    head: createHead(unheadOptions),
    error: false,
    nuxt: void 0,
    /* NuxtApp */
    payload: {},
    _payloadReducers: /* @__PURE__ */ Object.create(null),
    modules: /* @__PURE__ */ new Set()
  };
  return ssrContext;
}
function setSSRError(ssrContext, error) {
  ssrContext.error = true;
  ssrContext.payload = { error };
  ssrContext.url = error.url;
}

function buildAssetsDir() {
  return useRuntimeConfig().app.buildAssetsDir;
}
function buildAssetsURL(...path) {
  return joinRelativeURL(publicAssetsURL(), buildAssetsDir(), ...path);
}
function publicAssetsURL(...path) {
  const app = useRuntimeConfig().app;
  const publicBase = app.cdnURL || app.baseURL;
  return path.length ? joinRelativeURL(publicBase, ...path) : publicBase;
}

const APP_ROOT_OPEN_TAG = `<${appRootTag}${propsToString(appRootAttrs)}>`;
const APP_ROOT_CLOSE_TAG = `</${appRootTag}>`;
const getServerEntry = () => import('file://C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/.nuxt//dist/server/server.mjs').then((r) => r.default || r);
const getClientManifest = () => import('file://C:/Users/PROBOOK/source/repos/Toss-Online-Services/TossErp/toss-web/.nuxt//dist/server/client.manifest.mjs').then((r) => r.default || r).then((r) => typeof r === "function" ? r() : r);
const getSSRRenderer = lazyCachedFunction(async () => {
  const manifest = await getClientManifest();
  if (!manifest) {
    throw new Error("client.manifest is not available");
  }
  const createSSRApp = await getServerEntry();
  if (!createSSRApp) {
    throw new Error("Server bundle is not available");
  }
  const options = {
    manifest,
    renderToString: renderToString$1,
    buildAssetsURL
  };
  const renderer = createRenderer(createSSRApp, options);
  async function renderToString$1(input, context) {
    const html = await renderToString(input, context);
    if (process.env.NUXT_VITE_NODE_OPTIONS) {
      renderer.rendererContext.updateManifest(await getClientManifest());
    }
    return APP_ROOT_OPEN_TAG + html + APP_ROOT_CLOSE_TAG;
  }
  return renderer;
});
const getSPARenderer = lazyCachedFunction(async () => {
  const manifest = await getClientManifest();
  const spaTemplate = await Promise.resolve().then(function () { return _virtual__spaTemplate; }).then((r) => r.template).catch(() => "").then((r) => {
    {
      return APP_ROOT_OPEN_TAG + r + APP_ROOT_CLOSE_TAG;
    }
  });
  const options = {
    manifest,
    renderToString: () => spaTemplate,
    buildAssetsURL
  };
  const renderer = createRenderer(() => () => {
  }, options);
  const result = await renderer.renderToString({});
  const renderToString = (ssrContext) => {
    const config = useRuntimeConfig(ssrContext.event);
    ssrContext.modules ||= /* @__PURE__ */ new Set();
    ssrContext.payload.serverRendered = false;
    ssrContext.config = {
      public: config.public,
      app: config.app
    };
    return Promise.resolve(result);
  };
  return {
    rendererContext: renderer.rendererContext,
    renderToString
  };
});
function lazyCachedFunction(fn) {
  let res = null;
  return () => {
    if (res === null) {
      res = fn().catch((err) => {
        res = null;
        throw err;
      });
    }
    return res;
  };
}
function getRenderer(ssrContext) {
  return ssrContext.noSSR ? getSPARenderer() : getSSRRenderer();
}
const getSSRStyles = lazyCachedFunction(() => Promise.resolve().then(function () { return styles$1; }).then((r) => r.default || r));

async function renderInlineStyles(usedModules) {
  const styleMap = await getSSRStyles();
  const inlinedStyles = /* @__PURE__ */ new Set();
  for (const mod of usedModules) {
    if (mod in styleMap && styleMap[mod]) {
      for (const style of await styleMap[mod]()) {
        inlinedStyles.add(style);
      }
    }
  }
  return Array.from(inlinedStyles).map((style) => ({ innerHTML: style }));
}

const ROOT_NODE_REGEX = new RegExp(`^<${appRootTag}[^>]*>([\\s\\S]*)<\\/${appRootTag}>$`);
function getServerComponentHTML(body) {
  const match = body.match(ROOT_NODE_REGEX);
  return match?.[1] || body;
}
const SSR_SLOT_TELEPORT_MARKER = /^uid=([^;]*);slot=(.*)$/;
const SSR_CLIENT_TELEPORT_MARKER = /^uid=([^;]*);client=(.*)$/;
const SSR_CLIENT_SLOT_MARKER = /^island-slot=([^;]*);(.*)$/;
function getSlotIslandResponse(ssrContext) {
  if (!ssrContext.islandContext || !Object.keys(ssrContext.islandContext.slots).length) {
    return void 0;
  }
  const response = {};
  for (const [name, slot] of Object.entries(ssrContext.islandContext.slots)) {
    response[name] = {
      ...slot,
      fallback: ssrContext.teleports?.[`island-fallback=${name}`]
    };
  }
  return response;
}
function getClientIslandResponse(ssrContext) {
  if (!ssrContext.islandContext || !Object.keys(ssrContext.islandContext.components).length) {
    return void 0;
  }
  const response = {};
  for (const [clientUid, component] of Object.entries(ssrContext.islandContext.components)) {
    const html = ssrContext.teleports?.[clientUid]?.replaceAll("<!--teleport start anchor-->", "") || "";
    response[clientUid] = {
      ...component,
      html,
      slots: getComponentSlotTeleport(clientUid, ssrContext.teleports ?? {})
    };
  }
  return response;
}
function getComponentSlotTeleport(clientUid, teleports) {
  const entries = Object.entries(teleports);
  const slots = {};
  for (const [key, value] of entries) {
    const match = key.match(SSR_CLIENT_SLOT_MARKER);
    if (match) {
      const [, id, slot] = match;
      if (!slot || clientUid !== id) {
        continue;
      }
      slots[slot] = value;
    }
  }
  return slots;
}
function replaceIslandTeleports(ssrContext, html) {
  const { teleports, islandContext } = ssrContext;
  if (islandContext || !teleports) {
    return html;
  }
  for (const key in teleports) {
    const matchClientComp = key.match(SSR_CLIENT_TELEPORT_MARKER);
    if (matchClientComp) {
      const [, uid, clientId] = matchClientComp;
      if (!uid || !clientId) {
        continue;
      }
      html = html.replace(new RegExp(` data-island-uid="${uid}" data-island-component="${clientId}"[^>]*>`), (full) => {
        return full + teleports[key];
      });
      continue;
    }
    const matchSlot = key.match(SSR_SLOT_TELEPORT_MARKER);
    if (matchSlot) {
      const [, uid, slot] = matchSlot;
      if (!uid || !slot) {
        continue;
      }
      html = html.replace(new RegExp(` data-island-uid="${uid}" data-island-slot="${slot}"[^>]*>`), (full) => {
        return full + teleports[key];
      });
    }
  }
  return html;
}

const ISLAND_SUFFIX_RE = /\.json(\?.*)?$/;
const _SxA8c9 = defineEventHandler(async (event) => {
  const nitroApp = useNitroApp();
  setResponseHeaders(event, {
    "content-type": "application/json;charset=utf-8",
    "x-powered-by": "Nuxt"
  });
  const islandContext = await getIslandContext(event);
  const ssrContext = {
    ...createSSRContext(event),
    islandContext,
    noSSR: false,
    url: islandContext.url
  };
  const renderer = await getSSRRenderer();
  const renderResult = await renderer.renderToString(ssrContext).catch(async (error) => {
    await ssrContext.nuxt?.hooks.callHook("app:error", error);
    throw error;
  });
  const inlinedStyles = await renderInlineStyles(ssrContext.modules ?? []);
  await ssrContext.nuxt?.hooks.callHook("app:rendered", { ssrContext, renderResult });
  if (inlinedStyles.length) {
    ssrContext.head.push({ style: inlinedStyles });
  }
  {
    const { styles } = getRequestDependencies(ssrContext, renderer.rendererContext);
    const link = [];
    for (const resource of Object.values(styles)) {
      if ("inline" in getQuery(resource.file)) {
        continue;
      }
      if (resource.file.includes("scoped") && !resource.file.includes("pages/")) {
        link.push({ rel: "stylesheet", href: renderer.rendererContext.buildAssetsURL(resource.file), crossorigin: "" });
      }
    }
    if (link.length) {
      ssrContext.head.push({ link }, { mode: "server" });
    }
  }
  const islandHead = {};
  for (const entry of ssrContext.head.entries.values()) {
    for (const [key, value] of Object.entries(resolveUnrefHeadInput(entry.input))) {
      const currentValue = islandHead[key];
      if (Array.isArray(currentValue)) {
        currentValue.push(...value);
      }
      islandHead[key] = value;
    }
  }
  islandHead.link ||= [];
  islandHead.style ||= [];
  const islandResponse = {
    id: islandContext.id,
    head: islandHead,
    html: getServerComponentHTML(renderResult.html),
    components: getClientIslandResponse(ssrContext),
    slots: getSlotIslandResponse(ssrContext)
  };
  await nitroApp.hooks.callHook("render:island", islandResponse, { event, islandContext });
  return islandResponse;
});
async function getIslandContext(event) {
  let url = event.path || "";
  const componentParts = url.substring("/__nuxt_island".length + 1).replace(ISLAND_SUFFIX_RE, "").split("_");
  const hashId = componentParts.length > 1 ? componentParts.pop() : void 0;
  const componentName = componentParts.join("_");
  const context = event.method === "GET" ? getQuery$1(event) : await readBody(event);
  const ctx = {
    url: "/",
    ...context,
    id: hashId,
    name: componentName,
    props: destr$1(context.props) || {},
    slots: {},
    components: {}
  };
  return ctx;
}

const _lazy_O04_7U = () => Promise.resolve().then(function () { return chat_post$1; });
const _lazy_OdYviq = () => Promise.resolve().then(function () { return dashboard_get$1; });
const _lazy_ldjkMJ = () => Promise.resolve().then(function () { return login_post$1; });
const _lazy_3QfYhG = () => Promise.resolve().then(function () { return logout_post$1; });
const _lazy_uboXFf = () => Promise.resolve().then(function () { return me_get$1; });
const _lazy_dm0Ylo = () => Promise.resolve().then(function () { return register_post$1; });
const _lazy_qeG9EY = () => Promise.resolve().then(function () { return index_get$h; });
const _lazy_IgFCMQ = () => Promise.resolve().then(function () { return send_post$1; });
const _lazy_718C4M = () => Promise.resolve().then(function () { return index_get$f; });
const _lazy_sKfWU7 = () => Promise.resolve().then(function () { return outcomes_get$1; });
const _lazy_l7k4G1 = () => Promise.resolve().then(function () { return tenant_get$1; });
const _lazy_0z3Ghx = () => Promise.resolve().then(function () { return index_get$d; });
const _lazy_7MY2_F = () => Promise.resolve().then(function () { return join_post$1; });
const _lazy_aZiMCu = () => Promise.resolve().then(function () { return health_get$1; });
const _lazy_eIs7Yh = () => Promise.resolve().then(function () { return _id__get$1; });
const _lazy_B5uv1e = () => Promise.resolve().then(function () { return index_get$b; });
const _lazy_dU7IbP = () => Promise.resolve().then(function () { return index_post$5; });
const _lazy_u2VPpy = () => Promise.resolve().then(function () { return movements_post$1; });
const _lazy_YDU8mt = () => Promise.resolve().then(function () { return index_get$9; });
const _lazy_kB_XQ1 = () => Promise.resolve().then(function () { return _trackingNumber__get$1; });
const _lazy_8bkcHO = () => Promise.resolve().then(function () { return index_get$7; });
const _lazy_gDSLee = () => Promise.resolve().then(function () { return index_get$5; });
const _lazy_wdvhe5 = () => Promise.resolve().then(function () { return markRead_post$1; });
const _lazy_Ccj4Nb = () => Promise.resolve().then(function () { return setup_post$1; });
const _lazy_0PiByv = () => Promise.resolve().then(function () { return index_get$3; });
const _lazy_5rkuZ3 = () => Promise.resolve().then(function () { return index_post$3; });
const _lazy_ZmJxTy = () => Promise.resolve().then(function () { return index_get$1; });
const _lazy_Ck1Klp = () => Promise.resolve().then(function () { return index_post$1; });
const _lazy_APX_Vx = () => Promise.resolve().then(function () { return analytics_get$1; });
const _lazy_1ndFHV = () => Promise.resolve().then(function () { return catalog_get$1; });
const _lazy_NKkQw_ = () => Promise.resolve().then(function () { return execute_post$1; });
const _lazy_TIycte = () => Promise.resolve().then(function () { return manage_post$1; });
const _lazy_qSSj97 = () => Promise.resolve().then(function () { return renderer$1; });

const handlers = [
  { route: '', handler: _11TZDX, lazy: false, middleware: true, method: undefined },
  { route: '/api/ai/chat', handler: _lazy_O04_7U, lazy: true, middleware: false, method: "post" },
  { route: '/api/analytics/dashboard', handler: _lazy_OdYviq, lazy: true, middleware: false, method: "get" },
  { route: '/api/auth/login', handler: _lazy_ldjkMJ, lazy: true, middleware: false, method: "post" },
  { route: '/api/auth/logout', handler: _lazy_3QfYhG, lazy: true, middleware: false, method: "post" },
  { route: '/api/auth/me', handler: _lazy_uboXFf, lazy: true, middleware: false, method: "get" },
  { route: '/api/auth/register', handler: _lazy_dm0Ylo, lazy: true, middleware: false, method: "post" },
  { route: '/api/communication/conversations', handler: _lazy_qeG9EY, lazy: true, middleware: false, method: "get" },
  { route: '/api/communication/messages/send', handler: _lazy_IgFCMQ, lazy: true, middleware: false, method: "post" },
  { route: '/api/customers', handler: _lazy_718C4M, lazy: true, middleware: false, method: "get" },
  { route: '/api/dashboard/outcomes', handler: _lazy_sKfWU7, lazy: true, middleware: false, method: "get" },
  { route: '/api/debug/tenant', handler: _lazy_l7k4G1, lazy: true, middleware: false, method: "get" },
  { route: '/api/group-buying', handler: _lazy_0z3Ghx, lazy: true, middleware: false, method: "get" },
  { route: '/api/group-buying/join', handler: _lazy_7MY2_F, lazy: true, middleware: false, method: "post" },
  { route: '/api/health', handler: _lazy_aZiMCu, lazy: true, middleware: false, method: "get" },
  { route: '/api/inventory/:id', handler: _lazy_eIs7Yh, lazy: true, middleware: false, method: "get" },
  { route: '/api/inventory', handler: _lazy_B5uv1e, lazy: true, middleware: false, method: "get" },
  { route: '/api/inventory', handler: _lazy_dU7IbP, lazy: true, middleware: false, method: "post" },
  { route: '/api/inventory/movements', handler: _lazy_u2VPpy, lazy: true, middleware: false, method: "post" },
  { route: '/api/logistics', handler: _lazy_YDU8mt, lazy: true, middleware: false, method: "get" },
  { route: '/api/logistics/track/:trackingNumber', handler: _lazy_kB_XQ1, lazy: true, middleware: false, method: "get" },
  { route: '/api/manufacturing/production-orders', handler: _lazy_8bkcHO, lazy: true, middleware: false, method: "get" },
  { route: '/api/notifications', handler: _lazy_gDSLee, lazy: true, middleware: false, method: "get" },
  { route: '/api/notifications/mark-read', handler: _lazy_wdvhe5, lazy: true, middleware: false, method: "post" },
  { route: '/api/onboarding/setup', handler: _lazy_Ccj4Nb, lazy: true, middleware: false, method: "post" },
  { route: '/api/projects', handler: _lazy_0PiByv, lazy: true, middleware: false, method: "get" },
  { route: '/api/projects', handler: _lazy_5rkuZ3, lazy: true, middleware: false, method: "post" },
  { route: '/api/sales', handler: _lazy_ZmJxTy, lazy: true, middleware: false, method: "get" },
  { route: '/api/sales', handler: _lazy_Ck1Klp, lazy: true, middleware: false, method: "post" },
  { route: '/api/services/analytics', handler: _lazy_APX_Vx, lazy: true, middleware: false, method: "get" },
  { route: '/api/services/catalog', handler: _lazy_1ndFHV, lazy: true, middleware: false, method: "get" },
  { route: '/api/services/execute', handler: _lazy_NKkQw_, lazy: true, middleware: false, method: "post" },
  { route: '/api/services/manage', handler: _lazy_TIycte, lazy: true, middleware: false, method: "post" },
  { route: '/__nuxt_error', handler: _lazy_qSSj97, lazy: true, middleware: false, method: undefined },
  { route: '/__nuxt_island/**', handler: _SxA8c9, lazy: false, middleware: false, method: undefined },
  { route: '/**', handler: _lazy_qSSj97, lazy: true, middleware: false, method: undefined }
];

function createNitroApp() {
  const config = useRuntimeConfig();
  const hooks = createHooks();
  const captureError = (error, context = {}) => {
    const promise = hooks.callHookParallel("error", error, context).catch((error_) => {
      console.error("Error while capturing another error", error_);
    });
    if (context.event && isEvent(context.event)) {
      const errors = context.event.context.nitro?.errors;
      if (errors) {
        errors.push({ error, context });
      }
      if (context.event.waitUntil) {
        context.event.waitUntil(promise);
      }
    }
  };
  const h3App = createApp({
    debug: destr(true),
    onError: (error, event) => {
      captureError(error, { event, tags: ["request"] });
      return errorHandler(error, event);
    },
    onRequest: async (event) => {
      event.context.nitro = event.context.nitro || { errors: [] };
      const fetchContext = event.node.req?.__unenv__;
      if (fetchContext?._platform) {
        event.context = {
          _platform: fetchContext?._platform,
          // #3335
          ...fetchContext._platform,
          ...event.context
        };
      }
      if (!event.context.waitUntil && fetchContext?.waitUntil) {
        event.context.waitUntil = fetchContext.waitUntil;
      }
      event.fetch = (req, init) => fetchWithEvent(event, req, init, { fetch: localFetch });
      event.$fetch = (req, init) => fetchWithEvent(event, req, init, {
        fetch: $fetch
      });
      event.waitUntil = (promise) => {
        if (!event.context.nitro._waitUntilPromises) {
          event.context.nitro._waitUntilPromises = [];
        }
        event.context.nitro._waitUntilPromises.push(promise);
        if (event.context.waitUntil) {
          event.context.waitUntil(promise);
        }
      };
      event.captureError = (error, context) => {
        captureError(error, { event, ...context });
      };
      await nitroApp$1.hooks.callHook("request", event).catch((error) => {
        captureError(error, { event, tags: ["request"] });
      });
    },
    onBeforeResponse: async (event, response) => {
      await nitroApp$1.hooks.callHook("beforeResponse", event, response).catch((error) => {
        captureError(error, { event, tags: ["request", "response"] });
      });
    },
    onAfterResponse: async (event, response) => {
      await nitroApp$1.hooks.callHook("afterResponse", event, response).catch((error) => {
        captureError(error, { event, tags: ["request", "response"] });
      });
    }
  });
  const router = createRouter$1({
    preemptive: true
  });
  const nodeHandler = toNodeListener(h3App);
  const localCall = (aRequest) => callNodeRequestHandler(nodeHandler, aRequest);
  const localFetch = (input, init) => {
    if (!input.toString().startsWith("/")) {
      return globalThis.fetch(input, init);
    }
    return fetchNodeRequestHandler(
      nodeHandler,
      input,
      init
    ).then((response) => normalizeFetchResponse(response));
  };
  const $fetch = createFetch({
    fetch: localFetch,
    Headers: Headers$1,
    defaults: { baseURL: config.app.baseURL }
  });
  globalThis.$fetch = $fetch;
  h3App.use(createRouteRulesHandler({ localFetch }));
  for (const h of handlers) {
    let handler = h.lazy ? lazyEventHandler(h.handler) : h.handler;
    if (h.middleware || !h.route) {
      const middlewareBase = (config.app.baseURL + (h.route || "/")).replace(
        /\/+/g,
        "/"
      );
      h3App.use(middlewareBase, handler);
    } else {
      const routeRules = getRouteRulesForPath(
        h.route.replace(/:\w+|\*\*/g, "_")
      );
      if (routeRules.cache) {
        handler = cachedEventHandler(handler, {
          group: "nitro/routes",
          ...routeRules.cache
        });
      }
      router.use(h.route, handler, h.method);
    }
  }
  h3App.use(config.app.baseURL, router.handler);
  const app = {
    hooks,
    h3App,
    router,
    localCall,
    localFetch,
    captureError
  };
  return app;
}
function runNitroPlugins(nitroApp2) {
  for (const plugin of plugins) {
    try {
      plugin(nitroApp2);
    } catch (error) {
      nitroApp2.captureError(error, { tags: ["plugin"] });
      throw error;
    }
  }
}
const nitroApp$1 = createNitroApp();
function useNitroApp() {
  return nitroApp$1;
}
runNitroPlugins(nitroApp$1);

function defineRenderHandler(render) {
  const runtimeConfig = useRuntimeConfig();
  return eventHandler(async (event) => {
    const nitroApp = useNitroApp();
    const ctx = { event, render, response: void 0 };
    await nitroApp.hooks.callHook("render:before", ctx);
    if (!ctx.response) {
      if (event.path === `${runtimeConfig.app.baseURL}favicon.ico`) {
        setResponseHeader(event, "Content-Type", "image/x-icon");
        return send(
          event,
          "data:image/gif;base64,R0lGODlhAQABAIAAAAAAAP///yH5BAEAAAAALAAAAAABAAEAAAIBRAA7"
        );
      }
      ctx.response = await ctx.render(event);
      if (!ctx.response) {
        const _currentStatus = getResponseStatus(event);
        setResponseStatus(event, _currentStatus === 200 ? 500 : _currentStatus);
        return send(
          event,
          "No response returned from render handler: " + event.path
        );
      }
    }
    await nitroApp.hooks.callHook("render:response", ctx.response, ctx);
    if (ctx.response.headers) {
      setResponseHeaders(event, ctx.response.headers);
    }
    if (ctx.response.statusCode || ctx.response.statusMessage) {
      setResponseStatus(
        event,
        ctx.response.statusCode,
        ctx.response.statusMessage
      );
    }
    return ctx.response.body;
  });
}

const scheduledTasks = false;

const tasks = {
  
};

const __runningTasks__ = {};
async function runTask(name, {
  payload = {},
  context = {}
} = {}) {
  if (__runningTasks__[name]) {
    return __runningTasks__[name];
  }
  if (!(name in tasks)) {
    throw createError({
      message: `Task \`${name}\` is not available!`,
      statusCode: 404
    });
  }
  if (!tasks[name].resolve) {
    throw createError({
      message: `Task \`${name}\` is not implemented!`,
      statusCode: 501
    });
  }
  const handler = await tasks[name].resolve();
  const taskEvent = { name, payload, context };
  __runningTasks__[name] = handler.run(taskEvent);
  try {
    const res = await __runningTasks__[name];
    return res;
  } finally {
    delete __runningTasks__[name];
  }
}

if (!globalThis.crypto) {
  globalThis.crypto = nodeCrypto;
}
const { NITRO_NO_UNIX_SOCKET, NITRO_DEV_WORKER_ID } = process.env;
trapUnhandledNodeErrors();
parentPort?.on("message", (msg) => {
  if (msg && msg.event === "shutdown") {
    shutdown();
  }
});
const nitroApp = useNitroApp();
const server = new Server(toNodeListener(nitroApp.h3App));
let listener;
listen().catch(() => listen(
  true
  /* use random port */
)).catch((error) => {
  console.error("Dev worker failed to listen:", error);
  return shutdown();
});
nitroApp.router.get(
  "/_nitro/tasks",
  defineEventHandler(async (event) => {
    const _tasks = await Promise.all(
      Object.entries(tasks).map(async ([name, task]) => {
        const _task = await task.resolve?.();
        return [name, { description: _task?.meta?.description }];
      })
    );
    return {
      tasks: Object.fromEntries(_tasks),
      scheduledTasks
    };
  })
);
nitroApp.router.use(
  "/_nitro/tasks/:name",
  defineEventHandler(async (event) => {
    const name = getRouterParam(event, "name");
    const payload = {
      ...getQuery$1(event),
      ...await readBody(event).then((r) => r?.payload).catch(() => ({}))
    };
    return await runTask(name, { payload });
  })
);
function listen(useRandomPort = Boolean(
  NITRO_NO_UNIX_SOCKET || process.versions.webcontainer || "Bun" in globalThis && process.platform === "win32"
)) {
  return new Promise((resolve, reject) => {
    try {
      listener = server.listen(useRandomPort ? 0 : getSocketAddress(), () => {
        const address = server.address();
        parentPort?.postMessage({
          event: "listen",
          address: typeof address === "string" ? { socketPath: address } : { host: "localhost", port: address?.port }
        });
        resolve();
      });
    } catch (error) {
      reject(error);
    }
  });
}
function getSocketAddress() {
  const socketName = `nitro-worker-${process.pid}-${threadId}-${NITRO_DEV_WORKER_ID}-${Math.round(Math.random() * 1e4)}.sock`;
  if (process.platform === "win32") {
    return join(String.raw`\\.\pipe`, socketName);
  }
  if (process.platform === "linux") {
    const nodeMajor = Number.parseInt(process.versions.node.split(".")[0], 10);
    if (nodeMajor >= 20) {
      return `\0${socketName}`;
    }
  }
  return join(tmpdir(), socketName);
}
async function shutdown() {
  server.closeAllConnections?.();
  await Promise.all([
    new Promise((resolve) => listener?.close(resolve)),
    nitroApp.hooks.callHook("close").catch(console.error)
  ]);
  parentPort?.postMessage({ event: "exit" });
}

const _messages = { "appName": "Nuxt", "version": "", "statusCode": 500, "statusMessage": "Server error", "description": "An error occurred in the application and the page could not be served. If you are the application owner, check your server logs for details.", "stack": "" };
const template$1 = (messages) => {
  messages = { ..._messages, ...messages };
  return '<!DOCTYPE html><html lang="en"><head><title>' + escapeHtml(messages.statusCode) + " - " + escapeHtml(messages.statusMessage || "Internal Server Error") + `</title><meta charset="utf-8"><meta content="width=device-width,initial-scale=1.0,minimum-scale=1.0" name="viewport"><style>.spotlight{background:linear-gradient(45deg,#00dc82,#36e4da 50%,#0047e1);bottom:-40vh;filter:blur(30vh);height:60vh;opacity:.8}*,:after,:before{border-color:var(--un-default-border-color,#e5e7eb);border-style:solid;border-width:0;box-sizing:border-box}:after,:before{--un-content:""}html{line-height:1.5;-webkit-text-size-adjust:100%;font-family:ui-sans-serif,system-ui,sans-serif,Apple Color Emoji,Segoe UI Emoji,Segoe UI Symbol,Noto Color Emoji;font-feature-settings:normal;font-variation-settings:normal;-moz-tab-size:4;tab-size:4;-webkit-tap-highlight-color:transparent}body{line-height:inherit;margin:0}h1{font-size:inherit;font-weight:inherit}h1,p{margin:0}*,:after,:before{--un-rotate:0;--un-rotate-x:0;--un-rotate-y:0;--un-rotate-z:0;--un-scale-x:1;--un-scale-y:1;--un-scale-z:1;--un-skew-x:0;--un-skew-y:0;--un-translate-x:0;--un-translate-y:0;--un-translate-z:0;--un-pan-x: ;--un-pan-y: ;--un-pinch-zoom: ;--un-scroll-snap-strictness:proximity;--un-ordinal: ;--un-slashed-zero: ;--un-numeric-figure: ;--un-numeric-spacing: ;--un-numeric-fraction: ;--un-border-spacing-x:0;--un-border-spacing-y:0;--un-ring-offset-shadow:0 0 transparent;--un-ring-shadow:0 0 transparent;--un-shadow-inset: ;--un-shadow:0 0 transparent;--un-ring-inset: ;--un-ring-offset-width:0px;--un-ring-offset-color:#fff;--un-ring-width:0px;--un-ring-color:rgba(147,197,253,.5);--un-blur: ;--un-brightness: ;--un-contrast: ;--un-drop-shadow: ;--un-grayscale: ;--un-hue-rotate: ;--un-invert: ;--un-saturate: ;--un-sepia: ;--un-backdrop-blur: ;--un-backdrop-brightness: ;--un-backdrop-contrast: ;--un-backdrop-grayscale: ;--un-backdrop-hue-rotate: ;--un-backdrop-invert: ;--un-backdrop-opacity: ;--un-backdrop-saturate: ;--un-backdrop-sepia: }.pointer-events-none{pointer-events:none}.fixed{position:fixed}.left-0{left:0}.right-0{right:0}.z-10{z-index:10}.mb-6{margin-bottom:1.5rem}.mb-8{margin-bottom:2rem}.h-auto{height:auto}.min-h-screen{min-height:100vh}.flex{display:flex}.flex-1{flex:1 1 0%}.flex-col{flex-direction:column}.overflow-y-auto{overflow-y:auto}.rounded-t-md{border-top-left-radius:.375rem;border-top-right-radius:.375rem}.bg-black\\/5{background-color:#0000000d}.bg-white{--un-bg-opacity:1;background-color:rgb(255 255 255/var(--un-bg-opacity))}.p-8{padding:2rem}.px-10{padding-left:2.5rem;padding-right:2.5rem}.pt-14{padding-top:3.5rem}.text-6xl{font-size:3.75rem;line-height:1}.text-xl{font-size:1.25rem;line-height:1.75rem}.text-black{--un-text-opacity:1;color:rgb(0 0 0/var(--un-text-opacity))}.font-light{font-weight:300}.font-medium{font-weight:500}.leading-tight{line-height:1.25}.font-sans{font-family:ui-sans-serif,system-ui,-apple-system,BlinkMacSystemFont,Segoe UI,Roboto,Helvetica Neue,Arial,Noto Sans,sans-serif,Apple Color Emoji,Segoe UI Emoji,Segoe UI Symbol,Noto Color Emoji}.antialiased{-webkit-font-smoothing:antialiased;-moz-osx-font-smoothing:grayscale}@media (prefers-color-scheme:dark){.dark\\:bg-black{--un-bg-opacity:1;background-color:rgb(0 0 0/var(--un-bg-opacity))}.dark\\:bg-white\\/10{background-color:#ffffff1a}.dark\\:text-white{--un-text-opacity:1;color:rgb(255 255 255/var(--un-text-opacity))}}@media (min-width:640px){.sm\\:text-2xl{font-size:1.5rem;line-height:2rem}.sm\\:text-8xl{font-size:6rem;line-height:1}}</style><script>!function(){const e=document.createElement("link").relList;if(!(e&&e.supports&&e.supports("modulepreload"))){for(const e of document.querySelectorAll('link[rel="modulepreload"]'))r(e);new MutationObserver((e=>{for(const o of e)if("childList"===o.type)for(const e of o.addedNodes)"LINK"===e.tagName&&"modulepreload"===e.rel&&r(e)})).observe(document,{childList:!0,subtree:!0})}function r(e){if(e.ep)return;e.ep=!0;const r=function(e){const r={};return e.integrity&&(r.integrity=e.integrity),e.referrerPolicy&&(r.referrerPolicy=e.referrerPolicy),"use-credentials"===e.crossOrigin?r.credentials="include":"anonymous"===e.crossOrigin?r.credentials="omit":r.credentials="same-origin",r}(e);fetch(e.href,r)}}();<\/script></head><body class="antialiased bg-white dark:bg-black dark:text-white flex flex-col font-sans min-h-screen pt-14 px-10 text-black"><div class="fixed left-0 pointer-events-none right-0 spotlight"></div><h1 class="font-medium mb-6 sm:text-8xl text-6xl">` + escapeHtml(messages.statusCode) + '</h1><p class="font-light leading-tight mb-8 sm:text-2xl text-xl">' + escapeHtml(messages.description) + '</p><div class="bg-black/5 bg-white dark:bg-white/10 flex-1 h-auto overflow-y-auto rounded-t-md"><div class="font-light leading-tight p-8 text-xl z-10">' + escapeHtml(messages.stack) + "</div></div></body></html>";
};

const errorDev = /*#__PURE__*/Object.freeze(/*#__PURE__*/Object.defineProperty({
  __proto__: null,
  template: template$1
}, Symbol.toStringTag, { value: 'Module' }));

const template = "";

const _virtual__spaTemplate = /*#__PURE__*/Object.freeze(/*#__PURE__*/Object.defineProperty({
  __proto__: null,
  template: template
}, Symbol.toStringTag, { value: 'Module' }));

const styles = {};

const styles$1 = /*#__PURE__*/Object.freeze(/*#__PURE__*/Object.defineProperty({
  __proto__: null,
  default: styles
}, Symbol.toStringTag, { value: 'Module' }));

var __defProp = Object.defineProperty;
var __defNormalProp = (obj, key, value) => key in obj ? __defProp(obj, key, { enumerable: true, configurable: true, writable: true, value }) : obj[key] = value;
var __publicField = (obj, key, value) => __defNormalProp(obj, typeof key !== "symbol" ? key + "" : key, value);
const SERVICE_REGISTRY = {
  "inventory-management": {
    name: "Autonomous Inventory Management",
    description: "AI-powered inventory optimization with automatic reordering and supplier management",
    category: "inventory",
    requirements: ["inventory"],
    outcomes: [
      "Zero stockouts on critical items",
      "Optimized inventory levels",
      "Automated supplier communication",
      "Reduced holding costs"
    ],
    automation: {
      triggers: [
        {
          type: "threshold",
          condition: { metric: "stock_level", operator: "<=", value: "reorder_point" },
          description: "Stock level reaches reorder point"
        },
        {
          type: "schedule",
          condition: { cron: "0 9 * * 1" },
          // Every Monday at 9 AM
          description: "Weekly inventory optimization"
        }
      ],
      actions: [
        {
          type: "automation",
          target: "supplier_order",
          parameters: { auto_approve: true, urgent: false },
          description: "Generate and send purchase order to supplier"
        },
        {
          type: "notification",
          target: "business_owner",
          parameters: { channel: "whatsapp", priority: "medium" },
          description: "Notify owner of automated actions"
        }
      ]
    },
    pricing: {
      model: "outcome",
      outcomeMetric: "stockouts_prevented"
    }
  },
  "sales-automation": {
    name: "Intelligent Sales Processing",
    description: "End-to-end sales automation from quote to payment collection",
    category: "sales",
    requirements: ["sales", "crm"],
    outcomes: [
      "Instant invoice generation",
      "Automated payment reminders",
      "Customer retention optimization",
      "Revenue maximization"
    ],
    automation: {
      triggers: [
        {
          type: "event",
          condition: { event: "sale_completed" },
          description: "Sale transaction completed"
        },
        {
          type: "schedule",
          condition: { cron: "0 10 * * *" },
          // Daily at 10 AM
          description: "Daily payment follow-ups"
        }
      ],
      actions: [
        {
          type: "automation",
          target: "invoice_generation",
          parameters: { format: "pdf", delivery: "email_whatsapp" },
          description: "Generate and deliver invoice"
        },
        {
          type: "automation",
          target: "payment_reminder",
          parameters: { escalation: true },
          description: "Send payment reminders"
        }
      ]
    },
    pricing: {
      model: "usage",
      usageMetric: "invoices_processed"
    }
  },
  "financial-intelligence": {
    name: "Financial Analytics & Compliance",
    description: "Automated financial reporting, tax compliance, and cash flow optimization",
    category: "finance",
    requirements: ["analytics"],
    outcomes: [
      "Real-time financial visibility",
      "Automated tax compliance",
      "Cash flow optimization",
      "Expense optimization"
    ],
    automation: {
      triggers: [
        {
          type: "schedule",
          condition: { cron: "0 8 1 * *" },
          // First day of month at 8 AM
          description: "Monthly financial reports"
        },
        {
          type: "event",
          condition: { event: "expense_recorded" },
          description: "New expense recorded"
        }
      ],
      actions: [
        {
          type: "analysis",
          target: "financial_health",
          parameters: { include_forecasts: true },
          description: "Analyze financial health and trends"
        },
        {
          type: "automation",
          target: "report_generation",
          parameters: { formats: ["pdf", "excel"], recipients: ["owner", "accountant"] },
          description: "Generate and distribute financial reports"
        }
      ]
    },
    pricing: {
      model: "subscription",
      basePrice: 299
    }
  },
  "customer-engagement": {
    name: "AI Customer Relationship Management",
    description: "Automated customer communication, loyalty programs, and marketing campaigns",
    category: "customer",
    requirements: ["crm"],
    outcomes: [
      "Increased customer retention",
      "Personalized customer experience",
      "Automated marketing campaigns",
      "Customer lifetime value optimization"
    ],
    automation: {
      triggers: [
        {
          type: "event",
          condition: { event: "customer_birthday" },
          description: "Customer birthday"
        },
        {
          type: "threshold",
          condition: { metric: "days_since_purchase", operator: ">", value: 30 },
          description: "Customer inactive for 30+ days"
        }
      ],
      actions: [
        {
          type: "automation",
          target: "personalized_message",
          parameters: { channel: "whatsapp", personalization: true },
          description: "Send personalized customer message"
        },
        {
          type: "automation",
          target: "loyalty_reward",
          parameters: { type: "discount", value: "10%" },
          description: "Apply loyalty rewards"
        }
      ]
    },
    pricing: {
      model: "outcome",
      outcomeMetric: "customer_retention_rate"
    }
  }
};
class ServiceOrchestrator {
  constructor(context, tenant) {
    __publicField(this, "context");
    __publicField(this, "tenant");
    this.context = context;
    this.tenant = tenant;
  }
  // Execute a service by name
  async executeService(serviceName, parameters = {}) {
    const service = SERVICE_REGISTRY[serviceName];
    if (!service) {
      throw new Error(`Service '${serviceName}' not found`);
    }
    for (const requirement of service.requirements) {
      if (!this.tenant.subscription.features.includes(requirement)) {
        throw new Error(`Service requires feature: ${requirement}`);
      }
    }
    const execution = {
      serviceId: serviceName,
      tenantId: this.tenant.tenantId,
      triggeredBy: this.context.userId,
      startTime: /* @__PURE__ */ new Date(),
      status: "running",
      outcomes: [],
      usage: { apiCalls: 0, computeTime: 0, dataProcessed: 0 },
      cost: 0
    };
    try {
      for (const action of service.automation.actions) {
        await this.executeAction(action, parameters, execution);
      }
      execution.status = "completed";
      execution.endTime = /* @__PURE__ */ new Date();
      execution.cost = this.calculateServiceCost(service, execution);
      return execution;
    } catch (error) {
      execution.status = "failed";
      execution.endTime = /* @__PURE__ */ new Date();
      throw error;
    }
  }
  // Execute individual service action
  async executeAction(action, parameters, execution) {
    switch (action.type) {
      case "automation":
        await this.executeAutomation(action, parameters, execution);
        break;
      case "notification":
        await this.sendNotification(action, parameters, execution);
        break;
      case "integration":
        await this.performIntegration(action, parameters, execution);
        break;
      case "analysis":
        await this.performAnalysis(action, parameters, execution);
        break;
    }
  }
  async executeAutomation(action, parameters, execution) {
    console.log(`Executing automation: ${action.target}`, { action, parameters });
    execution.usage.apiCalls += 1;
    execution.usage.computeTime += 100;
    execution.outcomes.push({
      type: "automation",
      action: action.target,
      result: "success",
      timestamp: /* @__PURE__ */ new Date()
    });
  }
  async sendNotification(action, parameters, execution) {
    console.log(`Sending notification: ${action.target}`, { action, parameters });
    execution.usage.apiCalls += 1;
    execution.outcomes.push({
      type: "notification",
      target: action.target,
      channel: parameters.channel || "email",
      timestamp: /* @__PURE__ */ new Date()
    });
  }
  async performIntegration(action, parameters, execution) {
    console.log(`Performing integration: ${action.target}`, { action, parameters });
    execution.usage.apiCalls += 2;
    execution.usage.dataProcessed += 1024;
    execution.outcomes.push({
      type: "integration",
      target: action.target,
      result: "completed",
      timestamp: /* @__PURE__ */ new Date()
    });
  }
  async performAnalysis(action, parameters, execution) {
    console.log(`Performing analysis: ${action.target}`, { action, parameters });
    execution.usage.computeTime += 5e3;
    execution.usage.dataProcessed += 10240;
    execution.outcomes.push({
      type: "analysis",
      target: action.target,
      insights: ["Sample insight 1", "Sample insight 2"],
      timestamp: /* @__PURE__ */ new Date()
    });
  }
  calculateServiceCost(service, execution) {
    switch (service.pricing.model) {
      case "usage":
        return execution.usage.apiCalls * 0.01 + execution.usage.computeTime * 1e-3;
      case "outcome":
        return execution.outcomes.length * 5;
      case "subscription":
        return service.pricing.basePrice || 0;
      default:
        return 0;
    }
  }
  // Get available services for tenant
  getAvailableServices() {
    return Object.values(SERVICE_REGISTRY).filter(
      (service) => service.requirements.every((req) => this.tenant.subscription.features.includes(req))
    );
  }
  // Get service execution history
  async getServiceHistory(limit = 10) {
    return [];
  }
}

async function getTenantContext(event) {
  var _a;
  try {
    let tenantId = getHeader(event, "x-tenant-id") || getCookie(event, "tenant-id") || extractTenantFromSubdomain(event);
    if (!tenantId) {
      const token = getCookie(event, "auth-token") || ((_a = getHeader(event, "authorization")) == null ? void 0 : _a.replace("Bearer ", ""));
      if (token) {
        try {
          const parts = token.split(".");
          if (parts.length > 1) {
            tenantId = "demo-salon";
          }
        } catch (error) {
          console.warn("Failed to decode token for tenant extraction:", error);
        }
      }
    }
    if (!tenantId) return null;
    const tenant = await loadTenantConfig(tenantId);
    return tenant;
  } catch (error) {
    console.error("Error getting tenant context:", error);
    return null;
  }
}
function extractTenantFromSubdomain(event) {
  const host = getHeader(event, "host");
  if (!host) return null;
  const parts = host.split(".");
  if (parts.length >= 3 && !["www", "api", "admin"].includes(parts[0])) {
    return parts[0];
  }
  return null;
}
async function loadTenantConfig(tenantId) {
  const demoTenants = {
    "demo-salon": {
      tenantId: "demo-salon",
      tenantName: "Beautiful Hair Salon",
      subscription: {
        plan: "professional",
        status: "active",
        features: ["inventory", "sales", "crm", "analytics", "ai-assistant"],
        usageLimits: {
          users: 10,
          storage: 50,
          apiCalls: 1e4,
          aiOperations: 1e3
        }
      },
      settings: {
        timezone: "Africa/Johannesburg",
        currency: "ZAR",
        language: "en",
        businessType: "salon",
        customization: {
          brandColor: "#8B5CF6",
          features: {
            appointmentBooking: true,
            productCatalog: true,
            loyaltyProgram: true
          }
        }
      }
    },
    "demo-shop": {
      tenantId: "demo-shop",
      tenantName: "Township Spaza Shop",
      subscription: {
        plan: "basic",
        status: "active",
        features: ["inventory", "sales", "basic-analytics"],
        usageLimits: {
          users: 3,
          storage: 10,
          apiCalls: 2e3,
          aiOperations: 100
        }
      },
      settings: {
        timezone: "Africa/Johannesburg",
        currency: "ZAR",
        language: "en",
        businessType: "retail",
        customization: {
          brandColor: "#10B981",
          features: {
            barcodescanning: true,
            creditSales: true,
            mobilePOS: true
          }
        }
      }
    }
  };
  return demoTenants[tenantId] || null;
}
async function requireTenant(event) {
  const tenant = await getTenantContext(event);
  if (!tenant) {
    console.warn("No tenant context found, using demo-salon as fallback");
    const fallbackTenant = await loadTenantConfig("demo-salon");
    if (!fallbackTenant) {
      throw createError({
        statusCode: 500,
        statusMessage: "Unable to load fallback tenant configuration"
      });
    }
    return fallbackTenant;
  }
  return tenant;
}
function createServiceContext(tenant, userId, userRole) {
  return {
    tenantId: tenant.tenantId,
    userId,
    userRole,
    requestId: generateRequestId(),
    timestamp: /* @__PURE__ */ new Date()
  };
}
function generateRequestId() {
  return `req_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`;
}
async function trackServiceUsage(context, service, operation, metadata) {
  console.log("Service Usage:", {
    tenantId: context.tenantId,
    userId: context.userId,
    service,
    operation,
    timestamp: context.timestamp,
    requestId: context.requestId,
    metadata
  });
}

const chat_post = defineEventHandler(async (event) => {
  const tenant = await requireTenant(event);
  const body = await readBody(event);
  const { message, context, userId = "demo-user", conversationId } = body;
  if (!message || typeof message !== "string" || message.trim().length === 0) {
    throw createError({
      statusCode: 400,
      statusMessage: "Message is required and must be a non-empty string"
    });
  }
  const serviceContext = createServiceContext(tenant, userId, "owner");
  const orchestrator = new ServiceOrchestrator(serviceContext, tenant);
  const response = await processBusinessQuery(message, context, orchestrator, tenant);
  const finalConversationId = conversationId || `conv_${Math.random().toString(36).substr(2, 9)}`;
  return {
    success: true,
    data: {
      id: Math.random().toString(36).substr(2, 9),
      conversationId: finalConversationId,
      userMessage: message.trim(),
      aiResponse: response.response,
      responseType: response.context,
      actions: response.actions || [],
      insights: response.insights || {},
      services: response.services || [],
      tenantId: tenant.tenantId,
      timestamp: (/* @__PURE__ */ new Date()).toISOString(),
      processingTime: Math.floor(Math.random() * 500) + 200,
      language: tenant.settings.language || "en",
      confidence: 0.95
    }
  };
});
async function processBusinessQuery(message, context, orchestrator, tenant) {
  const query = message.toLowerCase();
  if (query.includes("stock") || query.includes("inventory")) {
    return await handleInventoryQuery(query, orchestrator, tenant);
  } else if (query.includes("sales") || query.includes("revenue")) {
    return await handleSalesQuery(query, orchestrator, tenant);
  } else if (query.includes("customer") || query.includes("client")) {
    return await handleCustomerQuery(query, orchestrator, tenant);
  } else if (query.includes("report") || query.includes("finance")) {
    return await handleFinanceQuery(query, orchestrator, tenant);
  } else if (query.includes("automate") || query.includes("setup")) {
    return await handleAutomationQuery(query, orchestrator, tenant);
  } else if (query.includes("group") || query.includes("bulk") || query.includes("buying")) {
    return await handleGroupBuyingQuery(query, orchestrator, tenant);
  } else {
    return await handleGeneralQuery(query, orchestrator, tenant);
  }
}
async function handleInventoryQuery(query, orchestrator, tenant) {
  const insights = await analyzeInventoryHealth(tenant.tenantId);
  let response = `\u{1F4E6} **Inventory Intelligence for ${tenant.tenantName}**

`;
  if (query.includes("low") || query.includes("reorder")) {
    response += `I found ${insights.lowStockItems} items need reordering:
`;
    response += `\u2022 Hair Shampoo (Professional) - 5 units left
`;
    response += `\u2022 Conditioning Treatment - 8 units left
`;
    response += `\u2022 Styling Gel - 3 units left

`;
    response += `\u{1F916} **Service as Software - I can handle this autonomously:**
`;
    response += `\u2705 Contact suppliers automatically
`;
    response += `\u2705 Compare prices and negotiate
`;
    response += `\u2705 Place orders with optimal timing
`;
    response += `\u2705 Track deliveries and update inventory
`;
    response += `\u2705 Handle all paperwork and invoicing

`;
    response += `\u{1F4B0} **Outcome-Based Pricing:** You only pay when I successfully prevent stockouts
`;
    response += `\u{1F4C8} **Guaranteed Result:** Zero stockouts on critical items or you don't pay`;
    return {
      response,
      context: "inventory",
      actions: [
        {
          type: "service",
          label: "Activate Auto-Inventory Service",
          description: "AI takes full control of inventory management",
          service: "inventory-management",
          parameters: { trigger: "manual", items: "low_stock" },
          pricing: "R50 per stockout prevented"
        },
        {
          type: "view",
          label: "View Inventory Details",
          route: "/inventory"
        }
      ],
      insights
    };
  }
  response += `Current Status:
`;
  response += `\u2022 Total Items: ${insights.totalItems}
`;
  response += `\u2022 Items Needing Attention: ${insights.lowStockItems}
`;
  response += `\u2022 Inventory Value: R${insights.totalValue.toLocaleString()}
`;
  response += `\u2022 AI Optimization Score: ${insights.optimization}/10

`;
  response += `\u{1F3AF} **Available AI Services:**
`;
  response += `\u{1F916} **Autonomous Inventory Management**
`;
  response += `\u2022 I monitor stock 24/7 and reorder automatically
`;
  response += `\u2022 Predict demand and optimize stock levels
`;
  response += `\u2022 Negotiate with suppliers for best prices
`;
  response += `\u2022 Guarantee zero stockouts on critical items

`;
  response += `\u{1F4A1} **Outcome:** You'll never run out of important items again, and I'll optimize your purchasing to save money`;
  return {
    response,
    context: "inventory",
    actions: [
      {
        type: "service",
        label: "Activate Full Inventory AI",
        description: "Complete hands-off inventory management",
        service: "inventory-management",
        parameters: { setup: "complete" },
        pricing: "R299/month + R25 per order placed"
      }
    ],
    insights
  };
}
async function handleSalesQuery(query, orchestrator, tenant) {
  const insights = await analyzeSalesPerformance(tenant.tenantId);
  let response = `\u{1F4C8} **Sales Intelligence for ${tenant.tenantName}**

`;
  if (query.includes("today") || query.includes("daily")) {
    response += `Today's Performance:
`;
    response += `\u2022 Revenue: R${insights.todayRevenue.toLocaleString()}
`;
    response += `\u2022 Transactions: ${insights.todayTransactions}
`;
    response += `\u2022 Average Sale: R${insights.averageSale}

`;
  }
  response += `\u{1F4CA} **This Month:**
`;
  response += `\u2022 Total Revenue: R${insights.monthlyRevenue.toLocaleString()}
`;
  response += `\u2022 Growth vs Last Month: +${insights.monthlyGrowth}%
`;
  response += `\u2022 Top Service: ${insights.topService}
`;
  response += `\u2022 Customer Retention: ${insights.retention}%

`;
  response += `\u{1F680} **Service as Software - Sales Automation:**
`;
  response += `Instead of using tools, I deliver these outcomes:

`;
  response += `\u2705 **Instant Invoicing**: Every sale generates and sends invoice automatically
`;
  response += `\u2705 **Payment Collection**: AI follows up on overdue payments via WhatsApp
`;
  response += `\u2705 **Customer Retention**: Automated loyalty programs and personal touches
`;
  response += `\u2705 **Sales Optimization**: Dynamic pricing and upselling suggestions

`;
  response += `\u{1F4B0} **Pricing Model**: 2% of additional revenue I generate for you
`;
  response += `\u{1F4C8} **Guarantee**: Increase monthly revenue by 15% or service is free`;
  return {
    response,
    context: "sales",
    actions: [
      {
        type: "service",
        label: "Activate Sales AI Service",
        description: "Complete sales process automation",
        service: "sales-automation",
        parameters: { level: "full" },
        pricing: "2% of additional revenue generated"
      },
      {
        type: "view",
        label: "Sales Dashboard",
        route: "/sales"
      }
    ],
    insights
  };
}
async function handleCustomerQuery(query, orchestrator, tenant) {
  const insights = await analyzeCustomerEngagement(tenant.tenantId);
  let response = `\u{1F465} **Customer Intelligence for ${tenant.tenantName}**

`;
  response += `\u{1F4CB} **Current Status:**
`;
  response += `\u2022 Total Customers: ${insights.totalCustomers}
`;
  response += `\u2022 Active This Month: ${insights.activeCustomers}
`;
  response += `\u2022 Average Lifetime Value: R${insights.lifetimeValue}
`;
  response += `\u2022 Satisfaction Score: ${insights.satisfaction}/5

`;
  response += `\u{1F916} **AI Customer Service - What I Do:**
`;
  response += `\u2705 **Personalized Communication**: Birthday messages, appointment reminders
`;
  response += `\u2705 **Loyalty Management**: Automatic rewards, tier upgrades
`;
  response += `\u2705 **Retention**: Predict who might leave and win them back
`;
  response += `\u2705 **Feedback Collection**: Gather and act on customer feedback

`;
  response += `\u{1F4A1} **Service Outcomes:**
`;
  response += `\u2022 Increase customer retention by 25%
`;
  response += `\u2022 Boost customer lifetime value by 30%
`;
  response += `\u2022 Reduce customer service time by 80%
`;
  response += `\u2022 Achieve 95%+ customer satisfaction

`;
  response += `\u{1F4B0} **Pricing**: R5 per customer per month (only for active customers I manage)`;
  return {
    response,
    context: "customer",
    actions: [
      {
        type: "service",
        label: "Activate Customer AI",
        description: "Complete customer relationship automation",
        service: "customer-engagement",
        parameters: { features: "all" },
        pricing: "R5 per active customer per month"
      },
      {
        type: "view",
        label: "Customer Management",
        route: "/customers"
      }
    ],
    insights
  };
}
async function handleFinanceQuery(query, orchestrator, tenant) {
  const insights = await analyzeFinancialHealth(tenant.tenantId);
  let response = `\u{1F4B0} **Financial Intelligence for ${tenant.tenantName}**

`;
  response += `\u{1F4CA} **Financial Health:**
`;
  response += `\u2022 Monthly Profit: R${insights.monthlyProfit.toLocaleString()}
`;
  response += `\u2022 Profit Margin: ${insights.profitMargin}%
`;
  response += `\u2022 Cash Flow: R${insights.cashFlow.toLocaleString()}
`;
  response += `\u2022 Outstanding Payments: R${insights.outstanding.toLocaleString()}

`;
  response += `\u{1F916} **Financial AI Service - Complete Automation:**
`;
  response += `\u2705 **Real-time Reporting**: Live financial dashboard
`;
  response += `\u2705 **Tax Compliance**: Automatic SARS submissions
`;
  response += `\u2705 **Expense Optimization**: Find and eliminate waste
`;
  response += `\u2705 **Cash Flow Management**: Predict and prevent shortfalls
`;
  response += `\u2705 **Accountant Integration**: Seamless handoff to your accountant

`;
  if (query.includes("report") || query.includes("generate")) {
    try {
      const execution = await orchestrator.executeService("financial-intelligence", {
        reportType: "comprehensive",
        period: "monthly"
      });
      response += `\u2705 **Report Generated Successfully!**
`;
      response += `Your comprehensive financial report has been automatically:
`;
      response += `\u2022 Generated and saved to your account
`;
      response += `\u2022 Emailed to your registered accountant
`;
      response += `\u2022 Backed up to secure cloud storage
`;
      response += `\u2022 Filed for tax compliance

`;
      response += `\u{1F4B0} **Cost**: R25 (outcome-based - only charged because report was successfully delivered)`;
    } catch (error) {
      response += `\u26A0\uFE0F Report generation in progress...
`;
      response += `I'll notify you when complete. No charge if it fails.`;
    }
  }
  response += `\u{1F4B0} **Service Pricing**:
`;
  response += `\u2022 Financial reports: R25 each (only when successfully delivered)
`;
  response += `\u2022 Tax compliance: R199/month (guaranteed accuracy)
`;
  response += `\u2022 Cash flow optimization: 10% of money saved`;
  return {
    response,
    context: "finance",
    actions: [
      {
        type: "service",
        label: "Activate Financial AI",
        description: "Complete financial management automation",
        service: "financial-intelligence",
        parameters: { automation: "full" },
        pricing: "Outcome-based: pay only for results"
      },
      {
        type: "view",
        label: "Financial Dashboard",
        route: "/analytics"
      }
    ],
    insights
  };
}
async function handleGroupBuyingQuery(query, orchestrator, tenant) {
  let response = `\u{1F91D} **Group Buying Intelligence for ${tenant.tenantName}**

`;
  response += `Available Group Purchases:

`;
  response += `**1. Hair Product Bulk Order** \u{1F487}\u200D\u2640\uFE0F
`;
  response += `\u2022 Organizer: Beauty Supply Network
`;
  response += `\u2022 Minimum: R5,000, Current: R18,500/R25,000 target
`;
  response += `\u2022 Your Suggested Amount: R3,500
`;
  response += `\u2022 Savings: 22% (R770 saved)
`;
  response += `\u2022 Deadline: 5 days

`;
  response += `**2. Salon Equipment Maintenance** \u{1F527}
`;
  response += `\u2022 Shared technician visits
`;
  response += `\u2022 Savings: 40% per visit
`;
  response += `\u2022 Next visit: Next Tuesday

`;
  response += `**3. Marketing Materials** \u{1F4F1}
`;
  response += `\u2022 Bulk printing and design
`;
  response += `\u2022 Savings: 35%
`;
  response += `\u2022 Professional photography included

`;
  response += `\u{1F916} **Group Buying AI Service:**
`;
  response += `I monitor all local group buying opportunities and:
`;
  response += `\u2705 Automatically join profitable group purchases
`;
  response += `\u2705 Organize group buys for your regular supplies
`;
  response += `\u2705 Coordinate with other businesses
`;
  response += `\u2705 Handle all logistics and payments

`;
  response += `\u{1F4B0} **Guaranteed Savings**: 15% minimum savings or service is free
`;
  response += `\u{1F4CA} **Your Potential Monthly Savings**: R2,400`;
  return {
    response,
    context: "groupbuying",
    actions: [
      {
        type: "service",
        label: "Join Hair Product Bulk Order",
        description: "Save R770 on next order",
        service: "group-buying",
        parameters: { group: "hair-products", amount: 3500 },
        pricing: "Free - you save money"
      },
      {
        type: "service",
        label: "Activate Group Buying AI",
        description: "Automatic participation in profitable group purchases",
        service: "group-buying-automation",
        parameters: { automation: "full" },
        pricing: "10% of money saved"
      }
    ]
  };
}
async function handleAutomationQuery(query, orchestrator, tenant) {
  const availableServices = orchestrator.getAvailableServices();
  let response = `\u{1F916} **Service as Software Setup for ${tenant.tenantName}**

`;
  response += `**Traditional SaaS vs Service as Software:**

`;
  response += `\u274C **Old Way**: You pay for tools and do the work
`;
  response += `\u2705 **New Way**: AI does the work, you pay for outcomes

`;
  response += `**Available AI Business Services:**

`;
  availableServices.forEach((service) => {
    response += `**${service.name}**
`;
    response += `${service.description}
`;
    response += `Guaranteed Outcomes: ${service.outcomes.slice(0, 2).join(", ")}

`;
  });
  response += `\u{1F4A1} **What This Means for You:**
`;
  response += `\u2022 AI agents work 24/7 managing your business
`;
  response += `\u2022 You focus on customers while AI handles operations
`;
  response += `\u2022 Pay only when AI delivers guaranteed results
`;
  response += `\u2022 No monthly fees unless you get value

`;
  response += `\u{1F3AF} **Recommended for ${tenant.settings.businessType}:**
`;
  response += `Based on your business type, I recommend starting with:
`;
  response += `1. **Inventory AI** - Never run out of products (R299/month)
`;
  response += `2. **Sales AI** - Automate invoicing and follow-ups (2% of extra revenue)
`;
  response += `3. **Customer AI** - Keep customers happy and loyal (R5/customer/month)

`;
  response += `\u{1F680} **Complete Package**: All services for R699/month + performance bonuses
`;
  response += `\u{1F4B0} **ROI Guarantee**: 300% return on investment or money back`;
  return {
    response,
    context: "automation",
    actions: [
      {
        type: "service",
        label: "Activate Complete AI Package",
        description: "All services with ROI guarantee",
        service: "full-automation",
        parameters: { level: "comprehensive" },
        pricing: "R699/month + performance fees"
      },
      {
        type: "setup",
        label: "Custom Setup Wizard",
        route: "/setup/services"
      }
    ],
    services: availableServices
  };
}
async function handleGeneralQuery(query, orchestrator, tenant) {
  const response = `\u{1F44B} **Hello! I'm your AI Business Partner for ${tenant.tenantName}**

\u{1F680} **I'm not just a chatbot - I'm a Service as Software platform:**

**Instead of giving you tools to use, I do the work for you:**

\u{1F916} **What I Actually Do:**
\u2022 Manage your inventory automatically (monitor, reorder, optimize)
\u2022 Process sales end-to-end (invoicing, payments, follow-ups)
\u2022 Handle customer relationships (messages, loyalty, retention)
\u2022 Generate financial reports and ensure compliance
\u2022 Find group buying opportunities and coordinate purchases
\u2022 Optimize your entire business operations

\u{1F4B0} **Revolutionary Pricing:**
\u2022 Pay only for actual business outcomes
\u2022 No monthly fees unless you get value
\u2022 Guaranteed ROI or money back

\u{1F4AC} **Try asking me:**
\u2022 "Set up automated inventory management"
\u2022 "Handle all my customer communications"
\u2022 "Generate this month's financial report"
\u2022 "Find me group buying opportunities"
\u2022 "Show me my sales performance"

\u{1F3AF} **I work 24/7 to deliver business outcomes while you focus on what you love.**`;
  return {
    response,
    context: "general",
    actions: [
      {
        type: "demo",
        label: "See AI Services in Action",
        route: "/demo/ai-services"
      },
      {
        type: "setup",
        label: "Quick Setup (5 minutes)",
        route: "/setup"
      },
      {
        type: "service",
        label: "Start Free Trial",
        description: "Try any service free for 7 days",
        service: "trial",
        parameters: { duration: 7 },
        pricing: "Free trial - no commitment"
      }
    ]
  };
}
async function analyzeInventoryHealth(tenantId) {
  return {
    totalItems: 157,
    lowStockItems: 12,
    totalValue: 45700,
    fastMoving: ["Professional Shampoo", "Hair Treatment", "Styling Products"],
    optimization: 8.2,
    alerts: 3
  };
}
async function analyzeSalesPerformance(tenantId) {
  return {
    todayRevenue: 2340,
    todayTransactions: 18,
    averageSale: 130,
    monthlyRevenue: 67800,
    monthlyGrowth: 15.3,
    topService: "Hair Treatment & Styling",
    retention: 87
  };
}
async function analyzeCustomerEngagement(tenantId) {
  return {
    totalCustomers: 342,
    activeCustomers: 156,
    lifetimeValue: 1250,
    satisfaction: 4.6,
    churnRisk: 23,
    loyaltyMembers: 89
  };
}
async function analyzeFinancialHealth(tenantId) {
  return {
    monthlyProfit: 23400,
    profitMargin: 34.5,
    cashFlow: 15600,
    outstanding: 8900,
    expenses: 44400,
    taxLiability: 6800
  };
}

const chat_post$1 = /*#__PURE__*/Object.freeze(/*#__PURE__*/Object.defineProperty({
  __proto__: null,
  default: chat_post
}, Symbol.toStringTag, { value: 'Module' }));

const dashboard_get = defineEventHandler(async (event) => {
  const query = getQuery$1(event);
  const { period = "30d", module = "all", metric = "overview" } = query;
  const now = /* @__PURE__ */ new Date();
  const periodMap = {
    "7d": 7,
    "30d": 30,
    "90d": 90,
    "1y": 365
  };
  const days = periodMap[period] || 30;
  const startDate = new Date(now.getTime() - days * 24 * 60 * 60 * 1e3);
  const demoAnalytics = {
    overview: {
      summary: {
        period: `${days} days`,
        startDate: startDate.toISOString(),
        endDate: now.toISOString(),
        currency: "ZAR"
      },
      kpis: {
        totalRevenue: 145320.5,
        totalCosts: 98745.3,
        netProfit: 46575.2,
        profitMargin: 32.1,
        avgOrderValue: 1453.21,
        customerGrowth: 18.5,
        inventoryTurnover: 2.8,
        cashFlow: 52890.4
      },
      trends: {
        revenue: [
          { date: "2024-08-01", value: 4200 },
          { date: "2024-08-02", value: 3850 },
          { date: "2024-08-03", value: 5100 },
          { date: "2024-08-04", value: 4650 },
          { date: "2024-08-05", value: 6200 },
          { date: "2024-08-06", value: 5800 },
          { date: "2024-08-07", value: 4300 },
          { date: "2024-08-08", value: 4950 },
          { date: "2024-08-09", value: 5450 },
          { date: "2024-08-10", value: 6100 }
        ],
        orders: [
          { date: "2024-08-01", value: 12 },
          { date: "2024-08-02", value: 8 },
          { date: "2024-08-03", value: 15 },
          { date: "2024-08-04", value: 11 },
          { date: "2024-08-05", value: 18 },
          { date: "2024-08-06", value: 16 },
          { date: "2024-08-07", value: 9 },
          { date: "2024-08-08", value: 13 },
          { date: "2024-08-09", value: 14 },
          { date: "2024-08-10", value: 17 }
        ],
        customers: [
          { date: "2024-08-01", value: 45 },
          { date: "2024-08-02", value: 47 },
          { date: "2024-08-03", value: 48 },
          { date: "2024-08-04", value: 49 },
          { date: "2024-08-05", value: 52 },
          { date: "2024-08-06", value: 53 },
          { date: "2024-08-07", value: 54 },
          { date: "2024-08-08", value: 55 },
          { date: "2024-08-09", value: 57 },
          { date: "2024-08-10", value: 58 }
        ]
      }
    },
    sales: {
      performance: {
        totalSales: 145320.5,
        avgDailySales: 4844.02,
        topSalesDay: { date: "2024-08-05", amount: 6200 },
        salesGrowth: 23.4,
        // % vs previous period
        conversionRate: 68.5
      },
      products: {
        topSelling: [
          { name: "Maize Meal (5kg)", quantity: 245, revenue: 29400, growth: 15.2 },
          { name: "Rice (2kg)", quantity: 189, revenue: 28490.5, growth: 8.7 },
          { name: "Cooking Oil (750ml)", quantity: 156, revenue: 23712, growth: 22.1 },
          { name: "Sugar (1kg)", quantity: 234, revenue: 10530, growth: -5.3 },
          { name: "Flour (2.5kg)", quantity: 87, revenue: 17835, growth: 31.8 }
        ],
        categories: [
          { name: "Staples", revenue: 89467.5, percentage: 61.6 },
          { name: "Cooking Oils", revenue: 35780, percentage: 24.6 },
          { name: "Spices & Seasonings", revenue: 12890, percentage: 8.9 },
          { name: "Other", revenue: 7183, percentage: 4.9 }
        ]
      },
      customers: {
        topCustomers: [
          { name: "Corner Caf\xE9", orders: 18, revenue: 32450, growth: 28.5 },
          { name: "Williams Construction", orders: 12, revenue: 24680, growth: 15.2 },
          { name: "Local Restaurant Co.", orders: 15, revenue: 28970, growth: 22.1 },
          { name: "Sunshine Bakery", orders: 8, revenue: 18320, growth: 45.6 }
        ],
        segments: [
          { type: "Business", count: 23, revenue: 98420, percentage: 67.7 },
          { type: "Individual", count: 35, revenue: 46900.5, percentage: 32.3 }
        ]
      }
    },
    inventory: {
      performance: {
        totalValue: 285600,
        turnoverRate: 2.8,
        averageAge: 18.5,
        // days
        stockAccuracy: 96.8,
        outOfStockEvents: 3
      },
      lowStock: [
        { name: "Rice (2kg)", currentStock: 3, minimumStock: 10, daysUntilOut: 2 },
        { name: "Premium Flour", currentStock: 8, minimumStock: 15, daysUntilOut: 5 },
        { name: "Turmeric Powder", currentStock: 12, minimumStock: 20, daysUntilOut: 8 }
      ],
      fastMoving: [
        { name: "Maize Meal (5kg)", avgDailySales: 8.2, velocity: "high" },
        { name: "Cooking Oil (750ml)", avgDailySales: 5.2, velocity: "high" },
        { name: "Rice (2kg)", avgDailySales: 6.3, velocity: "medium" },
        { name: "Sugar (1kg)", avgDailySales: 7.8, velocity: "medium" }
      ],
      movements: {
        totalIn: 1450,
        // units received
        totalOut: 1285,
        // units sold
        adjustments: -8,
        // shrinkage, damage, etc.
        netChange: 157
      }
    },
    finance: {
      cashFlow: {
        inflows: {
          sales: 145320.5,
          groupPurchases: 8500,
          otherIncome: 2150,
          total: 155970.5
        },
        outflows: {
          inventory: 89400,
          salaries: 28500,
          rent: 12e3,
          utilities: 3200,
          transport: 4850,
          other: 5680,
          total: 143630
        },
        netCashFlow: 12340.5
      },
      receivables: {
        total: 45680,
        current: 32450,
        // 0-30 days
        overdue30: 8950,
        // 31-60 days
        overdue60: 3280,
        // 61-90 days
        overdue90: 1e3,
        // 90+ days
        averageCollectionPeriod: 28.5
        // days
      },
      payables: {
        total: 28900,
        current: 22100,
        overdue: 6800,
        averagePaymentPeriod: 32.1
        // days
      }
    },
    groupBuying: {
      participation: {
        activeGroups: 3,
        totalCommitments: 12450,
        estimatedSavings: 1850,
        savingsPercentage: 12.9
      },
      groups: [
        { name: "Flour Purchase", progress: 76, savings: 18, yourCommitment: 4625 },
        { name: "Sugar Purchase", progress: 68, savings: 12.5, yourCommitment: 5950 },
        { name: "Packaging Supplies", progress: 40, savings: 15.9, yourCommitment: 1875 }
      ],
      impact: {
        totalSavingsToDate: 15680,
        averageSavingsPerOrder: 385.5,
        purchaseVolumeIncrease: 45.2
        // % increase in buying power
      }
    },
    ai: {
      insights: [
        {
          type: "opportunity",
          title: "Inventory Optimization",
          description: "Your rice stock is low but sales velocity is high. Consider increasing order quantity by 40% to reduce stockout risk.",
          impact: "medium",
          actionRequired: true
        },
        {
          type: "trend",
          title: "Sales Growth Pattern",
          description: "Sales have increased 23% this month, with strongest growth in cooking oils category. Consider expanding product range.",
          impact: "high",
          actionRequired: false
        },
        {
          type: "efficiency",
          title: "Group Buying Potential",
          description: "Based on your purchase patterns, joining the upcoming cooking oil group purchase could save you R1,250.",
          impact: "medium",
          actionRequired: true
        }
      ],
      predictions: {
        nextWeekSales: 32450,
        confidence: 87.5,
        stockoutRisk: [
          { product: "Rice (2kg)", probability: 85 },
          { product: "Premium Flour", probability: 45 }
        ],
        optimalReorderPoints: [
          { product: "Maize Meal (5kg)", current: 15, recommended: 20 },
          { product: "Cooking Oil (750ml)", current: 12, recommended: 18 }
        ]
      }
    }
  };
  let responseData = demoAnalytics;
  if (module !== "all") {
    responseData = { [module]: demoAnalytics[module] };
  }
  if (metric !== "overview" && demoAnalytics[metric]) {
    responseData = { [metric]: demoAnalytics[metric] };
  }
  return {
    success: true,
    data: {
      analytics: responseData,
      metadata: {
        period,
        module,
        metric,
        generatedAt: (/* @__PURE__ */ new Date()).toISOString(),
        dataFreshness: "real-time",
        // In demo, would show actual data age
        lastUpdated: new Date(now.getTime() - 5 * 60 * 1e3).toISOString()
        // 5 minutes ago
      }
    }
  };
});

const dashboard_get$1 = /*#__PURE__*/Object.freeze(/*#__PURE__*/Object.defineProperty({
  __proto__: null,
  default: dashboard_get
}, Symbol.toStringTag, { value: 'Module' }));

const login_post = defineEventHandler(async (event) => {
  const body = await readBody(event);
  const { email, password } = body;
  if (!email || !password) {
    throw createError({
      statusCode: 400,
      statusMessage: "Email and password are required"
    });
  }
  const demoUsers = [
    {
      id: "1",
      email: "owner@demo.toss.co.za",
      password: "password123",
      firstName: "Thabo",
      lastName: "Molefe",
      businessName: "Thabo's Spaza Shop",
      businessId: "business_1",
      role: "owner",
      status: "active"
    },
    {
      id: "2",
      email: "manager@demo.toss.co.za",
      password: "password123",
      firstName: "Nomsa",
      lastName: "Dlamini",
      businessName: "Thabo's Spaza Shop",
      businessId: "business_1",
      role: "manager",
      status: "active"
    },
    {
      id: "3",
      email: "employee@demo.toss.co.za",
      password: "password123",
      firstName: "Sipho",
      lastName: "Mthembu",
      businessName: "Thabo's Spaza Shop",
      businessId: "business_1",
      role: "employee",
      status: "active"
    }
  ];
  const user = demoUsers.find((u) => u.email === email && u.password === password);
  if (!user) {
    throw createError({
      statusCode: 401,
      statusMessage: "Invalid email or password"
    });
  }
  const token = Buffer.from(`${user.id}:${Date.now()}`).toString("base64");
  setCookie(event, "auth-token", token, {
    maxAge: 60 * 60 * 24 * 7,
    // 7 days
    secure: false,
    sameSite: "strict",
    httpOnly: true
  });
  const { password: _, ...userWithoutPassword } = user;
  return {
    success: true,
    user: {
      ...userWithoutPassword,
      createdAt: "2024-01-01T00:00:00Z",
      updatedAt: (/* @__PURE__ */ new Date()).toISOString()
    },
    token,
    permissions: user.role === "owner" ? ["admin"] : [user.role]
  };
});

const login_post$1 = /*#__PURE__*/Object.freeze(/*#__PURE__*/Object.defineProperty({
  __proto__: null,
  default: login_post
}, Symbol.toStringTag, { value: 'Module' }));

const logout_post = defineEventHandler(async (event) => {
  setCookie(event, "auth-token", "", {
    maxAge: 0,
    secure: false,
    sameSite: "strict",
    httpOnly: true
  });
  return {
    success: true,
    message: "Logged out successfully"
  };
});

const logout_post$1 = /*#__PURE__*/Object.freeze(/*#__PURE__*/Object.defineProperty({
  __proto__: null,
  default: logout_post
}, Symbol.toStringTag, { value: 'Module' }));

const me_get = defineEventHandler(async (event) => {
  const token = getCookie(event, "auth-token");
  if (!token) {
    throw createError({
      statusCode: 401,
      statusMessage: "No authentication token provided"
    });
  }
  try {
    const decoded = Buffer.from(token, "base64").toString("utf-8");
    const [userId] = decoded.split(":");
    const demoUsers = [
      {
        id: "1",
        email: "owner@demo.toss.co.za",
        firstName: "Thabo",
        lastName: "Molefe",
        businessName: "Thabo's Spaza Shop",
        businessId: "business_1",
        role: "owner",
        status: "active"
      },
      {
        id: "2",
        email: "manager@demo.toss.co.za",
        firstName: "Nomsa",
        lastName: "Dlamini",
        businessName: "Thabo's Spaza Shop",
        businessId: "business_1",
        role: "manager",
        status: "active"
      },
      {
        id: "3",
        email: "employee@demo.toss.co.za",
        firstName: "Sipho",
        lastName: "Mthembu",
        businessName: "Thabo's Spaza Shop",
        businessId: "business_1",
        role: "employee",
        status: "active"
      }
    ];
    const user = demoUsers.find((u) => u.id === userId);
    if (!user) {
      throw createError({
        statusCode: 401,
        statusMessage: "Invalid token"
      });
    }
    return {
      success: true,
      user: {
        ...user,
        createdAt: "2024-01-01T00:00:00Z",
        updatedAt: (/* @__PURE__ */ new Date()).toISOString()
      },
      permissions: user.role === "owner" ? ["admin"] : [user.role]
    };
  } catch (error) {
    throw createError({
      statusCode: 401,
      statusMessage: "Invalid token"
    });
  }
});

const me_get$1 = /*#__PURE__*/Object.freeze(/*#__PURE__*/Object.defineProperty({
  __proto__: null,
  default: me_get
}, Symbol.toStringTag, { value: 'Module' }));

const register_post = defineEventHandler(async (event) => {
  const body = await readBody(event);
  const { businessName, firstName, lastName, email, phone, password, businessType } = body;
  if (!businessName || !firstName || !lastName || !email || !password || !businessType) {
    throw createError({
      statusCode: 400,
      statusMessage: "All required fields must be provided"
    });
  }
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  if (!emailRegex.test(email)) {
    throw createError({
      statusCode: 400,
      statusMessage: "Invalid email format"
    });
  }
  if (password.length < 8) {
    throw createError({
      statusCode: 400,
      statusMessage: "Password must be at least 8 characters long"
    });
  }
  const existingEmails = [
    "owner@demo.toss.co.za",
    "manager@demo.toss.co.za",
    "employee@demo.toss.co.za"
  ];
  if (existingEmails.includes(email)) {
    throw createError({
      statusCode: 409,
      statusMessage: "An account with this email already exists"
    });
  }
  const newUser = {
    id: `user_${Date.now()}`,
    email,
    firstName,
    lastName,
    businessName,
    businessId: `business_${Date.now()}`,
    phone,
    businessType,
    role: "owner",
    // First user of a business becomes owner
    status: "pending",
    // Requires email verification
    createdAt: (/* @__PURE__ */ new Date()).toISOString(),
    updatedAt: (/* @__PURE__ */ new Date()).toISOString()
  };
  return {
    success: true,
    message: "Account created successfully. Please check your email to verify your account.",
    user: newUser
  };
});

const register_post$1 = /*#__PURE__*/Object.freeze(/*#__PURE__*/Object.defineProperty({
  __proto__: null,
  default: register_post
}, Symbol.toStringTag, { value: 'Module' }));

const index_get$g = defineEventHandler(async (event) => {
  const query = getQuery$1(event);
  const { type = "all", unreadOnly = false, limit = 20, participantId = "" } = query;
  const demoConversations = [
    {
      id: "conv_001",
      type: "direct",
      title: "Order Discussion - Corner Caf\xE9",
      participants: [
        {
          id: "USER_CURRENT",
          name: "You",
          role: "owner",
          avatar: null,
          isOnline: true
        },
        {
          id: "CUST-001",
          name: "Sarah Johnson",
          role: "customer",
          avatar: "https://images.unsplash.com/photo-1494790108755-2616b25c4ab4?w=64&h=64&fit=crop&crop=face",
          isOnline: false,
          lastSeen: "2024-08-25T12:30:00Z"
        }
      ],
      lastMessage: {
        id: "msg_latest_001",
        senderId: "CUST-001",
        senderName: "Sarah Johnson",
        content: "Thanks for the quick delivery! The maize meal quality is excellent as always. Can we schedule a regular weekly order?",
        type: "text",
        timestamp: "2024-08-25T14:30:00Z",
        isRead: false,
        attachments: []
      },
      unreadCount: 2,
      updatedAt: "2024-08-25T14:30:00Z",
      status: "active",
      tags: ["customer", "orders", "regular_customer"],
      metadata: {
        relatedOrderId: "ORD-2024-007",
        customerType: "business",
        priority: "normal"
      }
    },
    {
      id: "conv_002",
      type: "group",
      title: "Flour Group Purchase Coordination",
      participants: [
        {
          id: "USER_CURRENT",
          name: "You",
          role: "participant",
          avatar: null,
          isOnline: true
        },
        {
          id: "USER_ALEXANDRA",
          name: "Alexandra Business Network",
          role: "organizer",
          avatar: "https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=64&h=64&fit=crop&crop=face",
          isOnline: true,
          lastSeen: null
        },
        {
          id: "USER_SUNSHINE",
          name: "Sunshine Bakery",
          role: "participant",
          avatar: "https://images.unsplash.com/photo-1438761681033-6461ffad8d80?w=64&h=64&fit=crop&crop=face",
          isOnline: false,
          lastSeen: "2024-08-25T10:15:00Z"
        },
        {
          id: "USER_CORNER",
          name: "Corner Caf\xE9",
          role: "participant",
          avatar: "https://images.unsplash.com/photo-1544725176-7c40e5a71c5e?w=64&h=64&fit=crop&crop=face",
          isOnline: false,
          lastSeen: "2024-08-25T13:45:00Z"
        }
      ],
      lastMessage: {
        id: "msg_latest_002",
        senderId: "USER_ALEXANDRA",
        senderName: "Alexandra Business Network",
        content: "Great news everyone! We've reached 76% of our target. Just need 24 more bags to trigger the group discount. Deadline is this Thursday.",
        type: "text",
        timestamp: "2024-08-25T13:15:00Z",
        isRead: true,
        attachments: []
      },
      unreadCount: 0,
      updatedAt: "2024-08-25T13:15:00Z",
      status: "active",
      tags: ["group_buying", "flour", "coordination"],
      metadata: {
        groupPurchaseId: "gp_flour_001",
        progress: 76,
        deadline: "2024-08-28T23:59:59Z"
      }
    },
    {
      id: "conv_003",
      type: "support",
      title: "Logistics Issue - Late Delivery",
      participants: [
        {
          id: "USER_CURRENT",
          name: "You",
          role: "customer",
          avatar: null,
          isOnline: true
        },
        {
          id: "SUPPORT_001",
          name: "FastTrack Support",
          role: "support_agent",
          avatar: "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=64&h=64&fit=crop&crop=face",
          isOnline: true,
          lastSeen: null
        }
      ],
      lastMessage: {
        id: "msg_latest_003",
        senderId: "SUPPORT_001",
        senderName: "FastTrack Support",
        content: "I've contacted the driver and confirmed your delivery is next on the route. ETA is 15:45. You'll receive a tracking update shortly. Sorry for the delay!",
        type: "text",
        timestamp: "2024-08-25T15:30:00Z",
        isRead: false,
        attachments: []
      },
      unreadCount: 1,
      updatedAt: "2024-08-25T15:30:00Z",
      status: "resolved",
      tags: ["support", "logistics", "delivery"],
      metadata: {
        ticketId: "TICK-789456",
        shipmentId: "SHIP-001",
        priority: "medium",
        category: "delivery_delay"
      }
    },
    {
      id: "conv_004",
      type: "business",
      title: "Partnership Opportunity - Thabo's Hardware",
      participants: [
        {
          id: "USER_CURRENT",
          name: "You",
          role: "business_owner",
          avatar: null,
          isOnline: true
        },
        {
          id: "USER_THABO",
          name: "Thabo Mthembu",
          role: "business_owner",
          avatar: "https://images.unsplash.com/photo-1500648767791-00dcc994a43e?w=64&h=64&fit=crop&crop=face",
          isOnline: false,
          lastSeen: "2024-08-25T11:00:00Z"
        }
      ],
      lastMessage: {
        id: "msg_latest_004",
        senderId: "USER_THABO",
        senderName: "Thabo Mthembu",
        content: "I think there's a great opportunity for us to combine our customer bases. My construction clients often need bulk food supplies for their sites. What do you think about a referral partnership?",
        type: "text",
        timestamp: "2024-08-25T11:00:00Z",
        isRead: true,
        attachments: []
      },
      unreadCount: 0,
      updatedAt: "2024-08-25T11:00:00Z",
      status: "active",
      tags: ["partnership", "business_development", "collaboration"],
      metadata: {
        businessType: "referral_partnership",
        potentialValue: "high",
        category: "cross_promotion"
      }
    },
    {
      id: "conv_005",
      type: "direct",
      title: "Payment Overdue - Williams Construction",
      participants: [
        {
          id: "USER_CURRENT",
          name: "You",
          role: "vendor",
          avatar: null,
          isOnline: true
        },
        {
          id: "CUST-003",
          name: "John Williams",
          role: "customer",
          avatar: "https://images.unsplash.com/photo-1560250097-0b93528c311a?w=64&h=64&fit=crop&crop=face",
          isOnline: false,
          lastSeen: "2024-08-24T16:30:00Z"
        }
      ],
      lastMessage: {
        id: "msg_latest_005",
        senderId: "USER_CURRENT",
        senderName: "You",
        content: "Hi John, just a friendly reminder that invoice INV-2024-003 for R120.52 was due 2 days ago. Could you please arrange payment when convenient? Thanks!",
        type: "text",
        timestamp: "2024-08-25T09:00:00Z",
        isRead: true,
        attachments: [
          {
            id: "att_001",
            name: "INV-2024-003.pdf",
            type: "document",
            size: "245KB",
            url: "/documents/INV-2024-003.pdf"
          }
        ]
      },
      unreadCount: 0,
      updatedAt: "2024-08-25T09:00:00Z",
      status: "waiting_response",
      tags: ["payment", "overdue", "follow_up"],
      metadata: {
        invoiceId: "INV-2024-003",
        amount: 120.52,
        daysPastDue: 2,
        priority: "medium"
      }
    }
  ];
  let filteredConversations = demoConversations;
  if (type !== "all") {
    filteredConversations = filteredConversations.filter((conv) => conv.type === type);
  }
  if (unreadOnly === "true") {
    filteredConversations = filteredConversations.filter((conv) => conv.unreadCount > 0);
  }
  if (participantId) {
    filteredConversations = filteredConversations.filter(
      (conv) => conv.participants.some((p) => p.id === participantId)
    );
  }
  filteredConversations.sort((a, b) => new Date(b.updatedAt).getTime() - new Date(a.updatedAt).getTime());
  const limitedConversations = filteredConversations.slice(0, Number(limit));
  const stats = {
    total: filteredConversations.length,
    unread: demoConversations.filter((conv) => conv.unreadCount > 0).length,
    byType: {
      direct: demoConversations.filter((conv) => conv.type === "direct").length,
      group: demoConversations.filter((conv) => conv.type === "group").length,
      support: demoConversations.filter((conv) => conv.type === "support").length,
      business: demoConversations.filter((conv) => conv.type === "business").length
    },
    byStatus: {
      active: demoConversations.filter((conv) => conv.status === "active").length,
      resolved: demoConversations.filter((conv) => conv.status === "resolved").length,
      waiting_response: demoConversations.filter((conv) => conv.status === "waiting_response").length,
      archived: demoConversations.filter((conv) => conv.status === "archived").length
    },
    totalUnreadMessages: demoConversations.reduce((sum, conv) => sum + conv.unreadCount, 0)
  };
  return {
    success: true,
    data: {
      conversations: limitedConversations,
      stats,
      pagination: {
        total: filteredConversations.length,
        limit: Number(limit),
        page: 1
      }
    }
  };
});

const index_get$h = /*#__PURE__*/Object.freeze(/*#__PURE__*/Object.defineProperty({
  __proto__: null,
  default: index_get$g
}, Symbol.toStringTag, { value: 'Module' }));

const send_post = defineEventHandler(async (event) => {
  const method = getMethod(event);
  if (method !== "POST") {
    throw createError({
      statusCode: 405,
      statusMessage: "Method not allowed"
    });
  }
  const body = await readBody(event);
  const { conversationId, content, type = "text", attachments = [], replyToId = null } = body;
  if (!conversationId || !content) {
    throw createError({
      statusCode: 400,
      statusMessage: "conversationId and content are required"
    });
  }
  const validConversations = ["conv_001", "conv_002", "conv_003", "conv_004", "conv_005"];
  if (!validConversations.includes(conversationId)) {
    throw createError({
      statusCode: 404,
      statusMessage: "Conversation not found"
    });
  }
  const messageId = `msg_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`;
  const newMessage = {
    id: messageId,
    conversationId,
    senderId: "USER_CURRENT",
    senderName: "You",
    content,
    type,
    timestamp: (/* @__PURE__ */ new Date()).toISOString(),
    isRead: false,
    // Will be marked as read by recipients
    attachments: attachments.map((att) => ({
      id: `att_${Date.now()}_${Math.random().toString(36).substr(2, 6)}`,
      name: att.name,
      type: att.type,
      size: att.size,
      url: att.url || `/uploads/${att.name}`
    })),
    replyTo: replyToId ? {
      messageId: replyToId,
      preview: "Original message preview..."
      // Would fetch actual message content
    } : null,
    status: "sent",
    deliveryStatus: {
      sent: (/* @__PURE__ */ new Date()).toISOString(),
      delivered: null,
      read: null
    },
    reactions: [],
    editHistory: []
  };
  const updatedConversation = {
    id: conversationId,
    lastMessage: {
      id: messageId,
      senderId: "USER_CURRENT",
      senderName: "You",
      content,
      type,
      timestamp: (/* @__PURE__ */ new Date()).toISOString(),
      isRead: false,
      attachments: newMessage.attachments
    },
    updatedAt: (/* @__PURE__ */ new Date()).toISOString(),
    unreadCount: 0
    // Reset for sender, but would increment for recipients
  };
  return {
    success: true,
    data: {
      message: newMessage,
      conversation: updatedConversation,
      notifications: [
        {
          type: "message_sent",
          recipients: ["CUST-001"],
          // Example recipient IDs
          content: `New message in ${conversationId}`,
          timestamp: (/* @__PURE__ */ new Date()).toISOString()
        }
      ]
    }
  };
});

const send_post$1 = /*#__PURE__*/Object.freeze(/*#__PURE__*/Object.defineProperty({
  __proto__: null,
  default: send_post
}, Symbol.toStringTag, { value: 'Module' }));

const index_get$e = defineEventHandler(async (event) => {
  const query = getQuery$1(event);
  const { page = 1, limit = 20, search = "", status = "active" } = query;
  const demoCustomers = [
    {
      id: "CUST-001",
      customerNumber: "CUST-001",
      type: "individual",
      firstName: "Sipho",
      lastName: "Mthembu",
      companyName: null,
      email: "sipho@email.com",
      phone: "+27 82 123 4567",
      alternatePhone: null,
      idNumber: "8501015800084",
      vatNumber: null,
      status: "active",
      creditLimit: 1e3,
      currentBalance: 0,
      totalPurchases: 2547.85,
      lastPurchaseDate: "2024-08-24T10:30:00Z",
      registrationDate: "2024-01-15T09:00:00Z",
      billingAddress: {
        street: "123 Main Street",
        suburb: "Alexandra",
        city: "Johannesburg",
        province: "Gauteng",
        postalCode: "2090",
        country: "South Africa"
      },
      shippingAddress: {
        street: "123 Main Street",
        suburb: "Alexandra",
        city: "Johannesburg",
        province: "Gauteng",
        postalCode: "2090",
        country: "South Africa"
      },
      paymentTerms: "Cash",
      preferredPaymentMethod: "cash",
      notes: "Regular customer, very reliable",
      tags: ["regular", "local"],
      createdAt: "2024-01-15T09:00:00Z",
      updatedAt: "2024-08-24T10:30:00Z"
    },
    {
      id: "CUST-002",
      customerNumber: "CUST-002",
      type: "individual",
      firstName: "Mary",
      lastName: "Johnson",
      companyName: null,
      email: "mary@email.com",
      phone: "+27 71 987 6543",
      alternatePhone: "+27 11 555 0123",
      idNumber: "7203125600089",
      vatNumber: null,
      status: "active",
      creditLimit: 500,
      currentBalance: 98.47,
      totalPurchases: 1245.67,
      lastPurchaseDate: "2024-08-24T14:15:00Z",
      registrationDate: "2024-02-20T11:30:00Z",
      billingAddress: {
        street: "456 Oak Avenue",
        suburb: "Soweto",
        city: "Johannesburg",
        province: "Gauteng",
        postalCode: "1804",
        country: "South Africa"
      },
      shippingAddress: {
        street: "456 Oak Avenue",
        suburb: "Soweto",
        city: "Johannesburg",
        province: "Gauteng",
        postalCode: "1804",
        country: "South Africa"
      },
      paymentTerms: "30 Days",
      preferredPaymentMethod: "credit",
      notes: "Credit customer, sometimes late with payments",
      tags: ["credit", "monthly"],
      createdAt: "2024-02-20T11:30:00Z",
      updatedAt: "2024-08-24T14:15:00Z"
    },
    {
      id: "CUST-003",
      customerNumber: "CUST-003",
      type: "business",
      firstName: "David",
      lastName: "Williams",
      companyName: "Williams Construction",
      email: "david@williamsconst.co.za",
      phone: "+27 83 456 7890",
      alternatePhone: "+27 11 789 0123",
      idNumber: null,
      vatNumber: "4123456789",
      status: "active",
      creditLimit: 5e3,
      currentBalance: 120.52,
      totalPurchases: 15847.32,
      lastPurchaseDate: "2024-08-23T16:45:00Z",
      registrationDate: "2024-01-10T14:00:00Z",
      billingAddress: {
        street: "789 Industrial Road",
        suburb: "Midrand",
        city: "Johannesburg",
        province: "Gauteng",
        postalCode: "1685",
        country: "South Africa"
      },
      shippingAddress: {
        street: "789 Industrial Road",
        suburb: "Midrand",
        city: "Johannesburg",
        province: "Gauteng",
        postalCode: "1685",
        country: "South Africa"
      },
      paymentTerms: "30 Days",
      preferredPaymentMethod: "eft",
      notes: "Major business customer, bulk purchases",
      tags: ["business", "bulk", "vip"],
      createdAt: "2024-01-10T14:00:00Z",
      updatedAt: "2024-08-23T16:45:00Z"
    },
    {
      id: "CUST-004",
      customerNumber: "CUST-004",
      type: "individual",
      firstName: "Thandi",
      lastName: "Ndlovu",
      companyName: null,
      email: "thandi@email.com",
      phone: "+27 76 234 5678",
      alternatePhone: null,
      idNumber: "9008125700091",
      vatNumber: null,
      status: "inactive",
      creditLimit: 0,
      currentBalance: 0,
      totalPurchases: 345.2,
      lastPurchaseDate: "2024-06-15T12:20:00Z",
      registrationDate: "2024-03-05T10:15:00Z",
      billingAddress: {
        street: "321 Park Lane",
        suburb: "Randburg",
        city: "Johannesburg",
        province: "Gauteng",
        postalCode: "2125",
        country: "South Africa"
      },
      shippingAddress: {
        street: "321 Park Lane",
        suburb: "Randburg",
        city: "Johannesburg",
        province: "Gauteng",
        postalCode: "2125",
        country: "South Africa"
      },
      paymentTerms: "Cash",
      preferredPaymentMethod: "cash",
      notes: "Moved away, account inactive",
      tags: ["inactive"],
      createdAt: "2024-03-05T10:15:00Z",
      updatedAt: "2024-06-15T12:20:00Z"
    }
  ];
  let filteredCustomers = demoCustomers;
  if (search) {
    const searchLower = search.toString().toLowerCase();
    filteredCustomers = filteredCustomers.filter(
      (customer) => customer.firstName.toLowerCase().includes(searchLower) || customer.lastName.toLowerCase().includes(searchLower) || customer.companyName && customer.companyName.toLowerCase().includes(searchLower) || customer.email.toLowerCase().includes(searchLower) || customer.phone.includes(searchLower) || customer.customerNumber.toLowerCase().includes(searchLower)
    );
  }
  if (status && status !== "all") {
    filteredCustomers = filteredCustomers.filter(
      (customer) => customer.status === status.toString()
    );
  }
  const totalItems = filteredCustomers.length;
  const totalPages = Math.ceil(totalItems / Number(limit));
  const startIndex = (Number(page) - 1) * Number(limit);
  const endIndex = startIndex + Number(limit);
  const paginatedCustomers = filteredCustomers.slice(startIndex, endIndex);
  const totalCustomers = filteredCustomers.length;
  const activeCustomers = filteredCustomers.filter((c) => c.status === "active").length;
  const totalOutstanding = filteredCustomers.reduce((sum, customer) => sum + customer.currentBalance, 0);
  const totalLifetimeValue = filteredCustomers.reduce((sum, customer) => sum + customer.totalPurchases, 0);
  const averageCustomerValue = totalCustomers > 0 ? totalLifetimeValue / totalCustomers : 0;
  return {
    success: true,
    data: {
      customers: paginatedCustomers,
      pagination: {
        currentPage: Number(page),
        totalPages,
        totalItems,
        itemsPerPage: Number(limit),
        hasNextPage: Number(page) < totalPages,
        hasPreviousPage: Number(page) > 1
      },
      summary: {
        totalCustomers,
        activeCustomers,
        totalOutstanding,
        totalLifetimeValue,
        averageCustomerValue
      }
    }
  };
});

const index_get$f = /*#__PURE__*/Object.freeze(/*#__PURE__*/Object.defineProperty({
  __proto__: null,
  default: index_get$e
}, Symbol.toStringTag, { value: 'Module' }));

const outcomes_get = defineEventHandler(async (event) => {
  const tenant = await requireTenant(event);
  const dashboard = await generateOutcomesDashboard(tenant);
  return {
    success: true,
    data: {
      tenant: {
        id: tenant.tenantId,
        name: tenant.tenantName,
        businessType: tenant.settings.businessType
      },
      summary: dashboard.summary,
      businessOutcomes: dashboard.outcomes,
      servicePerformance: dashboard.services,
      automation: dashboard.automation,
      financial: dashboard.financial,
      predictions: dashboard.predictions,
      alerts: dashboard.alerts
    }
  };
});
async function generateOutcomesDashboard(tenant) {
  const currentDate = /* @__PURE__ */ new Date();
  tenant.settings.businessType;
  return {
    summary: {
      overallHealth: 92,
      // Business health score out of 100
      automationLevel: 87,
      // Percentage of processes automated
      monthlyGrowth: 15.3,
      // Business growth percentage
      customerSatisfaction: 4.7,
      // Out of 5
      operationalEfficiency: 94,
      // Efficiency score
      lastUpdated: currentDate.toISOString()
    },
    outcomes: {
      // Revenue & Sales Outcomes
      revenue: {
        title: "Revenue Optimization",
        current: 67800,
        target: 75e3,
        growth: 15.3,
        aiContribution: 23.7,
        // Percentage increase due to AI
        outcomes: [
          "Automated invoicing increased collection rate by 23%",
          "AI pricing optimization boosted average sale by R47",
          "Smart upselling generated R8,400 additional revenue",
          "Payment automation reduced overdue by 67%"
        ]
      },
      // Customer Outcomes
      customers: {
        title: "Customer Experience Excellence",
        satisfaction: 4.7,
        retention: 89,
        lifetimeValue: 1250,
        growth: 12.4,
        outcomes: [
          "AI personalization increased customer retention by 18%",
          "Automated birthday messages improved loyalty by 34%",
          "Smart recommendations boosted repeat purchases by 28%",
          "24/7 AI support reduced complaints by 45%"
        ]
      },
      // Operations Outcomes
      operations: {
        title: "Operational Excellence",
        efficiency: 94,
        costReduction: 23.7,
        timesSaved: 47.5,
        errorReduction: 89,
        outcomes: [
          "Automated inventory prevented 8 stockouts this month",
          "AI optimization reduced inventory costs by 18%",
          "Smart scheduling saved 47.5 hours of manual work",
          "Automated compliance reduced errors by 89%"
        ]
      },
      // Financial Outcomes
      financial: {
        title: "Financial Health",
        profitMargin: 34.5,
        cashFlow: 15600,
        costSavings: 2890,
        taxCompliance: 100,
        outcomes: [
          "AI expense categorization saved 12 hours monthly",
          "Automated reporting ensured 100% tax compliance",
          "Smart cash flow management prevented 2 shortfalls",
          "Cost optimization identified R2,890 in savings"
        ]
      }
    },
    services: {
      // Active Services Performance
      "inventory-management": {
        name: "Autonomous Inventory Management",
        status: "active",
        performance: 96,
        outcomes: 23,
        cost: 299,
        value: 3400,
        roi: 1041,
        metrics: {
          stockoutsPrevented: 8,
          ordersAutomated: 12,
          supplierNegotiations: 5,
          costOptimizations: 3
        }
      },
      "sales-automation": {
        name: "Intelligent Sales Processing",
        status: "active",
        performance: 94,
        outcomes: 67,
        cost: 678.5,
        value: 5641.9,
        roi: 731,
        metrics: {
          invoicesGenerated: 67,
          paymentsCollected: 45,
          upsellsExecuted: 23,
          customerFollowups: 156
        }
      },
      "customer-engagement": {
        name: "AI Customer Relationship",
        status: "active",
        performance: 92,
        outcomes: 234,
        cost: 195,
        value: 2800,
        roi: 1336,
        metrics: {
          personalizedMessages: 234,
          loyaltyRewards: 45,
          retentionActions: 67,
          satisfactionSurveys: 89
        }
      }
    },
    automation: {
      totalProcesses: 47,
      automatedProcesses: 41,
      automationRate: 87.2,
      timesSavedHours: 47.5,
      errorReduction: 89.3,
      activeAgents: [
        {
          name: "Inventory Agent",
          status: "working",
          lastAction: "Negotiated 12% discount with supplier",
          nextAction: "Weekly stock optimization review",
          confidence: 0.94
        },
        {
          name: "Sales Agent",
          status: "working",
          lastAction: "Sent 3 payment reminders via WhatsApp",
          nextAction: "Generate monthly sales report",
          confidence: 0.91
        },
        {
          name: "Customer Agent",
          status: "working",
          lastAction: "Sent birthday greeting to 5 customers",
          nextAction: "Plan loyalty rewards for top customers",
          confidence: 0.96
        },
        {
          name: "Finance Agent",
          status: "working",
          lastAction: "Categorized 12 expenses automatically",
          nextAction: "Prepare tax compliance report",
          confidence: 0.98
        }
      ]
    },
    financial: {
      investment: 1247.5,
      // Total monthly cost
      returns: 8732.4,
      // Total value delivered
      netProfit: 7484.9,
      // Profit after costs
      roi: 600.2,
      // ROI percentage
      paybackPeriod: 18,
      // Days to recover investment
      breakdown: {
        revenueIncrease: 5641.9,
        costSavings: 2890.5,
        timeValue: 2375,
        // Value of time saved
        complianceValue: 825
        // Value of automated compliance
      }
    },
    predictions: {
      nextMonth: {
        projectedRevenue: 78500,
        projectedGrowth: 18.2,
        anticipatedChallenges: ["Holiday season inventory planning", "Increased customer demand"],
        opportunities: ["Group buying for holiday stock", "Customer loyalty campaigns"],
        aiRecommendations: [
          "Increase inventory 25% for holiday season",
          "Launch automated holiday marketing campaign",
          "Prepare gift card and loyalty promotions"
        ]
      },
      quarterly: {
        projectedGrowth: 45.7,
        investmentRecommendations: ["Expand customer engagement service", "Add logistics automation"],
        riskFactors: ["Market saturation", "Competitive pressure"],
        mitigationStrategies: ["Diversify service offerings", "Strengthen customer relationships"]
      }
    },
    alerts: [
      {
        type: "opportunity",
        priority: "high",
        title: "Group Buying Opportunity",
        message: "Join bulk hair product purchase to save R770 (22% discount)",
        action: "Join group purchase",
        deadline: "3 days",
        value: 770
      },
      {
        type: "optimization",
        priority: "medium",
        title: "Customer Retention Improvement",
        message: "15 customers at risk of churning - AI intervention available",
        action: "Activate retention campaign",
        deadline: "1 week",
        value: 4500
      },
      {
        type: "automation",
        priority: "low",
        title: "New Automation Available",
        message: "Logistics coordination service now available for your area",
        action: "Enable logistics automation",
        deadline: "No deadline",
        value: 800
      }
    ]
  };
}

const outcomes_get$1 = /*#__PURE__*/Object.freeze(/*#__PURE__*/Object.defineProperty({
  __proto__: null,
  default: outcomes_get
}, Symbol.toStringTag, { value: 'Module' }));

const tenant_get = defineEventHandler(async (event) => {
  try {
    const tenantId = getHeader(event, "x-tenant-id") || getCookie(event, "tenant-id") || "demo-salon";
    return {
      success: true,
      debug: {
        headers: {
          "x-tenant-id": getHeader(event, "x-tenant-id"),
          "host": getHeader(event, "host"),
          "authorization": getHeader(event, "authorization")
        },
        cookies: {
          "tenant-id": getCookie(event, "tenant-id"),
          "auth-token": getCookie(event, "auth-token")
        },
        resolvedTenant: tenantId,
        timestamp: (/* @__PURE__ */ new Date()).toISOString()
      },
      data: {
        message: "Tenant debug test successful",
        tenantId
      }
    };
  } catch (error) {
    return {
      success: false,
      error: error.message,
      stack: error.stack
    };
  }
});

const tenant_get$1 = /*#__PURE__*/Object.freeze(/*#__PURE__*/Object.defineProperty({
  __proto__: null,
  default: tenant_get
}, Symbol.toStringTag, { value: 'Module' }));

const index_get$c = defineEventHandler(async (event) => {
  const query = getQuery$1(event);
  const { status = "all", category = "", limit = 20, search = "" } = query;
  const demoGroupPurchases = [
    {
      id: "gp_flour_001",
      title: "Premium Flour Bulk Purchase",
      description: "High-quality premium flour for bakeries and restaurants. Certified organic, stone-ground.",
      organizer: {
        id: "USER_ALEXANDRA",
        name: "Alexandra Business Network",
        rating: 4.8,
        completedPurchases: 23,
        trustScore: 95
      },
      product: {
        name: "Premium Organic Flour",
        brand: "Mill & Stone Co.",
        specifications: "25kg bags, stone-ground, organic certified",
        unitPrice: 450,
        // Regular price per 25kg bag
        groupPrice: 370,
        // Group purchase price
        savings: 18
        // Percentage savings
      },
      requirements: {
        minimumQuantity: 100,
        // Total bags needed for group price
        minimumParticipants: 5,
        maximumQuantity: 500,
        unitSize: "25kg bag"
      },
      progress: {
        currentQuantity: 76,
        currentParticipants: 12,
        progressPercentage: 76
      },
      timeline: {
        createdAt: "2024-08-20T10:00:00Z",
        deadline: "2024-08-28T23:59:59Z",
        deliveryDate: "2024-09-02T09:00:00Z",
        paymentDue: "2024-08-30T23:59:59Z"
      },
      status: "active",
      category: "food_ingredients",
      logistics: {
        deliveryMethod: "centralized_pickup",
        location: "Alexandra Business Hub, 45 1st Avenue",
        distributionCost: 25,
        // per participant
        coordinates: { lat: -26.1017, lng: 28.0875 }
      },
      participants: [
        { name: "Sunshine Bakery", quantity: 20, joinedAt: "2024-08-21T08:00:00Z" },
        { name: "Corner Caf\xE9", quantity: 8, joinedAt: "2024-08-21T14:30:00Z" },
        { name: "Artisan Breads", quantity: 25, joinedAt: "2024-08-22T11:15:00Z" },
        { name: "Local Restaurant Co.", quantity: 15, joinedAt: "2024-08-23T16:45:00Z" }
      ],
      terms: {
        paymentTerms: "Full payment required 2 days before delivery",
        cancellationPolicy: "Free cancellation up to 48 hours before payment due",
        qualityGuarantee: "100% satisfaction guaranteed or full refund",
        liability: "Organizer responsible for quality assurance"
      }
    },
    {
      id: "gp_sugar_002",
      title: "White Sugar - Commercial Grade",
      description: "Bulk purchase of commercial-grade white sugar for restaurants, cafes, and food businesses.",
      organizer: {
        id: "USER_MARCUS",
        name: "Marcus Food Distribution",
        rating: 4.6,
        completedPurchases: 18,
        trustScore: 89
      },
      product: {
        name: "Commercial White Sugar",
        brand: "Sweet Valley",
        specifications: "50kg bags, refined sugar, food grade",
        unitPrice: 680,
        groupPrice: 595,
        savings: 12.5
      },
      requirements: {
        minimumQuantity: 50,
        minimumParticipants: 3,
        maximumQuantity: 200,
        unitSize: "50kg bag"
      },
      progress: {
        currentQuantity: 34,
        currentParticipants: 7,
        progressPercentage: 68
      },
      timeline: {
        createdAt: "2024-08-22T14:00:00Z",
        deadline: "2024-08-30T23:59:59Z",
        deliveryDate: "2024-09-05T10:00:00Z",
        paymentDue: "2024-09-01T23:59:59Z"
      },
      status: "active",
      category: "food_ingredients",
      logistics: {
        deliveryMethod: "direct_delivery",
        location: "Various locations within 25km radius",
        distributionCost: 45,
        coordinates: { lat: -26.2041, lng: 28.0473 }
      },
      participants: [
        { name: "City Diner", quantity: 10, joinedAt: "2024-08-22T15:00:00Z" },
        { name: "Sweet Treats Bakery", quantity: 15, joinedAt: "2024-08-23T09:30:00Z" },
        { name: "Coffee & More", quantity: 5, joinedAt: "2024-08-24T12:00:00Z" }
      ],
      terms: {
        paymentTerms: "Payment due upon delivery confirmation",
        cancellationPolicy: "Cancellation fee applies after minimum reached",
        qualityGuarantee: "Quality guaranteed as per supplier standards",
        liability: "Shared liability among participants"
      }
    },
    {
      id: "gp_oil_003",
      title: "Cooking Oil - Sunflower",
      description: "Bulk sunflower cooking oil purchase for commercial kitchens and food service businesses.",
      organizer: {
        id: "USER_FATIMA",
        name: "Fatima's Restaurant Supply",
        rating: 4.9,
        completedPurchases: 31,
        trustScore: 98
      },
      product: {
        name: "Pure Sunflower Oil",
        brand: "Golden Fields",
        specifications: "20L containers, cold-pressed, non-GMO",
        unitPrice: 320,
        groupPrice: 275,
        savings: 14.1
      },
      requirements: {
        minimumQuantity: 40,
        minimumParticipants: 4,
        maximumQuantity: 120,
        unitSize: "20L container"
      },
      progress: {
        currentQuantity: 42,
        currentParticipants: 9,
        progressPercentage: 105
        // Exceeded minimum
      },
      timeline: {
        createdAt: "2024-08-18T09:00:00Z",
        deadline: "2024-08-26T23:59:59Z",
        deliveryDate: "2024-08-29T08:00:00Z",
        paymentDue: "2024-08-27T23:59:59Z"
      },
      status: "confirmed",
      // Minimum reached, now confirmed
      category: "food_ingredients",
      logistics: {
        deliveryMethod: "centralized_pickup",
        location: "Fatima's Warehouse, 123 Industrial Road",
        distributionCost: 15,
        coordinates: { lat: -26.1849, lng: 28.0653 }
      },
      participants: [
        { name: "Golden Spoon Restaurant", quantity: 12, joinedAt: "2024-08-18T10:00:00Z" },
        { name: "Pizza Palace", quantity: 8, joinedAt: "2024-08-19T14:00:00Z" },
        { name: "Family Kitchen", quantity: 6, joinedAt: "2024-08-20T11:00:00Z" },
        { name: "Taste of Home", quantity: 10, joinedAt: "2024-08-21T16:00:00Z" }
      ],
      terms: {
        paymentTerms: "Payment required within 24 hours of confirmation",
        cancellationPolicy: "No cancellation after confirmation",
        qualityGuarantee: "Fresh product guarantee with expiry date check",
        liability: "Organizer assumes full liability for product quality"
      }
    },
    {
      id: "gp_packaging_004",
      title: "Food-Grade Packaging Supplies",
      description: "Biodegradable food containers and packaging materials for takeaway and delivery services.",
      organizer: {
        id: "USER_THABO",
        name: "Thabo's Business Solutions",
        rating: 4.5,
        completedPurchases: 12,
        trustScore: 85
      },
      product: {
        name: "Eco-Friendly Food Containers",
        brand: "Green Pack",
        specifications: "Mixed sizes, biodegradable, leak-proof",
        unitPrice: 850,
        // Per case of 500 containers
        groupPrice: 715,
        savings: 15.9
      },
      requirements: {
        minimumQuantity: 20,
        // Cases
        minimumParticipants: 5,
        maximumQuantity: 100,
        unitSize: "Case (500 containers)"
      },
      progress: {
        currentQuantity: 8,
        currentParticipants: 3,
        progressPercentage: 40
      },
      timeline: {
        createdAt: "2024-08-24T13:00:00Z",
        deadline: "2024-09-05T23:59:59Z",
        deliveryDate: "2024-09-10T11:00:00Z",
        paymentDue: "2024-09-07T23:59:59Z"
      },
      status: "active",
      category: "packaging",
      logistics: {
        deliveryMethod: "mixed",
        // Some pickup, some delivery
        location: "Various options available",
        distributionCost: 35,
        coordinates: { lat: -26.1368, lng: 28.0881 }
      },
      participants: [
        { name: "Quick Bites", quantity: 3, joinedAt: "2024-08-24T14:00:00Z" },
        { name: "Healthy Meals Co.", quantity: 4, joinedAt: "2024-08-25T10:00:00Z" }
      ],
      terms: {
        paymentTerms: "COD or advance payment options available",
        cancellationPolicy: "Full refund if minimum not reached",
        qualityGuarantee: "Quality samples available for inspection",
        liability: "Standard commercial liability terms"
      }
    }
  ];
  let filteredPurchases = demoGroupPurchases;
  if (status !== "all") {
    filteredPurchases = filteredPurchases.filter((gp) => gp.status === status);
  }
  if (category) {
    filteredPurchases = filteredPurchases.filter((gp) => gp.category === category);
  }
  if (search) {
    const searchLower = search.toLowerCase();
    filteredPurchases = filteredPurchases.filter(
      (gp) => gp.title.toLowerCase().includes(searchLower) || gp.description.toLowerCase().includes(searchLower) || gp.product.name.toLowerCase().includes(searchLower)
    );
  }
  const limitedPurchases = filteredPurchases.slice(0, Number(limit));
  const stats = {
    total: filteredPurchases.length,
    active: demoGroupPurchases.filter((gp) => gp.status === "active").length,
    confirmed: demoGroupPurchases.filter((gp) => gp.status === "confirmed").length,
    completed: demoGroupPurchases.filter((gp) => gp.status === "completed").length,
    totalSavings: demoGroupPurchases.reduce((sum, gp) => sum + gp.product.savings, 0) / demoGroupPurchases.length,
    categories: {
      food_ingredients: demoGroupPurchases.filter((gp) => gp.category === "food_ingredients").length,
      packaging: demoGroupPurchases.filter((gp) => gp.category === "packaging").length,
      equipment: demoGroupPurchases.filter((gp) => gp.category === "equipment").length,
      supplies: demoGroupPurchases.filter((gp) => gp.category === "supplies").length
    }
  };
  return {
    success: true,
    data: {
      groupPurchases: limitedPurchases,
      stats,
      pagination: {
        total: filteredPurchases.length,
        limit: Number(limit),
        page: 1
      }
    }
  };
});

const index_get$d = /*#__PURE__*/Object.freeze(/*#__PURE__*/Object.defineProperty({
  __proto__: null,
  default: index_get$c
}, Symbol.toStringTag, { value: 'Module' }));

const join_post = defineEventHandler(async (event) => {
  const method = getMethod(event);
  if (method !== "POST") {
    throw createError({
      statusCode: 405,
      statusMessage: "Method not allowed"
    });
  }
  const body = await readBody(event);
  const { groupPurchaseId, quantity, paymentMethod = "bank_transfer", notes = "" } = body;
  if (!groupPurchaseId || !quantity) {
    throw createError({
      statusCode: 400,
      statusMessage: "groupPurchaseId and quantity are required"
    });
  }
  if (quantity <= 0) {
    throw createError({
      statusCode: 400,
      statusMessage: "Quantity must be greater than 0"
    });
  }
  const validGroupPurchases = ["gp_flour_001", "gp_sugar_002", "gp_oil_003", "gp_packaging_004"];
  if (!validGroupPurchases.includes(groupPurchaseId)) {
    throw createError({
      statusCode: 404,
      statusMessage: "Group purchase not found"
    });
  }
  const participantId = `participant_${Date.now()}`;
  const commitment = {
    id: participantId,
    groupPurchaseId,
    participantName: "Your Business",
    // Would be from auth context
    quantity,
    paymentMethod,
    notes,
    status: "committed",
    commitmentDate: (/* @__PURE__ */ new Date()).toISOString(),
    totalCost: quantity * 370,
    // Demo calculation based on flour example
    paymentStatus: "pending",
    deliveryPreference: "pickup"
    // Default
  };
  const updatedProgress = {
    currentQuantity: 76 + quantity,
    // Previous + new commitment
    currentParticipants: 13,
    // Previous + 1
    progressPercentage: Math.min(100, (76 + quantity) / 100 * 100)
    // Based on minimum of 100
  };
  return {
    success: true,
    data: {
      commitment,
      message: `Successfully joined group purchase with ${quantity} units`,
      updatedProgress,
      nextSteps: [
        "You will receive payment instructions within 24 hours",
        "Payment deadline is shown in the group purchase details",
        "Delivery/pickup details will be shared once the minimum is reached"
      ],
      estimatedSavings: quantity * (450 - 370),
      // Demo savings calculation
      totalCommitment: commitment.totalCost
    }
  };
});

const join_post$1 = /*#__PURE__*/Object.freeze(/*#__PURE__*/Object.defineProperty({
  __proto__: null,
  default: join_post
}, Symbol.toStringTag, { value: 'Module' }));

const health_get = defineEventHandler(async (event) => {
  return {
    status: "ok",
    timestamp: (/* @__PURE__ */ new Date()).toISOString(),
    service: "TOSS Service as Software Platform",
    version: "1.0.0",
    uptime: process.uptime()
  };
});

const health_get$1 = /*#__PURE__*/Object.freeze(/*#__PURE__*/Object.defineProperty({
  __proto__: null,
  default: health_get
}, Symbol.toStringTag, { value: 'Module' }));

const _id__get = defineEventHandler(async (event) => {
  const id = getRouterParam(event, "id");
  if (!id) {
    throw createError({
      statusCode: 400,
      statusMessage: "Item ID is required"
    });
  }
  const demoItems = {
    "1": {
      id: "1",
      name: "Maize Meal (1kg)",
      sku: "MM001",
      category: "Food & Beverages",
      brand: "ACE",
      description: "High quality maize meal for cooking. Perfect for making pap, porridge, and traditional dishes.",
      currentStock: 45,
      minimumStock: 20,
      maximumStock: 100,
      unitCost: 15.5,
      sellingPrice: 22.99,
      supplier: "Local Grain Supplier",
      supplierContact: "+27 11 123 4567",
      location: "Shelf A1",
      barcode: "6001234567890",
      lastRestocked: "2024-08-20T10:00:00Z",
      expiryDate: "2025-02-15T00:00:00Z",
      status: "active",
      images: ["/images/products/maize-meal.jpg", "/images/products/maize-meal-2.jpg"],
      stockMovements: [
        {
          id: "sm1",
          type: "restock",
          quantity: 50,
          unitCost: 15.5,
          date: "2024-08-20T10:00:00Z",
          reference: "PO-2024-001",
          notes: "Monthly restock from supplier"
        },
        {
          id: "sm2",
          type: "sale",
          quantity: -5,
          unitPrice: 22.99,
          date: "2024-08-21T14:30:00Z",
          reference: "INV-2024-045",
          notes: "Regular customer purchase"
        }
      ],
      nutritionalInfo: {
        energy: "1520 kJ per 100g",
        protein: "8.2g per 100g",
        carbohydrates: "78g per 100g",
        fat: "1.2g per 100g",
        fiber: "2.1g per 100g"
      },
      createdAt: "2024-01-15T09:00:00Z",
      updatedAt: "2024-08-20T10:00:00Z"
    }
  };
  const item = demoItems[id];
  if (!item) {
    throw createError({
      statusCode: 404,
      statusMessage: "Inventory item not found"
    });
  }
  const profitMargin = ((item.sellingPrice - item.unitCost) / item.sellingPrice * 100).toFixed(1);
  const stockValue = item.currentStock * item.unitCost;
  const stockStatus = item.currentStock <= 0 ? "out_of_stock" : item.currentStock <= item.minimumStock ? "low_stock" : "in_stock";
  const daysUntilExpiry = item.expiryDate ? Math.ceil((new Date(item.expiryDate).getTime() - (/* @__PURE__ */ new Date()).getTime()) / (1e3 * 60 * 60 * 24)) : null;
  return {
    success: true,
    data: {
      ...item,
      metrics: {
        profitMargin: Number(profitMargin),
        stockValue,
        stockStatus,
        daysUntilExpiry,
        turnoverRate: 2.4,
        // Example calculation
        averageMonthlySales: 18
      }
    }
  };
});

const _id__get$1 = /*#__PURE__*/Object.freeze(/*#__PURE__*/Object.defineProperty({
  __proto__: null,
  default: _id__get
}, Symbol.toStringTag, { value: 'Module' }));

const index_get$a = defineEventHandler(async (event) => {
  const query = getQuery$1(event);
  const { page = 1, limit = 20, search = "", category = "", lowStock = false } = query;
  const demoInventory = [
    {
      id: "1",
      name: "Maize Meal (1kg)",
      sku: "MM001",
      category: "Food & Beverages",
      brand: "ACE",
      description: "High quality maize meal for cooking",
      currentStock: 45,
      minimumStock: 20,
      maximumStock: 100,
      unitCost: 15.5,
      sellingPrice: 22.99,
      supplier: "Local Grain Supplier",
      location: "Shelf A1",
      lastRestocked: "2024-08-20T10:00:00Z",
      expiryDate: "2025-02-15T00:00:00Z",
      status: "active",
      images: ["/images/products/maize-meal.jpg"],
      createdAt: "2024-01-15T09:00:00Z",
      updatedAt: "2024-08-20T10:00:00Z"
    },
    {
      id: "2",
      name: "Sunflower Oil (750ml)",
      sku: "SO002",
      category: "Food & Beverages",
      brand: "Sunfoil",
      description: "Pure sunflower cooking oil",
      currentStock: 8,
      minimumStock: 15,
      maximumStock: 50,
      unitCost: 28,
      sellingPrice: 39.99,
      supplier: "Oil Distributors SA",
      location: "Shelf B2",
      lastRestocked: "2024-08-18T14:30:00Z",
      expiryDate: "2025-06-30T00:00:00Z",
      status: "low_stock",
      images: ["/images/products/sunflower-oil.jpg"],
      createdAt: "2024-01-20T11:00:00Z",
      updatedAt: "2024-08-18T14:30:00Z"
    },
    {
      id: "3",
      name: "Bread (White Loaf)",
      sku: "BR003",
      category: "Bakery",
      brand: "Albany",
      description: "Fresh white bread loaf",
      currentStock: 12,
      minimumStock: 10,
      maximumStock: 30,
      unitCost: 12,
      sellingPrice: 16.99,
      supplier: "Albany Bakeries",
      location: "Bread Section",
      lastRestocked: "2024-08-24T08:00:00Z",
      expiryDate: "2024-08-26T00:00:00Z",
      status: "active",
      images: ["/images/products/white-bread.jpg"],
      createdAt: "2024-02-01T08:00:00Z",
      updatedAt: "2024-08-24T08:00:00Z"
    },
    {
      id: "4",
      name: "Coca Cola (500ml)",
      sku: "CC004",
      category: "Beverages",
      brand: "Coca Cola",
      description: "Classic Coca Cola soft drink",
      currentStock: 24,
      minimumStock: 12,
      maximumStock: 48,
      unitCost: 8.5,
      sellingPrice: 14.99,
      supplier: "Coca Cola Distributors",
      location: "Fridge A",
      lastRestocked: "2024-08-23T16:00:00Z",
      expiryDate: "2025-01-15T00:00:00Z",
      status: "active",
      images: ["/images/products/coca-cola.jpg"],
      createdAt: "2024-01-25T10:00:00Z",
      updatedAt: "2024-08-23T16:00:00Z"
    },
    {
      id: "5",
      name: "Rice (2kg)",
      sku: "RC005",
      category: "Food & Beverages",
      brand: "Tastic",
      description: "Premium long grain white rice",
      currentStock: 3,
      minimumStock: 10,
      maximumStock: 40,
      unitCost: 45,
      sellingPrice: 65.99,
      supplier: "Rice Importers CC",
      location: "Shelf C1",
      lastRestocked: "2024-08-15T12:00:00Z",
      expiryDate: "2025-08-01T00:00:00Z",
      status: "critical_stock",
      images: ["/images/products/rice.jpg"],
      createdAt: "2024-01-30T13:00:00Z",
      updatedAt: "2024-08-15T12:00:00Z"
    }
  ];
  let filteredInventory = demoInventory;
  if (search) {
    const searchLower = search.toString().toLowerCase();
    filteredInventory = filteredInventory.filter(
      (item) => item.name.toLowerCase().includes(searchLower) || item.sku.toLowerCase().includes(searchLower) || item.brand.toLowerCase().includes(searchLower) || item.category.toLowerCase().includes(searchLower)
    );
  }
  if (category) {
    filteredInventory = filteredInventory.filter(
      (item) => item.category.toLowerCase() === category.toString().toLowerCase()
    );
  }
  if (lowStock === "true") {
    filteredInventory = filteredInventory.filter(
      (item) => item.currentStock <= item.minimumStock
    );
  }
  const totalItems = filteredInventory.length;
  const totalPages = Math.ceil(totalItems / Number(limit));
  const startIndex = (Number(page) - 1) * Number(limit);
  const endIndex = startIndex + Number(limit);
  const paginatedItems = filteredInventory.slice(startIndex, endIndex);
  const totalValue = filteredInventory.reduce((sum, item) => sum + item.currentStock * item.unitCost, 0);
  const lowStockItems = filteredInventory.filter((item) => item.currentStock <= item.minimumStock).length;
  const outOfStockItems = filteredInventory.filter((item) => item.currentStock === 0).length;
  const categories = [...new Set(filteredInventory.map((item) => item.category))];
  return {
    success: true,
    data: {
      items: paginatedItems,
      pagination: {
        currentPage: Number(page),
        totalPages,
        totalItems,
        itemsPerPage: Number(limit),
        hasNextPage: Number(page) < totalPages,
        hasPreviousPage: Number(page) > 1
      },
      summary: {
        totalItems: filteredInventory.length,
        totalValue,
        lowStockItems,
        outOfStockItems,
        categories
      }
    }
  };
});

const index_get$b = /*#__PURE__*/Object.freeze(/*#__PURE__*/Object.defineProperty({
  __proto__: null,
  default: index_get$a
}, Symbol.toStringTag, { value: 'Module' }));

const index_post$4 = defineEventHandler(async (event) => {
  var _a, _b, _c, _d;
  const body = await readBody(event);
  const { name, sku, category, unitCost, sellingPrice, minimumStock, maximumStock } = body;
  if (!name || !sku || !category || !unitCost || !sellingPrice) {
    throw createError({
      statusCode: 400,
      statusMessage: "Missing required fields: name, sku, category, unitCost, sellingPrice"
    });
  }
  if (unitCost < 0 || sellingPrice < 0) {
    throw createError({
      statusCode: 400,
      statusMessage: "Cost and selling price must be positive numbers"
    });
  }
  if (sellingPrice <= unitCost) {
    throw createError({
      statusCode: 400,
      statusMessage: "Selling price must be greater than unit cost"
    });
  }
  if (minimumStock && maximumStock && minimumStock >= maximumStock) {
    throw createError({
      statusCode: 400,
      statusMessage: "Minimum stock must be less than maximum stock"
    });
  }
  const newId = Math.random().toString(36).substr(2, 9);
  const newItem = {
    id: newId,
    name: name.trim(),
    sku: sku.trim().toUpperCase(),
    category: category.trim(),
    brand: ((_a = body.brand) == null ? void 0 : _a.trim()) || "",
    description: ((_b = body.description) == null ? void 0 : _b.trim()) || "",
    currentStock: Number(body.currentStock) || 0,
    minimumStock: Number(minimumStock) || 0,
    maximumStock: Number(maximumStock) || 100,
    unitCost: Number(unitCost),
    sellingPrice: Number(sellingPrice),
    supplier: ((_c = body.supplier) == null ? void 0 : _c.trim()) || "",
    location: ((_d = body.location) == null ? void 0 : _d.trim()) || "",
    expiryDate: body.expiryDate || null,
    status: "active",
    images: body.images || [],
    createdAt: (/* @__PURE__ */ new Date()).toISOString(),
    updatedAt: (/* @__PURE__ */ new Date()).toISOString()
  };
  return {
    success: true,
    message: "Inventory item created successfully",
    data: newItem
  };
});

const index_post$5 = /*#__PURE__*/Object.freeze(/*#__PURE__*/Object.defineProperty({
  __proto__: null,
  default: index_post$4
}, Symbol.toStringTag, { value: 'Module' }));

const movements_post = defineEventHandler(async (event) => {
  var _a, _b, _c;
  const body = await readBody(event);
  const { itemId, type, quantity, unitPrice, reference } = body;
  if (!itemId || !type || !quantity || !reference) {
    throw createError({
      statusCode: 400,
      statusMessage: "Missing required fields: itemId, type, quantity, reference"
    });
  }
  const validTypes = ["purchase", "sale", "adjustment", "transfer", "return", "damage", "restock"];
  if (!validTypes.includes(type)) {
    throw createError({
      statusCode: 400,
      statusMessage: "Invalid movement type. Must be one of: " + validTypes.join(", ")
    });
  }
  if ((type === "sale" || type === "transfer" || type === "damage") && quantity > 0) {
    throw createError({
      statusCode: 400,
      statusMessage: "Quantity must be negative for sales, transfers, and damage movements"
    });
  }
  if ((type === "purchase" || type === "restock" || type === "return") && quantity < 0) {
    throw createError({
      statusCode: 400,
      statusMessage: "Quantity must be positive for purchases, restocks, and returns"
    });
  }
  const movementId = Math.random().toString(36).substr(2, 9);
  const stockMovement = {
    id: movementId,
    itemId,
    type,
    quantity: Number(quantity),
    unitPrice: unitPrice ? Number(unitPrice) : null,
    totalValue: unitPrice ? Number(quantity) * Number(unitPrice) : 0,
    reference: reference.trim(),
    notes: ((_a = body.notes) == null ? void 0 : _a.trim()) || "",
    batchNumber: ((_b = body.batchNumber) == null ? void 0 : _b.trim()) || null,
    expiryDate: body.expiryDate || null,
    location: ((_c = body.location) == null ? void 0 : _c.trim()) || "",
    userId: body.userId || "system",
    // In real app, get from session
    createdAt: (/* @__PURE__ */ new Date()).toISOString()
  };
  return {
    success: true,
    message: "Stock movement recorded successfully",
    data: stockMovement
  };
});

const movements_post$1 = /*#__PURE__*/Object.freeze(/*#__PURE__*/Object.defineProperty({
  __proto__: null,
  default: movements_post
}, Symbol.toStringTag, { value: 'Module' }));

const index_get$8 = defineEventHandler(async (event) => {
  const query = getQuery$1(event);
  const { status = "all", type = "", limit = 20, search = "" } = query;
  const demoShipments = [
    {
      id: "SHIP-001",
      trackingNumber: "TN789456123",
      type: "delivery",
      status: "in_transit",
      priority: "standard",
      origin: {
        name: "Your Warehouse",
        address: "123 Business Street, Alexandra, Johannesburg",
        coordinates: { lat: -26.1017, lng: 28.0875 },
        contactPerson: "Warehouse Manager",
        contactPhone: "+27 11 123 4567"
      },
      destination: {
        name: "Corner Caf\xE9",
        address: "45 Main Road, Sandton, Johannesburg",
        coordinates: { lat: -26.1076, lng: 28.0567 },
        contactPerson: "Sarah Johnson",
        contactPhone: "+27 82 555 0123"
      },
      items: [
        {
          productId: "1",
          name: "Maize Meal (5kg)",
          quantity: 10,
          weight: 50,
          value: 120
        },
        {
          productId: "2",
          name: "Rice (2kg)",
          quantity: 5,
          weight: 10,
          value: 75.5
        }
      ],
      timeline: {
        orderDate: "2024-08-25T08:00:00Z",
        pickupDate: "2024-08-25T10:30:00Z",
        estimatedDelivery: "2024-08-25T16:00:00Z",
        actualDelivery: null
      },
      carrier: {
        name: "FastTrack Logistics",
        driver: "Michael Sithole",
        vehicle: "Truck - GP 123 ABC",
        phone: "+27 83 444 5555"
      },
      costs: {
        shipping: 85,
        insurance: 5.5,
        fuel: 15,
        total: 105.5
      },
      tracking: [
        {
          timestamp: "2024-08-25T08:00:00Z",
          status: "order_created",
          location: "Your Warehouse",
          message: "Shipment order created and assigned to carrier"
        },
        {
          timestamp: "2024-08-25T10:30:00Z",
          status: "picked_up",
          location: "Your Warehouse",
          message: "Items picked up by FastTrack Logistics"
        },
        {
          timestamp: "2024-08-25T12:15:00Z",
          status: "in_transit",
          location: "Halfway Point Depot",
          message: "In transit to destination. ETA: 16:00"
        }
      ],
      specialInstructions: "Handle with care - fragile items. Call customer 30 minutes before delivery.",
      insuranceValue: 195.5,
      signature: null,
      proofOfDelivery: null
    },
    {
      id: "SHIP-002",
      trackingNumber: "TN789456124",
      type: "pickup",
      status: "delivered",
      priority: "express",
      origin: {
        name: "Sunshine Suppliers",
        address: "78 Industrial Road, Germiston",
        coordinates: { lat: -26.2309, lng: 28.1772 },
        contactPerson: "Peter Williams",
        contactPhone: "+27 11 987 6543"
      },
      destination: {
        name: "Your Warehouse",
        address: "123 Business Street, Alexandra, Johannesburg",
        coordinates: { lat: -26.1017, lng: 28.0875 },
        contactPerson: "Warehouse Manager",
        contactPhone: "+27 11 123 4567"
      },
      items: [
        {
          productId: "3",
          name: "Cooking Oil (750ml)",
          quantity: 24,
          weight: 18,
          value: 456
        }
      ],
      timeline: {
        orderDate: "2024-08-24T14:00:00Z",
        pickupDate: "2024-08-24T16:30:00Z",
        estimatedDelivery: "2024-08-24T18:00:00Z",
        actualDelivery: "2024-08-24T17:45:00Z"
      },
      carrier: {
        name: "Express Courier Services",
        driver: "Jenny Mthembu",
        vehicle: "Van - GP 456 DEF",
        phone: "+27 84 333 2222"
      },
      costs: {
        shipping: 120,
        insurance: 8.5,
        fuel: 20,
        total: 148.5
      },
      tracking: [
        {
          timestamp: "2024-08-24T14:00:00Z",
          status: "order_created",
          location: "Sunshine Suppliers",
          message: "Express pickup order created"
        },
        {
          timestamp: "2024-08-24T16:30:00Z",
          status: "picked_up",
          location: "Sunshine Suppliers",
          message: "Items collected from supplier"
        },
        {
          timestamp: "2024-08-24T17:45:00Z",
          status: "delivered",
          location: "Your Warehouse",
          message: "Successfully delivered and signed for"
        }
      ],
      specialInstructions: "Express delivery required for urgent restock",
      insuranceValue: 456,
      signature: "Warehouse Manager - 24/08/2024 17:45",
      proofOfDelivery: "POD-IMG-789456124.jpg"
    },
    {
      id: "SHIP-003",
      trackingNumber: "TN789456125",
      type: "delivery",
      status: "scheduled",
      priority: "standard",
      origin: {
        name: "Your Warehouse",
        address: "123 Business Street, Alexandra, Johannesburg",
        coordinates: { lat: -26.1017, lng: 28.0875 },
        contactPerson: "Warehouse Manager",
        contactPhone: "+27 11 123 4567"
      },
      destination: {
        name: "Williams Construction",
        address: "234 Builder Road, Roodepoort",
        coordinates: { lat: -26.1625, lng: 27.8717 },
        contactPerson: "John Williams",
        contactPhone: "+27 82 777 8888"
      },
      items: [
        {
          productId: "4",
          name: "Sugar (1kg)",
          quantity: 15,
          weight: 15,
          value: 67.5
        },
        {
          productId: "5",
          name: "Rice (2kg)",
          quantity: 8,
          weight: 16,
          value: 120.52
        }
      ],
      timeline: {
        orderDate: "2024-08-25T09:00:00Z",
        pickupDate: "2024-08-26T08:00:00Z",
        estimatedDelivery: "2024-08-26T12:00:00Z",
        actualDelivery: null
      },
      carrier: {
        name: "Reliable Transport",
        driver: "TBD",
        vehicle: "TBD",
        phone: "+27 11 555 6666"
      },
      costs: {
        shipping: 95,
        insurance: 6,
        fuel: 18,
        total: 119
      },
      tracking: [
        {
          timestamp: "2024-08-25T09:00:00Z",
          status: "order_created",
          location: "Your Warehouse",
          message: "Delivery scheduled for tomorrow morning"
        }
      ],
      specialInstructions: "Construction site delivery - call before arrival. Gate access required.",
      insuranceValue: 188.02,
      signature: null,
      proofOfDelivery: null
    },
    {
      id: "SHIP-004",
      trackingNumber: "TN789456126",
      type: "return",
      status: "processing",
      priority: "standard",
      origin: {
        name: "Local Restaurant Co.",
        address: "567 Food Street, Braamfontein",
        coordinates: { lat: -26.1929, lng: 28.0367 },
        contactPerson: "Restaurant Manager",
        contactPhone: "+27 82 999 1111"
      },
      destination: {
        name: "Your Warehouse",
        address: "123 Business Street, Alexandra, Johannesburg",
        coordinates: { lat: -26.1017, lng: 28.0875 },
        contactPerson: "Returns Department",
        contactPhone: "+27 11 123 4567"
      },
      items: [
        {
          productId: "6",
          name: "Damaged Flour Bags",
          quantity: 3,
          weight: 15,
          value: 85
        }
      ],
      timeline: {
        orderDate: "2024-08-25T11:00:00Z",
        pickupDate: null,
        estimatedDelivery: "2024-08-26T14:00:00Z",
        actualDelivery: null
      },
      carrier: {
        name: "ReturnFast Logistics",
        driver: "TBD",
        vehicle: "TBD",
        phone: "+27 11 444 7777"
      },
      costs: {
        shipping: 0,
        // Free returns
        insurance: 0,
        fuel: 0,
        total: 0
      },
      tracking: [
        {
          timestamp: "2024-08-25T11:00:00Z",
          status: "return_requested",
          location: "Local Restaurant Co.",
          message: "Return request submitted for damaged items"
        },
        {
          timestamp: "2024-08-25T14:00:00Z",
          status: "processing",
          location: "Returns Department",
          message: "Return approved, scheduling pickup"
        }
      ],
      specialInstructions: "Quality inspection required. Document damage for supplier claim.",
      insuranceValue: 85,
      signature: null,
      proofOfDelivery: null,
      returnReason: "Damaged products - water damage during transport"
    }
  ];
  let filteredShipments = demoShipments;
  if (status !== "all") {
    filteredShipments = filteredShipments.filter((ship) => ship.status === status);
  }
  if (type) {
    filteredShipments = filteredShipments.filter((ship) => ship.type === type);
  }
  if (search) {
    const searchLower = search.toLowerCase();
    filteredShipments = filteredShipments.filter(
      (ship) => ship.trackingNumber.toLowerCase().includes(searchLower) || ship.destination.name.toLowerCase().includes(searchLower) || ship.carrier.name.toLowerCase().includes(searchLower)
    );
  }
  const limitedShipments = filteredShipments.slice(0, Number(limit));
  const stats = {
    total: filteredShipments.length,
    byStatus: {
      scheduled: demoShipments.filter((s) => s.status === "scheduled").length,
      in_transit: demoShipments.filter((s) => s.status === "in_transit").length,
      delivered: demoShipments.filter((s) => s.status === "delivered").length,
      processing: demoShipments.filter((s) => s.status === "processing").length
    },
    byType: {
      delivery: demoShipments.filter((s) => s.type === "delivery").length,
      pickup: demoShipments.filter((s) => s.type === "pickup").length,
      return: demoShipments.filter((s) => s.type === "return").length
    },
    totalValue: demoShipments.reduce((sum, ship) => sum + ship.items.reduce((itemSum, item) => itemSum + item.value, 0), 0),
    totalCosts: demoShipments.reduce((sum, ship) => sum + ship.costs.total, 0)
  };
  return {
    success: true,
    data: {
      shipments: limitedShipments,
      stats,
      pagination: {
        total: filteredShipments.length,
        limit: Number(limit),
        page: 1
      }
    }
  };
});

const index_get$9 = /*#__PURE__*/Object.freeze(/*#__PURE__*/Object.defineProperty({
  __proto__: null,
  default: index_get$8
}, Symbol.toStringTag, { value: 'Module' }));

const _trackingNumber__get = defineEventHandler(async (event) => {
  const trackingNumber = getRouterParam(event, "trackingNumber");
  if (!trackingNumber) {
    throw createError({
      statusCode: 400,
      statusMessage: "Tracking number is required"
    });
  }
  const demoTrackingData = {
    "TN789456123": {
      id: "SHIP-001",
      trackingNumber: "TN789456123",
      status: "in_transit",
      currentLocation: {
        name: "Halfway Point Depot",
        address: "Highway Junction, Johannesburg",
        coordinates: { lat: -26.125, lng: 28.01 },
        timestamp: "2024-08-25T12:15:00Z"
      },
      estimatedDelivery: "2024-08-25T16:00:00Z",
      destination: {
        name: "Corner Caf\xE9",
        address: "45 Main Road, Sandton, Johannesburg"
      },
      carrier: {
        name: "FastTrack Logistics",
        driver: "Michael Sithole",
        phone: "+27 83 444 5555"
      },
      timeline: [
        {
          timestamp: "2024-08-25T08:00:00Z",
          status: "order_created",
          location: "Your Warehouse",
          message: "Shipment order created and assigned to carrier",
          icon: "package"
        },
        {
          timestamp: "2024-08-25T10:30:00Z",
          status: "picked_up",
          location: "Your Warehouse",
          message: "Items picked up by FastTrack Logistics",
          icon: "truck"
        },
        {
          timestamp: "2024-08-25T12:15:00Z",
          status: "in_transit",
          location: "Halfway Point Depot",
          message: "In transit to destination. ETA: 16:00",
          icon: "route",
          current: true
        },
        {
          timestamp: null,
          status: "out_for_delivery",
          location: "Local Distribution Center",
          message: "Out for delivery - final leg",
          icon: "delivery",
          estimated: "2024-08-25T15:30:00Z"
        },
        {
          timestamp: null,
          status: "delivered",
          location: "Corner Caf\xE9",
          message: "Package delivered",
          icon: "check",
          estimated: "2024-08-25T16:00:00Z"
        }
      ],
      items: [
        { name: "Maize Meal (5kg)", quantity: 10 },
        { name: "Rice (2kg)", quantity: 5 }
      ],
      specialInstructions: "Handle with care - fragile items. Call customer 30 minutes before delivery.",
      nextUpdate: "2024-08-25T14:00:00Z"
    },
    "TN789456124": {
      id: "SHIP-002",
      trackingNumber: "TN789456124",
      status: "delivered",
      currentLocation: {
        name: "Your Warehouse",
        address: "123 Business Street, Alexandra, Johannesburg",
        coordinates: { lat: -26.1017, lng: 28.0875 },
        timestamp: "2024-08-24T17:45:00Z"
      },
      estimatedDelivery: "2024-08-24T18:00:00Z",
      destination: {
        name: "Your Warehouse",
        address: "123 Business Street, Alexandra, Johannesburg"
      },
      carrier: {
        name: "Express Courier Services",
        driver: "Jenny Mthembu",
        phone: "+27 84 333 2222"
      },
      timeline: [
        {
          timestamp: "2024-08-24T14:00:00Z",
          status: "order_created",
          location: "Sunshine Suppliers",
          message: "Express pickup order created",
          icon: "package"
        },
        {
          timestamp: "2024-08-24T16:30:00Z",
          status: "picked_up",
          location: "Sunshine Suppliers",
          message: "Items collected from supplier",
          icon: "truck"
        },
        {
          timestamp: "2024-08-24T17:45:00Z",
          status: "delivered",
          location: "Your Warehouse",
          message: "Successfully delivered and signed for",
          icon: "check",
          current: true
        }
      ],
      items: [
        { name: "Cooking Oil (750ml)", quantity: 24 }
      ],
      signature: "Warehouse Manager - 24/08/2024 17:45",
      proofOfDelivery: "POD-IMG-789456124.jpg",
      deliveryConfirmation: {
        signedBy: "Warehouse Manager",
        signedAt: "2024-08-24T17:45:00Z",
        notes: "All items received in good condition"
      }
    }
  };
  const tracking = demoTrackingData[trackingNumber];
  if (!tracking) {
    throw createError({
      statusCode: 404,
      statusMessage: "Tracking number not found"
    });
  }
  const completedSteps = tracking.timeline.filter((step) => step.timestamp).length;
  const totalSteps = tracking.timeline.length;
  const progressPercentage = Math.round(completedSteps / totalSteps * 100);
  const now = /* @__PURE__ */ new Date();
  const estimatedDelivery = new Date(tracking.estimatedDelivery);
  const isDelayed = tracking.status !== "delivered" && now > estimatedDelivery;
  return {
    success: true,
    data: {
      ...tracking,
      progress: {
        percentage: progressPercentage,
        completedSteps,
        totalSteps,
        isDelayed,
        delayMinutes: isDelayed ? Math.round((now.getTime() - estimatedDelivery.getTime()) / (1e3 * 60)) : 0
      },
      lastUpdated: (/* @__PURE__ */ new Date()).toISOString()
    }
  };
});

const _trackingNumber__get$1 = /*#__PURE__*/Object.freeze(/*#__PURE__*/Object.defineProperty({
  __proto__: null,
  default: _trackingNumber__get
}, Symbol.toStringTag, { value: 'Module' }));

const index_get$6 = defineEventHandler(async (event) => {
  const query = getQuery$1(event);
  const { status = "all", productType = "", limit = 20, search = "" } = query;
  const demoProductionOrders = [
    {
      id: "PROD-001",
      orderNumber: "PO-2024-001",
      productType: "food_processing",
      product: {
        id: "RECIPE-001",
        name: "Custom Spice Blend - Corner Caf\xE9",
        description: "Special spice blend mixture for Corner Caf\xE9's signature dishes",
        recipe: {
          ingredients: [
            { name: "Paprika", quantity: 2.5, unit: "kg", cost: 125 },
            { name: "Cumin", quantity: 1, unit: "kg", cost: 180 },
            { name: "Coriander", quantity: 1.5, unit: "kg", cost: 90 },
            { name: "Black Pepper", quantity: 0.5, unit: "kg", cost: 250 },
            { name: "Turmeric", quantity: 0.8, unit: "kg", cost: 95 }
          ],
          instructions: [
            "Dry roast coriander and cumin seeds separately",
            "Grind all spices to fine powder",
            "Mix according to proportions",
            "Sieve to ensure consistent texture",
            "Package in airtight containers"
          ],
          yieldQuantity: 6.3,
          yieldUnit: "kg"
        }
      },
      customer: {
        id: "CUST-001",
        name: "Corner Caf\xE9",
        contact: "Sarah Johnson",
        specialRequirements: "Halal certified, no MSG"
      },
      quantity: {
        ordered: 25,
        // units (6.3kg each = 157.5kg total)
        produced: 18,
        remaining: 7,
        unit: "batches"
      },
      status: "in_production",
      priority: "medium",
      timeline: {
        orderDate: "2024-08-20T10:00:00Z",
        startDate: "2024-08-22T08:00:00Z",
        estimatedCompletion: "2024-08-27T17:00:00Z",
        actualCompletion: null,
        deliveryDate: "2024-08-28T10:00:00Z"
      },
      costs: {
        materials: 18375,
        // 25 batches × 735 per batch
        labor: 3500,
        overhead: 1200,
        packaging: 650,
        total: 23725,
        sellingPrice: 31250,
        profit: 7525,
        margin: 24.1
      },
      production: {
        batchSize: 1,
        // One recipe = one batch
        batchesPerDay: 4,
        currentBatch: 19,
        qualityChecks: [
          {
            batchNumber: 18,
            checkDate: "2024-08-25T14:00:00Z",
            inspector: "Quality Controller",
            status: "passed",
            notes: "Texture and aroma excellent"
          }
        ],
        equipment: [
          { name: "Industrial Grinder", status: "in_use", nextMaintenance: "2024-09-01" },
          { name: "Commercial Mixer", status: "available", nextMaintenance: "2024-09-15" },
          { name: "Packaging Machine", status: "available", nextMaintenance: "2024-08-30" }
        ]
      },
      materials: {
        allocated: true,
        reservedStock: [
          { itemId: "SPICE-001", name: "Paprika", quantityReserved: 62.5, unit: "kg" },
          { itemId: "SPICE-002", name: "Cumin", quantityReserved: 25, unit: "kg" },
          { itemId: "SPICE-003", name: "Coriander", quantityReserved: 37.5, unit: "kg" },
          { itemId: "SPICE-004", name: "Black Pepper", quantityReserved: 12.5, unit: "kg" },
          { itemId: "SPICE-005", name: "Turmeric", quantityReserved: 20, unit: "kg" }
        ]
      },
      tracking: [
        {
          timestamp: "2024-08-20T10:00:00Z",
          event: "order_received",
          description: "Production order created",
          user: "Sales Team"
        },
        {
          timestamp: "2024-08-21T14:00:00Z",
          event: "materials_allocated",
          description: "All materials reserved from inventory",
          user: "Production Planner"
        },
        {
          timestamp: "2024-08-22T08:00:00Z",
          event: "production_started",
          description: "First batch started",
          user: "Production Supervisor"
        },
        {
          timestamp: "2024-08-25T15:00:00Z",
          event: "milestone_reached",
          description: "18 batches completed (72% done)",
          user: "Production Team"
        }
      ]
    },
    {
      id: "PROD-002",
      orderNumber: "PO-2024-002",
      productType: "packaging",
      product: {
        id: "PACKAGE-001",
        name: "Branded Food Containers - Local Restaurant Co.",
        description: "Custom printed food containers with restaurant branding",
        specifications: {
          material: "Biodegradable cardboard",
          size: "750ml capacity",
          printing: "2-color logo print",
          finish: "Food-safe coating"
        }
      },
      customer: {
        id: "CUST-002",
        name: "Local Restaurant Co.",
        contact: "Restaurant Manager",
        specialRequirements: "Eco-friendly materials only"
      },
      quantity: {
        ordered: 5e3,
        produced: 2800,
        remaining: 2200,
        unit: "pieces"
      },
      status: "in_production",
      priority: "high",
      timeline: {
        orderDate: "2024-08-18T09:00:00Z",
        startDate: "2024-08-21T07:00:00Z",
        estimatedCompletion: "2024-08-29T18:00:00Z",
        actualCompletion: null,
        deliveryDate: "2024-08-30T11:00:00Z"
      },
      costs: {
        materials: 8750,
        labor: 2800,
        overhead: 980,
        setup: 1500,
        // Printing setup costs
        total: 14030,
        sellingPrice: 18500,
        profit: 4470,
        margin: 24.2
      },
      production: {
        batchSize: 500,
        batchesPerDay: 6,
        currentBatch: 6,
        qualityChecks: [
          {
            batchNumber: 5,
            checkDate: "2024-08-25T11:00:00Z",
            inspector: "Print Quality Inspector",
            status: "passed",
            notes: "Print alignment and color consistency good"
          }
        ],
        equipment: [
          { name: "Printing Press", status: "in_use", nextMaintenance: "2024-09-05" },
          { name: "Die Cutting Machine", status: "in_use", nextMaintenance: "2024-09-10" },
          { name: "Folding Equipment", status: "scheduled", nextMaintenance: "2024-08-28" }
        ]
      },
      materials: {
        allocated: true,
        reservedStock: [
          { itemId: "CARDBOARD-001", name: "Biodegradable Cardboard", quantityReserved: 250, unit: "sheets" },
          { itemId: "INK-001", name: "Food-Safe Ink (Black)", quantityReserved: 2.5, unit: "liters" },
          { itemId: "INK-002", name: "Food-Safe Ink (Red)", quantityReserved: 1.8, unit: "liters" },
          { itemId: "COATING-001", name: "Food-Safe Coating", quantityReserved: 15, unit: "liters" }
        ]
      },
      tracking: [
        {
          timestamp: "2024-08-18T09:00:00Z",
          event: "order_received",
          description: "Custom packaging order created",
          user: "Sales Team"
        },
        {
          timestamp: "2024-08-19T15:00:00Z",
          event: "design_approved",
          description: "Customer approved final design proof",
          user: "Design Team"
        },
        {
          timestamp: "2024-08-21T07:00:00Z",
          event: "production_started",
          description: "Printing setup completed, production started",
          user: "Production Team"
        }
      ]
    },
    {
      id: "PROD-003",
      orderNumber: "PO-2024-003",
      productType: "assembly",
      product: {
        id: "ASSEMBLY-001",
        name: "Emergency Food Kits - Williams Construction",
        description: "Pre-assembled emergency food kits for construction site safety compliance",
        components: [
          { name: "Instant Noodles", quantity: 4, unit: "packets" },
          { name: "Energy Bars", quantity: 6, unit: "pieces" },
          { name: "Dried Fruit Mix", quantity: 2, unit: "packets" },
          { name: "Water Purification Tablets", quantity: 10, unit: "tablets" },
          { name: "Emergency Blanket", quantity: 1, unit: "piece" },
          { name: "Waterproof Container", quantity: 1, unit: "piece" }
        ]
      },
      customer: {
        id: "CUST-003",
        name: "Williams Construction",
        contact: "John Williams",
        specialRequirements: "Must meet safety regulations, 24-month shelf life"
      },
      quantity: {
        ordered: 100,
        produced: 100,
        remaining: 0,
        unit: "kits"
      },
      status: "completed",
      priority: "medium",
      timeline: {
        orderDate: "2024-08-10T13:00:00Z",
        startDate: "2024-08-12T08:00:00Z",
        estimatedCompletion: "2024-08-18T17:00:00Z",
        actualCompletion: "2024-08-17T15:30:00Z",
        deliveryDate: "2024-08-19T09:00:00Z"
      },
      costs: {
        materials: 15750,
        labor: 1200,
        overhead: 420,
        packaging: 350,
        total: 17720,
        sellingPrice: 23500,
        profit: 5780,
        margin: 24.6
      },
      production: {
        batchSize: 20,
        batchesPerDay: 5,
        currentBatch: 5,
        qualityChecks: [
          {
            batchNumber: 5,
            checkDate: "2024-08-17T14:00:00Z",
            inspector: "Final Assembly Inspector",
            status: "passed",
            notes: "All components present and properly sealed"
          }
        ],
        equipment: [
          { name: "Assembly Workstation", status: "available", nextMaintenance: "2024-09-01" },
          { name: "Sealing Machine", status: "available", nextMaintenance: "2024-08-30" }
        ]
      },
      materials: {
        allocated: false,
        // Materials consumed
        consumedStock: [
          { itemId: "FOOD-001", name: "Instant Noodles", quantityUsed: 400, unit: "packets" },
          { itemId: "FOOD-002", name: "Energy Bars", quantityUsed: 600, unit: "pieces" },
          { itemId: "FOOD-003", name: "Dried Fruit Mix", quantityUsed: 200, unit: "packets" },
          { itemId: "SAFETY-001", name: "Water Purification Tablets", quantityUsed: 1e3, unit: "tablets" },
          { itemId: "SAFETY-002", name: "Emergency Blankets", quantityUsed: 100, unit: "pieces" },
          { itemId: "CONTAINER-001", name: "Waterproof Containers", quantityUsed: 100, unit: "pieces" }
        ]
      },
      tracking: [
        {
          timestamp: "2024-08-10T13:00:00Z",
          event: "order_received",
          description: "Emergency kit assembly order created",
          user: "Sales Team"
        },
        {
          timestamp: "2024-08-11T10:00:00Z",
          event: "materials_sourced",
          description: "All components sourced and quality verified",
          user: "Procurement Team"
        },
        {
          timestamp: "2024-08-12T08:00:00Z",
          event: "assembly_started",
          description: "Kit assembly process started",
          user: "Assembly Team"
        },
        {
          timestamp: "2024-08-17T15:30:00Z",
          event: "completed",
          description: "All 100 kits assembled and quality checked",
          user: "Production Supervisor"
        },
        {
          timestamp: "2024-08-19T09:00:00Z",
          event: "delivered",
          description: "Kits delivered to construction site",
          user: "Logistics Team"
        }
      ]
    }
  ];
  let filteredOrders = demoProductionOrders;
  if (status !== "all") {
    filteredOrders = filteredOrders.filter((order) => order.status === status);
  }
  if (productType) {
    filteredOrders = filteredOrders.filter((order) => order.productType === productType);
  }
  if (search) {
    const searchLower = search.toLowerCase();
    filteredOrders = filteredOrders.filter(
      (order) => order.product.name.toLowerCase().includes(searchLower) || order.customer.name.toLowerCase().includes(searchLower) || order.orderNumber.toLowerCase().includes(searchLower)
    );
  }
  const limitedOrders = filteredOrders.slice(0, Number(limit));
  const stats = {
    total: filteredOrders.length,
    byStatus: {
      planning: demoProductionOrders.filter((o) => o.status === "planning").length,
      in_production: demoProductionOrders.filter((o) => o.status === "in_production").length,
      quality_check: demoProductionOrders.filter((o) => o.status === "quality_check").length,
      completed: demoProductionOrders.filter((o) => o.status === "completed").length,
      on_hold: demoProductionOrders.filter((o) => o.status === "on_hold").length
    },
    byType: {
      food_processing: demoProductionOrders.filter((o) => o.productType === "food_processing").length,
      packaging: demoProductionOrders.filter((o) => o.productType === "packaging").length,
      assembly: demoProductionOrders.filter((o) => o.productType === "assembly").length
    },
    totals: {
      revenue: demoProductionOrders.reduce((sum, order) => sum + order.costs.sellingPrice, 0),
      costs: demoProductionOrders.reduce((sum, order) => sum + order.costs.total, 0),
      profit: demoProductionOrders.reduce((sum, order) => sum + order.costs.profit, 0)
    },
    averageMargin: Math.round(demoProductionOrders.reduce((sum, order) => sum + order.costs.margin, 0) / demoProductionOrders.length * 10) / 10
  };
  return {
    success: true,
    data: {
      productionOrders: limitedOrders,
      stats,
      pagination: {
        total: filteredOrders.length,
        limit: Number(limit),
        page: 1
      }
    }
  };
});

const index_get$7 = /*#__PURE__*/Object.freeze(/*#__PURE__*/Object.defineProperty({
  __proto__: null,
  default: index_get$6
}, Symbol.toStringTag, { value: 'Module' }));

const index_get$4 = defineEventHandler(async (event) => {
  const query = getQuery$1(event);
  const { unreadOnly = false, limit = 20, type = "" } = query;
  const demoNotifications = [
    {
      id: "notif_1",
      type: "inventory_alert",
      category: "inventory",
      title: "Low Stock Alert",
      message: "Rice (2kg) is running low with only 3 units remaining. Minimum stock level is 10 units.",
      severity: "warning",
      isRead: false,
      actionRequired: true,
      actionUrl: "/inventory/5",
      actionText: "Reorder Now",
      metadata: {
        itemId: "5",
        itemName: "Rice (2kg)",
        currentStock: 3,
        minimumStock: 10,
        suggestedOrder: 25
      },
      createdAt: "2024-08-25T08:00:00Z",
      readAt: null
    },
    {
      id: "notif_2",
      type: "group_purchase",
      category: "collaboration",
      title: "Group Purchase Opportunity",
      message: "Join the bulk flour purchase organized by Alexandra Business Network. Save 18% with minimum 25kg order. Deadline in 3 days.",
      severity: "info",
      isRead: false,
      actionRequired: true,
      actionUrl: "/group-buying/flour-purchase-aug-2024",
      actionText: "Join Purchase",
      metadata: {
        purchaseId: "gp_flour_001",
        organizer: "Alexandra Business Network",
        savings: 18,
        deadline: "2024-08-28T23:59:59Z",
        minimumQuantity: 25,
        currentProgress: 76
      },
      createdAt: "2024-08-25T07:30:00Z",
      readAt: null
    },
    {
      id: "notif_3",
      type: "payment_reminder",
      category: "finance",
      title: "Payment Overdue",
      message: "Williams Construction has an overdue payment of R120.52. Invoice INV-2024-003 was due 2 days ago.",
      severity: "error",
      isRead: false,
      actionRequired: true,
      actionUrl: "/sales/INV-2024-003",
      actionText: "Send Reminder",
      metadata: {
        customerId: "CUST-003",
        customerName: "Williams Construction",
        invoiceId: "INV-2024-003",
        amount: 120.52,
        daysPastDue: 2
      },
      createdAt: "2024-08-25T07:00:00Z",
      readAt: null
    },
    {
      id: "notif_4",
      type: "ai_insight",
      category: "insights",
      title: "Business Performance Insight",
      message: "Your sales have increased 23% this month! Consider expanding your product range to capitalize on growing demand.",
      severity: "success",
      isRead: true,
      actionRequired: false,
      actionUrl: "/dashboard/insights",
      actionText: "View Details",
      metadata: {
        salesGrowth: 23,
        period: "month",
        recommendedActions: ["expand_product_range", "increase_inventory"]
      },
      createdAt: "2024-08-24T16:00:00Z",
      readAt: "2024-08-24T18:30:00Z"
    },
    {
      id: "notif_5",
      type: "tool_request",
      category: "sharing",
      title: "Tool Sharing Request",
      message: "Thabo's Hardware wants to borrow your drill for the weekend. Proposed rental: R50/day.",
      severity: "info",
      isRead: true,
      actionRequired: true,
      actionUrl: "/tools/requests/req_001",
      actionText: "Respond",
      metadata: {
        requesterId: "USER_THABO",
        requesterName: "Thabo's Hardware",
        toolId: "TOOL_DRILL_001",
        toolName: "Cordless Drill",
        proposedRate: 50,
        startDate: "2024-08-26T08:00:00Z",
        endDate: "2024-08-28T18:00:00Z"
      },
      createdAt: "2024-08-24T14:00:00Z",
      readAt: "2024-08-24T15:15:00Z"
    },
    {
      id: "notif_6",
      type: "system_update",
      category: "system",
      title: "System Maintenance",
      message: "Scheduled maintenance tonight from 02:00-04:00. All services will be temporarily unavailable.",
      severity: "warning",
      isRead: false,
      actionRequired: false,
      actionUrl: "/system/maintenance",
      actionText: "Learn More",
      metadata: {
        maintenanceStart: "2024-08-26T02:00:00Z",
        maintenanceEnd: "2024-08-26T04:00:00Z",
        affectedServices: ["api", "web", "mobile"]
      },
      createdAt: "2024-08-24T12:00:00Z",
      readAt: null
    },
    {
      id: "notif_7",
      type: "new_feature",
      category: "system",
      title: "New Feature: Mobile Payments",
      message: "You can now accept payments via SnapScan and Zapper! Enable mobile payments in your POS settings.",
      severity: "success",
      isRead: false,
      actionRequired: false,
      actionUrl: "/settings/payments",
      actionText: "Enable Now",
      metadata: {
        feature: "mobile_payments",
        providers: ["SnapScan", "Zapper"],
        estimatedImpact: "15% sales increase"
      },
      createdAt: "2024-08-24T10:00:00Z",
      readAt: null
    }
  ];
  let filteredNotifications = demoNotifications;
  if (unreadOnly === "true") {
    filteredNotifications = filteredNotifications.filter((notif) => !notif.isRead);
  }
  if (type) {
    filteredNotifications = filteredNotifications.filter((notif) => notif.type === type);
  }
  const limitedNotifications = filteredNotifications.slice(0, Number(limit));
  const totalNotifications = demoNotifications.length;
  const unreadCount = demoNotifications.filter((n) => !n.isRead).length;
  const severityCounts = {
    error: demoNotifications.filter((n) => n.severity === "error").length,
    warning: demoNotifications.filter((n) => n.severity === "warning").length,
    info: demoNotifications.filter((n) => n.severity === "info").length,
    success: demoNotifications.filter((n) => n.severity === "success").length
  };
  const categoryCounts = {
    inventory: demoNotifications.filter((n) => n.category === "inventory").length,
    finance: demoNotifications.filter((n) => n.category === "finance").length,
    collaboration: demoNotifications.filter((n) => n.category === "collaboration").length,
    insights: demoNotifications.filter((n) => n.category === "insights").length,
    sharing: demoNotifications.filter((n) => n.category === "sharing").length,
    system: demoNotifications.filter((n) => n.category === "system").length
  };
  return {
    success: true,
    data: {
      notifications: limitedNotifications,
      summary: {
        total: totalNotifications,
        unread: unreadCount,
        filtered: filteredNotifications.length,
        severityCounts,
        categoryCounts
      }
    }
  };
});

const index_get$5 = /*#__PURE__*/Object.freeze(/*#__PURE__*/Object.defineProperty({
  __proto__: null,
  default: index_get$4
}, Symbol.toStringTag, { value: 'Module' }));

const markRead_post = defineEventHandler(async (event) => {
  const method = getMethod(event);
  if (method !== "POST") {
    throw createError({
      statusCode: 405,
      statusMessage: "Method not allowed"
    });
  }
  const body = await readBody(event);
  const { notificationIds, markAllAsRead = false } = body;
  if (!markAllAsRead && (!notificationIds || !Array.isArray(notificationIds))) {
    throw createError({
      statusCode: 400,
      statusMessage: "notificationIds array is required when markAllAsRead is false"
    });
  }
  const updatedCount = markAllAsRead ? 7 : notificationIds.length;
  return {
    success: true,
    data: {
      updatedCount,
      message: markAllAsRead ? "All notifications marked as read" : `${updatedCount} notification(s) marked as read`,
      timestamp: (/* @__PURE__ */ new Date()).toISOString()
    }
  };
});

const markRead_post$1 = /*#__PURE__*/Object.freeze(/*#__PURE__*/Object.defineProperty({
  __proto__: null,
  default: markRead_post
}, Symbol.toStringTag, { value: 'Module' }));

const setup_post = defineEventHandler(async (event) => {
  const body = await readBody(event);
  const { step, data } = body;
  let result;
  switch (step) {
    case "assess-business":
      result = await assessBusiness(data);
      break;
    case "recommend-services":
      result = await recommendServices(data);
      break;
    case "setup-tenant":
      result = await setupTenant(data);
      break;
    case "activate-services":
      result = await activateInitialServices(data);
      break;
    default:
      throw createError({
        statusCode: 400,
        statusMessage: "Invalid onboarding step"
      });
  }
  return {
    success: true,
    step,
    data: result
  };
});
async function assessBusiness(businessData) {
  const {
    businessName,
    industry,
    size,
    currentRevenue,
    mainChallenges,
    currentSoftware,
    goals
  } = businessData;
  const assessment = {
    businessProfile: {
      maturityLevel: calculateMaturityLevel(size, currentRevenue, currentSoftware),
      automationPotential: assessAutomationPotential(industry, mainChallenges),
      growthProjection: calculateGrowthProjection(currentRevenue, goals),
      riskFactors: identifyRiskFactors(industry, size, mainChallenges)
    },
    opportunities: [
      {
        area: "Revenue Optimization",
        potential: "15-25% increase",
        timeframe: "30-60 days",
        confidence: "High",
        description: "Automated sales processes and pricing optimization"
      },
      {
        area: "Cost Reduction",
        potential: "R2,500-5,000/month savings",
        timeframe: "14-30 days",
        confidence: "Very High",
        description: "Eliminate manual processes and optimize operations"
      },
      {
        area: "Customer Satisfaction",
        potential: "20-30% improvement",
        timeframe: "60-90 days",
        confidence: "High",
        description: "AI-powered customer engagement and personalization"
      }
    ],
    recommendations: generateRecommendations(),
    projectedOutcomes: {
      month1: {
        revenueIncrease: calculateProjectedRevenue(currentRevenue, 0.05),
        costSavings: 1500,
        timesSaved: 15,
        processesAutomated: 5
      },
      month3: {
        revenueIncrease: calculateProjectedRevenue(currentRevenue, 0.15),
        costSavings: 3500,
        timesSaved: 35,
        processesAutomated: 15
      },
      month6: {
        revenueIncrease: calculateProjectedRevenue(currentRevenue, 0.25),
        costSavings: 5e3,
        timesSaved: 50,
        processesAutomated: 25
      }
    }
  };
  return assessment;
}
async function recommendServices(assessmentData) {
  const { businessProfile, opportunities } = assessmentData;
  const recommendations = {
    coreServices: [
      {
        serviceId: "sales-automation",
        priority: "critical",
        reason: "Highest revenue impact potential",
        expectedROI: "400-600%",
        timeToValue: "7-14 days",
        guaranteedOutcome: "15% revenue increase in 60 days",
        monthlyInvestment: 499,
        projectedMonthlyValue: 2500
      },
      {
        serviceId: "customer-engagement",
        priority: "high",
        reason: "Immediate customer retention benefits",
        expectedROI: "300-500%",
        timeToValue: "14-21 days",
        guaranteedOutcome: "20% customer retention improvement",
        monthlyInvestment: 249,
        projectedMonthlyValue: 1200
      }
    ],
    growthServices: [
      {
        serviceId: "inventory-management",
        priority: "medium",
        reason: "Significant cost optimization potential",
        expectedROI: "200-400%",
        timeToValue: "21-30 days",
        guaranteedOutcome: "Zero stockouts, 20% cost reduction",
        monthlyInvestment: 299,
        projectedMonthlyValue: 800,
        prerequisite: "Establish baseline inventory data"
      },
      {
        serviceId: "financial-intelligence",
        priority: "medium",
        reason: "Compliance and operational efficiency",
        expectedROI: "250-400%",
        timeToValue: "30-45 days",
        guaranteedOutcome: "100% compliance, 90% time reduction",
        monthlyInvestment: 299,
        projectedMonthlyValue: 1e3,
        prerequisite: "Connect financial accounts"
      }
    ],
    advancedServices: [
      {
        serviceId: "group-buying",
        priority: "low",
        reason: "Additional cost savings through collaboration",
        expectedROI: "150-300%",
        timeToValue: "45-60 days",
        guaranteedOutcome: "10-30% procurement savings",
        monthlyInvestment: 149,
        projectedMonthlyValue: 400,
        prerequisite: "Active in procurement community"
      }
    ],
    investmentSummary: {
      minimumPackage: {
        services: ["sales-automation", "customer-engagement"],
        monthlyInvestment: 748,
        projectedMonthlyValue: 3700,
        netProfit: 2952,
        roi: 394
      },
      recommendedPackage: {
        services: ["sales-automation", "customer-engagement", "inventory-management"],
        monthlyInvestment: 1047,
        projectedMonthlyValue: 4500,
        netProfit: 3453,
        roi: 330
      },
      comprehensivePackage: {
        services: ["sales-automation", "customer-engagement", "inventory-management", "financial-intelligence"],
        monthlyInvestment: 1346,
        projectedMonthlyValue: 5500,
        netProfit: 4154,
        roi: 309
      }
    }
  };
  return recommendations;
}
async function setupTenant(setupData) {
  const {
    businessData,
    selectedServices,
    billingInfo,
    preferences
  } = setupData;
  const tenantId = generateTenantId();
  const tenant = {
    tenantId,
    businessName: businessData.businessName,
    industry: businessData.industry,
    settings: {
      businessType: businessData.industry,
      automationLevel: preferences.automationLevel || "standard",
      communicationPreferences: preferences.communication || "email",
      reportingFrequency: preferences.reporting || "weekly"
    },
    subscription: {
      tier: billingInfo.tier || "standard",
      services: selectedServices,
      billingCycle: billingInfo.cycle || "monthly",
      startDate: (/* @__PURE__ */ new Date()).toISOString()
    },
    onboarding: {
      status: "setup-complete",
      completedAt: (/* @__PURE__ */ new Date()).toISOString(),
      nextSteps: [
        "Services will begin analyzing your business",
        "Initial outcomes expected within 24-48 hours",
        "Weekly progress reports will be sent"
      ]
    }
  };
  return {
    tenant,
    setupComplete: true,
    nextStep: "activate-services",
    welcomeMessage: `Welcome to TOSS Service as Software! Your business is now set up for autonomous success. Our AI agents will begin working on your behalf immediately.`,
    accessUrl: `https://app.toss.co.za/${tenantId}/dashboard`,
    supportContact: "support@toss.co.za"
  };
}
async function activateInitialServices(activationData) {
  const { tenantId, selectedServices } = activationData;
  const activationResults = [];
  for (const serviceId of selectedServices) {
    const activation = {
      serviceId,
      status: "activating",
      message: `${serviceId} is being set up for your business`,
      estimatedCompletion: calculateActivationTime(serviceId),
      initialActions: getInitialActions(serviceId)
    };
    activationResults.push(activation);
  }
  return {
    tenantId,
    activatedServices: activationResults,
    overallStatus: "activating",
    dashboard: {
      url: `https://app.toss.co.za/${tenantId}/dashboard`,
      firstLoginCode: generateLoginCode()
    },
    timeline: {
      immediate: "AI agents begin business analysis",
      "24hours": "First automated actions executed",
      "48hours": "Initial outcomes and reports available",
      "7days": "First ROI measurements available"
    },
    support: {
      onboardingSpecialist: "Sarah Mitchell",
      email: "onboarding@toss.co.za",
      phone: "+27 11 123 4567",
      scheduleMeeting: "https://calendly.com/toss-onboarding"
    }
  };
}
function calculateMaturityLevel(size, revenue, software) {
  let score = 0;
  if (size === "enterprise") score += 30;
  else if (size === "medium") score += 20;
  else score += 10;
  if (revenue > 1e6) score += 30;
  else if (revenue > 5e5) score += 20;
  else score += 10;
  score += Math.min(software.length * 5, 40);
  if (score >= 70) return "advanced";
  if (score >= 40) return "intermediate";
  return "basic";
}
function assessAutomationPotential(industry, challenges) {
  const highAutomationIndustries = ["retail", "manufacturing", "services"];
  const automationChallenges = ["manual processes", "data entry", "customer management"];
  let potential = 50;
  if (highAutomationIndustries.includes(industry)) potential += 20;
  challenges.forEach((challenge) => {
    if (automationChallenges.some((ac) => challenge.toLowerCase().includes(ac))) {
      potential += 10;
    }
  });
  return Math.min(potential, 95);
}
function calculateGrowthProjection(currentRevenue, goals) {
  const aggressive = goals.some((g) => g.includes("aggressive") || g.includes("rapid"));
  const conservative = goals.some((g) => g.includes("conservative") || g.includes("steady"));
  let growthRate = 0.15;
  if (aggressive) growthRate = 0.25;
  if (conservative) growthRate = 0.1;
  return {
    annualGrowthRate: growthRate,
    projectedRevenue: currentRevenue * (1 + growthRate),
    timeToDouble: Math.ceil(Math.log(2) / Math.log(1 + growthRate))
  };
}
function identifyRiskFactors(industry, size, challenges) {
  const risks = [];
  if (industry === "retail") risks.push("Seasonal demand fluctuations");
  if (size === "small") risks.push("Limited resources for technology adoption");
  if (challenges.includes("cash flow")) risks.push("Financial constraints may limit service adoption");
  return risks;
}
function generateRecommendations(businessData) {
  return [
    "Start with Sales Automation for immediate revenue impact",
    "Add Customer Engagement within 30 days for retention benefits",
    "Consider Group Buying for procurement cost savings",
    "Implement Financial Intelligence for compliance automation"
  ];
}
function calculateProjectedRevenue(current, increase) {
  return Math.round(current * increase);
}
function generateTenantId() {
  return "tenant_" + Math.random().toString(36).substring(2, 15);
}
function calculateActivationTime(serviceId) {
  const times = {
    "sales-automation": "2-4 minutes",
    "customer-engagement": "3-5 minutes",
    "inventory-management": "5-8 minutes",
    "financial-intelligence": "8-12 minutes"
  };
  return times[serviceId] || "5-10 minutes";
}
function getInitialActions(serviceId) {
  const actions = {
    "sales-automation": [
      "Analyze existing sales data",
      "Set up automated invoicing templates",
      "Configure payment reminders",
      "Optimize pricing strategies"
    ],
    "customer-engagement": [
      "Import customer database",
      "Analyze customer behavior patterns",
      "Set up personalized communication templates",
      "Create loyalty reward systems"
    ],
    "inventory-management": [
      "Analyze inventory turnover patterns",
      "Set up automated reorder points",
      "Configure supplier integration",
      "Optimize stock levels"
    ],
    "financial-intelligence": [
      "Connect accounting systems",
      "Set up automated categorization",
      "Configure compliance monitoring",
      "Create financial reporting dashboard"
    ]
  };
  return actions[serviceId] || [];
}
function generateLoginCode() {
  return Math.random().toString(36).substring(2, 8).toUpperCase();
}

const setup_post$1 = /*#__PURE__*/Object.freeze(/*#__PURE__*/Object.defineProperty({
  __proto__: null,
  default: setup_post
}, Symbol.toStringTag, { value: 'Module' }));

const index_get$2 = defineEventHandler(async (event) => {
  const query = getQuery$1(event);
  const { status = "all", priority = "", limit = 20, search = "" } = query;
  const demoProjects = [
    {
      id: "PROJ-001",
      title: "Kitchen Renovation - Corner Caf\xE9",
      description: "Complete kitchen equipment upgrade and renovation project for Corner Caf\xE9 including new appliances, layout redesign, and equipment installation.",
      client: {
        id: "CUST-001",
        name: "Corner Caf\xE9",
        contact: "Sarah Johnson",
        email: "sarah@cornercafe.co.za",
        phone: "+27 82 555 0123"
      },
      status: "in_progress",
      priority: "high",
      budget: {
        estimated: 125e3,
        actual: 87500,
        remaining: 37500,
        currency: "ZAR"
      },
      timeline: {
        startDate: "2024-08-15T08:00:00Z",
        estimatedEndDate: "2024-09-15T17:00:00Z",
        actualEndDate: null,
        daysRemaining: 21
      },
      progress: {
        percentage: 70,
        milestonesCompleted: 7,
        totalMilestones: 10,
        tasksCompleted: 28,
        totalTasks: 40
      },
      team: [
        {
          id: "USER_001",
          name: "Project Manager",
          role: "Project Manager",
          allocation: 100,
          contact: "+27 11 123 4567"
        },
        {
          id: "USER_002",
          name: "Kitchen Designer",
          role: "Design Lead",
          allocation: 75,
          contact: "+27 82 111 2222"
        },
        {
          id: "USER_003",
          name: "Installation Team",
          role: "Installation",
          allocation: 80,
          contact: "+27 83 333 4444"
        }
      ],
      milestones: [
        {
          id: "MILE-001",
          title: "Design Approval",
          status: "completed",
          dueDate: "2024-08-20T17:00:00Z",
          completedDate: "2024-08-19T15:30:00Z"
        },
        {
          id: "MILE-002",
          title: "Equipment Procurement",
          status: "completed",
          dueDate: "2024-08-25T17:00:00Z",
          completedDate: "2024-08-24T14:00:00Z"
        },
        {
          id: "MILE-003",
          title: "Installation Phase 1",
          status: "in_progress",
          dueDate: "2024-08-30T17:00:00Z",
          completedDate: null
        },
        {
          id: "MILE-004",
          title: "Final Testing & Handover",
          status: "pending",
          dueDate: "2024-09-15T17:00:00Z",
          completedDate: null
        }
      ],
      resources: [
        {
          type: "equipment",
          name: "Commercial Oven",
          quantity: 1,
          cost: 25e3,
          status: "delivered"
        },
        {
          type: "equipment",
          name: "Industrial Refrigerator",
          quantity: 2,
          cost: 35e3,
          status: "installed"
        },
        {
          type: "material",
          name: "Stainless Steel Counters",
          quantity: 4,
          cost: 15e3,
          status: "pending"
        }
      ],
      risks: [
        {
          id: "RISK-001",
          description: "Potential delay in custom counter delivery",
          impact: "medium",
          probability: "low",
          mitigation: "Alternative supplier identified as backup"
        }
      ],
      documents: [
        {
          name: "Kitchen Design Plans.pdf",
          type: "design",
          uploadedAt: "2024-08-18T10:00:00Z",
          size: "2.5MB"
        },
        {
          name: "Equipment Specifications.xlsx",
          type: "specifications",
          uploadedAt: "2024-08-20T14:30:00Z",
          size: "1.2MB"
        }
      ]
    },
    {
      id: "PROJ-002",
      title: "Inventory Management System Setup",
      description: "Implementation of new inventory tracking system for Williams Construction including barcode scanning, automated reordering, and reporting dashboards.",
      client: {
        id: "CUST-003",
        name: "Williams Construction",
        contact: "John Williams",
        email: "john@williams-construction.co.za",
        phone: "+27 82 777 8888"
      },
      status: "planning",
      priority: "medium",
      budget: {
        estimated: 45e3,
        actual: 5e3,
        remaining: 4e4,
        currency: "ZAR"
      },
      timeline: {
        startDate: "2024-09-01T08:00:00Z",
        estimatedEndDate: "2024-10-15T17:00:00Z",
        actualEndDate: null,
        daysRemaining: 51
      },
      progress: {
        percentage: 15,
        milestonesCompleted: 1,
        totalMilestones: 6,
        tasksCompleted: 3,
        totalTasks: 20
      },
      team: [
        {
          id: "USER_004",
          name: "System Analyst",
          role: "System Design",
          allocation: 60,
          contact: "+27 11 987 6543"
        },
        {
          id: "USER_005",
          name: "Implementation Specialist",
          role: "Technical Lead",
          allocation: 80,
          contact: "+27 84 555 6666"
        }
      ],
      milestones: [
        {
          id: "MILE-005",
          title: "Requirements Gathering",
          status: "completed",
          dueDate: "2024-08-30T17:00:00Z",
          completedDate: "2024-08-28T16:00:00Z"
        },
        {
          id: "MILE-006",
          title: "System Configuration",
          status: "pending",
          dueDate: "2024-09-15T17:00:00Z",
          completedDate: null
        }
      ],
      resources: [
        {
          type: "software",
          name: "Inventory Management License",
          quantity: 1,
          cost: 15e3,
          status: "ordered"
        },
        {
          type: "hardware",
          name: "Barcode Scanners",
          quantity: 5,
          cost: 8e3,
          status: "pending"
        }
      ],
      risks: [
        {
          id: "RISK-002",
          description: "Integration complexity with existing systems",
          impact: "high",
          probability: "medium",
          mitigation: "Detailed technical assessment planned"
        }
      ],
      documents: [
        {
          name: "Requirements Document.docx",
          type: "requirements",
          uploadedAt: "2024-08-25T11:00:00Z",
          size: "856KB"
        }
      ]
    },
    {
      id: "PROJ-003",
      title: "Group Buying Platform Integration",
      description: "Development and integration of collaborative purchasing platform for local business network including vendor management and logistics coordination.",
      client: {
        id: "CUST-INTERNAL",
        name: "Internal Development",
        contact: "Development Team",
        email: "dev@tosserp.co.za",
        phone: "+27 11 000 0000"
      },
      status: "completed",
      priority: "high",
      budget: {
        estimated: 8e4,
        actual: 75e3,
        remaining: 5e3,
        currency: "ZAR"
      },
      timeline: {
        startDate: "2024-07-01T08:00:00Z",
        estimatedEndDate: "2024-08-15T17:00:00Z",
        actualEndDate: "2024-08-12T15:30:00Z",
        daysRemaining: 0
      },
      progress: {
        percentage: 100,
        milestonesCompleted: 8,
        totalMilestones: 8,
        tasksCompleted: 35,
        totalTasks: 35
      },
      team: [
        {
          id: "USER_006",
          name: "Lead Developer",
          role: "Development Lead",
          allocation: 100,
          contact: "+27 82 444 5555"
        },
        {
          id: "USER_007",
          name: "UI/UX Designer",
          role: "Design",
          allocation: 50,
          contact: "+27 83 666 7777"
        }
      ],
      milestones: [
        {
          id: "MILE-007",
          title: "Platform Architecture",
          status: "completed",
          dueDate: "2024-07-15T17:00:00Z",
          completedDate: "2024-07-14T12:00:00Z"
        },
        {
          id: "MILE-008",
          title: "Core Features Development",
          status: "completed",
          dueDate: "2024-08-01T17:00:00Z",
          completedDate: "2024-07-30T16:45:00Z"
        },
        {
          id: "MILE-009",
          title: "Testing & Deployment",
          status: "completed",
          dueDate: "2024-08-15T17:00:00Z",
          completedDate: "2024-08-12T15:30:00Z"
        }
      ],
      resources: [
        {
          type: "development",
          name: "Cloud Infrastructure",
          quantity: 1,
          cost: 12e3,
          status: "active"
        },
        {
          type: "software",
          name: "Development Tools",
          quantity: 1,
          cost: 8e3,
          status: "active"
        }
      ],
      risks: [],
      documents: [
        {
          name: "Technical Specification.pdf",
          type: "technical",
          uploadedAt: "2024-07-10T09:00:00Z",
          size: "3.2MB"
        },
        {
          name: "Final Report.pdf",
          type: "report",
          uploadedAt: "2024-08-12T16:00:00Z",
          size: "1.8MB"
        }
      ]
    }
  ];
  let filteredProjects = demoProjects;
  if (status !== "all") {
    filteredProjects = filteredProjects.filter((proj) => proj.status === status);
  }
  if (priority) {
    filteredProjects = filteredProjects.filter((proj) => proj.priority === priority);
  }
  if (search) {
    const searchLower = search.toLowerCase();
    filteredProjects = filteredProjects.filter(
      (proj) => proj.title.toLowerCase().includes(searchLower) || proj.description.toLowerCase().includes(searchLower) || proj.client.name.toLowerCase().includes(searchLower)
    );
  }
  const limitedProjects = filteredProjects.slice(0, Number(limit));
  const stats = {
    total: filteredProjects.length,
    byStatus: {
      planning: demoProjects.filter((p) => p.status === "planning").length,
      in_progress: demoProjects.filter((p) => p.status === "in_progress").length,
      completed: demoProjects.filter((p) => p.status === "completed").length,
      on_hold: demoProjects.filter((p) => p.status === "on_hold").length
    },
    byPriority: {
      high: demoProjects.filter((p) => p.priority === "high").length,
      medium: demoProjects.filter((p) => p.priority === "medium").length,
      low: demoProjects.filter((p) => p.priority === "low").length
    },
    budget: {
      totalEstimated: demoProjects.reduce((sum, proj) => sum + proj.budget.estimated, 0),
      totalActual: demoProjects.reduce((sum, proj) => sum + proj.budget.actual, 0),
      totalRemaining: demoProjects.reduce((sum, proj) => sum + proj.budget.remaining, 0)
    },
    averageProgress: Math.round(demoProjects.reduce((sum, proj) => sum + proj.progress.percentage, 0) / demoProjects.length)
  };
  return {
    success: true,
    data: {
      projects: limitedProjects,
      stats,
      pagination: {
        total: filteredProjects.length,
        limit: Number(limit),
        page: 1
      }
    }
  };
});

const index_get$3 = /*#__PURE__*/Object.freeze(/*#__PURE__*/Object.defineProperty({
  __proto__: null,
  default: index_get$2
}, Symbol.toStringTag, { value: 'Module' }));

const index_post$2 = defineEventHandler(async (event) => {
  const method = getMethod(event);
  if (method !== "POST") {
    throw createError({
      statusCode: 405,
      statusMessage: "Method not allowed"
    });
  }
  const body = await readBody(event);
  const {
    title,
    description,
    clientId,
    priority = "medium",
    estimatedBudget,
    startDate,
    estimatedEndDate,
    teamMembers = [],
    milestones = []
  } = body;
  if (!title || !description || !clientId || !estimatedBudget || !startDate || !estimatedEndDate) {
    throw createError({
      statusCode: 400,
      statusMessage: "Title, description, clientId, estimatedBudget, startDate, and estimatedEndDate are required"
    });
  }
  if (estimatedBudget <= 0) {
    throw createError({
      statusCode: 400,
      statusMessage: "Estimated budget must be greater than 0"
    });
  }
  const start = new Date(startDate);
  const end = new Date(estimatedEndDate);
  if (start >= end) {
    throw createError({
      statusCode: 400,
      statusMessage: "End date must be after start date"
    });
  }
  const projectId = `PROJ-${Date.now()}`;
  const demoClients = {
    "CUST-001": { name: "Corner Caf\xE9", contact: "Sarah Johnson", email: "sarah@cornercafe.co.za", phone: "+27 82 555 0123" },
    "CUST-002": { name: "Local Restaurant Co.", contact: "Restaurant Manager", email: "manager@localrestaurant.co.za", phone: "+27 82 999 1111" },
    "CUST-003": { name: "Williams Construction", contact: "John Williams", email: "john@williams-construction.co.za", phone: "+27 82 777 8888" }
  };
  const client = demoClients[clientId];
  if (!client) {
    throw createError({
      statusCode: 400,
      statusMessage: "Invalid client ID"
    });
  }
  const timelineStart = new Date(startDate);
  const timelineEnd = new Date(estimatedEndDate);
  const daysTotal = Math.ceil((timelineEnd.getTime() - timelineStart.getTime()) / (1e3 * 60 * 60 * 24));
  const daysRemaining = Math.max(0, Math.ceil((timelineEnd.getTime() - (/* @__PURE__ */ new Date()).getTime()) / (1e3 * 60 * 60 * 24)));
  const newProject = {
    id: projectId,
    title,
    description,
    client: {
      id: clientId,
      ...client
    },
    status: "planning",
    priority,
    budget: {
      estimated: estimatedBudget,
      actual: 0,
      remaining: estimatedBudget,
      currency: "ZAR"
    },
    timeline: {
      startDate,
      estimatedEndDate,
      actualEndDate: null,
      daysTotal,
      daysRemaining
    },
    progress: {
      percentage: 0,
      milestonesCompleted: 0,
      totalMilestones: milestones.length,
      tasksCompleted: 0,
      totalTasks: 0
    },
    team: teamMembers.map((member) => ({
      id: member.id || `USER_${Date.now()}_${Math.random()}`,
      name: member.name,
      role: member.role,
      allocation: member.allocation || 100,
      contact: member.contact || ""
    })),
    milestones: milestones.map((milestone, index) => ({
      id: `MILE_${projectId}_${index + 1}`,
      title: milestone.title,
      description: milestone.description || "",
      status: "pending",
      dueDate: milestone.dueDate,
      completedDate: null,
      dependencies: milestone.dependencies || []
    })),
    resources: [],
    risks: [],
    documents: [],
    createdAt: (/* @__PURE__ */ new Date()).toISOString(),
    createdBy: "Current User",
    // Would be from auth context
    lastUpdated: (/* @__PURE__ */ new Date()).toISOString()
  };
  return {
    success: true,
    data: {
      project: newProject,
      message: `Project "${title}" created successfully`,
      nextSteps: [
        "Add team members if not already specified",
        "Define detailed milestones and tasks",
        "Upload relevant documents",
        "Set up project tracking and reporting"
      ]
    }
  };
});

const index_post$3 = /*#__PURE__*/Object.freeze(/*#__PURE__*/Object.defineProperty({
  __proto__: null,
  default: index_post$2
}, Symbol.toStringTag, { value: 'Module' }));

const index_get = defineEventHandler(async (event) => {
  const query = getQuery$1(event);
  const { page = 1, limit = 20, status = "", customerId = "", startDate = "", endDate = "" } = query;
  const demoSales = [
    {
      id: "INV-2024-001",
      invoiceNumber: "INV-2024-001",
      customerId: "CUST-001",
      customerName: "Sipho Mthembu",
      customerEmail: "sipho@email.com",
      customerPhone: "+27 82 123 4567",
      saleDate: "2024-08-24T10:30:00Z",
      dueDate: "2024-09-07T23:59:59Z",
      status: "paid",
      paymentMethod: "cash",
      paymentStatus: "completed",
      subtotal: 156.5,
      taxAmount: 22.98,
      discountAmount: 0,
      totalAmount: 179.48,
      paidAmount: 179.48,
      balanceAmount: 0,
      items: [
        {
          id: "1",
          name: "Maize Meal (1kg)",
          sku: "MM001",
          quantity: 3,
          unitPrice: 22.99,
          totalPrice: 68.97
        },
        {
          id: "2",
          name: "Sunflower Oil (750ml)",
          sku: "SO002",
          quantity: 2,
          unitPrice: 39.99,
          totalPrice: 79.98
        },
        {
          id: "4",
          name: "Coca Cola (500ml)",
          sku: "CC004",
          quantity: 1,
          unitPrice: 14.99,
          totalPrice: 14.99
        }
      ],
      notes: "Regular customer - monthly grocery shopping",
      salesPersonId: "USER-001",
      salesPersonName: "Nomsa Dlamini",
      createdAt: "2024-08-24T10:30:00Z",
      updatedAt: "2024-08-24T10:35:00Z"
    },
    {
      id: "INV-2024-002",
      invoiceNumber: "INV-2024-002",
      customerId: "CUST-002",
      customerName: "Mary Johnson",
      customerEmail: "mary@email.com",
      customerPhone: "+27 71 987 6543",
      saleDate: "2024-08-24T14:15:00Z",
      dueDate: "2024-09-07T23:59:59Z",
      status: "pending",
      paymentMethod: "credit",
      paymentStatus: "pending",
      subtotal: 89.97,
      taxAmount: 13.5,
      discountAmount: 5,
      totalAmount: 98.47,
      paidAmount: 0,
      balanceAmount: 98.47,
      items: [
        {
          id: "3",
          name: "Bread (White Loaf)",
          sku: "BR003",
          quantity: 2,
          unitPrice: 16.99,
          totalPrice: 33.98
        },
        {
          id: "5",
          name: "Rice (2kg)",
          sku: "RC005",
          quantity: 1,
          unitPrice: 65.99,
          totalPrice: 65.99
        }
      ],
      notes: "Credit customer - 30 days payment terms",
      salesPersonId: "USER-002",
      salesPersonName: "Thabo Molefe",
      createdAt: "2024-08-24T14:15:00Z",
      updatedAt: "2024-08-24T14:20:00Z"
    },
    {
      id: "INV-2024-003",
      invoiceNumber: "INV-2024-003",
      customerId: "CUST-003",
      customerName: "David Williams",
      customerEmail: "david@email.com",
      customerPhone: "+27 83 456 7890",
      saleDate: "2024-08-23T16:45:00Z",
      dueDate: "2024-09-06T23:59:59Z",
      status: "partially_paid",
      paymentMethod: "card",
      paymentStatus: "partial",
      subtotal: 245.89,
      taxAmount: 36.88,
      discountAmount: 12.25,
      totalAmount: 270.52,
      paidAmount: 150,
      balanceAmount: 120.52,
      items: [
        {
          id: "1",
          name: "Maize Meal (1kg)",
          sku: "MM001",
          quantity: 5,
          unitPrice: 22.99,
          totalPrice: 114.95
        },
        {
          id: "2",
          name: "Sunflower Oil (750ml)",
          sku: "SO002",
          quantity: 3,
          unitPrice: 39.99,
          totalPrice: 119.97
        },
        {
          id: "4",
          name: "Coca Cola (500ml)",
          sku: "CC004",
          quantity: 4,
          unitPrice: 14.99,
          totalPrice: 59.96
        }
      ],
      notes: "Bulk purchase - regular business customer",
      salesPersonId: "USER-001",
      salesPersonName: "Nomsa Dlamini",
      createdAt: "2024-08-23T16:45:00Z",
      updatedAt: "2024-08-24T09:15:00Z"
    }
  ];
  let filteredSales = demoSales;
  if (status) {
    filteredSales = filteredSales.filter((sale) => sale.status === status);
  }
  if (customerId) {
    filteredSales = filteredSales.filter((sale) => sale.customerId === customerId);
  }
  if (startDate) {
    filteredSales = filteredSales.filter(
      (sale) => new Date(sale.saleDate) >= new Date(startDate.toString())
    );
  }
  if (endDate) {
    filteredSales = filteredSales.filter(
      (sale) => new Date(sale.saleDate) <= new Date(endDate.toString())
    );
  }
  const totalItems = filteredSales.length;
  const totalPages = Math.ceil(totalItems / Number(limit));
  const startIndex = (Number(page) - 1) * Number(limit);
  const endIndex = startIndex + Number(limit);
  const paginatedSales = filteredSales.slice(startIndex, endIndex);
  const totalSalesAmount = filteredSales.reduce((sum, sale) => sum + sale.totalAmount, 0);
  const totalPaidAmount = filteredSales.reduce((sum, sale) => sum + sale.paidAmount, 0);
  const totalOutstandingAmount = filteredSales.reduce((sum, sale) => sum + sale.balanceAmount, 0);
  const averageOrderValue = filteredSales.length > 0 ? totalSalesAmount / filteredSales.length : 0;
  const statusCounts = {
    paid: filteredSales.filter((s) => s.status === "paid").length,
    pending: filteredSales.filter((s) => s.status === "pending").length,
    partially_paid: filteredSales.filter((s) => s.status === "partially_paid").length,
    cancelled: filteredSales.filter((s) => s.status === "cancelled").length
  };
  return {
    success: true,
    data: {
      sales: paginatedSales,
      pagination: {
        currentPage: Number(page),
        totalPages,
        totalItems,
        itemsPerPage: Number(limit),
        hasNextPage: Number(page) < totalPages,
        hasPreviousPage: Number(page) > 1
      },
      summary: {
        totalSales: filteredSales.length,
        totalSalesAmount,
        totalPaidAmount,
        totalOutstandingAmount,
        averageOrderValue,
        statusCounts
      }
    }
  };
});

const index_get$1 = /*#__PURE__*/Object.freeze(/*#__PURE__*/Object.defineProperty({
  __proto__: null,
  default: index_get
}, Symbol.toStringTag, { value: 'Module' }));

const index_post = defineEventHandler(async (event) => {
  var _a;
  const body = await readBody(event);
  const { customerId, items } = body;
  if (!customerId || !items || !Array.isArray(items) || items.length === 0) {
    throw createError({
      statusCode: 400,
      statusMessage: "Missing required fields: customerId and items array"
    });
  }
  for (const item of items) {
    if (!item.itemId || !item.quantity || !item.unitPrice) {
      throw createError({
        statusCode: 400,
        statusMessage: "Each item must have itemId, quantity, and unitPrice"
      });
    }
    if (item.quantity <= 0 || item.unitPrice <= 0) {
      throw createError({
        statusCode: 400,
        statusMessage: "Item quantity and unitPrice must be positive numbers"
      });
    }
  }
  const invoiceNumber = `INV-${(/* @__PURE__ */ new Date()).getFullYear()}-${String(Math.floor(Math.random() * 9999) + 1).padStart(3, "0")}`;
  let subtotal = 0;
  const processedItems = items.map((item) => {
    const totalPrice = Number(item.quantity) * Number(item.unitPrice);
    subtotal += totalPrice;
    return {
      itemId: item.itemId,
      name: item.name || "Unknown Item",
      sku: item.sku || "",
      quantity: Number(item.quantity),
      unitPrice: Number(item.unitPrice),
      totalPrice,
      discount: Number(item.discount) || 0
    };
  });
  const discountAmount = Number(body.discountAmount) || 0;
  const taxRate = Number(body.taxRate) || 0.15;
  const taxAmount = (subtotal - discountAmount) * taxRate;
  const totalAmount = subtotal - discountAmount + taxAmount;
  const newSale = {
    id: Math.random().toString(36).substr(2, 9),
    invoiceNumber,
    customerId,
    customerName: body.customerName || "",
    customerEmail: body.customerEmail || "",
    customerPhone: body.customerPhone || "",
    saleDate: (/* @__PURE__ */ new Date()).toISOString(),
    dueDate: body.dueDate || new Date(Date.now() + 30 * 24 * 60 * 60 * 1e3).toISOString(),
    // 30 days from now
    status: "pending",
    paymentMethod: body.paymentMethod || "cash",
    paymentStatus: "pending",
    subtotal,
    taxAmount,
    discountAmount,
    totalAmount,
    paidAmount: 0,
    balanceAmount: totalAmount,
    items: processedItems,
    notes: ((_a = body.notes) == null ? void 0 : _a.trim()) || "",
    salesPersonId: body.salesPersonId || "system",
    salesPersonName: body.salesPersonName || "System",
    createdAt: (/* @__PURE__ */ new Date()).toISOString(),
    updatedAt: (/* @__PURE__ */ new Date()).toISOString()
  };
  return {
    success: true,
    message: "Sale created successfully",
    data: newSale
  };
});

const index_post$1 = /*#__PURE__*/Object.freeze(/*#__PURE__*/Object.defineProperty({
  __proto__: null,
  default: index_post
}, Symbol.toStringTag, { value: 'Module' }));

const analytics_get = defineEventHandler(async (event) => {
  const tenant = await requireTenant(event);
  const query = getQuery$1(event);
  const period = query.period || "current-month";
  const analytics = await generateUsageAnalytics(tenant.tenantId, period);
  return {
    success: true,
    data: {
      tenant: {
        id: tenant.tenantId,
        name: tenant.tenantName,
        plan: tenant.subscription.plan
      },
      period,
      billing: analytics.billing,
      usage: analytics.usage,
      outcomes: analytics.outcomes,
      roi: analytics.roi,
      recommendations: analytics.recommendations
    }
  };
});
async function generateUsageAnalytics(tenantId, period) {
  const isCurrentMonth = period === "current-month";
  return {
    billing: {
      totalCost: 1247.5,
      breakdown: [
        { service: "Inventory Management", cost: 299, model: "subscription" },
        { service: "Sales Automation", cost: 678.5, model: "percentage", rate: "2% of additional revenue" },
        { service: "Customer Engagement", cost: 195, model: "per-customer", rate: "R5 per active customer" },
        { service: "Financial Reports", cost: 75, model: "outcome", count: 3 }
      ],
      projectedMonthlyCost: isCurrentMonth ? 1450 : 1247.5,
      costSavings: 2890,
      // Amount saved vs traditional software
      netValue: 4142.5
      // Total value delivered minus costs
    },
    usage: {
      totalServiceExecutions: 156,
      totalAPICallsMade: 2847,
      totalComputeTimeMs: 45629,
      dataProcessedMB: 127.3,
      servicesActive: 4,
      automationLevel: 89,
      // Percentage of tasks automated
      uptime: 99.97
    },
    outcomes: {
      inventoryOptimizations: 23,
      stockoutsPrevented: 8,
      invoicesGenerated: 67,
      paymentsCollected: 45,
      customerMessagesPersonalized: 234,
      financialReportsGenerated: 3,
      complianceChecksCompleted: 12,
      costSavingsIdentified: 2890
    },
    roi: {
      totalInvestment: 1247.5,
      totalReturns: 8732.4,
      roiPercentage: 600.2,
      paybackPeriodDays: 18,
      additionalRevenueGenerated: 5641.9,
      costsSaved: 2890.5,
      timeHoursSaved: 47.5,
      productivityIncrease: 340
    },
    recommendations: [
      {
        type: "optimization",
        title: "Upgrade Customer Engagement Service",
        description: "Adding advanced segmentation could increase customer retention by 15%",
        potentialValue: 3400,
        implementationCost: 150,
        confidence: 0.87
      },
      {
        type: "expansion",
        title: "Add Logistics Automation",
        description: "Shared delivery network could save R800/month on delivery costs",
        potentialValue: 800,
        implementationCost: 0,
        confidence: 0.92
      },
      {
        type: "integration",
        title: "Connect Accounting Software",
        description: "Direct integration with your accountant could save 5 hours/month",
        potentialValue: 1250,
        implementationCost: 99,
        confidence: 0.94
      }
    ]
  };
}

const analytics_get$1 = /*#__PURE__*/Object.freeze(/*#__PURE__*/Object.defineProperty({
  __proto__: null,
  default: analytics_get
}, Symbol.toStringTag, { value: 'Module' }));

const catalog_get = defineEventHandler(async (event) => {
  const tenant = await requireTenant(event);
  const availableServices = Object.entries(SERVICE_REGISTRY).filter(
    ([_, service]) => service.requirements.every((req) => tenant.subscription.features.includes(req))
  ).map(([key, service]) => ({
    id: key,
    name: service.name,
    description: service.description,
    category: service.category,
    outcomes: service.outcomes,
    pricing: service.pricing,
    requirements: service.requirements,
    // Add tenant-specific pricing calculations
    estimatedCost: calculateEstimatedCost(service, tenant),
    estimatedROI: calculateEstimatedROI(service, tenant)
  }));
  return {
    success: true,
    data: {
      tenant: {
        id: tenant.tenantId,
        name: tenant.tenantName,
        plan: tenant.subscription.plan,
        businessType: tenant.settings.businessType
      },
      services: availableServices,
      totalServices: availableServices.length,
      categories: [...new Set(availableServices.map((s) => s.category))],
      summary: {
        totalPotentialSavings: availableServices.reduce((sum, s) => sum + (s.estimatedROI || 0), 0),
        averageROI: availableServices.reduce((sum, s) => sum + (s.estimatedROI || 0), 0) / availableServices.length
      }
    }
  };
});
function calculateEstimatedCost(service, tenant) {
  switch (service.pricing.model) {
    case "usage":
      const businessSizeMultiplier = tenant.subscription.plan === "enterprise" ? 3 : tenant.subscription.plan === "professional" ? 2 : 1;
      return businessSizeMultiplier * 100;
    // Base estimation
    case "outcome":
      return 250;
    // Average outcome-based pricing
    case "subscription":
      return service.pricing.basePrice || 299;
    default:
      return 199;
  }
}
function calculateEstimatedROI(service, tenant) {
  const businessMultipliers = {
    "salon": { "inventory-management": 1200, "sales-automation": 2500, "customer-engagement": 1800 },
    "retail": { "inventory-management": 3500, "sales-automation": 4200, "financial-intelligence": 1500 },
    "restaurant": { "inventory-management": 2800, "customer-engagement": 2200, "sales-automation": 3100 }
  };
  const businessType = tenant.settings.businessType || "retail";
  const serviceMap = businessMultipliers[businessType] || businessMultipliers["retail"];
  return serviceMap[service.name.toLowerCase().replace(/[^a-z]/g, "-")] || 1e3;
}

const catalog_get$1 = /*#__PURE__*/Object.freeze(/*#__PURE__*/Object.defineProperty({
  __proto__: null,
  default: catalog_get
}, Symbol.toStringTag, { value: 'Module' }));

const execute_post = defineEventHandler(async (event) => {
  const tenant = await requireTenant(event);
  const body = await readBody(event);
  const { service, parameters = {}, userId = "demo-user" } = body;
  const serviceContext = createServiceContext(tenant, userId, "owner");
  const orchestrator = new ServiceOrchestrator(serviceContext, tenant);
  try {
    const execution = await orchestrator.executeService(service, parameters);
    await trackServiceUsage(serviceContext, service, "execution", {
      parameters,
      cost: execution.cost,
      outcomes: execution.outcomes.length
    });
    return {
      success: true,
      data: {
        executionId: execution.serviceId + "_" + Date.now(),
        service,
        status: execution.status,
        outcomes: execution.outcomes,
        cost: execution.cost,
        usage: execution.usage,
        startTime: execution.startTime,
        endTime: execution.endTime,
        message: generateServiceMessage(service, execution)
      }
    };
  } catch (error) {
    return {
      success: false,
      error: {
        message: error.message,
        code: "SERVICE_EXECUTION_FAILED",
        service
      }
    };
  }
});
function generateServiceMessage(service, execution) {
  switch (service) {
    case "inventory-management":
      return `\u2705 Inventory service executed successfully! I've analyzed your stock levels, identified ${execution.outcomes.length} optimization opportunities, and handled ${execution.usage.apiCalls} supplier communications. You'll never run out of critical items again.`;
    case "sales-automation":
      return `\u2705 Sales automation activated! I've processed ${execution.outcomes.length} sales actions, sent invoices automatically, and optimized your pricing. Your revenue is now being maximized 24/7.`;
    case "customer-engagement":
      return `\u2705 Customer AI is now active! I've analyzed ${execution.outcomes.length} customer interactions, personalized communications, and activated loyalty programs. Your customers will love the personal touch.`;
    case "financial-intelligence":
      return `\u2705 Financial reporting complete! I've generated comprehensive reports, ensured tax compliance, and identified cost-saving opportunities. Your financial health is now optimized.`;
    default:
      return `\u2705 Service "${service}" executed successfully with ${execution.outcomes.length} positive outcomes delivered.`;
  }
}

const execute_post$1 = /*#__PURE__*/Object.freeze(/*#__PURE__*/Object.defineProperty({
  __proto__: null,
  default: execute_post
}, Symbol.toStringTag, { value: 'Module' }));

const manage_post = defineEventHandler(async (event) => {
  const tenant = await requireTenant(event);
  const body = await readBody(event);
  const { action, serviceId, configuration } = body;
  let result;
  switch (action) {
    case "activate":
      result = await activateService(tenant, serviceId, configuration);
      break;
    case "pause":
      result = await pauseService(tenant, serviceId);
      break;
    case "configure":
      result = await configureService(tenant, serviceId, configuration);
      break;
    case "cancel":
      result = await cancelService(tenant, serviceId);
      break;
    default:
      throw createError({
        statusCode: 400,
        statusMessage: "Invalid action"
      });
  }
  return {
    success: true,
    data: result
  };
});
async function activateService(tenant, serviceId, config) {
  var _a;
  const service = getServiceDefinition(serviceId);
  return {
    serviceId,
    status: "activating",
    message: `${service.name} is being activated for your business`,
    estimatedSetup: "2-5 minutes",
    configuration: config,
    billing: {
      startDate: (/* @__PURE__ */ new Date()).toISOString(),
      billingCycle: service.billingCycle,
      rate: service.pricing[((_a = tenant.subscription) == null ? void 0 : _a.tier) || "standard"]
    },
    expectedOutcomes: service.guaranteedOutcomes,
    nextSteps: [
      "AI agent will analyze your current business data",
      "Service will begin delivering outcomes within 24 hours",
      "You will receive progress updates via dashboard"
    ]
  };
}
async function pauseService(tenant, serviceId) {
  return {
    serviceId,
    status: "paused",
    message: "Service has been paused. You will not be charged while paused.",
    pausedAt: (/* @__PURE__ */ new Date()).toISOString(),
    resumeOptions: {
      immediate: "Resume service immediately",
      scheduled: "Schedule automatic resume",
      conditional: "Resume when specific conditions are met"
    }
  };
}
async function configureService(tenant, serviceId, config) {
  return {
    serviceId,
    status: "configured",
    message: "Service configuration updated successfully",
    configuration: config,
    impact: calculateConfigurationImpact(),
    effectiveDate: (/* @__PURE__ */ new Date()).toISOString()
  };
}
async function cancelService(tenant, serviceId) {
  return {
    serviceId,
    status: "cancelled",
    message: "Service has been cancelled",
    cancelledAt: (/* @__PURE__ */ new Date()).toISOString(),
    finalBilling: {
      usageThisPeriod: 147.5,
      valueDelivered: 2890.3,
      refund: 0,
      finalInvoiceDate: new Date(Date.now() + 7 * 24 * 60 * 60 * 1e3).toISOString()
    },
    dataRetention: {
      businessData: "Retained for 90 days",
      reports: "Available for download for 30 days",
      automationSettings: "Archived for 1 year"
    }
  };
}
function getServiceDefinition(serviceId) {
  const services = {
    "inventory-management": {
      name: "Autonomous Inventory Management",
      description: "AI manages your inventory to prevent stockouts and optimize costs",
      billingCycle: "monthly",
      pricing: {
        starter: 199,
        standard: 299,
        premium: 399
      },
      guaranteedOutcomes: [
        "Zero stockouts guaranteed",
        "Reduce inventory costs by 15-25%",
        "Automated supplier negotiations",
        "Predictive demand planning"
      ]
    },
    "sales-automation": {
      name: "Intelligent Sales Processing",
      description: "AI handles invoicing, payments, and customer follow-ups",
      billingCycle: "monthly",
      pricing: {
        starter: 299,
        standard: 499,
        premium: 699
      },
      guaranteedOutcomes: [
        "Increase revenue by 10-20%",
        "Reduce payment delays by 50%",
        "Automated customer communications",
        "Smart pricing optimization"
      ]
    },
    "customer-engagement": {
      name: "AI Customer Relationship",
      description: "AI manages customer relationships to maximize satisfaction and retention",
      billingCycle: "monthly",
      pricing: {
        starter: 149,
        standard: 249,
        premium: 349
      },
      guaranteedOutcomes: [
        "Increase customer retention by 20%",
        "Improve satisfaction scores by 15%",
        "Automated loyalty programs",
        "Personalized customer experiences"
      ]
    },
    "financial-intelligence": {
      name: "Automated Financial Management",
      description: "AI handles bookkeeping, compliance, and financial optimization",
      billingCycle: "monthly",
      pricing: {
        starter: 199,
        standard: 299,
        premium: 449
      },
      guaranteedOutcomes: [
        "100% tax compliance guaranteed",
        "Reduce bookkeeping time by 90%",
        "Automated expense categorization",
        "Real-time financial insights"
      ]
    }
  };
  return services[serviceId] || null;
}
function calculateConfigurationImpact(serviceId, config) {
  return {
    performanceChange: "+5%",
    costChange: "+R50/month",
    timeToEffect: "24 hours",
    affectedOutcomes: [
      "Inventory turnover rate will improve by 8%",
      "Automated reorder points will be more aggressive"
    ]
  };
}

const manage_post$1 = /*#__PURE__*/Object.freeze(/*#__PURE__*/Object.defineProperty({
  __proto__: null,
  default: manage_post
}, Symbol.toStringTag, { value: 'Module' }));

function renderPayloadResponse(ssrContext) {
  return {
    body: stringify(splitPayload(ssrContext).payload, ssrContext._payloadReducers) ,
    statusCode: getResponseStatus(ssrContext.event),
    statusMessage: getResponseStatusText(ssrContext.event),
    headers: {
      "content-type": "application/json;charset=utf-8" ,
      "x-powered-by": "Nuxt"
    }
  };
}
function renderPayloadJsonScript(opts) {
  const contents = opts.data ? stringify(opts.data, opts.ssrContext._payloadReducers) : "";
  const payload = {
    "type": "application/json",
    "innerHTML": contents,
    "data-nuxt-data": appId,
    "data-ssr": !(opts.ssrContext.noSSR)
  };
  {
    payload.id = "__NUXT_DATA__";
  }
  if (opts.src) {
    payload["data-src"] = opts.src;
  }
  const config = uneval(opts.ssrContext.config);
  return [
    payload,
    {
      innerHTML: `window.__NUXT__={};window.__NUXT__.config=${config}`
    }
  ];
}
function splitPayload(ssrContext) {
  const { data, prerenderedAt, ...initial } = ssrContext.payload;
  return {
    initial: { ...initial, prerenderedAt },
    payload: { data, prerenderedAt }
  };
}

const renderSSRHeadOptions = {"omitLineBreaks":false};

globalThis.__buildAssetsURL = buildAssetsURL;
globalThis.__publicAssetsURL = publicAssetsURL;
const HAS_APP_TELEPORTS = !!(appTeleportAttrs.id);
const APP_TELEPORT_OPEN_TAG = HAS_APP_TELEPORTS ? `<${appTeleportTag}${propsToString(appTeleportAttrs)}>` : "";
const APP_TELEPORT_CLOSE_TAG = HAS_APP_TELEPORTS ? `</${appTeleportTag}>` : "";
const PAYLOAD_URL_RE = /^[^?]*\/_payload.json(?:\?.*)?$/ ;
const renderer = defineRenderHandler(async (event) => {
  const nitroApp = useNitroApp();
  const ssrError = event.path.startsWith("/__nuxt_error") ? getQuery$1(event) : null;
  if (ssrError && !("__unenv__" in event.node.req)) {
    throw createError({
      statusCode: 404,
      statusMessage: "Page Not Found: /__nuxt_error"
    });
  }
  const ssrContext = createSSRContext(event);
  const headEntryOptions = { mode: "server" };
  ssrContext.head.push(appHead, headEntryOptions);
  if (ssrError) {
    ssrError.statusCode &&= Number.parseInt(ssrError.statusCode);
    setSSRError(ssrContext, ssrError);
  }
  const isRenderingPayload = PAYLOAD_URL_RE.test(ssrContext.url);
  if (isRenderingPayload) {
    const url = ssrContext.url.substring(0, ssrContext.url.lastIndexOf("/")) || "/";
    ssrContext.url = url;
    event._path = event.node.req.url = url;
  }
  const routeOptions = getRouteRules(event);
  if (routeOptions.ssr === false) {
    ssrContext.noSSR = true;
  }
  const renderer = await getRenderer(ssrContext);
  const _rendered = await renderer.renderToString(ssrContext).catch(async (error) => {
    if (ssrContext._renderResponse && error.message === "skipping render") {
      return {};
    }
    const _err = !ssrError && ssrContext.payload?.error || error;
    await ssrContext.nuxt?.hooks.callHook("app:error", _err);
    throw _err;
  });
  const inlinedStyles = [];
  await ssrContext.nuxt?.hooks.callHook("app:rendered", { ssrContext, renderResult: _rendered });
  if (ssrContext._renderResponse) {
    return ssrContext._renderResponse;
  }
  if (ssrContext.payload?.error && !ssrError) {
    throw ssrContext.payload.error;
  }
  if (isRenderingPayload) {
    const response = renderPayloadResponse(ssrContext);
    return response;
  }
  const NO_SCRIPTS = routeOptions.noScripts;
  const { styles, scripts } = getRequestDependencies(ssrContext, renderer.rendererContext);
  if (ssrContext._preloadManifest && !NO_SCRIPTS) {
    ssrContext.head.push({
      link: [
        { rel: "preload", as: "fetch", fetchpriority: "low", crossorigin: "anonymous", href: buildAssetsURL(`builds/meta/${ssrContext.runtimeConfig.app.buildId}.json`) }
      ]
    }, { ...headEntryOptions, tagPriority: "low" });
  }
  if (inlinedStyles.length) {
    ssrContext.head.push({ style: inlinedStyles });
  }
  const link = [];
  for (const resource of Object.values(styles)) {
    if ("inline" in getQuery(resource.file)) {
      continue;
    }
    link.push({ rel: "stylesheet", href: renderer.rendererContext.buildAssetsURL(resource.file), crossorigin: "" });
  }
  if (link.length) {
    ssrContext.head.push({ link }, headEntryOptions);
  }
  if (!NO_SCRIPTS) {
    ssrContext.head.push({
      link: getPreloadLinks(ssrContext, renderer.rendererContext)
    }, headEntryOptions);
    ssrContext.head.push({
      link: getPrefetchLinks(ssrContext, renderer.rendererContext)
    }, headEntryOptions);
    ssrContext.head.push({
      script: renderPayloadJsonScript({ ssrContext, data: ssrContext.payload }) 
    }, {
      ...headEntryOptions,
      // this should come before another end of body scripts
      tagPosition: "bodyClose",
      tagPriority: "high"
    });
  }
  if (!routeOptions.noScripts) {
    const tagPosition = "head";
    ssrContext.head.push({
      script: Object.values(scripts).map((resource) => ({
        type: resource.module ? "module" : null,
        src: renderer.rendererContext.buildAssetsURL(resource.file),
        defer: resource.module ? null : true,
        // if we are rendering script tag payloads that import an async payload
        // we need to ensure this resolves before executing the Nuxt entry
        tagPosition,
        crossorigin: ""
      }))
    }, headEntryOptions);
  }
  const { headTags, bodyTags, bodyTagsOpen, htmlAttrs, bodyAttrs } = await renderSSRHead(ssrContext.head, renderSSRHeadOptions);
  const htmlContext = {
    htmlAttrs: htmlAttrs ? [htmlAttrs] : [],
    head: normalizeChunks([headTags]),
    bodyAttrs: bodyAttrs ? [bodyAttrs] : [],
    bodyPrepend: normalizeChunks([bodyTagsOpen, ssrContext.teleports?.body]),
    body: [
      replaceIslandTeleports(ssrContext, _rendered.html) ,
      APP_TELEPORT_OPEN_TAG + (HAS_APP_TELEPORTS ? joinTags([ssrContext.teleports?.[`#${appTeleportAttrs.id}`]]) : "") + APP_TELEPORT_CLOSE_TAG
    ],
    bodyAppend: [bodyTags]
  };
  await nitroApp.hooks.callHook("render:html", htmlContext, { event });
  return {
    body: renderHTMLDocument(htmlContext),
    statusCode: getResponseStatus(event),
    statusMessage: getResponseStatusText(event),
    headers: {
      "content-type": "text/html;charset=utf-8",
      "x-powered-by": "Nuxt"
    }
  };
});
function normalizeChunks(chunks) {
  return chunks.filter(Boolean).map((i) => i.trim());
}
function joinTags(tags) {
  return tags.join("");
}
function joinAttrs(chunks) {
  if (chunks.length === 0) {
    return "";
  }
  return " " + chunks.join(" ");
}
function renderHTMLDocument(html) {
  return `<!DOCTYPE html><html${joinAttrs(html.htmlAttrs)}><head>${joinTags(html.head)}</head><body${joinAttrs(html.bodyAttrs)}>${joinTags(html.bodyPrepend)}${joinTags(html.body)}${joinTags(html.bodyAppend)}</body></html>`;
}

const renderer$1 = /*#__PURE__*/Object.freeze(/*#__PURE__*/Object.defineProperty({
  __proto__: null,
  default: renderer
}, Symbol.toStringTag, { value: 'Module' }));
//# sourceMappingURL=index.mjs.map
