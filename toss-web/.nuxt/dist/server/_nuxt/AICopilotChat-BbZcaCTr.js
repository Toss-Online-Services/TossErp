import { defineComponent, ref, watch, nextTick, unref, mergeProps, useSSRContext } from "vue";
import { ssrRenderAttrs, ssrRenderList, ssrRenderClass, ssrInterpolate, ssrRenderStyle, ssrRenderAttr, ssrIncludeBooleanAttr } from "vue/server-renderer";
const _sfc_main = /* @__PURE__ */ defineComponent({
  __name: "AICopilotChat",
  __ssrInlineRender: true,
  setup(__props) {
    const isVisible = ref(true);
    const isOpen = ref(false);
    const isTyping = ref(false);
    const newMessage = ref("");
    const messagesContainer = ref();
    const messages = ref([
      {
        id: "1",
        type: "bot",
        content: "Hello! I'm your AI business co-pilot. I can help you with inventory management, sales analysis, group purchasing, and much more. What would you like to know?",
        timestamp: /* @__PURE__ */ new Date()
      }
    ]);
    const quickActions = [
      { text: "Show inventory status" },
      { text: "Today's sales summary" },
      { text: "Find group orders" },
      { text: "Check cash flow" },
      { text: "Order supplies" }
    ];
    function messageClass(message) {
      return message.type === "user" ? "flex justify-end" : "flex justify-start";
    }
    function formatTime(timestamp) {
      return timestamp.toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" });
    }
    function scrollToBottom() {
      if (messagesContainer.value) {
        messagesContainer.value.scrollTop = messagesContainer.value.scrollHeight;
      }
    }
    watch(messages, () => {
      nextTick(() => {
        scrollToBottom();
      });
    }, { deep: true });
    return (_ctx, _push, _parent, _attrs) => {
      if (unref(isVisible)) {
        _push(`<div${ssrRenderAttrs(mergeProps({ class: "fixed bottom-4 right-4 z-50" }, _attrs))}>`);
        if (unref(isOpen)) {
          _push(`<div class="bg-white dark:bg-gray-800 rounded-lg shadow-2xl border border-gray-200 dark:border-gray-700 w-80 h-96 flex flex-col"><div class="flex items-center justify-between p-4 border-b border-gray-200 dark:border-gray-700"><div class="flex items-center space-x-2"><div class="w-8 h-8 bg-gradient-to-r from-green-400 to-green-600 rounded-full flex items-center justify-center"><svg class="w-4 h-4 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.663 17h4.673M12 3v1m6.364 1.636l-.707.707M21 12h-1M4 12H3m3.343-5.657l-.707-.707m2.828 9.9a5 5 0 117.072 0l-.548.547A3.374 3.374 0 0014 18.469V19a2 2 0 11-4 0v-.531c0-.895-.356-1.754-.988-2.386l-.548-.547z"></path></svg></div><div><h3 class="text-sm font-semibold text-gray-900 dark:text-white">AI Co-Pilot</h3><p class="text-xs text-green-600 dark:text-green-400">Online</p></div></div><button class="text-gray-400 hover:text-gray-600 dark:hover:text-gray-300"><svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path></svg></button></div><div class="flex-1 overflow-y-auto p-4 space-y-4"><!--[-->`);
          ssrRenderList(unref(messages), (message) => {
            _push(`<div class="${ssrRenderClass(messageClass(message))}">`);
            if (message.type === "bot") {
              _push(`<div class="flex items-start space-x-2"><div class="w-6 h-6 bg-green-500 rounded-full flex items-center justify-center flex-shrink-0"><svg class="w-3 h-3 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.663 17h4.673M12 3v1m6.364 1.636l-.707.707M21 12h-1M4 12H3m3.343-5.657l-.707-.707m2.828 9.9a5 5 0 117.072 0l-.548.547A3.374 3.374 0 0014 18.469V19a2 2 0 11-4 0v-.531c0-.895-.356-1.754-.988-2.386l-.548-.547z"></path></svg></div><div class="bg-gray-100 dark:bg-gray-700 rounded-lg p-3 max-w-xs"><p class="text-sm text-gray-900 dark:text-white">${message.content ?? ""}</p><p class="text-xs text-gray-500 dark:text-gray-400 mt-1">${ssrInterpolate(formatTime(message.timestamp))}</p></div></div>`);
            } else {
              _push(`<div class="flex items-start space-x-2 justify-end"><div class="bg-blue-600 rounded-lg p-3 max-w-xs"><p class="text-sm text-white">${ssrInterpolate(message.content)}</p><p class="text-xs text-blue-200 mt-1">${ssrInterpolate(formatTime(message.timestamp))}</p></div><div class="w-6 h-6 bg-blue-600 rounded-full flex items-center justify-center flex-shrink-0"><span class="text-white text-xs font-medium">You</span></div></div>`);
            }
            _push(`</div>`);
          });
          _push(`<!--]-->`);
          if (unref(isTyping)) {
            _push(`<div class="flex items-start space-x-2"><div class="w-6 h-6 bg-green-500 rounded-full flex items-center justify-center flex-shrink-0"><svg class="w-3 h-3 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.663 17h4.673M12 3v1m6.364 1.636l-.707.707M21 12h-1M4 12H3m3.343-5.657l-.707-.707m2.828 9.9a5 5 0 117.072 0l-.548.547A3.374 3.374 0 0014 18.469V19a2 2 0 11-4 0v-.531c0-.895-.356-1.754-.988-2.386l-.548-.547z"></path></svg></div><div class="bg-gray-100 dark:bg-gray-700 rounded-lg p-3"><div class="flex space-x-1"><div class="w-2 h-2 bg-gray-400 rounded-full animate-bounce"></div><div class="w-2 h-2 bg-gray-400 rounded-full animate-bounce" style="${ssrRenderStyle({ "animation-delay": "0.1s" })}"></div><div class="w-2 h-2 bg-gray-400 rounded-full animate-bounce" style="${ssrRenderStyle({ "animation-delay": "0.2s" })}"></div></div></div></div>`);
          } else {
            _push(`<!---->`);
          }
          _push(`</div><div class="p-4 border-t border-gray-200 dark:border-gray-700"><div class="flex space-x-2"><input${ssrRenderAttr("value", unref(newMessage))}${ssrIncludeBooleanAttr(unref(isTyping)) ? " disabled" : ""} type="text" placeholder="Ask me anything about your business..." class="flex-1 px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white placeholder-gray-500 dark:placeholder-gray-400 focus:ring-2 focus:ring-blue-500 focus:border-transparent text-sm"><button${ssrIncludeBooleanAttr(!unref(newMessage).trim() || unref(isTyping)) ? " disabled" : ""} class="bg-blue-600 text-white p-2 rounded-lg hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"><svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 19l9 2-9-18-9 18 9-2zm0 0v-8"></path></svg></button></div><div class="mt-2 flex flex-wrap gap-1"><!--[-->`);
          ssrRenderList(quickActions, (action) => {
            _push(`<button class="px-2 py-1 text-xs bg-gray-100 dark:bg-gray-700 text-gray-700 dark:text-gray-300 rounded hover:bg-gray-200 dark:hover:bg-gray-600 transition-colors">${ssrInterpolate(action.text)}</button>`);
          });
          _push(`<!--]--></div></div></div>`);
        } else {
          _push(`<button class="bg-gradient-to-r from-green-400 to-green-600 text-white p-4 rounded-full shadow-lg hover:shadow-xl transition-all duration-200 hover:scale-105"><svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 12h.01M12 12h.01M16 12h.01M21 12c0 4.418-4.03 8-9 8a9.863 9.863 0 01-4.255-.949L3 20l1.395-3.72C3.512 15.042 3 13.574 3 12c0-4.418 4.03-8 9-8s9 3.582 9 8z"></path></svg></button>`);
        }
        _push(`</div>`);
      } else {
        _push(`<!---->`);
      }
    };
  }
});
const _sfc_setup = _sfc_main.setup;
_sfc_main.setup = (props, ctx) => {
  const ssrContext = useSSRContext();
  (ssrContext.modules || (ssrContext.modules = /* @__PURE__ */ new Set())).add("components/AICopilotChat.vue");
  return _sfc_setup ? _sfc_setup(props, ctx) : void 0;
};
export {
  _sfc_main as default
};
//# sourceMappingURL=AICopilotChat-BbZcaCTr.js.map
