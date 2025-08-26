
import type { DefineComponent, SlotsType } from 'vue'
type IslandComponent<T extends DefineComponent> = T & DefineComponent<{}, {refresh: () => Promise<void>}, {}, {}, {}, {}, {}, {}, {}, {}, {}, {}, SlotsType<{ fallback: { error: unknown } }>>
type HydrationStrategies = {
  hydrateOnVisible?: IntersectionObserverInit | true
  hydrateOnIdle?: number | true
  hydrateOnInteraction?: keyof HTMLElementEventMap | Array<keyof HTMLElementEventMap> | true
  hydrateOnMediaQuery?: string
  hydrateAfter?: number
  hydrateWhen?: boolean
  hydrateNever?: true
}
type LazyComponent<T> = (T & DefineComponent<HydrationStrategies, {}, {}, {}, {}, {}, {}, { hydrated: () => void }>)
interface _GlobalComponents {
      'AICopilotChat': typeof import("../components/AICopilotChat.vue")['default']
    'AppNavigation': typeof import("../components/AppNavigation.vue")['default']
    'ContactDetailsModal': typeof import("../components/ContactDetailsModal.vue")['default']
    'ContactModal': typeof import("../components/ContactModal.vue")['default']
    'CustomerDetailsModal': typeof import("../components/CustomerDetailsModal.vue")['default']
    'CustomerModal': typeof import("../components/CustomerModal.vue")['default']
    'ModuleCard': typeof import("../components/ModuleCard.vue")['default']
    'NotificationContainer': typeof import("../components/NotificationContainer.vue")['default']
    'SubscriptionModal': typeof import("../components/SubscriptionModal.vue")['default']
    'CommonThemeSwitcher': typeof import("../components/common/ThemeSwitcher.vue")['default']
    'CommonUserMenu': typeof import("../components/common/UserMenu.vue")['default']
    'CrmContactDetailsModal': typeof import("../components/crm/ContactDetailsModal.vue")['default']
    'CrmContactModal': typeof import("../components/crm/ContactModal.vue")['default']
    'CrmLeadDetailPanel': typeof import("../components/crm/LeadDetailPanel.vue")['default']
    'IconsIconBase': typeof import("../components/icons/IconBase.vue")['default']
    'LayoutHeader': typeof import("../components/layout/Header.vue")['default']
    'LayoutSidebar': typeof import("../components/layout/Sidebar.vue")['default']
    'NuxtWelcome': typeof import("../node_modules/nuxt/dist/app/components/welcome.vue")['default']
    'NuxtLayout': typeof import("../node_modules/nuxt/dist/app/components/nuxt-layout")['default']
    'NuxtErrorBoundary': typeof import("../node_modules/nuxt/dist/app/components/nuxt-error-boundary.vue")['default']
    'ClientOnly': typeof import("../node_modules/nuxt/dist/app/components/client-only")['default']
    'DevOnly': typeof import("../node_modules/nuxt/dist/app/components/dev-only")['default']
    'ServerPlaceholder': typeof import("../node_modules/nuxt/dist/app/components/server-placeholder")['default']
    'NuxtLink': typeof import("../node_modules/nuxt/dist/app/components/nuxt-link")['default']
    'NuxtLoadingIndicator': typeof import("../node_modules/nuxt/dist/app/components/nuxt-loading-indicator")['default']
    'NuxtTime': typeof import("../node_modules/nuxt/dist/app/components/nuxt-time.vue")['default']
    'NuxtRouteAnnouncer': typeof import("../node_modules/nuxt/dist/app/components/nuxt-route-announcer")['default']
    'NuxtImg': typeof import("../node_modules/nuxt/dist/app/components/nuxt-stubs")['NuxtImg']
    'NuxtPicture': typeof import("../node_modules/nuxt/dist/app/components/nuxt-stubs")['NuxtPicture']
    'ColorScheme': typeof import("../node_modules/@nuxtjs/color-mode/dist/runtime/component.vue3.vue")['default']
    'NuxtPage': typeof import("../node_modules/nuxt/dist/pages/runtime/page")['default']
    'NoScript': typeof import("../node_modules/nuxt/dist/head/runtime/components")['NoScript']
    'Link': typeof import("../node_modules/nuxt/dist/head/runtime/components")['Link']
    'Base': typeof import("../node_modules/nuxt/dist/head/runtime/components")['Base']
    'Title': typeof import("../node_modules/nuxt/dist/head/runtime/components")['Title']
    'Meta': typeof import("../node_modules/nuxt/dist/head/runtime/components")['Meta']
    'Style': typeof import("../node_modules/nuxt/dist/head/runtime/components")['Style']
    'Head': typeof import("../node_modules/nuxt/dist/head/runtime/components")['Head']
    'Html': typeof import("../node_modules/nuxt/dist/head/runtime/components")['Html']
    'Body': typeof import("../node_modules/nuxt/dist/head/runtime/components")['Body']
    'NuxtIsland': typeof import("../node_modules/nuxt/dist/app/components/nuxt-island")['default']
    'NuxtRouteAnnouncer': typeof import("../node_modules/nuxt/dist/app/components/server-placeholder")['default']
      'LazyAICopilotChat': LazyComponent<typeof import("../components/AICopilotChat.vue")['default']>
    'LazyAppNavigation': LazyComponent<typeof import("../components/AppNavigation.vue")['default']>
    'LazyContactDetailsModal': LazyComponent<typeof import("../components/ContactDetailsModal.vue")['default']>
    'LazyContactModal': LazyComponent<typeof import("../components/ContactModal.vue")['default']>
    'LazyCustomerDetailsModal': LazyComponent<typeof import("../components/CustomerDetailsModal.vue")['default']>
    'LazyCustomerModal': LazyComponent<typeof import("../components/CustomerModal.vue")['default']>
    'LazyModuleCard': LazyComponent<typeof import("../components/ModuleCard.vue")['default']>
    'LazyNotificationContainer': LazyComponent<typeof import("../components/NotificationContainer.vue")['default']>
    'LazySubscriptionModal': LazyComponent<typeof import("../components/SubscriptionModal.vue")['default']>
    'LazyCommonThemeSwitcher': LazyComponent<typeof import("../components/common/ThemeSwitcher.vue")['default']>
    'LazyCommonUserMenu': LazyComponent<typeof import("../components/common/UserMenu.vue")['default']>
    'LazyCrmContactDetailsModal': LazyComponent<typeof import("../components/crm/ContactDetailsModal.vue")['default']>
    'LazyCrmContactModal': LazyComponent<typeof import("../components/crm/ContactModal.vue")['default']>
    'LazyCrmLeadDetailPanel': LazyComponent<typeof import("../components/crm/LeadDetailPanel.vue")['default']>
    'LazyIconsIconBase': LazyComponent<typeof import("../components/icons/IconBase.vue")['default']>
    'LazyLayoutHeader': LazyComponent<typeof import("../components/layout/Header.vue")['default']>
    'LazyLayoutSidebar': LazyComponent<typeof import("../components/layout/Sidebar.vue")['default']>
    'LazyNuxtWelcome': LazyComponent<typeof import("../node_modules/nuxt/dist/app/components/welcome.vue")['default']>
    'LazyNuxtLayout': LazyComponent<typeof import("../node_modules/nuxt/dist/app/components/nuxt-layout")['default']>
    'LazyNuxtErrorBoundary': LazyComponent<typeof import("../node_modules/nuxt/dist/app/components/nuxt-error-boundary.vue")['default']>
    'LazyClientOnly': LazyComponent<typeof import("../node_modules/nuxt/dist/app/components/client-only")['default']>
    'LazyDevOnly': LazyComponent<typeof import("../node_modules/nuxt/dist/app/components/dev-only")['default']>
    'LazyServerPlaceholder': LazyComponent<typeof import("../node_modules/nuxt/dist/app/components/server-placeholder")['default']>
    'LazyNuxtLink': LazyComponent<typeof import("../node_modules/nuxt/dist/app/components/nuxt-link")['default']>
    'LazyNuxtLoadingIndicator': LazyComponent<typeof import("../node_modules/nuxt/dist/app/components/nuxt-loading-indicator")['default']>
    'LazyNuxtTime': LazyComponent<typeof import("../node_modules/nuxt/dist/app/components/nuxt-time.vue")['default']>
    'LazyNuxtRouteAnnouncer': LazyComponent<typeof import("../node_modules/nuxt/dist/app/components/nuxt-route-announcer")['default']>
    'LazyNuxtImg': LazyComponent<typeof import("../node_modules/nuxt/dist/app/components/nuxt-stubs")['NuxtImg']>
    'LazyNuxtPicture': LazyComponent<typeof import("../node_modules/nuxt/dist/app/components/nuxt-stubs")['NuxtPicture']>
    'LazyColorScheme': LazyComponent<typeof import("../node_modules/@nuxtjs/color-mode/dist/runtime/component.vue3.vue")['default']>
    'LazyNuxtPage': LazyComponent<typeof import("../node_modules/nuxt/dist/pages/runtime/page")['default']>
    'LazyNoScript': LazyComponent<typeof import("../node_modules/nuxt/dist/head/runtime/components")['NoScript']>
    'LazyLink': LazyComponent<typeof import("../node_modules/nuxt/dist/head/runtime/components")['Link']>
    'LazyBase': LazyComponent<typeof import("../node_modules/nuxt/dist/head/runtime/components")['Base']>
    'LazyTitle': LazyComponent<typeof import("../node_modules/nuxt/dist/head/runtime/components")['Title']>
    'LazyMeta': LazyComponent<typeof import("../node_modules/nuxt/dist/head/runtime/components")['Meta']>
    'LazyStyle': LazyComponent<typeof import("../node_modules/nuxt/dist/head/runtime/components")['Style']>
    'LazyHead': LazyComponent<typeof import("../node_modules/nuxt/dist/head/runtime/components")['Head']>
    'LazyHtml': LazyComponent<typeof import("../node_modules/nuxt/dist/head/runtime/components")['Html']>
    'LazyBody': LazyComponent<typeof import("../node_modules/nuxt/dist/head/runtime/components")['Body']>
    'LazyNuxtIsland': LazyComponent<typeof import("../node_modules/nuxt/dist/app/components/nuxt-island")['default']>
    'LazyNuxtRouteAnnouncer': LazyComponent<typeof import("../node_modules/nuxt/dist/app/components/server-placeholder")['default']>
}

