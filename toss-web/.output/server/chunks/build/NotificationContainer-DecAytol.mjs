import { defineComponent, ref, mergeProps, useSSRContext } from 'vue';
import { ssrRenderAttrs, ssrRenderList, ssrRenderClass, ssrInterpolate } from 'vue/server-renderer';
import { useRouter } from 'vue-router';
import { _ as _export_sfc } from './server.mjs';
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

const _sfc_main = /* @__PURE__ */ defineComponent({
  __name: "NotificationContainer",
  __ssrInlineRender: true,
  setup(__props, { expose: __expose }) {
    const router = useRouter();
    const notifications = ref([
      {
        id: "1",
        type: "info",
        title: "Group Purchase Available",
        message: "A new bulk order for office supplies is available. Join now to save 25%!",
        actions: [
          { label: "View Details", action: () => router.push("/group-buying"), primary: true },
          { label: "Dismiss", action: () => {
          } }
        ]
      },
      {
        id: "2",
        type: "warning",
        title: "Low Stock Alert",
        message: "You have 3 items running low in inventory. Consider reordering soon.",
        actions: [
          { label: "View Inventory", action: () => router.push("/inventory"), primary: true },
          { label: "Dismiss", action: () => {
          } }
        ]
      }
    ]);
    function getNotificationClasses(type) {
      switch (type) {
        case "success":
          return "border-green-200 dark:border-green-800 bg-green-50 dark:bg-green-900/20";
        case "error":
          return "border-red-200 dark:border-red-800 bg-red-50 dark:bg-red-900/20";
        case "warning":
          return "border-yellow-200 dark:border-yellow-800 bg-yellow-50 dark:bg-yellow-900/20";
        case "info":
          return "border-blue-200 dark:border-blue-800 bg-blue-50 dark:bg-blue-900/20";
        default:
          return "";
      }
    }
    function getIconClasses(type) {
      switch (type) {
        case "success":
          return "text-green-600";
        case "error":
          return "text-red-600";
        case "warning":
          return "text-yellow-600";
        case "info":
          return "text-blue-600";
        default:
          return "text-gray-600";
      }
    }
    function removeNotification(id) {
      const index = notifications.value.findIndex((n) => n.id === id);
      if (index > -1) {
        notifications.value.splice(index, 1);
      }
    }
    function addNotification(notification) {
      notifications.value.push(notification);
      if (notification.duration) {
        setTimeout(() => {
          removeNotification(notification.id);
        }, notification.duration);
      }
    }
    __expose({
      addNotification,
      removeNotification
    });
    return (_ctx, _push, _parent, _attrs) => {
      _push(`<div${ssrRenderAttrs(mergeProps({ class: "fixed top-4 right-4 z-50 space-y-4" }, _attrs))} data-v-2b80c1c0><div${ssrRenderAttrs({
        name: "notification",
        class: "space-y-4"
      })} data-v-2b80c1c0>`);
      ssrRenderList(notifications.value, (notification) => {
        _push(`<div class="${ssrRenderClass([getNotificationClasses(notification.type), "bg-white dark:bg-gray-800 rounded-lg shadow-lg border border-gray-200 dark:border-gray-700 p-4 max-w-sm"])}" data-v-2b80c1c0><div class="flex items-start space-x-3" data-v-2b80c1c0><div class="flex-shrink-0" data-v-2b80c1c0><svg class="${ssrRenderClass([getIconClasses(notification.type), "w-5 h-5"])}" fill="currentColor" viewBox="0 0 20 20" data-v-2b80c1c0>`);
        if (notification.type === "success") {
          _push(`<path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd" data-v-2b80c1c0></path>`);
        } else if (notification.type === "warning") {
          _push(`<path fill-rule="evenodd" d="M8.485 3.495c.673-1.167 2.357-1.167 3.03 0l6.28 10.875c.673 1.167-.17 2.625-1.516 2.625H3.72c-1.347 0-2.189-1.458-1.515-2.625L8.485 3.495zM10 6a.75.75 0 01.75.75v3.5a.75.75 0 01-1.5 0v-3.5A.75.75 0 0110 6zm0 9a1 1 0 100-2 1 1 0 000 2z" clip-rule="evenodd" data-v-2b80c1c0></path>`);
        } else if (notification.type === "error") {
          _push(`<path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.28 7.22a.75.75 0 00-1.06 1.06L8.94 10l-1.72 1.72a.75.75 0 101.06 1.06L10 11.06l1.72 1.72a.75.75 0 101.06-1.06L11.06 10l1.72-1.72a.75.75 0 00-1.06-1.06L10 8.94 8.28 7.22z" clip-rule="evenodd" data-v-2b80c1c0></path>`);
        } else {
          _push(`<path fill-rule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zM9 9a.75.75 0 000 1.5h.253a.25.25 0 01.244.304l-.459 2.066A1.75 1.75 0 0010.747 15H11a.75.75 0 000-1.5h-.253a.25.25 0 01-.244-.304l.459-2.066A1.75 1.75 0 009.253 9H9z" clip-rule="evenodd" data-v-2b80c1c0></path>`);
        }
        _push(`</svg></div><div class="flex-1" data-v-2b80c1c0><h4 class="text-sm font-medium text-gray-900 dark:text-white" data-v-2b80c1c0>${ssrInterpolate(notification.title)}</h4><p class="text-sm text-gray-600 dark:text-gray-400 mt-1" data-v-2b80c1c0>${ssrInterpolate(notification.message)}</p>`);
        if (notification.actions) {
          _push(`<div class="mt-3 flex space-x-2" data-v-2b80c1c0><!--[-->`);
          ssrRenderList(notification.actions, (action) => {
            _push(`<button class="${ssrRenderClass([action.primary ? "bg-blue-600 text-white hover:bg-blue-700" : "bg-gray-100 dark:bg-gray-700 text-gray-700 dark:text-gray-300 hover:bg-gray-200 dark:hover:bg-gray-600", "px-3 py-1 text-xs rounded-lg transition-colors"])}" data-v-2b80c1c0>${ssrInterpolate(action.label)}</button>`);
          });
          _push(`<!--]--></div>`);
        } else {
          _push(`<!---->`);
        }
        _push(`</div><button class="flex-shrink-0 text-gray-400 hover:text-gray-600 dark:hover:text-gray-300" data-v-2b80c1c0><svg class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20" data-v-2b80c1c0><path d="M6.28 5.22a.75.75 0 00-1.06 1.06L8.94 10l-3.72 3.72a.75.75 0 101.06 1.06L10 11.06l3.72 3.72a.75.75 0 101.06-1.06L11.06 10l3.72-3.72a.75.75 0 00-1.06-1.06L10 8.94 6.28 5.22z" data-v-2b80c1c0></path></svg></button></div></div>`);
      });
      _push(`</div></div>`);
    };
  }
});
const _sfc_setup = _sfc_main.setup;
_sfc_main.setup = (props, ctx) => {
  const ssrContext = useSSRContext();
  (ssrContext.modules || (ssrContext.modules = /* @__PURE__ */ new Set())).add("components/NotificationContainer.vue");
  return _sfc_setup ? _sfc_setup(props, ctx) : void 0;
};
const __nuxt_component_2 = /* @__PURE__ */ _export_sfc(_sfc_main, [["__scopeId", "data-v-2b80c1c0"]]);

export { __nuxt_component_2 as default };
//# sourceMappingURL=NotificationContainer-DecAytol.mjs.map
