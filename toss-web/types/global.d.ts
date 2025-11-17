import { vi } from 'vitest'

declare global {
  var useHead: typeof vi.fn
  var useRouter: typeof vi.fn
  var useRoute: typeof vi.fn
  var navigateTo: typeof vi.fn
  var useState: typeof vi.fn
  var useRuntimeConfig: typeof vi.fn
  var useApi: typeof vi.fn
  // $fetch is auto-imported by Nuxt
  const $fetch: typeof import('ofetch').$fetch
}

declare module 'h3' {
  import type { IncomingMessage, ServerResponse } from 'http'

  export interface H3Event {
    node: {
      req: IncomingMessage
      res: ServerResponse
    }
    context: Record<string, any>
    path?: string
    method?: string
    headers?: Record<string, string | string[]>
  }

  export type EventHandler<T = unknown> = (event: H3Event) => T | Promise<T>

  export function defineEventHandler<T = unknown>(handler: EventHandler<T>): EventHandler<T>
  export function readBody<T = unknown>(event: H3Event): Promise<T>
  export function getQuery(event: H3Event): Record<string, string | string[]>
}

declare module 'nitro/types' {
  export interface NitroApp {
    hooks: Record<string, any>
  }
}