declare module 'vue' {
  export interface GlobalComponents extends _GlobalComponents { }
}

export const AICopilotChat: typeof import("../components/AICopilotChat.vue")['default']
export const AppNavigation: typeof import("../components/AppNavigation.vue")['default']
export const ContactDetailsModal: typeof import("../components/ContactDetailsModal.vue")['default']
export const ContactModal: typeof import("../components/ContactModal.vue")['default']
export const CustomerDetailsModal: typeof import("../components/CustomerDetailsModal.vue")['default']
export const CustomerModal: typeof import("../components/CustomerModal.vue")['default']
export const ModuleCard: typeof import("../components/ModuleCard.vue")['default']
export const NotificationContainer: typeof import("../components/NotificationContainer.vue")['default']
export const SubscriptionModal: typeof import("../components/SubscriptionModal.vue")['default']
export const CommonThemeSwitcher: typeof import("../components/common/ThemeSwitcher.vue")['default']
export const CommonUserMenu: typeof import("../components/common/UserMenu.vue")['default']
export const CrmContactDetailsModal: typeof import("../components/crm/ContactDetailsModal.vue")['default']
export const CrmContactModal: typeof import("../components/crm/ContactModal.vue")['default']
export const CrmLeadDetailPanel: typeof import("../components/crm/LeadDetailPanel.vue")['default']
export const IconsIconBase: typeof import("../components/icons/IconBase.vue")['default']
export const LayoutHeader: typeof import("../components/layout/Header.vue")['default']
export const LayoutSidebar: typeof import("../components/layout/Sidebar.vue")['default']
export const NuxtWelcome: typeof import("../node_modules/nuxt/dist/app/components/welcome.vue")['default']
export const NuxtLayout: typeof import("../node_modules/nuxt/dist/app/components/nuxt-layout")['default']
export const NuxtErrorBoundary: typeof import("../node_modules/nuxt/dist/app/components/nuxt-error-boundary.vue")['default']
export const ClientOnly: typeof import("../node_modules/nuxt/dist/app/components/client-only")['default']
export const DevOnly: typeof import("../node_modules/nuxt/dist/app/components/dev-only")['default']
export const ServerPlaceholder: typeof import("../node_modules/nuxt/dist/app/components/server-placeholder")['default']
export const NuxtLink: typeof import("../node_modules/nuxt/dist/app/components/nuxt-link")['default']
export const NuxtLoadingIndicator: typeof import("../node_modules/nuxt/dist/app/components/nuxt-loading-indicator")['default']
export const NuxtTime: typeof import("../node_modules/nuxt/dist/app/components/nuxt-time.vue")['default']
export const NuxtRouteAnnouncer: typeof import("../node_modules/nuxt/dist/app/components/nuxt-route-announcer")['default']
export const NuxtImg: typeof import("../node_modules/nuxt/dist/app/components/nuxt-stubs")['NuxtImg']
export const NuxtPicture: typeof import("../node_modules/nuxt/dist/app/components/nuxt-stubs")['NuxtPicture']
export const ColorScheme: typeof import("../node_modules/@nuxtjs/color-mode/dist/runtime/component.vue3.vue")['default']
export const NuxtPage: typeof import("../node_modules/nuxt/dist/pages/runtime/page")['default']
export const NoScript: typeof import("../node_modules/nuxt/dist/head/runtime/components")['NoScript']
export const Link: typeof import("../node_modules/nuxt/dist/head/runtime/components")['Link']
export const Base: typeof import("../node_modules/nuxt/dist/head/runtime/components")['Base']
export const Title: typeof import("../node_modules/nuxt/dist/head/runtime/components")['Title']
export const Meta: typeof import("../node_modules/nuxt/dist/head/runtime/components")['Meta']
export const Style: typeof import("../node_modules/nuxt/dist/head/runtime/components")['Style']
export const Head: typeof import("../node_modules/nuxt/dist/head/runtime/components")['Head']
export const Html: typeof import("../node_modules/nuxt/dist/head/runtime/components")['Html']
export const Body: typeof import("../node_modules/nuxt/dist/head/runtime/components")['Body']
export const NuxtIsland: typeof import("../node_modules/nuxt/dist/app/components/nuxt-island")['default']
export const NuxtRouteAnnouncer: typeof import("../node_modules/nuxt/dist/app/components/server-placeholder")['default']
export const LazyAICopilotChat: LazyComponent<typeof import("../components/AICopilotChat.vue")['default']>
export const LazyAppNavigation: LazyComponent<typeof import("../components/AppNavigation.vue")['default']>
export const LazyContactDetailsModal: LazyComponent<typeof import("../components/ContactDetailsModal.vue")['default']>
export const LazyContactModal: LazyComponent<typeof import("../components/ContactModal.vue")['default']>
export const LazyCustomerDetailsModal: LazyComponent<typeof import("../components/CustomerDetailsModal.vue")['default']>
export const LazyCustomerModal: LazyComponent<typeof import("../components/CustomerModal.vue")['default']>
export const LazyModuleCard: LazyComponent<typeof import("../components/ModuleCard.vue")['default']>
export const LazyNotificationContainer: LazyComponent<typeof import("../components/NotificationContainer.vue")['default']>
export const LazySubscriptionModal: LazyComponent<typeof import("../components/SubscriptionModal.vue")['default']>
export const LazyCommonThemeSwitcher: LazyComponent<typeof import("../components/common/ThemeSwitcher.vue")['default']>
export const LazyCommonUserMenu: LazyComponent<typeof import("../components/common/UserMenu.vue")['default']>
export const LazyCrmContactDetailsModal: LazyComponent<typeof import("../components/crm/ContactDetailsModal.vue")['default']>
export const LazyCrmContactModal: LazyComponent<typeof import("../components/crm/ContactModal.vue")['default']>
export const LazyCrmLeadDetailPanel: LazyComponent<typeof import("../components/crm/LeadDetailPanel.vue")['default']>
export const LazyIconsIconBase: LazyComponent<typeof import("../components/icons/IconBase.vue")['default']>
export const LazyLayoutHeader: LazyComponent<typeof import("../components/layout/Header.vue")['default']>
export const LazyLayoutSidebar: LazyComponent<typeof import("../components/layout/Sidebar.vue")['default']>
export const LazyNuxtWelcome: LazyComponent<typeof import("../node_modules/nuxt/dist/app/components/welcome.vue")['default']>
export const LazyNuxtLayout: LazyComponent<typeof import("../node_modules/nuxt/dist/app/components/nuxt-layout")['default']>
export const LazyNuxtErrorBoundary: LazyComponent<typeof import("../node_modules/nuxt/dist/app/components/nuxt-error-boundary.vue")['default']>
export const LazyClientOnly: LazyComponent<typeof import("../node_modules/nuxt/dist/app/components/client-only")['default']>
export const LazyDevOnly: LazyComponent<typeof import("../node_modules/nuxt/dist/app/components/dev-only")['default']>
export const LazyServerPlaceholder: LazyComponent<typeof import("../node_modules/nuxt/dist/app/components/server-placeholder")['default']>
export const LazyNuxtLink: LazyComponent<typeof import("../node_modules/nuxt/dist/app/components/nuxt-link")['default']>
export const LazyNuxtLoadingIndicator: LazyComponent<typeof import("../node_modules/nuxt/dist/app/components/nuxt-loading-indicator")['default']>
export const LazyNuxtTime: LazyComponent<typeof import("../node_modules/nuxt/dist/app/components/nuxt-time.vue")['default']>
export const LazyNuxtRouteAnnouncer: LazyComponent<typeof import("../node_modules/nuxt/dist/app/components/nuxt-route-announcer")['default']>
export const LazyNuxtImg: LazyComponent<typeof import("../node_modules/nuxt/dist/app/components/nuxt-stubs")['NuxtImg']>
export const LazyNuxtPicture: LazyComponent<typeof import("../node_modules/nuxt/dist/app/components/nuxt-stubs")['NuxtPicture']>
export const LazyColorScheme: LazyComponent<typeof import("../node_modules/@nuxtjs/color-mode/dist/runtime/component.vue3.vue")['default']>
export const LazyNuxtPage: LazyComponent<typeof import("../node_modules/nuxt/dist/pages/runtime/page")['default']>
export const LazyNoScript: LazyComponent<typeof import("../node_modules/nuxt/dist/head/runtime/components")['NoScript']>
export const LazyLink: LazyComponent<typeof import("../node_modules/nuxt/dist/head/runtime/components")['Link']>
export const LazyBase: LazyComponent<typeof import("../node_modules/nuxt/dist/head/runtime/components")['Base']>
export const LazyTitle: LazyComponent<typeof import("../node_modules/nuxt/dist/head/runtime/components")['Title']>
export const LazyMeta: LazyComponent<typeof import("../node_modules/nuxt/dist/head/runtime/components")['Meta']>
export const LazyStyle: LazyComponent<typeof import("../node_modules/nuxt/dist/head/runtime/components")['Style']>
export const LazyHead: LazyComponent<typeof import("../node_modules/nuxt/dist/head/runtime/components")['Head']>
export const LazyHtml: LazyComponent<typeof import("../node_modules/nuxt/dist/head/runtime/components")['Html']>
export const LazyBody: LazyComponent<typeof import("../node_modules/nuxt/dist/head/runtime/components")['Body']>
export const LazyNuxtIsland: LazyComponent<typeof import("../node_modules/nuxt/dist/app/components/nuxt-island")['default']>
export const LazyNuxtRouteAnnouncer: LazyComponent<typeof import("../node_modules/nuxt/dist/app/components/server-placeholder")['default']>

export const componentNames: string[]
